using MdcHR26Apps.Models.EvaluationReport;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Report.ViewPage;

public partial class DirectorReportListView
{
    #region Inject
    [Inject] private IReportRepository reportRepository { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    #endregion

    #region Parameters
    [Parameter] public long Uid { get; set; }
    #endregion

    #region Variables
    private List<ReportDb> reports = new();
    #endregion

    #region Lifecycle
    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }
    #endregion

    #region Methods
    private async Task LoadData()
    {
        var allReports = await reportRepository.GetByUidAllAsync(Uid);
        reports = allReports.Where(r => !string.IsNullOrEmpty(r.TeamLeader_Evaluation_4) && string.IsNullOrEmpty(r.Director_Evaluation_4)).ToList();
    }

    private void HandleEdit(long rid)
    {
        NavigationManager.NavigateTo($"/report/director/edit/{rid}");
    }
    #endregion
}
