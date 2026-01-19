using Dapper;
using MdcHR26Apps.Models.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MdcHR26Apps.Models.Rank;

/// <summary>
/// ERankDb Repository 구현 (Dapper)
/// </summary>
public class ERankRepository(string connectionString, ILoggerFactory loggerFactory) : IERankRepository
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<ERankRepository>();
    private readonly string dbContext = connectionString;

    #region + 참고
    // [4][2][2] 리포지토리 클래스(비동기 방식): Micro ORM인 Dapper를 사용하여 CRUD 구현
    #endregion

    #region + [1] 입력: AddAsync
    public async Task<Int64> AddAsync(ERankDb rank)
    {
        const string sql = """
            INSERT INTO ERankDb
                (ERankNo, ERankName, ActivateStatus, Remarks)
            VALUES
                (@ERankNo, @ERankName, @ActivateStatus, @Remarks);
            SELECT CAST(SCOPE_IDENTITY() as bigint);
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteScalarAsync<Int64>(sql, rank);
    }
    #endregion

    #region + [2] 출력: GetByAllAsync
    public async Task<IEnumerable<ERankDb>> GetByAllAsync()
    {
        const string sql = "SELECT * FROM ERankDb ORDER BY ERankNo";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<ERankDb>(sql);
    }
    #endregion

    #region + [3] 상세: GetByIdAsync
    public async Task<ERankDb?> GetByIdAsync(long rankId)
    {
        const string sql = "SELECT * FROM ERankDb WHERE ERankId = @RankId";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<ERankDb>(sql, new { RankId = rankId });
    }
    #endregion

    #region + [4] 수정: UpdateAsync
    public async Task<int> UpdateAsync(ERankDb rank)
    {
        const string sql = """
            UPDATE ERankDb
            SET
                ERankNo = @ERankNo,
                ERankName = @ERankName,
                ActivateStatus = @ActivateStatus,
                Remarks = @Remarks
            WHERE ERankId = @ERankId
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, rank);
    }
    #endregion

    #region + [5] 삭제: DeleteAsync
    public async Task<int> DeleteAsync(long rankId)
    {
        const string sql = "DELETE FROM ERankDb WHERE ERankId = @RankId";

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, new { RankId = rankId });
    }
    #endregion

    #region + [6] 활성화된 직급 조회: GetActiveAsync
    public async Task<IEnumerable<ERankDb>> GetActiveAsync()
    {
        const string sql = """
            SELECT * FROM ERankDb
            WHERE ActivateStatus = 1
            ORDER BY ERankNo
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<ERankDb>(sql);
    }
    #endregion

    #region + [7] 드롭다운 목록: GetSelectListAsync
    public async Task<IEnumerable<SelectListModel>> GetSelectListAsync()
    {
        const string sql = """
            SELECT
                CAST(ERankId AS NVARCHAR) AS Value,
                ERankName AS Text
            FROM ERankDb
            WHERE ActivateStatus = 1
            ORDER BY ERankNo
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<SelectListModel>(sql);
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
