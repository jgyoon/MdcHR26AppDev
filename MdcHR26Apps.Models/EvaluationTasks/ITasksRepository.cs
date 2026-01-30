namespace MdcHR26Apps.Models.EvaluationTasks;

/// <summary>
/// TasksDb Repository Interface
/// </summary>
public interface ITasksRepository : IDisposable
{
    // === 기본 CRUD ===

    /// <summary>
    /// 업무 추가
    /// </summary>
    Task<Int64> AddAsync(TasksDb model);

    /// <summary>
    /// 전체 업무 조회
    /// </summary>
    Task<IEnumerable<TasksDb>> GetByAllAsync();

    /// <summary>
    /// ID로 업무 조회
    /// </summary>
    Task<TasksDb?> GetByIdAsync(Int64 id);

    /// <summary>
    /// 업무 수정
    /// </summary>
    Task<int> UpdateAsync(TasksDb model);

    /// <summary>
    /// 업무 삭제
    /// </summary>
    Task<int> DeleteAsync(Int64 id);

    // === 비즈니스 특화 메서드 ===

    /// <summary>
    /// 사용자별 업무 조회
    /// </summary>
    Task<IEnumerable<TasksDb>> GetByUidAsync(Int64 uid);

    /// <summary>
    /// 세부 협의서별 업무 조회
    /// </summary>
    Task<IEnumerable<TasksDb>> GetBySubAgreementNumberAsync(Int64 subAgreementNumber);

    /// <summary>
    /// 미완료 업무 조회
    /// </summary>
    Task<IEnumerable<TasksDb>> GetIncompleteTasksAsync(Int64 uid);

    /// <summary>
    /// 사용자별 업무 개수 조회
    /// </summary>
    Task<int> GetCountByUserAsync(long uid);

    /// <summary>
    /// 사용자별 업무 전체 삭제
    /// </summary>
    Task<bool> DeleteAllByUserAsync(long uid);
}
