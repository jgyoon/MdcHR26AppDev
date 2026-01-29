using MdcHR26Apps.Models.Views.v_MemberListDB;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Table;

public partial class MemberListTable
{
    [Parameter]
    public List<v_MemberListDB>? Members { get; set; }

    public int sortNo = 1;

    private int sortNoAdd2(int sort)
    {
        if (Members != null && Members.Count == sortNo)
        {
            sortNo = 1;
            return sort;
        }
        sortNo = sort + 1;
        return sort;
    }
}
