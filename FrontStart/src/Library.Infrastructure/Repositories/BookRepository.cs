using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Library.Infrastructure.Data;
using Library.Infrastructure.Exceptions;
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
        catch (Exception ex)
        {
            throw new RepositoryException("Cannot retrieve all books.", ex);
        }
    }

    public async Task<Book?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _context.Books.FindAsync(id);
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Cannot retrieve book with ID {id}.", ex);
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
            throw new RepositoryException("Cannot add new book to the database.", ex);
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Unexpected error occurred while adding a book.", ex);
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
            throw new RepositoryException("Cannot update book due to concurrency conflict.", ex);
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryException("Cannot update book in the database.", ex);
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Unexpected error occurred while updating a book.", ex);
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        try
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
                return;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryException($"Cannot delete book with ID {id}.", ex);
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Unexpected error occurred while deleting a book.", ex);
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
            throw new RepositoryException("Cannot create borrow record.", ex);
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Unexpected error occurred while creating a borrow record.", ex);
        }
    }
}
