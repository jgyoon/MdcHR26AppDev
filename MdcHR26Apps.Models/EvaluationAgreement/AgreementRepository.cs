using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;

namespace MdcHR26Apps.Models.EvaluationAgreement;

public class AgreementRepository(string connectionString, ILoggerFactory loggerFactory) : IAgreementRepository, IDisposable
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger(nameof(AgreementRepository));
    private readonly string dbContext = connectionString;

    #region + [1] 입력: AddAsync
    public async Task<AgreementDb> AddAsync(AgreementDb model)
    {
        try
        {
            const string query =
                "INSERT INTO AgreementDb(" +
                    "Uid, Report_Item_Number, Report_Item_Name_1, Report_Item_Name_2, Report_Item_Proportion) " +
                "VALUES(" +
                    "@Uid, @Report_Item_Number, @Report_Item_Name_1, @Report_Item_Name_2, @Report_Item_Proportion);" +
                "Select Cast(SCOPE_IDENTITY() As Int);";

            using (var connection = new SqlConnection(dbContext))
            {
                int id = await connection.ExecuteScalarAsync<int>(query, model);
                model.Aid = id;
            }
        }
        catch (Exception e)
        {
            _logger?.LogError($"ERROR({nameof(AddAsync)}): {e.Message}");
        }

        return model;
    }
    #endregion

    #region + [2] 출력: GetByAllAsync
    public async Task<List<AgreementDb>> GetByAllAsync()
    {
        const string query = "Select * From AgreementDb Order By Aid";

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.QueryAsync<AgreementDb>(query, commandType: CommandType.Text);
            return result.ToList();
        }
    }
    #endregion

    #region + [3] 상세: GetByIdAsync
    public async Task<AgreementDb> GetByIdAsync(long id)
    {
        const string query = "Select * From AgreementDb Where Aid = @id";

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.QueryFirstOrDefaultAsync<AgreementDb>(query, new { id }, commandType: CommandType.Text);
            return result ?? new AgreementDb();
        }
    }
    #endregion

    #region + [4] 수정: UpdateAsync
    public async Task<bool> UpdateAsync(AgreementDb model)
    {
        const string query = @"
            Update AgreementDb
            Set
                Uid = @Uid,
                Report_Item_Number = @Report_Item_Number,
                Report_Item_Name_1 = @Report_Item_Name_1,
                Report_Item_Name_2 = @Report_Item_Name_2,
                Report_Item_Proportion = @Report_Item_Proportion
            Where Aid = @Aid";
        try
        {
            using (var connection = new SqlConnection(dbContext))
            {
                return await connection.ExecuteAsync(query, model) > 0;
            }
        }
        catch (Exception e)
        {
            _logger?.LogError($"ERROR({nameof(UpdateAsync)}): {e.Message}");
        }

        return false;
    }
    #endregion

    #region + [5] 삭제: DeleteAsync
    public async Task<bool> DeleteAsync(long id)
    {
        const string query = "Delete AgreementDb Where Aid = @id";

        try
        {
            using (var connection = new SqlConnection(dbContext))
            {
                return await connection.ExecuteAsync(query, new { id }, commandType: CommandType.Text) > 0;
            }
        }
        catch (Exception er)
        {
            _logger?.LogError($"ERROR({nameof(DeleteAsync)}): {er.Message}");
        }

        return false;
    }
    #endregion

    #region + [6] 사용자별 출력: GetByUserIdAllAsync
    public async Task<List<AgreementDb>> GetByUserIdAllAsync(long userId)
    {
        const string query = "Select * From AgreementDb Where Uid = @userId Order By Aid";

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.QueryAsync<AgreementDb>(query, new { userId }, commandType: CommandType.Text);
            return result.ToList();
        }
    }
    #endregion

    #region + [7] 평가별 비중 출력: GetByTasksPeroportionAsync
    public async Task<List<AgreementDb>> GetByTasksPeroportionAsync(long userId, string deptName, string indexName)
    {
        const string query = @"
            Select Top(1) *
            From AgreementDb
            Where Uid = @userId
                And Report_Item_Name_1 = @deptName
                And Report_Item_Name_2 = @indexName
            Order By Aid";

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.QueryAsync<AgreementDb>(query, new { userId, deptName, indexName }, commandType: CommandType.Text);
            return result.ToList();
        }
    }
    #endregion

    #region + Dispose
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (db != null)
            {
                db.Dispose();
            }
        }
    }
    #endregion
}
