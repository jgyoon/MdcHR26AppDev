-- =============================================
-- 2026년 인사평가프로그램 - ProcessDb Seed 데이터
-- 작성일: 2026-01-26
-- 설명: 평가 프로세스 초기 데이터 생성
-- 비고: UserDb에 존재하는 모든 사용자에 대해 기본 ProcessDb 레코드 생성
--       평가자(TeamLeaderId, DirectorId)는 인사팀에서 추후 설정 예정
-- =============================================

USE [MdcHR2026];
GO

-- =============================================
-- ProcessDb (평가 프로세스 Seed 데이터)
-- =============================================
PRINT 'ProcessDb Seed 데이터 입력 중...';
GO

-- IDENTITY INSERT 활성화
SET IDENTITY_INSERT [dbo].[ProcessDb] ON;
GO

-- 모든 사용자에 대한 기본 ProcessDb 레코드 생성
-- 초기값:
--   - 모든 평가 단계는 false
--   - TeamLeaderId/DirectorId는 NULL (인사팀에서 추후 설정)
--   - 코멘트는 빈 문자열

INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (1, 1);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (2, 2);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (3, 3);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (4, 4);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (5, 5);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (6, 6);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (7, 7);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (8, 8);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (9, 9);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (10, 10);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (11, 11);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (12, 12);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (13, 13);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (14, 14);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (15, 15);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (16, 16);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (17, 17);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (18, 18);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (19, 19);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (20, 20);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (21, 21);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (22, 22);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (23, 23);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (24, 24);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (25, 25);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (26, 26);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (27, 27);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (28, 28);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (29, 29);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (30, 30);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (31, 31);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (32, 32);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (33, 33);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (34, 34);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (35, 35);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (36, 36);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (37, 37);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (38, 38);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (39, 39);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (40, 40);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (41, 41);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (42, 42);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (43, 43);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (44, 44);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (45, 45);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (46, 46);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (47, 47);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (48, 48);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (49, 49);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (50, 50);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (51, 51);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (52, 52);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (53, 53);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (54, 54);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (55, 55);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (56, 56);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (57, 57);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (58, 58);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (59, 59);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (60, 60);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (61, 61);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (62, 62);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (63, 63);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (64, 64);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (65, 65);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (66, 66);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (67, 67);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (68, 68);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (69, 69);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (70, 70);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (71, 71);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (72, 72);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (73, 73);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (74, 74);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (75, 75);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (76, 76);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (77, 77);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (78, 78);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (79, 79);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (80, 80);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (81, 81);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (82, 82);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (83, 83);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (84, 84);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (85, 85);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (86, 86);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (87, 87);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (88, 88);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (89, 89);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (90, 90);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (91, 91);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (92, 92);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (93, 93);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (94, 94);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (95, 95);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (96, 96);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (97, 97);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (98, 98);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (99, 99);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (100, 100);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (101, 101);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (102, 102);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (103, 103);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (104, 104);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (105, 105);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (106, 106);
INSERT INTO [dbo].[ProcessDb] (Pid, Uid) VALUES (107, 107);

-- IDENTITY INSERT 비활성화
SET IDENTITY_INSERT [dbo].[ProcessDb] OFF;
GO

PRINT 'ProcessDb Seed 데이터 입력 완료';
GO

-- =============================================
-- 입력 결과 확인
-- =============================================
PRINT '입력된 ProcessDb 레코드 수 확인...';
GO

SELECT COUNT(*) AS TotalProcessRecords
FROM [dbo].[ProcessDb];
GO

-- 사용자별 ProcessDb 레코드 확인
PRINT 'ProcessDb 레코드 목록 (평가자 미설정 상태)...';
GO

SELECT
    p.Pid,
    p.Uid,
    u.UserId,
    u.UserName AS 평가대상자,
    CASE WHEN p.TeamLeaderId IS NULL THEN N'미설정' ELSE CAST(p.TeamLeaderId AS NVARCHAR) END AS 부서장ID,
    CASE WHEN p.DirectorId IS NULL THEN N'미설정' ELSE CAST(p.DirectorId AS NVARCHAR) END AS 임원ID
FROM [dbo].[ProcessDb] p
    INNER JOIN [dbo].[UserDb] u ON p.Uid = u.Uid
ORDER BY p.Pid;
GO

PRINT '※ 평가자(부서장/임원)는 인사팀에서 추후 설정 필요';
PRINT '03_06_SeedData_Process.sql 스크립트 실행 완료';
GO
