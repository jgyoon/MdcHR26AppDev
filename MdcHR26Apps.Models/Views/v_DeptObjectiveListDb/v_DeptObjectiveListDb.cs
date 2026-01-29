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
    // DeptObjectiveDb 필드 (A)
    public Int64 DeptObjectiveDbId { get; set; }
    public Int64 EDepartId { get; set; }

    // EDepartmentDb 필드 (B)
    public string EDepartmentName { get; set; } = string.Empty;

    // DeptObjectiveDb 필드 (A)
    public string ObjectiveTitle { get; set; } = string.Empty;
    public string ObjectiveContents { get; set; } = string.Empty;
    public string Remarks { get; set; } = string.Empty;
}
