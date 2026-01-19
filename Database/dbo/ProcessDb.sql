CREATE TABLE [dbo].[ProcessDb]
(
    -- Primary Key
    -- 자동 ID생성
    -- [01] ProcessDb id
    [Pid] BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    -- [02] 사용자 ID (외래키 - 평가 대상자)
    [UId] BIGINT NOT NULL,
    -- [03] 부서장 ID (외래키 - 2차 평가자)
    [TeamLeaderId] BIGINT,
    -- [04] 임원 ID (외래키 - 3차 평가자)
    [DirectorId] BIGINT,
    -- [05] 직무평가합의 요청 여부
    [Is_Request] BIT NOT NULL DEFAULT 0,
    -- [06] 직무평가합의 여부
    [Is_Agreement] BIT NOT NULL DEFAULT 0,
    -- [07] 직무평가합의 Comment
    [Agreement_Comment] NVARCHAR(MAX),
    -- [08] 하위(세부)직무평가합의 요청 여부
    [Is_SubRequest] BIT NOT NULL DEFAULT 0,
    -- [09] 하위(세부)직무평가합의 여부
    [Is_SubAgreement] BIT NOT NULL DEFAULT 0,
    -- [10] 하위(세부)직무평가합의 Comment
    [SubAgreement_Comment] NVARCHAR(MAX),
    -- [11] 사용자 평가 제출
    [Is_User_Submission] BIT NOT NULL DEFAULT 0,
    -- [12] 부서장 평가 제출
    [Is_Teamleader_Submission] BIT NOT NULL DEFAULT 0,
    -- [13] 임원 평가 제출
    [Is_Director_Submission] BIT NOT NULL DEFAULT 0,
    -- [14] FeedBack 여부
    [FeedBackStatus] BIT NOT NULL DEFAULT 0,
    -- [15] 평가자 FeedBack 승인여부
    [FeedBack_Submission] BIT NOT NULL DEFAULT 0,

    -- 외래키 제약조건
    CONSTRAINT FK_ProcessDb_UserDb_User
        FOREIGN KEY (UId)
        REFERENCES [dbo].[UserDb](UId)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,

    CONSTRAINT FK_ProcessDb_UserDb_TeamLeader
        FOREIGN KEY (TeamLeaderId)
        REFERENCES [dbo].[UserDb](UId)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,

    CONSTRAINT FK_ProcessDb_UserDb_Director
        FOREIGN KEY (DirectorId)
        REFERENCES [dbo].[UserDb](UId)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
)
