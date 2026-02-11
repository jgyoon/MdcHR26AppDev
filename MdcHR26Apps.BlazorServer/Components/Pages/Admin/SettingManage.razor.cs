using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.Department;
using MdcHR26Apps.Models.Rank;
using MdcHR26Apps.Models.HRSetting;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin;

public partial class SettingManage(
    IEDepartmentRepository eDepartmentRepository,
    IERankRepository eRankRepository,
    IHRSettingRepository hRSettingRepository,
    LoginStatusService loginStatusService,
    UrlActions urlActions)
{
    // 탭 상태
    private string activeTab = "dept";

    // 부서 관리
    private List<EDepartmentDb> deptLists { get; set; } = new List<EDepartmentDb>();

    // 직급 관리
    private List<ERankDb> rankLists { get; set; } = new List<ERankDb>();

    // 시스템 설정
    private HRSettingDb? currentSetting { get; set; } = null;

    protected override async Task OnInitializedAsync()
    {
        await CheckLogined();
        await LoadData();
        StateHasChanged();
    }

    private async Task LoadData()
    {
        await Task.Delay(1);
        var depts = await eDepartmentRepository.GetByAllAsync();
        var ranks = await eRankRepository.GetByAllAsync();
        deptLists = depts.ToList();
        rankLists = ranks.ToList();

        // 시스템 설정 조회
        var setting = await hRSettingRepository.GetCurrentAsync();
        if (setting == null)
        {
            // 초기 레코드 생성
            await hRSettingRepository.InitializeAsync();
            setting = await hRSettingRepository.GetCurrentAsync();
        }
        currentSetting = setting;
    }

    #region Tab Management
    private void SetActiveTab(string tab)
    {
        activeTab = tab;
    }
    #endregion

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

    #region Department Methods
    private string GetDeptName(string? deptName)
    {
        return !string.IsNullOrEmpty(deptName) ? deptName : string.Empty;
    }

    private void MoveDeptCreatePage()
    {
        urlActions.MoveDeptCreatePage();
    }

    private void MoveDeptDetailsPage(Int64 eDepartId)
    {
        urlActions.MoveDeptDetailsPage(eDepartId);
    }
    #endregion

    #region Rank Methods
    private string GetRankName(string? rankName)
    {
        return !string.IsNullOrEmpty(rankName) ? rankName : string.Empty;
    }

    private void MoveRankCreatePage()
    {
        urlActions.MoveRankCreatePage();
    }

    private void MoveRankDetailsPage(Int64 eRankId)
    {
        urlActions.MoveRankDetailsPage(eRankId);
    }
    #endregion

    #region Common Methods
    private string GetStatusText(bool isActive)
    {
        return isActive ? "사용중" : "사용안함";
    }
    #endregion

    #region System Setting Methods
    /// <summary>
    /// 평가 오픈 토글
    /// </summary>
    private async Task UpdateEvaluationOpen()
    {
        if (currentSetting == null) return;

        await hRSettingRepository.UpdateAsync(currentSetting);
        StateHasChanged();
    }

    /// <summary>
    /// 평가 수정 토글
    /// </summary>
    private async Task UpdateEditOpen()
    {
        if (currentSetting == null) return;

        await hRSettingRepository.UpdateAsync(currentSetting);
        StateHasChanged();
    }
    #endregion
}
