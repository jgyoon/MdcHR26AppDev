using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MdcHR26Apps.Models.Views.v_ReportTaskListDB;

/// <summary>
/// v_ReportTaskListDB View Entity (읽기 전용)
/// ReportDb + TasksDb + SubAgreementDb 조인 뷰
/// </summary>
[Keyless]
[Table("v_ReportTaskListDB")]
public class v_ReportTaskListDB
{
    // ReportDb 필드
    public Int64 Rid { get; set; }
    public Int64 Pid { get; set; }
    public Int64 Uid { get; set; }
    public decimal Self_Score { get; set; }
    public decimal Evaluator_Score { get; set; }
    public decimal Final_Score { get; set; }
    public string Self_Comments { get; set; } = string.Empty;
    public string Evaluator_Comments { get; set; } = string.Empty;
    public int Report_Status { get; set; }

    // TasksDb 필드
    public Int64 Tid { get; set; }
    public Int64 SubAgreement_Number { get; set; }
    public string Task_Title { get; set; } = string.Empty;
    public string Task_Description { get; set; } = string.Empty;
    public DateTime Task_Start_Date { get; set; }
    public DateTime Task_End_Date { get; set; }
    public int Task_Status { get; set; }
    public decimal Task_Achievement_Rate { get; set; }

    // SubAgreementDb 필드
    public Int64 SAid { get; set; }
    public string SubAgreement_Title { get; set; } = string.Empty;
    public decimal Proportion { get; set; }

    // 추가 조인 필드 (UserDb)
    public string UserName { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
}
