-- =============================================
-- 2026년 인사평가프로그램 시드 데이터 스크립트
-- 작성일: 2025-12-16
-- 설명: 마스터 데이터 및 관리자 계정 생성
-- =============================================

USE [MdcHR2026];
GO

-- =============================================
-- 1단계: EDepartmentDb (부서 마스터 데이터)
-- =============================================
PRINT '부서 마스터 데이터 입력 중...';
GO

SET IDENTITY_INSERT [dbo].[EDepartmentDb] ON;
GO
INSERT INTO [dbo].[EDepartmentDb] ([EDepartId], [EDepartmentNo], [EDepartmentName], [ActivateStatus], [Remarks]) VALUES (
INSERT INTO [dbo].[EDepartmentDb]
    ([EDepartId], [EDepartmentNo], [EDepartmentName], [ActivateStatus], [Remarks])
VALUES
    (1, 10, N'경영지원팀', 1, N'경영지원 및 총무 업무'),
    (2, 20, N'기술개발팀', 1, N'기술 개발 및 연구'),
    (3, 30, N'영업팀', 1, N'영업 및 마케팅'),
    (4, 40, N'생산팀', 1, N'생산 및 품질관리'),
    (5, 50, N'재무회계팀', 1, N'재무 및 회계 업무');
GO

SET IDENTITY_INSERT [dbo].[EDepartmentDb] OFF;
GO

PRINT '부서 마스터 데이터 입력 완료 (5개)';
GO

-- =============================================
-- 2단계: ERankDb (직급 마스터 데이터)
-- =============================================
PRINT '직급 마스터 데이터 입력 중...';
GO

SET IDENTITY_INSERT [dbo].[ERankDb] ON;
GO

INSERT INTO [dbo].[ERankDb]
    ([ERankId], [ERankNo], [ERankName], [ActivateStatus], [Remarks])
VALUES
    (1, 10, N'사원', 1, N'일반 사원'),
    (2, 20, N'대리', 1, N'대리급'),
    (3, 30, N'과장', 1, N'과장급'),
    (4, 40, N'차장', 1, N'차장급'),
    (5, 50, N'부장', 1, N'부장급 (팀장)'),
    (6, 60, N'이사', 1, N'이사급 (임원)'),
    (7, 70, N'전무', 1, N'전무급 (임원)'),
    (8, 80, N'부사장', 1, N'부사장급 (임원)'),
    (9, 90, N'사장', 1, N'대표이사');
GO

SET IDENTITY_INSERT [dbo].[ERankDb] OFF;
GO

PRINT '직급 마스터 데이터 입력 완료 (9개)';
GO

-- =============================================
-- 3단계: UserDb (관리자 계정)
-- =============================================
PRINT '관리자 계정 생성 중...';
GO

-- Salt 생성 (16바이트 랜덤 데이터)
DECLARE @AdminSalt VARBINARY(16);
SET @AdminSalt = CAST(NEWID() AS VARBINARY(16));

-- 비밀번호: "admin1234" + Salt를 SHA-256으로 암호화
DECLARE @AdminPassword VARBINARY(32);
SET @AdminPassword = HASHBYTES('SHA2_256', 'admin1234' + CAST(@AdminSalt AS VARCHAR(MAX)));

SET IDENTITY_INSERT [dbo].[UserDb] ON;
GO

INSERT INTO [dbo].[UserDb]
    ([Uid], [UserId], [UserName], [UserPassword], [UserPasswordSalt], [ENumber], [Email],
     [EDepartId], [ERankId], [EStatus], [IsTeamLeader], [IsDirector], [IsAdministrator], [IsDeptObjectiveWriter])
VALUES
    (1, 'admin', N'시스템관리자', @AdminPassword, @AdminSalt, 'A0001', 'admin@company.com',
     1, 9, 1, 0, 0, 1, 1);
GO

SET IDENTITY_INSERT [dbo].[UserDb] OFF;
GO

PRINT '관리자 계정 생성 완료';
PRINT '  계정: admin / 비밀번호: admin1234 (Salt 기반 SHA-256 암호화)';
GO

-- =============================================
-- 4단계: EvaluationLists (평가 항목 마스터 데이터 - 예시)
-- =============================================
PRINT '평가 항목 마스터 데이터 입력 중...';
GO

SET IDENTITY_INSERT [dbo].[EvaluationLists] ON;
GO

INSERT INTO [dbo].[EvaluationLists]
    ([Eid], [Evaluation_Department_Number], [Evaluation_Department_Name],
     [Evaluation_Index_Number], [Evaluation_Index_Name],
     [Evaluation_Task_Number], [Evaluation_Task_Name],
     [Evaluation_Lists_Remark])
VALUES
    -- 경영지원팀 평가 항목
    (1, 10, N'경영지원팀', 1, N'업무 효율성', 1, N'일정 준수', N'계획 대비 일정 준수도'),
    (2, 10, N'경영지원팀', 1, N'업무 효율성', 2, N'업무 수행량', N'목표 대비 업무 수행량'),
    (3, 10, N'경영지원팀', 1, N'업무 효율성', 3, N'결과물 품질', N'결과물의 완성도'),
    (4, 10, N'경영지원팀', 2, N'업무 역량', 1, N'전문성', N'업무 전문 지식'),
    (5, 10, N'경영지원팀', 2, N'업무 역량', 2, N'문제 해결력', N'문제 분석 및 해결 능력'),

    -- 기술개발팀 평가 항목
    (6, 20, N'기술개발팀', 1, N'기술 역량', 1, N'기술 완성도', N'개발 결과물의 기술적 완성도'),
    (7, 20, N'기술개발팀', 1, N'기술 역량', 2, N'코드 품질', N'코드의 가독성 및 유지보수성'),
    (8, 20, N'기술개발팀', 2, N'혁신성', 1, N'기술 혁신', N'신기술 도입 및 적용'),
    (9, 20, N'기술개발팀', 2, N'혁신성', 2, N'창의적 문제해결', N'창의적 접근 방식'),

    -- 영업팀 평가 항목
    (10, 30, N'영업팀', 1, N'영업 성과', 1, N'목표 달성률', N'매출 목표 달성률'),
    (11, 30, N'영업팀', 1, N'영업 성과', 2, N'신규 고객 확보', N'신규 고객 유치 건수'),
    (12, 30, N'영업팀', 2, N'고객 관리', 1, N'고객 만족도', N'고객 만족도 조사 결과'),
    (13, 30, N'영업팀', 2, N'고객 관리', 2, N'고객 관계 유지', N'기존 고객 유지율'),

    -- 생산팀 평가 항목
    (14, 40, N'생산팀', 1, N'생산 효율', 1, N'생산 목표 달성', N'생산 계획 대비 달성률'),
    (15, 40, N'생산팀', 1, N'생산 효율', 2, N'불량률 관리', N'제품 불량률 개선도'),
    (16, 40, N'생산팀', 2, N'안전 관리', 1, N'안전 수칙 준수', N'안전 규정 준수율'),

    -- 재무회계팀 평가 항목
    (17, 50, N'재무회계팀', 1, N'재무 관리', 1, N'정확성', N'재무 보고서 정확도'),
    (18, 50, N'재무회계팀', 1, N'재무 관리', 2, N'적시성', N'보고서 제출 기한 준수'),
    (19, 50, N'재무회계팀', 2, N'예산 관리', 1, N'예산 준수', N'예산 범위 내 업무 수행'),
    (20, 50, N'재무회계팀', 2, N'예산 관리', 2, N'비용 절감', N'비용 절감 기여도');
GO

SET IDENTITY_INSERT [dbo].[EvaluationLists] OFF;
GO

PRINT '평가 항목 마스터 데이터 입력 완료 (20개)';
GO

-- =============================================
-- 완료 메시지
-- =============================================
PRINT '';
PRINT '=============================================';
PRINT '시드 데이터 입력이 완료되었습니다.';
PRINT '';
PRINT '입력된 데이터:';
PRINT '  - 부서 마스터: 5개';
PRINT '  - 직급 마스터: 9개';
PRINT '  - 관리자 계정: 1개 (admin / admin1234)';
PRINT '  - 평가 항목: 20개 (5개 부서별)';
PRINT '';
PRINT '참고사항:';
PRINT '  1. 관리자 비밀번호는 반드시 변경하세요';
PRINT '  2. 평가 항목은 실제 조직에 맞게 수정하세요';
PRINT '  3. 부서 및 직급은 필요에 따라 추가/수정하세요';
PRINT '=============================================';
GO

-- =============================================
-- 검증 쿼리 (선택사항)
-- =============================================
PRINT '';
PRINT '데이터 확인:';
GO

SELECT '부서 목록' AS Category, EDepartmentNo AS [순서], EDepartmentName AS [이름]
FROM [dbo].[EDepartmentDb]
ORDER BY EDepartmentNo;

SELECT '직급 목록' AS Category, ERankNo AS [순서], ERankName AS [이름]
FROM [dbo].[ERankDb]
ORDER BY ERankNo;

SELECT '관리자 계정' AS Category, UserId AS [계정], UserName AS [이름],
       Email AS [이메일]
FROM [dbo].[UserDb]
WHERE IsAdministrator = 1;

SELECT '평가 항목 통계' AS Category,
       Evaluation_Department_Name AS [부서],
       COUNT(*) AS [항목수]
FROM [dbo].[EvaluationLists]
GROUP BY Evaluation_Department_Name
ORDER BY Evaluation_Department_Name;
GO
