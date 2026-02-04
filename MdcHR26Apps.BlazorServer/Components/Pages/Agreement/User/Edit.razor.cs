using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.EvaluationAgreement;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Agreement.User
{
    public partial class Edit
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
        public string successMessage { get; set; } = string.Empty;

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

            // 권한 체크 - 본인만 수정 가능
            var loginUser = loginStatusService.LoginStatus;
            if (model.Uid != loginUser.LoginUid)
            {
                errorMessage = "수정 권한이 없습니다.";
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

        private async Task HandleValidSubmit()
        {
            if (model == null)
            {
                errorMessage = "데이터가 없습니다.";
                return;
            }

            try
            {
                errorMessage = string.Empty;
                successMessage = string.Empty;

                var result = await agreementRepository.UpdateAsync(model);
                if (result)
                {
                    successMessage = "직무평가 협의가 성공적으로 수정되었습니다.";
                    StateHasChanged();
                    await Task.Delay(1000);
                    urlActions.MoveAgreementUserIndexPage();
                }
                else
                {
                    errorMessage = "수정에 실패했습니다.";
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"오류가 발생했습니다: {ex.Message}";
            }
        }

        protected void Cancel()
        {
            urlActions.MoveAgreementUserIndexPage();
        }
    }
}
