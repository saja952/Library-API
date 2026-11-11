
using Library.Application.Common;
using Library.Application.DTOs;
using Library.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/authors")]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorService _authorService;
    private readonly IBookService _bookService;

    public AuthorsController(
        IAuthorService authorService,
        IBookService bookService)
    {
        _authorService = authorService;
        _bookService = bookService;
    }

    // GET /api/authors
    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _authorService.GetAllAsync(
            cancellationToken);

        return Ok(result.Data);
    }

    // GET /api/authors/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _authorService.GetByIdAsync(
            id, cancellationToken);

        if (!result.IsSuccess)
        {
            return HandleError(
                result.Error,
                result.ErrorMessage);
        }

        return Ok(result.Data);
    }

    // POST /api/authors
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateAuthorRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authorService.CreateAsync(
            request, cancellationToken);

        if (!result.IsSuccess)
        {
            return HandleError(
                result.Error,
                result.ErrorMessage);
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Data!.Id },
            result.Data);
    }

    // GET /api/authors/{id}/books
    [HttpGet("{id:int}/books")]
    public async Task<IActionResult> GetBooksByAuthor(
        int id,
        CancellationToken cancellationToken)
    {
        var authorResult = await _authorService.GetByIdAsync(
            id, cancellationToken);

        if (!authorResult.IsSuccess)
        {
            return HandleError(
                authorResult.Error,
                authorResult.ErrorMessage);
        }

        var booksResult = await _bookService.GetByAuthorIdAsync(
            id, cancellationToken);

        if (!booksResult.IsSuccess)
        {
            return HandleError(
                booksResult.Error,
                booksResult.ErrorMessage);
        }

        return Ok(booksResult.Data);
    }

    // Convert service errors to HTTP responses
    private IActionResult HandleError(
        ServiceError error,
        string? message)
    {
        return error switch
        {
            ServiceError.NotFound => NotFound(new
            {
                message
            }),

            ServiceError.Conflict => Conflict(new
            {
                message
            }),

            ServiceError.ValidationError => BadRequest(new
            {
                message
            }),

            _ => StatusCode(500, new
            {
                message = "An unexpected error occurred."
            })
        };
    }
}
