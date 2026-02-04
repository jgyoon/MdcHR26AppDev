using MdcHR26Apps.Models.EvaluationReport;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Report.ViewPage;

public partial class FeedBack_TotalReportListView
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
        reports = (await reportRepository.GetByUidAllAsync(Uid)).ToList();
    }

    private void HandleEdit(long rid)
    {
        NavigationManager.NavigateTo($"/report/feedback/{rid}");
    }
    #endregion
}
