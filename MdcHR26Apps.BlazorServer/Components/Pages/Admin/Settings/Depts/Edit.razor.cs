using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.Department;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin.Settings.Depts;

public partial class Edit(
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
        else
        {
            resultText = "부서 정보를 찾을 수 없습니다.";
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

    #region Update Department
    private async Task UpdateDepartment()
    {
        if (string.IsNullOrEmpty(model.EDepartmentName) || model.EDepartmentNo < 1)
        {
            resultText = "부서명과 부서번호(1 이상)는 필수입니다.";
            return;
        }

        // 중복 체크 (자기 자신 제외)
        var existing = await eDepartmentRepository.GetByDepartmentNoAsync(model.EDepartmentNo);
        if (existing != null && existing.EDepartId != model.EDepartId)
        {
            resultText = $"부서번호 {model.EDepartmentNo}는 이미 사용 중입니다.";
            return;
        }

        var result = await eDepartmentRepository.UpdateAsync(model);

        if (result > 0)
        {
            resultText = "부서 수정 성공";
            StateHasChanged();
            await Task.Delay(1000);
            urlActions.MoveDeptDetailsPage(Id);
        }
        else
        {
            resultText = "부서 수정 실패";
        }
    }
    #endregion

    #region Page Navigation
    private void MoveDetailsPage()
    {
        urlActions.MoveDeptDetailsPage(Id);
    }
    #endregion
}
