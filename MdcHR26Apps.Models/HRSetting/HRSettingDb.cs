using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.HRSetting;

/// <summary>
/// 시스템 설정 Entity (단일 레코드 관리)
/// </summary>
[Table("HRSetting")]
public class HRSettingDb
{
    /// <summary>
    /// 설정 ID (PK, IDENTITY)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 HRSid { get; set; }

    /// <summary>
    /// 평가 오픈 여부 (BIT)
    /// 0: 평가 종료, 1: 평가 오픈
    /// </summary>
    [Required]
    public bool Evaluation_Open { get; set; } = false;

    /// <summary>
    /// 평가 수정 가능 여부 (BIT)
    /// 0: 수정 불가, 1: 수정 가능 (SubAgree 팀장 초기화 등)
    /// </summary>
    [Required]
    public bool Edit_Open { get; set; } = false;
}
