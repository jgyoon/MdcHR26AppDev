using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.BlazorServer.Utils;
using MdcHR26Apps.Models.EvaluationProcess;
using MdcHR26Apps.Models.Views.v_TotalReportListDB;
using MdcHR26Apps.Models.User;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.TotalReport;

public partial class Index(
    LoginStatusService loginStatusService,
    UrlActions urlActions,
    Iv_TotalReportListRepository v_TotalReportListDBRepository,
    IProcessRepository processDbRepository,
    IUserRepository userDbRepository)
{
    #region + TotalReport관련
    // TotalReport
    public v_TotalReportListDB totalReportViewModel { get; set; } = new v_TotalReportListDB();

    // 평가순서관리
    public ProcessDb processDb { get; set; } = new ProcessDb();
    public List<ProcessDb> processDbList { get; set; } = new List<ProcessDb>();

    // 사용자 정보
    public UserDb userDb { get; set; } = new UserDb();

    // 펼쳐보기 - 기본값은 비활성화
    public bool UserReportCollapsed { get; set; } = false;
    public bool TeamLeaderReportCollapsed { get; set; } = false;
    public bool FeedBackReportCollapsed { get; set; } = false;
    public bool CompleteReportCollased { get; set; } = false;
    public bool FeedBackStatus { get; set; } = false;
    public bool FeedBackSubmit { get; set; } = false;
    public bool CompleteStatus { get; set; } = false;

    public bool Is_User_Submission { get; set; } = false;

    // AppState 대체: Modal 상태 관리
    public bool IsReportSubmitModalShow { get; set; } = false;
    #endregion

    public ScoreUtils scoreUtils { get; set; } = new ScoreUtils();

    // 기타
    public string resultText { get; set; } = String.Empty;

    protected override async Task OnInitializedAsync()
    {
        await CheckLogined();
        await SetData();
        await base.OnInitializedAsync();
    }

    #region + [0].[1] CheckLogined : IsloginCheck()
    private async Task CheckLogined()
    {
        await Task.Delay(0);
        if (!loginStatusService.IsloginCheck())
        {
            StateHasChanged();
            urlActions.MoveMainPage();
        }
    }
    #endregion

    private async Task SetData()
    {
        string? sessionUserId = loginStatusService.LoginStatus.LoginUserId;
        if (!String.IsNullOrEmpty(sessionUserId))
        {
            // 사용자 정보 먼저 조회
            userDb = await userDbRepository.GetByUserIdAsync(sessionUserId) ?? new UserDb();

            #region + ProcessDb 관련
            processDb = await processDbRepository.GetByUidAsync(userDb.Uid) ?? new ProcessDb();
            Is_User_Submission = processDb.Is_User_Submission;
            FeedBackStatus = processDb.FeedBackStatus;
            FeedBackSubmit = processDb.FeedBack_Submission;
            if (
                processDb.Is_User_Submission &&
                processDb.Is_Teamleader_Submission &&
                processDb.Is_Director_Submission
                )
            {
                CompleteStatus = true;
            }
            #endregion

            #region + TotalReport 관련
            totalReportViewModel = (await v_TotalReportListDBRepository.GetByUserIdAsync(userDb.Uid)).FirstOrDefault() ?? new v_TotalReportListDB();
            #endregion
        }
    }

    private void UserReportCollapsedToggle()
    {
        UserReportCollapsed = !UserReportCollapsed;
    }

    private void TeamLeaderReportCollapsedToggle()
    {
        TeamLeaderReportCollapsed = !TeamLeaderReportCollapsed;
    }

    private void FeedBackReportCollapsedToggle()
    {
        FeedBackReportCollapsed = !FeedBackReportCollapsed;
    }

    private void CompleteReportCollasedToggle()
    {
        CompleteReportCollased = !CompleteReportCollased;
    }

    #region + Report제출
    private async Task ReportSubmit()
    {
        await Task.Delay(5);
        IsReportSubmitModalShow = true;
        StateHasChanged();
    }
    #endregion

    #region + Report 취소
    private async Task ReportCencel()
    {
        processDb.FeedBackStatus = false;
        int affectedRows = await processDbRepository.UpdateAsync(processDb);

        if (affectedRows > 0)
        {
            resultText = "재평가 요청이 되었습니다.(피드백 취소)";
        }
        else
        {
            resultText = "재평가 요청이 실패했습니다.";
        }
    }
    #endregion
}
