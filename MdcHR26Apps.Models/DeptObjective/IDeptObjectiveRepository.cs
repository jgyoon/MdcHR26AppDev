namespace MdcHR26Apps.Models.DeptObjective;

/// <summary>
/// DeptObjectiveDb Repository Interface
/// </summary>
public interface IDeptObjectiveRepository : IDisposable
{
    // === 기본 CRUD ===

    /// <summary>
    /// 부서 목표 추가
    /// </summary>
    Task<Int64> AddAsync(DeptObjectiveDb model);

    /// <summary>
    /// 전체 부서 목표 조회
    /// </summary>
    Task<IEnumerable<DeptObjectiveDb>> GetByAllAsync();

    /// <summary>
    /// ID로 부서 목표 조회
    /// </summary>
    Task<DeptObjectiveDb?> GetByIdAsync(Int64 id);

    /// <summary>
    /// 부서 목표 수정
    /// </summary>
    Task<int> UpdateAsync(DeptObjectiveDb model);

    /// <summary>
    /// 부서 목표 삭제
    /// </summary>
    Task<int> DeleteAsync(Int64 id);

    // === 비즈니스 특화 메서드 ===

    /// <summary>
    /// 부서별 목표 조회
    /// </summary>
    Task<IEnumerable<DeptObjectiveDb>> GetByDepartmentAsync(Int64 departmentId);

    /// <summary>
    /// 활성화된 부서 목표 조회
    /// </summary>
    Task<IEnumerable<DeptObjectiveDb>> GetActiveAsync();
}
