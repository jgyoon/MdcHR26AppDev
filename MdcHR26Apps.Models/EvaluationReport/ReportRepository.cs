using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MdcHR26Apps.Models.EvaluationReport;

/// <summary>
/// ReportDb Repository 구현 (Dapper)
/// Primary Constructor 사용 (C# 13)
/// </summary>
public class ReportRepository(string connectionString, ILoggerFactory loggerFactory) : IReportRepository
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<ReportRepository>();
    private readonly string dbContext = connectionString;

    #region + [1] 입력: AddAsync
    /// <summary>
    /// 평가 보고서 추가
    /// </summary>
    public async Task<Int64> AddAsync(ReportDb model)
    {
        const string sql = """
            INSERT INTO ReportDb(
                Uid, Report_Item_Number, Report_Item_Name_1, Report_Item_Name_2,
                Report_Item_Proportion, Report_SubItem_Name, Report_SubItem_Proportion,
                Task_Number,
                User_Evaluation_1, User_Evaluation_2, User_Evaluation_3, User_Evaluation_4,
                TeamLeader_Evaluation_1, TeamLeader_Evaluation_2, TeamLeader_Evaluation_3, TeamLeader_Evaluation_4,
                Director_Evaluation_1, Director_Evaluation_2, Director_Evaluation_3, Director_Evaluation_4,
                Total_Score)
            VALUES(
                @Uid, @Report_Item_Number, @Report_Item_Name_1, @Report_Item_Name_2,
                @Report_Item_Proportion, @Report_SubItem_Name, @Report_SubItem_Proportion,
                @Task_Number,
                @User_Evaluation_1, @User_Evaluation_2, @User_Evaluation_3, @User_Evaluation_4,
                @TeamLeader_Evaluation_1, @TeamLeader_Evaluation_2, @TeamLeader_Evaluation_3, @TeamLeader_Evaluation_4,
                @Director_Evaluation_1, @Director_Evaluation_2, @Director_Evaluation_3, @Director_Evaluation_4,
                @Total_Score);
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteScalarAsync<Int64>(sql, model);
    }
    #endregion

    #region + [2] 출력: GetByAllAsync
    /// <summary>
    /// 전체 보고서 목록 조회
    /// </summary>
    public async Task<IEnumerable<ReportDb>> GetByAllAsync()
    {
        const string sql = "SELECT * FROM ReportDb ORDER BY Rid";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<ReportDb>(sql);
    }
    #endregion

    #region + [3] 상세: GetByIdAsync
    /// <summary>
    /// ID로 보고서 조회
    /// </summary>
    public async Task<ReportDb?> GetByIdAsync(Int64 id)
    {
        const string sql = "SELECT * FROM ReportDb WHERE Rid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<ReportDb>(sql, new { id });
    }
    #endregion

    #region + [4] 수정: UpdateAsync
    /// <summary>
    /// 보고서 수정
    /// </summary>
    public async Task<int> UpdateAsync(ReportDb model)
    {
        const string sql = """
            UPDATE ReportDb
            SET
                Uid = @Uid,
                Report_Item_Number = @Report_Item_Number,
                Report_Item_Name_1 = @Report_Item_Name_1,
                Report_Item_Name_2 = @Report_Item_Name_2,
                Report_Item_Proportion = @Report_Item_Proportion,
                Report_SubItem_Name = @Report_SubItem_Name,
                Report_SubItem_Proportion = @Report_SubItem_Proportion,
                Task_Number = @Task_Number,
                User_Evaluation_1 = @User_Evaluation_1,
                User_Evaluation_2 = @User_Evaluation_2,
                User_Evaluation_3 = @User_Evaluation_3,
                User_Evaluation_4 = @User_Evaluation_4,
                TeamLeader_Evaluation_1 = @TeamLeader_Evaluation_1,
                TeamLeader_Evaluation_2 = @TeamLeader_Evaluation_2,
                TeamLeader_Evaluation_3 = @TeamLeader_Evaluation_3,
                TeamLeader_Evaluation_4 = @TeamLeader_Evaluation_4,
                Director_Evaluation_1 = @Director_Evaluation_1,
                Director_Evaluation_2 = @Director_Evaluation_2,
                Director_Evaluation_3 = @Director_Evaluation_3,
                Director_Evaluation_4 = @Director_Evaluation_4,
                Total_Score = @Total_Score
            WHERE Rid = @Rid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, model);
    }
    #endregion

    #region + [5] 삭제: DeleteAsync
    /// <summary>
    /// 보고서 삭제
    /// </summary>
    public async Task<int> DeleteAsync(Int64 id)
    {
        const string sql = "DELETE FROM ReportDb WHERE Rid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, new { id });
    }
    #endregion

    #region + [6] 사용자별 조회: GetByUidAllAsync
    /// <summary>
    /// 사용자별 보고서 목록 조회
    /// </summary>
    public async Task<IEnumerable<ReportDb>> GetByUidAllAsync(Int64 uid)
    {
        const string sql = """
            SELECT * FROM ReportDb
            WHERE Uid = @uid
            ORDER BY Rid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<ReportDb>(sql, new { uid });
    }
    #endregion

    #region + [7] 사용자별 개수 조회: GetCountByUidAsync
    /// <summary>
    /// 사용자별 보고서 개수 조회
    /// </summary>
    public async Task<int> GetCountByUidAsync(long uid)
    {
        const string sql = "SELECT COUNT(*) FROM ReportDb WHERE Uid = @Uid";

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteScalarAsync<int>(sql, new { Uid = uid });
    }
    #endregion

    #region + [8] 사용자별 전체 삭제: DeleteAllByUidAsync
    /// <summary>
    /// 사용자별 보고서 전체 삭제
    /// </summary>
    public async Task<bool> DeleteAllByUidAsync(long uid)
    {
        const string sql = "DELETE FROM ReportDb WHERE Uid = @Uid";

        using var connection = new SqlConnection(dbContext);
        var result = await connection.ExecuteAsync(sql, new { Uid = uid });
        return result > 0;
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
