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
		  INNER JOIN [dbo].[EDepartmentDb] B ON A.EDepartId = B.EDepartId
