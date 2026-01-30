# 이슈 #012: v_ProcessTRListDB View 컬럼 불일치 오류

**날짜**: 2026-01-30
**상태**: 완료
**우선순위**: 높음
**관련 이슈**: [#011](011_phase3_3_admin_pages_build_errors.md)

---

## 개발자 요청

**배경**:
- Phase 3-3 TotalReport 관리자 페이지 구현 중
- 20260129_06 작업지시서에 따라 Step 4-14 완료
- 빌드는 성공했으나 런타임 오류 발생

**오류 현상**:
```
SqlException: Invalid column name 'Process_Year'.
Invalid column name 'Start_Date'.
```

**발생 위치**:
- `v_ProcessTRListRepository.GetAllAsync()` 메서드
- `v_ProcessTRListRepository.GetByAllAsync()` 메서드

**요청 사항**:
1. v_ProcessTRListDB View의 실제 컬럼 구조 확인
2. 2026년 DB 구조에 맞게 Repository SQL 수정
3. 동일한 문제가 있는 다른 Repository 메서드도 일괄 수정

---

## 문제 분석

### 1. 원인

**2025년 코드 기반으로 작성**:
- 작업지시서([20260129_03](../tasks/20260129_03_phase3_3_totalreport_admin_complete.md))가 2025년 코드 기반
- v_ProcessTRListRepository의 ORDER BY 절에 2025년 컬럼 사용

**2026년 v_ProcessTRListDB 실제 컬럼**:
```csharp
// 존재하는 컬럼
- Pid (Int64)
- UserId, UserName
- TeamLeader_Id, TeamLeader_Name
- Director_Id, Director_Name
- 상태 필드들 (Is_Request, Is_Agreement, ...)
- TRid
- 평가 점수 필드들 (User_Evaluation_1/2/3, TeamLeader_Evaluation_1/2/3, ...)
- Total_Score, Director_Score

// 존재하지 않는 컬럼 (2025년에만 있음)
- Process_Year ❌
- Start_Date ❌
- Process_Status ❌
- Final_Score ❌
- Final_Grade ❌
```

### 2. 영향 범위

**오류 발생 메서드** (`v_ProcessTRListRepository.cs`):
1. `GetByAllAsync()` - Line 24: `ORDER BY Process_Year DESC, Start_Date DESC`
2. `GetAllAsync()` - Line 142: `ORDER BY Process_Year DESC, Start_Date DESC`
3. `GetByUserIdAsync()` - Line 59: `ORDER BY Process_Year DESC, Start_Date DESC`
4. `GetByYearAsync()` - Line 75: `WHERE Process_Year = @year`
5. `GetByGradeAsync()` - Line 92: `WHERE Final_Grade = @grade`
6. `GetByProcessStatusAsync()` - Line 109: `WHERE Process_Status = @status`

**영향 받는 페이지**:
- `/Admin/TotalReport` (Index.razor) - GetAllAsync() 호출

---

## 해결 방안

### 1. ORDER BY 절 수정

**변경 전**:
```sql
ORDER BY Process_Year DESC, Start_Date DESC
```

**변경 후**:
```sql
ORDER BY Pid DESC
```

**사유**: Pid가 최신 프로세스일수록 큰 값이므로 내림차순 정렬로 최신순 조회 가능

### 2. 사용하지 않는 메서드 확인

**제거 또는 수정 필요**:
- `GetByYearAsync()` - Process_Year 컬럼 없음
- `GetByGradeAsync()` - Final_Grade 컬럼 없음
- `GetByProcessStatusAsync()` - Process_Status 컬럼 없음

**대안**:
1. 메서드 제거 (현재 사용하지 않는 경우)
2. 다른 컬럼으로 대체 구현 (사용 중인 경우)

### 3. 다른 Repository 확인

**동일 패턴 확인 필요**:
- `v_ReportTaskListRepository.cs` - 이미 Pid 사용 ✅
- 기타 View Repository들

---

## 진행 사항

- [x] 문제 확인 및 원인 분석
- [x] 작업지시서 작성
- [x] 코드 수정 (8개 수정 완료)
- [x] 빌드 테스트 (오류 0개)
- [x] 런타임 테스트 (/Admin/TotalReport 정상 로드)
- [x] 확인 완료

---

## 관련 문서

**작업지시서**:
- [20260130_01_fix_v_processtrllistdb_column_mismatch.md](../tasks/20260130_01_fix_v_processtrllistdb_column_mismatch.md) (작성 예정)

**관련 이슈**:
- [#011: Phase 3-3 관리자 페이지 빌드 오류 및 재작업](011_phase3_3_admin_pages_build_errors.md) - 같은 Phase 작업
- [#009: Phase 3 Blazor Server WebApp 개발](009_phase3_webapp_development.md) - 상위 이슈

**관련 파일**:
- `MdcHR26Apps.Models/Views/v_ProcessTRListDB/v_ProcessTRListDB.cs` - View Entity
- `MdcHR26Apps.Models/Views/v_ProcessTRListDB/v_ProcessTRListRepository.cs` - Repository 구현
- `MdcHR26Apps.BlazorServer/Components/Pages/Admin/TotalReport/Index.razor.cs` - 사용 페이지

---

## 개발자 피드백

**작업 시작**: 2026-01-30
**작업 완료**: 2026-01-30
**최종 상태**: 완료 ✅
**비고**:
- 2026년 DB 구조와 2025년 코드 기반 작업지시서 간 불일치 발견
- 8개 수정사항 완료:
  - 4개 ORDER BY 수정 (Process_Year DESC, Start_Date DESC → Pid DESC)
  - 1개 ORDER BY 수정 (Final_Score DESC → Total_Score DESC) - 추가 발견
  - 3개 메서드 제거 (GetByYearAsync, GetByGradeAsync, GetByProcessStatusAsync)
- 빌드 테스트 통과 (오류 0개)
- 런타임 테스트 성공: /Admin/TotalReport 페이지 정상 로드, SqlException 없음
