CREATE TABLE [dbo].[TotalReportDb]
(
    -- Primary Key
    -- 자동 ID생성
    -- [01] TotalReport id
    [TRid] BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    -- [02] User id (외래키)
    [UId] BIGINT NOT NULL,

    -- 평가대상자 평가점수
    -- [03] User_Evaluation_1(일정준수)
    [User_Evaluation_1] FLOAT NOT NULL,
    -- [04] User_Evaluation_2(업무 수행의 양)
    [User_Evaluation_2] FLOAT NOT NULL,
    -- [05] User_Evaluation_3(결과물)
    [User_Evaluation_3] FLOAT NOT NULL,
    -- [06] User_Evaluation_4(comment)
    [User_Evaluation_4] NVARCHAR(MAX),

    -- 부서장(팀장) 평가점수
    -- [07] TeamLeader_Evaluation_1(일정준수)
    [TeamLeader_Evaluation_1] FLOAT NOT NULL,
    -- [08] TeamLeader_Evaluation_2(업무 수행의 양)
    [TeamLeader_Evaluation_2] FLOAT NOT NULL,
    -- [09] TeamLeader_Evaluation_3(결과물)
    [TeamLeader_Evaluation_3] FLOAT NOT NULL,
    -- [10] TeamLeader_Comment(comment)
    [TeamLeader_Comment] NVARCHAR(MAX),

    -- feedback 1차 면담
    -- [11] Feedback_Evaluation_1(일정준수)
    [Feedback_Evaluation_1] FLOAT NOT NULL,
    -- [12] Feedback_Evaluation_2(업무 수행의 양)
    [Feedback_Evaluation_2] FLOAT NOT NULL,
    -- [13] Feedback_Evaluation_3(결과물)
    [Feedback_Evaluation_3] FLOAT NOT NULL,
    -- [14] Feedback_Comment(comment)
    [Feedback_Comment] NVARCHAR(MAX),

    -- 임원 평가점수
    -- [15] Director_Evaluation_1(일정준수)
    [Director_Evaluation_1] FLOAT NOT NULL,
    -- [16] Director_Evaluation_2(업무 수행의 양)
    [Director_Evaluation_2] FLOAT NOT NULL,
    -- [17] Director_Evaluation_3(결과물)
    [Director_Evaluation_3] FLOAT NOT NULL,
    -- [18] Director_Comment(comment)
    [Director_Comment] NVARCHAR(MAX),

    -- [19] Total_Score(종합점수)
    [Total_Score] FLOAT NOT NULL,
    -- [20] Director_Score(임원점수)
    [Director_Score] FLOAT NOT NULL,
    -- [21] TeamLeader_Score(부서장점수)
    [TeamLeader_Score] FLOAT NOT NULL DEFAULT 0,

    -- 외래키 제약조건
    CONSTRAINT FK_TotalReportDb_UserDb
        FOREIGN KEY (UId)
        REFERENCES [dbo].[UserDb](UId)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
)
