using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Common
{
    public partial class SortSelectorComponent
    {
        public record SortOption(string Value, string Text);

        [Parameter] public string label { get; set; } = "정렬";
        [Parameter] public List<SortOption> sortOptions { get; set; } = new();

        [Parameter] public string sortField { get; set; } = "Name";
        [Parameter] public EventCallback<string> sortFieldChanged { get; set; }

        [Parameter] public bool sortAsc { get; set; } = true;
        [Parameter] public EventCallback<bool> sortAscChanged { get; set; }

        private async Task ToggleSortDir()
        {
            await sortAscChanged.InvokeAsync(!sortAsc);
        }
    }
}
