using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MdcHR26Apps.Models.Views.v_AgreementDB;

/// <summary>
/// v_AgreementDB View Entity (읽기 전용)
/// AgreementDb + UserDb + EDepartmentDb + ERankDb 조인 뷰
/// </summary>
[Keyless]
[Table("v_AgreementDB")]
public class v_AgreementDB
{
    // AgreementDb 필드
    public Int64 Aid { get; set; }
    public Int64 Uid { get; set; }
    public int Report_Item_Number { get; set; }
    public string Report_Item_Name_1 { get; set; } = string.Empty;
    public string Report_Item_Name_2 { get; set; } = string.Empty;
    public int Report_Item_Proportion { get; set; }

    // UserDb 필드
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string EDepartmentName { get; set; } = string.Empty;
    public string ERankName { get; set; } = string.Empty;
}
