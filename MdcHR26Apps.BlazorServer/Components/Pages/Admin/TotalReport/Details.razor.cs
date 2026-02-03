using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.BlazorServer.Utils;
using MdcHR26Apps.Models.EvaluationAgreement;
using MdcHR26Apps.Models.EvaluationReport;
using MdcHR26Apps.Models.EvaluationSubAgreement;
using MdcHR26Apps.Models.EvaluationTasks;
using MdcHR26Apps.Models.Views.v_ProcessTRListDB;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin.TotalReport;

public partial class Details(
    Iv_ProcessTRListRepository processTRRepository,
    IReportRepository reportRepository,
    ITasksRepository tasksRepository,
    IAgreementRepository agreementRepository,
    ISubAgreementRepository subAgreementRepository,
    LoginStatusService loginStatusService,
    UrlActions urlActions,
    ScoreUtils scoreUtils)
{
    [Parameter]
    public long Id { get; set; }

    private v_ProcessTRListDB? model;
    private string isTotalReport = "없음";
    private int reportCount = 0;
    private int taskListCount = 0;
    private int subAgreementCount = 0;
    private int agreementCount = 0;

    protected override async Task OnInitializedAsync()
    {
        await CheckLogined();
        await LoadData();
    }

    private async Task CheckLogined()
    {
        await Task.CompletedTask;
        if (!loginStatusService.IsloginAndIsAdminCheck())
        {
            StateHasChanged();
            urlActions.MoveMainPage();
        }
    }

    private async Task LoadData()
    {
        model = await processTRRepository.GetByPidAsync(Id);

        if (model != null)
        {
            isTotalReport = model.TRid > 0 ? "있음" : "없음";
            reportCount = await reportRepository.GetCountByUidAsync(model.Uid);
            taskListCount = await tasksRepository.GetCountByUserAsync(model.Uid);

            // 25년 메서드 사용: GetByUserIdAllAsync
            var subAgreements = await subAgreementRepository.GetByUserIdAllAsync(model.Uid);
            subAgreementCount = subAgreements.Count;

            var agreements = await agreementRepository.GetByUserIdAllAsync(model.Uid);
            agreementCount = agreements.Count;
        }
    }

    private string GetDisplayStatus()
    {
        if (model == null) return "-";

        if (model.Is_Director_Submission) return "임원평가완료";
        if (model.Is_Teamleader_Submission) return "팀장평가완료";
        if (model.Is_User_Submission) return "본인평가완료";
        if (model.Is_SubAgreement) return "세부합의완료";
        if (model.Is_Agreement) return "직무합의완료";
        if (model.Is_SubRequest) return "세부합의요청";
        if (model.Is_Request) return "직무합의요청";
        return "대기중";
    }
}
