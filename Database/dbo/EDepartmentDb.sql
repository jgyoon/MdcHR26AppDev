CREATE TABLE [dbo].[EDepartmentDb]
(
    -- Primary Key
    -- 자동 ID생성 
    -- [01] EDepartmentDb id
    [EDepartId] BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    -- [02] 부서번호(정렬순서)
    [EDepartmentNo] INT NOT NULL UNIQUE,
    -- [03] 부서명
    [EDepartmentName] NVARCHAR(255) NOT NULL,
    -- [04] 활성화여부
    [ActivateStatus] BIT NOT NULL DEFAULT 1,  
    -- [99] 비고
    [Remarks] NVARCHAR(MAX)
);
