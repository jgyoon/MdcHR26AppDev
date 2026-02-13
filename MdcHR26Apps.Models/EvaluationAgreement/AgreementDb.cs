using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.EvaluationAgreement;

[Table("AgreementDb")]
public class AgreementDb
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 Aid { get; set; }

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

    // Navigation Property
    [ForeignKey("Uid")]
    public User.UserDb? User { get; set; }
}
