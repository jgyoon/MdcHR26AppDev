using MdcHR26Apps.Models.EvaluationReport;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Report;

public partial class TeamLeader_TotalReportList
{
    #region Parameters
    [Parameter] public List<ReportDb> reports { get; set; } = new();
    #endregion

    #region Methods
    private double GetAverageScore(double eval1, double eval2, double eval3)
    {
        return Math.Round((eval1 + eval2 + eval3) / 3, 2);
    }
    #endregion
}
