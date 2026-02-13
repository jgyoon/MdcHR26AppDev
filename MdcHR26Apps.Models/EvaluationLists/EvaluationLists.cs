using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.EvaluationLists;

[Table("EvaluationLists")]
public class EvaluationLists
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 Eid { get; set; }

    [Required]
    public int Evaluation_Department_Number { get; set; }

    [Required]
    [StringLength(20)]
    public string Evaluation_Department_Name { get; set; } = string.Empty;

    [Required]
    public int Evaluation_Index_Number { get; set; }

    [Required]
    [StringLength(100)]
    public string Evaluation_Index_Name { get; set; } = string.Empty;

    [Required]
    public int Evaluation_Task_Number { get; set; }

    [Required]
    [StringLength(100)]
    public string Evaluation_Task_Name { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Evaluation_Lists_Remark { get; set; }
}
