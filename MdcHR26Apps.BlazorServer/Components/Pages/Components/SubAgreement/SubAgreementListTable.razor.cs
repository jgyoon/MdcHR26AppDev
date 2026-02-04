using MdcHR26Apps.Models.EvaluationSubAgreement;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.SubAgreement;

public partial class SubAgreementListTable
{
    #region Parameters
    [Parameter] public List<SubAgreementDb> subAgreements { get; set; } = new();
    [Parameter] public EventCallback<long> OnEdit { get; set; }
    #endregion
}
