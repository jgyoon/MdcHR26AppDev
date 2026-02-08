using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.EvaluationProcess;
using MdcHR26Apps.Models.Views.v_TotalReportListDB;
using MdcHR26Apps.Models.User;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.TotalReport;

public partial class Result(
    LoginStatusService loginStatusService,
    UrlActions urlActions,
    Iv_TotalReportListRepository v_TotalReportListDBRepository,
    IProcessRepository processDbRepository,
    IUserRepository userDbRepository)
{
    [Parameter]
    public long Uid { get; set; }

    #region + TotalReport관련
    // TotalReport
    public v_TotalReportListDB totalReportViewModel { get; set; } = new v_TotalReportListDB();

    // 평가순서관리
    public ProcessDb processDb { get; set; } = new ProcessDb();
    public List<ProcessDb> processDbList { get; set; } = new List<ProcessDb>();

    // 사용자 정보
    public UserDb userDb { get; set; } = new UserDb();

    public bool CompleteStatus { get; set; } = false;
    #endregion

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
        // Uid 파라미터 사용
        if (Uid > 0)
        {
            #region + ProcessDb 관련
            userDb = await userDbRepository.GetByIdAsync(Uid);
            processDb = await processDbRepository.GetByUidAsync(Uid) ?? new ProcessDb();

            CompleteStatus = processDb.Is_Director_Submission;
            #endregion

            #region + TotalReport 관련
            totalReportViewModel = (await v_TotalReportListDBRepository.GetByUserIdAsync(Uid)).FirstOrDefault() ?? new v_TotalReportListDB();
            #endregion
        }
    }
}
