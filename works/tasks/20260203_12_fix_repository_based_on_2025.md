# 작업지시서: Repository 수정 (25년 메서드 기준, 26년 Entity 구조 적용)

**날짜**: 2026-02-03
**작업 타입**: Repository/Interface 수정
**예상 소요**: 3시간
**관련 이슈**: [#009](../issues/009_phase3_webapp_development.md)
**선행 작업**: [20260203_11: Entity 필드명 수정](20260203_11_fix_entity_db_field_names.md)

---

## 1. 작업 개요

### 배경
- Entity 필드명 수정 완료 (DB 테이블 기준)
- 26년 Repository가 임의로 작성되어 25년과 다름
- **옵션 A 적용**: 25년 메서드를 기준으로 하되, 26년 Entity 구조(Uid FK 등)에 맞춰 SQL 작성

### 원칙
1. **메서드 목록**: 25년 Repository 메서드 기준 (개수, 이름, 시그니처)
2. **SQL 쿼리**: 26년 Entity 필드명에 맞춰 수정
3. **FK 구조**: 26년 Navigation Property는 유지 (Dapper는 자동으로 매핑하지 않음)
4. **스타일**: Primary Constructor 패턴 적용 (C# 13)

### 목표
5개 Repository를 25년 기준으로 재작성하되, SQL은 26년 Entity 구조에 맞춤

---

## 2. 작업 내용

### 2.1. AgreementRepository.cs

#### 파일: `MdcHR26Apps.Models/EvaluationAgreement/AgreementRepository.cs`

#### 25년 메서드 목록 (7개) - 기준
1. `AddAsync(AgreementDb model)` - 추가
2. `GetByAllAsync()` - 전체 조회
3. `GetByIdAsync(long id)` - ID 조회
4. `UpdateAsync(AgreementDb model)` - 수정
5. `DeleteAsync(long id)` - 삭제
6. `GetByUserIdAllAsync(string UserId)` - **사용자별 조회** (26년에 없음, 추가 필요)
7. `GetByTasksPeroportionAsync(string UserId, string DeptName, string IndexName)` - **부서명/지표명 조회** (26년에 없음, 추가 필요)

#### 26년에서 제거할 메서드 (25년에 없음)
- `GetCountByUidAsync` → 제거
- `DeleteAllByUidAsync` → 제거
- `IsAgreementCompleteAsync` → 제거
- `GetPendingAgreementAsync` → 제거
- `GetByDeptObjectiveAsync` → 제거

#### 25년 vs 26년 Entity 필드 차이
| 25년 Entity | 26년 Entity | SQL 컬럼명 |
|------------|------------|-----------|
| `Aid` (long) | `Aid` (Int64) | `[Aid]` |
| `UserId` (string) | `Uid` (Int64) | `[Uid]` ⚠️ |
| `UserName` (string) | ❌ 없음 | ❌ 없음 |
| `Report_Item_Number` | `Report_Item_Number` | `[Report_Item_Number]` |
| `Report_Item_Name_1` | `Report_Item_Name_1` | `[Report_Item_Name_1]` |
| `Report_Item_Name_2` | `Report_Item_Name_2` | `[Report_Item_Name_2]` |
| `Report_Item_Proportion` | `Report_Item_Proportion` | `[Report_Item_Proportion]` |

**핵심 변경 사항**:
- 25년: `UserId` (string) + `UserName` (string)
- 26년: `Uid` (Int64) + Navigation Property `User`
- SQL 테이블: `[Uid] BIGINT` (FK)

#### 수정 방법

**25년 SQL (UserId 기준)**:
```sql
INSERT INTO AgreementDb(UserId, UserName, Report_Item_Number, ...)
VALUES(@UserId, @UserName, @Report_Item_Number, ...)
```

**26년 SQL (Uid 기준)**:
```sql
INSERT INTO AgreementDb(Uid, Report_Item_Number, Report_Item_Name_1, Report_Item_Name_2, Report_Item_Proportion)
VALUES(@Uid, @Report_Item_Number, @Report_Item_Name_1, @Report_Item_Name_2, @Report_Item_Proportion)
```

#### 수정 후 코드

```csharp
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;

namespace MdcHR26Apps.Models.EvaluationAgreement;

public class AgreementRepository(string connectionString, ILoggerFactory loggerFactory) : IAgreementRepository, IDisposable
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger(nameof(AgreementRepository));
    private readonly string dbContext = connectionString;

    #region + [1] 입력: AddAsync
    public async Task<AgreementDb> AddAsync(AgreementDb model)
    {
        try
        {
            const string query =
                "INSERT INTO AgreementDb(" +
                    "Uid, Report_Item_Number, Report_Item_Name_1, Report_Item_Name_2, Report_Item_Proportion) " +
                "VALUES(" +
                    "@Uid, @Report_Item_Number, @Report_Item_Name_1, @Report_Item_Name_2, @Report_Item_Proportion);" +
                "Select Cast(SCOPE_IDENTITY() As Int);";

            using (var connection = new SqlConnection(dbContext))
            {
                int id = await connection.ExecuteScalarAsync<int>(query, model);
                model.Aid = id;
            }
        }
        catch (Exception e)
        {
            _logger?.LogError($"ERROR({nameof(AddAsync)}): {e.Message}");
        }

        return model;
    }
    #endregion

    #region + [2] 출력: GetByAllAsync
    public async Task<List<AgreementDb>> GetByAllAsync()
    {
        const string query = "Select * From AgreementDb Order By Aid";

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.QueryAsync<AgreementDb>(query, commandType: CommandType.Text);
            return result.ToList();
        }
    }
    #endregion

    #region + [3] 상세: GetByIdAsync
    public async Task<AgreementDb> GetByIdAsync(long id)
    {
        const string query = "Select * From AgreementDb Where Aid = @id";

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.QueryFirstOrDefaultAsync<AgreementDb>(query, new { id }, commandType: CommandType.Text);
            return result ?? new AgreementDb();
        }
    }
    #endregion

    #region + [4] 수정: UpdateAsync
    public async Task<bool> UpdateAsync(AgreementDb model)
    {
        const string query = @"
            Update AgreementDb
            Set
                Uid = @Uid,
                Report_Item_Number = @Report_Item_Number,
                Report_Item_Name_1 = @Report_Item_Name_1,
                Report_Item_Name_2 = @Report_Item_Name_2,
                Report_Item_Proportion = @Report_Item_Proportion
            Where Aid = @Aid";
        try
        {
            using (var connection = new SqlConnection(dbContext))
            {
                return await connection.ExecuteAsync(query, model) > 0;
            }
        }
        catch (Exception e)
        {
            _logger?.LogError($"ERROR({nameof(UpdateAsync)}): {e.Message}");
        }

        return false;
    }
    #endregion

    #region + [5] 삭제: DeleteAsync
    public async Task<bool> DeleteAsync(long id)
    {
        const string query = "Delete AgreementDb Where Aid = @id";

        try
        {
            using (var connection = new SqlConnection(dbContext))
            {
                return await connection.ExecuteAsync(query, new { id }, commandType: CommandType.Text) > 0;
            }
        }
        catch (Exception er)
        {
            _logger?.LogError($"ERROR({nameof(DeleteAsync)}): {er.Message}");
        }

        return false;
    }
    #endregion

    #region + [6] 사용자별 출력: GetByUserIdAllAsync
    // 25년: UserId (string)
    // 26년: Uid (long) - 메서드명은 유지하되 파라미터를 long으로 변경
    public async Task<List<AgreementDb>> GetByUserIdAllAsync(long userId)
    {
        const string query = "Select * From AgreementDb Where Uid = @userId Order By Aid";

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.QueryAsync<AgreementDb>(query, new { userId }, commandType: CommandType.Text);
            return result.ToList();
        }
    }
    #endregion

    #region + [7] 평가별 비중 출력: GetByTasksPeroportionAsync
    // 25년: Report_Item_Name_1 = 부서명, Report_Item_Name_2 = 지표명
    // 26년: 동일한 구조
    public async Task<List<AgreementDb>> GetByTasksPeroportionAsync(long userId, string deptName, string indexName)
    {
        const string query = @"
            Select Top(1) *
            From AgreementDb
            Where Uid = @userId
                And Report_Item_Name_1 = @deptName
                And Report_Item_Name_2 = @indexName
            Order By Aid";

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.QueryAsync<AgreementDb>(query, new { userId, deptName, indexName }, commandType: CommandType.Text);
            return result.ToList();
        }
    }
    #endregion

    #region + Dispose
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (db != null)
            {
                db.Dispose();
            }
        }
    }
    #endregion
}
```

#### Interface 수정

**파일**: `MdcHR26Apps.Models/EvaluationAgreement/IAgreementRepository.cs`

```csharp
namespace MdcHR26Apps.Models.EvaluationAgreement;

public interface IAgreementRepository : IDisposable
{
    Task<AgreementDb> AddAsync(AgreementDb model);
    Task<List<AgreementDb>> GetByAllAsync();
    Task<AgreementDb> GetByIdAsync(long id);
    Task<bool> UpdateAsync(AgreementDb model);
    Task<bool> DeleteAsync(long id);
    Task<List<AgreementDb>> GetByUserIdAllAsync(long userId);
    Task<List<AgreementDb>> GetByTasksPeroportionAsync(long userId, string deptName, string indexName);
}
```

---

### 2.2. SubAgreementRepository.cs

#### 파일: `MdcHR26Apps.Models/EvaluationSubAgreement/SubAgreementRepository.cs`

#### 25년 메서드 목록 (7개)
1. `AddAsync(SubAgreementDb model)`
2. `GetByAllAsync()`
3. `GetByIdAsync(long id)`
4. `UpdateAsync(SubAgreementDb model)`
5. `DeleteAsync(long id)`
6. `GetByUserIdAllAsync(string UserId)` → 26년: `GetByUserIdAllAsync(long userId)`
7. `GetByTasksPeroportionAsync(string UserId, string DeptName, string IndexName)` → 26년: 파라미터 long으로 변경

#### 25년 vs 26년 Entity 필드 차이
| 25년 Entity | 26년 Entity | SQL 컬럼명 |
|------------|------------|-----------|
| `Sid` (long) | `Sid` (Int64) | `[Sid]` |
| `UserId` (string) | `Uid` (Int64) | `[Uid]` ⚠️ |
| `UserName` (string) | ❌ 없음 | ❌ 없음 |
| `Report_Item_Number` | `Report_Item_Number` | `[Report_Item_Number]` |
| `Report_Item_Name_1` | `Report_Item_Name_1` | `[Report_Item_Name_1]` |
| `Report_Item_Name_2` | `Report_Item_Name_2` | `[Report_Item_Name_2]` |
| `Report_Item_Proportion` | `Report_Item_Proportion` | `[Report_Item_Proportion]` |
| `Report_SubItem_Name` | `Report_SubItem_Name` | `[Report_SubItem_Name]` |
| `Report_SubItem_Proportion` | `Report_SubItem_Proportion` | `[Report_SubItem_Proportion]` |
| `Task_Number` | `Task_Number` | `[Task_Number]` |

#### 수정 후 코드

```csharp
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;

namespace MdcHR26Apps.Models.EvaluationSubAgreement;

public class SubAgreementRepository(string connectionString, ILoggerFactory loggerFactory) : ISubAgreementRepository, IDisposable
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger(nameof(SubAgreementRepository));
    private readonly string dbContext = connectionString;

    #region + [1] 입력: AddAsync
    public async Task<SubAgreementDb> AddAsync(SubAgreementDb model)
    {
        try
        {
            const string query =
                "INSERT INTO SubAgreementDb(" +
                    "Uid, Report_Item_Number, Report_Item_Name_1, Report_Item_Name_2, Report_Item_Proportion, " +
                    "Report_SubItem_Name, Report_SubItem_Proportion, Task_Number) " +
                "VALUES(" +
                    "@Uid, @Report_Item_Number, @Report_Item_Name_1, @Report_Item_Name_2, @Report_Item_Proportion, " +
                    "@Report_SubItem_Name, @Report_SubItem_Proportion, @Task_Number);" +
                "Select Cast(SCOPE_IDENTITY() As Int);";

            using (var connection = new SqlConnection(dbContext))
            {
                int id = await connection.ExecuteScalarAsync<int>(query, model);
                model.Sid = id;
            }
        }
        catch (Exception e)
        {
            _logger?.LogError($"ERROR({nameof(AddAsync)}): {e.Message}");
        }

        return model;
    }
    #endregion

    #region + [2] 출력: GetByAllAsync
    public async Task<List<SubAgreementDb>> GetByAllAsync()
    {
        const string query = "Select * From SubAgreementDb Order By Sid";

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.QueryAsync<SubAgreementDb>(query, commandType: CommandType.Text);
            return result.ToList();
        }
    }
    #endregion

    #region + [3] 상세: GetByIdAsync
    public async Task<SubAgreementDb> GetByIdAsync(long id)
    {
        const string query = "Select * From SubAgreementDb Where Sid = @id";

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.QueryFirstOrDefaultAsync<SubAgreementDb>(query, new { id }, commandType: CommandType.Text);
            return result ?? new SubAgreementDb();
        }
    }
    #endregion

    #region + [4] 수정: UpdateAsync
    public async Task<bool> UpdateAsync(SubAgreementDb model)
    {
        const string query = @"
            Update SubAgreementDb
            Set
                Uid = @Uid,
                Report_Item_Number = @Report_Item_Number,
                Report_Item_Name_1 = @Report_Item_Name_1,
                Report_Item_Name_2 = @Report_Item_Name_2,
                Report_Item_Proportion = @Report_Item_Proportion,
                Report_SubItem_Name = @Report_SubItem_Name,
                Report_SubItem_Proportion = @Report_SubItem_Proportion,
                Task_Number = @Task_Number
            Where Sid = @Sid";
        try
        {
            using (var connection = new SqlConnection(dbContext))
            {
                return await connection.ExecuteAsync(query, model) > 0;
            }
        }
        catch (Exception e)
        {
            _logger?.LogError($"ERROR({nameof(UpdateAsync)}): {e.Message}");
        }

        return false;
    }
    #endregion

    #region + [5] 삭제: DeleteAsync
    public async Task<bool> DeleteAsync(long id)
    {
        const string query = "Delete SubAgreementDb Where Sid = @id";

        try
        {
            using (var connection = new SqlConnection(dbContext))
            {
                return await connection.ExecuteAsync(query, new { id }, commandType: CommandType.Text) > 0;
            }
        }
        catch (Exception er)
        {
            _logger?.LogError($"ERROR({nameof(DeleteAsync)}): {er.Message}");
        }

        return false;
    }
    #endregion

    #region + [6] 사용자별 출력: GetByUserIdAllAsync
    public async Task<List<SubAgreementDb>> GetByUserIdAllAsync(long userId)
    {
        const string query = "Select * From SubAgreementDb Where Uid = @userId Order By Sid";

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.QueryAsync<SubAgreementDb>(query, new { userId }, commandType: CommandType.Text);
            return result.ToList();
        }
    }
    #endregion

    #region + [7] 평가별 비중 출력: GetByTasksPeroportionAsync
    public async Task<List<SubAgreementDb>> GetByTasksPeroportionAsync(long userId, string deptName, string indexName)
    {
        const string query = @"
            Select Top(1) *
            From SubAgreementDb
            Where Uid = @userId
                And Report_Item_Name_1 = @deptName
                And Report_Item_Name_2 = @indexName
            Order By Sid";

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.QueryAsync<SubAgreementDb>(query, new { userId, deptName, indexName }, commandType: CommandType.Text);
            return result.ToList();
        }
    }
    #endregion

    #region + Dispose
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (db != null)
            {
                db.Dispose();
            }
        }
    }
    #endregion
}
```

#### Interface 수정

**파일**: `MdcHR26Apps.Models/EvaluationSubAgreement/ISubAgreementRepository.cs`

```csharp
namespace MdcHR26Apps.Models.EvaluationSubAgreement;

public interface ISubAgreementRepository : IDisposable
{
    Task<SubAgreementDb> AddAsync(SubAgreementDb model);
    Task<List<SubAgreementDb>> GetByAllAsync();
    Task<SubAgreementDb> GetByIdAsync(long id);
    Task<bool> UpdateAsync(SubAgreementDb model);
    Task<bool> DeleteAsync(long id);
    Task<List<SubAgreementDb>> GetByUserIdAllAsync(long userId);
    Task<List<SubAgreementDb>> GetByTasksPeroportionAsync(long userId, string deptName, string indexName);
}
```

---

### 2.3. DeptObjectiveRepository.cs

#### 파일: `MdcHR26Apps.Models/DeptObjective/DeptObjectiveRepository.cs`

#### 25년 프로젝트: Repository 없음 ⚠️
- 25년 프로젝트에는 DeptObjective 테이블의 Repository가 없음 (View만 존재)
- **26년에서 새로 추가된 테이블**
- 기본 CRUD (5개 메서드)만 작성

#### 26년 Entity 필드 (Entity 수정 완료)
- `DeptObjectiveDbId` (Int64) - PK
- `EDepartId` (Int64) - FK
- `ObjectiveTitle` (string)
- `ObjectiveContents` (string)
- `CreatedBy` (Int64)
- `CreatedAt` (DateTime)
- `UpdatedBy` (Int64?)
- `UpdatedAt` (DateTime?)
- `Remarks` (string?)

#### 수정 후 코드

```csharp
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;

namespace MdcHR26Apps.Models.DeptObjective;

public class DeptObjectiveRepository(string connectionString, ILoggerFactory loggerFactory) : IDeptObjectiveRepository, IDisposable
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger(nameof(DeptObjectiveRepository));
    private readonly string dbContext = connectionString;

    #region + [1] 입력: AddAsync
    public async Task<DeptObjectiveDb> AddAsync(DeptObjectiveDb model)
    {
        try
        {
            const string query =
                "INSERT INTO DeptObjectiveDb(" +
                    "EDepartId, ObjectiveTitle, ObjectiveContents, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, Remarks) " +
                "VALUES(" +
                    "@EDepartId, @ObjectiveTitle, @ObjectiveContents, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @Remarks);" +
                "Select Cast(SCOPE_IDENTITY() As Int);";

            using (var connection = new SqlConnection(dbContext))
            {
                int id = await connection.ExecuteScalarAsync<int>(query, model);
                model.DeptObjectiveDbId = id;
            }
        }
        catch (Exception e)
        {
            _logger?.LogError($"ERROR({nameof(AddAsync)}): {e.Message}");
        }

        return model;
    }
    #endregion

    #region + [2] 출력: GetByAllAsync
    public async Task<List<DeptObjectiveDb>> GetByAllAsync()
    {
        const string query = "Select * From DeptObjectiveDb Order By DeptObjectiveDbId";

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.QueryAsync<DeptObjectiveDb>(query, commandType: CommandType.Text);
            return result.ToList();
        }
    }
    #endregion

    #region + [3] 상세: GetByIdAsync
    public async Task<DeptObjectiveDb> GetByIdAsync(long id)
    {
        const string query = "Select * From DeptObjectiveDb Where DeptObjectiveDbId = @id";

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.QueryFirstOrDefaultAsync<DeptObjectiveDb>(query, new { id }, commandType: CommandType.Text);
            return result ?? new DeptObjectiveDb();
        }
    }
    #endregion

    #region + [4] 수정: UpdateAsync
    public async Task<bool> UpdateAsync(DeptObjectiveDb model)
    {
        const string query = @"
            Update DeptObjectiveDb
            Set
                EDepartId = @EDepartId,
                ObjectiveTitle = @ObjectiveTitle,
                ObjectiveContents = @ObjectiveContents,
                UpdatedBy = @UpdatedBy,
                UpdatedAt = @UpdatedAt,
                Remarks = @Remarks
            Where DeptObjectiveDbId = @DeptObjectiveDbId";
        try
        {
            using (var connection = new SqlConnection(dbContext))
            {
                return await connection.ExecuteAsync(query, model) > 0;
            }
        }
        catch (Exception e)
        {
            _logger?.LogError($"ERROR({nameof(UpdateAsync)}): {e.Message}");
        }

        return false;
    }
    #endregion

    #region + [5] 삭제: DeleteAsync
    public async Task<bool> DeleteAsync(long id)
    {
        const string query = "Delete DeptObjectiveDb Where DeptObjectiveDbId = @id";

        try
        {
            using (var connection = new SqlConnection(dbContext))
            {
                return await connection.ExecuteAsync(query, new { id }, commandType: CommandType.Text) > 0;
            }
        }
        catch (Exception er)
        {
            _logger?.LogError($"ERROR({nameof(DeleteAsync)}): {er.Message}");
        }

        return false;
    }
    #endregion

    #region + Dispose
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (db != null)
            {
                db.Dispose();
            }
        }
    }
    #endregion
}
```

#### Interface 수정

**파일**: `MdcHR26Apps.Models/DeptObjective/IDeptObjectiveRepository.cs`

```csharp
namespace MdcHR26Apps.Models.DeptObjective;

public interface IDeptObjectiveRepository : IDisposable
{
    Task<DeptObjectiveDb> AddAsync(DeptObjectiveDb model);
    Task<List<DeptObjectiveDb>> GetByAllAsync();
    Task<DeptObjectiveDb> GetByIdAsync(long id);
    Task<bool> UpdateAsync(DeptObjectiveDb model);
    Task<bool> DeleteAsync(long id);
}
```

---

### 2.4. EvaluationListsRepository.cs

#### 파일: `MdcHR26Apps.Models/EvaluationLists/EvaluationListsRepository.cs`

#### 25년 메서드 목록 (9개)
1. `AddAsync(EvaluationLists model)`
2. `GetByAllAsync()`
3. `GetByIdAsync(long id)`
4. `UpdateAsync(EvaluationLists model)`
5. `DeleteAsync(long id)`
6. `GetByDeptAllAsync()` - 지표구분 출력 (SelectListModel 반환)
7. `GetByDeptNumberAsync(string DeptName)` - 지표구분번호 조회
8. `GetByIndexAllAsync(int DeptNo)` - 직무구분 출력 (SelectListModel 반환)
9. `GetByTasksAsync(string DeptName, string IndexName)` - 평가지표 출력 (SelectListModel 반환)

#### 25년 vs 26년 Entity 필드 (동일)
| 25년 Entity | 26년 Entity | SQL 컬럼명 |
|------------|------------|-----------|
| `Eid` | `Eid` | `[Eid]` ✅ |
| `Evaluation_Department_Number` | `Evaluation_Department_Number` | `[Evaluation_Department_Number]` ✅ |
| `Evaluation_Department_Name` | `Evaluation_Department_Name` | `[Evaluation_Department_Name]` ✅ |
| `Evaluation_Index_Number` | `Evaluation_Index_Number` | `[Evaluation_Index_Number]` ✅ |
| `Evaluation_Index_Name` | `Evaluation_Index_Name` | `[Evaluation_Index_Name]` ✅ |
| `Evaluation_Task_Number` | `Evaluation_Task_Number` | `[Evaluation_Task_Number]` ✅ |
| `Evaluation_Task_Name` | `Evaluation_Task_Name` | `[Evaluation_Task_Name]` ✅ |
| `Evaluation_Lists_Remark` | `Evaluation_Lists_Remark` | `[Evaluation_Lists_Remark]` ✅ |

**핵심**: 25년과 26년 Entity 필드가 동일하므로 SQL 수정 불필요

#### 수정 후 코드

```csharp
using Dapper;
using MdcHR26Apps.Models.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;

namespace MdcHR26Apps.Models.EvaluationLists;

public class EvaluationListsRepository(string connectionString, ILoggerFactory loggerFactory) : IEvaluationListsRepository, IDisposable
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger(nameof(EvaluationListsRepository));
    private readonly string dbContext = connectionString;

    #region + [1] 입력: AddAsync
    public async Task<EvaluationLists> AddAsync(EvaluationLists model)
    {
        try
        {
            const string query =
                "INSERT INTO EvaluationLists(" +
                    "Evaluation_Department_Number, Evaluation_Department_Name, Evaluation_Index_Number, " +
                    "Evaluation_Index_Name, Evaluation_Task_Number, Evaluation_Task_Name, Evaluation_Lists_Remark) " +
                "VALUES(" +
                    "@Evaluation_Department_Number, @Evaluation_Department_Name, @Evaluation_Index_Number, " +
                    "@Evaluation_Index_Name, @Evaluation_Task_Number, @Evaluation_Task_Name, @Evaluation_Lists_Remark);" +
                "Select Cast(SCOPE_IDENTITY() As Int);";

            using (var connection = new SqlConnection(dbContext))
            {
                int id = await connection.ExecuteScalarAsync<int>(query, model);
                model.Eid = id;
            }
        }
        catch (Exception e)
        {
            _logger?.LogError($"ERROR({nameof(AddAsync)}): {e.Message}");
        }

        return model;
    }
    #endregion

    #region + [2] 출력: GetByAllAsync
    public async Task<List<EvaluationLists>> GetByAllAsync()
    {
        const string query = "Select * From EvaluationLists Order By Eid";

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.QueryAsync<EvaluationLists>(query, commandType: CommandType.Text);
            return result.ToList();
        }
    }
    #endregion

    #region + [3] 상세: GetByIdAsync
    public async Task<EvaluationLists> GetByIdAsync(long id)
    {
        const string query = "Select * From EvaluationLists Where Eid = @id";

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.QueryFirstOrDefaultAsync<EvaluationLists>(query, new { id }, commandType: CommandType.Text);
            return result ?? new EvaluationLists();
        }
    }
    #endregion

    #region + [4] 수정: UpdateAsync
    public async Task<bool> UpdateAsync(EvaluationLists model)
    {
        const string query = @"
            Update EvaluationLists
            Set
                Evaluation_Department_Number = @Evaluation_Department_Number,
                Evaluation_Department_Name = @Evaluation_Department_Name,
                Evaluation_Index_Number = @Evaluation_Index_Number,
                Evaluation_Index_Name = @Evaluation_Index_Name,
                Evaluation_Task_Number = @Evaluation_Task_Number,
                Evaluation_Task_Name = @Evaluation_Task_Name,
                Evaluation_Lists_Remark = @Evaluation_Lists_Remark
            Where Eid = @Eid";
        try
        {
            using (var connection = new SqlConnection(dbContext))
            {
                return await connection.ExecuteAsync(query, model) > 0;
            }
        }
        catch (Exception e)
        {
            _logger?.LogError($"ERROR({nameof(UpdateAsync)}): {e.Message}");
        }

        return false;
    }
    #endregion

    #region + [5] 삭제: DeleteAsync
    public async Task<bool> DeleteAsync(long id)
    {
        const string query = "Delete EvaluationLists Where Eid = @id";

        try
        {
            using (var connection = new SqlConnection(dbContext))
            {
                return await connection.ExecuteAsync(query, new { id }, commandType: CommandType.Text) > 0;
            }
        }
        catch (Exception er)
        {
            _logger?.LogError($"ERROR({nameof(DeleteAsync)}): {er.Message}");
        }

        return false;
    }
    #endregion

    #region + [6] 지표구분 출력: GetByDeptAllAsync
    public async Task<List<SelectListModel>> GetByDeptAllAsync()
    {
        const string query = @"
            Select DISTINCT Evaluation_Department_Number, Evaluation_Department_Name
            From EvaluationLists
            Order By Evaluation_Department_Number";

        List<SelectListModel> resultList = new();

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.QueryAsync<EvaluationLists>(query, commandType: CommandType.Text);

            foreach (var item in result)
            {
                resultList.Add(new SelectListModel
                {
                    SelectListNumber = item.Evaluation_Department_Number,
                    SelectListName = !string.IsNullOrEmpty(item.Evaluation_Department_Name) ?
                        item.Evaluation_Department_Name : string.Empty
                });
            }

            return resultList;
        }
    }
    #endregion

    #region + [7] 지표구분번호 출력: GetByDeptNumberAsync
    public async Task<int> GetByDeptNumberAsync(string deptName)
    {
        const string query = "Select DISTINCT Evaluation_Department_Number From EvaluationLists Where Evaluation_Department_Name = @deptName";

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.ExecuteScalarAsync<int>(query, new { deptName }, commandType: CommandType.Text);
            return result;
        }
    }
    #endregion

    #region + [8] 직무구분 출력: GetByIndexAllAsync
    public async Task<List<SelectListModel>> GetByIndexAllAsync(int deptNo)
    {
        const string query = @"
            Select DISTINCT Evaluation_Index_Number, Evaluation_Index_Name
            From EvaluationLists
            Where Evaluation_Department_Number = @deptNo
            Order By Evaluation_Index_Number";

        List<SelectListModel> resultList = new();

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.QueryAsync<EvaluationLists>(query, new { deptNo }, commandType: CommandType.Text);

            foreach (var item in result)
            {
                resultList.Add(new SelectListModel
                {
                    SelectListNumber = item.Evaluation_Index_Number,
                    SelectListName = !string.IsNullOrEmpty(item.Evaluation_Index_Name) ?
                        item.Evaluation_Index_Name : string.Empty
                });
            }

            return resultList;
        }
    }
    #endregion

    #region + [9] 평가지표 출력: GetByTasksAsync
    public async Task<List<SelectListModel>> GetByTasksAsync(string deptName, string indexName)
    {
        const string query = @"
            Select Evaluation_Task_Number, Evaluation_Task_Name
            From EvaluationLists
            Where Evaluation_Department_Name = @deptName And Evaluation_Index_Name = @indexName
            Order By Evaluation_Task_Number";

        List<SelectListModel> resultList = new();

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.QueryAsync<EvaluationLists>(query, new { deptName, indexName }, commandType: CommandType.Text);

            foreach (var item in result)
            {
                resultList.Add(new SelectListModel
                {
                    SelectListNumber = item.Evaluation_Task_Number,
                    SelectListName = !string.IsNullOrEmpty(item.Evaluation_Task_Name) ?
                        item.Evaluation_Task_Name : string.Empty
                });
            }

            return resultList;
        }
    }
    #endregion

    #region + Dispose
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (db != null)
            {
                db.Dispose();
            }
        }
    }
    #endregion
}
```

#### Interface 수정

**파일**: `MdcHR26Apps.Models/EvaluationLists/IEvaluationListsRepository.cs`

```csharp
using MdcHR26Apps.Models.Common;

namespace MdcHR26Apps.Models.EvaluationLists;

public interface IEvaluationListsRepository : IDisposable
{
    Task<EvaluationLists> AddAsync(EvaluationLists model);
    Task<List<EvaluationLists>> GetByAllAsync();
    Task<EvaluationLists> GetByIdAsync(long id);
    Task<bool> UpdateAsync(EvaluationLists model);
    Task<bool> DeleteAsync(long id);
    Task<List<SelectListModel>> GetByDeptAllAsync();
    Task<int> GetByDeptNumberAsync(string deptName);
    Task<List<SelectListModel>> GetByIndexAllAsync(int deptNo);
    Task<List<SelectListModel>> GetByTasksAsync(string deptName, string indexName);
}
```

---

### 2.5. TasksRepository.cs

#### 파일: `MdcHR26Apps.Models/EvaluationTasks/TasksRepository.cs`

#### 25년 메서드 목록 (7개)
1. `AddAsync(TasksDb model)`
2. `GetByAllAsync()`
3. `GetByIdAsync(long id)`
4. `UpdateAsync(TasksDb model)`
5. `DeleteAsync(long id)`
6. `GetByListNoAllAsync(long TaksListNumber)` - 리스트번호별 조회
7. `DeleteByListNoAllAsync(long TaksListNumber)` - 리스트번호별 삭제

#### 25년 vs 26년 Entity 필드 (동일)
| 25년 Entity | 26년 Entity | SQL 컬럼명 |
|------------|------------|-----------|
| `Tid` | `Tid` | `[Tid]` ✅ |
| `TaskName` | `TaskName` | `[TaskName]` ✅ |
| `TaksListNumber` | `TaksListNumber` | `[TaksListNumber]` ✅ (오타 그대로) |
| `TaskStatus` | `TaskStatus` | `[TaskStatus]` ✅ |
| `TaskObjective` | `TaskObjective` | `[TaskObjective]` ✅ |
| `TargetProportion` | `TargetProportion` | `[TargetProportion]` ✅ |
| `ResultProportion` | `ResultProportion` | `[ResultProportion]` ✅ |
| `TargetDate` | `TargetDate` | `[TargetDate]` ✅ |
| `ResultDate` | `ResultDate` | `[ResultDate]` ✅ |
| `Task_Evaluation_1` | `Task_Evaluation_1` | `[Task_Evaluation_1]` ✅ |
| `Task_Evaluation_2` | `Task_Evaluation_2` | `[Task_Evaluation_2]` ✅ |
| `TaskLevel` | `TaskLevel` | `[TaskLevel]` ✅ |
| `TaskComments` | `TaskComments` | `[TaskComments]` ✅ |

**핵심**: 25년과 26년 Entity 필드가 동일하므로 SQL 수정 불필요

#### 수정 후 코드

```csharp
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;

namespace MdcHR26Apps.Models.EvaluationTasks;

public class TasksRepository(string connectionString, ILoggerFactory loggerFactory) : ITasksRepository, IDisposable
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger(nameof(TasksRepository));
    private readonly string dbContext = connectionString;

    #region + [1] 입력: AddAsync
    public async Task<TasksDb> AddAsync(TasksDb model)
    {
        try
        {
            const string query =
                "INSERT INTO TasksDb(" +
                    "TaskName, TaksListNumber, TaskStatus, TaskObjective, TargetProportion, ResultProportion, " +
                    "TargetDate, ResultDate, Task_Evaluation_1, Task_Evaluation_2, TaskLevel, TaskComments) " +
                "VALUES(" +
                    "@TaskName, @TaksListNumber, @TaskStatus, @TaskObjective, @TargetProportion, @ResultProportion, " +
                    "@TargetDate, @ResultDate, @Task_Evaluation_1, @Task_Evaluation_2, @TaskLevel, @TaskComments);" +
                "Select Cast(SCOPE_IDENTITY() As Int);";

            using (var connection = new SqlConnection(dbContext))
            {
                int id = await connection.ExecuteScalarAsync<int>(query, model);
                model.Tid = id;
            }
        }
        catch (Exception e)
        {
            _logger?.LogError($"ERROR({nameof(AddAsync)}): {e.Message}");
        }

        return model;
    }
    #endregion

    #region + [2] 출력: GetByAllAsync
    public async Task<List<TasksDb>> GetByAllAsync()
    {
        const string query = "Select * From TasksDb Order By Tid";

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.QueryAsync<TasksDb>(query, commandType: CommandType.Text);
            return result.ToList();
        }
    }
    #endregion

    #region + [3] 상세: GetByIdAsync
    public async Task<TasksDb> GetByIdAsync(long id)
    {
        const string query = "Select * From TasksDb Where Tid = @id";

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.QueryFirstOrDefaultAsync<TasksDb>(query, new { id }, commandType: CommandType.Text);
            return result ?? new TasksDb();
        }
    }
    #endregion

    #region + [4] 수정: UpdateAsync
    public async Task<bool> UpdateAsync(TasksDb model)
    {
        const string query = @"
            Update TasksDb
            Set
                TaskName = @TaskName,
                TaksListNumber = @TaksListNumber,
                TaskStatus = @TaskStatus,
                TaskObjective = @TaskObjective,
                TargetProportion = @TargetProportion,
                ResultProportion = @ResultProportion,
                TargetDate = @TargetDate,
                ResultDate = @ResultDate,
                Task_Evaluation_1 = @Task_Evaluation_1,
                Task_Evaluation_2 = @Task_Evaluation_2,
                TaskLevel = @TaskLevel,
                TaskComments = @TaskComments
            Where Tid = @Tid";
        try
        {
            using (var connection = new SqlConnection(dbContext))
            {
                return await connection.ExecuteAsync(query, model) > 0;
            }
        }
        catch (Exception e)
        {
            _logger?.LogError($"ERROR({nameof(UpdateAsync)}): {e.Message}");
        }

        return false;
    }
    #endregion

    #region + [5] 삭제: DeleteAsync
    public async Task<bool> DeleteAsync(long id)
    {
        const string query = "Delete TasksDb Where Tid = @id";

        try
        {
            using (var connection = new SqlConnection(dbContext))
            {
                return await connection.ExecuteAsync(query, new { id }, commandType: CommandType.Text) > 0;
            }
        }
        catch (Exception er)
        {
            _logger?.LogError($"ERROR({nameof(DeleteAsync)}): {er.Message}");
        }

        return false;
    }
    #endregion

    #region + [6] 리스트번호별 출력: GetByListNoAllAsync
    public async Task<List<TasksDb>> GetByListNoAllAsync(long taksListNumber)
    {
        const string query = "Select * From TasksDb Where TaksListNumber = @taksListNumber Order By Tid";

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.QueryAsync<TasksDb>(query, new { taksListNumber }, commandType: CommandType.Text);
            return result.ToList();
        }
    }
    #endregion

    #region + [7] 리스트번호별 삭제: DeleteByListNoAllAsync
    public async Task<bool> DeleteByListNoAllAsync(long taksListNumber)
    {
        const string query = "Delete TasksDb Where TaksListNumber = @taksListNumber";

        try
        {
            using (var connection = new SqlConnection(dbContext))
            {
                return await connection.ExecuteAsync(query, new { taksListNumber }, commandType: CommandType.Text) > 0;
            }
        }
        catch (Exception er)
        {
            _logger?.LogError($"ERROR({nameof(DeleteByListNoAllAsync)}): {er.Message}");
        }

        return false;
    }
    #endregion

    #region + Dispose
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (db != null)
            {
                db.Dispose();
            }
        }
    }
    #endregion
}
```

#### Interface 수정

**파일**: `MdcHR26Apps.Models/EvaluationTasks/ITasksRepository.cs`

```csharp
namespace MdcHR26Apps.Models.EvaluationTasks;

public interface ITasksRepository : IDisposable
{
    Task<TasksDb> AddAsync(TasksDb model);
    Task<List<TasksDb>> GetByAllAsync();
    Task<TasksDb> GetByIdAsync(long id);
    Task<bool> UpdateAsync(TasksDb model);
    Task<bool> DeleteAsync(long id);
    Task<List<TasksDb>> GetByListNoAllAsync(long taksListNumber);
    Task<bool> DeleteByListNoAllAsync(long taksListNumber);
}
```

---

## 3. 영향 받는 코드

### 3.1. ReportInit.razor.cs

**파일**: `MdcHR26Apps.BlazorServer/Components/Pages/Admin/TotalReport/ReportInit.razor.cs`

#### 현재 사용 중인 메서드 (제거됨)
- Line 62: `agreementRepository.GetCountByUidAsync(uid)` → **제거됨**
- Line 105: `agreementRepository.DeleteAllByUidAsync(uid)` → **제거됨**
- Line 61: `subAgreementRepository.GetCountByUidAsync(uid)` → **제거됨**
- Line 102: `subAgreementRepository.DeleteAllByUidAsync(uid)` → **제거됨**

#### 수정 방법

**기존 코드**:
```csharp
int agreementCount = await agreementRepository.GetCountByUidAsync(uid);
int subAgreementCount = await subAgreementRepository.GetCountByUidAsync(uid);
```

**수정 후**:
```csharp
var agreements = await agreementRepository.GetByUserIdAllAsync(uid);
int agreementCount = agreements.Count;

var subAgreements = await subAgreementRepository.GetByUserIdAllAsync(uid);
int subAgreementCount = subAgreements.Count;
```

**기존 코드**:
```csharp
await agreementRepository.DeleteAllByUidAsync(uid);
await subAgreementRepository.DeleteAllByUidAsync(uid);
```

**수정 후**:
```csharp
// Agreement 삭제
var agreements = await agreementRepository.GetByUserIdAllAsync(uid);
foreach (var agreement in agreements)
{
    await agreementRepository.DeleteAsync(agreement.Aid);
}

// SubAgreement 삭제
var subAgreements = await subAgreementRepository.GetByUserIdAllAsync(uid);
foreach (var subAgreement in subAgreements)
{
    await subAgreementRepository.DeleteAsync(subAgreement.Sid);
}
```

---

## 4. 테스트 시나리오

### 4.1. 빌드 검증
```bash
dotnet build MdcHR26Apps.Models
```
- ✅ 컴파일 오류 없음

### 4.2. Repository 메서드 검증
- [ ] AddAsync 테스트 (삽입 후 ID 반환 확인)
- [ ] GetByAllAsync 테스트 (전체 조회)
- [ ] GetByIdAsync 테스트 (단일 조회)
- [ ] UpdateAsync 테스트 (수정)
- [ ] DeleteAsync 테스트 (삭제)
- [ ] GetByUserIdAllAsync 테스트 (사용자별 조회)
- [ ] GetByTasksPeroportionAsync 테스트 (필터 조회)

---

## 5. 완료 조건

- [ ] AgreementRepository.cs 수정 완료
- [ ] IAgreementRepository.cs 수정 완료
- [ ] SubAgreementRepository.cs 수정 완료
- [ ] ISubAgreementRepository.cs 수정 완료
- [ ] DeptObjectiveRepository.cs 수정 완료 (25년 확인 후)
- [ ] IDeptObjectiveRepository.cs 수정 완료
- [ ] EvaluationListsRepository.cs 수정 완료 (25년 확인 후)
- [ ] IEvaluationListsRepository.cs 수정 완료
- [ ] TasksRepository.cs 수정 완료 (25년 확인 후)
- [ ] ITasksRepository.cs 수정 완료
- [ ] ReportInit.razor.cs 수정 완료
- [ ] 빌드 성공 (MdcHR26Apps.Models)
- [ ] 빌드 성공 (MdcHR26Apps.BlazorServer)
- [ ] 관련 이슈 업데이트

---

## 6. 관련 문서

**이슈**:
- [#009: Phase 3 WebApp 개발](../issues/009_phase3_webapp_development.md)

**선행 작업지시서**:
- [20260203_11: Entity 필드명 수정](20260203_11_fix_entity_db_field_names.md)

**25년 프로젝트 (기준)**:
- `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.Models\Agreement\AgreementDbRepositoryDapperAsync.cs`
- `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.Models\SubAgreement\SubAgreementDbRepositoryDapperAsync.cs`
- `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.Models\DeptObjective\DeptObjectiveDbRepositoryDapperAsync.cs`
- `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.Models\Evaluation\EvaluationListsDbRepositoryDapperAsync.cs`
- `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.Models\Tasks\TasksDbRepositoryDapperAsync.cs`

**26년 프로젝트 (수정 대상)**:
- `MdcHR26Apps.Models/EvaluationAgreement/AgreementRepository.cs`
- `MdcHR26Apps.Models/EvaluationSubAgreement/SubAgreementRepository.cs`
- `MdcHR26Apps.Models/DeptObjective/DeptObjectiveRepository.cs`
- `MdcHR26Apps.Models/EvaluationLists/EvaluationListsRepository.cs`
- `MdcHR26Apps.Models/EvaluationTasks/TasksRepository.cs`

**영향 받는 페이지**:
- `MdcHR26Apps.BlazorServer/Components/Pages/Admin/TotalReport/ReportInit.razor.cs`

---

## 7. 주의사항

1. **25년 메서드 기준**: 메서드 개수, 이름, 시그니처를 25년과 동일하게 유지
2. **SQL 쿼리 수정**: 26년 Entity 필드명에 맞춰 수정 (Uid, Report_Item_*)
3. **Navigation Property**: Dapper는 자동 매핑하지 않으므로 Entity의 Navigation Property는 null로 유지
4. **Primary Constructor**: C# 13 스타일 적용
5. **25년 확인 필요**: DeptObjective, EvaluationLists, Tasks는 25년 프로젝트 확인 후 작성
