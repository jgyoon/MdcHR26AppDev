using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MdcHR26Apps.Models.Views.v_EvaluationUsersList;

public class v_EvaluationUsersListRepository(string connectionString, ILoggerFactory loggerFactory)
    : Iv_EvaluationUsersListRepository
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<v_EvaluationUsersListRepository>();
    private readonly string dbContext = connectionString;

    #region + [1] 전체 조회: GetByAllAsync
    public async Task<IEnumerable<v_EvaluationUsersList>> GetByAllAsync()
    {
        const string sql = "SELECT * FROM v_EvaluationUsersList ORDER BY EUid";
        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_EvaluationUsersList>(sql);
    }
    #endregion

    #region + [2] Uid로 조회: GetByUidAsync
    public async Task<v_EvaluationUsersList?> GetByUidAsync(Int64 uid)
    {
        const string sql = "SELECT * FROM v_EvaluationUsersList WHERE Uid = @uid";
        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<v_EvaluationUsersList>(sql, new { uid });
    }
    #endregion

    #region + [3] 이름 검색: SearchByNameAsync
    public async Task<IEnumerable<v_EvaluationUsersList>> SearchByNameAsync(string userName)
    {
        const string sql = """
            SELECT * FROM v_EvaluationUsersList
            WHERE UserName LIKE @userName
            ORDER BY EUid
            """;
        using var connection = new SqlConnection(dbContext);
        // NVARCHAR 검색: 파라미터에 와일드카드 포함
        return await connection.QueryAsync<v_EvaluationUsersList>(sql, new { userName = userName + "%" });
    }
    #endregion

    #region + [#] Dispose
    public void Dispose()
    {
        db?.Dispose();
        GC.SuppressFinalize(this);
    }
    #endregion
}
