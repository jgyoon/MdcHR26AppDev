namespace MdcHR26Apps.Models.EvaluationProcess;

/// <summary>
/// ProcessDb Repository Interface
/// </summary>
public interface IProcessRepository : IDisposable
{
    // === 기본 CRUD ===

    /// <summary>
    /// 평가 프로세스 추가
    /// </summary>
    Task<Int64> AddAsync(ProcessDb model);

    /// <summary>
    /// 전체 프로세스 목록 조회
    /// </summary>
    Task<IEnumerable<ProcessDb>> GetByAllAsync();

    /// <summary>
    /// ID로 프로세스 조회
    /// </summary>
    Task<ProcessDb?> GetByIdAsync(Int64 id);

    /// <summary>
    /// 프로세스 정보 수정
    /// </summary>
    Task<int> UpdateAsync(ProcessDb model);

    /// <summary>
    /// 프로세스 삭제
    /// </summary>
    Task<int> DeleteAsync(Int64 id);

    // === 비즈니스 특화 메서드 ===

    /// <summary>
    /// 사용자 ID로 프로세스 조회
    /// </summary>
    Task<ProcessDb?> GetByUidAsync(Int64 uid);

    /// <summary>
    /// 합의 요청 여부 확인
    /// </summary>
    Task<bool> IsRequestCheckAsync(Int64 uid);

    /// <summary>
    /// 합의 완료 여부 확인
    /// </summary>
    Task<bool> IsAgreementCheckAsync(Int64 uid);

    /// <summary>
    /// 평가 제출 여부 확인
    /// </summary>
    Task<bool> IsReportSubmissionCheckAsync(Int64 uid);

    /// <summary>
    /// 사용자 존재 여부 확인
    /// </summary>
    Task<bool> CheckUidAsync(Int64 uid);

    /// <summary>
    /// 부서장 관할 팀원 목록 조회
    /// </summary>
    Task<IEnumerable<ProcessDb>> GetByTeamLeaderIdAsync(Int64 teamLeaderId);

    /// <summary>
    /// 부서장 관할 중 사용자 제출 완료 목록
    /// </summary>
    Task<IEnumerable<ProcessDb>> GetByTeamLeaderIdWithUserSubmissionAsync(Int64 teamLeaderId);

    /// <summary>
    /// 임원 관할 팀원 목록 조회
    /// </summary>
    Task<IEnumerable<ProcessDb>> GetByDirectorIdAsync(Int64 directorId);

    /// <summary>
    /// 임원 관할 중 부서장 제출 완료 목록
    /// </summary>
    Task<IEnumerable<ProcessDb>> GetByDirectorIdWithTeamleaderSubmissionAsync(Int64 directorId);

    /// <summary>
    /// 부서장 평가 제출 완료 목록
    /// </summary>
    Task<IEnumerable<ProcessDb>> GetByTeamLeaderIdWithTeamLeaderSubmissionAsync(Int64 teamLeaderId);
}
