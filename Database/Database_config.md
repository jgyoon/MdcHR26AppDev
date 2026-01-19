# MdcHR2026 데이터베이스 설계 문서

**작성일**: 2026-01-16
**데이터베이스**: MdcHR2026
**버전**: 1.0
**목적**: 2026년 인사평가 프로그램

---

## 📋 목차

1. [개요](#개요)
2. [테이블 구조](#테이블-구조)
3. [뷰 구조](#뷰-구조)
4. [테이블 관계도](#테이블-관계도)
5. [주요 특징](#주요-특징)
6. [초기 데이터](#초기-데이터)

---

## 개요

### 데이터베이스 목적
- 직원 개인별 업무 성과 평가 관리
- 부서 목표 설정 및 추적
- 평가 프로세스 자동화
- 종합 리포트 생성

### 주요 기능
- 사용자 및 권한 관리
- 부서 목표 관리
- 개인 업무 합의 및 평가
- 평가 프로세스 워크플로우
- 종합 성과 리포트

---

## 테이블 구조

### 📊 총 12개 테이블

#### 1️⃣ 마스터 데이터 테이블 (2개)

##### **EDepartmentDb** - 부서 마스터
```
역할: 회사 조직도 부서 정보 관리
주요 필드:
  - EDepartId (PK): 부서 ID
  - EDepartmentNo: 부서 번호 (정렬용, UNIQUE)
  - EDepartmentName: 부서명
  - ActivateStatus: 활성화 여부
  - Remarks: 비고
```

##### **ERankDb** - 직급 마스터
```
역할: 직급 정보 관리
주요 필드:
  - ERankId (PK): 직급 ID
  - ERankNo: 직급 번호 (정렬용, UNIQUE)
  - ERankName: 직급명
  - ActivateStatus: 활성화 여부
  - Remarks: 비고
```

---

#### 2️⃣ 사용자 및 권한 테이블 (1개)

##### **UserDb** - 사용자 정보
```
역할: 직원 정보 및 권한 관리
주요 필드:
  - UId (PK): 사용자 ID
  - UserId: 로그인 ID (UNIQUE)
  - UserName: 사용자 이름
  - UserPassword: 비밀번호 (SHA-256 해시)
  - UserPasswordSalt: 비밀번호 Salt (VARBINARY)
  - ENumber: 사원번호
  - Email: 이메일
  - EDepartId (FK): 부서 ID → EDepartmentDb
  - ERankId (FK): 직급 ID → ERankDb
  - EStatus: 활성화 상태
  - IsTeamLeader: 팀장 여부
  - IsDirector: 임원 여부
  - IsAdministrator: 관리자 여부
  - IsDeptObjectiveWriter: 부서 목표 작성 권한
  - Remarks: 비고

보안:
  - SHA-256 해시 + Salt 방식
  - Salt는 사용자별 고유 생성
```

---

#### 3️⃣ 부서 목표 관리 테이블 (1개)

##### **DeptObjectiveDb** - 부서 목표
```
역할: 부서별 연간 목표 관리
주요 필드:
  - DeptObjectiveDbId (PK): 부서 목표 ID
  - EDepartId (FK): 부서 ID → EDepartmentDb
  - Objective: 부서 목표 내용
  - CreatedBy (FK): 작성자 ID → UserDb
  - CreatedAt: 작성 일시
  - UpdatedBy (FK): 수정자 ID → UserDb
  - UpdatedAt: 수정 일시
  - Remarks: 비고

권한:
  - IsAdministrator=1: 모든 부서 목표 작성 가능
  - IsDeptObjectiveWriter=1 AND 소속 부서: 자기 부서 목표만 작성 가능
```

---

#### 4️⃣ 업무 합의 테이블 (2개)

##### **AgreementDb** - 대분류 업무 합의
```
역할: 직원의 주요 업무 카테고리 관리
주요 필드:
  - AgreementId (PK): 합의 ID
  - UId (FK): 사용자 ID → UserDb
  - Evaluation_Department_Number: 평가 부서 번호
  - Evaluation_Department_Name: 평가 부서명
  - Evaluation_Index_Number: 평가 지표 번호
  - Evaluation_Index_Name: 평가 지표명
  - Weight: 가중치 (%)
  - Remarks: 비고
```

##### **SubAgreementDb** - 세부 업무 합의
```
역할: 대분류 업무의 세부 업무 항목 관리
주요 필드:
  - SubAgreementId (PK): 세부 합의 ID
  - AgreementId (FK): 대분류 합의 ID → AgreementDb
  - Evaluation_Task_Number: 평가 작업 번호
  - Evaluation_Task_Name: 평가 작업명
  - Remarks: 비고

관계:
  - AgreementDb : SubAgreementDb = 1 : N
```

---

#### 5️⃣ 평가 프로세스 테이블 (3개)

##### **ProcessDb** - 평가 프로세스
```
역할: 평가 프로세스 관리 (상태, 단계)
주요 필드:
  - ProcessDbId (PK): 프로세스 ID
  - Euid (FK): 평가 대상자 ID → EvaluationUsers
  - ProcessStatus: 프로세스 상태
  - Year: 평가 연도
  - HalfType: 반기 구분 (상반기/하반기)
  - ActivateStatus: 활성화 여부
  - Remarks: 비고
```

##### **ReportDb** - 업무 리포트
```
역할: 직원의 업무 실적 리포트 관리
주요 필드:
  - ReportDbId (PK): 리포트 ID
  - ProcessDbId (FK): 프로세스 ID → ProcessDb
  - SubAgreementId (FK): 세부 합의 ID → SubAgreementDb
  - ResultScore: 결과 점수
  - Weight: 가중치
  - TotalScore: 총점
  - Remarks: 비고

관계:
  - ProcessDb : ReportDb = 1 : N
  - SubAgreementDb : ReportDb = 1 : 1
```

##### **TotalReportDb** - 종합 리포트
```
역할: 평가 기간의 종합 성과 리포트
주요 필드:
  - TotalReportDbId (PK): 종합 리포트 ID
  - ProcessDbId (FK): 프로세스 ID → ProcessDb
  - TotalAverageScore: 총 평균 점수
  - Year: 평가 연도
  - HalfType: 반기 구분
  - Remarks: 비고

관계:
  - ProcessDb : TotalReportDb = 1 : 1
```

---

#### 6️⃣ 보조 테이블 (3개)

##### **EvaluationUsers** - 평가 대상자
```
역할: 평가 대상 직원 관리
주요 필드:
  - Euid (PK): 평가 대상자 ID
  - UId (FK): 사용자 ID → UserDb (UNIQUE)

관계:
  - UserDb : EvaluationUsers = 1 : 1
  - 평가 대상자만 등록
```

##### **EvaluationLists** - 평가 항목 마스터
```
역할: 부서별/프로젝트별 평가 항목 템플릿
주요 필드:
  - Evaluation_Department_Number: 부서 번호
  - Evaluation_Department_Name: 부서명
  - Evaluation_Index_Number: 평가 지표 번호
  - Evaluation_Index_Name: 평가 지표명
  - Evaluation_Task_Number: 평가 작업 번호
  - Evaluation_Task_Name: 평가 작업명
  - Evaluation_Lists_Remark: 비고

특징:
  - 복합키 구조
  - 약 1,000여 건의 평가 항목 템플릿
```

##### **TasksDb** - 업무 작업
```
역할: 개별 업무 작업 추적
주요 필드:
  - TasksDbId (PK): 작업 ID
  - SubAgreementId (FK): 세부 합의 ID → SubAgreementDb
  - Status: 작업 상태
  - TaskName: 작업명
  - Remarks: 비고

관계:
  - SubAgreementDb : TasksDb = 1 : N
```

---

## 뷰 구조

### 📊 총 5개 뷰

#### **v_MemberListDB** - 부서원 목록
```sql
역할: 활성 사용자의 부서/직급 정보 조회
기반 테이블: UserDb, EDepartmentDb, ERankDb
주요 필드:
  - UId, UserId, UserName, ENumber
  - ERank (직급명)
  - EDepartId, EDepartmentName (부서 정보)
  - ActivateStatus, IsTeamLeader, IsDirector
  - IsAdministrator, IsDeptObjectiveWriter
필터링: EStatus = 1 (활성 사용자만)
```

#### **v_DeptObjectiveListDb** - 부서 목표 목록
```sql
역할: 부서 목표와 작성자 정보 조회
기반 테이블: DeptObjectiveDb, EDepartmentDb, UserDb
주요 필드:
  - DeptObjectiveDbId, Objective
  - EDepartId, EDepartmentName
  - CreatedBy, CreatedByName, CreatedAt
  - UpdatedBy, UpdatedByName, UpdatedAt
```

#### **v_ProcessTRListDB** - 평가 프로세스 및 종합 리포트
```sql
역할: 평가 프로세스와 종합 리포트 연계 조회
기반 테이블: ProcessDb, TotalReportDb, EvaluationUsers, UserDb
주요 필드:
  - ProcessDbId, ProcessStatus
  - Year, HalfType
  - Euid, UserId, UserName
  - TotalAverageScore
```

#### **v_ReportTaskListDB** - 평가 리포트 및 업무 목록
```sql
역할: 업무 리포트와 합의 정보 연계 조회
기반 테이블: ReportDb, SubAgreementDb, AgreementDb, ProcessDb
주요 필드:
  - ReportDbId, ResultScore, Weight, TotalScore
  - Evaluation_Department_Name, Evaluation_Index_Name
  - Evaluation_Task_Name
  - ProcessDbId, Year, HalfType
```

#### **v_TotalReportListDB** - 종합 리포트 목록
```sql
역할: 종합 리포트 전체 조회
기반 테이블: TotalReportDb, ProcessDb, EvaluationUsers, UserDb
주요 필드:
  - TotalReportDbId, TotalAverageScore
  - Year, HalfType
  - UserId, UserName
  - ProcessStatus
```

---

## 테이블 관계도

### 계층 구조

```
[마스터 데이터]
    EDepartmentDb (부서)
    ERankDb (직급)
         ↓
    [사용자]
    UserDb ──→ EvaluationUsers (평가 대상자)
         ↓              ↓
    [부서 목표]      [프로세스]
    DeptObjectiveDb  ProcessDb
                         ↓
    [업무 합의]      [리포트]
    AgreementDb      ReportDb
         ↓           TotalReportDb
    SubAgreementDb
         ↓
    [작업]
    TasksDb

[평가 템플릿]
    EvaluationLists (독립적)
```

### 외래키 관계

```
1. UserDb
   ├─ EDepartId → EDepartmentDb.EDepartId
   └─ ERankId → ERankDb.ERankId

2. DeptObjectiveDb
   ├─ EDepartId → EDepartmentDb.EDepartId
   ├─ CreatedBy → UserDb.UId
   └─ UpdatedBy → UserDb.UId

3. EvaluationUsers
   └─ UId → UserDb.UId (UNIQUE)

4. AgreementDb
   └─ UId → UserDb.UId

5. SubAgreementDb
   └─ AgreementId → AgreementDb.AgreementId

6. ProcessDb
   └─ Euid → EvaluationUsers.Euid

7. ReportDb
   ├─ ProcessDbId → ProcessDb.ProcessDbId
   └─ SubAgreementId → SubAgreementDb.SubAgreementId

8. TotalReportDb
   └─ ProcessDbId → ProcessDb.ProcessDbId

9. TasksDb
   └─ SubAgreementId → SubAgreementDb.SubAgreementId
```

### 참조 무결성
- **CASCADE 없음**: 모든 외래키는 기본 제약만 적용
- **삭제 방지**: 참조된 레코드는 삭제 전 참조 해제 필요
- **데이터 일관성**: 외래키로 데이터 무결성 보장

---

## 주요 특징

### 1️⃣ 보안
- **비밀번호 암호화**: SHA-256 해시 + Salt
- **Salt 저장**: 사용자별 고유 Salt (VARBINARY(16))
- **권한 관리**: 다단계 권한 시스템
  - IsAdministrator: 시스템 관리자
  - IsDirector: 임원
  - IsTeamLeader: 팀장
  - IsDeptObjectiveWriter: 부서 목표 작성자

### 2️⃣ 권한 체계
```
부서 목표 작성 권한:
  1. IsAdministrator = 1 → 모든 부서 목표 작성 가능
  2. IsDeptObjectiveWriter = 1 AND 소속 부서 → 자기 부서만
  3. 그 외 → 작성 불가
```

### 3️⃣ 감사 추적
- **작성 추적**: CreatedBy, CreatedAt
- **수정 추적**: UpdatedBy, UpdatedAt
- **이력 관리**: 부서 목표 변경 이력 추적 가능

### 4️⃣ 데이터 정합성
- **UNIQUE 제약**: UserId, EDepartmentNo, ERankNo
- **외래키 제약**: 모든 관계 테이블
- **NOT NULL 제약**: 필수 필드

### 5️⃣ 확장성
- **평가 항목 템플릿**: EvaluationLists로 유연한 평가 항목 관리
- **프로세스 상태 관리**: 다양한 평가 워크플로우 지원
- **반기 평가**: Year + HalfType으로 반기별 평가 관리

---

## 초기 데이터

### Seed 데이터 구성

#### **03_01_SeedData.sql** (336줄)
```
1. EDepartmentDb (부서 20개)
   - 경영지원본부, 전산팀, 영업팀, 개발팀 등

2. ERankDb (직급 14개)
   - 사원, 주임, 대리, 과장, 차장, 부장 등

3. UserDb (사용자 107명)
   - 관리자 계정: mdcadmin (비밀번호: xnd0580!!)
   - 모든 비밀번호 SHA-256 + Salt 암호화
   - IsDeptObjectiveWriter: 부서장/관리자에게 부여

4. EvaluationUsers (평가 대상자 107명)
   - UserDb의 모든 사용자를 평가 대상으로 등록
```

#### **03_05_SeedData_EvaluationLists.sql** (1,098줄)
```
EvaluationLists (약 1,000여 건)
  - 부서별 평가 항목 템플릿
  - 프로젝트별 평가 항목 (52개 프로젝트)
  - 개발기획, 개발, 개발검증, 양산이관 단계별 세부 작업
```

### 관리자 계정
```
UserId: mdcadmin
Password: xnd0580!!
권한:
  - IsAdministrator = 1
  - IsDeptObjectiveWriter = 1
  - 모든 기능 접근 가능
```

---

## 개발 참고사항

### 데이터베이스 재구성
```bash
# Windows
Database\rebuild_database.bat

# 실행 순서
1. 01_CreateTables.sql  → 테이블 생성
2. 02_CreateViews.sql   → 뷰 생성
3. 03_01_SeedData.sql   → 기본 데이터
4. 03_05_SeedData_EvaluationLists.sql → 평가 항목
```

### 주요 쿼리 패턴

#### 사용자 인증
```sql
DECLARE @InputPassword NVARCHAR(255) = N'사용자입력비밀번호';
DECLARE @UserId NVARCHAR(255) = N'로그인ID';

SELECT UId, UserName, IsAdministrator
FROM UserDb
WHERE UserId = @UserId
  AND UserPassword = HASHBYTES('SHA2_256', @InputPassword + CAST(UserPasswordSalt AS NVARCHAR(MAX)))
  AND EStatus = 1;
```

#### 부서 목표 조회
```sql
SELECT *
FROM v_DeptObjectiveListDb
WHERE EDepartId = @부서ID
ORDER BY CreatedAt DESC;
```

#### 평가 프로세스 조회
```sql
SELECT *
FROM v_ProcessTRListDB
WHERE Year = 2026
  AND HalfType = N'상반기'
  AND UserId = @사용자ID;
```

---

## 변경 이력

| 날짜 | 버전 | 변경 내용 | 관련 이슈 |
|------|------|-----------|-----------|
| 2025-12-16 | 0.1 | 초기 데이터베이스 설계 | #004 |
| 2026-01-14 | 0.9 | 비밀번호 보안 강화 (SHA-256 + Salt) | #006 |
| 2026-01-16 | 1.0 | MemberDb 제거, 부서 목표 권한 최적화 | #007 |

---

## 문의 및 지원

**프로젝트**: 2026년 인사평가프로그램
**데이터베이스**: MdcHR2026
**문서 버전**: 1.0
**최종 업데이트**: 2026-01-16
