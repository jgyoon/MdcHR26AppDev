using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.DeptObjective;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.DeptObjective.MainObjective
{
    public partial class Create
    {
        [Inject]
        public IDeptObjectiveRepository deptObjectiveDbReposiory { get; set; } = null!;
        public DeptObjectiveDb model { get; set; } = new DeptObjectiveDb();
        // 페이지이동
        [Inject]
        public UrlActions urlActions { get; set; } = null!;
        // 로그인관리
        [Inject]
        public LoginStatusService loginStatusService { get; set; } = null!;

        // 기타
        public string resultText { get; set; } = string.Empty;
        public Int64 deptId = 1;

        protected override async Task OnInitializedAsync()
        {
            await CheckLogined();
            await SetData();
            await base.OnInitializedAsync();
        }

        private async Task CheckLogined()
        {
            await Task.Delay(0);
            if (!loginStatusService.IsloginAndIsAdminCheck())
            {
                StateHasChanged();
                urlActions.MoveMainPage();
            }
        }

        private async Task SetData()
        {
            await Task.Delay(1);
        }

        #region + [1] CreateObjective : 목표생성
        private async Task CreateObjective()
        {
            if (!String.IsNullOrEmpty(model.ObjectiveTitle) && !String.IsNullOrEmpty(model.ObjectiveContents))
            {
                model.EDepartId = deptId;
                model.Remarks = string.Empty;

                // ObjectiveType 설정 (26년도 DB 변경)
                model.ObjectiveType = "Main";

                // 감사 필드 설정 (26년도 DB 변경)
                model.CreatedBy = loginStatusService.LoginStatus.LoginUid;
                model.CreatedAt = DateTime.Now;

                model = await deptObjectiveDbReposiory.AddAsync(model);

                if (model.DeptObjectiveDbId != 0)
                {
                    resultText = "목표 생성에 성공하셨습니다.";
                    StateHasChanged();
                    urlActions.MoveMainObjectivePage();
                }
                else
                {
                    resultText = "목표 생성에 실패하셨습니다.(입력실패1)";
                }

            }
            else 
            {
                resultText = "목표 작성이 되지 않았습니다.";
            }
        }
        #endregion

        #region + [15].[1] MoveMainObjectivePage : 전사목표 페이지
        public void MoveMainObjectivePage()
        {
            urlActions.MoveMainObjectivePage();
        }
        #endregion

    }
}
