using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MdcHR26Apps.Models.User;
using MdcHR26Apps.Models.Department;
using MdcHR26Apps.Models.Rank;

namespace MdcHR26Apps.Models;

/// <summary>
/// DI 컨테이너 확장 메서드
/// </summary>
public static class MdcHR26AppsAddExtensions
{
    /// <summary>
    /// MdcHR26Apps.Models의 모든 서비스를 DI 컨테이너에 등록
    /// </summary>
    public static IServiceCollection AddMdcHR26AppsModels(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 연결 문자열
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("연결 문자열 'DefaultConnection'을 찾을 수 없습니다.");

        // === EF Core DbContext ===
        services.AddDbContext<MdcHR26AppsAddDbContext>(options =>
            options.UseSqlServer(connectionString));

        // === Phase 2-1: Repository 등록 (3개) ===
        services.AddScoped<IUserRepository>(provider =>
        {
            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
            return new UserRepository(connectionString, loggerFactory);
        });

        services.AddScoped<IEDepartmentRepository>(provider =>
        {
            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
            return new EDepartmentRepository(connectionString, loggerFactory);
        });

        services.AddScoped<IERankRepository>(provider =>
        {
            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
            return new ERankRepository(connectionString, loggerFactory);
        });

        // === Phase 2-2, 2-3: 나머지 Repository (추후 추가) ===
        // services.AddScoped<IDeptObjectiveRepository>(provider => new DeptObjectiveRepository(connectionString));
        // services.AddScoped<IAgreementRepository>(provider => new AgreementRepository(connectionString));
        // services.AddScoped<ISubAgreementRepository>(provider => new SubAgreementRepository(connectionString));
        // services.AddScoped<IProcessRepository>(provider => new ProcessRepository(connectionString));
        // services.AddScoped<IReportRepository>(provider => new ReportRepository(connectionString));
        // services.AddScoped<ITotalReportRepository>(provider => new TotalReportRepository(connectionString));
        // services.AddScoped<IEvaluationUsersRepository>(provider => new EvaluationUsersRepository(connectionString));
        // services.AddScoped<ITasksRepository>(provider => new TasksRepository(connectionString));
        // services.AddScoped<IEvaluationListsRepository>(provider => new EvaluationListsRepository(connectionString));

        // === Phase 2-4: View Repository (추후 추가) ===
        // services.AddScoped<Iv_MemberListRepository>(provider => new v_MemberListRepository(connectionString));
        // services.AddScoped<Iv_DeptObjectiveListRepository>(provider => new v_DeptObjectiveListRepository(connectionString));
        // services.AddScoped<Iv_ProcessTRListRepository>(provider => new v_ProcessTRListRepository(connectionString));
        // services.AddScoped<Iv_ReportTaskListRepository>(provider => new v_ReportTaskListRepository(connectionString));
        // services.AddScoped<Iv_TotalReportListRepository>(provider => new v_TotalReportListRepository(connectionString));

        return services;
    }
}
