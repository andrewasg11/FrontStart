using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Library.Application.DTOs;
using Library.Application.Exceptions;
using Library.Application.Interfaces;
using Library.Domain.Entities;
using Library.Domain.Interfaces;

namespace Library.Application.Services;

// Book management logic (stock, memory caching, concurrency-safe borrowing)
public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IBorrowRepository _borrowRepository;
    private readonly IMemoryCache _cache;

    private const string AllBooksCacheKey = "all_books";

    public BookService(
        IBookRepository bookRepository,
        IBorrowRepository borrowRepository,
        IMemoryCache cache)
    {
        _bookRepository = bookRepository;
        _borrowRepository = borrowRepository;
        _cache = cache;
    }


    // Returns a cached list of books if available, otherwise fetches for database for 10 minutes
    public async Task<IEnumerable<BookResponseDto>> GetAllBooksAsync()
    {
        return await _cache.GetOrCreateAsync(AllBooksCacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
            var books = await _bookRepository.GetAllAsync();
            return books.Select(MapToDto);
        }) ?? Enumerable.Empty<BookResponseDto>();
    }

    public async Task<BookResponseDto> GetBookByIdAsync(Guid id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null)
            throw new NotFoundException($"Book with id '{id}' was not found.");
        return MapToDto(book);
    }

    public async Task<BookResponseDto> CreateBookAsync(CreateBookDto dto)
    {
        if (dto.AvailableCopies > dto.TotalCopies)
            throw new BadRequestException("AvailableCopies cannot exceed TotalCopies.");

        var book = new Book
        {
            Title = dto.Title,
            Author = dto.Author,
            ISBN = dto.ISBN,
            TotalCopies = dto.TotalCopies,
            AvailableCopies = dto.AvailableCopies
        };

        await _bookRepository.AddAsync(book);

        _cache.Remove(AllBooksCacheKey);

        return MapToDto(book);
    }

    public async Task<BookResponseDto> UpdateBookAsync(Guid id, CreateBookDto dto)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null)
            throw new NotFoundException($"Book with id '{id}' was not found.");

        if (dto.AvailableCopies > dto.TotalCopies)
            throw new BadRequestException("AvailableCopies cannot exceed TotalCopies.");

        book.Title = dto.Title;
        book.Author = dto.Author;
        book.ISBN = dto.ISBN;
        book.TotalCopies = dto.TotalCopies;
        book.AvailableCopies = dto.AvailableCopies;

        await _bookRepository.UpdateAsync(book);
        _cache.Remove(AllBooksCacheKey);

        return MapToDto(book);
    }

    public async Task DeleteBookAsync(Guid id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null)
            throw new NotFoundException($"Book with id '{id}' was not found.");

        await _bookRepository.DeleteAsync(id);
        _cache.Remove(AllBooksCacheKey);
    }


    // Decrements stock and creates a borrow record
    // Uses RowVersion to handle simultaneous borrow requests via ConflictException
    public async Task BorrowBookAsync(BorrowRequestDto request)
    {
        var book = await _bookRepository.GetByIdAsync(request.BookId);
        if (book == null)
            throw new NotFoundException($"Book with id '{request.BookId}' was not found.");

        if (book.AvailableCopies <= 0)
            throw new BadRequestException("No available copies. This book cannot be borrowed right now.");

        book.AvailableCopies--;

        var record = new BorrowRecord
        {
            BookId = request.BookId,
            MemberId = request.MemberId,
            BorrowDate = DateTime.UtcNow,
            Status = "Borrowed"
        };

        try
        {
            await _bookRepository.CreateBorrowRecordAsync(record);
            await _bookRepository.UpdateAsync(book);
            _cache.Remove(AllBooksCacheKey);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConflictException("This book was just borrowed by someone else. Please try again.");
        }
    }


    // Finds the active borrow record, marks it Returned, and increments AvailableCopies
    public async Task ReturnBookAsync(ReturnRequestDto request)
    {
        var book = await _bookRepository.GetByIdAsync(request.BookId);
        if (book == null)
            throw new NotFoundException($"Book with id '{request.BookId}' was not found.");

        var record = await _borrowRepository.GetActiveBorrowAsync(request.BookId, request.MemberId);
        if (record == null)
            throw new BadRequestException("No active borrow record found for this book and member.");

        record.ReturnDate = DateTime.UtcNow;
        record.Status = "Returned";
        book.AvailableCopies++;

        await _borrowRepository.UpdateAsync(record);
        await _bookRepository.UpdateAsync(book);
        _cache.Remove(AllBooksCacheKey);
    }

    private static BookResponseDto MapToDto(Book b) {
        return new BookResponseDto
        {
            Id = b.Id,
            Title = b.Title,
            Author = b.Author,
            ISBN = b.ISBN,
            TotalCopies = b.TotalCopies,
            AvailableCopies = b.AvailableCopies,
            IsBorrowed = b.IsBorrowed
        };
    }
}