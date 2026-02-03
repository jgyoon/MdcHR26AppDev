using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.EvaluationSubAgreement;

[Table("SubAgreementDb")]
public class SubAgreementDb
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 Sid { get; set; }

    [Required]
    public Int64 Uid { get; set; }

    [Required]
    public int Report_Item_Number { get; set; }

    [Required]
    public string Report_Item_Name_1 { get; set; } = string.Empty;

    [Required]
    public string Report_Item_Name_2 { get; set; } = string.Empty;

    [Required]
    public int Report_Item_Proportion { get; set; }

    [Required]
    public string Report_SubItem_Name { get; set; } = string.Empty;

    [Required]
    public int Report_SubItem_Proportion { get; set; }

    [Required]
    public Int64 Task_Number { get; set; }

    // Navigation Property
    [ForeignKey("Uid")]
    public User.UserDb? User { get; set; }
}
