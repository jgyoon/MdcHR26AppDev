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
    /// 특정 프로세스의 전체 결과 조회
    /// </summary>
    Task<IEnumerable<v_ProcessTRListDB>> GetByProcessIdAsync(Int64 processId);

    /// <summary>
    /// 특정 사용자의 평가 결과 조회
    /// </summary>
    Task<IEnumerable<v_ProcessTRListDB>> GetByUserIdAsync(Int64 uid);

    /// <summary>
    /// 연도별 평가 결과 조회
    /// </summary>
    Task<IEnumerable<v_ProcessTRListDB>> GetByYearAsync(int year);

    /// <summary>
    /// 등급별 평가 결과 조회
    /// </summary>
    Task<IEnumerable<v_ProcessTRListDB>> GetByGradeAsync(string grade);

    /// <summary>
    /// 프로세스 상태별 조회
    /// </summary>
    Task<IEnumerable<v_ProcessTRListDB>> GetByProcessStatusAsync(int status);

    /// <summary>
    /// 특정 프로세스의 특정 사용자 결과 조회
    /// </summary>
    Task<v_ProcessTRListDB?> GetByProcessAndUserAsync(Int64 processId, Int64 uid);
}
