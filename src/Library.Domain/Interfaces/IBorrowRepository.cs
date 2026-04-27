using Library.Domain.Entities;

namespace Library.Domain.Interfaces
{
    // Tracking book loan transactions and history
    public interface IBorrowRepository
    {
        Task<IEnumerable<BorrowRecord>> GetAllAsync();
        Task<IEnumerable<BorrowRecord>> GetByMemberIdAsync(Guid memberId);

        Task<BorrowRecord?> GetActiveBorrowAsync(Guid bookId, Guid memberId);

        Task AddAsync(BorrowRecord record);
        Task UpdateAsync(BorrowRecord record);
    }
}