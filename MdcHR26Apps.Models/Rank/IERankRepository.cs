using MdcHR26Apps.Models.Common;

namespace MdcHR26Apps.Models.Rank;

/// <summary>
/// ERankDb Repository Interface
/// </summary>
public interface IERankRepository : IDisposable
{
    // === 기본 CRUD ===
    Task<Int64> AddAsync(ERankDb rank);
    Task<IEnumerable<ERankDb>> GetByAllAsync();
    Task<ERankDb?> GetByIdAsync(long rankId);
    Task<int> UpdateAsync(ERankDb rank);
    Task<int> DeleteAsync(long rankId);

    // === 비즈니스 특화 메서드 ===

    /// <summary>
    /// 활성화된 직급 목록 조회
    /// </summary>
    Task<IEnumerable<ERankDb>> GetActiveAsync();

    /// <summary>
    /// 드롭다운용 직급 목록
    /// </summary>
    Task<IEnumerable<SelectListModel>> GetSelectListAsync();
}
