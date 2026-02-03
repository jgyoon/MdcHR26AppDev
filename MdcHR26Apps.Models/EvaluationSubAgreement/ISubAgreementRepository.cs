namespace MdcHR26Apps.Models.EvaluationSubAgreement;

public interface ISubAgreementRepository : IDisposable
{
    Task<SubAgreementDb> AddAsync(SubAgreementDb model);
    Task<List<SubAgreementDb>> GetByAllAsync();
    Task<SubAgreementDb> GetByIdAsync(long id);
    Task<bool> UpdateAsync(SubAgreementDb model);
    Task<bool> DeleteAsync(long id);
    Task<List<SubAgreementDb>> GetByUserIdAllAsync(long userId);
    Task<List<SubAgreementDb>> GetByTasksPeroportionAsync(long userId, string deptName, string indexName);
}
