# 작업지시서: Entity 필드명 DB 테이블 기준 수정 (5개)

**날짜**: 2026-02-03
**작업 타입**: Entity 수정
**예상 소요**: 1시간
**관련 이슈**: [#009](../issues/009_phase3_webapp_development.md)

---

## 1. 작업 개요

### 배경
- DB 테이블과 Entity 간 필드명 불일치 발견 (5개 Entity)
- Entity의 **필드명만** DB 테이블에 맞춰 수정
- Navigation Property 및 FK 구조는 유지

### 목표
DB 테이블 구조에 맞게 Entity 필드명 수정

---

## 2. 작업 내용

### 2.1. AgreementDb.cs

#### 파일: `MdcHR26Apps.Models/EvaluationAgreement/AgreementDb.cs`

#### DB 테이블 (Agreement.sql) - 기준
```sql
[Aid] BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
[Uid] BIGINT NOT NULL,
[Report_Item_Number] INT NOT NULL,
[Report_Item_Name_1] NVARCHAR(MAX) NOT NULL,
[Report_Item_Name_2] NVARCHAR(MAX) NOT NULL,
[Report_Item_Proportion] INT NOT NULL
```

#### 현재 Entity (잘못됨)
```csharp
public Int64 Aid { get; set; }
public Int64 Uid { get; set; }
public int Agreement_Item_Number { get; set; }  // ❌
public string Agreement_Item_Name_1 { get; set; }  // ❌
public string Agreement_Item_Name_2 { get; set; }  // ❌
public int Agreement_Item_Proportion { get; set; }  // ❌
public Int64? DeptObjective_Number { get; set; }  // ❌ DB에 없음
public bool Is_Agreement { get; set; }  // ❌ DB에 없음
public string? Agreement_Comment { get; set; }  // ❌ DB에 없음
public DateTime? Agreement_Date { get; set; }  // ❌ DB에 없음

[ForeignKey("Uid")]
public User.UserDb? User { get; set; }  // ✅ 유지
```

#### 수정 후 Entity (DB 테이블 기준)
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.EvaluationAgreement;

[Table("AgreementDb")]
public class AgreementDb
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 Aid { get; set; }

    [Required]
    public Int64 Uid { get; set; }

    [Required]
    public int Report_Item_Number { get; set; }

    [Required]
    public string Report_Item_Name_1 { get; set; } = string.Empty;

    [Required]
    public string Report_Item_Name_2 { get; set; } = string.Empty;

    [Required]
    public int Report_Item_Proportion { get; set; }

    // Navigation Property
    [ForeignKey("Uid")]
    public User.UserDb? User { get; set; }
}
```

---

### 2.2. SubAgreementDb.cs

#### 파일: `MdcHR26Apps.Models/EvaluationSubAgreement/SubAgreementDb.cs`

#### DB 테이블 (SubAgreement.sql) - 기준
```sql
[Sid] BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
[Uid] BIGINT NOT NULL,
[Report_Item_Number] INT NOT NULL,
[Report_Item_Name_1] NVARCHAR(MAX) NOT NULL,
[Report_Item_Name_2] NVARCHAR(MAX) NOT NULL,
[Report_Item_Proportion] INT NOT NULL,
[Report_SubItem_Name] NVARCHAR(MAX) NOT NULL,
[Report_SubItem_Proportion] INT NOT NULL,
[Task_Number] BIGINT NOT NULL
```

#### 수정 후 Entity
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.EvaluationSubAgreement;

[Table("SubAgreementDb")]
public class SubAgreementDb
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 Sid { get; set; }

    [Required]
    public Int64 Uid { get; set; }

    [Required]
    public int Report_Item_Number { get; set; }

    [Required]
    public string Report_Item_Name_1 { get; set; } = string.Empty;

    [Required]
    public string Report_Item_Name_2 { get; set; } = string.Empty;

    [Required]
    public int Report_Item_Proportion { get; set; }

    [Required]
    public string Report_SubItem_Name { get; set; } = string.Empty;

    [Required]
    public int Report_SubItem_Proportion { get; set; }

    [Required]
    public Int64 Task_Number { get; set; }

    // Navigation Property
    [ForeignKey("Uid")]
    public User.UserDb? User { get; set; }
}
```

---

### 2.3. DeptObjectiveDb.cs

#### 파일: `MdcHR26Apps.Models/DeptObjective/DeptObjectiveDb.cs`

#### DB 테이블 (DeptObjectiveDb.sql) - 기준
```sql
[DeptObjectiveDbId] BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
[EDepartId] BIGINT NOT NULL,
[ObjectiveTitle] NVARCHAR(MAX) NOT NULL,
[ObjectiveContents] NVARCHAR(MAX) NOT NULL,
[CreatedBy] BIGINT NOT NULL,
[CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
[UpdatedBy] BIGINT NULL,
[UpdatedAt] DATETIME NULL,
[Remarks] NVARCHAR(MAX)
```

#### 수정 후 Entity
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.DeptObjective;

[Table("DeptObjectiveDb")]
public class DeptObjectiveDb
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 DeptObjectiveDbId { get; set; }

    [Required]
    public Int64 EDepartId { get; set; }

    [Required]
    public string ObjectiveTitle { get; set; } = string.Empty;

    [Required]
    public string ObjectiveContents { get; set; } = string.Empty;

    [Required]
    public Int64 CreatedBy { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public Int64? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Remarks { get; set; }

    // Navigation Properties
    [ForeignKey("EDepartId")]
    public Department.EDepartmentDb? EDepartment { get; set; }

    [ForeignKey("CreatedBy")]
    public User.UserDb? Creator { get; set; }

    [ForeignKey("UpdatedBy")]
    public User.UserDb? Updater { get; set; }
}
```

---

### 2.4. EvaluationLists.cs

#### 파일: `MdcHR26Apps.Models/EvaluationLists/EvaluationLists.cs`

#### DB 테이블 (EvaluationLists.sql) - 기준
```sql
[Eid] BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
[Evaluation_Department_Number] INT NOT NULL,
[Evaluation_Department_Name] NVARCHAR(20) NOT NULL,
[Evaluation_Index_Number] INT NOT NULL,
[Evaluation_Index_Name] NVARCHAR(100) NOT NULL,
[Evaluation_Task_Number] INT NOT NULL,
[Evaluation_Task_Name] NVARCHAR(100) NOT NULL,
[Evaluation_Lists_Remark] NVARCHAR(100) NULL
```

#### 수정 후 Entity
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.EvaluationLists;

[Table("EvaluationLists")]
public class EvaluationLists
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 Eid { get; set; }

    [Required]
    public int Evaluation_Department_Number { get; set; }

    [Required]
    [StringLength(20)]
    public string Evaluation_Department_Name { get; set; } = string.Empty;

    [Required]
    public int Evaluation_Index_Number { get; set; }

    [Required]
    [StringLength(100)]
    public string Evaluation_Index_Name { get; set; } = string.Empty;

    [Required]
    public int Evaluation_Task_Number { get; set; }

    [Required]
    [StringLength(100)]
    public string Evaluation_Task_Name { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Evaluation_Lists_Remark { get; set; }
}
```

---

### 2.5. TasksDb.cs

#### 파일: `MdcHR26Apps.Models/EvaluationTasks/TasksDb.cs`

#### DB 테이블 (TasksDb.sql) - 기준
```sql
[Tid] BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
[TaskName] NVARCHAR(100) NOT NULL,
[TaksListNumber] BIGINT NOT NULL,
[TaskStatus] INT NOT NULL,
[TaskObjective] NVARCHAR(MAX) NOT NULL,
[TargetProportion] INT NOT NULL,
[ResultProportion] INT NOT NULL,
[TargetDate] DATETIME NOT NULL,
[ResultDate] DATETIME NOT NULL,
[Task_Evaluation_1] FLOAT NOT NULL,
[Task_Evaluation_2] FLOAT NOT NULL,
[TaskLevel] FLOAT NOT NULL,
[TaskComments] NVARCHAR(50) NOT NULL
```

#### 수정 후 Entity
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.EvaluationTasks;

[Table("TasksDb")]
public class TasksDb
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 Tid { get; set; }

    [Required]
    [StringLength(100)]
    public string TaskName { get; set; } = string.Empty;

    [Required]
    public Int64 TaksListNumber { get; set; }

    [Required]
    public int TaskStatus { get; set; }

    [Required]
    public string TaskObjective { get; set; } = string.Empty;

    [Required]
    public int TargetProportion { get; set; }

    [Required]
    public int ResultProportion { get; set; }

    [Required]
    public DateTime TargetDate { get; set; }

    [Required]
    public DateTime ResultDate { get; set; }

    [Required]
    public double Task_Evaluation_1 { get; set; }

    [Required]
    public double Task_Evaluation_2 { get; set; }

    [Required]
    public double TaskLevel { get; set; }

    [Required]
    [StringLength(50)]
    public string TaskComments { get; set; } = string.Empty;
}
```

---

## 3. 테스트 시나리오

### 3.1. 빌드 검증
```bash
dotnet build MdcHR26Apps.Models
```
- ✅ 컴파일 오류 없음

---

## 4. 완료 조건

- [x] AgreementDb.cs 수정 완료
- [x] SubAgreementDb.cs 수정 완료
- [x] DeptObjectiveDb.cs 수정 완료
- [x] EvaluationLists.cs 수정 완료
- [x] TasksDb.cs 수정 완료
- [x] 빌드 성공 (MdcHR26Apps.Models)
- [x] 관련 이슈 업데이트

---

## 5. 관련 문서

**이슈**:
- [#009: Phase 3 WebApp 개발](../issues/009_phase3_webapp_development.md)

**DB 테이블 (기준)**:
- `Database/dbo/Agreement.sql`
- `Database/dbo/SubAgreement.sql`
- `Database/dbo/DeptObjectiveDb.sql`
- `Database/dbo/EvaluationLists.sql`
- `Database/dbo/TasksDb.sql`

**Entity 파일 (수정 대상)**:
- `MdcHR26Apps.Models/EvaluationAgreement/AgreementDb.cs`
- `MdcHR26Apps.Models/EvaluationSubAgreement/SubAgreementDb.cs`
- `MdcHR26Apps.Models/DeptObjective/DeptObjectiveDb.cs`
- `MdcHR26Apps.Models/EvaluationLists/EvaluationLists.cs`
- `MdcHR26Apps.Models/EvaluationTasks/TasksDb.cs`
