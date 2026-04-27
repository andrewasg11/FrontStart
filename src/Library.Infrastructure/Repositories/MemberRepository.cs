using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Library.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories;

// EF Core implementation for managing member repository, data persistence, and lookups for member profiles
public class MemberRepository : IMemberRepository
{
    private readonly AppDbContext _context;

    public MemberRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Member>> GetAllAsync() =>
        await _context.Members.ToListAsync();

    public async Task<Member?> GetByIdAsync(Guid id) =>
        await _context.Members.FindAsync(id);

    // Email uniqueness checks during registration and updates
    public async Task<Member?> GetByEmailAsync(string email) =>
        await _context.Members.FirstOrDefaultAsync(m => m.Email == email);

    public async Task AddAsync(Member member)
    {
        await _context.Members.AddAsync(member);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Member member)
    {
        _context.Members.Update(member);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var member = await _context.Members.FindAsync(id);
        if (member != null)
        {
            _context.Members.Remove(member);
            await _context.SaveChangesAsync();
        }
    }
}