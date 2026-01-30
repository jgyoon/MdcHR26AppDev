using MdcHR26Apps.BlazorServer.Utils;
using MdcHR26Apps.Models.Views.v_ReportTaskListDB;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Common;

public partial class AdminTaskViewExcel(
    ExcelManage excelManage,
    IJSRuntime js,
    NavigationManager navigationManager)
{
    [Parameter]
    public List<v_ReportTaskListDB>? ReportTaskListDB { get; set; }

    [Parameter]
    public EventCallback<string> CommentChanged { get; set; }

    private string comment = string.Empty;

    private async Task ClickExportXLS()
    {
        if (ReportTaskListDB == null || ReportTaskListDB.Count == 0)
        {
            comment = "합의된 세부업무가 없어 파일을 생성하지 못했습니다.";
            await CommentChanged.InvokeAsync(comment);
            return;
        }

        string fileFolderPath = "tasks";
        string fileNameAdd = DateTime.Now.ToString("yy_MM_dd_HH_mm_ss");
        string fileName = $"관리자용_업무리스트_{fileNameAdd}.xlsx";

        // NavigationManager.BaseUri 사용 (자동으로 현재 환경 URL 감지)
        string fileUrl = $"{navigationManager.BaseUri}files/tasks/{fileName}";

        bool isCreateResult = excelManage.CreateExcelFile(ReportTaskListDB, fileFolderPath, fileName);

        if (isCreateResult)
        {
            comment = "파일을 생성했습니다.";
            await CommentChanged.InvokeAsync(comment);

            await js.InvokeVoidAsync("downloadURI", fileUrl, fileName);
            await Task.Delay(500);

            excelManage.DeleteExcelFile(fileFolderPath, fileName);

            await Task.Delay(3000);
            comment = string.Empty;
            await CommentChanged.InvokeAsync(comment);
        }
        else
        {
            comment = "파일을 생성하지 못했습니다.";
            await CommentChanged.InvokeAsync(comment);
        }
    }
}
