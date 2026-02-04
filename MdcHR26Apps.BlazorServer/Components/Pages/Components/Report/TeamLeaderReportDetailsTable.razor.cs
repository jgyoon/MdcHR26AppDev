using MdcHR26Apps.Models.EvaluationReport;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Report;

public partial class TeamLeaderReportDetailsTable
{
    #region Parameters
    [Parameter] public ReportDb? report { get; set; }
    [Parameter] public EventCallback OnSave { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }
    #endregion
}
