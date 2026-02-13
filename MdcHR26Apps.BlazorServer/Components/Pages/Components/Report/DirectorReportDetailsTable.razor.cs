using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.BlazorServer.Utils;
using MdcHR26Apps.Models.EvaluationReport;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Report;

public partial class DirectorReportDetailsTable
{
    [Parameter]
    public List<ReportDb> reports { get; set; } = new List<ReportDb>();

    // 페이지 이동
    [Inject]
    public UrlActions urlActions { get; set; } = null!;

    // 테이블 CSS Style
    public string table_style_1 = "white-space: nowrap; overflow: hidden; text-overflow: ellipsis; text-align: center; vertical-align: middle;";

    // 공용함수 호출
    public ScoreUtils scoreUtils = new ScoreUtils();

    private string htmlString(string content)
    {
        return content.Replace(". ", "<br>");
    }

    private void ReportEditAction(Int64 Rid)
    {
        urlActions.MoveComplete3rdEditPage(Rid);
    }
}
