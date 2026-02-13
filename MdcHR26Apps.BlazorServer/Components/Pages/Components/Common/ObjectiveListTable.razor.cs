using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.Views.v_DeptObjectiveListDb;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Common;

public partial class ObjectiveListTable
{
    [Parameter]
    public List<v_DeptObjectiveListDb> objectList { get; set; } = new List<v_DeptObjectiveListDb>();
    [Parameter]
    public bool isEdit { get; set; } = false;
    [Parameter]
    public bool isMain { get; set; } = false;
    // 페이지이동
    [Inject]
    public UrlActions urlActions { get; set; } = null!;

    public int sortNo = 1;
    private int sortNoAdd(int sort)
    {
        sortNo = sort + 1;
        return sort;
    }

    private int sortNoAdd2(int sort)
    {
        if (objectList.Count == sortNo)
        {
            sortNo = 1;
            return sort;
        }
        sortNo = sort + 1;
        return sort;
    }
}
