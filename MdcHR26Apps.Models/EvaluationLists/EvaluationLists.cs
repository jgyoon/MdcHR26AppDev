using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.EvaluationLists;

/// <summary>
/// 평가 항목 마스터 Entity
/// </summary>
[Table("EvaluationLists")]
public class EvaluationLists
{
    /// <summary>
    /// Evaluation List ID (PK, IDENTITY)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 ELid { get; set; }

    /// <summary>
    /// 평가 항목 번호
    /// </summary>
    [Required]
    public int Evaluation_Number { get; set; }

    /// <summary>
    /// 평가 항목명
    /// </summary>
    [Required]
    [StringLength(255)]
    public string Evaluation_Item { get; set; } = string.Empty;

    /// <summary>
    /// 평가 항목 설명
    /// </summary>
    public string? Evaluation_Description { get; set; }

    /// <summary>
    /// 배점
    /// </summary>
    [Required]
    public int Score { get; set; }

    /// <summary>
    /// 활성화 여부
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;
}
