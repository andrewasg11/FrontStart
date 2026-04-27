using Microsoft.AspNetCore.Mvc;
using Library.Application.DTOs;
using Library.Application.Interfaces;

namespace Library.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class MembersController : ControllerBase
{
    private readonly IMemberService _service;

    public MembersController(IMemberService service) => _service = service;

    // GET /api/members - Retrieves list of all library members
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var members = await _service.GetAllMembersAsync();
        return Ok(members);
    }

    // GET /api/members/{id} - Retreives details for a single member by their id
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var member = await _service.GetMemberByIdAsync(id);
        return Ok(member);  // NotFoundException handled by Middleware
    }

    // POST /api/members - Creates new member and returns profile with assigned id
    [HttpPost]
    public async Task<IActionResult> Create(CreateMemberDto dto)
    {
        var created = await _service.CreateMemberAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT /api/members/{id} - Updates existing members information (contact details or status)
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, CreateMemberDto dto)
    {
        var updated = await _service.UpdateMemberAsync(id, dto);
        return Ok(updated);
    }

    // DELETE /api/members/{id} - Deletes member profile from system
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteMemberAsync(id);
        return NoContent();
    }
}
