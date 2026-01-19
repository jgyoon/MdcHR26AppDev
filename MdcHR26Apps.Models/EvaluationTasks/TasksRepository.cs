using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MdcHR26Apps.Models.EvaluationTasks;

/// <summary>
/// TasksDb Repository 구현 (Dapper)
/// Primary Constructor 사용 (C# 13)
/// </summary>
public class TasksRepository(string connectionString, ILoggerFactory loggerFactory) : ITasksRepository
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<TasksRepository>();
    private readonly string dbContext = connectionString;

    #region + [1] 입력: AddAsync
    /// <summary>
    /// 업무 추가
    /// </summary>
    public async Task<Int64> AddAsync(TasksDb model)
    {
        const string sql = """
            INSERT INTO TasksDb(
                Uid, SubAgreement_Number, Task_Name, Task_Content,
                Task_Criteria, Estimated_Hours, Is_Completed)
            VALUES(
                @Uid, @SubAgreement_Number, @Task_Name, @Task_Content,
                @Task_Criteria, @Estimated_Hours, @Is_Completed);
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteScalarAsync<Int64>(sql, model);
    }
    #endregion

    #region + [2] 출력: GetByAllAsync
    /// <summary>
    /// 전체 업무 조회
    /// </summary>
    public async Task<IEnumerable<TasksDb>> GetByAllAsync()
    {
        const string sql = "SELECT * FROM TasksDb ORDER BY Tid";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<TasksDb>(sql);
    }
    #endregion

    #region + [3] 상세: GetByIdAsync
    /// <summary>
    /// ID로 업무 조회
    /// </summary>
    public async Task<TasksDb?> GetByIdAsync(Int64 id)
    {
        const string sql = "SELECT * FROM TasksDb WHERE Tid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<TasksDb>(sql, new { id });
    }
    #endregion

    #region + [4] 수정: UpdateAsync
    /// <summary>
    /// 업무 수정
    /// </summary>
    public async Task<int> UpdateAsync(TasksDb model)
    {
        const string sql = """
            UPDATE TasksDb
            SET
                Uid = @Uid,
                SubAgreement_Number = @SubAgreement_Number,
                Task_Name = @Task_Name,
                Task_Content = @Task_Content,
                Task_Criteria = @Task_Criteria,
                Estimated_Hours = @Estimated_Hours,
                Is_Completed = @Is_Completed
            WHERE Tid = @Tid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, model);
    }
    #endregion

    #region + [5] 삭제: DeleteAsync
    /// <summary>
    /// 업무 삭제
    /// </summary>
    public async Task<int> DeleteAsync(Int64 id)
    {
        const string sql = "DELETE FROM TasksDb WHERE Tid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, new { id });
    }
    #endregion

    #region + [6] 사용자별 조회: GetByUidAsync
    /// <summary>
    /// 사용자별 업무 조회
    /// </summary>
    public async Task<IEnumerable<TasksDb>> GetByUidAsync(Int64 uid)
    {
        const string sql = """
            SELECT * FROM TasksDb
            WHERE Uid = @uid
            ORDER BY Tid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<TasksDb>(sql, new { uid });
    }
    #endregion

    #region + [7] 세부 협의서별 조회: GetBySubAgreementNumberAsync
    /// <summary>
    /// 세부 협의서별 업무 조회
    /// </summary>
    public async Task<IEnumerable<TasksDb>> GetBySubAgreementNumberAsync(Int64 subAgreementNumber)
    {
        const string sql = """
            SELECT * FROM TasksDb
            WHERE SubAgreement_Number = @subAgreementNumber
            ORDER BY Tid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<TasksDb>(sql, new { subAgreementNumber });
    }
    #endregion

    #region + [8] 미완료 업무: GetIncompleteTasksAsync
    /// <summary>
    /// 미완료 업무 조회
    /// </summary>
    public async Task<IEnumerable<TasksDb>> GetIncompleteTasksAsync(Int64 uid)
    {
        const string sql = """
            SELECT * FROM TasksDb
            WHERE Uid = @uid AND Is_Completed = 0
            ORDER BY Tid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<TasksDb>(sql, new { uid });
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
