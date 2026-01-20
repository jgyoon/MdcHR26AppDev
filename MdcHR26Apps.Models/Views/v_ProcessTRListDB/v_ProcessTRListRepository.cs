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
            ORDER BY Process_Year DESC, Start_Date DESC
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
            ORDER BY Final_Score DESC
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
            ORDER BY Process_Year DESC, Start_Date DESC
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_ProcessTRListDB>(sql, new { uid });
    }
    #endregion

    #region + [4] 연도별 조회: GetByYearAsync
    /// <summary>
    /// 연도별 평가 결과 조회
    /// </summary>
    public async Task<IEnumerable<v_ProcessTRListDB>> GetByYearAsync(int year)
    {
        const string sql = """
            SELECT * FROM v_ProcessTRListDB
            WHERE Process_Year = @year
            ORDER BY Final_Score DESC
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_ProcessTRListDB>(sql, new { year });
    }
    #endregion

    #region + [5] 등급별 조회: GetByGradeAsync
    /// <summary>
    /// 등급별 평가 결과 조회
    /// </summary>
    public async Task<IEnumerable<v_ProcessTRListDB>> GetByGradeAsync(string grade)
    {
        const string sql = """
            SELECT * FROM v_ProcessTRListDB
            WHERE Final_Grade = @grade
            ORDER BY Process_Year DESC, Final_Score DESC
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_ProcessTRListDB>(sql, new { grade });
    }
    #endregion

    #region + [6] 프로세스 상태별 조회: GetByProcessStatusAsync
    /// <summary>
    /// 프로세스 상태별 조회
    /// </summary>
    public async Task<IEnumerable<v_ProcessTRListDB>> GetByProcessStatusAsync(int status)
    {
        const string sql = """
            SELECT * FROM v_ProcessTRListDB
            WHERE Process_Status = @status
            ORDER BY Process_Year DESC, Start_Date DESC
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_ProcessTRListDB>(sql, new { status });
    }
    #endregion

    #region + [7] 프로세스+사용자 조회: GetByProcessAndUserAsync
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
