using MdcHR26Apps.Models.EvaluationAgreement;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Agreement;

public partial class AgreementListTable
{
    #region Parameters
    [Parameter] public List<AgreementDb> agreements { get; set; } = new();
    [Parameter] public EventCallback<long> OnEdit { get; set; }
    #endregion
}
