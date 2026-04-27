using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Library.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories;

// EF Core implementation for managing transaction data, active loans, and member history
public class BorrowRepository : IBorrowRepository
{
    private readonly AppDbContext _context;

    public BorrowRepository(AppDbContext context)
    {
        _context = context;
    }

    // Retrieves all records, including related Book and Member data for mapping
    public async Task<IEnumerable<BorrowRecord>> GetAllAsync() =>
        await _context.BorrowRecords
            .Include(r => r.Book)
            .Include(r => r.Member)
            .ToListAsync();

    // Retrieves borrowing history for a specific member, including related entity data
    public async Task<IEnumerable<BorrowRecord>> GetByMemberIdAsync(Guid memberId) =>
        await _context.BorrowRecords
            .Include(r => r.Book)
            .Include(r => r.Member)
            .Where(r => r.MemberId == memberId)
            .ToListAsync();

    // Locates an active "Borrowed" record to process a book return
    public async Task<BorrowRecord?> GetActiveBorrowAsync(Guid bookId, Guid memberId) =>
        await _context.BorrowRecords
            .FirstOrDefaultAsync(r =>
                r.BookId == memberId &&
                r.MemberId == memberId &&
                r.Status == "Borrowed");

    public async Task AddAsync(BorrowRecord record)
    {
        await _context.BorrowRecords.AddAsync(record);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(BorrowRecord record)
    {
        _context.BorrowRecords.Update(record);
        await _context.SaveChangesAsync();
    }
}