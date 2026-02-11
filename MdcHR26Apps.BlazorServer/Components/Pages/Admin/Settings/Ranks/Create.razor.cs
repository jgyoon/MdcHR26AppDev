using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.Rank;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin.Settings.Ranks;

public partial class Create(
    IERankRepository eRankRepository,
    LoginStatusService loginStatusService,
    UrlActions urlActions)
{
    private ERankDb model { get; set; } = new ERankDb();
    private string resultText { get; set; } = string.Empty;
    private string resultRankNo { get; set; } = string.Empty;
    private string resultRankNoColor { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await CheckLogined();
        await base.OnInitializedAsync();
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

    #region Save Rank
    private async Task SaveRank()
    {
        if (string.IsNullOrEmpty(model.ERankName) || model.ERankNo < 1)
        {
            resultText = "직급명과 직급번호(1 이상)는 필수입니다.";
            return;
        }

        // 중복 체크
        var existing = await eRankRepository.GetByRankNoAsync(model.ERankNo);
        if (existing != null)
        {
            resultText = $"직급번호 {model.ERankNo}는 이미 사용 중입니다.";
            return;
        }

        var result = await eRankRepository.AddAsync(model);

        if (result > 0)
        {
            resultText = "직급 등록 성공";
            StateHasChanged();
            await Task.Delay(1000);
            urlActions.MoveSettingManagePage();
        }
        else
        {
            resultText = "직급 등록 실패";
        }
    }
    #endregion

    #region Page Navigation
    private void MoveSettingManagePage()
    {
        urlActions.MoveSettingManagePage();
    }
    #endregion

    #region Rank Number Check
    private async Task CheckRankNo(int rankNo)
    {
        // 직급번호가 1 미만이면 return
        if (rankNo < 1)
            return;

        var existing = await eRankRepository.GetByRankNoAsync(rankNo);
        if (existing == null)
        {
            resultRankNo = "사용가능한 직급번호입니다.";
            resultRankNoColor = "color:blue";
        }
        else
        {
            resultRankNo = "이미 사용 중인 직급번호입니다.";
            resultRankNoColor = "color:red";
        }
    }
    #endregion
}
