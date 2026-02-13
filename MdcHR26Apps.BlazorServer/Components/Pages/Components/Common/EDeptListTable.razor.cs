using MdcHR26Apps.Models.EvaluationLists;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Common;

public partial class EDeptListTable
{
    #region Parameters
    [Parameter] public List<EvaluationLists> evaluationLists { get; set; } = new();
    #endregion
}
