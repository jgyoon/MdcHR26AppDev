using Dapper;
using MdcHR26Apps.Models.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MdcHR26Apps.Models.Department;

/// <summary>
/// EDepartmentDb Repository 구현 (Dapper)
/// </summary>
public class EDepartmentRepository(string connectionString, ILoggerFactory loggerFactory) : IEDepartmentRepository
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<EDepartmentRepository>();
    private readonly string dbContext = connectionString;

    #region + 참고
    // [4][2][2] 리포지토리 클래스(비동기 방식): Micro ORM인 Dapper를 사용하여 CRUD 구현
    #endregion

    #region + [1] 입력: AddAsync
    public async Task<Int64> AddAsync(EDepartmentDb department)
    {
        const string sql = """
            INSERT INTO EDepartmentDb
                (EDepartmentNo, EDepartmentName, ActivateStatus, Remarks)
            VALUES
                (@EDepartmentNo, @EDepartmentName, @ActivateStatus, @Remarks);
            SELECT CAST(SCOPE_IDENTITY() as bigint);
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteScalarAsync<Int64>(sql, department);
    }
    #endregion

    #region + [2] 출력: GetByAllAsync
    public async Task<IEnumerable<EDepartmentDb>> GetByAllAsync()
    {
        const string sql = "SELECT * FROM EDepartmentDb ORDER BY EDepartmentNo";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<EDepartmentDb>(sql);
    }
    #endregion

    #region + [2].[1] 출력(활성화만 출력): GetByAllWithActivateStatusAsync
    public async Task<IEnumerable<EDepartmentDb>> GetByAllWithActivateStatusAsync()
    {
        const string sql = "SELECT * FROM EDepartmentDb WHERE ActivateStatus = 1 ORDER BY EDepartmentNo";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<EDepartmentDb>(sql);
    }
    #endregion

    #region + [3] 상세: GetByIdAsync
    public async Task<EDepartmentDb?> GetByIdAsync(long departmentId)
    {
        const string sql = "SELECT * FROM EDepartmentDb WHERE EDepartId = @DepartmentId";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<EDepartmentDb>(sql, new { DepartmentId = departmentId });
    }
    #endregion

    #region + [4] 수정: UpdateAsync
    public async Task<int> UpdateAsync(EDepartmentDb department)
    {
        const string sql = """
            UPDATE EDepartmentDb
            SET
                EDepartmentNo = @EDepartmentNo,
                EDepartmentName = @EDepartmentName,
                ActivateStatus = @ActivateStatus,
                Remarks = @Remarks
            WHERE EDepartId = @EDepartId
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, department);
    }
    #endregion

    #region + [5] 삭제: DeleteAsync
    public async Task<int> DeleteAsync(long departmentId)
    {
        const string sql = "DELETE FROM EDepartmentDb WHERE EDepartId = @DepartmentId";

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, new { DepartmentId = departmentId });
    }
    #endregion

    #region + [6] 활성화된 부서 조회: GetActiveAsync
    public async Task<IEnumerable<EDepartmentDb>> GetActiveAsync()
    {
        const string sql = """
            SELECT * FROM EDepartmentDb
            WHERE ActivateStatus = 1
            ORDER BY EDepartmentNo
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<EDepartmentDb>(sql);
    }
    #endregion

    #region + [7] 드롭다운 목록: GetSelectListAsync
    public async Task<IEnumerable<SelectListModel>> GetSelectListAsync()
    {
        const string sql = """
            SELECT
                CAST(EDepartId AS NVARCHAR) AS Value,
                EDepartmentName AS Text
            FROM EDepartmentDb
            WHERE ActivateStatus = 1
            ORDER BY EDepartmentNo
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<SelectListModel>(sql);
    }
    #endregion

    #region + [8] 부서명으로 ID 조회: GetIdByNameAsync
    public async Task<long> GetIdByNameAsync(string name)
    {
        const string sql = """
            SELECT EDepartId
            FROM EDepartmentDb
            WHERE EDepartmentName = @Name
            """;

        using var connection = new SqlConnection(dbContext);
        var result = await connection.QueryFirstOrDefaultAsync<long?>(sql, new { Name = name });
        return result ?? 0;
    }
    #endregion

    #region + [9] 부서번호로 조회: GetByDepartmentNoAsync
    public async Task<EDepartmentDb?> GetByDepartmentNoAsync(int departmentNo)
    {
        const string sql = "SELECT * FROM EDepartmentDb WHERE EDepartmentNo = @DepartmentNo";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<EDepartmentDb>(sql, new { DepartmentNo = departmentNo });
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
