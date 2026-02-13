using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.Result;

/// <summary>
/// 종합 평가 결과 Entity
/// </summary>
[Table("TotalReportDb")]
public class TotalReportDb
{
    /// <summary>
    /// TotalReport ID (PK, IDENTITY)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 TRid { get; set; }

    /// <summary>
    /// 평가 대상자 ID (FK → UserDb.Uid)
    /// </summary>
    [Required]
    public Int64 Uid { get; set; }

    // === 평가대상자 자가평가 ===

    /// <summary>
    /// 사용자 평가 - 일정준수
    /// </summary>
    [Required]
    public double User_Evaluation_1 { get; set; }

    /// <summary>
    /// 사용자 평가 - 업무 수행의 양
    /// </summary>
    [Required]
    public double User_Evaluation_2 { get; set; }

    /// <summary>
    /// 사용자 평가 - 결과물
    /// </summary>
    [Required]
    public double User_Evaluation_3 { get; set; }

    /// <summary>
    /// 사용자 평가 - 코멘트
    /// </summary>
    public string? User_Evaluation_4 { get; set; }

    // === 부서장(팀장) 평가 ===

    /// <summary>
    /// 팀장 평가 - 일정준수
    /// </summary>
    [Required]
    public double TeamLeader_Evaluation_1 { get; set; }

    /// <summary>
    /// 팀장 평가 - 업무 수행의 양
    /// </summary>
    [Required]
    public double TeamLeader_Evaluation_2 { get; set; }

    /// <summary>
    /// 팀장 평가 - 결과물
    /// </summary>
    [Required]
    public double TeamLeader_Evaluation_3 { get; set; }

    /// <summary>
    /// 팀장 평가 - 코멘트
    /// </summary>
    public string? TeamLeader_Comment { get; set; }

    // === 1차 피드백 면담 ===

    /// <summary>
    /// 피드백 - 일정준수
    /// </summary>
    [Required]
    public double Feedback_Evaluation_1 { get; set; }

    /// <summary>
    /// 피드백 - 업무 수행의 양
    /// </summary>
    [Required]
    public double Feedback_Evaluation_2 { get; set; }

    /// <summary>
    /// 피드백 - 결과물
    /// </summary>
    [Required]
    public double Feedback_Evaluation_3 { get; set; }

    /// <summary>
    /// 피드백 - 코멘트
    /// </summary>
    public string? Feedback_Comment { get; set; }

    // === 임원 평가 ===

    /// <summary>
    /// 임원 평가 - 일정준수
    /// </summary>
    [Required]
    public double Director_Evaluation_1 { get; set; }

    /// <summary>
    /// 임원 평가 - 업무 수행의 양
    /// </summary>
    [Required]
    public double Director_Evaluation_2 { get; set; }

    /// <summary>
    /// 임원 평가 - 결과물
    /// </summary>
    [Required]
    public double Director_Evaluation_3 { get; set; }

    /// <summary>
    /// 임원 평가 - 코멘트
    /// </summary>
    public string? Director_Comment { get; set; }

    // === 종합 점수 ===

    /// <summary>
    /// 종합 점수
    /// </summary>
    [Required]
    public double Total_Score { get; set; }

    /// <summary>
    /// 임원 점수
    /// </summary>
    [Required]
    public double Director_Score { get; set; }

    /// <summary>
    /// 팀장 점수
    /// </summary>
    [Required]
    public double TeamLeader_Score { get; set; } = 0;

    // Navigation Property
    [ForeignKey("Uid")]
    public User.UserDb? User { get; set; }
}
