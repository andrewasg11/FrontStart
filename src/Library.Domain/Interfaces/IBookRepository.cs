using Library.Domain.Entities;

namespace Library.Domain.Interfaces;

// Book entities and their borrow records
public interface IBookRepository
{
    Task<IEnumerable<Book>> GetAllAsync();

    Task<Book?> GetByIdAsync(Guid id);

    Task AddAsync(Book book);
    Task UpdateAsync(Book book);
    Task DeleteAsync(Guid id);
    Task CreateBorrowRecordAsync(BorrowRecord record);
}