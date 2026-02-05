using MdcHR26Apps.Models.Department;
using MdcHR26Apps.Models.Rank;

namespace MdcHR26Apps.BlazorServer.Utils;

/// <summary>
/// Primary Constructor 사용 (C# 13)
/// 사용자 관리 유틸리티
/// 2026년: DB에서 활성화된(ActivateStatus=true) 부서/직급 조회
/// </summary>
public class UserUtils(
    IEDepartmentRepository departmentRepository,
    IERankRepository rankRepository)
{
    private readonly IEDepartmentRepository _departmentRepository = departmentRepository;
    private readonly IERankRepository _rankRepository = rankRepository;

    /// <summary>
    /// 부서 목록 가져오기 (EDepartmentName 리스트)
    /// 2025년: 하드코딩 → 2026년: DB에서 활성화된 부서만 조회
    /// </summary>
    public async Task<List<string>> GetDeptListAsync()
    {
        var depts = await _departmentRepository.GetActiveAsync();
        return depts
            .OrderBy(d => d.EDepartmentNo)
            .Select(d => d.EDepartmentName)
            .ToList();
    }

    /// <summary>
    /// 직급 목록 가져오기 (ERankName 리스트)
    /// 2025년: 하드코딩 → 2026년: DB에서 활성화된 직급만 조회
    /// </summary>
    public async Task<List<string>> GetRankListAsync()
    {
        var ranks = await _rankRepository.GetActiveAsync();
        return ranks
            .OrderBy(r => r.ERankNo)
            .Select(r => r.ERankName)
            .ToList();
    }

    /// <summary>
    /// 부서명으로 부서 정보 조회
    /// </summary>
    public async Task<EDepartmentDb?> GetDepartmentByNameAsync(string name)
    {
        var depts = await _departmentRepository.GetActiveAsync();
        return depts.FirstOrDefault(d => d.EDepartmentName == name);
    }

    /// <summary>
    /// 직급명으로 직급 정보 조회
    /// </summary>
    public async Task<ERankDb?> GetRankByNameAsync(string name)
    {
        var ranks = await _rankRepository.GetActiveAsync();
        return ranks.FirstOrDefault(r => r.ERankName == name);
    }

    /// <summary>
    /// 활성화 상태 텍스트 변환
    /// </summary>
    public string isuse(bool isActive)
    {
        return isActive ? "사용중" : "미사용";
    }

    /// <summary>
    /// 비고 처리 (Null 대응)
    /// </summary>
    public string GetRemarks(string? remarks)
    {
        return !string.IsNullOrEmpty(remarks) ? remarks : string.Empty;
    }

    /// <summary>
    /// 무제한 Task 허용 부서 확인
    /// 특정 부서("인증본부")는 Task 제한 없음
    /// </summary>
    public bool GetIsUnLimitedTask(string? loginUserEDepartment)
    {
        return !string.IsNullOrEmpty(loginUserEDepartment) && loginUserEDepartment == "인증본부";
    }
}
