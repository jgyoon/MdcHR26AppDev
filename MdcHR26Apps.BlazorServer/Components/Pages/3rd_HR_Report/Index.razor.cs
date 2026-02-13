using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.EvaluationProcess;
using MdcHR26Apps.Models.Views.v_ProcessTRListDB;
using MdcHR26Apps.Models.User;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MdcHR26Apps.BlazorServer.Components.Pages._3rd_HR_Report
{
    public partial class Index
    {
        // 로그인관리(상태관리)
        [Inject]
        public LoginStatusService loginStatusService { get; set; } = null!;

        // 페이지이동
        [Inject]
        public UrlActions urlActions { get; set; } = null!;

        // 평가순서관리
        [Inject]
        public IProcessRepository processDbRepository { get; set; } = null!;
        public ProcessDb processDb { get; set; } = new ProcessDb();
        public List<ProcessDb> processDbList { get; set; } = new List<ProcessDb>();

        // 사용자 관리
        [Inject]
        public IUserRepository userDbRepository { get; set; } = null!;
        public UserDb usermodel { get; set; } = new UserDb();
        public List<UserDb> userlist { get; set; } = new List<UserDb>();

        #region + ProcessTRList_View 관련
        // v_ProcessTRListDB
        [Inject]
        public Iv_ProcessTRListRepository v_processTRListDBRepository { get; set; } = null!;
        public List<v_ProcessTRListDB> processTRLists { get; set; } = new List<v_ProcessTRListDB>();
        #endregion

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
            long sessionUserId = loginStatusService.LoginStatus.LoginUid;
            if (sessionUserId > 0)
            {
                processTRLists = await v_processTRListDBRepository.GetByDirectorIdAsync(sessionUserId);
            }
        }

        #region + [0].[1] CheckLogined : IsloginCheck() + IsTeamLeaderCheck()
        /// <summary>
        /// 로그인체크 확인(로그인 & 임원여부) 메서드
        /// </summary>
        /// <returns></returns>
        private async Task CheckLogined()
        {
            await Task.Delay(0);
            if (!loginStatusService.IsloginAndIsDirectorCheck())
            {
                StateHasChanged();
                urlActions.MoveMainPage();
            }
        }
        #endregion
    }
}
