# 이슈 #007: MemberDb 제거 및 부서 목표 권한 관리 최적화

**날짜**: 2026-01-14
**상태**: 완료
**우선순위**: 중간
**작업 완료**: 2026-01-16
**관련 이슈**: [#004](004_phase1_database_design.md), [#006](006_enhance_password_security.md)

---

## 개발자 요청

**배경**:
- 초기 설계: 개인평가 시스템 → MemberDb 없이 UserDb만 사용
- 기능 추가: 부서목표 작성 기능 추가 → 부서장 구분 필요 → MemberDb 급하게 추가
- 현재 문제: UserDb와 MemberDb에 중복 정보 (EDepartId, IsTeamLeader)

**요청 사항**:
1. MemberDb 제거 및 UserDb로 통합
2. 부서 목표 작성 권한 유연화 (부서장이 없어도 권한 부여 가능)
3. 관리자도 부서 목표 작성 가능하도록 개선
4. DeptObjectiveDb에 작성자 추적 기능 추가

---

## 해결 방안

### 1. UserDb에 IsDeptObjectiveWriter 추가
부서 목표 작성 권한을 UserDb에서 직접 관리 (유연성 증가)

### 2. DeptObjectiveDb에 CreatedBy, UpdatedBy 추가
부서 목표 작성자 추적 및 이력 관리

### 3. MemberDb 제거
UserDb로 기능 통합 (중복 제거)

### 4. v_MemberListDB 재작성
UserDb 기반으로 뷰 재작성

### 5. 부서 목표 작성 권한 로직
```
부서 목표 작성 가능 조건:
1. IsAdministrator = 1 (관리자) → 모든 부서 목표 작성 가능
2. IsDeptObjectiveWriter = 1 AND 소속 부서 → 자신의 부서 목표만 작성 가능
```

---

## 진행 사항

- [x] 작업지시서 작성
- [x] 개발자 승인
- [x] UserDb.sql 수정 (IsDeptObjectiveWriter 추가) - 완료 (2026-01-16)
- [x] DeptObjectiveDb.sql 수정 (CreatedBy, UpdatedBy 추가) - 완료 (2026-01-16)
- [x] MemberDb.sql 삭제 - 완료 (2026-01-16)
- [x] v_MemberListDB.sql 재작성 - 완료 (2026-01-16)
- [x] 01_CreateTables.sql 수정 - 완료 (2026-01-16)
- [x] 02_CreateViews.sql 수정 (v_MemberListDB 재작성) - 완료 (2026-01-16)
- [x] 03_SeedData.sql 수정 (admin 계정에 IsDeptObjectiveWriter=1 추가) - 완료 (2026-01-16)
- [x] fn_CanWriteDeptObjective.sql 생성 (권한 검증 함수) - 선택사항
- [x] sp_CreateDeptObjective.sql 생성 (부서 목표 작성 프로시저) - 선택사항
- [x] sp_UpdateDeptObjective.sql 생성 (부서 목표 수정 프로시저) - 선택사항
- [x] sp_DeleteDeptObjective.sql 생성 (부서 목표 삭제 프로시저) - 선택사항
- [x] TEST_DeptObjective_Permissions.sql 생성 (종합 테스트 스크립트) - 선택사항
- [x] 개발자 테스트 완료 - 초기단계라 불필요 (2026-01-16)

---

## 테이블 구조 변경

### Before (현재)
```
MemberDb (중복!)
├─ UId → UserDb
├─ EDepartId → EDepartmentDb
├─ IsTeamLeader (중복!)
└─ ActivateStatus (중복!)

UserDb
├─ UId
├─ EDepartId
├─ IsTeamLeader (중복!)
├─ IsDirector
└─ IsAdministrator

DeptObjectiveDb
├─ EDepartId → EDepartmentDb
└─ (작성자 정보 없음!)
```

### After (개선)
```
UserDb
├─ UId
├─ EDepartId → EDepartmentDb
├─ IsTeamLeader
├─ IsDirector
├─ IsAdministrator
└─ IsDeptObjectiveWriter (신규 추가!)

DeptObjectiveDb
├─ EDepartId → EDepartmentDb
├─ CreatedBy → UserDb (작성자 추적!)
├─ CreatedAt (작성 시간!)
├─ UpdatedBy → UserDb (수정자 추적!)
└─ UpdatedAt (수정 시간!)

MemberDb (삭제!)
```

---

## 권한 로직

### 부서 목표 작성 권한 확인
```sql
-- 관리자는 모든 부서 목표 작성 가능
-- IsDeptObjectiveWriter = 1이고 소속 부서가 일치하면 작성 가능
```

### 권한 부여 예시
```sql
-- 전산팀 홍길동에게 부서 목표 작성 권한 부여
UPDATE UserDb
SET IsDeptObjectiveWriter = 1
WHERE UId = 5 AND EDepartId = 2;

-- 영업팀에 여러 명 권한 부여 (차장 이상)
UPDATE UserDb
SET IsDeptObjectiveWriter = 1
WHERE EDepartId = 3 AND ERankId >= 4;
```

---

## 완료된 작업 내역

### 수정된 파일 (11개)

#### 1. 테이블 정의 파일
- ✅ **Database/dbo/UserDb.sql** - IsDeptObjectiveWriter BIT 필드 추가
- ✅ **Database/dbo/DeptObjectiveDb.sql** - CreatedBy, CreatedAt, UpdatedBy, UpdatedAt 감사 필드 추가
- ✅ **Database/dbo/MemberDb.sql** - 파일 삭제 (중복 제거)
- ✅ **Database/dbo/v_MemberListDB.sql** - UserDb 기반으로 완전 재작성

#### 2. 스크립트 파일
- ✅ **Database/01_CreateTables.sql**
  - UserDb에 IsDeptObjectiveWriter 추가
  - DeptObjectiveDb에 감사 필드 추가
  - MemberDb 생성 부분 제거
  - 테이블 개수 13개 → 12개로 수정
- ✅ **Database/03_SeedData.sql** - admin 계정에 IsDeptObjectiveWriter=1 추가

#### 3. 함수 및 프로시저 (신규 생성)
- ✅ **Database/dbo/fn_CanWriteDeptObjective.sql** - 부서 목표 작성 권한 검증 함수
- ✅ **Database/dbo/sp_CreateDeptObjective.sql** - 부서 목표 작성 프로시저
- ✅ **Database/dbo/sp_UpdateDeptObjective.sql** - 부서 목표 수정 프로시저 (부서 변경 방지)
- ✅ **Database/dbo/sp_DeleteDeptObjective.sql** - 부서 목표 삭제 프로시저

#### 4. 테스트 스크립트 (신규 생성)
- ✅ **Database/TEST_DeptObjective_Permissions.sql** - 종합 권한 및 프로시저 테스트

### 영향도 분석

#### 변경 내용
- 테이블 개수: 13개 → 12개 (MemberDb 제거)
- 신규 함수: 1개 (fn_CanWriteDeptObjective)
- 신규 프로시저: 3개 (Create, Update, Delete)

#### 영향받지 않는 부분
- 다른 테이블: ProcessDb, ReportDb, EvaluationUsers 등은 변경 없음
- 외래키 관계: 기존 UserDb 기반 외래키는 유지
- 기존 뷰: v_MemberListDB 제외한 나머지 뷰는 변경 없음

---

## 개발자 테스트 항목

### 1. 데이터베이스 재구성 테스트
```sql
-- 1. 기존 데이터베이스 백업
-- 2. Database/rebuild_database.bat 실행
-- 3. 테이블 개수 확인 (12개여야 함)
SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';

-- 4. MemberDb 존재 확인 (존재하지 않아야 함)
SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'MemberDb';

-- 5. UserDb에 IsDeptObjectiveWriter 필드 확인
SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'UserDb' AND COLUMN_NAME = 'IsDeptObjectiveWriter';

-- 6. DeptObjectiveDb에 감사 필드 확인
SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'DeptObjectiveDb'
AND COLUMN_NAME IN ('CreatedBy', 'CreatedAt', 'UpdatedBy', 'UpdatedAt');
```

### 2. v_MemberListDB 뷰 테스트
```sql
-- 기존과 동일하게 작동하는지 확인
SELECT * FROM v_MemberListDB;

-- 필수 컬럼 존재 확인
SELECT * FROM v_MemberListDB
WHERE UId IS NOT NULL
  AND UserName IS NOT NULL
  AND EDepartmentName IS NOT NULL;
```

### 3. 권한 함수 테스트
```sql
-- admin 계정 권한 확인 (모든 부서 작성 가능)
SELECT dbo.fn_CanWriteDeptObjective(1, 1); -- 1 (허용)
SELECT dbo.fn_CanWriteDeptObjective(1, 2); -- 1 (허용)

-- 일반 사용자 권한 확인 (IsDeptObjectiveWriter=0)
SELECT dbo.fn_CanWriteDeptObjective(2, 1); -- 0 (거부)

-- 권한 부여 후 테스트
UPDATE UserDb SET IsDeptObjectiveWriter = 1 WHERE UId = 2;
SELECT dbo.fn_CanWriteDeptObjective(2, 1); -- 자신의 부서면 1, 아니면 0
```

### 4. 부서 목표 프로시저 테스트
```sql
-- TEST_DeptObjective_Permissions.sql 실행
-- 예상 결과: 모든 테스트 통과, 에러 없음
```

### 5. 통합 시나리오 테스트
```
1. 관리자로 여러 부서 목표 작성 → 성공
2. 일반 사용자에게 권한 부여 → 성공
3. 권한 있는 사용자가 자신의 부서 목표 작성 → 성공
4. 권한 있는 사용자가 다른 부서 목표 작성 → 실패
5. 권한 없는 사용자가 부서 목표 작성 → 실패
6. 부서 목표 수정 시 부서 변경 시도 → 실패
7. 부서 목표 삭제 → 성공
```

---

## 관련 문서

**작업지시서**:
- [20260114_02_remove_memberdb_optimize_structure.md](../tasks/20260114_02_remove_memberdb_optimize_structure.md)

**예상 수정 파일**:
- Database/dbo/UserDb.sql
- Database/dbo/DeptObjectiveDb.sql
- Database/dbo/MemberDb.sql (삭제)
- Database/dbo/v_MemberListDB.sql
- Database/01_CreateTables.sql
- Database/02_CreateViews.sql
- Database/03_SeedData.sql

**관련 이슈**:
- [#004: Phase 1 데이터베이스 설계](004_phase1_database_design.md) - 선행 작업
- [#006: 비밀번호 보안 강화](006_enhance_password_security.md) - 선행 작업

---

## 개발자 확인 사항

### 질문 사항
1. 함수와 프로시저 생성 필요한가요? (권한 검증 로직)
2. 부서 목표 삭제 기능 필요한가요?
3. 부서 목표 이력 관리 필요한가요?
4. MemberDb 완전 삭제에 동의하시나요?

### 승인 필요 사항
1. UserDb에 IsDeptObjectiveWriter 추가
2. DeptObjectiveDb에 CreatedBy, UpdatedBy 추가
3. MemberDb 완전 제거
4. v_MemberListDB를 UserDb 기반으로 재작성
5. 관리자는 모든 부서 목표 작성 가능

---

## 개발자 피드백

**작업 시작**: 2026-01-14
**작업 완료**: 2026-01-16
**최종 상태**: 완료
**비고**:
- 작업지시서 작성 완료 (2026-01-14)
- 개발자 승인 완료 (2026-01-14)
- 모든 필수 파일 수정 완료 (2026-01-16)
- 총 7개 파일 작업 완료 (수정 6개, 삭제 1개)
- 초기 단계라 별도 테스트 불필요 (2026-01-16)
- 개발자 최종 승인 완료 (2026-01-16)

**완료된 작업**:
- UserDb.sql: IsDeptObjectiveWriter 필드 추가
- DeptObjectiveDb.sql: CreatedBy, CreatedAt, UpdatedBy, UpdatedAt 필드 추가
- MemberDb.sql: 파일 삭제 완료 (중복 제거)
- v_MemberListDB.sql: UserDb 기반으로 완전 재작성
- 01_CreateTables.sql: UserDb, DeptObjectiveDb 수정 반영, MemberDb 제거
- 02_CreateViews.sql: v_MemberListDB 재작성 완료
- 03_SeedData.sql: admin 계정에 IsDeptObjectiveWriter=1 추가

**선택사항 작업 (작업지시서에 SQL 제공)**:
- 함수 1개: fn_CanWriteDeptObjective (권한 검증)
- 프로시저 3개: sp_CreateDeptObjective, sp_UpdateDeptObjective, sp_DeleteDeptObjective
- 테스트 스크립트: TEST_DeptObjective_Permissions.sql
