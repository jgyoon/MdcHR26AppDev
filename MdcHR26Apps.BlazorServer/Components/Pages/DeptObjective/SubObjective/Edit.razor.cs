using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.DeptObjective;
using MdcHR26Apps.Models.Department;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.DeptObjective.SubObjective
{
    public partial class Edit
    {
        #region Parameters
        [Parameter]
        public Int64 Id { get; set; }
        #endregion
        [Inject]
        public IDeptObjectiveRepository deptObjectiveDbReposiory { get; set; } = null!;
        public DeptObjectiveDb model { get; set; } = new DeptObjectiveDb();
        // 부서관리
        [Inject]
        public IEDepartmentRepository eDepartmentDbRepository { get; set; } = null!;
        // 페이지이동
        [Inject]
        public UrlActions urlActions { get; set; } = null!;
        // 로그인관리
        [Inject]
        public LoginStatusService loginStatusService { get; set; } = null!;

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
            if (!loginStatusService.IsloginAndIsDeptObjectiveWriterCheck())
            {
                StateHasChanged();
                urlActions.MoveMainPage();
            }
        }

        private async Task SetData(Int64 Id)
        {
            model = await deptObjectiveDbReposiory.GetByIdAsync(Id);
        }

        #region + [1] EditObjective : 목표수정
        private async Task EditObjective()
        {
            if (!String.IsNullOrEmpty(model.ObjectiveTitle) && !String.IsNullOrEmpty(model.ObjectiveContents))
            {
                // 감사 필드 설정 (26년도 DB 변경)
                model.UpdatedBy = loginStatusService.LoginStatus.LoginUid;
                model.UpdatedAt = DateTime.Now;

                if (await deptObjectiveDbReposiory.UpdateAsync(model))
                {
                    resultText = "목표 수정에 성공하셨습니다.";
                    StateHasChanged();
                    urlActions.MoveSubObjectivePage();
                }
                else
                {
                    resultText = "목표 수정에 실패하셨습니다.(입력실패1)";
                }

            }
            else
            {
                resultText = "목표 작성이 되지 않았습니다.";
            }
        }
        #endregion

        #region + [15].[2] MoveSubObjectivePage : 부서목표 페이지
        public void MoveSubObjectivePage()
        {
            urlActions.MoveSubObjectivePage();
        }
        #endregion

    }
}
