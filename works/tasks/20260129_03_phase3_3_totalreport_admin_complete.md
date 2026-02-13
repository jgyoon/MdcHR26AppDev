# 작업지시서: Phase 3-3 Admin 페이지 완성 - TotalReport/Admin 및 CRUD 구현

**날짜**: 2026-01-29
**작업 유형**: 기능 추가 (Phase 3-3 완성)
**관련 이슈**:
- [#009: Phase 3 WebApp 개발](../issues/009_phase3_webapp_development.md)
- [#011: Phase 3-3 관리자 페이지 재작업](../issues/011_phase3_3_admin_pages_build_errors.md)
**선행 작업지시서**:
- `20260126_02_restructure_blazor_project.md` (프로젝트 구조 재정리)
- `20260128_01_implement_missing_components.md` (미구현 컴포넌트 3개)
- `20260129_02_implement_v_evaluation_users_list_models.md` (v_EvaluationUsersList 뷰)

**수정 이력**:
- 2026-01-29 (v1): 초안 작성
- 2026-01-29 (v2): NavigationManager.BaseUri 적용 (Step 6, 7), AppStateService 메서드 제거 (Step 9), Step 10-16을 9-15로 renumber
- 2026-01-29 (v3): ScoreUtils에 User_GetTotalScore_1/2/3 메서드 추가 (Step 2) - 개인평가에서 사용
- 2026-01-29 (v4): NavMenu 메뉴 추가 삭제 (Step 14), Admin/Index.razor 바로가기 버튼 방식 적용, Step 15를 14로 renumber

---

## 1. 작업 개요

### 1.1. 배경

**DB 구조 분석 완료**:
- db-optimizer로 2026년 DB 구조 확인 완료
- TotalReportDb가 Admin 페이지의 핵심 테이블임을 확인
- v_TotalReportListDB, v_ProcessTRListDB 뷰 존재

**2025년 인사평가 분석 완료**:
- `TotalReport/Admin` 폴더에 4개 페이지 존재:
  1. Index.razor - 전체 평가 목록
  2. Details.razor - 최종 결과 상세
  3. Edit.razor - 최종 등급 설정
  4. ReportInit.razor - 평가 초기화
- Admin 권한으로 전체 직원의 평가 결과 조회 및 관리

**현재 상태 (90% 완료)**:
- ✅ Admin 기본 페이지 (UserManage, SettingManage, EUsersManage)
- ✅ Users CRUD (Create, Edit, Delete, Details) - 순차 생성 로직 구현
- ✅ Settings CRUD (Depts, Ranks) - 기초정보 관리
- ✅ EvaluationUsers (Details, Edit)
- ✅ 공용 컴포넌트 3개 (DisplayResultText, EUserListTable, MemberListTable)
- ✅ v_EvaluationUsersList 뷰 구현
- ❌ **TotalReport/Admin 페이지 미구현** ← 이번 작업

### 1.2. 작업 목적

Phase 3-3을 완성하여 **관리자가 전체 직원의 평가 결과를 조회하고 관리**할 수 있도록 함.

### 1.3. 작업 범위

#### 신규 구현 (4개 페이지)
1. **TotalReport/Admin/Index.razor** - 전체 평가 목록
2. **TotalReport/Admin/Details.razor** - 최종 결과 상세
3. **TotalReport/Admin/Edit.razor** - 최종 등급 설정
4. **TotalReport/Admin/ReportInit.razor** - 평가 초기화

#### 추가 작업
5. **UrlActions.cs** - TotalReport 페이지 이동 메서드 추가
6. **Admin/Index.razor** - 전체 평가 바로가기 버튼 추가 (2025년 방식)
7. **엑셀 다운로드 기능** ⭐ 중요:
   - ExcelManage.cs (엑셀 생성/삭제 유틸리티)
   - AdminViewExcel.razor + .cs (평가 리스트 다운로드)
   - AdminTaskViewExcel.razor + .cs (업무 리스트 다운로드)
   - site.js (downloadURI 함수)
   - wwwroot/files/tasks/ 폴더 생성
8. **컴포넌트 추가**:
   - AdminReportListView.razor (전체 평가 목록 테이블)
   - ReportInitModal.razor (초기화 확인 모달)

### 1.4. 예상 작업 규모

**파일 수**: 16개
- 신규 생성: 13개 (4개 페이지 + 6개 컴포넌트 + 3개 유틸리티)
- 수정: 3개 (UrlActions, site.js, Program.cs)

**코드 라인**: 약 2,575줄
- Razor 페이지: 800줄
- Code-behind: 700줄
- Repository 메서드: 500줄
- 유틸리티: 475줄 (ScoreUtils 7개 메서드)
- 기타 (모델, 설정): 100줄

**예상 작업 시간**: 5-6시간
- Step 1-4 (기본 유틸리티): 1시간
- Step 5-8 (Excel 기능): 1.5시간
- Step 9-12 (메인 페이지): 2시간
- Step 13-14 (컴포넌트 + Repository): 1.5시간

---

## 2. DB 구조 분석 (db-optimizer 결과)

### 2.1. TotalReportDb 테이블

```sql
CREATE TABLE [dbo].[TotalReportDb](
    [TRid] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Uid] [bigint] NOT NULL,  -- FK → UserDb

    -- 1차 평가 (본인)
    [User_Evaluation_1] [float] NULL,  -- 일정
    [User_Evaluation_2] [float] NULL,  -- 양
    [User_Evaluation_3] [float] NULL,  -- 결과
    [User_Evaluation_4] [nvarchar](max) NULL,  -- 코멘트

    -- 2차 평가 (팀장)
    [TeamLeader_Evaluation_1] [float] NULL,
    [TeamLeader_Evaluation_2] [float] NULL,
    [TeamLeader_Evaluation_3] [float] NULL,
    [TeamLeader_Comment] [nvarchar](max) NULL,

    -- 1차 면담 (피드백)
    [Feedback_Evaluation_1] [float] NULL,
    [Feedback_Evaluation_2] [float] NULL,
    [Feedback_Evaluation_3] [float] NULL,
    [Feedback_Comment] [nvarchar](max) NULL,

    -- 3차 평가 (임원)
    [Director_Evaluation_1] [float] NULL,
    [Director_Evaluation_2] [float] NULL,
    [Director_Evaluation_3] [float] NULL,
    [Director_Comment] [nvarchar](max) NULL,

    -- 종합 점수
    [Total_Score] [float] NULL,      -- 최종 등급 (S, A, B, C)
    [Director_Score] [float] NULL,   -- 임원 점수
    [TeamLeader_Score] [float] NULL  -- 팀장 점수
)
```

### 2.2. v_TotalReportListDB 뷰

```sql
CREATE VIEW [dbo].[v_TotalReportListDB] AS
SELECT
    A.TRid, A.Uid,
    B.UserId, B.UserName,
    A.User_Evaluation_1, A.User_Evaluation_2, A.User_Evaluation_3, A.User_Evaluation_4,
    A.TeamLeader_Evaluation_1, A.TeamLeader_Evaluation_2, A.TeamLeader_Evaluation_3,
    A.TeamLeader_Comment,
    A.Feedback_Evaluation_1, A.Feedback_Evaluation_2, A.Feedback_Evaluation_3,
    A.Feedback_Comment,
    A.Director_Evaluation_1, A.Director_Evaluation_2, A.Director_Evaluation_3,
    A.Director_Comment,
    A.Total_Score, A.Director_Score, A.TeamLeader_Score
FROM TotalReportDb A
INNER JOIN UserDb B ON A.Uid = B.Uid
```

### 2.3. v_ProcessTRListDB 뷰

```sql
CREATE VIEW [dbo].[v_ProcessTRListDB] AS
SELECT
    P.Pid, P.Uid,
    U.UserId, U.UserName,
    TL.UserId AS TeamLeader_Id, TL.UserName AS TeamLeader_Name,
    DI.UserId AS Director_Id, DI.UserName AS Director_Name,
    P.Is_Request, P.Is_Agreement, P.Agreement_Comment,
    P.Is_SubRequest, P.Is_SubAgreement, P.SubAgreement_Comment,
    P.Is_User_Submission, P.Is_Teamleader_Submission, P.Is_Director_Submission,
    P.FeedBackStatus, P.FeedBack_Submission,
    -- TotalReportDb 필드 (LEFT JOIN)
    ISNULL(TR.User_Evaluation_1, 0) AS User_Evaluation_1,
    ISNULL(TR.User_Evaluation_2, 0) AS User_Evaluation_2,
    ISNULL(TR.User_Evaluation_3, 0) AS User_Evaluation_3,
    ISNULL(TR.User_Evaluation_4, '') AS User_Evaluation_4,
    ISNULL(TR.TeamLeader_Evaluation_1, 0) AS TeamLeader_Evaluation_1,
    ISNULL(TR.TeamLeader_Evaluation_2, 0) AS TeamLeader_Evaluation_2,
    ISNULL(TR.TeamLeader_Evaluation_3, 0) AS TeamLeader_Evaluation_3,
    ISNULL(TR.TeamLeader_Comment, '') AS TeamLeader_Comment,
    ISNULL(TR.Feedback_Evaluation_1, 0) AS Feedback_Evaluation_1,
    ISNULL(TR.Feedback_Evaluation_2, 0) AS Feedback_Evaluation_2,
    ISNULL(TR.Feedback_Evaluation_3, 0) AS Feedback_Evaluation_3,
    ISNULL(TR.Feedback_Comment, '') AS Feedback_Comment,
    ISNULL(TR.Director_Evaluation_1, 0) AS Director_Evaluation_1,
    ISNULL(TR.Director_Evaluation_2, 0) AS Director_Evaluation_2,
    ISNULL(TR.Director_Evaluation_3, 0) AS Director_Evaluation_3,
    ISNULL(TR.Director_Comment, '') AS Director_Comment,
    ISNULL(TR.Total_Score, 0) AS Total_Score,
    ISNULL(TR.Director_Score, 0) AS Director_Score,
    ISNULL(TR.TeamLeader_Score, 0) AS TeamLeader_Score,
    TR.TRid
FROM ProcessDb P
INNER JOIN UserDb U ON P.Uid = U.Uid
LEFT JOIN UserDb TL ON P.TeamLeaderId = TL.Uid
LEFT JOIN UserDb DI ON P.DirectorId = DI.Uid
LEFT JOIN TotalReportDb TR ON P.Uid = TR.Uid
```

**특징**:
- ProcessDb와 TotalReportDb 조인
- TotalReportDb가 없어도 조회 가능 (LEFT JOIN + ISNULL)
- 평가 프로세스 상태와 종합 리포트를 한 번에 조회

---

## 3. 2025년 vs 2026년 차이점

### 3.1. DB 구조 변경

| 테이블/뷰 | 필드 | 2025년 | 2026년 | 변경 사항 |
|----------|------|--------|--------|----------|
| **v_ProcessTRListDB** | 사용자 ID | UserId (VARCHAR) | Uid (BIGINT) | 외래키로 변경 |
| | 팀장 ID | TeamLeader_Id (VARCHAR) | TeamLeaderId (BIGINT) | 외래키로 변경 |
| | 임원 ID | Director_Id (VARCHAR) | DirectorId (BIGINT) | 외래키로 변경 |
| | 하위 합의 | 없음 | Is_SubRequest, Is_SubAgreement, SubAgreement_Comment | 신규 추가 |
| **TotalReportDb** | 동일 | (거의 동일) | (거의 동일) | 변경 없음 |

### 3.2. .NET 버전 차이

| 항목 | 2025년 | 2026년 |
|------|--------|--------|
| .NET 버전 | .NET 7.0 | .NET 10.0 |
| 렌더 모드 | 기본 | InteractiveServer |
| Primary Constructor | 미사용 ([Inject] 방식) | 사용 권장 |
| 네임스페이스 | Pages.TotalReport.Admin | Components.Pages.Admin.TotalReport |

---

## 4. 작업 단계

### Step 1: UrlActions에 TotalReport 메서드 추가

**파일**: `Data/UrlActions.cs`

**추가 메서드**:
```csharp
// TotalReport/Admin
public void MoveTotalReportAdminIndexPage() =>
    _navigationManager.NavigateTo("/Admin/TotalReport");

public void MoveTotalReportAdminDetailsPage(long pid) =>
    _navigationManager.NavigateTo($"/Admin/TotalReport/Details/{pid}");

public void MoveTotalReportAdminEditPage(long pid) =>
    _navigationManager.NavigateTo($"/Admin/TotalReport/Edit/{pid}");

public void MoveTotalReportAdminInitPage(long pid) =>
    _navigationManager.NavigateTo($"/Admin/TotalReport/ReportInit/{pid}");
```

**테스트**: 빌드 성공 확인

---

### Step 2: ScoreUtils 유틸리티 클래스 작성

**파일**: `Utils/ScoreUtils.cs` (신규 생성)

**목적**: 점수 계산 및 등급 변환 (2025년 코드 전체 복사)

**2025년 참고**: `MdcHR25Apps.BlazorApp/Utils/ScoreUtils.cs` (전체 복사)

**코드**:
```csharp
using MdcHR26Apps.BlazorServer.Models;
using MdcHR26Apps.Models.Report;

namespace MdcHR26Apps.BlazorServer.Utils;

public class ScoreUtils
{
    #region + 종합점수백분위환산(100th percentile score)
    /// <summary>
    /// 종합점수 백분위 환산
    /// </summary>
    /// <remarks>메서드명 오타(Scror) 주의: 2025년 코드와 동일하게 유지</remarks>
    public double TotalScroreTo100thpercentile(double Score)
    {
        double resultScore = 0;

        if (Score != 0)
        {
            resultScore = ((Score) / 300) * 100;  // 2025년 기준: 300점 만점 → 100점 만점
            resultScore = Math.Round(resultScore, 2);

            if (resultScore >= 100)
            {
                resultScore = 100;
            }
        }

        return resultScore;
    }
    #endregion

    #region + 일반점수백분위환산(100th percentile score)
    public double ScroreTo100thpercentile(double Score)
    {
        double resultScore = 0;

        if (Score != 0)
        {
            resultScore = Score;
            resultScore = Math.Round(resultScore, 2);

            if (resultScore >= 100)
            {
                resultScore = 100;
            }
        }

        return resultScore;
    }
    #endregion

    #region + [1] 평가대상자 점수 계산

    #region + [1].[1] User_GetTotalScore_1
    /// <summary>
    /// 평가대상자의 종합점수1(일정준수)를 구하는 메서드
    /// </summary>
    /// <param name="lists">평가대상자의 평가리스트</param>
    /// <returns>평가대상자의 종합점수1(double)</returns>
    public double User_GetTotalScore_1(List<ReportDb> lists)
    {
        double totalScore = 0;

        if (lists != null && lists.Count != 0)
        {
            // 평가비중 적용
            foreach (var item in lists)
            {
                totalScore = totalScore +
                    (double)(
                    item.User_Evaluation_1
                    * (double)(item.Report_Item_Proportion / (double)100)
                    * (double)(item.Report_SubItem_Proportion / (double)100)
                    );
            }

            // 소숫점 2자리까지 합산하여 계산
            totalScore = Math.Round(totalScore, 2);

            // 결과값이 100점 이상이면 100으로 수정
            if (totalScore >= 100)
            {
                totalScore = 100;
            }

            return totalScore;
        }
        else
        {
            return totalScore;
        }
    }
    #endregion

    #region + [1].[2] User_GetTotalScore_2
    /// <summary>
    /// 평가대상자의 종합점수2(업무수행도)를 구하는 메서드
    /// </summary>
    /// <param name="lists">평가대상자의 평가리스트</param>
    /// <returns>평가대상자의 종합점수2(double)</returns>
    public double User_GetTotalScore_2(List<ReportDb> lists)
    {
        double totalScore = 0;

        if (lists != null && lists.Count != 0)
        {
            // 평가비중 적용
            foreach (var item in lists)
            {
                totalScore = totalScore +
                    (double)(
                    item.User_Evaluation_2
                    * (double)(item.Report_Item_Proportion / (double)100)
                    * (double)(item.Report_SubItem_Proportion / (double)100)
                    );
            }

            // 소숫점 2자리까지 합산하여 계산
            totalScore = Math.Round(totalScore, 2);

            // 결과값이 100점 이상이면 100으로 수정
            if (totalScore >= 100)
            {
                totalScore = 100;
            }

            return totalScore;
        }
        else
        {
            return totalScore;
        }
    }
    #endregion

    #region + [1].[3] User_GetTotalScore_3
    /// <summary>
    /// 평가대상자의 종합점수3(결과평가(정성))를 구하는 메서드
    /// </summary>
    /// <param name="lists">평가대상자의 평가리스트</param>
    /// <returns>평가대상자의 종합점수3(double)</returns>
    public double User_GetTotalScore_3(List<ReportDb> lists)
    {
        double totalScore = 0;

        if (lists != null && lists.Count != 0)
        {
            // 평가비중 적용
            foreach (var item in lists)
            {
                totalScore = totalScore +
                    (double)(
                    item.User_Evaluation_3
                    * (double)(item.Report_Item_Proportion / (double)100)
                    * (double)(item.Report_SubItem_Proportion / (double)100)
                    );
            }

            // 소숫점 2자리까지 합산하여 계산
            totalScore = Math.Round(totalScore, 2);

            // 결과값이 100점 이상이면 100으로 수정
            if (totalScore >= 100)
            {
                totalScore = 100;
            }

            return totalScore;
        }
        else
        {
            return totalScore;
        }
    }
    #endregion

    #endregion

    #region + GetTotalScoreRankList : 등급 목록 반환
    /// <summary>
    /// 전체 등급 목록 반환 (100점 만점 기준: S, A, B, C, D)
    /// </summary>
    public List<TotalScoreRankModel> GetTotalScoreRankList()
    {
        List<TotalScoreRankModel> lists = new List<TotalScoreRankModel>
        {
            new TotalScoreRankModel(){ TotalScoreNumber = 100, TotalScoreRankName = "S" },
            new TotalScoreRankModel(){ TotalScoreNumber = 90, TotalScoreRankName = "A" },
            new TotalScoreRankModel(){ TotalScoreNumber = 80, TotalScoreRankName = "B" },
            new TotalScoreRankModel(){ TotalScoreNumber = 70, TotalScoreRankName = "C" },
            new TotalScoreRankModel(){ TotalScoreNumber = 60, TotalScoreRankName = "D" }
        };

        return lists;
    }
    #endregion

    #region + GetTotalScore : 점수 → 등급 변환
    /// <summary>
    /// 점수를 등급으로 변환 (100점 만점 기준)
    /// </summary>
    public string GetTotalScore(double score)
    {
        string result = string.Empty;
        if (score == 100)
        {
            return result = "S";
        }
        else if (score == 90)
        {
            return result = "A";
        }
        else if (score == 80)
        {
            return result = "B";
        }
        else if (score == 70)
        {
            return result = "C";
        }
        else if (score == 60)
        {
            return result = "D";
        }
        else
        {
            return result = "-";
        }
    }
    #endregion
}
```

**DI 등록** (`Program.cs`):
```csharp
builder.Services.AddTransient<ScoreUtils>();
```

**테스트**: 빌드 성공 확인

**주의사항**:
- **2025년 코드에서 필요한 메서드 7개 포함**
- 메서드명 오타(`TotalScroreTo100thpercentile`) 유지 - 2025년 호환성
- `TotalScroreTo100thpercentile`: 300점 만점 → 100점 만점 변환 (2025년 계산식 그대로)
- `ScroreTo100thpercentile`: 일반 점수 백분위 환산
- `User_GetTotalScore_1/2/3`: 개인평가 점수 계산 (일정준수, 업무수행도, 결과평가)
  - 각 평가 항목의 점수 × 직무 비중 × 세부직무 비중 계산
  - 소숫점 2자리 반올림, 100점 이상 시 100점으로 제한
- `GetTotalScoreRankList`: 등급 목록 (S, A, B, C, D)
- `GetTotalScore(double)`: 점수를 등급으로 변환
- **using 추가**: `using MdcHR26Apps.Models.Report;` (ReportDb 사용)

---

### Step 3: TotalScoreRankModel 클래스 작성

**파일**: `Models/TotalScoreRankModel.cs` (신규 생성)

**목적**: Edit 페이지에서 등급 선택 드롭다운용

**2025년 참고**: `MdcHR25Apps.BlazorApp/Models/TotalScoreRankModel.cs`

**코드**:
```csharp
namespace MdcHR26Apps.BlazorServer.Models;

public class TotalScoreRankModel
{
    public double TotalScoreNumber { get; set; }
    public string TotalScoreRankName { get; set; } = string.Empty;
}
```

**테스트**: 빌드 성공 확인

**주의사항**:
- 2025년 코드와 동일한 모델명 사용 (TotalScoreRankModel)
- double 타입 사용 (100점 만점 기준)

---

### Step 4: AdminReportListView 컴포넌트 작성

**파일**: `Components/Pages/Components/Table/AdminReportListView.razor` (신규 생성)

**목적**: 전체 평가 목록 테이블

**2025년 참고**: `MdcHR25Apps.BlazorApp/Components/ViewPage/TotalReportViewPage/AdminReportListView.razor`

**코드**:
```razor
@using MdcHR26Apps.Models.EvaluationProcess
@using MdcHR26Apps.BlazorServer.Utils
@inject UrlActions urlActions
@inject ScoreUtils scoreUtils

@if (processTRLists == null || !processTRLists.Any())
{
    <p><em>평가 정보가 없습니다.</em></p>
}
else
{
    <!-- 데스크톱 테이블 -->
    <div class="d-none d-md-block">
        <table class="table table-hover table-responsive-md">
            <thead>
                <tr>
                    <th>#</th>
                    <th>ID</th>
                    <th>이름</th>
                    <th>상태</th>
                    <th>최종등급</th>
                    <th>비고</th>
                </tr>
            </thead>
            <tbody>
                @{
                    int sortNo = 1;
                }
                @foreach (var item in processTRLists)
                {
                    <tr>
                        <td>@sortNo</td>
                        <td>@item.UserId</td>
                        <td>@item.UserName</td>
                        <td>@GetDisplayStatus(item)</td>
                        <td>@scoreUtils.GetTotalScore(item.Total_Score)</td>
                        <td>
                            <button type="button" class="btn btn-sm btn-info"
                                    @onclick="() => urlActions.MoveTotalReportAdminDetailsPage(item.Pid)">
                                상세
                            </button>
                        </td>
                    </tr>
                    sortNo++;
                }
            </tbody>
        </table>
    </div>

    <!-- 모바일 카드 -->
    <div class="d-block d-md-none">
        @foreach (var item in processTRLists)
        {
            <div class="card mb-2">
                <div class="card-body">
                    <h6 class="card-title">@item.UserName (@item.UserId)</h6>
                    <p class="card-text">
                        <strong>상태:</strong> @GetDisplayStatus(item)<br/>
                        <strong>최종등급:</strong> @scoreUtils.GetTotalScore(item.Total_Score)
                    </p>
                    <button type="button" class="btn btn-sm btn-info"
                            @onclick="() => urlActions.MoveTotalReportAdminDetailsPage(item.Pid)">
                        상세
                    </button>
                </div>
            </div>
        }
    </div>
}

@code {
    [Parameter]
    public List<v_ProcessTRListDB>? processTRLists { get; set; }

    private string GetDisplayStatus(v_ProcessTRListDB item)
    {
        if (item.Is_Director_Submission) return "임원평가완료";
        if (item.Is_Teamleader_Submission) return "팀장평가완료";
        if (item.Is_User_Submission) return "본인평가완료";
        if (item.Is_SubAgreement) return "세부합의완료";
        if (item.Is_Agreement) return "직무합의완료";
        if (item.Is_SubRequest) return "세부합의요청";
        if (item.Is_Request) return "직무합의요청";
        return "대기중";
    }
}
```

**테스트**: 빌드 성공 확인

---

### Step 5: ExcelManage 클래스 작성 ⭐ 엑셀 기능

**파일**: `Data/ExcelManage.cs` (신규 생성)

**목적**: ClosedXML을 사용한 엑셀 생성/삭제

**2025년 참고**: `MdcHR25Apps.BlazorApp/Data/ExcelManage.cs`

**NuGet 패키지 확인**:
```xml
<PackageReference Include="ClosedXML" Version="0.104.2" />
```
- 이미 Phase 3-1에서 설치됨

**코드**:
```csharp
using ClosedXML.Excel;
using MdcHR26Apps.BlazorServer.Utils;
using MdcHR26Apps.Models.EvaluationProcess;
using MdcHR26Apps.Models.Report;

namespace MdcHR26Apps.BlazorServer.Data;

public class ExcelManage(IWebHostEnvironment environment)
{
    private readonly IWebHostEnvironment _environment = environment;
    private readonly string _containerName = "files";
    private readonly string _folderPath = Path.Combine(environment.WebRootPath, "files");
    private readonly ScoreUtils _scoreUtils = new();

    #region + [1] CreateAdminExcelFile : 관리자용 평가 리스트 엑셀 생성
    /// <summary>
    /// 관리자용 전체 평가 현황 엑셀 파일 생성
    /// </summary>
    public bool CreateAdminExcelFile(List<v_ProcessTRListDB> data, string fileFolderPath, string fileName)
    {
        string exportFile = Path.Combine(_folderPath, fileFolderPath, fileName);

        if (File.Exists(exportFile))
        {
            File.Delete(exportFile);
        }

        if (!File.Exists(exportFile))
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("평가현황");

                // 헤더 설정
                worksheet.Cells("A1").Value = "No";
                worksheet.Cells("B1").Value = "이름";
                worksheet.Cells("C1").Value = "평가제출";
                worksheet.Cells("D1").Value = "평가자점수";
                worksheet.Cells("E1").Value = "부서장(팀장)이름";
                worksheet.Cells("F1").Value = "평가완료(팀장)";
                worksheet.Cells("G1").Value = "부서장(팀장)점수";
                worksheet.Cells("H1").Value = "2026년 종합평가";  // ← 2026년으로 변경
                worksheet.Cells("I1").Value = "미래성장방안";
                worksheet.Cells("J1").Value = "임원이름";
                worksheet.Cells("K1").Value = "평가완료(임원)";
                worksheet.Cells("L1").Value = "최종점수(임원)";
                worksheet.Cells("M1").Value = "종합평가";
                worksheet.Cells("N1").Value = "종합등급";

                int recordIndex = 2;

                foreach (var item in data)
                {
                    worksheet.Cell(recordIndex, 1).Value = recordIndex - 1;
                    worksheet.Cell(recordIndex, 2).Value = item.UserName;
                    worksheet.Cell(recordIndex, 3).Value = IsSubmission(item.Is_User_Submission);
                    worksheet.Cell(recordIndex, 4).Value = IsScore(item.User_Evaluation_1, item.User_Evaluation_2, item.User_Evaluation_3);
                    worksheet.Cell(recordIndex, 5).Value = item.TeamLeader_Name ?? "-";  // 2026년: null 처리
                    worksheet.Cell(recordIndex, 6).Value = IsSubmission(item.Is_Teamleader_Submission);
                    worksheet.Cell(recordIndex, 7).Value = item.TeamLeader_Score;
                    worksheet.Cell(recordIndex, 8).Value = item.TeamLeader_Comment ?? string.Empty;
                    worksheet.Cell(recordIndex, 9).Value = item.Feedback_Comment ?? string.Empty;
                    worksheet.Cell(recordIndex, 10).Value = item.Director_Name ?? "-";  // 2026년: null 처리
                    worksheet.Cell(recordIndex, 11).Value = IsSubmission(item.Is_Director_Submission);
                    worksheet.Cell(recordIndex, 12).Value = item.Director_Score;
                    worksheet.Cell(recordIndex, 13).Value = item.Director_Comment ?? string.Empty;
                    worksheet.Cell(recordIndex, 14).Value = _scoreUtils.GetTotalScore(item.Total_Score);

                    recordIndex++;
                }

                workbook.SaveAs(exportFile);
            }
        }

        return File.Exists(exportFile);
    }
    #endregion

    #region + [2] CreateExcelFile : 세부 업무 리스트 엑셀 생성
    /// <summary>
    /// 세부 업무 리스트 엑셀 파일 생성
    /// </summary>
    public bool CreateExcelFile(List<v_ReportTaskListDB> data, string fileFolderPath, string fileName)
    {
        string exportFile = Path.Combine(_folderPath, fileFolderPath, fileName);

        if (File.Exists(exportFile))
        {
            File.Delete(exportFile);
        }

        if (!File.Exists(exportFile))
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("세부업무");

                // 헤더 설정
                worksheet.Cells("A1").Value = "No";
                worksheet.Cells("B1").Value = "평가자ID";
                worksheet.Cells("C1").Value = "평가자명";
                worksheet.Cells("D1").Value = "직무구분";
                worksheet.Cells("E1").Value = "직무명";
                worksheet.Cells("F1").Value = "직무비중(%)";
                worksheet.Cells("G1").Value = "세부직무명";
                worksheet.Cells("H1").Value = "세부직무비중(%)";
                worksheet.Cells("I1").Value = "업무명";
                worksheet.Cells("J1").Value = "업무목표";
                worksheet.Cells("K1").Value = "업무목표달성도(%)";
                worksheet.Cells("L1").Value = "업무목표달성일";
                worksheet.Cells("M1").Value = "업무난이도";

                int recordIndex = 2;

                foreach (var item in data)
                {
                    worksheet.Cell(recordIndex, 1).Value = recordIndex - 1;
                    worksheet.Cell(recordIndex, 2).Value = item.UserId;
                    worksheet.Cell(recordIndex, 3).Value = item.UserName;
                    worksheet.Cell(recordIndex, 4).Value = item.Report_Item_Name_1;
                    worksheet.Cell(recordIndex, 5).Value = item.Report_Item_Name_2;
                    worksheet.Cell(recordIndex, 6).Value = item.Report_Item_Proportion;
                    worksheet.Cell(recordIndex, 7).Value = item.Report_SubItem_Name;
                    worksheet.Cell(recordIndex, 8).Value = item.Report_SubItem_Proportion;
                    worksheet.Cell(recordIndex, 9).Value = item.TaskName ?? "-";
                    worksheet.Cell(recordIndex, 10).Value = item.TaskObjective ?? "-";
                    worksheet.Cell(recordIndex, 11).Value = item.TargetProportion;
                    worksheet.Cell(recordIndex, 12).Value = item.TargetDate?.ToString("yyyy-MM-dd") ?? "-";
                    worksheet.Cell(recordIndex, 13).Value = LevelToName(item.TaskLevel);

                    recordIndex++;
                }

                workbook.SaveAs(exportFile);
            }
        }

        return File.Exists(exportFile);
    }
    #endregion

    #region + [3] DeleteExcelFile : 엑셀 파일 삭제
    /// <summary>
    /// 엑셀 파일 삭제
    /// </summary>
    public void DeleteExcelFile(string fileFolderPath, string fileName)
    {
        string deleteFile = Path.Combine(_folderPath, fileFolderPath, fileName);
        if (File.Exists(deleteFile))
        {
            File.Delete(deleteFile);
        }
    }
    #endregion

    #region + 유틸리티 메서드
    /// <summary>
    /// 업무 난이도 숫자 → 한글 변환
    /// </summary>
    private string LevelToName(double? levelNo)
    {
        if (levelNo == null) return "-";

        return levelNo switch
        {
            1.2 => "매우 어려움",
            1.1 => "어려움",
            1.0 => "보통",
            0.9 => "쉬움",
            0.8 => "매우 쉬움",
            _ => "-"
        };
    }

    /// <summary>
    /// 점수 계산 (3개 점수 합산 → 100점 만점)
    /// </summary>
    private double IsScore(double? score1, double? score2, double? score3)
    {
        double score = (score1 ?? 0) + (score2 ?? 0) + (score3 ?? 0);
        return _scoreUtils.TotalScroreTo100thpercentile(score);  // 오타 주의: 2025년 코드와 일치
    }

    /// <summary>
    /// 제출 여부 → Y/N 변환
    /// </summary>
    private string IsSubmission(bool status) => status ? "Y" : "N";
    #endregion
}
```

**주의사항**:
- `TotalScroreTo100thpercentile` 메서드는 Step 2에서 이미 추가됨
- 메서드명 오타(`Scror`) 유지 - 2025년 코드와 일치

**DI 등록** (`Program.cs`):
```csharp
builder.Services.AddTransient<ExcelManage>();
```

**wwwroot 폴더 생성**:
```bash
mkdir -p wwwroot/files/tasks
```

**테스트**: 빌드 성공 확인

---

### Step 6: AdminViewExcel 컴포넌트 작성

**파일**: `Components/Pages/Components/Common/AdminViewExcel.razor` (신규 생성)

**목적**: 평가 리스트 엑셀 다운로드 버튼

**2025년 참고**: `Components/CommonView/AdminViewExcel.razor`

**코드**:
```razor
<button class="btn btn-outline-success mb-1" @onclick="ClickExportXLS">
    <i class="bi bi-cloud-download"></i>
    평가리스트 다운로드
</button>
```

**Code-behind** (`AdminViewExcel.razor.cs`):
```csharp
using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.EvaluationProcess;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Common;

public partial class AdminViewExcel(
    ExcelManage excelManage,
    IJSRuntime js,
    NavigationManager navigationManager)
{
    [Parameter]
    public List<v_ProcessTRListDB>? ProcessTRLists { get; set; }

    [Parameter]
    public EventCallback<string> CommentChanged { get; set; }

    private string comment = string.Empty;

    private async Task ClickExportXLS()
    {
        if (ProcessTRLists == null || ProcessTRLists.Count == 0)
        {
            comment = "평가자가 없어 파일을 생성하지 못했습니다.";
            await CommentChanged.InvokeAsync(comment);
            return;
        }

        string fileFolderPath = "tasks";
        string fileNameAdd = DateTime.Now.ToString("yy_MM_dd_HH_mm_ss");
        string fileName = $"평가리스트_관리용_{fileNameAdd}.xlsx";

        // NavigationManager.BaseUri 사용 (자동으로 현재 환경 URL 감지)
        string fileUrl = $"{navigationManager.BaseUri}files/tasks/{fileName}";

        bool isCreateResult = excelManage.CreateAdminExcelFile(ProcessTRLists, fileFolderPath, fileName);

        if (isCreateResult)
        {
            comment = "파일을 생성했습니다.";
            await CommentChanged.InvokeAsync(comment);

            await js.InvokeVoidAsync("downloadURI", fileUrl, fileName);
            await Task.Delay(500);

            excelManage.DeleteExcelFile(fileFolderPath, fileName);

            await Task.Delay(3000);
            comment = string.Empty;
            await CommentChanged.InvokeAsync(comment);
        }
        else
        {
            comment = "파일을 생성하지 못했습니다.";
            await CommentChanged.InvokeAsync(comment);
        }
    }
}
```

**장점**:
- 하드코딩된 URL 제거
- `appsettings.json` 설정 불필요
- `NavigationManager.BaseUri`가 자동으로 환경 감지:
  - 개발: `https://localhost:5001/`
  - 배포: `https://hr2026.yourdomain.kr/`
- 포트 변경에도 자동 대응

**테스트**: 빌드 성공 확인

---

### Step 7: AdminTaskViewExcel 컴포넌트 작성

**파일**: `Components/Pages/Components/Common/AdminTaskViewExcel.razor` (신규 생성)

**목적**: 세부 업무 리스트 엑셀 다운로드 버튼

**2025년 참고**: `Components/CommonView/AdminTaskViewExcel.razor`

**코드**:
```razor
<button class="btn btn-outline-success mb-1" @onclick="ClickExportXLS">
    <i class="bi bi-cloud-download"></i>
    업무리스트 다운로드
</button>
```

**Code-behind** (`AdminTaskViewExcel.razor.cs`):
```csharp
using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.Report;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Common;

public partial class AdminTaskViewExcel(
    ExcelManage excelManage,
    IJSRuntime js,
    NavigationManager navigationManager)
{
    [Parameter]
    public List<v_ReportTaskListDB>? ReportTaskListDB { get; set; }

    [Parameter]
    public EventCallback<string> CommentChanged { get; set; }

    private string comment = string.Empty;

    private async Task ClickExportXLS()
    {
        if (ReportTaskListDB == null || ReportTaskListDB.Count == 0)
        {
            comment = "합의된 세부업무가 없어 파일을 생성하지 못했습니다.";
            await CommentChanged.InvokeAsync(comment);
            return;
        }

        string fileFolderPath = "tasks";
        string fileNameAdd = DateTime.Now.ToString("yy_MM_dd_HH_mm_ss");
        string fileName = $"관리자용_업무리스트_{fileNameAdd}.xlsx";

        // NavigationManager.BaseUri 사용 (자동으로 현재 환경 URL 감지)
        string fileUrl = $"{navigationManager.BaseUri}files/tasks/{fileName}";

        bool isCreateResult = excelManage.CreateExcelFile(ReportTaskListDB, fileFolderPath, fileName);

        if (isCreateResult)
        {
            comment = "파일을 생성했습니다.";
            await CommentChanged.InvokeAsync(comment);

            await js.InvokeVoidAsync("downloadURI", fileUrl, fileName);
            await Task.Delay(500);

            excelManage.DeleteExcelFile(fileFolderPath, fileName);

            await Task.Delay(3000);
            comment = string.Empty;
            await CommentChanged.InvokeAsync(comment);
        }
        else
        {
            comment = "파일을 생성하지 못했습니다.";
            await CommentChanged.InvokeAsync(comment);
        }
    }
}
```

**장점**:
- 하드코딩된 URL 제거
- Step 6과 동일한 방식 적용

**테스트**: 빌드 성공 확인

---

### Step 8: JavaScript downloadURI 함수 추가

**파일**: `wwwroot/js/site.js`

**추가 코드**:
```javascript
// 엑셀 파일 다운로드
window.downloadURI = function (uri, name) {
    var link = document.createElement("a");
    link.download = name;
    link.href = uri;
    link.target = "_blank";
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}
```

**App.razor에 스크립트 추가 확인**:
```html
<script src="js/site.js"></script>
```

**테스트**: 브라우저 콘솔에서 `downloadURI` 함수 존재 확인

---

### Step 9: Index.razor 작성

**파일**: `Components/Pages/Admin/TotalReport/Index.razor` (신규 생성)

**경로**: `/Admin/TotalReport`

**2025년 참고**: `TotalReport/Admin/Index.razor`

**코드**:
```razor
@page "/Admin/TotalReport"
@page "/Admin/TotalReport/Index"
@rendermode InteractiveServer

<PageTitle>전체 평가</PageTitle>

<h3>전체 평가</h3>
<hr />

<SearchbarComponent searchTerm="@searchTerm"
                    SearchAction="HandleSearchValueChanged"
                    SearchInitAction="SearchInit" />

@if (processTRLists == null || reportTaskLists == null)
{
    <p><em>Loading...</em></p>
}
else
{
    <div class="input-group mb-1">
        <!-- 엑셀 다운로드 버튼 -->
        <AdminTaskViewExcel ReportTaskListDB="reportTaskLists" CommentChanged="HandleCommentChanged" />
        <AdminViewExcel ProcessTRLists="processTRLists" CommentChanged="HandleCommentChanged" />
    </div>
    <DisplayResultText Comment="@comment" />
    <hr />
    <AdminReportListView processTRLists="processTRLists" />
}
```

**Code-behind** (`Index.razor.cs`):
```csharp
using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.EvaluationProcess;
using MdcHR26Apps.Models.Report;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin.TotalReport;

public partial class Index(
    Iv_ProcessTRListDBRepository processTRRepository,
    Iv_ReportTaskListDBRepository reportTaskRepository,
    LoginStatusService loginStatusService,
    UrlActions urlActions)
{
    private List<v_ProcessTRListDB>? processTRLists;
    private List<v_ProcessTRListDB>? allProcessTRLists;
    private List<v_ReportTaskListDB>? reportTaskLists;  // 세부 업무 리스트 추가
    private string searchTerm = string.Empty;
    private string comment = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await CheckLogined();
        await LoadData();
    }

    private async Task CheckLogined()
    {
        await Task.CompletedTask;
        if (!loginStatusService.IsloginAndIsAdminCheck())
        {
            StateHasChanged();
            urlActions.MoveMainPage();
        }
    }

    private async Task LoadData()
    {
        allProcessTRLists = await processTRRepository.GetAllAsync();
        processTRLists = allProcessTRLists;
        reportTaskLists = await reportTaskRepository.GetAllAsync();  // 세부 업무 리스트 로드
    }

    private void HandleSearchValueChanged(string value)
    {
        searchTerm = value;
        SearchAction();
    }

    private void SearchInit()
    {
        searchTerm = string.Empty;
        processTRLists = allProcessTRLists;
    }

    private void SearchAction()
    {
        if (string.IsNullOrEmpty(searchTerm))
        {
            processTRLists = allProcessTRLists;
            return;
        }

        processTRLists = allProcessTRLists?
            .Where(x =>
                (x.UserId != null && x.UserId.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (x.UserName != null && x.UserName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
            .ToList();
    }

    private void HandleCommentChanged(string value)
    {
        comment = value;
        StateHasChanged();
    }
}
```

**테스트**:
1. 빌드 성공 확인
2. 로그인 후 `/Admin/TotalReport` 접속
3. 전체 평가 목록 표시 확인

---

### Step 10: Details.razor 작성

**파일**: `Components/Pages/Admin/TotalReport/Details.razor` (신규 생성)

**경로**: `/Admin/TotalReport/Details/{Id:long}`

**2025년 참고**: `TotalReport/Admin/Details.razor`

**코드**:
```razor
@page "/Admin/TotalReport/Details/{Id:long}"
@rendermode InteractiveServer

<PageTitle>최종 결과 상세</PageTitle>

<h3>최종 결과 상세</h3>

@if (model == null)
{
    <p><em>Loading...</em></p>
}
else
{
    <div class="row">
        <div class="col-md-12">
            <ul class="list-group">
                <li class="list-group-item">
                    <label for="UserId">ID : </label>
                    @model.UserId
                </li>
                <li class="list-group-item">
                    <label for="UserName">이름 : </label>
                    @model.UserName
                </li>
                <li class="list-group-item">
                    <label for="Status">상태 : </label>
                    @GetDisplayStatus()
                </li>
                <li class="list-group-item">
                    <label for="TotalScore">최종결과 : </label>
                    @scoreUtils.GetTotalScore(model.Total_Score)
                </li>
                <li class="list-group-item">
                    <label for="TotalReportStatus">종합레포트 : </label>
                    @isTotalReport
                </li>
                <li class="list-group-item">
                    <label for="ReportStatus">평가레포트 개수 : </label>
                    @reportCount 개
                </li>
                <li class="list-group-item">
                    <label for="TasksStatus">세부업무개수 : </label>
                    @taskListCount 개
                </li>
                <li class="list-group-item">
                    <label for="SubAgreementStatus">세부직무합의 개수 : </label>
                    @subAgreementCount 개
                </li>
                <li class="list-group-item">
                    <label for="AgreementStatus">직무합의 개수 : </label>
                    @agreementCount 개
                </li>
            </ul>
        </div>
        <hr />
        <div class="col-md-12">
            <button type="button" class="btn btn-outline-primary" @onclick="urlActions.MoveTotalReportAdminIndexPage">목록</button>
            <button type="button" class="btn btn-outline-secondary" @onclick="() => urlActions.MoveTotalReportAdminEditPage(model.Pid)">수정</button>
            <button type="button" class="btn btn-outline-danger" @onclick="() => urlActions.MoveTotalReportAdminInitPage(model.Pid)">초기화</button>
        </div>
    </div>
}
```

**Code-behind** (`Details.razor.cs`):
```csharp
using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.BlazorServer.Utils;
using MdcHR26Apps.Models.Agreement;
using MdcHR26Apps.Models.EvaluationProcess;
using MdcHR26Apps.Models.Report;
using MdcHR26Apps.Models.SubAgreement;
using MdcHR26Apps.Models.Tasks;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin.TotalReport;

public partial class Details(
    Iv_ProcessTRListDBRepository processTRRepository,
    IReportRepository reportRepository,
    ITasksRepository tasksRepository,
    IAgreementRepository agreementRepository,
    ISubAgreementRepository subAgreementRepository,
    LoginStatusService loginStatusService,
    UrlActions urlActions,
    ScoreUtils scoreUtils)
{
    [Parameter]
    public long Id { get; set; }

    private v_ProcessTRListDB? model;
    private string isTotalReport = "없음";
    private int reportCount = 0;
    private int taskListCount = 0;
    private int subAgreementCount = 0;
    private int agreementCount = 0;

    protected override async Task OnInitializedAsync()
    {
        await CheckLogined();
        await LoadData();
    }

    private async Task CheckLogined()
    {
        await Task.CompletedTask;
        if (!loginStatusService.IsloginAndIsAdminCheck())
        {
            StateHasChanged();
            urlActions.MoveMainPage();
        }
    }

    private async Task LoadData()
    {
        model = await processTRRepository.GetByIdAsync(Id);

        if (model != null)
        {
            isTotalReport = model.TRid > 0 ? "있음" : "없음";
            reportCount = await reportRepository.GetCountByUidAsync(model.Uid);
            taskListCount = await tasksRepository.GetCountByUserAsync(model.Uid);
            subAgreementCount = await subAgreementRepository.GetCountByUidAsync(model.Uid);
            agreementCount = await agreementRepository.GetCountByUidAsync(model.Uid);
        }
    }

    private string GetDisplayStatus()
    {
        if (model == null) return "-";

        if (model.Is_Director_Submission) return "임원평가완료";
        if (model.Is_Teamleader_Submission) return "팀장평가완료";
        if (model.Is_User_Submission) return "본인평가완료";
        if (model.Is_SubAgreement) return "세부합의완료";
        if (model.Is_Agreement) return "직무합의완료";
        if (model.Is_SubRequest) return "세부합의요청";
        if (model.Is_Request) return "직무합의요청";
        return "대기중";
    }
}
```

**필요한 Repository 메서드**:
```csharp
// IReportRepository
Task<int> GetCountByUidAsync(long uid);

// ITasksRepository
Task<int> GetCountByUserAsync(long uid);

// IAgreementRepository
Task<int> GetCountByUidAsync(long uid);

// ISubAgreementRepository
Task<int> GetCountByUidAsync(long uid);

// Iv_ProcessTRListDBRepository
Task<v_ProcessTRListDB?> GetByIdAsync(long pid);
```

**테스트**:
1. 빌드 성공 확인
2. 상세 페이지 접속
3. 사용자 정보, 평가 상태, 개수 표시 확인

---

### Step 11: Edit.razor 작성

**파일**: `Components/Pages/Admin/TotalReport/Edit.razor` (신규 생성)

**경로**: `/Admin/TotalReport/Edit/{Id:long}`

**2025년 참고**: `TotalReport/Admin/Edit.razor`

**코드**:
```razor
@page "/Admin/TotalReport/Edit/{Id:long}"
@rendermode InteractiveServer

<PageTitle>최종 결과 설정</PageTitle>

<h3>최종 결과 설정</h3>

@if (!string.IsNullOrEmpty(resultText))
{
    <p><em>@resultText</em></p>
}

@if (model == null)
{
    <p><em>Loading...</em></p>
}
else if (model.TRid == 0)
{
    <div class="input-group mb-1">
        <button type="button" class="btn btn-danger" @onclick="() => urlActions.MoveTotalReportAdminDetailsPage(model.Pid)">이전페이지</button>
        <button type="button" class="btn btn-secondary" @onclick="urlActions.MoveTotalReportAdminIndexPage">목록</button>
    </div>
    <hr />
    <p><em>평가 작성이 되지 않았습니다.</em></p>
    <p><em>이전 페이지로 돌아가주세요.</em></p>
}
else
{
    @if (totalScoreList == null)
    {
        <p><em>Loading...</em></p>
    }
    else
    {
        <div class="row">
            <div class="col-md-12">
                <EditForm Model="editModel">
                    <div class="input-group mb-1">
                        <span class="input-group-text mb-1" style="width: 6em;">ID</span>
                        <InputText id="UserId" class="form-control mb-1" @bind-Value="@model.UserId" disabled />
                    </div>
                    <div class="input-group mb-1">
                        <span class="input-group-text mb-1" style="width: 6em;">이름</span>
                        <InputText id="UserName" class="form-control mb-1" @bind-Value="@model.UserName" disabled />
                    </div>
                    <div class="input-group mb-1">
                        <span class="input-group-text mb-1" style="width: 6em;">최종등급</span>
                        <InputSelect id="TotalScore" class="form-select mb-1" @bind-Value="@editModel.Total_Score">
                            @foreach (var item in totalScoreList)
                            {
                                <option value="@item.TotalScoreNumber">@item.TotalScoreRankName</option>
                            }
                        </InputSelect>
                    </div>
                    <div class="input-group mb-1">
                        <button type="button" class="btn btn-success" @onclick="UpdateTotalReport">변경</button>
                        <button type="button" class="btn btn-danger" @onclick="() => urlActions.MoveTotalReportAdminDetailsPage(model.Pid)">이전페이지</button>
                        <button type="button" class="btn btn-info" @onclick="urlActions.MoveTotalReportAdminIndexPage">목록</button>
                    </div>
                </EditForm>
            </div>
        </div>
    }
}
```

**Code-behind** (`Edit.razor.cs`):
```csharp
using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.BlazorServer.Models;
using MdcHR26Apps.BlazorServer.Utils;
using MdcHR26Apps.Models.EvaluationProcess;
using MdcHR26Apps.Models.TotalReport;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin.TotalReport;

public partial class Edit(
    Iv_ProcessTRListDBRepository processTRRepository,
    ITotalReportRepository totalReportRepository,
    LoginStatusService loginStatusService,
    UrlActions urlActions,
    ScoreUtils scoreUtils)
{
    [Parameter]
    public long Id { get; set; }

    private v_ProcessTRListDB? model;
    private TotalReportDb editModel = new();
    private List<TotalScoreRankModel>? totalScoreList;
    private string resultText = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await CheckLogined();
        await LoadData();
    }

    private async Task CheckLogined()
    {
        await Task.CompletedTask;
        if (!loginStatusService.IsloginAndIsAdminCheck())
        {
            StateHasChanged();
            urlActions.MoveMainPage();
        }
    }

    private async Task LoadData()
    {
        model = await processTRRepository.GetByIdAsync(Id);

        if (model != null && model.TRid > 0)
        {
            var totalReport = await totalReportRepository.GetByUidAsync(model.Uid);
            if (totalReport != null)
            {
                editModel = totalReport;
            }
        }

        // 등급 목록 초기화 - ScoreUtils 사용
        totalScoreList = scoreUtils.GetTotalScoreRankList();
    }

    private async Task UpdateTotalReport()
    {
        if (editModel.TRid == 0)
        {
            resultText = "종합레포트가 존재하지 않습니다.";
            return;
        }

        var success = await totalReportRepository.UpdateAsync(editModel);

        if (success)
        {
            resultText = "최종 등급 변경 완료";
            await Task.Delay(1000);
            urlActions.MoveTotalReportAdminDetailsPage(model!.Pid);
        }
        else
        {
            resultText = "변경 실패";
        }
    }
}
```

**필요한 Repository 메서드**:
```csharp
// ITotalReportRepository
Task<TotalReportDb?> GetByUidAsync(long uid);
Task<bool> UpdateAsync(TotalReportDb model);
```

**테스트**:
1. 빌드 성공 확인
2. Edit 페이지 접속
3. 등급 변경 후 저장
4. Details 페이지로 리디렉션 확인

---

### Step 12: ReportInit.razor 작성

**파일**: `Components/Pages/Admin/TotalReport/ReportInit.razor` (신규 생성)

**경로**: `/Admin/TotalReport/ReportInit/{Id:long}`

**2025년 참고**: `TotalReport/Admin/ReportInit.razor`

**코드**:
```razor
@page "/Admin/TotalReport/ReportInit/{Id:long}"
@rendermode InteractiveServer

<PageTitle>평가 초기화</PageTitle>

<h3>평가 초기화</h3>

@if (model == null)
{
    <p><em>Loading...</em></p>
}
else
{
    <div class="row">
        <div class="col-md-12">
            <ul class="list-group">
                <li class="list-group-item">
                    <label for="UserId">ID : </label>
                    @model.UserId
                </li>
                <li class="list-group-item">
                    <label for="UserName">이름 : </label>
                    @model.UserName
                </li>
                <li class="list-group-item">
                    <label for="Status">상태 : </label>
                    @GetDisplayStatus()
                </li>
                <li class="list-group-item">
                    <label for="TotalReportStatus">종합레포트 : </label>
                    @isTotalReport
                </li>
                <li class="list-group-item">
                    <label for="ReportStatus">평가레포트 개수 : </label>
                    @reportCount 개
                </li>
                <li class="list-group-item">
                    <label for="TasksStatus">세부업무개수 : </label>
                    @taskListCount 개
                </li>
                <li class="list-group-item">
                    <label for="SubAgreementStatus">세부직무합의 개수 : </label>
                    @subAgreementCount 개
                </li>
                <li class="list-group-item">
                    <label for="AgreementStatus">직무합의 개수 : </label>
                    @agreementCount 개
                </li>
            </ul>
        </div>
        <hr />
        <div class="col-md-12">
            <button type="button" class="btn btn-outline-primary" @onclick="urlActions.MoveTotalReportAdminIndexPage">목록</button>
            <button type="button" class="btn btn-outline-danger" @onclick="ShowInitModal">초기화</button>
        </div>

        @if (showModal)
        {
            <ReportInitModal Pid="@model.Pid" OnConfirm="HandleInitConfirm" OnCancel="HandleInitCancel" />
        }
    </div>
}
```

**Code-behind** (`ReportInit.razor.cs`):
```csharp
using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.Agreement;
using MdcHR26Apps.Models.EvaluationProcess;
using MdcHR26Apps.Models.Report;
using MdcHR26Apps.Models.SubAgreement;
using MdcHR26Apps.Models.Tasks;
using MdcHR26Apps.Models.TotalReport;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin.TotalReport;

public partial class ReportInit(
    Iv_ProcessTRListDBRepository processTRRepository,
    IProcessRepository processRepository,
    ITotalReportRepository totalReportRepository,
    IReportRepository reportRepository,
    ITasksRepository tasksRepository,
    IAgreementRepository agreementRepository,
    ISubAgreementRepository subAgreementRepository,
    LoginStatusService loginStatusService,
    UrlActions urlActions)
{
    [Parameter]
    public long Id { get; set; }

    private v_ProcessTRListDB? model;
    private string isTotalReport = "없음";
    private int reportCount = 0;
    private int taskListCount = 0;
    private int subAgreementCount = 0;
    private int agreementCount = 0;
    private bool showModal = false;

    protected override async Task OnInitializedAsync()
    {
        await CheckLogined();
        await LoadData();
    }

    private async Task CheckLogined()
    {
        await Task.CompletedTask;
        if (!loginStatusService.IsloginAndIsAdminCheck())
        {
            StateHasChanged();
            urlActions.MoveMainPage();
        }
    }

    private async Task LoadData()
    {
        model = await processTRRepository.GetByIdAsync(Id);

        if (model != null)
        {
            isTotalReport = model.TRid > 0 ? "있음" : "없음";
            reportCount = await reportRepository.GetCountByUidAsync(model.Uid);
            taskListCount = await tasksRepository.GetCountByUserAsync(model.Uid);
            subAgreementCount = await subAgreementRepository.GetCountByUidAsync(model.Uid);
            agreementCount = await agreementRepository.GetCountByUidAsync(model.Uid);
        }
    }

    private string GetDisplayStatus()
    {
        if (model == null) return "-";

        if (model.Is_Director_Submission) return "임원평가완료";
        if (model.Is_Teamleader_Submission) return "팀장평가완료";
        if (model.Is_User_Submission) return "본인평가완료";
        if (model.Is_SubAgreement) return "세부합의완료";
        if (model.Is_Agreement) return "직무합의완료";
        if (model.Is_SubRequest) return "세부합의요청";
        if (model.Is_Request) return "직무합의요청";
        return "대기중";
    }

    private void ShowInitModal()
    {
        showModal = true;
    }

    private async Task HandleInitConfirm()
    {
        if (model == null) return;

        // 1. TotalReportDb 삭제
        if (model.TRid > 0)
        {
            await totalReportRepository.DeleteByUidAsync(model.Uid);
        }

        // 2. ReportDb 전체 삭제
        await reportRepository.DeleteAllByUidAsync(model.Uid);

        // 3. TasksDb 전체 삭제
        await tasksRepository.DeleteAllByUserAsync(model.Uid);

        // 4. SubAgreementDb 전체 삭제
        await subAgreementRepository.DeleteAllByUidAsync(model.Uid);

        // 5. AgreementDb 전체 삭제
        await agreementRepository.DeleteAllByUidAsync(model.Uid);

        // 6. ProcessDb 상태 초기화
        var process = await processRepository.GetByUidAsync(model.Uid);
        if (process != null)
        {
            process.Is_Request = false;
            process.Is_Agreement = false;
            process.Agreement_Comment = string.Empty;
            process.Is_SubRequest = false;
            process.Is_SubAgreement = false;
            process.SubAgreement_Comment = string.Empty;
            process.Is_User_Submission = false;
            process.Is_Teamleader_Submission = false;
            process.Is_Director_Submission = false;
            process.FeedBackStatus = false;
            process.FeedBack_Submission = false;

            await processRepository.UpdateAsync(process);
        }

        showModal = false;
        urlActions.MoveTotalReportAdminIndexPage();
    }

    private void HandleInitCancel()
    {
        showModal = false;
    }
}
```

**필요한 Repository 메서드**:
```csharp
// ITotalReportRepository
Task<bool> DeleteByUidAsync(long uid);

// IReportRepository
Task<bool> DeleteAllByUidAsync(long uid);

// ITasksRepository
Task<bool> DeleteAllByUserAsync(long uid);

// IAgreementRepository
Task<bool> DeleteAllByUidAsync(long uid);

// ISubAgreementRepository
Task<bool> DeleteAllByUidAsync(long uid);

// IProcessRepository
Task<ProcessDb?> GetByUidAsync(long uid);
Task<bool> UpdateAsync(ProcessDb model);
```

**테스트**:
1. 빌드 성공 확인
2. ReportInit 페이지 접속
3. 초기화 버튼 클릭
4. 모달 확인 후 초기화 실행
5. ProcessDb 상태 초기화 확인

---

### Step 13: ReportInitModal 컴포넌트 작성

**파일**: `Components/Pages/Components/Modal/ReportInitModal.razor` (신규 생성)

**목적**: 평가 초기화 확인 모달

**코드**:
```razor
<div class="modal fade show d-block" tabindex="-1" role="dialog">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">평가 초기화 확인</h5>
            </div>
            <div class="modal-body">
                <p><strong>경고:</strong> 다음 데이터가 모두 삭제됩니다:</p>
                <ul>
                    <li>종합 리포트 (TotalReportDb)</li>
                    <li>평가 리포트 (ReportDb)</li>
                    <li>세부 업무 (TasksDb)</li>
                    <li>세부직무합의 (SubAgreementDb)</li>
                    <li>직무합의 (AgreementDb)</li>
                    <li>평가 프로세스 상태 (ProcessDb 초기화)</li>
                </ul>
                <p class="text-danger"><strong>정말 초기화하시겠습니까?</strong></p>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-danger" @onclick="OnConfirm">초기화</button>
                <button type="button" class="btn btn-secondary" @onclick="OnCancel">취소</button>
            </div>
        </div>
    </div>
</div>
<div class="modal-backdrop fade show"></div>

@code {
    [Parameter]
    public long Pid { get; set; }

    [Parameter]
    public EventCallback OnConfirm { get; set; }

    [Parameter]
    public EventCallback OnCancel { get; set; }
}
```

**테스트**: 빌드 성공 확인

---

### Step 14: Repository 메서드 추가 (Models 프로젝트)

**필요한 메서드 목록**:

#### 1. Iv_ProcessTRListDBRepository
```csharp
// 인터페이스
Task<v_ProcessTRListDB?> GetByIdAsync(long pid);

// 구현
public async Task<v_ProcessTRListDB?> GetByIdAsync(long pid)
{
    const string sql = "SELECT * FROM v_ProcessTRListDB WHERE Pid = @Pid";
    using var connection = new SqlConnection(_connectionString);
    return await connection.QueryFirstOrDefaultAsync<v_ProcessTRListDB>(sql, new { Pid = pid });
}
```

#### 2. IReportRepository
```csharp
// 인터페이스
Task<int> GetCountByUidAsync(long uid);
Task<bool> DeleteAllByUidAsync(long uid);

// 구현
public async Task<int> GetCountByUidAsync(long uid)
{
    const string sql = "SELECT COUNT(*) FROM ReportDb WHERE Uid = @Uid";
    using var connection = new SqlConnection(_connectionString);
    return await connection.ExecuteScalarAsync<int>(sql, new { Uid = uid });
}

public async Task<bool> DeleteAllByUidAsync(long uid)
{
    const string sql = "DELETE FROM ReportDb WHERE Uid = @Uid";
    using var connection = new SqlConnection(_connectionString);
    var result = await connection.ExecuteAsync(sql, new { Uid = uid });
    return result > 0;
}
```

#### 3. ITasksRepository
```csharp
// 인터페이스
Task<int> GetCountByUserAsync(long uid);
Task<bool> DeleteAllByUserAsync(long uid);

// 구현
public async Task<int> GetCountByUserAsync(long uid)
{
    const string sql = @"
        SELECT COUNT(*)
        FROM TasksDb T
        INNER JOIN ReportDb R ON T.TaksListNumber = R.Task_Number
        WHERE R.Uid = @Uid";
    using var connection = new SqlConnection(_connectionString);
    return await connection.ExecuteScalarAsync<int>(sql, new { Uid = uid });
}

public async Task<bool> DeleteAllByUserAsync(long uid)
{
    const string sql = @"
        DELETE FROM TasksDb
        WHERE TaksListNumber IN (
            SELECT Task_Number FROM ReportDb WHERE Uid = @Uid
        )";
    using var connection = new SqlConnection(_connectionString);
    var result = await connection.ExecuteAsync(sql, new { Uid = uid });
    return result > 0;
}
```

#### 4. IAgreementRepository
```csharp
// 인터페이스
Task<int> GetCountByUidAsync(long uid);
Task<bool> DeleteAllByUidAsync(long uid);

// 구현
public async Task<int> GetCountByUidAsync(long uid)
{
    const string sql = "SELECT COUNT(*) FROM AgreementDb WHERE Uid = @Uid";
    using var connection = new SqlConnection(_connectionString);
    return await connection.ExecuteScalarAsync<int>(sql, new { Uid = uid });
}

public async Task<bool> DeleteAllByUidAsync(long uid)
{
    const string sql = "DELETE FROM AgreementDb WHERE Uid = @Uid";
    using var connection = new SqlConnection(_connectionString);
    var result = await connection.ExecuteAsync(sql, new { Uid = uid });
    return result > 0;
}
```

#### 5. ISubAgreementRepository
```csharp
// 인터페이스
Task<int> GetCountByUidAsync(long uid);
Task<bool> DeleteAllByUidAsync(long uid);

// 구현
public async Task<int> GetCountByUidAsync(long uid)
{
    const string sql = "SELECT COUNT(*) FROM SubAgreementDb WHERE Uid = @Uid";
    using var connection = new SqlConnection(_connectionString);
    return await connection.ExecuteScalarAsync<int>(sql, new { Uid = uid });
}

public async Task<bool> DeleteAllByUidAsync(long uid)
{
    const string sql = "DELETE FROM SubAgreementDb WHERE Uid = @Uid";
    using var connection = new SqlConnection(_connectionString);
    var result = await connection.ExecuteAsync(sql, new { Uid = uid });
    return result > 0;
}
```

#### 6. ITotalReportRepository
```csharp
// 인터페이스
Task<TotalReportDb?> GetByUidAsync(long uid);
Task<bool> UpdateAsync(TotalReportDb model);
Task<bool> DeleteByUidAsync(long uid);

// 구현
public async Task<TotalReportDb?> GetByUidAsync(long uid)
{
    const string sql = "SELECT * FROM TotalReportDb WHERE Uid = @Uid";
    using var connection = new SqlConnection(_connectionString);
    return await connection.QueryFirstOrDefaultAsync<TotalReportDb>(sql, new { Uid = uid });
}

public async Task<bool> UpdateAsync(TotalReportDb model)
{
    const string sql = @"
        UPDATE TotalReportDb SET
            Total_Score = @Total_Score,
            Director_Score = @Director_Score,
            TeamLeader_Score = @TeamLeader_Score
        WHERE TRid = @TRid";
    using var connection = new SqlConnection(_connectionString);
    var result = await connection.ExecuteAsync(sql, model);
    return result > 0;
}

public async Task<bool> DeleteByUidAsync(long uid)
{
    const string sql = "DELETE FROM TotalReportDb WHERE Uid = @Uid";
    using var connection = new SqlConnection(_connectionString);
    var result = await connection.ExecuteAsync(sql, new { Uid = uid });
    return result > 0;
}
```

#### 7. IProcessRepository
```csharp
// 인터페이스
Task<ProcessDb?> GetByUidAsync(long uid);
Task<bool> UpdateAsync(ProcessDb model);

// 구현
public async Task<ProcessDb?> GetByUidAsync(long uid)
{
    const string sql = "SELECT * FROM ProcessDb WHERE Uid = @Uid";
    using var connection = new SqlConnection(_connectionString);
    return await connection.QueryFirstOrDefaultAsync<ProcessDb>(sql, new { Uid = uid });
}

public async Task<bool> UpdateAsync(ProcessDb model)
{
    const string sql = @"
        UPDATE ProcessDb SET
            TeamLeaderId = @TeamLeaderId,
            DirectorId = @DirectorId,
            Is_Request = @Is_Request,
            Is_Agreement = @Is_Agreement,
            Agreement_Comment = @Agreement_Comment,
            Is_SubRequest = @Is_SubRequest,
            Is_SubAgreement = @Is_SubAgreement,
            SubAgreement_Comment = @SubAgreement_Comment,
            Is_User_Submission = @Is_User_Submission,
            Is_Teamleader_Submission = @Is_Teamleader_Submission,
            Is_Director_Submission = @Is_Director_Submission,
            FeedBackStatus = @FeedBackStatus,
            FeedBack_Submission = @FeedBack_Submission
        WHERE Pid = @Pid";
    using var connection = new SqlConnection(_connectionString);
    var result = await connection.ExecuteAsync(sql, model);
    return result > 0;
}
```

**테스트**: 빌드 성공 확인

---

## 5. 테스트 계획

### 5.1. 빌드 테스트

**각 Step마다 빌드 확인**:
```bash
cd MdcHR26Apps.BlazorServer
dotnet build
```

**예상 결과**: 오류 0개, 경고 5개 이하

### 5.2. 기능 테스트

#### Test 1: 전체 평가 목록 조회
1. Admin 로그인
2. `/Admin/TotalReport` 접속
3. 전체 직원 목록 표시 확인
4. 검색 기능 테스트 (이름, ID)
5. 상태 표시 확인

#### Test 2: 최종 결과 상세
1. 목록에서 "상세" 버튼 클릭
2. 사용자 정보 표시 확인
3. 평가 상태 표시 확인
4. 개수 표시 (리포트, 업무, 합의) 확인

#### Test 3: 최종 등급 설정
1. Details 페이지에서 "수정" 버튼 클릭
2. 등급 드롭다운 (S, A, B, C) 표시 확인
3. 등급 변경 후 저장
4. Details 페이지로 리디렉션 확인
5. 변경된 등급 표시 확인

#### Test 4: 평가 초기화
1. Details 페이지에서 "초기화" 버튼 클릭
2. 초기화 확인 모달 표시 확인
3. "초기화" 클릭
4. ProcessDb 상태 초기화 확인
5. TotalReportDb 삭제 확인
6. ReportDb, TasksDb, AgreementDb, SubAgreementDb 삭제 확인

### 5.3. UI/UX 테스트

#### 반응형 테스트
1. 데스크톱 (>768px): 테이블 표시
2. 모바일 (<768px): 카드 표시

#### 네비게이션 테스트
1. NavMenu에서 "전체 평가" 클릭
2. 목록 → 상세 → 수정 → 상세 → 목록 경로 확인

---

## 6. 체크리스트

### Step별 체크리스트

- [ ] **Step 1**: UrlActions 메서드 추가 (4개)
- [ ] **Step 2**: ScoreUtils 클래스 작성 및 DI 등록 (7개 메서드)
- [ ] **Step 3**: TotalScoreRankModel 클래스 작성
- [ ] **Step 4**: AdminReportListView 컴포넌트 작성
- [ ] **Step 5**: ExcelManage 클래스 작성 ⭐
- [ ] **Step 6**: AdminViewExcel 컴포넌트 작성 (NavigationManager 사용) ⭐
- [ ] **Step 7**: AdminTaskViewExcel 컴포넌트 작성 (NavigationManager 사용) ⭐
- [ ] **Step 8**: JavaScript downloadURI 함수 추가 ⭐
- [ ] **Step 9**: Index.razor + .cs 작성 (엑셀 버튼 포함)
- [ ] **Step 10**: Details.razor + .cs 작성
- [ ] **Step 11**: Edit.razor + .cs 작성
- [ ] **Step 12**: ReportInit.razor + .cs 작성
- [ ] **Step 13**: ReportInitModal 컴포넌트 작성
- [ ] **Step 14**: Repository 메서드 추가 (Models 프로젝트)

### 빌드 체크리스트

- [ ] 빌드 성공 (오류 0개)
- [ ] 경고 5개 이하
- [ ] Models 프로젝트 빌드 성공
- [ ] BlazorServer 프로젝트 빌드 성공

### 기능 테스트 체크리스트

- [ ] 전체 평가 목록 조회
- [ ] 검색 기능 (이름, ID)
- [ ] 최종 결과 상세 표시
- [ ] 최종 등급 변경
- [ ] 평가 초기화
- [ ] 메뉴 네비게이션

### UI/UX 체크리스트

- [ ] 데스크톱 테이블 표시
- [ ] 모바일 카드 표시
- [ ] 버튼 동작 확인
- [ ] 모달 표시 및 닫기

---

## 7. 주의사항

### 7.1. DB 구조 차이 인지

**2025년 → 2026년**:
- `UserId` (VARCHAR) → `Uid` (BIGINT)
- `TeamLeader_Id` → `TeamLeaderId`
- `Director_Id` → `DirectorId`
- 하위 합의 필드 추가: `Is_SubRequest`, `Is_SubAgreement`, `SubAgreement_Comment`

### 7.2. .NET 10 형식 적용

**Primary Constructor 사용**:
```csharp
public partial class Index(
    Iv_ProcessTRListDBRepository processTRRepository,
    LoginStatusService loginStatusService,
    UrlActions urlActions)
{
    // 자동으로 필드로 사용 가능
}
```

**InteractiveServer 렌더 모드**:
```razor
@rendermode InteractiveServer
```

### 7.3. 참조 무결성

**초기화 순서 (외래키 역순)**:
1. TotalReportDb 삭제
2. ReportDb 삭제
3. TasksDb 삭제 (ReportDb 참조)
4. SubAgreementDb 삭제
5. AgreementDb 삭제
6. ProcessDb 상태 초기화 (삭제하지 않음)

### 7.4. 2025년 코드 복사 원칙

**임의 수정 금지**:
- 2025년 코드를 먼저 복사
- DB 구조 변경 사항만 최소 반영
- 단계별 빌드 테스트

### 7.5. NavigationManager.BaseUri 사용

**Excel 다운로드 URL 생성** (Step 6, 7):
- 하드코딩된 URL 대신 `NavigationManager.BaseUri` 사용
- 환경 자동 감지 (개발: `https://localhost:5001/`, 배포: `https://hr2026.yourdomain.kr/`)
- 포트 변경에 자동 대응
- `appsettings.json` 설정 불필요
- Primary Constructor로 `NavigationManager` 주입

**코드 예시**:
```csharp
public partial class AdminViewExcel(
    NavigationManager navigationManager)  // 주입
{
    private async Task ClickExportXLS()
    {
        string fileUrl = $"{navigationManager.BaseUri}files/tasks/{fileName}";
        // 개발: https://localhost:5001/files/tasks/filename.xlsx
        // 배포: https://hr2026.yourdomain.kr/files/tasks/filename.xlsx
    }
}
```

### 7.6. Admin 페이지 접근 방식

**TotalReport/Admin 페이지 접근**:
- NavMenu 메뉴 추가 방식 **사용 안 함** ❌
- **Admin/Index.razor에 바로가기 버튼 추가** ✅ (2025년 방식)

**Admin/Index.razor 수정 필요**:
```razor
<div class="input-group mb-1">
    <button class="btn btn-primary" @onclick="UserManagePage">사용자관리페이지</button>
    <button class="btn btn-secondary" @onclick="EUserManagePage">E사용자관리페이지</button>
    <button class="btn btn-success" @onclick="MoveTotalReportAdminMainPage">전체평가관리</button>
    <button class="btn btn-warning" @onclick="MoveDeptManagePage">부서관리페이지</button>
    <button class="btn btn-info" @onclick="MoveDevelopPage">개발자페이지 이동</button>
</div>
```

**Code-behind 메서드 추가**:
```csharp
private void MoveTotalReportAdminMainPage()
{
    urlActions.MoveTotalReportAdminIndexPage();
}
```

**장점**:
- 관리자 전용 기능을 Admin 페이지에서 집중 관리
- NavMenu가 복잡해지는 것 방지
- 2025년 UI/UX 일관성 유지

---

## 8. 완료 기준

### Phase 3-3 완성 (100%)

- ✅ Admin 기본 페이지 (UserManage, SettingManage, EUsersManage)
- ✅ Users CRUD (Create, Edit, Delete, Details)
- ✅ Settings CRUD (Depts, Ranks)
- ✅ EvaluationUsers (Details, Edit)
- ✅ 공용 컴포넌트 3개
- ✅ v_EvaluationUsersList 뷰
- ✅ **TotalReport/Admin 4개 페이지** ← 이번 작업

### 최종 확인 사항

1. 빌드 성공 (오류 0개)
2. 전체 평가 목록 조회 가능
3. 최종 등급 설정 가능
4. 평가 초기화 가능
5. 메뉴 네비게이션 정상 작동

---

## 9. 다음 단계 (Phase 3-4)

Phase 3-3 완성 후:

### Phase 3-4: 평가 프로세스 페이지
1. 직무평가 협의 (Agreement)
2. 세부직무평가 (SubAgreement)
3. 본인평가 (1st_HR_Report)
4. 부서장평가 (2nd_HR_Report)
5. 임원평가 (3rd_HR_Report)

---

**작성자**: Claude AI
**검토자**: 개발자
**승인 일자**: 추후 기재