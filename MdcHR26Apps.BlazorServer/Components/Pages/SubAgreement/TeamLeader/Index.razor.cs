using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.EvaluationProcess;
using MdcHR26Apps.Models.Views.v_ProcessTRListDB;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.SubAgreement.TeamLeader
{
    public partial class Index : ComponentBase
    {
        // 로그인관리(상태관리)
        [Inject]
        public LoginStatusService loginStatusService { get; set; } = null!;

        // 페이지이동
        [Inject]
        public UrlActions urlActions { get; set; } = null!;

        // v_ProcessTRListDB Repository
        [Inject]
        public Iv_ProcessTRListRepository processRepository { get; set; } = null!;

        public List<v_ProcessTRListDB> processDbList { get; set; } = new();

        //protected override async Task OnInitializedAsync()
        //{
        //    await CheckLogined();
        //    await SetData();
        //    await base.OnInitializedAsync();
        //}

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await CheckLogined();
                await SetData();
                StateHasChanged();
            }
            else
            {
                await SetData();
                StateHasChanged();
            }
        }

        private async Task SetData()
        {
            long sessionUid = loginStatusService.LoginStatus.LoginUid;
            if (sessionUid > 0)
            {
                processDbList = await processRepository.GetByTeamLeaderIdAsync(sessionUid);
            }
        }

        #region + [0].[1] CheckLogined : IsloginCheck() + IsTeamLeaderCheck()
        /// <summary>
        /// 로그인 체크 && 부서장 체크
        /// </summary>
        /// <returns></returns>
        private async Task CheckLogined()
        {
            await Task.Delay(0);
            if (!loginStatusService.IsloginAndIsTeamLeaderCheck())
            {
                StateHasChanged();
                urlActions.MoveMainPage();
            }
        }
        #endregion
    }

}
