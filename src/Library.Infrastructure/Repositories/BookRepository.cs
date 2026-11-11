
using Library.Application.Interfaces;
using Library.Domain.Entities;
using Library.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories;

public class BookRepository : IBookRepository
{
    private readonly LibraryDbContext _dbContext;

    public BookRepository(LibraryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Book>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _dbContext.Books
            .Include(b => b.Author)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Book>> GetByAuthorIdAsync(
        int authorId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Books
            .Where(b => b.AuthorId == authorId)
            .Include(b => b.Author)
            .ToListAsync(cancellationToken);
    }

    public async Task<Book?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Books
            .Include(b => b.Author)
            .FirstOrDefaultAsync(
                b => b.Id == id,
                cancellationToken);
    }

    public async Task<bool> IsbnExistsAsync(
        string isbn,
        int? excludeBookId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Books.AnyAsync(
            b => b.Isbn == isbn &&
                 (!excludeBookId.HasValue ||
                  b.Id != excludeBookId.Value),
            cancellationToken);
    }

    public async Task AddAsync(
        Book book,
        CancellationToken cancellationToken)
    {
        await _dbContext.Books.AddAsync(book, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        Book book,
        CancellationToken cancellationToken)
    {
        _dbContext.Books.Update(book);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        Book book,
        CancellationToken cancellationToken)
    {
        _dbContext.Books.Remove(book);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
