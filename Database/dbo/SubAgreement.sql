CREATE TABLE [dbo].[SubAgreementDb]
(
    -- Primary Key
    -- 자동 ID생성 
    -- [01] SubAgreementDb id
    [Sid] BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    -- [02] 사용자 ID (외래키)
    [Uid] BIGINT NOT NULL,
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
    -- [08] Report_Item_Proportion(세부직무 %)
    [Report_SubItem_Proportion] INT NOT NULL,
    -- [09] 하위 업무 리스트 번호
    [Task_Number] BIGINT NOT NULL,

    -- 외래키 제약조건
    CONSTRAINT FK_SubAgreementDb_UserDb
        FOREIGN KEY (Uid)
        REFERENCES [dbo].[UserDb](Uid)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
)