# Phase 1 작업 완료 보고서

**날짜**: 2025-12-16
**작업 단계**: Phase 1 - Database 설계 및 구축
**진행 상황**: ✅ 100% 완료
**관련 이슈**: [#005: Phase 1 작업 완료 보고서](../issues/005_phase1_progress_summary.md)

---

## ✅ 완료된 작업

### 1. 테이블 수정 (8개 완료)

#### 1-1. ERankDb.sql 생성 (신규)
- **위치**: [Database/dbo/ERankDb.sql](../../Database/dbo/ERankDb.sql)
- **내용**: 직급 마스터 테이블
- **구조**:
  - ERankId (PK)
  - ERankNo (정렬순서)
  - ERankName (직급명)
  - ActivateStatus
  - Remarks

#### 1-2. UserDb.sql 수정 완료
- **위치**: [Database/dbo/UserDb.sql](../../Database/dbo/UserDb.sql)
- **변경 내용**:
  - EDepartment NVARCHAR(20) → EDepartId BIGINT (외래키)
  - ERank NVARCHAR(10) → ERankId BIGINT (외래키)
  - FK_UserDb_EDepartmentDb 추가
  - FK_UserDb_ERankDb 추가

#### 1-3. Agreement.sql 수정 완료
- **위치**: [Database/dbo/Agreement.sql](../../Database/dbo/Agreement.sql)
- **변경 내용**:
  - UserId, UserName 제거
  - UId BIGINT (외래키) 추가
  - FK_AgreementDb_UserDb 추가

#### 1-4. SubAgreement.sql 수정 완료
- **위치**: [Database/dbo/SubAgreement.sql](../../Database/dbo/SubAgreement.sql)
- **변경 내용**:
  - UserId, UserName 제거
  - UId BIGINT (외래키) 추가
  - FK_SubAgreementDb_UserDb 추가

#### 1-5. ProcessDb.sql 수정 완료
- **위치**: [Database/dbo/ProcessDb.sql](../../Database/dbo/ProcessDb.sql)
- **변경 내용**:
  - UserId, UserName 제거
  - TeamLeader_Id, TeamLeader_Name 제거
  - Director_Id, Director_Name 제거
  - UId BIGINT (평가 대상자 외래키) 추가
  - TeamLeaderId BIGINT (부서장 외래키) 추가
  - DirectorId BIGINT (임원 외래키) 추가
  - 3개의 외래키 제약조건 추가

#### 1-6. ReportDb.sql 수정 완료
- **위치**: [Database/dbo/ReportDb.sql](../../Database/dbo/ReportDb.sql)
- **변경 내용**:
  - UserId, UserName 제거
  - UId BIGINT (외래키) 추가
  - FK_ReportDb_UserDb 추가

#### 1-7. TotalReportDb.sql 수정 완료
- **위치**: [Database/dbo/TotalReportDb.sql](../../Database/dbo/TotalReportDb.sql)
- **변경 내용**:
  - 기존 UId에 외래키 제약조건 추가
  - FK_TotalReportDb_UserDb 추가

#### 1-8. EvaluationUsers.sql 수정 완료
- **위치**: [Database/dbo/EvaluationUsers.sql](../../Database/dbo/EvaluationUsers.sql)
- **변경 내용**:
  - UserId, UserName 제거
  - TeamLeader_Id, TeamLeader_Name 제거
  - Director_Id, Director_Name 제거
  - UId BIGINT (평가 대상자 외래키) 추가
  - TeamLeaderId BIGINT (부서장 외래키) 추가
  - DirectorId BIGINT (임원 외래키) 추가
  - 3개의 외래키 제약조건 추가

### 2. 뷰(View) 수정 (5개 완료)

#### 2-1. v_MemberListDB.sql 수정 완료
- **위치**: [Database/dbo/v_MemberListDB.sql](../../Database/dbo/v_MemberListDB.sql)
- **변경 내용**: ERankDb JOIN 추가하여 직급명 조회

#### 2-2. v_DeptObjectiveListDb.sql 검토 완료
- **위치**: [Database/dbo/v_DeptObjectiveListDb.sql](../../Database/dbo/v_DeptObjectiveListDb.sql)
- **상태**: 이미 외래키 기반으로 작성되어 있어 수정 불필요

#### 2-3. v_ProcessTRListDB.sql 수정 완료
- **위치**: [Database/dbo/v_ProcessTRListDB.sql](../../Database/dbo/v_ProcessTRListDB.sql)
- **변경 내용**:
  - UserDb를 3번 JOIN (평가대상자, 팀장, 임원)
  - ProcessDb 및 TotalReportDb의 UId 기반으로 재작성

#### 2-4. v_ReportTaskListDB.sql 수정 완료
- **위치**: [Database/dbo/v_ReportTaskListDB.sql](../../Database/dbo/v_ReportTaskListDB.sql)
- **변경 내용**: UserDb JOIN 추가하여 UserId, UserName 조회

#### 2-5. v_TotalReportListDB.sql 수정 완료
- **위치**: [Database/dbo/v_TotalReportListDB.sql](../../Database/dbo/v_TotalReportListDB.sql)
- **변경 내용**: UId 대소문자 표기 통일 (Uid → UId)

### 3. 통합 실행 스크립트 작성 (3개 완료)

#### 3-1. 01_CreateTables.sql 작성 완료 ✅
- **위치**: [Database/01_CreateTables.sql](../../Database/01_CreateTables.sql)
- **내용**:
  - 기존 테이블 DROP (외래키 의존성 역순)
  - 마스터 데이터 테이블 생성 (EDepartmentDb, ERankDb)
  - UserDb 생성
  - UserDb 참조 테이블들 생성 (8개)
  - 독립 테이블 생성 (TasksDb, EvaluationLists)
  - 총 13개 테이블 생성
  - PRINT 문으로 진행 상황 표시

#### 3-2. 02_CreateViews.sql 작성 완료 ✅
- **위치**: [Database/02_CreateViews.sql](../../Database/02_CreateViews.sql)
- **내용**:
  - 기존 뷰 DROP
  - 5개 뷰 생성:
    - v_MemberListDB
    - v_DeptObjectiveListDb
    - v_ProcessTRListDB
    - v_ReportTaskListDB
    - v_TotalReportListDB
  - PRINT 문으로 진행 상황 표시

#### 3-3. 03_SeedData.sql 작성 완료 ✅
- **위치**: [Database/03_SeedData.sql](../../Database/03_SeedData.sql)
- **내용**:
  - EDepartmentDb: 5개 부서 데이터
  - ERankDb: 9개 직급 데이터 (사원~사장)
  - UserDb: 관리자 계정 1개 (admin / admin1234)
  - EvaluationLists: 20개 평가 항목 (5개 부서별)
  - 검증 쿼리 포함

---

## 📊 최종 DB 구조

### 외래키 관계도
```
EDepartmentDb (부서)          ERankDb (직급) ✅ 신규
    ↓ (1:N)                      ↓ (1:N)
    └─────────┬──────────────────┘
              ↓
          UserDb (사용자) ✅ 수정 완료
              ↓ (1:N)
              ├─ MemberDb ✅ (기존 외래키 사용 중 - 수정 불필요)
              ├─ AgreementDb ✅ 수정 완료
              ├─ SubAgreementDb ✅ 수정 완료
              ├─ ProcessDb ✅ 수정 완료 (3개 외래키)
              ├─ ReportDb ✅ 수정 완료
              ├─ TasksDb ✅ 현재 구조 유지 (외래키 없음)
              ├─ TotalReportDb ✅ 수정 완료
              └─ EvaluationUsers ✅ 수정 완료 (3개 외래키)

EDepartmentDb
    ↓ (1:N)
    └─ DeptObjectiveDb ✅ (기존 외래키 사용 중 - 수정 불필요)

EvaluationLists (독립적인 마스터 데이터 - 수정 불필요)
```

### 테이블 현황 (13개)
| 번호 | 테이블 | 상태 | 외래키 |
|------|--------|------|--------|
| 1 | EDepartmentDb | ✅ 완료 | - |
| 2 | ERankDb | ✅ 신규 생성 | - |
| 3 | UserDb | ✅ 수정 완료 | EDepartId, ERankId |
| 4 | MemberDb | ✅ 기존 유지 | UId, EDepartId |
| 5 | AgreementDb | ✅ 수정 완료 | UId |
| 6 | SubAgreementDb | ✅ 수정 완료 | UId |
| 7 | ProcessDb | ✅ 수정 완료 | UId, TeamLeaderId, DirectorId |
| 8 | ReportDb | ✅ 수정 완료 | UId |
| 9 | TasksDb | ✅ 현재 구조 유지 | - |
| 10 | TotalReportDb | ✅ 수정 완료 | UId |
| 11 | EvaluationUsers | ✅ 수정 완료 | UId, TeamLeaderId, DirectorId |
| 12 | DeptObjectiveDb | ✅ 기존 유지 | EDepartId |
| 13 | EvaluationLists | ✅ 기존 유지 | - |

### 뷰 현황 (5개)
| 번호 | 뷰 | 상태 |
|------|-------|------|
| 1 | v_MemberListDB.sql | ✅ 수정 완료 |
| 2 | v_DeptObjectiveListDb.sql | ✅ 검토 완료 (수정 불필요) |
| 3 | v_ProcessTRListDB.sql | ✅ 수정 완료 |
| 4 | v_ReportTaskListDB.sql | ✅ 수정 완료 |
| 5 | v_TotalReportListDB.sql | ✅ 수정 완료 |

### 통합 스크립트 (3개)
| 번호 | 스크립트 | 상태 |
|------|----------|------|
| 1 | 01_CreateTables.sql | ✅ 작성 완료 |
| 2 | 02_CreateViews.sql | ✅ 작성 완료 |
| 3 | 03_SeedData.sql | ✅ 작성 완료 |

---

## 📝 주요 의사결정 사항

### 1. ERankDb 도입 결정 ✅
**질문**: "RankDb도 도입하는 것도 좋지 않을까요?"

**결정**: ERankDb 신규 생성
- 직급 마스터 데이터 중앙화
- UserDb에서 ERankId 외래키 참조
- v_MemberListDB에서 직급명 조회 가능

### 2. TasksDb UId 추가 논의 ✅
**질문**: "TasksDb의 UId 추가의 이점이 있나요?"

**결정**: 현재 구조 유지 (UId 추가 안 함)
- **이유**: ReportDb에 UId와 Task_Number가 있어 간접 참조 가능
- **장점**: 구조 변경 최소화
- **단점**: TasksDb 독립 조회 시 ReportDb 경유 필요

---

## 🚀 다음 단계: Phase 2 준비

### Phase 2 작업 개요
**목표**: Dapper 기반 Model 및 Repository 클래스 작성

### 작업 순서
1. **Model 클래스 작성** (13개 테이블)
   - EDepartmentDb → EDepartment.cs
   - ERankDb → ERank.cs
   - UserDb → User.cs
   - ... (나머지 테이블들)

2. **Repository 클래스 작성** (13개)
   - EDepartmentRepository.cs
   - ERankRepository.cs
   - UserRepository.cs
   - ... (나머지)
   - CRUD 메서드 구현 (Dapper 사용)

3. **View Model 클래스 작성** (5개)
   - v_MemberListDB → MemberListView.cs
   - v_DeptObjectiveListDb → DeptObjectiveListView.cs
   - ... (나머지)

### 필요한 사전 작업
1. SQL Server에서 스크립트 실행
   ```sql
   -- 1단계: 테이블 생성
   실행: Database/01_CreateTables.sql

   -- 2단계: 뷰 생성
   실행: Database/02_CreateViews.sql

   -- 3단계: 시드 데이터 입력
   실행: Database/03_SeedData.sql
   ```

2. 데이터베이스 연결 확인
   - Connection String 설정
   - 관리자 계정 로그인 테스트 (admin / admin1234)

3. Dapper NuGet 패키지 설치 준비

---

## ✔️ 체크리스트

### Phase 1 완료 항목
- [x] ERankDb.sql 생성
- [x] UserDb.sql 수정
- [x] Agreement.sql 수정
- [x] SubAgreement.sql 수정
- [x] ProcessDb.sql 수정
- [x] ReportDb.sql 수정
- [x] TotalReportDb.sql 수정
- [x] EvaluationUsers.sql 수정
- [x] TasksDb.sql 현재 구조 유지 결정
- [x] v_MemberListDB.sql 수정
- [x] v_DeptObjectiveListDb.sql 검토
- [x] v_ProcessTRListDB.sql 수정
- [x] v_ReportTaskListDB.sql 수정
- [x] v_TotalReportListDB.sql 수정
- [x] 01_CreateTables.sql 작성
- [x] 02_CreateViews.sql 작성
- [x] 03_SeedData.sql 작성

### Phase 2 예정 항목
- [ ] SQL Server에서 스크립트 실행 및 테스트
- [ ] Model 클래스 작성 (13개)
- [ ] Repository 클래스 작성 (13개)
- [ ] View Model 클래스 작성 (5개)
- [ ] Unit Test 작성

---

## 📁 관련 문서

- **프로젝트 로드맵**: [20251216_01_project_roadmap.md](20251216_01_project_roadmap.md)
- **Phase 1 작업지시서**: [20251216_02_phase1_database_design.md](20251216_02_phase1_database_design.md)
- **현재 진행 요약**: 이 문서

---

## 📌 개발자 확인 사항

### SQL Server 실행 순서
```sql
-- 반드시 순서대로 실행하세요!
1. Database/01_CreateTables.sql  -- 테이블 생성
2. Database/02_CreateViews.sql   -- 뷰 생성
3. Database/03_SeedData.sql      -- 시드 데이터

-- 검증 쿼리
SELECT * FROM EDepartmentDb;     -- 부서 5개 확인
SELECT * FROM ERankDb;           -- 직급 9개 확인
SELECT * FROM UserDb;            -- 관리자 1개 확인
SELECT * FROM EvaluationLists;   -- 평가항목 20개 확인

-- 뷰 테스트
SELECT * FROM v_MemberListDB;
SELECT * FROM v_DeptObjectiveListDb;
SELECT * FROM v_ProcessTRListDB;
SELECT * FROM v_ReportTaskListDB;
SELECT * FROM v_TotalReportListDB;
```

### 주의사항
1. **관리자 비밀번호 변경**: admin1234는 초기 비밀번호이므로 반드시 변경
2. **시드 데이터 수정**: 실제 조직에 맞게 부서, 직급, 평가항목 수정
3. **외래키 제약조건**: DELETE 시 ON DELETE NO ACTION이므로 참조 데이터 먼저 삭제 필요

---

**작성일**: 2025-12-16
**진행률**: ✅ 100% (Phase 1 완료)
**다음 작업**: Phase 2 - Model 및 Repository 작성 (개발자 승인 후 진행)
