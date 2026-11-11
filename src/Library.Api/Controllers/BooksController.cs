
using Library.Application.Common;
using Library.Application.DTOs;
using Library.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/books")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    // GET /api/books
    [HttpGet]
    public async Task<ActionResult<List<BookResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _bookService.GetAllAsync(
            cancellationToken);

        return Ok(result.Data);
    }

    // GET /api/books/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _bookService.GetByIdAsync(
            id, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(new
            {
                message = result.ErrorMessage
            });
        }

        return Ok(result.Data);
    }

    // POST /api/books
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateBookRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _bookService.CreateAsync(
            request, cancellationToken);

        if (!result.IsSuccess)
        {
            return HandleError(result.Error, result.ErrorMessage);
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Data!.Id },
            result.Data);
    }

    // PUT /api/books/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateBookRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _bookService.UpdateAsync(
            id, request, cancellationToken);

        if (!result.IsSuccess)
        {
            return HandleError(result.Error, result.ErrorMessage);
        }

        return Ok(result.Data);
    }

    // DELETE /api/books/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _bookService.DeleteAsync(
            id, cancellationToken);

        if (!result.IsSuccess)
        {
            return HandleError(result.Error, result.ErrorMessage);
        }

        return NoContent();
    }

    // Convert Service errors to HTTP responses
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
