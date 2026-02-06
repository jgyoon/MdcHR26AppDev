using MdcHR26Apps.BlazorServer.Utils;
using MdcHR26Apps.Models.Views.v_TotalReportListDB;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Report.ViewPage;

public partial class User_TotalReportListView
{
    [Parameter]
    public v_TotalReportListDB TotalReportListDB { get; set; } = new v_TotalReportListDB();

    // 공용함수 호출
    public ScoreUtils scoreUtils = new ScoreUtils();

    public double totalScore = 0;

    // 테이블 CSS Style
    public string table_style_1 = "white-space: nowrap; overflow: hidden; text-overflow: ellipsis; text-align: center; vertical-align: middle;";
    public string table_style_2 = "overflow: hidden; text-overflow: ellipsis; text-align: left; vertical-align: middle;";

    protected override async Task OnInitializedAsync()
    {
        await SetData();
        await base.OnInitializedAsync();
    }

    public async Task SetData()
    {
        await Task.Delay(1);
        totalScore =
            TotalReportListDB.User_Evaluation_1 +
            TotalReportListDB.User_Evaluation_2 +
            TotalReportListDB.User_Evaluation_3;
    }
}
