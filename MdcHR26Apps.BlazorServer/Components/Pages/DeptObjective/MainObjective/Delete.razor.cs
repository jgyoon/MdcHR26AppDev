using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.DeptObjective;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.DeptObjective.MainObjective
{
    public partial class Delete
    {
        #region Parameters
        [Parameter]
        public Int64 Id { get; set; }
        #endregion
        [Inject]
        public IDeptObjectiveRepository deptObjectiveDbReposiory { get; set; } = null!;
        public DeptObjectiveDb model { get; set; } = new DeptObjectiveDb();

        // 로그인관리(상태관리)
        [Inject]
        public LoginStatusService loginStatusService { get; set; } = null!;

        // 페이지이동
        [Inject]
        public UrlActions urlActions { get; set; } = null!;

        // 기타
        public string resultText { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await CheckLogined();
            await SetData(Id);
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

        private async Task SetData(Int64 Id)
        {
            model = await deptObjectiveDbReposiory.GetByIdAsync(Id);
        }

        #region + [1] DeleteObjective : 목표삭제
        private async Task DeleteObjective()
        {
            if (model.DeptObjectiveDbId > 0)
            {
                if (await deptObjectiveDbReposiory.DeleteAsync(model.DeptObjectiveDbId))
                {
                    resultText = "목표 삭제에 성공하셨습니다.";
                    StateHasChanged();
                    urlActions.MoveMainObjectivePage();
                }
                else
                {
                    resultText = "목표 삭제에 실패하셨습니다.(입력실패1)";
                }
            }
            else
            {
                resultText = "목표 삭제에 실패하셨습니다.(입력실패2)";
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
