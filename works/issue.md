# 이슈 인덱스

**최종 업데이트**: 2026-01-16

---

## 이슈 현황 통계

- **총 이슈**: 8개 (참조 이슈 2개 포함)
- **프로젝트 이슈**: 6개 (#003부터 시작)
- **완료**: 7개
- **진행 중**: 0개
- **작업지시서 작성 완료**: 1개
- **보류**: 0개

---

## 작업지시서 작성 완료된 이슈

| 번호 | 제목 | 우선순위 | 작성일 | 상태 |
|------|------|----------|--------|------|
| [#008](issues/008_phase2_model_development.md) | Phase 2 - Model 개발 (Dapper) | 높음 | 2026-01-16 | 개발자 승인 대기 |

---

## 최근 완료된 이슈 (최근 5개)

| 번호 | 제목 | 완료일 | 관련 작업지시서 |
|------|------|--------|----------------|
| [#007](issues/007_remove_memberdb_optimize_structure.md) | MemberDb 제거 및 부서 목표 권한 관리 최적화 | 2026-01-16 | 20260114_02 |
| [#006](issues/006_enhance_password_security.md) | UserDb 비밀번호 보안 강화 | 2026-01-14 | 20260114_01 |
| [#005](issues/005_phase1_progress_summary.md) | Phase 1 작업 완료 보고서 | 2025-12-16 | 20251216_03 |
| [#004](issues/004_phase1_database_design.md) | Phase 1 데이터베이스 설계 및 구축 | 2025-12-16 | 20251216_02 |
| [#003](issues/003_project_roadmap.md) | 2026년 인사평가프로그램 개발 로드맵 | 2025-12-16 | 20251216_01 |

---

## 전체 이슈 목록

<details>
<summary>전체 이슈 보기 (8개)</summary>

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
- [#008](issues/008_phase2_model_development.md) - Phase 2 Model 개발 (Dapper) (작업지시서 작성 완료, 2026-01-16)

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
[#008] Phase 2 Model 개발 (작업지시서 작성 완료) ← 현재

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
| 20260116_01_phase2_model_development.md | 작업지시서 작성 완료 | [#008](issues/008_phase2_model_development.md) |

---

## 다음 이슈 번호

**다음 생성할 이슈**: #009

---

**관리자**: Claude AI & 개발자
**프로젝트**: 2026년 인사평가프로그램 (MdcHR26Apps)
