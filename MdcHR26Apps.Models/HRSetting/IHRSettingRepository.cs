namespace MdcHR26Apps.Models.HRSetting;

/// <summary>
/// HRSetting Repository Interface
/// Settings 패턴 (단일 레코드 관리)
/// </summary>
public interface IHRSettingRepository : IDisposable
{
    /// <summary>
    /// 현재 시스템 설정 조회
    /// </summary>
    Task<HRSettingDb?> GetCurrentAsync();

    /// <summary>
    /// 시스템 설정 업데이트
    /// </summary>
    Task<int> UpdateAsync(HRSettingDb setting);

    /// <summary>
    /// 초기 레코드 생성 (없을 경우)
    /// </summary>
    Task<int> InitializeAsync();
}
