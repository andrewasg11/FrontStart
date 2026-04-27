using Library.Application.DTOs;

namespace Library.Application.Interfaces
{
    // Business logic operations for managing library member profiles and registrations
    public interface IMemberService
    {
        Task<IEnumerable<MemberResponseDto>> GetAllMembersAsync();
        Task<MemberResponseDto> GetMemberByIdAsync(Guid id);
        Task<MemberResponseDto> CreateMemberAsync(CreateMemberDto dto);
        Task<MemberResponseDto> UpdateMemberAsync(Guid id, CreateMemberDto dto);
        Task DeleteMemberAsync(Guid id);
    }
}