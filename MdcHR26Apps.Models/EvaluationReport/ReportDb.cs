using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.EvaluationReport;

/// <summary>
/// 평가 보고서 Entity (개별 평가 항목)
/// </summary>
[Table("ReportDb")]
public class ReportDb
{
    /// <summary>
    /// Report ID (PK, IDENTITY)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 Rid { get; set; }

    /// <summary>
    /// 평가 대상자 ID (FK → UserDb.Uid)
    /// </summary>
    [Required]
    public Int64 Uid { get; set; }

    /// <summary>
    /// 보고서 항목 번호
    /// </summary>
    [Required]
    public int Report_Item_Number { get; set; }

    /// <summary>
    /// 지표 분류명
    /// </summary>
    [Required]
    public string Report_Item_Name_1 { get; set; } = string.Empty;

    /// <summary>
    /// 직무 분류명
    /// </summary>
    [Required]
    public string Report_Item_Name_2 { get; set; } = string.Empty;

    /// <summary>
    /// 직무 비율(%)
    /// </summary>
    [Required]
    public int Report_Item_Proportion { get; set; }

    /// <summary>
    /// 세부 직무명
    /// </summary>
    [Required]
    public string Report_SubItem_Name { get; set; } = string.Empty;

    /// <summary>
    /// 세부 직무 비율(%)
    /// </summary>
    [Required]
    public int Report_SubItem_Proportion { get; set; }

    /// <summary>
    /// 하위 업무 리스트 번호 (TasksDb 연결)
    /// </summary>
    [Required]
    public Int64 Task_Number { get; set; }

    // === 평가대상자 평가 ===

    /// <summary>
    /// 사용자 평가 - 일정준수
    /// </summary>
    [Required]
    public double User_Evaluation_1 { get; set; }

    /// <summary>
    /// 사용자 평가 - 업무 수행의 양
    /// </summary>
    [Required]
    public double User_Evaluation_2 { get; set; }

    /// <summary>
    /// 사용자 평가 - 결과물
    /// </summary>
    [Required]
    public double User_Evaluation_3 { get; set; }

    /// <summary>
    /// 사용자 평가 - 코멘트
    /// </summary>
    public string? User_Evaluation_4 { get; set; }

    // === 부서장/팀장 평가 ===

    /// <summary>
    /// 팀장 평가 - 일정준수
    /// </summary>
    [Required]
    public double TeamLeader_Evaluation_1 { get; set; }

    /// <summary>
    /// 팀장 평가 - 업무 수행의 양
    /// </summary>
    [Required]
    public double TeamLeader_Evaluation_2 { get; set; }

    /// <summary>
    /// 팀장 평가 - 결과물
    /// </summary>
    [Required]
    public double TeamLeader_Evaluation_3 { get; set; }

    /// <summary>
    /// 팀장 평가 - 코멘트
    /// </summary>
    public string? TeamLeader_Evaluation_4 { get; set; }

    // === 임원 평가 ===

    /// <summary>
    /// 임원 평가 - 일정준수
    /// </summary>
    [Required]
    public double Director_Evaluation_1 { get; set; }

    /// <summary>
    /// 임원 평가 - 업무 수행의 양
    /// </summary>
    [Required]
    public double Director_Evaluation_2 { get; set; }

    /// <summary>
    /// 임원 평가 - 결과물
    /// </summary>
    [Required]
    public double Director_Evaluation_3 { get; set; }

    /// <summary>
    /// 임원 평가 - 코멘트
    /// </summary>
    public string? Director_Evaluation_4 { get; set; }

    /// <summary>
    /// 종합 점수
    /// </summary>
    [Required]
    public double Total_Score { get; set; }

    // Navigation Property
    [ForeignKey("Uid")]
    public User.UserDb? User { get; set; }
}
