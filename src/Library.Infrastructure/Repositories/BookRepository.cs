using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Library.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories;

public class BookRepository : IBookRepository
{
    private readonly AppDbContext _context;

    public BookRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Book>> GetAllAsync()
    {
        try
        {
            return await _context.Books.ToListAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryException("Database error while retrieving all books.", ex);
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Unexpected error while retrieving all books.", ex);
        }
    }

    public async Task<Book?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _context.Books.FindAsync(id);
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryException($"Database error while retrieving book with ID {id}.", ex);
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Unexpected error while retrieving book with ID {id}.", ex);
        }
    }

    public async Task AddAsync(Book book)
    {
        try
        {
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryException("Database error while adding a new book.", ex);
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Unexpected error while adding a new book.", ex);
        }
    }

    public async Task UpdateAsync(Book book)
    {
        try
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new RepositoryException("Concurrency error while updating a book.", ex);
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryException("Database error while updating a book.", ex);
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Unexpected error while updating a book.", ex);
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        try
        {
            var book = await _context.Books.FindAsync(id);

            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryException($"Database error while deleting book with ID {id}.", ex);
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Unexpected error while deleting book with ID {id}.", ex);
        }
    }

    public async Task CreateBorrowRecordAsync(BorrowRecord record)
    {
        try
        {
            await _context.BorrowRecords.AddAsync(record);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryException("Database error while creating a borrow record.", ex);
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Unexpected error while creating a borrow record.", ex);
        }
    }
}
