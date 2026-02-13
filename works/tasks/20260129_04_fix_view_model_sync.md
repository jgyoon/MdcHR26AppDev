# 작업지시서: View(SQL)와 View(Model) 동기화

**날짜**: 2026-01-29
**작업 유형**: 버그 수정 (Phase 2 구조적 문제 해결)
**관련 이슈**: [#011: Phase 3-3 관리자 페이지 재작업](../issues/011_phase3_3_admin_pages_build_errors.md)
**선행 작업지시서**:
- `20260119_04_phase2_4_view_models.md` (Phase 2-4, 여기서 문제 발생)
**차단된 작업지시서**:
- `20260129_03_phase3_3_totalreport_admin_complete.md` (Step 4부터 차단)

---

## 1. 문제 상황

### 1.1. 증상

**발견 경위**:
- 20260129_03 작업 중 AdminReportListView 컴포넌트에서 v_ProcessTRListDB 사용 시도
- Model에 평가 점수 필드가 없어 컴파일 오류 발생
- TotalReport/Admin 페이지 구현 완전 차단

**재현 방법**:
1. Database/dbo/v_ProcessTRListDB.sql 파일 확인 → 38개 필드
2. MdcHR26Apps.Models/Views/v_ProcessTRListDB/v_ProcessTRListDB.cs 확인 → 15개 필드
3. 평가 점수 필드들이 모두 누락됨

### 1.2. 검증 결과

| View | SQL 필드 | Model 필드 | 상태 | 문제 |
|------|----------|------------|------|------|
| v_ProcessTRListDB | 38개 | 15개 | ❌ 심각 | 평가 점수 20개 필드 누락 |
| v_TotalReportListDB | 25개 | 17개 | ❌ 심각 | 평가 점수 15개 필드 누락 |
| v_DeptObjectiveListDb | 6개 | 7개 | ❌ 불일치 | 필드명 다름, SQL에 없는 필드 존재 |
| v_MemberListDB | 14개 | 14개 | ✅ 일치 | 문제 없음 |
| v_EvaluationUsersList | 14개 | 14개 | ✅ 일치 | 문제 없음 (최근 작업) |
| v_ReportTaskListDB | 가변 | 18개 | ✅ 의도 | SQL에 B.* 사용 (문제 없음) |

### 1.3. 영향도

**차단된 기능**:
- TotalReport/Admin 페이지 전체 (Index, Details, Edit, ReportInit)
- 평가 점수 표시 불가
- 평가 프로세스 상태 확인 불가

**영향받는 컴포넌트**:
- AdminReportListView.razor
- TotalReport/Admin 4개 페이지
- 기타 v_ProcessTRListDB를 사용하는 모든 페이지

---

## 2. 원인 분석

### 2.1. 근본 원인

**Phase 2-4 (20260119_04)에서 발생**:
- View Model 생성 시 **임의로 코드 축약**
- DB View의 모든 필드를 Model에 반영하지 않음
- "필요할 것 같은 것만" 선택하여 생성

**문제 코드 패턴**:
```csharp
// ❌ 잘못된 접근 - 임의 축약
public class v_ProcessTRListDB
{
    public Int64 Pid { get; set; }
    public Int64 Uid { get; set; }
    // ... 일부 필드만 선택
    public decimal Final_Score { get; set; }  // ← SQL에 없는 필드 추가
}
```

**올바른 접근**:
```csharp
// ✅ 올바른 접근 - DB View와 100% 일치
public class v_ProcessTRListDB
{
    // SQL의 모든 필드를 순서대로 그대로 반영
    public Int64 Pid { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    // ... SQL의 모든 필드
}
```

### 2.2. 누락된 필드 상세

**v_ProcessTRListDB 누락 필드 (약 20개)**:
- TeamLeader_Id, TeamLeader_Name
- Director_Id, Director_Name
- Is_Request, Is_Agreement, Is_SubRequest, Is_SubAgreement
- Is_User_Submission, Is_Teamleader_Submission, Is_Director_Submission
- FeedBackStatus, FeedBack_Submission
- User_Evaluation_1/2/3/4
- TeamLeader_Evaluation_1/2/3, TeamLeader_Comment
- Feedback_Evaluation_1/2/3, Feedback_Comment
- Director_Evaluation_1/2/3, Director_Comment
- Total_Score, Director_Score (TeamLeader_Score는 SQL에 없음)

**v_TotalReportListDB 누락 필드 (약 15개)**:
- User_Evaluation_1/2/3/4
- TeamLeader_Evaluation_1/2/3, TeamLeader_Comment
- Feedback_Evaluation_1/2/3, Feedback_Comment
- Director_Evaluation_1/2/3, Director_Comment
- Total_Score, Director_Score, TeamLeader_Score

**v_DeptObjectiveListDb 불일치**:
- SQL: DeptObjectiveDbId, ObjectiveTitle, ObjectiveContents, Remarks
- Model: DOid, Objective_Title, Objective_Description, Start_Date, End_Date
- 필드명 불일치 + Model에 SQL에 없는 필드 존재

---

## 3. 작업 단계

### 원칙

**절대 원칙**:
1. Database/dbo/*.sql 파일을 **진리의 원천**으로 삼음
2. SQL의 모든 필드를 순서대로 그대로 Model에 반영
3. 임의 축약 금지
4. 필드명 변경 금지 (SQL 그대로 사용)
5. 타입만 C# 타입으로 변환

**타입 매핑**:
- BIGINT → Int64
- INT → int
- NVARCHAR, VARCHAR → string
- FLOAT → double
- BIT → bool
- DATETIME → DateTime

---

### Step 1: v_ProcessTRListDB.cs 완전 재작성

**파일**: `MdcHR26Apps.Models/Views/v_ProcessTRListDB/v_ProcessTRListDB.cs`

**참고**: `Database/dbo/v_ProcessTRListDB.sql`

**작업**:
1. 기존 파일 전체 삭제
2. SQL 파일의 SELECT 절 확인 (38개 필드)
3. 순서대로 모든 필드를 C# 속성으로 작성

**전체 코드**:
```csharp
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MdcHR26Apps.Models.Views.v_ProcessTRListDB;

/// <summary>
/// v_ProcessTRListDB View Entity (읽기 전용)
/// ProcessDb + TotalReportDb + UserDb 조인 뷰
/// </summary>
[Keyless]
[Table("v_ProcessTRListDB")]
public class v_ProcessTRListDB
{
    // ProcessDb 필드
    public Int64 Pid { get; set; }

    // UserDb 필드 (U)
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;

    // TeamLeader 정보 (TL)
    public string TeamLeader_Id { get; set; } = string.Empty;
    public string TeamLeader_Name { get; set; } = string.Empty;

    // Director 정보 (D)
    public string Director_Id { get; set; } = string.Empty;
    public string Director_Name { get; set; } = string.Empty;

    // ProcessDb 상태 필드 (A)
    public bool Is_Request { get; set; }
    public bool Is_Agreement { get; set; }
    public bool Is_SubRequest { get; set; }
    public bool Is_SubAgreement { get; set; }
    public bool Is_User_Submission { get; set; }
    public bool Is_Teamleader_Submission { get; set; }
    public bool Is_Director_Submission { get; set; }
    public bool FeedBackStatus { get; set; }
    public bool FeedBack_Submission { get; set; }

    // UserDb Uid (A)
    public Int64 Uid { get; set; }

    // TotalReportDb 필드 (C)
    public Int64 TRid { get; set; }

    // [1] 평가대상자 평가 (User_Evaluation)
    public double User_Evaluation_1 { get; set; }
    public double User_Evaluation_2 { get; set; }
    public double User_Evaluation_3 { get; set; }
    public string User_Evaluation_4 { get; set; } = string.Empty;

    // [2] 팀장 평가 (TeamLeader_Evaluation)
    public double TeamLeader_Evaluation_1 { get; set; }
    public double TeamLeader_Evaluation_2 { get; set; }
    public double TeamLeader_Evaluation_3 { get; set; }
    public string TeamLeader_Comment { get; set; } = string.Empty;

    // [3] 피드백 (Feedback_Evaluation)
    public double Feedback_Evaluation_1 { get; set; }
    public double Feedback_Evaluation_2 { get; set; }
    public double Feedback_Evaluation_3 { get; set; }
    public string Feedback_Comment { get; set; } = string.Empty;

    // [4] 임원 평가 (Director_Evaluation)
    public double Director_Evaluation_1 { get; set; }
    public double Director_Evaluation_2 { get; set; }
    public double Director_Evaluation_3 { get; set; }
    public string Director_Comment { get; set; } = string.Empty;

    // [5] 종합 점수
    public double Total_Score { get; set; }
    public double Director_Score { get; set; }
}
```

**주의사항**:
- SQL에는 TeamLeader_Score가 없으므로 Model에도 없어야 함
- ISNULL로 감싸진 필드도 nullable이 아닌 기본값 사용 (double은 0, string은 Empty)
- 필드 순서는 SQL SELECT 순서와 동일하게

**빌드 테스트**: 오류 0개 확인

---

### Step 2: v_TotalReportListDB.cs 완전 재작성

**파일**: `MdcHR26Apps.Models/Views/v_TotalReportListDB/v_TotalReportListDB.cs`

**참고**: `Database/dbo/v_TotalReportListDB.sql`

**작업**:
1. 기존 파일 전체 삭제
2. SQL 파일의 SELECT 절 확인 (25개 필드)
3. 순서대로 모든 필드를 C# 속성으로 작성

**전체 코드**:
```csharp
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MdcHR26Apps.Models.Views.v_TotalReportListDB;

/// <summary>
/// v_TotalReportListDB View Entity (읽기 전용)
/// TotalReportDb + UserDb 조인 뷰
/// </summary>
[Keyless]
[Table("v_TotalReportListDB")]
public class v_TotalReportListDB
{
    // TotalReportDb 필드 (A)
    public Int64 TRid { get; set; }
    public Int64 Uid { get; set; }

    // UserDb 필드 (B)
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;

    // [1] 평가대상자 평가 (User_Evaluation)
    public double User_Evaluation_1 { get; set; }
    public double User_Evaluation_2 { get; set; }
    public double User_Evaluation_3 { get; set; }
    public string User_Evaluation_4 { get; set; } = string.Empty;

    // [2] 팀장 평가 (TeamLeader_Evaluation)
    public double TeamLeader_Evaluation_1 { get; set; }
    public double TeamLeader_Evaluation_2 { get; set; }
    public double TeamLeader_Evaluation_3 { get; set; }
    public string TeamLeader_Comment { get; set; } = string.Empty;

    // [3] 피드백 (Feedback_Evaluation)
    public double Feedback_Evaluation_1 { get; set; }
    public double Feedback_Evaluation_2 { get; set; }
    public double Feedback_Evaluation_3 { get; set; }
    public string Feedback_Comment { get; set; } = string.Empty;

    // [4] 임원 평가 (Director_Evaluation)
    public double Director_Evaluation_1 { get; set; }
    public double Director_Evaluation_2 { get; set; }
    public double Director_Evaluation_3 { get; set; }
    public string Director_Comment { get; set; } = string.Empty;

    // [5] 종합 점수
    public double Total_Score { get; set; }
    public double Director_Score { get; set; }
    public double TeamLeader_Score { get; set; }
}
```

**주의사항**:
- SQL에서 FLOAT는 C#에서 double로 변환
- NVARCHAR(MAX)는 string으로 변환
- NULL 가능 필드도 기본값 사용 (string.Empty, 0)

**빌드 테스트**: 오류 0개 확인

---

### Step 3: v_DeptObjectiveListDb.cs 완전 재작성

**파일**: `MdcHR26Apps.Models/Views/v_DeptObjectiveListDb/v_DeptObjectiveListDb.cs`

**참고**: `Database/dbo/v_DeptObjectiveListDb.sql`

**작업**:
1. 기존 파일 전체 삭제
2. SQL 파일의 SELECT 절 확인 (6개 필드)
3. 순서대로 모든 필드를 C# 속성으로 작성

**전체 코드**:
```csharp
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MdcHR26Apps.Models.Views.v_DeptObjectiveListDb;

/// <summary>
/// v_DeptObjectiveListDb View Entity (읽기 전용)
/// DeptObjectiveDb + EDepartmentDb 조인 뷰
/// </summary>
[Keyless]
[Table("v_DeptObjectiveListDb")]
public class v_DeptObjectiveListDb
{
    // DeptObjectiveDb 필드 (A)
    public Int64 DeptObjectiveDbId { get; set; }
    public Int64 EDepartId { get; set; }

    // EDepartmentDb 필드 (B)
    public string EDepartmentName { get; set; } = string.Empty;

    // DeptObjectiveDb 필드 (A)
    public string ObjectiveTitle { get; set; } = string.Empty;
    public string ObjectiveContents { get; set; } = string.Empty;
    public string Remarks { get; set; } = string.Empty;
}
```

**주의사항**:
- SQL의 필드명을 그대로 사용 (ObjectiveTitle, ObjectiveContents, Remarks)
- Start_Date, End_Date는 SQL에 없으므로 제거
- DOid → DeptObjectiveDbId로 변경 (SQL 그대로)
- Objective_Title → ObjectiveTitle (SQL 그대로)

**빌드 테스트**: 오류 0개 확인

---

### Step 4: 최종 빌드 테스트

**명령어**:
```bash
dotnet build
```

**예상 결과**:
- 오류: 0개
- 경고: 기존 경고만 (새 경고 없음)

**확인 사항**:
- 3개 View Model 파일이 정상적으로 컴파일됨
- 기존 코드에서 사용 중인 부분이 있다면 컴파일 오류 발생 (정상, 수정 필요)

---

## 4. 테스트 계획

개발자가 테스트할 항목:

### Test 1: 빌드 성공 확인
1. `dotnet build` 실행
2. **확인**: 오류 0개

### Test 2: 기존 사용 코드 확인
1. v_ProcessTRListDB, v_TotalReportListDB, v_DeptObjectiveListDb를 사용하는 코드 검색
2. **확인**: 필드명 변경으로 인한 오류 확인 및 수정

### Test 3: 20260129_03 작업 재개 가능 확인
1. AdminReportListView.razor에서 v_ProcessTRListDB 사용 시도
2. **확인**: 평가 점수 필드 접근 가능

---

## 5. 완료 조건

- [ ] Step 1: v_ProcessTRListDB.cs 완전 재작성 (38개 필드)
- [ ] Step 2: v_TotalReportListDB.cs 완전 재작성 (25개 필드)
- [ ] Step 3: v_DeptObjectiveListDb.cs 완전 재작성 (6개 필드)
- [ ] Step 4: 빌드 테스트 성공 (오류 0개)
- [ ] Test 1: 빌드 성공 확인
- [ ] Test 2: 기존 사용 코드 확인 및 수정
- [ ] Test 3: 20260129_03 작업 재개 가능 확인

---

## 6. 후속 작업

**즉시 진행**:
- 20260129_03 작업 재개 (Step 4부터)
- 또는 20260129_05 작업지시서 (2025년 코드 복사 기반 재작성)

**재발 방지**:
- View Model 생성 시 DB View와 100% 일치 원칙 재확인
- 임의 축약 절대 금지
- Phase 2 작업지시서에 이 원칙 명시

---

**작업 시작일**: 2026-01-29
**예상 소요 시간**: 30분
**우선순위**: 긴급 (Phase 3-3 작업 차단 중)
