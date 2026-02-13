using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Agreement.CommonView;

public partial class AgreementComment
{
    #region Parameters
    [Parameter] public string Comment { get; set; } = string.Empty;
    [Parameter] public bool Collapsed { get; set; } = false;
    [Parameter] public EventCallback Toggle { get; set; }
    #endregion

    #region Variables
    private bool _internalCollapsed = false;
    private bool EffectiveCollapsed => Toggle.HasDelegate ? Collapsed : _internalCollapsed;
    private string IconClass => EffectiveCollapsed ? "oi oi-minus" : "oi oi-plus";
    #endregion

    #region Methods
    private async Task HandleToggle()
    {
        if (Toggle.HasDelegate)
        {
            await Toggle.InvokeAsync();
        }
        else
        {
            _internalCollapsed = !_internalCollapsed;
        }
    }

    private string FormatComment(string comment)
    {
        // 줄바꿈을 <br> 변경
        return comment.Replace("\n", "<br />");
    }
    #endregion
}
