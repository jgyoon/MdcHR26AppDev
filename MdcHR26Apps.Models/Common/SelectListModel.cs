namespace MdcHR26Apps.Models.Common;

/// <summary>
/// 드롭다운 목록용 공통 모델
/// </summary>
public class SelectListModel
{
    /// <summary>
    /// 값 (ID)
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// 표시 텍스트
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// 선택 여부
    /// </summary>
    public bool Selected { get; set; } = false;

    /// <summary>
    /// 비활성화 여부
    /// </summary>
    public bool Disabled { get; set; } = false;

    /// <summary>
    /// 그룹명 (선택사항)
    /// </summary>
    public string? Group { get; set; }
}
