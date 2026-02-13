namespace MdcHR26Apps.Models.EvaluationSubAgreement;

public interface ISubAgreementRepository : IDisposable
{
    Task<SubAgreementDb> AddAsync(SubAgreementDb model);
    Task<List<SubAgreementDb>> GetByAllAsync();
    Task<SubAgreementDb> GetByIdAsync(long id);
    Task<bool> UpdateAsync(SubAgreementDb model);
    Task<bool> DeleteAsync(long id);
    Task<List<SubAgreementDb>> GetByUidAllAsync(long uid);
    Task<List<SubAgreementDb>> GetByTasksPeroportionAsync(long uid, string deptName, string indexName);
    Task<SubAgreementDb> GetByUidAndItemNamesAllAsync(long uid, string item1, string item2);
}
