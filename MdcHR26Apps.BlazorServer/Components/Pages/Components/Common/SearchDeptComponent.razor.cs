using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Common
{
    public partial class SearchDeptComponent
    {
        [Parameter]
        public List<string> deptlist { get; set; } = new List<string>();

        [Parameter]
        public EventCallback<string> SearchDeptAction { get; set; }

        [Parameter]
        public EventCallback<string> SearchInitAction { get; set; }

        public string selectedDept { get; set; } = string.Empty;

        private async Task OnSearchAction()
        {
            string newSearchValue = !String.IsNullOrEmpty(selectedDept) ? selectedDept : string.Empty;

            if (!String.IsNullOrEmpty(newSearchValue))
            {
                await SearchDeptAction.InvokeAsync(newSearchValue);
            }
        }
    }
}
