using Library.Application.Common;
using Library.Application.DTOs;
using Library.Application.Interfaces;
using Library.Domain.Entities;
using Microsoft.Extensions.Logging;
namespace Library.Application.Service;

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _authorRepository;
    private readonly ILogger<AuthorService> _logger;

    public AuthorService(
        IAuthorRepository authorRepository,
        ILogger<AuthorService> logger)
    {
        _authorRepository = authorRepository;
        _logger = logger;
    }

    public async Task<ServiceResult<List<AuthorResponse>>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var authors = await _authorRepository.GetAllAsync(cancellationToken);

        var response = authors.Select(a => new AuthorResponse
        {
            Id = a.Id,
            Name = a.Name
        }).ToList();

        _logger.LogInformation(
            "Retrieved {AuthorCount} authors", response.Count);

        return ServiceResult<List<AuthorResponse>>.Success(response);
    }

    public async Task<ServiceResult<AuthorResponse>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var author = await _authorRepository.GetByIdAsync(
            id, cancellationToken);

        if (author is null)
        {
            _logger.LogWarning("Author {AuthorId} not found", id);

            return ServiceResult<AuthorResponse>.Failure(
                ServiceError.NotFound,
                "Author not found.");
        }

        var response = new AuthorResponse
        {
            Id = author.Id,
            Name = author.Name
        };

        return ServiceResult<AuthorResponse>.Success(response);
    }

    public async Task<ServiceResult<AuthorResponse>> CreateAsync(
        CreateAuthorRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return ServiceResult<AuthorResponse>.Failure(
                ServiceError.ValidationError,
                "Author name is required.");
        }

        var author = new Author
        {
            Name = request.Name.Trim()
        };

        await _authorRepository.AddAsync(author, cancellationToken);

        _logger.LogInformation(
            "Created author {AuthorId}", author.Id);

        var response = new AuthorResponse
        {
            Id = author.Id,
            Name = author.Name
        };

        return ServiceResult<AuthorResponse>.Success(response);
    }
}
