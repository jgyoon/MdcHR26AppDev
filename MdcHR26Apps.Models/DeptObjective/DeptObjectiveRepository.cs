using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;

namespace MdcHR26Apps.Models.DeptObjective;

public class DeptObjectiveRepository(string connectionString, ILoggerFactory loggerFactory) : IDeptObjectiveRepository, IDisposable
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger(nameof(DeptObjectiveRepository));
    private readonly string dbContext = connectionString;

    #region + [1] 입력: AddAsync
    public async Task<DeptObjectiveDb> AddAsync(DeptObjectiveDb model)
    {
        try
        {
            const string query =
                "INSERT INTO DeptObjectiveDb(" +
                    "EDepartId, ObjectiveTitle, ObjectiveContents, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, Remarks) " +
                "VALUES(" +
                    "@EDepartId, @ObjectiveTitle, @ObjectiveContents, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @Remarks);" +
                "Select Cast(SCOPE_IDENTITY() As Int);";

            using (var connection = new SqlConnection(dbContext))
            {
                int id = await connection.ExecuteScalarAsync<int>(query, model);
                model.DeptObjectiveDbId = id;
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
    public async Task<List<DeptObjectiveDb>> GetByAllAsync()
    {
        const string query = "Select * From DeptObjectiveDb Order By DeptObjectiveDbId";

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.QueryAsync<DeptObjectiveDb>(query, commandType: CommandType.Text);
            return result.ToList();
        }
    }
    #endregion

    #region + [3] 상세: GetByIdAsync
    public async Task<DeptObjectiveDb> GetByIdAsync(long id)
    {
        const string query = "Select * From DeptObjectiveDb Where DeptObjectiveDbId = @id";

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.QueryFirstOrDefaultAsync<DeptObjectiveDb>(query, new { id }, commandType: CommandType.Text);
            return result ?? new DeptObjectiveDb();
        }
    }
    #endregion

    #region + [4] 수정: UpdateAsync
    public async Task<bool> UpdateAsync(DeptObjectiveDb model)
    {
        const string query = @"
            Update DeptObjectiveDb
            Set
                EDepartId = @EDepartId,
                ObjectiveTitle = @ObjectiveTitle,
                ObjectiveContents = @ObjectiveContents,
                UpdatedBy = @UpdatedBy,
                UpdatedAt = @UpdatedAt,
                Remarks = @Remarks
            Where DeptObjectiveDbId = @DeptObjectiveDbId";
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
        const string query = "Delete DeptObjectiveDb Where DeptObjectiveDbId = @id";

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
