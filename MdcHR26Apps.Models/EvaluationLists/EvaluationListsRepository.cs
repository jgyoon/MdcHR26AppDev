using Dapper;
using MdcHR26Apps.Models.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MdcHR26Apps.Models.EvaluationLists;

/// <summary>
/// EvaluationLists Repository 구현 (Dapper)
/// Primary Constructor 사용 (C# 13)
/// </summary>
public class EvaluationListsRepository(string connectionString, ILoggerFactory loggerFactory) : IEvaluationListsRepository
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<EvaluationListsRepository>();
    private readonly string dbContext = connectionString;

    #region + [1] 입력: AddAsync
    /// <summary>
    /// 평가 항목 추가
    /// </summary>
    public async Task<Int64> AddAsync(EvaluationLists model)
    {
        const string sql = """
            INSERT INTO EvaluationLists(
                Evaluation_Number, Evaluation_Item, Evaluation_Description, Score, IsActive)
            VALUES(
                @Evaluation_Number, @Evaluation_Item, @Evaluation_Description, @Score, @IsActive);
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteScalarAsync<Int64>(sql, model);
    }
    #endregion

    #region + [2] 출력: GetByAllAsync
    /// <summary>
    /// 전체 평가 항목 조회
    /// </summary>
    public async Task<IEnumerable<EvaluationLists>> GetByAllAsync()
    {
        const string sql = "SELECT * FROM EvaluationLists ORDER BY Evaluation_Number";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<EvaluationLists>(sql);
    }
    #endregion

    #region + [3] 상세: GetByIdAsync
    /// <summary>
    /// ID로 평가 항목 조회
    /// </summary>
    public async Task<EvaluationLists?> GetByIdAsync(Int64 id)
    {
        const string sql = "SELECT * FROM EvaluationLists WHERE ELid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<EvaluationLists>(sql, new { id });
    }
    #endregion

    #region + [4] 수정: UpdateAsync
    /// <summary>
    /// 평가 항목 수정
    /// </summary>
    public async Task<int> UpdateAsync(EvaluationLists model)
    {
        const string sql = """
            UPDATE EvaluationLists
            SET
                Evaluation_Number = @Evaluation_Number,
                Evaluation_Item = @Evaluation_Item,
                Evaluation_Description = @Evaluation_Description,
                Score = @Score,
                IsActive = @IsActive
            WHERE ELid = @ELid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, model);
    }
    #endregion

    #region + [5] 삭제: DeleteAsync
    /// <summary>
    /// 평가 항목 삭제
    /// </summary>
    public async Task<int> DeleteAsync(Int64 id)
    {
        const string sql = "DELETE FROM EvaluationLists WHERE ELid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, new { id });
    }
    #endregion

    #region + [6] 활성화된 항목: GetActiveAsync
    /// <summary>
    /// 활성화된 평가 항목 조회
    /// </summary>
    public async Task<IEnumerable<EvaluationLists>> GetActiveAsync()
    {
        const string sql = """
            SELECT * FROM EvaluationLists
            WHERE IsActive = 1
            ORDER BY Evaluation_Number
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<EvaluationLists>(sql);
    }
    #endregion

    #region + [7] 평가 번호로 조회: GetByNumberAsync
    /// <summary>
    /// 평가 번호로 조회
    /// </summary>
    public async Task<EvaluationLists?> GetByNumberAsync(int evaluationNumber)
    {
        const string sql = """
            SELECT * FROM EvaluationLists
            WHERE Evaluation_Number = @evaluationNumber
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<EvaluationLists>(sql, new { evaluationNumber });
    }
    #endregion

    #region + [8] 드롭다운 목록: GetSelectListAsync
    /// <summary>
    /// 드롭다운용 평가 항목 목록
    /// </summary>
    public async Task<IEnumerable<SelectListModel>> GetSelectListAsync()
    {
        const string sql = """
            SELECT
                CAST(ELid AS NVARCHAR) AS Value,
                Evaluation_Item + ' (' + CAST(Score AS NVARCHAR) + '점)' AS Text
            FROM EvaluationLists
            WHERE IsActive = 1
            ORDER BY Evaluation_Number
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
