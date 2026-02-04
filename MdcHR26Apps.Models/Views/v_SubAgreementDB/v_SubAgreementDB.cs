using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MdcHR26Apps.Models.Views.v_SubAgreementDB;

/// <summary>
/// v_SubAgreementDB View Entity (읽기 전용)
/// SubAgreementDb + UserDb + EDepartmentDb + ERankDb 조인 뷰
/// </summary>
[Keyless]
[Table("v_SubAgreementDB")]
public class v_SubAgreementDB
{
    // SubAgreementDb 필드
    public Int64 Sid { get; set; }
    public Int64 Uid { get; set; }
    public int Report_Item_Number { get; set; }
    public string Report_Item_Name_1 { get; set; } = string.Empty;
    public string Report_Item_Name_2 { get; set; } = string.Empty;
    public int Report_Item_Proportion { get; set; }
    public string Report_SubItem_Name { get; set; } = string.Empty;
    public int Report_SubItem_Proportion { get; set; }
    public Int64 Task_Number { get; set; }

    // UserDb 필드
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string EDepartmentName { get; set; } = string.Empty;
    public string ERankName { get; set; } = string.Empty;
}
