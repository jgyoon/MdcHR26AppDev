namespace MdcHR26Apps.BlazorServer.Models;

/// <summary>
/// 종합 점수 등급 모델
/// </summary>
public class TotalScoreRankModel
{
    /// <summary>
    /// 등급 기준 점수 (100, 90, 80, 70, 60)
    /// </summary>
    public double TotalScoreNumber { get; set; }

    /// <summary>
    /// 등급명 (S, A, B, C, D)
    /// </summary>
    public string TotalScoreRankName { get; set; } = string.Empty;
}
