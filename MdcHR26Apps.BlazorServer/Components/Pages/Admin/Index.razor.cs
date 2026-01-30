using MdcHR26Apps.BlazorServer.Data;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin
{
    public partial class Index
    {
        // 페이지이동
        [Inject]
        public UrlActions urlActions { get; set; } = null!;
        // 로그인관리(상태관리)
        [Inject]
        public LoginStatusService loginStatusService { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            await CheckLogined();
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await CheckLogined();
                StateHasChanged();
            }
            else
            {
                StateHasChanged();
            }
        }

        #region + [1] CheckLogined : IsloginAndIsAdminCheck()
        private async Task CheckLogined()
        {
            await Task.Delay(0);
            if (!loginStatusService.IsloginAndIsAdminCheck())
            {
                StateHasChanged();
                urlActions.MoveMainPage();
            }
        }
        #endregion

        #region + [2] MoveMainPage : 메인페이지 이동
        protected void MoveMainPage()
        {
            urlActions.MoveMainPage();
        }
        #endregion

        #region + [3] UserManagePage : 사용자 관리페이지 이동
        protected void UserManagePage()
        {
            urlActions.MoveUserManagePage();
        }
        #endregion

        #region + [4] EUserManagePage : 평가대상자 관리페이지 이동
        protected void EUserManagePage()
        {
            urlActions.MoveEUserManagePage();
        }
        #endregion

        #region + [5] MoveSettingManagePage : 기초정보 관리페이지 이동 (2026년 신규)
        protected void MoveSettingManagePage()
        {
            urlActions.MoveSettingManagePage();
        }
        #endregion

        #region + [6] MoveTotalReportAdminMainPage는 향후 Phase에서 추가 예정
        protected void MoveTotalReportAdminIndexPage()
        {
            urlActions.MoveTotalReportAdminIndexPage();
        }
        #endregion
    }
}
