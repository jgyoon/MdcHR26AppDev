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

### Phase 3-2: 로그인 및 인증
- [ ] Login.razor 구현
- [ ] Logout.razor 구현
- [ ] Manage.razor (비밀번호 변경)
- [ ] SHA-256 + Salt 로그인 연동
- [ ] 세션 관리

### Phase 3-3: 관리자 페이지
- [ ] 사용자 관리 (CRUD)
- [ ] 부서 관리 (CRUD)
- [ ] 평가대상자 관리
- [ ] 평가 관리 (평가 개시/종료)

### Phase 3-4: 평가 프로세스
- [ ] 직무평가 협의
- [ ] 세부직무평가
- [ ] 본인평가 (1차)
- [ ] 부서장평가 (2차)
- [ ] 임원평가 (3차)
- [ ] 최종 결과 리포트

### Phase 3-5: 공통 컴포넌트
- [ ] LoadingIndicator
- [ ] SearchbarComponent
- [ ] Modal 컴포넌트
- [ ] Table 컴포넌트
- [ ] Form 컴포넌트

### Phase 3-6: 엑셀 및 유틸리티
- [ ] ExcelManage (엑셀 내보내기)
- [ ] UserUtils (사용자 유틸)
- [ ] ScoreUtils (점수 계산)

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
**현재 상태**: Phase 3-1 완료, Phase 3-2 준비 중

**완료 내역**:
1. ✅ Blazor Server 프로젝트 생성 및 기본 설정
2. ✅ .NET 10 최신 기능 적용 (프로덕션 정렬)
3. ✅ Playwright 자동 테스트 환경 구축
4. ✅ test-runner Agent 생성

**다음 단계**:
1. Phase 3-2: 로그인 및 인증 시스템 구현
2. Login.razor, Logout.razor, Manage.razor 개발
