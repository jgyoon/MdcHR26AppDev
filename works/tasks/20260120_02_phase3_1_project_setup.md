# 작업지시서: Phase 3-1 - Blazor Server 프로젝트 생성 및 기본 설정

**날짜**: 2026-01-20
**작업 유형**: 신규 프로젝트 생성 (Phase 3-1)
**관련 이슈**: [#009: Phase 3 WebApp 개발](../issues/009_phase3_webapp_development.md)
**선행 작업지시서**: `20260120_01_phase3_blazor_webapp.md` (Phase 3 전체 계획)

---

## 1. 작업 개요

### 1.1. Phase 3-1 목표
Blazor Server 프로젝트를 생성하고 기본적인 실행 환경을 구축합니다.

### 1.2. 작업 범위
- ✅ Blazor Server 프로젝트 생성 (.NET 10.0)
- ✅ 프로젝트 참조 및 패키지 설치
- ✅ Program.cs DI 설정
- ✅ 환경 설정 (appsettings.json)
- ✅ 기본 레이아웃 및 페이지
- ✅ 상태 관리 서비스 (기본 구조)
- ✅ 빌드 및 실행 확인

### 1.3. 완료 기준
- 프로젝트가 정상적으로 빌드됨 (오류 0개)
- https://localhost:5001 접속 시 홈 페이지 표시
- Phase 2 Model DI 연동 확인

---

## 2. 프로젝트 생성

### 2.1. Git 브랜치 확인

현재 브랜치: `feature/phase3-webapp` (이미 생성됨)

```bash
# 현재 브랜치 확인
git branch

# feature/phase3-webapp 브랜치에 있는지 확인
git status
```

### 2.2. .NET CLI로 프로젝트 생성

**작업 디렉토리로 이동**:
```bash
cd C:\Codes\00_Develop_Cursor\10_MdcHR26Apps
```

**Blazor Server 프로젝트 생성**:
```bash
dotnet new blazor -o MdcHR26Apps.BlazorServer -f net10.0 --interactivity Server --all-interactive
```

**명령어 옵션 설명**:
- `-o MdcHR26Apps.BlazorServer`: 출력 디렉토리 지정
- `-f net10.0`: .NET 10.0 타겟 프레임워크
- `--interactivity Server`: Blazor Server 렌더링 모드
- `--all-interactive`: 모든 컴포넌트를 인터랙티브로 설정

**솔루션에 프로젝트 추가**:
```bash
dotnet sln add MdcHR26Apps.BlazorServer/MdcHR26Apps.BlazorServer.csproj
```

**생성 확인**:
```bash
# 프로젝트 폴더 구조 확인
dir MdcHR26Apps.BlazorServer

# 솔루션 프로젝트 목록 확인
dotnet sln list
```

**예상 출력**:
```
프로젝트
--------
MdcHR26Apps.Models\MdcHR26Apps.Models.csproj
MdcHR26Apps.BlazorServer\MdcHR26Apps.BlazorServer.csproj
```

---

## 3. 프로젝트 참조 및 패키지

### 3.1. 프로젝트 참조 추가

**MdcHR26Apps.BlazorServer.csproj 수정**:

```xml
<ItemGroup>
    <ProjectReference Include="..\MdcHR26Apps.Models\MdcHR26Apps.Models.csproj" />
</ItemGroup>
```

**또는 CLI 명령어**:
```bash
cd MdcHR26Apps.BlazorServer
dotnet add reference ..\MdcHR26Apps.Models\MdcHR26Apps.Models.csproj
```

### 3.2. NuGet 패키지 설치

```bash
cd MdcHR26Apps.BlazorServer
dotnet add package ClosedXML --version 0.104.2
```

**MdcHR26Apps.BlazorServer.csproj 확인**:

```xml
<ItemGroup>
    <PackageReference Include="ClosedXML" Version="0.104.2" />
</ItemGroup>
```

### 3.3. Bootstrap 로컬 설치 (LibMan 사용)

**중요**: CDN 대신 LibMan을 사용하여 Bootstrap을 로컬에 설치합니다.

#### 3.3.1. libman.json 파일 생성

프로젝트 루트에 `libman.json` 파일을 생성합니다:

```json
{
  "version": "1.0",
  "defaultProvider": "cdnjs",
  "libraries": [
    {
      "library": "bootstrap@5.3.3",
      "destination": "wwwroot/lib/bootstrap/",
      "files": [
        "css/bootstrap.min.css",
        "css/bootstrap.min.css.map"
      ]
    },
    {
      "library": "bootstrap-icons@1.11.3",
      "destination": "wwwroot/lib/bootstrap-icons/",
      "files": [
        "font/bootstrap-icons.css",
        "font/fonts/bootstrap-icons.woff",
        "font/fonts/bootstrap-icons.woff2"
      ]
    }
  ]
}
```

#### 3.3.2. LibMan 설치 (필요한 경우)

```bash
# LibMan CLI가 설치되지 않은 경우
dotnet tool install -g Microsoft.Web.LibraryManager.Cli

# 설치 확인
libman --version
```

#### 3.3.3. Bootstrap 다운로드

```bash
cd MdcHR26Apps.BlazorServer

# libman.json 기반으로 라이브러리 복원
libman restore
```

**예상 출력**:
```
wwwroot/lib/bootstrap/css/bootstrap.min.css written to disk
wwwroot/lib/bootstrap/css/bootstrap.min.css.map written to disk
wwwroot/lib/bootstrap-icons/font/bootstrap-icons.css written to disk
wwwroot/lib/bootstrap-icons/font/fonts/bootstrap-icons.woff written to disk
wwwroot/lib/bootstrap-icons/font/fonts/bootstrap-icons.woff2 written to disk
```

#### 3.3.4. 폴더 구조 확인

설치 후 다음 구조가 생성되어야 합니다:

```
wwwroot/
├── css/
│   ├── app.css
│   └── LoadingSpinner.css
└── lib/
    ├── bootstrap/
    │   └── css/
    │       ├── bootstrap.min.css
    │       └── bootstrap.min.css.map
    └── bootstrap-icons/
        └── font/
            ├── bootstrap-icons.css
            └── fonts/
                ├── bootstrap-icons.woff
                └── bootstrap-icons.woff2
```

---

## 4. Program.cs 설정

**전체 코드** (`Program.cs`):

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
        options.DetailedErrors = false;
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
        options.MaximumReceiveMessageSize = 32 * 1024;
        options.StreamBufferCapacity = 10;
    });

// ========================================
// 2. 상태 관리 서비스 등록
// ========================================
builder.Services.AddScoped<LoginStatusService>();
builder.Services.AddScoped<AppStateService>();
builder.Services.AddTransient<UrlActions>();

// ========================================
// 3. Model 계층 DI 등록 (Phase 2 연동)
// ========================================
var isProduction = builder.Configuration.GetValue<int>("AppSettings:IsProduction");
string connectionString;

if (isProduction == 0)
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}
else
{
    connectionString = builder.Configuration.GetConnectionString("MdcHR26AppsContainerConnection")
        ?? throw new InvalidOperationException("Connection string 'MdcHR26AppsContainerConnection' not found.");
}

builder.Services.AddDependencyInjectionContainerForMdcHR26AppModels(connectionString);

// ========================================
// 4. 보안 및 추가 서비스
// ========================================
builder.Services.AddAntiforgery();

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

---

## 5. 환경 설정 파일

### 5.1. appsettings.json

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

### 5.2. appsettings.Development.json

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

---

## 6. 상태 관리 서비스

### 6.1. Data/LoginStatus.cs

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

### 6.2. Data/LoginStatusService.cs

```csharp
namespace MdcHR26Apps.BlazorServer.Data;

public class LoginStatusService
{
    public LoginStatus LoginStatus { get; set; } = new();

    public event Action? OnChange;

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

    public bool IsloginCheck() => LoginStatus.IsLogin;

    public bool IsloginAndIsAdminCheck() =>
        LoginStatus.IsLogin && LoginStatus.LoginIsAdministrator;

    public bool IsloginAndIsTeamLeaderCheck() =>
        LoginStatus.IsLogin && LoginStatus.LoginIsTeamLeader;

    public bool IsloginAndIsDirectorCheck() =>
        LoginStatus.IsLogin && LoginStatus.LoginIsDirector;
}
```

### 6.3. Data/AppStateService.cs

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

    public bool GetIsOpen()
    {
        var isOpen = _configuration.GetValue<int>("AppSettings:IsOpen");
        return isOpen == 1;
    }

    public string TruncateText(string? text, int maxLength)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        return text.Length <= maxLength ? text : $"{text.Substring(0, maxLength)}...";
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
```

### 6.4. Data/UrlActions.cs

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

## 7. App 및 라우팅

### 7.1. Components/App.razor

```html
<!DOCTYPE html>
<html lang="ko">

<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <base href="/" />
    <link rel="stylesheet" href="lib/bootstrap/css/bootstrap.min.css" />
    <link rel="stylesheet" href="lib/bootstrap-icons/font/bootstrap-icons.css" />
    <link rel="stylesheet" href="css/app.css" />
    <link rel="stylesheet" href="css/LoadingSpinner.css" />
    <link rel="icon" type="image/png" href="favicon.png" />
    <HeadOutlet @rendermode="InteractiveServer" />
</head>

<body>
    <Routes @rendermode="InteractiveServer" />

    <div id="blazor-error-ui">
        오류가 발생했습니다.
        <a href="" class="reload">새로고침</a>
        <a class="dismiss">🗙</a>
    </div>

    <script src="_framework/blazor.web.js"></script>
</body>

</html>
```

### 7.2. Components/Routes.razor

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

### 7.3. Components/Pages/_Imports.razor

```csharp
@using System.Net.Http
@using System.Net.Http.Json
@using Microsoft.AspNetCore.Components.Forms
@using Microsoft.AspNetCore.Components.Routing
@using Microsoft.AspNetCore.Components.Web
@using static Microsoft.AspNetCore.Components.Web.RenderMode
@using Microsoft.AspNetCore.Components.Web.Virtualization
@using Microsoft.JSInterop
@using MdcHR26Apps.BlazorServer
@using MdcHR26Apps.BlazorServer.Components
@using MdcHR26Apps.BlazorServer.Data
@using MdcHR26Apps.Models
```

---

## 8. 레이아웃

### 8.1. Components/Layout/MainLayout.razor

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

### 8.2. Components/Layout/MainLayout.razor.css

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
    text-decoration: none;
}

.top-row a:hover {
    text-decoration: underline;
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

### 8.3. Components/Layout/NavMenu.razor

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
        <div class="nav-item px-3">
            <NavLink class="nav-link" href="" Match="NavLinkMatch.All">
                <i class="bi bi-house-door-fill" aria-hidden="true"></i> Home
            </NavLink>
        </div>

        @if (!loginStatusService.IsloginCheck())
        {
            <div class="nav-item px-3">
                <NavLink class="nav-link" href="auth/login">
                    <i class="bi bi-box-arrow-in-right" aria-hidden="true"></i> Login
                </NavLink>
            </div>
        }

        @if (loginStatusService.IsloginCheck())
        {
            @if (loginStatusService.IsloginAndIsAdminCheck())
            {
                <div class="nav-item px-3">
                    <NavLink class="nav-link" href="admin">
                        <i class="bi bi-gear-fill" aria-hidden="true"></i> 관리자
                    </NavLink>
                </div>
            }

            <div class="nav-item px-3">
                <NavLink class="nav-link" href="auth/logout">
                    <i class="bi bi-box-arrow-right" aria-hidden="true"></i> Logout
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

### 8.4. Components/Layout/NavMenu.razor.css

```css
.navbar-toggler {
    appearance: none;
    cursor: pointer;
    width: 3.5rem;
    height: 2.5rem;
    color: white;
    position: absolute;
    top: 0.5rem;
    right: 1rem;
    border: 1px solid rgba(255, 255, 255, 0.1);
    background: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 30 30'%3e%3cpath stroke='rgba%28255, 255, 255, 0.55%29' stroke-linecap='round' stroke-miterlimit='10' stroke-width='2' d='M4 7h22M4 15h22M4 23h22'/%3e%3c/svg%3e") no-repeat center/1.75rem rgba(255, 255, 255, 0.1);
}

.navbar-toggler:checked {
    background-color: rgba(255, 255, 255, 0.5);
}

.top-row {
    height: 3.5rem;
    background-color: rgba(0,0,0,0.4);
}

.navbar-brand {
    font-size: 1.1rem;
}

.bi {
    display: inline-block;
    width: 1rem;
    height: 1rem;
    margin-right: 0.75rem;
}

.nav-item {
    font-size: 0.9rem;
    padding-bottom: 0.5rem;
}

.nav-item:first-of-type {
    padding-top: 1rem;
}

.nav-item:last-of-type {
    padding-bottom: 1rem;
}

.nav-item a {
    color: #d7d7d7;
    display: flex;
    align-items: center;
    line-height: 3rem;
    padding: 0.5rem 1rem;
    border-radius: 4px;
}

.nav-item a.active {
    background-color: rgba(255,255,255,0.37);
    color: white;
}

.nav-item a:hover {
    background-color: rgba(255,255,255,0.1);
    color: white;
}

.nav-scrollable {
    display: none;
}

.navbar-toggler:checked ~ .nav-scrollable {
    display: block;
}

@media (min-width: 641px) {
    .navbar-toggler {
        display: none;
    }

    .nav-scrollable {
        display: block;
        height: calc(100vh - 3.5rem);
        overflow-y: auto;
    }
}
```

---

## 9. 기본 페이지

### 9.1. Components/Pages/Home.razor

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

### 9.2. Components/Pages/Error.razor

```html
@page "/Error"
@using System.Diagnostics

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

## 10. wwwroot 정적 파일

### 10.1. wwwroot/css/app.css

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
```

### 10.2. wwwroot/css/LoadingSpinner.css

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

---

## 11. 빌드 및 실행

### 11.1. 빌드

```bash
cd MdcHR26Apps.BlazorServer
dotnet build
```

**예상 출력**:
```
빌드 성공
    경고 0개
    오류 0개
```

### 11.2. 실행

```bash
dotnet run
```

**예상 출력**:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
```

### 11.3. 브라우저 접속

- https://localhost:5001
- 홈 페이지 정상 표시 확인
- "로그인이 필요합니다" 메시지 확인

---

## 12. 완료 체크리스트

- [ ] Blazor Server 프로젝트 생성 완료
- [ ] 프로젝트 참조 추가 (MdcHR26Apps.Models)
- [ ] NuGet 패키지 설치 (ClosedXML)
- [ ] **libman.json 생성 및 Bootstrap 로컬 설치** ⭐
- [ ] **libman restore 실행 (Bootstrap + Icons 다운로드)** ⭐
- [ ] Program.cs 설정 완료
- [ ] appsettings.json 설정 완료
- [ ] App.razor, Routes.razor 작성
- [ ] MainLayout, NavMenu 작성 (Bootstrap Icons 포함)
- [ ] Home.razor, Error.razor 작성
- [ ] LoginStatus, LoginStatusService 작성
- [ ] AppStateService, UrlActions 작성
- [ ] wwwroot/css 파일 작성
- [ ] **wwwroot/lib/bootstrap 폴더 확인** ⭐
- [ ] **wwwroot/lib/bootstrap-icons 폴더 확인** ⭐
- [ ] 빌드 성공 (오류 0개)
- [ ] 실행 확인 (https://localhost:5001)
- [ ] **Bootstrap 스타일 적용 확인 (네비게이션 바, 아이콘)** ⭐

---

## 13. 다음 단계

**Phase 3-2: 로그인 및 인증 시스템 구현**
- Login.razor
- Logout.razor
- Manage.razor
- SHA-256 로그인 연동

---

**작성자**: Claude AI
**검토자**: 개발자
**승인 일자**: 추후 기재
