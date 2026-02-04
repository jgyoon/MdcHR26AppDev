using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.EvaluationAgreement;
using MdcHR26Apps.Models.EvaluationProcess;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Agreement.User
{
    public partial class Index
    {
        // 페이지이동
        [Inject]
        public UrlActions urlActions { get; set; } = null!;

        // 로그인관리(상태관리)
        [Inject]
        public LoginStatusService loginStatusService { get; set; } = null!;

        // Agreement Repository
        [Inject]
        public IAgreementRepository agreementRepository { get; set; } = null!;

        // ProcessDb Repository
        [Inject]
        public IProcessRepository processRepository { get; set; } = null!;

        public List<AgreementDb> agreements { get; set; } = new();

        // ProcessDb 상태
        public ProcessDb processDb { get; set; } = new();

        // 상태 플래그
        public bool IsRequest { get; set; } = false;
        public bool IsAgreement { get; set; } = false;
        public string Agreement_Comment { get; set; } = string.Empty;

        // 비중 관련
        public int sumperoportion { get; set; } = 0;

        // 직무 개수 제한
        public int agreementItemMaxCount { get; set; } = 5;

        // 평가자 설정 여부
        public bool IsTeamLeaderAndDirectorSettings { get; set; } = false;

        // 결과 메시지
        public string resultText { get; set; } = string.Empty;

        // 펼쳐보기
        public bool Collapsed { get; set; } = true;
        public bool AgreementCollapsed { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            await CheckLogined();
            await LoadData();
            await base.OnInitializedAsync();
        }

        private async Task LoadData()
        {
            var loginUser = loginStatusService.LoginStatus;

            // ProcessDb 로드
            processDb = await processRepository.GetByUidAsync(loginUser.LoginUid) ?? new ProcessDb();
            IsRequest = processDb.Is_Request;
            IsAgreement = processDb.Is_Agreement;
            Agreement_Comment = processDb.Agreement_Comment ?? string.Empty;
            IsTeamLeaderAndDirectorSettings = GetTeamLeaderAndDirectorSettings(processDb);

            // Agreement 리스트 로드
            agreements = await agreementRepository.GetByUserIdAllAsync(loginUser.LoginUid);

            // 비중 합계 계산
            if (agreements != null && agreements.Count > 0)
            {
                setSumPeroportion(agreements);
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

        /// <summary>
        /// 직무 비중 총합을 구하는 메서드
        /// </summary>
        private void setSumPeroportion(List<AgreementDb> lists)
        {
            sumperoportion = 0;
            if (lists != null && lists.Count != 0)
            {
                foreach (var item in lists)
                {
                    sumperoportion += item.Report_Item_Proportion;
                }
            }
        }

        /// <summary>
        /// 부서장 & 임원 설정여부를 확인하는 메서드
        /// </summary>
        private bool GetTeamLeaderAndDirectorSettings(ProcessDb processDb)
        {
            return processDb.TeamLeaderId.HasValue && processDb.TeamLeaderId.Value > 0 &&
                   processDb.DirectorId.HasValue && processDb.DirectorId.Value > 0;
        }

        /// <summary>
        /// 중복된 값 찾기
        /// </summary>
        private bool IsDuplicated(List<AgreementDb> list)
        {
            var isDuplicated = list.GroupBy(x => x.Report_Item_Name_2)
                                   .Where(g => g.Count() > 1)
                                   .Select(g => g.Key)
                                   .ToList();
            return isDuplicated.Count > 0;
        }

        /// <summary>
        /// 합의요청
        /// </summary>
        protected async Task SetRequest()
        {
            processDb.Is_Request = true;

            int status = await processRepository.UpdateAsync(processDb);

            if (status > 0)
            {
                IsRequest = processDb.Is_Request;
                resultText = "합의요청이 완료되었습니다.";
                StateHasChanged();
            }
            else
            {
                resultText = "합의요청 실패!";
            }
        }

        /// <summary>
        /// 요청취소
        /// </summary>
        protected async Task SetRequestCancel()
        {
            processDb.Is_Request = false;

            int status = await processRepository.UpdateAsync(processDb);

            if (status > 0)
            {
                IsRequest = processDb.Is_Request;
                resultText = "요청이 취소되었습니다.";
                StateHasChanged();
            }
            else
            {
                resultText = "요청취소 실패!";
            }
        }

        /// <summary>
        /// Toggle 이벤트
        /// </summary>
        private void Toggle()
        {
            Collapsed = !Collapsed;
        }

        /// <summary>
        /// AgreementToggle 이벤트
        /// </summary>
        private void AgreementToggle()
        {
            AgreementCollapsed = !AgreementCollapsed;
        }

        #region Navigation Methods

        protected void MoveMainPage()
        {
            urlActions.MoveMainPage();
        }

        protected void CreateAgreement()
        {
            urlActions.MoveAgreementUserCreatePage();
        }

        protected void HandleDetails(long id)
        {
            urlActions.MoveAgreementUserDetailsPage(id);
        }

        #endregion
    }
}
