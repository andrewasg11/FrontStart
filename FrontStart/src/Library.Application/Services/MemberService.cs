using Library.Application.DTOs;
using Library.Application.Exceptions;
using Library.Application.Interfaces;
using Library.Domain.Entities;
using Library.Domain.Interfaces;

namespace Library.Application.Services;
// Manages library member accounts, registration, profile update and email validation
public class MemberService : IMemberService
{
    private readonly IMemberRepository _memberRepository;

    public MemberService(IMemberRepository memberRepository)
    {
        _memberRepository = memberRepository;
    }

    public async Task<IEnumerable<MemberResponseDto>> GetAllMembersAsync()
    {
        var members = await _memberRepository.GetAllAsync();
        return members.Select(MapToDto);
    }

    public async Task<MemberResponseDto> GetMemberByIdAsync(Guid id)
    {
        var member = await _memberRepository.GetByIdAsync(id);

        if (member == null)
            throw new NotFoundException($"Member with id '{id}' was not found.");

        return MapToDto(member);
    }

    public async Task<MemberResponseDto> CreateMemberAsync(CreateMemberDto dto)
    {
        var existing = await _memberRepository.GetByEmailAsync(dto.Email);

        if (existing != null)
            throw new ConflictException($"A member with email '{dto.Email}' already exists.");

        var member = new Member
        {
            FullName = dto.FullName,
            Email = dto.Email,
            MembershipDate = DateTime.UtcNow
        };

        await _memberRepository.AddAsync(member);
        return MapToDto(member);
    }

    // Updatesd existing members profile and validates email avialability
    public async Task<MemberResponseDto> UpdateMemberAsync(Guid id, CreateMemberDto dto)
    {
        var member = await _memberRepository.GetByIdAsync(id);
        if (member == null)
            throw new NotFoundException($"Member with id '{id}' was not found.");

        if (!string.Equals(member.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
        {
            var conflict = await _memberRepository.GetByEmailAsync(dto.Email);
            if (conflict != null)
                throw new ConflictException($"Email '{dto.Email}' is already in use.");
        }

        member.FullName = dto.FullName;
        member.Email = dto.Email;

        await _memberRepository.UpdateAsync(member);
        return MapToDto(member);
    }

    public async Task DeleteMemberAsync(Guid id)
    {
        var member = await _memberRepository.GetByIdAsync(id);
        if (member == null)
            throw new NotFoundException($"Member with id '{id}' was not found.");

        await _memberRepository.DeleteAsync(id);
    }

    // Maps member entity to a MemberResponseDTO
    private static MemberResponseDto MapToDto(Member m) => new()
    {
        Id = m.Id,
        FullName = m.FullName,
        Email = m.Email,
        MembershipDate = m.MembershipDate
    };
}