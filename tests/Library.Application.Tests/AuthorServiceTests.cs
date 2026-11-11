
using Library.Application.Common;
using Library.Application.DTOs;
using Library.Application.Interfaces;
using Library.Application.Service;
using Library.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Xunit;

namespace Library.Application.Tests;

public class AuthorServiceTests
{
    private readonly IAuthorRepository _authorRepository;
    private readonly AuthorService _authorService;

    public AuthorServiceTests()
    {
        _authorRepository = Substitute.For<IAuthorRepository>();

        _authorService = new AuthorService(
            _authorRepository,
            NullLogger<AuthorService>.Instance);
    }

    // TEST 1: Get existing author
    [Fact]
    public async Task GetById_WhenAuthorExists_ReturnsSuccess()
    {
        // Arrange
        var author = new Author
        {
            Id = 1,
            Name = "Robert C. Martin"
        };

        _authorRepository
            .GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Author?>(author));

        // Act
        var result = await _authorService.GetByIdAsync(
            1, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data.Id);
        Assert.Equal("Robert C. Martin", result.Data.Name);
    }

    // TEST 2: Get nonexistent author
    [Fact]
    public async Task GetById_WhenAuthorDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        _authorRepository
            .GetByIdAsync(999, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Author?>(null));

        // Act
        var result = await _authorService.GetByIdAsync(
            999, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceError.NotFound, result.Error);
    }

    // TEST 3: Create author with empty name
    [Fact]
    public async Task Create_WhenNameIsEmpty_ReturnsValidationError()
    {
        // Arrange
        var request = new CreateAuthorRequest
        {
            Name = ""
        };

        // Act
        var result = await _authorService.CreateAsync(
            request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceError.ValidationError, result.Error);
    }

    // TEST 4: Create author successfully
    [Fact]
    public async Task Create_WhenNameIsValid_ReturnsSuccess()
    {
        // Arrange
        var request = new CreateAuthorRequest
        {
            Name = "Robert C. Martin"
        };

        // Act
        var result = await _authorService.CreateAsync(
            request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("Robert C. Martin", result.Data.Name);

        await _authorRepository.Received(1)
            .AddAsync(
                Arg.Any<Author>(),
                Arg.Any<CancellationToken>());
    }
}
