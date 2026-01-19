# Phase 1 작업지시서: 데이터베이스 설계 및 구축

**날짜**: 2025-12-16
**작업 유형**: 데이터베이스 설계 및 구축
**관련 문서**: [프로젝트 로드맵](20251216_01_project_roadmap.md)
**관련 이슈**: [#004: Phase 1 데이터베이스 설계 및 구축](../issues/004_phase1_database_design.md)
**Phase**: 1단계 (Database)

---

## 1. 작업 개요

### 1.1. 목적
- 2025년 인사평가 DB를 기반으로 2026년 버전 DB 설계
- 테이블 간 외래키 연결을 통한 데이터 무결성 확보
- 중복 데이터(UserId, UserName 등) 제거

### 1.2. 현재 상태
- Database/dbo 폴더에 2025년 SQL 파일 복사 완료
- 12개 테이블 + 5개 뷰 파일 존재

### 1.3. 작업 범위
- DB 구조 분석 및 ERD 작성
- 외래키 관계 설계
- 테이블 스크립트 수정
- 뷰 스크립트 수정
- 초기 데이터(Seed) 스크립트 작성

---

## 2. 현재 파일 현황

### 2.1. 테이블 파일 (12개 → 13개로 확장)
```
Database/dbo/
├── UserDb.sql                 # 사용자 계정
├── EDepartmentDb.sql          # 부서 정보
├── ERankDb.sql               # 직급 정보 (신규 추가 예정)
├── MemberDb.sql              # 사용자-부서 연결 (이미 외래키 사용 중)
├── Agreement.sql             # 직무평가 합의
├── SubAgreement.sql          # 세부직무평가 합의
├── ProcessDb.sql             # 평가 프로세스 상태
├── ReportDb.sql              # 평가 보고서
├── TasksDb.sql               # 세부 업무
├── TotalReportDb.sql         # 종합 평가 보고서
├── EvaluationLists.sql       # 평가 항목 리스트
├── EvaluationUsers.sql       # 평가 대상자 목록
└── DeptObjectiveDb.sql       # 부서 목표
```

### 2.2. 뷰 파일 (5개)
```
Database/dbo/
├── v_MemberListDB.sql        # 사용자-부서 조인
├── v_DeptObjectiveListDb.sql # 부서 목표 리스트
├── v_ProcessTRListDB.sql     # 프로세스-종합보고서 조인
├── v_ReportTaskListDB.sql    # 보고서-업무 조인
└── v_TotalReportListDB.sql   # 종합 보고서 리스트
```

---

## 3. DB 구조 분석

### 3.1. 기본 테이블 (외래키 참조 대상)

#### 3.1.1. EDepartmentDb (부서)
- **PK**: EDepartId (BIGINT)
- **특징**: 이미 완성된 구조
- **수정 필요**: 없음

#### 3.1.2. ERankDb (직급) - 신규 추가
- **PK**: ERankId (BIGINT)
- **구조**: 도서관 프로젝트 ERankDb 참조
  - ERankNo (정렬순서)
  - ERankName (직급명)
  - ActivateStatus (활성화여부)
  - Remarks (비고)
- **목적**: 직급 정보 중앙 관리
- **수정 필요**: 신규 생성

#### 3.1.3. UserDb (사용자)
- **PK**: Uid (BIGINT)
- **현재 문제점**:
  - EDepartment (부서명)이 텍스트로 저장됨
  - ERank (직급명)이 텍스트로 저장됨
  - 외래키 연결 필요
- **개선 방안**:
  - EDepartment NVARCHAR(20) → EDepartId BIGINT (외래키)
  - ERank NVARCHAR(10) → ERankId BIGINT (외래키)
  - EDepartmentDb, ERankDb 참조

### 3.2. 연관 테이블 (외래키 사용)

#### 3.2.1. MemberDb (사용자-부서 연결)
- **현재 상태**: 이미 외래키 사용 중 ✅
- **외래키**:
  - UId → UserDb(Uid)
  - EDepartId → EDepartmentDb(EDepartId)
- **수정 필요**: 없음

#### 3.2.2. Agreement.sql (직무평가 합의)
- **현재 문제점**:
  - UserId VARCHAR(50) - 중복 데이터
  - UserName NVARCHAR(20) - 중복 데이터
- **개선 방안**:
  - UserId, UserName 제거
  - UId BIGINT (외래키) → UserDb(Uid) 참조

#### 3.2.3. SubAgreement.sql (세부직무평가 합의)
- **현재 문제점**:
  - UserId, UserName 중복
- **개선 방안**:
  - UserId, UserName 제거
  - UId BIGINT (외래키) 추가

#### 3.2.4. ProcessDb.sql (평가 프로세스)
- **현재 문제점**:
  - UserId, UserName - 평가 대상자 중복
  - TeamLeader_Id, TeamLeader_Name - 팀장 정보 중복
  - Director_Id, Director_Name - 임원 정보 중복
- **개선 방안**:
  - UId BIGINT (평가 대상자)
  - TeamLeaderId BIGINT (팀장)
  - DirectorId BIGINT (임원)
  - 모두 UserDb(Uid) 참조

#### 3.2.5. ReportDb.sql (평가 보고서)
- **현재 문제점**:
  - UserId, UserName 중복
  - Task_Number가 있으나 외래키 아님
- **개선 방안**:
  - UserId, UserName 제거
  - UId BIGINT (외래키) 추가
  - Task_Number → TaskId로 변경하고 TasksDb 참조 고려

#### 3.2.6. TasksDb.sql (세부 업무)
- **현재 문제점**:
  - 사용자 정보 없음 (어떤 사용자의 업무인지 불명확)
- **개선 방안**:
  - UId BIGINT (외래키) 추가 권장
  - 또는 ReportDb를 통해 간접 참조

#### 3.2.7. TotalReportDb.sql (종합 평가 보고서)
- **현재 상태**: 이미 UId 사용 중 ✅
- **개선 방안**:
  - UId BIGINT에 외래키 제약조건 추가
  - FOREIGN KEY (UId) REFERENCES UserDb(Uid)

#### 3.2.8. EvaluationUsers.sql (평가 대상자)
- **현재 문제점**:
  - UserId, UserName - 평가 대상자 중복
  - TeamLeader_Id, TeamLeader_Name - 부서장(2차평가자) 중복
  - Director_Id, Director_Name - 임원(3차평가자) 중복
- **테이블 목적**:
  - 평가 대상자 지정
  - 부서장(2차평가자) 지정
  - 임원(3차평가자) 지정
- **개선 방안**:
  - UserId, UserName, TeamLeader_Id, TeamLeader_Name, Director_Id, Director_Name 제거
  - UId BIGINT (평가 대상자) - UserDb 참조
  - TeamLeaderId BIGINT (부서장) - UserDb 참조
  - DirectorId BIGINT (임원) - UserDb 참조

#### 3.2.9. EvaluationLists.sql (평가 항목)
- **테이블 목적**: 평가 항목 마스터 데이터
- **특징**: 독립적인 마스터 데이터 테이블
- **수정 필요**: 없음 (다른 테이블과 연관성 없음)

#### 3.2.10. DeptObjectiveDb.sql (부서 목표)
- **현재 상태**: 이미 외래키 사용 중 ✅
- **외래키**:
  - EDepartId → EDepartmentDb(EDepartId)
- **수정 필요**: 없음

---

## 4. ERD 설계

### 4.1. 핵심 관계도

```
EDepartmentDb (부서)          ERankDb (직급) - 신규 추가
    ↓ (1:N)                      ↓ (1:N)
    ├─────────────┬───────────────┘
                  ↓
              UserDb (사용자) ← EDepartId, ERankId 외래키 추가
                  ↓ (1:N)
                  ├─ MemberDb (사용자-부서 연결) ✅ 이미 외래키 사용
                  ├─ AgreementDb (직무평가 합의) - UId 외래키 추가
                  ├─ SubAgreementDb (세부직무평가 합의) - UId 외래키 추가
                  ├─ ProcessDb (평가 프로세스) - UId, TeamLeaderId, DirectorId 외래키 추가
                  ├─ ReportDb (평가 보고서) - UId 외래키 추가
                  ├─ TasksDb (세부 업무) - UId 외래키 추가 권장
                  ├─ TotalReportDb (종합 보고서) - UId 외래키 제약조건 추가
                  └─ EvaluationUsers (평가 대상자) - UId, TeamLeaderId, DirectorId 외래키 추가

EDepartmentDb (부서)
    ↓ (1:N)
    └─ DeptObjectiveDb (부서 목표) ✅ 이미 외래키 사용

EvaluationLists (평가 항목) - 독립적인 마스터 데이터
```

### 4.2. 외래키 삭제 동작 (ON DELETE)
- **기본 원칙**: `ON DELETE NO ACTION` (기본값)
  - 평가 데이터는 역사적 기록이므로 함부로 삭제되면 안 됨
  - 사용자 삭제 시 평가 데이터는 유지
  - 필요 시 EStatus(재직여부)로 관리

---

## 5. 작업 순서

### 5.1. 1단계: 추가 파일 분석 ✅ 완료
- [x] EvaluationUsers.sql 파일 내용 확인
- [x] EvaluationLists.sql 파일 내용 확인
- [x] DeptObjectiveDb.sql 파일 내용 확인
- [x] 테이블 간 관계 최종 확정

### 5.2. 2단계: ERD 작성
- [ ] Mermaid 형식으로 ERD 작성
- [ ] works/images 폴더에 ERD 다이어그램 저장 (선택)
- [ ] 외래키 관계 문서화

### 5.3. 3단계: 테이블 스크립트 수정
#### 기본 테이블
- [ ] ERankDb.sql 생성 (신규)
  - 도서관 프로젝트 ERankDb.sql 참조
  - ERankId, ERankNo, ERankName, ActivateStatus, Remarks

- [ ] UserDb.sql 수정
  - EDepartment NVARCHAR(20) → EDepartId BIGINT 변경
  - ERank NVARCHAR(10) → ERankId BIGINT 변경
  - EDepartmentDb, ERankDb 외래키 추가

#### 연관 테이블
- [ ] Agreement.sql → AgreementDb.sql 수정
  - UserId, UserName 제거
  - UId 외래키 추가

- [ ] SubAgreement.sql → SubAgreementDb.sql 수정
  - UserId, UserName 제거
  - UId 외래키 추가

- [ ] ProcessDb.sql 수정
  - UserId, UserName, TeamLeader_Id, TeamLeader_Name, Director_Id, Director_Name 제거
  - UId, TeamLeaderId, DirectorId 외래키 추가

- [ ] ReportDb.sql 수정
  - UserId, UserName 제거
  - UId 외래키 추가
  - Task_Number → TaskId로 변경 검토

- [ ] TasksDb.sql 수정
  - UId 외래키 추가 검토

- [ ] TotalReportDb.sql 수정
  - UId에 외래키 제약조건 추가

- [ ] EvaluationUsers.sql 수정
  - UserId, UserName, TeamLeader_Id, TeamLeader_Name, Director_Id, Director_Name 제거
  - UId, TeamLeaderId, DirectorId 외래키 추가

- [ ] DeptObjectiveDb.sql 검토
  - 이미 외래키 사용 중 - 수정 불필요

- [ ] EvaluationLists.sql 검토
  - 독립적인 마스터 데이터 - 수정 불필요

### 5.4. 4단계: 뷰 스크립트 수정
- [ ] v_MemberListDB.sql 검토 (이미 외래키 기반)
- [ ] v_DeptObjectiveListDb.sql 수정
- [ ] v_ProcessTRListDB.sql 수정
- [ ] v_ReportTaskListDB.sql 수정
- [ ] v_TotalReportListDB.sql 수정

### 5.5. 5단계: 통합 실행 스크립트 작성
- [ ] 01_CreateTables.sql 작성
  - 생성 순서: EDepartmentDb → ERankDb → UserDb → 나머지 테이블
- [ ] 02_CreateViews.sql 작성
- [ ] 03_SeedData.sql 작성 (초기 데이터)
  - 부서 마스터 데이터 (EDepartmentDb)
  - 직급 마스터 데이터 (ERankDb) - 신규
  - 관리자 계정 (UserDb)
  - 평가 항목 기본 데이터 (EvaluationLists)

### 5.6. 6단계: 테스트 스크립트 작성
- [ ] 04_TestData.sql 작성 (테스트용 더미 데이터)
- [ ] 05_TestQueries.sql 작성 (검증 쿼리)

---

## 6. 수정 가이드라인

### 6.1. 외래키 추가 템플릿
```sql
-- 외래키 제약조건 추가
CONSTRAINT FK_테이블명_참조테이블명
    FOREIGN KEY (컬럼명)
    REFERENCES 참조테이블(참조컬럼)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
```

### 6.2. UserDb 수정 예시 (EDepartment, ERank → 외래키 변경)
**변경 전**:
```sql
CREATE TABLE [dbo].[UserDb]
(
    [Uid] BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    [UserId] VARCHAR(50) NOT NULL,
    [UserName] NVARCHAR(20) NOT NULL,
    [UserPassword] VARBINARY(100) NOT NULL,
    [ENumber] VARCHAR(10),
    [Email] VARCHAR(50),
    [EDepartment] NVARCHAR(20),        -- 텍스트로 저장
    [ERank] NVARCHAR(10),              -- 텍스트로 저장
    [EStatus] BIT NOT NULL DEFAULT 1,
    [IsTeamLeader] BIT NOT NULL DEFAULT 0,
    [IsDirector] BIT NOT NULL DEFAULT 0,
    [IsAdministrator] BIT NOT NULL DEFAULT 0
)
```

**변경 후**:
```sql
CREATE TABLE [dbo].[UserDb]
(
    [Uid] BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    [UserId] VARCHAR(50) NOT NULL,
    [UserName] NVARCHAR(20) NOT NULL,
    [UserPassword] VARBINARY(100) NOT NULL,
    [ENumber] VARCHAR(10),
    [Email] VARCHAR(50),
    [EDepartId] BIGINT,                -- 외래키로 변경
    [ERankId] BIGINT,                  -- 외래키로 변경
    [EStatus] BIT NOT NULL DEFAULT 1,
    [IsTeamLeader] BIT NOT NULL DEFAULT 0,
    [IsDirector] BIT NOT NULL DEFAULT 0,
    [IsAdministrator] BIT NOT NULL DEFAULT 0,

    -- 외래키 제약조건
    CONSTRAINT FK_UserDb_EDepartmentDb
        FOREIGN KEY (EDepartId)
        REFERENCES [dbo].[EDepartmentDb](EDepartId)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,

    CONSTRAINT FK_UserDb_ERankDb
        FOREIGN KEY (ERankId)
        REFERENCES [dbo].[ERankDb](ERankId)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
)
```

### 6.3. UserId, UserName 제거 후 UId 추가 예시
**변경 전**:
```sql
CREATE TABLE [dbo].[AgreementDb]
(
    [Aid] BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    [UserId] VARCHAR(50) NOT NULL,
    [UserName] NVARCHAR(20) NOT NULL,
    ...
)
```

**변경 후**:
```sql
CREATE TABLE [dbo].[AgreementDb]
(
    [Aid] BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    -- [02] 사용자 ID (외래키)
    [UId] BIGINT NOT NULL,
    ...
    -- 외래키 제약조건
    CONSTRAINT FK_AgreementDb_UserDb
        FOREIGN KEY (UId)
        REFERENCES [dbo].[UserDb](UId)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
)
```

### 6.3. 뷰 수정 예시
**변경 전** (직접 저장된 데이터 사용):
```sql
SELECT
    Aid,
    UserId,
    UserName,
    Report_Item_Number
FROM [dbo].[AgreementDb]
```

**변경 후** (JOIN으로 UserDb 참조):
```sql
SELECT
    A.Aid,
    A.UId,
    U.UserId,
    U.UserName,
    A.Report_Item_Number
FROM [dbo].[AgreementDb] A
INNER JOIN [dbo].[UserDb] U ON A.UId = U.UId
```

---

## 7. 주의사항

### 7.1. 데이터 마이그레이션
- 기존 2025년 DB에서 2026년 DB로 데이터 이전 시:
  - UserId로 UId 매핑 필요
  - 데이터 무결성 확인 필수
  - 마이그레이션 스크립트 별도 작성 검토

### 7.2. 성능 고려사항
- 외래키 컬럼에 인덱스 자동 생성 확인
- 대량 데이터 INSERT 시 제약조건 임시 비활성화 검토
- 뷰 조회 성능 테스트 필요

### 7.3. 명명 규칙
- 테이블명: PascalCase + Db (예: UserDb)
- 컬럼명: PascalCase
- 외래키명: FK_테이블명_참조테이블명
- 뷰명: v_설명 (예: v_MemberListDB)

### 7.4. 백업
- 현재 파일들은 2025년 원본을 복사한 것
- 수정 전 별도 백업 폴더 생성 검토
  - Database/dbo_backup_original/

---

## 8. 테스트 계획

### 8.1. 단위 테스트
- [ ] 각 테이블 생성 확인
- [ ] 외래키 제약조건 동작 확인
  - 참조 무결성 위반 시 에러 발생 확인
  - 존재하지 않는 UId INSERT 차단 확인
- [ ] 뷰 조회 확인

### 8.2. 통합 테스트
- [ ] 전체 테이블 생성 순서 확인
- [ ] 뷰 생성 확인
- [ ] 초기 데이터 INSERT 확인
- [ ] 복잡한 JOIN 쿼리 성능 테스트

### 8.3. 시나리오 테스트
**시나리오 1: 신규 사용자 평가 등록**
1. 부서 생성 (EDepartmentDb)
2. 직급 생성 (ERankDb) - 신규
3. 사용자 생성 (UserDb) - 부서ID, 직급ID 참조
4. 사용자-부서 연결 (MemberDb)
5. 직무평가 합의 등록 (AgreementDb)
6. 세부직무평가 합의 등록 (SubAgreementDb)
7. 평가 프로세스 시작 (ProcessDb)
7. 평가 보고서 작성 (ReportDb)
8. 종합 평가 완료 (TotalReportDb)

**시나리오 2: 외래키 제약 확인**
1. 존재하지 않는 UId로 AgreementDb INSERT → 에러 발생 확인
2. 평가 데이터가 있는 사용자 삭제 시도 → 에러 발생 확인

---

## 9. 산출물

### 9.1. 문서
- [ ] ERD 다이어그램 (works/images/erd_diagram.png)
- [ ] 테이블 관계 정의서 (별도 md 파일)
- [ ] 외래키 목록 정리 (별도 md 파일)

### 9.2. SQL 스크립트
- [ ] ERankDb.sql (신규 생성)
- [ ] 수정된 테이블 SQL (12개 → 13개)
  - 신규: ERankDb.sql
  - 수정: UserDb.sql, AgreementDb.sql, SubAgreementDb.sql, ProcessDb.sql, ReportDb.sql, TasksDb.sql, TotalReportDb.sql, EvaluationUsers.sql
  - 검토만: MemberDb.sql, DeptObjectiveDb.sql, EvaluationLists.sql
- [ ] 수정된 뷰 SQL (5개)
- [ ] 01_CreateTables.sql (통합 테이블 생성)
- [ ] 02_CreateViews.sql (통합 뷰 생성)
- [ ] 03_SeedData.sql (초기 데이터 - 직급 마스터 포함)
- [ ] 04_TestData.sql (테스트 데이터)
- [ ] 05_TestQueries.sql (검증 쿼리)

### 9.3. 테스트 결과
- [ ] 테스트 결과 보고서 (별도 md 파일)

---

## 10. 다음 단계 (Phase 2 준비)

Phase 1 완료 후:
- [ ] 개발자 최종 검토 및 승인
- [ ] Phase 2 작업지시서 작성 (Model 개발)
- [ ] Repository 인터페이스 설계

---

## 11. 작업 진행 상황

### 현재 상태: 작업 준비 완료
- [x] 2025년 SQL 파일 복사 완료
- [x] 추가 파일 분석 완료 (EvaluationUsers, EvaluationLists, DeptObjectiveDb)
- [ ] ERD 작성 대기
- [ ] 테이블 스크립트 수정 대기

### 개발자 확인 완료 사항
1. [x] 현재 Database/dbo 파일들 내용 확인 완료
   - EvaluationUsers: 평가 대상자 및 평가자(부서장, 임원) 지정 테이블 - 외래키 수정 필요
   - EvaluationLists: 평가 항목 마스터 데이터 - 수정 불필요
   - DeptObjectiveDb: 부서 목표 테이블 - 이미 외래키 사용 중
2. [x] ERD 작성 방식: Mermaid 사용 (수정 가능)
3. [x] 기존 2025년 데이터 마이그레이션: 불필요 (개발자가 직접 초기 데이터 입력 예정)
4. [ ] 작업지시서 검토 후 작업 시작 승인 대기

---

## 12. 예상 소요 작업

### 12.1. 분석 및 설계
- 추가 파일 분석
- ERD 작성
- 관계 정의

### 12.2. 스크립트 수정
- 테이블 12개 수정
- 뷰 5개 수정
- 통합 스크립트 작성

### 12.3. 테스트
- 단위 테스트
- 통합 테스트
- 시나리오 테스트

---

**작성 완료**: 2025-12-16
**검토 대기 중**
**다음 작업**: 추가 파일 분석 및 ERD 작성
