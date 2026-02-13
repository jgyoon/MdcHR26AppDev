using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.BlazorServer.Utils;
using MdcHR26Apps.Models.EvaluationAgreement;
using MdcHR26Apps.Models.EvaluationSubAgreement;
using MdcHR26Apps.Models.EvaluationTasks;
using MdcHR26Apps.BlazorServer.Models;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.SubAgreement.User
{
    public partial class Delete
    {
        #region Parameters
        [Parameter]
        public long Sid { get; set; }
        #endregion

        // 로그인관리(상태관리)
        [Inject]
        public LoginStatusService loginStatusService { get; set; } = null!;

        // 페이지이동
        [Inject]
        public UrlActions urlActions { get; set; } = null!;

        // 세부직무합의관리
        [Inject]
        public ISubAgreementRepository subAgreementDbRepository { get; set; } = null!;
        public SubAgreementDb model { get; set; } = new SubAgreementDb();

        // 세부업무관리
        [Inject]
        public ITasksRepository tasksDbRepository { get; set; } = null!;

        // 공용함수 호출
        [Inject]
        public UserUtils utils { get; set; } = null!;

        public bool modalShow = false;

        public string resultText { get; set; } = String.Empty;

        [Inject]
        public AppStateService AppState { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            await CheckLogined();
            await SetData(Sid);
            await base.OnInitializedAsync();
        }

        #region + CheckLogined : 로그인 체크
        /// <summary>
        /// 로그인 체크
        /// </summary>
        /// <returns></returns>
        private async Task CheckLogined()
        {
            await Task.Delay(0);
            if (!loginStatusService.IsloginCheck())
            {
                StateHasChanged();
                urlActions.MoveMainPage();
            }
        }
        #endregion

        private async Task SetData(long sid)
        {
            //Dispose();
            model = await subAgreementDbRepository.GetByIdAsync(sid);

            // 메모리 내 상태 컨테이너 서비스 적용
            AppState.OnChange += StateHasChanged;
            // 상태값 초기화
            AppState.AppStateInital();
        }

        public void Dispose()
        {
            AppState.OnChange -= StateHasChanged;
        }

        #region + [9].[1] MoveUserSubAgreementMainPage : 세부직무작성 메인페이지 이동
        public void MoveUserSubAgreementMainPage()
        {
            urlActions.MoveUserSubAgreementMainPage();
        }
        #endregion

        #region + [9].[4] MoveUserSubAgreementEditPage : 세부직무작성 수정페이지 이동
        public void MoveUserSubAgreementEditPage(long sid)
        {
            urlActions.MoveUserSubAgreementEditPage(sid);
        }
        #endregion

        #region + 직무 삭제 : DeleteUserSubAgreement
        private async Task DeleteUserSubAgreement()
        {
            await Task.Delay(1);

            modalShow = true;

            AppState.SetDeleteAction();
        }
        #endregion

    }
}
