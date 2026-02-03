namespace MdcHR26Apps.Models.EvaluationAgreement;

public interface IAgreementRepository : IDisposable
{
    Task<AgreementDb> AddAsync(AgreementDb model);
    Task<List<AgreementDb>> GetByAllAsync();
    Task<AgreementDb> GetByIdAsync(long id);
    Task<bool> UpdateAsync(AgreementDb model);
    Task<bool> DeleteAsync(long id);
    Task<List<AgreementDb>> GetByUserIdAllAsync(long userId);
    Task<List<AgreementDb>> GetByTasksPeroportionAsync(long userId, string deptName, string indexName);
}
