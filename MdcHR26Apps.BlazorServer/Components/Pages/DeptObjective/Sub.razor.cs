using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.Department;
using MdcHR26Apps.Models.Views.v_DeptObjectiveListDb;
using MdcHR26Apps.Models.Views.v_MemberListDB;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.DeptObjective;

public partial class Sub
{
    // 부서목표리스트View
    [Inject]
    public Iv_DeptObjectiveListRepository vDeptObjectiveListDbRepository { get; set; } = null!;
    public List<v_DeptObjectiveListDb> objectList { get; set; } = new List<v_DeptObjectiveListDb>();

    // 부서관리
    [Inject]
    public IEDepartmentRepository eDepartmentDbRepository { get; set; } = null!;

    // 멤버리스트View
    [Inject]
    public Iv_MemberListRepository vMemberListDbRepository { get; set; } = null!;
    public v_MemberListDB memberModel { get; set; } = new v_MemberListDB();

    // 로그인관리(상태관리)
    [Inject]
    public LoginStatusService loginStatusService { get; set; } = null!;

    // 페이지이동
    [Inject]
    public UrlActions urlActions { get; set; } = null!;

        // 기타
        public string deptName = string.Empty;
        public string userId = String.Empty;
        public Int64 deptId = 0;
        public bool isEdit { get; set; } = false;
        public bool isMain { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            await CheckLogined();
            await SetData();
            StateHasChanged();
        }

        private async Task SetData()
        {
            if (loginStatusService.IsloginCheck())
            {
                deptName = !String.IsNullOrEmpty(loginStatusService.LoginStatus.LoginUserEDepartment) ?
                    loginStatusService.LoginStatus.LoginUserEDepartment : string.Empty;
                userId = !String.IsNullOrEmpty(loginStatusService.LoginStatus.LoginUserId) ?
                    loginStatusService.LoginStatus.LoginUserId : string.Empty;
                if (!String.IsNullOrEmpty(deptName)) 
                {
                    deptId = await eDepartmentDbRepository.GetIdByNameAsync(deptName);
                }

                if (!String.IsNullOrEmpty(userId) && deptId > 0)
                {
                    memberModel = await vMemberListDbRepository.GetByUserIdAsync(userId) ?? new v_MemberListDB();
                    objectList = (await vDeptObjectiveListDbRepository.GetByDepartmentAsync(deptId) ?? new List<v_DeptObjectiveListDb>()).ToList();
                }

                if (memberModel.Uid > 0)
                {
                    isEdit = memberModel.IsDeptObjectiveWriter;
                }
            }
        }

    #region + [0].[1] CheckLogined : IsloginCheck()
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

    #region + [15].[7] MoveSubObjectiveCreatePage : 부서목표 생성페이지 이동
    public void MoveSubObjectiveCreatePage()
    {
        urlActions.MoveSubObjectiveCreatePage();
    }
    #endregion
}
