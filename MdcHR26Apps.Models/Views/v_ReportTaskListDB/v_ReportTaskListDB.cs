using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MdcHR26Apps.Models.Views.v_ReportTaskListDB;

/// <summary>
/// v_ReportTaskListDB View Entity (읽기 전용)
/// ProcessDb + ReportDb + TasksDb + UserDb 조인 뷰
/// 2026년 DB 구조 기반
/// </summary>
[Keyless]
[Table("v_ReportTaskListDB")]
public class v_ReportTaskListDB
{
    // ProcessDb 필드
    public Int64 Pid { get; set; }

    // ReportDb 필드
    public Int64 Rid { get; set; }
    public Int64 Uid { get; set; }
    public int Report_Item_Number { get; set; }
    public string Report_Item_Name_1 { get; set; } = string.Empty;
    public string Report_Item_Name_2 { get; set; } = string.Empty;
    public int Report_Item_Proportion { get; set; }
    public string Report_SubItem_Name { get; set; } = string.Empty;
    public int Report_SubItem_Proportion { get; set; }
    public Int64 Task_Number { get; set; }

    // ReportDb 평가 점수
    public double User_Evaluation_1 { get; set; }
    public double User_Evaluation_2 { get; set; }
    public double User_Evaluation_3 { get; set; }
    public string User_Evaluation_4 { get; set; } = string.Empty;
    public double TeamLeader_Evaluation_1 { get; set; }
    public double TeamLeader_Evaluation_2 { get; set; }
    public double TeamLeader_Evaluation_3 { get; set; }
    public string TeamLeader_Evaluation_4 { get; set; } = string.Empty;
    public double Director_Evaluation_1 { get; set; }
    public double Director_Evaluation_2 { get; set; }
    public double Director_Evaluation_3 { get; set; }
    public string Director_Evaluation_4 { get; set; } = string.Empty;
    public double Total_Score { get; set; }

    // UserDb 필드
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;

    // TasksDb 필드
    public Int64 Tid { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public Int64 TaksListNumber { get; set; }  // 오타 그대로 유지 (DB 컬럼명)
    public int TaskStatus { get; set; }
    public string TaskObjective { get; set; } = string.Empty;
    public int TargetProportion { get; set; }
    public int ResultProportion { get; set; }
    public DateTime TargetDate { get; set; }
    public DateTime ResultDate { get; set; }
    public double Task_Eval_1 { get; set; }  // DB View 별칭 (TasksDb.Task_Evaluation_1)
    public double Task_Eval_2 { get; set; }  // DB View 별칭 (TasksDb.Task_Evaluation_2)
    public double TaskLevel { get; set; }
    public string TaskComments { get; set; } = string.Empty;
}
