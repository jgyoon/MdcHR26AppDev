using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MdcHR26Apps.Models.Department;
using MdcHR26Apps.Models.Rank;

namespace MdcHR26Apps.Models.User;

/// <summary>
/// 사용자/직원 정보 Entity
/// </summary>
[Table("UserDb")]
public class UserDb
{
    /// <summary>
    /// 사용자 ID (PK, IDENTITY)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 Uid { get; set; }

    /// <summary>
    /// 로그인 ID
    /// </summary>
    [Required]
    [StringLength(50)]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 사용자 이름
    /// </summary>
    [Required]
    [StringLength(20)]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 비밀번호 해시 (SHA-256, VARBINARY(32))
    /// </summary>
    [Required]
    [MaxLength(32)]
    public byte[] UserPassword { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// 비밀번호 Salt (VARBINARY(16))
    /// </summary>
    [Required]
    [MaxLength(16)]
    public byte[] UserPasswordSalt { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// 비밀번호 (UI 바인딩용, DB에 저장 안 됨)
    /// Repository에서 해싱 후 UserPassword로 변환
    /// </summary>
    [NotMapped]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 사원번호
    /// </summary>
    [StringLength(10)]
    public string? ENumber { get; set; }

    /// <summary>
    /// 이메일
    /// </summary>
    [StringLength(50)]
    public string? Email { get; set; }

    /// <summary>
    /// 부서 ID (FK → EDepartmentDb.EDepartId)
    /// </summary>
    public long? EDepartId { get; set; }

    /// <summary>
    /// 직급 ID (FK → ERankDb.ERankId)
    /// </summary>
    public long? ERankId { get; set; }

    /// <summary>
    /// 재직 상태 (BIT: 1=재직, 0=퇴직)
    /// </summary>
    [Required]
    public bool EStatus { get; set; } = true;

    /// <summary>
    /// 팀장 여부
    /// </summary>
    [Required]
    public bool IsTeamLeader { get; set; } = false;

    /// <summary>
    /// 임원 여부
    /// </summary>
    [Required]
    public bool IsDirector { get; set; } = false;

    /// <summary>
    /// 관리자 여부
    /// </summary>
    [Required]
    public bool IsAdministrator { get; set; } = false;

    /// <summary>
    /// 부서 목표 작성 권한
    /// </summary>
    [Required]
    public bool IsDeptObjectiveWriter { get; set; } = false;

    // Navigation Properties
    [ForeignKey("EDepartId")]
    public EDepartmentDb? EDepartment { get; set; }

    [ForeignKey("ERankId")]
    public ERankDb? ERank { get; set; }
}
