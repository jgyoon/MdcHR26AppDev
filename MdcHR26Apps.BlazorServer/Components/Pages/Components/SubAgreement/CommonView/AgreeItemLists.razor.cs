using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.SubAgreement.CommonView;

public partial class AgreeItemLists
{
    #region Parameters
    [Parameter] public List<AgreeItemModel> items { get; set; } = new();
    [Parameter] public bool Collapsed { get; set; } = false;
    [Parameter] public EventCallback Toggle { get; set; }
    [Parameter] public int itemCount { get; set; } = 0;
    #endregion

    #region Variables
    private string IconClass => Collapsed ? "oi oi-minus" : "oi oi-plus";
    private string CheckedCode = "\u2713" + " ";
    #endregion

    #region Model
    public class AgreeItemModel
    {
        public string ItemName { get; set; } = string.Empty;
        public int ItemPeroportion { get; set; }
        public int ItemSubPeroportion { get; set; }
        public bool ItemCompleteStatus { get; set; }
    }
    #endregion
}
