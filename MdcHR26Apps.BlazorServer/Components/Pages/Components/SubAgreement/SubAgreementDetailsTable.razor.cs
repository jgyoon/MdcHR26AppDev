using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.EvaluationSubAgreement;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.SubAgreement;

public partial class SubAgreementDetailsTable
{
    #region Parameters
    [Parameter] public List<SubAgreementDb> subAgreements { get; set; } = new();
    #endregion

    // 페이지 이동
    [Inject] public UrlActions urlActions { get; set; } = null!;

    // 아이템 순번
    public int sortNo = 1;

    /// <summary>
    /// 아이템 순번 메서드
    /// </summary>
    /// <param name="sort"></param>
    /// <returns></returns>
    private int sortNoAdd2(int sort)
    {
        if (subAgreements.Count == sortNo)
        {
            sortNo = 1;
            return sort;
        }
        sortNo = sort + 1;
        return sort;
    }

    #region + MoveTeamLeaderAgreementSubDetailsPage : 세부상세페이지 이동
    private void TeamLeaderAgreementSubDetailsAction(Int64 Sid)
    {
        urlActions.MoveTeamLeaderSubAgreementSubDetailsPage(Sid);
    }
    #endregion

    protected override void OnParametersSet()
    {
        sortNo = 1;
        base.OnParametersSet();
    }
}
