using MdcHR26Apps.BlazorServer.Utils;
using MdcHR26Apps.Models.EvaluationTasks;
using Microsoft.AspNetCore.Components;
using System.Text.RegularExpressions;
using System.Web;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Form
{
    public partial class FormAgreeTask
    {
        [Parameter]
        public IEnumerable<TasksDb> tasks { get; set; } = new List<TasksDb>();

        [Parameter]
        public EventCallback<Int64> OnDeleteTask { get; set; }

        public TaskUtils taskutils = new TaskUtils();

        private async Task DeleteTask(Int64 taskId)
        {
            await OnDeleteTask.InvokeAsync(taskId);
        }

        private string GetTaskLevel(double TaskLevelScore)
        {
            return taskutils.LeveltoName(TaskLevelScore);
        }

        #region + 문자열 변경
        /// <summary>
        /// 문자열을 웹형식에 맞추어서 변환하는 메서드
        /// </summary>
        /// <param name="contenct">string</param>
        /// <returns>웹형식의 문자열</returns>
        private string replaceString(string contenct)
        {
            return Regex.Replace(HttpUtility.HtmlEncode(contenct), "\r?\n|\r", "<br />");
        }
        #endregion
    }
}
