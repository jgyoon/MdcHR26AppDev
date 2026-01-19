# 작업지시서: Phase 2 - Model 개발 (Dapper)

**날짜**: 2026-01-16
**작업 유형**: 아키텍처 구축 / 기능 추가
**관련 이슈**: [#008: Phase 2 Model 개발](../issues/008_phase2_model_development.md)
**관련 작업지시서**:
- `20251216_02_phase1_database_design.md` (선행 작업)
- `20260114_02_remove_memberdb_optimize_structure.md` (선행 작업)

---

## 1. 작업 개요

### 1.1. 목표
현재 프로젝트(2026년 인사평가프로그램)의 데이터베이스를 기반으로 Dapper를 사용한 Model 계층을 개발합니다.

### 1.2. 참조 프로젝트

| 프로젝트 | 경로 | 참조 목적 |
|---------|------|----------|
| **2025년 인사평가** | `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.Models` | 비즈니스 로직 및 구조 참조 |
| **도서관리(최신)** | `C:\Codes\36_MdcLibrary\MdcLibrary\MdcLibrary.Model` | 최신 기술 및 패턴 참조 |

### 1.3. 분석 완료 사항
- ✅ 현재 프로젝트 DB 구조 분석 완료
- ✅ 2025년 Model 프로젝트 분석 완료
- ✅ 도서관리 Model 프로젝트 분석 완료

---

## 2. 현재 프로젝트 데이터베이스 구조

### 2.1. 테이블 목록 (13개)

| 번호 | 테이블명 | 역할 | PK | 비고 |
|------|---------|------|-----|------|
| 1 | **EDepartmentDb** | 부서 마스터 | EDepartId | 마스터 데이터 |
| 2 | **ERankDb** | 직급 마스터 | ERankId | 마스터 데이터 |
| 3 | **UserDb** | 사용자/직원 정보 | Uid | **EStatus(BIT)** 포함 |
| 4 | **DeptObjectiveDb** | 부서 목표 | DeptObjectiveDbId | 감사 필드 포함 |
| 5 | **AgreementDb** | 직무평가 협의서 | Aid | - |
| 6 | **SubAgreementDb** | 상세 직무평가 협의서 | Sid | - |
| 7 | **ProcessDb** | 평가 프로세스 관리 | Pid | - |
| 8 | **ReportDb** | 평가 보고서 | Rid | - |
| 9 | **TotalReportDb** | 종합 보고서 | TRid | - |
| 10 | **EvaluationUsers** | 평가자 관리 | EUid | - |
| 11 | **TasksDb** | 업무 관리 | Tid | - |
| 12 | **EvaluationLists** | 평가 항목 마스터 | Eid | - |
| 13 | ~~**MemberDb**~~ | ~~회원 정보~~ | - | **#007에서 삭제됨** |

**중요**:
- **EStatusDb 테이블은 존재하지 않음** - 재직 상태는 `UserDb.EStatus` (BIT) 컬럼으로 관리
- MemberDb는 #007 이슈에서 제거되어 현재 테이블 개수는 **12개**

### 2.2. 뷰(View) 목록 (5개)

| 번호 | 뷰명 | 역할 |
|------|------|------|
| 1 | **v_MemberListDB** | 사용자 목록 (UserDb + EDepartmentDb + ERankDb 조인, EStatus=1 필터링) |
| 2 | **v_DeptObjectiveListDb** | 부서 목표 목록 (DeptObjectiveDb + EDepartmentDb 조인) |
| 3 | **v_ProcessTRListDB** | 평가 프로세스 & 종합 보고서 (ProcessDb + UserDb + TotalReportDb 조인) |
| 4 | **v_ReportTaskListDB** | 평가 보고서 & 업무 목록 (ReportDb + UserDb + TasksDb 조인) |
| 5 | **v_TotalReportListDB** | 종합 보고서 목록 (TotalReportDb + UserDb 조인) |

---

## 3. 개발 구조 설계

### 3.1. 프로젝트 구조

```
MdcHR26Apps.Models/
├── MdcHR26Apps.Models.csproj
├── MdcHR26AppsAddDbContext.cs              # EF Core DbContext
├── MdcHR26AppsAddExtensions.cs             # DI 컨테이너 설정
├── Common/
│   └── SelectListModel.cs                  # 공통 드롭다운 모델
├── User/
│   ├── UserDb.cs                           # Entity + Repository + Interface
│   ├── IUserRepository.cs
│   └── UserRepository.cs
├── Department/
│   ├── EDepartmentDb.cs
│   ├── IEDepartmentRepository.cs
│   └── EDepartmentRepository.cs
├── Rank/
│   ├── ERankDb.cs
│   ├── IERankRepository.cs
│   └── ERankRepository.cs
├── DeptObjective/
│   ├── DeptObjectiveDb.cs
│   ├── IDeptObjectiveRepository.cs
│   └── DeptObjectiveRepository.cs
├── EvaluationLists/
│   ├── EvaluationLists.cs
│   ├── IEvaluationListsRepository.cs
│   └── EvaluationListsRepository.cs
├── EvaluationProcess/
│   ├── ProcessDb.cs
│   ├── IProcessRepository.cs
│   └── ProcessRepository.cs
├── EvaluationReport/
│   ├── ReportDb.cs
│   ├── IReportRepository.cs
│   └── ReportRepository.cs
├── EvaluationAgreement/
│   ├── AgreementDb.cs
│   ├── IAgreementRepository.cs
│   └── AgreementRepository.cs
├── EvaluationSubAgreement/
│   ├── SubAgreementDb.cs
│   ├── ISubAgreementRepository.cs
│   └── SubAgreementRepository.cs
├── EvaluationTasks/
│   ├── TasksDb.cs
│   ├── ITasksRepository.cs
│   └── TasksRepository.cs
├── EvaluationUsers/
│   ├── EvaluationUsers.cs
│   ├── IEvaluationUsersRepository.cs
│   └── EvaluationUsersRepository.cs
├── Result/
│   ├── TotalReportDb.cs
│   ├── ITotalReportRepository.cs
│   └── TotalReportRepository.cs
└── Views/
    ├── v_MemberListDB.cs                       # 사용자 목록 뷰 모델
    ├── Iv_MemberListRepository.cs
    ├── v_MemberListRepository.cs
    ├── v_DeptObjectiveListDb.cs                # 부서 목표 목록 뷰 모델
    ├── Iv_DeptObjectiveListRepository.cs
    ├── v_DeptObjectiveListRepository.cs
    ├── v_ProcessTRListDB.cs                    # 프로세스 & 종합 보고서 뷰 모델
    ├── Iv_ProcessTRListRepository.cs
    ├── v_ProcessTRListRepository.cs
    ├── v_ReportTaskListDB.cs                   # 평가 보고서 & 업무 목록 뷰 모델
    ├── Iv_ReportTaskListRepository.cs
    ├── v_ReportTaskListRepository.cs
    ├── v_TotalReportListDB.cs                  # 종합 보고서 목록 뷰 모델
    ├── Iv_TotalReportListRepository.cs
    └── v_TotalReportListRepository.cs
```

**주의**: `Status/` 폴더와 `EStatusDb` 관련 파일은 **존재하지 않음** (재직 상태는 `UserDb.EStatus` 컬럼으로 관리)

### 3.2. 네이밍 규칙

| 항목 | 규칙 | 예시 |
|------|------|------|
| Entity 클래스 | `{TableName}` | `UserDb`, `ReportDb` |
| View 모델 | `v_{ViewName}` | `v_MemberListDB` |
| Interface | `I{Entity}Repository` | `IUserRepository` |
| Repository 구현 | `{Entity}Repository` | `UserRepository` |
| PK 필드 | `{TableName}Id` 또는 단축형 | `Uid`, `Rid`, `Aid` |
| FK 필드 | `{ReferencedTable}Id` | `EDepartId`, `ERankId` |
| Boolean 필드 | `Is{Property}` | `IsAdministrator`, `IsTeamLeader` |

---

## 4. 기술 스택 및 패턴

### 4.1. NuGet 패키지

```xml
<PackageReference Include="Dapper" Version="2.1.66" />
<PackageReference Include="Microsoft.Data.SqlClient" Version="6.1.1" />
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="10.0.0" />
<PackageReference Include="System.Configuration.ConfigurationManager" Version="10.0.0" />
```

### 4.2. 프레임워크 설정

```xml
<TargetFramework>net10.0</TargetFramework>
<ImplicitUsings>enable</ImplicitUsings>
<Nullable>enable</Nullable>
<LangVersion>latest</LangVersion>
```

### 4.3. .NET 10 새로운 기능 활용

#### 1) Primary Constructors (클래스)
```csharp
// .NET 10에서 클래스에도 Primary Constructor 사용 가능
public class UserRepository(string connectionString, ILoggerFactory loggerFactory)
    : IUserRepository, IDisposable
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<UserRepository>();
    private readonly string dbContext = connectionString;

    // ...
}
```

#### 2) Collection Expressions
```csharp
// 간결한 컬렉션 초기화
var departments = [dept1, dept2, dept3];
var emptyList = [];
```

#### 3) Improved Pattern Matching
```csharp
// 더 강력한 패턴 매칭
public async Task<UserDb> GetUserAsync(object identifier) => identifier switch
{
    Int64 id => await GetByIdAsync(id),
    string userId => await GetByUserIdAsync(userId),
    _ => throw new ArgumentException("Invalid identifier type")
};
```

### 4.4. 아키텍처 패턴

#### 1) Repository 패턴
- 각 Entity별로 Interface + Repository 구조
- Dapper를 사용한 고성능 데이터 액세스
- 비동기 메서드 (async/await)
- .NET 10 Primary Constructor 활용

#### 2) 의존성 주입 (DI)
- ASP.NET Core DI 컨테이너 사용
- Repository를 Singleton으로 등록
- LoggerFactory 통합
- Keyed Services (.NET 10) 활용 가능

#### 3) EF Core + Dapper 하이브리드
- EF Core 10.0: DbContext, 마이그레이션 용도
- Dapper: 주요 데이터 액세스 계층 (고성능)

### 4.5. .NET 10 주요 개선사항

#### 성능 개선
- **JIT 최적화**: 더 빠른 시작 시간 및 실행 속도
- **GC 개선**: 메모리 사용량 감소
- **LINQ 성능 향상**: 더 효율적인 쿼리 처리

#### 언어 기능
- **Primary Constructors for Classes**: 간결한 생성자
- **Collection Expressions**: `[]` 문법
- **Improved Pattern Matching**: 더 강력한 패턴 매칭
- **Enhanced Using Directives**: 전역 using 개선

#### EF Core 10.0 신기능
- **Complex Types**: 값 객체 지원 강화
- **Bulk Operations**: 대량 데이터 처리 성능 향상
- **Raw SQL Improvements**: 더 안전한 Raw SQL
- **JSON Columns**: JSON 데이터 타입 개선

---

## 5. 상세 구현 가이드

### 5.1. Entity 클래스 (UserDb 예시)

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.User
{
    #region + DB정보
    // [Uid] BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    // [UserId] NVARCHAR(50) UNIQUE NOT NULL,
    // [UserName] NVARCHAR(100) NOT NULL,
    // [UserPassword] VARBINARY(128) NOT NULL,
    // [UserPasswordSalt] VARBINARY(16) NOT NULL,
    // [Email] NVARCHAR(100) NULL,
    // [EDepartId] INT NOT NULL FOREIGN KEY REFERENCES EDepartmentDb(EDepartId),
    // [ERankId] INT NOT NULL FOREIGN KEY REFERENCES ERankDb(ERankId),
    // [EStatusId] INT NOT NULL FOREIGN KEY REFERENCES EStatusDb(EStatusId),
    // [IsTeamLeader] BIT NOT NULL DEFAULT 0,
    // [IsDirector] BIT NOT NULL DEFAULT 0,
    // [IsAdministrator] BIT NOT NULL DEFAULT 0,
    // [IsDeptObjectiveWriter] BIT NOT NULL DEFAULT 0,
    // [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    // [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE()
    #endregion
    public class UserDb
    {
        public Int64 Uid { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public byte[] UserPassword { get; set; } = Array.Empty<byte>();
        public byte[] UserPasswordSalt { get; set; } = Array.Empty<byte>();
        public string? Email { get; set; }
        public int EDepartId { get; set; }
        public int ERankId { get; set; }
        public int EStatusId { get; set; }
        public bool IsTeamLeader { get; set; }
        public bool IsDirector { get; set; }
        public bool IsAdministrator { get; set; }
        public bool IsDeptObjectiveWriter { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        // 비밀번호 입력용 (DB 저장 안 됨)
        [NotMapped]
        public string Password { get; set; } = string.Empty;
    }
}
```

### 5.2. Repository Interface (IUserRepository)

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MdcHR26Apps.Models.User
{
    public interface IUserRepository
    {
        #region [1] CRUD 기본 메서드
        Task<UserDb> AddAsync(UserDb model);
        Task<List<UserDb>> GetByAllAsync();
        Task<UserDb> GetByIdAsync(Int64 id);
        Task<bool> UpdateAsync(UserDb model);
        Task<bool> UpdateWithoutPasswordAsync(UserDb model);
        Task<bool> DeleteAsync(Int64 id);
        #endregion

        #region [2] 검색 및 조회 메서드
        Task<UserDb> GetByUserIdAsync(string userId);
        Task<List<UserDb>> GetByDepartmentAsync(int departId);
        Task<List<UserDb>> GetByRankAsync(int rankId);
        Task<List<UserDb>> GetByStatusAsync(int statusId);
        Task<List<UserDb>> GetTeamLeadersAsync();
        Task<List<UserDb>> GetDirectorsAsync();
        Task<List<UserDb>> GetAdministratorsAsync();
        Task<List<UserDb>> GetDeptObjectiveWritersAsync();
        Task<List<UserDb>> SearchByNameAsync(string name);
        #endregion

        #region [3] 비즈니스 메서드
        Task<bool> LoginCheck(string userId, string password);
        Task<bool> IsUserIdExistsAsync(string userId);
        Task<bool> UpdatePasswordAsync(Int64 uid, string newPassword);
        Task<int> GetCountByDepartmentAsync(int departId);
        #endregion

        #region [#] 리소스 해제
        void Dispose();
        #endregion
    }
}
```

### 5.3. Repository 구현 (UserRepository - .NET 10 버전)

```csharp
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MdcHR26Apps.Models.User
{
    /// <summary>
    /// UserDb Repository - .NET 10 Primary Constructor 사용
    /// </summary>
    public class UserRepository(string connectionString, ILoggerFactory loggerFactory)
        : IUserRepository, IDisposable
    {
        private readonly SqlConnection db = new(connectionString);
        private readonly ILogger _logger = loggerFactory.CreateLogger<UserRepository>();
        private readonly string dbContext = connectionString;

        #region [1] CRUD 기본 메서드

        /// <summary>
        /// 사용자 추가 (SHA-256 + Salt)
        /// </summary>
        public async Task<UserDb> AddAsync(UserDb model)
        {
            // Step 1: Salt 생성 (16바이트)
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            model.UserPasswordSalt = salt;

            // Step 2: SHA-256 해싱
            byte[] hashedPassword;
            using (var sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.Unicode.GetBytes(model.Password);
                byte[] passwordWithSalt = new byte[passwordBytes.Length + salt.Length];
                Buffer.BlockCopy(passwordBytes, 0, passwordWithSalt, 0, passwordBytes.Length);
                Buffer.BlockCopy(salt, 0, passwordWithSalt, passwordBytes.Length, salt.Length);
                hashedPassword = sha256.ComputeHash(passwordWithSalt);
            }

            try
            {
                const string query = @"
                    INSERT INTO UserDb (
                        UserId, UserName, UserPassword, UserPasswordSalt,
                        Email, EDepartId, ERankId, EStatusId,
                        IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter,
                        CreatedDate, ModifiedDate
                    )
                    VALUES (
                        @UserId, @UserName, @UserPassword, @UserPasswordSalt,
                        @Email, @EDepartId, @ERankId, @EStatusId,
                        @IsTeamLeader, @IsDirector, @IsAdministrator, @IsDeptObjectiveWriter,
                        GETDATE(), GETDATE()
                    );
                    SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

                using (var connection = new SqlConnection(dbContext))
                {
                    Int64 id = await connection.ExecuteScalarAsync<Int64>(query, new
                    {
                        model.UserId,
                        model.UserName,
                        UserPassword = hashedPassword,
                        UserPasswordSalt = salt,
                        model.Email,
                        model.EDepartId,
                        model.ERankId,
                        model.EStatusId,
                        model.IsTeamLeader,
                        model.IsDirector,
                        model.IsAdministrator,
                        model.IsDeptObjectiveWriter
                    });

                    model.Uid = id;
                }
            }
            catch (Exception e)
            {
                _logger?.LogError($"ERROR({nameof(AddAsync)}): {e.Message}");
            }

            return model;
        }

        /// <summary>
        /// 모든 사용자 조회
        /// </summary>
        public async Task<List<UserDb>> GetByAllAsync()
        {
            const string query = "SELECT * FROM UserDb ORDER BY Uid";

            using (var connection = new SqlConnection(dbContext))
            {
                var result = await connection.QueryAsync<UserDb>(query, commandType: CommandType.Text);
                return result.ToList();
            }
        }

        /// <summary>
        /// ID로 사용자 조회
        /// </summary>
        public async Task<UserDb> GetByIdAsync(Int64 id)
        {
            const string query = "SELECT * FROM UserDb WHERE Uid = @id";

            using (var connection = new SqlConnection(dbContext))
            {
                var result = await connection.QueryFirstOrDefaultAsync<UserDb>(
                    query, new { id }, commandType: CommandType.Text);
                return result ?? new UserDb();
            }
        }

        /// <summary>
        /// 사용자 정보 업데이트 (비밀번호 포함)
        /// </summary>
        public async Task<bool> UpdateAsync(UserDb model)
        {
            // 비밀번호 해싱
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            byte[] hashedPassword;
            using (var sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.Unicode.GetBytes(model.Password);
                byte[] passwordWithSalt = new byte[passwordBytes.Length + salt.Length];
                Buffer.BlockCopy(passwordBytes, 0, passwordWithSalt, 0, passwordBytes.Length);
                Buffer.BlockCopy(salt, 0, passwordWithSalt, passwordBytes.Length, salt.Length);
                hashedPassword = sha256.ComputeHash(passwordWithSalt);
            }

            const string query = @"
                UPDATE UserDb
                SET
                    UserId = @UserId,
                    UserName = @UserName,
                    UserPassword = @UserPassword,
                    UserPasswordSalt = @UserPasswordSalt,
                    Email = @Email,
                    EDepartId = @EDepartId,
                    ERankId = @ERankId,
                    EStatusId = @EStatusId,
                    IsTeamLeader = @IsTeamLeader,
                    IsDirector = @IsDirector,
                    IsAdministrator = @IsAdministrator,
                    IsDeptObjectiveWriter = @IsDeptObjectiveWriter,
                    ModifiedDate = GETDATE()
                WHERE Uid = @Uid";

            using (var connection = new SqlConnection(dbContext))
            {
                return await connection.ExecuteAsync(query, new
                {
                    model.Uid,
                    model.UserId,
                    model.UserName,
                    UserPassword = hashedPassword,
                    UserPasswordSalt = salt,
                    model.Email,
                    model.EDepartId,
                    model.ERankId,
                    model.EStatusId,
                    model.IsTeamLeader,
                    model.IsDirector,
                    model.IsAdministrator,
                    model.IsDeptObjectiveWriter
                }) > 0;
            }
        }

        /// <summary>
        /// 사용자 정보 업데이트 (비밀번호 제외)
        /// </summary>
        public async Task<bool> UpdateWithoutPasswordAsync(UserDb model)
        {
            const string query = @"
                UPDATE UserDb
                SET
                    UserId = @UserId,
                    UserName = @UserName,
                    Email = @Email,
                    EDepartId = @EDepartId,
                    ERankId = @ERankId,
                    EStatusId = @EStatusId,
                    IsTeamLeader = @IsTeamLeader,
                    IsDirector = @IsDirector,
                    IsAdministrator = @IsAdministrator,
                    IsDeptObjectiveWriter = @IsDeptObjectiveWriter,
                    ModifiedDate = GETDATE()
                WHERE Uid = @Uid";

            using (var connection = new SqlConnection(dbContext))
            {
                return await connection.ExecuteAsync(query, model) > 0;
            }
        }

        /// <summary>
        /// 사용자 삭제
        /// </summary>
        public async Task<bool> DeleteAsync(Int64 id)
        {
            const string query = "DELETE FROM UserDb WHERE Uid = @id";

            using (var connection = new SqlConnection(dbContext))
            {
                return await connection.ExecuteAsync(query, new { id }, commandType: CommandType.Text) > 0;
            }
        }

        #endregion

        #region [2] 검색 및 조회 메서드

        /// <summary>
        /// UserId로 사용자 조회
        /// </summary>
        public async Task<UserDb> GetByUserIdAsync(string userId)
        {
            const string query = "SELECT * FROM UserDb WHERE UserId = @userId";

            using (var connection = new SqlConnection(dbContext))
            {
                var result = await connection.QueryFirstOrDefaultAsync<UserDb>(
                    query, new { userId }, commandType: CommandType.Text);
                return result ?? new UserDb();
            }
        }

        /// <summary>
        /// 부서별 사용자 조회
        /// </summary>
        public async Task<List<UserDb>> GetByDepartmentAsync(int departId)
        {
            const string query = "SELECT * FROM UserDb WHERE EDepartId = @departId ORDER BY Uid";

            using (var connection = new SqlConnection(dbContext))
            {
                var result = await connection.QueryAsync<UserDb>(
                    query, new { departId }, commandType: CommandType.Text);
                return result.ToList();
            }
        }

        /// <summary>
        /// 직급별 사용자 조회
        /// </summary>
        public async Task<List<UserDb>> GetByRankAsync(int rankId)
        {
            const string query = "SELECT * FROM UserDb WHERE ERankId = @rankId ORDER BY Uid";

            using (var connection = new SqlConnection(dbContext))
            {
                var result = await connection.QueryAsync<UserDb>(
                    query, new { rankId }, commandType: CommandType.Text);
                return result.ToList();
            }
        }

        /// <summary>
        /// 재직상태별 사용자 조회 (EStatus: BIT - 1=재직, 0=퇴사)
        /// </summary>
        public async Task<List<UserDb>> GetByStatusAsync(bool isActive)
        {
            const string query = "SELECT * FROM UserDb WHERE EStatus = @isActive ORDER BY Uid";

            using (var connection = new SqlConnection(dbContext))
            {
                var result = await connection.QueryAsync<UserDb>(
                    query, new { isActive }, commandType: CommandType.Text);
                return result.ToList();
            }
        }

        /// <summary>
        /// 재직 중인 사용자만 조회
        /// </summary>
        public async Task<List<UserDb>> GetActiveUsersAsync()
        {
            return await GetByStatusAsync(true);
        }

        /// <summary>
        /// 팀장/부서장 목록 조회
        /// </summary>
        public async Task<List<UserDb>> GetTeamLeadersAsync()
        {
            const string query = "SELECT * FROM UserDb WHERE IsTeamLeader = 1 ORDER BY Uid";

            using (var connection = new SqlConnection(dbContext))
            {
                var result = await connection.QueryAsync<UserDb>(query, commandType: CommandType.Text);
                return result.ToList();
            }
        }

        /// <summary>
        /// 임원 목록 조회
        /// </summary>
        public async Task<List<UserDb>> GetDirectorsAsync()
        {
            const string query = "SELECT * FROM UserDb WHERE IsDirector = 1 ORDER BY Uid";

            using (var connection = new SqlConnection(dbContext))
            {
                var result = await connection.QueryAsync<UserDb>(query, commandType: CommandType.Text);
                return result.ToList();
            }
        }

        /// <summary>
        /// 관리자 목록 조회
        /// </summary>
        public async Task<List<UserDb>> GetAdministratorsAsync()
        {
            const string query = "SELECT * FROM UserDb WHERE IsAdministrator = 1 ORDER BY Uid";

            using (var connection = new SqlConnection(dbContext))
            {
                var result = await connection.QueryAsync<UserDb>(query, commandType: CommandType.Text);
                return result.ToList();
            }
        }

        /// <summary>
        /// 부서 목표 작성자 목록 조회
        /// </summary>
        public async Task<List<UserDb>> GetDeptObjectiveWritersAsync()
        {
            const string query = "SELECT * FROM UserDb WHERE IsDeptObjectiveWriter = 1 ORDER BY Uid";

            using (var connection = new SqlConnection(dbContext))
            {
                var result = await connection.QueryAsync<UserDb>(query, commandType: CommandType.Text);
                return result.ToList();
            }
        }

        /// <summary>
        /// 이름으로 검색 (부분 일치)
        /// </summary>
        public async Task<List<UserDb>> SearchByNameAsync(string name)
        {
            string searchName = name + "%";
            const string query = "SELECT * FROM UserDb WHERE UserName LIKE @searchName ORDER BY Uid";

            using (var connection = new SqlConnection(dbContext))
            {
                var result = await connection.QueryAsync<UserDb>(
                    query, new { searchName }, commandType: CommandType.Text);
                return result.ToList();
            }
        }

        #endregion

        #region [3] 비즈니스 메서드

        /// <summary>
        /// 로그인 확인 (SHA-256 + Salt 검증)
        /// </summary>
        public async Task<bool> LoginCheck(string userId, string password)
        {
            const string query = @"
                SELECT UserPasswordSalt, UserPassword
                FROM UserDb
                WHERE UserId = @userId";

            using (var connection = new SqlConnection(dbContext))
            {
                var result = await connection.QueryFirstOrDefaultAsync(query, new { userId });

                if (result == null)
                    return false;

                byte[] salt = result.UserPasswordSalt;
                byte[] storedHashedPassword = result.UserPassword;

                // 입력 비밀번호 해싱
                byte[] inputPasswordBytes = Encoding.Unicode.GetBytes(password);
                byte[] passwordWithSalt = new byte[inputPasswordBytes.Length + salt.Length];
                Buffer.BlockCopy(inputPasswordBytes, 0, passwordWithSalt, 0, inputPasswordBytes.Length);
                Buffer.BlockCopy(salt, 0, passwordWithSalt, inputPasswordBytes.Length, salt.Length);

                using (var sha256 = SHA256.Create())
                {
                    byte[] hashedInputPassword = sha256.ComputeHash(passwordWithSalt);
                    return System.Collections.StructuralComparisons.StructuralEqualityComparer.Equals(
                        hashedInputPassword, storedHashedPassword);
                }
            }
        }

        /// <summary>
        /// UserId 중복 확인
        /// </summary>
        public async Task<bool> IsUserIdExistsAsync(string userId)
        {
            const string query = "SELECT COUNT(*) FROM UserDb WHERE UserId = @userId";

            using (var connection = new SqlConnection(dbContext))
            {
                int count = await connection.ExecuteScalarAsync<int>(query, new { userId });
                return count > 0;
            }
        }

        /// <summary>
        /// 비밀번호만 변경
        /// </summary>
        public async Task<bool> UpdatePasswordAsync(Int64 uid, string newPassword)
        {
            // Salt 생성 및 해싱
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            byte[] hashedPassword;
            using (var sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.Unicode.GetBytes(newPassword);
                byte[] passwordWithSalt = new byte[passwordBytes.Length + salt.Length];
                Buffer.BlockCopy(passwordBytes, 0, passwordWithSalt, 0, passwordBytes.Length);
                Buffer.BlockCopy(salt, 0, passwordWithSalt, passwordBytes.Length, salt.Length);
                hashedPassword = sha256.ComputeHash(passwordWithSalt);
            }

            const string query = @"
                UPDATE UserDb
                SET
                    UserPassword = @hashedPassword,
                    UserPasswordSalt = @salt,
                    ModifiedDate = GETDATE()
                WHERE Uid = @uid";

            using (var connection = new SqlConnection(dbContext))
            {
                return await connection.ExecuteAsync(query, new { uid, hashedPassword, salt }) > 0;
            }
        }

        /// <summary>
        /// 부서별 사용자 수 조회
        /// </summary>
        public async Task<int> GetCountByDepartmentAsync(int departId)
        {
            const string query = "SELECT COUNT(*) FROM UserDb WHERE EDepartId = @departId";

            using (var connection = new SqlConnection(dbContext))
            {
                return await connection.ExecuteScalarAsync<int>(query, new { departId });
            }
        }

        #endregion

        #region [#] 리소스 해제

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
}
```

### 5.4. DbContext 구성 (MdcHR26AppsAddDbContext)

```csharp
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using MdcHR26Apps.Models.User;
using MdcHR26Apps.Models.Department;
using MdcHR26Apps.Models.Rank;
using MdcHR26Apps.Models.DeptObjective;
using MdcHR26Apps.Models.EvaluationLists;
using MdcHR26Apps.Models.EvaluationProcess;
using MdcHR26Apps.Models.EvaluationReport;
using MdcHR26Apps.Models.EvaluationAgreement;
using MdcHR26Apps.Models.EvaluationSubAgreement;
using MdcHR26Apps.Models.EvaluationTasks;
using MdcHR26Apps.Models.EvaluationUsers;
using MdcHR26Apps.Models.Result;
using MdcHR26Apps.Models.Views;

namespace MdcHR26Apps.Models
{
    public class MdcHR26AppsAddDbContext : DbContext
    {
        public MdcHR26AppsAddDbContext() : base() { }

        public MdcHR26AppsAddDbContext(DbContextOptions<MdcHR26AppsAddDbContext> options)
            : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        // 테이블 DbSets (12개 - EStatusDb는 테이블이 아님, UserDb.EStatus 컬럼으로 관리)
        public DbSet<EDepartmentDb> EDepartmentDb { get; set; } = null!;
        public DbSet<ERankDb> ERankDb { get; set; } = null!;
        public DbSet<UserDb> UserDb { get; set; } = null!;
        public DbSet<DeptObjectiveDb> DeptObjectiveDb { get; set; } = null!;
        public DbSet<AgreementDb> AgreementDb { get; set; } = null!;
        public DbSet<SubAgreementDb> SubAgreementDb { get; set; } = null!;
        public DbSet<ProcessDb> ProcessDb { get; set; } = null!;
        public DbSet<ReportDb> ReportDb { get; set; } = null!;
        public DbSet<TotalReportDb> TotalReportDb { get; set; } = null!;
        public DbSet<EvaluationUsers> EvaluationUsers { get; set; } = null!;
        public DbSet<TasksDb> TasksDb { get; set; } = null!;
        public DbSet<EvaluationLists> EvaluationLists { get; set; } = null!;

        // 뷰 DbSets (5개)
        public DbSet<v_MemberListDB> v_MemberListDB { get; set; } = null!;
        public DbSet<v_DeptObjectiveListDb> v_DeptObjectiveListDb { get; set; } = null!;
        public DbSet<v_ProcessTRListDB> v_ProcessTRListDB { get; set; } = null!;
        public DbSet<v_ReportTaskListDB> v_ReportTaskListDB { get; set; } = null!;
        public DbSet<v_TotalReportListDB> v_TotalReportListDB { get; set; } = null!;
    }
}
```

### 5.5. DI 컨테이너 설정 (MdcHR26AppsAddExtensions)

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MdcHR26Apps.Models.User;
using MdcHR26Apps.Models.Department;
using MdcHR26Apps.Models.Rank;
using MdcHR26Apps.Models.DeptObjective;
using MdcHR26Apps.Models.EvaluationLists;
using MdcHR26Apps.Models.EvaluationProcess;
using MdcHR26Apps.Models.EvaluationReport;
using MdcHR26Apps.Models.EvaluationAgreement;
using MdcHR26Apps.Models.EvaluationSubAgreement;
using MdcHR26Apps.Models.EvaluationTasks;
using MdcHR26Apps.Models.EvaluationUsers;
using MdcHR26Apps.Models.Result;
using MdcHR26Apps.Models.Views;

namespace MdcHR26Apps.Models
{
    public static class MdcHR26AppsAddExtensions
    {
        public static void AddDependencyInjectionContainerForMdcHR26AppsModels(
            this IServiceCollection services,
            string connectionString)
        {
            // DbContext 등록 (Transient)
            services.AddDbContext<MdcHR26AppsAddDbContext>(options =>
                options.UseSqlServer(connectionString),
                ServiceLifetime.Transient);

            ILoggerFactory factory = new LoggerFactory();

            // Repository 등록 (Singleton) - 12개 테이블
            services.AddSingleton<IEDepartmentRepository>(
                new EDepartmentRepository(connectionString, factory));

            services.AddSingleton<IERankRepository>(
                new ERankRepository(connectionString, factory));

            services.AddSingleton<IUserRepository>(
                new UserRepository(connectionString, factory));

            services.AddSingleton<IDeptObjectiveRepository>(
                new DeptObjectiveRepository(connectionString, factory));

            services.AddSingleton<IAgreementRepository>(
                new AgreementRepository(connectionString, factory));

            services.AddSingleton<ISubAgreementRepository>(
                new SubAgreementRepository(connectionString, factory));

            services.AddSingleton<IProcessRepository>(
                new ProcessRepository(connectionString, factory));

            services.AddSingleton<IReportRepository>(
                new ReportRepository(connectionString, factory));

            services.AddSingleton<ITotalReportRepository>(
                new TotalReportRepository(connectionString, factory));

            services.AddSingleton<IEvaluationUsersRepository>(
                new EvaluationUsersRepository(connectionString, factory));

            services.AddSingleton<ITasksRepository>(
                new TasksRepository(connectionString, factory));

            services.AddSingleton<IEvaluationListsRepository>(
                new EvaluationListsRepository(connectionString, factory));

            // View Repository 등록 (Singleton) - 5개 뷰
            services.AddSingleton<Iv_MemberListRepository>(
                new v_MemberListRepository(connectionString, factory));

            services.AddSingleton<Iv_DeptObjectiveListRepository>(
                new v_DeptObjectiveListRepository(connectionString, factory));

            services.AddSingleton<Iv_ProcessTRListRepository>(
                new v_ProcessTRListRepository(connectionString, factory));

            services.AddSingleton<Iv_ReportTaskListRepository>(
                new v_ReportTaskListRepository(connectionString, factory));

            services.AddSingleton<Iv_TotalReportListRepository>(
                new v_TotalReportListRepository(connectionString, factory));
        }
    }
}
```

---

## 6. 개발 작업 목록

### 6.1. 프로젝트 설정

- [ ] MdcHR26Apps.Models 프로젝트 생성
- [ ] NuGet 패키지 설치 (Dapper, EF Core, SqlClient 등)
- [ ] 프로젝트 설정 (net8.0, Nullable, ImplicitUsings)

### 6.2. 공통 모듈

- [ ] Common/SelectListModel.cs 생성
- [ ] MdcHR26AppsAddDbContext.cs 생성
- [ ] MdcHR26AppsAddExtensions.cs 생성

### 6.3. Entity 및 Repository (12개 테이블)

#### 마스터 데이터 (2개)
- [ ] Department/EDepartmentDb.cs + Interface + Repository
- [ ] Rank/ERankDb.cs + Interface + Repository

#### 사용자 관리 (1개)
- [ ] User/UserDb.cs + IUserRepository.cs + UserRepository.cs (EStatus BIT 컬럼 포함)

#### 부서 목표 (1개)
- [ ] DeptObjective/DeptObjectiveDb.cs + Interface + Repository

#### 평가 프로세스 (7개)
- [ ] Evaluation/Agreement/AgreementDb.cs + Interface + Repository
- [ ] Evaluation/SubAgreement/SubAgreementDb.cs + Interface + Repository
- [ ] Evaluation/Process/ProcessDb.cs + Interface + Repository
- [ ] Evaluation/Report/ReportDb.cs + Interface + Repository
- [ ] Result/TotalReportDb.cs + Interface + Repository
- [ ] Evaluation/EvaluationUsers/EvaluationUsers.cs + Interface + Repository
- [ ] Evaluation/Tasks/TasksDb.cs + Interface + Repository

#### 평가 항목 마스터 (1개)
- [ ] Evaluation/Lists/EvaluationLists.cs + Interface + Repository

### 6.4. View 모델 및 Repository (5개)

- [ ] Views/v_MemberListDB.cs + Interface + Repository
- [ ] Views/v_DeptObjectiveListDb.cs + Interface + Repository
- [ ] Views/v_ProcessTRListDB.cs + Interface + Repository
- [ ] Views/v_ReportTaskListDB.cs + Interface + Repository
- [ ] Views/v_TotalReportListDB.cs + Interface + Repository

---

## 7. 보안 고려사항

### 7.1. 비밀번호 보안

**알고리즘**: SHA-256 + Salt (16바이트)

```csharp
// 비밀번호 해싱 프로세스
1. RandomNumberGenerator.GetBytes(16) - Salt 생성
2. 비밀번호 + Salt 결합
3. SHA256.ComputeHash() - 해시 생성
4. DB 저장: UserPassword (VARBINARY(128)), UserPasswordSalt (VARBINARY(16))

// 로그인 검증
1. UserId로 Salt와 해시된 비밀번호 조회
2. 입력 비밀번호 + Salt 해싱
3. StructuralEqualityComparer로 바이트 배열 비교
```

### 7.2. SQL Injection 방지

- 모든 쿼리에 매개변수화된 쿼리 사용
- Dapper의 `@` 매개변수 바인딩 활용

### 7.3. 권한 관리

UserDb의 권한 필드:
- `IsTeamLeader`: 팀장/부서장 여부
- `IsDirector`: 임원 여부
- `IsAdministrator`: 시스템 관리자 여부
- `IsDeptObjectiveWriter`: 부서 목표 작성 권한

---

## 8. 테스트 항목

### 8.1. 단위 테스트

#### UserRepository 테스트
```csharp
[Fact]
public async Task AddAsync_ShouldReturnUserWithId()
{
    // Arrange
    var user = new UserDb
    {
        UserId = "testuser",
        UserName = "테스트 사용자",
        Password = "test1234!",
        Email = "test@test.com",
        EDepartId = 1,
        ERankId = 1,
        EStatus = true  // 재직중
    };

    // Act
    var result = await _userRepository.AddAsync(user);

    // Assert
    Assert.True(result.Uid > 0);
    Assert.NotEmpty(result.UserPasswordSalt);
    Assert.NotEmpty(result.UserPassword);
}

[Fact]
public async Task LoginCheck_ValidCredentials_ShouldReturnTrue()
{
    // Arrange
    var userId = "admin";
    var password = "admin1234!";

    // Act
    var result = await _userRepository.LoginCheck(userId, password);

    // Assert
    Assert.True(result);
}

[Fact]
public async Task LoginCheck_InvalidCredentials_ShouldReturnFalse()
{
    // Arrange
    var userId = "admin";
    var password = "wrongpassword";

    // Act
    var result = await _userRepository.LoginCheck(userId, password);

    // Assert
    Assert.False(result);
}
```

### 8.2. 통합 테스트

#### 전체 CRUD 테스트
```csharp
[Fact]
public async Task FullCRUD_Lifecycle_Test()
{
    // 1. Create
    var user = new UserDb { /* ... */ };
    var created = await _userRepository.AddAsync(user);
    Assert.True(created.Uid > 0);

    // 2. Read
    var retrieved = await _userRepository.GetByIdAsync(created.Uid);
    Assert.Equal(user.UserId, retrieved.UserId);

    // 3. Update
    retrieved.UserName = "수정된 이름";
    var updated = await _userRepository.UpdateWithoutPasswordAsync(retrieved);
    Assert.True(updated);

    // 4. Verify Update
    var verified = await _userRepository.GetByIdAsync(created.Uid);
    Assert.Equal("수정된 이름", verified.UserName);

    // 5. Delete
    var deleted = await _userRepository.DeleteAsync(created.Uid);
    Assert.True(deleted);

    // 6. Verify Deletion
    var notFound = await _userRepository.GetByIdAsync(created.Uid);
    Assert.Equal(0, notFound.Uid);
}
```

### 8.3. 성능 테스트

```csharp
[Fact]
public async Task GetByAllAsync_Performance_Test()
{
    var stopwatch = Stopwatch.StartNew();
    var users = await _userRepository.GetByAllAsync();
    stopwatch.Stop();

    Assert.True(stopwatch.ElapsedMilliseconds < 1000); // 1초 이내
    _logger.LogInformation($"GetByAllAsync took {stopwatch.ElapsedMilliseconds}ms");
}
```

---

## 9. 개발 순서 및 우선순위

### Phase 2-1: 기본 구조 (우선순위: 높음)
1. 프로젝트 설정 및 NuGet 패키지
2. Common 모듈 (SelectListModel)
3. DbContext 및 DI 설정
4. UserDb 완전 구현 (참조 모델, EStatus BIT 포함)
5. EDepartmentDb, ERankDb 구현 (마스터 데이터)

### Phase 2-2: 평가 핵심 기능 (우선순위: 중간)
6. ProcessDb 구현 (평가 프로세스 관리)
7. ReportDb 구현 (개별 평가)
8. TotalReportDb 구현 (종합 평가)
9. EvaluationUsers 구현 (평가자 관리)

### Phase 2-3: 확장 기능 (우선순위: 낮음)
10. DeptObjectiveDb 구현 (부서 목표)
11. AgreementDb, SubAgreementDb 구현 (직무평가 협의)
12. TasksDb 구현 (업무 관리)
13. EvaluationLists 구현 (평가 항목 마스터)

### Phase 2-4: View 모델 (우선순위: 낮음)
14. v_MemberListDB 구현 (재직자 목록 - EStatus=1 필터링)
15. 나머지 4개 View 모델 구현

---

## 10. 참고사항

### 10.1. 2025년 프로젝트와의 차이점

| 항목 | 2025년 | 2026년 (현재) |
|------|--------|--------------|
| **.NET 버전** | .NET 7.0 | **.NET 10.0** |
| **EF Core** | 7.0.16 | **10.0.0** |
| **C# 버전** | C# 11 | **C# 13** |
| **비밀번호 보안** | PWDENCRYPT (SQL) | SHA-256 + Salt (C#) |
| **테이블 개수** | 13개 (MemberDb 포함) | 12개 (MemberDb 제거) |
| **권한 관리** | 3개 필드 | 4개 필드 (+IsDeptObjectiveWriter) |
| **부서 목표** | MemberDb 기반 | UserDb 기반 (통합) |
| **감사 추적** | 미적용 | CreatedDate, ModifiedDate |
| **생성자** | 전통적 생성자 | **Primary Constructor** |
| **컬렉션** | 전통적 초기화 | **Collection Expressions** |

### 10.2. 도서관리 프로젝트에서 적용할 기술

1. **SHA-256 + Salt 보안**: 비밀번호 암호화
2. **감사 추적**: CreatedDate, ModifiedDate 필드
3. **복잡한 검색 메서드**: SearchBy, GetBy 패턴
4. **View 활용**: 복잡한 조인을 View로 처리
5. **비즈니스 메서드**: IsUserIdExistsAsync, GetCountBy 등

### 10.3. .NET 10 신기능 활용 계획

#### 1) Primary Constructors
```csharp
// 모든 Repository에 적용
public class DepartmentRepository(string connectionString, ILoggerFactory loggerFactory)
    : IEDepartmentRepository, IDisposable
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<DepartmentRepository>();
    // ...
}
```

#### 2) Collection Expressions
```csharp
// 빈 리스트 반환 시 간결한 표현
public async Task<List<UserDb>> GetByDepartmentAsync(int departId)
{
    // ...
    return result?.ToList() ?? [];  // .NET 10 Collection Expression
}
```

#### 3) Improved String Handling
```csharp
// Raw String Literals (C# 13)
const string query = """
    SELECT * FROM UserDb
    WHERE EDepartId = @departId
    ORDER BY Uid
    """;
```

#### 4) EF Core 10 Complex Types
```csharp
// 값 객체 지원
public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
}

public class UserDb
{
    public Int64 Uid { get; set; }
    public Address Address { get; set; }  // Complex Type
}
```

#### 5) Performance Improvements
```csharp
// LINQ 최적화 자동 활용
var activeUsers = await connection.QueryAsync<UserDb>(
    "SELECT * FROM UserDb WHERE EStatusId = 1"
);
// .NET 10에서 자동 성능 최적화
```

### 10.3. 개발 시 주의사항

1. **NULL 안전성**: C# 8.0 Nullable Reference Type 활성화
2. **비동기 패턴**: 모든 데이터 액세스는 async/await
3. **리소스 해제**: IDisposable 패턴 구현
4. **로깅**: ILogger 사용하여 에러 기록
5. **매개변수화 쿼리**: SQL Injection 방지
6. **using 블록**: SqlConnection 자동 해제

---

## 11. 완료 조건

### Phase 2-1 완료 조건
- [ ] MdcHR26Apps.Models 프로젝트 생성
- [ ] NuGet 패키지 설치
- [ ] Common 모듈 작성
- [ ] DbContext 작성
- [ ] DI Extensions 작성
- [ ] UserDb 완전 구현 및 테스트
- [ ] EDepartmentDb, ERankDb, EStatusDb 구현

### Phase 2-2 완료 조건
- [ ] 평가 관련 7개 Entity 구현
- [ ] Repository 및 Interface 작성
- [ ] 단위 테스트 작성
- [ ] 통합 테스트 통과

### Phase 2-3 완료 조건
- [ ] 확장 기능 3개 Entity 구현
- [ ] View 모델 6개 구현
- [ ] 전체 테스트 통과
- [ ] 성능 테스트 통과

### 최종 완료 조건
- [ ] 18개 Repository 모두 구현 (12 테이블 + 6 뷰)
- [ ] DI 컨테이너 등록 완료
- [ ] 전체 CRUD 테스트 통과
- [ ] 로그인 보안 테스트 통과
- [ ] 성능 기준 충족 (1000개 레코드 조회 < 1초)
- [ ] 개발자 코드 리뷰 완료

---

## 12. 다음 단계 (Phase 3)

Phase 2 완료 후 예상되는 다음 작업:
1. **Phase 3-1**: ASP.NET Core Web API 구축
2. **Phase 3-2**: 인증/인가 미들웨어 (JWT)
3. **Phase 3-3**: Blazor Server UI 개발
4. **Phase 3-4**: 평가 프로세스 워크플로우 구현

---

## 13. .NET 10 전용 개발 가이드

### 13.1. Primary Constructor 활용

#### Repository 클래스
```csharp
// ✅ .NET 10 권장 방식
public class UserRepository(string connectionString, ILoggerFactory loggerFactory)
    : IUserRepository, IDisposable
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<UserRepository>();
    private readonly string dbContext = connectionString;

    // 메서드 구현...
}

// ❌ 기존 방식 (사용 안 함)
public class UserRepository : IUserRepository, IDisposable
{
    private readonly SqlConnection db;
    private readonly ILogger _logger;
    private readonly string dbContext;

    public UserRepository(string connectionString, ILoggerFactory loggerFactory)
    {
        this.db = new SqlConnection(connectionString);
        this._logger = loggerFactory.CreateLogger(nameof(UserRepository));
        this.dbContext = connectionString;
    }
}
```

### 13.2. Collection Expressions

#### 빈 컬렉션 반환
```csharp
// ✅ .NET 10 권장 방식
public async Task<List<UserDb>> GetByDepartmentAsync(int departId)
{
    var result = await connection.QueryAsync<UserDb>(query, new { departId });
    return result?.ToList() ?? [];  // Collection Expression
}

// ❌ 기존 방식
return result?.ToList() ?? new List<UserDb>();
```

#### 컬렉션 초기화
```csharp
// ✅ .NET 10 권장 방식
var statusIds = [1, 2, 3];  // Collection Expression

// ❌ 기존 방식
var statusIds = new List<int> { 1, 2, 3 };
```

### 13.3. Raw String Literals (C# 13)

#### SQL 쿼리 가독성 향상
```csharp
// ✅ .NET 10 권장 방식 - Raw String Literal
const string query = """
    SELECT
        u.Uid,
        u.UserId,
        u.UserName,
        d.EDepartmentName,
        r.ERankName
    FROM UserDb u
    INNER JOIN EDepartmentDb d ON u.EDepartId = d.EDepartId
    INNER JOIN ERankDb r ON u.ERankId = r.ERankId
    WHERE u.EStatusId = @statusId
    ORDER BY u.Uid
    """;

// ❌ 기존 방식
const string query = @"
    SELECT
        u.Uid,
        u.UserId,
        u.UserName,
        d.EDepartmentName,
        r.ERankName
    FROM UserDb u
    INNER JOIN EDepartmentDb d ON u.EDepartId = d.EDepartId
    INNER JOIN ERankDb r ON u.ERankId = r.ERankId
    WHERE u.EStatusId = @statusId
    ORDER BY u.Uid";
```

### 13.4. Improved Pattern Matching

#### 다형성 조회
```csharp
// ✅ .NET 10 권장 방식
public async Task<UserDb> GetUserAsync(object identifier) => identifier switch
{
    Int64 id => await GetByIdAsync(id),
    string userId => await GetByUserIdAsync(userId),
    _ => throw new ArgumentException("Invalid identifier type", nameof(identifier))
};

// 사용 예시
var user1 = await GetUserAsync(1L);           // ID로 조회
var user2 = await GetUserAsync("admin");      // UserId로 조회
```

### 13.5. Global Using Directives

#### GlobalUsings.cs 파일 생성
```csharp
// MdcHR26Apps.Models/GlobalUsings.cs
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;
global using Dapper;
global using Microsoft.Data.SqlClient;
global using Microsoft.Extensions.Logging;
global using System.Data;
global using System.Security.Cryptography;
global using System.Text;
```

이후 각 파일에서 중복 using 제거 가능:
```csharp
// ✅ GlobalUsings.cs 사용 시
namespace MdcHR26Apps.Models.User
{
    // using 문 불필요 (GlobalUsings.cs에 정의됨)
    public class UserRepository(string connectionString, ILoggerFactory loggerFactory)
        : IUserRepository, IDisposable
    {
        // ...
    }
}
```

### 13.6. EF Core 10 Complex Types

#### 값 객체 정의 (예시)
```csharp
// 주소 값 객체
[ComplexType]
public class Address
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
}

// UserDb에 Complex Type 사용
public class UserDb
{
    public Int64 Uid { get; set; }
    public string UserId { get; set; } = string.Empty;

    // Complex Type - 단일 테이블에 평면화됨
    public Address Address { get; set; } = new();
}

// DbContext 설정
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<UserDb>()
        .ComplexProperty(u => u.Address);
}
```

### 13.7. 성능 최적화 활용

#### Span<T> 활용 (비밀번호 해싱)
```csharp
// ✅ .NET 10 권장 방식 - Span 활용
public byte[] HashPassword(string password, byte[] salt)
{
    Span<byte> passwordBytes = stackalloc byte[password.Length * sizeof(char)];
    Encoding.Unicode.GetBytes(password, passwordBytes);

    Span<byte> combined = stackalloc byte[passwordBytes.Length + salt.Length];
    passwordBytes.CopyTo(combined);
    salt.CopyTo(combined[passwordBytes.Length..]);

    return SHA256.HashData(combined);
}

// 힙 할당 없이 스택 메모리만 사용 → 성능 향상
```

### 13.8. Keyed Services (DI)

#### Repository를 Key로 구분
```csharp
// DI 등록
services.AddKeyedSingleton<IUserRepository>(
    "default",
    (sp, key) => new UserRepository(connectionString, sp.GetRequiredService<ILoggerFactory>())
);

services.AddKeyedSingleton<IUserRepository>(
    "cached",
    (sp, key) => new CachedUserRepository(connectionString, sp.GetRequiredService<ILoggerFactory>())
);

// 사용
public class UserService([FromKeyedServices("cached")] IUserRepository repository)
{
    // cached 버전의 Repository 주입
}
```

### 13.9. 개발 체크리스트

#### .NET 10 신기능 적용 확인
- [ ] 모든 Repository에 **Primary Constructor** 적용
- [ ] 빈 컬렉션 반환 시 **Collection Expression `[]`** 사용
- [ ] 복잡한 SQL은 **Raw String Literals `"""`** 사용
- [ ] **GlobalUsings.cs** 생성 및 공통 using 이동
- [ ] 다형성 조회에 **Improved Pattern Matching** 활용
- [ ] 성능 개선 가능한 곳에 **Span<T>** 활용
- [ ] EF Core 10 **Complex Types** 검토 (필요 시)
- [ ] **Keyed Services** 활용 검토 (캐싱 등)

### 13.10. 마이그레이션 가이드 (.NET 8 → .NET 10)

만약 기존 .NET 8 코드가 있다면:

#### 1. 프로젝트 파일 업데이트
```xml
<!-- Before -->
<TargetFramework>net8.0</TargetFramework>

<!-- After -->
<TargetFramework>net10.0</TargetFramework>
<LangVersion>latest</LangVersion>
```

#### 2. NuGet 패키지 업데이트
```bash
dotnet add package Microsoft.EntityFrameworkCore --version 10.0.0
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 10.0.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 10.0.0
```

#### 3. 코드 리팩토링
```csharp
// 1. Primary Constructor로 변환
// Before
public class UserRepository : IUserRepository
{
    private readonly string _connectionString;
    public UserRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
}

// After
public class UserRepository(string connectionString) : IUserRepository
{
    private readonly string _connectionString = connectionString;
}

// 2. Collection Expression 적용
// Before
return new List<UserDb>();

// After
return [];

// 3. Raw String Literal 적용
// Before
const string query = @"SELECT * FROM UserDb";

// After
const string query = """SELECT * FROM UserDb""";
```

---

**작성자**: Claude AI
**검토자**: 개발자
**최종 수정**: 2026-01-16
**프레임워크**: .NET 10.0 / C# 13 / EF Core 10.0
