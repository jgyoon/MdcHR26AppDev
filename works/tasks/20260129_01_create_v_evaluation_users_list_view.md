# 작업지시서 (Part 1): v_EvaluationUsersList 뷰 생성

**날짜**: 2026-01-29
**파일**: `Database/dbo/v_EvaluationUsersList.sql`
**작업 유형**: DB 뷰 생성
**관련 이슈**:
- [#011: Phase 3-3 관리자 페이지 빌드 오류 및 재작업](../issues/011_phase3_3_admin_pages_build_errors.md)
**후속 작업지시서**:
- [20260129_02_implement_v_evaluation_users_list_models.md](20260129_02_implement_v_evaluation_users_list_models.md)

---

## 1. 작업 개요

**목적**:
- EUsersManage 페이지에서 사용자 이름 표시를 위한 DB 뷰 생성

**배경**:
- 현재 EvaluationUsers 테이블에는 Uid (FK)만 있고 UserName이 없음
- UserManage는 v_MemberListDB 뷰를 사용하여 JOIN된 데이터 조회
- 동일한 패턴으로 v_EvaluationUsersList 뷰 필요

**현재 문제**:
- 이름: **미지정** ❌
- 평가부서장: **미지정** ❌
- 평가임원: **미지정** ❌

---

## 2. DB 뷰 생성

### 2.1. 참고: v_MemberListDB 구조

**기존 뷰** (`Database/dbo/v_MemberListDB.sql`):
```sql
CREATE VIEW [dbo].[v_MemberListDB]
AS SELECT
    U.Uid,
    U.UserId,
    U.UserName,
    U.ENumber,
    R.ERankName AS ERank,
    D.EDepartId,
    D.EDepartmentName,
    U.EStatus AS ActivateStatus,
    U.IsTeamLeader,
    U.IsDirector,
    U.IsAdministrator,
    U.IsDeptObjectiveWriter
FROM [dbo].[UserDb] U
LEFT JOIN [dbo].[EDepartmentDb] D ON U.EDepartId = D.EDepartId
LEFT JOIN [dbo].[ERankDb] R ON U.ERankId = R.ERankId
WHERE U.EStatus = 1
```

---

### 2.2. 새 뷰: v_EvaluationUsersList

**파일 생성**: `Database/dbo/v_EvaluationUsersList.sql`

**SQL 스크립트**:
```sql
USE MdcHR2026;
GO

-- 기존 뷰 삭제
IF OBJECT_ID('[dbo].[v_EvaluationUsersList]', 'V') IS NOT NULL
    DROP VIEW [dbo].[v_EvaluationUsersList];
GO

-- 뷰 생성
CREATE VIEW [dbo].[v_EvaluationUsersList]
AS SELECT
    EU.EUid,
    EU.Uid,
    U.UserId,
    U.UserName,
    U.ENumber,
    U.EDepartId,
    D.EDepartmentName,
    R.ERankName AS ERank,
    EU.Is_Evaluation,
    EU.TeamLeaderId,
    TL.UserName AS TeamLeaderName,
    EU.DirectorId,
    DI.UserName AS DirectorName,
    EU.Is_TeamLeader
FROM [dbo].[EvaluationUsers] EU
INNER JOIN [dbo].[UserDb] U ON EU.Uid = U.Uid
LEFT JOIN [dbo].[EDepartmentDb] D ON U.EDepartId = D.EDepartId
LEFT JOIN [dbo].[ERankDb] R ON U.ERankId = R.ERankId
LEFT JOIN [dbo].[UserDb] TL ON EU.TeamLeaderId = TL.Uid
LEFT JOIN [dbo].[UserDb] DI ON EU.DirectorId = DI.Uid
WHERE U.EStatus = 1
GO

PRINT 'v_EvaluationUsersList 뷰 생성 완료';
GO
```

---

### 2.3. 뷰 구조 설명

**주요 필드**:

| 필드명 | 타입 | 설명 | 출처 |
|--------|------|------|------|
| `EUid` | BIGINT | EvaluationUsers PK | EU.EUid |
| `Uid` | BIGINT | 평가대상자 ID (FK) | EU.Uid |
| `UserId` | VARCHAR | 로그인 ID | U.UserId |
| `UserName` | NVARCHAR | 사용자 이름 | U.UserName |
| `ENumber` | VARCHAR | 사원번호 | U.ENumber |
| `EDepartId` | BIGINT | 부서 ID | U.EDepartId |
| `EDepartmentName` | NVARCHAR | 부서명 | D.EDepartmentName |
| `ERank` | NVARCHAR | 직급명 | R.ERankName (별칭) |
| `Is_Evaluation` | BIT | 평가자 여부 | EU.Is_Evaluation |
| `TeamLeaderId` | BIGINT (nullable) | 부서장 ID | EU.TeamLeaderId |
| `TeamLeaderName` | NVARCHAR (nullable) | 부서장 이름 | TL.UserName |
| `DirectorId` | BIGINT (nullable) | 임원 ID | EU.DirectorId |
| `DirectorName` | NVARCHAR (nullable) | 임원 이름 | DI.UserName |
| `Is_TeamLeader` | BIT | 부서장 여부 | EU.Is_TeamLeader |

**JOIN 규칙**:
- EvaluationUsers ↔ UserDb: **INNER JOIN** (필수 관계)
- UserDb ↔ EDepartmentDb: **LEFT JOIN** (nullable)
- UserDb ↔ ERankDb: **LEFT JOIN** (nullable)
- TeamLeader: **LEFT JOIN** (nullable, 부서장 미지정 가능)
- Director: **LEFT JOIN** (nullable, 임원 미지정 가능)

**필터**:
- `WHERE U.EStatus = 1`: 재직자만 조회

---

## 3. 테스트 항목

개발자가 SQL Management Studio에서 테스트:

### Test 1: 뷰 생성 확인

**쿼리**:
```sql
SELECT TABLE_NAME, TABLE_TYPE
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME = 'v_EvaluationUsersList';
```

**확인**: 1건 조회 (TABLE_TYPE = 'VIEW') ✅

---

### Test 2: 전체 데이터 조회

**쿼리**:
```sql
SELECT TOP 10
    EUid,
    Uid,
    UserId,
    UserName,
    EDepartmentName,
    ERank,
    TeamLeaderName,
    DirectorName,
    Is_Evaluation
FROM v_EvaluationUsersList
ORDER BY EUid;
```

**확인**:
- UserName 정상 표시 (윤종국, 관리자 등) ✅
- TeamLeaderName, DirectorName 표시 (있는 경우) ✅
- NULL인 경우 NULL로 표시 ✅

---

### Test 3: 특정 사용자 조회

**쿼리**:
```sql
SELECT * FROM v_EvaluationUsersList
WHERE Uid = 1;
```

**확인**: 해당 사용자 상세 정보 표시 ✅

---

### Test 4: 이름 검색

**쿼리**:
```sql
-- NVARCHAR 검색 시 N'' 프리픽스 필수
SELECT Uid, UserName, TeamLeaderName, DirectorName
FROM v_EvaluationUsersList
WHERE UserName LIKE N'윤%';
```

**확인**: 해당 이름 포함 사용자만 조회 ✅

---

### Test 5: COUNT 확인

**쿼리**:
```sql
-- EvaluationUsers 테이블 개수
SELECT COUNT(*) AS EvaluationUsersCount
FROM EvaluationUsers;

-- 뷰 개수 (재직자만)
SELECT COUNT(*) AS ViewCount
FROM v_EvaluationUsersList;
```

**확인**: ViewCount ≤ EvaluationUsersCount (재직자 필터 적용) ✅

---

## 4. 완료 조건

- [ ] Database/dbo/v_EvaluationUsersList.sql 파일 생성
- [ ] SQL Management Studio에서 스크립트 실행
- [ ] Test 1 성공 (뷰 존재 확인)
- [ ] Test 2 성공 (전체 데이터 조회)
- [ ] Test 3 성공 (특정 사용자 조회)
- [ ] Test 4 성공 (이름 검색)
- [ ] Test 5 성공 (COUNT 확인)

---

## 5. 주의사항

1. **재직자 필터**: `WHERE U.EStatus = 1` 필수
2. **JOIN 순서**: INNER JOIN 먼저, LEFT JOIN 나중에
3. **별칭 일관성**: v_MemberListDB와 동일 패턴 (`ERank` 별칭)
4. **Nullable 처리**: TeamLeaderName, DirectorName은 NULL 허용

---

## 6. 다음 단계

뷰 생성 완료 후:
- [20260129_02_implement_v_evaluation_users_list_models.md](20260129_02_implement_v_evaluation_users_list_models.md) 작업 진행
- Model, Repository, 페이지 수정

---

**작성자**: Claude AI
**검토자**: 개발자
**승인 상태**: 검토 대기
**DB 작업**: 개발자 직접 실행
