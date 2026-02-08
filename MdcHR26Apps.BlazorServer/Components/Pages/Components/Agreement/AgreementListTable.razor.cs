using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.Views.v_ProcessTRListDB;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Agreement;

public partial class AgreementListTable
{
    [Parameter]
    public List<v_ProcessTRListDB> processDbList { get; set; } = new List<v_ProcessTRListDB>();

    // 페이지 이동
    [Inject]
    public UrlActions urlActions { get; set; } = null!;

    // 테이블 CSS Style
    public string table_style_1 = "white-space: nowrap; overflow: hidden; text-overflow: ellipsis; text-align: center; vertical-align: middle;";

    // Text CSS Style
    public string text_style_0 = "white-space: nowrap; overflow: hidden; text-overflow: ellipsis; text-align: center; vertical-align: middle;";
    public string text_style_1 = "white-space: nowrap; overflow: hidden; text-overflow: ellipsis; text-align: center; vertical-align: middle; color: blue; font-style: italic;";
    public string text_style_2 = "white-space: nowrap; overflow: hidden; text-overflow: ellipsis; text-align: center; vertical-align: middle; color: red; font-weight: bold;";

    // 기타
    public int sortNo = 1;

    #region + 합의요청여부
    public string IsRequestStatus(bool status)
    {
        return status ? "요청(O)" : "요청(X)";
    }
    #endregion

    #region + 합의승인여부
    private string IsAgreementStatus(bool status)
    {
        return status ? "승인(O)" : "승인(X)";
    }
    #endregion

    #region + 평가제출여부 Is_User_Submission
    private string IsReportSubmissionStatus(bool status)
    {
        return status ? "제출(O)" : "제출(X)";
    }
    #endregion

    #region + 합의진행여부 Is_RequestAndAgreement
    private string Is_RequestAndAgreementStatus(bool status1, bool status2)
    {
        bool status = status1 && status2;
        return status ? "합의(O)" : "대기중";
    }
    #endregion

    private int sortNoAdd3(int sort)
    {
        if (processDbList.Count == sortNo)
        {
            sortNo = 1;
            return sort;
        }
        sortNo = sort + 1;
        return sort;
    }

    private void AgreementDetailsAction(Int64 Pid)
    {
        urlActions.MoveAgreementTeamLeaderDetailsPage(Pid);
    }

    protected override void OnParametersSet()
    {
        sortNo = 1;
        base.OnParametersSet();
    }
}
