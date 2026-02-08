using MdcHR26Apps.BlazorServer.Utils;
using MdcHR26Apps.Models.Views.v_TotalReportListDB;
using Microsoft.AspNetCore.Components;
using System.Text.RegularExpressions;
using System.Web;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Report.ViewPage;

public partial class Complete_TotalReportListView
{
    [Parameter]
    public v_TotalReportListDB TotalReportListDB { get; set; } = new v_TotalReportListDB();

    // 공용함수 호출
    public ScoreUtils scoreUtils = new ScoreUtils();

    public double totalScore = 0;
    public string TeamLeader_Comment { get; set; } = String.Empty;
    public string Feedback_Comment { get; set; } = String.Empty;
    public string Director_Comment { get; set; } = String.Empty;

    // 테이블 CSS Style
    public string table_style_1 = "white-space: nowrap; overflow: hidden; text-overflow: ellipsis; text-align: center; vertical-align: middle;";
    public string table_style_2 = "overflow: hidden; text-overflow: ellipsis; text-align: left; vertical-align: middle;";

    protected override async Task OnInitializedAsync()
    {
        await SetData();
        await base.OnInitializedAsync();
    }

    public async Task SetData()
    {
        await Task.Delay(1);

        totalScore = TotalReportListDB.Director_Score;

        TeamLeader_Comment = !String.IsNullOrEmpty(TotalReportListDB.TeamLeader_Comment) ?
            TotalReportListDB.TeamLeader_Comment : String.Empty;
        Feedback_Comment = !String.IsNullOrEmpty(TotalReportListDB.Feedback_Comment) ?
            TotalReportListDB.Feedback_Comment : String.Empty;
        Director_Comment = !String.IsNullOrEmpty(TotalReportListDB.Director_Comment) ?
            TotalReportListDB.Director_Comment : String.Empty;
    }

    #region + 문자열 변경
    /// <summary>
    /// 문자열을 웹형식에 맞추어서 변환하는 메서드
    /// </summary>
    /// <param name="contenct">string</param>
    /// <returns>웹형식의 문자열</returns>
    private string replaceString(string contenct)
    {
        return Regex.Replace(HttpUtility.HtmlEncode(contenct), "\r?\n|\r", "<br />");
    }
    #endregion
}
