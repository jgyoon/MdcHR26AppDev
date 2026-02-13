using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.Views.v_DeptObjectiveListDb;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.DeptObjective;

public partial class Main
{
    // 부서목표리스트View
    [Inject]
    public Iv_DeptObjectiveListRepository vDeptObjectiveListDbRepository { get; set; } = null!;
    public List<v_DeptObjectiveListDb> objectList { get; set; } = new List<v_DeptObjectiveListDb>();

    // 로그인관리(상태관리)
    [Inject]
    public LoginStatusService loginStatusService { get; set; } = null!;
    // 페이지이동
    [Inject]
    public UrlActions urlActions { get; set; } = null!;

    // 기타
    public bool isAdmin { get; set; } = false;
    public bool isMain { get; set; } = true;

    protected override async Task OnInitializedAsync()
    {
        await CheckLogined();
        await SetData();
        StateHasChanged();
    }

    private async Task SetData()
    {
        await Task.Delay(1);
        isAdmin = loginStatusService.IsloginAndIsAdminCheck();
        // 메디칼파크 EDepartId : 1
        objectList = (await vDeptObjectiveListDbRepository.GetByDepartmentAsync(1)).ToList();
    }

    #region + [0].[1] CheckLogined : IsloginCheck()
    private async Task CheckLogined()
    {
        await Task.Delay(0);
        if (!loginStatusService.IsloginCheck())
        {
            StateHasChanged();
            urlActions.MoveMainPage();
        }
    }
    #endregion

    #region + [15].[3] MoveMainObjectiveCreatePage : 전산목표 생성페이지 이동
    public void MoveMainObjectiveCreatePage()
    {
        urlActions.MoveMainObjectiveCreatePage();
    }
    #endregion
}
