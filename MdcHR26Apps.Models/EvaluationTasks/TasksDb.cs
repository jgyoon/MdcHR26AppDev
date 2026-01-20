using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.EvaluationTasks;

/// <summary>
/// 업무/과업 관리 Entity
/// </summary>
[Table("TasksDb")]
public class TasksDb
{
    /// <summary>
    /// Task ID (PK, IDENTITY)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 Tid { get; set; }

    /// <summary>
    /// 평가 대상자 ID (FK → UserDb.Uid)
    /// </summary>
    [Required]
    public Int64 Uid { get; set; }

    /// <summary>
    /// 세부 협의서 번호 (FK → SubAgreementDb.SAid)
    /// </summary>
    [Required]
    public Int64 SubAgreement_Number { get; set; }

    /// <summary>
    /// 업무명
    /// </summary>
    [Required]
    [StringLength(255)]
    public string Task_Name { get; set; } = string.Empty;

    /// <summary>
    /// 업무 내용
    /// </summary>
    public string? Task_Content { get; set; }

    /// <summary>
    /// 목표 달성 기준
    /// </summary>
    public string? Task_Criteria { get; set; }

    /// <summary>
    /// 예상 소요 시간 (시간 단위)
    /// </summary>
    public int? Estimated_Hours { get; set; }

    /// <summary>
    /// 완료 여부
    /// </summary>
    [Required]
    public bool Is_Completed { get; set; } = false;

    // Navigation Properties
    [ForeignKey("Uid")]
    public User.UserDb? User { get; set; }

    [ForeignKey("SubAgreement_Number")]
    public EvaluationSubAgreement.SubAgreementDb? SubAgreement { get; set; }
}
