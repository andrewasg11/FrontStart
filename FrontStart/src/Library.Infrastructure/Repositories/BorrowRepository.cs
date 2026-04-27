using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Library.Infrastructure.Data;
using Library.Infrastructure.Exceptions;
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
        catch (Exception ex)
        {
            throw new RepositoryException("Failed to retrieve all borrow records.", ex);
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
        catch (Exception ex)
        {
            throw new RepositoryException($"Failed to retrieve borrow history for member {memberId}.", ex);
        }
    }

    public async Task<BorrowRecord?> GetActiveBorrowAsync(Guid bookId, Guid memberId)
    {
        try
        {
            return await _context.BorrowRecords
                .FirstOrDefaultAsync(r =>
                    r.BookId == bookId &&      // FIXED: was incorrectly comparing to memberId
                    r.MemberId == memberId &&
                    r.Status == "Borrowed");
        }
        catch (Exception ex)
        {
            throw new RepositoryException(
                $"Failed to retrieve active borrow record for Book {bookId} and Member {memberId}.",
                ex
            );
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
            throw new RepositoryException("Failed to add new borrow record.", ex);
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Unexpected error occurred while adding a borrow record.", ex);
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
            throw new RepositoryException("Failed to update borrow record due to concurrency conflict.", ex);
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryException("Failed to update borrow record.", ex);
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Unexpected error occurred while updating a borrow record.", ex);
        }
    }
}
