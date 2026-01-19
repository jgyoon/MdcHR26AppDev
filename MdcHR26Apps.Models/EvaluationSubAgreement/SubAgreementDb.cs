using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.EvaluationSubAgreement;

/// <summary>
/// 상세 직무평가 협의서 Entity (세부 항목)
/// </summary>
[Table("SubAgreementDb")]
public class SubAgreementDb
{
    /// <summary>
    /// SubAgreement ID (PK, IDENTITY)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 SAid { get; set; }

    /// <summary>
    /// 평가 대상자 ID (FK → UserDb.Uid)
    /// </summary>
    [Required]
    public Int64 Uid { get; set; }

    /// <summary>
    /// 상위 협의서 번호 (FK → AgreementDb.Aid)
    /// </summary>
    [Required]
    public Int64 Agreement_Number { get; set; }

    /// <summary>
    /// 세부 직무명
    /// </summary>
    [Required]
    [StringLength(255)]
    public string SubAgreement_Item_Name { get; set; } = string.Empty;

    /// <summary>
    /// 세부 직무 비율(%)
    /// </summary>
    [Required]
    public int SubAgreement_Item_Proportion { get; set; }

    /// <summary>
    /// 합의 여부
    /// </summary>
    [Required]
    public bool Is_SubAgreement { get; set; } = false;

    /// <summary>
    /// 합의 의견
    /// </summary>
    public string? SubAgreement_Comment { get; set; }

    /// <summary>
    /// 합의 일시
    /// </summary>
    public DateTime? SubAgreement_Date { get; set; }

    // Navigation Properties
    [ForeignKey("Uid")]
    public User.UserDb? User { get; set; }

    [ForeignKey("Agreement_Number")]
    public EvaluationAgreement.AgreementDb? Agreement { get; set; }
}
