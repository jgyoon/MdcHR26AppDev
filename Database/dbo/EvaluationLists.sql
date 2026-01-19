CREATE TABLE [dbo].[EvaluationLists]
(
    -- Primary Key
    -- 자동 ID생성 
    -- [01] Evaluation Lists id
    [Eid] BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    -- [02] 평가부서번호
    [Evaluation_Department_Number] INT NOT NULL,
    -- [03] 평가부서번호이름
    [Evaluation_Department_Name] NVARCHAR(20) NOT NULL,
    -- [04] 평가부서번호
    [Evaluation_Index_Number] INT NOT NULL,
    -- [05] 평가부서번호이름
    [Evaluation_Index_Name] NVARCHAR(100) NOT NULL,
    -- [06] 평가부서번호
    [Evaluation_Task_Number] INT NOT NULL,
    -- [07] 평가부서번호이름
    [Evaluation_Task_Name] NVARCHAR(100) NOT NULL,
    -- [08] 평가리스트 비고
    [Evaluation_Lists_Remark] NVARCHAR(100) NULL,
)

-- https://m.blog.naver.com/PostView.naver?isHttpsRedirect=true&blogId=icysword&logNo=140179180098

