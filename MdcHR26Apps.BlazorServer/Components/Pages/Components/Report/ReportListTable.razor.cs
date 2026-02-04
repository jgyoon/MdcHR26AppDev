using MdcHR26Apps.Models.EvaluationReport;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Report;

public partial class ReportListTable
{
    #region Parameters
    [Parameter] public List<ReportDb> reports { get; set; } = new();
    [Parameter] public EventCallback<long> OnEdit { get; set; }
    #endregion

    #region Variables
    private int sortNo = 1;
    #endregion
}
