using MdcHR26Apps.Models.Common;

namespace MdcHR26Apps.Models.Department;

/// <summary>
/// EDepartmentDb Repository Interface
/// </summary>
public interface IEDepartmentRepository : IDisposable
{
    // === 기본 CRUD ===
    Task<Int64> AddAsync(EDepartmentDb department);
    Task<IEnumerable<EDepartmentDb>> GetByAllAsync();
    Task<EDepartmentDb?> GetByIdAsync(long departmentId);
    Task<int> UpdateAsync(EDepartmentDb department);
    Task<int> DeleteAsync(long departmentId);

    // === 비즈니스 특화 메서드 ===

    /// <summary>
    /// 활성화된 부서 목록 조회
    /// </summary>
    Task<IEnumerable<EDepartmentDb>> GetActiveAsync();

    /// <summary>
    /// 드롭다운용 부서 목록
    /// </summary>
    Task<IEnumerable<SelectListModel>> GetSelectListAsync();
}
