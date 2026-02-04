using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MdcHR26Apps.Models.Views.v_ProcessTRListDB;

/// <summary>
/// v_ProcessTRListDB Repository 구현 (Dapper, 읽기 전용)
/// Primary Constructor 사용 (C# 13)
/// </summary>
public class v_ProcessTRListRepository(string connectionString, ILoggerFactory loggerFactory) : Iv_ProcessTRListRepository
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<v_ProcessTRListRepository>();
    private readonly string dbContext = connectionString;

    #region + [1] 전체 조회: GetByAllAsync
    /// <summary>
    /// 전체 평가 프로세스-결과 목록 조회
    /// </summary>
    public async Task<IEnumerable<v_ProcessTRListDB>> GetByAllAsync()
    {
        const string sql = """
            SELECT * FROM v_ProcessTRListDB
            ORDER BY Pid DESC
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_ProcessTRListDB>(sql);
    }
    #endregion

    #region + [2] 프로세스별 조회: GetByProcessIdAsync
    /// <summary>
    /// 특정 프로세스의 전체 결과 조회
    /// </summary>
    public async Task<IEnumerable<v_ProcessTRListDB>> GetByProcessIdAsync(Int64 processId)
    {
        const string sql = """
            SELECT * FROM v_ProcessTRListDB
            WHERE Pid = @processId
            ORDER BY Total_Score DESC
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_ProcessTRListDB>(sql, new { processId });
    }
    #endregion

    #region + [3] 사용자별 조회: GetByUserIdAsync
    /// <summary>
    /// 특정 사용자의 평가 결과 조회
    /// </summary>
    public async Task<IEnumerable<v_ProcessTRListDB>> GetByUserIdAsync(Int64 uid)
    {
        const string sql = """
            SELECT * FROM v_ProcessTRListDB
            WHERE Uid = @uid
            ORDER BY Pid DESC
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_ProcessTRListDB>(sql, new { uid });
    }
    #endregion

    #region + [4] 프로세스+사용자 조회: GetByProcessAndUserAsync
    /// <summary>
    /// 특정 프로세스의 특정 사용자 결과 조회
    /// </summary>
    public async Task<v_ProcessTRListDB?> GetByProcessAndUserAsync(Int64 processId, Int64 uid)
    {
        const string sql = """
            SELECT * FROM v_ProcessTRListDB
            WHERE Pid = @processId AND Uid = @uid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<v_ProcessTRListDB>(sql, new { processId, uid });
    }
    #endregion

    #region + [8] GetAllAsync (List 반환)
    /// <summary>
    /// 전체 평가 프로세스-결과 목록 조회 (List 반환)
    /// </summary>
    public async Task<List<v_ProcessTRListDB>> GetAllAsync()
    {
        const string sql = """
            SELECT * FROM v_ProcessTRListDB
            ORDER BY Pid DESC
            """;

        using var connection = new SqlConnection(dbContext);
        var result = await connection.QueryAsync<v_ProcessTRListDB>(sql);
        return result.AsList();
    }
    #endregion

    #region + [9] GetByPidAsync
    /// <summary>
    /// Pid로 특정 평가 프로세스-결과 조회
    /// </summary>
    public async Task<v_ProcessTRListDB?> GetByPidAsync(long pid)
    {
        const string sql = """
            SELECT * FROM v_ProcessTRListDB
            WHERE Pid = @Pid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<v_ProcessTRListDB>(sql, new { Pid = pid });
    }
    #endregion

    #region + [10] GetByTeamLeaderIdAsync
    /// <summary>
    /// 부서장 관할 팀원 목록 조회
    /// </summary>
    public async Task<List<v_ProcessTRListDB>> GetByTeamLeaderIdAsync(Int64 teamLeaderId)
    {
        const string sql = """
            SELECT * FROM v_ProcessTRListDB
            WHERE TeamLeader_Id = (SELECT UserId FROM UserDb WHERE Uid = @teamLeaderId)
            ORDER BY Pid DESC
            """;

        using var connection = new SqlConnection(dbContext);
        var result = await connection.QueryAsync<v_ProcessTRListDB>(sql, new { teamLeaderId });
        return result.AsList();
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
