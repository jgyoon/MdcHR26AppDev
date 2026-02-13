using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MdcHR26Apps.Models.Views.v_AgreementDB;

/// <summary>
/// v_AgreementDB Repository 구현 (Dapper, 읽기 전용)
/// Primary Constructor 사용 (C# 13)
/// </summary>
public class v_AgreementRepository(string connectionString, ILoggerFactory loggerFactory) : Iv_AgreementRepository
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<v_AgreementRepository>();
    private readonly string dbContext = connectionString;

    #region + [1] GetAllAsync
    /// <summary>
    /// 전체 협의 목록 조회
    /// </summary>
    public async Task<List<v_AgreementDB>> GetAllAsync()
    {
        const string sql = """
            SELECT * FROM v_AgreementDB
            ORDER BY Aid DESC
            """;

        using var connection = new SqlConnection(dbContext);
        var result = await connection.QueryAsync<v_AgreementDB>(sql);
        return result.AsList();
    }
    #endregion

    #region + [2] GetByIdAsync
    /// <summary>
    /// Aid로 협의 조회
    /// </summary>
    public async Task<v_AgreementDB?> GetByIdAsync(long aid)
    {
        const string sql = """
            SELECT * FROM v_AgreementDB
            WHERE Aid = @aid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<v_AgreementDB>(sql, new { aid });
    }
    #endregion

    #region + [3] GetByUserIdAsync
    /// <summary>
    /// 사용자별 협의 목록 조회
    /// </summary>
    public async Task<List<v_AgreementDB>> GetByUserIdAsync(Int64 uid)
    {
        const string sql = """
            SELECT * FROM v_AgreementDB
            WHERE Uid = @uid
            ORDER BY Aid DESC
            """;

        using var connection = new SqlConnection(dbContext);
        var result = await connection.QueryAsync<v_AgreementDB>(sql, new { uid });
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
