using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MdcHR26Apps.Models.Views.v_ProcessTRListDB;

/// <summary>
/// v_ProcessTRListDB View Entity (읽기 전용)
/// ProcessDb + TotalReportDb + UserDb 조인 뷰
/// </summary>
[Keyless]
[Table("v_ProcessTRListDB")]
public class v_ProcessTRListDB
{
    // ProcessDb 필드
    public Int64 Pid { get; set; }

    // UserDb 필드 (U)
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string EDepartmentName { get; set; } = string.Empty;

    // TeamLeader 정보 (TL)
    public string TeamLeader_Id { get; set; } = string.Empty;
    public string TeamLeader_Name { get; set; } = string.Empty;

    // Director 정보 (D)
    public string Director_Id { get; set; } = string.Empty;
    public string Director_Name { get; set; } = string.Empty;

    // ProcessDb 상태 필드 (A)
    public bool Is_Request { get; set; }
    public bool Is_Agreement { get; set; }
    public bool Is_SubRequest { get; set; }
    public bool Is_SubAgreement { get; set; }
    public bool Is_User_Submission { get; set; }
    public bool Is_Teamleader_Submission { get; set; }
    public bool Is_Director_Submission { get; set; }
    public bool FeedBackStatus { get; set; }
    public bool FeedBack_Submission { get; set; }

    // UserDb Uid (A)
    public Int64 Uid { get; set; }

    // TotalReportDb 필드 (C)
    public Int64 TRid { get; set; }

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
