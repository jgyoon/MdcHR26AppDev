using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.EvaluationAgreement;

/// <summary>
/// 직무평가 협의서 Entity (대분류)
/// </summary>
[Table("AgreementDb")]
public class AgreementDb
{
    /// <summary>
    /// Agreement ID (PK, IDENTITY)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 Aid { get; set; }

    /// <summary>
    /// 평가 대상자 ID (FK → UserDb.Uid)
    /// </summary>
    [Required]
    public Int64 Uid { get; set; }

    /// <summary>
    /// 협의서 항목 번호
    /// </summary>
    [Required]
    public int Agreement_Item_Number { get; set; }

    /// <summary>
    /// 지표 분류명
    /// </summary>
    [Required]
    [StringLength(255)]
    public string Agreement_Item_Name_1 { get; set; } = string.Empty;

    /// <summary>
    /// 직무 분류명
    /// </summary>
    [Required]
    [StringLength(255)]
    public string Agreement_Item_Name_2 { get; set; } = string.Empty;

    /// <summary>
    /// 직무 비율(%)
    /// </summary>
    [Required]
    public int Agreement_Item_Proportion { get; set; }

    /// <summary>
    /// 부서 목표 연결 (FK → DeptObjectiveDb.DOid)
    /// </summary>
    public Int64? DeptObjective_Number { get; set; }

    /// <summary>
    /// 합의 여부
    /// </summary>
    [Required]
    public bool Is_Agreement { get; set; } = false;

    /// <summary>
    /// 합의 의견
    /// </summary>
    public string? Agreement_Comment { get; set; }

    /// <summary>
    /// 합의 일시
    /// </summary>
    public DateTime? Agreement_Date { get; set; }

    // Navigation Properties
    [ForeignKey("Uid")]
    public User.UserDb? User { get; set; }

    [ForeignKey("DeptObjective_Number")]
    public DeptObjective.DeptObjectiveDb? DeptObjective { get; set; }
}
