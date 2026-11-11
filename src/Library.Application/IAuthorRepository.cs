
using Library.Domain.Entities;

namespace Library.Application.Interfaces;

public interface IAuthorRepository
{
    Task<List<Author>> GetAllAsync(CancellationToken cancellationToken);

    Task<Author?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<bool> ExistsAsync(
        int id,
        CancellationToken cancellationToken);

    Task AddAsync(
        Author author,
        CancellationToken cancellationToken);
}
