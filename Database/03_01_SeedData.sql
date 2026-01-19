-- =============================================
-- 2026년 인사평가프로그램 시드 데이터 스크립트
-- 작성일: 2026-01-13
-- 설명: Seed 데이터 생성
-- =============================================

USE [MdcHR2026];
GO

-- =============================================
-- 1단계: EDepartmentDb (부서 Seed 데이터)
-- =============================================
PRINT '부서 Seed 데이터 입력 중...';
GO

SET IDENTITY_INSERT [dbo].[EDepartmentDb] ON;
GO

INSERT INTO [dbo].[EDepartmentDb] ([EDepartId], [EDepartmentNo], [EDepartmentName], [ActivateStatus], [Remarks]) VALUES (1, 1, N'메디칼파크' ,1, N'');
INSERT INTO [dbo].[EDepartmentDb] ([EDepartId], [EDepartmentNo], [EDepartmentName], [ActivateStatus], [Remarks]) VALUES (2, 1000, N'경영지원본부' ,1, N'');
INSERT INTO [dbo].[EDepartmentDb] ([EDepartId], [EDepartmentNo], [EDepartmentName], [ActivateStatus], [Remarks]) VALUES (3, 1100, N'인사회계팀' ,1, N'');
INSERT INTO [dbo].[EDepartmentDb] ([EDepartId], [EDepartmentNo], [EDepartmentName], [ActivateStatus], [Remarks]) VALUES (4, 1300, N'구매자산팀' ,1, N'');
INSERT INTO [dbo].[EDepartmentDb] ([EDepartId], [EDepartmentNo], [EDepartmentName], [ActivateStatus], [Remarks]) VALUES (5, 1900, N'전산팀' ,1, N'');
INSERT INTO [dbo].[EDepartmentDb] ([EDepartId], [EDepartmentNo], [EDepartmentName], [ActivateStatus], [Remarks]) VALUES (6, 2000, N'기업부설연구소' ,1, N'');
INSERT INTO [dbo].[EDepartmentDb] ([EDepartId], [EDepartmentNo], [EDepartmentName], [ActivateStatus], [Remarks]) VALUES (7, 2100, N'스마트헬스케어 솔루션팀' ,1, N'');
INSERT INTO [dbo].[EDepartmentDb] ([EDepartId], [EDepartmentNo], [EDepartmentName], [ActivateStatus], [Remarks]) VALUES (8, 2200, N'의료기기 소모품 사업팀' ,1, N'');
INSERT INTO [dbo].[EDepartmentDb] ([EDepartId], [EDepartmentNo], [EDepartmentName], [ActivateStatus], [Remarks]) VALUES (9, 2300, N'의료기기 장비 사업팀' ,1, N'');
INSERT INTO [dbo].[EDepartmentDb] ([EDepartId], [EDepartmentNo], [EDepartmentName], [ActivateStatus], [Remarks]) VALUES (10, 2400, N'설계품질보증팀' ,1, N'');
INSERT INTO [dbo].[EDepartmentDb] ([EDepartId], [EDepartmentNo], [EDepartmentName], [ActivateStatus], [Remarks]) VALUES (11, 2500, N'선행생산기술팀' ,1, N'');
INSERT INTO [dbo].[EDepartmentDb] ([EDepartId], [EDepartmentNo], [EDepartmentName], [ActivateStatus], [Remarks]) VALUES (12, 2600, N'영상의료기기센터' ,1, N'');
INSERT INTO [dbo].[EDepartmentDb] ([EDepartId], [EDepartmentNo], [EDepartmentName], [ActivateStatus], [Remarks]) VALUES (13, 2700, N'메카팀' ,1, N'');
INSERT INTO [dbo].[EDepartmentDb] ([EDepartId], [EDepartmentNo], [EDepartmentName], [ActivateStatus], [Remarks]) VALUES (14, 2800, N'영상소프트웨어팀' ,1, N'');
INSERT INTO [dbo].[EDepartmentDb] ([EDepartId], [EDepartmentNo], [EDepartmentName], [ActivateStatus], [Remarks]) VALUES (15, 3000, N'영업본부' ,1, N'');
INSERT INTO [dbo].[EDepartmentDb] ([EDepartId], [EDepartmentNo], [EDepartmentName], [ActivateStatus], [Remarks]) VALUES (16, 4000, N'품질인증본부' ,1, N'');
INSERT INTO [dbo].[EDepartmentDb] ([EDepartId], [EDepartmentNo], [EDepartmentName], [ActivateStatus], [Remarks]) VALUES (17, 4100, N'인증팀' ,1, N'');
INSERT INTO [dbo].[EDepartmentDb] ([EDepartId], [EDepartmentNo], [EDepartmentName], [ActivateStatus], [Remarks]) VALUES (18, 4200, N'품질팀' ,1, N'');
INSERT INTO [dbo].[EDepartmentDb] ([EDepartId], [EDepartmentNo], [EDepartmentName], [ActivateStatus], [Remarks]) VALUES (19, 5000, N'생산팀' ,1, N'');
INSERT INTO [dbo].[EDepartmentDb] ([EDepartId], [EDepartmentNo], [EDepartmentName], [ActivateStatus], [Remarks]) VALUES (20, 6000, N'가공사업본부' ,1, N'');

SET IDENTITY_INSERT [dbo].[EDepartmentDb] OFF;
GO

PRINT '부서 Seed 데이터 입력 완료';
GO

-- =============================================
-- 2단계: ERankDb (직급 Seed 데이터)
-- =============================================
PRINT '직급 Seed 데이터 입력 중...';
GO

SET IDENTITY_INSERT [dbo].[ERankDb] ON;
GO

INSERT INTO [dbo].[ERankDb] (ERankId, ERankNo, ERankName, ActivateStatus, Remarks) VALUES (1, 1, N'직원', 1, N'');
INSERT INTO [dbo].[ERankDb] (ERankId, ERankNo, ERankName, ActivateStatus, Remarks) VALUES (2, 100, N'대표이사', 1, N'');
INSERT INTO [dbo].[ERankDb] (ERankId, ERankNo, ERankName, ActivateStatus, Remarks) VALUES (3, 200, N'사장', 1, N'');
INSERT INTO [dbo].[ERankDb] (ERankId, ERankNo, ERankName, ActivateStatus, Remarks) VALUES (4, 300, N'부사장', 1, N'');
INSERT INTO [dbo].[ERankDb] (ERankId, ERankNo, ERankName, ActivateStatus, Remarks) VALUES (5, 400, N'전무이사', 1, N'');
INSERT INTO [dbo].[ERankDb] (ERankId, ERankNo, ERankName, ActivateStatus, Remarks) VALUES (6, 500, N'상무이사', 1, N'');
INSERT INTO [dbo].[ERankDb] (ERankId, ERankNo, ERankName, ActivateStatus, Remarks) VALUES (7, 600, N'이사', 1, N'');
INSERT INTO [dbo].[ERankDb] (ERankId, ERankNo, ERankName, ActivateStatus, Remarks) VALUES (8, 700, N'부장', 1, N'');
INSERT INTO [dbo].[ERankDb] (ERankId, ERankNo, ERankName, ActivateStatus, Remarks) VALUES (9, 800, N'차장', 1, N'');
INSERT INTO [dbo].[ERankDb] (ERankId, ERankNo, ERankName, ActivateStatus, Remarks) VALUES (10, 900, N'과장', 1, N'');
INSERT INTO [dbo].[ERankDb] (ERankId, ERankNo, ERankName, ActivateStatus, Remarks) VALUES (11, 1000, N'대리', 1, N'');
INSERT INTO [dbo].[ERankDb] (ERankId, ERankNo, ERankName, ActivateStatus, Remarks) VALUES (12, 1100, N'주임', 1, N'');
INSERT INTO [dbo].[ERankDb] (ERankId, ERankNo, ERankName, ActivateStatus, Remarks) VALUES (13, 1200, N'반장', 1, N'');
INSERT INTO [dbo].[ERankDb] (ERankId, ERankNo, ERankName, ActivateStatus, Remarks) VALUES (14, 1300, N'사원', 1, N'');

GO

SET IDENTITY_INSERT [dbo].[ERankDb] OFF;
GO

PRINT '직급 Seed 데이터 입력 완료';
GO

-- =============================================
-- 3단계: UserDb Seed 데이터 입력
-- =============================================
PRINT '사용자 Seed 데이터 입력 생성 중...';
GO

SET IDENTITY_INSERT [dbo].[UserDb] ON;
GO

-- Salt 생성 (16바이트 랜덤 데이터)
-- Salt를 SHA-256으로 암호화
DECLARE @Salt VARBINARY(16);

-- SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (Uid, N'UserId', N'UserName', HASHBYTES('SHA2_256', N'UserPassword' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'Enumber', N'Email', EDepartId, ERankId, Estatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (1, N'mdcadmin', N'관리자', HASHBYTES('SHA2_256', N'xnd0580!!' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'', 1, 1, 1, 0, 0, 1, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (2, N'jslim', N'임정석', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'jslim@medicalpark.co.kr', 2, 3, 1, 1, 1, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (3, N'jgyoon', N'윤종국', HASHBYTES('SHA2_256', N'xnd0580+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'jgyoon@medicalpark.co.kr', 5, 9, 1, 0, 0, 1, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (4, N'ysjo', N'조용성', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'ysjo@medicalpark.co.kr', 5, 10, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (5, N'jeonghwa.seo', N'서정화', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'jeonghwa.seo@medicalpark.co.kr', 3, 9, 1, 1, 0, 1, 1);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (6, N'gyujin.jung', N'정규진', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'gyujin.jung@medicalpark.co.kr', 3, 10, 1, 0, 0, 1, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (7, N'ijbeak', N'백인지', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'ijbeak@medicalpark.co.kr', 3, 11, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (8, N'jungmin.lee', N'이정민', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'jungmin.lee@medicalpark.co.kr', 3, 12, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (9, N'yeseul.park', N'박예슬', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'yeseul.park@medicalpark.co.kr', 3, 12, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (10, N'hwkim', N'김현우', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'hwkim@medicalpark.co.kr', 4, 8, 1, 1, 0, 0, 1);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (11, N'yjpark', N'박영재', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'yjpark@medicalpark.co.kr', 4, 10, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (12, N'youngbin.lee', N'이영빈', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'youngbin.lee@medicalpark.co.kr', 4, 10, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (13, N'seongwoong.lee', N'이성웅', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'seongwoong.lee@medicalpark.co.kr', 7, 8, 1, 0, 0, 0, 1);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (14, N'yongmin.bae', N'배용민', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'yongmin.bae@medicalpark.co.kr', 7, 8, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (15, N'jungsoo.kim', N'김정수', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'jungsoo.kim@medicalpark.co.kr', 7, 9, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (16, N'hyeontae.kim', N'김현태', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'hyeontae.kim@medicalpark.co.kr', 7, 9, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (17, N'jhwan.kim', N'김정환_A.I', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'jhwan.kim@medicalpark.co.kr', 7, 10, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (18, N'seungho.lee', N'이승호', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'seungho.lee@medicalpark.co.kr', 7, 10, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (19, N'kyoungjin.min', N'민경진', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'kyoungjin.min@medicalpark.co.kr', 7, 11, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (20, N'sinyeong.kim', N'김신영', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'sinyeong.kim@medicalpark.co.kr', 7, 12, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (21, N'yujin.jang', N'장유진', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'yujin.jang@medicalpark.co.kr', 7, 12, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (22, N'jongmin.jeon', N'전종민', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'jongmin.jeon@medicalpark.co.kr', 7, 12, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (23, N'sklee', N'이승교', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'sklee@medicalpark.co.kr', 8, 7, 1, 1, 1, 0, 1);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (24, N'jongho.shin', N'신종호', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'jongho.shin@medicalpark.co.kr', 8, 7, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (25, N'yongki.kim', N'김용기', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'yongki.kim@medicalpark.co.kr', 8, 8, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (26, N'dykim', N'김대영', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'dykim@medicalpark.co.kr', 8, 9, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (27, N'yongkil.park', N'박용길', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'yongkil.park@medicalpark.co.kr', 8, 9, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (28, N'yhlee', N'이용하', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'yhlee@medicalpark.co.kr', 8, 11, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (29, N'hyunsu.jang', N'장현수', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'hyunsu.jang@medicalpark.co.kr', 8, 12, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (30, N'yschoi', N'최윤성', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'yschoi@medicalpark.co.kr', 9, 9, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (31, N'kyusung.woo', N'우규성', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'kyusung.woo@medicalpark.co.kr', 9, 9, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (32, N'gwansu.lee', N'이관수', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'gwansu.lee@medicalpark.co.kr', 9, 10, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (33, N'taeyang.kim', N'김태양', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'taeyang.kim@medicalpark.co.kr', 9, 11, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (34, N'wcyoon', N'윤원철', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'wcyoon@medicalpark.co.kr', 9, 11, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (35, N'hyeonwoo.im', N'임현우', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'hyeonwoo.im@medicalpark.co.kr', 9, 11, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (36, N'wjkim', N'김우진', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'wjkim@medicalpark.co.kr', 10, 9, 1, 1, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (37, N'hmkim', N'김혜민', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'hmkim@medicalpark.co.kr', 10, 10, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (38, N'hbsim', N'심혜빈', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'hbsim@medicalpark.co.kr', 10, 10, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (39, N'eunyoung.son', N'손은영', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'eunyoung.son@medicalpark.co.kr', 10, 11, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (40, N'minji.jeong', N'정민지', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'minji.jeong@medicalpark.co.kr', 10, 11, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (41, N'bskwon', N'권범승', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'bskwon@medicalpark.co.kr', 10, 12, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (42, N'hkkim', N'김현기', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'hkkim@medicalpark.co.kr', 11, 7, 1, 1, 0, 0, 1);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (43, N'yclee', N'이영철', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'yclee@medicalpark.co.kr', 11, 8, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (44, N'phillhyung.lee', N'이필형', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'phillhyung.lee@medicalpark.co.kr', 11, 9, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (45, N'jaesub.hwang', N'황재섭', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'jaesub.hwang@medicalpark.co.kr', 12, 4, 1, 1, 1, 0, 1);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (46, N'vincent.suh', N'서용석', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'vincent.suh@medicalpark.co.kr', 12, 8, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (47, N'dkahn', N'안동기', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'dkahn@medicalpark.co.kr', 12, 8, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (48, N'byungsu.shin', N'신병수', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'byungsu.shin@medicalpark.co.kr', 13, 8, 1, 1, 0, 0, 1);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (49, N'bongsu.kwon', N'권봉수', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'bongsu.kwon@medicalpark.co.kr', 13, 8, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (50, N'soochang.lee', N'이수창', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'soochang.lee@medicalpark.co.kr', 13, 8, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (51, N'jeonghwan.kim', N'김정환_Meca팀', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'jeonghwan.kim@medicalpark.co.kr', 13, 9, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (52, N'minho.son', N'손민호', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'minho.son@medicalpark.co.kr', 13, 9, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (53, N'kyungil.choi', N'최경일', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'kyungil.choi@medicalpark.co.kr', 13, 9, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (54, N'seon.kang', N'강선', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'seon.kang@medicalpark.co.kr', 13, 10, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (55, N'scpark', N'박수창', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'scpark@medicalpark.co.kr', 13, 10, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (56, N'seongsoo.park', N'박성수', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'seongsoo.park@medicalpark.co.kr', 13, 11, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (57, N'wscho', N'조우성', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'wscho@medicalpark.co.kr', 14, 8, 1, 1, 0, 0, 1);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (58, N'namouk.kim', N'김남욱', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'namouk.kim@medicalpark.co.kr', 14, 10, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (59, N'jinho.cho', N'조진호', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'jinho.cho@medicalpark.co.kr', 14, 10, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (60, N'songhee.kim', N'김송희', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'songhee.kim@medicalpark.co.kr', 14, 11, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (61, N'jhchoi', N'최진홍', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'jhchoi@medicalpark.co.kr', 15, 7, 1, 1, 1, 0, 1);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (62, N'wkson', N'손원국', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'wkson@medicalpark.co.kr', 15, 8, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (63, N'jehee.lee', N'이제희', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'jehee.lee@medicalpark.co.kr', 15, 8, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (64, N'nybae', N'배나영', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'nybae@medicalpark.co.kr', 15, 10, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (65, N'seunghoon.lee', N'이승훈', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'seunghoon.lee@medicalpark.co.kr', 15, 10, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (66, N'jhcho', N'조재현', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'jhcho@medicalpark.co.kr', 15, 10, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (67, N'sunyoung.jo', N'조선영', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'sunyoung.jo@medicalpark.co.kr', 15, 11, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (68, N'jiyeon.kim', N'김지연', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'jiyeon.kim@medicalpark.co.kr', 15, 12, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (69, N'sehyun.jang', N'장세현', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'sehyun.jang@medicalpark.co.kr', 16, 7, 1, 1, 1, 0, 1);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (70, N'kichan.an', N'안기찬', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'kichan.an@medicalpark.co.kr', 17, 8, 1, 1, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (71, N'phuong.nguyen', N'응웬티프엉', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'phuong.nguyen@medicalpark.co.kr', 17, 11, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (72, N'ejlee', N'이은진', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'ejlee@medicalpark.co.kr', 17, 11, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (73, N'jaehee.lim', N'임재희', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'jaehee.lim@medicalpark.co.kr', 17, 11, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (74, N'yujin.gil', N'길유진', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'yujin.gil@medicalpark.co.kr', 17, 12, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (75, N'kyuri.kim', N'김규리', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'kyuri.kim@medicalpark.co.kr', 17, 12, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (76, N'aram.lee', N'이아람', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'aram.lee@medicalpark.co.kr', 17, 12, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (77, N'seungju.lee', N'이승주', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'seungju.lee@medicalpark.co.kr', 18, 10, 1, 1, 0, 0, 1);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (78, N'chkim', N'김철홍', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'chkim@medicalpark.co.kr', 18, 10, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (79, N'shseo', N'서승호', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'shseo@medicalpark.co.kr', 18, 10, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (80, N'mihee.jo', N'조미희', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'mihee.jo@medicalpark.co.kr', 18, 10, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (81, N'donghoo.kim', N'김동후', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'donghoo.kim@medicalpark.co.kr', 18, 11, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (82, N'shyu', N'유성현', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'shyu@medicalpark.co.kr', 18, 11, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (83, N'junhwi.lim', N'임준휘', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'junhwi.lim@medicalpark.co.kr', 18, 11, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (84, N'cklee', N'이창교', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'cklee@medicalpark.co.kr', 18, 12, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (85, N'aeri.ko', N'고애리', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'aeri.ko@medicalpark.co.kr', 18, 14, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (86, N'hyunjung.won', N'원현정', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'hyunjung.won@medicalpark.co.kr', 18, 14, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (87, N'kanghyun.ko', N'고강현', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'kanghyun.ko@medicalpark.co.kr', 19, 9, 1, 1, 0, 0, 1);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (88, N'jaewook.yang', N'양제욱', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'jaewook.yang@medicalpark.co.kr', 19, 9, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (89, N'hsgwak', N'곽현석', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'hsgwak@medicalpark.co.kr', 19, 10, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (90, N'gyeongchan.park', N'박경찬', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'gyeongchan.park@medicalpark.co.kr', 19, 12, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (91, N'yskim', N'김윤숙', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'yskim@medicalpark.co.kr', 19, 13, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (92, N'yan.kim', N'김연', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'yan.kim@medicalpark.co.kr', 19, 14, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (93, N'eunhye.kim', N'김은혜', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'eunhye.kim@medicalpark.co.kr', 19, 14, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (94, N'jhkim', N'김정희', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'jhkim@medicalpark.co.kr', 19, 14, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (95, N'jhkim2', N'김정희2', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'jhkim2@medicalpark.co.kr', 19, 14, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (96, N'ynpark', N'박영남', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'ynpark@medicalpark.co.kr', 19, 14, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (97, N'hanna.park', N'박한나', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'hanna.park@medicalpark.co.kr', 19, 14, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (98, N'seulki.lee', N'이슬기', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'seulki.lee@medicalpark.co.kr', 19, 14, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (99, N'mhcho', N'조민혜', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'mhcho@medicalpark.co.kr', 19, 14, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (100, N'hamin.jo', N'조하민', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'hamin.jo@medicalpark.co.kr', 19, 14, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (101, N'joonho.joo', N'주준호', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'joonho.joo@medicalpark.co.kr', 19, 14, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (102, N'hjpark', N'박형재', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'hjpark@medicalpark.co.kr', 20, 9, 1, 1, 0, 0, 1);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (103, N'byeongchun.oh', N'오병천', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'byeongchun.oh@medicalpark.co.kr', 20, 11, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (104, N'soohyun.no', N'노수현', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'soohyun.no@medicalpark.co.kr', 20, 12, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (105, N'arin.kim', N'김아린', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'arin.kim@medicalpark.co.kr', 20, 14, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (106, N'honghua.li', N'이홍화', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'honghua.li@medicalpark.co.kr', 20, 14, 1, 0, 0, 0, 0);
SET @Salt = CAST(NEWID() AS VARBINARY(16)); INSERT INTO [dbo].[UserDb] (UId, UserId, UserName, UserPassword, UserPasswordSalt, ENumber, Email, EDepartId, ERankId, EStatus, IsTeamLeader, IsDirector, IsAdministrator, IsDeptObjectiveWriter) VALUES (107, N'yeonhwa.heo', N'허연화', HASHBYTES('SHA2_256', N'xnd852456+' + CAST(@Salt AS NVARCHAR(MAX))), @Salt, N'', N'yeonhwa.heo@medicalpark.co.kr', 20, 14, 1, 0, 0, 0, 0);

SET IDENTITY_INSERT [dbo].[UserDb] OFF;
GO

PRINT '사용자 Seed 데이터 생성 완료';
GO

-- =============================================
-- 4단계: EvaluationUsers 평가대상자 Seed 데이터 입력
-- =============================================
PRINT '평가대상자 Seed 데이터 입력 중...';
GO

SET IDENTITY_INSERT [dbo].[EvaluationUsers] ON;
GO

-- INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (Euid, UId);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (1, 1);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (2, 2);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (3, 3);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (4, 4);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (5, 5);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (6, 6);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (7, 7);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (8, 8);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (9, 9);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (10, 10);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (11, 11);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (12, 12);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (13, 13);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (14, 14);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (15, 15);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (16, 16);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (17, 17);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (18, 18);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (19, 19);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (20, 20);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (21, 21);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (22, 22);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (23, 23);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (24, 24);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (25, 25);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (26, 26);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (27, 27);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (28, 28);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (29, 29);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (30, 30);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (31, 31);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (32, 32);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (33, 33);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (34, 34);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (35, 35);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (36, 36);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (37, 37);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (38, 38);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (39, 39);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (40, 40);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (41, 41);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (42, 42);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (43, 43);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (44, 44);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (45, 45);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (46, 46);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (47, 47);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (48, 48);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (49, 49);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (50, 50);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (51, 51);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (52, 52);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (53, 53);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (54, 54);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (55, 55);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (56, 56);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (57, 57);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (58, 58);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (59, 59);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (60, 60);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (61, 61);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (62, 62);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (63, 63);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (64, 64);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (65, 65);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (66, 66);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (67, 67);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (68, 68);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (69, 69);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (70, 70);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (71, 71);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (72, 72);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (73, 73);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (74, 74);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (75, 75);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (76, 76);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (77, 77);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (78, 78);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (79, 79);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (80, 80);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (81, 81);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (82, 82);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (83, 83);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (84, 84);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (85, 85);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (86, 86);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (87, 87);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (88, 88);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (89, 89);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (90, 90);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (91, 91);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (92, 92);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (93, 93);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (94, 94);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (95, 95);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (96, 96);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (97, 97);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (98, 98);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (99, 99);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (100, 100);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (101, 101);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (102, 102);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (103, 103);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (104, 104);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (105, 105);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (106, 106);
INSERT INTO [dbo].[EvaluationUsers] (Euid, UId) VALUES (107, 107);

SET IDENTITY_INSERT [dbo].[EvaluationUsers] OFF;
GO

PRINT '평가대상자 Seed 데이터 입력 완료';
GO

-- =============================================
-- 완료 메시지
-- =============================================
PRINT '=============================================';
PRINT '기본 Seed 데이터 입력이 완료되었습니다.';
PRINT '총 4단계 데이터 입력:';
PRINT '  - EDepartmentDb (부서 20개)';
PRINT '  - ERankDb (직급 14개)';
PRINT '  - UserDb (사용자 107명, SHA-256 암호화)';
PRINT '  - EvaluationUsers (평가대상자 107명)';
PRINT '=============================================';
GO
-- =============================================
-- 검증 쿼리 (선택사항)
-- =============================================
