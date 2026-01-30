namespace MdcHR26Apps.Models.Result;

/// <summary>
/// TotalReportDb Repository Interface
/// </summary>
public interface ITotalReportRepository : IDisposable
{
    // === 기본 CRUD ===

    /// <summary>
    /// 종합 평가 추가
    /// </summary>
    Task<Int64> AddAsync(TotalReportDb model);

    /// <summary>
    /// 전체 종합 평가 목록 조회
    /// </summary>
    Task<IEnumerable<TotalReportDb>> GetByAllAsync();

    /// <summary>
    /// ID로 종합 평가 조회
    /// </summary>
    Task<TotalReportDb?> GetByIdAsync(Int64 id);

    /// <summary>
    /// 종합 평가 수정
    /// </summary>
    Task<int> UpdateAsync(TotalReportDb model);

    /// <summary>
    /// 종합 평가 삭제
    /// </summary>
    Task<int> DeleteAsync(Int64 id);

    /// <summary>
    /// 사용자별 종합 평가 조회
    /// </summary>
    Task<TotalReportDb?> GetByUidAsync(Int64 uid);

    /// <summary>
    /// Pid로 종합 평가 조회
    /// </summary>
    Task<TotalReportDb?> GetByPidAsync(long pid);
}
