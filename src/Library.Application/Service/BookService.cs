
using Library.Application.Common;
using Library.Application.DTOs;
using Library.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Library.Application.Service;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly ILogger<BookService> _logger;

    public BookService(
        IBookRepository bookRepository,
        IAuthorRepository authorRepository,
        ILogger<BookService> logger)
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
        _logger = logger;
    }

    // GET ALL BOOKS
    public async Task<ServiceResult<List<BookResponse>>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var books = await _bookRepository.GetAllAsync(cancellationToken);

        var response = books.Select(book => new BookResponse
        {
            Id = book.Id,
            Title = book.Title,
            Isbn = book.Isbn,
            PublishedYear = book.PublishedYear,
            AuthorId = book.AuthorId,
            AuthorName = book.Author.Name
        }).ToList();

        _logger.LogInformation(
            "Retrieved {BookCount} books", response.Count);

        return ServiceResult<List<BookResponse>>.Success(response);
    }

    // GET BOOK BY ID
    public async Task<ServiceResult<BookResponse>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetByIdAsync(
            id, cancellationToken);

        if (book is null)
        {
            _logger.LogWarning(
                "Book {BookId} not found", id);

            return ServiceResult<BookResponse>.Failure(
                ServiceError.NotFound,
                "Book not found.");
        }

        var response = new BookResponse
        {
            Id = book.Id,
            Title = book.Title,
            Isbn = book.Isbn,
            PublishedYear = book.PublishedYear,
            AuthorId = book.AuthorId,
            AuthorName = book.Author.Name
        };

        return ServiceResult<BookResponse>.Success(response);
    }

    // CREATE A NEW BOOK
    public async Task<ServiceResult<BookResponse>> CreateAsync(
        CreateBookRequest request,
        CancellationToken cancellationToken)
    {
        // Validate title
        if (string.IsNullOrWhiteSpace(request.Title) ||
            request.Title.Length > 200)
        {
            return ServiceResult<BookResponse>.Failure(
                ServiceError.ValidationError,
                "Book title is required and must not exceed 200 characters.");
        }

        // Validate ISBN
        if (string.IsNullOrEmpty(request.Isbn) ||
            !System.Text.RegularExpressions.Regex.IsMatch(
                request.Isbn, @"^[0-9]{13}$"))
        {
            return ServiceResult<BookResponse>.Failure(
                ServiceError.ValidationError,
                "ISBN must contain exactly 13 digits.");
        }

        // Validate publication year
        if (request.PublishedYear < 1450 ||
            request.PublishedYear > DateTime.UtcNow.Year)
        {
            return ServiceResult<BookResponse>.Failure(
                ServiceError.ValidationError,
                "Published year must be between 1450 and the current year.");
        }

        // Check whether the author exists
        var author = await _authorRepository.GetByIdAsync(
            request.AuthorId, cancellationToken);

        if (author is null)
        {
            return ServiceResult<BookResponse>.Failure(
                ServiceError.NotFound,
                "Author not found.");
        }

        // Check for duplicate ISBN
        var isbnExists = await _bookRepository.IsbnExistsAsync(
            request.Isbn, null, cancellationToken);

        if (isbnExists)
        {
            return ServiceResult<BookResponse>.Failure(
                ServiceError.Conflict,
                "A book with this ISBN already exists.");
        }

        // Create Book entity
        var book = new Library.Domain.Entities.Book
        {
            Title = request.Title.Trim(),
            Isbn = request.Isbn,
            PublishedYear = request.PublishedYear,
            AuthorId = request.AuthorId
        };

        // Save to PostgreSQL
        await _bookRepository.AddAsync(book, cancellationToken);

        _logger.LogInformation(
            "Created book {BookId} with ISBN {Isbn}",
            book.Id, book.Isbn);

        // Convert entity to DTO
        var response = new BookResponse
        {
            Id = book.Id,
            Title = book.Title,
            Isbn = book.Isbn,
            PublishedYear = book.PublishedYear,
            AuthorId = book.AuthorId,
            AuthorName = author.Name
        };

        return ServiceResult<BookResponse>.Success(response);
    }

    // UPDATE AN EXISTING BOOK
    public async Task<ServiceResult<BookResponse>> UpdateAsync(
        int id,
        UpdateBookRequest request,
        CancellationToken cancellationToken)
    {
        // Check if the book exists
        var book = await _bookRepository.GetByIdAsync(
            id, cancellationToken);

        if (book is null)
        {
            _logger.LogWarning(
                "Book {BookId} not found for update", id);

            return ServiceResult<BookResponse>.Failure(
                ServiceError.NotFound,
                "Book not found.");
        }

        // Validate title
        if (string.IsNullOrWhiteSpace(request.Title) ||
            request.Title.Length > 200)
        {
            return ServiceResult<BookResponse>.Failure(
                ServiceError.ValidationError,
                "Book title is required and must not exceed 200 characters.");
        }

        // Validate ISBN
        if (string.IsNullOrEmpty(request.Isbn) ||
            !System.Text.RegularExpressions.Regex.IsMatch(
                request.Isbn, @"^[0-9]{13}$"))
        {
            return ServiceResult<BookResponse>.Failure(
                ServiceError.ValidationError,
                "ISBN must contain exactly 13 digits.");
        }

        // Validate publication year
        if (request.PublishedYear < 1450 ||
            request.PublishedYear > DateTime.UtcNow.Year)
        {
            return ServiceResult<BookResponse>.Failure(
                ServiceError.ValidationError,
                "Published year must be between 1450 and the current year.");
        }

        // Check whether the author exists
        var author = await _authorRepository.GetByIdAsync(
            request.AuthorId, cancellationToken);

        if (author is null)
        {
            return ServiceResult<BookResponse>.Failure(
                ServiceError.NotFound,
                "Author not found.");
        }

        // Check ISBN uniqueness, excluding this book
        var isbnExists = await _bookRepository.IsbnExistsAsync(
            request.Isbn, id, cancellationToken);

        if (isbnExists)
        {
            return ServiceResult<BookResponse>.Failure(
                ServiceError.Conflict,
                "A book with this ISBN already exists.");
        }

        // Update the existing book
        book.Title = request.Title.Trim();
        book.Isbn = request.Isbn;
        book.PublishedYear = request.PublishedYear;
        book.AuthorId = request.AuthorId;

        // Save changes to PostgreSQL
        await _bookRepository.UpdateAsync(
            book, cancellationToken);

        _logger.LogInformation(
            "Updated book {BookId}", book.Id);

        // Convert entity to DTO
        var response = new BookResponse
        {
            Id = book.Id,
            Title = book.Title,
            Isbn = book.Isbn,
            PublishedYear = book.PublishedYear,
            AuthorId = book.AuthorId,
            AuthorName = author.Name
        };

        return ServiceResult<BookResponse>.Success(response);
    }

    // DELETE A BOOK
    public async Task<ServiceResult<bool>> DeleteAsync(
        int id,
        CancellationToken cancellationToken)
    {
        // Find the book
        var book = await _bookRepository.GetByIdAsync(
            id, cancellationToken);

        // Check if the book exists
        if (book is null)
        {
            _logger.LogWarning(
                "Book {BookId} not found for deletion", id);

            return ServiceResult<bool>.Failure(
                ServiceError.NotFound,
                "Book not found.");
        }

        // Delete from PostgreSQL
        await _bookRepository.DeleteAsync(
            book, cancellationToken);

        _logger.LogInformation(
            "Deleted book {BookId}", id);

        return ServiceResult<bool>.Success(true);
    }

    // GET BOOKS BY AUTHOR ID
    public async Task<ServiceResult<List<BookResponse>>> GetByAuthorIdAsync(
        int authorId,
        CancellationToken cancellationToken)
    {
        // Check whether the author exists
        var authorExists = await _authorRepository.ExistsAsync(
            authorId, cancellationToken);

        if (!authorExists)
        {
            _logger.LogWarning(
                "Author {AuthorId} not found", authorId);

            return ServiceResult<List<BookResponse>>.Failure(
                ServiceError.NotFound,
                "Author not found.");
        }

        // Retrieve books belonging to the author
        var books = await _bookRepository.GetByAuthorIdAsync(
            authorId, cancellationToken);

        // Convert entities to response DTOs
        var response = books.Select(book => new BookResponse
        {
            Id = book.Id,
            Title = book.Title,
            Isbn = book.Isbn,
            PublishedYear = book.PublishedYear,
            AuthorId = book.AuthorId,
            AuthorName = book.Author.Name
        }).ToList();

        _logger.LogInformation(
            "Retrieved {BookCount} books for author {AuthorId}",
            response.Count, authorId);

        return ServiceResult<List<BookResponse>>.Success(response);
    }


}
