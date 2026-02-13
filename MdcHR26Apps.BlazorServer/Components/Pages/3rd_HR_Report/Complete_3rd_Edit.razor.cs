using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.EvaluationProcess;
using MdcHR26Apps.Models.EvaluationReport;
using MdcHR26Apps.Models.Views.v_ReportTaskListDB;
using MdcHR26Apps.Models.EvaluationTasks;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages._3rd_HR_Report
{
    public partial class Complete_3rd_Edit
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
        public long Pid { get; set; } = 0;

        // 평가순서관리
        [Inject]
        public IProcessRepository processDbRepository { get; set; } = null!;
        public ProcessDb processDb { get; set; } = new ProcessDb();

        // 평가리스트 작성
        [Inject]
        public IReportRepository reportDbRepository { get; set; } = null!;
        public ReportDb model { get; set; } = new ReportDb();

        // 기타
        public string resultText { get; set; } = String.Empty;

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

        // 세부업무 펼쳐보기
        public bool TaskCollapsed { get; set; } = true;


        // 펼쳐보기-1(평가대상자)
        public bool reportCollapsed_1 { get; set; } = false;
        public bool reportCollapsed_2 { get; set; } = true;

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
            model = await reportDbRepository.GetByIdAsync(Id) ?? new ReportDb();

            tasklist = await tasksDbRepository.GetByListNoAllAsync(model.Task_Number) ?? new List<TasksDb>();
            if (tasklist.Count > 0)
            {
                CountTask();
            }

            if (model != null)
            {
                processDb = await processDbRepository.GetByUidAsync(model.Uid) ?? new ProcessDb();
                Pid = processDb.Pid;

                #region + 세부업무표
                v_ReportTaskLists = await v_ReportTaskListDBRepository.GetByTaksListNumberAllAsync(model.Task_Number) ?? new List<v_ReportTaskListDB>();
                #endregion
            }
        }

        #region + Todolist
        private void CountTask()
        {
            taskCount = tasklist.Count;
        }
        #endregion

        #region + 펼쳐보기
        /// <summary>
        /// Toggle 이벤트
        /// </summary>
        private void ReportToggle_1()
        {
            reportCollapsed_1 = !reportCollapsed_1;
        }

        private void ReportToggle_2()
        {
            reportCollapsed_2 = !reportCollapsed_2;
        }

        private void TaskToggle()
        {
            TaskCollapsed = !TaskCollapsed;
        }
        #endregion

        private void Move3ndDeteilsPage()
        {
            urlActions.Move3rdDeteilsPage(Pid);
        }
    }
}
