# 이슈 인덱스

**최종 업데이트**: 2026-02-08

---

## 이슈 현황 통계

- **총 이슈**: 17개 (참조 이슈 2개 포함)
- **프로젝트 이슈**: 15개 (#003부터 시작)
- **완료**: 14개
- **진행 중**: 2개 (#009, #016)
- **보류**: 0개

---

## 진행 중인 이슈

| 번호 | 제목 | 시작일 | 진행률 | 현재 단계 |
|------|------|--------|--------|----------|
| [#009](issues/009_phase3_webapp_development.md) | Phase 3 - Blazor Server WebApp 개발 | 2026-01-20 | 100% (Phase 3-4 완료) | Phase 3-1/2/3/4 완료, 전체 44개 페이지 구현 완료 (2026-02-08) |
| [#016](issues/016_phase3_4_db_sync_and_2025_differences.md) | Phase 3-4 DB 변경사항 미반영 및 2025년 차이점 발견 | 2026-02-06 | 90% | v_ProcessTRListDB.TeamLeader_Score 추가, 7개 컴포넌트 수정, TotalReport 페이지 완료, 25년도 컴포넌트 복사 완료 (2026-02-08) |

---

## 최근 완료된 이슈 (최근 5개)

| 번호 | 제목 | 완료일 | 관련 작업지시서 |
|------|------|--------|----------------|
| [#015](issues/015_agreement_teamleader_arbitrary_code_generation.md) | Agreement TeamLeader 페이지 - 임의 코드 작성으로 인한 디버깅 어려움 | 2026-02-05 | 20260204_11 |
| [#011](issues/011_phase3_3_admin_pages_build_errors.md) | Phase 3-3 관리자 페이지 빌드 오류 및 재작업 | 2026-01-30 | 20260126_01~20260129_06 (6개) |
| [#014](issues/014_site_js_not_loaded_app_razor.md) | site.js 파일이 App.razor에 로드되지 않음 | 2026-01-30 | - |
| [#013](issues/013_v_reporttasklistdb_entity_db_mismatch.md) | v_ReportTaskListDB Entity와 DB View 구조 불일치 | 2026-01-30 | 20260130_02 |
| [#012](issues/012_v_processtrllistdb_view_column_mismatch.md) | v_ProcessTRListDB View 컬럼 불일치 오류 | 2026-01-30 | 20260130_01 |

---

## 전체 이슈 목록

<details>
<summary>전체 이슈 보기 (12개)</summary>

### 참조 이슈 (다른 프로젝트)
- [#001](issues/001_checkup_mode_alimtalk_fix.md) - 알림톡 발송 시 검진 모드 시간 반영 (참조, 2025-11-10)
- [#002](issues/002_bulk_progressbar_full_width.md) - 대량 문자 전송 프로그레스바 전체 너비 표시 (참조, 2025-11-12)

### 프로젝트 설계
- [#003](issues/003_project_roadmap.md) - 2026년 인사평가프로그램 개발 로드맵 (완료, 2025-12-16)
- [#004](issues/004_phase1_database_design.md) - Phase 1 데이터베이스 설계 및 구축 (완료, 2025-12-16)
- [#005](issues/005_phase1_progress_summary.md) - Phase 1 작업 완료 보고서 (완료, 2025-12-16)

### 보안 및 구조 개선
- [#006](issues/006_enhance_password_security.md) - UserDb 비밀번호 보안 강화 (완료, 2026-01-14)
- [#007](issues/007_remove_memberdb_optimize_structure.md) - MemberDb 제거 및 부서 목표 권한 관리 최적화 (완료, 2026-01-16)

### Model 개발
- [#008](issues/008_phase2_model_development.md) - Phase 2 Model 개발 (Dapper) (완료, 2026-01-20)

### WebApp 개발
- [#009](issues/009_phase3_webapp_development.md) - Phase 3 Blazor Server WebApp 개발 (진행중, 2026-01-20~)
- [#010](issues/010_login_password_hash_order_mismatch.md) - 로그인 비밀번호 검증 실패 - 해시 순서 및 인코딩 불일치 (완료, 2026-01-22)
- [#011](issues/011_phase3_3_admin_pages_build_errors.md) - Phase 3-3 관리자 페이지 빌드 오류 및 재작업 (완료, 2026-01-30)
- [#012](issues/012_v_processtrllistdb_view_column_mismatch.md) - v_ProcessTRListDB View 컬럼 불일치 오류 (완료, 2026-01-30)
- [#013](issues/013_v_reporttasklistdb_entity_db_mismatch.md) - v_ReportTaskListDB Entity와 DB View 구조 불일치 (완료, 2026-01-30)
- [#014](issues/014_site_js_not_loaded_app_razor.md) - site.js 파일이 App.razor에 로드되지 않음 (완료, 2026-01-30)

</details>

---

## 이슈 간 관계도

```
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  본 프로젝트 이슈 (2026년 인사평가프로그램)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

[#003] 프로젝트 로드맵 (완료) ← 프로젝트 시작
    ↓
[#004] Phase 1 DB 설계 (완료)
    ↓
[#005] Phase 1 완료 보고서 (완료)
    ↓
[#006] 비밀번호 보안 강화 (완료)
    ↓
[#007] MemberDb 제거 및 최적화 (완료)
    ↓
[#008] Phase 2 Model 개발 (완료 ✅)
    ├─ Phase 2-1: 프로젝트 생성 및 기본 모델 (완료 ✅)
    ├─ Phase 2-2: 평가 핵심 모델 (완료 ✅)
    ├─ Phase 2-3: 목표/협의/업무 모델 (완료 ✅)
    └─ Phase 2-4: View 모델 (완료 ✅)
    ↓
[#009] Phase 3 Blazor Server WebApp 개발 (진행중 🔄)
    ├─ Phase 3-1: 프로젝트 생성 및 기본 설정 (완료 ✅)
    │   ├─ Blazor Server 프로젝트 생성
    │   ├─ .NET 10 최신 기능 적용
    │   ├─ Playwright 테스트 환경
    │   └─ test-runner Agent
    ├─ Phase 3-2: 로그인 및 인증 (완료 ✅)
    │   ├─ 로그인 페이지 UI (Login.razor)
    │   ├─ 로그아웃 기능 (Logout.razor)
    │   ├─ 비밀번호 변경 (Manage.razor)
    │   ├─ SHA-256 + Salt 로그인 연동
    │   └─ LoginStatusService (상태 관리)
    ├─ Phase 3-3: 관리자 페이지 (완료 ✅) - [#011]
    │   ├─ ✅ Users 관리 (Create, Edit, Delete, Details)
    │   ├─ ✅ Settings/Depts 관리 (Create, Edit, Delete, Details)
    │   ├─ ✅ Settings/Ranks 관리 (Create, Edit, Delete, Details)
    │   ├─ ✅ EvaluationUsers 관리 (Edit, Details)
    │   ├─ ✅ TotalReport (Index, Details, Edit, ReportInit)
    │   ├─ ✅ 공용 컴포넌트 8개
    │   ├─ ✅ 엑셀 다운로드 (AdminViewExcel, AdminTaskViewExcel)
    │   └─ ✅ DB View 동기화 (v_ProcessTRListDB, v_TotalReportListDB 등)
    ├─ 🔍 Phase 3-4 시작 전 검증 (완료 ✅ - 2026-02-03)
    │   ├─ ✅ 전체 6개 View 구조 검증 (Entity vs DB View)
    │   ├─ ✅ v_DeptObjectiveListDb (6개 필드)
    │   ├─ ✅ v_MemberListDB (11개 필드)
    │   ├─ ✅ v_TotalReportListDB (25개 필드)
    │   ├─ ✅ v_EvaluationUsersList (14개 필드)
    │   ├─ ✅ v_ProcessTRListDB (38개 필드) - 20260130_01
    │   └─ ✅ v_ReportTaskListDB (29개 필드) - 20260130_02
    ├─ 📝 Phase 3-4 컴포넌트 작업지시서 작성 (완료 ✅ - 2026-02-03)
    │   ├─ ✅ 2025년 프로젝트 컴포넌트 분석 (51개)
    │   ├─ ✅ 신규 구현 필요 컴포넌트 확인 (40개)
    │   ├─ ❌ 20260203_05_components_agreement.md (6개) - 폐기
    │   ├─ ❌ 20260203_06_components_subagreement.md (8개) - 폐기
    │   ├─ ❌ 20260203_07_components_report.md (17개) - 폐기
    │   ├─ ❌ 20260203_08_components_common_form.md (9개) - 폐기
    │   ├─ ✅ 20260203_13_components_agreement_v2.md (6개) - v2 재작성
    │   ├─ ✅ 20260203_14_components_subagreement_v2.md (8개) - v2 재작성
    │   ├─ ✅ 20260203_15_components_report_v2.md (15개) - v2 재작성
    │   └─ ✅ 20260203_16_components_common_form_v2.md (9개) - v2 재작성
    ├─ Phase 3-4: 컴포넌트 구현 (완료 ✅)
    │   ├─ ✅ Agreement 컴포넌트 (6개, 12 files) - 완료 (2026-02-04)
    │   ├─ ✅ SubAgreement 컴포넌트 (8개, 16 files) - 완료 (2026-02-04)
    │   ├─ ✅ Report 컴포넌트 (15개, 30 files) - 완료 (2026-02-04)
    │   │   ├─ Table 그룹 (9개): ReportListTable, TeamLeaderReportDetailsTable 등
    │   │   ├─ Modal 그룹 (3개): ReportDeleteModal, SubReportDeleteModal 등
    │   │   └─ ViewPage 그룹 (3개): ReportViewPage, Report2ViewPage 등
    │   └─ ✅ Common/Form 컴포넌트 (9개, 17 files) - 완료 (2026-02-04)
    │       ├─ Common 그룹 (3개, 5 files): CheckboxComponent, ObjectiveListTable, EDeptListTable
    │       └─ Form 그룹 (6개, 12 files): FormAgreeTask, FormGroup, FormSelectList 등
    ├─ Phase 3-4: 평가 프로세스 페이지 (완료 ✅)
    │   ├─ ✅ 직무평가 협의 (Agreement 7개, SubAgreement 10개) - 2026-02-05
    │   │   ├─ Agreement/User (5개): Index, Create, Edit, Delete, Details
    │   │   ├─ Agreement/TeamLeader (2개): Index, Details
    │   │   ├─ SubAgreement/User (5개): Index, Create, Edit, Delete, Details
    │   │   └─ SubAgreement/TeamLeader (5개): Index, Details, SubDetails, CompleteSubAgreement, ResetSubAgreement
    │   ├─ ✅ 본인평가 (1st_HR_Report 3개 페이지) - 이전 세션
    │   │   └─ Index, Edit, Details
    │   ├─ ✅ 부서장평가 (2nd_HR_Report 5개 페이지) - 이전 세션
    │   │   └─ Index, Edit, Details, Complete_2nd_Edit, Complete_2nd_Details
    │   ├─ ✅ 임원평가 (3rd_HR_Report 5개 페이지) - 이전 세션
    │   │   └─ Index, Edit, Details, Complete_3rd_Edit, Complete_3rd_Details
    │   ├─ ✅ 부서 목표 관리 (DeptObjective 10개 페이지) - 2026-02-08
    │   │   ├─ 목록 페이지 (2개): Main, Sub
    │   │   ├─ MainObjective CRUD (4개): Create, Edit, Delete, Details
    │   │   ├─ SubObjective CRUD (4개): Create, Edit, Delete, Details
    │   │   ├─ Start_Date/End_Date 제거 (26년도 DB에 없음)
    │   │   ├─ GetByDateRangeAsync 메서드 제거
    │   │   ├─ ObjectiveType 필드 추가 ("Main"/"Sub")
    │   │   └─ IsDeptObjectiveWriter 권한 적용
    │   └─ ✅ 결과 리포트 (TotalReport 4개 페이지) - 2026-02-08
    │
    │   **Phase 3-4 완료**: 44개 페이지 (88 files) ✅
    ├─ Phase 3-5: 공통 컴포넌트 (완료 ✅)
    │   ├─ SearchbarComponent
    │   ├─ Modal 컴포넌트 (UserDeleteModal, ReportInitModal)
    │   ├─ Table 컴포넌트 (UserListTable, EUserListTable, MemberListTable, AdminReportListView)
    │   └─ DisplayResultText
    └─ Phase 3-6: 엑셀 및 유틸리티 (완료 ✅)
        ├─ ExcelManage
        ├─ AdminViewExcel, AdminTaskViewExcel
        ├─ UserUtils
        ├─ ScoreUtils
        └─ TotalScoreRankModel
    ↓
[Phase 3-4 작업 중 발견된 문제점 및 개선사항] (2026-02-05)
    ├─ 문제 1: 임의 코드 작성으로 인한 기능 누락 (#015)
    │   └─ Agreement TeamLeader Details 페이지가 25년도와 완전히 다른 구조
    ├─ 문제 2: DB 변경사항 일부 미반영
    │   └─ SubAgreement: SAid → Sid, UserId → Uid 변경 일부 누락
    ├─ 개선 1: 25년도 코드 복사 원칙 수립
    │   └─ 25년도 코드 그대로 복사 → 26년도 DB 변경사항만 수정
    ├─ 개선 2: 26년도 DB 변경사항 체크리스트 작성
    │   └─ Entity PK, Repository 반환 타입, 네임스페이스 변경사항 정리
    └─ 개선 3: Report/DeptObjective 작업 시 적용
        └─ 25년도 코드 분석, 체크리스트 확인, 작업지시서 검토
    ↓
[#010] 로그인 비밀번호 검증 실패 (완료 ✅)
    ├─ 해시 순서 불일치 (Password+Salt 순서로 수정)
    ├─ 인코딩 불일치 (UTF-8 → Unicode 변경)
    ├─ DB 쿼리 최적화 (v_MemberListDB 활용)
    ├─ Navigation 상태 관리 개선 (forceLoad 정리)
    ├─ Playwright 테스트 환경 구축
    └─ 프로젝트 동기화 시스템 구축
        ├─ checklist-generator Agent
        ├─ sync-validator Agent
        └─ CLAUDE.md 동기화 가이드
    ↓
[#011] Phase 3-3 관리자 페이지 빌드 오류 및 재작업 (완료 ✅ - 2026-01-30)
    ├─ 롤백 후 재작업 진행 (2026-01-26)
    ├─ ✅ 작업지시서 6개 작성 및 완료
    │   ├─ 20260126_01_phase3_3_admin_pages_rebuild.md (재작업 계획)
    │   ├─ 20260126_02_restructure_blazor_project.md (구조 재정리)
    │   ├─ 20260126_03_missing_components_checklist.md (미구현 목록)
    │   ├─ 20260128_01_implement_missing_components.md (3개 컴포넌트 구현)
    │   ├─ 20260129_01_create_v_evaluation_users_list_view.md (DB 뷰 생성)
    │   └─ 20260129_02_implement_v_evaluation_users_list_models.md (Model/Repository/Page)
    │   └─ 20260129_06_phase3_3_totalreport_step4_14.md (TotalReport 페이지)
    ├─ ✅ 빌드 경고 14개 수정 완료
    ├─ ✅ 프로젝트 구조 재정리 (.NET 10 스타일)
    │   ├─ Pages/ → Components/Pages/ 통합
    │   ├─ 공용 컴포넌트 재정리 (Common/Modal/Table)
    │   ├─ 폴더명 복수형 적용 (Depts, Ranks)
    │   └─ 네임스페이스 및 경로 업데이트
    ├─ ✅ 미구현 컴포넌트 3개 구현 완료
    │   ├─ DisplayResultText (결과 메시지 표시)
    │   ├─ EUserListTable (평가대상자 목록)
    │   └─ MemberListTable (부서/직급별 사용자 목록)
    ├─ ✅ v_EvaluationUsersList 뷰 구현 완료
    │   ├─ DB 뷰 생성 (EvaluationUsers + UserDb 조인)
    │   ├─ Model/Repository/Page 연동
    │   ├─ 사용자 이름 표시 문제 해결
    │   └─ 검색 기능 활성화 (NVARCHAR 지원)
    ├─ ✅ DB View 동기화 완료
    │   ├─ v_ProcessTRListDB (15개 → 38개 필드)
    │   ├─ v_TotalReportListDB (17개 → 25개 필드)
    │   └─ v_DeptObjectiveListDb (6개 필드)
    └─ ✅ Admin 전체 페이지 구현 완료
        ├─ Users/ (Create, Edit, Delete, Details)
        ├─ Settings/Depts/ (Create, Edit, Delete, Details)
        ├─ Settings/Ranks/ (Create, Edit, Delete, Details)
        ├─ EvaluationUsers/ (Edit, Details)
        └─ TotalReport/ (Index, Details, Edit, ReportInit)

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  참조 이슈 (다른 프로젝트)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

[#001] 알림톡 검진 모드 수정 (참조) - 기술 참조용
[#002] 프로그레스바 UI 개선 (참조) - 기술 참조용
```

---

## 이슈 생성 규칙

### 파일 구조
```
works/
├── issue.md                          # 이슈 인덱스 (이 파일)
├── issues/                           # 개별 이슈 파일 폴더
│   ├── 001_checkup_mode_alimtalk_fix.md          # 참조 (다른 프로젝트)
│   ├── 002_bulk_progressbar_full_width.md        # 참조 (다른 프로젝트)
│   ├── 003_project_roadmap.md                    # ← 프로젝트 시작
│   ├── 004_phase1_database_design.md
│   ├── 005_phase1_progress_summary.md
│   ├── 006_enhance_password_security.md
│   ├── 007_remove_memberdb_optimize_structure.md
│   ├── 008_phase2_model_development.md
│   └── ...
└── tasks/
    ├── 20251216_01_project_roadmap.md            # ← 첫 작업지시서
    ├── 20251216_02_phase1_database_design.md
    └── [작업지시서들...]
```

### 명명 규칙
- **파일명**: `{번호}_{영문제목}.md`
- **번호**: 3자리 (001, 002, ..., 007)
- **영문 제목**: 소문자와 언더스코어 사용

### 이슈 상태
- **진행중**: 현재 작업 중인 이슈
- **완료**: 작업이 완전히 끝나고 개발자 피드백까지 받은 이슈
- **보류**: 일시적으로 중단되었거나 추후 처리할 이슈

---

## 작업지시서 연결

| 작업지시서 | 상태 | 이슈 |
|-----------|------|------|
| 20251110_01_checkup_mode_alimtalk_fix.md | 참조 | [#001](issues/001_checkup_mode_alimtalk_fix.md) |
| 20251112_06_bulk_progressbar_full_width.md | 참조 | [#002](issues/002_bulk_progressbar_full_width.md) |
| 20251216_01_project_roadmap.md | 완료 | [#003](issues/003_project_roadmap.md) ← 프로젝트 시작 |
| 20251216_02_phase1_database_design.md | 완료 | [#004](issues/004_phase1_database_design.md) |
| 20251216_03_phase1_progress_summary.md | 완료 | [#005](issues/005_phase1_progress_summary.md) |
| 20260114_01_enhance_password_security.md | 완료 | [#006](issues/006_enhance_password_security.md) |
| 20260114_02_remove_memberdb_optimize_structure.md | 완료 | [#007](issues/007_remove_memberdb_optimize_structure.md) |
| 20260116_01_phase2_model_development.md | 승인 완료 | [#008](issues/008_phase2_model_development.md) |
| 20260119_01_phase2_1_project_setup.md | 완료 (13개 파일) | [#008](issues/008_phase2_model_development.md) |
| 20260119_02_phase2_2_evaluation_core.md | 완료 (12개 파일) | [#008](issues/008_phase2_model_development.md) |
| 20260119_03_phase2_3_objective_agreement.md | 완료 (15개 파일) | [#008](issues/008_phase2_model_development.md) |
| 20260119_04_phase2_4_view_models.md | 완료 (15개 파일) | [#008](issues/008_phase2_model_development.md) |
| 20260120_01_phase3_blazor_webapp.md | 승인 완료 | [#009](issues/009_phase3_webapp_development.md) |
| 20260120_02_phase3_1_project_setup.md | 완료 ✅ | [#009](issues/009_phase3_webapp_development.md) |
| 20260121_01_fix_password_hash_order.md | 완료 (코드 수정) | [#010](issues/010_login_password_hash_order_mismatch.md) |
| 20260126_01_phase3_3_admin_pages_rebuild.md | 작성 완료 (재작업 계획) | [#011](issues/011_phase3_3_admin_pages_build_errors.md) |
| 20260126_02_restructure_blazor_project.md | 완료 ✅ (구조 재정리) | [#011](issues/011_phase3_3_admin_pages_build_errors.md) |
| 20260126_03_missing_components_checklist.md | 작성 완료 (미구현 목록) | [#011](issues/011_phase3_3_admin_pages_build_errors.md) |
| 20260128_01_implement_missing_components.md | 완료 ✅ (3개 컴포넌트 구현) | [#011](issues/011_phase3_3_admin_pages_build_errors.md) |
| 20260129_01_create_v_evaluation_users_list_view.md | 완료 ✅ (DB 뷰 생성) | [#011](issues/011_phase3_3_admin_pages_build_errors.md) |
| 20260129_02_implement_v_evaluation_users_list_models.md | 완료 ✅ (Model/Repository/Page) | [#011](issues/011_phase3_3_admin_pages_build_errors.md) |
| 20260129_06_phase3_3_totalreport_step4_14.md | 완료 ✅ (빌드 성공, 런타임 오류) | [#011](issues/011_phase3_3_admin_pages_build_errors.md) / [#012](issues/012_v_processtrllistdb_view_column_mismatch.md) |
| 20260130_01_fix_v_processtrllistdb_column_mismatch.md | 완료 ✅ | [#012](issues/012_v_processtrllistdb_view_column_mismatch.md) |
| 20260130_02_fix_v_reporttasklistdb_entity_mismatch.md | 완료 ✅ (코드 수정, 빌드 성공) | [#013](issues/013_v_reporttasklistdb_entity_db_mismatch.md) |
| 20260203_05_components_agreement.md | ❌ 폐기 (Entity 변경 전 작성) | [#009](issues/009_phase3_webapp_development.md) |
| 20260203_06_components_subagreement.md | ❌ 폐기 (Entity 변경 전 작성) | [#009](issues/009_phase3_webapp_development.md) |
| 20260203_07_components_report.md | ❌ 폐기 (Entity 변경 전 작성) | [#009](issues/009_phase3_webapp_development.md) |
| 20260203_08_components_common_form.md | ❌ 폐기 (Entity 변경 전 작성) | [#009](issues/009_phase3_webapp_development.md) |
| 20260203_11_fix_entity_db_field_names.md | 완료 ✅ (5개 Entity 필드명 수정) | [#009](issues/009_phase3_webapp_development.md) |
| 20260203_12_fix_repository_based_on_2025.md | 완료 ✅ (5개 Repository + 5개 Interface) | [#009](issues/009_phase3_webapp_development.md) |
| 20260203_13_REWRITE_GUIDE.md | 가이드 📖 (작업지시서 재작성 가이드) | [#009](issues/009_phase3_webapp_development.md) |
| 20260203_13_components_agreement_v2.md | 완료 ✅ (6개 컴포넌트, 12 files) - 2026-02-04 | [#009](issues/009_phase3_webapp_development.md) |
| 20260203_14_components_subagreement_v2.md | 완료 ✅ (8개 컴포넌트, 16 files) - 2026-02-04 | [#009](issues/009_phase3_webapp_development.md) |
| 20260203_15_components_report_v2.md | 완료 ✅ (15개 컴포넌트, 30 files) - 2026-02-04 | [#009](issues/009_phase3_webapp_development.md) |
| 20260203_16_components_common_form_v2.md | 완료 ✅ (9개 컴포넌트, 17 files) - 2026-02-04 | [#009](issues/009_phase3_webapp_development.md) |
| 20260204_01_phase3_4_pages_all.md | 작성 완료 (40개 페이지, 80 files) | [#009](issues/009_phase3_webapp_development.md) |
| 20260204_02_phase3_4_agreement_pages.md | 완료 ✅ (Agreement 7개 페이지, 14 files) - 2026-02-05 | [#009](issues/009_phase3_webapp_development.md) |
| 20260204_03_phase3_4_subagreement_pages.md | 완료 ✅ (SubAgreement 10개 페이지, 20 files) - 2026-02-05 | [#009](issues/009_phase3_webapp_development.md) |
| 20260204_04_phase3_4_report_pages.md | 진행 예정 (Report 13개 페이지, 26 files) | [#009](issues/009_phase3_webapp_development.md) |
| 20260204_05_phase3_4_deptobjective_pages.md | 진행 예정 (DeptObjective 10개 페이지, 20 files) | [#009](issues/009_phase3_webapp_development.md) |
| 20260204_11_agreement_teamleader_details_fix_approval_workflow.md | 완료 ✅ (Agreement TeamLeader Details 재작성) - 2026-02-05 | [#015](issues/015_agreement_teamleader_arbitrary_code_generation.md) |
| 20260208_01_totalreport_pages.md | 완료 ✅ (TotalReport 4개 페이지, 8 files + 25년도 컴포넌트 복사) - 2026-02-08 | [#009](issues/009_phase3_webapp_development.md) / [#016](issues/016_phase3_4_db_sync_and_2025_differences.md) |

---

## 다음 이슈 번호

**다음 생성할 이슈**: #017

---

**관리자**: Claude AI & 개발자
**프로젝트**: 2026년 인사평가프로그램 (MdcHR26Apps)
