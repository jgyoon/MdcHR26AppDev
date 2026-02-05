using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.Common;
using MdcHR26Apps.Models.EvaluationAgreement;
using MdcHR26Apps.Models.EvaluationLists;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Agreement.User
{
    public partial class Create
    {
        // 로그인관리(상태관리)
        [Inject]
        public LoginStatusService loginStatusService { get; set; } = null!;

        // 페이지이동
        [Inject]
        public UrlActions urlActions { get; set; } = null!;

        // 평가리스트 관리
        [Inject]
        public IEvaluationListsRepository listsRepository { get; set; } = null!;

        // 직무협의 관리
        [Inject]
        public IAgreementRepository agreementRepository { get; set; } = null!;

        public AgreementDb model { get; set; } = new();
        public List<AgreementDb> agreementDbList { get; set; } = new();

        // 평가지표/직무 리스트
        public List<SelectListModel> list1 { get; set; } = new();
        public List<SelectListModel> list2 { get; set; } = new();

        // 선택된 값들 (양방향 바인딩용)
        private string _selectedName1 = string.Empty;
        public string selectedName1
        {
            get => _selectedName1;
            set
            {
                if (_selectedName1 != value)
                {
                    _selectedName1 = value;
                    model.Report_Item_Name_1 = value;
                    _ = HandleList1Changed();
                }
            }
        }

        private string _selectedName2 = string.Empty;
        public string selectedName2
        {
            get => _selectedName2;
            set
            {
                if (_selectedName2 != value)
                {
                    _selectedName2 = value;
                    model.Report_Item_Name_2 = value;
                }
            }
        }

        private int _peroportion = 0;
        public int peroportion
        {
            get => _peroportion;
            set
            {
                if (_peroportion != value)
                {
                    _peroportion = value;
                    model.Report_Item_Proportion = value > maxperoportion ? maxperoportion : value;
                }
            }
        }

        public int itemNumber { get; set; } = 1;
        public int maxperoportion { get; set; } = 100;
        public string resultText { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await CheckLogined();
            await SetData();
            await base.OnInitializedAsync();
        }

        private async Task SetData()
        {
            // 평가지표 리스트 로드
            list1 = await listsRepository.GetByDeptAllAsync();

            // 기존 협의 리스트 로드
            var loginUser = loginStatusService.LoginStatus;
            agreementDbList = await agreementRepository.GetByUidAllAsync(loginUser.LoginUid);

            // 항목 번호 계산
            itemNumber = GetItemNumber(agreementDbList);

            // 사용 가능한 최대 비중 계산
            if (agreementDbList != null && agreementDbList.Count > 0)
            {
                maxperoportion = GetMaxValue(agreementDbList);
            }

            // 모델 초기화
            model = new AgreementDb
            {
                Uid = loginUser.LoginUid,
                Report_Item_Number = itemNumber,
                Report_Item_Name_1 = string.Empty,
                Report_Item_Name_2 = string.Empty,
                Report_Item_Proportion = 0
            };
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
        /// 항목 번호를 구하는 메서드
        /// </summary>
        private int GetItemNumber(List<AgreementDb> lists)
        {
            return lists.Count > 0 ? lists.Count + 1 : 1;
        }

        /// <summary>
        /// 사용 가능한 비중을 구하는 메서드
        /// </summary>
        private int GetMaxValue(List<AgreementDb> lists)
        {
            int defaultValue = 100;
            if (lists != null && lists.Count != 0)
            {
                foreach (var item in lists)
                {
                    defaultValue -= item.Report_Item_Proportion;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// 평가지표 선택 시 평가직무 리스트 로드
        /// </summary>
        private async Task HandleList1Changed()
        {
            if (!string.IsNullOrEmpty(model.Report_Item_Name_1))
            {
                int deptNumber = await listsRepository.GetByDeptNumberAsync(model.Report_Item_Name_1);
                list2 = await listsRepository.GetByIndexAllAsync(deptNumber);
                model.Report_Item_Name_2 = string.Empty;
                selectedName2 = string.Empty;
                peroportion = 0;
                StateHasChanged();
            }
        }

        /// <summary>
        /// 직무협의 생성
        /// </summary>
        private async Task CreateAgreement()
        {
            await Task.Delay(1);

            // 사용 가능한 평가비중이 없으면 생성하지 않음
            if (maxperoportion == 0)
            {
                resultText = "평가 작성에 실패했습니다. (설정 가능한 평가비중이 없습니다.)";
                await Task.Delay(1000);
                StateHasChanged();
                urlActions.MoveAgreementUserIndexPage();
                return;
            }

            var loginUser = loginStatusService.LoginStatus;

            // 유효성 검사
            if (!string.IsNullOrEmpty(model.Report_Item_Name_1) &&
                !string.IsNullOrEmpty(model.Report_Item_Name_2) &&
                model.Report_Item_Proportion > 0)
            {
                // 비중이 최대값을 초과하지 않도록 조정
                if (model.Report_Item_Proportion > maxperoportion)
                {
                    model.Report_Item_Proportion = maxperoportion;
                }

                var result = await agreementRepository.AddAsync(model);
                if (result != null && result.Aid > 0)
                {
                    resultText = "평가 생성에 성공하였습니다.";
                    StateHasChanged();
                    await Task.Delay(1000);
                    urlActions.MoveAgreementUserIndexPage();
                }
                else
                {
                    resultText = "평가 생성에 실패했습니다.";
                }
            }
            else
            {
                resultText = "평가 작성에 실패했습니다. 다시 작성해 주세요.";
            }
        }

        protected void Cancel()
        {
            urlActions.MoveAgreementUserIndexPage();
        }
    }
}
