namespace MdcHR26Apps.Models.Views.v_MemberListDB;

/// <summary>
/// v_MemberListDB Repository Interface (읽기 전용)
/// </summary>
public interface Iv_MemberListRepository : IDisposable
{
    /// <summary>
    /// 전체 회원 목록 조회 (부서명, 직급명 포함)
    /// </summary>
    Task<IEnumerable<v_MemberListDB>> GetByAllAsync();

    /// <summary>
    /// 특정 회원 조회 (Uid 기준)
    /// </summary>
    Task<v_MemberListDB?> GetByUidAsync(Int64 uid);

    /// <summary>
    /// 부서별 회원 목록 조회
    /// </summary>
    Task<IEnumerable<v_MemberListDB>> GetByDepartmentAsync(Int64 departId);

    /// <summary>
    /// 직급별 회원 목록 조회
    /// </summary>
    Task<IEnumerable<v_MemberListDB>> GetByRankAsync(Int64 rankId);

    /// <summary>
    /// 활성 회원 목록 조회 (EStatus = 1)
    /// </summary>
    Task<IEnumerable<v_MemberListDB>> GetActiveUsersAsync();

    /// <summary>
    /// 사용자 ID로 검색
    /// </summary>
    Task<v_MemberListDB?> GetByUserIdAsync(string userId);
}
