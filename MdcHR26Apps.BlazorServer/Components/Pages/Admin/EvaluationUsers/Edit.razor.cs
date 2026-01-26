using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.EvaluationProcess;
using MdcHR26Apps.Models.EvaluationUsers;
using MdcHR26Apps.Models.User;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin.EvaluationUsers;

public partial class Edit(
    IUserRepository userRepository,
    IEvaluationUsersRepository evaluationUsersRepository,
    IProcessRepository processRepository,
    LoginStatusService loginStatusService,
    UrlActions urlActions)
{
    #region Parameters
    [Parameter]
    public Int64 Id { get; set; }
    #endregion

    // 사용자정보
    private UserDb user { get; set; } = new UserDb();
    private List<UserDb> teamleaderlist { get; set; } = new List<UserDb>();
    private List<UserDb> directorlist { get; set; } = new List<UserDb>();

    // 평가대상자
    private Models.EvaluationUsers.EvaluationUsers model { get; set; } = new Models.EvaluationUsers.EvaluationUsers();

    // 평가순서관리
    private ProcessDb processDb { get; set; } = new ProcessDb();

    // UI 바인딩용
    private string selectedTeamLeaderName { get; set; } = string.Empty;
    private string selectedDirectorName { get; set; } = string.Empty;

    // 기타
    private string resultText { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await CheckLogined();
        await SetData(Id);
        await base.OnInitializedAsync();
    }

    private async Task SetData(Int64 Id)
    {
        model = await evaluationUsersRepository.GetByIdAsync(Id);

        if (model != null && model.Uid != 0)
        {
            var userResult = await userRepository.GetByIdAsync(model.Uid);
            if (userResult != null)
            {
                user = userResult;
            }

            // 평가자 목록 조회
            var teamLeaders = await userRepository.GetTeamLeadersAsync();
            var directors = await userRepository.GetDirectorsAsync();
            teamleaderlist = teamLeaders.ToList();
            directorlist = directors.ToList();

            // 현재 선택된 평가자 이름 조회
            if (model.TeamLeaderId.HasValue)
            {
                var teamLeader = teamleaderlist.FirstOrDefault(u => u.Uid == model.TeamLeaderId.Value);
                selectedTeamLeaderName = teamLeader?.UserName ?? string.Empty;
            }

            if (model.DirectorId.HasValue)
            {
                var director = directorlist.FirstOrDefault(u => u.Uid == model.DirectorId.Value);
                selectedDirectorName = director?.UserName ?? string.Empty;
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
    #endregion

    #region Edit User
    private async Task EditUser()
    {
        // 평가 대상자만 수정하고 아니면 평가순서 삭제
        if (model.Is_Evaluation)
        {
            if (model.Uid != 0 &&
                !string.IsNullOrEmpty(selectedTeamLeaderName) &&
                !string.IsNullOrEmpty(selectedDirectorName))
            {
                model.TeamLeaderId = SearchTeamLeaderUid(selectedTeamLeaderName);
                model.DirectorId = SearchDirectorUid(selectedDirectorName);

                int updateResult = await evaluationUsersRepository.UpdateAsync(model);

                if (updateResult > 0)
                {
                    resultText = "평가대상자 수정성공";

                    // 평가순서가 있으면 수정 없으면 생성
                    if (await processRepository.CheckUidAsync(model.Uid))
                    {
                        if (await EditProcessUser(model))
                        {
                            resultText = "평가순서 수정성공";
                        }
                    }
                    else
                    {
                        if (await CreateProcessUser(model))
                        {
                            resultText = "평가순서 생성성공";
                        }
                    }

                    StateHasChanged();
                    MoveEUserManagePage();
                }
                else
                {
                    resultText = "Failure";
                }
            }
            else
            {
                resultText = "사용자 수정에 실패했습니다.(입력실패0)";
            }
        }
        else
        {
            if (model.Uid != 0)
            {
                model.TeamLeaderId = !string.IsNullOrEmpty(selectedTeamLeaderName)
                    ? SearchTeamLeaderUid(selectedTeamLeaderName)
                    : null;
                model.DirectorId = !string.IsNullOrEmpty(selectedDirectorName)
                    ? SearchDirectorUid(selectedDirectorName)
                    : null;

                int updateResult = await evaluationUsersRepository.UpdateAsync(model);

                if (updateResult > 0 && !model.Is_Evaluation)
                {
                    resultText = "평가대상자 수정성공";

                    // 평가순서가 있으면 삭제
                    if (await processRepository.CheckUidAsync(model.Uid))
                    {
                        if (await DeleteProcessUser(model))
                        {
                            resultText = "평가순서 삭제성공";
                        }
                    }

                    StateHasChanged();
                    MoveEUserManagePage();
                }
                else
                {
                    resultText = "Failure";
                }
            }
            else
            {
                resultText = "사용자 수정에 실패했습니다.(입력실패1)";
            }
        }
    }
    #endregion

    #region Search Helper Methods
    private Int64? SearchTeamLeaderUid(string userName)
    {
        if (string.IsNullOrEmpty(userName))
            return null;

        var leader = teamleaderlist.FirstOrDefault(e => e.UserName == userName);
        return leader?.Uid;
    }

    private Int64? SearchDirectorUid(string userName)
    {
        if (string.IsNullOrEmpty(userName))
            return null;

        var director = directorlist.FirstOrDefault(e => e.UserName == userName);
        return director?.Uid;
    }
    #endregion

    #region Process Management
    private async Task<bool> EditProcessUser(Models.EvaluationUsers.EvaluationUsers model)
    {
        var result = await processRepository.GetByUidAsync(model.Uid);

        if (result != null)
        {
            processDb = result;
            processDb.Uid = model.Uid;
            processDb.TeamLeaderId = model.TeamLeaderId;
            processDb.DirectorId = model.DirectorId;

            int updateResult = await processRepository.UpdateAsync(processDb);
            return updateResult > 0;
        }

        return false;
    }

    private async Task<bool> CreateProcessUser(Models.EvaluationUsers.EvaluationUsers model)
    {
        processDb = new ProcessDb
        {
            Uid = model.Uid,
            TeamLeaderId = model.TeamLeaderId,
            DirectorId = model.DirectorId,
            Is_Request = false,
            Is_Agreement = false,
            Agreement_Comment = string.Empty,
            Is_SubRequest = false,
            Is_SubAgreement = false,
            SubAgreement_Comment = string.Empty,
            Is_User_Submission = false,
            Is_Teamleader_Submission = false,
            Is_Director_Submission = false,
            FeedBackStatus = false,
            FeedBack_Submission = false
        };

        var pid = await processRepository.AddAsync(processDb);
        return pid != 0;
    }

    private async Task<bool> DeleteProcessUser(Models.EvaluationUsers.EvaluationUsers model)
    {
        var result = await processRepository.GetByUidAsync(model.Uid);

        if (result != null)
        {
            processDb = result;
            int deleteResult = await processRepository.DeleteAsync(processDb.Pid);
            return deleteResult > 0;
        }

        return false;
    }
    #endregion
}
