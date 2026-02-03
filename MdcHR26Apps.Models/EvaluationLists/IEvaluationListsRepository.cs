using MdcHR26Apps.Models.Common;

namespace MdcHR26Apps.Models.EvaluationLists;

public interface IEvaluationListsRepository : IDisposable
{
    Task<EvaluationLists> AddAsync(EvaluationLists model);
    Task<List<EvaluationLists>> GetByAllAsync();
    Task<EvaluationLists> GetByIdAsync(long id);
    Task<bool> UpdateAsync(EvaluationLists model);
    Task<bool> DeleteAsync(long id);
    Task<List<SelectListModel>> GetByDeptAllAsync();
    Task<int> GetByDeptNumberAsync(string deptName);
    Task<List<SelectListModel>> GetByIndexAllAsync(int deptNo);
    Task<List<SelectListModel>> GetByTasksAsync(string deptName, string indexName);
}
