using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.EvaluationProcess;

/// <summary>
/// 평가 프로세스 Entity
/// 평가 단계별 워크플로우 상태 관리
/// </summary>
[Table("ProcessDb")]
public class ProcessDb
{
    /// <summary>
    /// Process ID (PK, IDENTITY)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 Pid { get; set; }

    /// <summary>
    /// 평가 대상자 ID (FK → UserDb.Uid)
    /// </summary>
    [Required]
    public Int64 Uid { get; set; }

    /// <summary>
    /// 부서장 ID (FK → UserDb.Uid)
    /// </summary>
    public Int64? TeamLeaderId { get; set; }

    /// <summary>
    /// 임원 ID (FK → UserDb.Uid)
    /// </summary>
    public Int64? DirectorId { get; set; }

    /// <summary>
    /// 직무평가 합의 요청 여부
    /// </summary>
    [Required]
    public bool Is_Request { get; set; } = false;

    /// <summary>
    /// 직무평가 합의 여부
    /// </summary>
    [Required]
    public bool Is_Agreement { get; set; } = false;

    /// <summary>
    /// 직무평가 합의 코멘트
    /// </summary>
    public string? Agreement_Comment { get; set; }

    /// <summary>
    /// 하위(세부) 직무평가 합의 요청 여부
    /// </summary>
    [Required]
    public bool Is_SubRequest { get; set; } = false;

    /// <summary>
    /// 하위(세부) 직무평가 합의 여부
    /// </summary>
    [Required]
    public bool Is_SubAgreement { get; set; } = false;

    /// <summary>
    /// 하위(세부) 직무평가 합의 코멘트
    /// </summary>
    public string? SubAgreement_Comment { get; set; }

    /// <summary>
    /// 사용자 평가 제출 여부
    /// </summary>
    [Required]
    public bool Is_User_Submission { get; set; } = false;

    /// <summary>
    /// 부서장 평가 제출 여부
    /// </summary>
    [Required]
    public bool Is_Teamleader_Submission { get; set; } = false;

    /// <summary>
    /// 임원 평가 제출 여부
    /// </summary>
    [Required]
    public bool Is_Director_Submission { get; set; } = false;

    /// <summary>
    /// 피드백 여부
    /// </summary>
    [Required]
    public bool FeedBackStatus { get; set; } = false;

    /// <summary>
    /// 평가자 피드백 승인 여부
    /// </summary>
    [Required]
    public bool FeedBack_Submission { get; set; } = false;

    // Navigation Properties
    [ForeignKey("Uid")]
    public User.UserDb? User { get; set; }

    [ForeignKey("TeamLeaderId")]
    public User.UserDb? TeamLeader { get; set; }

    [ForeignKey("DirectorId")]
    public User.UserDb? Director { get; set; }
    public long UserId { get; set; }
}
