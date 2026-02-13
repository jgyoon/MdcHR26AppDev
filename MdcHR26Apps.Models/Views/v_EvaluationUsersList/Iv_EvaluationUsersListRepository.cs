namespace MdcHR26Apps.Models.Views.v_EvaluationUsersList;

public interface Iv_EvaluationUsersListRepository : IDisposable
{
    Task<IEnumerable<v_EvaluationUsersList>> GetByAllAsync();
    Task<v_EvaluationUsersList?> GetByUidAsync(Int64 uid);
    Task<IEnumerable<v_EvaluationUsersList>> SearchByNameAsync(string userName);
}
