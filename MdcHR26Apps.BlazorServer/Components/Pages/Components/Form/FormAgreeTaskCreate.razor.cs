using MdcHR26Apps.BlazorServer.Models;
using MdcHR26Apps.BlazorServer.Utils;
using MdcHR26Apps.Models.EvaluationTasks;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Form
{
    public partial class FormAgreeTaskCreate
    {
        [Parameter]
        public TasksDb task { get; set; } = new TasksDb();

        [Parameter]
        public EventCallback<TasksDb> OnCreateTask { get; set; }

        public TaskUtils taskutils = new TaskUtils();
        public List<TaskLevelModel> taskLevels = new List<TaskLevelModel>();

        private string taskNameLength = "0";
        protected override async Task OnInitializedAsync()
        {
            await SetData();
            await base.OnInitializedAsync();
        }

        private async Task SetData()
        {
            await Task.Delay(0);
            taskLevels = taskutils.GetTaskLevels();
        }

        private async Task HandleValidSubmit()
        {
            await OnCreateTask.InvokeAsync(task);
            //task = new TasksDb(); // 폼을 초기화
        }

        private void UpdateTaskNameLength(ChangeEventArgs e)
        {
            var value = e?.Value?.ToString() ?? string.Empty;
            task.TaskName = value;
            taskNameLength = value?.Length.ToString() ?? "0";
        }
    }
}
