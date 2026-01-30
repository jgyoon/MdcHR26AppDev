using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MdcHR26Apps.Models.EvaluationAgreement;

/// <summary>
/// AgreementDb Repository 구현 (Dapper)
/// Primary Constructor 사용 (C# 13)
/// </summary>
public class AgreementRepository(string connectionString, ILoggerFactory loggerFactory) : IAgreementRepository
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<AgreementRepository>();
    private readonly string dbContext = connectionString;

    #region + [1] 입력: AddAsync
    /// <summary>
    /// 직무평가 협의서 추가
    /// </summary>
    public async Task<Int64> AddAsync(AgreementDb model)
    {
        const string sql = """
            INSERT INTO AgreementDb(
                Uid, Agreement_Item_Number, Agreement_Item_Name_1, Agreement_Item_Name_2,
                Agreement_Item_Proportion, DeptObjective_Number, Is_Agreement,
                Agreement_Comment, Agreement_Date)
            VALUES(
                @Uid, @Agreement_Item_Number, @Agreement_Item_Name_1, @Agreement_Item_Name_2,
                @Agreement_Item_Proportion, @DeptObjective_Number, @Is_Agreement,
                @Agreement_Comment, @Agreement_Date);
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteScalarAsync<Int64>(sql, model);
    }
    #endregion

    #region + [2] 출력: GetByAllAsync
    /// <summary>
    /// 전체 협의서 조회
    /// </summary>
    public async Task<IEnumerable<AgreementDb>> GetByAllAsync()
    {
        const string sql = "SELECT * FROM AgreementDb ORDER BY Aid";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<AgreementDb>(sql);
    }
    #endregion

    #region + [3] 상세: GetByIdAsync
    /// <summary>
    /// ID로 협의서 조회
    /// </summary>
    public async Task<AgreementDb?> GetByIdAsync(Int64 id)
    {
        const string sql = "SELECT * FROM AgreementDb WHERE Aid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<AgreementDb>(sql, new { id });
    }
    #endregion

    #region + [4] 수정: UpdateAsync
    /// <summary>
    /// 협의서 수정
    /// </summary>
    public async Task<int> UpdateAsync(AgreementDb model)
    {
        const string sql = """
            UPDATE AgreementDb
            SET
                Uid = @Uid,
                Agreement_Item_Number = @Agreement_Item_Number,
                Agreement_Item_Name_1 = @Agreement_Item_Name_1,
                Agreement_Item_Name_2 = @Agreement_Item_Name_2,
                Agreement_Item_Proportion = @Agreement_Item_Proportion,
                DeptObjective_Number = @DeptObjective_Number,
                Is_Agreement = @Is_Agreement,
                Agreement_Comment = @Agreement_Comment,
                Agreement_Date = @Agreement_Date
            WHERE Aid = @Aid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, model);
    }
    #endregion

    #region + [5] 삭제: DeleteAsync
    /// <summary>
    /// 협의서 삭제
    /// </summary>
    public async Task<int> DeleteAsync(Int64 id)
    {
        const string sql = "DELETE FROM AgreementDb WHERE Aid = @id";

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, new { id });
    }
    #endregion

    #region + [6] 사용자별 조회: GetByUidAsync
    /// <summary>
    /// 사용자별 협의서 조회
    /// </summary>
    public async Task<IEnumerable<AgreementDb>> GetByUidAsync(Int64 uid)
    {
        const string sql = """
            SELECT * FROM AgreementDb
            WHERE Uid = @uid
            ORDER BY Agreement_Item_Number
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<AgreementDb>(sql, new { uid });
    }
    #endregion

    #region + [7] 합의 완료 확인: IsAgreementCompleteAsync
    /// <summary>
    /// 합의 완료 여부 확인 (모든 항목이 합의되었는지)
    /// </summary>
    public async Task<bool> IsAgreementCompleteAsync(Int64 uid)
    {
        const string sql = """
            SELECT COUNT(*)
            FROM AgreementDb
            WHERE Uid = @uid AND Is_Agreement = 0
            """;

        using var connection = new SqlConnection(dbContext);
        int count = await connection.ExecuteScalarAsync<int>(sql, new { uid });
        return count == 0; // 미합의 항목이 0개면 완료
    }
    #endregion

    #region + [8] 합의 대기: GetPendingAgreementAsync
    /// <summary>
    /// 합의 대기 중인 항목 조회
    /// </summary>
    public async Task<IEnumerable<AgreementDb>> GetPendingAgreementAsync(Int64 uid)
    {
        const string sql = """
            SELECT * FROM AgreementDb
            WHERE Uid = @uid AND Is_Agreement = 0
            ORDER BY Agreement_Item_Number
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<AgreementDb>(sql, new { uid });
    }
    #endregion

    #region + [9] 부서 목표별 조회: GetByDeptObjectiveAsync
    /// <summary>
    /// 부서 목표별 협의서 조회
    /// </summary>
    public async Task<IEnumerable<AgreementDb>> GetByDeptObjectiveAsync(Int64 deptObjectiveId)
    {
        const string sql = """
            SELECT * FROM AgreementDb
            WHERE DeptObjective_Number = @deptObjectiveId
            ORDER BY Aid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<AgreementDb>(sql, new { deptObjectiveId });
    }
    #endregion

    #region + [10] 사용자별 개수 조회: GetCountByUidAsync
    /// <summary>
    /// 사용자별 협의서 개수 조회
    /// </summary>
    public async Task<int> GetCountByUidAsync(long uid)
    {
        const string sql = "SELECT COUNT(*) FROM AgreementDb WHERE Uid = @Uid";

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteScalarAsync<int>(sql, new { Uid = uid });
    }
    #endregion

    #region + [11] 사용자별 전체 삭제: DeleteAllByUidAsync
    /// <summary>
    /// 사용자별 협의서 전체 삭제
    /// </summary>
    public async Task<bool> DeleteAllByUidAsync(long uid)
    {
        const string sql = "DELETE FROM AgreementDb WHERE Uid = @Uid";

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
