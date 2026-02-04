using MdcHR26Apps.Models.EvaluationSubAgreement;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.SubAgreement;

public partial class SubAgreementDetailsTable
{
    #region Parameters
    [Parameter] public SubAgreementDb? subAgreement { get; set; }
    [Parameter] public EventCallback OnEdit { get; set; }
    [Parameter] public EventCallback OnDelete { get; set; }
    [Parameter] public EventCallback OnBack { get; set; }
    #endregion
}
