CREATE TABLE [dbo].[HRSetting]
(
    -- Primary Key
    -- 자동 ID생성 
    -- [01] HRSetting id
    [HRSid] BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    -- [02] 평가오픈
    [Evaluation_Open] BIT NOT NULL DEFAULT 0,
    -- [03] 평가수정
    [Edit_Open] BIT NOT NULL DEFAULT 0
)