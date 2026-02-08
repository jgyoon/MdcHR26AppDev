using MdcHR26Apps.BlazorServer.Utils;
using MdcHR26Apps.Models.EvaluationTasks;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Form;

public partial class FormTaskItem
{
    [Parameter]
    public TasksDb task { get; set; } = new TasksDb();

    // 공용함수 호출
    public TaskUtils taskutils = new TaskUtils();
}
