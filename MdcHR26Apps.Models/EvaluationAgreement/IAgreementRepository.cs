namespace MdcHR26Apps.Models.EvaluationAgreement;

/// <summary>
/// AgreementDb Repository Interface
/// </summary>
public interface IAgreementRepository : IDisposable
{
    // === 기본 CRUD ===

    /// <summary>
    /// 직무평가 협의서 추가
    /// </summary>
    Task<Int64> AddAsync(AgreementDb model);

    /// <summary>
    /// 전체 협의서 조회
    /// </summary>
    Task<IEnumerable<AgreementDb>> GetByAllAsync();

    /// <summary>
    /// ID로 협의서 조회
    /// </summary>
    Task<AgreementDb?> GetByIdAsync(Int64 id);

    /// <summary>
    /// 협의서 수정
    /// </summary>
    Task<int> UpdateAsync(AgreementDb model);

    /// <summary>
    /// 협의서 삭제
    /// </summary>
    Task<int> DeleteAsync(Int64 id);

    // === 비즈니스 특화 메서드 ===

    /// <summary>
    /// 사용자별 협의서 조회
    /// </summary>
    Task<IEnumerable<AgreementDb>> GetByUidAsync(Int64 uid);

    /// <summary>
    /// 합의 완료 여부 확인
    /// </summary>
    Task<bool> IsAgreementCompleteAsync(Int64 uid);

    /// <summary>
    /// 합의 대기 중인 항목 조회
    /// </summary>
    Task<IEnumerable<AgreementDb>> GetPendingAgreementAsync(Int64 uid);

    /// <summary>
    /// 부서 목표별 협의서 조회
    /// </summary>
    Task<IEnumerable<AgreementDb>> GetByDeptObjectiveAsync(Int64 deptObjectiveId);
}
