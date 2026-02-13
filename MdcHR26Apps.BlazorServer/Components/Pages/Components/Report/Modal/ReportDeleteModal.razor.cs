using MdcHR26Apps.Models.EvaluationReport;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Report.Modal;

public partial class ReportDeleteModal
{
    #region Inject
    [Inject] private IReportRepository reportRepository { get; set; } = null!;
    #endregion

    #region Parameters
    [Parameter] public ReportDb? model { get; set; }
    [Parameter] public EventCallback OnDeleteSuccess { get; set; }
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

    private async Task DeleteReport()
    {
        if (model != null)
        {
            var result = await reportRepository.DeleteAsync(model.Rid);
            if (result > 0)
            {
                resultText = "삭제되었습니다.";
                await Task.Delay(1000);
                await OnDeleteSuccess.InvokeAsync();
                Close();
            }
            else
            {
                resultText = "삭제 실패했습니다.";
            }
        }
    }
    #endregion
}
