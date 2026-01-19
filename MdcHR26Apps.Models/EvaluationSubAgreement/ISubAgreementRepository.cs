namespace MdcHR26Apps.Models.EvaluationSubAgreement;

/// <summary>
/// SubAgreementDb Repository Interface
/// </summary>
public interface ISubAgreementRepository : IDisposable
{
    // === 기본 CRUD ===

    /// <summary>
    /// 상세 협의서 추가
    /// </summary>
    Task<Int64> AddAsync(SubAgreementDb model);

    /// <summary>
    /// 전체 상세 협의서 조회
    /// </summary>
    Task<IEnumerable<SubAgreementDb>> GetByAllAsync();

    /// <summary>
    /// ID로 상세 협의서 조회
    /// </summary>
    Task<SubAgreementDb?> GetByIdAsync(Int64 id);

    /// <summary>
    /// 상세 협의서 수정
    /// </summary>
    Task<int> UpdateAsync(SubAgreementDb model);

    /// <summary>
    /// 상세 협의서 삭제
    /// </summary>
    Task<int> DeleteAsync(Int64 id);

    // === 비즈니스 특화 메서드 ===

    /// <summary>
    /// 사용자별 상세 협의서 조회
    /// </summary>
    Task<IEnumerable<SubAgreementDb>> GetByUidAsync(Int64 uid);

    /// <summary>
    /// 상위 협의서별 세부 항목 조회
    /// </summary>
    Task<IEnumerable<SubAgreementDb>> GetByAgreementNumberAsync(Int64 agreementNumber);

    /// <summary>
    /// 합의 완료 여부 확인
    /// </summary>
    Task<bool> IsSubAgreementCompleteAsync(Int64 uid);

    /// <summary>
    /// 합의 대기 중인 세부 항목 조회
    /// </summary>
    Task<IEnumerable<SubAgreementDb>> GetPendingSubAgreementAsync(Int64 uid);

    /// <summary>
    /// 상위 협의서의 세부 항목 비율 합계
    /// </summary>
    Task<int> GetTotalProportionAsync(Int64 agreementNumber);
}
