# 작업지시서: Phase 3-3 TotalReport/Admin 페이지 완성 (Step 4-14)

**날짜**: 2026-01-29
**작업 유형**: 기능 추가
**관련 이슈**:
- [#009: Phase 3 WebApp 개발](../issues/009_phase3_webapp_development.md)
- [#011: Phase 3-3 관리자 페이지 재작업](../issues/011_phase3_3_admin_pages_build_errors.md)
**선행 작업지시서**:
- `20260129_03_phase3_3_totalreport_admin_complete.md` (Step 1-3 완료)
- `20260129_04_fix_view_model_sync.md` (View/Model 동기화 완료) ✅

---

## 1. 작업 개요

### 1.1. 배경

**완료된 작업**:
- ✅ Step 1-3: UrlActions, ScoreUtils, TotalScoreRankModel 완료
- ✅ 20260129_04: View/Model 동기화 완료
  - v_ProcessTRListDB: 38개 필드 (평가 점수, 상태 필드 포함)
  - v_TotalReportListDB: 25개 필드 (모든 평가 점수 포함)

**이번 작업**:
- Step 4-14: TotalReport/Admin 페이지 및 컴포넌트 구현
- 엑셀 기능, Repository 메서드 추가

### 1.2. 작업 범위

**11개 파일 생성**:
1. AdminReportListView.razor (컴포넌트)
2. ExcelManage.cs (유틸리티)
3. AdminViewExcel.razor + .cs (엑셀 다운로드)
4. AdminTaskViewExcel.razor + .cs (업무 엑셀)
5. site.js 수정 (downloadURI 함수)
6. Index.razor + .cs (전체 평가 목록)
7. Details.razor + .cs (상세 보기)
8. Edit.razor + .cs (등급 수정)
9. ReportInit.razor + .cs (초기화)
10. ReportInitModal.razor (초기화 모달)
11. Repository 메서드 추가

---

## 2. 작업 단계

### ⚠️ 중요 사항

**View Model 사용**:
- v_ProcessTRListDB는 38개 필드를 가지고 있음 (20260129_04에서 수정)
- AdminReportListView에서 사용하는 모든 필드가 존재함:
  - Is_Request, Is_Agreement, Is_SubRequest, Is_SubAgreement
  - Is_User_Submission, Is_Teamleader_Submission, Is_Director_Submission
  - FeedBackStatus, FeedBack_Submission
  - Total_Score, UserId, UserName

**엑셀 파일 URL**:
- NavigationManager.BaseUri 사용 (환경별 자동 감지)
- 하드코딩 금지

---

## 3. Step 4-14 상세

### Step 4: AdminReportListView 컴포넌트 작성

**파일**: `Components/Pages/Components/Table/AdminReportListView.razor` (신규)

**목적**: 전체 평가 목록 테이블 컴포넌트

**2025년 참고**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Components\ViewPage\AdminReportListView.razor`

**중요**: 2025년 코드를 그대로 복사하고 네임스페이스만 변경

**AdminReportListView.razor 코드**:
```razor
@using MdcHR26Apps.Models.Views.v_ProcessTRListDB
@using MdcHR26Apps.BlazorServer.Utils
@inject UrlActions urlActions
@inject ScoreUtils scoreUtils

<table class="table table-hover table-responsive-md">
    <thead>
        <tr>
            <th class="table-header">
                <div class="row align-items-center">
                    <div class="col-12 col-md-2">#</div>
                    <div class="col-12 col-md-5">이름</div>
                    <div class="col-12 col-md-5">평가자<br />점수</div>
                </div>
            </th>
            <th class="table-header">
                <div class="row align-items-center">
                    <div class="col-12 col-md-6">부서장 이름</div>
                    <div class="col-12 col-md-6">부서장<br />점수</div>
                </div>
            </th>
            <th class="table-header">
                <div class="row align-items-center">
                    <div class="col-12 col-md-6">임원 이름</div>
                    <div class="col-12 col-md-6">최종<br />점수</div>
                </div>
            </th>
            <th class="table-header">
                <div class="row align-items-center">
                    <div class="col-12 col-md-6">최종 등급</div>
                    <div class="col-12 col-md-6">비고</div>
                </div>
            </th>
        </tr>
    </thead>
    <tbody>
        @foreach (var item in processTRLists)
        {
            <tr>
                <td class="table-cell">
                    <div class="row">
                        <div class="col-12 col-md-2">@sortNoAdd3(sortNo)</div>
                        <div class="col-12 col-md-5">@item.UserName</div>
                        <div class="col-12 col-md-5" style=@text_style_1>@IsReportSubmissionStatus(item.Is_User_Submission, item.User_Evaluation_1, item.User_Evaluation_2, item.User_Evaluation_3)</div>
                    </div>
                </td>
                <td class="table-cell">
                    <div class="row">
                        <div class="col-12 col-md-6">@item.TeamLeader_Name</div>
                        <div class="col-12 col-md-6" style=@text_style_2>@IsTeamleaderSubmissionStatus(item.Is_Teamleader_Submission, item.TeamLeader_Score)</div>
                    </div>
                </td>
                <td class="table-cell">
                    <div class="row">
                        <div class="col-12 col-md-6">@item.Director_Name</div>
                        <div class="col-12 col-md-6" style=@text_style_3>@GetDirectorScore(item.Is_Director_Submission, item.Director_Score)</div>
                    </div>
                </td>
                <td class="table-cell">
                    <div class="row align-items-center">
                        <div class="col-12 col-md-6">@scoreUtils.GetTotalScore(@item.Total_Score)</div>
                        <div class="col-12 col-md-6">
                            <button id="DetailButton" class="btn btn-info" @onclick="(() => MoveAdminDetailsPage(item.Pid))">상세</button>
                        </div>
                    </div>
                </td>
            </tr>
        }
    </tbody>
</table>

@code {
    [Parameter]
    public List<v_ProcessTRListDB> processTRLists { get; set; } = new List<v_ProcessTRListDB>();

    // 평가순서관리
    [Inject]
    public IProcessDbRepository processDbRepository { get; set; } = null!;
    public ProcessDb processDb { get; set; } = new ProcessDb();
    public List<ProcessDb> processDbList { get; set; } = new List<ProcessDb>();

    // 페이지 이동
    [Inject]
    public UrlActions urlActions { get; set; } = null!;

    // 테이블 CSS Style
    public string table_style_1 = "white-space: nowrap; overflow: hidden; text-overflow: ellipsis; text-align: center; vertical-align: middle;";

    // Text CSS Style
    public string text_style_1 = "white-space: nowrap; overflow: hidden; text-overflow: ellipsis; text-align: center; vertical-align: middle; color: black; font-style: italic;";
    public string text_style_2 = "white-space: nowrap; overflow: hidden; text-overflow: ellipsis; text-align: center; vertical-align: middle; color: blue; font-style: italic;";
    public string text_style_3 = "white-space: nowrap; overflow: hidden; text-overflow: ellipsis; text-align: center; vertical-align: middle; color: red; font-weight: bold;";

    // 기타
    public int sortNo = 1;

    public string IsNotEvaluate1 { get; set; } = "평가자 제출(X)";
    public string IsNotEvaluate2 { get; set; } = "부서장 평가중";

    // 공용함수 호출
    public ScoreUtils scoreUtils = new ScoreUtils();

    private int sortNoAdd3(int sort)
    {
        if (processTRLists.Count == sortNo)
        {
            sortNo = 1;
            return sort;
        }
        sortNo = sort + 1;
        return sort;
    }

    #region + 평가제출여부 Is_User_Submission
    private string IsReportSubmissionStatus(bool status, double score_1, double score_2, double score_3)
    {

        if (status)
        {
            double score = score_1 + score_2 + score_3;
            score = scoreUtils.TotalScroreTo100thpercentile(score);
            return score.ToString();
        }
        else
        {
            return "제출(X)";
        }
    }
    #endregion

    #region + 팀장평가제출여부 Is_Teamleader_Submission
    private string IsTeamleaderSubmissionStatus(bool status, double teamleader_score)
    {
        if (status)
        {
            return teamleader_score.ToString();
        }
        else
        {
            return "평가(X)";
        }
    }
    #endregion

    #region + 최종점수표기
    private string GetDirectorScore(bool status, double score)
    {
        return status ? score.ToString() : "평가(X)";
    }
    #endregion

    private void MoveAdminDetailsPage(Int64 Pid)
    {
        urlActions.MoveAdminDetailsPage(Pid);
    }

    private void DirectorDetailsAction(Int64 Pid)
    {
        urlActions.Move3rdDeteilsPage(Pid);
    }

    private void CompleteDetailsAction(Int64 Pid)
    {
        urlActions.MoveComplete3rdDetailsPage(Pid);
    }


    #region + 평가취소 : CancelAction
    private async Task CancelAction(Int64 Pid)
    {
        processDb = await processDbRepository.GetByIdAsync(Pid);

        if (processDb.Pid != 0)
        {
            processDb.Is_Director_Submission = false;
            if (await processDbRepository.UpdateAsync(processDb))
            {
                urlActions.Move3rdMainPage();
            }
            else
            {
                processDb.Is_Director_Submission = true;
            }
        }
    }
    #endregion
}
```

**네임스페이스 변경 사항** (2025년 → 2026년):
```csharp
// 2025년
@using MdcHR25Apps.Models.ProcessView
@using MdcHR25Apps.BlazorApp.Utils

// 2026년
@using MdcHR26Apps.Models.Views.v_ProcessTRListDB
@using MdcHR26Apps.BlazorServer.Utils
```

**핵심 메서드 설명**:
1. **IsReportSubmissionStatus(bool, double, double, double)**: 평가자 점수 합산 후 백분위 변환
2. **IsTeamleaderSubmissionStatus(bool, double)**: 팀장 점수 표시 (제출 여부)
3. **GetDirectorScore(bool, double)**: 임원 점수 표시 (평가 여부)
4. **sortNoAdd3(int)**: 순번 관리

**v_ProcessTRListDB 필드 사용**:
- Is_User_Submission, User_Evaluation_1/2/3
- Is_Teamleader_Submission, TeamLeader_Score
- Is_Director_Submission, Director_Score
- Total_Score, UserName, TeamLeader_Name, Director_Name, Pid

---

### Step 5: ExcelManage 클래스 작성

**파일**: `Utils/ExcelManage.cs` (신규)

**목적**: 엑셀 생성 및 삭제 유틸리티

**참고**: 20260129_03의 Step 5 참조 (642-867줄)

**주의사항**:
- ClosedXML 라이브러리 사용
- wwwroot/files/tasks/ 폴더에 엑셀 저장
- 파일명 형식: yy_MM_dd_HH_mm_ss.xlsx

---

### Step 6: AdminViewExcel 컴포넌트 작성

**파일**: `Components/Pages/Components/Common/AdminViewExcel.razor` + `.cs` (신규)

**목적**: 평가 리스트 엑셀 다운로드

**참고**: 20260129_03의 Step 6 참조 (868-958줄)

**중요 변경**:
```csharp
// ✅ NavigationManager.BaseUri 사용
string fileUrl = $"{navigationManager.BaseUri}files/tasks/{fileName}";

// ❌ 하드코딩 금지
// string fileUrl = $"https://localhost:5001/files/tasks/{fileName}";
```

---

### Step 7: AdminTaskViewExcel 컴포넌트 작성

**파일**: `Components/Pages/Components/Common/AdminTaskViewExcel.razor` + `.cs` (신규)

**목적**: 업무 리스트 엑셀 다운로드

**참고**: 20260129_03의 Step 7 참조 (959-1045줄)

**중요 변경**:
```csharp
// ✅ NavigationManager.BaseUri 사용
string fileUrl = $"{navigationManager.BaseUri}files/tasks/{fileName}";
```

---

### Step 8: JavaScript downloadURI 함수 추가

**파일**: `wwwroot/js/site.js` (수정)

**목적**: 엑셀 파일 다운로드 함수

**참고**: 20260129_03의 Step 8 참조 (1046-1072줄)

**코드**:
```javascript
window.downloadURI = function (uri, name) {
    var link = document.createElement("a");
    link.download = name;
    link.href = uri;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    delete link;
}
```

---

### Step 9: Index.razor 작성

**파일**: `Components/Pages/Admin/TotalReport/Index.razor` + `.cs` (신규)

**목적**: 전체 평가 목록 페이지

**참고**: 20260129_03의 Step 9 참조 (1073-1198줄)

**주의사항**:
- @page "/Admin/TotalReport"
- v_ProcessTRListRepository 사용
- AdminReportListView 컴포넌트 사용
- AdminViewExcel, AdminTaskViewExcel 컴포넌트 사용

---

### Step 10: Details.razor 작성

**파일**: `Components/Pages/Admin/TotalReport/Details.razor` + `.cs` (신규)

**목적**: 최종 결과 상세 페이지

**참고**: 20260129_03의 Step 10 참조 (1199-1376줄)

**주의사항**:
- @page "/Admin/TotalReport/Details/{pid:long}"
- v_ProcessTRListDB 사용 (38개 필드)
- 모든 평가 단계별 점수 표시

---

### Step 11: Edit.razor 작성

**파일**: `Components/Pages/Admin/TotalReport/Edit.razor` + `.cs` (신규)

**목적**: 최종 등급 설정 페이지

**참고**: 20260129_03의 Step 11 참조 (1377-1550줄)

**주의사항**:
- @page "/Admin/TotalReport/Edit/{pid:long}"
- TotalReportDb Repository 사용
- 등급 선택 (S, A+, A, B+, B, C) 및 저장

---

### Step 12: ReportInit.razor 작성

**파일**: `Components/Pages/Admin/TotalReport/ReportInit.razor` + `.cs` (신규)

**목적**: 평가 초기화 페이지

**참고**: 20260129_03의 Step 12 참조 (1551-1791줄)

**주의사항**:
- @page "/Admin/TotalReport/ReportInit/{pid:long}"
- ReportInitModal 사용
- 초기화 확인 후 TotalReportDb 삭제

---

### Step 13: ReportInitModal 컴포넌트 작성

**파일**: `Components/Pages/Components/Modal/ReportInitModal.razor` (신규)

**목적**: 평가 초기화 확인 모달

**참고**: 20260129_03의 Step 13 참조 (1792-1842줄)

**주의사항**:
- Bootstrap Modal 사용
- EventCallback<bool> OnConfirm 사용

---

### Step 14: Repository 메서드 추가

**파일**: `MdcHR26Apps.Models/Views/v_ProcessTRListDB/v_ProcessTRListRepository.cs` 등 (수정)

**목적**: TotalReport 페이지에서 사용할 Repository 메서드

**참고**: 20260129_03의 Step 14 참조 (1843-2095줄)

**추가 메서드**:
```csharp
// v_ProcessTRListRepository
Task<List<v_ProcessTRListDB>> GetAllAsync();
Task<v_ProcessTRListDB?> GetByPidAsync(long pid);

// TotalReportDbRepository
Task<TotalReportDb?> GetByPidAsync(long pid);
Task<int> UpdateAsync(TotalReportDb entity);
Task<int> DeleteAsync(long trid);
```

---

## 4. 빌드 테스트

각 Step 완료 후:
```bash
dotnet build
```

**예상 결과**: 오류 0개

---

## 5. 완료 조건

- [ ] Step 4: AdminReportListView 컴포넌트 (1개 파일)
- [ ] Step 5: ExcelManage 유틸리티 (1개 파일)
- [ ] Step 6: AdminViewExcel 컴포넌트 (2개 파일)
- [ ] Step 7: AdminTaskViewExcel 컴포넌트 (2개 파일)
- [ ] Step 8: site.js 수정 (downloadURI 함수)
- [ ] Step 9: Index.razor (2개 파일)
- [ ] Step 10: Details.razor (2개 파일)
- [ ] Step 11: Edit.razor (2개 파일)
- [ ] Step 12: ReportInit.razor (2개 파일)
- [ ] Step 13: ReportInitModal (1개 파일)
- [ ] Step 14: Repository 메서드 추가 (3개 파일 수정)
- [ ] 전체 빌드 테스트 성공

---

## 6. 참고 사항

**상세 코드**:
- 20260129_03_phase3_3_totalreport_admin_complete.md의 Step 4-14 참조
- 각 Step의 전체 코드가 작업지시서에 포함되어 있음

**View Model 변경사항 (20260129_04)**:
- v_ProcessTRListDB: 15개 → 38개 필드
- v_TotalReportListDB: 17개 → 25개 필드
- 모든 평가 점수 필드 포함

**테스트 항목**:
1. 전체 평가 목록 조회
2. 상세 보기 (모든 평가 점수 표시)
3. 등급 수정
4. 평가 초기화
5. 엑셀 다운로드 (평가 리스트, 업무 리스트)

---

**작업 시작일**: 2026-01-29
**예상 소요 시간**: 4-5시간
**우선순위**: 높음 (Phase 3-3 완료)
**전제 조건**: 20260129_04 완료 ✅