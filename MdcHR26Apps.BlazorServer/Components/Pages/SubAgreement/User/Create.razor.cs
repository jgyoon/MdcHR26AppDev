using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.BlazorServer.Models;
using MdcHR26Apps.BlazorServer.Utils;
using MdcHR26Apps.Models.EvaluationAgreement;
using MdcHR26Apps.Models.Common;
using MdcHR26Apps.Models.EvaluationLists;
using MdcHR26Apps.Models.EvaluationSubAgreement;
using MdcHR26Apps.Models.EvaluationTasks;
using MdcHR26Apps.BlazorServer.Models;
using MdcHR26Apps.BlazorServer.Utils;
using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace MdcHR26Apps.BlazorServer.Components.Pages.SubAgreement.User
{
    public partial class Create
    {
        // 로그인관리(상태관리)
        [Inject]
        public LoginStatusService loginStatusService { get; set; } = null!;

        // 페이지이동
        [Inject]
        public UrlActions urlActions { get; set; } = null!;

        // 평가리스트 관리
        [Inject]
        public IEvaluationListsRepository ListsRepository { get; set; } = null!;
        public EvaluationLists listsmodel { get; set; } = new EvaluationLists();
        public List<SelectListModel> list0 { get; set; } = new List<SelectListModel>();
        public List<SelectListModel> list1 { get; set; } = new List<SelectListModel>();
        public List<SelectListModel> list2 { get; set; } = new List<SelectListModel>();
        // 지표분류리스트(세부평가리스트)
        public List<SelectListModel> list3 { get; set; } = new List<SelectListModel>();

        // 세부직무합의관리
        [Inject]
        public ISubAgreementRepository subAgreementDbRepository { get; set; } = null!;
        public SubAgreementDb model { get; set; } = new SubAgreementDb();
        public List<SubAgreementDb> subAgreementDblist { get; set; } = new List<SubAgreementDb>();

        // 직무합의관리
        [Inject]
        public IAgreementRepository agreementDbRepository { get; set; } = null!;
        public List<AgreementDb> agreementDblist { get; set; } = new List<AgreementDb>();
        public List<AgreementDb> aDblist1 { get; set; } = new List<AgreementDb>();
        public List<AgreementDb> aDblist2 { get; set; } = new List<AgreementDb>();


        // 세부업무관리
        [Inject]
        public ITasksRepository tasksDbRepository { get; set; } = null!;
        public List<TasksDb> tasklist { get; set; } = new List<TasksDb>();
        public TasksDb tasks { get; set; } = new TasksDb();
        public int taskNumber { get; set; } = 0;
        public int taskCount { get; set; } = 0;

        // 세부업무 펼쳐보기
        public bool TaskCollapsed { get; set; } = false;
        public bool TaskCreateCollapsed { get; set; } = true;

        // 기타
        public bool list1_Stauts { get; set; } = false;
        public bool list2_Stauts { get; set; } = false;
        public bool list3_Stauts { get; set; } = false;
        public string? DeptName { get; set; } = string.Empty;
        public string? IndexName { get; set; } = string.Empty;

        //public bool list3_Stauts { get; set; } = false;
        //public string? TaskContent { get; set; } = string.Empty;

        public int itemNumber { get; set; } = 1;
        public int peroportion { get; set; } = 0;
        public int maxperoportion { get; set; } = 100;

        // 기타
        public string resultText { get; set; } = String.Empty;

        public string addTaskErrorText { get; set; } = String.Empty;

        // 테이블 CSS Style
        public string table_style_2 = "text-align: center; vertical-align: middle;";

        // 공용함수 호출
        public TaskUtils taskutils = new TaskUtils();
        public List<TaskLevelModel> taskLevels = new List<TaskLevelModel>();
        [Inject]
        public UserUtils utils { get; set; } = null!;
        public bool isUnLimitedTask { get; set; } = false;


        // 설정파일 호출
        [Inject]
        public IConfiguration Config { get; set; } = null!;
        // 설정년도
        public int titleYear = 2026;

        protected override async Task OnInitializedAsync()
        {
            await CheckLogined();
            await SetData();
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

        private async Task SetData()
        {
            //list1 = await ListsRepository.GetByDeptAllAsync();
            long sessionUid = loginStatusService.LoginStatus.LoginUid;
            if (sessionUid > 0)
            {
                subAgreementDblist = await subAgreementDbRepository.GetByUidAllAsync(sessionUid);
                agreementDblist = await agreementDbRepository.GetByUidAllAsync(sessionUid);
                // https://stackoverflow.com/questions/19406242/select-distinct-using-linq
                aDblist1 = agreementDblist.GroupBy(e => e.Report_Item_Name_1).Select(grp => grp.First()).ToList();
                foreach (var agreement in aDblist1)
                {
                    list0.Add(new SelectListModel
                    {
                        SelectListNumber = (int)agreement.Aid,
                        SelectListName = agreement.Report_Item_Name_1,
                        Text = agreement.Report_Item_Name_1,
                        Value = agreement.Report_Item_Name_1
                    });
                }
                list1 = list0;
            }

            // 업무 수 제한 해제여부 확인
            if (!String.IsNullOrEmpty(loginStatusService.LoginStatus.LoginUserEDepartment))
            {
                isUnLimitedTask = utils.GetIsUnLimitedTask(loginStatusService.LoginStatus.LoginUserEDepartment);
            }

            itemNumber = GetitemNumber(subAgreementDblist);

            TasksInit();

            taskLevels = taskutils.GetTaskLevels();
        }

        /// <summary>
        /// itemNubmer를 구하는 메서드
        /// </summary>
        /// <param name="lists">List<SubAgreementDb> lists</param>
        /// <returns>lists.Count + 1</returns>
        private int GetitemNumber(List<SubAgreementDb> lists)
        {
            if (lists.Count > 0)
            {
                return lists.Count + 1;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// 직무내역리스트를 설정하는 메서드
        /// </summary>
        /// <param name="e">Deptment</param>
        /// <returns>list2 설정</returns>
        private async Task SetList2(string value)
        {
            await Task.Delay(0);

            model.Report_Item_Name_1 = value;

            if (!string.IsNullOrEmpty(model.Report_Item_Name_1))
            {
                list2 = new List<SelectListModel>();
                aDblist2 = agreementDblist.Where(e => e.Report_Item_Name_1.Equals(model.Report_Item_Name_1)).ToList();
                foreach (var agreement in aDblist2)
                {
                    list2.Add(new SelectListModel
                    {
                        SelectListNumber = (int)agreement.Aid,
                        SelectListName = agreement.Report_Item_Name_2,
                        Text = agreement.Report_Item_Name_2,
                        Value = agreement.Report_Item_Name_2
                    });
                }

                model.Report_Item_Name_2 = String.Empty;
                list3 = new List<SelectListModel>();
                model.Report_SubItem_Name = String.Empty;
                peroportion = 0;
                //StateHasChanged();
            }
        }

        /// <summary>
        /// 직무내역리스트를 설정하는 메서드
        /// </summary>
        /// <param name="e">Deptment</param>
        /// <returns>list2 설정</returns>
        private async Task SetList3(string value)
        {
            model.Report_Item_Name_2 = value;

            if (!string.IsNullOrEmpty(model.Report_Item_Name_1) && !string.IsNullOrEmpty(model.Report_Item_Name_2))
            {
                list3 = await ListsRepository.GetByTasksAsync(model.Report_Item_Name_1, model.Report_Item_Name_2);
                maxperoportion = GetMaxVaule(GetSubPeroportion(subAgreementDblist, model.Report_Item_Name_1, model.Report_Item_Name_2));
                model.Report_SubItem_Name = String.Empty;
                peroportion = 0;
            }
        }

        private void SetReportSubItemName(string value)
        {
            model.Report_SubItem_Name = value;
        }

        private void HandlePeroportionChanged(int newValue)
        {
            // 여기에서 newValue를 사용하여 필요한 로직을 수행
            peroportion = newValue;
        }

        private void DeleteTask(Int64 taskId)
        {
            DeleteTodo(taskId);
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

        /// <summary>
        /// 사용가능한 비중을 구하는 메서드
        /// </summary>
        /// <param name="lists">List<SubAgreementDb> lists</param>
        /// <returns>현재 설정된 비중 외 메서드</returns>
        private int GetMaxVaule(int sumVaule)
        {
            int defalutVaule = 100;

            return defalutVaule - sumVaule;
        }

        #region + 직무별 세부직무 총합을 구하기(2)
        /// <summary>
        /// 직무별 세부직무 총합을 구하기 메서드(2)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ItemName_1"></param>
        /// <param name="ItemName_2"></param>
        /// <returns></returns>
        private int GetSubPeroportion(List<SubAgreementDb> model, string ItemName_1, string ItemName_2)
        {
            int resultVaule = 0;

            if (model.Count != 0)
            {
                List<SubAgreementDb> lists = (List<SubAgreementDb>)model.Where(e => e.Report_Item_Name_1.Equals(ItemName_1) && e.Report_Item_Name_2.Equals(ItemName_2)).ToList();
                if (lists.Count != 0)
                {
                    foreach (var item in lists)
                    {
                        resultVaule = resultVaule + item.Report_SubItem_Proportion;
                    }
                }
            }
            return resultVaule;
        }
        #endregion

        #region + 직무비중을 구하기
        private async Task<int> GetAgreementDbPeroportion(long uid, string ItemName_1, string ItemName_2)
        {
            int resultVaule = 0;

            if (!String.IsNullOrEmpty(ItemName_1) && !String.IsNullOrEmpty(ItemName_2))
            {
                List<AgreementDb> lists = await agreementDbRepository.GetByTasksPeroportionAsync(uid, ItemName_1, ItemName_2);
                if (lists.Count > 0)
                {
                    resultVaule = lists[0].Report_Item_Proportion;
                }
            }

            return resultVaule;
        }
        #endregion

        #region + [9].[1] MoveUserSubAgreementMainPage : 세부직무작성 메인페이지 이동
        public void MoveUserSubAgreementMainPage()
        {
            urlActions.MoveUserSubAgreementMainPage();
        }
        #endregion

        #region + 세부직무작성 생성 : CreateUserSubAgreement
        /// <summary>
        /// 직무작성 생성
        /// </summary>
        /// <returns></returns>
        private async Task CreateUserSubAgreement()
        {
            await Task.Delay(1);

            // 작성가능한 평가비중이 없으면 평가표를 생성하지 않고 목록으로 이동
            if (maxperoportion == 0)
            {
                resultText = "평가작성에 실패했습니다.-3(설정가능한 평가비중이 없습니다.)";
                await Task.Delay(1000);
                StateHasChanged();
                urlActions.MoveUserSubAgreementMainPage();
                return;
            }

            long sessionUid = loginStatusService.LoginStatus.LoginUid;
            string? sessionUserName = loginStatusService.LoginStatus.LoginUserName;

            if (sessionUid > 0 &&
                !string.IsNullOrEmpty(sessionUserName) &&
                !string.IsNullOrEmpty(model.Report_Item_Name_1) &&
                !string.IsNullOrEmpty(model.Report_Item_Name_2) &&
                !string.IsNullOrEmpty(model.Report_SubItem_Name) &&
                peroportion != 0
                )
            {
                #region + [0] 평가기본정보
                // [02] 사용자 계정
                model.Uid = sessionUid;
                // [04] Report_Item_Number
                model.Report_Item_Number = itemNumber;
                // [05] Report_Item_Name_1(지표분류명)
                //model.Report_Item_Name_1 = !string.IsNullOrEmpty(DeptName) ? DeptName : string.Empty;
                // [06] Report_Item_Name_2(직무분류명)
                //model.Report_Item_Name_2 = !string.IsNullOrEmpty(IndexName) ? IndexName : string.Empty;
                // [07] Report_Item_Proportion(직무 %)
                model.Report_Item_Proportion = await GetAgreementDbPeroportion(model.Uid, model.Report_Item_Name_1, model.Report_Item_Name_2);
                //if (!string.IsNullOrEmpty(DeptName) && !string.IsNullOrEmpty(IndexName))
                //{
                //    model.Report_Item_Proportion = await GetAgreementDbPeroportion(model.Uid, DeptName, IndexName);
                //}
                // [08] Report_SubItem_Name(세부직무명)
                //model.Report_SubItem_Name = !String.IsNullOrEmpty(model.Report_SubItem_Name) ? model.Report_SubItem_Name : string.Empty;
                // [09] Report_Item_Proportion(세부직무 %)
                model.Report_SubItem_Proportion = peroportion;
                #endregion

                if (string.IsNullOrEmpty(model.Report_Item_Name_1) ||
                    string.IsNullOrEmpty(model.Report_Item_Name_2) ||
                    string.IsNullOrEmpty(model.Report_SubItem_Name) ||
                    model.Report_SubItem_Proportion == 0 ||
                    tasklist.Count == 0
                    )
                {
                    resultText = "평가작성에 실패했습니다.-4(설정을 잘못하였습니다.)";
                    await Task.Delay(1000);
                    resultText = "다시 처음부터 설정해주세요.";
                    StateHasChanged();
                    //urlActions.MoveUserSubAgreementMainPage();
                    return;
                }
                else
                {
                    model = await subAgreementDbRepository.AddAsync(model);
                    if (model.Sid != 0)
                    {
                        // 세부업무리스트 번호 추가
                        model.Task_Number = model.Sid;
                        bool updatestatus = await subAgreementDbRepository.UpdateAsync(model);

                        if (updatestatus)
                        {
                            // 세부리스트 생성
                            await CreateTask(model.Sid);
                        }

                        resultText = "평가 생성에 성공하였습니다.";
                        StateHasChanged();
                        // 평가메인페이지 이동
                        urlActions.MoveUserSubAgreementMainPage();
                    }
                    else
                    {
                        resultText = "평가 생성에 실패했습니다.-2";
                    }
                }
            }
            else
            {
                // 실패
                resultText = "평가작성에 실패했습니다.-1";
            }
        }
        #endregion

        #region + TEST(확인 후 삭제)
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

        private List<TodoItem> todos = new();

        private int sortNoAdd(int taskNumber)
        {
            if (tasklist.Count == taskNumber)
            {
                taskNumber = 1;
                return taskNumber;
            }
            taskNumber = taskNumber + 1;
            return taskNumber;
        }


        private async Task AddTodo()
        {
            // 제목길이 제한(100자)
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
            //tasks.TargetDate = DateTime.Now;
            tasks.TargetDate = new DateTime(titleYear, 12, 31);
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
        #endregion
    }

    /// <summary>
    /// 확인 후 삭제
    /// </summary>
    public class TodoItem
    {
        public string? Title { get; set; }
        public bool IsDone { get; set; }
    }
}
