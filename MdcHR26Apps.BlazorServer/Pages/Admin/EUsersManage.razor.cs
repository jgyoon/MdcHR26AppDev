using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.EvaluationUsers;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Pages.Admin;

public partial class EUsersManage(
    IEvaluationUsersRepository evaluationUsersRepository,
    LoginStatusService loginStatusService,
    UrlActions urlActions)
{
    // 평가사용자 관리
    private List<Models.EvaluationUsers.EvaluationUsers> userlist { get; set; } = new List<Models.EvaluationUsers.EvaluationUsers>();

    // 검색창 추가
    private string searchTerm { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await CheckLogined();
        await SetData();
        StateHasChanged();
    }

    private async Task SetData()
    {
        var users = await evaluationUsersRepository.GetByAllAsync();
        userlist = users.ToList();
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
            // 2026년: 검색은 UserName 기반이므로 전체 조회 후 필터링
            var allUsers = await evaluationUsersRepository.GetByAllAsync();
            // TODO: Navigation Property를 통한 UserName 검색 필요
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
}
