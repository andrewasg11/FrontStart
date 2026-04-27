using Microsoft.AspNetCore.Mvc;
using Library.Application.Interfaces;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BorrowsController : ControllerBase
{
    private readonly IBorrowService _service;

    public BorrowsController(IBorrowService service) => _service = service;

    // GET /api/borrows — Retrieves complete history of borrows and returns in the system
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var records = await _service.GetAllRecordsAsync();
        return Ok(records);
    }

    // GET /api/borrows/member/{memberId} — Retrieves all borrow records associated with member id
    [HttpGet("member/{memberId:guid}")]
    public async Task<IActionResult> GetMemberHistory(Guid memberId)
    {
        var records = await _service.GetMemberHistoryAsync(memberId);
        return Ok(records);
    }
}
