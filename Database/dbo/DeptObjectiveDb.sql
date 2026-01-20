CREATE TABLE [dbo].[DeptObjectiveDb]
(
    -- Primary Key
    -- 자동 ID생성
    -- [01] DeptObjectiveDb id
    [DeptObjectiveDbId] BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    -- [02] EDepartmentDb id
    [EDepartId] BIGINT NOT NULL,
    -- [03] ObjectiveTitle
    [ObjectiveTitle] NVARCHAR(MAX) NOT NULL,
    -- [04] ObjectiveContents
    [ObjectiveContents] NVARCHAR(MAX) NOT NULL,
    -- [05] 작성자 (외래키)
    [CreatedBy] BIGINT NOT NULL,
    -- [06] 작성일시
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    -- [07] 수정자 (외래키)
    [UpdatedBy] BIGINT NULL,
    -- [08] 수정일시
    [UpdatedAt] DATETIME NULL,
    -- [99] 비고
    [Remarks] NVARCHAR(MAX),

    -- 외래키 제약조건
    CONSTRAINT FK_DeptObjectiveDb_EDepartmentDb
        FOREIGN KEY (EDepartId)
        REFERENCES [dbo].[EDepartmentDb](EDepartId)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,

    CONSTRAINT FK_DeptObjectiveDb_UserDb_CreatedBy
        FOREIGN KEY (CreatedBy)
        REFERENCES [dbo].[UserDb](Uid)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,

    CONSTRAINT FK_DeptObjectiveDb_UserDb_UpdatedBy
        FOREIGN KEY (UpdatedBy)
        REFERENCES [dbo].[UserDb](Uid)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);

