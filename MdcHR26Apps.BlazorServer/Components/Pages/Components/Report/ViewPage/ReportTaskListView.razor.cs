using MdcHR26Apps.Models.Views.v_ReportTaskListDB;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Report.ViewPage;

/// <summary>
/// 세부업무 리스트 카드 뷰 컴포넌트 (25년도 기준)
/// </summary>
public partial class ReportTaskListView
{
    [Parameter]
    public List<v_ReportTaskListDB> ReportTaskListDB { get; set; } = new List<v_ReportTaskListDB>();

    // 테이블 CSS Style
    public string table_style_1 = "white-space: nowrap; overflow: hidden; text-overflow: ellipsis; text-align: center; vertical-align: middle;";

    // 아이템 순번
    public int sortNo = 1;

    /// <summary>
    /// 아이템 순번 메서드
    /// </summary>
    /// <param name="sort"></param>
    /// <returns></returns>
    private int sortNoAdd2(int sort)
    {
        if (ReportTaskListDB.Count == sortNo)
        {
            sortNo = 1;
            return sort;
        }
        sortNo = sort + 1;
        return sort;
    }

    private string ReportItemName(string item_Name_1, string item_Name_2, int item_Proportion)
    {
        string reportItemName = $"{item_Name_1} - {item_Name_2} ({+item_Proportion} %)";
        return reportItemName;
    }

    private string ReportSubItemName(string subItem_Name, int subItem_Proportion)
    {
        string reportSubItemName = $"{subItem_Name}({+subItem_Proportion} %)";
        return reportSubItemName;
    }

    public string LeveltoName(double levelno)
    {
        string levelString = String.Empty;

        switch (levelno)
        {
            case 1.2:
                levelString = "매우 어려움";
                break;
            case 1.1:
                levelString = "어려움";
                break;
            case 1:
                levelString = "보통";
                break;
            case 0.9:
                levelString = "쉬움";
                break;
            case 0.8:
                levelString = "매우 쉬움";
                break;
            default:
                break;
        }

        return levelString;
    }
}