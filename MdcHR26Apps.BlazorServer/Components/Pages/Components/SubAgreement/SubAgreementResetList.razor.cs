using MdcHR26Apps.Models.EvaluationSubAgreement;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.SubAgreement;

public partial class SubAgreementResetList
{
    #region Parameters
    [Parameter] public List<SubAgreementDb> reportDbList { get; set; } = new();
    #endregion

    #region Variables
    private int sortNo = 1;
    #endregion

    protected override void OnParametersSet()
    {
        sortNo = 1;
        base.OnParametersSet();
    }
}
