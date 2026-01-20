using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MdcHR26Apps.Models.Views.v_TotalReportListDB;

/// <summary>
/// v_TotalReportListDB View Entity (읽기 전용)
/// TotalReportDb + ProcessDb + UserDb + EDepartmentDb + ERankDb 조인 뷰
/// </summary>
[Keyless]
[Table("v_TotalReportListDB")]
public class v_TotalReportListDB
{
    // TotalReportDb 필드
    public Int64 TRid { get; set; }
    public Int64 Pid { get; set; }
    public Int64 Uid { get; set; }
    public decimal Final_Score { get; set; }
    public string Final_Grade { get; set; } = string.Empty;
    public string Comments { get; set; } = string.Empty;
    public int Report_Status { get; set; }
    public DateTime Created_Date { get; set; }

    // ProcessDb 필드
    public int Process_Year { get; set; }
    public string Process_Title { get; set; } = string.Empty;
    public DateTime Process_Start_Date { get; set; }
    public DateTime Process_End_Date { get; set; }
    public int Process_Status { get; set; }

    // UserDb 필드
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;

    // EDepartmentDb 필드
    public Int64 EDepartId { get; set; }
    public string EDepartName { get; set; } = string.Empty;

    // ERankDb 필드
    public Int64 ERankId { get; set; }
    public string ERankName { get; set; } = string.Empty;
}
