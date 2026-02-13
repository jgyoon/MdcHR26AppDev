# 작업지시서: DB 테이블 기준 Entity 수정 (5개)

**날짜**: 2026-02-03
**작업 타입**: Entity 재작성
**예상 소요**: 2시간
**관련 이슈**: [#009](../issues/009_phase3_webapp_development.md)

---

## 1. 작업 개요

### 배경
- Phase 3-4 컴포넌트 작업지시서 작성 중 DB 테이블과 Entity 간 불일치 발견
- 5개 테이블(AgreementDb, SubAgreementDb, DeptObjectiveDb, EvaluationLists, TasksDb)의 Entity가 DB 테이블 구조와 다름
- 이전 이슈 #012, #013과 동일하게 **DB 테이블을 기준으로 Entity 수정** 필요

### 목표
DB 테이블 구조에 맞게 5개 Entity 파일을 정확히 수정

---

## 2. 작업 내용

### 2.1. AgreementDb Entity 수정

#### 파일: `MdcHR26Apps.Models/EvaluationAgreement/AgreementDb.cs`

#### 현재 Entity (잘못됨)
```csharp
public class AgreementDb
{
    [Key]
    public Int64 Aid { get; set; }

    [Required]
    public Int64 Uid { get; set; }

    [Required]
    public int Agreement_Item_Number { get; set; }

    [Required]
    [StringLength(255)]
    public string Agreement_Item_Name_1 { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string Agreement_Item_Name_2 { get; set; } = string.Empty;

    [Required]
    public int Agreement_Item_Proportion { get; set; }

    public Int64? DeptObjective_Number { get; set; }

    [Required]
    public bool Is_Agreement { get; set; } = false;

    public string? Agreement_Comment { get; set; }

    public DateTime? Agreement_Date { get; set; }

    // Navigation Properties...
}
```

#### DB 테이블 (Agreement.sql) - 정답
```sql
[Aid] BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
[Uid] BIGINT NOT NULL,
[Report_Item_Number] INT NOT NULL,
[Report_Item_Name_1] NVARCHAR(MAX) NOT NULL,
[Report_Item_Name_2] NVARCHAR(MAX) NOT NULL,
[Report_Item_Proportion] INT NOT NULL,

FOREIGN KEY (Uid) REFERENCES [dbo].[UserDb](Uid)
```

#### 수정 후 Entity (DB 테이블 기준)
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.EvaluationAgreement;

/// <summary>
/// 직무평가 협의서 Entity (대분류)
/// </summary>
[Table("AgreementDb")]
public class AgreementDb
{
    /// <summary>
    /// Agreement ID (PK, IDENTITY)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 Aid { get; set; }

    /// <summary>
    /// 평가 대상자 ID (FK → UserDb.Uid)
    /// </summary>
    [Required]
    public Int64 Uid { get; set; }

    /// <summary>
    /// Report_Item_Number
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

    // Navigation Properties
    [ForeignKey("Uid")]
    public User.UserDb? User { get; set; }
}
```

---

### 2.2. SubAgreementDb Entity 수정

#### 파일: `MdcHR26Apps.Models/EvaluationSubAgreement/SubAgreementDb.cs`

#### 현재 Entity (잘못됨)
```csharp
public class SubAgreementDb
{
    [Key]
    public Int64 SAid { get; set; }

    [Required]
    public Int64 Uid { get; set; }

    [Required]
    public Int64 Agreement_Number { get; set; }

    [Required]
    [StringLength(255)]
    public string SubAgreement_Item_Name { get; set; } = string.Empty;

    [Required]
    public int SubAgreement_Item_Proportion { get; set; }

    [Required]
    public bool Is_SubAgreement { get; set; } = false;

    public string? SubAgreement_Comment { get; set; }

    public DateTime? SubAgreement_Date { get; set; }

    // Navigation Properties...
}
```

#### DB 테이블 (SubAgreement.sql) - 정답
```sql
[Sid] BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
[Uid] BIGINT NOT NULL,
[Report_Item_Number] INT NOT NULL,
[Report_Item_Name_1] NVARCHAR(MAX) NOT NULL,
[Report_Item_Name_2] NVARCHAR(MAX) NOT NULL,
[Report_Item_Proportion] INT NOT NULL,
[Report_SubItem_Name] NVARCHAR(MAX) NOT NULL,
[Report_SubItem_Proportion] INT NOT NULL,
[Task_Number] BIGINT NOT NULL,

FOREIGN KEY (Uid) REFERENCES [dbo].[UserDb](Uid)
```

#### 수정 후 Entity (DB 테이블 기준)
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.EvaluationSubAgreement;

/// <summary>
/// 상세 직무평가 협의서 Entity (세부 항목)
/// </summary>
[Table("SubAgreementDb")]
public class SubAgreementDb
{
    /// <summary>
    /// SubAgreement ID (PK, IDENTITY)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 Sid { get; set; }

    /// <summary>
    /// 평가 대상자 ID (FK → UserDb.Uid)
    /// </summary>
    [Required]
    public Int64 Uid { get; set; }

    /// <summary>
    /// Report_Item_Number
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
    /// 하위 업무 리스트 번호
    /// </summary>
    [Required]
    public Int64 Task_Number { get; set; }

    // Navigation Properties
    [ForeignKey("Uid")]
    public User.UserDb? User { get; set; }
}
```

---

### 2.3. DeptObjectiveDb Entity 수정

#### 파일: `MdcHR26Apps.Models/DeptObjective/DeptObjectiveDb.cs`

#### 현재 Entity (잘못됨)
```csharp
public class DeptObjectiveDb
{
    [Key]
    public Int64 DOid { get; set; }

    [Required]
    public Int64 EDepartId { get; set; }

    [Required]
    [StringLength(255)]
    public string Objective_Title { get; set; } = string.Empty;

    public string? Objective_Content { get; set; }

    public string? Achievement_Criteria { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;

    [ForeignKey("EDepartId")]
    public Department.EDepartmentDb? EDepartment { get; set; }
}
```

#### DB 테이블 (DeptObjectiveDb.sql) - 정답
```sql
[DeptObjectiveDbId] BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
[EDepartId] BIGINT NOT NULL,
[ObjectiveTitle] NVARCHAR(MAX) NOT NULL,
[ObjectiveContents] NVARCHAR(MAX) NOT NULL,
[CreatedBy] BIGINT NOT NULL,
[CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
[UpdatedBy] BIGINT NULL,
[UpdatedAt] DATETIME NULL,
[Remarks] NVARCHAR(MAX),

FOREIGN KEY (EDepartId) REFERENCES [dbo].[EDepartmentDb](EDepartId),
FOREIGN KEY (CreatedBy) REFERENCES [dbo].[UserDb](Uid),
FOREIGN KEY (UpdatedBy) REFERENCES [dbo].[UserDb](Uid)
```

#### 수정 후 Entity (DB 테이블 기준)
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.DeptObjective;

/// <summary>
/// 부서 목표 Entity
/// </summary>
[Table("DeptObjectiveDb")]
public class DeptObjectiveDb
{
    /// <summary>
    /// DeptObjective ID (PK, IDENTITY)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 DeptObjectiveDbId { get; set; }

    /// <summary>
    /// 부서 ID (FK → EDepartmentDb.EDepartId)
    /// </summary>
    [Required]
    public Int64 EDepartId { get; set; }

    /// <summary>
    /// 목표 제목
    /// </summary>
    [Required]
    public string ObjectiveTitle { get; set; } = string.Empty;

    /// <summary>
    /// 목표 내용
    /// </summary>
    [Required]
    public string ObjectiveContents { get; set; } = string.Empty;

    /// <summary>
    /// 작성자 (FK → UserDb.Uid)
    /// </summary>
    [Required]
    public Int64 CreatedBy { get; set; }

    /// <summary>
    /// 작성일시
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 수정자 (FK → UserDb.Uid)
    /// </summary>
    public Int64? UpdatedBy { get; set; }

    /// <summary>
    /// 수정일시
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 비고
    /// </summary>
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

### 2.4. EvaluationLists Entity 수정

#### 파일: `MdcHR26Apps.Models/EvaluationLists/EvaluationLists.cs`

#### 현재 Entity (잘못됨)
```csharp
public class EvaluationLists
{
    [Key]
    public Int64 ELid { get; set; }

    [Required]
    public int Evaluation_Number { get; set; }

    [Required]
    [StringLength(255)]
    public string Evaluation_Item { get; set; } = string.Empty;

    public string? Evaluation_Description { get; set; }

    [Required]
    public int Score { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;
}
```

#### DB 테이블 (EvaluationLists.sql) - 정답
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

#### 수정 후 Entity (DB 테이블 기준)
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.EvaluationLists;

/// <summary>
/// 평가 항목 마스터 Entity
/// </summary>
[Table("EvaluationLists")]
public class EvaluationLists
{
    /// <summary>
    /// Evaluation List ID (PK, IDENTITY)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 Eid { get; set; }

    /// <summary>
    /// 평가 부서 번호
    /// </summary>
    [Required]
    public int Evaluation_Department_Number { get; set; }

    /// <summary>
    /// 평가 부서 이름
    /// </summary>
    [Required]
    [StringLength(20)]
    public string Evaluation_Department_Name { get; set; } = string.Empty;

    /// <summary>
    /// 평가 지표 번호
    /// </summary>
    [Required]
    public int Evaluation_Index_Number { get; set; }

    /// <summary>
    /// 평가 지표 이름
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Evaluation_Index_Name { get; set; } = string.Empty;

    /// <summary>
    /// 평가 업무 번호
    /// </summary>
    [Required]
    public int Evaluation_Task_Number { get; set; }

    /// <summary>
    /// 평가 업무 이름
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Evaluation_Task_Name { get; set; } = string.Empty;

    /// <summary>
    /// 평가 리스트 비고
    /// </summary>
    [StringLength(100)]
    public string? Evaluation_Lists_Remark { get; set; }
}
```

---

### 2.5. TasksDb Entity 수정

#### 파일: `MdcHR26Apps.Models/EvaluationTasks/TasksDb.cs`

#### 현재 Entity (잘못됨)
```csharp
public class TasksDb
{
    [Key]
    public Int64 Tid { get; set; }

    [Required]
    public Int64 Uid { get; set; }

    [Required]
    public Int64 SubAgreement_Number { get; set; }

    [Required]
    [StringLength(255)]
    public string Task_Name { get; set; } = string.Empty;

    public string? Task_Content { get; set; }

    public string? Task_Criteria { get; set; }

    public int? Estimated_Hours { get; set; }

    [Required]
    public bool Is_Completed { get; set; } = false;

    // Navigation Properties...
}
```

#### DB 테이블 (TasksDb.sql) - 정답
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

#### 수정 후 Entity (DB 테이블 기준)
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.EvaluationTasks;

/// <summary>
/// 업무/과업 관리 Entity
/// </summary>
[Table("TasksDb")]
public class TasksDb
{
    /// <summary>
    /// Task ID (PK, IDENTITY)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 Tid { get; set; }

    /// <summary>
    /// 업무명
    /// </summary>
    [Required]
    [StringLength(100)]
    public string TaskName { get; set; } = string.Empty;

    /// <summary>
    /// 업무 리스트 번호
    /// </summary>
    [Required]
    public Int64 TaksListNumber { get; set; }

    /// <summary>
    /// 업무 상태 (0: 진행중, 1: 종료, 2: 보류, 3: 취소)
    /// </summary>
    [Required]
    public int TaskStatus { get; set; }

    /// <summary>
    /// 업무 목표
    /// </summary>
    [Required]
    public string TaskObjective { get; set; } = string.Empty;

    /// <summary>
    /// 목표 달성도
    /// </summary>
    [Required]
    public int TargetProportion { get; set; }

    /// <summary>
    /// 결과 달성도
    /// </summary>
    [Required]
    public int ResultProportion { get; set; }

    /// <summary>
    /// 목표 달성 일자
    /// </summary>
    [Required]
    public DateTime TargetDate { get; set; }

    /// <summary>
    /// 결과 달성 일자
    /// </summary>
    [Required]
    public DateTime ResultDate { get; set; }

    /// <summary>
    /// 업무 평가 - 일정준수
    /// </summary>
    [Required]
    public double Task_Evaluation_1 { get; set; }

    /// <summary>
    /// 업무 평가 - 업무수행도
    /// </summary>
    [Required]
    public double Task_Evaluation_2 { get; set; }

    /// <summary>
    /// 업무 수준 - 난이도 (S: 1.2, A: 1.0, B: 0.8, C: 0.6)
    /// </summary>
    [Required]
    public double TaskLevel { get; set; }

    /// <summary>
    /// 업무 코멘트
    /// </summary>
    [Required]
    [StringLength(50)]
    public string TaskComments { get; set; } = string.Empty;
}
```

---

## 3. Repository 수정

**중요**: Entity 변경에 따라 5개 Repository 모두 SQL 쿼리 수정 필요

### 3.1. AgreementRepository.cs

필드명 변경 필요:
- `Agreement_Item_*` → `Report_Item_*` (4개 필드)
- 제거: `DeptObjective_Number`, `Is_Agreement`, `Agreement_Comment`, `Agreement_Date`

**수정 메서드**:
- `AddAsync`: INSERT 문 필드명 변경
- `UpdateAsync`: UPDATE 문 필드명 변경
- `GetByUidAsync`: ORDER BY 변경 (`Agreement_Item_Number` → `Report_Item_Number`)
- **제거할 메서드**: `IsAgreementCompleteAsync`, `GetPendingAgreementAsync`, `GetByDeptObjectiveAsync`

---

### 3.2. SubAgreementRepository.cs

**전체 구조 변경**:
- PK: `SAid` → `Sid`
- 모든 SQL 쿼리 재작성 필요

**수정 메서드**:
- `AddAsync`: 전체 필드 변경 (Report_Item_*, Report_SubItem_*, Task_Number)
- `UpdateAsync`: 전체 필드 변경
- 모든 조회 메서드: `SAid` → `Sid` 변경
- **제거할 메서드**: `GetByAgreementNumberAsync`, `IsSubAgreementCompleteAsync`, `GetPendingSubAgreementAsync`
- **수정할 메서드**: `GetTotalProportionAsync` (필드명 변경)

---

### 3.3. DeptObjectiveRepository.cs

필드명 변경 필요:
- PK: `DOid` → `DeptObjectiveDbId`
- `Objective_Title` → `ObjectiveTitle`, `Objective_Content` → `ObjectiveContents`
- 추가: `CreatedBy`, `CreatedAt`, `UpdatedBy`, `UpdatedAt`, `Remarks`
- 제거: `Achievement_Criteria`, `IsActive`

**수정 메서드**:
- `AddAsync`: INSERT 문 재작성
- `UpdateAsync`: UPDATE 문 재작성 (UpdatedBy, UpdatedAt 추가)
- 모든 조회 메서드: `DOid` → `DeptObjectiveDbId` 변경
- **제거할 메서드**: `GetActiveAsync`

---

### 3.4. EvaluationListsRepository.cs

**전체 구조 변경**:
- PK: `ELid` → `Eid`
- 모든 SQL 쿼리 재작성 필요

**수정 메서드**:
- `AddAsync`: 전체 필드 변경 (Evaluation_Department_*, Evaluation_Index_*, Evaluation_Task_*)
- `UpdateAsync`: 전체 필드 변경
- 모든 조회 메서드: `ELid` → `Eid` 변경
- **제거할 메서드**: `GetActiveAsync`, `GetSelectListAsync`

---

### 3.5. TasksRepository.cs

**전체 구조 변경**:
- 모든 SQL 쿼리 재작성 필요
- Uid 필드 없음 → 사용자별 조회 불가

**수정 메서드**:
- `AddAsync`: 전체 필드 변경 (TaskName, TaksListNumber, TaskStatus 등 12개 필드)
- `UpdateAsync`: 전체 필드 변경
- **제거할 메서드**: `GetByUidAsync`, `GetBySubAgreementNumberAsync`, `GetIncompleteTasksAsync`
- **유지할 메서드**: `GetCountByUserAsync`, `DeleteAllByUserAsync` (TaksListNumber 사용)

---

## 4. Interface 수정

각 Repository의 Interface도 수정 필요:
- `IAgreementRepository.cs`
- `ISubAgreementRepository.cs`
- `IDeptObjectiveRepository.cs`
- `IEvaluationListsRepository.cs`
- `ITasksRepository.cs`

제거된 메서드의 메서드 시그니처를 Interface에서도 제거해야 함.

---

## 5. 주의사항

### 5.1. 필드명 변경 주의
- **AgreementDb**: `Agreement_Item_*` → `Report_Item_*` (필드명 변경)
- **SubAgreementDb**: PK `SAid` → `Sid` (PK명 변경), 구조 완전 변경
- **DeptObjectiveDb**: PK `DOid` → `DeptObjectiveDbId` (PK명 변경), 필드 추가 (CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, Remarks)
- **EvaluationLists**: PK `ELid` → `Eid` (PK명 변경), 구조 완전 변경
- **TasksDb**: 구조 완전 변경 (Uid, SubAgreement_Number 제거)

### 5.2. Navigation Property 제거
- **SubAgreementDb**: Agreement FK 제거됨
- **TasksDb**: User, SubAgreement FK 제거됨

### 5.3. Repository 메서드 제거
많은 메서드가 제거되므로 **해당 메서드를 사용하는 코드가 있는지 확인 필수**:
- AgreementRepository: 3개 메서드 제거
- SubAgreementRepository: 3개 메서드 제거
- DeptObjectiveRepository: 1개 메서드 제거
- EvaluationListsRepository: 2개 메서드 제거
- TasksRepository: 3개 메서드 제거

---

## 6. 테스트 시나리오

### 6.1. 빌드 검증
```bash
dotnet build MdcHR26Apps.Models
```
- ✅ 컴파일 오류 없음

### 6.2. Repository SQL 검증
각 Repository의 SQL 쿼리가 DB 테이블과 일치하는지 확인:
- SELECT * 쿼리 실행 테스트
- INSERT/UPDATE 쿼리 실행 테스트
- 필드명 오타 확인

### 6.3. Interface 검증
- 제거된 메서드가 Interface에서도 제거되었는지 확인
- 메서드 시그니처가 일치하는지 확인

---

## 7. 완료 조건

- [ ] 5개 Entity 파일 수정 완료
  - [ ] AgreementDb.cs (필드명 변경)
  - [ ] SubAgreementDb.cs (전체 구조 변경)
  - [ ] DeptObjectiveDb.cs (PK명, 필드 추가)
  - [ ] EvaluationLists.cs (전체 구조 변경)
  - [ ] TasksDb.cs (전체 구조 변경)
- [ ] 5개 Repository 파일 수정 완료
  - [ ] AgreementRepository.cs (필드명 변경, 3개 메서드 제거)
  - [ ] SubAgreementRepository.cs (전체 구조 변경, 3개 메서드 제거)
  - [ ] DeptObjectiveRepository.cs (필드명 변경, 1개 메서드 제거)
  - [ ] EvaluationListsRepository.cs (전체 구조 변경, 2개 메서드 제거)
  - [ ] TasksRepository.cs (전체 구조 변경, 3개 메서드 제거)
- [ ] 5개 Interface 파일 수정 완료
- [ ] DB 테이블 구조와 100% 일치 확인
- [ ] 빌드 성공 (MdcHR26Apps.Models)
- [ ] Repository SQL 테스트 완료
- [ ] 제거된 메서드를 사용하는 코드 확인 및 수정
- [ ] 관련 이슈 업데이트

---

## 8. 관련 문서

**이슈**:
- [#009: Phase 3 WebApp 개발](../issues/009_phase3_webapp_development.md)
- [#012: v_ProcessTRListDB 불일치](../issues/012_v_processtrllistdb_view_column_mismatch.md)
- [#013: v_ReportTaskListDB 불일치](../issues/013_v_reporttasklistdb_entity_db_mismatch.md)

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

**Repository 파일 (수정 대상)**:
- `MdcHR26Apps.Models/EvaluationAgreement/AgreementRepository.cs`
- `MdcHR26Apps.Models/EvaluationSubAgreement/SubAgreementRepository.cs`
- `MdcHR26Apps.Models/DeptObjective/DeptObjectiveRepository.cs`
- `MdcHR26Apps.Models/EvaluationLists/EvaluationListsRepository.cs`
- `MdcHR26Apps.Models/EvaluationTasks/TasksRepository.cs`

**Interface 파일 (수정 대상)**:
- `MdcHR26Apps.Models/EvaluationAgreement/IAgreementRepository.cs`
- `MdcHR26Apps.Models/EvaluationSubAgreement/ISubAgreementRepository.cs`
- `MdcHR26Apps.Models/DeptObjective/IDeptObjectiveRepository.cs`
- `MdcHR26Apps.Models/EvaluationLists/IEvaluationListsRepository.cs`
- `MdcHR26Apps.Models/EvaluationTasks/ITasksRepository.cs`
