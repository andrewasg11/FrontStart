using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Library.Infrastructure.Data;
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
        catch (DbUpdateException ex)
        {
            throw new RepositoryException("Database error while retrieving all members.", ex);
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Unexpected error while retrieving all members.", ex);
        }
    }

    public async Task<Member?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _context.Members.FindAsync(id);
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryException($"Database error while retrieving member with ID {id}.", ex);
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Unexpected error while retrieving member with ID {id}.", ex);
        }
    }

    public async Task<Member?> GetByEmailAsync(string email)
    {
        try
        {
            return await _context.Members.FirstOrDefaultAsync(m => m.Email == email);
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryException($"Database error while retrieving member by email '{email}'.", ex);
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Unexpected error while retrieving member by email '{email}'.", ex);
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
            throw new RepositoryException("Database error while adding a new member.", ex);
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Unexpected error while adding a new member.", ex);
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
            throw new RepositoryException("Concurrency error while updating a member.", ex);
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryException("Database error while updating a member.", ex);
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Unexpected error while updating a member.", ex);
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        try
        {
            var member = await _context.Members.FindAsync(id);

            if (member != null)
            {
                _context.Members.Remove(member);
                await _context.SaveChangesAsync();
            }
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryException($"Database error while deleting member with ID {id}.", ex);
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Unexpected error while deleting member with ID {id}.", ex);
        }
    }
}
