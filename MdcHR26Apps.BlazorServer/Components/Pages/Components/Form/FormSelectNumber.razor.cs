using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Form
{
    public partial class FormSelectNumber
    {
        [Parameter]
        public string Id { get; set; } = Guid.NewGuid().ToString(); // 고유한 값 부여

        [Parameter]
        public string? Label { get; set; }

        [Parameter]
        public int BindValue { get; set; }

        [Parameter]
        public bool IsDisabled { get; set; } = false; // 기본값은 false

        [Parameter]
        public bool IsEdit { get; set; } = false; // 기본값은 false

        [Parameter]
        public int maxperoportion { get; set; } = 100;

        [Parameter]
        public int peroportion { get; set; } = 0;

        [Parameter]
        public EventCallback<int> PeroportionChanged { get; set; }

        private async Task OnPeroportionChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e?.Value?.ToString(), out var newValue))
            {
                peroportion = newValue;
                await PeroportionChanged.InvokeAsync(newValue);
            }
        }
    }
}
