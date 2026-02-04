using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.EvaluationProcess;
using MdcHR26Apps.Models.Views.v_AgreementDB;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Agreement.TeamLeader
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

        // 직무합의관리 (View 사용)
        [Inject]
        public Iv_AgreementRepository v_agreementRepository { get; set; } = null!;
        public List<v_AgreementDB> model { get; set; } = new List<v_AgreementDB>();

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

        private async Task SetData(Int64 Id)
        {
            var loadedProcessDb = await processDbRepository.GetByIdAsync(Id);
            if (loadedProcessDb == null)
            {
                resultText = "데이터를 찾을 수 없습니다.";
                return;
            }

            processDb = loadedProcessDb;
            model = await v_agreementRepository.GetByUserIdAsync(processDb.Uid);

            // userName은 v_AgreementDB에서 가져옴 (UserName 포함)
            userName = model.FirstOrDefault()?.UserName ?? string.Empty;

            Old_Agreement_Comment =
                !String.IsNullOrEmpty(processDb.Agreement_Comment) ? processDb.Agreement_Comment : String.Empty;
            if (!String.IsNullOrEmpty(processDb.Agreement_Comment))
            {
                processDb.Agreement_Comment = String.Empty;
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
            if (String.IsNullOrEmpty(processDb.Agreement_Comment))
            {
                resultText = "합의 코멘트를 입력하여 주세요.";
                return;
            }

            //processDb.Agreement_Comment = processDb.Agreement_Comment + "[" + DateTime.Now.ToString() + "]";
            if (!String.IsNullOrEmpty(Old_Agreement_Comment))
            {
                processDb.Agreement_Comment =
                    Old_Agreement_Comment + "\r\n​"
                    + "[승인]" + processDb.Agreement_Comment
                    + "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", cultureInfo) + "]";
            }
            else
            {
                processDb.Agreement_Comment =
                    "[승인]" + processDb.Agreement_Comment
                    + "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", cultureInfo) + "]";
            }
            processDb.Is_Agreement = true;

            int updateStatus = await processDbRepository.UpdateAsync(processDb);

            if (updateStatus > 0)
            {
                resultText = "합의 승인 성공";
                StateHasChanged();
                urlActions.MoveAgreementTeamLeaderIndexPage();
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
            if (String.IsNullOrEmpty(processDb.Agreement_Comment))
            {
                resultText = "합의 코멘트를 입력하여 주세요.";
                return;
            }

            //processDb.Agreement_Comment = processDb.Agreement_Comment + "[" + DateTime.Now.ToString() + "]";
            if (!String.IsNullOrEmpty(Old_Agreement_Comment))
            {
                processDb.Agreement_Comment =
                    Old_Agreement_Comment + "\r\n​"
                    + "[반려]" + processDb.Agreement_Comment
                    + "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", cultureInfo) + "]";
            }
            else
            {
                processDb.Agreement_Comment =
                    "[반려]" + processDb.Agreement_Comment
                    + "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", cultureInfo) + "]";
            }
            processDb.Is_Request = false;

            int updateStatus = await processDbRepository.UpdateAsync(processDb);

            if (updateStatus > 0)
            {
                resultText = "합의 반려 성공";
                StateHasChanged();
                urlActions.MoveAgreementTeamLeaderIndexPage();
            }
            else
            {
                resultText = "합의 반려 실패";
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
