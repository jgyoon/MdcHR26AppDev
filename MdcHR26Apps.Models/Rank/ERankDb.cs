using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.Rank;

/// <summary>
/// 직급 마스터 Entity
/// </summary>
[Table("ERankDb")]
public class ERankDb
{
    /// <summary>
    /// 직급 ID (PK, IDENTITY)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 ERankId { get; set; }

    /// <summary>
    /// 직급 번호 (정렬 순서, UNIQUE)
    /// </summary>
    [Required]
    public int ERankNo { get; set; }

    /// <summary>
    /// 직급명
    /// </summary>
    [Required]
    [StringLength(255)]
    public string ERankName { get; set; } = string.Empty;

    /// <summary>
    /// 활성화 여부 (BIT)
    /// </summary>
    [Required]
    public bool ActivateStatus { get; set; } = true;

    /// <summary>
    /// 비고
    /// </summary>
    public string? Remarks { get; set; }
}
