using MdcHR26Apps.Models.EvaluationAgreement;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Form;

public partial class FormAgreeTaskCreate
{
    #region Parameters
    [Parameter] public EventCallback<AgreementDb> OnSubmit { get; set; }
    #endregion

    #region Variables
    private AgreementDb newAgreement = new();
    #endregion

    #region Methods
    private async Task HandleValidSubmit()
    {
        await OnSubmit.InvokeAsync(newAgreement);
        newAgreement = new(); // 초기화
    }
    #endregion
}
