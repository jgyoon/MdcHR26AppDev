using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.BlazorServer.Utils;
using MdcHR26Apps.Models.Rank;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin.Settings.Ranks;

public partial class Delete(
    IERankRepository eRankRepository,
    LoginStatusService loginStatusService,
    UrlActions urlActions,
    UserUtils utils)
{
    #region Parameters
    [Parameter]
    public Int64 Id { get; set; }
    #endregion

    private ERankDb model { get; set; } = new ERankDb();
    private string resultText { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await CheckLogined();
        await SetData(Id);
        await base.OnInitializedAsync();
    }

    private async Task SetData(Int64 id)
    {
        var result = await eRankRepository.GetByIdAsync(id);
        if (result != null)
        {
            model = result;
        }
        else
        {
            resultText = "직급 정보를 찾을 수 없습니다.";
        }
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

    #region Delete Rank
    private async Task DeleteRank()
    {
        var result = await eRankRepository.DeleteAsync(Id);

        if (result > 0)
        {
            resultText = "직급 삭제 성공";
            StateHasChanged();
            await Task.Delay(1000);
            urlActions.MoveSettingManagePage();
        }
        else
        {
            resultText = "직급 삭제 실패 (사용 중인 직급일 수 있습니다)";
        }
    }
    #endregion

    #region Page Navigation
    private void MoveSettingManagePage()
    {
        urlActions.MoveSettingManagePage();
    }

    private void MoveDetailsPage()
    {
        urlActions.MoveRankDetailsPage(Id);
    }
    #endregion
}
