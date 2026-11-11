
using Library.Application.Common;
using Library.Application.DTOs;

namespace Library.Application.Interfaces;

public interface IBookService
{
    Task<ServiceResult<List<BookResponse>>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<ServiceResult<BookResponse>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<ServiceResult<BookResponse>> CreateAsync(
        CreateBookRequest request,
        CancellationToken cancellationToken);

    Task<ServiceResult<BookResponse>> UpdateAsync(
        int id,
        UpdateBookRequest request,
        CancellationToken cancellationToken);

    Task<ServiceResult<bool>> DeleteAsync(
        int id,
        CancellationToken cancellationToken);

    Task<ServiceResult<List<BookResponse>>> GetByAuthorIdAsync(
        int authorId,
        CancellationToken cancellationToken);
}
