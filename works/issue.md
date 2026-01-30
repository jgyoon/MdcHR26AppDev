# 이슈 인덱스

**최종 업데이트**: 2026-01-30

---

## 이슈 현황 통계

- **총 이슈**: 14개 (참조 이슈 2개 포함)
- **프로젝트 이슈**: 12개 (#003부터 시작)
- **완료**: 12개
- **진행 중**: 2개
- **보류**: 0개

---

## 진행 중인 이슈

| 번호 | 제목 | 시작일 | 진행률 | 현재 단계 |
|------|------|--------|--------|----------|
| [#009](issues/009_phase3_webapp_development.md) | Phase 3 - Blazor Server WebApp 개발 | 2026-01-20 | 40% (Phase 3-3 진행 중) | Admin 페이지 기본 컴포넌트 완성, CRUD 기능 구현 대기 |
| [#011](issues/011_phase3_3_admin_pages_build_errors.md) | Phase 3-3 관리자 페이지 빌드 오류 및 재작업 | 2026-01-22 | 90% (기본 컴포넌트 완성) | v_EvaluationUsersList 뷰 구현 완료, CRUD 기능 구현 대기 |

---

## 최근 완료된 이슈 (최근 5개)

| 번호 | 제목 | 완료일 | 관련 작업지시서 |
|------|------|--------|----------------|
| [#014](issues/014_site_js_not_loaded_app_razor.md) | site.js 파일이 App.razor에 로드되지 않음 | 2026-01-30 | - |
| [#013](issues/013_v_reporttasklistdb_entity_db_mismatch.md) | v_ReportTaskListDB Entity와 DB View 구조 불일치 | 2026-01-30 | 20260130_02 |
| [#012](issues/012_v_processtrllistdb_view_column_mismatch.md) | v_ProcessTRListDB View 컬럼 불일치 오류 | 2026-01-30 | 20260130_01 |
| [#010](issues/010_login_password_hash_order_mismatch.md) | 로그인 비밀번호 검증 실패 - 해시 순서 및 인코딩 불일치 | 2026-01-22 | 20260121_01 |
| [#008](issues/008_phase2_model_development.md) | Phase 2 - Model 개발 (Dapper) | 2026-01-20 | 20260119_01~04 (4개) |

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
- [#011](issues/011_phase3_3_admin_pages_build_errors.md) - Phase 3-3 관리자 페이지 빌드 오류 및 재작업 (진행중, 2026-01-22~)
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
    │   ├─ 로그인 페이지 UI (2025년 스타일)
    │   ├─ 로그인 인증 로직
    │   ├─ 로그아웃 기능
    │   └─ LoginStatusService (상태 관리)
    ├─ Phase 3-3: 관리자 페이지 (진행중 🔄)
    │   ├─ [#011] 작업지시서 3개 완료 (진행중 🔄)
    │   │   ├─ ✅ 빌드 경고 14개 수정
    │   │   ├─ ✅ 프로젝트 구조 재정리 (.NET 10 스타일)
    │   │   └─ ⏳ 미구현 컴포넌트 3개 (DisplayResultText, EUserListTable, MemberListTable)
    │   └─ Admin 페이지 기본 구조 완성 (2026-01-28)
    ├─ Phase 3-4: 평가 프로세스 (예정 ⏳)
    ├─ Phase 3-5: 공통 컴포넌트 (예정 ⏳)
    └─ Phase 3-6: 엑셀 및 유틸리티 (예정 ⏳)
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
[#011] Phase 3-3 관리자 페이지 빌드 오류 (진행중 🔄 - 90%)
    ├─ 롤백 후 재작업 진행 (2026-01-26)
    ├─ ✅ 작업지시서 6개 작성 완료
    │   ├─ 20260126_01_phase3_3_admin_pages_rebuild.md (재작업 계획)
    │   ├─ 20260126_02_restructure_blazor_project.md (구조 재정리)
    │   ├─ 20260126_03_missing_components_checklist.md (미구현 목록)
    │   ├─ 20260128_01_implement_missing_components.md (3개 컴포넌트 구현)
    │   ├─ 20260129_01_create_v_evaluation_users_list_view.md (DB 뷰 생성)
    │   └─ 20260129_02_implement_v_evaluation_users_list_models.md (Model/Repository/Page)
    ├─ ✅ 빌드 경고 14개 수정 완료
    ├─ ✅ 프로젝트 구조 재정리 (.NET 10 스타일)
    │   ├─ Pages/ → Components/Pages/ 통합
    │   ├─ 공용 컴포넌트 재정리 (Common/Modal/Table)
    │   ├─ 폴더명 복수형 적용 (Depts, Ranks)
    │   └─ 네임스페이스 및 경로 업데이트
    ├─ ✅ 미구현 컴포넌트 3개 구현 완료 (2026-01-28)
    │   ├─ DisplayResultText (결과 메시지 표시)
    │   ├─ EUserListTable (평가대상자 목록)
    │   └─ MemberListTable (부서/직급별 사용자 목록)
    ├─ ✅ v_EvaluationUsersList 뷰 구현 완료 (2026-01-29)
    │   ├─ DB 뷰 생성 (EvaluationUsers + UserDb 조인)
    │   ├─ Model/Repository/Page 연동
    │   ├─ 사용자 이름 표시 문제 해결 ("미지정" → 실제 이름)
    │   └─ 검색 기능 활성화 (NVARCHAR 지원)
    └─ ⏳ Admin CRUD 기능 구현 (다음 작업)

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

---

## 다음 이슈 번호

**다음 생성할 이슈**: #015

---

**관리자**: Claude AI & 개발자
**프로젝트**: 2026년 인사평가프로그램 (MdcHR26Apps)
