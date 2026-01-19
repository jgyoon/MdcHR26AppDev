using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MdcHR26Apps.Models.Views.v_DeptObjectiveListDb;

/// <summary>
/// v_DeptObjectiveListDb View Entity (읽기 전용)
/// DeptObjectiveDb + EDepartmentDb 조인 뷰
/// </summary>
[Keyless]
[Table("v_DeptObjectiveListDb")]
public class v_DeptObjectiveListDb
{
    public Int64 DOid { get; set; }
    public Int64 EDepartId { get; set; }
    public string EDepartName { get; set; } = string.Empty;
    public string Objective_Title { get; set; } = string.Empty;
    public string Objective_Description { get; set; } = string.Empty;
    public DateTime Start_Date { get; set; }
    public DateTime End_Date { get; set; }
}
