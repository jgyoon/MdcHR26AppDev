CREATE TABLE [dbo].[UserDb]
(
    -- Primary Key
    -- 자동 ID생성 
    -- [01] User id
    [Uid] BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    -- [02] 사용자 계정
    [UserId] VARCHAR(50) NOT NULL,
    -- [03] 사용자이름
    [UserName] NVARCHAR(20) NOT NULL,
    -- [04] 비밀번호
    -- 암호화를 위해서 VARBINARY로
    -- SHA2_256 알고리즘을 활용한 일방향 암호화
    -- SHA-256 해시값의 크기는 32바이트
    [UserPassword] VARBINARY(32) NOT NULL,
    -- [05] Salt
    -- 랜덤데이터로 입력
    -- CAST(NEWID() AS VARBINARY(128))
    -- 16바이트 길이의 salt
    [UserPasswordSalt] VARBINARY(16) NOT NULL,
    -- [06] 사번
    [ENumber] VARCHAR(10),
    -- [07] 이메일
    [Email] VARCHAR(50),
    -- [08] 부서 ID (외래키)
    [EDepartId] BIGINT,
    -- [09] 직급 ID (외래키)
    [ERankId] BIGINT,
    -- [10] 재직여부
    [EStatus] BIT NOT NULL DEFAULT 1,
    -- [11] 평가자(팀장) 여부 설정
    [IsTeamLeader] BIT NOT NULL DEFAULT 0,
    -- [12] 평가자(임원) 여부 설정
    [IsDirector] BIT NOT NULL DEFAULT 0,
    -- [13] 관리자 여부 설정
    [IsAdministrator] BIT NOT NULL DEFAULT 0,
    -- [14] 부서 목표 작성 권한
    [IsDeptObjectiveWriter] BIT NOT NULL DEFAULT 0,

    -- 외래키 제약조건
    CONSTRAINT FK_UserDb_EDepartmentDb
        FOREIGN KEY (EDepartId)
        REFERENCES [dbo].[EDepartmentDb](EDepartId)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,

    CONSTRAINT FK_UserDb_ERankDb
        FOREIGN KEY (ERankId)
        REFERENCES [dbo].[ERankDb](ERankId)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
)

-- https://m.blog.naver.com/PostView.naver?isHttpsRedirect=true&blogId=icysword&logNo=140179180098

-- SELECT 
--     Uid, UserId, UserName 
--     FROM [dbo].[UserDb]
