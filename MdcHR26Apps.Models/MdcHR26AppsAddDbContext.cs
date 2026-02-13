using Microsoft.EntityFrameworkCore;
using MdcHR26Apps.Models.User;
using MdcHR26Apps.Models.Department;
using MdcHR26Apps.Models.Rank;
using MdcHR26Apps.Models.EvaluationProcess;
using MdcHR26Apps.Models.EvaluationReport;
using MdcHR26Apps.Models.Result;
using MdcHR26Apps.Models.EvaluationUsers;
using MdcHR26Apps.Models.DeptObjective;
using MdcHR26Apps.Models.EvaluationAgreement;
using MdcHR26Apps.Models.EvaluationSubAgreement;
using MdcHR26Apps.Models.EvaluationTasks;
using MdcHR26Apps.Models.EvaluationLists;
using MdcHR26Apps.Models.Views.v_MemberListDB;
using MdcHR26Apps.Models.Views.v_DeptObjectiveListDb;
using MdcHR26Apps.Models.Views.v_ProcessTRListDB;
using MdcHR26Apps.Models.Views.v_ReportTaskListDB;
using MdcHR26Apps.Models.Views.v_TotalReportListDB;
using MdcHR26Apps.Models.Views.v_EvaluationUsersList;

namespace MdcHR26Apps.Models;

/// <summary>
/// EF Core DbContext
/// 12개 테이블 + 5개 뷰 매핑
/// </summary>
public class MdcHR26AppsAddDbContext(DbContextOptions<MdcHR26AppsAddDbContext> options) : DbContext(options)
{

    #region + 참고 패키지 설치
    // Install-Package Dapper -Version 2.1.66
    // Install-Package Microsoft.Data.SqlClient -Version 5.2.2
    // Install-Package Microsoft.EntityFrameworkCore -Version 9.0.0
    // Install-Package Microsoft.EntityFrameworkCore.SqlServer -Version 9.0.0
    // Install-Package Microsoft.EntityFrameworkCore.Tools -Version 9.0.0
    // Install-Package System.Configuration.ConfigurationManager -Version 9.0.0
    #endregion

    // === Phase 2-1: 기본 마스터 데이터 (3개) ===
    public DbSet<UserDb> UserDb { get; set; } = null!;
    public DbSet<EDepartmentDb> EDepartmentDb { get; set; } = null!;
    public DbSet<ERankDb> ERankDb { get; set; } = null!;

    // === Phase 2-2: 평가 핵심 테이블 (4개) ===
    public DbSet<ProcessDb> ProcessDb { get; set; } = null!;
    public DbSet<ReportDb> ReportDb { get; set; } = null!;
    public DbSet<TotalReportDb> TotalReportDb { get; set; } = null!;
    public DbSet<EvaluationUsers.EvaluationUsers> EvaluationUsers { get; set; } = null!;

    // === Phase 2-3: 목표/협의/업무 모델 (5개) ===
    public DbSet<DeptObjectiveDb> DeptObjectiveDb { get; set; } = null!;
    public DbSet<AgreementDb> AgreementDb { get; set; } = null!;
    public DbSet<SubAgreementDb> SubAgreementDb { get; set; } = null!;
    public DbSet<TasksDb> TasksDb { get; set; } = null!;
    public DbSet<EvaluationLists.EvaluationLists> EvaluationLists { get; set; } = null!;

    // === Phase 2-4: 5개 뷰 ===
    public DbSet<v_MemberListDB> v_MemberListDB { get; set; } = null!;
    public DbSet<v_DeptObjectiveListDb> v_DeptObjectiveListDb { get; set; } = null!;
    public DbSet<v_ProcessTRListDB> v_ProcessTRListDB { get; set; } = null!;
    public DbSet<v_ReportTaskListDB> v_ReportTaskListDB { get; set; } = null!;
    public DbSet<v_TotalReportListDB> v_TotalReportListDB { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // === Phase 2-1: 테이블 매핑 ===
        modelBuilder.Entity<UserDb>().ToTable("UserDb");
        modelBuilder.Entity<EDepartmentDb>().ToTable("EDepartmentDb");
        modelBuilder.Entity<ERankDb>().ToTable("ERankDb");

        // === Phase 2-2: 평가 핵심 테이블 매핑 ===
        modelBuilder.Entity<ProcessDb>().ToTable("ProcessDb");
        modelBuilder.Entity<ReportDb>().ToTable("ReportDb");
        modelBuilder.Entity<TotalReportDb>().ToTable("TotalReportDb");
        modelBuilder.Entity<EvaluationUsers.EvaluationUsers>().ToTable("EvaluationUsers");

        // === Phase 2-3: 목표/협의/업무 모델 매핑 ===
        modelBuilder.Entity<DeptObjectiveDb>().ToTable("DeptObjectiveDb");
        modelBuilder.Entity<AgreementDb>().ToTable("AgreementDb");
        modelBuilder.Entity<SubAgreementDb>().ToTable("SubAgreementDb");
        modelBuilder.Entity<TasksDb>().ToTable("TasksDb");
        modelBuilder.Entity<EvaluationLists.EvaluationLists>().ToTable("EvaluationLists");

        // === Phase 2-1: BIT 타입 매핑 (UserDb.EStatus) ===
        //  EStatusDb 는 존재하지 않으므로 EStatus 필드를 명시적으로 할 필요는 없음
        // modelBuilder.Entity<UserDb>()
        //     .Property(u => u.EStatus)
        //     .HasColumnType("BIT");

        // === Phase 2-4: 뷰 매핑 ===
        modelBuilder.Entity<v_MemberListDB>().ToView("v_MemberListDB").HasNoKey();
        modelBuilder.Entity<v_DeptObjectiveListDb>().ToView("v_DeptObjectiveListDb").HasNoKey();
        modelBuilder.Entity<v_ProcessTRListDB>().ToView("v_ProcessTRListDB").HasNoKey();
        modelBuilder.Entity<v_ReportTaskListDB>().ToView("v_ReportTaskListDB").HasNoKey();
        modelBuilder.Entity<v_TotalReportListDB>().ToView("v_TotalReportListDB").HasNoKey();
        modelBuilder.Entity<v_EvaluationUsersList>().ToView("v_EvaluationUsersList").HasNoKey();
    }
}
