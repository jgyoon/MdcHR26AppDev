using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.Views.v_ProcessTRListDB;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Agreement.TeamLeader
{
    public partial class Index
    {
        // 페이지이동
        [Inject]
        public UrlActions urlActions { get; set; } = null!;

        // 로그인관리(상태관리)
        [Inject]
        public LoginStatusService loginStatusService { get; set; } = null!;

        // v_ProcessTRListDB Repository
        [Inject]
        public Iv_ProcessTRListRepository processRepository { get; set; } = null!;

        public List<v_ProcessTRListDB> processDbList { get; set; } = new();
        public LoginStatus loginUser { get; set; } = new();
        public string errorMessage { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await CheckLogined();
            await LoadData();
            await base.OnInitializedAsync();
        }

        private async Task LoadData()
        {
            loginUser = loginStatusService.LoginStatus;

            // 팀리더가 관할하는 팀원들의 Process 조회 (v_ProcessTRListDB)
            processDbList = await processRepository.GetByTeamLeaderIdAsync(loginUser.LoginUid);
        }

        private async Task CheckLogined()
        {
            await Task.Delay(0);
            if (!loginStatusService.IsloginAndIsTeamLeaderCheck())
            {
                errorMessage = "팀장 권한이 필요합니다.";
                StateHasChanged();
                await Task.Delay(2000);
                urlActions.MoveMainPage();
            }
        }

        #region Navigation Methods

        protected void MoveMainPage()
        {
            urlActions.MoveMainPage();
        }

        protected void HandleDetails(long uid)
        {
            urlActions.MoveAgreementTeamLeaderDetailsPage(uid);
        }

        #endregion
    }
}
