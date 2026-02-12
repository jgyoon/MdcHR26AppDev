using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.BlazorServer.Utils;
using MdcHR26Apps.Models.EvaluationProcess;
using MdcHR26Apps.Models.Views.v_ProcessTRListDB;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Report.ViewPage;

public partial class TeamLeaderReportListView
{
    [Parameter]
    public List<v_ProcessTRListDB> processTRLists { get; set; } = new List<v_ProcessTRListDB>();

    // 평가순서관리
    [Inject]
    public IProcessRepository processDbRepository { get; set; } = null!;
    public ProcessDb processDb { get; set; } = new ProcessDb();
    public List<ProcessDb> processDbList { get; set; } = new List<ProcessDb>();

    // 페이지 이동
    [Inject]
    public UrlActions urlActions { get; set; } = null!;

    // 기타
    public int sortNo = 1;

    public string IsNotEvaluate { get; set; } = "평가 제출(X)";

    // 공용함수 호출
    public ScoreUtils scoreUtils = new ScoreUtils();

    protected override void OnParametersSet()
    {
        sortNo = 1;
        base.OnParametersSet();
    }

    private int sortNoAdd3(int sort)
    {
        if (processTRLists.Count == sortNo)
        {
            sortNo = 1;
            return sort;
        }
        sortNo = sort + 1;
        return sort;
    }

    #region + 평가제출여부 Is_User_Submission
    private string IsReportSubmissionStatus(bool status, double score_1, double score_2, double score_3)
    {

        if (status)
        {
            double score = score_1 + score_2 + score_3;
            score = scoreUtils.TotalScroreTo100thpercentile(score);
            return score.ToString();
        }
        else
        {
            return "제출(X)";
        }
    }
    #endregion

    #region + 팀장평가제출여부 Is_Teamleader_Submission
    private string IsTeamleaderSubmissionStatus(bool status, double teamleader_score)
    {
        if (status)
        {
            return teamleader_score.ToString();
        }
        else
        {
            return "평가(X)";
        }
    }
    #endregion

    private void TeamLeaderDetailsAction(Int64 Pid)
    {
        urlActions.Move2ndDeteilsPage(Pid);
    }

    private void CompleteDetailsAction(Int64 Pid)
    {
        urlActions.MoveCompleteDetailsPage(Pid);
    }

    #region + 평가취소 : CancelAction
    private async Task CancelAction(Int64 Pid)
    {
        processDb = await processDbRepository.GetByIdAsync(Pid) ?? new ProcessDb();

        if (processDb.Pid != 0)
        {
            processDb.Is_Teamleader_Submission = false;
            int affectedRows = await processDbRepository.UpdateAsync(processDb);
            if (affectedRows > 0)
            {
                urlActions.Move2ndMainPage();
            }
            else
            {
                processDb.Is_Teamleader_Submission = true;
            }
        }
    }
    #endregion
}
