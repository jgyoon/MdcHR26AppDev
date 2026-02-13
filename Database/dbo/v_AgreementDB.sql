CREATE VIEW [dbo].[v_AgreementDB]
AS SELECT
	A.Aid,
	A.Uid,
	A.Report_Item_Number,
	A.Report_Item_Name_1,
	A.Report_Item_Name_2,
	A.Report_Item_Proportion,
	U.UserId,
	U.UserName,
	ISNULL(ED.EDepartmentName, '') AS EDepartmentName,
	ISNULL(ER.ERankName, '') AS ERankName
FROM
	[dbo].[AgreementDb] A
	INNER JOIN [dbo].[UserDb] U ON A.Uid = U.Uid
	LEFT JOIN [dbo].[EDepartmentDb] ED ON U.EDepartId = ED.EDepartId
	LEFT JOIN [dbo].[ERankDb] ER ON U.ERankId = ER.ERankId
