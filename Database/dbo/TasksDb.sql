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
)


-- Tid, TaskName, TaksListNumber, TaskStatus, TaskObjective, ObjectiveProportion, ResultProportion, TaskComments

-- @Tid, @TaskName, @TaksListNumber, @TaskStatus, @TaskObjective, @ObjectiveProportion, @ResultProportion, @TaskComments


-- TaskName = @TaskName,
-- TaksListNumber = @TaksListNumber,
-- TaskStatus = @TaskStatus,
-- TaskObjective = @TaskObjective,
-- ObjectiveProportion = @ObjectiveProportion,
-- ResultProportion = @ResultProportion,
-- TaskComments = @TaskComments

-- TaskName, TaksListNumber, TaskStatus, TaskObjective, TargetProportion, ResultProportion, TargetDate, ResultDate, TaskComments
-- @TaskName, @TaksListNumber, @TaskStatus, @TaskObjective, @TargetProportion, @ResultProportion, @TargetDate, @ResultDate, @TaskComments

-- TaskName = @TaskName,
-- TaksListNumber = @TaksListNumber,
-- TaskStatus = @TaskStatus,
-- TaskObjective = @TaskObjective,
-- TargetProportion = @TargetProportion,
-- ResultProportion = @ResultProportion,
-- TargetDate = @TargetDate,
-- ResultDate = @ResultDate,
-- TaskComments = @TaskComments