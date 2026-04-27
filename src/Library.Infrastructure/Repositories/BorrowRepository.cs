using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Library.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories;

public class BorrowRepository : IBorrowRepository
{
    private readonly AppDbContext _context;

    public BorrowRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BorrowRecord>> GetAllAsync()
    {
        try
        {
            return await _context.BorrowRecords
                .Include(r => r.Book)
                .Include(r => r.Member)
                .ToListAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryException("Database error while retrieving all borrow records.", ex);
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Unexpected error while retrieving all borrow records.", ex);
        }
    }

    public async Task<IEnumerable<BorrowRecord>> GetByMemberIdAsync(Guid memberId)
    {
        try
        {
            return await _context.BorrowRecords
                .Include(r => r.Book)
                .Include(r => r.Member)
                .Where(r => r.MemberId == memberId)
                .ToListAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryException($"Database error while retrieving borrow history for member {memberId}.", ex);
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Unexpected error while retrieving borrow history for member {memberId}.", ex);
        }
    }

    public async Task<BorrowRecord?> GetActiveBorrowAsync(Guid bookId, Guid memberId)
    {
        try
        {
            return await _context.BorrowRecords
                .FirstOrDefaultAsync(r =>
                    r.BookId == bookId &&
                    r.MemberId == memberId &&
                    r.Status == "Borrowed");
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryException(
                $"Database error while retrieving active borrow record for book {bookId} and member {memberId}.", ex);
        }
        catch (Exception ex)
        {
            throw new RepositoryException(
                $"Unexpected error while retrieving active borrow record for book {bookId} and member {memberId}.", ex);
        }
    }

    public async Task AddAsync(BorrowRecord record)
    {
        try
        {
            await _context.BorrowRecords.AddAsync(record);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryException("Database error while adding a borrow record.", ex);
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Unexpected error while adding a borrow record.", ex);
        }
    }

    public async Task UpdateAsync(BorrowRecord record)
    {
        try
        {
            _context.BorrowRecords.Update(record);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new RepositoryException("Concurrency error while updating a borrow record.", ex);
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryException("Database error while updating a borrow record.", ex);
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Unexpected error while updating a borrow record.", ex);
        }
    }
}
