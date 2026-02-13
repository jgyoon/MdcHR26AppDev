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