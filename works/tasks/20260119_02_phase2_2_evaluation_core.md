# 작업지시서: Phase 2-2 - 평가 핵심 모델 구현

**날짜**: 2026-01-19
**작업 유형**: 기능 추가 (Phase 2-2)
**관련 이슈**: [#008: Phase 2 Model 개발](../issues/008_phase2_model_development.md)
**관련 작업지시서**:
- `20260116_01_phase2_model_development.md` (Phase 2 전체 계획)
- `20260119_01_phase2_1_project_setup.md` (Phase 2-1 완료)

**수정 이력**:
- 2026-01-19 (v1): 초안 작성 - 2025년 비즈니스 로직 분석 기반
- 2026-01-19 (v2): 데이터 타입 통일 - long → Int64 (Phase 2-1 일관성 유지)

---

## 1. 작업 개요

### 1.1. Phase 2-2 목표
평가 프로세스의 핵심이 되는 4개 모델을 구현합니다.

### 1.2. 작업 범위
1. **ProcessDb**: 평가 프로세스 및 워크플로우 관리
2. **ReportDb**: 개별 평가 보고서 (세부 평가 항목별)
3. **TotalReportDb**: 종합 평가 결과 (최종 점수)
4. **EvaluationUsers**: 평가 참여자 관리 (평가자-피평가자 매핑)

### 1.3. 기술 스택
- **.NET**: 9.0
- **C#**: 13 (Primary Constructors, Raw String Literals)
- **ORM**: Dapper 2.1.66 (주), EF Core 9.0.0 (보조)
- **Database**: SQL Server
- **패턴**: Repository Pattern, Dependency Injection

---

## 2. 프로젝트 구조

### 2.1. 폴더 구조
```
MdcHR26Apps.Models/
├── EvaluationProcess/
│   ├── ProcessDb.cs                    # Entity
│   ├── IProcessRepository.cs           # Interface
│   └── ProcessRepository.cs            # Dapper 구현체
├── EvaluationReport/
│   ├── ReportDb.cs                     # Entity
│   ├── IReportRepository.cs            # Interface
│   └── ReportRepository.cs             # Dapper 구현체
├── Result/
│   ├── TotalReportDb.cs                # Entity
│   ├── ITotalReportRepository.cs       # Interface
│   └── TotalReportRepository.cs        # Dapper 구현체
├── EvaluationUsers/
│   ├── EvaluationUsers.cs              # Entity
│   ├── IEvaluationUsersRepository.cs   # Interface
│   └── EvaluationUsersRepository.cs    # Dapper 구현체
├── MdcHR26AppsAddDbContext.cs          # ← 4개 DbSet 추가
└── MdcHR26AppsAddExtensions.cs         # ← 4개 Repository 등록
```

### 2.2. Phase 2-2 개발 파일 목록 (12개)

| 번호 | 파일명 | 용도 | 비고 |
|------|--------|------|------|
| 1 | EvaluationProcess/ProcessDb.cs | 평가 프로세스 Entity | 15개 필드 |
| 2 | EvaluationProcess/IProcessRepository.cs | Interface | 15개 메서드 |
| 3 | EvaluationProcess/ProcessRepository.cs | Repository 구현 | Dapper + Primary Constructor |
| 4 | EvaluationReport/ReportDb.cs | 평가 보고서 Entity | 22개 필드 |
| 5 | EvaluationReport/IReportRepository.cs | Interface | 6개 메서드 |
| 6 | EvaluationReport/ReportRepository.cs | Repository 구현 | Dapper + Primary Constructor |
| 7 | Result/TotalReportDb.cs | 종합 결과 Entity | 21개 필드 |
| 8 | Result/ITotalReportRepository.cs | Interface | 6개 메서드 |
| 9 | Result/TotalReportRepository.cs | Repository 구현 | Dapper + Primary Constructor |
| 10 | EvaluationUsers/EvaluationUsers.cs | 평가 참여자 Entity | 7개 필드 |
| 11 | EvaluationUsers/IEvaluationUsersRepository.cs | Interface | 9개 메서드 |
| 12 | EvaluationUsers/EvaluationUsersRepository.cs | Repository 구현 | Dapper + Primary Constructor |

**추가 수정**:
- `MdcHR26AppsAddDbContext.cs`: 4개 DbSet 추가
- `MdcHR26AppsAddExtensions.cs`: 4개 Repository DI 등록

---

## 3. 단계별 작업 내용

### 3.1. ProcessDb Entity 및 Repository

#### 3.1.1. Entity (ProcessDb.cs)

**파일**: `EvaluationProcess/ProcessDb.cs`

**DB 스키마 기반** (2026년):
- PK: `Pid` (BIGINT IDENTITY)
- FK: `Uid`, `TeamLeaderId`, `DirectorId` → `UserDb.Uid`

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.EvaluationProcess;

/// <summary>
/// 평가 프로세스 Entity
/// 평가 단계별 워크플로우 상태 관리
/// </summary>
[Table("ProcessDb")]
public class ProcessDb
{
    /// <summary>
    /// Process ID (PK, IDENTITY)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 Pid { get; set; }

    /// <summary>
    /// 평가 대상자 ID (FK → UserDb.Uid)
    /// </summary>
    [Required]
    public Int64 Uid { get; set; }

    /// <summary>
    /// 부서장 ID (FK → UserDb.Uid)
    /// </summary>
    public Int64? TeamLeaderId { get; set; }

    /// <summary>
    /// 임원 ID (FK → UserDb.Uid)
    /// </summary>
    public Int64? DirectorId { get; set; }

    /// <summary>
    /// 직무평가 합의 요청 여부
    /// </summary>
    [Required]
    public bool Is_Request { get; set; } = false;

    /// <summary>
    /// 직무평가 합의 여부
    /// </summary>
    [Required]
    public bool Is_Agreement { get; set; } = false;

    /// <summary>
    /// 직무평가 합의 코멘트
    /// </summary>
    public string? Agreement_Comment { get; set; }

    /// <summary>
    /// 하위(세부) 직무평가 합의 요청 여부
    /// </summary>
    [Required]
    public bool Is_SubRequest { get; set; } = false;

    /// <summary>
    /// 하위(세부) 직무평가 합의 여부
    /// </summary>
    [Required]
    public bool Is_SubAgreement { get; set; } = false;

    /// <summary>
    /// 하위(세부) 직무평가 합의 코멘트
    /// </summary>
    public string? SubAgreement_Comment { get; set; }

    /// <summary>
    /// 사용자 평가 제출 여부
    /// </summary>
    [Required]
    public bool Is_User_Submission { get; set; } = false;

    /// <summary>
    /// 부서장 평가 제출 여부
    /// </summary>
    [Required]
    public bool Is_Teamleader_Submission { get; set; } = false;

    /// <summary>
    /// 임원 평가 제출 여부
    /// </summary>
    [Required]
    public bool Is_Director_Submission { get; set; } = false;

    /// <summary>
    /// 피드백 여부
    /// </summary>
    [Required]
    public bool FeedBackStatus { get; set; } = false;

    /// <summary>
    /// 평가자 피드백 승인 여부
    /// </summary>
    [Required]
    public bool FeedBack_Submission { get; set; } = false;

    // Navigation Properties
    [ForeignKey("Uid")]
    public UserDb? User { get; set; }

    [ForeignKey("TeamLeaderId")]
    public UserDb? TeamLeader { get; set; }

    [ForeignKey("DirectorId")]
    public UserDb? Director { get; set; }
}
```

**2025년 대비 변경사항**:
- `UserId` (string) → `Uid` (long) FK 참조
- `UserName` 필드 제거 (View를 통해 조회)
- `TeamLeader_Id/Name` → `TeamLeaderId` (long?) FK 참조
- `Director_Id/Name` → `DirectorId` (long?) FK 참조

#### 3.1.2. Interface (IProcessRepository.cs)

**파일**: `EvaluationProcess/IProcessRepository.cs`

```csharp
namespace MdcHR26Apps.Models.EvaluationProcess;

/// <summary>
/// ProcessDb Repository Interface
/// </summary>
public interface IProcessRepository : IDisposable
{
    // === 기본 CRUD ===

    /// <summary>
    /// 평가 프로세스 추가
    /// </summary>
    Task<Int64> AddAsync(ProcessDb model);

    /// <summary>
    /// 전체 프로세스 목록 조회
    /// </summary>
    Task<IEnumerable<ProcessDb>> GetByAllAsync();

    /// <summary>
    /// ID로 프로세스 조회
    /// </summary>
    Task<ProcessDb?> GetByIdAsync(Int64 id);

    /// <summary>
    /// 프로세스 정보 수정
    /// </summary>
    Task<int> UpdateAsync(ProcessDb model);

    /// <summary>
    /// 프로세스 삭제
    /// </summary>
    Task<int> DeleteAsync(Int64 id);

    // === 비즈니스 특화 메서드 ===

    /// <summary>
    /// 사용자 ID로 프로세스 조회
    /// </summary>
    Task<ProcessDb?> GetByUidAsync(Int64 uid);

    /// <summary>
    /// 합의 요청 여부 확인
    /// </summary>
    Task<bool> IsRequestCheckAsync(Int64 uid);

    /// <summary>
    /// 합의 완료 여부 확인
    /// </summary>
    Task<bool> IsAgreementCheckAsync(Int64 uid);

    /// <summary>
    /// 평가 제출 여부 확인
    /// </summary>
    Task<bool> IsReportSubmissionCheckAsync(Int64 uid);

    /// <summary>
    /// 사용자 존재 여부 확인
    /// </summary>
    Task<bool> CheckUidAsync(Int64 uid);

    /// <summary>
    /// 부서장 관할 팀원 목록 조회
    /// </summary>
    Task<IEnumerable<ProcessDb>> GetByTeamLeaderIdAsync(Int64 teamLeaderId);

    /// <summary>
    /// 부서장 관할 중 사용자 제출 완료 목록
    /// </summary>
    Task<IEnumerable<ProcessDb>> GetByTeamLeaderIdWithUserSubmissionAsync(Int64 teamLeaderId);

    /// <summary>
    /// 임원 관할 팀원 목록 조회
    /// </summary>
    Task<IEnumerable<ProcessDb>> GetByDirectorIdAsync(Int64 directorId);

    /// <summary>
    /// 임원 관할 중 부서장 제출 완료 목록
    /// </summary>
    Task<IEnumerable<ProcessDb>> GetByDirectorIdWithTeamleaderSubmissionAsync(Int64 directorId);

    /// <summary>
    /// 부서장 평가 제출 완료 목록
    /// </summary>
    Task<IEnumerable<ProcessDb>> GetByTeamLeaderIdWithTeamLeaderSubmissionAsync(Int64 teamLeaderId);
}
```

**2025년 대비 변경사항**:
- 매개변수: `string userId` → `long uid`
- 메서드명: `GetByUserIdAsync` → `GetByUidAsync`
- 메서드명: `CheckUserIdAsync` → `CheckUidAsync`

#### 3.1.3. Repository 구현 (ProcessRepository.cs)

**파일**: `EvaluationProcess/ProcessRepository.cs`

**주요 개선사항** (2025 → 2026):
1. Boolean 비교: `'true'` → `1` 또는 `@Is_Request`
2. Primary Constructor 적용
3. Raw String Literals 사용
4. Uid 기반 FK 참조

```csharp
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MdcHR26Apps.Models.EvaluationProcess;

/// <summary>
/// ProcessDb Repository 구현 (Dapper)
/// Primary Constructor 사용 (C# 13)
/// </summary>
public class ProcessRepository(string connectionString, ILoggerFactory loggerFactory) : IProcessRepository
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<ProcessRepository>();
    private readonly string dbContext = connectionString;

    #region + [1] 입력: AddAsync
    /// <summary>
    /// 평가 프로세스 추가
    /// </summary>
    public async Task<long> AddAsync(ProcessDb model)
    {
        const string sql = """
            INSERT INTO ProcessDb(
                Uid, TeamLeaderId, DirectorId,
                Is_Request, Is_Agreement, Agreement_Comment,
                Is_SubRequest, Is_SubAgreement, SubAgreement_Comment,
                Is_User_Submission, Is_Teamleader_Submission, Is_Director_Submission,
                FeedBackStatus, FeedBack_Submission)
            VALUES(
                @Uid, @TeamLeaderId, @DirectorId,
                @Is_Request, @Is_Agreement, @Agreement_Comment,
                @Is_SubRequest, @Is_SubAgreement, @SubAgreement_Comment,
                @Is_User_Submission, @Is_Teamleader_Submission, @Is_Director_Submission,
                @FeedBackStatus, @FeedBack_Submission);
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteScalarAsync<Int64>(sql, model);
    }
    #endregion

    #region + [2] 출력: GetByAllAsync
    /// <summary>
    /// 전체 프로세스 목록 조회
    /// </summary>
    public async Task<IEnumerable<ProcessDb>> GetByAllAsync()
    {
        const string sql = "SELECT * FROM ProcessDb ORDER BY Pid";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<ProcessDb>(sql);
    }
    #endregion

    #region + [3] 상세: GetByIdAsync
    /// <summary>
    /// ID로 프로세스 조회
    /// </summary>
    public async Task<ProcessDb?> GetByIdAsync(Int64 id)
    {
        const string sql = "SELECT * FROM ProcessDb WHERE Pid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<ProcessDb>(sql, new { id });
    }
    #endregion

    #region + [4] 수정: UpdateAsync
    /// <summary>
    /// 프로세스 정보 수정
    /// </summary>
    public async Task<int> UpdateAsync(ProcessDb model)
    {
        const string sql = """
            UPDATE ProcessDb
            SET
                Uid = @Uid,
                TeamLeaderId = @TeamLeaderId,
                DirectorId = @DirectorId,
                Is_Request = @Is_Request,
                Is_Agreement = @Is_Agreement,
                Agreement_Comment = @Agreement_Comment,
                Is_SubRequest = @Is_SubRequest,
                Is_SubAgreement = @Is_SubAgreement,
                SubAgreement_Comment = @SubAgreement_Comment,
                Is_User_Submission = @Is_User_Submission,
                Is_Teamleader_Submission = @Is_Teamleader_Submission,
                Is_Director_Submission = @Is_Director_Submission,
                FeedBackStatus = @FeedBackStatus,
                FeedBack_Submission = @FeedBack_Submission
            WHERE Pid = @Pid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, model);
    }
    #endregion

    #region + [5] 삭제: DeleteAsync
    /// <summary>
    /// 프로세스 삭제
    /// </summary>
    public async Task<int> DeleteAsync(Int64 id)
    {
        const string sql = "DELETE FROM ProcessDb WHERE Pid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, new { id });
    }
    #endregion

    #region + [6] 사용자별 조회: GetByUidAsync
    /// <summary>
    /// 사용자 ID로 프로세스 조회
    /// </summary>
    public async Task<ProcessDb?> GetByUidAsync(Int64 uid)
    {
        const string sql = "SELECT * FROM ProcessDb WHERE Uid = @uid";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<ProcessDb>(sql, new { uid });
    }
    #endregion

    #region + [7] 합의 요청 확인: IsRequestCheckAsync
    /// <summary>
    /// 합의 요청 여부 확인
    /// 개선: Boolean 비교 수정 ('true' → 1)
    /// </summary>
    public async Task<bool> IsRequestCheckAsync(Int64 uid)
    {
        const string sql = """
            SELECT COUNT(*)
            FROM ProcessDb
            WHERE Uid = @uid AND Is_Request = 1
            """;

        using var connection = new SqlConnection(dbContext);
        int count = await connection.ExecuteScalarAsync<int>(sql, new { uid });
        return count > 0;
    }
    #endregion

    #region + [8] 합의 완료 확인: IsAgreementCheckAsync
    /// <summary>
    /// 합의 완료 여부 확인
    /// 개선: Boolean 비교 수정 ('true' → 1)
    /// </summary>
    public async Task<bool> IsAgreementCheckAsync(Int64 uid)
    {
        const string sql = """
            SELECT COUNT(*)
            FROM ProcessDb
            WHERE Uid = @uid AND Is_Agreement = 1
            """;

        using var connection = new SqlConnection(dbContext);
        int count = await connection.ExecuteScalarAsync<int>(sql, new { uid });
        return count > 0;
    }
    #endregion

    #region + [9] 평가 제출 확인: IsReportSubmissionCheckAsync
    /// <summary>
    /// 평가 제출 여부 확인
    /// 개선: Boolean 비교 수정 ('true' → 1)
    /// </summary>
    public async Task<bool> IsReportSubmissionCheckAsync(Int64 uid)
    {
        const string sql = """
            SELECT COUNT(*)
            FROM ProcessDb
            WHERE Uid = @uid AND Is_User_Submission = 1
            """;

        using var connection = new SqlConnection(dbContext);
        int count = await connection.ExecuteScalarAsync<int>(sql, new { uid });
        return count > 0;
    }
    #endregion

    #region + [10] 사용자 존재 확인: CheckUidAsync
    /// <summary>
    /// 사용자 존재 여부 확인
    /// </summary>
    public async Task<bool> CheckUidAsync(long uid)
    {
        const string sql = "SELECT COUNT(*) FROM ProcessDb WHERE Uid = @uid";

        using var connection = new SqlConnection(dbContext);
        int count = await connection.ExecuteScalarAsync<int>(sql, new { uid });
        return count > 0;
    }
    #endregion

    #region + [11] 부서장 팀원 목록: GetByTeamLeaderIdAsync
    /// <summary>
    /// 부서장 관할 팀원 목록 조회
    /// </summary>
    public async Task<IEnumerable<ProcessDb>> GetByTeamLeaderIdAsync(Int64 teamLeaderId)
    {
        const string sql = """
            SELECT * FROM ProcessDb
            WHERE TeamLeaderId = @teamLeaderId
            ORDER BY Pid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<ProcessDb>(sql, new { teamLeaderId });
    }
    #endregion

    #region + [12] 부서장 팀원 중 제출 완료: GetByTeamLeaderIdWithUserSubmissionAsync
    /// <summary>
    /// 부서장 관할 중 사용자 제출 완료 목록
    /// </summary>
    public async Task<IEnumerable<ProcessDb>> GetByTeamLeaderIdWithUserSubmissionAsync(Int64 teamLeaderId)
    {
        const string sql = """
            SELECT * FROM ProcessDb
            WHERE TeamLeaderId = @teamLeaderId
              AND Is_User_Submission = 1
            ORDER BY Pid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<ProcessDb>(sql, new { teamLeaderId });
    }
    #endregion

    #region + [13] 임원 관할 목록: GetByDirectorIdAsync
    /// <summary>
    /// 임원 관할 팀원 목록 조회
    /// </summary>
    public async Task<IEnumerable<ProcessDb>> GetByDirectorIdAsync(Int64 directorId)
    {
        const string sql = """
            SELECT * FROM ProcessDb
            WHERE DirectorId = @directorId
            ORDER BY Pid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<ProcessDb>(sql, new { directorId });
    }
    #endregion

    #region + [14] 임원 관할 중 제출 완료: GetByDirectorIdWithTeamleaderSubmissionAsync
    /// <summary>
    /// 임원 관할 중 부서장 제출 완료 목록
    /// </summary>
    public async Task<IEnumerable<ProcessDb>> GetByDirectorIdWithTeamleaderSubmissionAsync(Int64 directorId)
    {
        const string sql = """
            SELECT * FROM ProcessDb
            WHERE DirectorId = @directorId
              AND Is_Teamleader_Submission = 1
            ORDER BY Pid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<ProcessDb>(sql, new { directorId });
    }
    #endregion

    #region + [15] 부서장 제출 완료 목록: GetByTeamLeaderIdWithTeamLeaderSubmissionAsync
    /// <summary>
    /// 부서장 평가 제출 완료 목록
    /// </summary>
    public async Task<IEnumerable<ProcessDb>> GetByTeamLeaderIdWithTeamLeaderSubmissionAsync(Int64 teamLeaderId)
    {
        const string sql = """
            SELECT * FROM ProcessDb
            WHERE TeamLeaderId = @teamLeaderId
              AND Is_Teamleader_Submission = 1
            ORDER BY Pid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<ProcessDb>(sql, new { teamLeaderId });
    }
    #endregion

    #region + [#] Dispose
    /// <summary>
    /// 리소스 해제
    /// </summary>
    public void Dispose()
    {
        db?.Dispose();
        GC.SuppressFinalize(this);
    }
    #endregion
}
```

**주요 개선사항**:
- ✅ Boolean 비교 로직 수정 (`'true'` → `1`)
- ✅ `string userId` → `long uid` 변경
- ✅ Primary Constructor 적용
- ✅ Raw String Literals 사용

---

### 3.2. ReportDb Entity 및 Repository

#### 3.2.1. Entity (ReportDb.cs)

**파일**: `EvaluationReport/ReportDb.cs`

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.EvaluationReport;

/// <summary>
/// 평가 보고서 Entity (개별 평가 항목)
/// </summary>
[Table("ReportDb")]
public class ReportDb
{
    /// <summary>
    /// Report ID (PK, IDENTITY)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 Rid { get; set; }

    /// <summary>
    /// 평가 대상자 ID (FK → UserDb.Uid)
    /// </summary>
    [Required]
    public Int64 Uid { get; set; }

    /// <summary>
    /// 보고서 항목 번호
    /// </summary>
    [Required]
    public int Report_Item_Number { get; set; }

    /// <summary>
    /// 지표 분류명
    /// </summary>
    [Required]
    public string Report_Item_Name_1 { get; set; } = string.Empty;

    /// <summary>
    /// 직무 분류명
    /// </summary>
    [Required]
    public string Report_Item_Name_2 { get; set; } = string.Empty;

    /// <summary>
    /// 직무 비율(%)
    /// </summary>
    [Required]
    public int Report_Item_Proportion { get; set; }

    /// <summary>
    /// 세부 직무명
    /// </summary>
    [Required]
    public string Report_SubItem_Name { get; set; } = string.Empty;

    /// <summary>
    /// 세부 직무 비율(%)
    /// </summary>
    [Required]
    public int Report_SubItem_Proportion { get; set; }

    /// <summary>
    /// 하위 업무 리스트 번호 (TasksDb 연결)
    /// </summary>
    [Required]
    public Int64 Task_Number { get; set; }

    // === 평가대상자 평가 ===

    /// <summary>
    /// 사용자 평가 - 일정준수
    /// </summary>
    [Required]
    public double User_Evaluation_1 { get; set; }

    /// <summary>
    /// 사용자 평가 - 업무 수행의 양
    /// </summary>
    [Required]
    public double User_Evaluation_2 { get; set; }

    /// <summary>
    /// 사용자 평가 - 결과물
    /// </summary>
    [Required]
    public double User_Evaluation_3 { get; set; }

    /// <summary>
    /// 사용자 평가 - 코멘트
    /// </summary>
    public string? User_Evaluation_4 { get; set; }

    // === 부서장/팀장 평가 ===

    /// <summary>
    /// 팀장 평가 - 일정준수
    /// </summary>
    [Required]
    public double TeamLeader_Evaluation_1 { get; set; }

    /// <summary>
    /// 팀장 평가 - 업무 수행의 양
    /// </summary>
    [Required]
    public double TeamLeader_Evaluation_2 { get; set; }

    /// <summary>
    /// 팀장 평가 - 결과물
    /// </summary>
    [Required]
    public double TeamLeader_Evaluation_3 { get; set; }

    /// <summary>
    /// 팀장 평가 - 코멘트
    /// </summary>
    public string? TeamLeader_Evaluation_4 { get; set; }

    // === 임원 평가 ===

    /// <summary>
    /// 임원 평가 - 일정준수
    /// </summary>
    [Required]
    public double Director_Evaluation_1 { get; set; }

    /// <summary>
    /// 임원 평가 - 업무 수행의 양
    /// </summary>
    [Required]
    public double Director_Evaluation_2 { get; set; }

    /// <summary>
    /// 임원 평가 - 결과물
    /// </summary>
    [Required]
    public double Director_Evaluation_3 { get; set; }

    /// <summary>
    /// 임원 평가 - 코멘트
    /// </summary>
    public string? Director_Evaluation_4 { get; set; }

    /// <summary>
    /// 종합 점수
    /// </summary>
    [Required]
    public double Total_Score { get; set; }

    // Navigation Property
    [ForeignKey("Uid")]
    public UserDb? User { get; set; }
}
```

**2025년 대비 변경사항**:
- `UserId` (string) → `Uid` (long) FK 참조
- `UserName` 필드 제거 (View를 통해 조회)

#### 3.2.2. Interface (IReportRepository.cs)

**파일**: `EvaluationReport/IReportRepository.cs`

```csharp
namespace MdcHR26Apps.Models.EvaluationReport;

/// <summary>
/// ReportDb Repository Interface
/// </summary>
public interface IReportRepository : IDisposable
{
    // === 기본 CRUD ===

    /// <summary>
    /// 평가 보고서 추가
    /// </summary>
    Task<Int64> AddAsync(ReportDb model);

    /// <summary>
    /// 전체 보고서 목록 조회
    /// </summary>
    Task<IEnumerable<ReportDb>> GetByAllAsync();

    /// <summary>
    /// ID로 보고서 조회
    /// </summary>
    Task<ReportDb?> GetByIdAsync(Int64 id);

    /// <summary>
    /// 보고서 수정
    /// </summary>
    Task<int> UpdateAsync(ReportDb model);

    /// <summary>
    /// 보고서 삭제
    /// </summary>
    Task<int> DeleteAsync(Int64 id);

    /// <summary>
    /// 사용자별 보고서 목록 조회
    /// </summary>
    Task<IEnumerable<ReportDb>> GetByUidAllAsync(Int64 uid);
}
```

**2025년 대비 변경사항**:
- 메서드명: `GetByUserIdAllAsync` → `GetByUidAllAsync`
- 매개변수: `string UserId` → `long uid`

#### 3.2.3. Repository 구현 (ReportRepository.cs)

**파일**: `EvaluationReport/ReportRepository.cs`

```csharp
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MdcHR26Apps.Models.EvaluationReport;

/// <summary>
/// ReportDb Repository 구현 (Dapper)
/// Primary Constructor 사용 (C# 13)
/// </summary>
public class ReportRepository(string connectionString, ILoggerFactory loggerFactory) : IReportRepository
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<ReportRepository>();
    private readonly string dbContext = connectionString;

    #region + [1] 입력: AddAsync
    /// <summary>
    /// 평가 보고서 추가
    /// </summary>
    public async Task<long> AddAsync(ReportDb model)
    {
        const string sql = """
            INSERT INTO ReportDb(
                Uid, Report_Item_Number, Report_Item_Name_1, Report_Item_Name_2,
                Report_Item_Proportion, Report_SubItem_Name, Report_SubItem_Proportion,
                Task_Number,
                User_Evaluation_1, User_Evaluation_2, User_Evaluation_3, User_Evaluation_4,
                TeamLeader_Evaluation_1, TeamLeader_Evaluation_2, TeamLeader_Evaluation_3, TeamLeader_Evaluation_4,
                Director_Evaluation_1, Director_Evaluation_2, Director_Evaluation_3, Director_Evaluation_4,
                Total_Score)
            VALUES(
                @Uid, @Report_Item_Number, @Report_Item_Name_1, @Report_Item_Name_2,
                @Report_Item_Proportion, @Report_SubItem_Name, @Report_SubItem_Proportion,
                @Task_Number,
                @User_Evaluation_1, @User_Evaluation_2, @User_Evaluation_3, @User_Evaluation_4,
                @TeamLeader_Evaluation_1, @TeamLeader_Evaluation_2, @TeamLeader_Evaluation_3, @TeamLeader_Evaluation_4,
                @Director_Evaluation_1, @Director_Evaluation_2, @Director_Evaluation_3, @Director_Evaluation_4,
                @Total_Score);
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteScalarAsync<Int64>(sql, model);
    }
    #endregion

    #region + [2] 출력: GetByAllAsync
    /// <summary>
    /// 전체 보고서 목록 조회
    /// </summary>
    public async Task<IEnumerable<ReportDb>> GetByAllAsync()
    {
        const string sql = "SELECT * FROM ReportDb ORDER BY Rid";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<ReportDb>(sql);
    }
    #endregion

    #region + [3] 상세: GetByIdAsync
    /// <summary>
    /// ID로 보고서 조회
    /// </summary>
    public async Task<ReportDb?> GetByIdAsync(Int64 id)
    {
        const string sql = "SELECT * FROM ReportDb WHERE Rid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<ReportDb>(sql, new { id });
    }
    #endregion

    #region + [4] 수정: UpdateAsync
    /// <summary>
    /// 보고서 수정
    /// </summary>
    public async Task<int> UpdateAsync(ReportDb model)
    {
        const string sql = """
            UPDATE ReportDb
            SET
                Uid = @Uid,
                Report_Item_Number = @Report_Item_Number,
                Report_Item_Name_1 = @Report_Item_Name_1,
                Report_Item_Name_2 = @Report_Item_Name_2,
                Report_Item_Proportion = @Report_Item_Proportion,
                Report_SubItem_Name = @Report_SubItem_Name,
                Report_SubItem_Proportion = @Report_SubItem_Proportion,
                Task_Number = @Task_Number,
                User_Evaluation_1 = @User_Evaluation_1,
                User_Evaluation_2 = @User_Evaluation_2,
                User_Evaluation_3 = @User_Evaluation_3,
                User_Evaluation_4 = @User_Evaluation_4,
                TeamLeader_Evaluation_1 = @TeamLeader_Evaluation_1,
                TeamLeader_Evaluation_2 = @TeamLeader_Evaluation_2,
                TeamLeader_Evaluation_3 = @TeamLeader_Evaluation_3,
                TeamLeader_Evaluation_4 = @TeamLeader_Evaluation_4,
                Director_Evaluation_1 = @Director_Evaluation_1,
                Director_Evaluation_2 = @Director_Evaluation_2,
                Director_Evaluation_3 = @Director_Evaluation_3,
                Director_Evaluation_4 = @Director_Evaluation_4,
                Total_Score = @Total_Score
            WHERE Rid = @Rid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, model);
    }
    #endregion

    #region + [5] 삭제: DeleteAsync
    /// <summary>
    /// 보고서 삭제
    /// </summary>
    public async Task<int> DeleteAsync(Int64 id)
    {
        const string sql = "DELETE FROM ReportDb WHERE Rid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, new { id });
    }
    #endregion

    #region + [6] 사용자별 조회: GetByUidAllAsync
    /// <summary>
    /// 사용자별 보고서 목록 조회
    /// </summary>
    public async Task<IEnumerable<ReportDb>> GetByUidAllAsync(Int64 uid)
    {
        const string sql = """
            SELECT * FROM ReportDb
            WHERE Uid = @uid
            ORDER BY Rid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<ReportDb>(sql, new { uid });
    }
    #endregion

    #region + [#] Dispose
    /// <summary>
    /// 리소스 해제
    /// </summary>
    public void Dispose()
    {
        db?.Dispose();
        GC.SuppressFinalize(this);
    }
    #endregion
}
```

---

### 3.3. TotalReportDb Entity 및 Repository

#### 3.3.1. Entity (TotalReportDb.cs)

**파일**: `Result/TotalReportDb.cs`

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.Result;

/// <summary>
/// 종합 평가 결과 Entity
/// </summary>
[Table("TotalReportDb")]
public class TotalReportDb
{
    /// <summary>
    /// TotalReport ID (PK, IDENTITY)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 TRid { get; set; }

    /// <summary>
    /// 평가 대상자 ID (FK → UserDb.Uid)
    /// </summary>
    [Required]
    public Int64 Uid { get; set; }

    // === 평가대상자 자가평가 ===

    /// <summary>
    /// 사용자 평가 - 일정준수
    /// </summary>
    [Required]
    public double User_Evaluation_1 { get; set; }

    /// <summary>
    /// 사용자 평가 - 업무 수행의 양
    /// </summary>
    [Required]
    public double User_Evaluation_2 { get; set; }

    /// <summary>
    /// 사용자 평가 - 결과물
    /// </summary>
    [Required]
    public double User_Evaluation_3 { get; set; }

    /// <summary>
    /// 사용자 평가 - 코멘트
    /// </summary>
    public string? User_Evaluation_4 { get; set; }

    // === 부서장(팀장) 평가 ===

    /// <summary>
    /// 팀장 평가 - 일정준수
    /// </summary>
    [Required]
    public double TeamLeader_Evaluation_1 { get; set; }

    /// <summary>
    /// 팀장 평가 - 업무 수행의 양
    /// </summary>
    [Required]
    public double TeamLeader_Evaluation_2 { get; set; }

    /// <summary>
    /// 팀장 평가 - 결과물
    /// </summary>
    [Required]
    public double TeamLeader_Evaluation_3 { get; set; }

    /// <summary>
    /// 팀장 평가 - 코멘트
    /// </summary>
    public string? TeamLeader_Comment { get; set; }

    // === 1차 피드백 면담 ===

    /// <summary>
    /// 피드백 - 일정준수
    /// </summary>
    [Required]
    public double Feedback_Evaluation_1 { get; set; }

    /// <summary>
    /// 피드백 - 업무 수행의 양
    /// </summary>
    [Required]
    public double Feedback_Evaluation_2 { get; set; }

    /// <summary>
    /// 피드백 - 결과물
    /// </summary>
    [Required]
    public double Feedback_Evaluation_3 { get; set; }

    /// <summary>
    /// 피드백 - 코멘트
    /// </summary>
    public string? Feedback_Comment { get; set; }

    // === 임원 평가 ===

    /// <summary>
    /// 임원 평가 - 일정준수
    /// </summary>
    [Required]
    public double Director_Evaluation_1 { get; set; }

    /// <summary>
    /// 임원 평가 - 업무 수행의 양
    /// </summary>
    [Required]
    public double Director_Evaluation_2 { get; set; }

    /// <summary>
    /// 임원 평가 - 결과물
    /// </summary>
    [Required]
    public double Director_Evaluation_3 { get; set; }

    /// <summary>
    /// 임원 평가 - 코멘트
    /// </summary>
    public string? Director_Comment { get; set; }

    // === 종합 점수 ===

    /// <summary>
    /// 종합 점수
    /// </summary>
    [Required]
    public double Total_Score { get; set; }

    /// <summary>
    /// 임원 점수
    /// </summary>
    [Required]
    public double Director_Score { get; set; }

    /// <summary>
    /// 팀장 점수
    /// </summary>
    [Required]
    public double TeamLeader_Score { get; set; } = 0;

    // Navigation Property
    [ForeignKey("Uid")]
    public UserDb? User { get; set; }
}
```

#### 3.3.2. Interface (ITotalReportRepository.cs)

**파일**: `Result/ITotalReportRepository.cs`

```csharp
namespace MdcHR26Apps.Models.Result;

/// <summary>
/// TotalReportDb Repository Interface
/// </summary>
public interface ITotalReportRepository : IDisposable
{
    // === 기본 CRUD ===

    /// <summary>
    /// 종합 평가 추가
    /// </summary>
    Task<Int64> AddAsync(TotalReportDb model);

    /// <summary>
    /// 전체 종합 평가 목록 조회
    /// </summary>
    Task<IEnumerable<TotalReportDb>> GetByAllAsync();

    /// <summary>
    /// ID로 종합 평가 조회
    /// </summary>
    Task<TotalReportDb?> GetByIdAsync(Int64 id);

    /// <summary>
    /// 종합 평가 수정
    /// </summary>
    Task<int> UpdateAsync(TotalReportDb model);

    /// <summary>
    /// 종합 평가 삭제
    /// </summary>
    Task<int> DeleteAsync(Int64 id);

    /// <summary>
    /// 사용자별 종합 평가 조회
    /// </summary>
    Task<TotalReportDb?> GetByUidAsync(Int64 uid);
}
```

**2025년 대비 변경사항**:
- 메서드명: `GetByUserIdAsync` → `GetByUidAsync`
- 매개변수: `long UId` → `long uid` (일관성)

#### 3.3.3. Repository 구현 (TotalReportRepository.cs)

**파일**: `Result/TotalReportRepository.cs`

```csharp
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MdcHR26Apps.Models.Result;

/// <summary>
/// TotalReportDb Repository 구현 (Dapper)
/// Primary Constructor 사용 (C# 13)
/// </summary>
public class TotalReportRepository(string connectionString, ILoggerFactory loggerFactory) : ITotalReportRepository
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<TotalReportRepository>();
    private readonly string dbContext = connectionString;

    #region + [1] 입력: AddAsync
    /// <summary>
    /// 종합 평가 추가
    /// </summary>
    public async Task<long> AddAsync(TotalReportDb model)
    {
        const string sql = """
            INSERT INTO TotalReportDb(
                Uid,
                User_Evaluation_1, User_Evaluation_2, User_Evaluation_3, User_Evaluation_4,
                TeamLeader_Evaluation_1, TeamLeader_Evaluation_2, TeamLeader_Evaluation_3, TeamLeader_Comment,
                Feedback_Evaluation_1, Feedback_Evaluation_2, Feedback_Evaluation_3, Feedback_Comment,
                Director_Evaluation_1, Director_Evaluation_2, Director_Evaluation_3, Director_Comment,
                Total_Score, Director_Score, TeamLeader_Score)
            VALUES(
                @Uid,
                @User_Evaluation_1, @User_Evaluation_2, @User_Evaluation_3, @User_Evaluation_4,
                @TeamLeader_Evaluation_1, @TeamLeader_Evaluation_2, @TeamLeader_Evaluation_3, @TeamLeader_Comment,
                @Feedback_Evaluation_1, @Feedback_Evaluation_2, @Feedback_Evaluation_3, @Feedback_Comment,
                @Director_Evaluation_1, @Director_Evaluation_2, @Director_Evaluation_3, @Director_Comment,
                @Total_Score, @Director_Score, @TeamLeader_Score);
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteScalarAsync<Int64>(sql, model);
    }
    #endregion

    #region + [2] 출력: GetByAllAsync
    /// <summary>
    /// 전체 종합 평가 목록 조회
    /// </summary>
    public async Task<IEnumerable<TotalReportDb>> GetByAllAsync()
    {
        const string sql = "SELECT * FROM TotalReportDb ORDER BY TRid";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<TotalReportDb>(sql);
    }
    #endregion

    #region + [3] 상세: GetByIdAsync
    /// <summary>
    /// ID로 종합 평가 조회
    /// </summary>
    public async Task<TotalReportDb?> GetByIdAsync(Int64 id)
    {
        const string sql = "SELECT * FROM TotalReportDb WHERE TRid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<TotalReportDb>(sql, new { id });
    }
    #endregion

    #region + [4] 수정: UpdateAsync
    /// <summary>
    /// 종합 평가 수정
    /// </summary>
    public async Task<int> UpdateAsync(TotalReportDb model)
    {
        const string sql = """
            UPDATE TotalReportDb
            SET
                Uid = @Uid,
                User_Evaluation_1 = @User_Evaluation_1,
                User_Evaluation_2 = @User_Evaluation_2,
                User_Evaluation_3 = @User_Evaluation_3,
                User_Evaluation_4 = @User_Evaluation_4,
                TeamLeader_Evaluation_1 = @TeamLeader_Evaluation_1,
                TeamLeader_Evaluation_2 = @TeamLeader_Evaluation_2,
                TeamLeader_Evaluation_3 = @TeamLeader_Evaluation_3,
                TeamLeader_Comment = @TeamLeader_Comment,
                Feedback_Evaluation_1 = @Feedback_Evaluation_1,
                Feedback_Evaluation_2 = @Feedback_Evaluation_2,
                Feedback_Evaluation_3 = @Feedback_Evaluation_3,
                Feedback_Comment = @Feedback_Comment,
                Director_Evaluation_1 = @Director_Evaluation_1,
                Director_Evaluation_2 = @Director_Evaluation_2,
                Director_Evaluation_3 = @Director_Evaluation_3,
                Director_Comment = @Director_Comment,
                Total_Score = @Total_Score,
                Director_Score = @Director_Score,
                TeamLeader_Score = @TeamLeader_Score
            WHERE TRid = @TRid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, model);
    }
    #endregion

    #region + [5] 삭제: DeleteAsync
    /// <summary>
    /// 종합 평가 삭제
    /// </summary>
    public async Task<int> DeleteAsync(Int64 id)
    {
        const string sql = "DELETE FROM TotalReportDb WHERE TRid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, new { id });
    }
    #endregion

    #region + [6] 사용자별 조회: GetByUidAsync
    /// <summary>
    /// 사용자별 종합 평가 조회
    /// </summary>
    public async Task<TotalReportDb?> GetByUidAsync(Int64 uid)
    {
        const string sql = "SELECT * FROM TotalReportDb WHERE Uid = @uid";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<TotalReportDb>(sql, new { uid });
    }
    #endregion

    #region + [#] Dispose
    /// <summary>
    /// 리소스 해제
    /// </summary>
    public void Dispose()
    {
        db?.Dispose();
        GC.SuppressFinalize(this);
    }
    #endregion
}
```

---

### 3.4. EvaluationUsers Entity 및 Repository

#### 3.4.1. Entity (EvaluationUsers.cs)

**파일**: `EvaluationUsers/EvaluationUsers.cs`

**2026년 DB 스키마 기반**:
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.EvaluationUsers;

/// <summary>
/// 평가 참여자 Entity
/// </summary>
[Table("EvaluationUsers")]
public class EvaluationUsers
{
    /// <summary>
    /// Evaluation User ID (PK, IDENTITY)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 EUid { get; set; }

    /// <summary>
    /// 평가 대상자 ID (FK → UserDb.Uid)
    /// </summary>
    [Required]
    public Int64 Uid { get; set; }

    /// <summary>
    /// 평가자 여부
    /// </summary>
    [Required]
    public bool Is_Evaluation { get; set; } = true;

    /// <summary>
    /// 부서장 ID (FK → UserDb.Uid)
    /// </summary>
    public Int64? TeamLeaderId { get; set; }

    /// <summary>
    /// 임원 ID (FK → UserDb.Uid)
    /// </summary>
    public Int64? DirectorId { get; set; }

    /// <summary>
    /// 부서장 여부
    /// </summary>
    [Required]
    public bool Is_TeamLeader { get; set; } = false;

    // Navigation Properties
    [ForeignKey("Uid")]
    public UserDb? User { get; set; }

    [ForeignKey("TeamLeaderId")]
    public UserDb? TeamLeader { get; set; }

    [ForeignKey("DirectorId")]
    public UserDb? Director { get; set; }
}
```

**2025년 대비 변경사항**:
- `UserId`, `UserName` 제거 → `Uid` (long) FK 참조
- `TeamLeader_Id/Name` → `TeamLeaderId` (long?) FK 참조
- `Director_Id/Name` → `DirectorId` (long?) FK 참조
- `Is_TeamLeader` 필드 추가 (2026년 DB 스키마)

#### 3.4.2. Interface (IEvaluationUsersRepository.cs)

**파일**: `EvaluationUsers/IEvaluationUsersRepository.cs`

```csharp
namespace MdcHR26Apps.Models.EvaluationUsers;

/// <summary>
/// EvaluationUsers Repository Interface
/// </summary>
public interface IEvaluationUsersRepository : IDisposable
{
    // === 기본 CRUD ===

    /// <summary>
    /// 평가 참여자 추가
    /// </summary>
    Task<Int64> AddAsync(EvaluationUsers model);

    /// <summary>
    /// 전체 참여자 목록 조회
    /// </summary>
    Task<IEnumerable<EvaluationUsers>> GetByAllAsync();

    /// <summary>
    /// ID로 참여자 조회
    /// </summary>
    Task<EvaluationUsers?> GetByIdAsync(Int64 id);

    /// <summary>
    /// 참여자 정보 수정
    /// </summary>
    Task<int> UpdateAsync(EvaluationUsers model);

    /// <summary>
    /// 참여자 삭제
    /// </summary>
    Task<int> DeleteAsync(Int64 id);

    // === 비즈니스 특화 메서드 ===

    /// <summary>
    /// Uid 존재 여부 확인
    /// </summary>
    Task<bool> CheckUidAsync(Int64 uid);

    /// <summary>
    /// Uid로 참여자 조회
    /// </summary>
    Task<EvaluationUsers?> GetByUidAsync(Int64 uid);

    /// <summary>
    /// 부서장의 팀원 목록 조회
    /// </summary>
    Task<IEnumerable<EvaluationUsers>> GetByTeamLeaderIdAsync(Int64 teamLeaderId);

    /// <summary>
    /// 사용자 이름으로 검색 (부분 일치)
    /// 참고: View를 통한 조회 권장
    /// </summary>
    Task<IEnumerable<EvaluationUsers>> SearchByNameAsync(string userName);
}
```

**2025년 대비 변경사항**:
- 메서드명: `CheckUserIdAsync` → `CheckUidAsync`
- 메서드명: `GetByUserIdAsync` → `GetByUidAsync`
- 메서드명: `GetByUserNameAllAsync` → `SearchByNameAsync` (View 권장)
- 매개변수: `string userid` → `long uid`

#### 3.4.3. Repository 구현 (EvaluationUsersRepository.cs)

**파일**: `EvaluationUsers/EvaluationUsersRepository.cs`

```csharp
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MdcHR26Apps.Models.EvaluationUsers;

/// <summary>
/// EvaluationUsers Repository 구현 (Dapper)
/// Primary Constructor 사용 (C# 13)
/// </summary>
public class EvaluationUsersRepository(string connectionString, ILoggerFactory loggerFactory) : IEvaluationUsersRepository
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<EvaluationUsersRepository>();
    private readonly string dbContext = connectionString;

    #region + [1] 입력: AddAsync
    /// <summary>
    /// 평가 참여자 추가
    /// </summary>
    public async Task<long> AddAsync(EvaluationUsers model)
    {
        const string sql = """
            INSERT INTO EvaluationUsers(
                Uid, Is_Evaluation, TeamLeaderId, DirectorId, Is_TeamLeader)
            VALUES(
                @Uid, @Is_Evaluation, @TeamLeaderId, @DirectorId, @Is_TeamLeader);
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteScalarAsync<Int64>(sql, model);
    }
    #endregion

    #region + [2] 출력: GetByAllAsync
    /// <summary>
    /// 전체 참여자 목록 조회
    /// </summary>
    public async Task<IEnumerable<EvaluationUsers>> GetByAllAsync()
    {
        const string sql = "SELECT * FROM EvaluationUsers ORDER BY EUid";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<EvaluationUsers>(sql);
    }
    #endregion

    #region + [3] 상세: GetByIdAsync
    /// <summary>
    /// ID로 참여자 조회
    /// </summary>
    public async Task<EvaluationUsers?> GetByIdAsync(Int64 id)
    {
        const string sql = "SELECT * FROM EvaluationUsers WHERE EUid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<EvaluationUsers>(sql, new { id });
    }
    #endregion

    #region + [4] 수정: UpdateAsync
    /// <summary>
    /// 참여자 정보 수정
    /// </summary>
    public async Task<int> UpdateAsync(EvaluationUsers model)
    {
        const string sql = """
            UPDATE EvaluationUsers
            SET
                Uid = @Uid,
                Is_Evaluation = @Is_Evaluation,
                TeamLeaderId = @TeamLeaderId,
                DirectorId = @DirectorId,
                Is_TeamLeader = @Is_TeamLeader
            WHERE EUid = @EUid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, model);
    }
    #endregion

    #region + [5] 삭제: DeleteAsync
    /// <summary>
    /// 참여자 삭제
    /// </summary>
    public async Task<int> DeleteAsync(Int64 id)
    {
        const string sql = "DELETE FROM EvaluationUsers WHERE EUid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, new { id });
    }
    #endregion

    #region + [6] Uid 존재 확인: CheckUidAsync
    /// <summary>
    /// Uid 존재 여부 확인
    /// </summary>
    public async Task<bool> CheckUidAsync(long uid)
    {
        const string sql = "SELECT COUNT(*) FROM EvaluationUsers WHERE Uid = @uid";

        using var connection = new SqlConnection(dbContext);
        int count = await connection.ExecuteScalarAsync<int>(sql, new { uid });
        return count > 0;
    }
    #endregion

    #region + [7] Uid로 조회: GetByUidAsync
    /// <summary>
    /// Uid로 참여자 조회
    /// </summary>
    public async Task<EvaluationUsers?> GetByUidAsync(Int64 uid)
    {
        const string sql = "SELECT * FROM EvaluationUsers WHERE Uid = @uid";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<EvaluationUsers>(sql, new { uid });
    }
    #endregion

    #region + [8] 팀원 목록: GetByTeamLeaderIdAsync
    /// <summary>
    /// 부서장의 팀원 목록 조회
    /// </summary>
    public async Task<IEnumerable<EvaluationUsers>> GetByTeamLeaderIdAsync(Int64 teamLeaderId)
    {
        const string sql = """
            SELECT * FROM EvaluationUsers
            WHERE TeamLeaderId = @teamLeaderId
            ORDER BY EUid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<EvaluationUsers>(sql, new { teamLeaderId });
    }
    #endregion

    #region + [9] 이름 검색: SearchByNameAsync
    /// <summary>
    /// 사용자 이름으로 검색 (부분 일치)
    /// 참고: 실제로는 View (v_MemberListDB 등)를 통한 조회 권장
    /// </summary>
    public async Task<IEnumerable<EvaluationUsers>> SearchByNameAsync(string userName)
    {
        // 이 메서드는 UserDb와 JOIN 필요
        // View를 통한 조회 권장
        const string sql = """
            SELECT EU.*
            FROM EvaluationUsers EU
            INNER JOIN UserDb U ON EU.Uid = U.Uid
            WHERE U.UserName LIKE @userName + '%'
            ORDER BY EU.EUid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<EvaluationUsers>(sql, new { userName });
    }
    #endregion

    #region + [#] Dispose
    /// <summary>
    /// 리소스 해제
    /// </summary>
    public void Dispose()
    {
        db?.Dispose();
        GC.SuppressFinalize(this);
    }
    #endregion
}
```

**주요 변경사항**:
- ✅ `string userId` → `long uid` 변경
- ✅ `UserName` 검색은 UserDb JOIN 필요 (View 권장)
- ✅ Primary Constructor 적용
- ✅ Raw String Literals 사용

---

### 3.5. DbContext 및 Extensions 업데이트

#### 3.5.1. MdcHR26AppsAddDbContext.cs 수정

**파일**: `MdcHR26AppsAddDbContext.cs`

**수정 내용**:
```csharp
// === Phase 2-2: 평가 핵심 모델 (4개) 추가 ===
public DbSet<ProcessDb> ProcessDb { get; set; } = null!;
public DbSet<ReportDb> ReportDb { get; set; } = null!;
public DbSet<TotalReportDb> TotalReportDb { get; set; } = null!;
public DbSet<EvaluationUsers> EvaluationUsers { get; set; } = null!;
```

**OnModelCreating 메서드 수정**:
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // === Phase 2-1: 기본 마스터 데이터 (3개) ===
    modelBuilder.Entity<UserDb>().ToTable("UserDb");
    modelBuilder.Entity<EDepartmentDb>().ToTable("EDepartmentDb");
    modelBuilder.Entity<ERankDb>().ToTable("ERankDb");

    // === Phase 2-2: 평가 핵심 모델 (4개) 추가 ===
    modelBuilder.Entity<ProcessDb>().ToTable("ProcessDb");
    modelBuilder.Entity<ReportDb>().ToTable("ReportDb");
    modelBuilder.Entity<TotalReportDb>().ToTable("TotalReportDb");
    modelBuilder.Entity<EvaluationUsers>().ToTable("EvaluationUsers");
}
```

#### 3.5.2. MdcHR26AppsAddExtensions.cs 수정

**파일**: `MdcHR26AppsAddExtensions.cs`

**수정 내용**:
```csharp
// === Phase 2-2: Repository 등록 (4개) 추가 ===
services.AddScoped<IProcessRepository>(provider =>
    new ProcessRepository(connectionString, loggerFactory));

services.AddScoped<IReportRepository>(provider =>
    new ReportRepository(connectionString, loggerFactory));

services.AddScoped<ITotalReportRepository>(provider =>
    new TotalReportRepository(connectionString, loggerFactory));

services.AddScoped<IEvaluationUsersRepository>(provider =>
    new EvaluationUsersRepository(connectionString, loggerFactory));
```

---

## 4. 테스트 항목

개발자가 테스트할 항목:

### 4.1. 프로젝트 빌드 테스트
1. Visual Studio 2022에서 MdcHR26Apps.Models 프로젝트 열기
2. 솔루션 빌드 (Ctrl+Shift+B)
3. **확인**: 빌드 성공, 오류 0개 ✅

### 4.2. 파일 구조 확인
솔루션 탐색기에서 다음 파일들이 정상적으로 생성되었는지 확인:

```
MdcHR26Apps.Models/
├── EvaluationProcess/
│   ├── ProcessDb.cs                    ✅
│   ├── IProcessRepository.cs           ✅
│   └── ProcessRepository.cs            ✅
├── EvaluationReport/
│   ├── ReportDb.cs                     ✅
│   ├── IReportRepository.cs            ✅
│   └── ReportRepository.cs             ✅
├── Result/
│   ├── TotalReportDb.cs                ✅
│   ├── ITotalReportRepository.cs       ✅
│   └── TotalReportRepository.cs        ✅
├── EvaluationUsers/
│   ├── EvaluationUsers.cs              ✅
│   ├── IEvaluationUsersRepository.cs   ✅
│   └── EvaluationUsersRepository.cs    ✅
├── MdcHR26AppsAddDbContext.cs          ✅ (수정)
└── MdcHR26AppsAddExtensions.cs         ✅ (수정)
```

### 4.3. 코드 품질 확인
1. **Primary Constructor** 사용 확인
   - 모든 Repository 클래스
2. **Raw String Literals** 사용 확인
   - SQL 문에 `"""` 사용
3. **Nullable 참조 형식** 확인
   - `string?` 타입 사용
4. **Boolean 비교 개선** 확인
   - `'true'` → `1` 사용

### 4.4. 데이터베이스 연결 테스트 (추후 Web 프로젝트에서)
Phase 2-2 완료 후 Web 프로젝트 생성 시:
1. appsettings.json에 연결 문자열 설정
2. Program.cs에 `builder.Services.AddMdcHR26AppsModels(builder.Configuration);` 추가
3. Controller에서 Repository 주입 테스트
4. 간단한 CRUD 동작 확인

---

## 5. 예상 결과

### 수정 전
- ProcessDb, ReportDb, TotalReportDb, EvaluationUsers 모델 없음 ❌
- 평가 프로세스 관련 Repository 없음 ❌

### 수정 후
- 4개 Entity 클래스 생성 완료 ✅
- 12개 Repository 파일 생성 완료 (Interface + 구현) ✅
- DbContext 및 Extensions 업데이트 완료 ✅
- Dapper + Primary Constructor 적용 ✅
- 2025년 비즈니스 로직 반영 ✅
- Boolean 비교 로직 개선 ✅

---

## 6. 주의사항

1. **FK 참조 변경**: `UserId` (string) → `Uid` (long)로 모든 메서드 매개변수 변경
2. **Boolean 비교**: `'true'` 문자열 비교 대신 `1` 또는 매개변수 사용
3. **UserName 조회**: View를 통한 조회 권장 (직접 테이블 조회 시 JOIN 필요)
4. **Primary Constructor**: C# 13 기능이므로 .NET 9.0 + LangVersion=latest 필수
5. **연결 문자열**: appsettings.json의 `ConnectionStrings:DefaultConnection` 설정 필요
6. **Phase 순차 진행**: Phase 2-2 완료 후 2-3, 2-4 순서대로 진행

---

## 7. 완료 조건

- [ ] EvaluationProcess/ 폴더 3개 파일 작성 (Entity, Interface, Repository)
- [ ] EvaluationReport/ 폴더 3개 파일 작성
- [ ] Result/ 폴더 3개 파일 작성
- [ ] EvaluationUsers/ 폴더 3개 파일 작성
- [ ] MdcHR26AppsAddDbContext.cs 수정 (4개 DbSet 추가)
- [ ] MdcHR26AppsAddExtensions.cs 수정 (4개 Repository 등록)
- [ ] 빌드 성공 (오류 0개)
- [ ] 폴더 구조 확인
- [ ] Primary Constructor 적용 확인
- [ ] Raw String Literals 적용 확인
- [ ] Boolean 비교 로직 개선 확인

---

## 8. 다음 단계 (Phase 2-3)

Phase 2-2 완료 후:
1. **작업지시서 작성**: `20260119_03_phase2_3_agreement_tasks.md`
2. **개발 대상**:
   - DeptObjectiveDb (부서 목표)
   - AgreementDb (직무평가 협의서)
   - SubAgreementDb (상세 직무평가 협의서)
   - TasksDb (업무 관리)
   - EvaluationLists (평가 항목 마스터)
3. **DbContext 업데이트**: 5개 테이블 추가
4. **Extensions 업데이트**: 5개 Repository 등록

---

**작성자**: Claude AI
**검토 필요**: 개발자
**승인 후**: 코드 작성 시작
