using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MdcHR26Apps.Models.Views.v_SubAgreementDB;

/// <summary>
/// v_SubAgreementDB Repository 구현 (Dapper, 읽기 전용)
/// Primary Constructor 사용 (C# 13)
/// </summary>
public class v_SubAgreementRepository(string connectionString, ILoggerFactory loggerFactory) : Iv_SubAgreementRepository
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<v_SubAgreementRepository>();
    private readonly string dbContext = connectionString;

    #region + [1] GetAllAsync
    /// <summary>
    /// 전체 세부협의 목록 조회
    /// </summary>
    public async Task<List<v_SubAgreementDB>> GetAllAsync()
    {
        const string sql = """
            SELECT * FROM v_SubAgreementDB
            ORDER BY Sid DESC
            """;

        using var connection = new SqlConnection(dbContext);
        var result = await connection.QueryAsync<v_SubAgreementDB>(sql);
        return result.AsList();
    }
    #endregion

    #region + [2] GetByIdAsync
    /// <summary>
    /// Sid로 세부협의 조회
    /// </summary>
    public async Task<v_SubAgreementDB?> GetByIdAsync(long sid)
    {
        const string sql = """
            SELECT * FROM v_SubAgreementDB
            WHERE Sid = @sid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<v_SubAgreementDB>(sql, new { sid });
    }
    #endregion

    #region + [3] GetByUserIdAsync
    /// <summary>
    /// 사용자별 세부협의 목록 조회
    /// </summary>
    public async Task<List<v_SubAgreementDB>> GetByUserIdAsync(Int64 uid)
    {
        const string sql = """
            SELECT * FROM v_SubAgreementDB
            WHERE Uid = @uid
            ORDER BY Sid DESC
            """;

        using var connection = new SqlConnection(dbContext);
        var result = await connection.QueryAsync<v_SubAgreementDB>(sql, new { uid });
        return result.AsList();
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
