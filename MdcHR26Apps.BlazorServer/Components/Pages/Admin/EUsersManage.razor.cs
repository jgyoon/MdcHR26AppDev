using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.Views.v_EvaluationUsersList;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using MdcHR26Apps.BlazorServer.Utils;
using MdcHR26Apps.BlazorServer.Components.Pages.Components.Common;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin;

public partial class EUsersManage(
    Iv_EvaluationUsersListRepository evaluationUsersListRepository,
    LoginStatusService loginStatusService,
    UrlActions urlActions,
    UserUtils utils)
{
    // 평가사용자 관리
    private List<v_EvaluationUsersList> userlist { get; set; } = new();

    // 검색창 추가
    private string searchTerm { get; set; } = string.Empty;
    public List<string> deptlist = new List<string>();
    public string selectedDept { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await CheckLogined();
        await SetData();
        StateHasChanged();
    }

    private async Task SetData()
    {
        var users = await evaluationUsersListRepository.GetByAllAsync();
        userlist = users.ToList();
        deptlist = await utils.GetDeptListAsync();
    }

    #region Login Check
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

    #region Page Navigation
    private void MoveMainPage()
    {
        urlActions.MoveMainPage();
    }
    #endregion

    #region Search Logic
    private async Task Search()
    {
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var allUsers = await evaluationUsersListRepository.SearchByNameAsync(searchTerm);
            userlist = allUsers.ToList();
        }
        else
        {
            await SetData();
        }
    }

    private async Task SearchInit()
    {
        searchTerm = string.Empty;
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
        searchTerm = newSearchValue;
        await Search();
    }
    #endregion

    #region + [9].[2] 검색로직 추가(부서)
    private async Task SearchDept()
    {
        if (!String.IsNullOrWhiteSpace(selectedDept))
        {
            // 2026년: 클라이언트에서 EDepartmentName 필터링
            var allUsers = await evaluationUsersListRepository.GetByAllAsync();
            userlist = allUsers
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
