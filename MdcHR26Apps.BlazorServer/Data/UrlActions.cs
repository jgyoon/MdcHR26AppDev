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

    // === [7] 직무평가 협의 (Agreement/User) ===
    public void MoveAgreementUserIndexPage() => _navigationManager.NavigateTo("/Agreement/User/Index");
    public void MoveAgreementUserCreatePage() => _navigationManager.NavigateTo("/Agreement/User/Create");
    public void MoveAgreementUserEditPage(long id) => _navigationManager.NavigateTo($"/Agreement/User/Edit/{id}");
    public void MoveAgreementUserDeletePage(long id) => _navigationManager.NavigateTo($"/Agreement/User/Delete/{id}");
    public void MoveAgreementUserDetailsPage(long id) => _navigationManager.NavigateTo($"/Agreement/User/Details/{id}");

    // === [8] 직무평가 협의 (Agreement/TeamLeader) ===
    public void MoveAgreementTeamLeaderIndexPage() => _navigationManager.NavigateTo("/Agreement/TeamLeader/Index");
    public void MoveAgreementTeamLeaderDetailsPage(long id) => _navigationManager.NavigateTo($"/Agreement/TeamLeader/Details/{id}");

    // === [9] 세부직무평가 협의 (SubAgreement/User) ===
    public void MoveUserSubAgreementMainPage() => _navigationManager.NavigateTo("/SubAgreement/User/Index");
    public void MoveUserSubAgreementCreatePage() => _navigationManager.NavigateTo("/SubAgreement/User/Create");
    public void MoveUserSubAgreementEditPage(long sid) => _navigationManager.NavigateTo($"/SubAgreement/User/Edit/{sid}");
    public void MoveUserSubAgreementDeletePage(long sid) => _navigationManager.NavigateTo($"/SubAgreement/User/Delete/{sid}");
    public void MoveUserSubAgreementDetailsPage(long sid) => _navigationManager.NavigateTo($"/SubAgreement/User/Details/{sid}");

    // === [10] 세부직무평가 협의 (SubAgreement/TeamLeader) ===
    public void MoveTeamLeaderSubAgreementIndexPage() => _navigationManager.NavigateTo("/SubAgreement/TeamLeader/Index");
    public void MoveTeamLeaderSubAgreementMainPage() => _navigationManager.NavigateTo("/SubAgreement/TeamLeader/Index");
    public void MoveTeamLeaderSubAgreementDetailsPage(long pid) => _navigationManager.NavigateTo($"/SubAgreement/TeamLeader/Details/{pid}");
    public void MoveTeamLeaderSubAgreementSubDetailsPage(long sid) => _navigationManager.NavigateTo($"/SubAgreement/TeamLeader/SubDetails/{sid}");
    public void MoveTeamLeaderResetSubAgreementPage(long pid) => _navigationManager.NavigateTo($"/SubAgreement/TeamLeader/ResetSubAgreement/{pid}");
    public void MoveTeamLeaderCompleteSubAgreement(long pid) => _navigationManager.NavigateTo($"/SubAgreement/TeamLeader/CompleteSubAgreement/{pid}");

    // === [11] 평가 보고서 (Report) ===
    public void MoveReportMainPage() => _navigationManager.NavigateTo("/Report/User/Index");
}
