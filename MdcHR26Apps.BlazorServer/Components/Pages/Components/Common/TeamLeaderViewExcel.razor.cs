using MdcHR26Apps.BlazorServer.Utils;
using MdcHR26Apps.Models.Views.v_ProcessTRListDB;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Common;

/// <summary>
/// 팀장용 평가 리스트 엑셀 다운로드 컴포넌트
/// </summary>
public partial class TeamLeaderViewExcel(
    ExcelManage excelManage,
    IJSRuntime js,
    NavigationManager navigationManager)
{
    [Parameter]
    public List<v_ProcessTRListDB>? processTRLists { get; set; }

    public string Comment { get; set; } = string.Empty;

    private async Task ClickExportXLS()
    {
        if (processTRLists == null || processTRLists.Count == 0)
        {
            Comment = "평가자가 없어 파일을 생성하지 못했습니다.";
            return;
        }

        string fileFolderPath = "tasks";
        string fileNameAdd = DateTime.Now.ToString("yy_MM_dd_HH_mm_ss");
        string fileName = $"평가리스트_팀장용_{fileNameAdd}.xlsx";

        // NavigationManager.BaseUri 사용 (자동으로 현재 환경 URL 감지)
        // string fileUrl = $"{navigationManager.BaseUri}files/tasks/{fileName}";
        // 한글 파일명 URL 안전성 위해 fileUrl 생성 시 인코딩설정
        string fileUrl = $"{navigationManager.BaseUri}files/tasks/{Uri.EscapeDataString(fileName)}";


        bool isCreateResult = excelManage.CreateTeamLeaderExcelFile(processTRLists, fileFolderPath, fileName);

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