using MdcHR26Apps.Models.Views.v_AgreementDB;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Agreement;

public partial class AgreementDetailsTableList
{
    [Parameter]
    public List<v_AgreementDB> agreements { get; set; } = new List<v_AgreementDB>();

    // 아이템 순번
    public int sortNo = 1;

    /// <summary>
    /// 아이템 순번 메서드
    /// </summary>
    /// <param name="sort"></param>
    /// <returns></returns>
    private int sortNoAdd2(int sort)
    {
        if (agreements.Count == sortNo)
        {
            sortNo = 1;
            return sort;
        }
        sortNo = sort + 1;
        return sort;
    }
}
