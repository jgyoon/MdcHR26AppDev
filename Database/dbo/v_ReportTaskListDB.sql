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
	INNER JOIN [dbo].[TasksDb] B ON A.Task_Number = B.TaksListNumber
