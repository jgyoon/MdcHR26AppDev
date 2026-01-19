-- =============================================
-- 2026년 인사평가프로그램 테이블 생성 스크립트
-- 작성일: 2025-12-16
-- 설명: 외래키 의존성 순서에 따라 테이블 생성
-- =============================================

USE [MdcHR2026];
GO

-- =============================================
-- 1단계: 기존 테이블 삭제 (역순)
-- =============================================
PRINT '기존 테이블 삭제 중...';
GO

-- 외래키가 있는 테이블들 먼저 삭제
IF OBJECT_ID('[dbo].[EvaluationUsers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EvaluationUsers];
IF OBJECT_ID('[dbo].[TotalReportDb]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TotalReportDb];
IF OBJECT_ID('[dbo].[ReportDb]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ReportDb];
IF OBJECT_ID('[dbo].[ProcessDb]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProcessDb];
IF OBJECT_ID('[dbo].[SubAgreementDb]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SubAgreementDb];
IF OBJECT_ID('[dbo].[AgreementDb]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AgreementDb];
IF OBJECT_ID('[dbo].[DeptObjectiveDb]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DeptObjectiveDb];

-- 독립적인 테이블
IF OBJECT_ID('[dbo].[TasksDb]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TasksDb];
IF OBJECT_ID('[dbo].[EvaluationLists]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EvaluationLists];

-- UserDb 삭제 (많은 테이블이 참조)
IF OBJECT_ID('[dbo].[UserDb]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserDb];

-- 마스터 데이터 테이블 삭제
IF OBJECT_ID('[dbo].[ERankDb]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ERankDb];
IF OBJECT_ID('[dbo].[EDepartmentDb]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EDepartmentDb];

PRINT '기존 테이블 삭제 완료';
GO

-- =============================================
-- 2단계: 마스터 데이터 테이블 생성
-- =============================================
PRINT '마스터 데이터 테이블 생성 중...';
GO

-- [01] EDepartmentDb (부서 마스터)
PRINT '  - EDepartmentDb 생성';
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
GO

-- [02] ERankDb (직급 마스터)
PRINT '  - ERankDb 생성';
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
GO

PRINT '마스터 데이터 테이블 생성 완료';
GO

-- =============================================
-- 3단계: UserDb 생성 (마스터 테이블 참조)
-- =============================================
PRINT 'UserDb 생성 중...';
GO

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
);
GO

PRINT 'UserDb 생성 완료';
GO

-- =============================================
-- 4단계: UserDb를 참조하는 테이블들 생성
-- =============================================
PRINT 'UserDb 참조 테이블들 생성 중...';
GO

-- [04] DeptObjectiveDb
PRINT '  - DeptObjectiveDb 생성';
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
        REFERENCES [dbo].[UserDb](UId)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,

    CONSTRAINT FK_DeptObjectiveDb_UserDb_UpdatedBy
        FOREIGN KEY (UpdatedBy)
        REFERENCES [dbo].[UserDb](UId)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);
GO

-- [05] AgreementDb
PRINT '  - AgreementDb 생성';
CREATE TABLE [dbo].[AgreementDb]
(
    -- Primary Key
    -- 자동 ID생성
    -- [01] AgreementDb id
    [Aid] BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
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

    -- 외래키 제약조건
    CONSTRAINT FK_AgreementDb_UserDb
        FOREIGN KEY (UId)
        REFERENCES [dbo].[UserDb](UId)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);
GO

-- [07] SubAgreementDb
PRINT '  - SubAgreementDb 생성';
CREATE TABLE [dbo].[SubAgreementDb]
(
    -- Primary Key
    -- 자동 ID생성
    -- [01] SubAgreementDb id
    [Sid] BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
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
    -- [08] Report_Item_Proportion(세부직무 %)
    [Report_SubItem_Proportion] INT NOT NULL,
    -- [09] 하위 업무 리스트 번호
    [Task_Number] BIGINT NOT NULL,

    -- 외래키 제약조건
    CONSTRAINT FK_SubAgreementDb_UserDb
        FOREIGN KEY (UId)
        REFERENCES [dbo].[UserDb](UId)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);
GO

-- [08] ProcessDb
PRINT '  - ProcessDb 생성';
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
);
GO

-- [09] ReportDb
PRINT '  - ReportDb 생성';
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
);
GO

-- [10] TotalReportDb
PRINT '  - TotalReportDb 생성';
CREATE TABLE [dbo].[TotalReportDb]
(
    -- Primary Key
    -- 자동 ID생성
    -- [01] TotalReport id
    [TRid] BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    -- [02] User id (외래키)
    [UId] BIGINT NOT NULL,

    -- 평가대상자 평가점수
    -- [03] User_Evaluation_1(일정준수)
    [User_Evaluation_1] FLOAT NOT NULL,
    -- [04] User_Evaluation_2(업무 수행의 양)
    [User_Evaluation_2] FLOAT NOT NULL,
    -- [05] User_Evaluation_3(결과물)
    [User_Evaluation_3] FLOAT NOT NULL,
    -- [06] User_Evaluation_4(comment)
    [User_Evaluation_4] NVARCHAR(MAX),

    -- 부서장(팀장) 평가점수
    -- [07] TeamLeader_Evaluation_1(일정준수)
    [TeamLeader_Evaluation_1] FLOAT NOT NULL,
    -- [08] TeamLeader_Evaluation_2(업무 수행의 양)
    [TeamLeader_Evaluation_2] FLOAT NOT NULL,
    -- [09] TeamLeader_Evaluation_3(결과물)
    [TeamLeader_Evaluation_3] FLOAT NOT NULL,
    -- [10] TeamLeader_Comment(comment)
    [TeamLeader_Comment] NVARCHAR(MAX),

    -- feedback 1차 면담
    -- [11] Feedback_Evaluation_1(일정준수)
    [Feedback_Evaluation_1] FLOAT NOT NULL,
    -- [12] Feedback_Evaluation_2(업무 수행의 양)
    [Feedback_Evaluation_2] FLOAT NOT NULL,
    -- [13] Feedback_Evaluation_3(결과물)
    [Feedback_Evaluation_3] FLOAT NOT NULL,
    -- [14] Feedback_Comment(comment)
    [Feedback_Comment] NVARCHAR(MAX),

    -- 임원 평가점수
    -- [15] Director_Evaluation_1(일정준수)
    [Director_Evaluation_1] FLOAT NOT NULL,
    -- [16] Director_Evaluation_2(업무 수행의 양)
    [Director_Evaluation_2] FLOAT NOT NULL,
    -- [17] Director_Evaluation_3(결과물)
    [Director_Evaluation_3] FLOAT NOT NULL,
    -- [18] Director_Comment(comment)
    [Director_Comment] NVARCHAR(MAX),

    -- [19] Total_Score(종합점수)
    [Total_Score] FLOAT NOT NULL,
    -- [20] Director_Score(임원점수)
    [Director_Score] FLOAT NOT NULL,
    -- [21] TeamLeader_Score(부서장점수)
    [TeamLeader_Score] FLOAT NOT NULL DEFAULT 0,

    -- 외래키 제약조건
    CONSTRAINT FK_TotalReportDb_UserDb
        FOREIGN KEY (UId)
        REFERENCES [dbo].[UserDb](UId)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);
GO

-- [11] EvaluationUsers
PRINT '  - EvaluationUsers 생성';
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
);
GO

PRINT 'UserDb 참조 테이블들 생성 완료';
GO

-- =============================================
-- 5단계: 독립적인 테이블들 생성
-- =============================================
PRINT '독립 테이블들 생성 중...';
GO

-- [12] TasksDb (독립적인 테이블)
PRINT '  - TasksDb 생성';
CREATE TABLE [dbo].[TasksDb]
(
    -- Primary Key
    -- 자동 ID생성
    -- [01] Tasks id
    [Tid] BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    -- [02] Task Name(업무명)
    [TaskName] NVARCHAR(100) NOT NULL,
    -- [03] Taks List Number(업무 리스트 번호)
    [TaksListNumber] BIGINT NOT NULL,
    -- [04] Task Status(업무 상태)
    -- 0 : 진행중
    -- 1 : 종료
    -- 2 : 보류
    -- 3 : 취소
    [TaskStatus] INT NOT NULL,
    -- [05] Task Objective(업무 목표)
    [TaskObjective] NVARCHAR(MAX) NOT NULL,
    -- [06] Target Proportion (목표 달성도)
    [TargetProportion] INT NOT NULL,
    -- [07] Result Proportion (결과 달성도)
    [ResultProportion] INT NOT NULL,
    -- [08] Target Date(목표달성일자)
    [TargetDate] DATETIME NOT NULL,
    -- [09] Result Date(결과달성일자)
    [ResultDate] DATETIME NOT NULL,
    -- [10] Task_Evaluation_1(일정준수)
    [Task_Evaluation_1] FLOAT NOT NULL,
    -- [11] Task_Evaluation_2(업무수행도)
    [Task_Evaluation_2] FLOAT NOT NULL,
    -- [12] Task Level(업무수준-난이도)
    -- S : 1.2
    -- A : 1.0(기본값)
    -- B : 0.8
    -- C : 0.6
    [TaskLevel] FLOAT NOT NULL,
    -- [13] Task Comments(업무 코멘트)
    [TaskComments] NVARCHAR(50) NOT NULL
);
GO

-- [13] EvaluationLists (독립적인 마스터 데이터)
PRINT '  - EvaluationLists 생성';
CREATE TABLE [dbo].[EvaluationLists]
(
    -- Primary Key
    -- 자동 ID생성
    -- [01] Evaluation Lists id
    [Eid] BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    -- [02] 평가부서번호
    [Evaluation_Department_Number] INT NOT NULL,
    -- [03] 평가부서번호이름
    [Evaluation_Department_Name] NVARCHAR(20) NOT NULL,
    -- [04] 평가부서번호
    [Evaluation_Index_Number] INT NOT NULL,
    -- [05] 평가부서번호이름
    [Evaluation_Index_Name] NVARCHAR(100) NOT NULL,
    -- [06] 평가부서번호
    [Evaluation_Task_Number] INT NOT NULL,
    -- [07] 평가부서번호이름
    [Evaluation_Task_Name] NVARCHAR(100) NOT NULL,
    -- [08] 평가리스트 비고
    [Evaluation_Lists_Remark] NVARCHAR(100) NULL
);
GO

PRINT '독립 테이블들 생성 완료';
GO

-- =============================================
-- 완료 메시지
-- =============================================
PRINT '';
PRINT '=============================================';
PRINT '모든 테이블 생성이 완료되었습니다.';
PRINT '총 12개 테이블 생성:';
PRINT '  - EDepartmentDb (부서 마스터)';
PRINT '  - ERankDb (직급 마스터)';
PRINT '  - UserDb (사용자)';
PRINT '  - DeptObjectiveDb (부서 목표)';
PRINT '  - AgreementDb (직무평가합의)';
PRINT '  - SubAgreementDb (세부직무평가합의)';
PRINT '  - ProcessDb (평가 프로세스)';
PRINT '  - ReportDb (평가 리포트)';
PRINT '  - TotalReportDb (종합 리포트)';
PRINT '  - EvaluationUsers (평가자 관리)';
PRINT '  - TasksDb (업무 관리)';
PRINT '  - EvaluationLists (평가 항목 마스터)';
PRINT '=============================================';
GO
