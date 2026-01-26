using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.Department;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Pages.Admin.Settings.Dept;

public partial class Create(
    IEDepartmentRepository _eDepartmentRepository,
    LoginStatusService loginStatusService,
    UrlActions urlActions)
{
    // 부서관리
    private EDepartmentDb model { get; set; } = new EDepartmentDb();

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
