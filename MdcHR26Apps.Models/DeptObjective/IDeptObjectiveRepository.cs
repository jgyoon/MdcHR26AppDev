namespace MdcHR26Apps.Models.DeptObjective;

public interface IDeptObjectiveRepository : IDisposable
{
    Task<DeptObjectiveDb> AddAsync(DeptObjectiveDb model);
    Task<List<DeptObjectiveDb>> GetByAllAsync();
    Task<DeptObjectiveDb> GetByIdAsync(long id);
    Task<bool> UpdateAsync(DeptObjectiveDb model);
    Task<bool> DeleteAsync(long id);
}
