
using Library.Domain.Entities;

namespace Library.Application.Interfaces;

public interface IBookRepository
{
    Task<List<Book>> GetAllAsync(CancellationToken cancellationToken);

    Task<List<Book>> GetByAuthorIdAsync(
        int authorId,
        CancellationToken cancellationToken);

    Task<Book?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<bool> IsbnExistsAsync(
        string isbn,
        int? excludeBookId,
        CancellationToken cancellationToken);

    Task AddAsync(Book book, CancellationToken cancellationToken);

    Task UpdateAsync(Book book, CancellationToken cancellationToken);

    Task DeleteAsync(Book book, CancellationToken cancellationToken);
}
