using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.DeptObjective;
using MdcHR26Apps.Models.Department;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.DeptObjective.SubObjective
{
    public partial class Create
    {
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
        public string deptName = string.Empty;
        public Int64 deptId = 0;

        protected override async Task OnInitializedAsync()
        {
            await CheckLogined();
            await SetData();
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

        private async Task SetData()
        {
            if (loginStatusService.IsloginCheck())
            {
                deptName = !String.IsNullOrEmpty(loginStatusService.LoginStatus.LoginUserEDepartment) ?
                    loginStatusService.LoginStatus.LoginUserEDepartment : string.Empty;
                if (!String.IsNullOrEmpty(deptName))
                {
                    deptId = await eDepartmentDbRepository.GetIdByNameAsync(deptName);
                }
            }
        }

        #region + [1] CreateObjective : 목표생성
        private async Task CreateObjective()
        {
            if (!String.IsNullOrEmpty(model.ObjectiveTitle) && !String.IsNullOrEmpty(model.ObjectiveContents) && deptId > 0)
            {
                model.EDepartId = deptId;
                model.Remarks = string.Empty;

                // ObjectiveType 설정 (26년도 DB 변경)
                model.ObjectiveType = "Sub";

                // 감사 필드 설정 (26년도 DB 변경)
                model.CreatedBy = loginStatusService.LoginStatus.LoginUid;
                model.CreatedAt = DateTime.Now;

                model = await deptObjectiveDbReposiory.AddAsync(model);

                if (model.DeptObjectiveDbId != 0)
                {
                    resultText = "목표 생성에 성공하셨습니다.";
                    StateHasChanged();
                    urlActions.MoveSubObjectivePage();
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

        #region + [15].[2] MoveSubObjectivePage : 부서목표 페이지
        public void MoveSubObjectivePage()
        {
            urlActions.MoveSubObjectivePage();
        }
        #endregion
    }
}
