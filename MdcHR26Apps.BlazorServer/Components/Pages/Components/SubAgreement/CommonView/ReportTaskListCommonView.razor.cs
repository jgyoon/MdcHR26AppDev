using MdcHR26Apps.Models.Views.v_ReportTaskListDB;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.SubAgreement.CommonView;

public partial class ReportTaskListCommonView
{
    #region Parameters
    [Parameter] public bool Collapsed { get; set; } = false;
    [Parameter] public EventCallback Toggle { get; set; }
    [Parameter] public List<v_ReportTaskListDB> ReportTaskListDB { get; set; } = new();
    [Parameter] public EventCallback<long> OnSelectTask { get; set; }
    #endregion

    #region Variables
    private string IconClass => Collapsed ? "oi oi-minus" : "oi oi-plus";
    #endregion
}
