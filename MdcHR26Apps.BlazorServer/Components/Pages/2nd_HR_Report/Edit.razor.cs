using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.BlazorServer.Utils;
using MdcHR26Apps.Models.EvaluationProcess;
using MdcHR26Apps.Models.EvaluationReport;
using MdcHR26Apps.Models.Views.v_ReportTaskListDB;
using MdcHR26Apps.Models.EvaluationTasks;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages._2nd_HR_Report
{
    public partial class Edit
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

        // 기타        
        public string resultText { get; set; } = String.Empty;

        #region + 평가점수관련 변수모음 - 부서장(팀장)
        // 평가점수 결과1(일정준수)
        public double resultEvaluation_1 { get; set; } = 0;
        // 평가점수 결과2(업무수행도)
        public double resultEvaluation_2 { get; set; } = 0;
        // 평가점수 결과3(결과물)
        public double resultEvaluation_3 { get; set; } = 0;

        // 평가점수1(일정준수) - 평가대상자
        public double User_Evaluation_1 { get; set; } = 0;
        // 평가점수2(업무수행도) - 평가대상자
        public double User_Evaluation_2 { get; set; } = 0;
        // 평가점수3(결과물) - 평가대상자
        public double User_Evaluation_3 { get; set; } = 0;

        // 평가점수1(일정준수) - 부서장(팀장)
        public double TeamLeader_Evaluation_1 { get; set; } = 0;
        // 평가점수2(업무수행도) - 부서장(팀장)
        public double TeamLeader_Evaluation_2 { get; set; } = 0;
        // 평가점수3(결과물) - 부서장(팀장)
        public double TeamLeader_Evaluation_3 { get; set; } = 0;
        #endregion

        // 세부직무
        [Inject]
        public ITasksRepository tasksDbRepository { get; set; } = null!;
        public List<TasksDb> tasklist { get; set; } = new List<TasksDb>();
        public TasksDb tasks { get; set; } = new TasksDb();
        public int taskCount { get; set; } = 0;
        // 테이블 CSS Style 
        public string table_style_2 = "text-align: center; vertical-align: middle;";

        // 세부업무 펼쳐보기
        public bool TaskCollapsed { get; set; } = false;


        // 펼쳐보기-1(평가대상자)
        public bool reportCollapsed_1 { get; set; } = true;

        // 공통함수 호출
        public TaskUtils taskUtils = new TaskUtils();

        // 세무업무표 
        [Inject]
        public Iv_ReportTaskListRepository v_ReportTaskListDBRepository { get; set; } = null!;
        public List<v_ReportTaskListDB> v_ReportTaskLists { get; set; } = new List<v_ReportTaskListDB>();

        protected override async Task OnInitializedAsync()
        {
            await CheckLogined();
            await SetData(Id);
            await base.OnInitializedAsync();
        }

        private async Task SetData(Int64 Id)
        {
            model = await reportDbRepository.GetByIdAsync(Id);

            tasklist = await tasksDbRepository.GetByListNoAllAsync(model.Task_Number);
            if (tasklist.Count > 0)
            {
                CountTask();
            }

            if (model != null)
            {
                processDb = await processDbRepository.GetByUidAsync(model.Uid);
                Pid = processDb.Pid;

                #region + 평가대상자 평가점수 입력
                User_Evaluation_1 = model.User_Evaluation_1;
                User_Evaluation_2 = model.User_Evaluation_2;
                User_Evaluation_3 = model.User_Evaluation_3;
                #endregion

                #region + 부서장(팀장) 평가점수 입력
                TeamLeader_Evaluation_1 = model.TeamLeader_Evaluation_1;
                TeamLeader_Evaluation_2 = model.TeamLeader_Evaluation_2;
                TeamLeader_Evaluation_3 = model.TeamLeader_Evaluation_3;
                #endregion

                #region + 평가 반영값 계산 - 부서장(팀장)
                //resultEvaluation_1 = result_Evaluation(model.User_Evaluation_1, model.TeamLeader_Evaluation_1);
                //resultEvaluation_2 = result_Evaluation(model.User_Evaluation_2, model.TeamLeader_Evaluation_2);
                //resultEvaluation_3 = result_Evaluation(model.User_Evaluation_3, model.TeamLeader_Evaluation_3);
                #endregion

                #region + 세부업무표
                v_ReportTaskLists = await v_ReportTaskListDBRepository.GetByTaksListNumberAllAsync(model.Task_Number);
                #endregion
            }
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

        #region + 팀장평가입력 : TeamLeaderUpdateReport
        private async Task TeamLeaderUpdateReport()
        {
            #region + [0] 평가기본정보
            // [02] 사용자 계정 : model.UserId
            // [03] 사용자이름 : model.UserName
            // [04] Report_Item_Number : model.Report_Item_Number
            // [05] Report_Item_Name_1(지표분류명) model.Report_Item_Name_1
            // [06] Report_Item_Name_2(직무분류명) model.Report_Item_Name_2
            // [07] Report_Item_Proportion(직무 %) model.Report_Item_Proportion
            // [08] Report_SubItem_Name(세부직무명) model.Report_SubItem_Name
            // [09] Report_Item_Proportion(세부직무 %) model.Report_SubItem_Proportion
            #endregion

            #region + [1] 평가대상자 평가
            // [10] User_Evaluation_1(일정준수) model.User_Evaluation_1
            // [11] User_Evaluation_2(업무수행도) model.User_Evaluation_2
            // [12] User_Evaluation_3(결과물) model.public double User_Evaluation_3
            // [13] User_Evaluation_4(comment) model.User_Evaluation_4
            #endregion

            #region + [2] 부서장(팀장) 평가
            // [14] TeamLeader_Evaluation_1(일정준수) model.TeamLeader_Evaluation_1
            // [15] TeamLeader_Evaluation_2(업무수행도) model.TeamLeader_Evaluation_2
            // [16] TeamLeader_Evaluation_3(결과물) model.TeamLeader_Evaluation_3
            // [17] TeamLeader_Evaluation_4(comment) model.TeamLeader_Evaluation_4
            #endregion

            #region + [3] 임원 평가
            // [18] Director_Evaluation_1(일정준수) model.Director_Evaluation_1
            // [19] Director_Evaluation_2(업무수행도) model.Director_Evaluation_2
            // [20] Director_Evaluation_3(결과물) model.Director_Evaluation_3
            // [21] Director_Evaluation_4(comment) model.Director_Evaluation_4
            #endregion

            // [23] Total_Score(종합점수) model.Total_Score

            #region + 100 점 초과 방지
            model.TeamLeader_Evaluation_1 = model.TeamLeader_Evaluation_1 >= 100 ? 100 : model.TeamLeader_Evaluation_1;
            model.TeamLeader_Evaluation_2 = model.TeamLeader_Evaluation_2 >= 100 ? 100 : model.TeamLeader_Evaluation_2;
            model.TeamLeader_Evaluation_3 = model.TeamLeader_Evaluation_3 >= 100 ? 100 : model.TeamLeader_Evaluation_3;
            #endregion

            // 평가시 평가Comment에 평가일 입력
            model.TeamLeader_Evaluation_4 = "평가일 : " + DateTime.Now.ToString("yyyy-MM-dd");

            // 평가가 입력되지 않았을 경우 관련하여 문구를 출력함
            if (!String.IsNullOrEmpty(model.TeamLeader_Evaluation_4))
            {
                int affectedRows = await reportDbRepository.UpdateAsync(model);
                if (affectedRows > 0)
                {
                    resultText = "평가 작성에 성공하였습니다.";
                    StateHasChanged();
                    // 팀장평가상세페이지 이동
                    Move2ndDeteilsPage();
                }
                else
                {
                    resultText = "평가 작성에 실패하였습니다.";
                }
            }
            else
            {
                resultText = "평가 작성를 위한 점수가 누락되었습니다.";
            }
        }
        #endregion

        #region + [6].[2] Move2ndDeteilsPage : 팀장평가상세페이지 이동
        public void Move2ndDeteilsPage()
        {
            urlActions.Move2ndDeteilsPage(Pid);
        }
        #endregion

        // 확인 후 삭제
        #region + 평가수치를 %로 표시
        /// <summary>
        /// 평가수치를 %로 표시하는 메서드
        /// </summary>
        /// <param name="Value">double Value</param>
        /// <returns>평가수치를 %로 표시하는 String</returns>
        private string ConvertScore(double Value)
        {
            int score = 0;
            switch (Value)
            {
                case -1:
                    score = 0;
                    break;
                case -0.5:
                    score = 50;
                    break;

                case 0:
                    score = 100;
                    break;

                case 0.5:
                    score = 150;
                    break;

                case 1:
                    score = 200;
                    break;

                default:
                    break;
            }

            return score.ToString();
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
        #endregion

        /// <summary>
        /// 결과점수를 계산하는 메서드
        /// </summary>
        /// <param name="UserEvaluation"></param>
        /// <param name="TeamLeaderEvaluation"></param>
        /// <returns></returns>
        private double result_Evaluation(double UserEvaluation, double TeamLeaderEvaluation)
        {
            if (UserEvaluation != 0 && TeamLeaderEvaluation != 0)
            {
                //return UserEvaluation * (TeamLeaderEvaluation / 100);
                return Math.Round(UserEvaluation * (TeamLeaderEvaluation / 100), 2);
            }
            else
            {
                return 0;
            }
        }

        #region + 평가점수를 계산하는 메서드 모음 - 부서장(팀장)
        /// <summary>
        /// 평가점수를 계산하는 메서드1
        /// </summary>
        /// <param name="e"></param>
        private void SetResultEvaluation1(ChangeEventArgs e)
        {
            double changeValue = Convert.ToDouble(e.Value != null ? e.Value : 0);
            TeamLeader_Evaluation_1 = changeValue;
            resultEvaluation_1 = result_Evaluation(User_Evaluation_1, TeamLeader_Evaluation_1);
        }

        /// <summary>
        /// 평가점수를 계산하는 메서드2
        /// </summary>
        /// <param name="e"></param>
        private void SetResultEvaluation2(ChangeEventArgs e)
        {
            double changeValue = Convert.ToDouble(e.Value != null ? e.Value : 0);
            TeamLeader_Evaluation_2 = changeValue;
            resultEvaluation_2 = result_Evaluation(User_Evaluation_2, TeamLeader_Evaluation_2);
        }

        /// <summary>
        /// 평가점수를 계산하는 메서드3
        /// </summary>
        /// <param name="e"></param>
        private void SetResultEvaluation3(ChangeEventArgs e)
        {
            double changeValue = Convert.ToDouble(e.Value != null ? e.Value : 0);
            TeamLeader_Evaluation_3 = changeValue;
            resultEvaluation_3 = result_Evaluation(User_Evaluation_3, TeamLeader_Evaluation_3);
        }
        #endregion

        #region + Todolist
        private void CountTask()
        {
            taskCount = tasklist.Count;
        }

        private void TaskToggle()
        {
            TaskCollapsed = !TaskCollapsed;
        }
        #endregion
    }
}
