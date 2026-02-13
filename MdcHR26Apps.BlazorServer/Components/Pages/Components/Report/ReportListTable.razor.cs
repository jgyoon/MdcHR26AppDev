using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.EvaluationReport;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Report;

public partial class ReportListTable
{
    #region Parameters
    [Parameter] public List<ReportDb> reports { get; set; } = new();
    #endregion

    // 페이지 이동
    [Inject]
    public UrlActions urlActions { get; set; } = null!;

    #region Variables
    private int sortNo = 1;
    #endregion

    protected override void OnParametersSet()
    {
        sortNo = 1;
        base.OnParametersSet();
    }

    #region + ReportEditAction : 평가 수정페이지 이동
    private void ReportEditAction(Int64 Rid)
    {
        urlActions.Move1stReportEditPage(Rid);
    }
    #endregion

    /// <summary>
    /// 아이템 순번 메서드
    /// </summary>
    private int sortNoAdd2(int sort)
    {
        if (reports.Count == sortNo)
        {
            sortNo = 1;
            return sort;
        }
        sortNo = sort + 1;
        return sort;
    }
}
