# 이슈 #013: v_ReportTaskListDB Entity와 DB View 구조 불일치

**날짜**: 2026-01-30
**상태**: 완료
**우선순위**: 높음
**관련 이슈**: [#012](012_v_processtrllistdb_view_column_mismatch.md)

---

## 개발자 요청

**배경**:
- Issue #012 수정 후 런타임 테스트 중 새로운 오류 발생
- v_ProcessTRListDB는 수정 완료했으나, v_ReportTaskListDB에서 동일 유형 오류 발생

**오류 현상**:
```
SqlException: Invalid column name 'Pid'.
Invalid column name 'Task_Start_Date'.
```

**발생 위치**:
- `v_ReportTaskListRepository.GetAllAsync()` - Line 45
- `/Admin/TotalReport` 페이지 로드 시

**요청 사항**:
1. v_ReportTaskListDB View에 Pid 추가 (ProcessDb 조인)
2. Entity 클래스를 실제 DB 구조에 맞게 전체 재작성
3. Repository 및 Interface 수정

---

## 문제 분석

### 1. 원인

**Entity가 2025년 또는 다른 버전의 DB 구조 기반**:
- Entity에는 있지만 실제 DB에 없는 컬럼 다수
- View는 ProcessDb와 조인하지 않아 Pid가 없음
- TasksDb 컬럼명과 Entity 컬럼명이 완전히 다름

### 2. 현재 v_ReportTaskListDB View 구조

```sql
SELECT
    A.Rid,
    U.UserId, U.UserName,
    A.Report_Item_Number,
    A.Report_Item_Name_1,
    A.Report_Item_Name_2,
    A.Report_Item_Proportion,
    A.Report_SubItem_Name,
    A.Report_SubItem_Proportion,
    B.*  -- TasksDb 전체
FROM ReportDb A
INNER JOIN UserDb U ON A.Uid = U.Uid
INNER JOIN TasksDb B ON A.Task_Number = B.TaksListNumber
```

**문제점**:
- ProcessDb 조인 없음 → Pid 없음
- B.*로 TasksDb 컬럼을 가져오지만, 컬럼명이 Entity와 다름

### 3. Entity vs 실제 DB 불일치

| Entity 컬럼 | 실제 DB | 상태 |
|-------------|---------|------|
| `Pid` | 없음 (ProcessDb 조인 필요) | ❌ |
| `Self_Score` | ReportDb에 없음 | ❌ |
| `Evaluator_Score` | ReportDb에 없음 | ❌ |
| `Final_Score` | ReportDb에 없음 | ❌ |
| `Report_Status` | ReportDb에 없음 | ❌ |
| `Task_Title` | TasksDb.TaskName | ❌ 불일치 |
| `Task_Description` | TasksDb.TaskObjective | ❌ 불일치 |
| `Task_Start_Date` | TasksDb에 없음 | ❌ |
| `Task_End_Date` | TasksDb.TargetDate / ResultDate | ❌ 불일치 |
| `Task_Status` | TasksDb.TaskStatus | ✅ 일치 |
| `Task_Achievement_Rate` | TasksDb.TargetProportion? | ❌ 불일치 |
| `SubAgreement_Number` | TasksDb.TaksListNumber | ❌ 불일치 |
| `SAid` | 테이블 없음 | ❌ |

---

## 해결 방안

### Option 1: DB View에 별칭 사용 ❌
- TasksDb 컬럼에 AS로 별칭 지정
- 존재하지 않는 컬럼은 NULL 또는 계산
- **비추천**: DB에 없는 컬럼이 너무 많음

### Option 2: Entity를 실제 DB에 맞게 재작성 ✅ (채택)
- **실제 DB 구조에 맞게 Entity 전체 재작성**
- View는 ProcessDb 조인만 추가하고 컬럼명 그대로 사용
- Repository도 수정
- **추천**: 정확하고 일관성 있음

---

## 작업 계획

### Phase 1: DB View 수정 (개발자 작업)
**파일**: `Database/dbo/v_ReportTaskListDB.sql`

1. ProcessDb 조인 추가
2. 명시적 컬럼 선택 (B.* 제거)

```sql
SELECT
    P.Pid,                           -- ProcessDb 추가
    A.Rid,
    A.Uid,
    U.UserId,
    U.UserName,
    A.Report_Item_Number,
    A.Report_Item_Name_1,
    A.Report_Item_Name_2,
    A.Report_Item_Proportion,
    A.Report_SubItem_Name,
    A.Report_SubItem_Proportion,
    A.Task_Number,
    -- TasksDb 컬럼 명시
    B.Tid,
    B.TaskName,
    B.TaksListNumber,
    B.TaskStatus,
    B.TaskObjective,
    B.TargetProportion,
    B.ResultProportion,
    B.TargetDate,
    B.ResultDate,
    B.Task_Evaluation_1,
    B.Task_Evaluation_2,
    B.TaskLevel,
    B.TaskComments
FROM ReportDb A
INNER JOIN UserDb U ON A.Uid = U.Uid
INNER JOIN ProcessDb P ON A.Uid = P.Uid    -- 추가
INNER JOIN TasksDb B ON A.Task_Number = B.TaksListNumber
```

### Phase 2: Entity 재작성 (Claude 작업)
**파일**: `MdcHR26Apps.Models/Views/v_ReportTaskListDB/v_ReportTaskListDB.cs`

실제 DB View에 맞게 전체 재작성:
- ProcessDb 필드
- ReportDb 필드
- UserDb 필드
- TasksDb 필드

### Phase 3: Repository 및 Interface 수정 (Claude 작업)
**파일**:
- `v_ReportTaskListRepository.cs`
- `Iv_ReportTaskListRepository.cs`

ORDER BY 등 필요한 부분만 수정

---

## 진행 사항

- [x] 문제 확인 및 원인 분석
- [x] 해결 방안 결정
- [x] 작업지시서 작성
- [x] DB View 수정 (개발자)
- [x] Entity 재작성 (62줄)
- [x] Repository 수정 (Task_Start_Date → TargetDate)
- [x] ExcelManage.cs 수정 (컬럼명 매핑)
- [x] 빌드 테스트 (오류 0개)
- [x] 런타임 테스트 (/Admin/TotalReport 정상 로드)

---

## 관련 문서

**작업지시서**:
- [20260130_02_fix_v_reporttasklistdb_entity_mismatch.md](../tasks/20260130_02_fix_v_reporttasklistdb_entity_mismatch.md) (작성 예정)

**관련 이슈**:
- [#012: v_ProcessTRListDB View 컬럼 불일치](012_v_processtrllistdb_view_column_mismatch.md) - 동일 유형
- [#011: Phase 3-3 관리자 페이지 빌드 오류](011_phase3_3_admin_pages_build_errors.md) - 상위 이슈

**관련 파일**:
- `Database/dbo/v_ReportTaskListDB.sql` - View 정의
- `Database/dbo/ReportDb.sql` - ReportDb 테이블
- `Database/dbo/TasksDb.sql` - TasksDb 테이블
- `Database/dbo/ProcessDb.sql` - ProcessDb 테이블
- `MdcHR26Apps.Models/Views/v_ReportTaskListDB/v_ReportTaskListDB.cs` - Entity
- `MdcHR26Apps.Models/Views/v_ReportTaskListDB/v_ReportTaskListRepository.cs` - Repository

---

## 개발자 피드백

**작업 시작**: 2026-01-30
**작업 완료**: 2026-01-30
**최종 상태**: 완료 ✅
**비고**:
- Entity 자체가 실제 DB와 완전히 불일치
- 별칭 사용 대신 Entity를 DB에 맞게 재작성하는 것으로 결정
- DB View는 ProcessDb 조인 추가 및 명시적 컬럼 선택으로 수정
- 수정 사항:
  - DB View: ProcessDb 조인, 명시적 컬럼 선택 (개발자 작업 완료)
  - Entity: 전체 재작성 (62줄)
  - Repository: Task_Start_Date → TargetDate 일괄 변경
  - ExcelManage.cs: 컬럼명 매핑 수정
- 빌드 테스트 통과 (오류 0개)
- 런타임 테스트 성공: /Admin/TotalReport 페이지 정상 로드, SqlException 없음
