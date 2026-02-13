using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.BlazorServer.Models;
using MdcHR26Apps.BlazorServer.Utils;
using MdcHR26Apps.Models.EvaluationAgreement;
using MdcHR26Apps.Models.EvaluationProcess;
using MdcHR26Apps.Models.EvaluationSubAgreement;
using MdcHR26Apps.Models.EvaluationTasks;
using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace MdcHR26Apps.BlazorServer.Components.Pages.SubAgreement.User
{
    public partial class Edit
    {
        #region Parameters
        [Parameter]
        public long Sid { get; set; }
        #endregion

        // 로그인관리(상태관리)
        [Inject]
        public LoginStatusService loginStatusService { get; set; } = null!;

        // 페이지이동
        [Inject]
        public UrlActions urlActions { get; set; } = null!;

        //// 직무합의관리
        //[Inject]
        //public IAgreementRepository agreementDbRepository { get; set; } = null!;
        //public AgreementDb model { get; set; } = new AgreementDb();
        //public List<AgreementDb> agreementDblist { get; set; } = new List<AgreementDb>();

        // 세부직무합의관리
        [Inject]
        public ISubAgreementRepository subAgreementDbRepository { get; set; } = null!;
        public SubAgreementDb model { get; set; } = new SubAgreementDb();
        public List<SubAgreementDb> subAgreementDblist { get; set; } = new List<SubAgreementDb>();

        // 세부업무관리
        [Inject]
        public ITasksRepository tasksDbRepository { get; set; } = null!;
        public List<TasksDb> tasklist { get; set; } = new List<TasksDb>();
        public List<TasksDb> oldtasklist { get; set; } = new List<TasksDb>();
        public TasksDb tasks { get; set; } = new TasksDb();
        public int taskNumber { get; set; } = 1;
        public int taskCount { get; set; } = 0;

        // 세부업무 펼쳐보기
        public bool TaskCollapsed { get; set; } = false;
        public bool TaskCreateCollapsed { get; set; } = false;

        // 평가순서관리
        [Inject]
        public IProcessRepository processDbRepository { get; set; } = null!;
        public ProcessDb processDb { get; set; } = new ProcessDb();

        // 기타
        public string resultText { get; set; } = String.Empty;
        public int maxperoportion { get; set; } = 0;
        public string addTaskErrorText { get; set; } = String.Empty;

        // 합의요청 신청 여부
        public bool IsRequest { get; set; } = false;
        // 합의요청 승인 여부
        public bool IsAgreement { get; set; } = false;

        // 테이블 CSS Style
        public string table_style_2 = "text-align: center; vertical-align: middle;";

        // 공용함수 호출
        public TaskUtils taskutils = new TaskUtils();
        public List<TaskLevelModel> taskLevels = new List<TaskLevelModel>();
        [Inject]
        public UserUtils utils { get; set; } = null!;
        public bool isUnLimitedTask { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            await CheckLogined();
            await SetData(Sid);
            await base.OnInitializedAsync();
        }

        #region + CheckLogined : 로그인 체크
        /// <summary>
        /// 로그인 체크
        /// </summary>
        /// <returns></returns>
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

        private async Task SetData(long sid)
        {
            model = await subAgreementDbRepository.GetByIdAsync(sid);
            if (model != null)
            {
                long sessionUid = loginStatusService.LoginStatus.LoginUid;
                if (sessionUid > 0)
                {
                    var subAgreementDb =
                        await subAgreementDbRepository.GetByUidAndItemNamesAllAsync(sessionUid,
                        model.Report_Item_Name_1, model.Report_Item_Name_2);

                    if (subAgreementDb != null && subAgreementDb.Sid > 0)
                    {
                        subAgreementDblist = new List<SubAgreementDb> { subAgreementDb };
                    }

                    maxperoportion = model.Report_SubItem_Proportion;
                    if (subAgreementDblist != null && subAgreementDblist.Count > 0)
                    {
                        maxperoportion += GetMaxVaule(subAgreementDblist);
                    }

                }

                // 업무 수 제한 해제여부 확인
                if (!String.IsNullOrEmpty(loginStatusService.LoginStatus.LoginUserEDepartment))
                {
                    isUnLimitedTask = utils.GetIsUnLimitedTask(loginStatusService.LoginStatus.LoginUserEDepartment);
                }

                // 세부직무 불러오기
                //tasklist = await tasksDbRepository.GetByListNoAllAsync(model.Task_Number);
                //oldtasklist = tasklist;
                oldtasklist = await tasksDbRepository.GetByListNoAllAsync(model.Task_Number);
                tasklist = oldtasklist;
                CountTask();

                taskLevels = taskutils.GetTaskLevels();

                TasksInit();
            }
        }

        /// <summary>
        /// 사용가능한 비중을 구하는 메서드
        /// </summary>
        /// <param name="lists">List<SubAgreementDb> lists</param>
        /// <returns>현재 설정된 비중 외 메서드</returns>
        private int GetMaxVaule(List<SubAgreementDb> lists)
        {
            int defalutVaule = 100;
            if (lists != null && lists.Count != 0)
            {
                foreach (var item in lists)
                {
                    defalutVaule = defalutVaule - item.Report_SubItem_Proportion;
                }
            }

            return defalutVaule;
        }

        #region + [9].[1] MoveUserSubAgreementMainPage : 세부직무작성 메인페이지 이동
        public void MoveUserSubAgreementMainPage()
        {
            urlActions.MoveUserSubAgreementMainPage();
        }
        #endregion

        #region + 세부직무수정 : EditUserSubAgreement
        private async Task EditUserSubAgreement()
        {
            if (tasklist.Count == 0)
            {
                resultText = "세부직무가 없습니다.";
                return;
            }

            #region + 평가기본정보
            // [02] 사용자 계정 : model.Uid
            // [03] 사용자이름 : model.UserName
            // [04] Report_Item_Number : model.Report_Item_Number
            // [05] Report_Item_Name_1(지표분류명) model.Report_Item_Name_1
            // [06] Report_Item_Name_2(직무분류명) model.Report_Item_Name_2
            // [07] Report_Item_Proportion(직무 %) model.Report_Item_Proportion
            // [08] Report_SubItem_Name(세부직무명) model.Report_SubItem_Name
            // [09] Report_Item_Proportion(세부직무 %) model.Report_SubItem_Proportion
            #endregion

            if (await subAgreementDbRepository.UpdateAsync(model))
            {
                resultText = "평가 수정에 성공하였습니다.";

                // 세부직무작성

                await DeleteTask(oldtasklist);
                await CreateTask(model.Sid);

                StateHasChanged();
                // 평가메인페이지 이동
                urlActions.MoveUserSubAgreementMainPage();
            }
            else
            {
                resultText = "평가 수정에 실패하였습니다.";
            }
        }
        #endregion

        private void HandlePeroportionChanged(int newValue)
        {
            // 여기에서 newValue를 사용하여 필요한 로직을 수행
            model.Report_SubItem_Proportion = newValue;
        }

        #region + Todolist
        private async Task CreateTask(Int64 Tid)
        {
            // 2026-12-31를 기준등록일로 설정
            string resultDateString = "2026-12-31";
            CultureInfo provider = new CultureInfo("ko-KR");
            DateTime resultDate = DateTime.ParseExact(resultDateString, "yyyy-MM-dd", provider);

            if (tasklist.Count > 0)
            {
                foreach (var task in tasklist)
                {
                    #region + taskDb Model
                    // [01] Tasks id
                    // task.Tid
                    // [02] Task Name(업무명)
                    // task.TaskName
                    // [03] Taks List Number(업무 리스트 번호)
                    task.TaksListNumber = Tid;
                    // [04] Task Status(업무 상태)
                    // 0 : 진행중
                    // 1 : 종료
                    // 2 : 보류
                    // 3 : 취소
                    task.TaskStatus = 0;
                    // [05] Task Objective(업무 목표)
                    // task.TaskObjective
                    // [06] Target Proportion(목표 달성도)
                    // task.TargetProportion
                    // [07] Result Proportion(결과 달성도)
                    task.ResultProportion = 0;
                    // [08] Target Date(목표달성일자)
                    // task.TargetDate
                    // [09] Result Date(결과달성일자)
                    task.ResultDate = resultDate;
                    // [10] Task_Evaluation_1(일정준수)
                    // public double Task_Evaluation_1
                    // [11] Task_Evaluation_2(업무수행도)
                    // public double Task_Evaluation_2
                    // [12] Task Level(업무수준-난이도)
                    // S : 1.2
                    // A : 1.0(기본값)
                    // B : 0.8
                    // C : 0.6
                    // task.TaskLevel(업무수준-난이도)
                    // [13] Task Comments(업무 코멘트)
                    task.TaskComments = String.Empty;
                    #endregion
                }

                foreach (var task in tasklist)
                {
                    await tasksDbRepository.AddAsync(task);
                }
            }
        }

        private async Task DeleteTask(List<TasksDb> oldtasklist)
        {
            if (oldtasklist.Count > 0)
            {
                foreach (var task in oldtasklist)
                {
                    await tasksDbRepository.DeleteAsync(task.Tid);
                }
            }
        }

        private List<TodoItem> todos = new();

        private int sortNoAdd(int taskNumber)
        {
            taskNumber = taskNumber + 1;
            return taskNumber;
        }

        private async Task AddTodo()
        {
            // 예외조건
            //1.업무명
            //  - 작성이 되지 않은 경우
            //  - 100자 이상인 경우
            //2.업무목표
            //  - 작성이 되지 않은 경우
            //3.계획달성도
            //  - 0 또는 100 이상 경우
            //4.업무난이도
            //  - 미설정

            if (tasks.TaskName == null || tasks.TaskName == String.Empty || tasks.TaskName.Length > 100)
            {
                addTaskErrorText = "업무명이 없거나 너무 길게 작성되었습니다.";
                //tasks = new TasksDb();
                await AddTodoInit();
                TasksInit();
            }
            else if (tasks.TaskObjective == null || tasks.TaskObjective == String.Empty)
            {
                addTaskErrorText = "업무목표가 작성이 되지 않았습니다.";
                //tasks = new TasksDb();
                await AddTodoInit();
                TasksInit();
            }
            else if (tasks.TargetProportion == 0 || tasks.TargetProportion > 100)
            {
                addTaskErrorText = "계획달성도가 잘 못 작성되었습니다.";
                //tasks = new TasksDb();
                await AddTodoInit();
                TasksInit();
            }
            else if (tasks.TaskLevel == 0)
            {
                addTaskErrorText = "업무난이도가 설정되지 않았습니다.";
                //tasks = new TasksDb();
                await AddTodoInit();
                TasksInit();
            }
            else
            {
                Int64 itemNo = tasklist.Count + 1;

                tasklist.Add(new TasksDb()
                {
                    Tid = itemNo,
                    TaskName = tasks.TaskName,
                    TaskObjective = tasks.TaskObjective,
                    TargetProportion = tasks.TargetProportion,
                    TargetDate = tasks.TargetDate,
                    TaskLevel = tasks.TaskLevel
                }
                );

                //tasks = new TasksDb();
                //tasks.TargetDate = DateTime.Now;
                TasksInit();
                // Todo: Add the todo

                TaskCollapsed = false;

                CountTask();
            }
        }

        private string LeveltoName(double levelno)
        {
            string levelString = String.Empty;

            switch (levelno)
            {
                case 1.2:
                    levelString = "S";
                    break;
                case 1:
                    levelString = "A";
                    break;
                case 0.8:
                    levelString = "B";
                    break;
                case 0.6:
                    levelString = "C";
                    break;
                default:
                    break;
            }

            return levelString;
        }

        private async Task AddTodoInit()
        {
            await Task.Delay(3000);
            addTaskErrorText = String.Empty;
        }

        private void DeleteTodo(Int64 Tid)
        {
            List<TasksDb> removelist = tasklist.Where(e => e.Tid.Equals(Tid)).ToList();
            tasklist = tasklist.Except(removelist).ToList();
            CountTask();
        }
        private void TasksInit()
        {
            tasks = new TasksDb();

            // 2026-12-31를 기준등록일로 설정
            tasks.TargetDate = new DateTime(2026, 12, 31);
        }

        private void CountTask()
        {
            taskCount = tasklist.Count;
        }

        private void TaskToggle()
        {
            TaskCollapsed = !TaskCollapsed;
        }

        private void TaskCreateToggle()
        {
            TaskCreateCollapsed = !TaskCreateCollapsed;
        }

        private async Task HandleValidSubmit(TasksDb tasks)
        {

            if (IsTaskValidation(tasks))
            {
                Int64 itemNo = tasklist.Count + 1;

                tasklist.Add(new TasksDb()
                {
                    Tid = itemNo,
                    TaskName = tasks.TaskName,
                    TaskObjective = tasks.TaskObjective,
                    TargetProportion = tasks.TargetProportion,
                    TargetDate = tasks.TargetDate,
                    TaskLevel = tasks.TaskLevel
                }
                );
                TasksInit();

                TaskCollapsed = false;

                CountTask();
            }
            else
            {
                await AddTodoInit();
                TasksInit();
            }
        }

        private bool IsTaskValidation(TasksDb tasks)
        {
            bool taskValidation = false;
            // 제목길이 제한(100자)
            if (tasks.TaskName == null || tasks.TaskName == String.Empty || tasks.TaskName.Length > 100)
            {
                addTaskErrorText = "업무명이 없거나 너무 길게 작성되었습니다.";
                taskValidation = false;
            }
            else if (tasks.TaskObjective == null || tasks.TaskObjective == String.Empty)
            {
                addTaskErrorText = "업무목표가 작성이 되지 않았습니다.";
                taskValidation = false;
            }
            else if (tasks.TargetProportion == 0 || tasks.TargetProportion > 100)
            {
                addTaskErrorText = "계획달성도가 잘 못 작성되었습니다.";
                taskValidation = false;
            }
            else if (tasks.TaskLevel == 0)
            {
                addTaskErrorText = "업무난이도가 설정되지 않았습니다.";
                taskValidation = false;
            }
            else
            {
                taskValidation = true;
            }
            return taskValidation;
        }

        #endregion

    }
}
