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
    public string UserEmail { get; set; } = string.Empty;
    public Int64 EDepartId { get; set; }
    public string EDepartName { get; set; } = string.Empty;
    public Int64 ERankId { get; set; }
    public string ERankName { get; set; } = string.Empty;
    public int EStatus { get; set; }
}
