# 작업지시서: Phase 2-3 - 목표/협의/업무 모델 구현

**날짜**: 2026-01-19
**작업 유형**: 기능 추가 (Phase 2-3)
**관련 이슈**: [#008: Phase 2 Model 개발](../issues/008_phase2_model_development.md)
**관련 작업지시서**:
- `20260116_01_phase2_model_development.md` (Phase 2 전체 계획)
- `20260119_01_phase2_1_project_setup.md` (Phase 2-1 완료)
- `20260119_02_phase2_2_evaluation_core.md` (Phase 2-2 완료)

**수정 이력**:
- 2026-01-19 (v1): 초안 작성 - 2025년 비즈니스 로직 분석 기반
- 2026-01-19 (v2): 데이터 타입 통일 - long → Int64 (Phase 2-1, 2-2 일관성 유지)

---

## 1. 작업 개요

### 1.1. Phase 2-3 목표
목표 수립 및 협의 프로세스 관련 5개 모델을 구현합니다.

### 1.2. 작업 범위
1. **DeptObjectiveDb**: 부서 목표 관리
2. **AgreementDb**: 직무평가 협의서 (대분류)
3. **SubAgreementDb**: 상세 직무평가 협의서 (세부 항목)
4. **TasksDb**: 업무/과업 관리
5. **EvaluationLists**: 평가 항목 마스터 데이터

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
├── DeptObjective/
│   ├── DeptObjectiveDb.cs              # Entity
│   ├── IDeptObjectiveRepository.cs     # Interface
│   └── DeptObjectiveRepository.cs      # Dapper 구현체
├── EvaluationAgreement/
│   ├── AgreementDb.cs                  # Entity
│   ├── IAgreementRepository.cs         # Interface
│   └── AgreementRepository.cs          # Dapper 구현체
├── EvaluationSubAgreement/
│   ├── SubAgreementDb.cs               # Entity
│   ├── ISubAgreementRepository.cs      # Interface
│   └── SubAgreementRepository.cs       # Dapper 구현체
├── EvaluationTasks/
│   ├── TasksDb.cs                      # Entity
│   ├── ITasksRepository.cs             # Interface
│   └── TasksRepository.cs              # Dapper 구현체
├── EvaluationLists/
│   ├── EvaluationLists.cs              # Entity
│   ├── IEvaluationListsRepository.cs   # Interface
│   └── EvaluationListsRepository.cs    # Dapper 구현체
├── MdcHR26AppsAddDbContext.cs          # ← 5개 DbSet 추가
└── MdcHR26AppsAddExtensions.cs         # ← 5개 Repository 등록
```

### 2.2. Phase 2-3 개발 파일 목록 (15개)

| 번호 | 파일명 | 용도 | 비고 |
|------|--------|------|------|
| 1 | DeptObjective/DeptObjectiveDb.cs | 부서 목표 Entity | 6개 필드 |
| 2 | DeptObjective/IDeptObjectiveRepository.cs | Interface | 7개 메서드 |
| 3 | DeptObjective/DeptObjectiveRepository.cs | Repository 구현 | Dapper + Primary Constructor |
| 4 | EvaluationAgreement/AgreementDb.cs | 직무평가 협의서 Entity | 10개 필드 |
| 5 | EvaluationAgreement/IAgreementRepository.cs | Interface | 9개 메서드 |
| 6 | EvaluationAgreement/AgreementRepository.cs | Repository 구현 | Dapper + Primary Constructor |
| 7 | EvaluationSubAgreement/SubAgreementDb.cs | 상세 협의서 Entity | 8개 필드 |
| 8 | EvaluationSubAgreement/ISubAgreementRepository.cs | Interface | 10개 메서드 |
| 9 | EvaluationSubAgreement/SubAgreementRepository.cs | Repository 구현 | Dapper + Primary Constructor |
| 10 | EvaluationTasks/TasksDb.cs | 업무 관리 Entity | 8개 필드 |
| 11 | EvaluationTasks/ITasksRepository.cs | Interface | 8개 메서드 |
| 12 | EvaluationTasks/TasksRepository.cs | Repository 구현 | Dapper + Primary Constructor |
| 13 | EvaluationLists/EvaluationLists.cs | 평가 항목 Entity | 6개 필드 |
| 14 | EvaluationLists/IEvaluationListsRepository.cs | Interface | 8개 메서드 |
| 15 | EvaluationLists/EvaluationListsRepository.cs | Repository 구현 | Dapper + Primary Constructor |

**추가 수정**:
- `MdcHR26AppsAddDbContext.cs`: 5개 DbSet 추가
- `MdcHR26AppsAddExtensions.cs`: 5개 Repository DI 등록

---

## 3. 단계별 작업 내용

### 3.1. DeptObjectiveDb Entity 및 Repository

#### 3.1.1. Entity (DeptObjectiveDb.cs)

**파일**: `DeptObjective/DeptObjectiveDb.cs`

**DB 스키마 기반** (2026년):
- PK: `DOid` (BIGINT IDENTITY)
- FK: `EDepartId` → `EDepartmentDb.EDepartId`

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.DeptObjective;

/// <summary>
/// 부서 목표 Entity
/// </summary>
[Table("DeptObjectiveDb")]
public class DeptObjectiveDb
{
    /// <summary>
    /// DeptObjective ID (PK, IDENTITY)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 DOid { get; set; }

    /// <summary>
    /// 부서 ID (FK → EDepartmentDb.EDepartId)
    /// </summary>
    [Required]
    public Int64 EDepartId { get; set; }

    /// <summary>
    /// 목표 제목
    /// </summary>
    [Required]
    [StringLength(255)]
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
    [Required]
    public bool IsActive { get; set; } = true;

    // Navigation Property
    [ForeignKey("EDepartId")]
    public EDepartmentDb? EDepartment { get; set; }
}
```

**2025년 대비 변경사항**:
- `EDepartmentId` (string) → `EDepartId` (long) FK 참조
- `EDepartmentName` 필드 제거 (View를 통해 조회)

#### 3.1.2. Interface (IDeptObjectiveRepository.cs)

**파일**: `DeptObjective/IDeptObjectiveRepository.cs`

```csharp
namespace MdcHR26Apps.Models.DeptObjective;

/// <summary>
/// DeptObjectiveDb Repository Interface
/// </summary>
public interface IDeptObjectiveRepository : IDisposable
{
    // === 기본 CRUD ===

    /// <summary>
    /// 부서 목표 추가
    /// </summary>
    Task<Int64> AddAsync(DeptObjectiveDb model);

    /// <summary>
    /// 전체 부서 목표 조회
    /// </summary>
    Task<IEnumerable<DeptObjectiveDb>> GetByAllAsync();

    /// <summary>
    /// ID로 부서 목표 조회
    /// </summary>
    Task<DeptObjectiveDb?> GetByIdAsync(Int64 id);

    /// <summary>
    /// 부서 목표 수정
    /// </summary>
    Task<int> UpdateAsync(DeptObjectiveDb model);

    /// <summary>
    /// 부서 목표 삭제
    /// </summary>
    Task<int> DeleteAsync(Int64 id);

    // === 비즈니스 특화 메서드 ===

    /// <summary>
    /// 부서별 목표 조회
    /// </summary>
    Task<IEnumerable<DeptObjectiveDb>> GetByDepartmentAsync(Int64 departmentId);

    /// <summary>
    /// 활성화된 부서 목표 조회
    /// </summary>
    Task<IEnumerable<DeptObjectiveDb>> GetActiveAsync();
}
```

#### 3.1.3. Repository 구현 (DeptObjectiveRepository.cs)

**파일**: `DeptObjective/DeptObjectiveRepository.cs`

```csharp
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MdcHR26Apps.Models.DeptObjective;

/// <summary>
/// DeptObjectiveDb Repository 구현 (Dapper)
/// Primary Constructor 사용 (C# 13)
/// </summary>
public class DeptObjectiveRepository(string connectionString, ILoggerFactory loggerFactory) : IDeptObjectiveRepository
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<DeptObjectiveRepository>();
    private readonly string dbContext = connectionString;

    #region + [1] 입력: AddAsync
    /// <summary>
    /// 부서 목표 추가
    /// </summary>
    public async Task<Int64> AddAsync(DeptObjectiveDb model)
    {
        const string sql = """
            INSERT INTO DeptObjectiveDb(
                EDepartId, Objective_Title, Objective_Content, Achievement_Criteria, IsActive)
            VALUES(
                @EDepartId, @Objective_Title, @Objective_Content, @Achievement_Criteria, @IsActive);
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteScalarAsync<Int64>(sql, model);
    }
    #endregion

    #region + [2] 출력: GetByAllAsync
    /// <summary>
    /// 전체 부서 목표 조회
    /// </summary>
    public async Task<IEnumerable<DeptObjectiveDb>> GetByAllAsync()
    {
        const string sql = "SELECT * FROM DeptObjectiveDb ORDER BY DOid";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<DeptObjectiveDb>(sql);
    }
    #endregion

    #region + [3] 상세: GetByIdAsync
    /// <summary>
    /// ID로 부서 목표 조회
    /// </summary>
    public async Task<DeptObjectiveDb?> GetByIdAsync(Int64 id)
    {
        const string sql = "SELECT * FROM DeptObjectiveDb WHERE DOid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<DeptObjectiveDb>(sql, new { id });
    }
    #endregion

    #region + [4] 수정: UpdateAsync
    /// <summary>
    /// 부서 목표 수정
    /// </summary>
    public async Task<int> UpdateAsync(DeptObjectiveDb model)
    {
        const string sql = """
            UPDATE DeptObjectiveDb
            SET
                EDepartId = @EDepartId,
                Objective_Title = @Objective_Title,
                Objective_Content = @Objective_Content,
                Achievement_Criteria = @Achievement_Criteria,
                IsActive = @IsActive
            WHERE DOid = @DOid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, model);
    }
    #endregion

    #region + [5] 삭제: DeleteAsync
    /// <summary>
    /// 부서 목표 삭제
    /// </summary>
    public async Task<int> DeleteAsync(Int64 id)
    {
        const string sql = "DELETE FROM DeptObjectiveDb WHERE DOid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, new { id });
    }
    #endregion

    #region + [6] 부서별 조회: GetByDepartmentAsync
    /// <summary>
    /// 부서별 목표 조회
    /// </summary>
    public async Task<IEnumerable<DeptObjectiveDb>> GetByDepartmentAsync(Int64 departmentId)
    {
        const string sql = """
            SELECT * FROM DeptObjectiveDb
            WHERE EDepartId = @departmentId
            ORDER BY DOid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<DeptObjectiveDb>(sql, new { departmentId });
    }
    #endregion

    #region + [7] 활성화된 목표: GetActiveAsync
    /// <summary>
    /// 활성화된 부서 목표 조회
    /// </summary>
    public async Task<IEnumerable<DeptObjectiveDb>> GetActiveAsync()
    {
        const string sql = """
            SELECT * FROM DeptObjectiveDb
            WHERE IsActive = 1
            ORDER BY DOid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<DeptObjectiveDb>(sql);
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

### 3.2. AgreementDb Entity 및 Repository

#### 3.2.1. Entity (AgreementDb.cs)

**파일**: `EvaluationAgreement/AgreementDb.cs`

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.EvaluationAgreement;

/// <summary>
/// 직무평가 협의서 Entity (대분류)
/// </summary>
[Table("AgreementDb")]
public class AgreementDb
{
    /// <summary>
    /// Agreement ID (PK, IDENTITY)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 Aid { get; set; }

    /// <summary>
    /// 평가 대상자 ID (FK → UserDb.Uid)
    /// </summary>
    [Required]
    public Int64 Uid { get; set; }

    /// <summary>
    /// 협의서 항목 번호
    /// </summary>
    [Required]
    public int Agreement_Item_Number { get; set; }

    /// <summary>
    /// 지표 분류명
    /// </summary>
    [Required]
    [StringLength(255)]
    public string Agreement_Item_Name_1 { get; set; } = string.Empty;

    /// <summary>
    /// 직무 분류명
    /// </summary>
    [Required]
    [StringLength(255)]
    public string Agreement_Item_Name_2 { get; set; } = string.Empty;

    /// <summary>
    /// 직무 비율(%)
    /// </summary>
    [Required]
    public int Agreement_Item_Proportion { get; set; }

    /// <summary>
    /// 부서 목표 연결 (FK → DeptObjectiveDb.DOid)
    /// </summary>
    public Int64? DeptObjective_Number { get; set; }

    /// <summary>
    /// 합의 여부
    /// </summary>
    [Required]
    public bool Is_Agreement { get; set; } = false;

    /// <summary>
    /// 합의 의견
    /// </summary>
    public string? Agreement_Comment { get; set; }

    /// <summary>
    /// 합의 일시
    /// </summary>
    public DateTime? Agreement_Date { get; set; }

    // Navigation Properties
    [ForeignKey("Uid")]
    public UserDb? User { get; set; }

    [ForeignKey("DeptObjective_Number")]
    public DeptObjectiveDb? DeptObjective { get; set; }
}
```

**2025년 대비 변경사항**:
- `UserId` (string) → `Uid` (long) FK 참조
- `UserName` 필드 제거 (View를 통해 조회)

#### 3.2.2. Interface (IAgreementRepository.cs)

**파일**: `EvaluationAgreement/IAgreementRepository.cs`

```csharp
namespace MdcHR26Apps.Models.EvaluationAgreement;

/// <summary>
/// AgreementDb Repository Interface
/// </summary>
public interface IAgreementRepository : IDisposable
{
    // === 기본 CRUD ===

    /// <summary>
    /// 직무평가 협의서 추가
    /// </summary>
    Task<Int64> AddAsync(AgreementDb model);

    /// <summary>
    /// 전체 협의서 조회
    /// </summary>
    Task<IEnumerable<AgreementDb>> GetByAllAsync();

    /// <summary>
    /// ID로 협의서 조회
    /// </summary>
    Task<AgreementDb?> GetByIdAsync(Int64 id);

    /// <summary>
    /// 협의서 수정
    /// </summary>
    Task<int> UpdateAsync(AgreementDb model);

    /// <summary>
    /// 협의서 삭제
    /// </summary>
    Task<int> DeleteAsync(Int64 id);

    // === 비즈니스 특화 메서드 ===

    /// <summary>
    /// 사용자별 협의서 조회
    /// </summary>
    Task<IEnumerable<AgreementDb>> GetByUidAsync(Int64 uid);

    /// <summary>
    /// 합의 완료 여부 확인
    /// </summary>
    Task<bool> IsAgreementCompleteAsync(Int64 uid);

    /// <summary>
    /// 합의 대기 중인 항목 조회
    /// </summary>
    Task<IEnumerable<AgreementDb>> GetPendingAgreementAsync(Int64 uid);

    /// <summary>
    /// 부서 목표별 협의서 조회
    /// </summary>
    Task<IEnumerable<AgreementDb>> GetByDeptObjectiveAsync(Int64 deptObjectiveId);
}
```

#### 3.2.3. Repository 구현 (AgreementRepository.cs)

**파일**: `EvaluationAgreement/AgreementRepository.cs`

```csharp
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MdcHR26Apps.Models.EvaluationAgreement;

/// <summary>
/// AgreementDb Repository 구현 (Dapper)
/// Primary Constructor 사용 (C# 13)
/// </summary>
public class AgreementRepository(string connectionString, ILoggerFactory loggerFactory) : IAgreementRepository
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<AgreementRepository>();
    private readonly string dbContext = connectionString;

    #region + [1] 입력: AddAsync
    /// <summary>
    /// 직무평가 협의서 추가
    /// </summary>
    public async Task<Int64> AddAsync(AgreementDb model)
    {
        const string sql = """
            INSERT INTO AgreementDb(
                Uid, Agreement_Item_Number, Agreement_Item_Name_1, Agreement_Item_Name_2,
                Agreement_Item_Proportion, DeptObjective_Number, Is_Agreement,
                Agreement_Comment, Agreement_Date)
            VALUES(
                @Uid, @Agreement_Item_Number, @Agreement_Item_Name_1, @Agreement_Item_Name_2,
                @Agreement_Item_Proportion, @DeptObjective_Number, @Is_Agreement,
                @Agreement_Comment, @Agreement_Date);
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteScalarAsync<Int64>(sql, model);
    }
    #endregion

    #region + [2] 출력: GetByAllAsync
    /// <summary>
    /// 전체 협의서 조회
    /// </summary>
    public async Task<IEnumerable<AgreementDb>> GetByAllAsync()
    {
        const string sql = "SELECT * FROM AgreementDb ORDER BY Aid";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<AgreementDb>(sql);
    }
    #endregion

    #region + [3] 상세: GetByIdAsync
    /// <summary>
    /// ID로 협의서 조회
    /// </summary>
    public async Task<AgreementDb?> GetByIdAsync(Int64 id)
    {
        const string sql = "SELECT * FROM AgreementDb WHERE Aid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<AgreementDb>(sql, new { id });
    }
    #endregion

    #region + [4] 수정: UpdateAsync
    /// <summary>
    /// 협의서 수정
    /// </summary>
    public async Task<int> UpdateAsync(AgreementDb model)
    {
        const string sql = """
            UPDATE AgreementDb
            SET
                Uid = @Uid,
                Agreement_Item_Number = @Agreement_Item_Number,
                Agreement_Item_Name_1 = @Agreement_Item_Name_1,
                Agreement_Item_Name_2 = @Agreement_Item_Name_2,
                Agreement_Item_Proportion = @Agreement_Item_Proportion,
                DeptObjective_Number = @DeptObjective_Number,
                Is_Agreement = @Is_Agreement,
                Agreement_Comment = @Agreement_Comment,
                Agreement_Date = @Agreement_Date
            WHERE Aid = @Aid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, model);
    }
    #endregion

    #region + [5] 삭제: DeleteAsync
    /// <summary>
    /// 협의서 삭제
    /// </summary>
    public async Task<int> DeleteAsync(Int64 id)
    {
        const string sql = "DELETE FROM AgreementDb WHERE Aid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, new { id });
    }
    #endregion

    #region + [6] 사용자별 조회: GetByUidAsync
    /// <summary>
    /// 사용자별 협의서 조회
    /// </summary>
    public async Task<IEnumerable<AgreementDb>> GetByUidAsync(Int64 uid)
    {
        const string sql = """
            SELECT * FROM AgreementDb
            WHERE Uid = @uid
            ORDER BY Agreement_Item_Number
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<AgreementDb>(sql, new { uid });
    }
    #endregion

    #region + [7] 합의 완료 확인: IsAgreementCompleteAsync
    /// <summary>
    /// 합의 완료 여부 확인 (모든 항목이 합의되었는지)
    /// </summary>
    public async Task<bool> IsAgreementCompleteAsync(Int64 uid)
    {
        const string sql = """
            SELECT COUNT(*)
            FROM AgreementDb
            WHERE Uid = @uid AND Is_Agreement = 0
            """;

        using var connection = new SqlConnection(dbContext);
        int count = await connection.ExecuteScalarAsync<int>(sql, new { uid });
        return count == 0; // 미합의 항목이 0개면 완료
    }
    #endregion

    #region + [8] 합의 대기: GetPendingAgreementAsync
    /// <summary>
    /// 합의 대기 중인 항목 조회
    /// </summary>
    public async Task<IEnumerable<AgreementDb>> GetPendingAgreementAsync(Int64 uid)
    {
        const string sql = """
            SELECT * FROM AgreementDb
            WHERE Uid = @uid AND Is_Agreement = 0
            ORDER BY Agreement_Item_Number
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<AgreementDb>(sql, new { uid });
    }
    #endregion

    #region + [9] 부서 목표별 조회: GetByDeptObjectiveAsync
    /// <summary>
    /// 부서 목표별 협의서 조회
    /// </summary>
    public async Task<IEnumerable<AgreementDb>> GetByDeptObjectiveAsync(Int64 deptObjectiveId)
    {
        const string sql = """
            SELECT * FROM AgreementDb
            WHERE DeptObjective_Number = @deptObjectiveId
            ORDER BY Aid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<AgreementDb>(sql, new { deptObjectiveId });
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

### 3.3. SubAgreementDb Entity 및 Repository

#### 3.3.1. Entity (SubAgreementDb.cs)

**파일**: `EvaluationSubAgreement/SubAgreementDb.cs`

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.EvaluationSubAgreement;

/// <summary>
/// 상세 직무평가 협의서 Entity (세부 항목)
/// </summary>
[Table("SubAgreementDb")]
public class SubAgreementDb
{
    /// <summary>
    /// SubAgreement ID (PK, IDENTITY)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 SAid { get; set; }

    /// <summary>
    /// 평가 대상자 ID (FK → UserDb.Uid)
    /// </summary>
    [Required]
    public Int64 Uid { get; set; }

    /// <summary>
    /// 상위 협의서 번호 (FK → AgreementDb.Aid)
    /// </summary>
    [Required]
    public Int64 Agreement_Number { get; set; }

    /// <summary>
    /// 세부 직무명
    /// </summary>
    [Required]
    [StringLength(255)]
    public string SubAgreement_Item_Name { get; set; } = string.Empty;

    /// <summary>
    /// 세부 직무 비율(%)
    /// </summary>
    [Required]
    public int SubAgreement_Item_Proportion { get; set; }

    /// <summary>
    /// 합의 여부
    /// </summary>
    [Required]
    public bool Is_SubAgreement { get; set; } = false;

    /// <summary>
    /// 합의 의견
    /// </summary>
    public string? SubAgreement_Comment { get; set; }

    /// <summary>
    /// 합의 일시
    /// </summary>
    public DateTime? SubAgreement_Date { get; set; }

    // Navigation Properties
    [ForeignKey("Uid")]
    public UserDb? User { get; set; }

    [ForeignKey("Agreement_Number")]
    public AgreementDb? Agreement { get; set; }
}
```

**2025년 대비 변경사항**:
- `UserId` (string) → `Uid` (long) FK 참조
- `UserName` 필드 제거 (View를 통해 조회)

#### 3.3.2. Interface (ISubAgreementRepository.cs)

**파일**: `EvaluationSubAgreement/ISubAgreementRepository.cs`

```csharp
namespace MdcHR26Apps.Models.EvaluationSubAgreement;

/// <summary>
/// SubAgreementDb Repository Interface
/// </summary>
public interface ISubAgreementRepository : IDisposable
{
    // === 기본 CRUD ===

    /// <summary>
    /// 상세 협의서 추가
    /// </summary>
    Task<Int64> AddAsync(SubAgreementDb model);

    /// <summary>
    /// 전체 상세 협의서 조회
    /// </summary>
    Task<IEnumerable<SubAgreementDb>> GetByAllAsync();

    /// <summary>
    /// ID로 상세 협의서 조회
    /// </summary>
    Task<SubAgreementDb?> GetByIdAsync(Int64 id);

    /// <summary>
    /// 상세 협의서 수정
    /// </summary>
    Task<int> UpdateAsync(SubAgreementDb model);

    /// <summary>
    /// 상세 협의서 삭제
    /// </summary>
    Task<int> DeleteAsync(Int64 id);

    // === 비즈니스 특화 메서드 ===

    /// <summary>
    /// 사용자별 상세 협의서 조회
    /// </summary>
    Task<IEnumerable<SubAgreementDb>> GetByUidAsync(Int64 uid);

    /// <summary>
    /// 상위 협의서별 세부 항목 조회
    /// </summary>
    Task<IEnumerable<SubAgreementDb>> GetByAgreementNumberAsync(Int64 agreementNumber);

    /// <summary>
    /// 합의 완료 여부 확인
    /// </summary>
    Task<bool> IsSubAgreementCompleteAsync(Int64 uid);

    /// <summary>
    /// 합의 대기 중인 세부 항목 조회
    /// </summary>
    Task<IEnumerable<SubAgreementDb>> GetPendingSubAgreementAsync(Int64 uid);

    /// <summary>
    /// 상위 협의서의 세부 항목 비율 합계
    /// </summary>
    Task<int> GetTotalProportionAsync(Int64 agreementNumber);
}
```

#### 3.3.3. Repository 구현 (SubAgreementRepository.cs)

**파일**: `EvaluationSubAgreement/SubAgreementRepository.cs`

```csharp
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MdcHR26Apps.Models.EvaluationSubAgreement;

/// <summary>
/// SubAgreementDb Repository 구현 (Dapper)
/// Primary Constructor 사용 (C# 13)
/// </summary>
public class SubAgreementRepository(string connectionString, ILoggerFactory loggerFactory) : ISubAgreementRepository
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<SubAgreementRepository>();
    private readonly string dbContext = connectionString;

    #region + [1] 입력: AddAsync
    /// <summary>
    /// 상세 협의서 추가
    /// </summary>
    public async Task<Int64> AddAsync(SubAgreementDb model)
    {
        const string sql = """
            INSERT INTO SubAgreementDb(
                Uid, Agreement_Number, SubAgreement_Item_Name, SubAgreement_Item_Proportion,
                Is_SubAgreement, SubAgreement_Comment, SubAgreement_Date)
            VALUES(
                @Uid, @Agreement_Number, @SubAgreement_Item_Name, @SubAgreement_Item_Proportion,
                @Is_SubAgreement, @SubAgreement_Comment, @SubAgreement_Date);
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteScalarAsync<Int64>(sql, model);
    }
    #endregion

    #region + [2] 출력: GetByAllAsync
    /// <summary>
    /// 전체 상세 협의서 조회
    /// </summary>
    public async Task<IEnumerable<SubAgreementDb>> GetByAllAsync()
    {
        const string sql = "SELECT * FROM SubAgreementDb ORDER BY SAid";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<SubAgreementDb>(sql);
    }
    #endregion

    #region + [3] 상세: GetByIdAsync
    /// <summary>
    /// ID로 상세 협의서 조회
    /// </summary>
    public async Task<SubAgreementDb?> GetByIdAsync(Int64 id)
    {
        const string sql = "SELECT * FROM SubAgreementDb WHERE SAid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<SubAgreementDb>(sql, new { id });
    }
    #endregion

    #region + [4] 수정: UpdateAsync
    /// <summary>
    /// 상세 협의서 수정
    /// </summary>
    public async Task<int> UpdateAsync(SubAgreementDb model)
    {
        const string sql = """
            UPDATE SubAgreementDb
            SET
                Uid = @Uid,
                Agreement_Number = @Agreement_Number,
                SubAgreement_Item_Name = @SubAgreement_Item_Name,
                SubAgreement_Item_Proportion = @SubAgreement_Item_Proportion,
                Is_SubAgreement = @Is_SubAgreement,
                SubAgreement_Comment = @SubAgreement_Comment,
                SubAgreement_Date = @SubAgreement_Date
            WHERE SAid = @SAid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, model);
    }
    #endregion

    #region + [5] 삭제: DeleteAsync
    /// <summary>
    /// 상세 협의서 삭제
    /// </summary>
    public async Task<int> DeleteAsync(Int64 id)
    {
        const string sql = "DELETE FROM SubAgreementDb WHERE SAid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, new { id });
    }
    #endregion

    #region + [6] 사용자별 조회: GetByUidAsync
    /// <summary>
    /// 사용자별 상세 협의서 조회
    /// </summary>
    public async Task<IEnumerable<SubAgreementDb>> GetByUidAsync(Int64 uid)
    {
        const string sql = """
            SELECT * FROM SubAgreementDb
            WHERE Uid = @uid
            ORDER BY SAid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<SubAgreementDb>(sql, new { uid });
    }
    #endregion

    #region + [7] 상위 협의서별 조회: GetByAgreementNumberAsync
    /// <summary>
    /// 상위 협의서별 세부 항목 조회
    /// </summary>
    public async Task<IEnumerable<SubAgreementDb>> GetByAgreementNumberAsync(Int64 agreementNumber)
    {
        const string sql = """
            SELECT * FROM SubAgreementDb
            WHERE Agreement_Number = @agreementNumber
            ORDER BY SAid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<SubAgreementDb>(sql, new { agreementNumber });
    }
    #endregion

    #region + [8] 합의 완료 확인: IsSubAgreementCompleteAsync
    /// <summary>
    /// 합의 완료 여부 확인
    /// </summary>
    public async Task<bool> IsSubAgreementCompleteAsync(Int64 uid)
    {
        const string sql = """
            SELECT COUNT(*)
            FROM SubAgreementDb
            WHERE Uid = @uid AND Is_SubAgreement = 0
            """;

        using var connection = new SqlConnection(dbContext);
        int count = await connection.ExecuteScalarAsync<int>(sql, new { uid });
        return count == 0;
    }
    #endregion

    #region + [9] 합의 대기: GetPendingSubAgreementAsync
    /// <summary>
    /// 합의 대기 중인 세부 항목 조회
    /// </summary>
    public async Task<IEnumerable<SubAgreementDb>> GetPendingSubAgreementAsync(Int64 uid)
    {
        const string sql = """
            SELECT * FROM SubAgreementDb
            WHERE Uid = @uid AND Is_SubAgreement = 0
            ORDER BY SAid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<SubAgreementDb>(sql, new { uid });
    }
    #endregion

    #region + [10] 비율 합계: GetTotalProportionAsync
    /// <summary>
    /// 상위 협의서의 세부 항목 비율 합계
    /// </summary>
    public async Task<int> GetTotalProportionAsync(Int64 agreementNumber)
    {
        const string sql = """
            SELECT COALESCE(SUM(SubAgreement_Item_Proportion), 0)
            FROM SubAgreementDb
            WHERE Agreement_Number = @agreementNumber
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteScalarAsync<int>(sql, new { agreementNumber });
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

### 3.4. TasksDb Entity 및 Repository

#### 3.4.1. Entity (TasksDb.cs)

**파일**: `EvaluationTasks/TasksDb.cs`

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.EvaluationTasks;

/// <summary>
/// 업무/과업 관리 Entity
/// </summary>
[Table("TasksDb")]
public class TasksDb
{
    /// <summary>
    /// Task ID (PK, IDENTITY)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 Tid { get; set; }

    /// <summary>
    /// 평가 대상자 ID (FK → UserDb.Uid)
    /// </summary>
    [Required]
    public Int64 Uid { get; set; }

    /// <summary>
    /// 세부 협의서 번호 (FK → SubAgreementDb.SAid)
    /// </summary>
    [Required]
    public Int64 SubAgreement_Number { get; set; }

    /// <summary>
    /// 업무명
    /// </summary>
    [Required]
    [StringLength(255)]
    public string Task_Name { get; set; } = string.Empty;

    /// <summary>
    /// 업무 내용
    /// </summary>
    public string? Task_Content { get; set; }

    /// <summary>
    /// 목표 달성 기준
    /// </summary>
    public string? Task_Criteria { get; set; }

    /// <summary>
    /// 예상 소요 시간 (시간 단위)
    /// </summary>
    public int? Estimated_Hours { get; set; }

    /// <summary>
    /// 완료 여부
    /// </summary>
    [Required]
    public bool Is_Completed { get; set; } = false;

    // Navigation Properties
    [ForeignKey("Uid")]
    public UserDb? User { get; set; }

    [ForeignKey("SubAgreement_Number")]
    public SubAgreementDb? SubAgreement { get; set; }
}
```

**2025년 대비 변경사항**:
- `UserId` (string) → `Uid` (long) FK 참조
- `UserName` 필드 제거 (View를 통해 조회)

#### 3.4.2. Interface (ITasksRepository.cs)

**파일**: `EvaluationTasks/ITasksRepository.cs`

```csharp
namespace MdcHR26Apps.Models.EvaluationTasks;

/// <summary>
/// TasksDb Repository Interface
/// </summary>
public interface ITasksRepository : IDisposable
{
    // === 기본 CRUD ===

    /// <summary>
    /// 업무 추가
    /// </summary>
    Task<Int64> AddAsync(TasksDb model);

    /// <summary>
    /// 전체 업무 조회
    /// </summary>
    Task<IEnumerable<TasksDb>> GetByAllAsync();

    /// <summary>
    /// ID로 업무 조회
    /// </summary>
    Task<TasksDb?> GetByIdAsync(Int64 id);

    /// <summary>
    /// 업무 수정
    /// </summary>
    Task<int> UpdateAsync(TasksDb model);

    /// <summary>
    /// 업무 삭제
    /// </summary>
    Task<int> DeleteAsync(Int64 id);

    // === 비즈니스 특화 메서드 ===

    /// <summary>
    /// 사용자별 업무 조회
    /// </summary>
    Task<IEnumerable<TasksDb>> GetByUidAsync(Int64 uid);

    /// <summary>
    /// 세부 협의서별 업무 조회
    /// </summary>
    Task<IEnumerable<TasksDb>> GetBySubAgreementNumberAsync(Int64 subAgreementNumber);

    /// <summary>
    /// 미완료 업무 조회
    /// </summary>
    Task<IEnumerable<TasksDb>> GetIncompleteTasksAsync(Int64 uid);
}
```

#### 3.4.3. Repository 구현 (TasksRepository.cs)

**파일**: `EvaluationTasks/TasksRepository.cs`

```csharp
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MdcHR26Apps.Models.EvaluationTasks;

/// <summary>
/// TasksDb Repository 구현 (Dapper)
/// Primary Constructor 사용 (C# 13)
/// </summary>
public class TasksRepository(string connectionString, ILoggerFactory loggerFactory) : ITasksRepository
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<TasksRepository>();
    private readonly string dbContext = connectionString;

    #region + [1] 입력: AddAsync
    /// <summary>
    /// 업무 추가
    /// </summary>
    public async Task<Int64> AddAsync(TasksDb model)
    {
        const string sql = """
            INSERT INTO TasksDb(
                Uid, SubAgreement_Number, Task_Name, Task_Content,
                Task_Criteria, Estimated_Hours, Is_Completed)
            VALUES(
                @Uid, @SubAgreement_Number, @Task_Name, @Task_Content,
                @Task_Criteria, @Estimated_Hours, @Is_Completed);
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteScalarAsync<Int64>(sql, model);
    }
    #endregion

    #region + [2] 출력: GetByAllAsync
    /// <summary>
    /// 전체 업무 조회
    /// </summary>
    public async Task<IEnumerable<TasksDb>> GetByAllAsync()
    {
        const string sql = "SELECT * FROM TasksDb ORDER BY Tid";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<TasksDb>(sql);
    }
    #endregion

    #region + [3] 상세: GetByIdAsync
    /// <summary>
    /// ID로 업무 조회
    /// </summary>
    public async Task<TasksDb?> GetByIdAsync(Int64 id)
    {
        const string sql = "SELECT * FROM TasksDb WHERE Tid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<TasksDb>(sql, new { id });
    }
    #endregion

    #region + [4] 수정: UpdateAsync
    /// <summary>
    /// 업무 수정
    /// </summary>
    public async Task<int> UpdateAsync(TasksDb model)
    {
        const string sql = """
            UPDATE TasksDb
            SET
                Uid = @Uid,
                SubAgreement_Number = @SubAgreement_Number,
                Task_Name = @Task_Name,
                Task_Content = @Task_Content,
                Task_Criteria = @Task_Criteria,
                Estimated_Hours = @Estimated_Hours,
                Is_Completed = @Is_Completed
            WHERE Tid = @Tid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, model);
    }
    #endregion

    #region + [5] 삭제: DeleteAsync
    /// <summary>
    /// 업무 삭제
    /// </summary>
    public async Task<int> DeleteAsync(Int64 id)
    {
        const string sql = "DELETE FROM TasksDb WHERE Tid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, new { id });
    }
    #endregion

    #region + [6] 사용자별 조회: GetByUidAsync
    /// <summary>
    /// 사용자별 업무 조회
    /// </summary>
    public async Task<IEnumerable<TasksDb>> GetByUidAsync(Int64 uid)
    {
        const string sql = """
            SELECT * FROM TasksDb
            WHERE Uid = @uid
            ORDER BY Tid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<TasksDb>(sql, new { uid });
    }
    #endregion

    #region + [7] 세부 협의서별 조회: GetBySubAgreementNumberAsync
    /// <summary>
    /// 세부 협의서별 업무 조회
    /// </summary>
    public async Task<IEnumerable<TasksDb>> GetBySubAgreementNumberAsync(Int64 subAgreementNumber)
    {
        const string sql = """
            SELECT * FROM TasksDb
            WHERE SubAgreement_Number = @subAgreementNumber
            ORDER BY Tid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<TasksDb>(sql, new { subAgreementNumber });
    }
    #endregion

    #region + [8] 미완료 업무: GetIncompleteTasksAsync
    /// <summary>
    /// 미완료 업무 조회
    /// </summary>
    public async Task<IEnumerable<TasksDb>> GetIncompleteTasksAsync(Int64 uid)
    {
        const string sql = """
            SELECT * FROM TasksDb
            WHERE Uid = @uid AND Is_Completed = 0
            ORDER BY Tid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<TasksDb>(sql, new { uid });
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

### 3.5. EvaluationLists Entity 및 Repository

#### 3.5.1. Entity (EvaluationLists.cs)

**파일**: `EvaluationLists/EvaluationLists.cs`

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.EvaluationLists;

/// <summary>
/// 평가 항목 마스터 Entity
/// </summary>
[Table("EvaluationLists")]
public class EvaluationLists
{
    /// <summary>
    /// Evaluation List ID (PK, IDENTITY)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 ELid { get; set; }

    /// <summary>
    /// 평가 항목 번호
    /// </summary>
    [Required]
    public int Evaluation_Number { get; set; }

    /// <summary>
    /// 평가 항목명
    /// </summary>
    [Required]
    [StringLength(255)]
    public string Evaluation_Item { get; set; } = string.Empty;

    /// <summary>
    /// 평가 항목 설명
    /// </summary>
    public string? Evaluation_Description { get; set; }

    /// <summary>
    /// 배점
    /// </summary>
    [Required]
    public int Score { get; set; }

    /// <summary>
    /// 활성화 여부
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;
}
```

#### 3.5.2. Interface (IEvaluationListsRepository.cs)

**파일**: `EvaluationLists/IEvaluationListsRepository.cs`

```csharp
using MdcHR26Apps.Models.Common;

namespace MdcHR26Apps.Models.EvaluationLists;

/// <summary>
/// EvaluationLists Repository Interface
/// </summary>
public interface IEvaluationListsRepository : IDisposable
{
    // === 기본 CRUD ===

    /// <summary>
    /// 평가 항목 추가
    /// </summary>
    Task<Int64> AddAsync(EvaluationLists model);

    /// <summary>
    /// 전체 평가 항목 조회
    /// </summary>
    Task<IEnumerable<EvaluationLists>> GetByAllAsync();

    /// <summary>
    /// ID로 평가 항목 조회
    /// </summary>
    Task<EvaluationLists?> GetByIdAsync(Int64 id);

    /// <summary>
    /// 평가 항목 수정
    /// </summary>
    Task<int> UpdateAsync(EvaluationLists model);

    /// <summary>
    /// 평가 항목 삭제
    /// </summary>
    Task<int> DeleteAsync(Int64 id);

    // === 비즈니스 특화 메서드 ===

    /// <summary>
    /// 활성화된 평가 항목 조회
    /// </summary>
    Task<IEnumerable<EvaluationLists>> GetActiveAsync();

    /// <summary>
    /// 평가 번호로 조회
    /// </summary>
    Task<EvaluationLists?> GetByNumberAsync(int evaluationNumber);

    /// <summary>
    /// 드롭다운용 평가 항목 목록
    /// </summary>
    Task<IEnumerable<SelectListModel>> GetSelectListAsync();
}
```

#### 3.5.3. Repository 구현 (EvaluationListsRepository.cs)

**파일**: `EvaluationLists/EvaluationListsRepository.cs`

```csharp
using Dapper;
using MdcHR26Apps.Models.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MdcHR26Apps.Models.EvaluationLists;

/// <summary>
/// EvaluationLists Repository 구현 (Dapper)
/// Primary Constructor 사용 (C# 13)
/// </summary>
public class EvaluationListsRepository(string connectionString, ILoggerFactory loggerFactory) : IEvaluationListsRepository
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<EvaluationListsRepository>();
    private readonly string dbContext = connectionString;

    #region + [1] 입력: AddAsync
    /// <summary>
    /// 평가 항목 추가
    /// </summary>
    public async Task<Int64> AddAsync(EvaluationLists model)
    {
        const string sql = """
            INSERT INTO EvaluationLists(
                Evaluation_Number, Evaluation_Item, Evaluation_Description, Score, IsActive)
            VALUES(
                @Evaluation_Number, @Evaluation_Item, @Evaluation_Description, @Score, @IsActive);
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteScalarAsync<Int64>(sql, model);
    }
    #endregion

    #region + [2] 출력: GetByAllAsync
    /// <summary>
    /// 전체 평가 항목 조회
    /// </summary>
    public async Task<IEnumerable<EvaluationLists>> GetByAllAsync()
    {
        const string sql = "SELECT * FROM EvaluationLists ORDER BY Evaluation_Number";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<EvaluationLists>(sql);
    }
    #endregion

    #region + [3] 상세: GetByIdAsync
    /// <summary>
    /// ID로 평가 항목 조회
    /// </summary>
    public async Task<EvaluationLists?> GetByIdAsync(Int64 id)
    {
        const string sql = "SELECT * FROM EvaluationLists WHERE ELid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<EvaluationLists>(sql, new { id });
    }
    #endregion

    #region + [4] 수정: UpdateAsync
    /// <summary>
    /// 평가 항목 수정
    /// </summary>
    public async Task<int> UpdateAsync(EvaluationLists model)
    {
        const string sql = """
            UPDATE EvaluationLists
            SET
                Evaluation_Number = @Evaluation_Number,
                Evaluation_Item = @Evaluation_Item,
                Evaluation_Description = @Evaluation_Description,
                Score = @Score,
                IsActive = @IsActive
            WHERE ELid = @ELid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, model);
    }
    #endregion

    #region + [5] 삭제: DeleteAsync
    /// <summary>
    /// 평가 항목 삭제
    /// </summary>
    public async Task<int> DeleteAsync(Int64 id)
    {
        const string sql = "DELETE FROM EvaluationLists WHERE ELid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, new { id });
    }
    #endregion

    #region + [6] 활성화된 항목: GetActiveAsync
    /// <summary>
    /// 활성화된 평가 항목 조회
    /// </summary>
    public async Task<IEnumerable<EvaluationLists>> GetActiveAsync()
    {
        const string sql = """
            SELECT * FROM EvaluationLists
            WHERE IsActive = 1
            ORDER BY Evaluation_Number
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<EvaluationLists>(sql);
    }
    #endregion

    #region + [7] 평가 번호로 조회: GetByNumberAsync
    /// <summary>
    /// 평가 번호로 조회
    /// </summary>
    public async Task<EvaluationLists?> GetByNumberAsync(int evaluationNumber)
    {
        const string sql = """
            SELECT * FROM EvaluationLists
            WHERE Evaluation_Number = @evaluationNumber
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<EvaluationLists>(sql, new { evaluationNumber });
    }
    #endregion

    #region + [8] 드롭다운 목록: GetSelectListAsync
    /// <summary>
    /// 드롭다운용 평가 항목 목록
    /// </summary>
    public async Task<IEnumerable<SelectListModel>> GetSelectListAsync()
    {
        const string sql = """
            SELECT
                CAST(ELid AS NVARCHAR) AS Value,
                Evaluation_Item + ' (' + CAST(Score AS NVARCHAR) + '점)' AS Text
            FROM EvaluationLists
            WHERE IsActive = 1
            ORDER BY Evaluation_Number
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<SelectListModel>(sql);
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

### 3.6. DbContext 및 Extensions 업데이트

#### 3.6.1. MdcHR26AppsAddDbContext.cs 수정

**파일**: `MdcHR26AppsAddDbContext.cs`

**수정 내용**:
```csharp
// === Phase 2-3: 목표/협의/업무 모델 (5개) 추가 ===
public DbSet<DeptObjectiveDb> DeptObjectiveDb { get; set; } = null!;
public DbSet<AgreementDb> AgreementDb { get; set; } = null!;
public DbSet<SubAgreementDb> SubAgreementDb { get; set; } = null!;
public DbSet<TasksDb> TasksDb { get; set; } = null!;
public DbSet<EvaluationLists> EvaluationLists { get; set; } = null!;
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

    // === Phase 2-2: 평가 핵심 모델 (4개) ===
    modelBuilder.Entity<ProcessDb>().ToTable("ProcessDb");
    modelBuilder.Entity<ReportDb>().ToTable("ReportDb");
    modelBuilder.Entity<TotalReportDb>().ToTable("TotalReportDb");
    modelBuilder.Entity<EvaluationUsers>().ToTable("EvaluationUsers");

    // === Phase 2-3: 목표/협의/업무 모델 (5개) 추가 ===
    modelBuilder.Entity<DeptObjectiveDb>().ToTable("DeptObjectiveDb");
    modelBuilder.Entity<AgreementDb>().ToTable("AgreementDb");
    modelBuilder.Entity<SubAgreementDb>().ToTable("SubAgreementDb");
    modelBuilder.Entity<TasksDb>().ToTable("TasksDb");
    modelBuilder.Entity<EvaluationLists>().ToTable("EvaluationLists");
}
```

#### 3.6.2. MdcHR26AppsAddExtensions.cs 수정

**파일**: `MdcHR26AppsAddExtensions.cs`

**수정 내용**:
```csharp
// === Phase 2-3: Repository 등록 (5개) 추가 ===
services.AddScoped<IDeptObjectiveRepository>(provider =>
    new DeptObjectiveRepository(connectionString, loggerFactory));

services.AddScoped<IAgreementRepository>(provider =>
    new AgreementRepository(connectionString, loggerFactory));

services.AddScoped<ISubAgreementRepository>(provider =>
    new SubAgreementRepository(connectionString, loggerFactory));

services.AddScoped<ITasksRepository>(provider =>
    new TasksRepository(connectionString, loggerFactory));

services.AddScoped<IEvaluationListsRepository>(provider =>
    new EvaluationListsRepository(connectionString, loggerFactory));
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
├── DeptObjective/
│   ├── DeptObjectiveDb.cs                ✅
│   ├── IDeptObjectiveRepository.cs       ✅
│   └── DeptObjectiveRepository.cs        ✅
├── EvaluationAgreement/
│   ├── AgreementDb.cs                    ✅
│   ├── IAgreementRepository.cs           ✅
│   └── AgreementRepository.cs            ✅
├── EvaluationSubAgreement/
│   ├── SubAgreementDb.cs                 ✅
│   ├── ISubAgreementRepository.cs        ✅
│   └── SubAgreementRepository.cs         ✅
├── EvaluationTasks/
│   ├── TasksDb.cs                        ✅
│   ├── ITasksRepository.cs               ✅
│   └── TasksRepository.cs                ✅
├── EvaluationLists/
│   ├── EvaluationLists.cs                ✅
│   ├── IEvaluationListsRepository.cs     ✅
│   └── EvaluationListsRepository.cs      ✅
├── MdcHR26AppsAddDbContext.cs            ✅ (수정)
└── MdcHR26AppsAddExtensions.cs           ✅ (수정)
```

### 4.3. 코드 품질 확인
1. **Primary Constructor** 사용 확인
   - 모든 Repository 클래스
2. **Raw String Literals** 사용 확인
   - SQL 문에 `"""` 사용
3. **Nullable 참조 형식** 확인
   - `string?` 타입 사용
4. **Int64 타입 통일** 확인
   - 모든 ID 필드 및 FK는 `Int64` 사용

---

## 5. 예상 결과

### 수정 전
- DeptObjectiveDb, AgreementDb, SubAgreementDb, TasksDb, EvaluationLists 모델 없음 ❌
- 목표/협의/업무 관련 Repository 없음 ❌

### 수정 후
- 5개 Entity 클래스 생성 완료 ✅
- 15개 Repository 파일 생성 완료 (Interface + 구현) ✅
- DbContext 및 Extensions 업데이트 완료 ✅
- Dapper + Primary Constructor 적용 ✅
- 2025년 비즈니스 로직 반영 ✅
- Int64 타입 통일 ✅

---

## 6. 주의사항

1. **FK 참조 정규화**: `UserId` (string) → `Uid` (long)로 모든 메서드 매개변수 변경
2. **Boolean 비교**: `1` 또는 `0` 사용 (Phase 2-2에서 검증 완료)
3. **계층 구조**: Agreement → SubAgreement → Tasks (FK 관계 유지)
4. **비율 합계 검증**: SubAgreement의 비율 합계가 100%인지 검증 로직 필요 (비즈니스 레이어)
5. **Primary Constructor**: C# 13 기능이므로 .NET 9.0 + LangVersion=latest 필수
6. **연결 문자열**: appsettings.json의 `ConnectionStrings:DefaultConnection` 설정 필요

---

## 7. 완료 조건

- [ ] DeptObjective/ 폴더 3개 파일 작성 (Entity, Interface, Repository)
- [ ] EvaluationAgreement/ 폴더 3개 파일 작성
- [ ] EvaluationSubAgreement/ 폴더 3개 파일 작성
- [ ] EvaluationTasks/ 폴더 3개 파일 작성
- [ ] EvaluationLists/ 폴더 3개 파일 작성
- [ ] MdcHR26AppsAddDbContext.cs 수정 (5개 DbSet 추가)
- [ ] MdcHR26AppsAddExtensions.cs 수정 (5개 Repository 등록)
- [ ] 빌드 성공 (오류 0개)
- [ ] 폴더 구조 확인
- [ ] Primary Constructor 적용 확인
- [ ] Raw String Literals 적용 확인
- [ ] Int64 타입 통일 확인

---

## 8. 다음 단계 (Phase 2-4)

Phase 2-3 완료 후:
1. **작업지시서 작성**: `20260119_04_phase2_4_view_models.md`
2. **개발 대상**:
   - v_MemberListDB (재직자 목록)
   - v_DeptObjectiveListDb (부서 목표 목록)
   - v_ProcessTRListDB (프로세스 & 종합 보고서)
   - v_ReportTaskListDB (평가 보고서 & 업무)
   - v_TotalReportListDB (종합 보고서 목록)
3. **DbContext 업데이트**: 5개 View 엔티티 추가
4. **Extensions 업데이트**: 5개 View Repository 등록 (선택 사항)

---

**작성자**: Claude AI
**검토 필요**: 개발자
**승인 후**: 코드 작성 시작
