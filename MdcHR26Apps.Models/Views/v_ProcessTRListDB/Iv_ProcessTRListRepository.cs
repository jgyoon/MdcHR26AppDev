namespace MdcHR26Apps.Models.Views.v_ProcessTRListDB;

/// <summary>
/// v_ProcessTRListDB Repository Interface (읽기 전용)
/// </summary>
public interface Iv_ProcessTRListRepository : IDisposable
{
    /// <summary>
    /// 전체 평가 프로세스-결과 목록 조회
    /// </summary>
    Task<IEnumerable<v_ProcessTRListDB>> GetByAllAsync();

    /// <summary>
    /// 전체 평가 프로세스-결과 목록 조회 (List 반환)
    /// </summary>
    Task<List<v_ProcessTRListDB>> GetAllAsync();

    /// <summary>
    /// Pid로 특정 평가 프로세스-결과 조회
    /// </summary>
    Task<v_ProcessTRListDB?> GetByPidAsync(long pid);

    /// <summary>
    /// 특정 프로세스의 전체 결과 조회
    /// </summary>
    Task<IEnumerable<v_ProcessTRListDB>> GetByProcessIdAsync(Int64 processId);

    /// <summary>
    /// 특정 사용자의 평가 결과 조회
    /// </summary>
    Task<IEnumerable<v_ProcessTRListDB>> GetByUserIdAsync(Int64 uid);

    /// <summary>
    /// 특정 프로세스의 특정 사용자 결과 조회
    /// </summary>
    Task<v_ProcessTRListDB?> GetByProcessAndUserAsync(Int64 processId, Int64 uid);
}
