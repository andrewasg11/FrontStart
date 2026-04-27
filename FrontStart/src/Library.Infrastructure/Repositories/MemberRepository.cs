using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Library.Infrastructure.Data;
using Library.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories;

public class MemberRepository : IMemberRepository
{
    private readonly AppDbContext _context;

    public MemberRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Member>> GetAllAsync()
    {
        try
        {
            return await _context.Members.ToListAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Failed to retrieve all members.", ex);
        }
    }

    public async Task<Member?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _context.Members.FindAsync(id);
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Failed to retrieve member with ID {id}.", ex);
        }
    }

    public async Task<Member?> GetByEmailAsync(string email)
    {
        try
        {
            return await _context.Members.FirstOrDefaultAsync(m => m.Email == email);
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Failed to retrieve member with email {email}.", ex);
        }
    }

    public async Task AddAsync(Member member)
    {
        try
        {
            await _context.Members.AddAsync(member);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryException("Failed to add new member.", ex);
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Unexpected error occurred while adding a member.", ex);
        }
    }

    public async Task UpdateAsync(Member member)
    {
        try
        {
            _context.Members.Update(member);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new RepositoryException("Failed to update member due to concurrency conflict.", ex);
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryException("Failed to update member.", ex);
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Unexpected error occurred while updating a member.", ex);
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        try
        {
            var member = await _context.Members.FindAsync(id);

            if (member == null)
                return;

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryException($"Failed to delete member with ID {id}.", ex);
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Unexpected error occurred while deleting a member.", ex);
        }
    }
}
