using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.EvaluationProcess;
using MdcHR26Apps.Models.EvaluationUsers;
using MdcHR26Apps.Models.User;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Modal
{
    public partial class UserDeleteModal
    {
        #region Parameters
        [Parameter]
        public Int64 Uid { get; set; }
        #endregion

        #region Inject Services
        // 사용자 정보
        [Inject]
        public IUserRepository userRepository { get; set; } = null!;
        public UserDb model { get; set; } = new UserDb();

        // 평가 참여자
        [Inject]
        public IEvaluationUsersRepository evaluationUsersRepository { get; set; } = null!;

        // 평가 프로세스
        [Inject]
        public IProcessRepository processRepository { get; set; } = null!;

        // 페이지 이동
        [Inject]
        public UrlActions urlActions { get; set; } = null!;
        #endregion

        #region Modal Variables
        public Guid Guid = Guid.NewGuid();
        public string ModalDisplay = "none;";
        public string ModalClass = "";
        public bool ShowBackdrop = false;
        #endregion

        // 기타
        public string resultText { get; set; } = String.Empty;

        protected override async Task OnInitializedAsync()
        {
            await SetData(Uid);
        }

        private async Task SetData(Int64 uid)
        {
            model = await userRepository.GetByIdAsync(uid) ?? new UserDb();
            Open();
        }

        #region + Modal Functions
        public void Open()
        {
            ModalDisplay = "block;";
            ModalClass = "Show";
            ShowBackdrop = true;
            StateHasChanged();
        }

        public void Close()
        {
            ModalDisplay = "none";
            ModalClass = "";
            ShowBackdrop = false;
            StateHasChanged();
        }
        #endregion

        #region + DeleteUser : 사용자 삭제 (역순: ProcessDb → EvaluationUsers → UserDb)
        private async Task DeleteUser()
        {
            try
            {
                // Step 1: ProcessDb 삭제 (있는 경우만)
                if (await processRepository.CheckUidAsync(Uid))
                {
                    var processDb = await processRepository.GetByUidAsync(Uid);
                    if (processDb != null)
                    {
                        int processResult = await processRepository.DeleteAsync(processDb.Pid);
                        if (processResult <= 0)
                        {
                            resultText = "평가 프로세스 삭제 실패";
                            return;
                        }
                    }
                }
                // ProcessDb가 없어도 계속 진행 (평가 대상자가 아닌 경우)

                // Step 2: EvaluationUsers 삭제 (있는 경우만)
                if (await evaluationUsersRepository.CheckUidAsync(Uid))
                {
                    var evaluationUser = await evaluationUsersRepository.GetByUidAsync(Uid);
                    if (evaluationUser != null)
                    {
                        int evalResult = await evaluationUsersRepository.DeleteAsync(evaluationUser.EUid);
                        if (evalResult <= 0)
                        {
                            resultText = "평가 참여자 삭제 실패";
                            return;
                        }
                    }
                }
                // EvaluationUsers가 없어도 계속 진행 (평가 대상자가 아닌 경우)

                // Step 3: UserDb 삭제 (필수)
                // 평가 데이터가 없는 사용자도 삭제 가능
                int userResult = await userRepository.DeleteAsync(Uid);
                if (userResult <= 0)
                {
                    resultText = "사용자 삭제 실패 - UserDb 삭제 오류";
                    return;
                }

                // 성공
                resultText = "Success - 사용자 삭제 완료";
                StateHasChanged();
                await Task.Delay(1000); // 1초 대기
                UserManagePage();
            }
            catch (Exception ex)
            {
                resultText = $"삭제 오류: {ex.Message}";
            }
        }
        #endregion

        #region + UserManagePage : 사용자관리 페이지 이동
        protected void UserManagePage()
        {
            urlActions.MoveUserManagePage();
        }
        #endregion
    }
}
