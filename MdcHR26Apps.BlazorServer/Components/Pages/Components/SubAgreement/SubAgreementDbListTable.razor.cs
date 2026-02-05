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

    #region Methods
    protected override void OnParametersSet()
    {
        sortNo = 1; // 파라미터가 변경될 때마다 sortNo 리셋
        base.OnParametersSet();
    }
    #endregion
}
