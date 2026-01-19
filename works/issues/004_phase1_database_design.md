# 이슈 #004: Phase 1 데이터베이스 설계 및 구축

**날짜**: 2025-12-16
**상태**: 완료
**우선순위**: 높음
**관련 이슈**: [#003](003_project_roadmap.md), [#005](005_phase1_progress_summary.md), [#006](006_enhance_password_security.md)

---

## 개발자 요청

**배경**:
- 2025년 인사평가 DB를 기반으로 2026년 버전 DB 설계 필요
- 테이블 간 외래키 연결을 통한 데이터 무결성 확보
- 중복 데이터(UserId, UserName 등) 제거

**요청 사항**:
1. DB 구조 분석 및 ERD 작성
2. 외래키 관계 설계
3. 테이블 및 뷰 스크립트 작성
4. 초기 데이터(Seed) 스크립트 작성

---

## 해결 방안

### 1. DB 구조 개선
- **ERankDb 신규 추가**: 직급 마스터 데이터 중앙화
- **UserDb 수정**: EDepartId, ERankId 외래키 참조
- **중복 제거**: UserId, UserName 필드 제거 및 외래키로 대체

### 2. 테이블 수정 (8개)
1. ERankDb.sql - 신규 생성
2. UserDb.sql - 외래키 추가
3. Agreement.sql - UId 외래키 추가
4. SubAgreement.sql - UId 외래키 추가
5. ProcessDb.sql - UId, TeamLeaderId, DirectorId 외래키 추가
6. ReportDb.sql - UId 외래키 추가
7. TotalReportDb.sql - UId 외래키 제약조건 추가
8. EvaluationUsers.sql - UId, TeamLeaderId, DirectorId 외래키 추가

### 3. 뷰 수정 (5개)
- 외래키 기반 JOIN으로 재작성

---

## 진행 사항

- [x] 작업지시서 작성
- [x] 2025년 DB 구조 분석
- [x] ERD 설계
- [x] 테이블 스크립트 수정 (8개)
- [x] 뷰 스크립트 수정 (5개)
- [x] 01_CreateTables.sql 작성
- [x] 02_CreateViews.sql 작성
- [x] 03_SeedData.sql 작성
- [x] 개발자 승인

---

## 테이블 구조 변경

### 외래키 관계도
```
EDepartmentDb (부서)          ERankDb (직급) [신규]
    ↓ (1:N)                      ↓ (1:N)
    └─────────┬──────────────────┘
              ↓
          UserDb (사용자) [수정]
              ↓ (1:N)
              ├─ MemberDb [기존]
              ├─ AgreementDb [수정]
              ├─ SubAgreementDb [수정]
              ├─ ProcessDb [수정]
              ├─ ReportDb [수정]
              ├─ TotalReportDb [수정]
              └─ EvaluationUsers [수정]

EDepartmentDb
    ↓ (1:N)
    └─ DeptObjectiveDb [기존]

EvaluationLists [기존 - 독립 마스터]
```

### 테이블 현황 (13개)
- 기본 테이블: EDepartmentDb, ERankDb, UserDb
- 연관 테이블: MemberDb, AgreementDb, SubAgreementDb, ProcessDb, ReportDb, TasksDb, TotalReportDb, EvaluationUsers, DeptObjectiveDb
- 독립 테이블: EvaluationLists

---

## 산출물

### SQL 스크립트
1. Database/dbo/ERankDb.sql (신규)
2. Database/dbo/UserDb.sql (수정)
3. Database/dbo/Agreement.sql (수정)
4. Database/dbo/SubAgreement.sql (수정)
5. Database/dbo/ProcessDb.sql (수정)
6. Database/dbo/ReportDb.sql (수정)
7. Database/dbo/TotalReportDb.sql (수정)
8. Database/dbo/EvaluationUsers.sql (수정)
9. Database/dbo/v_MemberListDB.sql (수정)
10. Database/dbo/v_ProcessTRListDB.sql (수정)
11. Database/dbo/v_ReportTaskListDB.sql (수정)
12. Database/dbo/v_TotalReportListDB.sql (수정)
13. Database/01_CreateTables.sql (통합)
14. Database/02_CreateViews.sql (통합)
15. Database/03_SeedData.sql (초기 데이터)

---

## 관련 문서

**작업지시서**:
- [20251216_02_phase1_database_design.md](../tasks/20251216_02_phase1_database_design.md)
- [20251216_03_phase1_progress_summary.md](../tasks/20251216_03_phase1_progress_summary.md)

**관련 이슈**:
- [#003: 프로젝트 로드맵](003_project_roadmap.md) - 선행 작업
- [#005: Phase 1 진행 요약](005_phase1_progress_summary.md) - 완료 보고서
- [#006: 비밀번호 보안 강화](006_enhance_password_security.md) - 후속 작업

---

## 개발자 피드백

**작업 완료 확인**: 2025-12-16
**최종 상태**: 완료
**비고**:
- DB 구조 설계 완료 (13개 테이블, 5개 뷰)
- 외래키 관계 정립 완료
- 초기 데이터 스크립트 작성 완료
- Phase 2 (Model 개발) 준비 완료
