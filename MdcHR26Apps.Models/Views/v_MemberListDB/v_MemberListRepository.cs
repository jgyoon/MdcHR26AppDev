using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MdcHR26Apps.Models.Views.v_MemberListDB;

/// <summary>
/// v_MemberListDB Repository 구현 (Dapper, 읽기 전용)
/// Primary Constructor 사용 (C# 13)
/// </summary>
public class v_MemberListRepository(string connectionString, ILoggerFactory loggerFactory) : Iv_MemberListRepository
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<v_MemberListRepository>();
    private readonly string dbContext = connectionString;

    #region + [1] 전체 조회: GetByAllAsync
    /// <summary>
    /// 전체 회원 목록 조회 (부서명, 직급명 포함)
    /// </summary>
    public async Task<IEnumerable<v_MemberListDB>> GetByAllAsync()
    {
        const string sql = """
            SELECT * FROM v_MemberListDB
            ORDER BY EDepartName, ERankName, UserName
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_MemberListDB>(sql);
    }
    #endregion

    #region + [2] Uid로 조회: GetByUidAsync
    /// <summary>
    /// 특정 회원 조회 (Uid 기준)
    /// </summary>
    public async Task<v_MemberListDB?> GetByUidAsync(Int64 uid)
    {
        const string sql = """
            SELECT * FROM v_MemberListDB
            WHERE Uid = @uid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<v_MemberListDB>(sql, new { uid });
    }
    #endregion

    #region + [3] 부서별 조회: GetByDepartmentAsync
    /// <summary>
    /// 부서별 회원 목록 조회
    /// </summary>
    public async Task<IEnumerable<v_MemberListDB>> GetByDepartmentAsync(Int64 departId)
    {
        const string sql = """
            SELECT * FROM v_MemberListDB
            WHERE EDepartId = @departId
            ORDER BY ERankName, UserName
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_MemberListDB>(sql, new { departId });
    }
    #endregion

    #region + [4] 직급별 조회: GetByRankAsync
    /// <summary>
    /// 직급별 회원 목록 조회
    /// </summary>
    public async Task<IEnumerable<v_MemberListDB>> GetByRankAsync(Int64 rankId)
    {
        const string sql = """
            SELECT * FROM v_MemberListDB
            WHERE ERankId = @rankId
            ORDER BY EDepartName, UserName
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_MemberListDB>(sql, new { rankId });
    }
    #endregion

    #region + [5] 활성 회원 조회: GetActiveUsersAsync
    /// <summary>
    /// 활성 회원 목록 조회 (EStatus = 1)
    /// </summary>
    public async Task<IEnumerable<v_MemberListDB>> GetActiveUsersAsync()
    {
        const string sql = """
            SELECT * FROM v_MemberListDB
            WHERE EStatus = 1
            ORDER BY EDepartName, ERankName, UserName
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_MemberListDB>(sql);
    }
    #endregion

    #region + [6] UserId로 조회: GetByUserIdAsync
    /// <summary>
    /// 사용자 ID로 검색
    /// </summary>
    public async Task<v_MemberListDB?> GetByUserIdAsync(string userId)
    {
        const string sql = """
            SELECT * FROM v_MemberListDB
            WHERE UserId = @userId
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<v_MemberListDB>(sql, new { userId });
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
