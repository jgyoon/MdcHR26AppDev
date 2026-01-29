using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MdcHR26Apps.Models.Views.v_TotalReportListDB;

/// <summary>
/// v_TotalReportListDB View Entity (읽기 전용)
/// TotalReportDb + UserDb 조인 뷰
/// </summary>
[Keyless]
[Table("v_TotalReportListDB")]
public class v_TotalReportListDB
{
    // TotalReportDb 필드 (A)
    public Int64 TRid { get; set; }
    public Int64 Uid { get; set; }

    // UserDb 필드 (B)
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;

    // [1] 평가대상자 평가 (User_Evaluation)
    public double User_Evaluation_1 { get; set; }
    public double User_Evaluation_2 { get; set; }
    public double User_Evaluation_3 { get; set; }
    public string User_Evaluation_4 { get; set; } = string.Empty;

    // [2] 팀장 평가 (TeamLeader_Evaluation)
    public double TeamLeader_Evaluation_1 { get; set; }
    public double TeamLeader_Evaluation_2 { get; set; }
    public double TeamLeader_Evaluation_3 { get; set; }
    public string TeamLeader_Comment { get; set; } = string.Empty;

    // [3] 피드백 (Feedback_Evaluation)
    public double Feedback_Evaluation_1 { get; set; }
    public double Feedback_Evaluation_2 { get; set; }
    public double Feedback_Evaluation_3 { get; set; }
    public string Feedback_Comment { get; set; } = string.Empty;

    // [4] 임원 평가 (Director_Evaluation)
    public double Director_Evaluation_1 { get; set; }
    public double Director_Evaluation_2 { get; set; }
    public double Director_Evaluation_3 { get; set; }
    public string Director_Comment { get; set; } = string.Empty;

    // [5] 종합 점수
    public double Total_Score { get; set; }
    public double Director_Score { get; set; }
    public double TeamLeader_Score { get; set; }
}
