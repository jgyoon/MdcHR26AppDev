using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.SubAgreement.CommonView;

public partial class AgreeItemLists
{
    #region Parameters
    [Parameter] public List<AgreeItemModel> items { get; set; } = new();
    #endregion

    #region Variables
    private bool Collapsed = false;
    private string IconClass => Collapsed ? "oi oi-minus" : "oi oi-plus";
    private int itemCount => items?.Count ?? 0;
    private string CheckedCode = "??";
    #endregion

    #region Methods
    private void Toggle()
    {
        Collapsed = !Collapsed;
    }
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
