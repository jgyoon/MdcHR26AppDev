namespace MdcHR26Apps.Models.EvaluationTasks;

public interface ITasksRepository : IDisposable
{
    Task<TasksDb> AddAsync(TasksDb model);
    Task<List<TasksDb>> GetByAllAsync();
    Task<TasksDb> GetByIdAsync(long id);
    Task<bool> UpdateAsync(TasksDb model);
    Task<bool> DeleteAsync(long id);
    Task<List<TasksDb>> GetByListNoAllAsync(long taksListNumber);
    Task<bool> DeleteByListNoAllAsync(long taksListNumber);
    Task<int> GetCountByUserAsync(long uid);
    Task<bool> DeleteAllByUserAsync(long uid);
}
