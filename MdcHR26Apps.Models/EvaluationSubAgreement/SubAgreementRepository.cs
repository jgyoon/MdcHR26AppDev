using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;

namespace MdcHR26Apps.Models.EvaluationSubAgreement;

public class SubAgreementRepository(string connectionString, ILoggerFactory loggerFactory) : ISubAgreementRepository, IDisposable
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger(nameof(SubAgreementRepository));
    private readonly string dbContext = connectionString;

    #region + [1] 입력: AddAsync
    public async Task<SubAgreementDb> AddAsync(SubAgreementDb model)
    {
        try
        {
            const string query =
                "INSERT INTO SubAgreementDb(" +
                    "Uid, Report_Item_Number, Report_Item_Name_1, Report_Item_Name_2, Report_Item_Proportion, " +
                    "Report_SubItem_Name, Report_SubItem_Proportion, Task_Number) " +
                "VALUES(" +
                    "@Uid, @Report_Item_Number, @Report_Item_Name_1, @Report_Item_Name_2, @Report_Item_Proportion, " +
                    "@Report_SubItem_Name, @Report_SubItem_Proportion, @Task_Number);" +
                "Select Cast(SCOPE_IDENTITY() As Int);";

            using (var connection = new SqlConnection(dbContext))
            {
                int id = await connection.ExecuteScalarAsync<int>(query, model);
                model.Sid = id;
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
    public async Task<List<SubAgreementDb>> GetByAllAsync()
    {
        const string query = "Select * From SubAgreementDb Order By Sid";

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.QueryAsync<SubAgreementDb>(query, commandType: CommandType.Text);
            return result.ToList();
        }
    }
    #endregion

    #region + [3] 상세: GetByIdAsync
    public async Task<SubAgreementDb> GetByIdAsync(long id)
    {
        const string query = "Select * From SubAgreementDb Where Sid = @id";

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.QueryFirstOrDefaultAsync<SubAgreementDb>(query, new { id }, commandType: CommandType.Text);
            return result ?? new SubAgreementDb();
        }
    }
    #endregion

    #region + [4] 수정: UpdateAsync
    public async Task<bool> UpdateAsync(SubAgreementDb model)
    {
        const string query = @"
            Update SubAgreementDb
            Set
                Uid = @Uid,
                Report_Item_Number = @Report_Item_Number,
                Report_Item_Name_1 = @Report_Item_Name_1,
                Report_Item_Name_2 = @Report_Item_Name_2,
                Report_Item_Proportion = @Report_Item_Proportion,
                Report_SubItem_Name = @Report_SubItem_Name,
                Report_SubItem_Proportion = @Report_SubItem_Proportion,
                Task_Number = @Task_Number
            Where Sid = @Sid";
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
        const string query = "Delete SubAgreementDb Where Sid = @id";

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
    public async Task<List<SubAgreementDb>> GetByUserIdAllAsync(long userId)
    {
        const string query = "Select * From SubAgreementDb Where Uid = @userId Order By Sid";

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.QueryAsync<SubAgreementDb>(query, new { userId }, commandType: CommandType.Text);
            return result.ToList();
        }
    }
    #endregion

    #region + [7] 사용자 && 지표분류명 && 직무분류명 출력: GetByTasksPeroportionAsync
    public async Task<List<SubAgreementDb>> GetByTasksPeroportionAsync(long userId, string deptName, string indexName)
    {
        const string query = @"
            Select Top(1) *
            From SubAgreementDb
            Where Uid = @userId
                And Report_Item_Name_1 = @deptName
                And Report_Item_Name_2 = @indexName
            Order By Sid";

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.QueryAsync<SubAgreementDb>(query, new { userId, deptName, indexName }, commandType: CommandType.Text);
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
