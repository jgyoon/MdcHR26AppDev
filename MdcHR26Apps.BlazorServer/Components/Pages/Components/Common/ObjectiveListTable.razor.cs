using MdcHR26Apps.Models.DeptObjective;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Common;

public partial class ObjectiveListTable
{
    #region Parameters
    [Parameter] public List<DeptObjectiveDb> objectives { get; set; } = new();
    [Parameter] public EventCallback<long> OnEdit { get; set; }
    [Parameter] public EventCallback<long> OnDelete { get; set; }
    #endregion
}
