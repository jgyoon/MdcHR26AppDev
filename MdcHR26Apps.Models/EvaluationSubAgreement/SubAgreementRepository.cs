using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MdcHR26Apps.Models.EvaluationSubAgreement;

/// <summary>
/// SubAgreementDb Repository 구현 (Dapper)
/// Primary Constructor 사용 (C# 13)
/// </summary>
public class SubAgreementRepository(string connectionString, ILoggerFactory loggerFactory) : ISubAgreementRepository
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<SubAgreementRepository>();
    private readonly string dbContext = connectionString;

    #region + [1] 입력: AddAsync
    /// <summary>
    /// 상세 협의서 추가
    /// </summary>
    public async Task<Int64> AddAsync(SubAgreementDb model)
    {
        const string sql = """
            INSERT INTO SubAgreementDb(
                Uid, Agreement_Number, SubAgreement_Item_Name, SubAgreement_Item_Proportion,
                Is_SubAgreement, SubAgreement_Comment, SubAgreement_Date)
            VALUES(
                @Uid, @Agreement_Number, @SubAgreement_Item_Name, @SubAgreement_Item_Proportion,
                @Is_SubAgreement, @SubAgreement_Comment, @SubAgreement_Date);
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteScalarAsync<Int64>(sql, model);
    }
    #endregion

    #region + [2] 출력: GetByAllAsync
    /// <summary>
    /// 전체 상세 협의서 조회
    /// </summary>
    public async Task<IEnumerable<SubAgreementDb>> GetByAllAsync()
    {
        const string sql = "SELECT * FROM SubAgreementDb ORDER BY SAid";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<SubAgreementDb>(sql);
    }
    #endregion

    #region + [3] 상세: GetByIdAsync
    /// <summary>
    /// ID로 상세 협의서 조회
    /// </summary>
    public async Task<SubAgreementDb?> GetByIdAsync(Int64 id)
    {
        const string sql = "SELECT * FROM SubAgreementDb WHERE SAid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<SubAgreementDb>(sql, new { id });
    }
    #endregion

    #region + [4] 수정: UpdateAsync
    /// <summary>
    /// 상세 협의서 수정
    /// </summary>
    public async Task<int> UpdateAsync(SubAgreementDb model)
    {
        const string sql = """
            UPDATE SubAgreementDb
            SET
                Uid = @Uid,
                Agreement_Number = @Agreement_Number,
                SubAgreement_Item_Name = @SubAgreement_Item_Name,
                SubAgreement_Item_Proportion = @SubAgreement_Item_Proportion,
                Is_SubAgreement = @Is_SubAgreement,
                SubAgreement_Comment = @SubAgreement_Comment,
                SubAgreement_Date = @SubAgreement_Date
            WHERE SAid = @SAid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, model);
    }
    #endregion

    #region + [5] 삭제: DeleteAsync
    /// <summary>
    /// 상세 협의서 삭제
    /// </summary>
    public async Task<int> DeleteAsync(Int64 id)
    {
        const string sql = "DELETE FROM SubAgreementDb WHERE SAid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, new { id });
    }
    #endregion

    #region + [6] 사용자별 조회: GetByUidAsync
    /// <summary>
    /// 사용자별 상세 협의서 조회
    /// </summary>
    public async Task<IEnumerable<SubAgreementDb>> GetByUidAsync(Int64 uid)
    {
        const string sql = """
            SELECT * FROM SubAgreementDb
            WHERE Uid = @uid
            ORDER BY SAid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<SubAgreementDb>(sql, new { uid });
    }
    #endregion

    #region + [7] 상위 협의서별 조회: GetByAgreementNumberAsync
    /// <summary>
    /// 상위 협의서별 세부 항목 조회
    /// </summary>
    public async Task<IEnumerable<SubAgreementDb>> GetByAgreementNumberAsync(Int64 agreementNumber)
    {
        const string sql = """
            SELECT * FROM SubAgreementDb
            WHERE Agreement_Number = @agreementNumber
            ORDER BY SAid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<SubAgreementDb>(sql, new { agreementNumber });
    }
    #endregion

    #region + [8] 합의 완료 확인: IsSubAgreementCompleteAsync
    /// <summary>
    /// 합의 완료 여부 확인
    /// </summary>
    public async Task<bool> IsSubAgreementCompleteAsync(Int64 uid)
    {
        const string sql = """
            SELECT COUNT(*)
            FROM SubAgreementDb
            WHERE Uid = @uid AND Is_SubAgreement = 0
            """;

        using var connection = new SqlConnection(dbContext);
        int count = await connection.ExecuteScalarAsync<int>(sql, new { uid });
        return count == 0;
    }
    #endregion

    #region + [9] 합의 대기: GetPendingSubAgreementAsync
    /// <summary>
    /// 합의 대기 중인 세부 항목 조회
    /// </summary>
    public async Task<IEnumerable<SubAgreementDb>> GetPendingSubAgreementAsync(Int64 uid)
    {
        const string sql = """
            SELECT * FROM SubAgreementDb
            WHERE Uid = @uid AND Is_SubAgreement = 0
            ORDER BY SAid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<SubAgreementDb>(sql, new { uid });
    }
    #endregion

    #region + [10] 비율 합계: GetTotalProportionAsync
    /// <summary>
    /// 상위 협의서의 세부 항목 비율 합계
    /// </summary>
    public async Task<int> GetTotalProportionAsync(Int64 agreementNumber)
    {
        const string sql = """
            SELECT COALESCE(SUM(SubAgreement_Item_Proportion), 0)
            FROM SubAgreementDb
            WHERE Agreement_Number = @agreementNumber
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteScalarAsync<int>(sql, new { agreementNumber });
    }
    #endregion

    #region + [11] 사용자별 개수 조회: GetCountByUidAsync
    /// <summary>
    /// 사용자별 세부 협의서 개수 조회
    /// </summary>
    public async Task<int> GetCountByUidAsync(long uid)
    {
        const string sql = "SELECT COUNT(*) FROM SubAgreementDb WHERE Uid = @Uid";

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteScalarAsync<int>(sql, new { Uid = uid });
    }
    #endregion

    #region + [12] 사용자별 전체 삭제: DeleteAllByUidAsync
    /// <summary>
    /// 사용자별 세부 협의서 전체 삭제
    /// </summary>
    public async Task<bool> DeleteAllByUidAsync(long uid)
    {
        const string sql = "DELETE FROM SubAgreementDb WHERE Uid = @Uid";

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
