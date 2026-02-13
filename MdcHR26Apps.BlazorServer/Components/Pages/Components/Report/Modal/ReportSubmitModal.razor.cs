using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Report.Modal;

public partial class ReportSubmitModal
{
    #region Parameters
    [Parameter] public EventCallback OnSubmit { get; set; }
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

    private async Task Submit()
    {
        resultText = "제출되었습니다.";
        await Task.Delay(1000);
        await OnSubmit.InvokeAsync();
        Close();
    }
    #endregion
}
