CREATE VIEW [dbo].[v_ReportTaskListDB]
AS SELECT
    -- ProcessDb 필드
    P.Pid,

    -- ReportDb 필드
    A.Rid,
    A.Uid,
    A.Report_Item_Number,
    A.Report_Item_Name_1,
    A.Report_Item_Name_2,
    A.Report_Item_Proportion,
    A.Report_SubItem_Name,
    A.Report_SubItem_Proportion,
    A.Task_Number,
    A.User_Evaluation_1,
    A.User_Evaluation_2,
    A.User_Evaluation_3,
    A.User_Evaluation_4,
    A.TeamLeader_Evaluation_1,
    A.TeamLeader_Evaluation_2,
    A.TeamLeader_Evaluation_3,
    A.TeamLeader_Evaluation_4,
    A.Director_Evaluation_1,
    A.Director_Evaluation_2,
    A.Director_Evaluation_3,
    A.Director_Evaluation_4,
    A.Total_Score,

    -- UserDb 필드
    U.UserId,
    U.UserName,
    ISNULL(ED.EDepartmentName, '') AS EDepartmentName,
    ISNULL(ER.ERankName, '') AS ERankName,

    -- TasksDb 필드 (명시적 선택, B.* 제거)
    B.Tid,
    B.TaskName,
    B.TaksListNumber,
    B.TaskStatus,
    B.TaskObjective,
    B.TargetProportion,
    B.ResultProportion,
    B.TargetDate,
    B.ResultDate,
    B.Task_Evaluation_1 AS Task_Eval_1,
    B.Task_Evaluation_2 AS Task_Eval_2,
    B.TaskLevel,
    B.TaskComments
FROM
    [dbo].[ReportDb] A
    INNER JOIN [dbo].[UserDb] U ON A.Uid = U.Uid
    LEFT JOIN [dbo].[EDepartmentDb] ED ON U.EDepartId = ED.EDepartId
    LEFT JOIN [dbo].[ERankDb] ER ON U.ERankId = ER.ERankId
    INNER JOIN [dbo].[ProcessDb] P ON A.Uid = P.Uid
    INNER JOIN [dbo].[TasksDb] B ON A.Task_Number = B.TaksListNumber