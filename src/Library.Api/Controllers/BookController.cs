using Microsoft.AspNetCore.Mvc;
using Library.Application.DTOs;
using Library.Application.Interfaces;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    // Controller depends on IBookService only
    // All rules (copies check, cache, concurrency) are enforced in BookService.
    private readonly IBookService _service;

    public BooksController(IBookService service) => _service = service;

    // GET /api/books — Retrieves all books
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var books = await _service.GetAllBooksAsync();
        return Ok(books);
    }

    // GET /api/books/{id} - Gets book by its unique id
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var book = await _service.GetBookByIdAsync(id);
        return Ok(book);    // NotFoundException handled by ExceptionMiddleware 
    }

    // POST /api/books - Creates a new book and returns it to confirm created resource
    [HttpPost]
    public async Task<IActionResult> Create(CreateBookDto dto)
    {
        var created = await _service.CreateBookAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT /api/books/{id} - Updates book record based on Id and data
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, CreateBookDto dto)
    {
        var updated = await _service.UpdateBookAsync(id, dto);
        return Ok(updated);
    }

    // DELETE /api/books/{id} - Deletes record from the system
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteBookAsync(id);
        return NoContent();
    }

    // POST /api/books/borrow - Request to borrow a book
    [HttpPost("borrow")]
    public async Task<IActionResult> Borrow(BorrowRequestDto request)
    {
        await _service.BorrowBookAsync(request);
        return Ok(new { message = "Book borrowed successfully." });
    }

    // POST /api/books/return - Process the return of a borrowed book and updates inventory
    [HttpPost("return")]
    public async Task<IActionResult> Return(ReturnRequestDto request)
    {
        await _service.ReturnBookAsync(request);
        return Ok(new { message = "Book returned successfully." });
    }
}
