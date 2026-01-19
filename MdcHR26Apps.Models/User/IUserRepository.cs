using MdcHR26Apps.Models.Common;

namespace MdcHR26Apps.Models.User;

/// <summary>
/// UserDb Repository Interface
/// </summary>
public interface IUserRepository : IDisposable
{
    // === 기본 CRUD ===

    /// <summary>
    /// 사용자 추가
    /// </summary>
    Task<Int64> AddAsync(UserDb user);

    /// <summary>
    /// 전체 사용자 조회
    /// </summary>
    Task<IEnumerable<UserDb>> GetByAllAsync();

    /// <summary>
    /// ID로 사용자 조회
    /// </summary>
    Task<UserDb?> GetByIdAsync(long uid);

    /// <summary>
    /// 사용자 정보 수정
    /// </summary>
    Task<int> UpdateAsync(UserDb user);

    /// <summary>
    /// 사용자 삭제
    /// </summary>
    Task<int> DeleteAsync(long uid);

    // === 비즈니스 특화 메서드 ===

    /// <summary>
    /// 재직자 목록 조회 (EStatus = true)
    /// </summary>
    Task<IEnumerable<UserDb>> GetActiveUsersAsync();

    /// <summary>
    /// 부서별 사용자 조회
    /// </summary>
    Task<IEnumerable<UserDb>> GetByDepartmentAsync(long departmentId);

    /// <summary>
    /// 로그인 인증 (Uid로 조회)
    /// </summary>
    Task<UserDb?> AuthenticateAsync(string userId);

    /// <summary>
    /// 비밀번호 변경
    /// </summary>
    Task<int> UpdatePasswordAsync(long uid, byte[] passwordHash, byte[] salt);

    /// <summary>
    /// 재직 상태 변경
    /// </summary>
    Task<int> UpdateStatusAsync(long uid, bool status);

    /// <summary>
    /// 비밀번호 없이 정보만 수정
    /// </summary>
    Task<int> UpdateWithoutPasswordAsync(UserDb user);

    /// <summary>
    /// 로그인 체크 (UserId + Password)
    /// </summary>
    Task<bool> LoginCheckAsync(string userId, string password);

    /// <summary>
    /// UserId 존재 여부 확인
    /// </summary>
    Task<bool> UserIdCheckAsync(string userId);

    /// <summary>
    /// UserId로 사용자 조회
    /// </summary>
    Task<UserDb?> GetByUserIdAsync(string userId);

    /// <summary>
    /// 팀장 목록 조회 (IsTeamLeader = true)
    /// </summary>
    Task<IEnumerable<UserDb>> GetTeamLeadersAsync();

    /// <summary>
    /// 임원 목록 조회 (IsDirector = true)
    /// </summary>
    Task<IEnumerable<UserDb>> GetDirectorsAsync();

    /// <summary>
    /// 사용자명으로 검색 (LIKE 검색)
    /// </summary>
    Task<IEnumerable<UserDb>> GetByUserNameAsync(string userName);

    /// <summary>
    /// 드롭다운용 사용자 목록 (재직자만)
    /// </summary>
    Task<IEnumerable<SelectListModel>> GetSelectListAsync();
}
