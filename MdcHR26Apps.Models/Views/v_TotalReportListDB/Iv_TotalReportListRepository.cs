namespace MdcHR26Apps.Models.Views.v_TotalReportListDB;

/// <summary>
/// v_TotalReportListDB Repository Interface (읽기 전용)
/// </summary>
public interface Iv_TotalReportListRepository : IDisposable
{
    /// <summary>
    /// 전체 최종 평가 결과 조회
    /// </summary>
    Task<IEnumerable<v_TotalReportListDB>> GetByAllAsync();

    /// <summary>
    /// 특정 최종 평가 결과 조회 (TRid 기준)
    /// </summary>
    Task<v_TotalReportListDB?> GetByIdAsync(Int64 id);

    /// <summary>
    /// 특정 사용자의 최종 평가 결과 조회
    /// </summary>
    Task<IEnumerable<v_TotalReportListDB>> GetByUserIdAsync(Int64 uid);

    /// <summary>
    /// 프로세스별 최종 평가 결과 조회
    /// </summary>
    Task<IEnumerable<v_TotalReportListDB>> GetByProcessIdAsync(Int64 processId);

    /// <summary>
    /// 연도별 최종 평가 결과 조회
    /// </summary>
    Task<IEnumerable<v_TotalReportListDB>> GetByYearAsync(int year);

    /// <summary>
    /// 등급별 최종 평가 결과 조회
    /// </summary>
    Task<IEnumerable<v_TotalReportListDB>> GetByGradeAsync(string grade);

    /// <summary>
    /// 부서별 최종 평가 결과 조회
    /// </summary>
    Task<IEnumerable<v_TotalReportListDB>> GetByDepartmentAsync(Int64 departId);

    /// <summary>
    /// 직급별 최종 평가 결과 조회
    /// </summary>
    Task<IEnumerable<v_TotalReportListDB>> GetByRankAsync(Int64 rankId);

    /// <summary>
    /// 점수 범위별 조회 (최소~최대)
    /// </summary>
    Task<IEnumerable<v_TotalReportListDB>> GetByScoreRangeAsync(decimal minScore, decimal maxScore);
}
