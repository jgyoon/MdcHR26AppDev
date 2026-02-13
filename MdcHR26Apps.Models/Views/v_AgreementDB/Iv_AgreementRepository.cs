namespace MdcHR26Apps.Models.Views.v_AgreementDB;

/// <summary>
/// v_AgreementDB Repository Interface (읽기 전용)
/// </summary>
public interface Iv_AgreementRepository : IDisposable
{
    /// <summary>
    /// 전체 협의 목록 조회
    /// </summary>
    Task<List<v_AgreementDB>> GetAllAsync();

    /// <summary>
    /// Aid로 협의 조회
    /// </summary>
    Task<v_AgreementDB?> GetByIdAsync(long aid);

    /// <summary>
    /// 사용자별 협의 목록 조회
    /// </summary>
    Task<List<v_AgreementDB>> GetByUserIdAsync(Int64 uid);
}
