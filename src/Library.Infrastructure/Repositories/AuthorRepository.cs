
using Library.Application.Interfaces;
using Library.Domain.Entities;
using Library.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private readonly LibraryDbContext _dbContext;

    public AuthorRepository(LibraryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Author>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _dbContext.Authors
            .ToListAsync(cancellationToken);
    }

    public async Task<Author?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Authors
            .FirstOrDefaultAsync(
                a => a.Id == id,
                cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Authors
            .AnyAsync(
                a => a.Id == id,
                cancellationToken);
    }

    public async Task AddAsync(
        Author author,
        CancellationToken cancellationToken)
    {
        await _dbContext.Authors.AddAsync(
            author,
            cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
