using MdcHR26Apps.BlazorServer.Components.Pages.Components.Agreement.Modal;
using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.EvaluationAgreement;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Agreement.User
{
    public partial class Delete
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
        public string errorMessage { get; set; } = string.Empty;

        private AgreementDeleteModal? deleteModal;

        protected override async Task OnInitializedAsync()
        {
            await CheckLogined();
            await LoadData();
            await base.OnInitializedAsync();
        }

        private async Task LoadData()
        {
            model = await agreementRepository.GetByIdAsync(Id);

            if (model == null)
            {
                errorMessage = "데이터를 찾을 수 없습니다.";
                return;
            }

            // 권한 체크 - 본인만 삭제 가능
            var loginUser = loginStatusService.LoginStatus;
            if (model.Uid != loginUser.LoginUid)
            {
                errorMessage = "삭제 권한이 없습니다.";
                StateHasChanged();
                await Task.Delay(2000);
                urlActions.MoveAgreementUserIndexPage();
            }
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

        private void OpenDeleteModal()
        {
            deleteModal?.Open();
        }

        private void CloseModal()
        {
            // Modal이 닫힐 때 처리
        }

        private async Task HandleDeleteSuccess()
        {
            // 삭제 성공 후 Index 페이지로 이동
            await Task.Delay(500);
            urlActions.MoveAgreementUserIndexPage();
        }

        protected void Cancel()
        {
            urlActions.MoveAgreementUserIndexPage();
        }
    }
}
