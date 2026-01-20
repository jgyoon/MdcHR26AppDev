CREATE TABLE [dbo].[AgreementDb]
(
    -- Primary Key
    -- 자동 ID생성 
    -- [01] AgreementDb id
    [Aid] BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
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

    -- 외래키 제약조건
    CONSTRAINT FK_AgreementDb_UserDb
        FOREIGN KEY (Uid)
        REFERENCES [dbo].[UserDb](Uid)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
)