using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.EvaluationProcess;
using MdcHR26Apps.Models.EvaluationSubAgreement;
using MdcHR26Apps.Models.EvaluationTasks;
using MdcHR26Apps.BlazorServer.Models;
using MdcHR26Apps.BlazorServer.Utils;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.SubAgreement.TeamLeader
{
    public partial class SubDetails
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

        [Inject]
        public IProcessRepository processDbRepository { get; set; } = null!;
        public ProcessDb processDb { get; set; } = new ProcessDb();

        // 세부직무합의관리
        [Inject]
        public ISubAgreementRepository subAgreementDbRepository { get; set; } = null!;
        public SubAgreementDb subAgreementmodel { get; set; } = new SubAgreementDb();


        // 세부업무관리
        [Inject]
        public ITasksRepository tasksDbRepository { get; set; } = null!;
        public List<TasksDb> model { get; set; } = new List<TasksDb>();

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
            subAgreementmodel = await subAgreementDbRepository.GetByIdAsync(id);
            long taskListNumber = subAgreementmodel.Task_Number != 0 ? subAgreementmodel.Task_Number : id;
            model = await tasksDbRepository.GetByListNoAllAsync(taskListNumber);
            if (subAgreementmodel != null)
            {
                processDb = await processDbRepository.GetByUidAsync(subAgreementmodel.Uid);
            }
        }

        #region + [10].[2] MoveTeamLeaderSubAgreementDetailsPage : 부서장 세부직무합의 메인페이지 이동
        public void MoveTeamLeaderSubAgreementDetailsPage(long pid)
        {
            urlActions.MoveTeamLeaderSubAgreementDetailsPage(pid);
        }
        #endregion
    }
}
