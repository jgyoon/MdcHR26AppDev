using MdcHR26Apps.Models.EvaluationAgreement;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Form;

public partial class FormAgreeTask
{
    #region Parameters
    [Parameter] public AgreementDb agreement { get; set; } = new();
    [Parameter] public EventCallback<AgreementDb> OnChange { get; set; }
    #endregion
}
