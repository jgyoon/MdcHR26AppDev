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

        // 로그인관리(상태관리)
        [Inject]
        public LoginStatusService loginStatusService { get; set; } = null!;

        // 페이지이동
        [Inject]
        public UrlActions urlActions { get; set; } = null!;

        // 직무협의관리
        [Inject]
        public IAgreementRepository agreementRepository { get; set; } = null!;
        public AgreementDb model { get; set; } = new AgreementDb();
        public List<AgreementDb> agreementDblist { get; set; } = new List<AgreementDb>();

        // 기타
        public string resultText { get; set; } = String.Empty;
        public int maxperoportion { get; set; } = 0;

        protected override async Task OnInitializedAsync()
        {
            await CheckLogined();
            await SetData(Id);
            await base.OnInitializedAsync();
        }

        private async Task SetData(Int64 Id)
        {
            var loginUser = loginStatusService.LoginStatus;
            long sessionUid = loginUser.LoginUid;

            // 전체 Agreement 목록 로드 (비중 계산용)
            agreementDblist = await agreementRepository.GetByUidAllAsync(sessionUid);

            // 현재 수정할 Agreement 로드
            model = await agreementRepository.GetByIdAsync(Id);

            if (model != null)
            {
                // 현재 비중 + 사용 가능한 비중 = 최대 설정 가능 비중
                maxperoportion = model.Report_Item_Proportion;
                if (agreementDblist != null && agreementDblist.Count > 0)
                {
                    maxperoportion += GetMaxVaule(agreementDblist);
                }
            }
        }

        #region + CheckLogined : 로그인 체크
        /// <summary>
        /// 로그인 체크
        /// </summary>
        private async Task CheckLogined()
        {
            await Task.Delay(0);
            if (!loginStatusService.IsloginCheck())
            {
                StateHasChanged();
                urlActions.MoveLoginPage();
            }
        }
        #endregion

        /// <summary>
        /// 사용가능한 비중을 구하는 메서드
        /// </summary>
        /// <param name="lists">전체 Agreement 목록</param>
        /// <returns>현재 설정된 비중 외 메서드</returns>
        private int GetMaxVaule(List<AgreementDb> lists)
        {
            int defalutVaule = 100;
            if (lists != null && lists.Count != 0)
            {
                foreach (var item in lists)
                {
                    defalutVaule = defalutVaule - item.Report_Item_Proportion;
                }
            }

            return defalutVaule;
        }

        /// <summary>
        /// FormSelectNumber에서 비중 변경 시 호출
        /// </summary>
        private void HandlePeroportionChanged(int newValue)
        {
            model.Report_Item_Proportion = newValue;
        }

        #region + MoveUserAgreementMainPage : 직무작성 메인페이지 이동
        public void MoveUserAgreementMainPage()
        {
            urlActions.MoveAgreementUserIndexPage();
        }
        #endregion

        #region + 직무수정 : EditUserAgreement
        private async Task EditUserAgreement()
        {
            // 비중 0% 검증
            if (model.Report_Item_Proportion == 0)
            {
                resultText = "직무 비중은 0 % 로 설정할 수 없습니다. 다시 설정해주세요.";
                return;
            }

            // 최대 직무 비중 이상 설정 금지
            model.Report_Item_Proportion = model.Report_Item_Proportion > maxperoportion
                ? maxperoportion
                : model.Report_Item_Proportion;

            if (await agreementRepository.UpdateAsync(model))
            {
                resultText = "평가 수정에 성공하였습니다.";
                StateHasChanged();
                // 평가메인페이지 이동
                urlActions.MoveAgreementUserIndexPage();
            }
            else
            {
                resultText = "평가 수정에 실패하였습니다.";
            }
        }
        #endregion
    }
}
