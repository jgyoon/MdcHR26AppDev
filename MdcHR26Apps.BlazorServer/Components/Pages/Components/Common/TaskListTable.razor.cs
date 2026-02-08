using MdcHR26Apps.BlazorServer.Utils;
using MdcHR26Apps.Models.EvaluationTasks;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Common;

public partial class TaskListTable
{
    [Parameter] public List<TasksDb> tasklist { get; set; } = new List<TasksDb>();

    // 테이블 CSS Style
    public string table_style_2 = "text-align: center; vertical-align: middle;";

    public int sortNo = 1;

    // 공통함수 호출
    public TaskUtils taskUtils = new TaskUtils();

    private int sortNoAdd(int sort)
    {
        sortNo = sort + 1;
        return sort;
    }
}
