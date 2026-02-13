# 작업지시서: Phase 3-4 Report 컴포넌트 구현 (15개)

**날짜**: 2026-02-04
**작업 유형**: 신규 기능 추가
**관련 이슈**: [#009: Phase 3 Blazor Server WebApp 개발](../issues/009_phase3_webapp_development.md)
**참조 가이드**: [20260203_13_REWRITE_GUIDE.md](20260203_13_REWRITE_GUIDE.md)
**선행 작업**: [20260203_14_components_subagreement_v2.md](20260203_14_components_subagreement_v2.md)

---

## 1. 작업 개요

**목적**:
- 평가 보고서(Report) 관련 Blazor 컴포넌트 15개 구현
- 3단계 평가 프로세스 (본인 → 팀장 → 임원) UI 구현
- 2025년 프로젝트 구조 참조, 2026년 Entity/Repository 구조 적용

**배경**:
- SubAgreement 컴포넌트 작업지시서 완료 (14번)
- ReportDb는 SubAgreementDb 기반으로 생성된 평가 데이터
- 본인평가(User), 부서장평가(TeamLeader), 임원평가(Director) 3단계 처리

---

## 2. Entity 및 Repository 구조

### 2.1. ReportDb Entity

**경로**: `MdcHR26Apps.Models\EvaluationReport\ReportDb.cs`

**필드 구조** (총 21개 필드):
```csharp
public class ReportDb
{
    // 기본 정보
    public Int64 Rid { get; set; }                      // PK
    public Int64 Uid { get; set; }                      // FK (UserDb)
    public int Report_Item_Number { get; set; }
    public string Report_Item_Name_1 { get; set; }
    public string Report_Item_Name_2 { get; set; }
    public int Report_Item_Proportion { get; set; }
    public string Report_SubItem_Name { get; set; }
    public int Report_SubItem_Proportion { get; set; }
    public Int64 Task_Number { get; set; }

    // 본인 평가 (User)
    public double User_Evaluation_1 { get; set; }       // 일정준수
    public double User_Evaluation_2 { get; set; }       // 업무 수행의 양
    public double User_Evaluation_3 { get; set; }       // 결과물
    public string? User_Evaluation_4 { get; set; }      // 코멘트

    // 팀장 평가 (TeamLeader)
    public double TeamLeader_Evaluation_1 { get; set; }
    public double TeamLeader_Evaluation_2 { get; set; }
    public double TeamLeader_Evaluation_3 { get; set; }
    public string? TeamLeader_Evaluation_4 { get; set; }

    // 임원 평가 (Director)
    public double Director_Evaluation_1 { get; set; }
    public double Director_Evaluation_2 { get; set; }
    public double Director_Evaluation_3 { get; set; }
    public string? Director_Evaluation_4 { get; set; }

    public double Total_Score { get; set; }             // 종합 점수
}
```

### 2.2. IReportRepository 메서드

**경로**: `MdcHR26Apps.Models\EvaluationReport\IReportRepository.cs`

**⚠️ 주의**: 현재 IReportRepository에 `GetCountByUidAsync()`, `DeleteAllByUidAsync()` 메서드가 있으나, **25년 메서드 패턴을 따라야 합니다**.

**사용 가능한 메서드** (25년 기준):
```csharp
Task<Int64> AddAsync(ReportDb model);
Task<IEnumerable<ReportDb>> GetByAllAsync();
Task<ReportDb?> GetByIdAsync(Int64 id);
Task<int> UpdateAsync(ReportDb model);
Task<int> DeleteAsync(Int64 id);
Task<IEnumerable<ReportDb>> GetByUidAllAsync(Int64 uid);
```

**❌ 사용 금지 메서드**:
- `GetCountByUidAsync()` - 대신 `GetByUidAllAsync().Count()` 사용
- `DeleteAllByUidAsync()` - 대신 `foreach` + `DeleteAsync()` 사용

---

## 3. 구현할 컴포넌트 (15개)

### 카테고리별 분류

#### A. Table 컴포넌트 (4개)
1. ReportListTable - 평가 목록 테이블
2. TeamLeaderReportDetailsTable - 팀장 평가 상세
3. DirectorReportDetailsTable - 임원 평가 상세
4. TeamLeader_TotalReportList - 팀장용 전체 리포트 목록

#### B. Modal 컴포넌트 (3개)
5. ReportDeleteModal - 평가 삭제 모달
6. ReportInitModal - 평가 초기화 모달
7. ReportSubmitModal - 평가 제출 모달

#### C. ViewPage 컴포넌트 (8개)
8. AdminReportListView - 관리자 평가 목록
9. Complete_TotalReportListView - 완료된 평가 목록
10. DirectorReportListView - 임원 평가 목록
11. ReportTaskListView - 평가 업무 목록
12. TeamLeaderReportListView - 팀장 평가 목록
13. User_TotalReportListView - 사용자 전체 리포트
14. FeedBack_TotalReportListView - 피드백 리포트
15. TeamLeader_TotalReportListView - 팀장 전체 리포트

---

## 4. 컴포넌트 상세 구현

### 4.1. ReportListTable.razor

**경로**: `MdcHR26Apps.BlazorServer\Components\Report\ReportListTable.razor`

**목적**: 평가 목록 테이블 (체크박스, 평가 상태 표시)

**주요 기능**:
- 평가 완료 여부 체크박스 (User_Evaluation_4 기준)
- 평가지표, 평가직무, 세부직무 표시
- "평가하기" / "수정하기" 버튼 (평가 여부에 따라)

**2025년 참조**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Components\ReportListTable.razor`

**구현 내용**:
```razor
<table class="table table-hover table-responsive-md">
    <thead>
        <tr>
            <th>#</th>
            <th>순번</th>
            <th>
                <div class="row">
                    <div class="col-12 col-md-5">평가지표</div>
                    <div class="col-12 col-md-5">평가직무</div>
                    <div class="col-12 col-md-2">직무비중</div>
                </div>
            </th>
            <th>
                <div class="row">
                    <div class="col-12 col-md-6">세부직무</div>
                    <div class="col-12 col-md-6">세부직무비중</div>
                </div>
            </th>
            <th>비고</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var item in reports)
        {
            <tr>
                <td>
                    @if (string.IsNullOrEmpty(item.User_Evaluation_4))
                    {
                        <input class="form-check-input" type="checkbox" disabled>
                    }
                    else
                    {
                        <input class="form-check-input" type="checkbox" checked disabled>
                    }
                </td>
                <td>@sortNo</td>
                <td>
                    <div class="row">
                        <div class="col-12 col-md-5">@item.Report_Item_Name_1</div>
                        <div class="col-12 col-md-5">@item.Report_Item_Name_2</div>
                        <div class="col-12 col-md-2">@item.Report_Item_Proportion %</div>
                    </div>
                </td>
                <td>
                    <div class="row">
                        <div class="col-12 col-md-6">@item.Report_SubItem_Name</div>
                        <div class="col-12 col-md-6">@item.Report_SubItem_Proportion %</div>
                    </div>
                </td>
                <td>
                    @if (string.IsNullOrEmpty(item.User_Evaluation_4))
                    {
                        <button class="btn btn-info" @onclick="@(() => OnEdit(item.Rid))">평가하기</button>
                    }
                    else
                    {
                        <button class="btn btn-info" @onclick="@(() => OnEdit(item.Rid))">수정하기</button>
                    }
                </td>
            </tr>
            sortNo++;
        }
    </tbody>
</table>

@code {
    [Parameter] public List<ReportDb> reports { get; set; } = new();
    [Parameter] public EventCallback<long> OnEdit { get; set; }

    private int sortNo = 1;
}
```

---

### 4.2. TeamLeaderReportDetailsTable.razor

**경로**: `MdcHR26Apps.BlazorServer\Components\Report\TeamLeaderReportDetailsTable.razor`

**목적**: 팀장 평가 상세 테이블

**주요 기능**:
- 평가지표, 평가직무, 세부직무 표시
- 본인 평가 결과 표시 (User_Evaluation_1~4)
- 팀장 평가 입력 필드 (TeamLeader_Evaluation_1~4)

**구현 내용**:
```razor
@if (report != null)
{
    <h4>평가 항목</h4>
    <table class="table">
        <tbody>
            <tr>
                <th>평가지표</th>
                <td>@report.Report_Item_Name_1</td>
            </tr>
            <tr>
                <th>평가직무</th>
                <td>@report.Report_Item_Name_2</td>
            </tr>
            <tr>
                <th>세부직무</th>
                <td>@report.Report_SubItem_Name</td>
            </tr>
        </tbody>
    </table>

    <h5 class="mt-4">본인 평가 결과</h5>
    <table class="table">
        <tbody>
            <tr>
                <th>일정준수</th>
                <td>@report.User_Evaluation_1</td>
            </tr>
            <tr>
                <th>업무 수행의 양</th>
                <td>@report.User_Evaluation_2</td>
            </tr>
            <tr>
                <th>결과물</th>
                <td>@report.User_Evaluation_3</td>
            </tr>
            <tr>
                <th>코멘트</th>
                <td>@report.User_Evaluation_4</td>
            </tr>
        </tbody>
    </table>

    <h5 class="mt-4">팀장 평가 입력</h5>
    <div class="mb-3">
        <label class="form-label">일정준수</label>
        <input type="number" class="form-control" @bind="report.TeamLeader_Evaluation_1" step="0.1" />
    </div>
    <div class="mb-3">
        <label class="form-label">업무 수행의 양</label>
        <input type="number" class="form-control" @bind="report.TeamLeader_Evaluation_2" step="0.1" />
    </div>
    <div class="mb-3">
        <label class="form-label">결과물</label>
        <input type="number" class="form-control" @bind="report.TeamLeader_Evaluation_3" step="0.1" />
    </div>
    <div class="mb-3">
        <label class="form-label">코멘트</label>
        <textarea class="form-control" @bind="report.TeamLeader_Evaluation_4" rows="3"></textarea>
    </div>

    <div class="mt-3">
        <button class="btn btn-primary" @onclick="OnSave">저장</button>
        <button class="btn btn-secondary" @onclick="OnCancel">취소</button>
    </div>
}

@code {
    [Parameter] public ReportDb? report { get; set; }
    [Parameter] public EventCallback OnSave { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }
}
```

---

### 4.3. DirectorReportDetailsTable.razor

**경로**: `MdcHR26Apps.BlazorServer\Components\Report\DirectorReportDetailsTable.razor`

**목적**: 임원 평가 상세 테이블

**주요 기능**:
- 평가지표, 평가직무, 세부직무 표시
- 본인 평가 + 팀장 평가 결과 표시
- 임원 평가 입력 필드 (Director_Evaluation_1~4)

**구현 내용**:
```razor
@if (report != null)
{
    <h4>평가 항목</h4>
    <table class="table">
        <tbody>
            <tr>
                <th>평가지표</th>
                <td>@report.Report_Item_Name_1</td>
            </tr>
            <tr>
                <th>평가직무</th>
                <td>@report.Report_Item_Name_2</td>
            </tr>
            <tr>
                <th>세부직무</th>
                <td>@report.Report_SubItem_Name</td>
            </tr>
        </tbody>
    </table>

    <h5 class="mt-4">본인 평가 결과</h5>
    <table class="table table-sm">
        <tbody>
            <tr>
                <th>일정준수</th>
                <td>@report.User_Evaluation_1</td>
            </tr>
            <tr>
                <th>업무 수행의 양</th>
                <td>@report.User_Evaluation_2</td>
            </tr>
            <tr>
                <th>결과물</th>
                <td>@report.User_Evaluation_3</td>
            </tr>
        </tbody>
    </table>

    <h5 class="mt-4">팀장 평가 결과</h5>
    <table class="table table-sm">
        <tbody>
            <tr>
                <th>일정준수</th>
                <td>@report.TeamLeader_Evaluation_1</td>
            </tr>
            <tr>
                <th>업무 수행의 양</th>
                <td>@report.TeamLeader_Evaluation_2</td>
            </tr>
            <tr>
                <th>결과물</th>
                <td>@report.TeamLeader_Evaluation_3</td>
            </tr>
        </tbody>
    </table>

    <h5 class="mt-4">임원 평가 입력</h5>
    <div class="mb-3">
        <label class="form-label">일정준수</label>
        <input type="number" class="form-control" @bind="report.Director_Evaluation_1" step="0.1" />
    </div>
    <div class="mb-3">
        <label class="form-label">업무 수행의 양</label>
        <input type="number" class="form-control" @bind="report.Director_Evaluation_2" step="0.1" />
    </div>
    <div class="mb-3">
        <label class="form-label">결과물</label>
        <input type="number" class="form-control" @bind="report.Director_Evaluation_3" step="0.1" />
    </div>
    <div class="mb-3">
        <label class="form-label">코멘트</label>
        <textarea class="form-control" @bind="report.Director_Evaluation_4" rows="3"></textarea>
    </div>

    <div class="mt-3">
        <button class="btn btn-primary" @onclick="OnSave">저장</button>
        <button class="btn btn-secondary" @onclick="OnCancel">취소</button>
    </div>
}

@code {
    [Parameter] public ReportDb? report { get; set; }
    [Parameter] public EventCallback OnSave { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }
}
```

---

### 4.4. TeamLeader_TotalReportList.razor

**경로**: `MdcHR26Apps.BlazorServer\Components\Report\TeamLeader_TotalReportList.razor`

**목적**: 팀장용 전체 리포트 목록 (간소화 테이블)

**구현 내용**:
```razor
<table class="table table-sm">
    <thead>
        <tr>
            <th>번호</th>
            <th>평가지표</th>
            <th>평가직무</th>
            <th>세부직무</th>
            <th>본인평가</th>
            <th>팀장평가</th>
            <th>상태</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var item in reports)
        {
            <tr>
                <td>@item.Report_Item_Number</td>
                <td>@item.Report_Item_Name_1</td>
                <td>@item.Report_Item_Name_2</td>
                <td>@item.Report_SubItem_Name</td>
                <td>@GetAverageScore(item.User_Evaluation_1, item.User_Evaluation_2, item.User_Evaluation_3)</td>
                <td>@GetAverageScore(item.TeamLeader_Evaluation_1, item.TeamLeader_Evaluation_2, item.TeamLeader_Evaluation_3)</td>
                <td>
                    @if (string.IsNullOrEmpty(item.TeamLeader_Evaluation_4))
                    {
                        <span class="badge bg-warning">미평가</span>
                    }
                    else
                    {
                        <span class="badge bg-success">완료</span>
                    }
                </td>
            </tr>
        }
    </tbody>
</table>

@code {
    [Parameter] public List<ReportDb> reports { get; set; } = new();

    private double GetAverageScore(double eval1, double eval2, double eval3)
    {
        return Math.Round((eval1 + eval2 + eval3) / 3, 2);
    }
}
```

---

### 4.5-4.7. Modal 컴포넌트 (3개)

#### ReportDeleteModal.razor

**경로**: `MdcHR26Apps.BlazorServer\Components\Report\Modal\ReportDeleteModal.razor`

**목적**: 평가 삭제 확인 모달

**구현 내용**:
```razor
@inject IReportRepository reportRepository

@if (model != null)
{
    <div class="modal @ModalClass" tabindex="-1" style="display:@ModalDisplay;">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">평가 삭제</h5>
                    <button type="button" class="btn-close" @onclick="Close"></button>
                </div>
                <div class="modal-body">
                    <p>이 평가를 삭제하시겠습니까?</p>
                    <p>세부직무: @model.Report_SubItem_Name</p>

                    @if (!string.IsNullOrEmpty(resultText))
                    {
                        <div class="alert alert-info">@resultText</div>
                    }
                </div>
                <div class="modal-footer">
                    <button class="btn btn-danger" @onclick="DeleteReport">예(Yes)</button>
                    <button class="btn btn-secondary" @onclick="Close">아니요(No)</button>
                </div>
            </div>
        </div>
    </div>

    @if (ShowBackdrop)
    {
        <div class="modal-backdrop fade show"></div>
    }
}

@code {
    [Parameter] public ReportDb? model { get; set; }
    [Parameter] public EventCallback OnDeleteSuccess { get; set; }
    [Parameter] public EventCallback OnClose { get; set; }

    private string ModalDisplay = "none;";
    private string ModalClass = "";
    private bool ShowBackdrop = false;
    private string resultText = string.Empty;

    public void Open()
    {
        ModalDisplay = "block;";
        ModalClass = "show";
        ShowBackdrop = true;
        StateHasChanged();
    }

    private void Close()
    {
        ModalDisplay = "none";
        ModalClass = "";
        ShowBackdrop = false;
        resultText = string.Empty;
        OnClose.InvokeAsync();
    }

    private async Task DeleteReport()
    {
        if (model != null)
        {
            var result = await reportRepository.DeleteAsync(model.Rid);

            if (result > 0)
            {
                resultText = "삭제되었습니다.";
                await Task.Delay(1000);
                await OnDeleteSuccess.InvokeAsync();
                Close();
            }
            else
            {
                resultText = "삭제 실패했습니다.";
            }
        }
    }
}
```

#### ReportInitModal.razor

**경로**: `MdcHR26Apps.BlazorServer\Components\Report\Modal\ReportInitModal.razor`

**목적**: 평가 초기화 확인 모달

**구현 내용**:
```razor
@inject IReportRepository reportRepository

<div class="modal @ModalClass" tabindex="-1" style="display:@ModalDisplay;">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">평가 초기화</h5>
                <button type="button" class="btn-close" @onclick="Close"></button>
            </div>
            <div class="modal-body">
                <p>모든 평가를 초기화하시겠습니까?</p>
                <p class="text-danger">⚠️ 이 작업은 되돌릴 수 없습니다.</p>

                @if (!string.IsNullOrEmpty(resultText))
                {
                    <div class="alert alert-warning">@resultText</div>
                }
            </div>
            <div class="modal-footer">
                <button class="btn btn-danger" @onclick="InitReports">예(Yes)</button>
                <button class="btn btn-secondary" @onclick="Close">아니요(No)</button>
            </div>
        </div>
    </div>
</div>

@if (ShowBackdrop)
{
    <div class="modal-backdrop fade show"></div>
}

@code {
    [Parameter] public long Uid { get; set; }
    [Parameter] public EventCallback OnInitSuccess { get; set; }
    [Parameter] public EventCallback OnClose { get; set; }

    private string ModalDisplay = "none;";
    private string ModalClass = "";
    private bool ShowBackdrop = false;
    private string resultText = string.Empty;

    public void Open()
    {
        ModalDisplay = "block;";
        ModalClass = "show";
        ShowBackdrop = true;
        StateHasChanged();
    }

    private void Close()
    {
        ModalDisplay = "none";
        ModalClass = "";
        ShowBackdrop = false;
        resultText = string.Empty;
        OnClose.InvokeAsync();
    }

    private async Task InitReports()
    {
        var reports = await reportRepository.GetByUidAllAsync(Uid);

        foreach (var report in reports)
        {
            await reportRepository.DeleteAsync(report.Rid);
        }

        resultText = "초기화되었습니다.";
        await Task.Delay(1000);
        await OnInitSuccess.InvokeAsync();
        Close();
    }
}
```

#### ReportSubmitModal.razor

**경로**: `MdcHR26Apps.BlazorServer\Components\Report\Modal\ReportSubmitModal.razor`

**목적**: 평가 제출 확인 모달

**구현 내용**:
```razor
<div class="modal @ModalClass" tabindex="-1" style="display:@ModalDisplay;">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">평가 제출</h5>
                <button type="button" class="btn-close" @onclick="Close"></button>
            </div>
            <div class="modal-body">
                <p>평가를 제출하시겠습니까?</p>
                <p>제출 후에는 수정할 수 없습니다.</p>

                @if (!string.IsNullOrEmpty(resultText))
                {
                    <div class="alert alert-success">@resultText</div>
                }
            </div>
            <div class="modal-footer">
                <button class="btn btn-primary" @onclick="SubmitReport">제출</button>
                <button class="btn btn-secondary" @onclick="Close">취소</button>
            </div>
        </div>
    </div>
</div>

@if (ShowBackdrop)
{
    <div class="modal-backdrop fade show"></div>
}

@code {
    [Parameter] public EventCallback OnSubmitSuccess { get; set; }
    [Parameter] public EventCallback OnClose { get; set; }

    private string ModalDisplay = "none;";
    private string ModalClass = "";
    private bool ShowBackdrop = false;
    private string resultText = string.Empty;

    public void Open()
    {
        ModalDisplay = "block;";
        ModalClass = "show";
        ShowBackdrop = true;
        StateHasChanged();
    }

    private void Close()
    {
        ModalDisplay = "none";
        ModalClass = "";
        ShowBackdrop = false;
        resultText = string.Empty;
        OnClose.InvokeAsync();
    }

    private async Task SubmitReport()
    {
        resultText = "제출되었습니다.";
        await Task.Delay(1000);
        await OnSubmitSuccess.InvokeAsync();
        Close();
    }
}
```

---

### 4.8-4.15. ViewPage 컴포넌트 (8개)

**공통 구조**:
```razor
@page "/report/[페이지명]"
@inject IReportRepository reportRepository
@inject NavigationManager NavigationManager

<h3>[페이지 제목]</h3>

@if (reports == null)
{
    <p><em>Loading...</em></p>
}
else
{
    <[컴포넌트명] reports="reports" OnEdit="HandleEdit" />
}

@code {
    private List<ReportDb> reports = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        // Repository 호출
    }

    private void HandleEdit(long rid)
    {
        NavigationManager.NavigateTo($"/report/edit/{rid}");
    }
}
```

**8개 ViewPage 목록**:
1. **AdminReportListView** - 관리자용 평가 목록 (`/admin/reports`)
2. **Complete_TotalReportListView** - 완료된 평가 목록 (`/reports/complete`)
3. **DirectorReportListView** - 임원 평가 목록 (`/director/reports`)
4. **ReportTaskListView** - 평가 업무 목록 (`/reports/tasks`)
5. **TeamLeaderReportListView** - 팀장 평가 목록 (`/teamleader/reports`)
6. **User_TotalReportListView** - 사용자 전체 리포트 (`/user/reports`)
7. **FeedBack_TotalReportListView** - 피드백 리포트 (`/reports/feedback`)
8. **TeamLeader_TotalReportListView** - 팀장 전체 리포트 (`/teamleader/reports/total`)

---

## 5. 폴더 구조

**⚠️ 중요**: Agreement/SubAgreement와 동일한 통합 경로 사용

**생성할 폴더**:
```
MdcHR26Apps.BlazorServer/
└── Components/
    └── Pages/
        └── Components/
            └── Report/                      ← 통합 경로
                ├── ReportListTable.razor
                ├── ReportListTable.razor.cs
                ├── TeamLeaderReportDetailsTable.razor
                ├── TeamLeaderReportDetailsTable.razor.cs
                ├── DirectorReportDetailsTable.razor
                ├── DirectorReportDetailsTable.razor.cs
                ├── TeamLeader_TotalReportList.razor
                ├── TeamLeader_TotalReportList.razor.cs
                ├── Modal/
                │   ├── ReportDeleteModal.razor
                │   ├── ReportDeleteModal.razor.cs
                │   ├── ReportInitModal.razor
                │   ├── ReportInitModal.razor.cs
                │   ├── ReportSubmitModal.razor
                │   └── ReportSubmitModal.razor.cs
                └── ViewPage/
                    ├── AdminReportListView.razor
                    ├── AdminReportListView.razor.cs
                    ├── Complete_TotalReportListView.razor
                    ├── Complete_TotalReportListView.razor.cs
                    ├── DirectorReportListView.razor
                    ├── DirectorReportListView.razor.cs
                    ├── ReportTaskListView.razor
                    ├── ReportTaskListView.razor.cs
                    ├── TeamLeaderReportListView.razor
                    ├── TeamLeaderReportListView.razor.cs
                    ├── User_TotalReportListView.razor
                    ├── User_TotalReportListView.razor.cs
                    ├── FeedBack_TotalReportListView.razor
                    ├── FeedBack_TotalReportListView.razor.cs
                    ├── TeamLeader_TotalReportListView.razor
                    └── TeamLeader_TotalReportListView.razor.cs
```

---

## 6. 공통 구현 사항

### 6.1. 네임스페이스
```csharp
namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Report;
```

**⚠️ 중요**: 컴포넌트는 통합 경로 `Components/Pages/Components/Report/`에 생성합니다.

### 6.2. 필수 using 구문
```csharp
@using MdcHR26Apps.Models.EvaluationReport
@inject IReportRepository reportRepository
@inject NavigationManager NavigationManager
```

### 6.3. 3단계 평가 필드 그룹
- **User_Evaluation_1~4**: 본인 평가
- **TeamLeader_Evaluation_1~4**: 팀장 평가
- **Director_Evaluation_1~4**: 임원 평가

### 6.4. 평가 완료 여부 판단
```csharp
// 본인 평가 완료
bool isUserCompleted = !string.IsNullOrEmpty(report.User_Evaluation_4);

// 팀장 평가 완료
bool isTeamLeaderCompleted = !string.IsNullOrEmpty(report.TeamLeader_Evaluation_4);

// 임원 평가 완료
bool isDirectorCompleted = !string.IsNullOrEmpty(report.Director_Evaluation_4);
```

---

## 7. 테스트 항목

개발자가 테스트할 항목:

### Test 1: 컴포넌트 빌드 확인
1. 프로젝트 빌드 실행
2. **확인**: 경고 0개, 오류 0개

### Test 2: ReportListTable 렌더링
1. 샘플 데이터 전달 (User_Evaluation_4 있음/없음)
2. **확인**: 체크박스 상태 정상, "평가하기"/"수정하기" 버튼 분기

### Test 3: TeamLeaderReportDetailsTable 입력
1. ReportDb 전달, 팀장 평가 입력
2. **확인**: 본인 평가 읽기 전용, 팀장 평가 입력 가능

### Test 4: DirectorReportDetailsTable 입력
1. ReportDb 전달, 임원 평가 입력
2. **확인**: 본인+팀장 평가 읽기 전용, 임원 평가 입력 가능

### Test 5: ReportInitModal 동작
1. Uid 전달, 모달 Open()
2. 초기화 확인 클릭
3. **확인**: GetByUidAllAsync() 호출 → foreach DeleteAsync() 실행

### Test 6: ViewPage 라우팅
1. 각 ViewPage URL 접근
2. **확인**: 라우팅 정상, Repository 호출, 컴포넌트 렌더링

---

## 8. 완료 조건

- [ ] 15개 컴포넌트 파일 생성 (razor + razor.cs)
- [ ] 폴더 구조 생성 (Components/Pages/Components/Report/, Modal/, ViewPage/)
- [ ] 빌드 성공 (경고 0개, 오류 0개)
- [ ] Entity 필드명 정확히 사용 (User_Evaluation_*, TeamLeader_Evaluation_*, Director_Evaluation_*)
- [ ] Repository 메서드 정확히 사용 (GetByUidAllAsync, DeleteAsync)
- [ ] 3단계 평가 분기 로직 구현
- [ ] Test 1-6 모두 통과
- [ ] 2025년 프로젝트와 UI/UX 일치

---

## 9. 주의사항

### ❌ 절대 사용 금지
- `GetCountByUidAsync()` - 대신 `GetByUidAllAsync().Count()` 사용
- `DeleteAllByUidAsync()` - 대신 `foreach` + `DeleteAsync()` 사용

### ✅ 반드시 사용
- `GetByUidAllAsync(long uid)` - 사용자별 평가 목록
- `DeleteAsync(long rid)` - 개별 평가 삭제
- `UpdateAsync(ReportDb model)` - 평가 수정

### ReportDb 특성
- SubAgreementDb 기반으로 생성 (Task_Number 연계)
- 3단계 평가 프로세스: User → TeamLeader → Director
- 평가 완료 여부는 _Evaluation_4 (코멘트) 필드로 판단
- Total_Score는 3단계 평가 완료 후 계산

### 참조 문서
- [20260203_14_components_subagreement_v2.md](20260203_14_components_subagreement_v2.md) - SubAgreement 컴포넌트
- [20260203_13_REWRITE_GUIDE.md](20260203_13_REWRITE_GUIDE.md) - 재작성 가이드

---

**작성일**: 2026-02-04
**담당**: Claude Sonnet 4.5
**관련 이슈**: [#009](../issues/009_phase3_webapp_development.md)
**다음 작업지시서**: 20260203_16_components_common_form_v2.md (9개 컴포넌트)
