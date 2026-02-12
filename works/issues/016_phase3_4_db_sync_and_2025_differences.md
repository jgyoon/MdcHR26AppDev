# 이슈 #016: Phase 3-4 페이지 작업 - DB 변경사항 미반영 및 2025년 차이점 발견

**날짜**: 2026-02-06
**상태**: 진행중
**우선순위**: 높음
**관련 이슈**: [#009](009_phase3_webapp_development.md), [#015](015_agreement_teamleader_arbitrary_code_generation.md)

---

## 개발자 요청

**배경**:
- Phase 3-4 페이지 작업 진행 중
- Agreement (20260204_02) 완료
- SubAgreement (20260204_03) 완료
- Report (20260204_04) 진행 필요
- DeptObjective (20260204_05) 진행 필요

**발견된 문제**:
1. **DB 변경사항이 반영되지 않은 부분 발견**
   - 일부 코드에서 26년도 DB 구조 변경사항이 제대로 반영되지 않음
   - Entity 필드명 변경 (UserId → Uid, SAid → Sid 등)이 누락된 부분 존재

2. **2025년도 프로그램과 다른 점이 많음**
   - Agreement/SubAgreement 작업 중 예상보다 많은 차이점 발견
   - 임의 코드 작성으로 인한 기능 누락 (이슈 #015 참조)
   - 비즈니스 로직 차이로 인한 디버깅 시간 증가

**요청 사항**:
1. Agreement/SubAgreement 작업에서 발견된 문제점 정리
2. Report/DeptObjective 작업 시 동일한 문제 방지 방안 수립
3. DB 변경사항 체크리스트 작성
4. 2025년도 코드 복사 원칙 재확인

---

## 문제 상황

### 1. Agreement 작업 중 발견된 문제

#### 문제 1-1: 임의 코드 작성 (이슈 #015)
- **증상**: TeamLeader Details 페이지가 25년도와 완전히 다른 구조로 구현됨
- **원인**: 25년도 코드를 복사하지 않고 임의로 새로운 코드 작성
- **해결**: 작업지시서 재작성 후 25년도 코드 기준으로 재구현
- **작업지시서**: 20260204_11_agreement_teamleader_details_fix_approval_workflow.md

#### 문제 1-2: DB 변경사항 미반영
- **증상**: 일부 코드에서 `UserId` (string) → `Uid` (long) 변경이 누락됨
- **원인**: 작업지시서 검토 시 26년도 DB 구조 변경사항을 완전히 반영하지 못함
- **해결**: 코드 리뷰 후 누락된 부분 수정

#### 문제 1-3: Repository 반환 타입 불일치
- **증상**: `UpdateAsync()` 반환 타입이 `bool`에서 `int`로 변경되었으나 일부 코드에서 미반영
- **원인**: 작업지시서에 명시되었으나 실제 코드 작성 시 누락
- **해결**: 전체 코드 검토 후 수정

### 2. SubAgreement 작업 중 발견된 문제

#### 문제 2-1: Entity PK 필드명 변경 누락
- **증상**: `SAid` → `Sid` 변경이 일부 코드에서 누락됨
- **원인**: 작업지시서에서 명시했으나 모든 참조를 빠짐없이 수정하지 못함
- **해결**: 빌드 오류 및 런타임 오류 발생 시 수정

#### 문제 2-2: 25년도와 다른 컴포넌트 사용
- **증상**: 25년도에서 사용한 컴포넌트와 다른 컴포넌트 사용
- **원인**: 작업 전 25년도 코드 분석이 불충분
- **해결**: 25년도 코드를 다시 확인하고 동일한 컴포넌트 사용

### 3. 공통 문제

#### 문제 3-1: 26년도 DB 변경사항 전체 목록 부재
- **증상**: 작업지시서마다 DB 변경사항을 반복 기술
- **원인**: 26년도 전체 DB 변경사항을 정리한 문서 부재
- **필요**: 중앙화된 DB 변경사항 체크리스트

#### 문제 3-2: 25년도 코드 구조 이해 부족
- **증상**: 임의로 코드를 재작성하거나 기능을 단순화
- **원인**: 작업 전 25년도 코드를 충분히 분석하지 않음
- **필요**: 25년도 코드 분석 프로세스 정립

---

## 해결 방안

### 1. 26년도 DB 변경사항 체크리스트 작성

#### Entity 필드명 변경
| Entity | 25년도 PK | 26년도 PK | 비고 |
|--------|----------|----------|------|
| AgreementDb | Aid | Aid | 동일 |
| SubAgreementDb | SAid | Sid | ✅ 변경 |
| DeptObjectiveDb | DOid | DeptObjectiveDbId | ✅ 변경 |
| TasksDb | Tid | Tid | 동일 |
| ProcessDb | Pid | Pid | 동일 |
| UserDb | UserId (string) | Uid (long) | ✅ 타입 변경 |

#### Repository 반환 타입 변경
| 메서드 | 25년도 | 26년도 | 코드 변경 |
|-------|-------|-------|----------|
| UpdateAsync | bool | int | `if (success)` → `if (affectedRows > 0)` |
| DeleteAsync | bool | int | `if (success)` → `if (affectedRows > 0)` |
| AddAsync | Entity | Entity | 동일 |
| GetByIdAsync | Entity | Entity | 동일 |
| GetByAllAsync | List | List | 동일 |

#### 네임스페이스 변경
```csharp
// 25년도 → 26년도
MdcHR25Apps → MdcHR26Apps
BlazorApp → BlazorServer
Pages.Agreement → Components.Pages.Agreement
```

### 2. 25년도 코드 복사 프로세스 정립

#### Step 1: 25년도 코드 분석
1. [ ] 해당 페이지 25년도 .razor 파일 전체 읽기
2. [ ] 해당 페이지 25년도 .razor.cs 파일 전체 읽기
3. [ ] Repository 메서드 사용 패턴 확인
4. [ ] Entity 필드 사용 확인
5. [ ] 비즈니스 로직 이해
6. [ ] 사용 컴포넌트 목록 확인

#### Step 2: 작업지시서 작성
1. [ ] 25년도 코드 경로 명시
2. [ ] 26년도 코드 경로 명시
3. [ ] 26년도 DB 변경사항 명시 (체크리스트 참조)
4. [ ] 변경 예시 코드 작성 (Before/After)

#### Step 3: 파일 복사 및 수정
1. [ ] 25년도에서 26년도로 파일 복사
2. [ ] 네임스페이스 일괄 변경
3. [ ] Entity PK 필드명 변경
4. [ ] UserId → Uid 변경
5. [ ] Repository 반환 타입 처리 변경
6. [ ] 빌드 테스트

#### Step 4: 기능 검증
1. [ ] 25년도 기능과 동일한지 확인
2. [ ] 비즈니스 로직 변경 없음 확인
3. [ ] 컴포넌트 정상 동작 확인

### 3. Report/DeptObjective 작업 전 준비

#### Report 작업 (20260204_04)
- [ ] 25년도 1st_HR_Report 코드 분석
- [ ] 25년도 2nd_HR_Report 코드 분석
- [ ] 25년도 3rd_HR_Report 코드 분석
- [ ] TasksDb, SubAgreementDb 변경사항 확인
- [ ] 작업지시서 검토 (20260204_04)
- [ ] DB 변경사항 체크리스트 반영 확인

#### DeptObjective 작업 (20260204_05)
- [ ] 25년도 DeptObjective 코드 분석
- [ ] DeptObjectiveDb 변경사항 확인 (DOid → DeptObjectiveDbId)
- [ ] 감사 필드 (CreatedBy/At, UpdatedBy/At) 사용 확인
- [ ] 작업지시서 검토 (20260204_05)
- [ ] DB 변경사항 체크리스트 반영 확인

---

## 진행 사항

### Agreement 작업
- [x] Agreement 페이지 구현 (20260204_02)
- [x] Agreement TeamLeader Details 재작성 (20260204_11)
- [x] 이슈 #015 생성 (임의 코드 작성 금지 원칙)

### SubAgreement 작업
- [x] SubAgreement 페이지 구현 (20260204_03)
- [x] DB 변경사항 반영 (SAid → Sid, UserId → Uid)
- [x] SubAgreementDbListTable 합의 완료 기능 추가 (2026-02-12)
  - [x] IsSubAgreementCompleted 파라미터 추가
  - [x] Agreement 패턴 적용 (완료 시 "상세" 버튼 → "완료" 텍스트)
  - [x] 5개 파일 수정 (SubAgreementDbListTable, SubAgreementDbListView, Index)

### Report 작업 - 2026-02-08
- [x] 2nd_HR_Report, 3rd_HR_Report Details 페이지 "No DATA" 문제 해결
- [x] v_ProcessTRListDB.TeamLeader_Score 필드 추가 (DB 뷰, C# 모델)
- [x] 컴포넌트 파라미터 불일치 문제 수정 (7개 컴포넌트)
  - [x] TeamLeaderReportDetailsTable (report → reports)
  - [x] User_TotalReportListView (Uid → TotalReportListDB)
  - [x] CompletedTaskListView (신규 생성)
  - [x] DirectorReportListView (Uid → processTRLists)
  - [x] DirectorReportDetailsTable (report → reports)
  - [x] FeedBack_TotalReportListView (Uid → TotalReportListDB)
  - [x] Complete_TotalReportListView (Uid → TotalReportListDB)
- [x] v_ReportTaskListRepository.GetByTaksListNumberAllAsync 메서드 추가
- [x] Complete_3rd_Edit 세부업무 필터링 수정 (GetByUidAllAsync → GetByTaksListNumberAllAsync)
- [x] UrlActions 메서드 추가 (MoveComplete3rdEditPage)
- [x] 평가 페이지 완성 (1st/2nd/3rd HR Report 기본 기능 완료)

### TotalReport 작업 - 2026-02-08
- [x] TotalReport 페이지 작업지시서 작성 (20260208_01)
- [x] TotalReport 페이지 구현 (8개 파일 생성)
  - [x] TotalReport/Index.razor + .cs (사용자 평가 제출)
  - [x] TotalReport/Result.razor + .cs (평가 결과 조회)
  - [x] TotalReport/TeamLeader/Index.razor + .cs (팀장 목록)
  - [x] TotalReport/TeamLeader/CompleteDetails.razor + .cs (팀장 완료 상세)
- [x] UrlActions 메서드 4개 추가
- [x] 25년도 컴포넌트 복사 (TeamLeader_TotalReportListView)
  - [x] 26년도 컴포넌트가 리팩토링되어 Uid만 받도록 변경됨
  - [x] 25년도 방식(v_TotalReportListDB 전달) 복사
  - [x] View entity 사용 원칙 준수
- [x] 빌드 성공 (오류 0개)

### 개선 작업
- [x] 이슈 #016 생성 (현재 이슈)
- [x] 26년도 DB 변경사항 체크리스트 작성
- [x] 25년도 코드 복사 프로세스 정립
- [ ] Report 작업 전 준비
- [ ] DeptObjective 작업 전 준비

### 빌드 경고 수정 - 2026-02-12
- [x] 빌드 경고 수정 (69개 → 61개)
  - [x] 중복 using 지시문 제거 (3개 파일)
    - SubAgreement/User/Create.razor.cs
    - SubAgreement/User/Delete.razor.cs
    - SubAgreement/User/Edit.razor.cs
  - [x] 사용하지 않는 필드 제거 (2개)
    - NavMenu.razor: `isEvaluationOpen`
    - ExcelManage.cs: `_containerName`
  - [x] 사용하지 않는 매개변수 제거 (1개)
    - TotalReport/TeamLeader/CompleteDetails.razor.cs: `userDbRepository`
- [x] Nullable 참조 경고 작업지시서 작성
  - [x] 작업지시서: 20260212_01_fix_nullable_reference_warnings.md
  - [x] 4가지 수정 방법 분석 및 장단점 비교
  - [x] 2026년 최신 트렌드 검증 (WebSearch)
  - [x] ?? 연산자 중심으로 간소화
  - [x] Microsoft 공식 권장사항 및 커뮤니티 합의 확인
  - [x] 본 프로젝트 적용 방침 확정: `?? new ClassName()` 패턴 사용

---

## 26년도 DB 변경사항 전체 정리

### 1. Entity PK 필드명 변경

```csharp
// SubAgreementDb
public long SAid { get; set; }  // ❌ 25년도
public long Sid { get; set; }   // ✅ 26년도

// DeptObjectiveDb
public long DOid { get; set; }              // ❌ 25년도
public long DeptObjectiveDbId { get; set; } // ✅ 26년도

// UserDb (타입 변경)
public string UserId { get; set; }  // ❌ 25년도
public long Uid { get; set; }       // ✅ 26년도
```

### 2. 관련 필드 변경

```csharp
// 모든 Entity에서 UserId → Uid 변경
public string UserId { get; set; }  // ❌ 25년도
public long Uid { get; set; }       // ✅ 26년도

// LoginStatus에서도 동일
loginUser.LoginUserId  // ❌ 25년도
loginUser.LoginUid     // ✅ 26년도
```

### 3. Repository 반환 타입 변경

```csharp
// 25년도
Task<bool> UpdateAsync(Entity model);
Task<bool> DeleteAsync(long id);

// 26년도
Task<int> UpdateAsync(Entity model);  // 영향받은 행 수 반환
Task<int> DeleteAsync(long id);       // 영향받은 행 수 반환
```

### 4. 사용 패턴 변경

```csharp
// 25년도
bool success = await repository.UpdateAsync(model);
if (success)
{
    // 성공 처리
}

// 26년도
int affectedRows = await repository.UpdateAsync(model);
if (affectedRows > 0)
{
    // 성공 처리
}
```

---

## 테스트 결과

### Agreement 테스트
**결과**: ✅ 성공 (재작성 후)
- User 페이지 (5개): Index, Create, Edit, Delete, Details
- TeamLeader 페이지 (2개): Index, Details
- 25년도 기능과 동일

### SubAgreement 테스트
**결과**: ⏸️ 진행 중
- User 페이지 (5개): Index, Create, Edit, Delete, Details
- TeamLeader 페이지 (5개): Index, Details, SubDetails, CompleteSubAgreement, ResetSubAgreement
- DB 변경사항 반영 확인 필요

---

## 다음 작업

### 우선순위 1: Report 작업 준비
1. [ ] 25년도 1st_HR_Report 코드 분석 (3개 페이지)
2. [ ] 25년도 2nd_HR_Report 코드 분석 (5개 페이지)
3. [ ] 25년도 3rd_HR_Report 코드 분석 (5개 페이지)
4. [ ] 작업지시서 검토 (20260204_04)
5. [ ] Report 작업 진행

### 우선순위 2: DeptObjective 작업 준비
1. [ ] 25년도 DeptObjective 코드 분석 (10개 페이지)
2. [ ] DeptObjectiveDb 변경사항 확인
3. [ ] 작업지시서 검토 (20260204_05)
4. [ ] DeptObjective 작업 진행

---

## 관련 문서

**작업지시서**:
- [20260204_02_phase3_4_agreement_pages.md](../tasks/20260204_02_phase3_4_agreement_pages.md) - Agreement 페이지 (완료)
- [20260204_03_phase3_4_subagreement_pages.md](../tasks/20260204_03_phase3_4_subagreement_pages.md) - SubAgreement 페이지 (완료)
- [20260204_04_phase3_4_report_pages.md](../tasks/20260204_04_phase3_4_report_pages.md) - Report 페이지 (진행 필요)
- [20260204_05_phase3_4_deptobjective_pages.md](../tasks/20260204_05_phase3_4_deptobjective_pages.md) - DeptObjective 페이지 (진행 필요)
- [20260204_11_agreement_teamleader_details_fix_approval_workflow.md](../tasks/20260204_11_agreement_teamleader_details_fix_approval_workflow.md) - Agreement TeamLeader Details 재작성
- [20260208_01_totalreport_pages.md](../tasks/20260208_01_totalreport_pages.md) - TotalReport 페이지 (완료)
- [20260212_01_fix_nullable_reference_warnings.md](../tasks/20260212_01_fix_nullable_reference_warnings.md) - Nullable 참조 경고 수정 (진행 필요)

**관련 이슈**:
- [#009: Phase 3 Blazor Server WebApp 개발](009_phase3_webapp_development.md) - Phase 3-4 진행 중
- [#015: Agreement TeamLeader 페이지 - 임의 코드 작성으로 인한 디버깅 어려움](015_agreement_teamleader_arbitrary_code_generation.md)

**참조 프로젝트**:
- 25년도 프로젝트: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp`
- 26년도 프로젝트: `c:\Codes\00_Develop_Cursor\10_MdcHR26Apps\MdcHR26Apps.BlazorServer`

---

## 개발자 피드백

**작업 시작**: 2026-02-06
**현재 상태**: 진행중
**비고**:
- Agreement/SubAgreement 작업에서 예상보다 많은 문제 발견
- DB 변경사항 체크리스트 작성으로 Report/DeptObjective 작업 시 동일한 문제 방지
- 25년도 코드 복사 프로세스 정립으로 작업 효율성 향상 기대

---

## 요약

**문제**: Agreement/SubAgreement 작업 중 DB 변경사항 미반영 및 2025년 차이점 많음
**원인**: 26년도 DB 변경사항 체크리스트 부재, 25년도 코드 분석 불충분
**해결**: DB 변경사항 체크리스트 작성, 25년도 코드 복사 프로세스 정립
**다음 작업**: Report/DeptObjective 작업 전 준비 (25년도 코드 분석, 작업지시서 검토)