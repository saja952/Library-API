
using Library.Application.Common;
using Library.Application.DTOs;
using Library.Application.Interfaces;
using Library.Application.Service;
using Library.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Xunit;

namespace Library.Application.Tests;

public class BookServiceTests
{
    private readonly IBookRepository _bookRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly BookService _bookService;

    public BookServiceTests()
    {
        _bookRepository = Substitute.For<IBookRepository>();
        _authorRepository = Substitute.For<IAuthorRepository>();

        _bookService = new BookService(
            _bookRepository,
            _authorRepository,
            NullLogger<BookService>.Instance);
    }

    private static Book CreateTestBook()
    {
        return new Book
        {
            Id = 1,
            Title = "Clean Code",
            Isbn = "9780132350884",
            PublishedYear = 2008,
            AuthorId = 1,
            Author = new Author
            {
                Id = 1,
                Name = "Robert C. Martin"
            }
        };
    }

    // TEST 1: Get existing book
    [Fact]
    public async Task GetById_WhenBookExists_ReturnsSuccess()
    {
        var book = CreateTestBook();

        _bookRepository
            .GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Book?>(book));

        var result = await _bookService.GetByIdAsync(
            1, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("Clean Code", result.Data.Title);
        Assert.Equal("Robert C. Martin", result.Data.AuthorName);
    }

    // TEST 2: Get nonexistent book
    [Fact]
    public async Task GetById_WhenBookDoesNotExist_ReturnsNotFound()
    {
        _bookRepository
            .GetByIdAsync(999, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Book?>(null));

        var result = await _bookService.GetByIdAsync(
            999, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceError.NotFound, result.Error);
    }

    // TEST 3: Create book with invalid title
    [Fact]
    public async Task Create_WhenTitleIsEmpty_ReturnsValidationError()
    {
        var request = new CreateBookRequest
        {
            Title = "",
            Isbn = "9780132350884",
            PublishedYear = 2008,
            AuthorId = 1
        };

        var result = await _bookService.CreateAsync(
            request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceError.ValidationError, result.Error);
    }

    // TEST 4: Create book with invalid ISBN
    [Fact]
    public async Task Create_WhenIsbnIsInvalid_ReturnsValidationError()
    {
        var request = new CreateBookRequest
        {
            Title = "Clean Code",
            Isbn = "12345",
            PublishedYear = 2008,
            AuthorId = 1
        };

        var result = await _bookService.CreateAsync(
            request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceError.ValidationError, result.Error);
    }

    // TEST 5: Create book with future publication year
    [Fact]
    public async Task Create_WhenYearIsInFuture_ReturnsValidationError()
    {
        var request = new CreateBookRequest
        {
            Title = "Future Book",
            Isbn = "9780132350884",
            PublishedYear = DateTime.UtcNow.Year + 1,
            AuthorId = 1
        };

        var result = await _bookService.CreateAsync(
            request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceError.ValidationError, result.Error);
    }

    // TEST 6: Create book with nonexistent author
    [Fact]
    public async Task Create_WhenAuthorDoesNotExist_ReturnsNotFound()
    {
        var request = new CreateBookRequest
        {
            Title = "Clean Code",
            Isbn = "9780132350884",
            PublishedYear = 2008,
            AuthorId = 999
        };

        _authorRepository
            .GetByIdAsync(999, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Author?>(null));

        var result = await _bookService.CreateAsync(
            request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceError.NotFound, result.Error);
    }

    // TEST 7: Create book with duplicate ISBN
    [Fact]
    public async Task Create_WhenIsbnExists_ReturnsConflict()
    {
        var request = new CreateBookRequest
        {
            Title = "Clean Code",
            Isbn = "9780132350884",
            PublishedYear = 2008,
            AuthorId = 1
        };

        var author = new Author
        {
            Id = 1,
            Name = "Robert C. Martin"
        };

        _authorRepository
            .GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Author?>(author));

        _bookRepository
            .IsbnExistsAsync(
                request.Isbn,
                null,
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));

        var result = await _bookService.CreateAsync(
            request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceError.Conflict, result.Error);
    }

    // TEST 8: Update nonexistent book
    [Fact]
    public async Task Update_WhenBookDoesNotExist_ReturnsNotFound()
    {
        var request = new UpdateBookRequest
        {
            Title = "Updated Book",
            Isbn = "9780132350884",
            PublishedYear = 2008,
            AuthorId = 1
        };

        _bookRepository
            .GetByIdAsync(999, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Book?>(null));

        var result = await _bookService.UpdateAsync(
            999, request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceError.NotFound, result.Error);
    }

    // TEST 9: Delete nonexistent book
    [Fact]
    public async Task Delete_WhenBookDoesNotExist_ReturnsNotFound()
    {
        _bookRepository
            .GetByIdAsync(999, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Book?>(null));

        var result = await _bookService.DeleteAsync(
            999, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceError.NotFound, result.Error);
    }
}
