using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.Department;
using MdcHR26Apps.Models.Rank;
using MdcHR26Apps.Models.User;
using Microsoft.AspNetCore.Components;
using System.Security.Cryptography;
using System.Text;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin.Users
{
    public partial class Edit
    {
        #region Parameters
        [Parameter]
        public Int64 Uid { get; set; }
        #endregion

        // 페이지이동
        [Inject]
        public UrlActions urlActions { get; set; } = null!;

        // 로그인관리(상태관리)
        [Inject]
        public LoginStatusService loginStatusService { get; set; } = null!;

        // 사용자정보
        [Inject]
        public IUserRepository userRepository { get; set; } = null!;
        public UserDb model { get; set; } = new UserDb();

        // 직급정보
        [Inject]
        public IERankRepository eRankRepository { get; set; } = null!;
        public List<ERankDb> eRankDbList { get; set; } = new List<ERankDb>();
        public ERankDb eRankDb { get; set; } = new ERankDb();

        // 부서정보
        [Inject]
        public IEDepartmentRepository eDepartmentRepository { get; set; } = null!;
        public List<EDepartmentDb> eDepartmentDbList { get; set; } = new List<EDepartmentDb>();
        public EDepartmentDb eDepartmentDb { get; set; } = new EDepartmentDb();

        // 기타
        public string resultText { get; set; } = string.Empty;
        public string eDepartmentName { get; set; } = string.Empty;
        public string eRankName { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await CheckLogined();
            await SetData(Uid);
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

        private async Task SetData(Int64 uid)
        {
            // 사용자 정보 조회
            model = await userRepository.GetByIdAsync(uid) ?? new UserDb();

            if (model != null && model.Uid != 0)
            {
                // 현재 직급/부서 정보 조회 (nullable 처리)
                if (model.ERankId.HasValue)
                {
                    eRankDb = await eRankRepository.GetByIdAsync(model.ERankId.Value) ?? new ERankDb();
                }

                if (model.EDepartId.HasValue)
                {
                    eDepartmentDb = await eDepartmentRepository.GetByIdAsync(model.EDepartId.Value) ?? new EDepartmentDb();
                }
            }

            // 전체 직급/부서 목록 조회 (드롭다운용)
            var ranks = await eRankRepository.GetByAllAsync();
            eRankDbList = ranks.ToList();

            var depts = await eDepartmentRepository.GetByAllAsync();
            eDepartmentDbList = depts.ToList();

            // 현재 부서명/직급명 설정
            eDepartmentName = eDepartmentDb.EDepartmentName ?? string.Empty;
            eRankName = eRankDb.ERankName ?? string.Empty;
        }

        #region + [3] EditUser : 사용자 정보 수정
        private async Task EditUser()
        {
            if (!String.IsNullOrEmpty(model.UserName))
            {
                model.Email = String.IsNullOrEmpty(model.Email) ? string.Empty : model.Email;
                model.ENumber = String.IsNullOrEmpty(model.ENumber) ? string.Empty : model.ENumber;

                // 2026년: UpdateWithoutPasswordAsync 반환값이 int (영향받은 행 수)
                int rowsAffected = await userRepository.UpdateWithoutPasswordAsync(model);

                if (rowsAffected > 0)
                {
                    resultText = "사용자 수정성공";
                    StateHasChanged();
                    UserManagePage();
                }
                else
                {
                    resultText = "사용자 수정에 실패했습니다.(입력실패1)";
                }
            }
            else
            {
                resultText = "사용자 수정에 실패했습니다.(입력실패0)";
            }
        }
        #endregion

        #region + [4] PasswordInitialize : 비밀번호 초기화
        private async Task PasswordInitialize()
        {
            const string initialPassword = "xnd852456+";

            // Step 1: Salt 생성 (16바이트)
            byte[] salt = RandomNumberGenerator.GetBytes(16);

            // Step 2: 비밀번호 + Salt → SHA-256 해싱
            byte[] hashedPassword;
            using (var sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.Unicode.GetBytes(initialPassword);
                byte[] passwordWithSalt = new byte[passwordBytes.Length + salt.Length];
                Buffer.BlockCopy(passwordBytes, 0, passwordWithSalt, 0, passwordBytes.Length);
                Buffer.BlockCopy(salt, 0, passwordWithSalt, passwordBytes.Length, salt.Length);
                hashedPassword = sha256.ComputeHash(passwordWithSalt);
            }

            // Step 3: UpdatePasswordAsync 호출
            int rowsAffected = await userRepository.UpdatePasswordAsync(model.Uid, hashedPassword, salt);

            if (rowsAffected > 0)
            {
                resultText = "비밀번호 초기화 성공";
                StateHasChanged();
                UserManagePage();
            }
            else
            {
                resultText = "비밀번호 초기화 실패";
            }
        }
        #endregion

        #region + [2] MoveMainPage : 메인페이지 이동
        protected void MoveMainPage()
        {
            urlActions.MoveMainPage();
        }
        #endregion

        #region + [3] UserManagePage : 사용자관리 페이지 이동
        protected void UserManagePage()
        {
            urlActions.MoveUserManagePage();
        }
        #endregion
    }
}
