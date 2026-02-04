using MdcHR26Apps.Models.EvaluationTasks;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Form;

public partial class FormTaskItem
{
    #region Parameters
    [Parameter] public TasksDb task { get; set; } = new();
    [Parameter] public EventCallback<TasksDb> OnChange { get; set; }
    #endregion
}
