using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.EvaluationReport;
using Microsoft.AspNetCore.Components;
using System.Text.RegularExpressions;
using System.Web;

namespace MdcHR26Apps.BlazorServer.Components.Pages._1st_HR_Report
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

        // 평가리스트 작성
        [Inject]
        public IReportRepository reportDbRepository { get; set; } = null!;
        public ReportDb model { get; set; } = new ReportDb();

        protected override async Task OnInitializedAsync()
        {
            await CheckLogined();
            await SetData(Id);
            await base.OnInitializedAsync();
        }

        private async Task SetData(long Id)
        {
            model = await reportDbRepository.GetByIdAsync(Id);
        }

        #region + 로그인 여부 확인
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

        private string htmlString(string content)
        {
            return content.Replace(". ", "<br>");
        }

        private string htmlString2(string content)
        {
            return content.Replace(System.Environment.NewLine, "<br>");
        }

        // https://stackoverflow.com/questions/64157834/how-can-i-have-new-line-in-blazor
        private string replaceString(string contenct)
        {
            return Regex.Replace(HttpUtility.HtmlEncode(contenct), "\r?\n|\r", "<br />");
        }

        #region + [4].[1] MoveReportMainPage : 본인평가페이지 이동
        public void MoveReportMainPage()
        {
            urlActions.MoveReportMainPage();
        }
        #endregion

        #region + [4].[4] MoveReportEditPage : 평가 수정페이지 이동
        public void MoveReportEditPage(long Rid)
        {
            urlActions.Move1stReportEditPage(Rid);
        }
        #endregion

        #region + [4].[5] MoveReportDeletePage : 평가 삭제페이지 이동
        public void MoveReportDeletePage(long Rid)
        {
            // 26년도에는 Delete 페이지가 없으므로 Details 페이지로 이동
            urlActions.Move1stReportDetailsPage(Rid);
        }
        #endregion
    }
}
