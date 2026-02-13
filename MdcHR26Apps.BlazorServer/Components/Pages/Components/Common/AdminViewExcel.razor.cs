using MdcHR26Apps.BlazorServer.Utils;
using MdcHR26Apps.Models.Views.v_ProcessTRListDB;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Common;

public partial class AdminViewExcel(
    ExcelManage excelManage,
    IJSRuntime js,
    NavigationManager navigationManager)
{
    [Parameter]
    public List<v_ProcessTRListDB>? ProcessTRLists { get; set; }

    [Parameter]
    public EventCallback<string> CommentChanged { get; set; }

    private string comment = string.Empty;

    private async Task ClickExportXLS()
    {
        if (ProcessTRLists == null || ProcessTRLists.Count == 0)
        {
            comment = "평가자가 없어 파일을 생성하지 못했습니다.";
            await CommentChanged.InvokeAsync(comment);
            return;
        }

        string fileFolderPath = "tasks";
        string fileNameAdd = DateTime.Now.ToString("yy_MM_dd_HH_mm_ss");
        string fileName = $"평가리스트_관리용_{fileNameAdd}.xlsx";

        // NavigationManager.BaseUri 사용 (자동으로 현재 환경 URL 감지)
        // string fileUrl = $"{navigationManager.BaseUri}files/tasks/{fileName}";
        // 한글 파일명 URL 안전성 위해 fileUrl 생성 시 인코딩설정
        string fileUrl = $"{navigationManager.BaseUri}files/tasks/{Uri.EscapeDataString(fileName)}";


        bool isCreateResult = excelManage.CreateAdminExcelFile(ProcessTRLists, fileFolderPath, fileName);

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
