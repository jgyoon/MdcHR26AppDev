using MdcHR26Apps.Models.EvaluationAgreement;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Agreement;

public partial class AgreementDetailsTable
{
    #region Parameters
    [Parameter] public AgreementDb? agreement { get; set; }
    [Parameter] public EventCallback OnEdit { get; set; }
    [Parameter] public EventCallback OnDelete { get; set; }
    [Parameter] public EventCallback OnBack { get; set; }
    #endregion
}
