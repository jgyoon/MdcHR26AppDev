using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.BlazorServer.Utils;
using MdcHR26Apps.Models.EvaluationProcess;
using MdcHR26Apps.Models.EvaluationReport;
using MdcHR26Apps.Models.Views.v_TotalReportListDB;
using MdcHR26Apps.Models.Result;
using MdcHR26Apps.Models.User;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages._3rd_HR_Report
{
    public partial class Details
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
        public List<ReportDb> model { get; set; } = new List<ReportDb>();

        // 기타
        public string userName { get; set; } = String.Empty;
        public string resultText { get; set; } = String.Empty;
        public bool Is_Director_Submission { get; set; } = false;

        public double totalScore { get; set; } = 0;
        public double totalScore_1 { get; set; } = 0;
        public double totalScore_2 { get; set; } = 0;
        public double totalScore_3 { get; set; } = 0;

        #region + 평가항목 작성여부 변수
        // 임원 평가 작성가능 여부(제출가능 여부)
        public bool IsDirector_EvaluationComplete { get; set; } = false;
        // 일정준수 작성여부
        public bool Is_Director_Evaluation_1 { get; set; } = false;
        // 업무수행도 작성여부
        public bool Is_Director_Evaluation_2 { get; set; } = false;
        // 결과물 작성여부
        public bool Is_Director_Evaluation_3 { get; set; } = false;
        // Comment 작성여부
        public bool Is_Director_Evaluation_4 { get; set; } = false;
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
        public bool TeamLeaderReportCollapsed { get; set; } = false;
        public bool FeedBackReportCollapsed { get; set; } = false;
        public bool FeedBackReport2Collapsed { get; set; } = false;
        public bool DirectorReportCollapsed { get; set; } = true;


        // 임원평가 값
        public double report_Director_Evaluation_1 { get; set; } = 0;
        public double report_Director_Evaluation_2 { get; set; } = 0;
        public double report_Director_Evaluation_3 { get; set; } = 0;
        public string report_Director_Comment { get; set; } = String.Empty;

        public double report_total_1 { get; set; } = 0;
        public double report_total_2 { get; set; } = 0;
        public double report_total_3 { get; set; } = 0;

        public string report_Feedback_Comment { get; set; } = String.Empty;

        public string update_resultText { get; set; } = String.Empty;
        public string update_resultText_1 { get; set; } = String.Empty;

        //public bool submissionStatus { get; set; } = false;

        // 공용함수 호출
        public ScoreUtils scoreUtils = new ScoreUtils();

        // 평가비중
        // 평가자 : 20%
        // 부서장(팀장) : 80%
        public double userScoreProportion = 0.2;
        public double teamLeaderScoreProportion = 0.8;

        // 임원평가점수
        public double director_Score { get; set; } = 0;

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

        private async Task SetData(long Id)
        {
            processDb = await processDbRepository.GetByIdAsync(Id) ?? new ProcessDb();
            model = await reportDbRepository.GetByUidAllAsync(processDb.Uid) ?? new List<ReportDb>();
            Is_Director_Submission = processDb.Is_Director_Submission;

            // 26년도: ProcessDb에 UserName 없음 -> UserDb에서 가져오기
            userDb = await userDbRepository.GetByIdAsync(processDb.Uid) ?? new UserDb();
            userName = userDb.UserName;

            //IsDirector_EvaluationComplete = GetIsDirector_EvaluationComplete(model);
            totalScore = GetTotalScore(model);

            totalScore_1 = GetTotalScore_1(model);
            totalScore_2 = GetTotalScore_2(model);
            totalScore_3 = GetTotalScore_3(model);

            #region + TotalReport 관련
            totalReportModel = await totalReportDbRepository.GetByUidAsync(userDb.Uid) ?? new TotalReportDb();
            var totalReportViewList = await v_TotalReportListDBRepository.GetByUserIdAsync(userDb.Uid);
            totalReportViewModel = totalReportViewList.FirstOrDefault() ?? new v_TotalReportListDB();
            #endregion
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

        private void DirectorUpdateReport()
        {
            if (
           //     report_Director_Evaluation_1 != 0 &&
           //report_Director_Evaluation_2 != 0 &&
           //report_Director_Evaluation_3 != 0 &&
           !String.IsNullOrEmpty(report_Director_Comment)
           )
            {
                if (!String.IsNullOrEmpty(update_resultText_1))
                {
                    update_resultText_1 = String.Empty;
                }
                IsDirector_EvaluationComplete = true;

                // 펼쳐보기 모두 닫기
                UserReportCollapsed = false;
                TeamLeaderReportCollapsed = false;
                FeedBackReportCollapsed = false;
                FeedBackReport2Collapsed = false;
                DirectorReportCollapsed = false;
                //DirectorReportCollapsedToggle();
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

        private void DirectorReportClear()
        {
            report_Director_Evaluation_1 = 0;
            report_Director_Evaluation_2 = 0;
            report_Director_Evaluation_3 = 0;
            report_Director_Comment = String.Empty;
            director_Score = 0;
            IsDirector_EvaluationComplete = false;

            if (!String.IsNullOrEmpty(update_resultText_1))
            {
                update_resultText_1 = String.Empty;
            }
        }

        #region + 임원평가제출
        protected async Task DirectorReportSubmission()
        {
            processDb.Is_Director_Submission = true;
            int affectedRows = await processDbRepository.UpdateAsync(processDb);
            if (affectedRows > 0)
            {
                Is_Director_Submission = true;
                await UpdateTotalReport();
                urlActions.Move3rdMainPage();
            }
            else
            {
                processDb.Is_Director_Submission = false;
            }
        }
        #endregion

        #region + 임원평가제출 취소
        protected void DirectorReportSubmissionCencel()
        {
            DirectorReportClear();
            UserReportCollapsed = false;
            FeedBackReportCollapsed = false;
            FeedBackReport2Collapsed = true;
            DirectorReportCollapsed = true;
        }
        #endregion

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
            //public long TRid { get; set; }
            //// [0].[02] User id => UserDb Uid
            //public long Uid { get; set; }
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
            //// [5].[01] Total_Score(종합점수)
            //public double Total_Score { get; set; }
            //// [5].[02] Director_Score(임원점수)
            //public double Director_Score { get; set; }
            #endregion

            #region + [1] model 값 입력
            //// [4].[01] Director_Evaluation_1(일정준수)
            //public double Director_Evaluation_1 { get; set; }
            totalReportModel.Director_Evaluation_1 = report_Director_Evaluation_1;
            //// [4].[02] Director_Evaluation_2(업무수행도)
            //public double Director_Evaluation_2 { get; set; }
            totalReportModel.Director_Evaluation_2 = report_Director_Evaluation_2;
            //// [4].[03] Director_Evaluation_3(결과물)
            //public double Director_Evaluation_3 { get; set; }
            totalReportModel.Director_Evaluation_3 = report_Director_Evaluation_3;
            //// [4].[04] Director_Comment(comment)
            //public string? Director_Comment { get; set; }
            totalReportModel.Director_Comment = !String.IsNullOrEmpty(report_Director_Comment) ?
                report_Director_Comment : String.Empty;
            ////[5].[02] Director_Score(임원점수) - 최종점수
            //public double Director_Score { get; set; }
            totalReportModel.Director_Score = director_Score;
            #endregion

            #region + [2] (비활성화)TotalScore 입력 : 팀장 평가 + 피드백
            //report_total_1 = totalReportModel.TeamLeader_Evaluation_1 + totalReportModel.Feedback_Evaluation_1 + totalReportModel.Director_Evaluation_1;
            //report_total_2 = totalReportModel.TeamLeader_Evaluation_2 + totalReportModel.Feedback_Evaluation_2 + totalReportModel.Director_Evaluation_2;
            //report_total_3 = totalReportModel.TeamLeader_Evaluation_3 + totalReportModel.Feedback_Evaluation_3 + totalReportModel.Director_Evaluation_3;

            //totalReportModel.Total_Score = report_total_1 + report_total_2 + report_total_3;
            #endregion

            #region + [2] TotalScore 입력 - 비활성화(확인 후 삭제)
            // 비중적용 - 비활성화
            //report_total_1 =
            //    (totalReportModel.User_Evaluation_1 * userScoreProportion) +
            //    ((totalReportModel.TeamLeader_Evaluation_1 + totalReportModel.Feedback_Evaluation_1) * teamLeaderScoreProportion);
            //report_total_2 =
            //    (totalReportModel.User_Evaluation_2 * userScoreProportion) +
            //    ((totalReportModel.TeamLeader_Evaluation_2 + totalReportModel.Feedback_Evaluation_2) * teamLeaderScoreProportion);
            //report_total_3 =
            //    (totalReportModel.User_Evaluation_3 * userScoreProportion) +
            //    ((totalReportModel.TeamLeader_Evaluation_3 + totalReportModel.Feedback_Evaluation_3) * teamLeaderScoreProportion);

            // 토탈점수 합산(소숫점 2자리)
            //report_total_1 = Math.Round(report_total_1, 2);
            //report_total_2 = Math.Round(report_total_2, 2);
            //report_total_3 = Math.Round(report_total_3, 2);

            // 변경 전
            //totalReportModel.Total_Score = report_total_1 + report_total_2 + report_total_3;

            // 변경 후
            // ((평가자 (20%) + 부서장 (80%)) / 3) - 비활성화
            //totalReportModel.Total_Score = report_total_1 + report_total_2 + report_total_3;
            //totalReportModel.Total_Score = (totalReportModel.Total_Score / 300) * 100;
            //totalReportModel.Total_Score = Math.Round(totalReportModel.Total_Score, 2);

            //totalReportModel.Total_Score = totalReportModel.Total_Score + totalReportModel.Director_Score;
            //totalReportModel.Total_Score = Math.Round(totalReportModel.Total_Score, 2);
            #endregion

            #region + 100 점 초과 방지
            totalReportModel.Director_Evaluation_1 = totalReportModel.Director_Evaluation_1 >= 100 ? 100 : totalReportModel.Director_Evaluation_1;
            totalReportModel.Director_Evaluation_2 = totalReportModel.Director_Evaluation_2 >= 100 ? 100 : totalReportModel.Director_Evaluation_2;
            totalReportModel.Director_Evaluation_3 = totalReportModel.Director_Evaluation_3 >= 100 ? 100 : totalReportModel.Director_Evaluation_3;
            #endregion

            int affectedRows = 0;

            affectedRows = await totalReportDbRepository.UpdateAsync(totalReportModel);

            if (affectedRows > 0)
            {
                update_resultText = "종합평가작성(임원)에 성공하였습니다.";
            }
            else
            {
                update_resultText = "종합평가작성(임원)에 실패하였습니다.";
            }
        }
        #endregion

        #region + 평가항목 작성여부 확인
        /// <summary>
        /// 평가대상자의 평가항목작성 여부를 체크하는 메서드
        /// </summary>
        /// <param name="lists">평가대상자의 평가리스트</param>
        /// <returns>평가대상자의 평가항목작성 여부</returns>
        private bool GetIsDirector_EvaluationComplete(List<ReportDb> lists)
        {
            Is_Director_Evaluation_1 = GetIsDirectorEvaluation1(lists);
            Is_Director_Evaluation_2 = GetIsDirectorEvaluation2(lists);
            Is_Director_Evaluation_3 = GetIsDirectorEvaluation3(lists);
            Is_Director_Evaluation_4 = GetIsDirectorEvaluation4(lists);
            if (Is_Director_Evaluation_1 &&
                Is_Director_Evaluation_2 &&
                Is_Director_Evaluation_3 &&
                Is_Director_Evaluation_4)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 평가대상자의 평가항목(DirectorEvaluation1)작성 여부를 체크하는 메서드
        /// </summary>
        /// <param name="lists">평가대상자의 평가리스트</param>
        /// <returns>평가항목(DirectorEvaluation1)작성 여부</returns>
        private bool GetIsDirectorEvaluation1(List<ReportDb> lists)
        {
            double DirectorEvaluation1 = 0;
            if (lists != null && lists.Count != 0)
            {
                foreach (var item in lists)
                {
                    DirectorEvaluation1 = item.Director_Evaluation_1;
                    if (DirectorEvaluation1 == 0)
                    {
                        break;
                    }
                }
                return DirectorEvaluation1 != 0 ? true : false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 평가대상자의 평가항목(DirectorEvaluation2)작성 여부를 체크하는 메서드
        /// </summary>
        /// <param name="lists">평가대상자의 평가리스트</param>
        /// <returns>평가항목(DirectorEvaluation2)작성 여부</returns>
        private bool GetIsDirectorEvaluation2(List<ReportDb> lists)
        {
            double DirectorEvaluation2 = 0;
            if (lists != null && lists.Count != 0)
            {
                foreach (var item in lists)
                {
                    DirectorEvaluation2 = item.Director_Evaluation_2;
                    if (DirectorEvaluation2 == 0)
                    {
                        break;
                    }
                }
                return DirectorEvaluation2 != 0 ? true : false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 평가대상자의 평가항목(DirectorEvaluation3)작성 여부를 체크하는 메서드
        /// </summary>
        /// <param name="lists">평가대상자의 평가리스트</param>
        /// <returns>평가항목(DirectorEvaluation3)작성 여부</returns>
        private bool GetIsDirectorEvaluation3(List<ReportDb> lists)
        {
            double DirectorEvaluation3 = 0;
            if (lists != null && lists.Count != 0)
            {
                foreach (var item in lists)
                {
                    DirectorEvaluation3 = item.Director_Evaluation_3;
                    if (DirectorEvaluation3 == 0)
                    {
                        break;
                    }
                }
                return DirectorEvaluation3 != 0 ? true : false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 평가대상자의 평가항목(DirectorEvaluation4)작성 여부를 체크하는 메서드
        /// </summary>
        /// <param name="lists">평가대상자의 평가리스트</param>
        /// <returns>평가항목(DirectorEvaluation4)작성 여부</returns>
        private bool GetIsDirectorEvaluation4(List<ReportDb> lists)
        {
            double DirectorEvaluation4 = 0;
            if (lists != null && lists.Count != 0)
            {
                foreach (var item in lists)
                {
                    if (item.Director_Evaluation_4 != null)
                    {
                        DirectorEvaluation4 = item.Director_Evaluation_4.Length;
                        if (DirectorEvaluation4 == 0)
                        {
                            break;
                        }
                    }
                }
                return DirectorEvaluation4 != 0 ? true : false;
            }
            else
            {
                return false;
            }
        }
        #endregion


        #region + 평가대상자의 종합점수
        /// <summary>
        /// 평가대상자의 종합점수를 구하는 메서드
        /// </summary>
        /// <param name="lists">평가대상자의 평가리스트</param>
        /// <returns>평가대상자의 종합점수(double)</returns>
        private double GetTotalScore(List<ReportDb> lists)
        {
            double totalScore = 0;
            //int itemCount = 0;

            //List<ReportDb> list1 = new List<ReportDb>();


            if (lists != null && lists.Count != 0)
            {
                //list1 = lists.GroupBy(e => e.Report_Item_Name_1).Select(grp => grp.Select(l => l.Report_Item_Name_1).Distinct()).ToList();

                #region + 평가비중 미적용
                //foreach (var item in lists)
                //{
                //    totalScore = totalScore + item.Total_Score;
                //}
                //return (totalScore) / lists.Count;
                #endregion

                #region + 평가비중 적용
                foreach (var item in lists)
                {
                    totalScore = totalScore
                        + (double)(item.Total_Score
                        * (double)(item.Report_Item_Proportion / (double)100)
                        * (double)(item.Report_SubItem_Proportion / (double)100));

                    //double itemTotal_Score = item.Total_Score;
                    //double Item_Proportion = item.Report_Item_Proportion / (double)100;
                    //double SubItem_Proportion = item.Report_SubItem_Proportion / (double)100;
                    //itemTotal_Score = itemTotal_Score * Item_Proportion * SubItem_Proportion;

                    //totalScore = totalScore + itemTotal_Score;
                }
                return totalScore;
                #endregion



            }
            else
            {
                return totalScore;
            }
        }
        #endregion

        #region + 평가대상자의 종합점수 1
        /// <summary>
        /// 평가대상자의 종합점수1를 구하는 메서드
        /// </summary>
        /// <param name="lists">평가대상자의 평가리스트</param>
        /// <returns>평가대상자의 종합점수1(double)</returns>
        private double GetTotalScore_1(List<ReportDb> lists)
        {
            double totalScore = 0;
            //int itemCount = 0;

            //List<ReportDb> list1 = new List<ReportDb>();


            if (lists != null && lists.Count != 0)
            {
                //list1 = lists.GroupBy(e => e.Report_Item_Name_1).Select(grp => grp.Select(l => l.Report_Item_Name_1).Distinct()).ToList();

                #region + 평가비중 미적용
                //foreach (var item in lists)
                //{
                //    totalScore = totalScore + (item.User_Evaluation_1 + item.TeamLeader_Evaluation_1 + item.Director_Evaluation_1);
                //}
                //return (totalScore) / lists.Count;
                #endregion

                #region + 평가비중 적용
                foreach (var item in lists)
                {
                    totalScore = totalScore +
                        (double)((item.User_Evaluation_1 + item.TeamLeader_Evaluation_1 + item.Director_Evaluation_1)
                        * (double)(item.Report_Item_Proportion / (double)100)
                        * (double)(item.Report_SubItem_Proportion / (double)100)
                        );
                }
                return totalScore;
                #endregion

            }
            else
            {
                return totalScore;
            }
        }
        #endregion

        #region + 평가대상자의 종합점수 2
        /// <summary>
        /// 평가대상자의 종합점수2를 구하는 메서드
        /// </summary>
        /// <param name="lists">평가대상자의 평가리스트</param>
        /// <returns>평가대상자의 종합점수2(double)</returns>
        private double GetTotalScore_2(List<ReportDb> lists)
        {
            double totalScore = 0;
            //int itemCount = 0;

            //List<ReportDb> list1 = new List<ReportDb>();


            if (lists != null && lists.Count != 0)
            {
                //list1 = lists.GroupBy(e => e.Report_Item_Name_1).Select(grp => grp.Select(l => l.Report_Item_Name_1).Distinct()).ToList();

                #region + 평가비중 미적용
                //foreach (var item in lists)
                //{
                //    totalScore = totalScore + (item.User_Evaluation_2 + item.TeamLeader_Evaluation_2 + item.Director_Evaluation_2);
                //}
                //return (totalScore) / lists.Count;
                #endregion

                #region + 평가비중 적용
                foreach (var item in lists)
                {
                    totalScore = totalScore +
                        (double)((item.User_Evaluation_2 + item.TeamLeader_Evaluation_2 + item.Director_Evaluation_2)
                        * (double)(item.Report_Item_Proportion / (double)100)
                        * (double)(item.Report_SubItem_Proportion / (double)100)
                        );
                }
                return totalScore;
                #endregion
            }
            else
            {
                return totalScore;
            }
        }
        #endregion

        #region + 평가대상자의 종합점수 3
        /// <summary>
        /// 평가대상자의 종합점수3를 구하는 메서드
        /// </summary>
        /// <param name="lists">평가대상자의 평가리스트</param>
        /// <returns>평가대상자의 종합점수3(double)</returns>
        private double GetTotalScore_3(List<ReportDb> lists)
        {
            double totalScore = 0;

            if (lists != null && lists.Count != 0)
            {
                #region + 평가비중 미적용
                //foreach (var item in lists)
                //{
                //    totalScore = totalScore + (item.User_Evaluation_3 + item.TeamLeader_Evaluation_3 + item.Director_Evaluation_3);
                //}
                //return (totalScore) / lists.Count;
                #endregion

                #region + 평가비중 적용
                foreach (var item in lists)
                {
                    totalScore = totalScore +
                        (double)((item.User_Evaluation_3 + item.TeamLeader_Evaluation_3 + item.Director_Evaluation_3)
                        * (double)(item.Report_Item_Proportion / (double)100)
                        * (double)(item.Report_SubItem_Proportion / (double)100)
                        );
                }
                return totalScore;
                #endregion
            }
            else
            {
                return totalScore;
            }
        }
        #endregion

        private void Move3rdMainPage()
        {
            urlActions.Move3rdMainPage();
        }
    }
}
