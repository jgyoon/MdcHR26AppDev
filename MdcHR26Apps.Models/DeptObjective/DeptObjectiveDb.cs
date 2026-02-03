using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.DeptObjective;

[Table("DeptObjectiveDb")]
public class DeptObjectiveDb
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 DeptObjectiveDbId { get; set; }

    [Required]
    public Int64 EDepartId { get; set; }

    [Required]
    public string ObjectiveTitle { get; set; } = string.Empty;

    [Required]
    public string ObjectiveContents { get; set; } = string.Empty;

    [Required]
    public Int64 CreatedBy { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public Int64? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Remarks { get; set; }

    // Navigation Properties
    [ForeignKey("EDepartId")]
    public Department.EDepartmentDb? EDepartment { get; set; }

    [ForeignKey("CreatedBy")]
    public User.UserDb? Creator { get; set; }

    [ForeignKey("UpdatedBy")]
    public User.UserDb? Updater { get; set; }
}
