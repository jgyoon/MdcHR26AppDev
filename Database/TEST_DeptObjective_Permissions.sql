-- =============================================
-- 부서 목표 권한 관리 테스트 스크립트
-- 작성일: 2026-01-15
-- 설명: MemberDb 제거 및 IsDeptObjectiveWriter 권한 시스템 테스트
-- =============================================

USE [MdcHR2026];
GO

PRINT '=============================================';
PRINT '테스트 시작: 부서 목표 권한 관리 시스템';
PRINT '=============================================';
GO

-- =============================================
-- 1. 스키마 테스트
-- =============================================
PRINT '';
PRINT '=== 1. 스키마 구조 확인 ===';
GO

-- UserDb에 IsDeptObjectiveWriter 컬럼 존재 확인
PRINT '1-1. UserDb.IsDeptObjectiveWriter 컬럼 확인';
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'UserDb'
  AND COLUMN_NAME = 'IsDeptObjectiveWriter';
GO

-- DeptObjectiveDb에 CreatedBy, UpdatedBy 컬럼 존재 확인
PRINT '1-2. DeptObjectiveDb 감사 필드 확인';
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'DeptObjectiveDb'
  AND COLUMN_NAME IN ('CreatedBy', 'CreatedAt', 'UpdatedBy', 'UpdatedAt')
ORDER BY ORDINAL_POSITION;
GO

-- MemberDb 삭제 확인
PRINT '1-3. MemberDb 삭제 확인';
SELECT COUNT(*) AS MemberDbCount
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME = 'MemberDb';
-- 결과: 0이면 정상 삭제
GO

-- v_MemberListDB 뷰 존재 확인
PRINT '1-4. v_MemberListDB 뷰 확인';
SELECT TABLE_NAME, TABLE_TYPE
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME = 'v_MemberListDB';
GO

-- =============================================
-- 2. 테스트 데이터 생성
-- =============================================
PRINT '';
PRINT '=== 2. 테스트 데이터 생성 ===';
GO

-- 테스트용 사용자 생성
PRINT '2-1. 테스트 사용자 생성';

-- 일반 사용자 (권한 없음)
INSERT INTO UserDb
    (UserId, UserName, UserPassword, UserPasswordSalt, ENumber, EDepartId, ERankId, EStatus, IsDeptObjectiveWriter, IsAdministrator)
VALUES
    ('user01', N'홍길동', 0x00, 0x00, 'U0001', 2, 3, 1, 0, 0);  -- 기술개발팀, 과장, 권한 없음

DECLARE @User01Id BIGINT = SCOPE_IDENTITY();

-- 부서 목표 작성 권한 보유자
INSERT INTO UserDb
    (UserId, UserName, UserPassword, UserPasswordSalt, ENumber, EDepartId, ERankId, EStatus, IsDeptObjectiveWriter, IsAdministrator)
VALUES
    ('user02', N'김팀장', 0x00, 0x00, 'U0002', 2, 5, 1, 1, 0);  -- 기술개발팀, 부장, 권한 있음

DECLARE @User02Id BIGINT = SCOPE_IDENTITY();

-- 다른 부서 부서 목표 작성 권한 보유자
INSERT INTO UserDb
    (UserId, UserName, UserPassword, UserPasswordSalt, ENumber, EDepartId, ERankId, EStatus, IsDeptObjectiveWriter, IsAdministrator)
VALUES
    ('user03', N'이차장', 0x00, 0x00, 'U0003', 3, 4, 1, 1, 0);  -- 영업팀, 차장, 권한 있음

DECLARE @User03Id BIGINT = SCOPE_IDENTITY();

PRINT '  - 테스트 사용자 3명 생성 완료';
GO

-- =============================================
-- 3. 권한 검증 함수 테스트
-- =============================================
PRINT '';
PRINT '=== 3. 권한 검증 함수 테스트 ===';
GO

DECLARE @User01Id BIGINT = (SELECT Uid FROM UserDb WHERE UserId = 'user01');
DECLARE @User02Id BIGINT = (SELECT Uid FROM UserDb WHERE UserId = 'user02');
DECLARE @User03Id BIGINT = (SELECT Uid FROM UserDb WHERE UserId = 'user03');
DECLARE @AdminId BIGINT = 1;  -- 관리자

-- 3-1. 관리자 권한 테스트 (모든 부서 작성 가능)
PRINT '3-1. 관리자 권한 테스트';
SELECT
    '관리자 → 기술개발팀(2)' AS 테스트케이스,
    dbo.fn_CanWriteDeptObjective(@AdminId, 2) AS 권한,
    CASE WHEN dbo.fn_CanWriteDeptObjective(@AdminId, 2) = 1 THEN '성공' ELSE '실패' END AS 결과;

SELECT
    '관리자 → 영업팀(3)' AS 테스트케이스,
    dbo.fn_CanWriteDeptObjective(@AdminId, 3) AS 권한,
    CASE WHEN dbo.fn_CanWriteDeptObjective(@AdminId, 3) = 1 THEN '성공' ELSE '실패' END AS 결과;

-- 3-2. 부서 목표 작성 권한자 테스트 (자기 부서만 가능)
PRINT '3-2. 부서 목표 작성 권한자 테스트';
SELECT
    '김팀장(기술개발팀) → 기술개발팀(2)' AS 테스트케이스,
    dbo.fn_CanWriteDeptObjective(@User02Id, 2) AS 권한,
    CASE WHEN dbo.fn_CanWriteDeptObjective(@User02Id, 2) = 1 THEN '성공' ELSE '실패' END AS 결과;

SELECT
    '김팀장(기술개발팀) → 영업팀(3)' AS 테스트케이스,
    dbo.fn_CanWriteDeptObjective(@User02Id, 3) AS 권한,
    CASE WHEN dbo.fn_CanWriteDeptObjective(@User02Id, 3) = 0 THEN '성공 (권한 없음)' ELSE '실패 (권한 있으면 안됨)' END AS 결과;

-- 3-3. 일반 사용자 테스트 (권한 없음)
PRINT '3-3. 일반 사용자 권한 테스트';
SELECT
    '홍길동(일반) → 기술개발팀(2)' AS 테스트케이스,
    dbo.fn_CanWriteDeptObjective(@User01Id, 2) AS 권한,
    CASE WHEN dbo.fn_CanWriteDeptObjective(@User01Id, 2) = 0 THEN '성공 (권한 없음)' ELSE '실패 (권한 있으면 안됨)' END AS 결과;
GO

-- =============================================
-- 4. 부서 목표 작성 프로시저 테스트
-- =============================================
PRINT '';
PRINT '=== 4. 부서 목표 작성 프로시저 테스트 ===';
GO

DECLARE @User02Id BIGINT = (SELECT Uid FROM UserDb WHERE UserId = 'user02');
DECLARE @AdminId BIGINT = 1;

-- 4-1. 성공 케이스: 권한 보유자
PRINT '4-1. 권한 보유자 작성 테스트';
BEGIN TRY
    EXEC sp_CreateDeptObjective
        @EDepartId = 2,
        @ObjectiveTitle = N'2026년 기술개발팀 목표',
        @ObjectiveContents = N'신기술 도입 및 시스템 안정화',
        @LoginUserId = @User02Id;
    PRINT '  결과: 성공';
END TRY
BEGIN CATCH
    PRINT '  결과: 실패 - ' + ERROR_MESSAGE();
END CATCH
GO

-- 4-2. 성공 케이스: 관리자
PRINT '4-2. 관리자 작성 테스트 (다른 부서)';
DECLARE @AdminId BIGINT = 1;
BEGIN TRY
    EXEC sp_CreateDeptObjective
        @EDepartId = 3,
        @ObjectiveTitle = N'2026년 영업팀 목표',
        @ObjectiveContents = N'매출 목표 달성',
        @LoginUserId = @AdminId;
    PRINT '  결과: 성공';
END TRY
BEGIN CATCH
    PRINT '  결과: 실패 - ' + ERROR_MESSAGE();
END CATCH
GO

-- 4-3. 실패 케이스: 권한 없는 사용자
PRINT '4-3. 권한 없는 사용자 작성 테스트 (실패해야 정상)';
DECLARE @User01Id BIGINT = (SELECT Uid FROM UserDb WHERE UserId = 'user01');
BEGIN TRY
    EXEC sp_CreateDeptObjective
        @EDepartId = 2,
        @ObjectiveTitle = N'불법 작성 시도',
        @ObjectiveContents = N'이 목표는 생성되면 안됨',
        @LoginUserId = @User01Id;
    PRINT '  결과: 실패 (생성되면 안됨)';
END TRY
BEGIN CATCH
    PRINT '  결과: 성공 (권한 없어서 차단됨) - ' + ERROR_MESSAGE();
END CATCH
GO

-- =============================================
-- 5. 부서 목표 수정 프로시저 테스트
-- =============================================
PRINT '';
PRINT '=== 5. 부서 목표 수정 프로시저 테스트 ===';
GO

DECLARE @User02Id BIGINT = (SELECT Uid FROM UserDb WHERE UserId = 'user02');
DECLARE @ObjectiveId BIGINT;

-- 수정할 목표 ID 조회
SELECT TOP 1 @ObjectiveId = DeptObjectiveDbId
FROM DeptObjectiveDb
WHERE EDepartId = 2;

-- 5-1. 성공 케이스: 권한 보유자 수정
PRINT '5-1. 권한 보유자 수정 테스트';
BEGIN TRY
    EXEC sp_UpdateDeptObjective
        @DeptObjectiveDbId = @ObjectiveId,
        @EDepartId = 2,
        @ObjectiveTitle = N'2026년 기술개발팀 목표 (수정)',
        @ObjectiveContents = N'신기술 도입 및 시스템 안정화 + 보안 강화',
        @LoginUserId = @User02Id;
    PRINT '  결과: 성공';
END TRY
BEGIN CATCH
    PRINT '  결과: 실패 - ' + ERROR_MESSAGE();
END CATCH
GO

-- 5-2. 실패 케이스: 부서 변경 시도 (db-optimizer 권장사항)
PRINT '5-2. 부서 변경 방지 테스트 (실패해야 정상)';
DECLARE @User02Id BIGINT = (SELECT Uid FROM UserDb WHERE UserId = 'user02');
DECLARE @ObjectiveId BIGINT;

SELECT TOP 1 @ObjectiveId = DeptObjectiveDbId
FROM DeptObjectiveDb
WHERE EDepartId = 2;

BEGIN TRY
    EXEC sp_UpdateDeptObjective
        @DeptObjectiveDbId = @ObjectiveId,
        @EDepartId = 3,  -- 다른 부서로 변경 시도
        @ObjectiveTitle = N'부서 변경 시도',
        @ObjectiveContents = N'이 수정은 실패해야 함',
        @LoginUserId = @User02Id;
    PRINT '  결과: 실패 (부서 변경되면 안됨)';
END TRY
BEGIN CATCH
    PRINT '  결과: 성공 (부서 변경 차단됨) - ' + ERROR_MESSAGE();
END CATCH
GO

-- =============================================
-- 6. 부서 목표 조회 테스트
-- =============================================
PRINT '';
PRINT '=== 6. 부서 목표 조회 테스트 ===';
GO

PRINT '6-1. 생성된 부서 목표 조회 (작성자 정보 포함)';
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
INNER JOIN UserDb U1 ON DO.CreatedBy = U1.Uid
LEFT JOIN UserDb U2 ON DO.UpdatedBy = U2.Uid
ORDER BY D.EDepartmentNo, DO.CreatedAt DESC;
GO

-- =============================================
-- 7. v_MemberListDB 뷰 테스트
-- =============================================
PRINT '';
PRINT '=== 7. v_MemberListDB 뷰 테스트 ===';
GO

PRINT '7-1. 전체 회원 조회 (재직자만)';
SELECT
    Uid,
    UserId,
    UserName,
    ENumber,
    EDepartmentName AS 부서,
    ERank AS 직급,
    IsAdministrator AS 관리자,
    IsDeptObjectiveWriter AS 부서목표작성권한
FROM v_MemberListDB
ORDER BY EDepartId, ERank DESC;
GO

PRINT '7-2. 부서 목표 작성 권한자만 조회';
SELECT
    UserName AS 이름,
    EDepartmentName AS 부서,
    ERank AS 직급,
    CASE
        WHEN IsAdministrator = 1 THEN '관리자 (모든 부서)'
        WHEN IsDeptObjectiveWriter = 1 THEN '부서 목표 작성자 (소속 부서만)'
        ELSE '-'
    END AS 권한
FROM v_MemberListDB
WHERE IsDeptObjectiveWriter = 1 OR IsAdministrator = 1
ORDER BY IsAdministrator DESC, EDepartId;
GO

-- =============================================
-- 8. 테스트 데이터 정리
-- =============================================
PRINT '';
PRINT '=== 8. 테스트 데이터 정리 ===';
GO

PRINT '8-1. 생성된 부서 목표 삭제';
DELETE FROM DeptObjectiveDb
WHERE CreatedBy IN (
    SELECT Uid FROM UserDb WHERE UserId IN ('user02', 'admin')
);
PRINT '  - 부서 목표 삭제 완료';
GO

PRINT '8-2. 테스트 사용자 삭제';
DELETE FROM UserDb
WHERE UserId IN ('user01', 'user02', 'user03');
PRINT '  - 테스트 사용자 삭제 완료';
GO

-- =============================================
-- 9. 테스트 결과 요약
-- =============================================
PRINT '';
PRINT '=============================================';
PRINT '테스트 완료: 부서 목표 권한 관리 시스템';
PRINT '=============================================';
PRINT '';
PRINT '✅ 검증 항목:';
PRINT '  1. UserDb.IsDeptObjectiveWriter 필드 추가';
PRINT '  2. DeptObjectiveDb 감사 필드 추가 (CreatedBy, UpdatedBy)';
PRINT '  3. MemberDb 삭제 확인';
PRINT '  4. v_MemberListDB 뷰 재작성';
PRINT '  5. fn_CanWriteDeptObjective 함수 동작';
PRINT '  6. sp_CreateDeptObjective 프로시저 권한 검증';
PRINT '  7. sp_UpdateDeptObjective 프로시저 권한 검증';
PRINT '  8. 부서 변경 방지 로직';
PRINT '  9. 관리자 모든 부서 작성 권한';
PRINT ' 10. 부서 목표 작성자 소속 부서만 작성 권한';
PRINT '';
PRINT '개발자 확인 사항:';
PRINT '  - 위 테스트 결과를 확인하고 모든 케이스가 정상 동작하는지 검토';
PRINT '  - 필요 시 추가 테스트 케이스 작성';
PRINT '=============================================';
GO
