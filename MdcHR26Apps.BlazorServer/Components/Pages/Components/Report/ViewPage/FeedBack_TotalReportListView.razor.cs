using MdcHR26Apps.BlazorServer.Utils;
using MdcHR26Apps.Models.Views.v_TotalReportListDB;
using Microsoft.AspNetCore.Components;
using System.Text.RegularExpressions;
using System.Web;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Report.ViewPage;

public partial class FeedBack_TotalReportListView
{
    [Parameter]
    public v_TotalReportListDB TotalReportListDB { get; set; } = new v_TotalReportListDB();

    // 공용함수 호출
    public ScoreUtils scoreUtils = new ScoreUtils();

    public double totalScore = 0;

    public double totalScore_user = 0;
    public double totalScore_teamleader = 0;
    public double totalScore_feedback = 0;
    public double totalScore_Director = 0;

    public double Score_1 = 0;
    public double Score_2 = 0;
    public double Score_3 = 0;

    public double Score_user_1 = 0;
    public double Score_user_2 = 0;
    public double Score_user_3 = 0;
    public double Score_teamleader_1 = 0;
    public double Score_teamleader_2 = 0;
    public double Score_teamleader_3 = 0;
    public double Score_feedback_1 = 0;
    public double Score_feedback_2 = 0;
    public double Score_feedback_3 = 0;
    public double Score_Director_1 = 0;
    public double Score_Director_2 = 0;
    public double Score_Director_3 = 0;

    // 평가비중
    // 평가자 : 20%
    // 부서장(팀장) : 80%
    public double userScoreProportion = 0.2;
    public double teamLeaderScoreProportion = 0.8;

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

        Score_user_1 = TotalReportListDB.User_Evaluation_1;
        Score_user_2 = TotalReportListDB.User_Evaluation_2;
        Score_user_3 = TotalReportListDB.User_Evaluation_3;
        Score_teamleader_1 = TotalReportListDB.TeamLeader_Evaluation_1;
        Score_teamleader_2 = TotalReportListDB.TeamLeader_Evaluation_2;
        Score_teamleader_3 = TotalReportListDB.TeamLeader_Evaluation_3;
        Score_feedback_1 = TotalReportListDB.Feedback_Evaluation_1;
        Score_feedback_2 = TotalReportListDB.Feedback_Evaluation_2;
        Score_feedback_3 = TotalReportListDB.Feedback_Evaluation_3;

        // 변경 후
        Score_1 =
            (Score_user_1 * userScoreProportion) +
            ((Score_teamleader_1 + Score_feedback_1) * teamLeaderScoreProportion);
        Score_2 =
            (Score_user_2 * userScoreProportion) +
            ((Score_teamleader_2 + Score_feedback_2) * teamLeaderScoreProportion);
        Score_3 =
            (Score_user_3 * userScoreProportion) +
            ((Score_teamleader_3 + Score_feedback_3) * teamLeaderScoreProportion);
        // 소숫점 2자리까지 표시
        Score_1 = Math.Round(Score_1, 2);
        Score_2 = Math.Round(Score_2, 2);
        Score_3 = Math.Round(Score_3, 2);

        totalScore = Score_1 + Score_2 + Score_3;

        totalScore_user = Score_user_1 + Score_user_2 + Score_user_3;
        totalScore_teamleader = Score_teamleader_1 + Score_teamleader_2 + Score_teamleader_3;
        totalScore_feedback = Score_feedback_1 + Score_feedback_2 + Score_feedback_3;
        totalScore_Director = Score_Director_1 + Score_Director_2 + Score_Director_3;

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
