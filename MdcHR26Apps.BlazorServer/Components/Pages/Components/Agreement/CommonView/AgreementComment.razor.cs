using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Agreement.CommonView;

public partial class AgreementComment
{
    #region Parameters
    [Parameter] public string Comment { get; set; } = string.Empty;
    #endregion

    #region Variables
    private bool Collapsed = false;
    private string IconClass => Collapsed ? "oi oi-minus" : "oi oi-plus";
    #endregion

    #region Methods
    private void Toggle()
    {
        Collapsed = !Collapsed;
    }

    private string FormatComment(string comment)
    {
        // 줄바꿈을 <br> ?�그�?변??
        return comment.Replace("\n", "<br />");
    }
    #endregion
}
