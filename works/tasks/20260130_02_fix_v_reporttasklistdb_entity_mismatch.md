# 작업지시서: v_ReportTaskListDB Entity 및 DB View 수정

**날짜**: 2026-01-30
**작업 타입**: 버그 수정 + Entity 재작성
**예상 소요**: 1시간
**관련 이슈**: [#013](../issues/013_v_reporttasklistdb_entity_db_mismatch.md)

---

## 1. 작업 개요

### 배경
- Issue #012 수정 후 v_ReportTaskListDB에서 동일 오류 발생
- Entity가 2025년 코드 기반으로 작성되어 실제 DB와 불일치
- ProcessDb 조인 없어 Pid가 없음

### 목표
1. DB View에 ProcessDb 조인 추가 (Pid 포함)
2. Entity를 실제 DB 구조에 맞게 전체 재작성
3. Repository 및 Interface 수정

---

## 2. Phase 1: DB View 수정 (개발자 작업)

### 파일: `Database/dbo/v_ReportTaskListDB.sql`

#### 현재 SQL
```sql
CREATE VIEW [dbo].[v_ReportTaskListDB]
AS SELECT
    A.Rid,
    U.UserId,
    U.UserName,
    A.Report_Item_Number,
    A.Report_Item_Name_1,
    A.Report_Item_Name_2,
    A.Report_Item_Proportion,
    A.Report_SubItem_Name,
    A.Report_SubItem_Proportion,
    B.*
FROM
    [dbo].[ReportDb] A
    INNER JOIN [dbo].[UserDb] U ON A.Uid = U.Uid
    INNER JOIN [dbo].[TasksDb] B ON A.Task_Number = B.TaksListNumber
```

#### 수정 후 SQL
```sql
CREATE VIEW [dbo].[v_ReportTaskListDB]
AS SELECT
    -- ProcessDb 필드
    P.Pid,

    -- ReportDb 필드
    A.Rid,
    A.Uid,
    A.Report_Item_Number,
    A.Report_Item_Name_1,
    A.Report_Item_Name_2,
    A.Report_Item_Proportion,
    A.Report_SubItem_Name,
    A.Report_SubItem_Proportion,
    A.Task_Number,
    A.User_Evaluation_1,
    A.User_Evaluation_2,
    A.User_Evaluation_3,
    A.User_Evaluation_4,
    A.TeamLeader_Evaluation_1,
    A.TeamLeader_Evaluation_2,
    A.TeamLeader_Evaluation_3,
    A.TeamLeader_Evaluation_4,
    A.Director_Evaluation_1,
    A.Director_Evaluation_2,
    A.Director_Evaluation_3,
    A.Director_Evaluation_4,
    A.Total_Score,

    -- UserDb 필드
    U.UserId,
    U.UserName,

    -- TasksDb 필드 (명시적 선택, B.* 제거)
    B.Tid,
    B.TaskName,
    B.TaksListNumber,
    B.TaskStatus,
    B.TaskObjective,
    B.TargetProportion,
    B.ResultProportion,
    B.TargetDate,
    B.ResultDate,
    B.Task_Evaluation_1 AS Task_Eval_1,
    B.Task_Evaluation_2 AS Task_Eval_2,
    B.TaskLevel,
    B.TaskComments
FROM
    [dbo].[ReportDb] A
    INNER JOIN [dbo].[UserDb] U ON A.Uid = U.Uid
    INNER JOIN [dbo].[ProcessDb] P ON A.Uid = P.Uid    -- 추가
    INNER JOIN [dbo].[TasksDb] B ON A.Task_Number = B.TaksListNumber
```

#### 수정 사항
1. **ProcessDb 조인 추가**: `INNER JOIN ProcessDb P ON A.Uid = P.Uid`
2. **Pid 추가**: `P.Pid`
3. **명시적 컬럼 선택**: `B.*` 제거, 필요한 컬럼만 명시
4. **TasksDb.Task_Evaluation_1/2 별칭**: ReportDb와 중복 방지

#### 개발자 작업 순서
1. SSMS에서 `Database/dbo/v_ReportTaskListDB.sql` 수정
2. 기존 View 삭제: `DROP VIEW IF EXISTS [dbo].[v_ReportTaskListDB]`
3. 새 View 생성: 위 SQL 실행
4. 확인: `SELECT TOP 10 * FROM v_ReportTaskListDB`

---

## 3. Phase 2: Entity 재작성 (Claude 작업)

### 파일: `MdcHR26Apps.Models/Views/v_ReportTaskListDB/v_ReportTaskListDB.cs`

#### 전체 재작성

**기존 Entity 삭제 후 새로 작성**:

```csharp
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MdcHR26Apps.Models.Views.v_ReportTaskListDB;

/// <summary>
/// v_ReportTaskListDB View Entity (읽기 전용)
/// ProcessDb + ReportDb + TasksDb + UserDb 조인 뷰
/// 2026년 DB 구조 기반
/// </summary>
[Keyless]
[Table("v_ReportTaskListDB")]
public class v_ReportTaskListDB
{
    // ProcessDb 필드
    public Int64 Pid { get; set; }

    // ReportDb 필드
    public Int64 Rid { get; set; }
    public Int64 Uid { get; set; }
    public int Report_Item_Number { get; set; }
    public string Report_Item_Name_1 { get; set; } = string.Empty;
    public string Report_Item_Name_2 { get; set; } = string.Empty;
    public int Report_Item_Proportion { get; set; }
    public string Report_SubItem_Name { get; set; } = string.Empty;
    public int Report_SubItem_Proportion { get; set; }
    public Int64 Task_Number { get; set; }

    // ReportDb 평가 점수
    public double User_Evaluation_1 { get; set; }
    public double User_Evaluation_2 { get; set; }
    public double User_Evaluation_3 { get; set; }
    public string User_Evaluation_4 { get; set; } = string.Empty;
    public double TeamLeader_Evaluation_1 { get; set; }
    public double TeamLeader_Evaluation_2 { get; set; }
    public double TeamLeader_Evaluation_3 { get; set; }
    public string TeamLeader_Evaluation_4 { get; set; } = string.Empty;
    public double Director_Evaluation_1 { get; set; }
    public double Director_Evaluation_2 { get; set; }
    public double Director_Evaluation_3 { get; set; }
    public string Director_Evaluation_4 { get; set; } = string.Empty;
    public double Total_Score { get; set; }

    // UserDb 필드
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;

    // TasksDb 필드
    public Int64 Tid { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public Int64 TaksListNumber { get; set; }  // 오타 그대로 유지 (DB 컬럼명)
    public int TaskStatus { get; set; }
    public string TaskObjective { get; set; } = string.Empty;
    public int TargetProportion { get; set; }
    public int ResultProportion { get; set; }
    public DateTime TargetDate { get; set; }
    public DateTime ResultDate { get; set; }
    public double Task_Eval_1 { get; set; }  // DB View 별칭
    public double Task_Eval_2 { get; set; }  // DB View 별칭
    public double TaskLevel { get; set; }
    public string TaskComments { get; set; } = string.Empty;
}
```

---

## 4. Phase 3: Repository 수정 (Claude 작업)

### 파일: `MdcHR26Apps.Models/Views/v_ReportTaskListDB/v_ReportTaskListRepository.cs`

#### 수정 1: GetByAllAsync() - Line 25

**변경 전**:
```csharp
ORDER BY Pid DESC, Rid, Task_Start_Date
```

**변경 후**:
```csharp
ORDER BY Pid DESC, Rid, TargetDate
```

**사유**: Task_Start_Date → TargetDate (실제 컬럼명)

---

#### 수정 2: GetAllAsync() - Line 41

**변경 전**:
```csharp
ORDER BY Pid DESC, Rid, Task_Start_Date
```

**변경 후**:
```csharp
ORDER BY Pid DESC, Rid, TargetDate
```

---

#### 수정 3: GetByReportIdAsync() - Line 59

**변경 전**:
```csharp
ORDER BY Task_Start_Date
```

**변경 후**:
```csharp
ORDER BY TargetDate
```

---

#### 수정 4: GetByUserIdAsync() - Line 76

**변경 전**:
```csharp
ORDER BY Pid DESC, Task_Start_Date
```

**변경 후**:
```csharp
ORDER BY Pid DESC, TargetDate
```

---

#### 수정 5: GetByProcessIdAsync() - Line 93

**변경 전**:
```csharp
ORDER BY Rid, Task_Start_Date
```

**변경 후**:
```csharp
ORDER BY Rid, TargetDate
```

---

#### 수정 6: GetByTaskStatusAsync() - Line 110

**변경 전**:
```csharp
ORDER BY Pid DESC, Task_Start_Date
```

**변경 후**:
```csharp
ORDER BY Pid DESC, TargetDate
```

---

#### 수정 7: GetBySubAgreementAsync() - Line 144

**변경 전**:
```csharp
ORDER BY Task_Start_Date
```

**변경 후**:
```csharp
ORDER BY TargetDate
```

---

### 파일: `MdcHR26Apps.Models/Views/v_ReportTaskListDB/Iv_ReportTaskListRepository.cs`

**수정 없음** - 메서드 시그니처는 그대로 유지

---

## 5. 테스트 계획

### 빌드 테스트
```bash
cd MdcHR26Apps.BlazorServer
dotnet build
```
**예상 결과**: 오류 0개

### 런타임 테스트
1. DB View 수정 완료 확인
2. 애플리케이션 실행
3. Admin 로그인
4. `/Admin/TotalReport` 접속
5. **기대 결과**: SqlException 없이 정상 로드

---

## 6. 주의사항

### DB View 수정 (개발자)
- ProcessDb 조인 시 ON 조건: `A.Uid = P.Uid` (Rid가 아님)
- TasksDb.Task_Evaluation_1/2 별칭 필수 (ReportDb와 컬럼명 중복)
- B.* 제거하고 필요한 컬럼만 명시적 선택

### Entity 재작성 (Claude)
- **기존 Entity 전체 삭제** 후 새로 작성
- DB View 컬럼명과 정확히 일치
- TaksListNumber 오타 그대로 유지 (DB 컬럼명)

### Repository 수정 (Claude)
- Task_Start_Date → TargetDate로 일괄 변경
- 다른 컬럼명은 그대로 유지

---

## 7. 관련 문서

**이슈**: [#013](../issues/013_v_reporttasklistdb_entity_db_mismatch.md)
**DB 구조**:
- [ReportDb.sql](../../Database/dbo/ReportDb.sql)
- [TasksDb.sql](../../Database/dbo/TasksDb.sql)
- [ProcessDb.sql](../../Database/dbo/ProcessDb.sql)
**선행 작업**: [20260130_01](20260130_01_fix_v_processtrllistdb_column_mismatch.md) (v_ProcessTRListDB 수정)
