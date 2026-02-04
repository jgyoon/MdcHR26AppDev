using MdcHR26Apps.Models.EvaluationTasks;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.SubAgreement.CommonView;

public partial class ReportTaskListCommonView
{
    #region Inject
    [Inject] private ITasksRepository tasksRepository { get; set; } = null!;
    #endregion

    #region Parameters
    [Parameter] public long SubAgreementId { get; set; }
    [Parameter] public EventCallback<long> OnSelectTask { get; set; }
    #endregion

    #region Variables
    private List<TasksDb> tasks = new();
    #endregion

    #region Lifecycle
    protected override async Task OnInitializedAsync()
    {
        await LoadTasks();
    }
    #endregion

    #region Methods
    private async Task LoadTasks()
    {
        // SubAgreement?Ä ?∞Í≤∞???ÖÎ¨¥ Î™©Î°ù Î°úÎìú
        // ?§Ï†ú Íµ¨ÌòÑ ???ÑÌÑ∞Îß?Î°úÏßÅ Ï∂îÍ?
        tasks = new List<TasksDb>();
    }
    #endregion
}
