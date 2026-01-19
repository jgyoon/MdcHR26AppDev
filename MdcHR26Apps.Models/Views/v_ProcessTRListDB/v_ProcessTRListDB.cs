using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MdcHR26Apps.Models.Views.v_ProcessTRListDB;

/// <summary>
/// v_ProcessTRListDB View Entity (읽기 전용)
/// ProcessDb + TotalReportDb 조인 뷰
/// </summary>
[Keyless]
[Table("v_ProcessTRListDB")]
public class v_ProcessTRListDB
{
    // ProcessDb 필드
    public Int64 Pid { get; set; }
    public int Process_Year { get; set; }
    public string Process_Title { get; set; } = string.Empty;
    public DateTime Start_Date { get; set; }
    public DateTime End_Date { get; set; }
    public string Process_Description { get; set; } = string.Empty;
    public int Process_Status { get; set; }

    // TotalReportDb 필드
    public Int64 TRid { get; set; }
    public Int64 Uid { get; set; }
    public decimal Final_Score { get; set; }
    public string Final_Grade { get; set; } = string.Empty;
    public string Comments { get; set; } = string.Empty;
    public int Report_Status { get; set; }
    public DateTime Created_Date { get; set; }

    // 추가 조인 필드 (UserDb)
    public string UserName { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
}
