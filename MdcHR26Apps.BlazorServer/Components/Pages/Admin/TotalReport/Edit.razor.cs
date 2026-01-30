using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.BlazorServer.Models;
using MdcHR26Apps.BlazorServer.Utils;
using MdcHR26Apps.Models.Result;
using MdcHR26Apps.Models.Views.v_ProcessTRListDB;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin.TotalReport;

public partial class Edit(
    Iv_ProcessTRListRepository processTRRepository,
    ITotalReportRepository totalReportRepository,
    LoginStatusService loginStatusService,
    UrlActions urlActions,
    ScoreUtils scoreUtils)
{
    [Parameter]
    public long Id { get; set; }

    private v_ProcessTRListDB? model;
    private TotalReportDb editModel = new();
    private List<TotalScoreRankModel>? totalScoreList;
    private string resultText = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await CheckLogined();
        await LoadData();
    }

    private async Task CheckLogined()
    {
        await Task.CompletedTask;
        if (!loginStatusService.IsloginAndIsAdminCheck())
        {
            StateHasChanged();
            urlActions.MoveMainPage();
        }
    }

    private async Task LoadData()
    {
        model = await processTRRepository.GetByPidAsync(Id);

        if (model != null && model.TRid > 0)
        {
            var totalReport = await totalReportRepository.GetByPidAsync(model.Pid);
            if (totalReport != null)
            {
                editModel = totalReport;
            }
        }

        // 등급 목록 초기화 - ScoreUtils 사용
        totalScoreList = scoreUtils.GetTotalScoreRankList();
    }

    private async Task UpdateTotalReport()
    {
        if (editModel.TRid == 0)
        {
            resultText = "종합레포트가 존재하지 않습니다.";
            return;
        }

        var result = await totalReportRepository.UpdateAsync(editModel);

        if (result > 0)
        {
            resultText = "최종 등급 변경 완료";
            await Task.Delay(1000);
            urlActions.MoveTotalReportAdminDetailsPage(model!.Pid);
        }
        else
        {
            resultText = "변경 실패";
        }
    }
}
