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

}
