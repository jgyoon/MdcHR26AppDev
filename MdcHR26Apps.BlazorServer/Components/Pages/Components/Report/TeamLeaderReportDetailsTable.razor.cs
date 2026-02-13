using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.EvaluationReport;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Report;

public partial class TeamLeaderReportDetailsTable
{
    [Parameter]
    public List<ReportDb> reports { get; set; } = new List<ReportDb>();

    // 페이지 이동
    [Inject]
    public UrlActions urlActions { get; set; } = null!;

    private string htmlString(string content)
    {
        return content.Replace(". ", "<br>");
    }

    private void ReportEditAction(Int64 Rid)
    {
        urlActions.Move2ndEditPage(Rid);
    }
}
