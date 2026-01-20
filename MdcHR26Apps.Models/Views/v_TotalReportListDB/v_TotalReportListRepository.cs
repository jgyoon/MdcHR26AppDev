using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MdcHR26Apps.Models.Views.v_TotalReportListDB;

/// <summary>
/// v_TotalReportListDB Repository 구현 (Dapper, 읽기 전용)
/// Primary Constructor 사용 (C# 13)
/// </summary>
public class v_TotalReportListRepository(string connectionString, ILoggerFactory loggerFactory) : Iv_TotalReportListRepository
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<v_TotalReportListRepository>();
    private readonly string dbContext = connectionString;

    #region + [1] 전체 조회: GetByAllAsync
    /// <summary>
    /// 전체 최종 평가 결과 조회
    /// </summary>
    public async Task<IEnumerable<v_TotalReportListDB>> GetByAllAsync()
    {
        const string sql = """
            SELECT * FROM v_TotalReportListDB
            ORDER BY Process_Year DESC, Final_Score DESC
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_TotalReportListDB>(sql);
    }
    #endregion

    #region + [2] ID로 조회: GetByIdAsync
    /// <summary>
    /// 특정 최종 평가 결과 조회 (TRid 기준)
    /// </summary>
    public async Task<v_TotalReportListDB?> GetByIdAsync(Int64 id)
    {
        const string sql = """
            SELECT * FROM v_TotalReportListDB
            WHERE TRid = @id
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<v_TotalReportListDB>(sql, new { id });
    }
    #endregion

    #region + [3] 사용자별 조회: GetByUserIdAsync
    /// <summary>
    /// 특정 사용자의 최종 평가 결과 조회
    /// </summary>
    public async Task<IEnumerable<v_TotalReportListDB>> GetByUserIdAsync(Int64 uid)
    {
        const string sql = """
            SELECT * FROM v_TotalReportListDB
            WHERE Uid = @uid
            ORDER BY Process_Year DESC
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_TotalReportListDB>(sql, new { uid });
    }
    #endregion

    #region + [4] 프로세스별 조회: GetByProcessIdAsync
    /// <summary>
    /// 프로세스별 최종 평가 결과 조회
    /// </summary>
    public async Task<IEnumerable<v_TotalReportListDB>> GetByProcessIdAsync(Int64 processId)
    {
        const string sql = """
            SELECT * FROM v_TotalReportListDB
            WHERE Pid = @processId
            ORDER BY Final_Score DESC
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_TotalReportListDB>(sql, new { processId });
    }
    #endregion

    #region + [5] 연도별 조회: GetByYearAsync
    /// <summary>
    /// 연도별 최종 평가 결과 조회
    /// </summary>
    public async Task<IEnumerable<v_TotalReportListDB>> GetByYearAsync(int year)
    {
        const string sql = """
            SELECT * FROM v_TotalReportListDB
            WHERE Process_Year = @year
            ORDER BY Final_Score DESC
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_TotalReportListDB>(sql, new { year });
    }
    #endregion

    #region + [6] 등급별 조회: GetByGradeAsync
    /// <summary>
    /// 등급별 최종 평가 결과 조회
    /// </summary>
    public async Task<IEnumerable<v_TotalReportListDB>> GetByGradeAsync(string grade)
    {
        const string sql = """
            SELECT * FROM v_TotalReportListDB
            WHERE Final_Grade = @grade
            ORDER BY Process_Year DESC, Final_Score DESC
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_TotalReportListDB>(sql, new { grade });
    }
    #endregion

    #region + [7] 부서별 조회: GetByDepartmentAsync
    /// <summary>
    /// 부서별 최종 평가 결과 조회
    /// </summary>
    public async Task<IEnumerable<v_TotalReportListDB>> GetByDepartmentAsync(Int64 departId)
    {
        const string sql = """
            SELECT * FROM v_TotalReportListDB
            WHERE EDepartId = @departId
            ORDER BY Process_Year DESC, Final_Score DESC
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_TotalReportListDB>(sql, new { departId });
    }
    #endregion

    #region + [8] 직급별 조회: GetByRankAsync
    /// <summary>
    /// 직급별 최종 평가 결과 조회
    /// </summary>
    public async Task<IEnumerable<v_TotalReportListDB>> GetByRankAsync(Int64 rankId)
    {
        const string sql = """
            SELECT * FROM v_TotalReportListDB
            WHERE ERankId = @rankId
            ORDER BY Process_Year DESC, Final_Score DESC
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_TotalReportListDB>(sql, new { rankId });
    }
    #endregion

    #region + [9] 점수 범위별 조회: GetByScoreRangeAsync
    /// <summary>
    /// 점수 범위별 조회 (최소~최대)
    /// </summary>
    public async Task<IEnumerable<v_TotalReportListDB>> GetByScoreRangeAsync(decimal minScore, decimal maxScore)
    {
        const string sql = """
            SELECT * FROM v_TotalReportListDB
            WHERE Final_Score >= @minScore AND Final_Score <= @maxScore
            ORDER BY Final_Score DESC
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_TotalReportListDB>(sql, new { minScore, maxScore });
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
