using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.EvaluationProcess;
using MdcHR26Apps.Models.EvaluationReport;
using MdcHR26Apps.Models.EvaluationSubAgreement;
using MdcHR26Apps.Models.User;
using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace MdcHR26Apps.BlazorServer.Components.Pages.SubAgreement.TeamLeader
{
    public partial class ResetSubAgreement
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

        // 세부직무합의
        [Inject]
        public ISubAgreementRepository subAgreementDbRepository { get; set; } = null!;
        public List<SubAgreementDb> model { get; set; } = new List<SubAgreementDb>();

        // 평가리스트 작성
        [Inject]
        public IReportRepository reportDbRepository { get; set; } = null!;

        // 사용자 정보
        [Inject]
        public IUserRepository userDbRepository { get; set; } = null!;

        // 기타
        public string userName { get; set; } = String.Empty;
        public string resultText { get; set; } = String.Empty;
        public CultureInfo cultureInfo { get; set; } = new CultureInfo("ko-KR");

        protected override async Task OnInitializedAsync()
        {
            await CheckLogined();
            await SetData(Id);
            await base.OnInitializedAsync();
        }

        private async Task SetData(long id)
        {
            processDb = await processDbRepository.GetByIdAsync(id);
            model = await subAgreementDbRepository.GetByUidAllAsync(processDb.Uid);
            var userDb = await userDbRepository.GetByIdAsync(processDb.Uid);
            userName = userDb.UserName;
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

        /// <summary>
        /// 합의 초기화
        /// </summary>
        /// <returns></returns>
        private async Task ResetSubAgreementAction()
        {
            // 프로세스 초기화
            processDb.Is_SubRequest = false;
            processDb.Is_SubAgreement = false;
            processDb.SubAgreement_Comment =
                    processDb.SubAgreement_Comment + "\r\n​"
                    + "[합의초기화 : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", cultureInfo) + "]";
            int updateStatus = await processDbRepository.UpdateAsync(processDb);

            if (updateStatus > 0)
            {
                // 평가표 삭제
                await DeleteReports();
                StateHasChanged();
            }
            //await Task.Delay(10000);
            resultText = "합의 초기화 성공";
            StateHasChanged();
            urlActions.MoveTeamLeaderSubAgreementMainPage();
        }

        /// <summary>
        /// 합의 초기화 취소
        /// </summary>
        private void ResetSubAgreementCencelAction()
        {
            resultText = "합의 초기화 취소";
            StateHasChanged();
            urlActions.MoveTeamLeaderSubAgreementMainPage();
        }

        /// <summary>
        /// 평가표 삭제
        /// </summary>
        /// <returns></returns>
        private async Task DeleteReports()
        {
            if (processDb != null && processDb.Uid > 0)
            {
                // 사용자의 모든 ReportDb 조회
                var reportList = await reportDbRepository.GetByUidAllAsync(processDb.Uid);

                if (reportList != null && reportList.Count > 0)
                {
                    foreach (var item in reportList)
                    {
                        int deleteStatusInt = 0;

                        string deleteResult = String.Empty;

                        deleteStatusInt = await reportDbRepository.DeleteAsync(item.Rid);
                        bool deleteStatus = deleteStatusInt > 0;

                        deleteResult =
                            deleteStatus ?
                            "삭제성공 - " + item.Rid :
                            "삭제실패 - " + item.Rid;
                        resultText = resultText + deleteResult + " | ";
                    }
                }
            }
        }
    }
}
