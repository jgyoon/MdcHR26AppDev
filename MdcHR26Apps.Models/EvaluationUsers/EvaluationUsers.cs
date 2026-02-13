using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.EvaluationUsers;

/// <summary>
/// 평가 참여자 Entity
/// </summary>
[Table("EvaluationUsers")]
public class EvaluationUsers
{
    /// <summary>
    /// Evaluation User ID (PK, IDENTITY)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 EUid { get; set; }

    /// <summary>
    /// 평가 대상자 ID (FK → UserDb.Uid)
    /// </summary>
    [Required]
    public Int64 Uid { get; set; }

    /// <summary>
    /// 평가자 여부
    /// </summary>
    [Required]
    public bool Is_Evaluation { get; set; } = true;

    /// <summary>
    /// 부서장 ID (FK → UserDb.Uid)
    /// </summary>
    public Int64? TeamLeaderId { get; set; }

    /// <summary>
    /// 임원 ID (FK → UserDb.Uid)
    /// </summary>
    public Int64? DirectorId { get; set; }

    /// <summary>
    /// 부서장 여부
    /// </summary>
    [Required]
    public bool Is_TeamLeader { get; set; } = false;

    // Navigation Properties
    [ForeignKey("Uid")]
    public User.UserDb? User { get; set; }

    [ForeignKey("TeamLeaderId")]
    public User.UserDb? TeamLeader { get; set; }

    [ForeignKey("DirectorId")]
    public User.UserDb? Director { get; set; }
}
