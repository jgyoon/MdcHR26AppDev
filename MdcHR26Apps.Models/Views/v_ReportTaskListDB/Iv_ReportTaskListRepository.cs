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
    /// 전체 평가 보고서-업무 목록 조회 (List 반환)
    /// </summary>
    Task<List<v_ReportTaskListDB>> GetAllAsync();

    /// <summary>
    /// 특정 보고서의 업무 목록 조회
    /// </summary>
    Task<IEnumerable<v_ReportTaskListDB>> GetByReportIdAsync(Int64 reportId);

    /// <summary>
    /// 특정 사용자의 보고서-업무 조회
    /// </summary>
    Task<List<v_ReportTaskListDB>> GetByUidAsync(Int64 uid);

    /// <summary>
    /// 특정 사용자의 모든 보고서-업무 조회 (Alias)
    /// </summary>
    Task<List<v_ReportTaskListDB>> GetByUidAllAsync(Int64 uid);

    /// <summary>
    /// Task_Number별 보고서-업무 목록 조회
    /// </summary>
    Task<List<v_ReportTaskListDB>> GetByTaksListNumberAllAsync(Int64 TaksListNumber);

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
