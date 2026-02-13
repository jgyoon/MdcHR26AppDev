using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.EvaluationProcess;
using MdcHR26Apps.Models.Views.v_ProcessTRListDB;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.SubAgreement;

public partial class SubAgreementListTable
{
    [Parameter]
    public List<v_ProcessTRListDB> processDbList { get; set; } = new List<v_ProcessTRListDB>();

    // 페이지 이동
    [Inject]
    public UrlActions urlActions { get; set; } = null!;

    [Inject]
    public AppStateService AppState { get; set; } = null!;

    // 기타
    public int sortNo = 1;

    #region + Methods
    protected override void OnParametersSet()
    {
        sortNo = 1; // 파라미터가 변경될 때마다 sortNo 리셋
        base.OnParametersSet();
    }
    #endregion

    #region + 합의요청여부
    public string IsRequestStatus(bool status)
    {
        return status ? "요청(O)" : "요청(X)";
    }
    #endregion

    #region + 합의승인여부
    private string IsAgreementStatus(bool status)
    {
        return status ? "승인(O)" : "승인(X)";
    }
    #endregion

    #region + 평가제출여부 Is_User_Submission
    private string IsReportSubmissionStatus(bool status)
    {
        return status ? "제출(O)" : "제출(X)";
    }
    #endregion

    #region + 합의진행여부 Is_SubRequestAndSubAgreement
    private string Is_SubRequestAndSubAgreementStatus(bool status1, bool status2)
    {
        bool status = status1 && status2;
        return status ? "합의(O)" : "대기중";
    }
    #endregion

    private void SubAgreementDetailsAction(Int64 Pid)
    {
        urlActions.MoveTeamLeaderSubAgreementDetailsPage(Pid);
    }

    private void SubAgreementResetAction(Int64 Pid)
    {
        urlActions.MoveTeamLeaderResetSubAgreementPage(Pid);
    }

    private void CompleteSubAgreementDetailsAction(Int64 Pid)
    {
        urlActions.MoveTeamLeaderCompleteSubAgreement(Pid);
    }
}
