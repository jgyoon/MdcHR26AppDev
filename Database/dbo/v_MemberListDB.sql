CREATE VIEW [dbo].[v_MemberListDB]
AS SELECT
    U.Uid,
    U.UserId,
    U.UserName,
    U.ENumber,
    R.ERankId,
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
WHERE U.EStatus = 1
