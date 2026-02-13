# 작업지시서: Report 관련 컴포넌트 구현

**날짜**: 2026-02-03
**작업 타입**: 컴포넌트 구현
**예상 소요**: 3-4시간
**관련 이슈**: [#009](../issues/009_phase3_webapp_development.md)
**선행 작업**: [20260203_06_components_subagreement.md](20260203_06_components_subagreement.md)

---

## 1. 작업 개요

### 배경
- Agreement, SubAgreement 컴포넌트 구현 완료
- 1st/2nd/3rd HR Report (평가) 관련 컴포넌트 구현 필요

### 목표
Report (본인평가/부서장평가/임원평가) 페이지에서 사용할 컴포넌트를 2026년 프로젝트에 구현

### 구현 범위
1. **Table 컴포넌트** (5개)
   - ReportListTable: 평가 목록 테이블
   - TeamLeaderReportDetailsTable: 부서장 평가 상세 테이블
   - DirectorReportDetailsTable: 임원 평가 상세 테이블
   - TeamLeader_TotalReportList: 부서장용 최종 평가 목록
   - TaskListTable: 업무 목록 테이블

2. **ViewPage 컴포넌트** (8개)
   - ReportTaskListView: 평가 업무 목록 뷰
   - TeamLeaderReportListView: 부서장 평가 목록 뷰
   - DirectorReportListView: 임원 평가 목록 뷰
   - User_TotalReportListView: 사용자 최종 평가 뷰
   - TeamLeader_TotalReportListView: 부서장 최종 평가 뷰
   - Complete_TotalReportListView: 완료된 최종 평가 뷰
   - FeedBack_TotalReportListView: 피드백 평가 뷰
   - CompletedTaskListView: 완료된 업무 목록 뷰

3. **Modal 컴포넌트** (2개)
   - ReportDeleteModal: 평가 삭제 확인 모달
   - ReportSubmitModal: 평가 제출 확인 모달

4. **CommonView 컴포넌트** (2개)
   - TeamLeaderViewExcel: 부서장 엑셀 다운로드 버튼
   - DirectorViewExcel: 임원 엑셀 다운로드 버튼

---

## 2. 데이터 모델 (2026년 DB 구조)

### ReportDb
**경로**: `MdcHR26Apps.Models/ReportDb/ReportDb.cs`

**주요 필드**:
- Rid (Int64) - PK
- Uid (Int64) - FK to UserDb
- User_Evaluation_1~4 (본인 평가)
- TeamLeader_Evaluation_1~4 (부서장 평가)
- Director_Evaluation_1~4 (임원 평가)
- Total_Score (최종 점수)

### TasksDb
**경로**: `MdcHR26Apps.Models/TasksDb/TasksDb.cs`

### v_ReportTaskListDB
**경로**: `MdcHR26Apps.Models/Views/v_ReportTaskListDB/v_ReportTaskListDB.cs`

---

## 3. 컴포넌트 구현

### 3.1. ReportListTable.razor

**경로**: `MdcHR26Apps.BlazorServer/Components/Pages/Components/Table/ReportListTable.razor`

**기능**: 평가 목록 테이블 (본인평가 대상 목록)

**Parameters**:
```csharp
[Parameter] public List<ReportDb> reports { get; set; } = new();
[Parameter] public EventCallback<long> OnEdit { get; set; }
[Parameter] public EventCallback<long> OnDetails { get; set; }
```

**코드**:
```razor
@using MdcHR26Apps.Models.ReportDb

<table class="table table-hover table-striped">
    <thead>
        <tr>
            <th>#</th>
            <th>직무명</th>
            <th>세부업무</th>
            <th>비중</th>
            <th>본인평가</th>
            <th>상태</th>
            <th>작업</th>
        </tr>
    </thead>
    <tbody>
        @{ int sortNo = 1; }
        @foreach (var item in reports)
        {
            <tr>
                <td>@sortNo</td>
                <td>@item.Report_Item_Name_1</td>
                <td>@item.Report_SubItem_Name</td>
                <td>@item.Report_SubItem_Proportion %</td>
                <td>@item.User_Evaluation_3.ToString("F1")</td>
                <td>
                    @if (item.User_Evaluation_3 > 0)
                    {
                        <span class="badge bg-success">평가완료</span>
                    }
                    else
                    {
                        <span class="badge bg-warning">미평가</span>
                    }
                </td>
                <td>
                    <button class="btn btn-sm btn-primary me-1" @onclick="@(() => OnEdit.InvokeAsync(item.Rid))">평가</button>
                    <button class="btn btn-sm btn-info" @onclick="@(() => OnDetails.InvokeAsync(item.Rid))">상세</button>
                </td>
            </tr>
            sortNo++;
        }
    </tbody>
</table>
```

---

### 3.2. TeamLeaderReportDetailsTable.razor

**경로**: `MdcHR26Apps.BlazorServer/Components/Pages/Components/Table/TeamLeaderReportDetailsTable.razor`

**기능**: 부서장 평가 상세 테이블

**Parameters**:
```csharp
[Parameter] public ReportDb? report { get; set; }
```

**코드**:
```razor
@using MdcHR26Apps.Models.ReportDb

@if (report != null)
{
    <table class="table table-bordered">
        <tbody>
            <tr class="table-light">
                <th colspan="2" class="text-center">평가 대상 정보</th>
            </tr>
            <tr>
                <th style="width: 25%;">직무명</th>
                <td>@report.Report_Item_Name_1</td>
            </tr>
            <tr>
                <th>세부 업무</th>
                <td>@report.Report_SubItem_Name</td>
            </tr>
            <tr>
                <th>비중</th>
                <td>@report.Report_SubItem_Proportion %</td>
            </tr>

            <tr class="table-light">
                <th colspan="2" class="text-center">본인 평가 결과</th>
            </tr>
            <tr>
                <th>난이도</th>
                <td>@report.User_Evaluation_1 / 10</td>
            </tr>
            <tr>
                <th>성과</th>
                <td>@report.User_Evaluation_2 / 10</td>
            </tr>
            <tr>
                <th>총점</th>
                <td><strong>@report.User_Evaluation_3.ToString("F1")</strong> / 10</td>
            </tr>
            <tr>
                <th>코멘트</th>
                <td style="white-space: pre-wrap;">@report.User_Evaluation_4</td>
            </tr>

            <tr class="table-light">
                <th colspan="2" class="text-center">부서장 평가 결과</th>
            </tr>
            <tr>
                <th>난이도</th>
                <td>
                    @if (report.TeamLeader_Evaluation_1 > 0)
                    {
                        @report.TeamLeader_Evaluation_1.ToString("F1")
                        <text> / 10</text>
                    }
                    else
                    {
                        <span class="text-muted">미평가</span>
                    }
                </td>
            </tr>
            <tr>
                <th>성과</th>
                <td>
                    @if (report.TeamLeader_Evaluation_2 > 0)
                    {
                        @report.TeamLeader_Evaluation_2.ToString("F1")
                        <text> / 10</text>
                    }
                    else
                    {
                        <span class="text-muted">미평가</span>
                    }
                </td>
            </tr>
            <tr>
                <th>총점</th>
                <td>
                    @if (report.TeamLeader_Evaluation_3 > 0)
                    {
                        <strong>@report.TeamLeader_Evaluation_3.ToString("F1")</strong>
                        <text> / 10</text>
                    }
                    else
                    {
                        <span class="text-muted">미평가</span>
                    }
                </td>
            </tr>
            <tr>
                <th>코멘트</th>
                <td style="white-space: pre-wrap;">@report.TeamLeader_Evaluation_4</td>
            </tr>
        </tbody>
    </table>
}
```

---

### 3.3. DirectorReportDetailsTable.razor

**경로**: `MdcHR26Apps.BlazorServer/Components/Pages/Components/Table/DirectorReportDetailsTable.razor`

**기능**: 임원 평가 상세 테이블 (본인+부서장+임원 평가 모두 표시)

**Parameters**:
```csharp
[Parameter] public ReportDb? report { get; set; }
```

**코드**:
```razor
@using MdcHR26Apps.Models.ReportDb

@if (report != null)
{
    <table class="table table-bordered">
        <tbody>
            @* 평가 대상 정보 *@
            <tr class="table-light">
                <th colspan="2" class="text-center">평가 대상 정보</th>
            </tr>
            <tr>
                <th style="width: 25%;">직무명</th>
                <td>@report.Report_Item_Name_1</td>
            </tr>
            <tr>
                <th>세부 업무</th>
                <td>@report.Report_SubItem_Name</td>
            </tr>

            @* 본인 평가 *@
            <tr class="table-light">
                <th colspan="2" class="text-center">본인 평가</th>
            </tr>
            <tr>
                <th>총점</th>
                <td><strong>@report.User_Evaluation_3.ToString("F1")</strong> / 10</td>
            </tr>

            @* 부서장 평가 *@
            <tr class="table-light">
                <th colspan="2" class="text-center">부서장 평가</th>
            </tr>
            <tr>
                <th>총점</th>
                <td><strong>@report.TeamLeader_Evaluation_3.ToString("F1")</strong> / 10</td>
            </tr>
            <tr>
                <th>코멘트</th>
                <td style="white-space: pre-wrap;">@report.TeamLeader_Evaluation_4</td>
            </tr>

            @* 임원 평가 *@
            <tr class="table-light">
                <th colspan="2" class="text-center">임원 평가</th>
            </tr>
            <tr>
                <th>난이도</th>
                <td>
                    @if (report.Director_Evaluation_1 > 0)
                    {
                        @report.Director_Evaluation_1.ToString("F1")
                        <text> / 10</text>
                    }
                    else
                    {
                        <span class="text-muted">미평가</span>
                    }
                </td>
            </tr>
            <tr>
                <th>성과</th>
                <td>
                    @if (report.Director_Evaluation_2 > 0)
                    {
                        @report.Director_Evaluation_2.ToString("F1")
                        <text> / 10</text>
                    }
                    else
                    {
                        <span class="text-muted">미평가</span>
                    }
                </td>
            </tr>
            <tr>
                <th>총점</th>
                <td>
                    @if (report.Director_Evaluation_3 > 0)
                    {
                        <strong>@report.Director_Evaluation_3.ToString("F1")</strong>
                        <text> / 10</text>
                    }
                    else
                    {
                        <span class="text-muted">미평가</span>
                    }
                </td>
            </tr>
            <tr>
                <th>코멘트</th>
                <td style="white-space: pre-wrap;">@report.Director_Evaluation_4</td>
            </tr>

            @* 최종 점수 *@
            <tr class="table-success">
                <th>최종 총점</th>
                <td><h5 class="mb-0">@report.Total_Score.ToString("F1") / 10</h5></td>
            </tr>
        </tbody>
    </table>
}
```

---

### 3.4. TaskListTable.razor

**경로**: `MdcHR26Apps.BlazorServer/Components/Pages/Components/Table/TaskListTable.razor`

**기능**: 업무 목록 테이블 (TasksDb)

**Parameters**:
```csharp
[Parameter] public List<TasksDb> tasks { get; set; } = new();
[Parameter] public EventCallback<long> OnDetails { get; set; }
```

**코드**:
```razor
@using MdcHR26Apps.Models.TasksDb

<table class="table table-hover table-striped">
    <thead>
        <tr>
            <th>#</th>
            <th>업무명</th>
            <th>목표</th>
            <th>목표비중</th>
            <th>결과비중</th>
            <th>상태</th>
            <th>작업</th>
        </tr>
    </thead>
    <tbody>
        @{ int sortNo = 1; }
        @foreach (var task in tasks)
        {
            <tr>
                <td>@sortNo</td>
                <td>@task.TaskName</td>
                <td class="text-truncate" style="max-width: 200px;">@task.TaskObjective</td>
                <td>@task.TargetProportion %</td>
                <td>@task.ResultProportion %</td>
                <td>
                    @if (task.TaskStatus == 0)
                    {
                        <span class="badge bg-secondary">미완료</span>
                    }
                    else if (task.TaskStatus == 1)
                    {
                        <span class="badge bg-warning">진행중</span>
                    }
                    else
                    {
                        <span class="badge bg-success">완료</span>
                    }
                </td>
                <td>
                    <button class="btn btn-sm btn-info" @onclick="@(() => OnDetails.InvokeAsync(task.Tid))">상세</button>
                </td>
            </tr>
            sortNo++;
        }
    </tbody>
</table>
```

---

### 3.5. ViewPage 컴포넌트들

**공통 구조**: 읽기 전용 테이블, 데이터를 깔끔하게 표시

#### ReportTaskListView.razor
```razor
@using MdcHR26Apps.Models.Views.v_ReportTaskListDB

<div class="table-responsive">
    <table class="table table-bordered table-sm">
        <thead class="table-light">
            <tr>
                <th>#</th>
                <th>직무</th>
                <th>세부업무</th>
                <th>본인</th>
                <th>부서장</th>
                <th>임원</th>
                <th>최종</th>
            </tr>
        </thead>
        <tbody>
            @{ int sortNo = 1; }
            @foreach (var item in ReportTaskList)
            {
                <tr>
                    <td>@sortNo</td>
                    <td>@item.Report_Item_Name_1</td>
                    <td>@item.Report_SubItem_Name</td>
                    <td>@item.User_Evaluation_3.ToString("F1")</td>
                    <td>@item.TeamLeader_Evaluation_3.ToString("F1")</td>
                    <td>@item.Director_Evaluation_3.ToString("F1")</td>
                    <td><strong>@item.Total_Score.ToString("F1")</strong></td>
                </tr>
                sortNo++;
            }
        </tbody>
    </table>
</div>

@code {
    [Parameter] public List<v_ReportTaskListDB> ReportTaskList { get; set; } = new();
}
```

---

### 3.6. Modal 컴포넌트

#### ReportDeleteModal.razor

**경로**: `MdcHR26Apps.BlazorServer/Components/Pages/Components/Modal/ReportDeleteModal.razor`

```razor
@using MdcHR26Apps.Models.ReportDb

@if (Show)
{
    <div class="modal fade show d-block" tabindex="-1" style="background-color: rgba(0,0,0,0.5);">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">평가 삭제 확인</h5>
                    <button type="button" class="btn-close" @onclick="OnCancel"></button>
                </div>
                <div class="modal-body">
                    @if (Report != null)
                    {
                        <p>다음 평가를 정말 삭제하시겠습니까?</p>
                        <div class="alert alert-warning">
                            <strong>직무:</strong> @Report.Report_Item_Name_1<br />
                            <strong>세부업무:</strong> @Report.Report_SubItem_Name
                        </div>
                        <p class="text-danger"><small>⚠️ 이 작업은 되돌릴 수 없습니다.</small></p>
                    }
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" @onclick="OnCancel">취소</button>
                    <button type="button" class="btn btn-danger" @onclick="OnConfirm">삭제</button>
                </div>
            </div>
        </div>
    </div>
}

@code {
    [Parameter] public bool Show { get; set; }
    [Parameter] public ReportDb? Report { get; set; }
    [Parameter] public EventCallback OnConfirm { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }
}
```

#### ReportSubmitModal.razor

```razor
@if (Show)
{
    <div class="modal fade show d-block" tabindex="-1" style="background-color: rgba(0,0,0,0.5);">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">평가 제출 확인</h5>
                    <button type="button" class="btn-close" @onclick="OnCancel"></button>
                </div>
                <div class="modal-body">
                    <p>평가를 제출하시겠습니까?</p>
                    <div class="alert alert-info">
                        <strong>평가 개수:</strong> @TotalCount 개<br />
                        <strong>완료:</strong> @CompletedCount 개<br />
                        <strong>미완료:</strong> @(TotalCount - CompletedCount) 개
                    </div>
                    @if (TotalCount > CompletedCount)
                    {
                        <p class="text-warning"><small>⚠️ 미완료 평가가 있습니다.</small></p>
                    }
                    <p class="text-danger"><small>제출 후에는 수정이 불가능합니다.</small></p>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" @onclick="OnCancel">취소</button>
                    <button type="button" class="btn btn-primary" @onclick="OnConfirm">제출</button>
                </div>
            </div>
        </div>
    </div>
}

@code {
    [Parameter] public bool Show { get; set; }
    [Parameter] public int TotalCount { get; set; }
    [Parameter] public int CompletedCount { get; set; }
    [Parameter] public EventCallback OnConfirm { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }
}
```

---

### 3.7. CommonView - Excel 다운로드

#### TeamLeaderViewExcel.razor

```razor
<div class="mb-3">
    <button class="btn btn-success" @onclick="DownloadExcel">
        <span class="oi oi-data-transfer-download me-1"></span>
        엑셀 다운로드
    </button>
</div>

@code {
    [Parameter] public EventCallback OnDownload { get; set; }

    private async Task DownloadExcel()
    {
        await OnDownload.InvokeAsync();
    }
}
```

#### DirectorViewExcel.razor

```razor
<div class="mb-3">
    <button class="btn btn-success" @onclick="DownloadExcel">
        <span class="oi oi-data-transfer-download me-1"></span>
        전체 평가 엑셀 다운로드
    </button>
</div>

@code {
    [Parameter] public EventCallback OnDownload { get; set; }

    private async Task DownloadExcel()
    {
        await OnDownload.InvokeAsync();
    }
}
```

---

## 4. 완료 조건

- [ ] ReportListTable.razor 완료
- [ ] TeamLeaderReportDetailsTable.razor 완료
- [ ] DirectorReportDetailsTable.razor 완료
- [ ] TeamLeader_TotalReportList.razor 완료 (간단)
- [ ] TaskListTable.razor 완료
- [ ] ReportTaskListView.razor 완료
- [ ] 기타 ViewPage 7개 완료 (유사 구조)
- [ ] ReportDeleteModal.razor 완료
- [ ] ReportSubmitModal.razor 완료
- [ ] TeamLeaderViewExcel.razor 완료
- [ ] DirectorViewExcel.razor 완료
- [ ] 빌드 오류 0개

---

## 5. 다음 단계

**다음 작업지시서**: [20260203_08_components_common_form.md](20260203_08_components_common_form.md)
- 공통 컴포넌트 및 Form 컴포넌트 구현

---

## 6. 관련 문서

**이슈**: [#009: Phase 3 WebApp 개발](../issues/009_phase3_webapp_development.md)
**Phase 2 Model**:
- [ReportDb](../../MdcHR26Apps.Models/ReportDb/)
- [TasksDb](../../MdcHR26Apps.Models/TasksDb/)
- [v_ReportTaskListDB](../../MdcHR26Apps.Models/Views/v_ReportTaskListDB/)
**참조 프로젝트**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Components`
