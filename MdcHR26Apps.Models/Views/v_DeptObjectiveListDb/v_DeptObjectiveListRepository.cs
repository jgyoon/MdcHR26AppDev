using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MdcHR26Apps.Models.Views.v_DeptObjectiveListDb;

/// <summary>
/// v_DeptObjectiveListDb Repository 구현 (Dapper, 읽기 전용)
/// Primary Constructor 사용 (C# 13)
/// </summary>
public class v_DeptObjectiveListRepository(string connectionString, ILoggerFactory loggerFactory) : Iv_DeptObjectiveListRepository
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<v_DeptObjectiveListRepository>();
    private readonly string dbContext = connectionString;

    #region + [1] 전체 조회: GetByAllAsync
    /// <summary>
    /// 전체 부서 목표 목록 조회 (부서명 포함)
    /// </summary>
    public async Task<IEnumerable<v_DeptObjectiveListDb>> GetByAllAsync()
    {
        const string sql = """
            SELECT * FROM v_DeptObjectiveListDb
            ORDER BY Start_Date DESC, EDepartName
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_DeptObjectiveListDb>(sql);
    }
    #endregion

    #region + [2] ID로 조회: GetByIdAsync
    /// <summary>
    /// 특정 부서 목표 조회 (DOid 기준)
    /// </summary>
    public async Task<v_DeptObjectiveListDb?> GetByIdAsync(Int64 id)
    {
        const string sql = """
            SELECT * FROM v_DeptObjectiveListDb
            WHERE DOid = @id
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<v_DeptObjectiveListDb>(sql, new { id });
    }
    #endregion

    #region + [3] 부서별 조회: GetByDepartmentAsync
    /// <summary>
    /// 부서별 목표 목록 조회
    /// </summary>
    public async Task<IEnumerable<v_DeptObjectiveListDb>> GetByDepartmentAsync(Int64 departId)
    {
        const string sql = """
            SELECT * FROM v_DeptObjectiveListDb
            WHERE EDepartId = @departId
            ORDER BY Start_Date DESC
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_DeptObjectiveListDb>(sql, new { departId });
    }
    #endregion

    #region + [4] 기간별 조회: GetByDateRangeAsync
    /// <summary>
    /// 기간별 목표 조회
    /// </summary>
    public async Task<IEnumerable<v_DeptObjectiveListDb>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        const string sql = """
            SELECT * FROM v_DeptObjectiveListDb
            WHERE Start_Date >= @startDate AND End_Date <= @endDate
            ORDER BY Start_Date DESC, EDepartName
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_DeptObjectiveListDb>(sql, new { startDate, endDate });
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
