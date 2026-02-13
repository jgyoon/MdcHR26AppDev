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

        [Parameter] public string selectedDept { get; set; } = string.Empty;
        [Parameter] public EventCallback<string> SelectedDeptChanged { get; set; }


        private async Task OnSearchAction()
        {
            string newSearchValue = !String.IsNullOrEmpty(selectedDept) ? selectedDept : string.Empty;
            await SelectedDeptChanged.InvokeAsync(newSearchValue);   // 부모 상태 동기화
            await SearchDeptAction.InvokeAsync(newSearchValue);      // 검색 실행(전체 포함)

            // if (!String.IsNullOrEmpty(newSearchValue))
            // {
            //     await SearchDeptAction.InvokeAsync(newSearchValue);
            // }
        }
    }
}
