using Library.Application.DTOs;

namespace Library.Application.Interfaces;

public interface IBookService
{
    Task<IEnumerable<BookResponseDto>> GetAllBooksAsync();
    Task<BookResponseDto> GetBookByIdAsync(Guid id);
    Task<BookResponseDto> CreateBookAsync(CreateBookDto dto);
    Task<BookResponseDto> UpdateBookAsync(Guid id, CreateBookDto dto);
    Task DeleteBookAsync(Guid id);
    Task BorrowBookAsync(BorrowRequestDto request);
    Task ReturnBookAsync(ReturnRequestDto request);
}