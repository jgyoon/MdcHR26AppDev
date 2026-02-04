using MdcHR26Apps.Models.EvaluationReport;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Report.Modal;

public partial class ReportInitModal
{
    #region Inject
    [Inject] private IReportRepository reportRepository { get; set; } = null!;
    #endregion

    #region Parameters
    [Parameter] public long Uid { get; set; }
    [Parameter] public EventCallback OnInitSuccess { get; set; }
    [Parameter] public EventCallback OnClose { get; set; }
    #endregion

    #region Modal Variables
    private string ModalDisplay = "none;";
    private string ModalClass = "";
    private bool ShowBackdrop = false;
    private string resultText = string.Empty;
    #endregion

    #region Methods
    public void Open()
    {
        ModalDisplay = "block;";
        ModalClass = "show";
        ShowBackdrop = true;
        StateHasChanged();
    }

    private void Close()
    {
        ModalDisplay = "none";
        ModalClass = "";
        ShowBackdrop = false;
        resultText = string.Empty;
        OnClose.InvokeAsync();
    }

    private async Task InitReports()
    {
        var reports = await reportRepository.GetByUidAllAsync(Uid);
        foreach (var report in reports)
        {
            await reportRepository.DeleteAsync(report.Rid);
        }
        resultText = "초기화되었습니다.";
        await Task.Delay(1000);
        await OnInitSuccess.InvokeAsync();
        Close();
    }
    #endregion
}
