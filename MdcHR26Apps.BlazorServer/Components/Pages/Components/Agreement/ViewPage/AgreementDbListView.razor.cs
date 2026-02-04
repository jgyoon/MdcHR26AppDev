using MdcHR26Apps.Models.EvaluationAgreement;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Agreement.ViewPage;

public partial class AgreementDbListView
{
    #region Inject
    [Inject] private IAgreementRepository agreementRepository { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    #endregion

    #region Parameters
    [Parameter] public long Uid { get; set; }
    [Parameter] public bool IsAgreementCompleted { get; set; } = true;
    #endregion

    #region Variables
    private List<AgreementDb> agreements = new();
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
        agreements = await agreementRepository.GetByUserIdAllAsync(Uid);
    }

    private void HandleDetailsClick(long aid)
    {
        NavigationManager.NavigateTo($"/Agreement/User/Details/{aid}");
    }
    #endregion
}
