# 작업지시서: MemberDb 제거 및 부서 목표 권한 관리 최적화

**날짜**: 2026-01-14
**작업 유형**: DB 구조 개선 - 중복 테이블 제거 및 권한 관리 유연화
**우선순위**: 중간
**예상 작업 범위**: 중간 (5개 파일 수정)
**관련 이슈**: [#007: MemberDb 제거 및 부서 목표 권한 관리 최적화](../issues/007_remove_memberdb_optimize_structure.md)

---

## 1. 작업 배경

### 히스토리
1. **초기 설계**: 개인평가 시스템 → MemberDb 없이 UserDb만 사용
2. **기능 추가**: 부서목표 작성 기능 추가 → 부서장 구분 필요 → MemberDb 급하게 추가
3. **현재 문제**:
   - UserDb와 MemberDb에 중복 정보 (EDepartId, IsTeamLeader)
   - 외래키 관계 정리 후 MemberDb가 불필요해짐
   - 부서별 부서장이 없을 수 있음 (유연성 필요)

### 핵심 요구사항
- **부서 목표 작성 권한 유연화**: 부서장이 없어도 권한 부여 가능
- **관리자도 부서 목표 작성 가능**: IsAdministrator = 1인 경우도 작성 가능
- **효율적인 구조**: 중복 제거 및 데이터 일관성 확보

---

## 2. 해결 방안

### 2-1. UserDb에 IsDeptObjectiveWriter 추가
부서 목표 작성 권한을 UserDb에서 직접 관리 (유연성 증가)

### 2-2. DeptObjectiveDb에 CreatedBy 추가
부서 목표 작성자 추적 및 이력 관리

### 2-3. MemberDb 제거
UserDb로 기능 통합 (중복 제거)

### 2-4. v_MemberListDB 재작성
UserDb 기반으로 뷰 재작성

### 2-5. 부서 목표 작성 권한 로직
```
부서 목표 작성 가능 조건:
1. IsAdministrator = 1 (관리자) → 모든 부서 목표 작성 가능
2. IsDeptObjectiveWriter = 1 AND 소속 부서 → 자신의 부서 목표만 작성 가능
```

---

## 3. 작업 내용

### 3-1. UserDb.sql 수정

**파일**: [Database/dbo/UserDb.sql](../../Database/dbo/UserDb.sql)

**변경 전**:
```sql
-- [13] 관리자 여부 설정
[IsAdministrator] BIT NOT NULL DEFAULT 0,
```

**변경 후**:
```sql
-- [13] 관리자 여부 설정
[IsAdministrator] BIT NOT NULL DEFAULT 0,
-- [14] 부서 목표 작성 권한
[IsDeptObjectiveWriter] BIT NOT NULL DEFAULT 0,
```

**필드 설명**:
- `IsDeptObjectiveWriter = 1`: 자신의 소속 부서(EDepartId)의 부서 목표 작성 권한
- 부서장, 부서 담당자 등에게 권한 부여 가능
- 부서별로 여러 명에게 권한 부여 가능

---

### 3-2. DeptObjectiveDb.sql 수정

**파일**: [Database/dbo/DeptObjectiveDb.sql](../../Database/dbo/DeptObjectiveDb.sql)

**변경 전**:
```sql
CREATE TABLE [dbo].[DeptObjectiveDb]
(
    [DeptObjectiveDbId] BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    [EDepartId] BIGINT NOT NULL,
    [ObjectiveTitle] NVARCHAR(MAX) NOT NULL,
    [ObjectiveContents] NVARCHAR(MAX) NOT NULL,
    [Remarks] NVARCHAR(MAX),
    FOREIGN KEY (EDepartId) REFERENCES [dbo].[EDepartmentDb](EDepartId)
);
```

**변경 후**:
```sql
CREATE TABLE [dbo].[DeptObjectiveDb]
(
    -- Primary Key
    -- 자동 ID생성
    -- [01] DeptObjectiveDb id
    [DeptObjectiveDbId] BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    -- [02] EDepartmentDb id
    [EDepartId] BIGINT NOT NULL,
    -- [03] ObjectiveTitle
    [ObjectiveTitle] NVARCHAR(MAX) NOT NULL,
    -- [04] ObjectiveContents
    [ObjectiveContents] NVARCHAR(MAX) NOT NULL,
    -- [05] 작성자 (외래키)
    [CreatedBy] BIGINT NOT NULL,
    -- [06] 작성일시
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    -- [07] 수정자 (외래키)
    [UpdatedBy] BIGINT NULL,
    -- [08] 수정일시
    [UpdatedAt] DATETIME NULL,
    -- [99] 비고
    [Remarks] NVARCHAR(MAX),

    -- 외래키 제약조건
    CONSTRAINT FK_DeptObjectiveDb_EDepartmentDb
        FOREIGN KEY (EDepartId)
        REFERENCES [dbo].[EDepartmentDb](EDepartId)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,

    CONSTRAINT FK_DeptObjectiveDb_UserDb_CreatedBy
        FOREIGN KEY (CreatedBy)
        REFERENCES [dbo].[UserDb](UId)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,

    CONSTRAINT FK_DeptObjectiveDb_UserDb_UpdatedBy
        FOREIGN KEY (UpdatedBy)
        REFERENCES [dbo].[UserDb](UId)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);
```

---

### 3-3. MemberDb.sql 삭제

**파일**: [Database/dbo/MemberDb.sql](../../Database/dbo/MemberDb.sql)

이 파일은 삭제 예정 (작업 시점에 백업 후 삭제)

---

### 3-4. v_MemberListDB.sql 재작성

**파일**: [Database/dbo/v_MemberListDB.sql](../../Database/dbo/v_MemberListDB.sql)

**변경 전** (MemberDb 사용):
```sql
CREATE VIEW [dbo].[v_MemberListDB]
AS SELECT
    A.MemberId,
    A.Uid,
    B.UserId,
    B.UserName,
    R.ERankName AS ERank,
    C.EDepartId,
    C.EDepartmentName,
    A.ActivateStatus,
    A.IsTeamLeader,
    A.Remarks
FROM [dbo].[MemberDb] A
INNER JOIN [dbo].[UserDb] B ON A.Uid = B.Uid
INNER JOIN [dbo].[EDepartmentDb] C ON A.EDepartId = C.EDepartId
LEFT JOIN [dbo].[ERankDb] R ON B.ERankId = R.ERankId
```

**변경 후** (UserDb 사용):
```sql
CREATE VIEW [dbo].[v_MemberListDB]
AS SELECT
    U.UId,
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
WHERE U.EStatus = 1;  -- 재직 중인 사람만
```

---

### 3-5. 01_CreateTables.sql 수정

**파일**: [Database/01_CreateTables.sql](../../Database/01_CreateTables.sql)

**수정 사항**:

#### (1) UserDb 생성 부분 수정 (Line 106-148)

**변경 전**:
```sql
-- [13] 관리자 여부 설정
[IsAdministrator] BIT NOT NULL DEFAULT 0,
```

**변경 후**:
```sql
-- [13] 관리자 여부 설정
[IsAdministrator] BIT NOT NULL DEFAULT 0,
-- [14] 부서 목표 작성 권한
[IsDeptObjectiveWriter] BIT NOT NULL DEFAULT 0,
```

#### (2) MemberDb 생성 부분 제거 (Line 160-184)
전체 삭제

#### (3) DeptObjectiveDb 생성 부분 수정 (Line 186-205)
CreatedBy, CreatedAt, UpdatedBy, UpdatedAt 추가

#### (4) DROP 순서에서 MemberDb 제거 (Line 31-32)

#### (5) 완료 메시지 수정 (Line 588-602)
- MemberDb 제거
- 총 12개 테이블로 변경

---

### 3-6. 02_CreateViews.sql 수정

**파일**: [Database/02_CreateViews.sql](../../Database/02_CreateViews.sql)

**수정 사항**:
- v_MemberListDB 뷰 정의를 3-4의 새 버전으로 교체

---

### 3-7. 03_SeedData.sql 수정

**파일**: [Database/03_SeedData.sql](../../Database/03_SeedData.sql)

**수정 사항**:

관리자 계정 INSERT 수정:
```sql
INSERT INTO [dbo].[UserDb]
    ([UId], [UserId], [UserName], [UserPassword], [UserPasswordSalt], [ENumber], [Email],
     [EDepartId], [ERankId], [EStatus], [IsTeamLeader], [IsDirector], [IsAdministrator], [IsDeptObjectiveWriter])
VALUES
    (1, 'admin', N'시스템관리자', @AdminPassword, @AdminSalt, 'A0001', 'admin@company.com',
     1, 9, 1, 0, 0, 1, 1);  -- IsDeptObjectiveWriter = 1 추가
```

---

## 4. 부서 목표 작성 권한 로직

### 4-1. 권한 검증 쿼리 (함수 또는 프로시저)

```sql
-- 부서 목표 작성 권한 확인 함수
CREATE FUNCTION dbo.fn_CanWriteDeptObjective
(
    @UserId BIGINT,
    @EDepartId BIGINT
)
RETURNS BIT
AS
BEGIN
    DECLARE @CanWrite BIT = 0;

    -- 1. 관리자는 모든 부서 목표 작성 가능
    IF EXISTS (
        SELECT 1 FROM UserDb
        WHERE UId = @UserId
          AND IsAdministrator = 1
          AND EStatus = 1
    )
    BEGIN
        SET @CanWrite = 1;
        RETURN @CanWrite;
    END

    -- 2. IsDeptObjectiveWriter = 1이고 소속 부서가 일치하면 작성 가능
    IF EXISTS (
        SELECT 1 FROM UserDb
        WHERE UId = @UserId
          AND EDepartId = @EDepartId
          AND IsDeptObjectiveWriter = 1
          AND EStatus = 1
    )
    BEGIN
        SET @CanWrite = 1;
    END

    RETURN @CanWrite;
END
GO
```

### 4-2. 부서 목표 작성 저장 프로시저

```sql
CREATE PROCEDURE sp_CreateDeptObjective
    @EDepartId BIGINT,
    @ObjectiveTitle NVARCHAR(MAX),
    @ObjectiveContents NVARCHAR(MAX),
    @LoginUserId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. 권한 검증
    IF dbo.fn_CanWriteDeptObjective(@LoginUserId, @EDepartId) = 0
    BEGIN
        RAISERROR('권한 없음: 부서 목표 작성 권한이 없습니다.', 16, 1);
        RETURN;
    END

    -- 2. 부서 목표 저장
    INSERT INTO DeptObjectiveDb (EDepartId, ObjectiveTitle, ObjectiveContents, CreatedBy)
    VALUES (@EDepartId, @ObjectiveTitle, @ObjectiveContents, @LoginUserId);

    PRINT '부서 목표 작성 완료';
END
GO
```

### 4-3. 부서 목표 수정 저장 프로시저

```sql
CREATE PROCEDURE sp_UpdateDeptObjective
    @DeptObjectiveDbId BIGINT,
    @ObjectiveTitle NVARCHAR(MAX),
    @ObjectiveContents NVARCHAR(MAX),
    @LoginUserId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @EDepartId BIGINT;

    -- 기존 목표의 부서 ID 조회
    SELECT @EDepartId = EDepartId
    FROM DeptObjectiveDb
    WHERE DeptObjectiveDbId = @DeptObjectiveDbId;

    IF @EDepartId IS NULL
    BEGIN
        RAISERROR('부서 목표를 찾을 수 없습니다.', 16, 1);
        RETURN;
    END

    -- 권한 검증
    IF dbo.fn_CanWriteDeptObjective(@LoginUserId, @EDepartId) = 0
    BEGIN
        RAISERROR('권한 없음: 부서 목표 수정 권한이 없습니다.', 16, 1);
        RETURN;
    END

    -- 부서 목표 수정
    UPDATE DeptObjectiveDb
    SET ObjectiveTitle = @ObjectiveTitle,
        ObjectiveContents = @ObjectiveContents,
        UpdatedBy = @LoginUserId,
        UpdatedAt = GETDATE()
    WHERE DeptObjectiveDbId = @DeptObjectiveDbId;

    PRINT '부서 목표 수정 완료';
END
GO
```

---

## 5. 사용 예시

### 5-1. 권한 부여

```sql
-- 전산팀 홍길동에게 부서 목표 작성 권한 부여
UPDATE UserDb
SET IsDeptObjectiveWriter = 1
WHERE UId = 5
  AND EDepartId = 2;  -- 전산팀

-- 영업팀에 여러 명 권한 부여 (팀장, 차장 모두)
UPDATE UserDb
SET IsDeptObjectiveWriter = 1
WHERE EDepartId = 3  -- 영업팀
  AND ERankId >= 4;  -- 차장 이상
```

### 5-2. 권한 조회

```sql
-- 부서별 목표 작성 권한자 조회
SELECT
    D.EDepartmentName AS 부서명,
    U.UserName AS 이름,
    R.ERankName AS 직급,
    CASE
        WHEN U.IsAdministrator = 1 THEN '관리자'
        WHEN U.IsDeptObjectiveWriter = 1 THEN '부서 목표 작성자'
        ELSE '-'
    END AS 권한
FROM UserDb U
LEFT JOIN EDepartmentDb D ON U.EDepartId = D.EDepartId
LEFT JOIN ERankDb R ON U.ERankId = R.ERankId
WHERE (U.IsDeptObjectiveWriter = 1 OR U.IsAdministrator = 1)
  AND U.EStatus = 1
ORDER BY D.EDepartmentNo, U.ERankId DESC;
```

### 5-3. 부서 목표 작성 (권한 검증 포함)

```sql
-- 방법 1: 직접 INSERT (권한 확인)
DECLARE @LoginUserId BIGINT = 5;  -- 홍길동
DECLARE @EDepartId BIGINT = 2;    -- 전산팀

INSERT INTO DeptObjectiveDb (EDepartId, ObjectiveTitle, ObjectiveContents, CreatedBy)
SELECT @EDepartId, N'2026년 전산팀 목표', N'시스템 안정화 및 신규 서비스 개발', @LoginUserId
WHERE dbo.fn_CanWriteDeptObjective(@LoginUserId, @EDepartId) = 1;

-- 방법 2: 저장 프로시저 사용 (권한 자동 검증)
EXEC sp_CreateDeptObjective
    @EDepartId = 2,
    @ObjectiveTitle = N'2026년 전산팀 목표',
    @ObjectiveContents = N'시스템 안정화 및 신규 서비스 개발',
    @LoginUserId = 5;
```

### 5-4. 관리자의 모든 부서 목표 작성

```sql
-- 관리자(admin)가 모든 부서 목표 작성 가능
DECLARE @AdminUserId BIGINT = 1;

-- 경영지원팀 목표 작성
EXEC sp_CreateDeptObjective
    @EDepartId = 1,
    @ObjectiveTitle = N'2026년 경영지원팀 목표',
    @ObjectiveContents = N'경영 지원 업무 효율화',
    @LoginUserId = @AdminUserId;

-- 전산팀 목표 작성
EXEC sp_CreateDeptObjective
    @EDepartId = 2,
    @ObjectiveTitle = N'2026년 전산팀 목표',
    @ObjectiveContents = N'시스템 안정화',
    @LoginUserId = @AdminUserId;
```

### 5-5. 부서 목표 조회 (작성자 정보 포함)

```sql
SELECT
    DO.DeptObjectiveDbId,
    D.EDepartmentName AS 부서명,
    DO.ObjectiveTitle AS 목표제목,
    DO.ObjectiveContents AS 목표내용,
    U1.UserName AS 작성자,
    DO.CreatedAt AS 작성일시,
    U2.UserName AS 수정자,
    DO.UpdatedAt AS 수정일시
FROM DeptObjectiveDb DO
INNER JOIN EDepartmentDb D ON DO.EDepartId = D.EDepartId
INNER JOIN UserDb U1 ON DO.CreatedBy = U1.UId
LEFT JOIN UserDb U2 ON DO.UpdatedBy = U2.UId
ORDER BY D.EDepartmentNo, DO.CreatedAt DESC;
```

---

## 6. 테이블 구조 변경 요약

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
└─ (권한 관리 불가!)
```

### After (개선)
```
UserDb
├─ UId
├─ EDepartId → EDepartmentDb
├─ IsTeamLeader
├─ IsDirector
├─ IsAdministrator
└─ IsDeptObjectiveWriter ✅ 신규 추가!

DeptObjectiveDb
├─ EDepartId → EDepartmentDb
├─ CreatedBy → UserDb ✅ 작성자 추적!
├─ CreatedAt ✅ 작성 시간!
├─ UpdatedBy → UserDb ✅ 수정자 추적!
└─ UpdatedAt ✅ 수정 시간!

MemberDb ❌ 삭제!

권한 로직:
✅ 관리자(IsAdministrator=1) → 모든 부서 목표 작성 가능
✅ 부서 목표 작성자(IsDeptObjectiveWriter=1) → 자기 부서만 작성 가능
✅ 부서별 여러 명에게 권한 부여 가능
✅ 부서장이 없어도 OK
```

---

## 7. 영향도 분석

### 7-1. 수정 대상 파일
| 파일 | 작업 | 영향도 |
|------|------|--------|
| UserDb.sql | IsDeptObjectiveWriter 추가 | 중간 |
| DeptObjectiveDb.sql | CreatedBy, UpdatedBy 추가 | 중간 |
| MemberDb.sql | 삭제 | 높음 |
| v_MemberListDB.sql | 완전 재작성 | 높음 |
| 01_CreateTables.sql | 3곳 수정 | 중간 |
| 02_CreateViews.sql | 1곳 수정 | 낮음 |
| 03_SeedData.sql | IsDeptObjectiveWriter 추가 | 낮음 |

### 7-2. 영향받지 않는 부분
- ✅ **다른 테이블**: ProcessDb, ReportDb, EvaluationUsers 등은 변경 없음
- ✅ **외래키 관계**: 기존 UserDb 기반 외래키는 유지
- ✅ **EDepartmentDb**: 수정 불필요 (더 간단한 구조)

---

## 8. 테스트 계획

### 8-1. 스키마 테스트
```sql
-- 1. UserDb 구조 확인
EXEC sp_help 'UserDb';
-- IsDeptObjectiveWriter 컬럼 존재 확인

-- 2. DeptObjectiveDb 구조 확인
EXEC sp_help 'DeptObjectiveDb';
-- CreatedBy, UpdatedBy 컬럼 존재 확인

-- 3. MemberDb 삭제 확인
SELECT * FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME = 'MemberDb';
-- 결과: 0 rows
```

### 8-2. 권한 부여 테스트
```sql
-- 테스트 사용자 생성
INSERT INTO UserDb (UserId, UserName, UserPassword, UserPasswordSalt, EDepartId, ERankId, IsDeptObjectiveWriter)
VALUES ('test01', N'홍길동', 0x00, 0x00, 2, 5, 1);

-- 권한 확인
SELECT dbo.fn_CanWriteDeptObjective(SCOPE_IDENTITY(), 2) AS CanWrite;
-- 결과: 1 (권한 있음)
```

### 8-3. 부서 목표 작성 권한 테스트

**성공 케이스 1: 권한 보유자**
```sql
DECLARE @UserId BIGINT = (SELECT UId FROM UserDb WHERE UserId = 'test01');

EXEC sp_CreateDeptObjective
    @EDepartId = 2,
    @ObjectiveTitle = N'테스트 목표',
    @ObjectiveContents = N'테스트 내용',
    @LoginUserId = @UserId;
-- 결과: 성공
```

**성공 케이스 2: 관리자**
```sql
DECLARE @AdminUserId BIGINT = 1;

EXEC sp_CreateDeptObjective
    @EDepartId = 3,  -- 다른 부서도 작성 가능
    @ObjectiveTitle = N'관리자 작성',
    @ObjectiveContents = N'관리자는 모든 부서 가능',
    @LoginUserId = @AdminUserId;
-- 결과: 성공
```

**실패 케이스: 권한 없음**
```sql
DECLARE @NormalUserId BIGINT = 999;  -- 권한 없는 사용자

EXEC sp_CreateDeptObjective
    @EDepartId = 2,
    @ObjectiveTitle = N'불법 작성',
    @ObjectiveContents = N'실패해야 함',
    @LoginUserId = @NormalUserId;
-- 결과: 오류 (권한 없음)
```

### 8-4. v_MemberListDB 뷰 테스트
```sql
-- 전체 조회
SELECT * FROM v_MemberListDB
ORDER BY EDepartmentNo;

-- 부서 목표 작성 권한자만 조회
SELECT * FROM v_MemberListDB
WHERE IsDeptObjectiveWriter = 1 OR IsAdministrator = 1;
```

---

## 9. 롤백 계획

### 문제 발생 시
1. **MemberDb.sql 백업 파일 복원**
2. **UserDb, DeptObjectiveDb 원복**
   - IsDeptObjectiveWriter, CreatedBy 컬럼 제거
3. **01_CreateTables.sql 이전 버전으로 복원**

---

## 10. 체크리스트

### 작업 전
- [ ] 현재 DB 전체 백업
- [ ] MemberDb.sql 파일 백업
- [ ] 개발자 승인 확인

### 파일 수정
- [ ] UserDb.sql 수정 (IsDeptObjectiveWriter 추가)
- [ ] DeptObjectiveDb.sql 수정 (CreatedBy, UpdatedBy 추가)
- [ ] MemberDb.sql 삭제 (백업 보관)
- [ ] v_MemberListDB.sql 재작성
- [ ] 01_CreateTables.sql 수정
- [ ] 02_CreateViews.sql 수정
- [ ] 03_SeedData.sql 수정
- [ ] fn_CanWriteDeptObjective 함수 생성 (선택사항)
- [ ] sp_CreateDeptObjective 프로시저 생성 (선택사항)
- [ ] sp_UpdateDeptObjective 프로시저 생성 (선택사항)

### 테스트
- [ ] 스키마 생성 테스트
- [ ] 권한 부여 테스트
- [ ] 부서 목표 작성 권한 테스트 (성공/실패 케이스)
- [ ] 관리자의 모든 부서 작성 테스트
- [ ] v_MemberListDB 조회 테스트
- [ ] 외래키 제약조건 동작 확인

### 문서화
- [ ] 변경 내역 기록
- [ ] Phase 1 진행 요약 업데이트

---

## 11. 추가 고려사항

### 11-1. 부서 목표 삭제 권한 (선택사항)
작성자만 삭제 가능하도록:
```sql
CREATE PROCEDURE sp_DeleteDeptObjective
    @DeptObjectiveDbId BIGINT,
    @LoginUserId BIGINT
AS
BEGIN
    DECLARE @EDepartId BIGINT;
    DECLARE @CreatedBy BIGINT;

    SELECT @EDepartId = EDepartId, @CreatedBy = CreatedBy
    FROM DeptObjectiveDb
    WHERE DeptObjectiveDbId = @DeptObjectiveDbId;

    -- 권한 확인: 작성자 또는 관리자만 삭제 가능
    IF @CreatedBy <> @LoginUserId
       AND NOT EXISTS (SELECT 1 FROM UserDb WHERE UId = @LoginUserId AND IsAdministrator = 1)
    BEGIN
        RAISERROR('권한 없음: 작성자만 삭제 가능합니다.', 16, 1);
        RETURN;
    END

    DELETE FROM DeptObjectiveDb
    WHERE DeptObjectiveDbId = @DeptObjectiveDbId;
END
GO
```

### 11-2. 부서 목표 이력 관리 (선택사항)
변경 이력을 추적하려면:
```sql
CREATE TABLE DeptObjectiveHistory
(
    HistoryId BIGINT PRIMARY KEY IDENTITY(1,1),
    DeptObjectiveDbId BIGINT NOT NULL,
    EDepartId BIGINT NOT NULL,
    ObjectiveTitle NVARCHAR(MAX),
    ObjectiveContents NVARCHAR(MAX),
    ChangedBy BIGINT NOT NULL,
    ChangedAt DATETIME NOT NULL DEFAULT GETDATE(),
    ChangeType VARCHAR(10) NOT NULL  -- 'CREATE', 'UPDATE', 'DELETE'
);
```

---

## 12. 개발자 확인 사항

### 질문 사항
1. **함수와 프로시저 생성** 필요한가요? (권한 검증 로직)
2. **부서 목표 삭제 기능** 필요한가요?
3. **부서 목표 이력 관리** 필요한가요?
4. **MemberDb 완전 삭제**에 동의하시나요?

### 승인 필요 사항
1. ✅ UserDb에 IsDeptObjectiveWriter 추가
2. ✅ DeptObjectiveDb에 CreatedBy, UpdatedBy 추가
3. ✅ MemberDb 완전 제거
4. ✅ v_MemberListDB를 UserDb 기반으로 재작성
5. ✅ 관리자는 모든 부서 목표 작성 가능

---

**작성일**: 2026-01-14
**작업 예상 시간**: 1시간 (수정 + 테스트)
**우선순위**: 중간 (효율성 개선 및 유연성 향상)
**다음 단계**: 개발자 승인 후 작업 진행
