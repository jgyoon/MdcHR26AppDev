using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.EvaluationAgreement;
using MdcHR26Apps.Models.EvaluationLists;
using MdcHR26Apps.Models.EvaluationSubAgreement;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.SubAgreement.User
{
    public partial class Details
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

        // 평가리스트 관리
        [Inject]
        public IEvaluationListsRepository ListsRepository { get; set; } = null!;
        public EvaluationLists listsmodel { get; set; } = new EvaluationLists();
        // 지표분류리스트(평가부서)
        public List<EvaluationLists> list1 { get; set; } = new List<EvaluationLists>();
        // 지표분류리스트(평가리스트)
        public List<EvaluationLists> list2 { get; set; } = new List<EvaluationLists>();

        // 세부직무합의관리
        [Inject]
        public ISubAgreementRepository subAgreementDbRepository { get; set; } = null!;
        public SubAgreementDb model { get; set; } = new SubAgreementDb();
        public List<SubAgreementDb> subAgreementDblist { get; set; } = new List<SubAgreementDb>();

        // 기타
        public bool list1_Stauts { get; set; } = false;
        public bool list2_Stauts { get; set; } = false;
        public string? DeptName { get; set; } = string.Empty;

        //public bool list3_Stauts { get; set; } = false;
        //public string? IndexName { get; set; } = string.Empty;
        //public string? TaskContent { get; set; } = string.Empty;

        public int itemNumber { get; set; } = 1;
        public int peroportion { get; set; } = 0;
        public int maxperoportion { get; set; } = 100;

        // 기타
        public string resultText { get; set; } = String.Empty;

        protected override async Task OnInitializedAsync()
        {
            await CheckLogined();
            await SetData(Sid);
            await base.OnInitializedAsync();
        }

        private async Task SetData(long sid)
        {
            model = await subAgreementDbRepository.GetByIdAsync(sid);
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

        #region + [9].[5] MoveUserSubAgreementDeletePage : 세부직무작성 삭제페이지 이동
        public void MoveUserSubAgreementDeletePage(long sid)
        {
            urlActions.MoveUserSubAgreementDeletePage(sid);
        }
        #endregion

    }
}
