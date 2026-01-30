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
    UrlActions urlActions)
{
    private List<v_ProcessTRListDB>? processTRLists;
    private List<v_ProcessTRListDB>? allProcessTRLists;
    private List<v_ReportTaskListDB>? reportTaskLists;
    private string searchTerm = string.Empty;
    private string comment = string.Empty;

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
    }

    private void HandleSearchValueChanged(string value)
    {
        searchTerm = value;
        SearchAction();
    }

    private void SearchInit()
    {
        searchTerm = string.Empty;
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
}
