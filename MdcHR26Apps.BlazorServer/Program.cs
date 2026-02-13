using MdcHR26Apps.BlazorServer.Components;
using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.BlazorServer.Utils;
using MdcHR26Apps.Models;
using Microsoft.AspNetCore.DataProtection;
using System.IO;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// Data Protection (persist keys for containers)
// ========================================
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/var/dpkeys"))
    .SetApplicationName("MdcHR26Apps");

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
// 2-1. 유틸리티 서비스 등록
// ========================================
builder.Services.AddScoped<UserUtils>();
builder.Services.AddTransient<ScoreUtils>();
builder.Services.AddTransient<ExcelManage>();

// ========================================
// 3. Model 계층 DI 등록 (Phase 2 연동)
// ========================================
// AppSettings:IsProduction 값에 따라 자동으로 연결 문자열 선택
// 0 = 개발환경 (DefaultConnection - LocalDB)
// 1 = 프로덕션 환경 (MdcHR26AppsContainerConnection - Docker)
builder.Services.AddMdcHR26AppsModels(builder.Configuration);

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

// 런타임 생성 파일 전용 서빙
var tasksPath = Path.Combine(app.Environment.WebRootPath!, "files", "tasks");
Directory.CreateDirectory(tasksPath);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(tasksPath),
    RequestPath = "/files/tasks"
});

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
