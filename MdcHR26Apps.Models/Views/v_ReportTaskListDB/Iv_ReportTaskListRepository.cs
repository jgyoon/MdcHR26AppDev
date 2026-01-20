namespace MdcHR26Apps.Models.Views.v_ReportTaskListDB;

/// <summary>
/// v_ReportTaskListDB Repository Interface (읽기 전용)
/// </summary>
public interface Iv_ReportTaskListRepository : IDisposable
{
    /// <summary>
    /// 전체 평가 보고서-업무 목록 조회
    /// </summary>
    Task<IEnumerable<v_ReportTaskListDB>> GetByAllAsync();

    /// <summary>
    /// 특정 보고서의 업무 목록 조회
    /// </summary>
    Task<IEnumerable<v_ReportTaskListDB>> GetByReportIdAsync(Int64 reportId);

    /// <summary>
    /// 특정 사용자의 보고서-업무 조회
    /// </summary>
    Task<IEnumerable<v_ReportTaskListDB>> GetByUserIdAsync(Int64 uid);

    /// <summary>
    /// 프로세스별 보고서-업무 조회
    /// </summary>
    Task<IEnumerable<v_ReportTaskListDB>> GetByProcessIdAsync(Int64 processId);

    /// <summary>
    /// 업무 상태별 조회
    /// </summary>
    Task<IEnumerable<v_ReportTaskListDB>> GetByTaskStatusAsync(int taskStatus);

    /// <summary>
    /// 달성률 기준 조회 (특정 달성률 이상)
    /// </summary>
    Task<IEnumerable<v_ReportTaskListDB>> GetByAchievementRateAsync(decimal minRate);

    /// <summary>
    /// 특정 세부협의 항목의 업무 조회
    /// </summary>
    Task<IEnumerable<v_ReportTaskListDB>> GetBySubAgreementAsync(Int64 subAgreementId);
}
