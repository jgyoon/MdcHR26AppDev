using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.Department;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Pages.Admin.Settings.Dept;

public partial class Delete(
    IEDepartmentRepository eDepartmentRepository,
    LoginStatusService loginStatusService,
    UrlActions urlActions)
{
    #region Parameters
    [Parameter]
    public Int64 Id { get; set; }
    #endregion

    // 부서관리
    private EDepartmentDb model { get; set; } = new EDepartmentDb();

    // 기타
    private string resultText { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await CheckLogined();
        await SetData(Id);
        await base.OnInitializedAsync();
    }

    private async Task SetData(Int64 Id)
    {
        var result = await eDepartmentRepository.GetByIdAsync(Id);
        if (result != null)
        {
            model = result;
        }
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
