CREATE TABLE [dbo].[EvaluationUsers]
(
    -- Primary Key
    -- 자동 ID생성
    -- [01] Evaluation User id
    [EUid] BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    -- [02] 사용자 ID (외래키 - 평가 대상자)
    [UId] BIGINT NOT NULL,
    -- [03] 평가자여부
    [Is_Evaluation] BIT NOT NULL DEFAULT 1,
    -- [04] 부서장 ID (외래키 - 2차 평가자)
    [TeamLeaderId] BIGINT,
    -- [05] 임원 ID (외래키 - 3차 평가자)
    [DirectorId] BIGINT,
    -- [06] 부서장여부
    [Is_TeamLeader] BIT NOT NULL DEFAULT 0,

    -- 외래키 제약조건
    CONSTRAINT FK_EvaluationUsers_UserDb_User
        FOREIGN KEY (UId)
        REFERENCES [dbo].[UserDb](UId)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,

    CONSTRAINT FK_EvaluationUsers_UserDb_TeamLeader
        FOREIGN KEY (TeamLeaderId)
        REFERENCES [dbo].[UserDb](UId)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,

    CONSTRAINT FK_EvaluationUsers_UserDb_Director
        FOREIGN KEY (DirectorId)
        REFERENCES [dbo].[UserDb](UId)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
)
