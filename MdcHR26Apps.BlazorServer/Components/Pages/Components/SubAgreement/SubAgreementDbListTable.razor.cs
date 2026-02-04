using MdcHR26Apps.Models.EvaluationSubAgreement;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.SubAgreement;

public partial class SubAgreementDbListTable
{
    #region Parameters
    [Parameter] public List<SubAgreementDb> subAgreementDbs { get; set; } = new();
    [Parameter] public EventCallback<long> OnDetailsClick { get; set; }
    #endregion

    #region Variables
    private int sortNo = 1;
    #endregion
}
