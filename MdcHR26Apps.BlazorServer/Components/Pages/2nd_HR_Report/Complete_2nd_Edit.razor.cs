using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.EvaluationProcess;
using MdcHR26Apps.Models.EvaluationReport;
using MdcHR26Apps.Models.Views.v_ReportTaskListDB;
using MdcHR26Apps.Models.EvaluationTasks;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages._2nd_HR_Report
{
    public partial class Complete_2nd_Edit
    {
        #region Parameters
        [Parameter]
        public Int64 Id { get; set; }
        #endregion

        // 로그인관리(상태관리)
        [Inject]
        public LoginStatusService loginStatusService { get; set; } = null!;

        // 페이지이동
        [Inject]
        public UrlActions urlActions { get; set; } = null!;
        public Int64 Pid { get; set; } = 0;

        // 평가순서관리
        [Inject]
        public IProcessRepository processDbRepository { get; set; } = null!;
        public ProcessDb processDb { get; set; } = new ProcessDb();

        // 평가리스트 작성
        [Inject]
        public IReportRepository reportDbRepository { get; set; } = null!;
        public ReportDb model { get; set; } = new ReportDb();

        // 세부직무
        [Inject]
        public ITasksRepository tasksDbRepository { get; set; } = null!;
        public List<TasksDb> tasklist { get; set; } = new List<TasksDb>();
        public TasksDb tasks { get; set; } = new TasksDb();
        public int taskCount { get; set; } = 0;

        // 세무업무표 
        [Inject]
        public Iv_ReportTaskListRepository v_ReportTaskListDBRepository { get; set; } = null!;
        public List<v_ReportTaskListDB> v_ReportTaskLists { get; set; } = new List<v_ReportTaskListDB>();


        // 펼쳐보기-1
        public bool reportCollapsed_1 { get; set; } = false;

        // 세부업무 펼쳐보기
        public bool TaskCollapsed { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            await CheckLogined();
            await SetData(Id);
            await base.OnInitializedAsync();
        }

        #region + [1] CheckLogined : IsloginCheck() + IsTeamLeaderCheck()
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

        private async Task SetData(Int64 Id)
        {
            model = await reportDbRepository.GetByIdAsync(Id);

            processDb = await processDbRepository.GetByUidAsync(model.Uid);

            tasklist = await tasksDbRepository.GetByListNoAllAsync(model.Task_Number);

            v_ReportTaskLists = await v_ReportTaskListDBRepository.GetByUidAllAsync(model.Uid);
        }

        #region + 펼쳐보기
        /// <summary>
        /// Toggle 이벤트
        /// </summary>
        private void ReportToggle_1()
        {
            reportCollapsed_1 = !reportCollapsed_1;
        }
        #endregion

        private void TaskToggle()
        {
            TaskCollapsed = !TaskCollapsed;
        }

        private void MoveCompleteDetailsPage()
        {
            urlActions.MoveCompleteDetailsPage(processDb.Pid);
        }

    }
}
