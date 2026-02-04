using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.EvaluationAgreement;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Agreement.User
{
    public partial class Details
    {
        #region Parameters
        [Parameter]
        public Int64 Id { get; set; }
        #endregion

        // 페이지이동
        [Inject]
        public UrlActions urlActions { get; set; } = null!;

        // 로그인관리(상태관리)
        [Inject]
        public LoginStatusService loginStatusService { get; set; } = null!;

        // Agreement Repository
        [Inject]
        public IAgreementRepository agreementRepository { get; set; } = null!;

        public AgreementDb? model { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await CheckLogined();
            await LoadData();
            await base.OnInitializedAsync();
        }

        private async Task LoadData()
        {
            model = await agreementRepository.GetByIdAsync(Id);
        }

        private async Task CheckLogined()
        {
            await Task.Delay(0);
            if (!loginStatusService.IsloginCheck())
            {
                StateHasChanged();
                urlActions.MoveLoginPage();
            }
        }

        #region Event Handlers

        private void HandleEdit()
        {
            urlActions.MoveAgreementUserEditPage(Id);
        }

        private void HandleDelete()
        {
            urlActions.MoveAgreementUserDeletePage(Id);
        }

        private void HandleBack()
        {
            urlActions.MoveAgreementUserIndexPage();
        }

        #endregion
    }
}
