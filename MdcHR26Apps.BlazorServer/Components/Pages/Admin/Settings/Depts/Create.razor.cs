using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.Department;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin.Settings.Depts;

public partial class Create(
    IEDepartmentRepository eDepartmentRepository,
    LoginStatusService loginStatusService,
    UrlActions urlActions)
{
    private EDepartmentDb model { get; set; } = new EDepartmentDb();
    private string resultText { get; set; } = string.Empty;
    private string resultDeptNo { get; set; } = string.Empty;
    private string resultDeptNoColor { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await CheckLogined();
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

    private async Task SaveDepartment()
    {
        if (string.IsNullOrEmpty(model.EDepartmentName) || model.EDepartmentNo < 1)
        {
            resultText = "부서명과 부서번호(1 이상)는 필수입니다.";
            return;
        }

        // 중복 체크
        var existing = await eDepartmentRepository.GetByDepartmentNoAsync(model.EDepartmentNo);
        if (existing != null)
        {
            resultText = $"부서번호 {model.EDepartmentNo}는 이미 사용 중입니다.";
            return;
        }

        var result = await eDepartmentRepository.AddAsync(model);

        if (result > 0)
        {
            resultText = "부서 등록 성공";
            StateHasChanged();
            await Task.Delay(1000);
            urlActions.MoveSettingManagePage();
        }
        else
        {
            resultText = "부서 등록 실패";
        }
    }

    private void MoveSettingManagePage()
    {
        urlActions.MoveSettingManagePage();
    }

    #region Department Number Check
    private async Task CheckDepartmentNo(int deptNo)
    {
        // 부서번호가 1 미만이면 return
        if (deptNo < 1)
            return;

        var existing = await eDepartmentRepository.GetByDepartmentNoAsync(deptNo);
        if (existing == null)
        {
            resultDeptNo = "사용가능한 부서번호입니다.";
            resultDeptNoColor = "color:blue";
        }
        else
        {
            resultDeptNo = "이미 사용 중인 부서번호입니다.";
            resultDeptNoColor = "color:red";
        }
    }
    #endregion
}
