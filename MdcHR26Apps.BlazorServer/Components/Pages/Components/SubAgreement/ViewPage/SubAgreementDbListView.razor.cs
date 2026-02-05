using MdcHR26Apps.Models.EvaluationSubAgreement;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.SubAgreement.ViewPage;

public partial class SubAgreementDbListView
{
    #region Inject
    [Inject] private ISubAgreementRepository subAgreementRepository { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    #endregion

    #region Parameters
    [Parameter] public long Uid { get; set; }
    [Parameter] public List<SubAgreementDb>? subAgreementDbs { get; set; }
    #endregion

    #region Variables
    private List<SubAgreementDb> subAgreements = new();
    #endregion

    #region Lifecycle
    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }
    #endregion

    #region Methods
    private async Task LoadData()
    {
        if (subAgreementDbs != null)
        {
            subAgreements = subAgreementDbs;
            return;
        }

        if (Uid != 0)
        {
            subAgreements = await subAgreementRepository.GetByUidAllAsync(Uid);
        }
    }

    private void HandleDetailsClick(long sid)
    {
        NavigationManager.NavigateTo($"/SubAgreement/User/Details/{sid}");
    }
    #endregion
}
