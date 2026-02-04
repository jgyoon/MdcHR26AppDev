using MdcHR26Apps.Models.EvaluationAgreement;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Agreement;

public partial class AgreementDbListTable
{
    #region Parameters
    [Parameter] public List<AgreementDb> agreementDbs { get; set; } = new();
    [Parameter] public EventCallback<long> OnDetailsClick { get; set; }
    #endregion

    #region Variables
    private int sortNo = 1;
    #endregion

    #region Helper Methods
    private string sortNoAdd2(int num)
    {
        return (num).ToString();
    }
    #endregion
}
