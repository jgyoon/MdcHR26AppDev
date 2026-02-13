using MdcHR26Apps.Models.EvaluationAgreement;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Agreement.Modal;

public partial class AgreementDeleteModal
{
    #region Parameters
    [Parameter] public AgreementDb? model { get; set; }
    [Parameter] public EventCallback OnDeleteSuccess { get; set; }
    [Parameter] public EventCallback OnClose { get; set; }
    #endregion

    #region Inject Services
    [Inject] private IAgreementRepository agreementRepository { get; set; } = null!;
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
    private async Task DeleteUserAgreement()
    {
        if (model != null)
        {
            var result = await agreementRepository.DeleteAsync(model.Aid);

            if (result)
            {
                resultText = "삭제에 성공했습니다.";
                await Task.Delay(1000);
                await OnDeleteSuccess.InvokeAsync();
                Close();
            }
            else
            {
                resultText = "삭제에 실패했습니다.";
            }
        }
    }
    #endregion
}
