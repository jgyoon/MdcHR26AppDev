using MdcHR26Apps.Models.Common;

namespace MdcHR26Apps.Models.EvaluationLists;

/// <summary>
/// EvaluationLists Repository Interface
/// </summary>
public interface IEvaluationListsRepository : IDisposable
{
    // === 기본 CRUD ===

    /// <summary>
    /// 평가 항목 추가
    /// </summary>
    Task<Int64> AddAsync(EvaluationLists model);

    /// <summary>
    /// 전체 평가 항목 조회
    /// </summary>
    Task<IEnumerable<EvaluationLists>> GetByAllAsync();

    /// <summary>
    /// ID로 평가 항목 조회
    /// </summary>
    Task<EvaluationLists?> GetByIdAsync(Int64 id);

    /// <summary>
    /// 평가 항목 수정
    /// </summary>
    Task<int> UpdateAsync(EvaluationLists model);

    /// <summary>
    /// 평가 항목 삭제
    /// </summary>
    Task<int> DeleteAsync(Int64 id);

    // === 비즈니스 특화 메서드 ===

    /// <summary>
    /// 활성화된 평가 항목 조회
    /// </summary>
    Task<IEnumerable<EvaluationLists>> GetActiveAsync();

    /// <summary>
    /// 평가 번호로 조회
    /// </summary>
    Task<EvaluationLists?> GetByNumberAsync(int evaluationNumber);

    /// <summary>
    /// 드롭다운용 평가 항목 목록
    /// </summary>
    Task<IEnumerable<SelectListModel>> GetSelectListAsync();
}
