# 작업지시서: Phase 2-4 - View 모델 구현

**날짜**: 2026-01-19
**작업 유형**: 기능 추가 (Phase 2-4)
**관련 이슈**: [#008: Phase 2 Model 개발](../issues/008_phase2_model_development.md)
**관련 작업지시서**:
- `20260116_01_phase2_model_development.md` (Phase 2 전체 계획)
- `20260119_01_phase2_1_project_setup.md` (Phase 2-1 완료)
- `20260119_02_phase2_2_evaluation_core.md` (Phase 2-2 완료)
- `20260119_03_phase2_3_objective_agreement.md` (Phase 2-3 완료)

**수정 이력**:
- 2026-01-19 (v1): 초안 작성 - 2025년 View 구조 분석 기반
- 2026-01-19 (v2): 데이터 타입 통일 - long → Int64 (Phase 2-1, 2-2, 2-3 일관성 유지)

---

## 1. 작업 개요

### 1.1. Phase 2-4 목표
복잡한 JOIN 쿼리를 View로 구현하여 성능 최적화 및 코드 간소화를 실현합니다.

### 1.2. 작업 범위
1. **v_MemberListDB**: 재직자 목록 (UserDb + 부서/직급 정보)
2. **v_DeptObjectiveListDb**: 부서 목표 목록 (DeptObjectiveDb + 부서명)
3. **v_ProcessTRListDB**: 프로세스 & 종합 보고서 (ProcessDb + TotalReportDb + User 정보)
4. **v_ReportTaskListDB**: 평가 보고서 & 업무 (ReportDb + TasksDb 정보)
5. **v_TotalReportListDB**: 종합 보고서 목록 (TotalReportDb + User 정보)

### 1.3. 기술 스택
- **.NET**: 9.0
- **C#**: 13 (Primary Constructors, Raw String Literals)
- **ORM**: Dapper 2.1.66 (주), EF Core 9.0.0 (보조)
- **Database**: SQL Server Views
- **패턴**: Repository Pattern, Dependency Injection

---

## 2. 프로젝트 구조

### 2.1. 폴더 구조
```
MdcHR26Apps.Models/
├── Views/
│   ├── v_MemberListDB.cs                   # Entity (Key 없음)
│   ├── Iv_MemberListRepository.cs          # Interface
│   ├── v_MemberListRepository.cs           # Dapper 구현체
│   ├── v_DeptObjectiveListDb.cs            # Entity (Key 없음)
│   ├── Iv_DeptObjectiveListRepository.cs   # Interface
│   ├── v_DeptObjectiveListRepository.cs    # Dapper 구현체
│   ├── v_ProcessTRListDB.cs                # Entity (Key 없음)
│   ├── Iv_ProcessTRListRepository.cs       # Interface
│   ├── v_ProcessTRListRepository.cs        # Dapper 구현체
│   ├── v_ReportTaskListDB.cs               # Entity (Key 없음)
│   ├── Iv_ReportTaskListRepository.cs      # Interface
│   ├── v_ReportTaskListRepository.cs       # Dapper 구현체
│   ├── v_TotalReportListDB.cs              # Entity (Key 없음)
│   ├── Iv_TotalReportListRepository.cs     # Interface
│   └── v_TotalReportListRepository.cs      # Dapper 구현체
├── MdcHR26AppsAddDbContext.cs              # ← 5개 View 엔티티 추가
└── MdcHR26AppsAddExtensions.cs             # ← 5개 View Repository 등록 (선택)
```

### 2.2. Phase 2-4 개발 파일 목록 (15개)

| 번호 | 파일명 | 용도 | 비고 |
|------|--------|------|------|
| 1 | Views/v_MemberListDB.cs | 재직자 목록 Entity | Key 없음, 읽기 전용 |
| 2 | Views/Iv_MemberListRepository.cs | Interface | 조회 메서드만 |
| 3 | Views/v_MemberListRepository.cs | Repository 구현 | Dapper + Primary Constructor |
| 4 | Views/v_DeptObjectiveListDb.cs | 부서 목표 목록 Entity | Key 없음, 읽기 전용 |
| 5 | Views/Iv_DeptObjectiveListRepository.cs | Interface | 조회 메서드만 |
| 6 | Views/v_DeptObjectiveListRepository.cs | Repository 구현 | Dapper + Primary Constructor |
| 7 | Views/v_ProcessTRListDB.cs | 프로세스 & 종합 보고서 Entity | Key 없음, 읽기 전용 |
| 8 | Views/Iv_ProcessTRListRepository.cs | Interface | 조회 메서드만 |
| 9 | Views/v_ProcessTRListRepository.cs | Repository 구현 | Dapper + Primary Constructor |
| 10 | Views/v_ReportTaskListDB.cs | 평가 보고서 & 업무 Entity | Key 없음, 읽기 전용 |
| 11 | Views/Iv_ReportTaskListRepository.cs | Interface | 조회 메서드만 |
| 12 | Views/v_ReportTaskListRepository.cs | Repository 구현 | Dapper + Primary Constructor |
| 13 | Views/v_TotalReportListDB.cs | 종합 보고서 목록 Entity | Key 없음, 읽기 전용 |
| 14 | Views/Iv_TotalReportListRepository.cs | Interface | 조회 메서드만 |
| 15 | Views/v_TotalReportListRepository.cs | Repository 구현 | Dapper + Primary Constructor |

**추가 수정** (선택사항):
- `MdcHR26AppsAddDbContext.cs`: 5개 View 엔티티 활성화 (HasNoKey 설정)
- `MdcHR26AppsAddExtensions.cs`: 5개 View Repository DI 등록 (필요시)

---

## 3. 단계별 작업 내용

### 3.1. v_MemberListDB Entity 및 Repository

#### 3.1.1. Entity (v_MemberListDB.cs)

**파일**: `Views/v_MemberListDB.cs`

**DB View 기반**:
- View 이름: `v_MemberListDB`
- 기반 테이블: `UserDb LEFT JOIN EDepartmentDb LEFT JOIN ERankDb`
- 조건: `EStatus = 1` (재직자만)

```csharp
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MdcHR26Apps.Models.Views;

/// <summary>
/// 재직자 목록 View Entity
/// UserDb + 부서/직급 정보 (EStatus = 1)
/// </summary>
[Table("v_MemberListDB")]
[Keyless]
public class v_MemberListDB
{
    /// <summary>
    /// 사용자 ID
    /// </summary>
    public Int64 Uid { get; set; }

    /// <summary>
    /// 로그인 ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 사용자 이름
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 사원번호
    /// </summary>
    public string? ENumber { get; set; }

    /// <summary>
    /// 이메일
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 부서 ID
    /// </summary>
    public Int64? EDepartId { get; set; }

    /// <summary>
    /// 부서명
    /// </summary>
    public string? EDepartmentName { get; set; }

    /// <summary>
    /// 직급 ID
    /// </summary>
    public Int64? ERankId { get; set; }

    /// <summary>
    /// 직급명
    /// </summary>
    public string? ERankName { get; set; }

    /// <summary>
    /// 팀장 여부
    /// </summary>
    public bool IsTeamLeader { get; set; }

    /// <summary>
    /// 임원 여부
    /// </summary>
    public bool IsDirector { get; set; }

    /// <summary>
    /// 관리자 여부
    /// </summary>
    public bool IsAdministrator { get; set; }

    /// <summary>
    /// 부서 목표 작성 권한
    /// </summary>
    public bool IsDeptObjectiveWriter { get; set; }
}
```

**View 생성 SQL** (참고용):
```sql
CREATE VIEW v_MemberListDB AS
SELECT
    U.Uid,
    U.UserId,
    U.UserName,
    U.ENumber,
    U.Email,
    U.EDepartId,
    D.EDepartmentName,
    U.ERankId,
    R.ERankName,
    U.IsTeamLeader,
    U.IsDirector,
    U.IsAdministrator,
    U.IsDeptObjectiveWriter
FROM UserDb U
LEFT JOIN EDepartmentDb D ON U.EDepartId = D.EDepartId
LEFT JOIN ERankDb R ON U.ERankId = R.ERankId
WHERE U.EStatus = 1;
```

#### 3.1.2. Interface (Iv_MemberListRepository.cs)

**파일**: `Views/Iv_MemberListRepository.cs`

```csharp
namespace MdcHR26Apps.Models.Views;

/// <summary>
/// v_MemberListDB Repository Interface
/// 읽기 전용 (View)
/// </summary>
public interface Iv_MemberListRepository : IDisposable
{
    /// <summary>
    /// 전체 재직자 목록 조회
    /// </summary>
    Task<IEnumerable<v_MemberListDB>> GetByAllAsync();

    /// <summary>
    /// ID로 재직자 조회
    /// </summary>
    Task<v_MemberListDB?> GetByIdAsync(Int64 uid);

    /// <summary>
    /// 부서별 재직자 조회
    /// </summary>
    Task<IEnumerable<v_MemberListDB>> GetByDepartmentAsync(Int64 departmentId);

    /// <summary>
    /// 팀장 목록 조회
    /// </summary>
    Task<IEnumerable<v_MemberListDB>> GetTeamLeadersAsync();

    /// <summary>
    /// 임원 목록 조회
    /// </summary>
    Task<IEnumerable<v_MemberListDB>> GetDirectorsAsync();

    /// <summary>
    /// 사용자명으로 검색
    /// </summary>
    Task<IEnumerable<v_MemberListDB>> SearchByNameAsync(string userName);
}
```

#### 3.1.3. Repository 구현 (v_MemberListRepository.cs)

**파일**: `Views/v_MemberListRepository.cs`

```csharp
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MdcHR26Apps.Models.Views;

/// <summary>
/// v_MemberListDB Repository 구현 (Dapper)
/// Primary Constructor 사용 (C# 13)
/// </summary>
public class v_MemberListRepository(string connectionString, ILoggerFactory loggerFactory) : Iv_MemberListRepository
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<v_MemberListRepository>();
    private readonly string dbContext = connectionString;

    #region + [1] 전체 조회: GetByAllAsync
    /// <summary>
    /// 전체 재직자 목록 조회
    /// </summary>
    public async Task<IEnumerable<v_MemberListDB>> GetByAllAsync()
    {
        const string sql = """
            SELECT * FROM v_MemberListDB
            ORDER BY EDepartId, ERankId, Uid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_MemberListDB>(sql);
    }
    #endregion

    #region + [2] ID 조회: GetByIdAsync
    /// <summary>
    /// ID로 재직자 조회
    /// </summary>
    public async Task<v_MemberListDB?> GetByIdAsync(Int64 uid)
    {
        const string sql = "SELECT * FROM v_MemberListDB WHERE Uid = @uid";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<v_MemberListDB>(sql, new { uid });
    }
    #endregion

    #region + [3] 부서별 조회: GetByDepartmentAsync
    /// <summary>
    /// 부서별 재직자 조회
    /// </summary>
    public async Task<IEnumerable<v_MemberListDB>> GetByDepartmentAsync(Int64 departmentId)
    {
        const string sql = """
            SELECT * FROM v_MemberListDB
            WHERE EDepartId = @departmentId
            ORDER BY ERankId, Uid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_MemberListDB>(sql, new { departmentId });
    }
    #endregion

    #region + [4] 팀장 조회: GetTeamLeadersAsync
    /// <summary>
    /// 팀장 목록 조회
    /// </summary>
    public async Task<IEnumerable<v_MemberListDB>> GetTeamLeadersAsync()
    {
        const string sql = """
            SELECT * FROM v_MemberListDB
            WHERE IsTeamLeader = 1
            ORDER BY EDepartId, Uid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_MemberListDB>(sql);
    }
    #endregion

    #region + [5] 임원 조회: GetDirectorsAsync
    /// <summary>
    /// 임원 목록 조회
    /// </summary>
    public async Task<IEnumerable<v_MemberListDB>> GetDirectorsAsync()
    {
        const string sql = """
            SELECT * FROM v_MemberListDB
            WHERE IsDirector = 1
            ORDER BY Uid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_MemberListDB>(sql);
    }
    #endregion

    #region + [6] 이름 검색: SearchByNameAsync
    /// <summary>
    /// 사용자명으로 검색
    /// </summary>
    public async Task<IEnumerable<v_MemberListDB>> SearchByNameAsync(string userName)
    {
        const string sql = """
            SELECT * FROM v_MemberListDB
            WHERE UserName LIKE @userName + '%'
            ORDER BY UserName
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_MemberListDB>(sql, new { userName });
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

### 3.2. v_DeptObjectiveListDb Entity 및 Repository

#### 3.2.1. Entity (v_DeptObjectiveListDb.cs)

**파일**: `Views/v_DeptObjectiveListDb.cs`

```csharp
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MdcHR26Apps.Models.Views;

/// <summary>
/// 부서 목표 목록 View Entity
/// DeptObjectiveDb + 부서명
/// </summary>
[Table("v_DeptObjectiveListDb")]
[Keyless]
public class v_DeptObjectiveListDb
{
    /// <summary>
    /// DeptObjective ID
    /// </summary>
    public Int64 DOid { get; set; }

    /// <summary>
    /// 부서 ID
    /// </summary>
    public Int64 EDepartId { get; set; }

    /// <summary>
    /// 부서명
    /// </summary>
    public string EDepartmentName { get; set; } = string.Empty;

    /// <summary>
    /// 목표 제목
    /// </summary>
    public string Objective_Title { get; set; } = string.Empty;

    /// <summary>
    /// 목표 내용
    /// </summary>
    public string? Objective_Content { get; set; }

    /// <summary>
    /// 목표 달성 기준
    /// </summary>
    public string? Achievement_Criteria { get; set; }

    /// <summary>
    /// 활성화 여부
    /// </summary>
    public bool IsActive { get; set; }
}
```

**View 생성 SQL** (참고용):
```sql
CREATE VIEW v_DeptObjectiveListDb AS
SELECT
    DO.DOid,
    DO.EDepartId,
    D.EDepartmentName,
    DO.Objective_Title,
    DO.Objective_Content,
    DO.Achievement_Criteria,
    DO.IsActive
FROM DeptObjectiveDb DO
INNER JOIN EDepartmentDb D ON DO.EDepartId = D.EDepartId;
```

#### 3.2.2. Interface (Iv_DeptObjectiveListRepository.cs)

**파일**: `Views/Iv_DeptObjectiveListRepository.cs`

```csharp
namespace MdcHR26Apps.Models.Views;

/// <summary>
/// v_DeptObjectiveListDb Repository Interface
/// 읽기 전용 (View)
/// </summary>
public interface Iv_DeptObjectiveListRepository : IDisposable
{
    /// <summary>
    /// 전체 부서 목표 목록 조회
    /// </summary>
    Task<IEnumerable<v_DeptObjectiveListDb>> GetByAllAsync();

    /// <summary>
    /// ID로 부서 목표 조회
    /// </summary>
    Task<v_DeptObjectiveListDb?> GetByIdAsync(Int64 id);

    /// <summary>
    /// 부서별 목표 조회
    /// </summary>
    Task<IEnumerable<v_DeptObjectiveListDb>> GetByDepartmentAsync(Int64 departmentId);

    /// <summary>
    /// 활성화된 목표 조회
    /// </summary>
    Task<IEnumerable<v_DeptObjectiveListDb>> GetActiveAsync();
}
```

#### 3.2.3. Repository 구현 (v_DeptObjectiveListRepository.cs)

**파일**: `Views/v_DeptObjectiveListRepository.cs`

```csharp
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MdcHR26Apps.Models.Views;

/// <summary>
/// v_DeptObjectiveListDb Repository 구현 (Dapper)
/// Primary Constructor 사용 (C# 13)
/// </summary>
public class v_DeptObjectiveListRepository(string connectionString, ILoggerFactory loggerFactory) : Iv_DeptObjectiveListRepository
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<v_DeptObjectiveListRepository>();
    private readonly string dbContext = connectionString;

    #region + [1] 전체 조회: GetByAllAsync
    /// <summary>
    /// 전체 부서 목표 목록 조회
    /// </summary>
    public async Task<IEnumerable<v_DeptObjectiveListDb>> GetByAllAsync()
    {
        const string sql = "SELECT * FROM v_DeptObjectiveListDb ORDER BY DOid";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_DeptObjectiveListDb>(sql);
    }
    #endregion

    #region + [2] ID 조회: GetByIdAsync
    /// <summary>
    /// ID로 부서 목표 조회
    /// </summary>
    public async Task<v_DeptObjectiveListDb?> GetByIdAsync(Int64 id)
    {
        const string sql = "SELECT * FROM v_DeptObjectiveListDb WHERE DOid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<v_DeptObjectiveListDb>(sql, new { id });
    }
    #endregion

    #region + [3] 부서별 조회: GetByDepartmentAsync
    /// <summary>
    /// 부서별 목표 조회
    /// </summary>
    public async Task<IEnumerable<v_DeptObjectiveListDb>> GetByDepartmentAsync(Int64 departmentId)
    {
        const string sql = """
            SELECT * FROM v_DeptObjectiveListDb
            WHERE EDepartId = @departmentId
            ORDER BY DOid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_DeptObjectiveListDb>(sql, new { departmentId });
    }
    #endregion

    #region + [4] 활성화된 목표: GetActiveAsync
    /// <summary>
    /// 활성화된 목표 조회
    /// </summary>
    public async Task<IEnumerable<v_DeptObjectiveListDb>> GetActiveAsync()
    {
        const string sql = """
            SELECT * FROM v_DeptObjectiveListDb
            WHERE IsActive = 1
            ORDER BY DOid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_DeptObjectiveListDb>(sql);
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

### 3.3. 나머지 3개 View 구조

Phase 2-4의 나머지 3개 View는 복잡도가 높으므로 간략한 구조만 제시합니다.

#### 3.3.1. v_ProcessTRListDB (프로세스 & 종합 보고서)

**Entity 구조**:
- `ProcessDb` + `TotalReportDb` + `UserDb` 정보
- 평가 진행 상황과 점수를 한 번에 조회

**주요 필드**:
- Process 정보: `Pid`, `Uid`, `Is_Request`, `Is_Agreement` 등
- TotalReport 정보: `TRid`, `Total_Score`, `Director_Score` 등
- User 정보: `UserName`, `EDepartmentName`, `ERankName`

**View SQL** (참고):
```sql
CREATE VIEW v_ProcessTRListDB AS
SELECT
    P.*,
    TR.TRid, TR.Total_Score, TR.Director_Score, TR.TeamLeader_Score,
    U.UserName, D.EDepartmentName, R.ERankName
FROM ProcessDb P
LEFT JOIN TotalReportDb TR ON P.Uid = TR.Uid
INNER JOIN UserDb U ON P.Uid = U.Uid
LEFT JOIN EDepartmentDb D ON U.EDepartId = D.EDepartId
LEFT JOIN ERankDb R ON U.ERankId = R.ERankId;
```

#### 3.3.2. v_ReportTaskListDB (평가 보고서 & 업무)

**Entity 구조**:
- `ReportDb` + `TasksDb` + `UserDb` 정보
- 평가 항목별 업무 목록 조회

**주요 필드**:
- Report 정보: `Rid`, `Report_Item_Name_1`, `Report_Item_Name_2` 등
- Tasks 정보: `Tid`, `Task_Name`, `Task_Content` 등
- User 정보: `UserName`

**View SQL** (참고):
```sql
CREATE VIEW v_ReportTaskListDB AS
SELECT
    R.*,
    T.Tid, T.Task_Name, T.Task_Content, T.Is_Completed,
    U.UserName
FROM ReportDb R
LEFT JOIN TasksDb T ON R.Task_Number = T.Tid
INNER JOIN UserDb U ON R.Uid = U.Uid;
```

#### 3.3.3. v_TotalReportListDB (종합 보고서 목록)

**Entity 구조**:
- `TotalReportDb` + `UserDb` 정보
- 종합 평가 결과 조회

**주요 필드**:
- TotalReport 정보: 모든 필드
- User 정보: `UserName`, `EDepartmentName`, `ERankName`

**View SQL** (참고):
```sql
CREATE VIEW v_TotalReportListDB AS
SELECT
    TR.*,
    U.UserName, D.EDepartmentName, R.ERankName
FROM TotalReportDb TR
INNER JOIN UserDb U ON TR.Uid = U.Uid
LEFT JOIN EDepartmentDb D ON U.EDepartId = D.EDepartId
LEFT JOIN ERankDb R ON U.ERankId = R.ERankId;
```

---

### 3.4. DbContext 업데이트 (선택사항)

#### 3.4.1. MdcHR26AppsAddDbContext.cs 수정

**파일**: `MdcHR26AppsAddDbContext.cs`

**수정 내용**:
```csharp
using MdcHR26Apps.Models.Views;

// === Phase 2-4: 5개 뷰 ===
public DbSet<v_MemberListDB> v_MemberListDB { get; set; } = null!;
public DbSet<v_DeptObjectiveListDb> v_DeptObjectiveListDb { get; set; } = null!;
public DbSet<v_ProcessTRListDB> v_ProcessTRListDB { get; set; } = null!;
public DbSet<v_ReportTaskListDB> v_ReportTaskListDB { get; set; } = null!;
public DbSet<v_TotalReportListDB> v_TotalReportListDB { get; set; } = null!;
```

**OnModelCreating 메서드 수정**:
```csharp
// === Phase 2-4: 뷰 매핑 ===
modelBuilder.Entity<v_MemberListDB>().ToView("v_MemberListDB").HasNoKey();
modelBuilder.Entity<v_DeptObjectiveListDb>().ToView("v_DeptObjectiveListDb").HasNoKey();
modelBuilder.Entity<v_ProcessTRListDB>().ToView("v_ProcessTRListDB").HasNoKey();
modelBuilder.Entity<v_ReportTaskListDB>().ToView("v_ReportTaskListDB").HasNoKey();
modelBuilder.Entity<v_TotalReportListDB>().ToView("v_TotalReportListDB").HasNoKey();
```

#### 3.4.2. MdcHR26AppsAddExtensions.cs 수정 (선택사항)

**파일**: `MdcHR26AppsAddExtensions.cs`

**수정 내용** (필요시만):
```csharp
using MdcHR26Apps.Models.Views;

// === Phase 2-4: View Repository 등록 (선택) ===
services.AddScoped<Iv_MemberListRepository>(provider =>
{
    var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
    return new v_MemberListRepository(connectionString, loggerFactory);
});

services.AddScoped<Iv_DeptObjectiveListRepository>(provider =>
{
    var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
    return new v_DeptObjectiveListRepository(connectionString, loggerFactory);
});

// ... 나머지 3개 View Repository
```

---

## 4. 테스트 항목

개발자가 테스트할 항목:

### 4.1. Database View 생성
**중요**: 먼저 SQL Server에 5개 View를 생성해야 합니다.

```sql
-- 1. v_MemberListDB
-- 2. v_DeptObjectiveListDb
-- 3. v_ProcessTRListDB
-- 4. v_ReportTaskListDB
-- 5. v_TotalReportListDB
```

### 4.2. 프로젝트 빌드 테스트
1. Visual Studio 2022에서 MdcHR26Apps.Models 프로젝트 열기
2. 솔루션 빌드 (Ctrl+Shift+B)
3. **확인**: 빌드 성공, 오류 0개

### 4.3. 파일 구조 확인

```
MdcHR26Apps.Models/
└── Views/
    ├── v_MemberListDB.cs ✅
    ├── Iv_MemberListRepository.cs ✅
    ├── v_MemberListRepository.cs ✅
    ├── v_DeptObjectiveListDb.cs ✅
    ├── Iv_DeptObjectiveListRepository.cs ✅
    ├── v_DeptObjectiveListRepository.cs ✅
    └── ... (나머지 9개 파일)
```

---

## 5. 예상 결과

### 수정 전
- View 모델 없음 ❌
- 복잡한 JOIN 쿼리를 매번 Repository에서 작성 ❌

### 수정 후
- 5개 View Entity 클래스 생성 완료 ✅
- 15개 파일 생성 완료 (Entity 5개, Interface 5개, Repository 5개) ✅
- 읽기 전용 최적화 (CUD 메서드 없음) ✅
- DbContext View 매핑 (HasNoKey) ✅
- 성능 최적화 및 코드 간소화 ✅

---

## 6. 주의사항

1. **Database View 선행 작업**: 코드 작성 전에 SQL Server에 5개 View 생성 필수
2. **HasNoKey 설정**: View는 Primary Key가 없으므로 `[Keyless]` 속성 필수
3. **읽기 전용**: View는 CUD(Create, Update, Delete) 작업 불가
4. **Int64 타입 통일**: 모든 ID 필드는 `Int64` 사용
5. **Primary Constructor**: C# 13 기능이므로 .NET 9.0 필수
6. **View 성능**: 복잡한 JOIN이 포함되므로 인덱스 최적화 권장

---

## 7. 완료 조건

- [ ] Database에 5개 View 생성 (SQL Server)
- [ ] Views/ 폴더 15개 파일 작성 (Entity 5개, Interface 5개, Repository 5개)
- [ ] MdcHR26AppsAddDbContext.cs 수정 (5개 View 엔티티 추가, 선택)
- [ ] MdcHR26AppsAddExtensions.cs 수정 (5개 View Repository 등록, 선택)
- [ ] 빌드 성공 (오류 0개)
- [ ] Primary Constructor 적용 확인
- [ ] Raw String Literals 적용 확인
- [ ] HasNoKey 설정 확인

---

## 8. Phase 2 전체 완료

Phase 2-4 완료 시:
- **Phase 2-1**: 13개 파일 (기본 마스터 데이터) ✅
- **Phase 2-2**: 12개 파일 (평가 핵심 모델) ✅
- **Phase 2-3**: 15개 파일 (목표/협의/업무) ✅
- **Phase 2-4**: 15개 파일 (View 모델) ⏳

**총 파일 수**: **55개** (Entity, Interface, Repository, DbContext, Extensions)

---

## 9. 다음 단계 (Phase 3)

Phase 2 완료 후:
1. **Phase 3: Web API 개발**
   - ASP.NET Core Web API 프로젝트 생성
   - Controller 구현
   - Swagger 설정
   - JWT 인증 구현

2. **Phase 4: 프론트엔드 개발**
   - React 또는 Blazor 선택
   - UI/UX 구현

---

**작성자**: Claude AI
**검토 필요**: 개발자
**승인 후**: SQL View 생성 → 코드 작성 시작

**중요**: Phase 2-4는 **Database View를 먼저 생성**해야 작업이 가능합니다!
