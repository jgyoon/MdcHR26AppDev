using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.Rank;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin.Settings.Ranks;

public partial class Edit(
    IERankRepository eRankRepository,
    LoginStatusService loginStatusService,
    UrlActions urlActions)
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

    private async Task SetData(Int64 Id)
    {
        var result = await eRankRepository.GetByIdAsync(Id);
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

    #region Update Rank
    private async Task UpdateRank()
    {
        if (string.IsNullOrEmpty(model.ERankName) || model.ERankNo < 1)
        {
            resultText = "직급명과 직급번호(1 이상)는 필수입니다.";
            return;
        }

        // 중복 체크 (자기 자신 제외)
        var existing = await eRankRepository.GetByRankNoAsync(model.ERankNo);
        if (existing != null && existing.ERankId != model.ERankId)
        {
            resultText = $"직급번호 {model.ERankNo}는 이미 사용 중입니다.";
            return;
        }

        var result = await eRankRepository.UpdateAsync(model);

        if (result > 0)
        {
            resultText = "직급 수정 성공";
            StateHasChanged();
            await Task.Delay(1000);
            urlActions.MoveRankDetailsPage(Id);
        }
        else
        {
            resultText = "직급 수정 실패";
        }
    }
    #endregion

    #region Page Navigation
    private void MoveDetailsPage()
    {
        urlActions.MoveRankDetailsPage(Id);
    }
    #endregion
}
