-- =============================================
-- 2026년 인사평가프로그램 뷰 생성 스크립트
-- 작성일: 2025-12-16
-- 설명: 외래키 기반으로 수정된 5개 뷰 생성
-- =============================================

USE [MdcHR2026];
GO

-- =============================================
-- 1단계: 기존 뷰 삭제
-- =============================================
PRINT '기존 뷰 삭제 중...';
GO

IF OBJECT_ID('[dbo].[v_TotalReportListDB]', 'V') IS NOT NULL
    DROP VIEW [dbo].[v_TotalReportListDB];
IF OBJECT_ID('[dbo].[v_ReportTaskListDB]', 'V') IS NOT NULL
    DROP VIEW [dbo].[v_ReportTaskListDB];
IF OBJECT_ID('[dbo].[v_ProcessTRListDB]', 'V') IS NOT NULL
    DROP VIEW [dbo].[v_ProcessTRListDB];
IF OBJECT_ID('[dbo].[v_DeptObjectiveListDb]', 'V') IS NOT NULL
    DROP VIEW [dbo].[v_DeptObjectiveListDb];
IF OBJECT_ID('[dbo].[v_MemberListDB]', 'V') IS NOT NULL
    DROP VIEW [dbo].[v_MemberListDB];

PRINT '기존 뷰 삭제 완료';
GO

-- =============================================
-- 2단계: 뷰 생성
-- =============================================
PRINT '뷰 생성 중...';
GO

-- [01] v_MemberListDB
PRINT '  - v_MemberListDB 생성';
GO
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
WHERE U.EStatus = 1;
GO

-- [02] v_DeptObjectiveListDb
PRINT '  - v_DeptObjectiveListDb 생성';
GO
CREATE VIEW [dbo].[v_DeptObjectiveListDb]
AS SELECT
	A.DeptObjectiveDbId,
	A.EDepartId,
	B.EDepartmentName,
	A.ObjectiveTitle,
	A.ObjectiveContents,
	A.Remarks
FROM
	[dbo].[DeptObjectiveDb] A
	INNER JOIN [dbo].[EDepartmentDb] B ON A.EDepartId = B.EDepartId;
GO

-- [03] v_ProcessTRListDB
PRINT '  - v_ProcessTRListDB 생성';
GO
CREATE VIEW [dbo].[v_ProcessTRListDB]
AS SELECT
	A.Pid,
	U.UserId,
	U.UserName,
	ISNULL(TL.UserId, '') AS TeamLeader_Id,
	ISNULL(TL.UserName, '') AS TeamLeader_Name,
	ISNULL(D.UserId, '') AS Director_Id,
	ISNULL(D.UserName, '') AS Director_Name,
	A.Is_Request,
	A.Is_Agreement,
	A.Is_SubRequest,
	A.Is_SubAgreement,
	A.Is_User_Submission,
	A.Is_Teamleader_Submission,
	A.Is_Director_Submission,
	A.FeedBackStatus,
	A.FeedBack_Submission,
	A.Uid,
	ISNULL(C.TRid, 0) AS TRid,
	ISNULL(C.User_Evaluation_1, 0) AS User_Evaluation_1,
	ISNULL(C.User_Evaluation_2, 0) AS User_Evaluation_2,
	ISNULL(C.User_Evaluation_3, 0) AS User_Evaluation_3,
	ISNULL(C.User_Evaluation_4, '') AS User_Evaluation_4,
	ISNULL(C.TeamLeader_Evaluation_1, 0) AS TeamLeader_Evaluation_1,
	ISNULL(C.TeamLeader_Evaluation_2, 0) AS TeamLeader_Evaluation_2,
	ISNULL(C.TeamLeader_Evaluation_3, 0) AS TeamLeader_Evaluation_3,
	ISNULL(C.TeamLeader_Comment, '') AS TeamLeader_Comment,
	ISNULL(C.Feedback_Evaluation_1, 0) AS Feedback_Evaluation_1,
	ISNULL(C.Feedback_Evaluation_2, 0) AS Feedback_Evaluation_2,
	ISNULL(C.Feedback_Evaluation_3, 0) AS Feedback_Evaluation_3,
	ISNULL(C.Feedback_Comment, '') AS Feedback_Comment,
	ISNULL(C.Director_Evaluation_1, 0) AS Director_Evaluation_1,
	ISNULL(C.Director_Evaluation_2, 0) AS Director_Evaluation_2,
	ISNULL(C.Director_Evaluation_3, 0) AS Director_Evaluation_3,
	ISNULL(C.Director_Comment,'') AS Director_Comment,
	ISNULL(C.Total_Score, 0) AS Total_Score,
	ISNULL(C.Director_Score, 0) AS Director_Score
FROM
	[dbo].[ProcessDb] A
	INNER JOIN [dbo].[UserDb] U ON A.Uid = U.Uid
	LEFT JOIN [dbo].[UserDb] TL ON A.TeamLeaderId = TL.Uid
	LEFT JOIN [dbo].[UserDb] D ON A.DirectorId = D.Uid
	LEFT JOIN [dbo].[TotalReportDb] C ON A.Uid = C.Uid;
GO

-- [04] v_ReportTaskListDB
PRINT '  - v_ReportTaskListDB 생성';
GO
CREATE VIEW [dbo].[v_ReportTaskListDB]
AS SELECT
	A.Rid,
	U.UserId,
	U.UserName,
	A.Report_Item_Number,
	A.Report_Item_Name_1,
	A.Report_Item_Name_2,
	A.Report_Item_Proportion,
	A.Report_SubItem_Name,
	A.Report_SubItem_Proportion,
	B.*
FROM
	[dbo].[ReportDb] A
	INNER JOIN [dbo].[UserDb] U ON A.Uid = U.Uid
	INNER JOIN [dbo].[TasksDb] B ON A.Task_Number = B.TaksListNumber;
GO

-- [05] v_TotalReportListDB
PRINT '  - v_TotalReportListDB 생성';
GO
CREATE VIEW [dbo].[v_TotalReportListDB]
AS SELECT
	A.TRid,
	A.Uid,
	B.UserId,
	B.UserName,
	A.User_Evaluation_1,
	A.User_Evaluation_2,
	A.User_Evaluation_3,
	A.User_Evaluation_4,
	A.TeamLeader_Evaluation_1,
	A.TeamLeader_Evaluation_2,
	A.TeamLeader_Evaluation_3,
	A.TeamLeader_Comment,
	A.Feedback_Evaluation_1,
	A.Feedback_Evaluation_2,
	A.Feedback_Evaluation_3,
	A.Feedback_Comment,
	A.Director_Evaluation_1,
	A.Director_Evaluation_2,
	A.Director_Evaluation_3,
	A.Director_Comment,
	A.Total_Score,
	A.Director_Score,
	A.TeamLeader_Score
FROM
	[dbo].[TotalReportDb] A
	INNER JOIN [dbo].[UserDb] B ON A.Uid = B.Uid;
GO

PRINT '뷰 생성 완료';
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

-- =============================================
-- 완료 메시지
-- =============================================
PRINT '';
PRINT '=============================================';
PRINT '모든 뷰 생성이 완료되었습니다.';
PRINT '총 6개 뷰 생성:';
PRINT '  - v_MemberListDB (부서원 목록)';
PRINT '  - v_DeptObjectiveListDb (부서 목표 목록)';
PRINT '  - v_ProcessTRListDB (평가 프로세스 및 종합 리포트)';
PRINT '  - v_ReportTaskListDB (평가 리포트 및 업무 목록)';
PRINT '  - v_TotalReportListDB (종합 리포트 목록)';
PRINT '  - v_EvaluationUsersList (평가자 목록 목록)';
PRINT '=============================================';
GO
