
using Library.Application.Common;
using Library.Application.DTOs;

namespace Library.Application.Interfaces;

public interface IAuthorService
{
    Task<ServiceResult<List<AuthorResponse>>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<ServiceResult<AuthorResponse>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<ServiceResult<AuthorResponse>> CreateAsync(
        CreateAuthorRequest request,
        CancellationToken cancellationToken);
}
