# 이슈 #009: Phase 3 - Blazor Server WebApp 개발

**날짜**: 2026-01-20
**상태**: 진행중
**우선순위**: 높음
**관련 이슈**: [#008](008_phase2_model_development.md), [#004](004_phase1_database_design.md)

---

## 개발자 요청

**배경**:
- Phase 1 완료: 데이터베이스 설계 및 구축 완료 (12 테이블, 5 뷰)
- Phase 2 완료: Model 계층 개발 완료 (55 파일, 147 메서드)
- 다음 단계: 사용자 인터페이스 개발 필요

**요청 사항**:
1. 2025년 인사평가 BlazorApp 구조 참조
2. 도서관리 프로젝트의 최신 기술 적용 (.NET 10.0)
3. Blazor Server 기반 실시간 웹 애플리케이션 구축
4. 3단계 평가 프로세스 구현 (본인 → 부서장 → 임원)
5. 역할 기반 메뉴 및 접근 제어
6. 엑셀 내보내기 기능

---

## 해결 방안

### 1. 참조 프로젝트 분석

#### 2025년 인사평가 프로젝트 (비즈니스 로직)
- **경로**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp`
- **분석 완료**: ✅
- **주요 참조 사항**:
  - Blazor Server 아키텍처
  - 3단계 평가 프로세스
  - 역할 기반 메뉴 (관리자, 부서장, 임원)
  - 상태 관리 서비스 (LoginStatusService, AppState)
  - 모달 컴포넌트 패턴
  - 엑셀 내보내기 (ClosedXML)
  - 검색/필터 컴포넌트

#### 도서관리 프로젝트 (최신 기술)
- **경로**: `C:\Codes\36_MdcLibrary\MdcLibrary\MdcLibrary.Server`
- **분석 완료**: ✅
- **주요 참조 사항**:
  - .NET 8.0 최신 기능 (.NET 10.0 업그레이드)
  - InteractiveServer 렌더 모드
  - Enhanced Navigation
  - 반응형 UI (Bootstrap)
  - 구조화된 로깅
  - 보안 강화 (Rate Limiting, CSRF)

### 2. 프로젝트 구조 설계

```
MdcHR26Apps.BlazorServer/
├── Components/
│   ├── Layout/ (MainLayout, NavMenu)
│   ├── Pages/
│   │   ├── Auth/ (Login, Logout, Manage)
│   │   ├── Admin/ (사용자, 부서, 평가관리)
│   │   ├── Agreement/ (직무평가)
│   │   ├── 1st_HR_Report/ (본인평가)
│   │   ├── 2nd_HR_Report/ (부서장평가)
│   │   ├── 3rd_HR_Report/ (임원평가)
│   │   └── TotalReport/ (결과리포트)
│   └── Components/ (재사용 컴포넌트)
├── Data/ (상태 관리)
├── Utils/ (유틸리티)
├── Models/ (로컬 모델)
└── wwwroot/ (정적 파일)
```

### 3. 기술 스택

- **프레임워크**: .NET 10.0
- **C# 버전**: C# 13
- **Blazor**: Blazor Server (SignalR)
- **ORM**: Dapper + EF Core (Phase 2 연동)
- **UI**: Bootstrap 5.x
- **엑셀**: ClosedXML
- **인증**: Custom Auth (SHA-256 + Salt)

### 4. 핵심 구현 사항

#### Blazor Server 설정
```csharp
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options => { /* ... */ })
    .AddHubOptions(options => { /* SignalR 최적화 */ });
```

#### 상태 관리
- LoginStatusService (로그인 상태)
- AppStateService (전역 상태)
- UrlActions (네비게이션)

#### 역할 기반 접근 제어
- 관리자 (Administrator)
- 부서장/팀장 (TeamLeader)
- 임원 (Director)
- 일반 사용자

---

## 진행 사항

### Phase 3-1: 프로젝트 생성 및 기본 설정 ✅
- [x] Blazor Server 프로젝트 생성 (.NET 10.0)
- [x] 프로젝트 참조 추가 (MdcHR26Apps.Models)
- [x] NuGet 패키지 설치
- [x] Program.cs DI 설정
- [x] appsettings.json 환경 설정
- [x] 기본 레이아웃 (MainLayout, NavMenu)
- [x] 상태 관리 서비스
- [x] 빌드 및 실행 확인
- [x] .NET 10 최신 기능 적용 (@Assets[], ResourcePreloader, ImportMap, ReconnectModal)
- [x] Playwright 테스트 환경 구축 (Chromium 브라우저, 4개 테스트)
- [x] test-runner Agent 생성 (자동 테스트 실행)

### Phase 3-2: 로그인 및 인증 ✅
- [x] Login.razor 구현
- [x] Logout.razor 구현
- [x] Manage.razor (비밀번호 변경)
- [x] SHA-256 + Salt 로그인 연동
- [x] 세션 관리

### Phase 3-3: 관리자 페이지 ✅
- [x] 사용자 관리 (CRUD) - Users/
- [x] 부서 관리 (CRUD) - Settings/Depts/
- [x] 직급 관리 (CRUD) - Settings/Ranks/
- [x] 평가대상자 관리 - EvaluationUsers/
- [x] 최종결과 관리 (TotalReport) - TotalReport/

### Phase 3-4: 평가 프로세스
- [ ] 직무평가 협의
- [ ] 세부직무평가
- [ ] 본인평가 (1차)
- [ ] 부서장평가 (2차)
- [ ] 임원평가 (3차)
- [ ] 최종 결과 리포트

### Phase 3-5: 공통 컴포넌트 ✅
- [x] SearchbarComponent
- [x] Modal 컴포넌트 (UserDeleteModal, ReportInitModal)
- [x] Table 컴포넌트 (UserListTable, EUserListTable, MemberListTable, AdminReportListView)
- [x] DisplayResultText

### Phase 3-6: 엑셀 및 유틸리티 ✅
- [x] ExcelManage (엑셀 내보내기)
- [x] AdminViewExcel, AdminTaskViewExcel
- [x] UserUtils (사용자 유틸)
- [x] ScoreUtils (점수 계산)
- [x] TotalScoreRankModel

### 테스트 단계
- [ ] 로그인 테스트
- [ ] 권한 테스트
- [ ] 평가 프로세스 테스트
- [ ] 반응형 UI 테스트
- [ ] 엑셀 다운로드 테스트

---

## 개발 대상 목록

### 페이지 (약 40개)

| 카테고리 | 페이지 | 우선순위 |
|---------|--------|----------|
| **기본** | Home, Error | 🔴 High |
| **인증** | Login, Logout, Manage | 🔴 High |
| **관리자** | Users, Depts, EvaluationUsers, Index | 🟡 Medium |
| **직무평가** | User, TeamLeader (Agreement, SubAgreement) | 🟡 Medium |
| **평가** | 1st, 2nd, 3rd_HR_Report | 🔴 High |
| **목표** | DeptObjective | 🟡 Medium |
| **결과** | TotalReport | 🔴 High |

### 컴포넌트 (약 20개)

| 카테고리 | 컴포넌트 | 우선순위 |
|---------|---------|----------|
| **Common** | LoadingIndicator, DisplayResultText, Searchbar | 🔴 High |
| **Form** | FormSelectList, FormTaskItem, FormAgreeTask | 🟡 Medium |
| **Modal** | DeleteModal, ConfirmModal | 🟡 Medium |
| **Table** | UserListTable, ProcessListTable, ReportListTable | 🟡 Medium |

### 상태 관리 및 서비스 (8개)

| 파일 | 용도 | 우선순위 |
|------|------|----------|
| LoginStatus.cs | 로그인 상태 모델 | 🔴 High |
| LoginStatusService.cs | 로그인 관리 | 🔴 High |
| AppStateService.cs | 전역 상태 | 🔴 High |
| UrlActions.cs | 네비게이션 | 🔴 High |
| ExcelManage.cs | 엑셀 처리 | 🟢 Low |
| UserUtils.cs | 사용자 유틸 | 🟡 Medium |
| ScoreUtils.cs | 점수 계산 | 🟡 Medium |
| AppSettings.cs | 설정 모델 | 🔴 High |

---

## 개발 우선순위

### 🔴 High Priority (Phase 3-1, 3-2)
1. 프로젝트 생성 및 기본 설정
2. Program.cs, appsettings.json
3. MainLayout, NavMenu
4. Login, Logout, Manage
5. LoginStatusService, AppStateService
6. Home, Error 페이지

### 🟡 Medium Priority (Phase 3-3, 3-4)
7. 관리자 페이지 (Users, Depts)
8. 직무평가 협의 (Agreement, SubAgreement)
9. 평가 프로세스 (1st, 2nd, 3rd)
10. 부서 목표 관리

### 🟢 Low Priority (Phase 3-5, 3-6)
11. 엑셀 내보내기
12. 공통 컴포넌트
13. 유틸리티 클래스

---

## 기술적 개선 사항

### 2025년 프로젝트 대비 개선

| 항목 | 2025년 | 2026년 (Phase 3) |
|------|--------|------------------|
| **.NET 버전** | .NET 7.0 | **.NET 10.0** ⭐ |
| **Blazor** | Blazor Server | **Blazor Server + Enhanced Navigation** ⭐ |
| **렌더 모드** | 기본 | **InteractiveServer** ⭐ |
| **보안** | 기본 인증 | **Rate Limiting + CSRF** ⭐ |
| **로깅** | 기본 | **구조화된 로깅** ⭐ |
| **UI** | Bootstrap 5 | **반응형 UI (모바일/데스크톱)** ⭐ |
| **상태 관리** | Scoped 서비스 | **Event 기반 상태 관리** ⭐ |

### 도서관리 프로젝트에서 적용

1. ✅ **.NET 10.0 최신 기능**
2. ✅ **InteractiveServer 렌더 모드**
3. ✅ **반응형 UI 패턴** (모바일/데스크톱)
4. ✅ **에러 핸들링** (Global Error UI)
5. ✅ **구조화된 로깅**

---

## 관련 문서

**작업지시서**:
- [20260120_01_phase3_blazor_webapp.md](../tasks/20260120_01_phase3_blazor_webapp.md) - Phase 3 전체 계획
- [20260120_02_phase3_1_project_setup.md](../tasks/20260120_02_phase3_1_project_setup.md) - Phase 3-1 프로젝트 생성 ✅
- [20260203_05_components_agreement.md](../tasks/20260203_05_components_agreement.md) - Agreement 컴포넌트 (6개) 📝
- [20260203_06_components_subagreement.md](../tasks/20260203_06_components_subagreement.md) - SubAgreement 컴포넌트 (8개) 📝
- [20260203_07_components_report.md](../tasks/20260203_07_components_report.md) - Report 컴포넌트 (17개) 📝
- [20260203_08_components_common_form.md](../tasks/20260203_08_components_common_form.md) - Common/Form 컴포넌트 (9개) 📝

**선행 이슈**:
- [#008: Phase 2 Model 개발](008_phase2_model_development.md)
- [#004: Phase 1 데이터베이스 설계](004_phase1_database_design.md)

**참조 프로젝트**:
- 2025년 인사평가: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp`
- 도서관리: `C:\Codes\36_MdcLibrary\MdcLibrary\MdcLibrary.Server`

---

## 개발자 확인 사항

### 질문 사항
1. Phase 3-1부터 단계적으로 진행할까요?
2. 모바일 반응형 UI를 적용할까요?
3. 엑셀 내보내기는 어느 시점에 구현할까요?
4. Rate Limiting을 적용할까요?

### 승인 필요 사항
1. ✅ **.NET 10.0 사용**
2. ✅ **Blazor Server + InteractiveServer 렌더 모드**
3. ✅ **Phase 2 Model 계층 연동**
4. ✅ **Bootstrap 5.x 반응형 UI**
5. ✅ **SHA-256 + Salt 로그인**
6. ✅ **ClosedXML 엑셀 처리**
7. ✅ **역할 기반 접근 제어**
8. ✅ **3단계 평가 프로세스**

---

## 개발자 피드백

**작업 시작**: 2026-01-20
**작업지시서 작성**: 2026-01-20 (2개)
**Phase 3-1 완료**: 2026-01-20 ✅
**Phase 3-2 완료**: 2026-01-21 ✅
**Phase 3-3 완료**: 2026-01-30 ✅
**현재 상태**: Phase 3-4 평가 프로세스 준비 중

**완료 내역**:
1. ✅ Blazor Server 프로젝트 생성 및 기본 설정
2. ✅ .NET 10 최신 기능 적용
3. ✅ Playwright 자동 테스트 환경 구축
4. ✅ 로그인 및 인증 시스템 (Login, Logout, Manage)
5. ✅ 관리자 페이지 전체 (Users, Settings, EvaluationUsers, TotalReport)
6. ✅ 공통 컴포넌트 및 유틸리티
7. ✅ 엑셀 다운로드 기능
8. ✅ DB View 동기화 (v_ProcessTRListDB, v_ReportTaskListDB 등)

**Phase 3-4 시작 전 검증**: 2026-02-03 ✅
- ✅ **전체 View Table vs Entity 구조 검증 완료**
- 총 6개 View 검증 (모두 일치)
  1. ✅ v_DeptObjectiveListDb (6개 필드) - 일치
  2. ✅ v_MemberListDB (11개 필드) - 일치
  3. ✅ v_TotalReportListDB (25개 필드) - 일치
  4. ✅ v_EvaluationUsersList (14개 필드) - 일치
  5. ✅ v_ProcessTRListDB (38개 필드) - 수정 완료 (20260130_01)
  6. ✅ v_ReportTaskListDB (29개 필드) - 수정 완료 (20260130_02)
- **결론**: Phase 3-4 진행 안전성 확보

**Phase 3-4 컴포넌트 구현 계획**: 2026-02-03
- **접근 방식 변경**: 2025년 프로젝트의 모든 컴포넌트를 먼저 구현한 후 Phase 3-4 페이지 작업 진행
- **2025년 컴포넌트 분석**:
  - 전체: 51개 컴포넌트
  - 이미 구현: 11개 (SearchbarComponent, DisplayResultText, UserListTable 등)
  - 신규 구현 필요: 40개 컴포넌트
- **작업지시서 작성 완료** (기능별 분할):
  1. [20260203_05_components_agreement.md](../tasks/20260203_05_components_agreement.md) - 6개 Agreement 컴포넌트
  2. [20260203_06_components_subagreement.md](../tasks/20260203_06_components_subagreement.md) - 8개 SubAgreement 컴포넌트
  3. [20260203_07_components_report.md](../tasks/20260203_07_components_report.md) - 17개 Report 컴포넌트
  4. [20260203_08_components_common_form.md](../tasks/20260203_08_components_common_form.md) - 9개 Common/Form 컴포넌트
- **2026 DB 구조 적응**: AgreementDb 필드명 변경사항 반영 (Item_Number, Item_Title, Item_Contents, Item_Proportion)

**DB/Entity 필드명 동기화**: 2026-02-03 ✅
- ✅ **5개 Entity 필드명 수정 완료** (DB 테이블 기준)
  1. ✅ AgreementDb.cs - Agreement_Item_* → Report_Item_* 변경, DB에 없는 필드 4개 삭제
  2. ✅ SubAgreementDb.cs - PK SAid→Sid 변경, Report_Item_*, Report_SubItem_*, Task_Number 추가
  3. ✅ DeptObjectiveDb.cs - PK DOid→DeptObjectiveDbId 변경, 감사 필드(CreatedBy/At, UpdatedBy/At) 추가
  4. ✅ EvaluationLists.cs - PK ELid→Eid 변경, Evaluation_Department/Index/Task 필드 구조 변경
  5. ✅ TasksDb.cs - 전체 재구조화 (TaskName, TaksListNumber, TaskStatus, TaskObjective 등)
- ✅ **빌드 검증 완료** (MdcHR26Apps.Models) - 경고 0개, 오류 0개
- **작업지시서**: [20260203_11_fix_entity_db_field_names.md](../tasks/20260203_11_fix_entity_db_field_names.md)

**Repository 수정 (25년 메서드 기준, 26년 Entity 구조 적용)**: 2026-02-03 ✅
- ✅ **5개 Repository + 5개 Interface 작성 완료**
  1. ✅ AgreementRepository.cs (11→7개 메서드) - GetCountByUidAsync 등 26년 전용 메서드 5개 제거
  2. ✅ SubAgreementRepository.cs (12→7개 메서드) - GetCountByUidAsync 등 26년 전용 메서드 5개 제거
  3. ✅ DeptObjectiveRepository.cs (5개 메서드) - 25년에 없음, 26년 신규 테이블, 기본 CRUD만 구현
  4. ✅ EvaluationListsRepository.cs (8→9개 메서드) - SelectListModel 반환 메서드 포함
  5. ✅ TasksRepository.cs (10→7개 메서드 + 26년 요구 2개) - 25년 기본 7개 + GetCountByUserAsync, DeleteAllByUserAsync (26년 BlazorServer에서 사용)
- ✅ **2개 Blazor 페이지 수정 완료**
  - ReportInit.razor.cs: GetCountByUidAsync → GetByUserIdAllAsync().Count, DeleteAllByUidAsync → foreach DeleteAsync
  - Details.razor.cs: 동일한 방식으로 수정
- ✅ **빌드 검증 완료**
  - MdcHR26Apps.Models: 경고 0개, 오류 0개
  - MdcHR26Apps.BlazorServer: 경고 3개, 오류 0개
- **작업지시서**: [20260203_12_fix_repository_based_on_2025.md](../tasks/20260203_12_fix_repository_based_on_2025.md)
- **핵심 변경 사항**:
  - 25년 Repository 메서드 기준 (개수, 이름, 시그니처)
  - 26년 Entity 필드명에 맞춰 SQL 쿼리 수정 (Uid, Report_Item_*)
  - SelectListModel 속성 변경 (SelectListNumber/Name → Value/Text)
- **Git Commit**: `5e784db` - 21개 파일 변경, +3,029/-1,552줄

**Repository 메서드 변경 사항 요약**:
| Repository | 25년 메서드 | 26년 메서드 (수정 전) | 최종 메서드 | 비고 |
|-----------|------------|-------------------|-----------|------|
| AgreementRepository | 7개 | 11개 | 7개 | GetCountByUidAsync 등 4개 제거 |
| SubAgreementRepository | 7개 | 12개 | 7개 | GetCountByUidAsync 등 5개 제거 |
| DeptObjectiveRepository | - | 10개 | 5개 | 26년 신규, 기본 CRUD만 유지 |
| EvaluationListsRepository | 9개 | 8개 | 9개 | SelectListModel 메서드 추가 |
| TasksRepository | 7개 | 10개 | 9개 | 25년 7개 + 26년 요구 2개 유지 |

**추후 작업시 주의사항**:
1. **Repository 메서드 호출 시**:
   - `GetCountByUidAsync()` → `GetByUserIdAllAsync().Count` 사용
   - `DeleteAllByUidAsync()` → `foreach` + `DeleteAsync()` 사용
   - 파라미터 타입: `string userId` → `long uid` 변경됨

2. **SelectListModel 사용 시**:
   - 속성명 변경: `SelectListNumber` → `Value`, `SelectListName` → `Text`
   - Value는 string 타입이므로 `.ToString()` 필요

3. **SQL 쿼리 작성 시**:
   - `UserId` (string) → `Uid` (long) 사용
   - `Agreement_Item_*` → `Report_Item_*` 사용
   - `SAid` → `Sid`, `DOid` → `DeptObjectiveDbId`, `ELid` → `Eid`

4. **25년 참조 프로젝트**:
   - 경로: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.Models\`
   - 메서드 구조 확인 시 참조 가능

**다음 단계**:
1. ⚠️ **작업지시서 재작성 필요** (Entity/Repository 변경으로 기존 05-08 폐기)
   - 재작성 가이드: [20260203_13_REWRITE_GUIDE.md](../tasks/20260203_13_REWRITE_GUIDE.md)
   - 신규 작업지시서: 13 (Agreement), 14 (SubAgreement), 15 (Report), 16 (Common/Form)

2. Phase 3-4 컴포넌트 구현 (40개)
   - Agreement 관련 (6개): AgreementDbListTable, AgreementDetailsTable, AgreementListTable, AgreementDbListView, AgreementDeleteModal, AgreementComment
   - SubAgreement 관련 (8개): SubAgreementDbListTable, SubAgreementDetailsTable, SubAgreementListTable, SubAgreementResetList, SubAgreementDbListView, SubAgreementDeleteModal, AgreeItemLists, ReportTaskListCommonView
   - Report 관련 (17개): ReportListTable, TeamLeaderReportDetailsTable, DirectorReportDetailsTable, 각종 ViewPage 컴포넌트, Modal, Excel 다운로드
   - Common/Form (9개): CheckboxComponent, FormAgreeTask, FormAgreeTaskCreate, FormGroup, FormSelectList, FormSelectNumber, FormTaskItem, ObjectiveListTable, EDeptListTable

3. 컴포넌트 구현 완료 후 Phase 3-4 페이지 작업 재작성
   - 직무평가 협의 (Agreement, SubAgreement)
   - 본인평가 (1st_HR_Report)
   - 부서장평가 (2nd_HR_Report)
   - 임원평가 (3rd_HR_Report)
   - 부서 목표 관리 (DeptObjective)
