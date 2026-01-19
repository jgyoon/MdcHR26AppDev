using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MdcHR26Apps.Models.DeptObjective;

/// <summary>
/// DeptObjectiveDb Repository 구현 (Dapper)
/// Primary Constructor 사용 (C# 13)
/// </summary>
public class DeptObjectiveRepository(string connectionString, ILoggerFactory loggerFactory) : IDeptObjectiveRepository
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<DeptObjectiveRepository>();
    private readonly string dbContext = connectionString;

    #region + [1] 입력: AddAsync
    /// <summary>
    /// 부서 목표 추가
    /// </summary>
    public async Task<Int64> AddAsync(DeptObjectiveDb model)
    {
        const string sql = """
            INSERT INTO DeptObjectiveDb(
                EDepartId, Objective_Title, Objective_Content, Achievement_Criteria, IsActive)
            VALUES(
                @EDepartId, @Objective_Title, @Objective_Content, @Achievement_Criteria, @IsActive);
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteScalarAsync<Int64>(sql, model);
    }
    #endregion

    #region + [2] 출력: GetByAllAsync
    /// <summary>
    /// 전체 부서 목표 조회
    /// </summary>
    public async Task<IEnumerable<DeptObjectiveDb>> GetByAllAsync()
    {
        const string sql = "SELECT * FROM DeptObjectiveDb ORDER BY DOid";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<DeptObjectiveDb>(sql);
    }
    #endregion

    #region + [3] 상세: GetByIdAsync
    /// <summary>
    /// ID로 부서 목표 조회
    /// </summary>
    public async Task<DeptObjectiveDb?> GetByIdAsync(Int64 id)
    {
        const string sql = "SELECT * FROM DeptObjectiveDb WHERE DOid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<DeptObjectiveDb>(sql, new { id });
    }
    #endregion

    #region + [4] 수정: UpdateAsync
    /// <summary>
    /// 부서 목표 수정
    /// </summary>
    public async Task<int> UpdateAsync(DeptObjectiveDb model)
    {
        const string sql = """
            UPDATE DeptObjectiveDb
            SET
                EDepartId = @EDepartId,
                Objective_Title = @Objective_Title,
                Objective_Content = @Objective_Content,
                Achievement_Criteria = @Achievement_Criteria,
                IsActive = @IsActive
            WHERE DOid = @DOid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, model);
    }
    #endregion

    #region + [5] 삭제: DeleteAsync
    /// <summary>
    /// 부서 목표 삭제
    /// </summary>
    public async Task<int> DeleteAsync(Int64 id)
    {
        const string sql = "DELETE FROM DeptObjectiveDb WHERE DOid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, new { id });
    }
    #endregion

    #region + [6] 부서별 조회: GetByDepartmentAsync
    /// <summary>
    /// 부서별 목표 조회
    /// </summary>
    public async Task<IEnumerable<DeptObjectiveDb>> GetByDepartmentAsync(Int64 departmentId)
    {
        const string sql = """
            SELECT * FROM DeptObjectiveDb
            WHERE EDepartId = @departmentId
            ORDER BY DOid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<DeptObjectiveDb>(sql, new { departmentId });
    }
    #endregion

    #region + [7] 활성화된 목표: GetActiveAsync
    /// <summary>
    /// 활성화된 부서 목표 조회
    /// </summary>
    public async Task<IEnumerable<DeptObjectiveDb>> GetActiveAsync()
    {
        const string sql = """
            SELECT * FROM DeptObjectiveDb
            WHERE IsActive = 1
            ORDER BY DOid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<DeptObjectiveDb>(sql);
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
