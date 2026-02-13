using MdcHR26Apps.Models.EvaluationSubAgreement;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.SubAgreement.Modal;

public partial class SubAgreementDeleteModal
{
    #region Parameters
    [Parameter] public SubAgreementDb? model { get; set; }
    [Parameter] public EventCallback OnDeleteSuccess { get; set; }
    [Parameter] public EventCallback OnClose { get; set; }
    #endregion

    #region Inject Services
    [Inject] private ISubAgreementRepository subAgreementRepository { get; set; } = null!;
    #endregion

    #region Modal Variables
    private string ModalDisplay = "none;";
    private string ModalClass = "";
    private bool ShowBackdrop = false;
    private string resultText = string.Empty;
    #endregion

    #region Modal Functions
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
    #endregion

    #region Business Logic
    private async Task DeleteUserSubAgreement()
    {
        if (model != null)
        {
            var result = await subAgreementRepository.DeleteAsync(model.Sid);

            if (result)
            {
                resultText = "?? œ?˜ì—ˆ?µë‹ˆ??";
                await Task.Delay(1000);
                await OnDeleteSuccess.InvokeAsync();
                Close();
            }
            else
            {
                resultText = "?? œ ?¤íŒ¨?ˆìŠµ?ˆë‹¤.";
            }
        }
    }
    #endregion
}
