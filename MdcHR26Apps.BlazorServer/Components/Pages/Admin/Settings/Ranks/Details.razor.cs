using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.BlazorServer.Utils;
using MdcHR26Apps.Models.Rank;
using MdcHR26Apps.Models.Views.v_MemberListDB;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin.Settings.Ranks;

public partial class Details(
    IERankRepository eRankRepository,
    Iv_MemberListRepository vMemberListRepository,
    LoginStatusService loginStatusService,
    UrlActions urlActions,
    UserUtils utils)
{
    #region Parameters
    [Parameter]
    public Int64 Id { get; set; }
    #endregion

    // 직급관리
    private ERankDb model { get; set; } = new ERankDb();

    // 멤버리스트View
    private List<v_MemberListDB> memberList { get; set; } = new List<v_MemberListDB>();

    // 가능여부 체크(삭제)
    private bool isPossible { get; set; } = false;
    // Tooltip 메시지
    private string tooltipMessage { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await CheckLogined();
        await SetData(Id);
        await base.OnInitializedAsync();
    }

    private async Task SetData(Int64 Id)
    {
        var result = await eRankRepository.GetByIdAsync(Id);
        if (result != null)
        {
            model = result;
        }

        if (model.ERankId != 0)
        {
            var members = await vMemberListRepository.GetByRankAsync(model.ERankId);
            memberList = members.ToList();
        }

        // 멤버가 없으면 삭제 가능
        if (memberList.Count == 0)
        {
            isPossible = true;
        }

        tooltipMessage = "사용 중인 구성원이 있습니다.";
    }

    #region Login Check
    private async Task CheckLogined()
    {
        await Task.Delay(0);
        if (!loginStatusService.IsloginAndIsAdminCheck())
        {
            StateHasChanged();
            urlActions.MoveMainPage();
        }
    }
    #endregion

    #region Page Navigation
    private void MoveSettingManagePage()
    {
        urlActions.MoveSettingManagePage();
    }

    private void MoveEditPage()
    {
        urlActions.MoveRankEditPage(Id);
    }

    private void MoveDeletePage()
    {
        urlActions.MoveRankDeletePage(Id);
    }
    #endregion
}
