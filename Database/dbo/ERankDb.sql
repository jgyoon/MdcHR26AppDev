CREATE TABLE [dbo].[ERankDb]
(
    -- Primary Key
    -- 자동 ID생성
    -- [01] ERank id
    [ERankId] BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    -- [02] 직급번호(정렬순서)
    [ERankNo] INT NOT NULL UNIQUE,
    -- [03] 직급명
    [ERankName] NVARCHAR(255) NOT NULL,
    -- [04] 활성화여부
    [ActivateStatus] BIT NOT NULL DEFAULT 1,
    -- [99] 비고
    [Remarks] NVARCHAR(MAX)
);
