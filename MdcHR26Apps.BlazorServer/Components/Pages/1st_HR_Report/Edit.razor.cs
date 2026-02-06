using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.BlazorServer.Utils;
using MdcHR26Apps.Models.EvaluationProcess;
using MdcHR26Apps.Models.EvaluationReport;
using MdcHR26Apps.Models.EvaluationTasks;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages._1st_HR_Report
{
    public partial class Edit
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

        // 평가리스트 작성
        [Inject]
        public IReportRepository reportDbRepository { get; set; } = null!;
        public ReportDb model { get; set; } = new ReportDb();
        // 확인 후 삭제
        public List<ReportDb> reportlist { get; set; } = new List<ReportDb>();

        // 확인 후 삭제
        // 평가순서관리
        [Inject]
        public IProcessRepository processDbRepository { get; set; } = null!;
        public ProcessDb processDb { get; set; } = new ProcessDb();

        // 기타
        public string resultText { get; set; } = String.Empty;

        // 확인 후 삭제
        public int maxperoportion { get; set; } = 0;

        // 합의요청 신청 여부
        public bool IsRequest { get; set; } = false;
        // 합의요청 승인 여부
        public bool IsAgreement { get; set; } = false;

        // 세부직무
        [Inject]
        public ITasksRepository tasksDbRepository { get; set; } = null!;
        public List<TasksDb> tasklist { get; set; } = new List<TasksDb>();
        public TasksDb tasks { get; set; } = new TasksDb();
        public int taskCount { get; set; } = 0;
        //public DateTime targetdates = DateTime.Now;
        //public DateTime resultdates = DateTime.Now;
        // 테이블 CSS Style
        public string table_style_2 = "text-align: center; vertical-align: middle;";

        // 세부업무 펼쳐보기
        public bool TaskCollapsed { get; set; } = true;

        // 계산값
        public double Calculate_User_Evaluation_1_Value { get; set; } = 0;
        public double Calculate_User_Evaluation_2_Value { get; set; } = 0;

        // 공용함수 호출
        public TaskUtils taskutils = new TaskUtils();

        protected override async Task OnInitializedAsync()
        {
            await CheckLogined();
            await SetData(Id);
            await base.OnInitializedAsync();
        }

        //protected override async Task OnAfterRenderAsync(bool firstRender)
        //{
        //    if (firstRender)
        //    {
        //        await CheckLogined();
        //        await SetData(Id);
        //        StateHasChanged();
        //    }
        //}

        private async Task SetData(long Id)
        {
            model = await reportDbRepository.GetByIdAsync(Id);

            tasklist = await tasksDbRepository.GetByListNoAllAsync(model.Task_Number);
            if (tasklist.Count > 0)
            {
                CountTask();
                if (model.User_Evaluation_3 > 0 && !String.IsNullOrEmpty(model.User_Evaluation_4))
                {
                    await CalculateAction();
                }
            }
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

        #region + 평가수정 : EditReport
        private async Task EditReport()
        {
            // 자동계산
            await CalculateAction();

            if (!String.IsNullOrEmpty(model.User_Evaluation_4) && model.User_Evaluation_4.Length >= 1000)
            {
                //model.User_Evaluation_4 = String.Empty;
                resultText = "비고는 1000자 이상 작성을 할 수 없습니다. 다시 작성해주세요.";
                return;
            }

            #region + [0] 평가기본정보
            // [02] 사용자 계정 : model.Uid
            // [03] 사용자이름 : model.UserName
            // [04] Report_Item_Number : model.Report_Item_Number
            // [05] Report_Item_Name_1(지표분류명) model.Report_Item_Name_1
            // [06] Report_Item_Name_2(직무분류명) model.Report_Item_Name_2
            // [07] Report_Item_Proportion(직무 %) model.Report_Item_Proportion
            // [08] Report_SubItem_Name(세부직무명) model.Report_SubItem_Name
            // [09] Report_Item_Proportion(세부직무 %) model.Report_SubItem_Proportion
            // [10] Task_Number(하위 업무 리스트 번호) model.Task_Number
            #endregion

            #region + [1] 평가대상자 평가
            // [11] User_Evaluation_1(일정준수) model.User_Evaluation_1
            model.User_Evaluation_1 = Calculate_User_Evaluation_1_Value;
            // [12] User_Evaluation_2(업무수행도) model.User_Evaluation_2
            model.User_Evaluation_2 = Calculate_User_Evaluation_2_Value;
            // [13] User_Evaluation_3(결과물) model.public double User_Evaluation_3
            // 만일 평가값이 100점이상이면 100으로 고정
            model.User_Evaluation_3 = model.User_Evaluation_3 >= 100 ? 100 : model.User_Evaluation_3;
            // [14] User_Evaluation_4(comment) model.User_Evaluation_4
            #endregion

            #region + [2] 부서장(팀장) 평가
            // [15] TeamLeader_Evaluation_1(일정준수) model.TeamLeader_Evaluation_1
            // [16] TeamLeader_Evaluation_2(업무수행도) model.TeamLeader_Evaluation_2
            // [17] TeamLeader_Evaluation_3(결과물) model.TeamLeader_Evaluation_3
            // [18] TeamLeader_Evaluation_4(comment) model.TeamLeader_Evaluation_4
            #endregion

            #region + [3] 임원 평가
            // [19] Director_Evaluation_1(일정준수) model.Director_Evaluation_1
            // [20] Director_Evaluation_2(업무수행도) model.Director_Evaluation_2
            // [21] Director_Evaluation_3(결과물) model.Director_Evaluation_3
            // [22] Director_Evaluation_4(comment) model.Director_Evaluation_4
            #endregion

            // [23] Total_Score(종합점수) model.Total_Score

            if (model.User_Evaluation_1 != 0 &&
                model.User_Evaluation_2 != 0 &&
                model.User_Evaluation_3 != 0 &&
                !String.IsNullOrEmpty(model.User_Evaluation_4))
            {
                int affectedRows = await reportDbRepository.UpdateAsync(model);
                if (affectedRows > 0)
                {
                    resultText = "평가 작성(수정)에 성공하였습니다.";

                    // Task update
                    await UpdateTasks(tasklist);

                    StateHasChanged();
                    // 평가메인페이지 이동
                    urlActions.MoveReportMainPage();
                }
                else
                {
                    resultText = "평가 작성(수정)에 실패하였습니다.";
                }

            }
            else
            {
                resultText = "평가작성이 되지 않았습니다.";
            }
        }
        #endregion

        #region + [4].[1] MoveReportMainPage : 본인평가페이지 이동
        public void MoveReportMainPage()
        {
            urlActions.MoveReportMainPage();
        }
        #endregion

        #region + Todolist
        private async Task UpdateTasks(List<TasksDb> tasklist)
        {
            if (tasklist.Count > 0)
            {
                foreach (var item in tasklist)
                {
                    await tasksDbRepository.UpdateAsync(item);
                }
            }
        }

        private void CountTask()
        {
            taskCount = tasklist.Count;
        }

        private void TaskToggle()
        {
            TaskCollapsed = !TaskCollapsed;
        }
        #endregion

        #region + 자동점수계산

        #region + CalculateAction
        private async Task CalculateAction()
        {
            await Calculate_User_Evaluation_1();
            await Calculate_User_Evaluation_2();
        }

        #endregion

        #region + 자동점수계산 -1 : Calculate_User_Evaluation_1
        private async Task Calculate_User_Evaluation_1()
        {
            await Task.Delay(1);
            double CalculateValue = 0;

            if (tasklist.Count > 0)
            {
                foreach (var item in tasklist)
                {
                    //CalculateValue = CalculateValue + ((item.ResultDate - item.TargetDate).Days * item.TaskLevel);
                    CalculateValue = CalculateValue + (CalculateDays(item.TargetDate, item.ResultDate) * item.TaskLevel);
                }

                //await UpdateTasks(tasklist);

                if (CalculateValue != 0)
                {
                    Calculate_User_Evaluation_1_Value = CalculateValue / tasklist.Count;
                    Calculate_User_Evaluation_1_Value = Math.Round(Calculate_User_Evaluation_1_Value, 2);
                }

                // 최고점수 초과시 최고점으로 수정(5 => 100)
                if (Calculate_User_Evaluation_1_Value >= 100)
                {
                    Calculate_User_Evaluation_1_Value = 100;
                }
            }

        }
        #endregion

        #region + 자동점수계산 -2 : Calculate_User_Evaluation_2//
        private async Task Calculate_User_Evaluation_2()
        {
            await Task.Delay(1);
            double CalculateValue = 0;

            if (tasklist.Count > 0)
            {
                foreach (var item in tasklist)
                {
                    // 초과입력시 100으로 수정
                    item.ResultProportion = item.ResultProportion > 100 ? 100 : item.ResultProportion;
                    //CalculateValue = CalculateValue + ((item.ResultProportion - item.TargetProportion) * item.TaskLevel);
                    CalculateValue = CalculateValue + (CalculateProportion(item.TargetProportion, item.ResultProportion) * item.TaskLevel);
                }

                //await UpdateTasks(tasklist);

                if (CalculateValue != 0)
                {
                    Calculate_User_Evaluation_2_Value = CalculateValue / tasklist.Count;
                    Calculate_User_Evaluation_2_Value = Math.Round(Calculate_User_Evaluation_2_Value, 2);
                }

                // 최고점수 초과시 최고점으로 수정(5 => 100)
                if (Calculate_User_Evaluation_2_Value >= 100)
                {
                    Calculate_User_Evaluation_2_Value = 100;
                }
            }
        }
        #endregion

        #region + CalculateDays
        private double CalculateDays(DateTime targetDate, DateTime resultDate)
        {
            double resultVale = (targetDate - resultDate).Days;

            if (resultVale >= 0) // 마감일 준수 0일(5 => 100) : 100 점
            {
                resultVale = 100;
            }
            else if (resultVale < 0 && resultVale >= -30) // 마감일 초과 30일 이내(5 => 100) : 90 점
            {
                resultVale = 90;
            }
            else if (resultVale < -30 && resultVale >= -60) // 마감일 초과 60일 이내(5 => 100) : 80 점
            {
                resultVale = 80;
            }
            else if (resultVale < -60 && resultVale >= -90) // 마감일 초과 90일 이내(5 => 100) : 70 점
            {
                resultVale = 70;
            }
            else if (resultVale < -90 && resultVale >= -120) // 마감일 초과 120일 이내(5 => 100) : 60 점
            {
                resultVale = 60;
            }
            else // 마감일 초과 120일 이후(5 => 100) : 50 점
            {
                resultVale = 50;
            }

            return resultVale;
        }
        #endregion

        #region + CalculateProportion
        private double CalculateProportion(int targetProportion, int resultProportion)
        {
            double resultVale = -(targetProportion - resultProportion);

            if (resultVale >= 0) // 직척도 준수 0% 이상(5 => 100) : 100 점
            {
                resultVale = 100;
            }
            else if (resultVale < 0 && resultVale >= -10) // 직척도 준수 -10% 이하(5 => 100) : 90 점
            {
                resultVale = 90;
            }
            else if (resultVale < -10 && resultVale >= -20) // 직척도 준수 -20% 이하(5 => 100) : 80 점
            {
                resultVale = 80;
            }
            else if (resultVale < -20 && resultVale >= -30) // 직척도 준수 -30% 이하(5 => 100) : 70 점
            {
                resultVale = 70;
            }
            else if (resultVale < -30 && resultVale >= -40) // 직척도 준수 -40% 이하(5 => 100) : 60 점
            {
                resultVale = 60;
            }
            else // 직척도 준수 -40% 이상(5 => 100) : 50 점
            {
                resultVale = 50;
            }

            return resultVale;
        }
        #endregion

        #endregion
    }
}
