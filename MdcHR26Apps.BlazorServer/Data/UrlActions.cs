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

    // === [3] 부서 관리 (Dept) - 2026년 경로 변경: /Admin/Dept → /Admin/Settings/Dept ===
    public void MoveDeptCreatePage() => _navigationManager.NavigateTo("/Admin/Settings/Dept/Create");
    public void MoveDeptDetailsPage(long deptId) => _navigationManager.NavigateTo($"/Admin/Settings/Dept/Details/{deptId}");
    public void MoveDeptEditPage(long deptId) => _navigationManager.NavigateTo($"/Admin/Settings/Dept/Edit/{deptId}");
    public void MoveDeptDeletePage(long deptId) => _navigationManager.NavigateTo($"/Admin/Settings/Dept/Delete/{deptId}");

    // === [4] 직급 관리 (Rank) - 2026년 신규 ===
    public void MoveRankCreatePage() => _navigationManager.NavigateTo("/Admin/Settings/Rank/Create");
    public void MoveRankDetailsPage(long rankId) => _navigationManager.NavigateTo($"/Admin/Settings/Rank/Details/{rankId}");
    public void MoveRankEditPage(long rankId) => _navigationManager.NavigateTo($"/Admin/Settings/Rank/Edit/{rankId}");
    public void MoveRankDeletePage(long rankId) => _navigationManager.NavigateTo($"/Admin/Settings/Rank/Delete/{rankId}");

    // === [5] 평가대상자 관리 (EvaluationUsers) ===
    public void MoveEUserManagePage() => _navigationManager.NavigateTo("/Admin/EUsersManage");
    public void MoveEUserDetailsPage(long uid) => _navigationManager.NavigateTo($"/Admin/EvaluationUsers/Details/{uid}");
    public void MoveEUsersEditPage(long uid) => _navigationManager.NavigateTo($"/Admin/EvaluationUsers/Edit/{uid}");
}
