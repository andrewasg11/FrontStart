using Library.Domain.Entities;

namespace Library.Domain.Interfaces
{
    // Manage library member records and lookups
    public interface IMemberRepository
    {
        Task<IEnumerable<Member>> GetAllAsync();
        Task<Member?> GetByIdAsync(Guid id);
        Task<Member?> GetByEmailAsync(string email);
        Task AddAsync(Member member);
        Task UpdateAsync(Member member);
        Task DeleteAsync(Guid id);
    }
}