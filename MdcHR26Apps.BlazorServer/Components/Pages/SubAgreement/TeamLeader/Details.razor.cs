using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.EvaluationAgreement;
using MdcHR26Apps.Models.EvaluationProcess;
using MdcHR26Apps.Models.EvaluationReport;
using MdcHR26Apps.Models.EvaluationSubAgreement;
using MdcHR26Apps.Models.User;
using Microsoft.AspNetCore.Components;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;

namespace MdcHR26Apps.BlazorServer.Components.Pages.SubAgreement.TeamLeader
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

        // 세부직무합의관리
        [Inject]
        public ISubAgreementRepository subAgreementDbRepository { get; set; } = null!;
        public List<SubAgreementDb> model { get; set; } = new List<SubAgreementDb>();

        // 평가리스트 작성
        [Inject]
        public IReportRepository reportDbRepository { get; set; } = null!;
        public ReportDb report { get; set; } = new ReportDb();

        // 사용자 정보
        [Inject]
        public IUserRepository userDbRepository { get; set; } = null!;

        // 기타
        public string userName { get; set; } = String.Empty;
        public string resultText { get; set; } = String.Empty;
        // 이전 합의내역
        public string Old_Agreement_Comment { get; set; } = String.Empty;
        public CultureInfo cultureInfo { get; set; } = new CultureInfo("ko-KR");

        // 펼쳐보기
        public bool Collapsed { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            await CheckLogined();
            await SetData(Id);
            await base.OnInitializedAsync();
        }
        private async Task SetData(long id)
        {
            processDb = await processDbRepository.GetByIdAsync(id) ?? new ProcessDb();
            model = await subAgreementDbRepository.GetByUidAllAsync(processDb.Uid) ?? new List<SubAgreementDb>();
            var userDb = await userDbRepository.GetByIdAsync(processDb.Uid) ?? new UserDb();
            userName = userDb.UserName;

            Old_Agreement_Comment =
                !String.IsNullOrEmpty(processDb.SubAgreement_Comment) ? processDb.SubAgreement_Comment : String.Empty;
            if (!String.IsNullOrEmpty(processDb.SubAgreement_Comment))
            {
                processDb.SubAgreement_Comment = String.Empty;
            }
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

        #region + 합의 승인 : AgreementConfirm
        private async Task AgreementConfirm()
        {
            //await Task.Delay(1);
            if (String.IsNullOrEmpty(processDb.SubAgreement_Comment))
            {
                resultText = "합의 코멘트를 입력하여 주세요.";
                return;
            }

            //processDb.SubAgreement_Comment = processDb.SubAgreement_Comment + "[" + DateTime.Now.ToString() + "]";
            if (!String.IsNullOrEmpty(Old_Agreement_Comment))
            {
                processDb.SubAgreement_Comment =
                    Old_Agreement_Comment + "\r\n​"
                    + "[승인]" + processDb.SubAgreement_Comment
                    + "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", cultureInfo) + "]";
            }
            else
            {
                processDb.SubAgreement_Comment =
                    "[승인]" + processDb.SubAgreement_Comment
                    + "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", cultureInfo) + "]";
            }
            processDb.Is_SubAgreement = true;

            int updateStatus = await processDbRepository.UpdateAsync(processDb);

            if (updateStatus > 0)
            {
                resultText = "합의 승인 성공";
                await CreateReport();
                if (report.Rid != 0)
                {
                    resultText = "평가 생성에 성공하였습니다.";
                    StateHasChanged();
                    // 평가메인페이지 이동
                    urlActions.MoveTeamLeaderSubAgreementMainPage();
                }
                else
                {
                    resultText = "평가 생성에 실패했습니다.-2";
                }
            }
            else
            {
                resultText = "합의 승인 실패";
            }

        }
        #endregion

        #region + 합의 반려 : AgreementRefer
        private async Task AgreementRefer()
        {
            if (String.IsNullOrEmpty(processDb.SubAgreement_Comment))
            {
                resultText = "합의 코멘트를 입력하여 주세요.";
                return;
            }

            if (!String.IsNullOrEmpty(Old_Agreement_Comment))
            {
                processDb.SubAgreement_Comment =
                    Old_Agreement_Comment + "\r\n​"
                    + "[반려]" + processDb.SubAgreement_Comment
                    + "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", cultureInfo) + "]";
            }
            else
            {
                processDb.SubAgreement_Comment =
                    "[반려]" + processDb.SubAgreement_Comment
                    + "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", cultureInfo) + "]";
            }
            processDb.Is_SubRequest = false;

            int updateStatus = await processDbRepository.UpdateAsync(processDb);

            if (updateStatus > 0)
            {
                resultText = "합의 반려 성공";
                StateHasChanged();
                urlActions.MoveTeamLeaderSubAgreementMainPage();
            }
            else
            {
                resultText = "합의 반려 실패";
            }
        }
        #endregion

        #region + 평가표 생성 : CreateReport
        private async Task CreateReport()
        {
            await Task.Delay(1);

            if (model != null && model.Count > 0)
            {
                foreach (var item in model)
                {
                    ReportDb reports = new ReportDb();

                    #region + [0] 평가기본정보
                    // [02] 사용자 계정
                    reports.Uid = item.Uid;
                    // [04] Report_Item_Number
                    reports.Report_Item_Number = item.Report_Item_Number;
                    // [05] Report_Item_Name_1(지표분류명)
                    reports.Report_Item_Name_1 = item.Report_Item_Name_1;
                    // [06] Report_Item_Name_2(직무분류명)
                    reports.Report_Item_Name_2 = item.Report_Item_Name_2;
                    // [07] Report_Item_Proportion(직무 %)
                    reports.Report_Item_Proportion = item.Report_Item_Proportion;
                    // [08] Report_SubItem_Name(세부직무명)
                    reports.Report_SubItem_Name = item.Report_SubItem_Name;
                    // [09] Report_Item_Proportion(세부직무 %)
                    reports.Report_SubItem_Proportion = item.Report_SubItem_Proportion;
                    // [10] 하위 업무 리스트 번호
                    reports.Task_Number = item.Task_Number;
                    #endregion

                    #region + 평가대상자 평가
                    // [11] User_Evaluation_1(일정준수)
                    reports.User_Evaluation_1 = 0;
                    // [12] User_Evaluation_2(업무수행도)
                    reports.User_Evaluation_2 = 0;
                    // [13] User_Evaluation_3(결과물)
                    reports.User_Evaluation_3 = 0;
                    // [14] User_Evaluation_4(comment)
                    reports.User_Evaluation_4 = String.Empty;
                    #endregion

                    #region + [2] 부서장(팀장) 평가
                    // [15] TeamLeader_Evaluation_1(일정준수)
                    reports.TeamLeader_Evaluation_1 = 0;
                    // [16] TeamLeader_Evaluation_2(업무수행도)
                    reports.TeamLeader_Evaluation_2 = 0;
                    // [17] TeamLeader_Evaluation_3(결과물)
                    reports.TeamLeader_Evaluation_3 = 0;
                    // [18] TeamLeader_Evaluation_4(comment)
                    reports.TeamLeader_Evaluation_4 = String.Empty;
                    #endregion

                    #region  + [3] 임원 평가
                    // [19] Director_Evaluation_1(일정준수)
                    reports.Director_Evaluation_1 = 0;
                    // [20] Director_Evaluation_2(업무수행도)
                    reports.Director_Evaluation_2 = 0;
                    // [21] Director_Evaluation_3(결과물)
                    reports.Director_Evaluation_3 = 0;
                    // [22] Director_Evaluation_4(comment)
                    reports.Director_Evaluation_4 = String.Empty;
                    #endregion

                    // [23] Total_Score(종합점수)
                    reports.Total_Score = 0;
                    var rid = await reportDbRepository.AddAsync(reports);
                    report.Rid = rid;
                }
                //IsCreateReportStatus = true;
            }
        }
        #endregion

        #region + 웹페이지 줄바꿈 처리
        // https://stackoverflow.com/questions/64157834/how-can-i-have-new-line-in-blazor
        /// <summary>
        /// 웹페이지 줄바꿈 처리하는 메서드
        /// </summary>
        /// <param name="contenct">string contenct</param>
        /// <returns>string replaceString</returns>
        private string replaceString(string contenct)
        {
            return Regex.Replace(HttpUtility.HtmlEncode(contenct), "\r?\n|\r", "<br />");
        }
        #endregion

        /// <summary>
        /// Toggle 이벤트
        /// </summary>
        private void Toggle()
        {
            Collapsed = !Collapsed;
        }
    }
}
