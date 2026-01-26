using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.User;
using MdcHR26Apps.Models.Views.v_MemberListDB;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Pages.Admin.Users
{
    public partial class Delete
    {
        #region Parameters
        [Parameter]
        public Int64 Uid { get; set; }
        #endregion

        // 페이지이동
        [Inject]
        public UrlActions urlActions { get; set; } = null!;

        // 로그인관리(상태관리)
        [Inject]
        public LoginStatusService loginStatusService { get; set; } = null!;

        // 사용자정보 (뷰 - 읽기용)
        [Inject]
        public Iv_MemberListRepository memberListRepository { get; set; } = null!;
        public v_MemberListDB model { get; set; } = new v_MemberListDB();

        // 기타
        public string resultText { get; set; } = String.Empty;
        public bool showModal = false;

        protected override async Task OnInitializedAsync()
        {
            await CheckLogined();
            await SetData(Uid);
            await base.OnInitializedAsync();
        }

        private async Task CheckLogined()
        {
            await Task.Delay(0);
            if (!loginStatusService.IsloginAndIsAdminCheck())
            {
                StateHasChanged();
                urlActions.MoveMainPage();
            }
        }

        private async Task SetData(Int64 uid)
        {
            model = await memberListRepository.GetByUidAsync(uid) ?? new v_MemberListDB();
        }

        #region + [1] ShowDeleteModal : 삭제 모달 표시
        private async Task ShowDeleteModal()
        {
            await Task.Delay(0);
            showModal = true;
            StateHasChanged();
        }
        #endregion

        #region + Helper Methods : 상태 문자열 변환
        private string EStatusString(bool status)
        {
            return status ? "재직" : "퇴직";
        }

        private string IsAdminString(bool isAdmin)
        {
            return isAdmin ? "관리자" : "일반사용자";
        }
        #endregion

        #region + [2] MoveMainPage : 메인페이지 이동
        protected void MoveMainPage()
        {
            urlActions.MoveMainPage();
        }
        #endregion

        #region + [3] UserManagePage : 사용자관리 페이지 이동
        protected void UserManagePage()
        {
            urlActions.MoveUserManagePage();
        }
        #endregion

        #region + [4] UserDetailsAction : 사용자정보 상세페이지 이동
        private void UserDetailsAction(Int64 uid)
        {
            urlActions.MoveUserDetailsPage(uid);
        }
        #endregion

        #region + [5] UserEditAction : 사용자정보 수정페이지 이동
        private void UserEditAction(Int64 uid)
        {
            urlActions.MoveUserEditPage(uid);
        }
        #endregion
    }
}
