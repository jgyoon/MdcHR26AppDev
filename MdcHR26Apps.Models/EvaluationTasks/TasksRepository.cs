using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;

namespace MdcHR26Apps.Models.EvaluationTasks;

public class TasksRepository(string connectionString, ILoggerFactory loggerFactory) : ITasksRepository, IDisposable
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger(nameof(TasksRepository));
    private readonly string dbContext = connectionString;

    #region + [1] 입력: AddAsync
    public async Task<TasksDb> AddAsync(TasksDb model)
    {
        try
        {
            const string query =
                "INSERT INTO TasksDb(" +
                    "TaskName, TaksListNumber, TaskStatus, TaskObjective, TargetProportion, ResultProportion, " +
                    "TargetDate, ResultDate, Task_Evaluation_1, Task_Evaluation_2, TaskLevel, TaskComments) " +
                "VALUES(" +
                    "@TaskName, @TaksListNumber, @TaskStatus, @TaskObjective, @TargetProportion, @ResultProportion, " +
                    "@TargetDate, @ResultDate, @Task_Evaluation_1, @Task_Evaluation_2, @TaskLevel, @TaskComments);" +
                "Select Cast(SCOPE_IDENTITY() As Int);";

            using (var connection = new SqlConnection(dbContext))
            {
                int id = await connection.ExecuteScalarAsync<int>(query, model);
                model.Tid = id;
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
    public async Task<List<TasksDb>> GetByAllAsync()
    {
        const string query = "Select * From TasksDb Order By Tid";

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.QueryAsync<TasksDb>(query, commandType: CommandType.Text);
            return result.ToList();
        }
    }
    #endregion

    #region + [3] 상세: GetByIdAsync
    public async Task<TasksDb> GetByIdAsync(long id)
    {
        const string query = "Select * From TasksDb Where Tid = @id";

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.QueryFirstOrDefaultAsync<TasksDb>(query, new { id }, commandType: CommandType.Text);
            return result ?? new TasksDb();
        }
    }
    #endregion

    #region + [4] 수정: UpdateAsync
    public async Task<bool> UpdateAsync(TasksDb model)
    {
        const string query = @"
            Update TasksDb
            Set
                TaskName = @TaskName,
                TaksListNumber = @TaksListNumber,
                TaskStatus = @TaskStatus,
                TaskObjective = @TaskObjective,
                TargetProportion = @TargetProportion,
                ResultProportion = @ResultProportion,
                TargetDate = @TargetDate,
                ResultDate = @ResultDate,
                Task_Evaluation_1 = @Task_Evaluation_1,
                Task_Evaluation_2 = @Task_Evaluation_2,
                TaskLevel = @TaskLevel,
                TaskComments = @TaskComments
            Where Tid = @Tid";
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
        const string query = "Delete TasksDb Where Tid = @id";

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

    #region + [6] 리스트번호별 출력: GetByListNoAllAsync
    public async Task<List<TasksDb>> GetByListNoAllAsync(long taksListNumber)
    {
        const string query = "Select * From TasksDb Where TaksListNumber = @taksListNumber Order By Tid";

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.QueryAsync<TasksDb>(query, new { taksListNumber }, commandType: CommandType.Text);
            return result.ToList();
        }
    }
    #endregion

    #region + [7] 리스트번호별 삭제: DeleteByListNoAllAsync
    public async Task<bool> DeleteByListNoAllAsync(long taksListNumber)
    {
        const string query = "Delete TasksDb Where TaksListNumber = @taksListNumber";

        try
        {
            using (var connection = new SqlConnection(dbContext))
            {
                return await connection.ExecuteAsync(query, new { taksListNumber }, commandType: CommandType.Text) > 0;
            }
        }
        catch (Exception er)
        {
            _logger?.LogError($"ERROR({nameof(DeleteByListNoAllAsync)}): {er.Message}");
        }

        return false;
    }
    #endregion

    #region + [8] 사용자별 개수 조회: GetCountByUserAsync
    // 26년 추가 메서드 (BlazorServer에서 사용)
    public async Task<int> GetCountByUserAsync(long uid)
    {
        const string query = @"
            SELECT COUNT(*)
            FROM TasksDb T
            INNER JOIN ReportDb R ON T.TaksListNumber = R.Task_Number
            WHERE R.Uid = @Uid";

        using (var connection = new SqlConnection(dbContext))
        {
            return await connection.ExecuteScalarAsync<int>(query, new { Uid = uid }, commandType: CommandType.Text);
        }
    }
    #endregion

    #region + [9] 사용자별 전체 삭제: DeleteAllByUserAsync
    // 26년 추가 메서드 (BlazorServer에서 사용)
    public async Task<bool> DeleteAllByUserAsync(long uid)
    {
        const string query = @"
            DELETE FROM TasksDb
            WHERE TaksListNumber IN (
                SELECT Task_Number FROM ReportDb WHERE Uid = @Uid
            )";

        try
        {
            using (var connection = new SqlConnection(dbContext))
            {
                var result = await connection.ExecuteAsync(query, new { Uid = uid }, commandType: CommandType.Text);
                return result > 0;
            }
        }
        catch (Exception er)
        {
            _logger?.LogError($"ERROR({nameof(DeleteAllByUserAsync)}): {er.Message}");
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
