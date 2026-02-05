namespace MdcHR26Apps.Models.Views.v_SubAgreementDB;

/// <summary>
/// v_SubAgreementDB Repository Interface (읽기 전용)
/// </summary>
public interface Iv_SubAgreementRepository : IDisposable
{
    /// <summary>
    /// 전체 세부협의 목록 조회
    /// </summary>
    Task<List<v_SubAgreementDB>> GetAllAsync();

    /// <summary>
    /// Sid로 세부협의 조회
    /// </summary>
    Task<v_SubAgreementDB?> GetByIdAsync(long sid);

    /// <summary>
    /// 사용자별 세부협의 목록 조회
    /// </summary>
    Task<List<v_SubAgreementDB>> GetByUidAsync(Int64 uid);
}
