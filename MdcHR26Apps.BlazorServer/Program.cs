using MdcHR26Apps.BlazorServer.Components;
using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.BlazorServer.Utils;
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
// 2-1. 유틸리티 서비스 등록
// ========================================
builder.Services.AddScoped<UserUtils>();
builder.Services.AddTransient<ScoreUtils>();

// ========================================
// 3. Model 계층 DI 등록 (Phase 2 연동)
// ========================================
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
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
