CREATE VIEW [dbo].[v_TotalReportListDB]
AS SELECT
	A.TRid,
	A.Uid,
	B.UserId,
	B.UserName,
	ISNULL(ED.EDepartmentName, '') AS EDepartmentName,
	ISNULL(ER.ERankName, '') AS ERankName,
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
	INNER JOIN [dbo].[UserDb] B ON A.Uid = B.Uid
	LEFT JOIN [dbo].[EDepartmentDb] ED ON B.EDepartId = ED.EDepartId
	LEFT JOIN [dbo].[ERankDb] ER ON B.ERankId = ER.ERankId
