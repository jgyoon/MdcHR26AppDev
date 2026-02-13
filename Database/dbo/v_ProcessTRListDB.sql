CREATE VIEW [dbo].[v_ProcessTRListDB]
AS SELECT
	A.Pid,
	U.UserId,
	U.UserName,
	ISNULL(ED.EDepartmentName, '') AS EDepartmentName,
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
	ISNULL(C.Director_Score, 0) AS Director_Score,
	ISNULL(C.TeamLeader_Score, 0) AS TeamLeader_Score
FROM
	[dbo].[ProcessDb] A
	INNER JOIN [dbo].[UserDb] U ON A.Uid = U.Uid
	LEFT JOIN [dbo].[EDepartmentDb] ED ON U.EDepartId = ED.EDepartId
	LEFT JOIN [dbo].[UserDb] TL ON A.TeamLeaderId = TL.Uid
	LEFT JOIN [dbo].[UserDb] D ON A.DirectorId = D.Uid
	LEFT JOIN [dbo].[TotalReportDb] C ON A.Uid = C.Uid
