using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.Views.v_MemberListDB;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin.Users
{
    public partial class Details
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

        protected override async Task OnInitializedAsync()
        {
            await CheckLogined();
            await SetData(Uid);
            await base.OnInitializedAsync();
        }

        private async Task SetData(Int64 uid)
        {
            model = await memberListRepository.GetByUidAsync(uid) ?? new v_MemberListDB();
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

        #region + Helper Methods : 상태 문자열 변환

        #region + [1] 재직여부
        private string EStatusString(bool status)
        {
            return status ? "재직" : "퇴직";
        }
        #endregion

        #region + [2] 관리자여부
        private string IsAdminString(bool isAdmin)
        {
            return isAdmin ? "관리자" : "일반사용자";
        }
        #endregion

        #region + [3] 2차 평가자여부 체크
        public string isTeamLeader(bool isTeamLeader)
        {
            return isTeamLeader ? "부서장(2차 평가자)" : "해당없음";
        }
        #endregion        

        #region + [4] 3차 평가자여부 체크
        public string isDirector(bool isDirector)
        {
            return isDirector ? "임원(3차 평가자)" : "해당없음";
        }
        #endregion 

        #endregion

        #region + [0] MoveMainPage : 메인페이지 이동
        protected void MoveMainPage()
        {
            urlActions.MoveMainPage();
        }
        #endregion

        #region + [1] UserManagePage : 사용자관리 페이지 이동
        protected void UserManagePage()
        {
            urlActions.MoveUserManagePage();
        }
        #endregion

        #region + [2] UserEditAction : 사용자정보 수정페이지 이동
        private void UserEditAction(Int64 uid)
        {
            urlActions.MoveUserEditPage(uid);
        }
        #endregion

        #region + [3] UserDeleteAction : 사용자정보 삭제페이지 이동
        private void UserDeleteAction(Int64 uid)
        {
            urlActions.MoveUserDeletePage(uid);
        }
        #endregion
    }
}
