namespace MdcHR26Apps.Models.EvaluationUsers;

/// <summary>
/// EvaluationUsers Repository Interface
/// </summary>
public interface IEvaluationUsersRepository : IDisposable
{
    // === 기본 CRUD ===

    /// <summary>
    /// 평가 참여자 추가
    /// </summary>
    Task<Int64> AddAsync(EvaluationUsers model);

    /// <summary>
    /// 전체 참여자 목록 조회
    /// </summary>
    Task<IEnumerable<EvaluationUsers>> GetByAllAsync();

    /// <summary>
    /// ID로 참여자 조회
    /// </summary>
    Task<EvaluationUsers?> GetByIdAsync(Int64 id);

    /// <summary>
    /// 참여자 정보 수정
    /// </summary>
    Task<int> UpdateAsync(EvaluationUsers model);

    /// <summary>
    /// 참여자 삭제
    /// </summary>
    Task<int> DeleteAsync(Int64 id);

    // === 비즈니스 특화 메서드 ===

    /// <summary>
    /// Uid 존재 여부 확인
    /// </summary>
    Task<bool> CheckUidAsync(Int64 uid);

    /// <summary>
    /// Uid로 참여자 조회
    /// </summary>
    Task<EvaluationUsers?> GetByUidAsync(Int64 uid);

    /// <summary>
    /// 부서장의 팀원 목록 조회
    /// </summary>
    Task<IEnumerable<EvaluationUsers>> GetByTeamLeaderIdAsync(Int64 teamLeaderId);

    /// <summary>
    /// 사용자 이름으로 검색 (부분 일치)
    /// 참고: View를 통한 조회 권장
    /// </summary>
    Task<IEnumerable<EvaluationUsers>> SearchByNameAsync(string userName);
}
