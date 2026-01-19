using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.DeptObjective;

/// <summary>
/// 부서 목표 Entity
/// </summary>
[Table("DeptObjectiveDb")]
public class DeptObjectiveDb
{
    /// <summary>
    /// DeptObjective ID (PK, IDENTITY)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 DOid { get; set; }

    /// <summary>
    /// 부서 ID (FK → EDepartmentDb.EDepartId)
    /// </summary>
    [Required]
    public Int64 EDepartId { get; set; }

    /// <summary>
    /// 목표 제목
    /// </summary>
    [Required]
    [StringLength(255)]
    public string Objective_Title { get; set; } = string.Empty;

    /// <summary>
    /// 목표 내용
    /// </summary>
    public string? Objective_Content { get; set; }

    /// <summary>
    /// 목표 달성 기준
    /// </summary>
    public string? Achievement_Criteria { get; set; }

    /// <summary>
    /// 활성화 여부
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;

    // Navigation Property
    [ForeignKey("EDepartId")]
    public Department.EDepartmentDb? EDepartment { get; set; }
}
