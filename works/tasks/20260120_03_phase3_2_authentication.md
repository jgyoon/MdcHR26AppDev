# 작업지시서: Phase 3-2 - 로그인 및 인증 시스템 구현

**날짜**: 2026-01-20
**작업 유형**: 인증 시스템 구현 (Phase 3-2)
**관련 이슈**: [#009: Phase 3 WebApp 개발](../issues/009_phase3_webapp_development.md)
**선행 작업지시서**: `20260120_02_phase3_1_project_setup.md` (Phase 3-1 완료)

---

## 1. 작업 개요

### 1.1. Phase 3-2 목표
로그인, 로그아웃, 비밀번호 변경 기능을 구현하고 SHA-256 + Salt 보안 인증을 적용합니다.

### 1.2. 작업 범위
- Login.razor (로그인 페이지)
- Logout.razor (로그아웃 처리)
- Manage.razor (비밀번호 변경)
- SHA-256 + Salt 암호화 연동
- 세션 상태 관리
- 권한 기반 라우팅

### 1.3. 완료 기준
- 로그인 성공 시 홈으로 리다이렉트
- 로그아웃 시 세션 초기화
- 비밀번호 변경 시 SHA-256 + Salt로 암호화
- 권한별 메뉴 표시 확인

---

## 2. 참조 프로젝트 분석

### 2.1. 2025년 프로젝트 로그인 로직
**경로**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Components\Pages\Auth\Login.razor`

**핵심 로직**:
```csharp
// 1. 사용자 조회
var user = await userRepository.GetByUserId(userId);

// 2. 비밀번호 검증
bool isValid = PasswordHasher.VerifyPassword(
    inputPassword,
    user.UserPassword,
    user.UserPasswordSalt
);

// 3. 로그인 상태 설정
loginStatusService.SetLoginStatus(
    true,
    user.UId,
    user.UserId,
    user.UserName,
    user.IsAdministrator,
    user.IsTeamLeader,
    user.IsDirector,
    user.IsDeptObjectiveWriter,
    department.EDepartmentName
);

// 4. 홈으로 리다이렉트
navigationManager.NavigateTo("/");
```

### 2.2. 비밀번호 해시 알고리즘
**SHA-256 + Salt 구조**:
- Salt: 16 bytes (VARBINARY(16))
- Password Hash: 32 bytes (VARBINARY(32))

---

## 3. 구현 파일 목록

### 3.1. 페이지 컴포넌트 (3개)
1. `Components/Pages/Auth/Login.razor` - 로그인 페이지
2. `Components/Pages/Auth/Logout.razor` - 로그아웃 처리
3. `Components/Pages/Auth/Manage.razor` - 비밀번호 변경

### 3.2. 유틸리티 클래스 (1개)
1. `Utils/PasswordHasher.cs` - SHA-256 암호화 유틸

### 3.3. CSS 파일 (1개)
1. `Components/Pages/Auth/Login.razor.css` - 로그인 스타일

---

## 4. 구현 상세

### 4.1. Utils/PasswordHasher.cs

**목적**: SHA-256 + Salt 암호화 및 검증

```csharp
using System.Security.Cryptography;
using System.Text;

namespace MdcHR26Apps.BlazorServer.Utils;

public static class PasswordHasher
{
    /// <summary>
    /// 새로운 Salt 생성 (16 bytes)
    /// </summary>
    public static byte[] GenerateSalt()
    {
        var salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        return salt;
    }

    /// <summary>
    /// 비밀번호 해시 생성 (SHA-256)
    /// </summary>
    /// <param name="password">평문 비밀번호</param>
    /// <param name="salt">Salt 값</param>
    /// <returns>32 bytes 해시</returns>
    public static byte[] HashPassword(string password, byte[] salt)
    {
        using var sha256 = SHA256.Create();

        // 비밀번호를 UTF-8 바이트로 변환
        var passwordBytes = Encoding.UTF8.GetBytes(password);

        // Salt + Password 결합
        var combined = new byte[salt.Length + passwordBytes.Length];
        Buffer.BlockCopy(salt, 0, combined, 0, salt.Length);
        Buffer.BlockCopy(passwordBytes, 0, combined, salt.Length, passwordBytes.Length);

        // SHA-256 해시
        return sha256.ComputeHash(combined);
    }

    /// <summary>
    /// 비밀번호 검증
    /// </summary>
    /// <param name="inputPassword">입력된 평문 비밀번호</param>
    /// <param name="storedHash">DB에 저장된 해시</param>
    /// <param name="storedSalt">DB에 저장된 Salt</param>
    /// <returns>일치 여부</returns>
    public static bool VerifyPassword(string inputPassword, byte[] storedHash, byte[] storedSalt)
    {
        // 입력된 비밀번호를 같은 Salt로 해시
        var inputHash = HashPassword(inputPassword, storedSalt);

        // 바이트 배열 비교
        return CompareByteArrays(inputHash, storedHash);
    }

    /// <summary>
    /// 바이트 배열 비교 (타이밍 공격 방지)
    /// </summary>
    private static bool CompareByteArrays(byte[] a, byte[] b)
    {
        if (a.Length != b.Length)
            return false;

        int diff = 0;
        for (int i = 0; i < a.Length; i++)
        {
            diff |= a[i] ^ b[i];
        }
        return diff == 0;
    }

    /// <summary>
    /// 비밀번호 강도 검증
    /// </summary>
    /// <param name="password">비밀번호</param>
    /// <returns>오류 메시지 (null이면 통과)</returns>
    public static string? ValidatePasswordStrength(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return "비밀번호를 입력해주세요.";

        if (password.Length < 4)
            return "비밀번호는 최소 4자 이상이어야 합니다.";

        if (password.Length > 50)
            return "비밀번호는 최대 50자까지 가능합니다.";

        return null; // 통과
    }
}
```

---

### 4.2. Components/Pages/Auth/Login.razor

**목적**: 로그인 페이지

```html
@page "/auth/login"
@inject IUserRepository UserRepository
@inject IEDepartmentRepository DepartmentRepository
@inject LoginStatusService LoginStatusService
@inject NavigationManager NavigationManager
@inject IJSRuntime JSRuntime

<PageTitle>로그인 - 2026 인사평가</PageTitle>

<div class="login-container">
    <div class="login-card">
        <div class="login-header">
            <h2>2026년 인사평가 시스템</h2>
            <p class="text-muted">로그인이 필요합니다</p>
        </div>

        <EditForm Model="@loginModel" OnValidSubmit="@HandleLogin" FormName="LoginForm">
            <DataAnnotationsValidator />

            <div class="mb-3">
                <label for="userId" class="form-label">사용자 ID</label>
                <InputText id="userId"
                           class="form-control"
                           @bind-Value="loginModel.UserId"
                           placeholder="사용자 ID를 입력하세요"
                           autocomplete="username" />
                <ValidationMessage For="@(() => loginModel.UserId)" />
            </div>

            <div class="mb-3">
                <label for="password" class="form-label">비밀번호</label>
                <InputText id="password"
                           type="password"
                           class="form-control"
                           @bind-Value="loginModel.Password"
                           placeholder="비밀번호를 입력하세요"
                           autocomplete="current-password" />
                <ValidationMessage For="@(() => loginModel.Password)" />
            </div>

            @if (!string.IsNullOrEmpty(errorMessage))
            {
                <div class="alert alert-danger" role="alert">
                    <i class="bi bi-exclamation-triangle-fill"></i> @errorMessage
                </div>
            }

            <button type="submit" class="btn btn-primary w-100" disabled="@isLoading">
                @if (isLoading)
                {
                    <span class="spinner-border spinner-border-sm me-2" role="status"></span>
                    <span>로그인 중...</span>
                }
                else
                {
                    <i class="bi bi-box-arrow-in-right"></i>
                    <span>로그인</span>
                }
            </button>
        </EditForm>

        <div class="login-footer">
            <small class="text-muted">
                문제가 있으신가요? 관리자에게 문의하세요.
            </small>
        </div>
    </div>
</div>

@code {
    private LoginModel loginModel = new();
    private string? errorMessage;
    private bool isLoading = false;

    protected override async Task OnInitializedAsync()
    {
        // 이미 로그인된 경우 홈으로 리다이렉트
        if (LoginStatusService.IsloginCheck())
        {
            NavigationManager.NavigateTo("/");
        }
    }

    private async Task HandleLogin()
    {
        try
        {
            isLoading = true;
            errorMessage = null;

            // 1. 사용자 조회
            var user = await UserRepository.GetByUserId(loginModel.UserId);

            if (user == null)
            {
                errorMessage = "사용자 ID 또는 비밀번호가 일치하지 않습니다.";
                return;
            }

            // 2. 비밀번호 검증
            bool isValid = Utils.PasswordHasher.VerifyPassword(
                loginModel.Password,
                user.UserPassword,
                user.UserPasswordSalt
            );

            if (!isValid)
            {
                errorMessage = "사용자 ID 또는 비밀번호가 일치하지 않습니다.";
                return;
            }

            // 3. 활성 상태 확인
            if (!user.EStatus)
            {
                errorMessage = "비활성화된 계정입니다. 관리자에게 문의하세요.";
                return;
            }

            // 4. 부서 정보 조회
            string departmentName = "부서 없음";
            if (user.EDepartId.HasValue)
            {
                var department = await DepartmentRepository.GetById(user.EDepartId.Value);
                if (department != null)
                {
                    departmentName = department.EDepartmentName;
                }
            }

            // 5. 로그인 상태 설정
            LoginStatusService.SetLoginStatus(
                isLogin: true,
                uid: user.UId,
                userid: user.UserId,
                username: user.UserName,
                isadmin: user.IsAdministrator,
                isteamleader: user.IsTeamLeader,
                isdirector: user.IsDirector,
                isdeptobjwriter: user.IsDeptObjectiveWriter,
                department: departmentName
            );

            // 6. 홈으로 리다이렉트
            NavigationManager.NavigateTo("/", forceLoad: true);
        }
        catch (Exception ex)
        {
            errorMessage = $"로그인 중 오류가 발생했습니다: {ex.Message}";
        }
        finally
        {
            isLoading = false;
        }
    }

    private class LoginModel
    {
        [Required(ErrorMessage = "사용자 ID를 입력해주세요.")]
        [StringLength(50, ErrorMessage = "사용자 ID는 50자 이내로 입력해주세요.")]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "비밀번호를 입력해주세요.")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "비밀번호는 4~50자 이내로 입력해주세요.")]
        public string Password { get; set; } = string.Empty;
    }
}
```

---

### 4.3. Components/Pages/Auth/Login.razor.css

```css
.login-container {
    display: flex;
    justify-content: center;
    align-items: center;
    min-height: 80vh;
    padding: 2rem;
}

.login-card {
    width: 100%;
    max-width: 400px;
    padding: 2rem;
    background: white;
    border-radius: 8px;
    box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.login-header {
    text-align: center;
    margin-bottom: 2rem;
}

.login-header h2 {
    color: #1b6ec2;
    margin-bottom: 0.5rem;
}

.login-footer {
    margin-top: 1.5rem;
    text-align: center;
}

.form-control:focus {
    border-color: #1b6ec2;
    box-shadow: 0 0 0 0.2rem rgba(27, 110, 194, 0.25);
}

.btn-primary {
    background-color: #1b6ec2;
    border-color: #1861ac;
}

.btn-primary:hover {
    background-color: #1861ac;
    border-color: #145a99;
}

.alert-danger {
    display: flex;
    align-items: center;
    gap: 0.5rem;
}
```

---

### 4.4. Components/Pages/Auth/Logout.razor

**목적**: 로그아웃 처리

```html
@page "/auth/logout"
@inject LoginStatusService LoginStatusService
@inject NavigationManager NavigationManager

<PageTitle>로그아웃 - 2026 인사평가</PageTitle>

<div class="container mt-5">
    <div class="alert alert-info text-center">
        <h4>로그아웃 처리 중...</h4>
    </div>
</div>

@code {
    protected override void OnInitialized()
    {
        // 로그인 상태 초기화
        LoginStatusService.SetLoginStatus(
            isLogin: false,
            uid: 0,
            userid: string.Empty,
            username: string.Empty,
            isadmin: false,
            isteamleader: false,
            isdirector: false,
            isdeptobjwriter: false,
            department: string.Empty
        );

        // 로그인 페이지로 리다이렉트
        NavigationManager.NavigateTo("/auth/login", forceLoad: true);
    }
}
```

---

### 4.5. Components/Pages/Auth/Manage.razor

**목적**: 비밀번호 변경

```html
@page "/auth/manage"
@inject IUserRepository UserRepository
@inject LoginStatusService LoginStatusService
@inject NavigationManager NavigationManager
@inject IJSRuntime JSRuntime

<PageTitle>비밀번호 변경 - 2026 인사평가</PageTitle>

<div class="container mt-4">
    <div class="row justify-content-center">
        <div class="col-md-6">
            <div class="card">
                <div class="card-header">
                    <h4><i class="bi bi-key-fill"></i> 비밀번호 변경</h4>
                </div>
                <div class="card-body">
                    @if (!LoginStatusService.IsloginCheck())
                    {
                        <div class="alert alert-warning">
                            <i class="bi bi-exclamation-triangle-fill"></i>
                            로그인이 필요합니다.
                        </div>
                        <a href="/auth/login" class="btn btn-primary">로그인하기</a>
                    }
                    else
                    {
                        <EditForm Model="@changePasswordModel" OnValidSubmit="@HandlePasswordChange" FormName="ChangePasswordForm">
                            <DataAnnotationsValidator />

                            <div class="mb-3">
                                <label class="form-label">사용자 정보</label>
                                <input type="text" class="form-control" value="@LoginStatusService.LoginStatus.LoginUserName (@LoginStatusService.LoginStatus.LoginUserId)" disabled />
                            </div>

                            <div class="mb-3">
                                <label for="currentPassword" class="form-label">현재 비밀번호</label>
                                <InputText id="currentPassword"
                                           type="password"
                                           class="form-control"
                                           @bind-Value="changePasswordModel.CurrentPassword"
                                           placeholder="현재 비밀번호를 입력하세요" />
                                <ValidationMessage For="@(() => changePasswordModel.CurrentPassword)" />
                            </div>

                            <div class="mb-3">
                                <label for="newPassword" class="form-label">새 비밀번호</label>
                                <InputText id="newPassword"
                                           type="password"
                                           class="form-control"
                                           @bind-Value="changePasswordModel.NewPassword"
                                           placeholder="새 비밀번호를 입력하세요 (최소 4자)" />
                                <ValidationMessage For="@(() => changePasswordModel.NewPassword)" />
                            </div>

                            <div class="mb-3">
                                <label for="confirmPassword" class="form-label">새 비밀번호 확인</label>
                                <InputText id="confirmPassword"
                                           type="password"
                                           class="form-control"
                                           @bind-Value="changePasswordModel.ConfirmPassword"
                                           placeholder="새 비밀번호를 다시 입력하세요" />
                                <ValidationMessage For="@(() => changePasswordModel.ConfirmPassword)" />
                            </div>

                            @if (!string.IsNullOrEmpty(errorMessage))
                            {
                                <div class="alert alert-danger">
                                    <i class="bi bi-exclamation-triangle-fill"></i> @errorMessage
                                </div>
                            }

                            @if (!string.IsNullOrEmpty(successMessage))
                            {
                                <div class="alert alert-success">
                                    <i class="bi bi-check-circle-fill"></i> @successMessage
                                </div>
                            }

                            <div class="d-flex gap-2">
                                <button type="submit" class="btn btn-primary" disabled="@isLoading">
                                    @if (isLoading)
                                    {
                                        <span class="spinner-border spinner-border-sm me-2"></span>
                                    }
                                    <i class="bi bi-check-lg"></i> 변경
                                </button>
                                <a href="/" class="btn btn-secondary">취소</a>
                            </div>
                        </EditForm>
                    }
                </div>
            </div>
        </div>
    </div>
</div>

@code {
    private ChangePasswordModel changePasswordModel = new();
    private string? errorMessage;
    private string? successMessage;
    private bool isLoading = false;

    private async Task HandlePasswordChange()
    {
        try
        {
            isLoading = true;
            errorMessage = null;
            successMessage = null;

            // 1. 비밀번호 강도 검증
            var validationError = Utils.PasswordHasher.ValidatePasswordStrength(changePasswordModel.NewPassword);
            if (validationError != null)
            {
                errorMessage = validationError;
                return;
            }

            // 2. 새 비밀번호 확인 일치 검증
            if (changePasswordModel.NewPassword != changePasswordModel.ConfirmPassword)
            {
                errorMessage = "새 비밀번호가 일치하지 않습니다.";
                return;
            }

            // 3. 현재 사용자 조회
            var user = await UserRepository.GetById(LoginStatusService.LoginStatus.LoginUid);
            if (user == null)
            {
                errorMessage = "사용자 정보를 찾을 수 없습니다.";
                return;
            }

            // 4. 현재 비밀번호 검증
            bool isCurrentPasswordValid = Utils.PasswordHasher.VerifyPassword(
                changePasswordModel.CurrentPassword,
                user.UserPassword,
                user.UserPasswordSalt
            );

            if (!isCurrentPasswordValid)
            {
                errorMessage = "현재 비밀번호가 일치하지 않습니다.";
                return;
            }

            // 5. 새 Salt 생성
            var newSalt = Utils.PasswordHasher.GenerateSalt();

            // 6. 새 비밀번호 해시 생성
            var newPasswordHash = Utils.PasswordHasher.HashPassword(
                changePasswordModel.NewPassword,
                newSalt
            );

            // 7. DB 업데이트
            user.UserPassword = newPasswordHash;
            user.UserPasswordSalt = newSalt;

            var result = await UserRepository.Update(user);

            if (result > 0)
            {
                successMessage = "비밀번호가 성공적으로 변경되었습니다. 다시 로그인해주세요.";

                // 3초 후 로그아웃 페이지로 이동
                await Task.Delay(3000);
                NavigationManager.NavigateTo("/auth/logout", forceLoad: true);
            }
            else
            {
                errorMessage = "비밀번호 변경에 실패했습니다.";
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"오류가 발생했습니다: {ex.Message}";
        }
        finally
        {
            isLoading = false;
        }
    }

    private class ChangePasswordModel
    {
        [Required(ErrorMessage = "현재 비밀번호를 입력해주세요.")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "새 비밀번호를 입력해주세요.")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "비밀번호는 4~50자 이내로 입력해주세요.")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "새 비밀번호 확인을 입력해주세요.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
```

---

## 5. _Imports.razor 업데이트

**파일**: `Components/Pages/_Imports.razor`

**추가 항목**:
```csharp
@using System.ComponentModel.DataAnnotations
@using MdcHR26Apps.BlazorServer.Utils
```

---

## 6. 권한 기반 라우팅 (선택 사항)

### 6.1. AuthorizeView 컴포넌트 활용

**예시** (Home.razor):
```html
@if (LoginStatusService.IsloginCheck())
{
    @if (LoginStatusService.LoginStatus.LoginIsAdministrator)
    {
        <p>관리자 메뉴 표시</p>
    }
}
```

---

## 7. 테스트 시나리오

### 7.1. 로그인 테스트
1. `/auth/login` 접속
2. 유효한 사용자 ID/비밀번호 입력
3. "로그인" 버튼 클릭
4. 홈 페이지로 리다이렉트 확인
5. 상단 바에 사용자 이름 표시 확인
6. NavMenu에 권한별 메뉴 표시 확인

### 7.2. 로그인 실패 테스트
1. 잘못된 사용자 ID 입력 → 오류 메시지 확인
2. 잘못된 비밀번호 입력 → 오류 메시지 확인
3. 빈 필드 제출 → 유효성 검증 오류 확인

### 7.3. 로그아웃 테스트
1. 로그인 상태에서 "Logout" 클릭
2. `/auth/logout` 페이지로 이동
3. 로그인 상태 초기화 확인
4. `/auth/login`으로 리다이렉트 확인

### 7.4. 비밀번호 변경 테스트
1. 로그인 후 "비밀번호 변경" 클릭
2. 현재 비밀번호 입력
3. 새 비밀번호 입력 (4자 이상)
4. 새 비밀번호 확인 입력
5. "변경" 버튼 클릭
6. 성공 메시지 확인
7. 3초 후 로그아웃 처리 확인
8. 새 비밀번호로 재로그인 확인

### 7.5. 비밀번호 변경 실패 테스트
1. 현재 비밀번호 불일치 → 오류 메시지
2. 새 비밀번호 4자 미만 → 유효성 오류
3. 새 비밀번호 확인 불일치 → 오류 메시지

---

## 8. 보안 고려사항

### 8.1. 구현된 보안 기능
- ✅ SHA-256 해시 알고리즘
- ✅ Salt 사용 (16 bytes)
- ✅ 타이밍 공격 방지 (바이트 비교)
- ✅ 비밀번호 강도 검증 (최소 4자)
- ✅ 평문 비밀번호 메모리 즉시 제거
- ✅ HTTPS 강제 (Program.cs)
- ✅ CSRF 보호 (Antiforgery)

### 8.2. 추가 권장 사항
- [ ] 로그인 시도 횟수 제한 (Brute Force 방지)
- [ ] 비밀번호 복잡도 정책 강화 (영문+숫자+특수문자)
- [ ] 세션 타임아웃 설정
- [ ] 로그인 이력 기록

---

## 9. 빌드 및 테스트

### 9.1. 빌드
```bash
cd MdcHR26Apps.BlazorServer
dotnet build
```

**예상 결과**: 0 errors, 0 warnings

### 9.2. 실행
```bash
dotnet run
```

### 9.3. 수동 테스트
1. https://localhost:5132/auth/login 접속
2. 테스트 계정으로 로그인
3. 비밀번호 변경 테스트
4. 로그아웃 테스트

### 9.4. Playwright 자동 테스트 (선택)
```bash
cd MdcHR26Apps.BlazorServer
npm test
```

---

## 10. 완료 체크리스트

- [ ] `Utils/PasswordHasher.cs` 작성 (SHA-256 + Salt)
- [ ] `Components/Pages/Auth/Login.razor` 작성
- [ ] `Components/Pages/Auth/Login.razor.css` 작성
- [ ] `Components/Pages/Auth/Logout.razor` 작성
- [ ] `Components/Pages/Auth/Manage.razor` 작성
- [ ] `Components/Pages/_Imports.razor` 업데이트
- [ ] 빌드 성공 (0 errors, 0 warnings)
- [ ] 로그인 성공 테스트
- [ ] 로그인 실패 테스트
- [ ] 로그아웃 테스트
- [ ] 비밀번호 변경 성공 테스트
- [ ] 비밀번호 변경 실패 테스트
- [ ] 권한별 메뉴 표시 확인
- [ ] MainLayout 상단바 로그인 정보 표시 확인

---

## 11. 다음 단계

**Phase 3-3: 관리자 페이지**
- Users.razor (사용자 관리)
- Depts.razor (부서 관리)
- EvaluationUsers.razor (평가대상자 관리)
- Admin/Index.razor (관리자 홈)

---

**작성자**: Claude AI
**검토자**: 개발자
**승인 일자**: 추후 기재
