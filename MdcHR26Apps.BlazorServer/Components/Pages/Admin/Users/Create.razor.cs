using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.BlazorServer.Utils;
using MdcHR26Apps.Models.EvaluationProcess;
using MdcHR26Apps.Models.EvaluationUsers;
using MdcHR26Apps.Models.User;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin.Users
{
    public partial class Create
    {
        // 사용자정보
        [Inject]
        public IUserRepository userRepository { get; set; } = null!;
        public UserDb model { get; set; } = new UserDb();

        // 로그인관리(상태관리)
        [Inject]
        public LoginStatusService loginStatusService { get; set; } = null!;

        // 페이지이동
        [Inject]
        public UrlActions urlActions { get; set; } = null!;

        // 평가대상자 추가
        [Inject]
        public IEvaluationUsersRepository evaluationUsersRepository { get; set; } = null!;
        public Models.EvaluationUsers.EvaluationUsers evaluationUsers { get; set; } = new Models.EvaluationUsers.EvaluationUsers();

        // 평가순서관리
        [Inject]
        public IProcessRepository processRepository { get; set; } = null!;
        public ProcessDb processDb { get; set; } = new ProcessDb();

        // 기타
        public string resultText { get; set; } = String.Empty;
        public string resultUser { get; set; } = String.Empty;
        public string resultUserColor { get; set; } = String.Empty;

        // 공용함수 호출
        [Inject]
        public UserUtils utils { get; set; } = null!;
        public List<string> deptlist = new List<string>();
        public List<string> ranklist = new List<string>();

        // 2026년: 부서/직급 선택값 (EDepartId/ERankId로 변환 필요)
        private string selectedDeptName { get; set; } = "none";
        private string selectedRankName { get; set; } = "none";

        // 비밀번호 (UI 바인딩용)
        private string password { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await CheckLogined();
            await SetData();
            await base.OnInitializedAsync();
        }

        private async Task CheckLogined()
        {
            await Task.Delay(0);
            if (!loginStatusService.IsloginAndIsAdminCheck())
            {
                StateHasChanged();
                urlActions.MoveMainPage();
            }
        }

        private async Task SetData()
        {
            deptlist = await utils.GetDeptListAsync();
            ranklist = await utils.GetRankListAsync();
        }

        #region + 사용자 추가 : CreateUser
        private async Task CreateUser()
        {
            if (model.UserId != null && password != null && model.UserName != null)
            {
                if (!await userRepository.UserIdCheckAsync(model.UserId))
                {
                    // 2026년: 부서명/직급명 → EDepartId/ERankId 변환
                    var dept = await utils.GetDepartmentByNameAsync(selectedDeptName);
                    var rank = await utils.GetRankByNameAsync(selectedRankName);

                    if (dept == null || rank == null)
                    {
                        resultText = "부서 또는 직급을 선택해주세요.";
                        return;
                    }

                    model.EDepartId = dept.EDepartId;
                    model.ERankId = rank.ERankId;
                    model.Password = password;  // UI에서 입력받은 비밀번호 할당
                    model.Email = model.Email == string.Empty || model.Email == null ? string.Empty : model.Email;
                    model.ENumber = model.ENumber == string.Empty || model.ENumber == null ? string.Empty : model.ENumber;
                    model.IsAdministrator = false;
                    model.EStatus = true;

                    // 1단계: UserDb 생성 (Repository에서 비밀번호 해싱 처리)
                    var uid = await userRepository.AddAsync(model);
                    if (uid != 0)
                    {
                        model.Uid = uid;

                        // 2단계: EvaluationUsers 추가
                        await EvaluationUsersAdd(model);
                        // 3단계: ProcessDb 추가
                        await ProcessAdd(model);

                        StateHasChanged();
                        MoveUserManagePage();
                    }
                    else
                    {
                        await Console.Out.WriteLineAsync(uid.ToString());
                        resultText = "사용자 생성에 실패했습니다.(입력실패2)";
                    }
                }
                else
                {
                    resultUser = "같은 ID가 존재합니다.";
                    resultUserColor = "color:red";
                    resultText = "사용자 생성에 실패했습니다.(입력실패1)";
                }
            }
            else
            {
                resultText = "사용자 생성에 실패했습니다.(입력실패0)";
            }
        }
        #endregion

        #region + 사용자관리페이지 이동
        protected void MoveUserManagePage()
        {
            urlActions.MoveUserManagePage();
        }
        #endregion

        #region + 아이디 중복체크
        private async Task CheckUserId(string id)
        {
            // id가 null 이거나 Empty 이면 return
            if (string.IsNullOrEmpty(id))
                return;

            if (!await userRepository.UserIdCheckAsync(id))
            {
                resultUser = "사용가능한 ID입니다.";
                resultUserColor = "color:blue";
            }
            else
            {
                resultUser = "같은 ID가 존재합니다.";
                resultUserColor = "color:red";
            }
        }
        #endregion

        #region + 평가대상자 추가 (2026년: Uid, TeamLeaderId, DirectorId 사용)
        private async Task EvaluationUsersAdd(UserDb model)
        {
            evaluationUsers.Uid = model.Uid;  // 2026년: UserId → Uid (BIGINT)
            // UserName 필드 제거 (2026년 DB에 없음)
            evaluationUsers.Is_Evaluation = true;
            evaluationUsers.TeamLeaderId = null;  // 2026년: String.Empty → null (BIGINT nullable)
            evaluationUsers.DirectorId = null;    // 2026년: String.Empty → null (BIGINT nullable)
            evaluationUsers.Is_TeamLeader = false;

            if (!await evaluationUsersRepository.CheckUidAsync(evaluationUsers.Uid))
            {
                await evaluationUsersRepository.AddAsync(evaluationUsers);
            }
        }
        #endregion

        #region + 평가순서 추가 (2026년: Uid 사용, Sub 필드 추가)
        private async Task ProcessAdd(UserDb model)
        {
            processDb.Uid = model.Uid;  // 2026년: UserId → Uid (BIGINT)
            // UserName 필드 제거 (2026년 DB에 없음)
            processDb.TeamLeaderId = null;  // 2026년: String.Empty → null (BIGINT nullable)
            processDb.DirectorId = null;    // 2026년: String.Empty → null (BIGINT nullable)
            processDb.Is_Request = false;
            processDb.Is_Agreement = false;
            processDb.Agreement_Comment = String.Empty;
            processDb.Is_SubRequest = false;  // 2026년 추가 필드
            processDb.Is_SubAgreement = false;  // 2026년 추가 필드
            processDb.SubAgreement_Comment = String.Empty;  // 2026년 추가 필드
            processDb.Is_User_Submission = false;
            processDb.Is_Teamleader_Submission = false;
            processDb.Is_Director_Submission = false;
            processDb.FeedBackStatus = false;
            processDb.FeedBack_Submission = false;

            if (!await processRepository.CheckUidAsync(processDb.Uid))
            {
                await processRepository.AddAsync(processDb);
            }
        }
        #endregion
    }
}
