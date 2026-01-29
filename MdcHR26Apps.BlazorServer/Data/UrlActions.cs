using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Data;

/// <summary>
/// Primary Constructor 사용 (C# 13)
/// 페이지 이동 통합 관리
/// </summary>
public class UrlActions(NavigationManager navigationManager)
{
    private readonly NavigationManager _navigationManager = navigationManager;

    // === 기본 페이지 ===
    public void MoveMainPage() => _navigationManager.NavigateTo("/");
    public void MoveLoginPage() => _navigationManager.NavigateTo("/auth/login");
    public void MoveLogoutPage() => _navigationManager.NavigateTo("/auth/logout");
    public void MoveAdminPage() => _navigationManager.NavigateTo("/admin");

    // === [1] 기초정보 관리 (Settings) - 2026년 신규 ===
    public void MoveSettingManagePage() => _navigationManager.NavigateTo("/Admin/SettingManage");

    // === [2] 사용자 관리 (User) ===
    public void MoveUserManagePage() => _navigationManager.NavigateTo("/Admin/UserManage");
    public void MoveUserCreatePage() => _navigationManager.NavigateTo("/Admin/Users/Create");
    public void MoveUserDetailsPage(long uid) => _navigationManager.NavigateTo($"/Admin/Users/Details/{uid}");
    public void MoveUserEditPage(long uid) => _navigationManager.NavigateTo($"/Admin/Users/Edit/{uid}");
    public void MoveUserDeletePage(long uid) => _navigationManager.NavigateTo($"/Admin/Users/Delete/{uid}");

    // === [3] 부서 관리 (Depts) - 2026년 경로 변경: /Admin/Dept → /Admin/Settings/Depts ===
    public void MoveDeptCreatePage() => _navigationManager.NavigateTo("/Admin/Settings/Depts/Create");
    public void MoveDeptDetailsPage(long deptId) => _navigationManager.NavigateTo($"/Admin/Settings/Depts/Details/{deptId}");
    public void MoveDeptEditPage(long deptId) => _navigationManager.NavigateTo($"/Admin/Settings/Depts/Edit/{deptId}");
    public void MoveDeptDeletePage(long deptId) => _navigationManager.NavigateTo($"/Admin/Settings/Depts/Delete/{deptId}");

    // === [4] 직급 관리 (Ranks) - 2026년 신규 ===
    public void MoveRankCreatePage() => _navigationManager.NavigateTo("/Admin/Settings/Ranks/Create");
    public void MoveRankDetailsPage(long rankId) => _navigationManager.NavigateTo($"/Admin/Settings/Ranks/Details/{rankId}");
    public void MoveRankEditPage(long rankId) => _navigationManager.NavigateTo($"/Admin/Settings/Ranks/Edit/{rankId}");
    public void MoveRankDeletePage(long rankId) => _navigationManager.NavigateTo($"/Admin/Settings/Ranks/Delete/{rankId}");

    // === [5] 평가대상자 관리 (EvaluationUsers) ===
    public void MoveEUserManagePage() => _navigationManager.NavigateTo("/Admin/EUsersManage");
    public void MoveEUserDetailsPage(long uid) => _navigationManager.NavigateTo($"/Admin/EvaluationUsers/Details/{uid}");
    public void MoveEUsersEditPage(long uid) => _navigationManager.NavigateTo($"/Admin/EvaluationUsers/Edit/{uid}");

    // === [6] 전체 평가 관리 (TotalReport/Admin) ===
    public void MoveTotalReportAdminIndexPage() => _navigationManager.NavigateTo("/Admin/TotalReport");
    public void MoveTotalReportAdminDetailsPage(long pid) => _navigationManager.NavigateTo($"/Admin/TotalReport/Details/{pid}");
    public void MoveTotalReportAdminEditPage(long pid) => _navigationManager.NavigateTo($"/Admin/TotalReport/Edit/{pid}");
    public void MoveTotalReportAdminInitPage(long pid) => _navigationManager.NavigateTo($"/Admin/TotalReport/ReportInit/{pid}");
}
