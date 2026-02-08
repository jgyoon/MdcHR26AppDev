using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.EvaluationProcess;
using MdcHR26Apps.Models.User;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.TotalReport.TeamLeader;

public partial class Index(
    LoginStatusService loginStatusService,
    UrlActions urlActions,
    IProcessRepository processDbRepository,
    IUserRepository userDbRepository)
{
    // 평가순서관리
    public ProcessDb processDb { get; set; } = new ProcessDb();
    public List<ProcessDb> processDbList { get; set; } = new List<ProcessDb>();

    // 사용자 관리
    public UserDb usermodel { get; set; } = new UserDb();
    public List<UserDb> userlist { get; set; } = new List<UserDb>();

    protected override async Task OnInitializedAsync()
    {
        await CheckLogined();
        await SetData();
        await base.OnInitializedAsync();
    }

    private async Task SetData()
    {
        string? sessionUserId = loginStatusService.LoginStatus.LoginUserId;
        if (!String.IsNullOrEmpty(sessionUserId))
        {
            UserDb user = await userDbRepository.GetByUserIdAsync(sessionUserId);
            processDbList = await processDbRepository.GetByTeamLeaderIdWithTeamLeaderSubmissionAsync(user.Uid);
        }
    }

    #region + [0].[1] CheckLogined : IsloginCheck() + IsTeamLeaderCheck()
    /// <summary>
    /// 로그인체크 확인(로그인 & 부서장여부) 메서드
    /// </summary>
    /// <returns></returns>
    private async Task CheckLogined()
    {
        await Task.Delay(0);
        if (!loginStatusService.IsloginAndIsTeamLeaderCheck())
        {
            StateHasChanged();
            urlActions.MoveMainPage();
        }
    }
    #endregion
}
