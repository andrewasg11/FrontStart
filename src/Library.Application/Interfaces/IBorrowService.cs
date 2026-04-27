using Library.Application.DTOs;

namespace Library.Application.Interfaces
{
    public interface IBorrowService
    {
        Task<IEnumerable<BorrowRecordResponseDto>> GetAllRecordsAsync();
        Task<IEnumerable<BorrowRecordResponseDto>> GetMemberHistoryAsync(Guid memberId);
    }
}