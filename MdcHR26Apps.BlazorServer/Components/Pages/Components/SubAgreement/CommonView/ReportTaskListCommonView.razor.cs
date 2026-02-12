using MdcHR26Apps.BlazorServer.Utils;
using MdcHR26Apps.Models.Views.v_ReportTaskListDB;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.SubAgreement.CommonView;

public partial class ReportTaskListCommonView(
    ExcelManage excelManage,
    IJSRuntime js,
    NavigationManager navigationManager)
{
    #region Parameters
    [Parameter] public bool Collapsed { get; set; } = false;
    [Parameter] public EventCallback Toggle { get; set; }
    [Parameter] public List<v_ReportTaskListDB> ReportTaskListDB { get; set; } = new();
    [Parameter] public EventCallback<long> OnSelectTask { get; set; }
    #endregion

    #region Variables
    private string IconClass => Collapsed ? "oi oi-minus" : "oi oi-plus";
    #endregion

    public string Comment { get; set; } = string.Empty;

    private async Task ClickExportXLS()
    {
        if (ReportTaskListDB == null || ReportTaskListDB.Count == 0)
        {
            Comment = "평가자가 없어 파일을 생성하지 못했습니다.";
            return;
        }

        string fileFolderPath = "tasks";
        string fileNameAdd = DateTime.Now.ToString("yy_MM_dd_HH_mm_ss");
        string fileName = $"업무리스트_{fileNameAdd}.xlsx";

        // NavigationManager.BaseUri 사용 (자동으로 현재 환경 URL 감지)
        string fileUrl = $"{navigationManager.BaseUri}files/tasks/{fileName}";

        bool isCreateResult = excelManage.CreateExcelFile(ReportTaskListDB, fileFolderPath, fileName);

        if (isCreateResult)
        {
            Comment = "파일을 생성했습니다.";
            StateHasChanged();

            await js.InvokeVoidAsync("downloadURI", fileUrl, fileName);
            await Task.Delay(500);

            excelManage.DeleteExcelFile(fileFolderPath, fileName);

            await Task.Delay(3000);
            Comment = string.Empty;
            StateHasChanged();
        }
        else
        {
            Comment = "파일을 생성하지 못했습니다.";
        }
    }
            
}
