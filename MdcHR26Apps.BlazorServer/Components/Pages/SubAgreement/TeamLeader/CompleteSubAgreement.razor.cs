using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.EvaluationProcess;
using MdcHR26Apps.Models.EvaluationReport;
using MdcHR26Apps.Models.EvaluationSubAgreement;
using MdcHR26Apps.Models.User;
using MdcHR26Apps.Models.Views.v_ReportTaskListDB;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MdcHR26Apps.BlazorServer.Components.Pages.SubAgreement.TeamLeader
{
    public partial class CompleteSubAgreement
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

        // 세부직무합의
        [Inject]
        public ISubAgreementRepository subAgreementDbRepository { get; set; } = null!;
        public List<SubAgreementDb> model { get; set; } = new List<SubAgreementDb>();

        // 세무업무표
        [Inject]
        public Iv_ReportTaskListRepository v_ReportTaskListDBRepository { get; set; } = null!;
        public List<v_ReportTaskListDB> v_ReportTaskLists { get; set; } = new List<v_ReportTaskListDB>();

        // 사용자 정보
        [Inject]
        public IUserRepository userDbRepository { get; set; } = null!;

        // 세부업무내역 펼쳐보기 - 기본값은 비활성화
        public bool TaskListCollapsed { get; set; } = false;

        // 기타
        public string userName { get; set; } = String.Empty;

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

        private async Task SetData(long id)
        {
            processDb = await processDbRepository.GetByIdAsync(id) ?? new ProcessDb();
            model = await subAgreementDbRepository.GetByUidAllAsync(processDb.Uid) ?? new List<SubAgreementDb>();
            var userDb = await userDbRepository.GetByIdAsync(processDb.Uid) ?? new UserDb();
            userName = userDb.UserName;

            v_ReportTaskLists = await v_ReportTaskListDBRepository.GetByUidAllAsync(processDb.Uid) ?? new List<v_ReportTaskListDB>();
        }

        /// <summary>
        /// TaskListToggle 이벤트
        /// </summary>
        private void TaskListToggle()
        {
            TaskListCollapsed = !TaskListCollapsed;
        }

        private void MoveTeamLeaderSubAgreementMainPage()
        {
            urlActions.MoveTeamLeaderSubAgreementMainPage();
        }
    }
}
