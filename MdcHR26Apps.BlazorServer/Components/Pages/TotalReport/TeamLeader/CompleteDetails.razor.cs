using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.EvaluationProcess;
using MdcHR26Apps.Models.EvaluationReport;
using MdcHR26Apps.Models.Views.v_ProcessTRListDB;
using MdcHR26Apps.Models.Views.v_ReportTaskListDB;
using MdcHR26Apps.Models.Views.v_TotalReportListDB;
using MdcHR26Apps.Models.User;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.TotalReport.TeamLeader;

public partial class CompleteDetails(
    LoginStatusService loginStatusService,
    UrlActions urlActions,
    Iv_ProcessTRListRepository processTRRepository,
    IReportRepository reportDbRepository,
    Iv_ReportTaskListRepository v_ReportTaskListDBRepository,
    Iv_TotalReportListRepository v_TotalReportListDBRepository,
    IUserRepository userDbRepository)
{
    #region Parameters
    [Parameter]
    public Int64 Id { get; set; }
    #endregion

    // 평가순서관리 (v_ProcessTRListDB 사용)
    public v_ProcessTRListDB processTR { get; set; } = new v_ProcessTRListDB();

    // 평가리스트 작성
    public ReportDb report { get; set; } = new ReportDb();
    public List<ReportDb> model { get; set; } = new List<ReportDb>();

    // 세무업무표
    public List<v_ReportTaskListDB> v_ReportTaskLists { get; set; } = new List<v_ReportTaskListDB>();

    // 기타
    public string userName { get; set; } = String.Empty;

    #region + TotalReport관련
    public v_TotalReportListDB totalReportViewModel { get; set; } = new v_TotalReportListDB();

    // 사용자 정보
    public UserDb userDb { get; set; } = new UserDb();

    // 펼쳐보기 - 기본값은 비활성화
    public bool UserReportCollapsed { get; set; } = false;
    public bool TeamLeaderReportCollapsed { get; set; } = false;
    public bool FeedBackReportCollapsed { get; set; } = true;

    #endregion

    protected override async Task OnInitializedAsync()
    {
        await CheckLogined();
        await SetData(Id);
        await base.OnInitializedAsync();
    }

    #region + [0].[1] CheckLogined : IsloginCheck() + IsTeamLeaderCheck()
    /// <summary>
    /// 로그인 체크 && 부서장 체크
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

    private async Task SetData(Int64 id)
    {
        // v_ProcessTRListDB 한 번 조회로 모든 정보 획득
        processTR = await processTRRepository.GetByPidAsync(id) ?? new v_ProcessTRListDB();
        userName = processTR.UserName;

        // ReportDb 목록 조회
        model = await reportDbRepository.GetByUidAllAsync(processTR.Uid);

        // 세무업무표 조회
        v_ReportTaskLists = await v_ReportTaskListDBRepository.GetByUidAllAsync(processTR.Uid);

        #region + TotalReport 관련
        // v_ProcessTRListDB에 이미 TotalReport 정보가 포함되어 있으므로
        // 별도 조회 불필요하지만, 상세 정보가 필요하면 조회
        totalReportViewModel = (await v_TotalReportListDBRepository.GetByUserIdAsync(processTR.Uid)).FirstOrDefault() ?? new v_TotalReportListDB();
        #endregion
    }

    private void MoveCompleteTeamLeaderMainPage()
    {
        urlActions.MoveTeamLeaderTotalReportIndexPage();
    }
}
