using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.EvaluationUsers;
using MdcHR26Apps.Models.User;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Pages.Admin.EvaluationUsers;

public partial class Details(
    IEvaluationUsersRepository evaluationUsersRepository,
    IUserRepository userRepository,
    LoginStatusService loginStatusService,
    UrlActions urlActions)
{
    #region Parameters
    [Parameter]
    public Int64 Id { get; set; }
    #endregion

    // 평가대상자정보
    private Models.EvaluationUsers.EvaluationUsers model { get; set; } = new Models.EvaluationUsers.EvaluationUsers();
    private UserDb user { get; set; } = new UserDb();

    // 평가자 이름
    private string? teamLeaderName { get; set; }
    private string? directorName { get; set; }

    // 기타
    private string isSetupString = "미지정";

    protected override async Task OnInitializedAsync()
    {
        await CheckLogined();
        await SetData(Id);
    }

    private async Task SetData(Int64 id)
    {
        model = await evaluationUsersRepository.GetByIdAsync(id);

        if (model != null && model.Uid != 0)
        {
            // 사용자 정보 조회
            user = await userRepository.GetByIdAsync(model.Uid);

            // 평가자 이름 조회
            if (model.TeamLeaderId.HasValue)
            {
                var teamLeader = await userRepository.GetByIdAsync(model.TeamLeaderId.Value);
                teamLeaderName = teamLeader?.UserName;
            }

            if (model.DirectorId.HasValue)
            {
                var director = await userRepository.GetByIdAsync(model.DirectorId.Value);
                directorName = director?.UserName;
            }
        }
    }

    #region Login Check
    private async Task CheckLogined()
    {
        await Task.Delay(1);
        if (!loginStatusService.IsloginAndIsAdminCheck())
        {
            StateHasChanged();
            urlActions.MoveMainPage();
        }
    }
    #endregion

    #region Page Navigation
    private void MoveMainPage()
    {
        urlActions.MoveMainPage();
    }

    private void MoveEUserManagePage()
    {
        urlActions.MoveEUserManagePage();
    }

    private void MoveEUsersEditPage(Int64 Uid)
    {
        urlActions.MoveEUsersEditPage(Uid);
    }
    #endregion

    private string isuse(bool isEvaluation)
    {
        return isEvaluation ? "평가대상자" : "미평가대상자";
    }
}
