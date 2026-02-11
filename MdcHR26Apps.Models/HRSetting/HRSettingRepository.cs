using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MdcHR26Apps.Models.HRSetting;

/// <summary>
/// HRSetting Repository 구현 (Dapper)
/// Primary Constructor 사용 (C# 13)
/// </summary>
public class HRSettingRepository(string connectionString, ILoggerFactory loggerFactory) : IHRSettingRepository
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<HRSettingRepository>();
    private readonly string dbContext = connectionString;

    #region + [1] 현재 설정 조회: GetCurrentAsync
    /// <summary>
    /// 현재 시스템 설정 조회 (단일 레코드)
    /// </summary>
    public async Task<HRSettingDb?> GetCurrentAsync()
    {
        const string sql = "SELECT TOP 1 * FROM HRSetting";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<HRSettingDb>(sql);
    }
    #endregion

    #region + [2] 설정 업데이트: UpdateAsync
    /// <summary>
    /// 시스템 설정 업데이트
    /// </summary>
    public async Task<int> UpdateAsync(HRSettingDb setting)
    {
        const string sql = """
            UPDATE HRSetting
            SET Evaluation_Open = @Evaluation_Open,
                Edit_Open = @Edit_Open
            WHERE HRSid = @HRSid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, setting);
    }
    #endregion

    #region + [3] 초기화: InitializeAsync
    /// <summary>
    /// 초기 레코드 생성 (없을 경우)
    /// </summary>
    public async Task<int> InitializeAsync()
    {
        const string sql = """
            IF NOT EXISTS (SELECT 1 FROM HRSetting)
            BEGIN
                INSERT INTO HRSetting (Evaluation_Open, Edit_Open)
                VALUES (0, 0)
            END
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql);
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
