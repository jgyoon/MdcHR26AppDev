# 작업지시서: Phase 2-1 - 프로젝트 생성 및 기본 모델 구현

**날짜**: 2026-01-19
**작업 유형**: 프로젝트 생성 / 기능 추가
**관련 이슈**: [#008: Phase 2 Model 개발](../issues/008_phase2_model_development.md)
**관련 작업지시서**:
- `20260116_01_phase2_model_development.md` (Phase 2 전체 계획)
- `20260114_02_remove_memberdb_optimize_structure.md` (선행 작업)

**수정 이력**:
- 2026-01-19 (v1): Entity 모델을 실제 DB 테이블 구조에 맞게 수정 (Database/01_CreateTables.sql 기준)
  - UserDb: Uid(long, IDENTITY), UserId(string), UserPassword/Salt(byte[]), 권한 필드 추가
  - EDepartmentDb: EDepartId(long, IDENTITY), EDepartmentNo(int, UNIQUE), ActivateStatus(bool)
  - ERankDb: ERankId(long, IDENTITY), ERankNo(int, UNIQUE), ActivateStatus(bool)
  - Repository SQL 문 모두 실제 컬럼명에 맞게 수정
- 2026-01-19 (v2): 2025년 프로젝트 비즈니스 로직 메서드 추가
  - UpdateWithoutPasswordAsync: 비밀번호 제외 정보 수정
  - LoginCheckAsync: SHA-256 해시 기반 로그인 검증
  - UserIdCheckAsync: UserId 중복 확인
  - GetByUserIdAsync: UserId로 사용자 조회
  - GetTeamLeadersAsync, GetDirectorsAsync: 권한별 조회
  - GetByUserNameAsync: 사용자명 LIKE 검색
  - Dispose 메서드 추가

---

## 1. 작업 개요

### 1.1. Phase 2-1 목표
MdcHR26Apps.Models 프로젝트를 생성하고, 기본 인프라와 핵심 마스터 데이터 모델을 구현합니다.

### 1.2. 작업 범위
1. **프로젝트 생성**: .NET 10.0 Class Library
2. **NuGet 패키지 설치**: Dapper 2.1.66, EF Core 10.0.0, SqlClient 6.1.1
3. **공통 모듈**: SelectListModel.cs (드롭다운용)
4. **핵심 Entity**: UserDb, EDepartmentDb, ERankDb
5. **Repository 패턴**: Interface + 구현체 (Dapper 기반)
6. **EF Core DbContext**: 12개 테이블, 5개 뷰 매핑
7. **DI Extensions**: Repository 등록

### 1.3. 기술 스택
- **.NET**: 10.0
- **C#**: 13 (Primary Constructors, Collection Expressions)
- **ORM**: Dapper 2.1.66 (주), EF Core 10.0.0 (보조)
- **Database**: SQL Server
- **패턴**: Repository Pattern, Dependency Injection

---

## 2. 프로젝트 구조

### 2.1. 폴더 구조
```
MdcHR26Apps.Models/
├── MdcHR26Apps.Models.csproj          # 프로젝트 파일
├── MdcHR26AppsAddDbContext.cs         # EF Core DbContext
├── MdcHR26AppsAddExtensions.cs        # DI 설정
├── Common/
│   └── SelectListModel.cs             # 공통 드롭다운 모델
├── User/
│   ├── UserDb.cs                      # Entity (EStatus BIT 포함)
│   ├── IUserRepository.cs             # Interface
│   └── UserRepository.cs              # Dapper 구현체
├── Department/
│   ├── EDepartmentDb.cs               # Entity
│   ├── IEDepartmentRepository.cs      # Interface
│   └── EDepartmentRepository.cs       # Dapper 구현체
└── Rank/
    ├── ERankDb.cs                     # Entity
    ├── IERankRepository.cs            # Interface
    └── ERankRepository.cs             # Dapper 구현체
```

### 2.2. Phase 2-1 개발 파일 목록 (13개)
| 번호 | 파일명 | 용도 | 비고 |
|------|--------|------|------|
| 1 | MdcHR26Apps.Models.csproj | 프로젝트 설정 | .NET 10.0, NuGet 패키지 |
| 2 | Common/SelectListModel.cs | 드롭다운 모델 | 공통 모듈 |
| 3 | User/UserDb.cs | 사용자 Entity | **EStatus(BIT)** 포함 |
| 4 | User/IUserRepository.cs | Interface | 11개 메서드 |
| 5 | User/UserRepository.cs | Repository 구현 | Dapper + Primary Constructor |
| 6 | Department/EDepartmentDb.cs | 부서 Entity | 마스터 데이터 |
| 7 | Department/IEDepartmentRepository.cs | Interface | 7개 메서드 |
| 8 | Department/EDepartmentRepository.cs | Repository 구현 | Dapper + Primary Constructor |
| 9 | Rank/ERankDb.cs | 직급 Entity | 마스터 데이터 |
| 10 | Rank/IERankRepository.cs | Interface | 7개 메서드 |
| 11 | Rank/ERankRepository.cs | Repository 구현 | Dapper + Primary Constructor |
| 12 | MdcHR26AppsAddDbContext.cs | EF Core DbContext | 12 테이블 + 5 뷰 |
| 13 | MdcHR26AppsAddExtensions.cs | DI Extension | 3개 Repository 등록 |

---

## 3. 단계별 작업 내용

### 3.1. 프로젝트 생성 및 설정

#### 프로젝트 파일 (MdcHR26Apps.Models.csproj)
```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <LangVersion>latest</LangVersion>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Dapper" Version="2.1.66" />
    <PackageReference Include="Microsoft.Data.SqlClient" Version="6.1.1" />
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="10.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="System.Configuration.ConfigurationManager" Version="10.0.0" />
  </ItemGroup>

</Project>
```

**설정 설명**:
- `TargetFramework`: .NET 10.0
- `ImplicitUsings`: using 문 자동 포함
- `Nullable`: nullable 참조 형식 활성화
- `LangVersion`: C# 13 최신 기능 사용 (Primary Constructors)

---

### 3.2. NuGet 패키지

| 패키지 | 버전 | 용도 |
|--------|------|------|
| Dapper | 2.1.66 | Micro ORM (주 데이터 액세스) |
| Microsoft.Data.SqlClient | 6.1.1 | SQL Server 연결 |
| Microsoft.EntityFrameworkCore | 10.0.0 | EF Core (DbContext) |
| Microsoft.EntityFrameworkCore.SqlServer | 10.0.0 | SQL Server Provider |
| Microsoft.EntityFrameworkCore.Tools | 10.0.0 | Migration 도구 |
| System.Configuration.ConfigurationManager | 10.0.0 | 설정 관리 |

---

### 3.3. 공통 모듈 - SelectListModel

**파일**: `Common/SelectListModel.cs`

```csharp
namespace MdcHR26Apps.Models.Common;

/// <summary>
/// 드롭다운 목록용 공통 모델
/// </summary>
public class SelectListModel
{
    /// <summary>
    /// 값 (ID)
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// 표시 텍스트
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// 선택 여부
    /// </summary>
    public bool Selected { get; set; } = false;

    /// <summary>
    /// 비활성화 여부
    /// </summary>
    public bool Disabled { get; set; } = false;

    /// <summary>
    /// 그룹명 (선택사항)
    /// </summary>
    public string? Group { get; set; }
}
```

**용도**:
- ASP.NET Core의 SelectList 대체
- 부서, 직급 등 드롭다운 목록 데이터 전달
- API 응답 시 클라이언트에 전달

---

### 3.4. UserDb Entity 및 Repository

#### 3.4.1. Entity (UserDb.cs)

**파일**: `User/UserDb.cs`

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.User;

/// <summary>
/// 사용자/직원 정보 Entity
/// </summary>
[Table("UserDb")]
public class UserDb
{
    /// <summary>
    /// 사용자 ID (PK, IDENTITY)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 Uid { get; set; }

    /// <summary>
    /// 로그인 ID
    /// </summary>
    [Required]
    [StringLength(50)]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 사용자 이름
    /// </summary>
    [Required]
    [StringLength(20)]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 비밀번호 해시 (SHA-256, VARBINARY(32))
    /// </summary>
    [Required]
    [MaxLength(32)]
    public byte[] UserPassword { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// 비밀번호 Salt (VARBINARY(16))
    /// </summary>
    [Required]
    [MaxLength(16)]
    public byte[] UserPasswordSalt { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// 사원번호
    /// </summary>
    [StringLength(10)]
    public string? ENumber { get; set; }

    /// <summary>
    /// 이메일
    /// </summary>
    [StringLength(50)]
    public string? Email { get; set; }

    /// <summary>
    /// 부서 ID (FK → EDepartmentDb.EDepartId)
    /// </summary>
    public long? EDepartId { get; set; }

    /// <summary>
    /// 직급 ID (FK → ERankDb.ERankId)
    /// </summary>
    public long? ERankId { get; set; }

    /// <summary>
    /// 재직 상태 (BIT: 1=재직, 0=퇴직)
    /// </summary>
    [Required]
    public bool EStatus { get; set; } = true;

    /// <summary>
    /// 팀장 여부
    /// </summary>
    [Required]
    public bool IsTeamLeader { get; set; } = false;

    /// <summary>
    /// 임원 여부
    /// </summary>
    [Required]
    public bool IsDirector { get; set; } = false;

    /// <summary>
    /// 관리자 여부
    /// </summary>
    [Required]
    public bool IsAdministrator { get; set; } = false;

    /// <summary>
    /// 부서 목표 작성 권한
    /// </summary>
    [Required]
    public bool IsDeptObjectiveWriter { get; set; } = false;

    // Navigation Properties
    [ForeignKey("EDepartId")]
    public EDepartmentDb? EDepartment { get; set; }

    [ForeignKey("ERankId")]
    public ERankDb? ERank { get; set; }
}
```

**중요 사항**:
- Primary Key는 `Uid` (BIGINT IDENTITY) - 자동 증가
- `UserId`는 로그인 ID (VARCHAR(50), UNIQUE)
- `UserPassword`, `UserPasswordSalt`는 **VARBINARY** 타입 (C#에서는 `byte[]`)
- SHA-256 해시(32바이트) + Salt(16바이트) 방식 (#006에서 적용)
- `EStatus`는 **BIT 타입** (C#에서는 `bool`)
- 권한 필드: `IsTeamLeader`, `IsDirector`, `IsAdministrator`, `IsDeptObjectiveWriter`
- Foreign Keys: `EDepartId`, `ERankId` (둘 다 `long?` nullable)

#### 3.4.2. Interface (IUserRepository.cs)

**파일**: `User/IUserRepository.cs`

```csharp
using MdcHR26Apps.Models.Common;

namespace MdcHR26Apps.Models.User;

/// <summary>
/// UserDb Repository Interface
/// </summary>
public interface IUserRepository : IDisposable
{
    // === 기본 CRUD ===

    /// <summary>
    /// 사용자 추가
    /// </summary>
    Task<Int64> AddAsync(UserDb user);

    /// <summary>
    /// 전체 사용자 조회
    /// </summary>
    Task<IEnumerable<UserDb>> GetByAllAsync();

    /// <summary>
    /// ID로 사용자 조회
    /// </summary>
    Task<UserDb?> GetByIdAsync(long uid);

    /// <summary>
    /// 사용자 정보 수정
    /// </summary>
    Task<int> UpdateAsync(UserDb user);

    /// <summary>
    /// 사용자 삭제
    /// </summary>
    Task<int> DeleteAsync(long uid);

    // === 비즈니스 특화 메서드 ===

    /// <summary>
    /// 재직자 목록 조회 (EStatus = true)
    /// </summary>
    Task<IEnumerable<UserDb>> GetActiveUsersAsync();

    /// <summary>
    /// 부서별 사용자 조회
    /// </summary>
    Task<IEnumerable<UserDb>> GetByDepartmentAsync(long departmentId);

    /// <summary>
    /// 로그인 인증 (Uid로 조회)
    /// </summary>
    Task<UserDb?> AuthenticateAsync(string userId);

    /// <summary>
    /// 비밀번호 변경
    /// </summary>
    Task<int> UpdatePasswordAsync(long uid, byte[] passwordHash, byte[] salt);

    /// <summary>
    /// 재직 상태 변경
    /// </summary>
    Task<int> UpdateStatusAsync(long uid, bool status);

    /// <summary>
    /// 비밀번호 없이 정보만 수정
    /// </summary>
    Task<int> UpdateWithoutPasswordAsync(UserDb user);

    /// <summary>
    /// 로그인 체크 (UserId + Password)
    /// </summary>
    Task<bool> LoginCheckAsync(string userId, string password);

    /// <summary>
    /// UserId 존재 여부 확인
    /// </summary>
    Task<bool> UserIdCheckAsync(string userId);

    /// <summary>
    /// UserId로 사용자 조회
    /// </summary>
    Task<UserDb?> GetByUserIdAsync(string userId);

    /// <summary>
    /// 팀장 목록 조회 (IsTeamLeader = true)
    /// </summary>
    Task<IEnumerable<UserDb>> GetTeamLeadersAsync();

    /// <summary>
    /// 임원 목록 조회 (IsDirector = true)
    /// </summary>
    Task<IEnumerable<UserDb>> GetDirectorsAsync();

    /// <summary>
    /// 사용자명으로 검색 (LIKE 검색)
    /// </summary>
    Task<IEnumerable<UserDb>> GetByUserNameAsync(string userName);

    /// <summary>
    /// 드롭다운용 사용자 목록 (재직자만)
    /// </summary>
    Task<IEnumerable<SelectListModel>> GetSelectListAsync();
}
```

#### 3.4.3. Repository 구현 (UserRepository.cs)

**파일**: `User/UserRepository.cs`

```csharp
using Dapper;
using MdcHR26Apps.Models.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MdcHR26Apps.Models.User;

/// <summary>
/// UserDb Repository 구현 (Dapper)
/// Primary Constructor 사용 (C# 13)
/// </summary>
public class UserRepository(string connectionString, ILoggerFactory loggerFactory) : IUserRepository
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<UserRepository>();
    private readonly string dbContext = connectionString;

    #region + 참고
    // [4][2][2] 리포지토리 클래스(비동기 방식): Micro ORM인 Dapper를 사용하여 CRUD 구현
    // 2025년 프로젝트 참조: C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.Models\User\UserDbRepositoryDapperAsync.cs
    #endregion

    #region + [1] 입력: AddAsync
    /// <summary>
    /// 사용자 추가
    /// </summary>
    public async Task<Int64> AddAsync(UserDb user)
    {
        const string sql = """
            INSERT INTO UserDb
                (UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email,
                 EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter)
            VALUES
                (@UserId, @UserName, @UserPassword, @UserPasswordSalt, @ENumber, @Email,
                 @EDepartId, @ERankId, @EStatus, @IsTeamLeader, @IsDirector, @IsAdministrator, @IsDeptObjectiveWriter);
            SELECT CAST(SCOPE_IDENTITY() as bigint);
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteScalarAsync<Int64>(sql, user);
    }
    #endregion

    #region + [2] 출력: GetByAllAsync
    /// <summary>
    /// 전체 사용자 조회
    /// </summary>
    public async Task<IEnumerable<UserDb>> GetByAllAsync()
    {
        const string sql = "SELECT * FROM UserDb ORDER BY Uid";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<UserDb>(sql);
    }
    #endregion

    #region + [3] 상세: GetByIdAsync
    /// <summary>
    /// ID로 사용자 조회
    /// </summary>
    public async Task<UserDb?> GetByIdAsync(long uid)
    {
        const string sql = "SELECT * FROM UserDb WHERE Uid = @Uid";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<UserDb>(sql, new { Uid = uid });
    }
    #endregion

    #region + [4] 수정: UpdateAsync
    /// <summary>
    /// 사용자 정보 수정
    /// </summary>
    public async Task<int> UpdateAsync(UserDb user)
    {
        const string sql = """
            UPDATE UserDb
            SET
                UserName = @UserName,
                ENumber = @ENumber,
                Email = @Email,
                EDepartId = @EDepartId,
                ERankId = @ERankId,
                EStatus = @EStatus,
                IsTeamLeader = @IsTeamLeader,
                IsDirector = @IsDirector,
                IsAdministrator = @IsAdministrator,
                IsDeptObjectiveWriter = @IsDeptObjectiveWriter
            WHERE Uid = @Uid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, user);
    }
    #endregion

    #region + [5] 삭제: DeleteAsync
    /// <summary>
    /// 사용자 삭제
    /// </summary>
    public async Task<int> DeleteAsync(long uid)
    {
        const string sql = "DELETE FROM UserDb WHERE Uid = @Uid";

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, new { Uid = uid });
    }
    #endregion

    #region + [6] 재직자 조회: GetActiveUsersAsync
    /// <summary>
    /// 재직자 목록 조회 (EStatus = 1)
    /// </summary>
    public async Task<IEnumerable<UserDb>> GetActiveUsersAsync()
    {
        const string sql = """
            SELECT * FROM UserDb
            WHERE EStatus = 1
            ORDER BY EDepartId, ERankId, Uid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<UserDb>(sql);
    }
    #endregion

    #region + [7] 부서별 사용자 조회: GetByDepartmentAsync
    /// <summary>
    /// 부서별 사용자 조회
    /// </summary>
    public async Task<IEnumerable<UserDb>> GetByDepartmentAsync(long departmentId)
    {
        const string sql = """
            SELECT * FROM UserDb
            WHERE EDepartId = @DepartmentId AND EStatus = 1
            ORDER BY ERankId, Uid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<UserDb>(sql, new { DepartmentId = departmentId });
    }
    #endregion

    #region + [8] 로그인 인증: AuthenticateAsync
    /// <summary>
    /// 로그인 인증 (UserId로 조회, 재직자만)
    /// </summary>
    public async Task<UserDb?> AuthenticateAsync(string userId)
    {
        const string sql = """
            SELECT * FROM UserDb
            WHERE UserId = @UserId AND EStatus = 1
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<UserDb>(sql, new { UserId = userId });
    }
    #endregion

    #region + [9] 비밀번호 변경: UpdatePasswordAsync
    /// <summary>
    /// 비밀번호 변경
    /// </summary>
    public async Task<int> UpdatePasswordAsync(long uid, byte[] passwordHash, byte[] salt)
    {
        const string sql = """
            UPDATE UserDb
            SET
                UserPassword = @PasswordHash,
                UserPasswordSalt = @Salt
            WHERE Uid = @Uid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, new
        {
            Uid = uid,
            PasswordHash = passwordHash,
            Salt = salt
        });
    }
    #endregion

    #region + [10] 재직 상태 변경: UpdateStatusAsync
    /// <summary>
    /// 재직 상태 변경
    /// </summary>
    public async Task<int> UpdateStatusAsync(long uid, bool status)
    {
        const string sql = """
            UPDATE UserDb
            SET EStatus = @Status
            WHERE Uid = @Uid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, new
        {
            Uid = uid,
            Status = status
        });
    }
    #endregion

    #region + [11] 비밀번호 없이 수정: UpdateWithoutPasswordAsync
    /// <summary>
    /// 비밀번호 없이 정보만 수정
    /// </summary>
    public async Task<int> UpdateWithoutPasswordAsync(UserDb user)
    {
        const string sql = """
            UPDATE UserDb
            SET
                UserName = @UserName,
                ENumber = @ENumber,
                Email = @Email,
                EDepartId = @EDepartId,
                ERankId = @ERankId,
                EStatus = @EStatus,
                IsTeamLeader = @IsTeamLeader,
                IsDirector = @IsDirector,
                IsAdministrator = @IsAdministrator,
                IsDeptObjectiveWriter = @IsDeptObjectiveWriter
            WHERE Uid = @Uid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, user);
    }
    #endregion

    #region + [12] 로그인 체크: LoginCheckAsync
    /// <summary>
    /// 로그인 체크 (UserId + Password)
    /// SHA-256 해시 비교
    /// </summary>
    public async Task<bool> LoginCheckAsync(string userId, string password)
    {
        const string sql = """
            SELECT UserPassword, UserPasswordSalt
            FROM UserDb
            WHERE UserId = @UserId AND EStatus = 1
            """;

        using var connection = new SqlConnection(dbContext);
        var result = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { UserId = userId });

        if (result == null) return false;

        // Salt와 입력 비밀번호로 해시 생성
        byte[] salt = result.UserPasswordSalt;
        byte[] storedHash = result.UserPassword;

        // 비밀번호 + Salt 조합하여 해시 생성
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
        var combined = new byte[passwordBytes.Length + salt.Length];
        Buffer.BlockCopy(passwordBytes, 0, combined, 0, passwordBytes.Length);
        Buffer.BlockCopy(salt, 0, combined, passwordBytes.Length, salt.Length);
        byte[] inputHash = sha256.ComputeHash(combined);

        // 해시 비교
        return storedHash.SequenceEqual(inputHash);
    }
    #endregion

    #region + [13] UserId 존재 확인: UserIdCheckAsync
    /// <summary>
    /// UserId 존재 여부 확인
    /// </summary>
    public async Task<bool> UserIdCheckAsync(string userId)
    {
        const string sql = "SELECT COUNT(*) FROM UserDb WHERE UserId = @UserId";

        using var connection = new SqlConnection(dbContext);
        int count = await connection.ExecuteScalarAsync<int>(sql, new { UserId = userId });
        return count > 0;
    }
    #endregion

    #region + [14] UserId로 조회: GetByUserIdAsync
    /// <summary>
    /// UserId로 사용자 조회
    /// </summary>
    public async Task<UserDb?> GetByUserIdAsync(string userId)
    {
        const string sql = "SELECT * FROM UserDb WHERE UserId = @UserId";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<UserDb>(sql, new { UserId = userId });
    }
    #endregion

    #region + [15] 팀장 목록: GetTeamLeadersAsync
    /// <summary>
    /// 팀장 목록 조회 (IsTeamLeader = true)
    /// </summary>
    public async Task<IEnumerable<UserDb>> GetTeamLeadersAsync()
    {
        const string sql = """
            SELECT * FROM UserDb
            WHERE IsTeamLeader = 1 AND EStatus = 1
            ORDER BY Uid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<UserDb>(sql);
    }
    #endregion

    #region + [16] 임원 목록: GetDirectorsAsync
    /// <summary>
    /// 임원 목록 조회 (IsDirector = true)
    /// </summary>
    public async Task<IEnumerable<UserDb>> GetDirectorsAsync()
    {
        const string sql = """
            SELECT * FROM UserDb
            WHERE IsDirector = 1 AND EStatus = 1
            ORDER BY Uid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<UserDb>(sql);
    }
    #endregion

    #region + [17] 사용자명 검색: GetByUserNameAsync
    /// <summary>
    /// 사용자명으로 검색 (LIKE 검색)
    /// </summary>
    public async Task<IEnumerable<UserDb>> GetByUserNameAsync(string userName)
    {
        const string sql = """
            SELECT * FROM UserDb
            WHERE UserName LIKE @UserName + '%'
            ORDER BY Uid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<UserDb>(sql, new { UserName = userName });
    }
    #endregion

    #region + [18] 드롭다운 목록: GetSelectListAsync
    /// <summary>
    /// 드롭다운용 사용자 목록 (재직자만)
    /// </summary>
    public async Task<IEnumerable<SelectListModel>> GetSelectListAsync()
    {
        const string sql = """
            SELECT
                CAST(Uid AS NVARCHAR) AS Value,
                UserName + ' (' + UserId + ')' AS Text
            FROM UserDb
            WHERE EStatus = 1
            ORDER BY UserName
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

**구현 특징**:
- **Primary Constructor** 사용: `(string connectionString)` 매개변수로 DI
- **Raw String Literals** (C# 13): `"""` 사용으로 SQL 가독성 향상
- **비동기 메서드**: 모든 DB 작업은 `async/await`
- **Connection 관리**: `using` 문으로 자동 해제
- **BIT 타입 매핑**: SQL의 `BIT`는 C#의 `bool`로 자동 매핑

---

### 3.5. EDepartmentDb Entity 및 Repository

#### 3.5.1. Entity (EDepartmentDb.cs)

**파일**: `Department/EDepartmentDb.cs`

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.Department;

/// <summary>
/// 부서 마스터 Entity
/// </summary>
[Table("EDepartmentDb")]
public class EDepartmentDb
{
    /// <summary>
    /// 부서 ID (PK, IDENTITY)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 EDepartId { get; set; }

    /// <summary>
    /// 부서 번호 (정렬 순서, UNIQUE)
    /// </summary>
    [Required]
    public int EDepartmentNo { get; set; }

    /// <summary>
    /// 부서명
    /// </summary>
    [Required]
    [StringLength(255)]
    public string EDepartmentName { get; set; } = string.Empty;

    /// <summary>
    /// 활성화 여부 (BIT)
    /// </summary>
    [Required]
    public bool ActivateStatus { get; set; } = true;

    /// <summary>
    /// 비고
    /// </summary>
    public string? Remarks { get; set; }
}
```

#### 3.5.2. Interface (IEDepartmentRepository.cs)

**파일**: `Department/IEDepartmentRepository.cs`

```csharp
using MdcHR26Apps.Models.Common;

namespace MdcHR26Apps.Models.Department;

/// <summary>
/// EDepartmentDb Repository Interface
/// </summary>
public interface IEDepartmentRepository : IDisposable
{
    // === 기본 CRUD ===
    Task<Int64> AddAsync(EDepartmentDb department);
    Task<IEnumerable<EDepartmentDb>> GetByAllAsync();
    Task<EDepartmentDb?> GetByIdAsync(long departmentId);
    Task<int> UpdateAsync(EDepartmentDb department);
    Task<int> DeleteAsync(long departmentId);

    // === 비즈니스 특화 메서드 ===

    /// <summary>
    /// 활성화된 부서 목록 조회
    /// </summary>
    Task<IEnumerable<EDepartmentDb>> GetActiveAsync();

    /// <summary>
    /// 드롭다운용 부서 목록
    /// </summary>
    Task<IEnumerable<SelectListModel>> GetSelectListAsync();
}
```

#### 3.5.3. Repository 구현 (EDepartmentRepository.cs)

**파일**: `Department/EDepartmentRepository.cs`

```csharp
using Dapper;
using MdcHR26Apps.Models.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MdcHR26Apps.Models.Department;

/// <summary>
/// EDepartmentDb Repository 구현 (Dapper)
/// </summary>
public class EDepartmentRepository(string connectionString, ILoggerFactory loggerFactory) : IEDepartmentRepository, IDisposable
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<EDepartmentRepository>();
    private readonly string dbContext = connectionString;

    #region + 참고
    // [4][2][2] 리포지토리 클래스(비동기 방식): Micro ORM인 Dapper를 사용하여 CRUD 구현
    #endregion

    #region + [1] 입력: AddAsync
    public async Task<Int64> AddAsync(EDepartmentDb department)
    {
        const string sql = """
            INSERT INTO EDepartmentDb
                (EDepartmentNo, EDepartmentName, ActivateStatus, Remarks)
            VALUES
                (@EDepartmentNo, @EDepartmentName, @ActivateStatus, @Remarks);
            SELECT CAST(SCOPE_IDENTITY() as bigint);
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteScalarAsync<Int64>(sql, department);
    }
    #endregion

    #region + [2] 출력: GetByAllAsync
    public async Task<IEnumerable<EDepartmentDb>> GetByAllAsync()
    {
        const string sql = "SELECT * FROM EDepartmentDb ORDER BY EDepartmentNo";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<EDepartmentDb>(sql);
    }
    #endregion

    #region + [3] 상세: GetByIdAsync
    public async Task<EDepartmentDb?> GetByIdAsync(long departmentId)
    {
        const string sql = "SELECT * FROM EDepartmentDb WHERE EDepartId = @DepartmentId";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<EDepartmentDb>(sql, new { DepartmentId = departmentId });
    }
    #endregion

    #region + [4] 수정: UpdateAsync
    public async Task<int> UpdateAsync(EDepartmentDb department)
    {
        const string sql = """
            UPDATE EDepartmentDb
            SET
                EDepartmentNo = @EDepartmentNo,
                EDepartmentName = @EDepartmentName,
                ActivateStatus = @ActivateStatus,
                Remarks = @Remarks
            WHERE EDepartId = @EDepartId
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, department);
    }
    #endregion

    #region + [5] 삭제: DeleteAsync
    public async Task<int> DeleteAsync(long departmentId)
    {
        const string sql = "DELETE FROM EDepartmentDb WHERE EDepartId = @DepartmentId";

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, new { DepartmentId = departmentId });
    }
    #endregion

    #region + [6] 활성화된 부서 조회: GetActiveAsync
    public async Task<IEnumerable<EDepartmentDb>> GetActiveAsync()
    {
        const string sql = """
            SELECT * FROM EDepartmentDb
            WHERE ActivateStatus = 1
            ORDER BY EDepartmentNo
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<EDepartmentDb>(sql);
    }
    #endregion

    #region + [7] 드롭다운 목록: GetSelectListAsync
    public async Task<IEnumerable<SelectListModel>> GetSelectListAsync()
    {
        const string sql = """
            SELECT
                CAST(EDepartId AS NVARCHAR) AS Value,
                EDepartmentName AS Text
            FROM EDepartmentDb
            WHERE ActivateStatus = 1
            ORDER BY EDepartmentNo
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

### 3.6. ERankDb Entity 및 Repository

#### 3.6.1. Entity (ERankDb.cs)

**파일**: `Rank/ERankDb.cs`

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.Rank;

/// <summary>
/// 직급 마스터 Entity
/// </summary>
[Table("ERankDb")]
public class ERankDb
{
    /// <summary>
    /// 직급 ID (PK, IDENTITY)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 ERankId { get; set; }

    /// <summary>
    /// 직급 번호 (정렬 순서, UNIQUE)
    /// </summary>
    [Required]
    public int ERankNo { get; set; }

    /// <summary>
    /// 직급명
    /// </summary>
    [Required]
    [StringLength(255)]
    public string ERankName { get; set; } = string.Empty;

    /// <summary>
    /// 활성화 여부 (BIT)
    /// </summary>
    [Required]
    public bool ActivateStatus { get; set; } = true;

    /// <summary>
    /// 비고
    /// </summary>
    public string? Remarks { get; set; }
}
```

#### 3.6.2. Interface (IERankRepository.cs)

**파일**: `Rank/IERankRepository.cs`

```csharp
using MdcHR26Apps.Models.Common;

namespace MdcHR26Apps.Models.Rank;

/// <summary>
/// ERankDb Repository Interface
/// </summary>
public interface IERankRepository : IDisposable
{
    // === 기본 CRUD ===
    Task<Int64> AddAsync(ERankDb rank);
    Task<IEnumerable<ERankDb>> GetByAllAsync();
    Task<ERankDb?> GetByIdAsync(long rankId);
    Task<int> UpdateAsync(ERankDb rank);
    Task<int> DeleteAsync(long rankId);

    // === 비즈니스 특화 메서드 ===

    /// <summary>
    /// 활성화된 직급 목록 조회
    /// </summary>
    Task<IEnumerable<ERankDb>> GetActiveAsync();

    /// <summary>
    /// 드롭다운용 직급 목록
    /// </summary>
    Task<IEnumerable<SelectListModel>> GetSelectListAsync();
}
```

#### 3.6.3. Repository 구현 (ERankRepository.cs)

**파일**: `Rank/ERankRepository.cs`

```csharp
using Dapper;
using MdcHR26Apps.Models.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MdcHR26Apps.Models.Rank;

/// <summary>
/// ERankDb Repository 구현 (Dapper)
/// </summary>
public class ERankRepository(string connectionString, ILoggerFactory loggerFactory) : IERankRepository, IDisposable
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<ERankRepository>();
    private readonly string dbContext = connectionString;

    #region + 참고
    // [4][2][2] 리포지토리 클래스(비동기 방식): Micro ORM인 Dapper를 사용하여 CRUD 구현
    #endregion

    #region + [1] 입력: AddAsync
    public async Task<Int64> AddAsync(ERankDb rank)
    {
        const string sql = """
            INSERT INTO ERankDb
                (ERankNo, ERankName, ActivateStatus, Remarks)
            VALUES
                (@ERankNo, @ERankName, @ActivateStatus, @Remarks);
            SELECT CAST(SCOPE_IDENTITY() as bigint);
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteScalarAsync<Int64>(sql, rank);
    }
    #endregion

    #region + [2] 출력: GetByAllAsync
    public async Task<IEnumerable<ERankDb>> GetByAllAsync()
    {
        const string sql = "SELECT * FROM ERankDb ORDER BY ERankNo";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<ERankDb>(sql);
    }
    #endregion

    #region + [3] 상세: GetByIdAsync
    public async Task<ERankDb?> GetByIdAsync(long rankId)
    {
        const string sql = "SELECT * FROM ERankDb WHERE ERankId = @RankId";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<ERankDb>(sql, new { RankId = rankId });
    }
    #endregion

    #region + [4] 수정: UpdateAsync
    public async Task<int> UpdateAsync(ERankDb rank)
    {
        const string sql = """
            UPDATE ERankDb
            SET
                ERankNo = @ERankNo,
                ERankName = @ERankName,
                ActivateStatus = @ActivateStatus,
                Remarks = @Remarks
            WHERE ERankId = @ERankId
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, rank);
    }
    #endregion

    #region + [5] 삭제: DeleteAsync
    public async Task<int> DeleteAsync(long rankId)
    {
        const string sql = "DELETE FROM ERankDb WHERE ERankId = @RankId";

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, new { RankId = rankId });
    }
    #endregion

    #region + [6] 활성화된 직급 조회: GetActiveAsync
    public async Task<IEnumerable<ERankDb>> GetActiveAsync()
    {
        const string sql = """
            SELECT * FROM ERankDb
            WHERE ActivateStatus = 1
            ORDER BY ERankNo
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<ERankDb>(sql);
    }
    #endregion

    #region + [7] 드롭다운 목록: GetSelectListAsync
    public async Task<IEnumerable<SelectListModel>> GetSelectListAsync()
    {
        const string sql = """
            SELECT
                CAST(ERankId AS NVARCHAR) AS Value,
                ERankName AS Text
            FROM ERankDb
            WHERE ActivateStatus = 1
            ORDER BY ERankNo
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

### 3.7. EF Core DbContext

**파일**: `MdcHR26AppsAddDbContext.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using MdcHR26Apps.Models.User;
using MdcHR26Apps.Models.Department;
using MdcHR26Apps.Models.Rank;

namespace MdcHR26Apps.Models;

/// <summary>
/// EF Core DbContext
/// 12개 테이블 + 5개 뷰 매핑
/// </summary>
public class MdcHR26AppsAddDbContext(DbContextOptions<MdcHR26AppsAddDbContext> options) : DbContext(options)
{
    // === Phase 2-1: 기본 마스터 데이터 (3개) ===
    public DbSet<UserDb> UserDb { get; set; } = null!;
    public DbSet<EDepartmentDb> EDepartmentDb { get; set; } = null!;
    public DbSet<ERankDb> ERankDb { get; set; } = null!;

    // === Phase 2-2, 2-3: 나머지 9개 테이블 (추후 추가) ===
    // public DbSet<DeptObjectiveDb> DeptObjectiveDb { get; set; }
    // public DbSet<AgreementDb> AgreementDb { get; set; }
    // public DbSet<SubAgreementDb> SubAgreementDb { get; set; }
    // public DbSet<ProcessDb> ProcessDb { get; set; }
    // public DbSet<ReportDb> ReportDb { get; set; }
    // public DbSet<TotalReportDb> TotalReportDb { get; set; }
    // public DbSet<EvaluationUsers> EvaluationUsers { get; set; }
    // public DbSet<TasksDb> TasksDb { get; set; }
    // public DbSet<EvaluationLists> EvaluationLists { get; set; }

    // === Phase 2-4: 5개 뷰 (추후 추가) ===
    // public DbSet<v_MemberListDB> v_MemberListDB { get; set; }
    // public DbSet<v_DeptObjectiveListDb> v_DeptObjectiveListDb { get; set; }
    // public DbSet<v_ProcessTRListDB> v_ProcessTRListDB { get; set; }
    // public DbSet<v_ReportTaskListDB> v_ReportTaskListDB { get; set; }
    // public DbSet<v_TotalReportListDB> v_TotalReportListDB { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // === Phase 2-1: 테이블 매핑 ===
        modelBuilder.Entity<UserDb>().ToTable("UserDb");
        modelBuilder.Entity<EDepartmentDb>().ToTable("EDepartmentDb");
        modelBuilder.Entity<ERankDb>().ToTable("ERankDb");

        // === Phase 2-1: BIT 타입 매핑 (UserDb.EStatus) ===
        //  EStatusDb 는 존재하지 않으므로 EStatus 필드를 명시적으로 할 필요는 없음
        // modelBuilder.Entity<UserDb>()
        //     .Property(u => u.EStatus)
        //     .HasColumnType("BIT");

        // === Phase 2-4: 뷰 매핑 (추후 추가) ===
        // modelBuilder.Entity<v_MemberListDB>().ToView("v_MemberListDB").HasNoKey();
        // modelBuilder.Entity<v_DeptObjectiveListDb>().ToView("v_DeptObjectiveListDb").HasNoKey();
        // modelBuilder.Entity<v_ProcessTRListDB>().ToView("v_ProcessTRListDB").HasNoKey();
        // modelBuilder.Entity<v_ReportTaskListDB>().ToView("v_ReportTaskListDB").HasNoKey();
        // modelBuilder.Entity<v_TotalReportListDB>().ToView("v_TotalReportListDB").HasNoKey();
    }
}
```

**구현 특징**:
- **Primary Constructor**: `(DbContextOptions<MdcHR26AppsAddDbContext> options)`
- **Phase 2-1**: UserDb, EDepartmentDb, ERankDb만 활성화
- **Phase 2-2, 2-3, 2-4**: 나머지는 주석 처리 (추후 단계에서 활성화)
<!-- - **BIT 매핑**: `EStatus` 필드는 명시적으로 `BIT` 타입 지정 -->

---

### 3.8. DI Extensions

**파일**: `MdcHR26AppsAddExtensions.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        // LoggerFactory 생성
        var loggerFactory = provider.GetRequiredService<ILoggerFactory>();

        // === Phase 2-1: Repository 등록 (3개) ===
        services.AddScoped<IUserRepository>(provider =>
            new UserRepository(connectionString, loggerFactory));

        services.AddScoped<IEDepartmentRepository>(provider =>
            new EDepartmentRepository(connectionString, loggerFactory));

        services.AddScoped<IERankRepository>(provider =>
            new ERankRepository(connectionString, loggerFactory));

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
```

**사용 방법** (Program.cs에서):
```csharp
// appsettings.json 또는 appsettings.Development.json에 연결 문자열 설정
builder.Services.AddMdcHR26AppsModels(builder.Configuration);
```

---

## 4. 테스트 항목

### 4.1. 프로젝트 빌드 테스트
1. Visual Studio 2022에서 MdcHR26Apps.Models 프로젝트 열기
2. 솔루션 빌드 (Ctrl+Shift+B)
3. **확인**: 빌드 성공, 오류 0개 ✅

### 4.2. NuGet 패키지 확인
1. NuGet 패키지 관리자 열기
2. **확인**: 6개 패키지 설치 완료 ✅
   - Dapper 2.1.66
   - Microsoft.Data.SqlClient 6.1.1
   - Microsoft.EntityFrameworkCore 10.0.0
   - Microsoft.EntityFrameworkCore.SqlServer 10.0.0
   - Microsoft.EntityFrameworkCore.Tools 10.0.0
   - System.Configuration.ConfigurationManager 10.0.0

### 4.3. 파일 구조 확인
솔루션 탐색기에서 다음 파일들이 정상적으로 생성되었는지 확인:

```
MdcHR26Apps.Models/
├── MdcHR26Apps.Models.csproj          ✅
├── MdcHR26AppsAddDbContext.cs         ✅
├── MdcHR26AppsAddExtensions.cs        ✅
├── Common/
│   └── SelectListModel.cs             ✅
├── User/
│   ├── UserDb.cs                      ✅
│   ├── IUserRepository.cs             ✅
│   └── UserRepository.cs              ✅
├── Department/
│   ├── EDepartmentDb.cs               ✅
│   ├── IEDepartmentRepository.cs      ✅
│   └── EDepartmentRepository.cs       ✅
└── Rank/
    ├── ERankDb.cs                     ✅
    ├── IERankRepository.cs            ✅
    └── ERankRepository.cs             ✅
```

### 4.4. 코드 스타일 확인
1. **Primary Constructor** 사용 확인
   - UserRepository, EDepartmentRepository, ERankRepository
2. **Raw String Literals** 사용 확인
   - SQL 문에 `"""` 사용
3. **Nullable 참조 형식** 확인
   - `string?` 타입 사용

### 4.5. 데이터베이스 연결 테스트 (추후 Web 프로젝트에서)
Phase 2-1 완료 후 Web 프로젝트 생성 시:
1. appsettings.json에 연결 문자열 설정
2. Program.cs에 `builder.Services.AddMdcHR26AppsModels(builder.Configuration);` 추가
3. Controller에서 Repository 주입 테스트
4. 간단한 CRUD 동작 확인

---

## 5. 예상 결과

### 수정 전
- MdcHR26Apps.Models 프로젝트 없음 ❌
- Entity 및 Repository 코드 없음 ❌

### 수정 후
- .NET 10.0 Class Library 프로젝트 생성 ✅
- 13개 파일 생성 완료 ✅
- Dapper + EF Core 하이브리드 구조 ✅
- Repository Pattern + DI 적용 ✅
- Primary Constructor (C# 13) 활용 ✅
- Phase 2-2, 2-3을 위한 확장 가능한 구조 ✅

---

## 6. 주의사항

1. **EStatus 타입**: UserDb.EStatus는 `bool` (BIT), **EStatusDb 테이블은 존재하지 않음**
2. **Primary Constructor**: C# 13 기능이므로 .NET 10.0 + LangVersion=latest 필수
3. **연결 문자열**: appsettings.json의 `ConnectionStrings:DefaultConnection` 설정 필요
4. **비밀번호 보안**: SHA-256 + Salt 방식 사용 (#006 이슈 참조)
5. **Phase 순차 진행**: Phase 2-1 완료 후 2-2, 2-3, 2-4 순서대로 진행
6. **Git 브랜치**: feature/phase2-1 브랜치에서 작업 중

---

## 7. 완료 조건

- [ ] MdcHR26Apps.Models.csproj 생성
- [ ] NuGet 패키지 6개 설치
- [ ] Common/SelectListModel.cs 작성
- [ ] User/ 폴더 3개 파일 작성 (Entity, Interface, Repository)
- [ ] Department/ 폴더 3개 파일 작성
- [ ] Rank/ 폴더 3개 파일 작성
- [ ] MdcHR26AppsAddDbContext.cs 작성
- [ ] MdcHR26AppsAddExtensions.cs 작성
- [ ] 빌드 성공 (오류 0개)
- [ ] 폴더 구조 확인
- [ ] Primary Constructor 적용 확인
- [ ] Raw String Literals 적용 확인

---

## 8. 다음 단계 (Phase 2-2)

Phase 2-1 완료 후:
1. **작업지시서 작성**: `20260119_02_phase2_2_evaluation_core.md`
2. **개발 대상**:
   - ProcessDb (평가 프로세스)
   - ReportDb (개별 평가 보고서)
   - TotalReportDb (종합 보고서)
   - EvaluationUsers (평가자 관리)
3. **DbContext 업데이트**: 4개 테이블 추가
4. **Extensions 업데이트**: 4개 Repository 등록

---

**작성자**: Claude AI
**검토 필요**: 개발자
**승인 후**: 코드 작성 시작
