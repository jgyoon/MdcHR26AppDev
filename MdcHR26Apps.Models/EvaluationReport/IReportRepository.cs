namespace MdcHR26Apps.Models.EvaluationReport;

/// <summary>
/// ReportDb Repository Interface
/// </summary>
public interface IReportRepository : IDisposable
{
    // === 기본 CRUD ===

    /// <summary>
    /// 평가 보고서 추가
    /// </summary>
    Task<Int64> AddAsync(ReportDb model);

    /// <summary>
    /// 전체 보고서 목록 조회
    /// </summary>
    Task<IEnumerable<ReportDb>> GetByAllAsync();

    /// <summary>
    /// ID로 보고서 조회
    /// </summary>
    Task<ReportDb?> GetByIdAsync(Int64 id);

    /// <summary>
    /// 보고서 수정
    /// </summary>
    Task<int> UpdateAsync(ReportDb model);

    /// <summary>
    /// 보고서 삭제
    /// </summary>
    Task<int> DeleteAsync(Int64 id);

    /// <summary>
    /// 사용자별 보고서 목록 조회
    /// </summary>
    Task<IEnumerable<ReportDb>> GetByUidAllAsync(Int64 uid);

    /// <summary>
    /// 사용자별 보고서 개수 조회
    /// </summary>
    Task<int> GetCountByUidAsync(long uid);

    /// <summary>
    /// 사용자별 보고서 전체 삭제
    /// </summary>
    Task<bool> DeleteAllByUidAsync(long uid);
}
