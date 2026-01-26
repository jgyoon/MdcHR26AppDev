using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.Rank;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin.Settings.Ranks;

public partial class Create(
    IERankRepository _eRankRepository,
    LoginStatusService loginStatusService,
    UrlActions urlActions)
{
    // 직급관리
    private ERankDb model { get; set; } = new ERankDb();

    // 기타
    private string resultText { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await CheckLogined();
        await SetData();
        StateHasChanged();
    }

    private async Task SetData()
    {
        await Task.Delay(1);
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
}
