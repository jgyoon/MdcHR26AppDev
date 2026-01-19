CREATE TABLE [dbo].[ReportDb]
(
    -- Primary Key
    -- 자동 ID생성
    -- [01] Report id
    [Rid] BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    -- [02] 사용자 ID (외래키)
    [UId] BIGINT NOT NULL,
    -- [03] Report_Item_Number
    [Report_Item_Number] INT NOT NULL,
    -- [04] Report_Item_Name_1(지표분류명)
    [Report_Item_Name_1] NVARCHAR(MAX) NOT NULL,
    -- [05] Report_Item_Name_2(직무분류명)
    [Report_Item_Name_2] NVARCHAR(MAX) NOT NULL,
    -- [06] Report_Item_Proportion(직무 %)
    [Report_Item_Proportion] INT NOT NULL,
    -- [07] Report_SubItem_Name(세부직무명)
    [Report_SubItem_Name] NVARCHAR(MAX) NOT NULL,
    -- [08] Report_SubItem_Proportion(세부직무 %)
    [Report_SubItem_Proportion] INT NOT NULL,

    -- [09] 하위 업무 리스트 번호
    [Task_Number] BIGINT NOT NULL,

    -- 평가대상자
    -- [10] User_Evaluation_1(일정준수)
    [User_Evaluation_1] FLOAT NOT NULL,
    -- [11] User_Evaluation_2(업무 수행의 양)
    [User_Evaluation_2] FLOAT NOT NULL,
    -- [12] User_Evaluation_3(결과물)
    [User_Evaluation_3] FLOAT NOT NULL,
    -- [13] User_Evaluation_4(comment)
    [User_Evaluation_4] NVARCHAR(MAX),

    -- 부서장(팀장)
    -- [14] TeamLeader_Evaluation_1(일정준수)
    [TeamLeader_Evaluation_1] FLOAT NOT NULL,
    -- [15] TeamLeader_Evaluation_2(업무 수행의 양)
    [TeamLeader_Evaluation_2] FLOAT NOT NULL,
    -- [16] TeamLeader_Evaluation_3(결과물)
    [TeamLeader_Evaluation_3] FLOAT NOT NULL,
    -- [17] TeamLeader_Evaluation_4(comment)
    [TeamLeader_Evaluation_4] NVARCHAR(MAX),

    -- 임원
    -- [18] Director_Evaluation_1(일정준수)
    [Director_Evaluation_1] FLOAT NOT NULL,
    -- [19] Director_Evaluation_2(업무 수행의 양)
    [Director_Evaluation_2] FLOAT NOT NULL,
    -- [20] Director_Evaluation_3(결과물)
    [Director_Evaluation_3] FLOAT NOT NULL,
    -- [21] Director_Evaluation_4(comment)
    [Director_Evaluation_4] NVARCHAR(MAX),

    -- [22] Total_Score(종합점수)
    [Total_Score] FLOAT NOT NULL,

    -- 외래키 제약조건
    CONSTRAINT FK_ReportDb_UserDb
        FOREIGN KEY (UId)
        REFERENCES [dbo].[UserDb](UId)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
)
