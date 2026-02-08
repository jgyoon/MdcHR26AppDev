using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MdcHR26Apps.Models.Views.v_ReportTaskListDB;

/// <summary>
/// v_ReportTaskListDB Repository 구현 (Dapper, 읽기 전용)
/// Primary Constructor 사용 (C# 13)
/// </summary>
public class v_ReportTaskListRepository(string connectionString, ILoggerFactory loggerFactory) : Iv_ReportTaskListRepository
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<v_ReportTaskListRepository>();
    private readonly string dbContext = connectionString;

    #region + [1] 전체 조회: GetByAllAsync
    /// <summary>
    /// 전체 평가 보고서-업무 목록 조회
    /// </summary>
    public async Task<IEnumerable<v_ReportTaskListDB>> GetByAllAsync()
    {
        const string sql = """
            SELECT * FROM v_ReportTaskListDB
            ORDER BY Pid DESC, Rid, TargetDate
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_ReportTaskListDB>(sql);
    }
    #endregion

    #region + [1-1] GetAllAsync (List 반환)
    /// <summary>
    /// 전체 평가 보고서-업무 목록 조회 (List 반환)
    /// </summary>
    public async Task<List<v_ReportTaskListDB>> GetAllAsync()
    {
        const string sql = """
            SELECT * FROM v_ReportTaskListDB
            ORDER BY Pid DESC, Rid, TargetDate
            """;

        using var connection = new SqlConnection(dbContext);
        var result = await connection.QueryAsync<v_ReportTaskListDB>(sql);
        return result.AsList();
    }
    #endregion

    #region + [2] 보고서별 조회: GetByReportIdAsync
    /// <summary>
    /// 특정 보고서의 업무 목록 조회
    /// </summary>
    public async Task<IEnumerable<v_ReportTaskListDB>> GetByReportIdAsync(Int64 reportId)
    {
        const string sql = """
            SELECT * FROM v_ReportTaskListDB
            WHERE Rid = @reportId
            ORDER BY TargetDate
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_ReportTaskListDB>(sql, new { reportId });
    }
    #endregion

    #region + [3] 사용자별 조회: GetByUidAsync
    /// <summary>
    /// 특정 사용자의 보고서-업무 조회
    /// </summary>
    public async Task<List<v_ReportTaskListDB>> GetByUidAsync(Int64 uid)
    {
        const string sql = """
            SELECT * FROM v_ReportTaskListDB
            WHERE Uid = @uid
            ORDER BY Pid DESC, TargetDate
            """;

        using var connection = new SqlConnection(dbContext);
        var result = await connection.QueryAsync<v_ReportTaskListDB>(sql, new { uid });
        return result.AsList();
    }
    #endregion

    #region + [3-1] 사용자별 조회: GetByUidAllAsync (Alias)
    /// <summary>
    /// 특정 사용자의 모든 보고서-업무 조회
    /// </summary>
    public async Task<List<v_ReportTaskListDB>> GetByUidAllAsync(Int64 uid)
    {
        return await GetByUidAsync(uid);
    }
    #endregion

    #region + [3-2] Task_Number별 조회: GetByTaksListNumberAllAsync
    /// <summary>
    /// Task_Number별 보고서-업무 목록 조회
    /// </summary>
    public async Task<List<v_ReportTaskListDB>> GetByTaksListNumberAllAsync(Int64 TaksListNumber)
    {
        const string sql = """
            SELECT * FROM v_ReportTaskListDB
            WHERE TaksListNumber = @TaksListNumber
            ORDER BY Tid
            """;

        using var connection = new SqlConnection(dbContext);
        var result = await connection.QueryAsync<v_ReportTaskListDB>(sql, new { TaksListNumber });
        return result.AsList();
    }
    #endregion

    #region + [4] 프로세스별 조회: GetByProcessIdAsync
    /// <summary>
    /// 프로세스별 보고서-업무 조회
    /// </summary>
    public async Task<IEnumerable<v_ReportTaskListDB>> GetByProcessIdAsync(Int64 processId)
    {
        const string sql = """
            SELECT * FROM v_ReportTaskListDB
            WHERE Pid = @processId
            ORDER BY Rid, TargetDate
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_ReportTaskListDB>(sql, new { processId });
    }
    #endregion

    #region + [5] 업무 상태별 조회: GetByTaskStatusAsync
    /// <summary>
    /// 업무 상태별 조회
    /// </summary>
    public async Task<IEnumerable<v_ReportTaskListDB>> GetByTaskStatusAsync(int taskStatus)
    {
        const string sql = """
            SELECT * FROM v_ReportTaskListDB
            WHERE Task_Status = @taskStatus
            ORDER BY Pid DESC, TargetDate
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_ReportTaskListDB>(sql, new { taskStatus });
    }
    #endregion

    #region + [6] 달성률별 조회: GetByAchievementRateAsync
    /// <summary>
    /// 달성률 기준 조회 (특정 달성률 이상)
    /// </summary>
    public async Task<IEnumerable<v_ReportTaskListDB>> GetByAchievementRateAsync(decimal minRate)
    {
        const string sql = """
            SELECT * FROM v_ReportTaskListDB
            WHERE Task_Achievement_Rate >= @minRate
            ORDER BY Task_Achievement_Rate DESC
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_ReportTaskListDB>(sql, new { minRate });
    }
    #endregion

    #region + [7] 세부협의별 조회: GetBySubAgreementAsync
    /// <summary>
    /// 특정 세부협의 항목의 업무 조회
    /// </summary>
    public async Task<IEnumerable<v_ReportTaskListDB>> GetBySubAgreementAsync(Int64 subAgreementId)
    {
        const string sql = """
            SELECT * FROM v_ReportTaskListDB
            WHERE SAid = @subAgreementId
            ORDER BY TargetDate
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_ReportTaskListDB>(sql, new { subAgreementId });
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
