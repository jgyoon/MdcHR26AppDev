using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MdcHR26Apps.Models.Views.v_EvaluationUsersList;

/// <summary>
/// v_EvaluationUsersList View Entity (읽기 전용)
/// EvaluationUsers + UserDb + 부서장/임원 정보 조인 뷰
/// </summary>
[Keyless]
[Table("v_EvaluationUsersList")]
public class v_EvaluationUsersList
{
    public Int64 EUid { get; set; }
    public Int64 Uid { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string ENumber { get; set; } = string.Empty;
    public Int64 EDepartId { get; set; }
    public string EDepartmentName { get; set; } = string.Empty;
    public string ERank { get; set; } = string.Empty;
    public bool Is_Evaluation { get; set; }
    public Int64? TeamLeaderId { get; set; }
    public string? TeamLeaderName { get; set; }
    public Int64? DirectorId { get; set; }
    public string? DirectorName { get; set; }
    public bool Is_TeamLeader { get; set; }
}
