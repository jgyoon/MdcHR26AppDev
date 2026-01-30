using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.BlazorServer.Utils;
using MdcHR26Apps.Models.EvaluationAgreement;
using MdcHR26Apps.Models.EvaluationProcess;
using MdcHR26Apps.Models.EvaluationReport;
using MdcHR26Apps.Models.EvaluationSubAgreement;
using MdcHR26Apps.Models.EvaluationTasks;
using MdcHR26Apps.Models.Result;
using MdcHR26Apps.Models.Views.v_ProcessTRListDB;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin.TotalReport;

public partial class ReportInit(
    Iv_ProcessTRListRepository processTRRepository,
    IProcessRepository processRepository,
    ITotalReportRepository totalReportRepository,
    IReportRepository reportRepository,
    ITasksRepository tasksRepository,
    IAgreementRepository agreementRepository,
    ISubAgreementRepository subAgreementRepository,
    LoginStatusService loginStatusService,
    UrlActions urlActions)
{
    [Parameter]
    public long Id { get; set; }

    private v_ProcessTRListDB? model;
    private string isTotalReport = "없음";
    private int reportCount = 0;
    private int taskListCount = 0;
    private int subAgreementCount = 0;
    private int agreementCount = 0;
    private bool showModal = false;

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
            subAgreementCount = await subAgreementRepository.GetCountByUidAsync(model.Uid);
            agreementCount = await agreementRepository.GetCountByUidAsync(model.Uid);
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

    private void ShowInitModal()
    {
        showModal = true;
    }

    private async Task HandleInitConfirm()
    {
        if (model == null) return;

        // 1. TotalReportDb 삭제
        if (model.TRid > 0)
        {
            await totalReportRepository.DeleteAsync(model.TRid);
        }

        // 2. ReportDb 전체 삭제
        await reportRepository.DeleteAllByUidAsync(model.Uid);

        // 3. TasksDb 전체 삭제
        await tasksRepository.DeleteAllByUserAsync(model.Uid);

        // 4. SubAgreementDb 전체 삭제
        await subAgreementRepository.DeleteAllByUidAsync(model.Uid);

        // 5. AgreementDb 전체 삭제
        await agreementRepository.DeleteAllByUidAsync(model.Uid);

        // 6. ProcessDb 상태 초기화
        var process = await processRepository.GetByUidAsync(model.Uid);
        if (process != null)
        {
            process.Is_Request = false;
            process.Is_Agreement = false;
            process.Agreement_Comment = string.Empty;
            process.Is_SubRequest = false;
            process.Is_SubAgreement = false;
            process.SubAgreement_Comment = string.Empty;
            process.Is_User_Submission = false;
            process.Is_Teamleader_Submission = false;
            process.Is_Director_Submission = false;
            process.FeedBackStatus = false;
            process.FeedBack_Submission = false;

            await processRepository.UpdateAsync(process);
        }

        showModal = false;
        urlActions.MoveTotalReportAdminIndexPage();
    }

    private void HandleInitCancel()
    {
        showModal = false;
    }
}
