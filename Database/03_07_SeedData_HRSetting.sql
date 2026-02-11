-- =============================================
-- 2026년 인사평가프로그램 - HRSetting Seed 데이터
-- 작성일: 2026-02-11
-- 설명: 인사평가프로그램 설정 DB 입력
-- 비고: 시스템 설정 초기값 (평가 오픈: 비활성화, 평가 수정: 비활성화)
--       단일 레코드로 관리되며, Admin 페이지에서 토글 스위치로 변경 가능
-- =============================================

USE [MdcHR2026];
GO

-- =============================================
-- HRSetting
-- =============================================
PRINT 'HRSetting Seed 데이터 입력 중...';
GO

-- IDENTITY INSERT 활성화
SET IDENTITY_INSERT [dbo].[HRSetting] ON;
GO

INSERT INTO [dbo].[HRSetting] (HRSid, Evaluation_Open, Edit_Open) VALUES (1, 0, 0);

-- IDENTITY INSERT 비활성화
SET IDENTITY_INSERT [dbo].[HRSetting] OFF;
GO

PRINT 'HRSetting Seed 데이터 입력 완료';
GO

-- =============================================
-- 입력 결과 확인
-- =============================================
PRINT '입력된 HRSetting 레코드 수 확인...';
GO

SELECT COUNT(*) AS TotalHRSettingRecords
FROM [dbo].[HRSetting];
GO

-- HRSetting 레코드 확인 (단일 레코드)
PRINT 'HRSetting 레코드 상태 확인...';
GO

SELECT * FROM [dbo].[HRSetting];
GO

PRINT '03_07_HRSetting_Seed.sql 스크립트 실행 완료';
GO
