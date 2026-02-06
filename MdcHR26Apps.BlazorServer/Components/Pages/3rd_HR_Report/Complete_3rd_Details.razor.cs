using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.EvaluationProcess;
using MdcHR26Apps.Models.EvaluationReport;
using MdcHR26Apps.Models.Views.v_TotalReportListDB;
using MdcHR26Apps.Models.Views.v_ReportTaskListDB;
using MdcHR26Apps.Models.User;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages._3rd_HR_Report
{
    public partial class Complete_3rd_Details
    {
        #region Parameters
        [Parameter]
        public long Id { get; set; }
        #endregion

        // 로그인관리(상태관리)
        [Inject]
        public LoginStatusService loginStatusService { get; set; } = null!;

        // 페이지이동
        [Inject]
        public UrlActions urlActions { get; set; } = null!;

        // 평가순서관리
        [Inject]
        public IProcessRepository processDbRepository { get; set; } = null!;
        public ProcessDb processDb { get; set; } = new ProcessDb();
        public List<ProcessDb> processDbList { get; set; } = new List<ProcessDb>();


        // 평가리스트 작성
        [Inject]
        public IReportRepository reportDbRepository { get; set; } = null!;
        public ReportDb report { get; set; } = new ReportDb();
        public List<ReportDb> model { get; set; } = new List<ReportDb>();

        // 세무업무표
        [Inject]
        public Iv_ReportTaskListRepository v_ReportTaskListDBRepository { get; set; } = null!;
        public List<v_ReportTaskListDB> v_ReportTaskLists { get; set; } = new List<v_ReportTaskListDB>();

        // 기타
        public string userName { get; set; } = String.Empty;

        #region + TotalReport관련
        [Inject]
        public Iv_TotalReportListRepository v_TotalReportListDBRepository { get; set; } = null!;
        public v_TotalReportListDB totalReportViewModel { get; set; } = new v_TotalReportListDB();

        // 사용자 정보
        [Inject]
        public IUserRepository userDbRepository { get; set; } = null!;
        public UserDb userDb { get; set; } = new UserDb();

        // 펼쳐보기 - 기본값은 비활성화
        public bool UserReportCollapsed { get; set; } = false;
        public bool TeamLeaderReportCollapsed { get; set; } = false;
        public bool FeedBackReportCollapsed { get; set; } = false;
        public bool FeedBackReport2Collapsed { get; set; } = false;
        public bool DirectorReportCollapsed { get; set; } = true;
        #endregion

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await CheckLogined();
                await SetData(Id);
                StateHasChanged();
            }
        }

        #region + [0].[1] CheckLogined : IsloginCheck() + IsTeamLeaderCheck()
        /// <summary>
        /// 로그인체크 확인(로그인 & 임원여부) 메서드
        /// </summary>
        /// <returns></returns>
        private async Task CheckLogined()
        {
            await Task.Delay(0);
            if (!loginStatusService.IsloginAndIsDirectorCheck())
            {
                StateHasChanged();
                urlActions.MoveMainPage();
            }
        }
        #endregion

        private async Task SetData(long Id)
        {
            processDb = await processDbRepository.GetByIdAsync(Id);
            model = await reportDbRepository.GetByUidAllAsync(processDb.Uid);

            // 26년도: ProcessDb에 UserName 없음 -> UserDb에서 가져오기
            userDb = await userDbRepository.GetByIdAsync(processDb.Uid);
            userName = userDb.UserName;

            #region + TotalReport 관련
            var totalReportViewList = await v_TotalReportListDBRepository.GetByUserIdAsync(userDb.Uid);
            totalReportViewModel = totalReportViewList.FirstOrDefault() ?? new v_TotalReportListDB();
            #endregion
        }

        #region + Toggle
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

        private void FeedBackReport2CollapsedToggle()
        {
            FeedBackReport2Collapsed = !FeedBackReport2Collapsed;
        }

        private void DirectorReportCollapsedToggle()
        {
            DirectorReportCollapsed = !DirectorReportCollapsed;
        }
        #endregion

        private void Move3rdMainPage()
        {
            urlActions.Move3rdMainPage();
        }
    }
}
