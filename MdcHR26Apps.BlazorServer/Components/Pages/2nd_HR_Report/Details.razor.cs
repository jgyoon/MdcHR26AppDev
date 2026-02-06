using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.BlazorServer.Utils;
using MdcHR26Apps.Models.EvaluationProcess;
using MdcHR26Apps.Models.EvaluationReport;
using MdcHR26Apps.Models.Views.v_TotalReportListDB;
using MdcHR26Apps.Models.Result;
using MdcHR26Apps.Models.User;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages._2nd_HR_Report
{
    public partial class Details
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

        // 평가순서관리
        [Inject]
        public IProcessRepository processDbRepository { get; set; } = null!;
        public ProcessDb processDb { get; set; } = new ProcessDb();
        public List<ProcessDb> processDbList { get; set; } = new List<ProcessDb>();

        // 평가리스트 작성
        [Inject]
        public IReportRepository reportDbRepository { get; set; } = null!;
        public List<ReportDb> model { get; set; } = new List<ReportDb>();

        // 기타
        public string userName { get; set; } = String.Empty;
        public string resultText { get; set; } = String.Empty;
        public bool Is_TeamLeader_Submission { get; set; } = false;

        #region + 평가항목 작성여부 변수
        // 부서장(팀장) 평가 작성가능 여부(제출가능 여부)
        public bool IsTeamLeader_EvaluationComplete { get; set; } = false;
        // 부서장(팀장) 평가 작성가능 여부(제출가능 여부)-2
        public bool Sub_IsTeamLeader_EvaluationComplete { get; set; } = false;

        // 일정준수 작성여부
        public bool Is_TeamLeader_Evaluation_1 { get; set; } = false;
        // 업무수행도 작성여부
        public bool Is_TeamLeader_Evaluation_2 { get; set; } = false;
        // 결과물 작성여부
        public bool Is_TeamLeader_Evaluation_3 { get; set; } = false;
        // Comment 작성여부
        public bool Is_TeamLeader_Evaluation_4 { get; set; } = false;
        #endregion

        #region + TotalReport관련
        // TotalReport
        [Inject]
        public ITotalReportRepository totalReportDbRepository { get; set; } = null!;
        public TotalReportDb totalReportModel { get; set; } = new TotalReportDb();
        [Inject]
        public Iv_TotalReportListRepository v_TotalReportListDBRepository { get; set; } = null!;
        public v_TotalReportListDB totalReportViewModel { get; set; } = new v_TotalReportListDB();

        // 사용자 정보
        [Inject]
        public IUserRepository userDbRepository { get; set; } = null!;
        public UserDb userDb { get; set; } = new UserDb();

        // 펼쳐보기 - 기본값은 비활성화
        public bool UserReportCollapsed { get; set; } = false;
        public bool TeamLeaderReportCollapsed { get; set; } = true;

        // 부서장평가 값
        public double report_TeamLeader_Evaluation_1 { get; set; } = 0;
        public double report_TeamLeader_Evaluation_2 { get; set; } = 0;
        public double report_TeamLeader_Evaluation_3 { get; set; } = 0;
        public double report_TeamLeader_Score { get; set; } = 0;
        public string report_TeamLeader_Comment { get; set; } = String.Empty;
        public string report_Feedback_Comment { get; set; } = String.Empty;

        public string update_resultText { get; set; } = String.Empty;
        public string update_resultText_1 { get; set; } = String.Empty;

        //public bool submissionStatus { get; set; } = false; 

        // 공용함수 호출
        public ScoreUtils scoreUtils = new ScoreUtils();

        // 팀장평가점수
        public double teamLeader_Score { get; set; } = 0;

        #endregion

        protected override async Task OnInitializedAsync()
        {
            await CheckLogined();
            await SetData(Id);
            await base.OnInitializedAsync();
        }

        private async Task SetData(Int64 Id)
        {
            Console.WriteLine($"[2nd_HR_Report Details] Parameter Id (Pid): {Id}");
            processDb = await processDbRepository.GetByIdAsync(Id);
            Console.WriteLine($"[2nd_HR_Report Details] ProcessDb found: Pid={processDb.Pid}, Uid={processDb.Uid}");
            model = await reportDbRepository.GetByUidAllAsync(processDb.Uid);
            Console.WriteLine($"[2nd_HR_Report Details] ReportDb Count: {model.Count}");
            Is_TeamLeader_Submission = processDb.Is_Teamleader_Submission;
            userDb = await userDbRepository.GetByIdAsync(processDb.Uid);
            userName = userDb.UserName;
            //IsTeamLeader_EvaluationComplete = GetIsTeamLeader_EvaluationComplete(model);
            Sub_IsTeamLeader_EvaluationComplete = GetIsTeamLeader_EvaluationComplete(model);

            #region + TotalReport 관련
            totalReportModel = await totalReportDbRepository.GetByUidAsync(userDb.Uid) ?? new TotalReportDb();
            var totalReportViewList = await v_TotalReportListDBRepository.GetByUserIdAsync(userDb.Uid);
            totalReportViewModel = totalReportViewList.FirstOrDefault() ?? new v_TotalReportListDB();


            //report_TeamLeader_Evaluation_1 = totalReportModel.TeamLeader_Evaluation_1 == 0 ?
            //    scoreUtils.TeamLeader_GetTotalScore_1(model) : 0;
            //report_TeamLeader_Evaluation_2 = totalReportModel.TeamLeader_Evaluation_2 == 0 ?
            //    scoreUtils.TeamLeader_GetTotalScore_2(model) : 0;
            //report_TeamLeader_Evaluation_3 = totalReportModel.TeamLeader_Evaluation_3 == 0 ?
            //    scoreUtils.TeamLeader_GetTotalScore_3(model) : 0;

            //report_TeamLeader_Evaluation_1 = scoreUtils.TeamLeader_GetTotalScore_1(model);
            //report_TeamLeader_Evaluation_2 = scoreUtils.TeamLeader_GetTotalScore_2(model);
            //report_TeamLeader_Evaluation_3 = scoreUtils.TeamLeader_GetTotalScore_3(model);
            //TeamLeader_Score = report_TeamLeader_Evaluation_1 + report_TeamLeader_Evaluation_2 + report_TeamLeader_Evaluation_3;
            //TeamLeader_Score = scoreUtils.TotalScroreTo100thpercentile(TeamLeader_Score);

            report_TeamLeader_Score = totalReportModel.TeamLeader_Score;
            report_TeamLeader_Comment = !String.IsNullOrEmpty(totalReportModel.TeamLeader_Comment) ?
                totalReportModel.TeamLeader_Comment : String.Empty;
            report_Feedback_Comment = !String.IsNullOrEmpty(totalReportModel.Feedback_Comment) ?
                totalReportModel.Feedback_Comment : String.Empty;
            #endregion
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

        #region + 팀장평가제출
        protected async Task TeamLeaderReportSubmission()
        {
            processDb.Is_Teamleader_Submission = true;
            int affectedRows = await processDbRepository.UpdateAsync(processDb);
            if (affectedRows > 0)
            {
                Is_TeamLeader_Submission = true;
                await UpdateTotalReport();
                urlActions.Move2ndMainPage();
            }
            else
            {
                processDb.Is_Teamleader_Submission = false;
            }
        }
        #endregion


        #region + 평가항목 작성여부 확인
        /// <summary>
        /// 평가대상자의 평가항목작성 여부를 체크하는 메서드
        /// </summary>
        /// <param name="lists">평가대상자의 평가리스트</param>
        /// <returns></returns>
        private bool GetIsTeamLeader_EvaluationComplete(List<ReportDb> lists)
        {
            Is_TeamLeader_Evaluation_1 = GetIsTeamLeaderEvaluation1(lists);
            Is_TeamLeader_Evaluation_2 = GetIsTeamLeaderEvaluation2(lists);
            Is_TeamLeader_Evaluation_3 = GetIsTeamLeaderEvaluation3(lists);
            if (Is_TeamLeader_Evaluation_1 &&
                Is_TeamLeader_Evaluation_2 &&
                Is_TeamLeader_Evaluation_3
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 평가대상자의 평가항목(TeamLeaderEvaluation1)작성 여부를 체크하는 메서드
        /// </summary>
        /// <param name="lists">평가대상자의 평가리스트</param>
        /// <returns></returns>
        private bool GetIsTeamLeaderEvaluation1(List<ReportDb> lists)
        {
            double TeamLeaderEvaluation1 = 0;
            if (lists != null && lists.Count != 0)
            {
                foreach (var item in lists)
                {
                    TeamLeaderEvaluation1 = item.TeamLeader_Evaluation_1;
                    if (TeamLeaderEvaluation1 == 0)
                    {
                        break;
                    }
                }
                return TeamLeaderEvaluation1 != 0 ? true : false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 평가대상자의 평가항목(TeamLeaderEvaluation2)작성 여부를 체크하는 메서드
        /// </summary>
        /// <param name="lists">평가대상자의 평가리스트</param>
        /// <returns></returns>
        private bool GetIsTeamLeaderEvaluation2(List<ReportDb> lists)
        {
            double TeamLeaderEvaluation2 = 0;
            if (lists != null && lists.Count != 0)
            {
                foreach (var item in lists)
                {
                    TeamLeaderEvaluation2 = item.TeamLeader_Evaluation_2;
                    if (TeamLeaderEvaluation2 == 0)
                    {
                        break;
                    }
                }
                return TeamLeaderEvaluation2 != 0 ? true : false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 평가대상자의 평가항목(TeamLeaderEvaluation3)작성 여부를 체크하는 메서드
        /// </summary>
        /// <param name="lists">평가대상자의 평가리스트</param>
        /// <returns></returns>
        private bool GetIsTeamLeaderEvaluation3(List<ReportDb> lists)
        {
            double TeamLeaderEvaluation3 = 0;
            if (lists != null && lists.Count != 0)
            {
                foreach (var item in lists)
                {
                    TeamLeaderEvaluation3 = item.TeamLeader_Evaluation_3;
                    if (TeamLeaderEvaluation3 == 0)
                    {
                        break;
                    }
                }
                return TeamLeaderEvaluation3 != 0 ? true : false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 평가대상자의 평가항목(TeamLeaderEvaluation4)작성 여부를 체크하는 메서드
        /// </summary>
        /// <param name="lists">평가대상자의 평가리스트</param>
        /// <returns></returns>
        private bool GetIsTeamLeaderEvaluation4(List<ReportDb> lists)
        {
            double TeamLeaderEvaluation4 = 0;
            if (lists != null && lists.Count != 0)
            {
                foreach (var item in lists)
                {
                    if (item.TeamLeader_Evaluation_4 != null)
                    {
                        TeamLeaderEvaluation4 = item.TeamLeader_Evaluation_4.Length;
                        if (TeamLeaderEvaluation4 == 0)
                        {
                            break;
                        }
                    }
                }
                return TeamLeaderEvaluation4 != 0 ? true : false;
            }
            else
            {
                return false;
            }
        }
        #endregion

        private void UserReportCollapsedToggle()
        {
            UserReportCollapsed = !UserReportCollapsed;
        }

        private void TeamLeaderReportCollapsedToggle()
        {
            TeamLeaderReportCollapsed = !TeamLeaderReportCollapsed;
        }


        private void TeamLeaderUpdateReport()
        {
            if (
            report_TeamLeader_Score != 0 &&
            !String.IsNullOrEmpty(report_TeamLeader_Comment) &&
            !String.IsNullOrEmpty(report_Feedback_Comment)
            )
            {
                if (!String.IsNullOrEmpty(update_resultText_1))
                {
                    update_resultText_1 = String.Empty;
                }
                IsTeamLeader_EvaluationComplete = true;
                TeamLeaderReportCollapsedToggle();
            }
            else
            {
                if (!String.IsNullOrEmpty(update_resultText_1))
                {
                    update_resultText_1 = String.Empty;
                }
                update_resultText_1 = "평가작성을 확인해주세요.";
            }
        }

        private void TeamLeaderReportClear()
        {
            //report_TeamLeader_Evaluation_1 = 0;
            //report_TeamLeader_Evaluation_2 = 0;
            //report_TeamLeader_Evaluation_3 = 0;
            report_TeamLeader_Score = 0;
            report_TeamLeader_Comment = String.Empty;
            report_Feedback_Comment = String.Empty;
            IsTeamLeader_EvaluationComplete = false;
            if (!String.IsNullOrEmpty(update_resultText_1))
            {
                update_resultText_1 = String.Empty;
            }
        }


        #region + TotalReport 작성
        /// <summary>
        /// TotalReport를 생성하는 메서드
        /// ReportList의 값을 조합하여 TotalReport를 생성한다.
        /// </summary>
        /// <returns></returns>
        private async Task UpdateTotalReport()
        {
            //await Task.Delay(1);

            #region + model정보

            #region + [0] 결과 기본정보
            //// [0].[01] TotalReport id
            //public Int64 TRid { get; set; }
            //// [0].[02] User id => UserDb Uid
            //public Int64 Uid { get; set; }
            #endregion

            #region + [1] 평가대상자 평가점수
            //// [1].[01] User_Evaluation_1(일정준수) - 일정준수
            //public double User_Evaluation_1 { get; set; }
            //// [1].[02] User_Evaluation_2(업무수행도) - 업무수행도
            //public double User_Evaluation_2 { get; set; }
            //// [1].[03] User_Evaluation_3(결과물) - 결과평가(정성)
            //public double User_Evaluation_3 { get; set; }
            //// [1].[04] User_Evaluation_4(comment)
            //public string? User_Evaluation_4 { get; set; }
            #endregion

            #region + [2] 부서장(팀장) 평가점수
            //// [2].[01] TeamLeader_Evaluation_1(일정준수) - 일정준수
            //public double TeamLeader_Evaluation_1 { get; set; }
            //// [2].[02] TeamLeader_Evaluation_2(업무수행도) - 업무수행도
            //public double TeamLeader_Evaluation_2 { get; set; }
            //// [2].[03] TeamLeader_Evaluation_3(결과물) - 결과평가(정성)
            //public double TeamLeader_Evaluation_3 { get; set; }
            //// [2].[04] TeamLeader_Comment(comment)
            //public string? TeamLeader_Comment { get; set; }
            #endregion

            #region + [3] feedback 1차 면담
            //// [3].[01] Feedback_Evaluation_1(일정준수) - 일정준수
            //public double Feedback_Evaluation_1 { get; set; }
            //// [3].[02] Feedback_Evaluation_2(업무수행도) - 업무수행도
            //public double Feedback_Evaluation_2 { get; set; }
            //// [3].[03] Feedback_Evaluation_3(결과물) - 결과평가(정성)
            //public double Feedback_Evaluation_3 { get; set; }
            //// [3].[04] Feedback_Comment(comment) 
            //public string? Feedback_Comment { get; set; }
            #endregion

            #region + [4] 임원 평가점수
            //// [4].[01] Director_Evaluation_1(일정준수)
            //public double Director_Evaluation_1 { get; set; }
            //// [4].[02] Director_Evaluation_2(업무수행도)
            //public double Director_Evaluation_2 { get; set; }
            //// [4].[03] Director_Evaluation_3(결과물)
            //public double Director_Evaluation_3 { get; set; }
            //// [4].[04] Director_Comment(comment)
            //public string? Director_Comment { get; set; }
            #endregion

            #region + [5] 종합평가점수
            //// [5] Total_Score(종합점수)
            //public double Total_Score { get; set; }
            //// [5].[02] Director_Score(임원점수)
            //public double Director_Score { get; set; }
            //// [5].[03] TeamLeader_Score(임원점수)
            //public double TeamLeader_Score { get; set; }
            #endregion

            #endregion

            #region + [1] model 값 입력
            // [2].[04] TeamLeader_Comment(comment) - 부서장(팀장) Comment
            totalReportModel.TeamLeader_Comment = report_TeamLeader_Comment;
            // [3].[04] Feedback_Comment(comment) - 피드 포워드
            totalReportModel.Feedback_Comment = report_Feedback_Comment;
            // [5].[03] TeamLeader_Score(임원점수) - 100 이상이면 100으로 수정
            totalReportModel.TeamLeader_Score = report_TeamLeader_Score > 100 ? 100 : report_TeamLeader_Score;
            // [2] 부서장(팀장) 평가점수
            // [2].[01] TeamLeader_Evaluation_1(일정준수) - 일정준수            
            //totalReportModel.TeamLeader_Evaluation_1 = report_TeamLeader_Evaluation_1;
            // [2].[02] TeamLeader_Evaluation_2(업무수행도) - 업무수행도
            //totalReportModel.TeamLeader_Evaluation_2 = report_TeamLeader_Evaluation_2;
            // [2].[03] TeamLeader_Evaluation_3(결과물) - 결과평가(정성)
            //totalReportModel.TeamLeader_Evaluation_3 = report_TeamLeader_Evaluation_3;
            #endregion

            int affectedRows = 0;

            if (
                totalReportModel.TeamLeader_Score != 0 &&
                !String.IsNullOrEmpty(totalReportModel.TeamLeader_Comment) &&
                !String.IsNullOrEmpty(totalReportModel.Feedback_Comment)
                )
            {
                affectedRows = await totalReportDbRepository.UpdateAsync(totalReportModel);
            }

            if (affectedRows > 0)
            {
                update_resultText = "종합평가작성(부서장)에 성공하였습니다.";
            }
            else
            {
                update_resultText = "종합평가작성(부서장)에 실패하였습니다.";
            }
        }
        #endregion

        private void Move2ndMainPage()
        {
            urlActions.Move2ndMainPage();
        }
    }
}
