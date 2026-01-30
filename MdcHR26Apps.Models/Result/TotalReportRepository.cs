using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MdcHR26Apps.Models.Result;

/// <summary>
/// TotalReportDb Repository 구현 (Dapper)
/// Primary Constructor 사용 (C# 13)
/// </summary>
public class TotalReportRepository(string connectionString, ILoggerFactory loggerFactory) : ITotalReportRepository
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<TotalReportRepository>();
    private readonly string dbContext = connectionString;

    #region + [1] 입력: AddAsync
    /// <summary>
    /// 종합 평가 추가
    /// </summary>
    public async Task<Int64> AddAsync(TotalReportDb model)
    {
        const string sql = """
            INSERT INTO TotalReportDb(
                Uid,
                User_Evaluation_1, User_Evaluation_2, User_Evaluation_3, User_Evaluation_4,
                TeamLeader_Evaluation_1, TeamLeader_Evaluation_2, TeamLeader_Evaluation_3, TeamLeader_Comment,
                Feedback_Evaluation_1, Feedback_Evaluation_2, Feedback_Evaluation_3, Feedback_Comment,
                Director_Evaluation_1, Director_Evaluation_2, Director_Evaluation_3, Director_Comment,
                Total_Score, Director_Score, TeamLeader_Score)
            VALUES(
                @Uid,
                @User_Evaluation_1, @User_Evaluation_2, @User_Evaluation_3, @User_Evaluation_4,
                @TeamLeader_Evaluation_1, @TeamLeader_Evaluation_2, @TeamLeader_Evaluation_3, @TeamLeader_Comment,
                @Feedback_Evaluation_1, @Feedback_Evaluation_2, @Feedback_Evaluation_3, @Feedback_Comment,
                @Director_Evaluation_1, @Director_Evaluation_2, @Director_Evaluation_3, @Director_Comment,
                @Total_Score, @Director_Score, @TeamLeader_Score);
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteScalarAsync<Int64>(sql, model);
    }
    #endregion

    #region + [2] 출력: GetByAllAsync
    /// <summary>
    /// 전체 종합 평가 목록 조회
    /// </summary>
    public async Task<IEnumerable<TotalReportDb>> GetByAllAsync()
    {
        const string sql = "SELECT * FROM TotalReportDb ORDER BY TRid";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<TotalReportDb>(sql);
    }
    #endregion

    #region + [3] 상세: GetByIdAsync
    /// <summary>
    /// ID로 종합 평가 조회
    /// </summary>
    public async Task<TotalReportDb?> GetByIdAsync(Int64 id)
    {
        const string sql = "SELECT * FROM TotalReportDb WHERE TRid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<TotalReportDb>(sql, new { id });
    }
    #endregion

    #region + [4] 수정: UpdateAsync
    /// <summary>
    /// 종합 평가 수정
    /// </summary>
    public async Task<int> UpdateAsync(TotalReportDb model)
    {
        const string sql = """
            UPDATE TotalReportDb
            SET
                Uid = @Uid,
                User_Evaluation_1 = @User_Evaluation_1,
                User_Evaluation_2 = @User_Evaluation_2,
                User_Evaluation_3 = @User_Evaluation_3,
                User_Evaluation_4 = @User_Evaluation_4,
                TeamLeader_Evaluation_1 = @TeamLeader_Evaluation_1,
                TeamLeader_Evaluation_2 = @TeamLeader_Evaluation_2,
                TeamLeader_Evaluation_3 = @TeamLeader_Evaluation_3,
                TeamLeader_Comment = @TeamLeader_Comment,
                Feedback_Evaluation_1 = @Feedback_Evaluation_1,
                Feedback_Evaluation_2 = @Feedback_Evaluation_2,
                Feedback_Evaluation_3 = @Feedback_Evaluation_3,
                Feedback_Comment = @Feedback_Comment,
                Director_Evaluation_1 = @Director_Evaluation_1,
                Director_Evaluation_2 = @Director_Evaluation_2,
                Director_Evaluation_3 = @Director_Evaluation_3,
                Director_Comment = @Director_Comment,
                Total_Score = @Total_Score,
                Director_Score = @Director_Score,
                TeamLeader_Score = @TeamLeader_Score
            WHERE TRid = @TRid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, model);
    }
    #endregion

    #region + [5] 삭제: DeleteAsync
    /// <summary>
    /// 종합 평가 삭제
    /// </summary>
    public async Task<int> DeleteAsync(Int64 id)
    {
        const string sql = "DELETE FROM TotalReportDb WHERE TRid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, new { id });
    }
    #endregion

    #region + [6] 사용자별 조회: GetByUidAsync
    /// <summary>
    /// 사용자별 종합 평가 조회
    /// </summary>
    public async Task<TotalReportDb?> GetByUidAsync(Int64 uid)
    {
        const string sql = "SELECT * FROM TotalReportDb WHERE Uid = @uid";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<TotalReportDb>(sql, new { uid });
    }
    #endregion

    #region + [7] Pid로 조회: GetByPidAsync
    /// <summary>
    /// Pid로 종합 평가 조회
    /// </summary>
    public async Task<TotalReportDb?> GetByPidAsync(long pid)
    {
        const string sql = """
            SELECT T.*
            FROM TotalReportDb T
            INNER JOIN ProcessDb P ON T.Uid = P.Uid
            WHERE P.Pid = @Pid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<TotalReportDb>(sql, new { Pid = pid });
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
