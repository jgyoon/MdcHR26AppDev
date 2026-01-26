using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Common
{
    public partial class SearchbarComponent
    {
        [Parameter]
        public string searchTerm { get; set; } = string.Empty;

        [Parameter]
        public EventCallback<string> SearchAction { get; set; }

        [Parameter]
        public EventCallback<string> SearchInitAction { get; set; }

        private async Task OnSearchAction()
        {
            string newSearchValue = !String.IsNullOrEmpty(searchTerm) ? searchTerm : string.Empty;

            if (!String.IsNullOrEmpty(newSearchValue))
            {
                await SearchAction.InvokeAsync(newSearchValue);
            }
        }

        private async Task SearchEnterKeyPress(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await OnSearchAction();
            }
        }

        private async Task OnSearchInitAction()
        {
            await SearchInitAction.InvokeAsync();
        }
    }
}
