using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.Views.v_MemberListDB;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using MdcHR26Apps.BlazorServer.Utils;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin
{
    public partial class UserManage
    {
        // 사용자 관리 - 2026년: v_MemberListDB 뷰 사용 (EDepartmentName, ERank 표시)
        [Inject]
        public Iv_MemberListRepository memberListRepository { get; set; } = null!;
        public List<v_MemberListDB>? userlist { get; set; } = new List<v_MemberListDB>();
        // 로그인관리(상태관리)
        [Inject]
        public LoginStatusService loginStatusService { get; set; } = null!;
        // 페이지이동
        [Inject]
        public UrlActions urlActions { get; set; } = null!;

        // 검색창 추가
        private string searchTerm { get; set; } = string.Empty;
        // 공용함수 호출
        [Inject]
        public UserUtils utils { get; set; } = null!;
        public List<string> deptlist = new List<string>();
        public string selectedDept { get; set; } = string.Empty;        

        #region + State 클래스 사용
        // State 클래스 사용
        private class UserManageState
        {
            public List<v_MemberListDB>? Userlist { get; set; }
            public string? SearchTerm { get; set; }
        }

        private UserManageState state = new UserManageState();
        #endregion

        protected override async Task OnInitializedAsync()
        {
            await CheckLogined();
            await SetData();
            StateHasChanged();
        }

        private async Task SetData()
        {
            var users = await memberListRepository.GetByAllAsync();
            state.Userlist = users.ToList();
            deptlist = await utils.GetDeptListAsync();
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

        #region + [3] UserCreatePage : 사용자 생성페이지 이동
        protected void UserCreatePage()
        {
            urlActions.MoveUserCreatePage();
        }
        #endregion

        #region + [9].[1] 검색로직 추가
        private async Task Search()
        {
            if (!String.IsNullOrWhiteSpace(state.SearchTerm))
            {
                // 2026년: 클라이언트에서 UserName 필터링
                var allUsers = await memberListRepository.GetByAllAsync();
                state.Userlist = allUsers
                    .Where(u => u.UserName.Contains(state.SearchTerm))
                    .ToList();
            }
            else
            {
                await SetData();
            }
        }

        private async Task SearchInit()
        {
            state.SearchTerm = string.Empty;
            selectedDept = string.Empty;
            await SetData();
        }

        private async Task SearchEnterKeyPress(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await Search();
            }
        }

        private async Task HandleSearchValueChanged(string newSearchValue)
        {
            state.SearchTerm = newSearchValue;
            await Search();
        }


        #endregion

        #region + [9].[2] 검색로직 추가(부서)
        private async Task SearchDept()
        {
            if (!String.IsNullOrWhiteSpace(selectedDept))
            {
                // 2026년: 클라이언트에서 EDepartmentName 필터링
                var allUsers = await memberListRepository.GetByAllAsync();
                state.Userlist = allUsers
                    .Where(u => u.EDepartmentName.Contains(selectedDept))
                    .ToList();
            }
            else
            {
                await SetData();
            }
        }
        private async Task HandleDeptValueChanged(string newSearchValue)
        {
            selectedDept = newSearchValue;
            await SearchDept();
        }
        #endregion
    }
}
