using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.BlazorServer.Utils;
using MdcHR26Apps.Models.Department;
using MdcHR26Apps.Models.Views.v_MemberListDB;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin.Settings.Depts;

public partial class Details(
    IEDepartmentRepository eDepartmentRepository,
    Iv_MemberListRepository vMemberListRepository,
    LoginStatusService loginStatusService,
    UrlActions urlActions,
    UserUtils utils)
{
    #region Parameters
    [Parameter]
    public Int64 Id { get; set; }
    #endregion

    // 부서관리
    private EDepartmentDb model { get; set; } = new EDepartmentDb();

    // 멤버리스트View
    private List<v_MemberListDB> memberList { get; set; } = new List<v_MemberListDB>();

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

        if (model.EDepartId != 0)
        {
            var members = await vMemberListRepository.GetByDepartmentAsync(model.EDepartId);
            memberList = members.ToList();
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

    #region Page Navigation
    private void MoveSettingManagePage()
    {
        urlActions.MoveSettingManagePage();
    }
    #endregion
}
