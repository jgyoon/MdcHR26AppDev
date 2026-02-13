using Dapper;
using MdcHR26Apps.Models.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;

namespace MdcHR26Apps.Models.EvaluationLists;

public class EvaluationListsRepository(string connectionString, ILoggerFactory loggerFactory) : IEvaluationListsRepository, IDisposable
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger(nameof(EvaluationListsRepository));
    private readonly string dbContext = connectionString;

    #region + [1] 입력: AddAsync
    public async Task<EvaluationLists> AddAsync(EvaluationLists model)
    {
        try
        {
            const string query =
                "INSERT INTO EvaluationLists(" +
                    "Evaluation_Department_Number, Evaluation_Department_Name, Evaluation_Index_Number, " +
                    "Evaluation_Index_Name, Evaluation_Task_Number, Evaluation_Task_Name, Evaluation_Lists_Remark) " +
                "VALUES(" +
                    "@Evaluation_Department_Number, @Evaluation_Department_Name, @Evaluation_Index_Number, " +
                    "@Evaluation_Index_Name, @Evaluation_Task_Number, @Evaluation_Task_Name, @Evaluation_Lists_Remark);" +
                "Select Cast(SCOPE_IDENTITY() As Int);";

            using (var connection = new SqlConnection(dbContext))
            {
                int id = await connection.ExecuteScalarAsync<int>(query, model);
                model.Eid = id;
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
    public async Task<List<EvaluationLists>> GetByAllAsync()
    {
        const string query = "Select * From EvaluationLists Order By Eid";

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.QueryAsync<EvaluationLists>(query, commandType: CommandType.Text);
            return result.ToList();
        }
    }
    #endregion

    #region + [3] 상세: GetByIdAsync
    public async Task<EvaluationLists> GetByIdAsync(long id)
    {
        const string query = "Select * From EvaluationLists Where Eid = @id";

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.QueryFirstOrDefaultAsync<EvaluationLists>(query, new { id }, commandType: CommandType.Text);
            return result ?? new EvaluationLists();
        }
    }
    #endregion

    #region + [4] 수정: UpdateAsync
    public async Task<bool> UpdateAsync(EvaluationLists model)
    {
        const string query = @"
            Update EvaluationLists
            Set
                Evaluation_Department_Number = @Evaluation_Department_Number,
                Evaluation_Department_Name = @Evaluation_Department_Name,
                Evaluation_Index_Number = @Evaluation_Index_Number,
                Evaluation_Index_Name = @Evaluation_Index_Name,
                Evaluation_Task_Number = @Evaluation_Task_Number,
                Evaluation_Task_Name = @Evaluation_Task_Name,
                Evaluation_Lists_Remark = @Evaluation_Lists_Remark
            Where Eid = @Eid";
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
        const string query = "Delete EvaluationLists Where Eid = @id";

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

    #region + [6] 지표구분 출력: GetByDeptAllAsync
    public async Task<List<SelectListModel>> GetByDeptAllAsync()
    {
        const string query = @"
            Select DISTINCT Evaluation_Department_Number, Evaluation_Department_Name
            From EvaluationLists
            Order By Evaluation_Department_Number";

        List<SelectListModel> resultList = new();

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.QueryAsync<EvaluationLists>(query, commandType: CommandType.Text);

            foreach (var item in result)
            {
                resultList.Add(new SelectListModel
                {
                    Value = item.Evaluation_Department_Number.ToString(),
                    Text = !string.IsNullOrEmpty(item.Evaluation_Department_Name) ?
                        item.Evaluation_Department_Name : string.Empty
                });
            }

            return resultList;
        }
    }
    #endregion

    #region + [7] 지표구분번호 출력: GetByDeptNumberAsync
    public async Task<int> GetByDeptNumberAsync(string deptName)
    {
        const string query = "Select DISTINCT Evaluation_Department_Number From EvaluationLists Where Evaluation_Department_Name = @deptName";

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.ExecuteScalarAsync<int>(query, new { deptName }, commandType: CommandType.Text);
            return result;
        }
    }
    #endregion

    #region + [8] 직무구분 출력: GetByIndexAllAsync
    public async Task<List<SelectListModel>> GetByIndexAllAsync(int deptNo)
    {
        const string query = @"
            Select DISTINCT Evaluation_Index_Number, Evaluation_Index_Name
            From EvaluationLists
            Where Evaluation_Department_Number = @deptNo
            Order By Evaluation_Index_Number";

        List<SelectListModel> resultList = new();

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.QueryAsync<EvaluationLists>(query, new { deptNo }, commandType: CommandType.Text);

            foreach (var item in result)
            {
                resultList.Add(new SelectListModel
                {
                    Value = item.Evaluation_Index_Number.ToString(),
                    Text = !string.IsNullOrEmpty(item.Evaluation_Index_Name) ?
                        item.Evaluation_Index_Name : string.Empty
                });
            }

            return resultList;
        }
    }
    #endregion

    #region + [9] 평가지표 출력: GetByTasksAsync
    public async Task<List<SelectListModel>> GetByTasksAsync(string deptName, string indexName)
    {
        const string query = @"
            Select Evaluation_Task_Number, Evaluation_Task_Name
            From EvaluationLists
            Where Evaluation_Department_Name = @deptName And Evaluation_Index_Name = @indexName
            Order By Evaluation_Task_Number";

        List<SelectListModel> resultList = new();

        using (var connection = new SqlConnection(dbContext))
        {
            var result = await connection.QueryAsync<EvaluationLists>(query, new { deptName, indexName }, commandType: CommandType.Text);

            foreach (var item in result)
            {
                resultList.Add(new SelectListModel
                {
                    Value = item.Evaluation_Task_Number.ToString(),
                    Text = !string.IsNullOrEmpty(item.Evaluation_Task_Name) ?
                        item.Evaluation_Task_Name : string.Empty
                });
            }

            return resultList;
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
