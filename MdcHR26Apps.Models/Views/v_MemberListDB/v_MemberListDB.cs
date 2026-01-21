using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MdcHR26Apps.Models.Views.v_MemberListDB;

/// <summary>
/// v_MemberListDB View Entity (읽기 전용)
/// UserDb + EDepartmentDb + ERankDb 조인 뷰
/// </summary>
[Keyless]
[Table("v_MemberListDB")]
public class v_MemberListDB
{
    public Int64 Uid { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string ENumber { get; set; } = string.Empty;
    public string ERank { get; set; } = string.Empty;
    public Int64 EDepartId { get; set; }
    public string EDepartmentName { get; set; } = string.Empty;
    public bool ActivateStatus { get; set; }
    public bool IsTeamLeader { get; set; }
    public bool IsDirector { get; set; }
    public bool IsAdministrator { get; set; }
    public bool IsDeptObjectiveWriter { get; set; }
}
