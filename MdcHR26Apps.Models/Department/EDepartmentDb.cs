using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.Department;

/// <summary>
/// 부서 마스터 Entity
/// </summary>
[Table("EDepartmentDb")]
public class EDepartmentDb
{
    /// <summary>
    /// 부서 ID (PK, IDENTITY)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 EDepartId { get; set; }

    /// <summary>
    /// 부서 번호 (정렬 순서, UNIQUE)
    /// </summary>
    [Required]
    public int EDepartmentNo { get; set; }

    /// <summary>
    /// 부서명
    /// </summary>
    [Required]
    [StringLength(255)]
    public string EDepartmentName { get; set; } = string.Empty;

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
