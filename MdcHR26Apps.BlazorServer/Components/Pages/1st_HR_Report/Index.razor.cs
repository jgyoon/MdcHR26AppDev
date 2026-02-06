using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.BlazorServer.Utils;
using MdcHR26Apps.Models.EvaluationProcess;
using MdcHR26Apps.Models.EvaluationReport;
using MdcHR26Apps.Models.Views.v_TotalReportListDB;
using MdcHR26Apps.Models.Result;
using MdcHR26Apps.Models.User;
using Microsoft.AspNetCore.Components;
using System.Text.RegularExpressions;
using System.Web;

namespace MdcHR26Apps.BlazorServer.Components.Pages._1st_HR_Report
{
    public partial class Index
    {
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

        // 평가리스트 작성
        [Inject]
        public IReportRepository reportDbRepository { get; set; } = null!;
        public List<ReportDb> model { get; set; } = new List<ReportDb>();

        // 기타
        public bool IsRequest { get; set; } = false;
        public bool IsAgreement { get; set; } = false;
        public bool IsSubRequest { get; set; } = false;
        public bool IsSubAgreement { get; set; } = false;
        public string Sub_Agreement_Comment { get; set; } = string.Empty;
        public int sumperoportion { get; set; } = 0;
        public bool IS_SumPeroportionStatus { get; set; } = false;
        public double sumsubperoportion { get; set; } = 0;
        public bool IS_SumSubPeroportionStatus { get; set; } = false;
        public bool IsTeamLeaderAndDiretorSettings = false;

        // 확인 후 삭제
        //public string Agreement_Comment { get; set; } = string.Empty;

        #region + 평가항목 작성여부 변수
        // 사용자 평가 작성가능 여부(제출가능 여부)
        public bool IsUser_EvaluationComplete { get; set; } = false;

        // 일정준수 작성여부
        public bool Is_User_Evaluation_1 { get; set; } = false;
        // 업무수행도 작성여부
        public bool Is_User_Evaluation_2 { get; set; } = false;
        // 결과물 작성여부
        public bool Is_User_Evaluation_3 { get; set; } = false;
        // Comment 작성여부
        public bool Is_User_Evaluation_4 { get; set; } = false;
        #endregion

        // 기타
        public bool Is_User_Submission { get; set; } = false;
        public bool Is_TeamLeader_Submission { get; set; } = false;
        public bool Is_Director_Submission { get; set; } = false;


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

        // 공용함수 호출
        public ScoreUtils scoreUtils = new ScoreUtils();

        // 기타함수
        public string resultText { get; set; } = String.Empty;
        #endregion

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await CheckLogined();
                await SetData();
                if (model != null && model.Count > 0)
                {
                    // 사용자 평가 작성가능 여부(제출가능 여부)
                    IsUser_EvaluationComplete = GetIsUser_EvaluationComplete(model);
                }
                StateHasChanged();
            }
            else
            {
                await SetData();
                StateHasChanged();
            }
        }

        private async Task SetData()
        {
            //await Task.Delay(500);
            long sessionUserId = loginStatusService.LoginStatus.LoginUid;
            if (sessionUserId > 0)
            {
                #region + processDb
                processDb = await processDbRepository.GetByUidAsync(sessionUserId);
                // 직무평가 합의
                IsRequest = processDb.Is_Request;
                // 직무평가합의 여부
                IsAgreement = processDb.Is_Agreement;
                // 세부직무평가 합의
                IsSubRequest = processDb.Is_SubRequest;
                // 세부직무평가합의 여부
                IsSubAgreement = processDb.Is_SubAgreement;
                // 평가제출 여부
                Is_User_Submission = processDb.Is_User_Submission;
                // 부서장(팀장) && 임원 설정 여부
                IsTeamLeaderAndDiretorSettings = GetTeamLeaderAndDiretorSettings(processDb);
                // 부서장(팀장) 평가여부
                Is_TeamLeader_Submission = processDb.Is_Teamleader_Submission;
                // 임원 평가여부
                Is_Director_Submission = processDb.Is_Director_Submission;
                #endregion

                model = await reportDbRepository.GetByUidAllAsync(sessionUserId);
                //setSumPeroportion(model);

                #region + TotalReport관련
                // 사용자정보
                userDb = await userDbRepository.GetByIdAsync(sessionUserId);
                var totalReportList = await v_TotalReportListDBRepository.GetByUserIdAsync(userDb.Uid);
                totalReportViewModel = totalReportList.FirstOrDefault() ?? new v_TotalReportListDB();
                #endregion
            }
        }

        #region + [0].[1] CheckLogined : IsloginCheck()
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

        #region + [0].[2] MoveMainPage : 메인페이지 이동
        protected void MoveMainPage()
        {
            urlActions.MoveMainPage();
        }
        #endregion

        #region + [0].[2] MoveReportCreatePage : 평가 생성페이지 이동
        protected void MoveReportCreatePage()
        {
            // 26년도에는 Create 페이지가 없으므로 Index로 이동
            urlActions.Move1stReportIndexPage();
        }
        #endregion

        #region + 직무합의요청
        protected async Task SetRequst()
        {
            processDb.Is_Request = true;

            await processDbRepository.UpdateAsync(processDb);
        }
        #endregion

        #region + 평가제출
        protected async Task ReportSubmission()
        {
            if (processDb.Is_Request && processDb.Is_Agreement)
            {
                processDb.Is_User_Submission = true;
                //await processDbRepository.UpdateAsync(processDb);
                //Is_User_Submission = true;
                int affectedRows = await processDbRepository.UpdateAsync(processDb);
                if (affectedRows > 0)
                {
                    Is_User_Submission = true;
                    await CreateTotalReport();

                    await Task.Delay(3000);
                    resultText = String.Empty;
                    StateHasChanged();
                }
                else
                {
                    processDb.Is_User_Submission = false;
                }
            }
        }
        #endregion

        #region + 평가제출 취소
        private async Task ReportSubmissionCancle()
        {
            if (processDb.Is_User_Submission && !processDb.Is_Teamleader_Submission)
            {
                processDb.Is_User_Submission = false;
                int affectedRows = await processDbRepository.UpdateAsync(processDb);
                if (affectedRows > 0 && totalReportViewModel.TRid > 0)
                {
                    Is_User_Submission = false;
                    int deleteResult = await totalReportDbRepository.DeleteAsync(totalReportViewModel.TRid);
                    if (deleteResult > 0)
                    {
                        resultText = "평가 제출를 취소하였습니다.";
                        StateHasChanged();
                        await Task.Delay(3000);
                        resultText = String.Empty;
                        StateHasChanged();
                    }
                    else
                    {
                        resultText = "평가 제출 취소에 실패했습니다.";
                    }
                }
                else
                {
                    processDb.Is_User_Submission = true;
                }
            }
        }
        #endregion

        #region + 합의여부확인
        private string result()
        {
            return IsAgreement ? "합의(O)" : "합의(X)";
        }
        #endregion

        #region + 초기화(확인 후 삭제) InitButton
        private async Task InitButton()
        {
            processDb.Is_Request = false;
            processDb.Is_Agreement = false;
            processDb.Is_User_Submission = false;
            await processDbRepository.UpdateAsync(processDb);
            StateHasChanged();
        }
        #endregion

        #region + 직무 비중 총합
        /// <summary>
        /// 직무 비중 총합을 구하는 메서드
        /// </summary>
        /// <param name="lists"></param>
        private void setSumPeroportion(List<ReportDb> lists)
        {
            if (lists != null && lists.Count != 0)
            {
                foreach (var item in lists)
                {
                    sumperoportion += item.Report_Item_Proportion;
                }
            }
        }
        #endregion

        #region + 세무직무 비중(100%) 확인
        /// <summary>
        /// 세부 직무 비중 총합이 100% 인지 구하는 메서드
        /// </summary>
        /// <param name="lists"></param>
        private void SetSubPeroportionStatus(List<ReportDb> lists)
        {
            bool resultStatus = false;
            if (lists != null && lists.Count != 0)
            {
                foreach (var item in lists)
                {
                    sumsubperoportion += item.Report_SubItem_Proportion;
                }

                if (sumsubperoportion != 0)
                {
                    resultStatus = (sumsubperoportion / lists.Count) == 100 ? true : false;
                }
            }

            IS_SumSubPeroportionStatus = resultStatus;
        }
        #endregion

        /// <summary>
        /// 부서장(팀장) && 임원 설정 여부를 확인하는 메서드
        /// </summary>
        /// <param name="processDb">ProcessDb processDb</param>
        /// <returns>부서장(팀장) && 임원 설정 여부</returns>
        private bool GetTeamLeaderAndDiretorSettings(ProcessDb processDb)
        {
            if (processDb.TeamLeaderId.HasValue && processDb.DirectorId.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #region + 평가항목 작성여부 확인
        /// <summary>
        /// 평가항목 작성여부를 확인하는 메서드
        /// </summary>
        /// <param name="lists">List<ReportDb> lists</param>
        /// <returns>평가항목 작성여부</returns>
        private bool GetIsUser_EvaluationComplete(List<ReportDb> lists)
        {
            Is_User_Evaluation_1 = GetIsUserEvaluation1(lists);
            Is_User_Evaluation_2 = GetIsUserEvaluation2(lists);
            Is_User_Evaluation_3 = GetIsUserEvaluation3(lists);
            Is_User_Evaluation_4 = GetIsUserEvaluation4(lists);
            if (Is_User_Evaluation_1 && Is_User_Evaluation_2 && Is_User_Evaluation_3 && Is_User_Evaluation_4)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// UserEvaluation1의 항목이 입력되었는지 확인하는 메서드
        /// </summary>
        /// <param name="lists">List<ReportDb> lists</param>
        /// <returns>UserEvaluation1의 항목이 입력되었는지 확인</returns>
        private bool GetIsUserEvaluation1(List<ReportDb> lists)
        {
            double UserEvaluation1 = 0;
            if (lists != null && lists.Count != 0)
            {
                foreach (var item in lists)
                {
                    UserEvaluation1 = item.User_Evaluation_1;
                    if (UserEvaluation1 == 0)
                    {
                        break;
                    }
                }
                return UserEvaluation1 != 0 ? true : false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// UserEvaluation2의 항목이 입력되었는지 확인하는 메서드
        /// </summary>
        /// <param name="lists">List<ReportDb> lists</param>
        /// <returns>UserEvaluation2의 항목이 입력되었는지 확인</returns>
        private bool GetIsUserEvaluation2(List<ReportDb> lists)
        {
            double UserEvaluation2 = 0;
            if (lists != null && lists.Count != 0)
            {
                foreach (var item in lists)
                {
                    UserEvaluation2 = item.User_Evaluation_2;
                    if (UserEvaluation2 == 0)
                    {
                        break;
                    }
                }
                return UserEvaluation2 != 0 ? true : false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// UserEvaluation3의 항목이 입력되었는지 확인하는 메서드
        /// </summary>
        /// <param name="lists">List<ReportDb> lists</param>
        /// <returns>UserEvaluation3의 항목이 입력되었는지 확인</returns>
        private bool GetIsUserEvaluation3(List<ReportDb> lists)
        {
            double UserEvaluation3 = 0;
            if (lists != null && lists.Count != 0)
            {
                foreach (var item in lists)
                {
                    UserEvaluation3 = item.User_Evaluation_3;
                    if (UserEvaluation3 == 0)
                    {
                        break;
                    }
                }
                return UserEvaluation3 != 0 ? true : false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// UserEvaluation4의 항목이 입력되었는지 확인하는 메서드
        /// </summary>
        /// <param name="lists">List<ReportDb> lists</param>
        /// <returns>UserEvaluation4의 항목이 입력되었는지 확인</returns>
        private bool GetIsUserEvaluation4(List<ReportDb> lists)
        {
            int UserEvaluation4 = 0;
            if (lists != null && lists.Count != 0)
            {
                foreach (var item in lists)
                {
                    if (item.User_Evaluation_4 != null)
                    {
                        UserEvaluation4 = item.User_Evaluation_4.Length;
                        if (UserEvaluation4 == 0)
                        {
                            break;
                        }
                    }
                }
                return UserEvaluation4 != 0 ? true : false;
            }
            else
            {
                return false;
            }
        }
        #endregion

        // https://stackoverflow.com/questions/64157834/how-can-i-have-new-line-in-blazor
        private string replaceString(string contenct)
        {
            return Regex.Replace(HttpUtility.HtmlEncode(contenct), "\r?\n|\r", "<br />");
        }

        #region + TotalReport 작성
        /// <summary>
        /// TotalReport를 생성하는 메서드
        /// ReportList의 값을 조합하여 TotalReport를 생성한다.
        /// </summary>
        /// <returns></returns>
        private async Task CreateTotalReport()
        {
            await Task.Delay(1);

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
            //// [5].[01] Total_Score(종합점수)
            //public double Total_Score { get; set; }
            //// [5].[02] Director_Score(임원점수)
            //public double Director_Score { get; set; }

            #endregion

            #region + [1] model 값 입력
            // [0] 결과 기본정보
            //[0].[01] TotalReport id
            //totalReportModel.TRid;
            //[0].[02] User id => UserDb Uid
            totalReportModel.Uid = userDb.Uid;

            // [1] 평가대상자 평가점수
            // [1].[01] User_Evaluation_1(일정준수) - 일정준수
            totalReportModel.User_Evaluation_1 = scoreUtils.User_GetTotalScore_1(model);
            // [1].[02] User_Evaluation_2(업무수행도) - 업무수행도
            totalReportModel.User_Evaluation_2 = scoreUtils.User_GetTotalScore_2(model);
            // [1].[03] User_Evaluation_3(결과물) - 결과평가(정성)
            totalReportModel.User_Evaluation_3 = scoreUtils.User_GetTotalScore_3(model);
            // [1].[04] User_Evaluation_4(comment)
            totalReportModel.User_Evaluation_4 = string.Empty;

            // [2] 부서장(팀장) 평가점수
            // [2].[01] TeamLeader_Evaluation_1(일정준수) - 일정준수
            totalReportModel.TeamLeader_Evaluation_1 = 0;
            // [2].[02] TeamLeader_Evaluation_2(업무수행도) - 업무수행도
            totalReportModel.TeamLeader_Evaluation_2 = 0;
            // [2].[03] TeamLeader_Evaluation_3(결과물) - 결과평가(정성)
            totalReportModel.TeamLeader_Evaluation_3 = 0;
            // [2].[04] TeamLeader_Comment(comment)
            totalReportModel.TeamLeader_Comment = string.Empty;

            // [3] feedback 1차 면담
            // [3].[01] Feedback_Evaluation_1(일정준수) - 일정준수
            totalReportModel.Feedback_Evaluation_1 = 0;
            // [3].[02] Feedback_Evaluation_2(업무수행도) - 업무수행도
            totalReportModel.Feedback_Evaluation_2 = 0;
            // [3].[03] Feedback_Evaluation_3(결과물) - 결과평가(정성)
            totalReportModel.Feedback_Evaluation_3 = 0;
            // [3].[04] Feedback_Comment(comment)
            totalReportModel.Feedback_Comment = string.Empty;

            // [4] 임원 평가점수
            // [4].[01] Director_Evaluation_1(일정준수)
            totalReportModel.Director_Evaluation_1 = 0;
            // [4].[02] Director_Evaluation_2(업무수행도)
            totalReportModel.Director_Evaluation_2 = 0;
            // [4].[03] Director_Evaluation_3(결과물)
            totalReportModel.Director_Evaluation_3 = 0;
            // [4].[04] Director_Comment(comment)
            totalReportModel.Director_Comment = string.Empty;

            // [5].[01] Total_Score(종합점수)
            totalReportModel.Total_Score = 0;
            // [5].[02] Director_Score(임원점수)
            totalReportModel.Director_Score = 0;
            // [5].[03] TeamLeader_Score(임원점수)
            totalReportModel.TeamLeader_Score = 0;
            #endregion

            long insertedId = await totalReportDbRepository.AddAsync(totalReportModel);
            if (insertedId != 0)
            {
                totalReportModel.TRid = insertedId;
                resultText = "평가 제출을 성공하였습니다.";
                StateHasChanged();
            }
            else
            {
                resultText = "평가 제출에 실패했습니다.";
            }
        }
        #endregion

    }
}
