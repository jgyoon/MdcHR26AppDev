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

        // 비밀번호 + Salt 조합하여 해시 생성 (SQL NVARCHAR는 Unicode 인코딩)
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var passwordBytes = System.Text.Encoding.Unicode.GetBytes(password);
        var combined = new byte[passwordBytes.Length + salt.Length];
        Buffer.BlockCopy(passwordBytes, 0, combined, 0, passwordBytes.Length);
        Buffer.BlockCopy(salt, 0, combined, passwordBytes.Length, salt.Length);
        byte[] inputHash = sha256.ComputeHash(combined);

        // 해시 비교
        return System.Collections.StructuralComparisons.StructuralEqualityComparer.Equals(inputHash, storedHash);
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
