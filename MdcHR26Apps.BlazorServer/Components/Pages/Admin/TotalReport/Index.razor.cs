using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.BlazorServer.Utils;
using MdcHR26Apps.Models.Views.v_ProcessTRListDB;
using MdcHR26Apps.Models.Views.v_ReportTaskListDB;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin.TotalReport;

public partial class Index(
    Iv_ProcessTRListRepository processTRRepository,
    Iv_ReportTaskListRepository reportTaskRepository,
    LoginStatusService loginStatusService,
    UrlActions urlActions,
    UserUtils utils)
{
    private List<v_ProcessTRListDB>? processTRLists;
    private List<v_ProcessTRListDB>? allProcessTRLists;
    private List<v_ReportTaskListDB>? reportTaskLists;
    private string searchTerm = string.Empty;
    private string comment = string.Empty;
    public List<string> deptlist = new List<string>();
    public string selectedDept { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await CheckLogined();
        await LoadData();
    }

    private async Task CheckLogined()
    {
        await Task.CompletedTask;
        if (!loginStatusService.IsloginAndIsAdminCheck())
        {
            StateHasChanged();
            urlActions.MoveMainPage();
        }
    }

    private async Task LoadData()
    {
        allProcessTRLists = await processTRRepository.GetAllAsync();
        processTRLists = allProcessTRLists;
        reportTaskLists = await reportTaskRepository.GetAllAsync();
        deptlist = await utils.GetDeptListAsync();
    }

    private void HandleSearchValueChanged(string value)
    {
        searchTerm = value;
        SearchAction();
    }

    private void SearchInit()
    {
        searchTerm = string.Empty;
        selectedDept = string.Empty;
        processTRLists = allProcessTRLists;
    }

    private void SearchAction()
    {
        if (string.IsNullOrEmpty(searchTerm))
        {
            processTRLists = allProcessTRLists;
            return;
        }

        processTRLists = allProcessTRLists?
            .Where(x =>
                (x.UserId != null && x.UserId.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (x.UserName != null && x.UserName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
            .ToList();
    }

    private void HandleCommentChanged(string value)
    {
        comment = value;
        StateHasChanged();
    }

    #region + [9].[2] 검색로직 추가(부서)
    private async Task SearchDept()
    {
        if (!String.IsNullOrWhiteSpace(selectedDept))
        {
            // 2026년: 클라이언트에서 EDepartmentName 필터링
            var allUsers = await processTRRepository.GetByAllAsync();
            processTRLists = allUsers
                .Where(u => u.EDepartmentName.Contains(selectedDept))
                .ToList();
        }
        else
        {
            await LoadData();
        }
    }
    private async Task HandleDeptValueChanged(string newSearchValue)
    {
        selectedDept = newSearchValue;
        await SearchDept();
    }
    #endregion
}
