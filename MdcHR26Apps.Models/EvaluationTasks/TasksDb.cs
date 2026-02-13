using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.EvaluationTasks;

[Table("TasksDb")]
public class TasksDb
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 Tid { get; set; }

    [Required]
    [StringLength(100)]
    public string TaskName { get; set; } = string.Empty;

    [Required]
    public Int64 TaksListNumber { get; set; }

    [Required]
    public int TaskStatus { get; set; }

    [Required]
    public string TaskObjective { get; set; } = string.Empty;

    [Required]
    public int TargetProportion { get; set; }

    [Required]
    public int ResultProportion { get; set; }

    [Required]
    public DateTime TargetDate { get; set; }

    [Required]
    public DateTime ResultDate { get; set; }

    [Required]
    public double Task_Evaluation_1 { get; set; }

    [Required]
    public double Task_Evaluation_2 { get; set; }

    [Required]
    public double TaskLevel { get; set; }

    [Required]
    [StringLength(50)]
    public string TaskComments { get; set; } = string.Empty;
}
