namespace MdcHR26Apps.Models.Views.v_DeptObjectiveListDb;

/// <summary>
/// v_DeptObjectiveListDb Repository Interface (읽기 전용)
/// </summary>
public interface Iv_DeptObjectiveListRepository : IDisposable
{
    /// <summary>
    /// 전체 부서 목표 목록 조회 (부서명 포함)
    /// </summary>
    Task<IEnumerable<v_DeptObjectiveListDb>> GetByAllAsync();

    /// <summary>
    /// 특정 부서 목표 조회 (DOid 기준)
    /// </summary>
    Task<v_DeptObjectiveListDb?> GetByIdAsync(Int64 id);

    /// <summary>
    /// 부서별 목표 목록 조회
    /// </summary>
    Task<IEnumerable<v_DeptObjectiveListDb>> GetByDepartmentAsync(Int64 departId);

    /// <summary>
    /// 기간별 목표 조회
    /// </summary>
    Task<IEnumerable<v_DeptObjectiveListDb>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
}
