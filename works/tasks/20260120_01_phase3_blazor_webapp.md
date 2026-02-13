# 작업지시서: Phase 3 - Blazor Server WebApp 개발

**날짜**: 2026-01-20
**작업 유형**: 신규 프로젝트 생성 (Phase 3)
**관련 이슈**: [#009: Phase 3 WebApp 개발](../issues/009_phase3_webapp_development.md)
**관련 작업지시서**:
- `20260116_01_phase2_model_development.md` (Phase 2 전체 계획)
- `20251216_02_phase1_database_design.md` (Phase 1 DB 설계)

**수정 이력**:
- 2026-01-20 (v1): 초안 작성 - 2025년 BlazorApp 및 도서관리 Server 분석 기반

---

## 1. 작업 개요

### 1.1. Phase 3 목표
2026년 인사평가 시스템의 사용자 인터페이스(Blazor Server) 프로젝트를 생성합니다.

### 1.2. 작업 범위
1. **Blazor Server 프로젝트 생성** (.NET 10.0 기반)
2. **프로젝트 구조 설계** (2025년 기반 + 도서관리 최신 기술)
3. **기본 설정 및 NuGet 패키지 설치**
4. **DI 컨테이너 설정** (Program.cs)
5. **환경 설정 파일** (appsettings.json)
6. **프로젝트 참조** (MdcHR26Apps.Models)

### 1.3. 기술 스택

| 항목 | 기술 | 버전 | 비고 |
|------|------|------|------|
| **프레임워크** | .NET | 10.0 | 최신 LTS |
| **C#** | C# 13 | - | 최신 언어 기능 |
| **Blazor** | Blazor Server | - | SignalR 기반 실시간 통신 |
| **ORM** | Dapper + EF Core | 2.1.66 / 10.0.0 | Phase 2 연동 |
| **UI** | Bootstrap | 5.x | 반응형 디자인 |
| **엑셀** | ClosedXML | 최신 | 평가 결과 내보내기 |
| **인증** | Custom Auth | - | SHA-256 + Salt |

---

## 2. 참조 프로젝트 분석 요약

### 2.1. 2025년 인사평가 (MdcHR25Apps.BlazorApp)

**경로**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp`

**핵심 참조 사항**:
- ✅ **Blazor Server** 아키텍처
- ✅ **3단계 평가 프로세스** (본인 → 부서장 → 임원)
- ✅ **역할 기반 메뉴** (관리자, 부서장, 임원)
- ✅ **상태 관리 서비스** (LoginStatusService, AppState)
- ✅ **모달 컴포넌트 패턴**
- ✅ **엑셀 내보내기** (ClosedXML)
- ✅ **검색/필터 컴포넌트**

**프로젝트 구조**:
```
Pages/
├── Auth/ (Login, Logout, Manage)
├── Admin/ (사용자, 부서, 평가관리)
├── 1st_HR_Report/ (본인평가)
├── 2nd_HR_Report/ (부서장평가)
├── 3rd_HR_Report/ (임원평가)
├── Agreement/ (직무평가)
├── DeptObjective/ (목표관리)
└── TotalReport/ (결과리포트)

Components/
├── CommonComponents/ (Loading, Search, etc.)
├── FormComponents/ (Form 입력)
├── Modal/ (모달 대화상자)
└── ViewPage/ (페이지별 컴포넌트)

Data/
├── LoginStatusService.cs
├── AppState.cs
├── ExcelManage.cs
└── UrlControls.cs
```

### 2.2. 도서관리 (MdcLibrary.Server)

**경로**: `C:\Codes\36_MdcLibrary\MdcLibrary\MdcLibrary.Server`

**최신 기술 참조**:
- ✅ **.NET 8.0 → 10.0 업그레이드 적용**
- ✅ **InteractiveServer 렌더 모드** (Blazor 최신 패턴)
- ✅ **Enhanced Navigation** (더 빠른 페이지 전환)
- ✅ **반응형 UI** (Bootstrap 기반 모바일/데스크톱)
- ✅ **구조화된 로깅** (ILogger 활용)
- ✅ **에러 핸들링** (Global Error UI)
- ✅ **Rate Limiting** (보안 강화)

**개선 포인트**:
```csharp
// 1. Program.cs - Blazor 세부 설정
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options => { /* ... */ })
    .AddHubOptions(options => { /* SignalR 최적화 */ });

// 2. App.razor - 렌더 모드
<Routes @rendermode="InteractiveServer" />

// 3. 반응형 테이블 (모바일/데스크톱 이중 UI)
<div class="d-none d-md-block"><!-- 데스크톱 --></div>
<div class="d-block d-md-none"><!-- 모바일 --></div>
```

### 2.3. Phase 2 Model (MdcHR26Apps.Models)

**완료 내역**:
- ✅ 55개 파일 (17 Entity, 17 Interface, 17 Repository, 4 Common)
- ✅ 147개 메서드 (CRUD + 비즈니스 로직)
- ✅ SHA-256 + Salt 로그인
- ✅ Dapper + EF Core 하이브리드
- ✅ Primary Constructors, Raw String Literals (C# 13)

**DI 등록 방식**:
```csharp
// MdcHR26AppsAddExtensions.cs
public static void AddDependencyInjectionContainerForMdcHR26AppModels(
    this IServiceCollection services, string connectionString)
{
    // DbContext 등록
    services.AddDbContext<MdcHR26AppsAddDbContext>(...);

    // Repository 등록 (17개)
    services.AddSingleton<IUserRepository>(...);
    services.AddSingleton<IProcessRepository>(...);
    // ...
}
```

### 2.4. Phase 1 Database

**DB 구조**:
- ✅ 12개 테이블 (UserDb, ProcessDb, ReportDb, TotalReportDb 등)
- ✅ 5개 뷰 (v_MemberListDB, v_ProcessTRListDB 등)
- ✅ 외래키 관계 정립 (데이터 무결성)
- ✅ 초기 데이터 (Seed Data)

---

## 3. 프로젝트 생성 및 구조 설계

### 3.1. 프로젝트 폴더 구조

```
MdcHR26Apps.BlazorServer/
├── Components/
│   ├── Layout/
│   │   ├── MainLayout.razor
│   │   ├── MainLayout.razor.css
│   │   └── NavMenu.razor
│   ├── Pages/
│   │   ├── Auth/                      # 인증
│   │   │   ├── Login.razor
│   │   │   ├── Logout.razor
│   │   │   └── Manage.razor
│   │   ├── Admin/                      # 관리자 페이지
│   │   │   ├── Users/                 # 사용자 관리
│   │   │   ├── Depts/                 # 부서 관리
│   │   │   ├── EvaluationUsers/       # 평가대상자 관리
│   │   │   └── Index.razor            # 관리자 메인
│   │   ├── Agreement/                  # 직무평가 협의
│   │   │   ├── User/                  # 사용자 직무작성
│   │   │   └── TeamLeader/            # 팀장 합의
│   │   ├── SubAgreement/              # 세부직무평가
│   │   ├── 1st_HR_Report/             # 본인평가 (1차)
│   │   ├── 2nd_HR_Report/             # 부서장평가 (2차)
│   │   ├── 3rd_HR_Report/             # 임원평가 (3차)
│   │   ├── DeptObjective/             # 부서 목표관리
│   │   ├── TotalReport/               # 최종 평가결과
│   │   ├── Home.razor                 # 홈 페이지
│   │   ├── Error.razor                # 에러 페이지
│   │   └── _Imports.razor             # 전역 using
│   └── Components/                     # 재사용 컴포넌트
│       ├── Common/
│       │   ├── LoadingIndicator.razor
│       │   ├── DisplayResultText.razor
│       │   └── SearchbarComponent.razor
│       ├── Forms/
│       │   ├── FormSelectList.razor
│       │   └── FormTaskItem.razor
│       ├── Modal/
│       │   ├── DeleteModal.razor
│       │   └── ConfirmModal.razor
│       └── Tables/
│           ├── UserListTable.razor
│           └── ProcessListTable.razor
│
├── Data/                               # 상태 관리 및 서비스
│   ├── LoginStatusService.cs          # 로그인 상태 관리
│   ├── LoginStatus.cs                 # 로그인 상태 모델
│   ├── AppStateService.cs             # 전역 앱 상태
│   ├── UrlActions.cs                  # URL 네비게이션
│   └── ExcelManage.cs                 # 엑셀 처리
│
├── Utils/                              # 유틸리티
│   ├── UserUtils.cs                   # 사용자 관련 유틸
│   └── ScoreUtils.cs                  # 점수 계산
│
├── Models/                             # 로컬 모델
│   ├── ChangePasswordModel.cs
│   └── TotalScoreModel.cs
│
├── wwwroot/                            # 정적 파일
│   ├── css/
│   │   ├── app.css
│   │   ├── site.css
│   │   ├── bootstrap/
│   │   └── LoadingSpinner.css
│   ├── js/
│   │   └── site.js
│   └── favicon.png
│
├── Program.cs                          # 애플리케이션 시작점
├── App.razor                           # Blazor 라우팅
├── appsettings.json                   # 환경 설정
├── appsettings.Development.json       # 개발 환경
├── Dockerfile                          # Docker 이미지
└── MdcHR26Apps.BlazorServer.csproj    # 프로젝트 파일
```

### 3.2. 프로젝트 생성 방법

**Option 1: Visual Studio 2022 (권장)**
```
1. 새 프로젝트 만들기
2. "Blazor Web App" 템플릿 선택
3. 프로젝트 이름: MdcHR26Apps.BlazorServer
4. 위치: C:\Codes\00_Develop_Cursor\10_MdcHR26Apps\
5. 프레임워크: .NET 10.0
6. 인증: 없음 (커스텀 인증 구현)
7. Interactivity type: Server
8. Include sample pages: 체크 해제
```

**Option 2: .NET CLI**
```bash
cd C:\Codes\00_Develop_Cursor\10_MdcHR26Apps
dotnet new blazor -o MdcHR26Apps.BlazorServer -f net10.0 --interactivity Server --no-https false
```

### 3.3. 프로젝트 참조 추가

```xml
<ItemGroup>
    <ProjectReference Include="..\MdcHR26Apps.Models\MdcHR26Apps.Models.csproj" />
</ItemGroup>
```

---

## 4. NuGet 패키지 설치

### 4.1. 필수 패키지

```xml
<ItemGroup>
    <!-- Blazor 필수 패키지 (이미 포함) -->
    <!-- Microsoft.AspNetCore.Components.Web (자동 포함) -->

    <!-- 엑셀 처리 -->
    <PackageReference Include="ClosedXML" Version="0.104.2" />

    <!-- Docker 지원 (선택) -->
    <PackageReference Include="Microsoft.VisualStudio.Azure.Containers.Tools.Targets" Version="1.21.0" />
</ItemGroup>
```

### 4.2. 설치 명령어 (CLI)

```bash
cd MdcHR26Apps.BlazorServer
dotnet add package ClosedXML --version 0.104.2
dotnet add package Microsoft.VisualStudio.Azure.Containers.Tools.Targets --version 1.21.0
```

---

## 5. Program.cs 설정

### 5.1. 전체 코드 (주석 포함)

```csharp
using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// 1. Blazor Server 서비스 등록
// ========================================
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        options.DetailedErrors = false;  // 운영 환경에서는 false
        options.DisconnectedCircuitMaxRetained = 100;
        options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(3);
        options.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(1);
        options.MaxBufferedUnacknowledgedRenderBatches = 10;
    })
    .AddHubOptions(options =>
    {
        options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
        options.EnableDetailedErrors = false;
        options.HandshakeTimeout = TimeSpan.FromSeconds(15);
        options.KeepAliveInterval = TimeSpan.FromSeconds(15);
        options.MaximumParallelInvocationsPerClient = 1;
        options.MaximumReceiveMessageSize = 32 * 1024;  // 32KB
        options.StreamBufferCapacity = 10;
    });

// ========================================
// 2. 상태 관리 서비스 등록
// ========================================
builder.Services.AddScoped<LoginStatusService>();
builder.Services.AddScoped<AppStateService>();
builder.Services.AddTransient<UrlActions>();
builder.Services.AddTransient<ExcelManage>();

// ========================================
// 3. Model 계층 DI 등록 (Phase 2 연동)
// ========================================
var isProduction = builder.Configuration.GetValue<int>("AppSettings:IsProduction");
string connectionString;

if (isProduction == 0)
{
    // 개발 환경: LocalDB
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}
else
{
    // 운영 환경: Docker Container DB
    connectionString = builder.Configuration.GetConnectionString("MdcHR26AppsContainerConnection")
        ?? throw new InvalidOperationException("Connection string 'MdcHR26AppsContainerConnection' not found.");
}

builder.Services.AddDependencyInjectionContainerForMdcHR26AppModels(connectionString);

// ========================================
// 4. 보안 및 추가 서비스
// ========================================
builder.Services.AddAntiforgery();  // CSRF 방어

var app = builder.Build();

// ========================================
// 5. 미들웨어 설정
// ========================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
```

### 5.2. 환경별 분기 처리

| 환경 | IsProduction | 연결 문자열 | 비고 |
|------|-------------|------------|------|
| **개발** | 0 | DefaultConnection | LocalDB |
| **운영** | 1 | MdcHR26AppsContainerConnection | Docker |

---

## 6. appsettings.json 설정

### 6.1. appsettings.json (운영 환경)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=MdcHR26Apps;Integrated Security=true;TrustServerCertificate=true;",
    "MdcHR26AppsContainerConnection": "Data Source=mssql_server;Database=MdcHR2026;User ID=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=true;Connect Timeout=30;"
  },
  "AppSettings": {
    "IsProduction": 1,
    "IsOpen": 1
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### 6.2. appsettings.Development.json (개발 환경)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=MdcHR26Apps;Integrated Security=true;TrustServerCertificate=true;"
  },
  "AppSettings": {
    "IsProduction": 0,
    "IsOpen": 1
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  }
}
```

### 6.3. AppSettings 모델

**위치**: `Models/AppSettings.cs`

```csharp
namespace MdcHR26Apps.BlazorServer.Models;

public class AppSettings
{
    public int IsProduction { get; set; }
    public int IsOpen { get; set; }
}
```

---

## 7. App.razor 설정

### 7.1. App.razor

```html
<!DOCTYPE html>
<html lang="ko">

<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <base href="/" />
    <link rel="stylesheet" href="css/bootstrap/bootstrap.min.css" />
    <link rel="stylesheet" href="css/app.css" />
    <link rel="stylesheet" href="css/LoadingSpinner.css" />
    <link rel="icon" type="image/png" href="favicon.png" />
    <HeadOutlet @rendermode="InteractiveServer" />
</head>

<body>
    <Routes @rendermode="InteractiveServer" />

    <!-- Error UI -->
    <div id="blazor-error-ui">
        오류가 발생했습니다.
        <a href="" class="reload">새로고침</a>
        <a class="dismiss">🗙</a>
    </div>

    <script src="_framework/blazor.web.js"></script>
</body>

</html>
```

### 7.2. Routes.razor

**위치**: `Components/Routes.razor`

```html
<Router AppAssembly="typeof(Program).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="routeData" DefaultLayout="typeof(Layout.MainLayout)" />
        <FocusOnNavigate RouteData="routeData" Selector="h1" />
    </Found>
    <NotFound>
        <PageTitle>찾을 수 없음</PageTitle>
        <LayoutView Layout="typeof(Layout.MainLayout)">
            <p role="alert">페이지를 찾을 수 없습니다.</p>
        </LayoutView>
    </NotFound>
</Router>
```

---

## 8. 상태 관리 서비스

### 8.1. LoginStatus.cs (로그인 상태 모델)

**위치**: `Data/LoginStatus.cs`

```csharp
namespace MdcHR26Apps.BlazorServer.Data;

public class LoginStatus
{
    public bool IsLogin { get; set; }
    public Int64 LoginUid { get; set; }
    public string? LoginUserId { get; set; }
    public string? LoginUserName { get; set; }
    public bool LoginIsAdministrator { get; set; }
    public bool LoginIsTeamLeader { get; set; }
    public bool LoginIsDirector { get; set; }
    public bool LoginIsDeptObjectiveWriter { get; set; }
    public string? LoginUserEDepartment { get; set; }
}
```

### 8.2. LoginStatusService.cs (로그인 관리)

**위치**: `Data/LoginStatusService.cs`

```csharp
namespace MdcHR26Apps.BlazorServer.Data;

public class LoginStatusService
{
    public LoginStatus LoginStatus { get; set; } = new();

    public event Action? OnChange;

    // 로그인 상태 설정
    public LoginStatus SetLoginStatus(
        bool isLogin,
        Int64 uid,
        string userid,
        string username,
        bool isadmin,
        bool isteamleader,
        bool isdirector,
        bool isdeptobjwriter,
        string department)
    {
        LoginStatus.IsLogin = isLogin;
        LoginStatus.LoginUid = uid;
        LoginStatus.LoginUserId = userid;
        LoginStatus.LoginUserName = username;
        LoginStatus.LoginIsAdministrator = isadmin;
        LoginStatus.LoginIsTeamLeader = isteamleader;
        LoginStatus.LoginIsDirector = isdirector;
        LoginStatus.LoginIsDeptObjectiveWriter = isdeptobjwriter;
        LoginStatus.LoginUserEDepartment = department;

        OnChange?.Invoke();
        return LoginStatus;
    }

    // 로그인 확인
    public bool IsloginCheck() => LoginStatus.IsLogin;

    // 관리자 확인
    public bool IsloginAndIsAdminCheck() =>
        LoginStatus.IsLogin && LoginStatus.LoginIsAdministrator;

    // 부서장 확인
    public bool IsloginAndIsTeamLeaderCheck() =>
        LoginStatus.IsLogin && LoginStatus.LoginIsTeamLeader;

    // 임원 확인
    public bool IsloginAndIsDirectorCheck() =>
        LoginStatus.IsLogin && LoginStatus.LoginIsDirector;
}
```

### 8.3. AppStateService.cs (전역 상태)

**위치**: `Data/AppStateService.cs`

```csharp
namespace MdcHR26Apps.BlazorServer.Data;

public class AppStateService
{
    private readonly IConfiguration _configuration;

    public AppStateService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public event Action? OnChange;

    // 평가 개시 여부 확인
    public bool GetIsOpen()
    {
        var isOpen = _configuration.GetValue<int>("AppSettings:IsOpen");
        return isOpen == 1;
    }

    // 텍스트 트림 (UI 표시용)
    public string TruncateText(string? text, int maxLength)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        return text.Length <= maxLength ? text : $"{text.Substring(0, maxLength)}...";
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
```

### 8.4. UrlActions.cs (네비게이션)

**위치**: `Data/UrlActions.cs`

```csharp
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Data;

public class UrlActions
{
    private readonly NavigationManager _navigationManager;

    public UrlActions(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
    }

    public void MoveMainPage() => _navigationManager.NavigateTo("/");
    public void MoveLoginPage() => _navigationManager.NavigateTo("/auth/login");
    public void MoveLogoutPage() => _navigationManager.NavigateTo("/auth/logout");
    public void MoveAdminPage() => _navigationManager.NavigateTo("/admin");
}
```

---

## 9. 레이아웃 구성

### 9.1. MainLayout.razor

**위치**: `Components/Layout/MainLayout.razor`

```html
@inherits LayoutComponentBase
@inject LoginStatusService loginStatusService
@implements IDisposable

<div class="page">
    <div class="sidebar">
        <NavMenu />
    </div>

    <main>
        <div class="top-row px-4">
            @if (loginStatusService.IsloginCheck())
            {
                <span>
                    @loginStatusService.LoginStatus.LoginUserName (@loginStatusService.LoginStatus.LoginUserId)
                </span>
                <a href="/auth/logout">Logout</a>
                <a href="/auth/manage">비밀번호 변경</a>
            }
            else
            {
                <a href="/auth/login">Login</a>
            }
        </div>

        <article class="content px-4">
            @Body
        </article>
    </main>
</div>

<div id="blazor-error-ui">
    오류가 발생했습니다.
    <a href="" class="reload">새로고침</a>
    <a class="dismiss">🗙</a>
</div>

@code {
    protected override void OnInitialized()
    {
        loginStatusService.OnChange += StateHasChanged;
    }

    public void Dispose()
    {
        loginStatusService.OnChange -= StateHasChanged;
    }
}
```

### 9.2. MainLayout.razor.css

**위치**: `Components/Layout/MainLayout.razor.css`

```css
.page {
    position: relative;
    display: flex;
    flex-direction: column;
}

main {
    flex: 1;
}

.sidebar {
    background-image: linear-gradient(180deg, rgb(5, 39, 103) 0%, #3a0647 70%);
}

.top-row {
    background-color: #f7f7f7;
    border-bottom: 1px solid #d6d5d5;
    justify-content: flex-end;
    height: 3.5rem;
    display: flex;
    align-items: center;
}

.top-row a {
    margin-left: 1.5rem;
}

@media (min-width: 641px) {
    .page {
        flex-direction: row;
    }

    .sidebar {
        width: 250px;
        height: 100vh;
        position: sticky;
        top: 0;
    }

    .top-row {
        position: sticky;
        top: 0;
        z-index: 1;
    }

    .content {
        padding-top: 1.1rem;
    }
}
```

### 9.3. NavMenu.razor (기본 구조)

**위치**: `Components/Layout/NavMenu.razor`

```html
@inject LoginStatusService loginStatusService
@inject AppStateService appStateService
@implements IDisposable

<div class="top-row ps-3 navbar navbar-dark">
    <div class="container-fluid">
        <a class="navbar-brand" href="">2026 인사평가</a>
    </div>
</div>

<input type="checkbox" title="Navigation menu" class="navbar-toggler" />

<div class="nav-scrollable" onclick="document.querySelector('.navbar-toggler').click()">
    <nav class="flex-column">
        <!-- Home -->
        <div class="nav-item px-3">
            <NavLink class="nav-link" href="" Match="NavLinkMatch.All">
                <span class="bi bi-house-door-fill-nav-menu" aria-hidden="true"></span> Home
            </NavLink>
        </div>

        <!-- 로그인 (비로그인 시) -->
        @if (!loginStatusService.IsloginCheck())
        {
            <div class="nav-item px-3">
                <NavLink class="nav-link" href="auth/login">
                    <span class="bi bi-box-arrow-in-right" aria-hidden="true"></span> Login
                </NavLink>
            </div>
        }

        <!-- 로그인 사용자 메뉴 -->
        @if (loginStatusService.IsloginCheck())
        {
            <!-- 부서 목표 -->
            <div class="nav-item px-3">
                <NavLink class="nav-link" href="deptobjective">
                    <span class="bi bi-clipboard-check" aria-hidden="true"></span> 부서 목표
                </NavLink>
            </div>

            <!-- 관리자 메뉴 -->
            @if (loginStatusService.IsloginAndIsAdminCheck())
            {
                <div class="nav-item px-3">
                    <NavLink class="nav-link" href="admin">
                        <span class="bi bi-gear-fill" aria-hidden="true"></span> 관리자
                    </NavLink>
                </div>
            }

            <!-- 로그아웃 -->
            <div class="nav-item px-3">
                <NavLink class="nav-link" href="auth/logout">
                    <span class="bi bi-box-arrow-right" aria-hidden="true"></span> Logout
                </NavLink>
            </div>
        }
    </nav>
</div>

@code {
    protected override void OnInitialized()
    {
        appStateService.OnChange += StateHasChanged;
    }

    public void Dispose()
    {
        appStateService.OnChange -= StateHasChanged;
    }
}
```

---

## 10. 기본 페이지 구성

### 10.1. Home.razor (홈 페이지)

**위치**: `Components/Pages/Home.razor`

```html
@page "/"
@inject LoginStatusService loginStatusService

<PageTitle>2026 인사평가</PageTitle>

<h1>2026년 인사평가 시스템</h1>

@if (loginStatusService.IsloginCheck())
{
    <p>환영합니다, <strong>@loginStatusService.LoginStatus.LoginUserName</strong>님!</p>
    <p>부서: @loginStatusService.LoginStatus.LoginUserEDepartment</p>
}
else
{
    <p>로그인이 필요합니다.</p>
    <a href="/auth/login" class="btn btn-primary">로그인하기</a>
}
```

### 10.2. Error.razor (에러 페이지)

**위치**: `Components/Pages/Error.razor`

```html
@page "/Error"

<PageTitle>오류</PageTitle>

<h1 class="text-danger">오류 발생</h1>
<h2 class="text-danger">요청 처리 중 오류가 발생했습니다.</h2>

@if (ShowRequestId)
{
    <p>
        <strong>Request ID:</strong> <code>@RequestId</code>
    </p>
}

<h3>문제 해결 방법:</h3>
<ul>
    <li>페이지를 새로고침해 보세요</li>
    <li>로그아웃 후 다시 로그인해 보세요</li>
    <li>문제가 지속되면 관리자에게 문의하세요</li>
</ul>

@code {
    [CascadingParameter]
    private HttpContext? HttpContext { get; set; }

    private string? RequestId { get; set; }
    private bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    protected override void OnInitialized() =>
        RequestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier;
}
```

---

## 11. wwwroot 정적 파일

### 11.1. css/LoadingSpinner.css

**위치**: `wwwroot/css/LoadingSpinner.css`

```css
.spinner-container {
    display: flex;
    justify-content: center;
    align-items: center;
    height: 240px;
}

.spinner {
    width: 50px;
    height: 50px;
    border-radius: 50%;
    border: 5px solid #e0e0e0;
    border-bottom: 5px solid #fe9616;
    animation: spin 1s linear infinite;
}

@keyframes spin {
    0% { transform: rotate(0deg); }
    100% { transform: rotate(360deg); }
}
```

### 11.2. css/app.css (기본 스타일)

**위치**: `wwwroot/css/app.css`

```css
html, body {
    font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif;
}

h1:focus {
    outline: none;
}

a, .btn-link {
    color: #0071c1;
}

.btn-primary {
    color: #fff;
    background-color: #1b6ec2;
    border-color: #1861ac;
}

.btn:focus, .btn:active:focus, .btn-link.nav-link:focus, .form-control:focus, .form-check-input:focus {
    box-shadow: 0 0 0 0.1rem white, 0 0 0 0.25rem #258cfb;
}

.content {
    padding-top: 1.1rem;
}

.valid.modified:not([type=checkbox]) {
    outline: 1px solid #26b050;
}

.invalid {
    outline: 1px solid red;
}

.validation-message {
    color: red;
}

#blazor-error-ui {
    background: lightyellow;
    bottom: 0;
    box-shadow: 0 -1px 2px rgba(0, 0, 0, 0.2);
    display: none;
    left: 0;
    padding: 0.6rem 1.25rem 0.7rem 1.25rem;
    position: fixed;
    width: 100%;
    z-index: 1000;
}

#blazor-error-ui .dismiss {
    cursor: pointer;
    position: absolute;
    right: 0.75rem;
    top: 0.5rem;
}

.blazor-error-boundary {
    background: url(data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNTYiIGhlaWdodD0iNDkiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyIgeG1sbnM6eGxpbms9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkveGxpbmsiIG92ZXJmbG93PSJoaWRkZW4iPjxkZWZzPjxjbGlwUGF0aCBpZD0iY2xpcDAiPjxyZWN0IHg9IjIzNSIgeT0iNTEiIHdpZHRoPSI1NiIgaGVpZ2h0PSI0OSIvPjwvY2xpcFBhdGg+PC9kZWZzPjxnIGNsaXAtcGF0aD0idXJsKCNjbGlwMCkiIHRyYW5zZm9ybT0idHJhbnNsYXRlKC0yMzUgLTUxKSI+PHBhdGggZD0iTTI2My41MDYgNTFDMjY0LjcxNyA1MSAyNjUuODEzIDUxLjQ4MzcgMjY2LjYwNiA1Mi4yNjU4TDI2Ny4wMzQgNTIuNjk0NUwyNjcuNTM5IDUzLjAxODNMMjY4LjEyMSA1My4zOTczIDI2OS4xNDEgNTMuOTYzMyAyNjkuNTg2IDU0LjE4MzFMMjcwLjU0NCA1NC43MzI4TDI3MS4yNTggNTUuMjAwMyAyNzEuNzU0IDU1LjU0NDMgMjcyLjE2NyA1NS44NTQzIDI3Mi41OCA1Ni4xNTQyIDI3My4xMSA1Ni41NjQxIDI3My41ODcgNTYuOTM3MyAyNzQuMDQ3IDU3LjMxNDMgMjc0LjQ5NyA1Ny42OTAzIDI3NC45NCA1OC4wODU4IDI3NS4zNzYgNTguNDc0NSAyNzUuODEzIDU4Ljg2MzIgMjc2LjIzOSA1OS4yNjE4IDI3Ni42NjYgNTkuNjU5MyAyNzcuMDkxIDYwLjA3NDYgMjc3LjUxMyA2MC40NzA1IDI3Ny45MzggNjAuODg2NSAyNzguMzU4IDYxLjMwOTMgMjc4Ljc3MyA2MS43MzI0IDI3OS4xNzUgNjIuMTU1NSAyNzkuNTc5IDYyLjU4NTUgMjgwLjAzNiA2My4wNzkgMjgwLjQ5MyA2My41NjMyIDI4MC44ODEgNjMuOTc0NSAyODEuMzYzIDY0LjUwMiAyODEuNzkxIDY0Ljk1ODcgMjgyLjEzIDY1LjMyMTcgMjgyLjMzNCA2NS41MzEyIDI4Mi41MzcgNjUuNzQwNyAyODIuNzk3IDY2LjAwNjggMjgzLjAzIDY2LjI0NTYgMjgzLjM0NCA2Ni41NTc1IDI4My43MjggNjYuOTQzMiAyODQuMDcxIDY3LjI4NjcgMjg0LjQ1MiA2Ny42NTM4IDI4NC44MzcgNjguMDM1TDI4NS40MjEgNjguNjU0MUwyODUuODUxIDY5LjEzMjZMMjg2LjUzNiA2OS45NjIgMjg2Ljk2OSA3MC41MDQgMjg3LjU5NyA3MS4zMDI4TDI4OC4wNDcgNzEuOTk1OEwyODguNjg1IDcyLjkwNzlMMjg5LjA2MSA3My41NTAzTDI4OS43NTEgNzQuNTYyTDI5MC4wODggNzUuMTQ3NyAyOTAuNTQ3IDc1Ljk5MTdMMjkxLjAyNyA3Ni45MDA5TDI5MS40NjcgNzcuNzY5MyAyOTEuODMxIDc4LjQ3MDMgMjkyLjE2MSA3OS4xMjIzIDI5Mi40OTIgNzkuNzQzM0wyOTIuODE3IDgwLjM0MTggMjkzLjE2NyA4MC45ODcxIDI5My40OTIgODEuNTgyMSAyOTMuODEzIDgyLjE3NzIgMjk0LjEzMSA4Mi43NzIzIDI5NC40MTkgODMuMzE5MyAyOTQuNzA3IDgzLjg2NjQgMjk0Ljk5MSA4NC40MTM1IDI5NS4yNjkgODQuOTQyMyAyOTUuNTQ1IDg1LjQ3MTEgMjk1LjgxNSA4NS45ODI2IDI5Ni4wODggODYuNTA0MSAyOTYuMzM4IDg2Ljk4ODMgMjk2LjU4NiA4Ny40NzI1IDI5Ni44MjggODcuOTQzNCAyOTcuMDY2IDg4LjQwNjYgMjk3LjMwMiA4OC44Njk3IDI5Ny41MTEgODkuMjgyMSAyOTcuNzE2IDg5LjY4NTQgMjk3LjkxMSA5MC4wNzIzIDI5OC4wOTYgOTAuNDQxIDI5OC4yODEgOTAuODA5NyAyOTguNDUgOTEuMTQyOCAyOTguNjE0IDkxLjQ2MzUgMjk4Ljc3MyA5MS43ODQzIDI5OC45MTcgOTIuMDYwNCAyOTkuMDU1IDkyLjMyMzkgMjk5LjE4NyA5Mi41NzU4IDI5OS4zMTQgOTIuODE1IDI5OS40MjggOTMuMDM0NiAyOTkuNTM0IDkzLjI0MTkgMjk5LjYyOSA5My40MjI4IDI5OS43MTggOTMuNTkzOCAyOTkuODAxIDkzLjc1MjMgMjk5Ljg3NyA5My44OTY5IDI5OS45NDcgOTQuMDI5OUMyOTkuOTgxIDk0LjA5NzEgMzAwIDk0LjE0NiAzMDAgOTQuMTk1TDMwMCA5NC4yMjE0QzMwMCA5NC4yNzY0IDI5OS45OTQgOTQuMzI5OSAyOTkuOTYzIDk0LjM5MTRDMjk5LjkzMiA5NC40NTI5IDI5OS44NzEgOTQuNDk2NSAyOTkuODE2IDk0LjU1MDhMMjk5LjczIDk0LjYzNzNMMjk5LjU5MiA5NC43NTk1IDI5OS40MTQgOTQuOTE0IDI5OS4xOTYgOTUuMTAxIDI5OC45MzkgOTUuMzIwM0wyOTguNjMgOTUuNTgxNkwyOTguMjY2IDk1Ljg3NzlMMjk3Ljg1IDk2LjIwODNMMjk3LjM4MiA5Ni41NzI4TDI5Ni44NjQgOTYuOTcwOCAyOTYuMjk5IDk3LjQwMTdMMjk1LjY4OCA5Ny44NjUzIDI5NS4wMjggOTguMzYyIDI5NC4zMjIgOTguODkxMyAyOTMuNTcgOTkuNDUzNSAyOTIuNzc0IDEwMC4wNDhMMjkxLjkzNCAxMDAuNjc1IDI5MS4wNTIgMTAxLjMzNEwyOTAuMTI4IDEwMi4wMjUgMjg5LjE2NCAxMDIuNzQ4IDI4OC4xNjIgMTAzLjUwMiAyODcuMTIxIDEwNC4yODkgMjg2LjA0NCAxMDUuMTA3IDI4NC45MzEgMTA1Ljk1NyAyODMuNzgzIDEwNi44MzkgMjgyLjYwMSAxMDcuNzUyIDI4MS4zODYgMTA4LjY5NiAyODAuMTQgMTA5LjY3MSAyNzguODYyIDExMC42NzcgMjc3LjU1NCAxMTEuNzE0IDI3Ni4yMTggMTEyLjc4MiAyNzQuODUzIDExMy44OCAyNzMuNDYyIDExNS4wMDggMjcyLjA0NCAxMTYuMTY3IDI3MC42MDIgMTE3LjM1NiAyNjkuMTM3IDExOC41NzQgMjY3LjY1IDExOS44MjIgMjY2LjE0IDExMS4wOTkgMjY0LjYxMSAxMjIuNDA1IDI2My4wNjQgMTIzLjc0IDI2MS41IDEyNS4xMDRMMjYwLjY0NCAxMjUuOTYyTDI1OS43MzkgMTI2Ljg1MSAyNTguNzg5IDEyNy43NzMgMjU3Ljc5NCAxMjguNzI1IDI1Ni43NTUgMTI5LjcwNiAyNTUuNjcxIDEzMC43MTggMjU0LjU0NSAxMzEuNzU5IDI1My4zNzcgMTMyLjgyOSAyNTIuMTY4IDEzMy45MjggMjUwLjkyIDEzNS4wNTUgMjQ5LjYzMyAxMzYuMjExIDI0OC4zMDkgMTM3LjM5NCAyNDYuOTQ5IDEzOC42MDQgMjQ1LjU1MyAxMzkuODQyIDI0NC4xMjMgMTQxLjEwNiAyNDIuNjU4IDE0Mi4zOTcgMjQxLjE2MSAxNDMuNzEzIDIzOS42MzEgMTQ1LjA1NiAyMzguMDcgMTQ2LjQyNCAyMzYuNDc3IDE0Ny44MTggMjM0Ljg1NSAxNDkuMjM3IDIzMy4yMDQgMTUwLjY4MSAyMzEuNTI0IDE1Mi4xNSAyMjkuODE3IDE1My42NDIgMjI4LjA4NCAxNTUuMTU4IDIyNi4zMjQgMTU2LjY5NiAyMjQuNTQgMTU4LjI1NkwyMjMuNDgzIDE1OC44OThDMjIyLjY4MyAxNTkuNDE5IDIyMS45MTQgMTU5LjkxOSAyMjEuMTc3IDE2MC4zOTggMjIwLjQ3MyAxNjAuODU0IDIxOS44MDEgMTYxLjI5IDIxOS4xNjEgMTYxLjcwNSAyMTguNTU0IDE2Mi4xIDIxNy45NzkgMTYyLjQ3NSAyMTcuNDM2IDE2Mi44MyAyMTYuOTI1IDE2My4xNjQgMjE2LjQ0NiAxNjMuNDc4IDIxNS45OTggMTYzLjc3MiAyMTUuNTgyIDE2NC4wNDUgMjE1LjE5OCAxNjQuMjk4IDIxNC44NDUgMTY0LjUzIDIxNC41MjMgMTY0Ljc0MSAyMTQuMjMyIDE2NC45MzEgMjEzLjk3MiAxNjUuMSAyMTMuNzQzIDE2NS4yNDggMjEzLjU0NCAxNjUuMzc1IDIxMy4zNzcgMTY1LjQ4MSAyMTMuMjM4IDE2NS41NjYgMjEzLjEzMSAxNjUuNjMgMjEzLjA1MyAxNjUuNjczQzIxMi45ODYgMTY1LjcwNyAyMTIuOTUgMTY1LjcyMSAyMTIuOTUgMTY1LjcyMUMyMTIuOTE1IDE2NS43MjEgMjEyLjg2NCAxNjUuNzA3IDIxMi43OTcgMTY1LjY3M0wyMTIuNzQ4IDE2NS42M0wyMTIuNjQ1IDE2NS41NjZMMjEyLjQzOSAxNjUuNDgxTDIxMi4xMTcgMTY1LjM3NUwyMTEuNjc4IDE2NS4yNDhMMjExLjEyMSAxNjUuMTAyTDIxMC40NDggMTY0LjkzMSAyMDkuNjU3IDE2NC43NDEgMjA4Ljc1IDE2NC41MyAyMDcuNzI1IDE2NC4yOTggMjA2LjU4MyAxNjQuMDQ1IDIwNS4zMjQgMTYzLjc3MiAyMDMuOTQ5IDE2My40NzggMjAyLjQ1NyAxNjMuMTY0IDIwMC44NSAxNjIuODMgMTk5LjEyNyAxNjIuNDc1IDE5Ny4yOSAxNjIuMSAxOTUuMzQgMTYxLjcwNSAxOTMuMjc2IDE2MS4yOSAxOTEuMDk5IDE2MC44NTQgMTg4LjgxIDE2MC4zOTggMTg2LjQxMSAxNTkuOTE5IDE4My45MDQgMTU5LjQxOSAxODEuMjg4IDE1OC44OThMMTc4LjU2NSAxNTguMzU2TDE3NS43MzggMTU3Ljc5MyAxNzIuODA2IDE1Ny4yMSAxNjkuNzcgMTU2LjYwNyAxNjYuNjMgMTU1Ljk4MiAxNjMuMzg4IDE1NS4zMzcgMTYwLjA0NSAxNTQuNjcyIDE1Ni42IDE1My45ODggMTUzLjA1NSAxNTMuMjg0IDE0OS40MSAxNTIuNTYxIDE0NS42NjYgMTUxLjgyIDE0MS44MjIgMTUxLjA2IDE0Ny44OCAxNTAuMjgzIDEzMy44NDEgMTQ5LjQ4OCAxMjkuNzA1IDE0OC42NzYgMTI1LjQ3MyAxNDcuODQ4IDEyMS4xNDYgMTQ3LjAwNCAxMTYuNzI1IDE0Ni4xNDQgMTEyLjIwOSAxNDUuMjcgMTA3LjYwMSAxNDQuMzgxIDEwMi45IDE0My40NzcgOTguMTA4NiAxNDIuNTYgOTMuMjI2NCAxNDEuNjI5IDg4LjI1NDQgMTQwLjY4NSA4My4xOTMzIDEzOS43MjggNzguMDQzMyAxMzguNzU5IDcyLjgwNTUgMTM3Ljc3NyA2Ny40Nzk5IDEzNi43ODMgNjIuMDY3OCAxMzUuNzc4IDU2LjU2OTYgMTM0Ljc2MSA1MC45ODY2IDEzMy43MzMgNDUuMzE5NiAxMzIuNjk0IDM5LjU3MDMgMTMxLjY0NSAzMy43Mzk0IDEzMC41ODYgMjcuODI4MiAxMjkuNTE3IDIxLjgzODUgMTI4LjQzOSAxNS43NzA5IDEyNy4zNTIgOS42MjY2NyAxMjYuMjU2IDMuNDA3MDggMTI1LjE1MSAyLjgxNDA3IDEyNC45NTggMi4yMTEwNiAxMjQuNzQ4IDEuNTk3NTEgMTI0LjUyMSAwLjk3MzkzIDEyNC4yNzcgMC4zNDAyNjQgMTI0LjAxNyAxLjIyMDkyZS0wNyAxMjMuNzM5QzAuMjEyMDkyIDEyMy42MDkgMC4wODczOTIxIDEyMy40NDMgMC4wMDEzNTIxMiAxMjMuMjQyTC0wLjAwMTY0Nzg5IDEyMy4xMDRDLTAuMDA2NjQ3ODkgMTIyLjk2OSAtMC4wMDE2NDc4OSAxMjIuODI0IC0wLjAwMTY0Nzg5IDEyMi42NzFMMCAxMjEuNThDMCAxMjEuNDI3IDAgMTIxLjI4MiAwIDEyMS4xNDhMMCAxMjEuMDA5VjEyMC44NzJWMTIwLjczNVYxMjAuNTk5VjEyMC40NjVWMTIwLjMzMlYxMjAuMlYxMjAuMDcxVjExOS45NDNWMTE5LjgxNlYxMTE5LjY5MVYxMTkuNTY4VjExOS40NDdWMTE5LjMyOFYxMTkuMjFWMTE5LjA5M1YxMTguOTc5VjExOC44NjdWMTE4Ljc1NlYxMTguNjQ3VjExOC41NFYxMTguNDM1VjExOC4zMzJWMTE4LjIzVjExOC4xM1YxMTguMDMyVjExNy45MzZWMTE3Ljg0MVYxMTcuNzQ4VjExNy42NTdWMTE3LjU2N1YxMTcuNDc5VjExNy4zOTJWMTE3LjMwN1YxMTcuMjIzVjExNy4xNDFWMTE3LjA2VjExNi45OFYxMTYuOTAyVjExNi44MjVWMTE2Ljc1VjExNi42NzZWMTE2LjYwM1YxMTYuNTMxVjExNi40NjFWMTE2LjM5MlYxMTYuMzI0VjExNi4yNTdWMTE2LjE5MVYxMTYuMTI3VjExNi4wNjNWMTE2LjAwMVYxMTUuOTRWMTE1Ljg4VjExNS44MjFWMTE1Ljc2M1YxMTUuNzA2VjExNS42NVYxMTUuNTk1VjExNS41NDFWMTE1LjQ4OFYxMTUuNDM2VjExNS4zODVWMTE1LjMzNVYxMTUuMjg2VjExNS4yMzhWMTE1LjE5MVYxMTUuMTQ1VjExNS4wOThWMTE1LjA1NFYxMTUuMDExVjExNC45NjhWMTE0LjkyNlYxMTQuODg1VjExNC44NDVWMTE0LjgwNlYxMTQuNzY4VjExNC43M1YxMTQuNjkzVjExNC42NTdWMTE0LjYyMVYxMTQuNTg2VjExNC41NTJWMTE0LjUxOVYxMTQuNDg2VjExNC40NTRWMTE0LjQyM1YxMTQuMzkzVjExNC4zNjNWMTE0LjMzNFYxMTQuMzA2VjExNC4yNzhWMTE0LjI1MVYxMTQuMjI0VjExNC4xOThWMTE0LjE3M1YxMTQuMTQ4VjExNC4xMjRWMTE0LjFWMTE0LjA3N1YxMTQuMDU0VjExNC4wMzJWMTE0LjAxVjExMy45ODlWMTEzLjk2OVYxMTMuOTQ5VjExMy45MjlWMTEzLjkxVjExMy44OTFWMTEzLjg3M1YxMTMuODU1VjExMy44MzhWMTEzLjgyMVYxMTMuODA0VjExMy43ODhWMTEzLjc3MlYxMTMuNzU3VjExMy43NDJWMTEzLjcyN1YxMTMuNzEzVjExMy42OTlWMTEzLjY4NlYxMTMuNjczVjExMy42NlYxMTMuNjQ4VjExMy42MzZWMTEzLjYyNFYxMTMuNjEyVjExMy42MDFWMTEzLjU5VjExMy41OFYxMTMuNTdWMTEzLjU2VjExMy41NVYxMTMuNTQxVjExMy41MzJWMTEzLjUyM1YxMTMuNTE0VjExMy41MDZWMTEzLjQ5OFYxMTMuNDlWMTEzLjQ4MlYxMTMuNDc1VjExMy40NjhWMTEzLjQ2MVYxMTMuNDU0VjExMy40NDhWMTEzLjQ0MVYxMTMuNDM1VjExMy40MjlWMTEzLjQyM1YxMTMuNDE4VjExMy40MTJWMTEzLjQwN1YxMTMuNDAyVjExMy4zOTdWMTEzLjM5MlYxMTMuMzg4VjExMy4zODNWMTEzLjM3OVYxMTMuMzc1VjExMy4zNzFWMTEzLjM2N1YxMTMuMzYzVjExMy4zNlYxMTMuMzU3VjExMy4zNTNWMTEzLjM1VjExMy4zNDdWMTEzLjM0NFYxMTMuMzQyVjExMy4zMzlWMTEzLjMzN1YxMTMuMzM0VjExMy4zMzJWMTEzLjMzVjExMy4zMjhWMTEzLjMyNlYxMTMuMzI0VjExMy4zMjNWMTEzLjMyMVYxMTMuMzJWMTEzLjMxOFYxMTMuMzE3VjExMy4zMTVWMTEzLjMxNFYxMTMuMzEzVjExMy4zMTJWMTEzLjMxMVYxMTMuMzFWMTEzLjMwOVYxMTMuMzA4VjExMy4zMDdWMTEzLjMwN1YxMTMuMzA2VjExMy4zMDZWMTEzLjMwNVYxMTMuMzA1VjExMy4zMDVWMTEzLjMwNVYxMTMuMzA1VjExMy4zMDVWMTEzLjMwNVYxMTMuMzA2VjExMy4zMDZWMTEzLjMwN1YxMTMuMzA3VjExMy4zMDhWMTEzLjMwOVYxMTMuMzFWMTEzLjMxMVYxMTMuMzEyVjExMy4zMTNWMTEzLjMxNFYxMTMuMzE2VjExMy4zMTdWMTEzLjMxOVYxMTMuMzIxVjExMy4zMjNWMTEzLjMyNVYxMTMuMzI3VjExMy4zMjlWMTEzLjMzMVYxMTMuMzM0VjExMy4zMzdWMTEzLjMzOVYxMTMuMzQyVjExMy4zNDVWMTEzLjM0OFYxMTMuMzUyVjExMy4zNTVWMTEzLjM1OVYxMTMuMzYzVjExMy4zNjdWMTEzLjM3MVYxMTMuMzc1VjExMy4zODBWMTEzLjM4NFYxMTMuMzg5VjExMy4zOTRWMTEzLjM5OVYxMTMuNDA0VjExMy40MXYxMTMuNDE2VjExMy40MjFWMTEzLjQyN1YxMTMuNDM0VjExMy40NFYxMTMuNDQ3VjExMy40NTNWMTEzLjQ2VjExMy40NjdWMTEzLjQ3NFYxMTMuNDgyVjExMy40OVYxMTMuNDk4VjExMy41MDZWMTEzLjUxNFYxMTMuNTIzVjExMy41MzFWMTEzLjU0VjExMy41NDlWMTEzLjU1OVYxMTMuNTY4VjExMy41NzhWMTEzLjU4OFYxMTMuNTk4VjExMy42MDlWMTEzLjYyVjExMy42MzFWMTEzLjY0MlYxMTMuNjUzVjExMy42NjVWMTEzLjY3N1YxMTMuNjg5VjExMy43MDFWMTEzLjcxNFYxMTMuNzI3VjExMy43NFYxMTMuNzUzVjExMy43NjdWMTEzLjc4MVYxMTMuNzk1VjExMy44MDlWMTEzLjgyNFYxMTMuODM5VjExMy44NTRWMTEzLjg2OVYxMTMuODg1VjExMy45MDFWMTEzLjkxN1YxMTMuOTM0VjExMy45NTFWMTEzLjk2OFYxMTMuOTg1VjExNC4wMDNWMTE0LjAyMVYxMTQuMDRWMTE0LjA1OFYxMTQuMDc3VjExNC4wOTZWMTE0LjExNlYxMTQuMTM2VjExNC4xNTZWMTE0LjE3N1YxMTQuMTk4VjExNC4yMTlWMTE0LjI0MVYxMTQuMjYzVjExNC4yODVWMTE0LjMwOFYxMTQuMzMxVjExNC4zNTVWMTE0LjM3OVYxMTQuNDAzVjExNC40MjdWMTE0LjQ1MlYxMTQuNDc3VjExNC41MDJWMTE0LjUyOFYxMTQuNTU1VjExNC41ODJWMTE0LjYwOVYxMTQuNjM2VjExNC42NjRWMTE0LjY5MlYxMTQuNzIxVjExNC43NVYxMTQuNzc5VjExNC44MDlWMTE0LjgzOVYxMTQuODY5VjExNC45VjExNC45MzFWMTE0Ljk2M1YxMTQuOTk1VjExNS4wMjhWMTE1LjA2MVYxMTUuMDk0VjExNS4xMjhWMTE1LjE2MlYxMTUuMTk3VjExNS4yMzJWMTE1LjI2N1YxMTUuMzAzVjExNS4zMzlWMTE1LjM3NlYxMTUuNDEzVjExNS40NVYxMTUuNDg4VjExNS41MjZWMTE1LjU2NFYxMTUuNjAzVjExNS42NDNWMTE1LjY4MlYxMTUuNzIzVjExNS43NjNWMTE1LjgwNFYxMTUuODQ1VjExNS44ODdWMTE1LjkyOVYxMTUuOTcyVjExNi4wMTVWMTE2LjA1OVYxMTYuMTAzVjExNi4xNDdWMTE2LjE5MlYxMTYuMjM4VjExNi4yODRWMTE2LjMzVjExNi4zNzdWMTE2LjQyNFYxMTYuNDcyVjExNi41MlYxMTYuNTY5VjExNi42MThWMTE2LjY2OFYxMTYuNzE5VjExNi43N1YxMTYuODIyVjExNi44NzRWMTE2LjkyN1YxMTYuOThWMTE3LjAzNFYxMTcuMDg5VjExNy4xNDRWMTE3LjJWMTE3LjI1NlYxMTcuMzEzVjExNy4zNzFWMTE3LjQyOVYxMTcuNDg4VjExNy41NDdWMTE3LjYwN1YxMTcuNjY4VjExNy43MjlWMTE3LjgxMTI1LmQgYmMgY2YuYiBlZi5mIDAtMTIzNiAzNzAtMTIzNiAzNzBjMCAwIDI4Ni41MiAxMjEuOCAyODYuNTIgMjQ2Ljc1IDAgMTI0Ljk1LTI4Ni41MiAyNDYuNzUtMjg2LjUyIDI0Ni43NSAwIDAtMzMuNyAyMTkuMDUtMzMuNyAyMTkuMDVsMzMuNy00Mi41UzYwLjcgNzg4LjA1IDYwLjcgNzg4LjA1TDI2My41MDYgNTF6Ii8+PC9nPjwvc3ZnPg==) no-repeat 1rem/1.8rem, #b32121;
    padding: 1rem 1rem 1rem 3.7rem;
    color: white;
}

.blazor-error-boundary::after {
    content: "An error has occurred."
}
```

---

## 12. 빌드 및 실행 확인

### 12.1. 빌드 명령어

```bash
cd MdcHR26Apps.BlazorServer
dotnet build
```

**예상 출력**:
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

### 12.2. 실행 명령어

```bash
dotnet run
```

**예상 출력**:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

### 12.3. 브라우저 접속

- **HTTPS**: https://localhost:5001
- **HTTP**: http://localhost:5000

---

## 13. 다음 단계 (Phase 3-2)

Phase 3-1 완료 후 다음 작업지시서:

### Phase 3-2: 로그인 및 인증 시스템
- Login.razor 구현
- Logout.razor 구현
- Manage.razor (비밀번호 변경)
- LoginStatusService 완성
- SHA-256 + Salt 로그인 연동

### Phase 3-3: 관리자 페이지
- 사용자 관리 (CRUD)
- 부서 관리 (CRUD)
- 평가대상자 관리
- 평가 관리 (평가 개시/종료)

### Phase 3-4: 평가 프로세스 페이지
- 직무평가 협의
- 본인평가 (1차)
- 부서장평가 (2차)
- 임원평가 (3차)
- 최종 결과

---

## 14. 참고 자료

### 14.1. Blazor 공식 문서
- [Blazor 개요](https://learn.microsoft.com/ko-kr/aspnet/core/blazor/)
- [Blazor Server](https://learn.microsoft.com/ko-kr/aspnet/core/blazor/hosting-models#blazor-server)
- [SignalR](https://learn.microsoft.com/ko-kr/aspnet/core/signalr/)

### 14.2. 참조 프로젝트
- 2025년 인사평가: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp`
- 도서관리: `C:\Codes\36_MdcLibrary\MdcLibrary\MdcLibrary.Server`

### 14.3. Phase 2 Model
- Models 프로젝트: `C:\Codes\00_Develop_Cursor\10_MdcHR26Apps\MdcHR26Apps.Models`
- Repository 사용 예시: 작업지시서 20260119_01~04 참조

---

## 15. 체크리스트

### Phase 3-1 완료 기준
- [ ] Blazor Server 프로젝트 생성 (.NET 10.0)
- [ ] 프로젝트 참조 추가 (MdcHR26Apps.Models)
- [ ] NuGet 패키지 설치 (ClosedXML)
- [ ] Program.cs DI 설정 완료
- [ ] appsettings.json 환경 설정 완료
- [ ] App.razor, Routes.razor 구성
- [ ] MainLayout, NavMenu 기본 구조
- [ ] Home.razor, Error.razor 페이지
- [ ] LoginStatus, LoginStatusService, AppStateService 작성
- [ ] UrlActions 작성
- [ ] wwwroot 정적 파일 (CSS) 추가
- [ ] 빌드 성공 (오류 0개)
- [ ] 로컬 실행 확인 (https://localhost:5001)

---

**작성자**: Claude AI
**검토자**: 개발자
**승인 일자**: 추후 기재
