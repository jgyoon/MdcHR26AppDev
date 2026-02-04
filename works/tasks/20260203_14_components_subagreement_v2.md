# 작업지시서: Phase 3-4 SubAgreement 컴포넌트 구현 (8개)

**날짜**: 2026-02-04
**작업 유형**: 신규 기능 추가
**관련 이슈**: [#009: Phase 3 Blazor Server WebApp 개발](../issues/009_phase3_webapp_development.md)
**참조 가이드**: [20260203_13_REWRITE_GUIDE.md](20260203_13_REWRITE_GUIDE.md)
**선행 작업**: [20260203_13_components_agreement_v2.md](20260203_13_components_agreement_v2.md)

---

## 1. 작업 개요

**목적**:
- 세부직무평가 협의(SubAgreement) 관련 Blazor 컴포넌트 8개 구현
- 2025년 프로젝트 구조 참조, 2026년 Entity/Repository 구조 적용

**배경**:
- Agreement 컴포넌트 작업지시서 완료 (13번)
- SubAgreement는 Agreement의 하위 직무 상세 평가 담당
- 세부 직무 항목, 비중, 업무 목록 관리

---

## 2. Entity 및 Repository 구조

### 2.1. SubAgreementDb Entity

**경로**: `MdcHR26Apps.Models\EvaluationSubAgreement\SubAgreementDb.cs`

**필드 구조**:
```csharp
public class SubAgreementDb
{
    public Int64 Sid { get; set; }                      // PK
    public Int64 Uid { get; set; }                      // FK (UserDb)
    public int Report_Item_Number { get; set; }
    public string Report_Item_Name_1 { get; set; }
    public string Report_Item_Name_2 { get; set; }
    public int Report_Item_Proportion { get; set; }
    public string Report_SubItem_Name { get; set; }     // 세부직무명
    public int Report_SubItem_Proportion { get; set; }  // 세부직무비중
    public Int64 Task_Number { get; set; }              // 업무번호
}
```

### 2.2. ISubAgreementRepository 메서드 (7개)

**경로**: `MdcHR26Apps.Models\EvaluationSubAgreement\ISubAgreementRepository.cs`

```csharp
Task<SubAgreementDb> AddAsync(SubAgreementDb model);
Task<List<SubAgreementDb>> GetByAllAsync();
Task<SubAgreementDb> GetByIdAsync(long id);
Task<bool> UpdateAsync(SubAgreementDb model);
Task<bool> DeleteAsync(long id);
Task<List<SubAgreementDb>> GetByUserIdAllAsync(long userId);
Task<List<SubAgreementDb>> GetByTasksPeroportionAsync(long userId, string deptName, string indexName);
```

**주의**: Agreement와 동일하게 제거된 메서드 사용 금지
- ❌ `GetCountByUidAsync()` - 제거됨
- ❌ `DeleteAllByUidAsync()` - 제거됨
- ✅ 대신 `GetByUserIdAllAsync().Count` 사용
- ✅ 대신 `foreach` + `DeleteAsync()` 사용

---

## 3. 구현할 컴포넌트 (8개)

### 3.1. SubAgreementDbListTable.razor

**경로**: `MdcHR26Apps.BlazorServer\Components\SubAgreement\SubAgreementDbListTable.razor`

**목적**: 세부직무평가 목록을 테이블 형식으로 표시

**주요 기능**:
- SubAgreementDb 목록 표시 (평가지표, 평가직무, 세부평가직무, 세부직무비중)
- 상세 버튼 (UserSubAgreementDetailsAction 이벤트)
- 반응형 UI (col-12/col-md-5/col-md-6)

**2025년 참조**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Components\SubAgreementDbListTable.razor`

**구현 내용**:
```razor
<table class="table table-hover table-responsive-md">
    <thead>
        <tr>
            <th>#</th>
            <th>
                <div class="row">
                    <div class="col-12 col-md-5">평가지표</div>
                    <div class="col-12 col-md-5">평가직무</div>
                    <div class="col-12 col-md-2">직무비중</div>
                </div>
            </th>
            <th>
                <div class="row">
                    <div class="col-12 col-md-6">세부평가직무</div>
                    <div class="col-12 col-md-6">세부직무비중</div>
                </div>
            </th>
            <th>비고</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var item in subAgreementDbs)
        {
            <tr>
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
                    <button class="btn btn-info" @onclick="@(() => OnDetailsClick(item.Sid))">상세</button>
                </td>
            </tr>
            sortNo++;
        }
    </tbody>
</table>

@code {
    [Parameter] public List<SubAgreementDb> subAgreementDbs { get; set; } = new();
    [Parameter] public EventCallback<long> OnDetailsClick { get; set; }

    private int sortNo = 1;
}
```

---

### 3.2. SubAgreementDetailsTable.razor

**경로**: `MdcHR26Apps.BlazorServer\Components\SubAgreement\SubAgreementDetailsTable.razor`

**목적**: 세부직무평가 상세 정보를 테이블 형식으로 표시

**주요 기능**:
- 단일 SubAgreementDb 상세 정보 표시
- 평가지표, 평가직무, 세부평가직무, 비중 표시
- 편집/삭제 버튼

**2025년 참조**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Components\SubAgreementDetailsTable.razor`

**구현 내용**:
```razor
@if (subAgreement != null)
{
    <table class="table">
        <tbody>
            <tr>
                <th>평가지표</th>
                <td>@subAgreement.Report_Item_Name_1</td>
            </tr>
            <tr>
                <th>평가직무</th>
                <td>@subAgreement.Report_Item_Name_2</td>
            </tr>
            <tr>
                <th>직무비중</th>
                <td>@subAgreement.Report_Item_Proportion %</td>
            </tr>
            <tr>
                <th>세부평가직무</th>
                <td>@subAgreement.Report_SubItem_Name</td>
            </tr>
            <tr>
                <th>세부직무비중</th>
                <td>@subAgreement.Report_SubItem_Proportion %</td>
            </tr>
            <tr>
                <th>업무번호</th>
                <td>@subAgreement.Task_Number</td>
            </tr>
        </tbody>
    </table>

    <div class="mt-3">
        <button class="btn btn-primary" @onclick="OnEdit">편집</button>
        <button class="btn btn-danger" @onclick="OnDelete">삭제</button>
        <button class="btn btn-secondary" @onclick="OnBack">목록</button>
    </div>
}

@code {
    [Parameter] public SubAgreementDb? subAgreement { get; set; }
    [Parameter] public EventCallback OnEdit { get; set; }
    [Parameter] public EventCallback OnDelete { get; set; }
    [Parameter] public EventCallback OnBack { get; set; }
}
```

---

### 3.3. SubAgreementListTable.razor

**경로**: `MdcHR26Apps.BlazorServer\Components\SubAgreement\SubAgreementListTable.razor`

**목적**: 세부직무평가 목록 (간소화 버전)

**주요 기능**:
- SubAgreementDb 목록 간단 표시
- 선택/편집 기능

**2025년 참조**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Components\SubAgreementListTable.razor`

**구현 내용**:
```razor
<table class="table table-sm">
    <thead>
        <tr>
            <th>번호</th>
            <th>평가지표</th>
            <th>평가직무</th>
            <th>세부직무</th>
            <th>비중</th>
            <th>편집</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var item in subAgreements)
        {
            <tr>
                <td>@item.Report_Item_Number</td>
                <td>@item.Report_Item_Name_1</td>
                <td>@item.Report_Item_Name_2</td>
                <td>@item.Report_SubItem_Name</td>
                <td>@item.Report_SubItem_Proportion %</td>
                <td>
                    <button class="btn btn-sm btn-outline-primary" @onclick="@(() => OnEdit(item.Sid))">편집</button>
                </td>
            </tr>
        }
    </tbody>
</table>

@code {
    [Parameter] public List<SubAgreementDb> subAgreements { get; set; } = new();
    [Parameter] public EventCallback<long> OnEdit { get; set; }
}
```

---

### 3.4. SubAgreementResetList.razor

**경로**: `MdcHR26Apps.BlazorServer\Components\SubAgreement\SubAgreementResetList.razor`

**목적**: 세부직무평가 재설정 목록 (읽기 전용)

**주요 기능**:
- 세부직무평가 목록 표시 (비고 없음)
- 재설정/초기화 시 참조용
- 반응형 UI

**2025년 참조**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Components\SubAgreementResetList.razor`

**구현 내용**:
```razor
<table class="table table-hover table-responsive-md">
    <thead>
        <tr>
            <th>#</th>
            <th>
                <div class="row">
                    <div class="col-12 col-md-5">평가지표</div>
                    <div class="col-12 col-md-5">평가직무</div>
                    <div class="col-12 col-md-2">직무비중</div>
                </div>
            </th>
            <th>
                <div class="row">
                    <div class="col-12 col-md-6">세부평가직무</div>
                    <div class="col-12 col-md-6">세부직무비중</div>
                </div>
            </th>
        </tr>
    </thead>
    <tbody>
        @foreach (var item in reportDbList)
        {
            <tr>
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
            </tr>
            sortNo++;
        }
    </tbody>
</table>

@code {
    [Parameter] public List<SubAgreementDb> reportDbList { get; set; } = new();

    private int sortNo = 1;
}
```

---

### 3.5. SubAgreementDbListView.razor

**경로**: `MdcHR26Apps.BlazorServer\Components\SubAgreement\ViewPage\SubAgreementDbListView.razor`

**목적**: 세부직무평가 목록 전체 페이지

**주요 기능**:
- SubAgreementDbListTable 컴포넌트 통합
- Repository 연동
- 검색/필터 기능

**2025년 참조**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Components\ViewPage\SubAgreementDbListView.razor`

**구현 내용**:
```razor
@inject ISubAgreementRepository subAgreementRepository
@inject NavigationManager NavigationManager

<h3>세부직무평가 목록</h3>

@if (subAgreements == null)
{
    <p><em>Loading...</em></p>
}
else
{
    <SubAgreementDbListTable subAgreementDbs="subAgreements" OnDetailsClick="HandleDetailsClick" />
}

@code {
    [Parameter] public long Uid { get; set; }

    private List<SubAgreementDb> subAgreements = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        subAgreements = await subAgreementRepository.GetByUserIdAllAsync(Uid);
    }

    private void HandleDetailsClick(long sid)
    {
        NavigationManager.NavigateTo($"/subagreement/details/{sid}");
    }
}
```

---

### 3.6. SubAgreementDeleteModal.razor

**경로**: `MdcHR26Apps.BlazorServer\Components\SubAgreement\Modal\SubAgreementDeleteModal.razor`

**목적**: 세부직무 삭제 확인 모달

**주요 기능**:
- 삭제 확인 UI
- Repository DeleteAsync 호출
- 결과 메시지 표시

**2025년 참조**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Components\Modal\SubAgreementDeleteModal.razor`

**구현 내용**:
```razor
@inject ISubAgreementRepository subAgreementRepository

@if (model != null)
{
    <div class="modal @ModalClass" tabindex="-1" role="dialog" style="display:@ModalDisplay;">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">세부직무 삭제</h5>
                    <button type="button" class="btn-close" @onclick="Close"></button>
                </div>
                <div class="modal-body">
                    <p>이 세부직무를 삭제하시겠습니까?</p>
                    <p>평가지표: @model.Report_Item_Name_1</p>
                    <p>평가직무: @model.Report_Item_Name_2</p>
                    <p>세부직무: @model.Report_SubItem_Name</p>

                    @if (!string.IsNullOrEmpty(resultText))
                    {
                        <div class="alert alert-info">@resultText</div>
                    }
                </div>
                <div class="modal-footer">
                    <button class="btn btn-danger" @onclick="DeleteUserSubAgreement">예(Yes)</button>
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
    [Parameter] public SubAgreementDb? model { get; set; }
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

    private async Task DeleteUserSubAgreement()
    {
        if (model != null)
        {
            var result = await subAgreementRepository.DeleteAsync(model.Sid);

            if (result)
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

---

### 3.7. AgreeItemLists.razor

**경로**: `MdcHR26Apps.BlazorServer\Components\SubAgreement\CommonView\AgreeItemLists.razor`

**목적**: 합의된 직무 목록 표시 (접기/펼치기)

**주요 기능**:
- 합의된 직무 개수 표시
- 직무 목록 (ItemName, ItemPeroportion, ItemSubPeroportion)
- 완료 상태 표시 (CheckedCode)
- 토글 기능

**2025년 참조**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Components\CommonView\AgreeItemLists.razor`

**구현 내용**:
```razor
@if (items == null)
{
    <p><em>Loading...</em></p>
}
else if (items.Count == 0)
{
    <p><em>합의된 직무내용이 없습니다.</em></p>
}
else
{
    <div class="row mb-3">
        <div class="col-12">
            <h5>
                합의된 직무 수 : @itemCount 개
                <span @onclick="Toggle" class="@IconClass" style="cursor: pointer;"></span>
            </h5>
        </div>
    </div>

    @if (Collapsed)
    {
        <hr />
        <div class="row">
            <div class="col-12">
                <ul class="list-group">
                    @foreach (var item in items)
                    {
                        <li class="list-group-item">
                            <label>
                                @if (item.ItemCompleteStatus)
                                {
                                    @CheckedCode
                                }
                                @item.ItemName ( @item.ItemPeroportion % ) :
                            </label>
                            @item.ItemSubPeroportion %
                        </li>
                    }
                </ul>
            </div>
        </div>
        <hr />
    }
}

@code {
    [Parameter] public List<AgreeItemModel> items { get; set; } = new();

    private bool Collapsed = false;
    private string IconClass => Collapsed ? "oi oi-minus" : "oi oi-plus";
    private int itemCount => items?.Count ?? 0;
    private string CheckedCode = "✓ ";

    private void Toggle()
    {
        Collapsed = !Collapsed;
    }

    // AgreeItemModel 정의 (로컬 모델)
    public class AgreeItemModel
    {
        public string ItemName { get; set; } = string.Empty;
        public int ItemPeroportion { get; set; }
        public int ItemSubPeroportion { get; set; }
        public bool ItemCompleteStatus { get; set; }
    }
}
```

---

### 3.8. ReportTaskListCommonView.razor

**경로**: `MdcHR26Apps.BlazorServer\Components\SubAgreement\CommonView\ReportTaskListCommonView.razor`

**목적**: 업무 목록 공용 뷰 컴포넌트

**주요 기능**:
- 업무 목록 표시 (TasksDb 연동)
- 세부직무별 업무 필터링
- 업무 선택/추가 기능

**2025년 참조**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Components\CommonView\ReportTaskListCommonView.razor`

**구현 내용**:
```razor
@inject ITasksRepository tasksRepository

@if (tasks == null)
{
    <p><em>Loading...</em></p>
}
else if (tasks.Count == 0)
{
    <p><em>등록된 업무가 없습니다.</em></p>
}
else
{
    <div class="row mb-3">
        <div class="col-12">
            <h5>업무 목록 (@tasks.Count 개)</h5>
        </div>
    </div>

    <table class="table table-sm">
        <thead>
            <tr>
                <th>번호</th>
                <th>업무명</th>
                <th>업무목표</th>
                <th>목표비중</th>
                <th>선택</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var task in tasks)
            {
                <tr>
                    <td>@task.TaksListNumber</td>
                    <td>@task.TaskName</td>
                    <td>@task.TaskObjective</td>
                    <td>@task.TargetProportion %</td>
                    <td>
                        <button class="btn btn-sm btn-primary" @onclick="@(() => OnSelectTask(task.Tid))">선택</button>
                    </td>
                </tr>
            }
        </tbody>
    </table>
}

@code {
    [Parameter] public long SubAgreementId { get; set; }
    [Parameter] public EventCallback<long> OnSelectTask { get; set; }

    private List<TasksDb> tasks = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadTasks();
    }

    private async Task LoadTasks()
    {
        // SubAgreement와 연결된 업무 목록 로드
        // 실제 구현 시 필터링 로직 추가
        tasks = new List<TasksDb>();
    }
}
```

---

## 4. 폴더 구조

**생성할 폴더**:
```
MdcHR26Apps.BlazorServer/
└── Components/
    └── SubAgreement/
        ├── SubAgreementDbListTable.razor
        ├── SubAgreementDbListTable.razor.cs
        ├── SubAgreementDetailsTable.razor
        ├── SubAgreementDetailsTable.razor.cs
        ├── SubAgreementListTable.razor
        ├── SubAgreementListTable.razor.cs
        ├── SubAgreementResetList.razor
        ├── SubAgreementResetList.razor.cs
        ├── ViewPage/
        │   ├── SubAgreementDbListView.razor
        │   └── SubAgreementDbListView.razor.cs
        ├── Modal/
        │   ├── SubAgreementDeleteModal.razor
        │   └── SubAgreementDeleteModal.razor.cs
        └── CommonView/
            ├── AgreeItemLists.razor
            ├── AgreeItemLists.razor.cs
            ├── ReportTaskListCommonView.razor
            └── ReportTaskListCommonView.razor.cs
```

---

## 5. 공통 구현 사항

### 5.1. 네임스페이스
```csharp
namespace MdcHR26Apps.BlazorServer.Components.SubAgreement;
```

### 5.2. 필수 using 구문
```csharp
@using MdcHR26Apps.Models.EvaluationSubAgreement
@using MdcHR26Apps.Models.EvaluationTasks
@inject ISubAgreementRepository subAgreementRepository
@inject ITasksRepository tasksRepository
@inject NavigationManager NavigationManager
```

### 5.3. SubAgreement 특화 필드
- `Report_SubItem_Name`: 세부직무명
- `Report_SubItem_Proportion`: 세부직무비중
- `Task_Number`: 업무번호 (TasksDb 연계)

### 5.4. 반응형 CSS 클래스
- 3단 구조: 평가지표/평가직무 (col-md-5) + 세부직무 (col-md-6)
- `table-responsive-md`: 모바일 대응

---

## 6. 테스트 항목

개발자가 테스트할 항목:

### Test 1: 컴포넌트 빌드 확인
1. 프로젝트 빌드 실행
2. **확인**: 경고 0개, 오류 0개

### Test 2: SubAgreementDbListTable 렌더링
1. 테스트 페이지에 SubAgreementDbListTable 추가
2. 샘플 데이터 전달 (Report_SubItem_* 필드 포함)
3. **확인**: 3단 컬럼 구조 정상 렌더링

### Test 3: SubAgreementDeleteModal 동작
1. 모달 Open() 호출
2. 삭제 확인 클릭
3. **확인**: Repository DeleteAsync 호출 (Sid 사용)

### Test 4: AgreeItemLists 토글
1. AgreeItemModel 리스트 전달
2. 접기/펼치기 아이콘 클릭
3. **확인**: 목록 표시/숨김, 체크 표시 정상

### Test 5: ReportTaskListCommonView 연동
1. SubAgreementId 전달
2. TasksDb Repository 연동
3. **확인**: 업무 목록 로드, 선택 버튼 동작

### Test 6: 반응형 UI
1. 브라우저 창 크기 조정 (모바일 사이즈)
2. **확인**: 3단 컬럼이 세로로 정렬

---

## 7. 완료 조건

- [ ] 8개 컴포넌트 파일 생성 (razor + razor.cs)
- [ ] 폴더 구조 생성 (SubAgreement/ViewPage/Modal/CommonView)
- [ ] 빌드 성공 (경고 0개, 오류 0개)
- [ ] Entity 필드명 정확히 사용 (Report_SubItem_*)
- [ ] Repository 메서드 정확히 사용 (Sid, long uid)
- [ ] TasksDb 연동 (ReportTaskListCommonView)
- [ ] Test 1-6 모두 통과
- [ ] 2025년 프로젝트와 UI/UX 일치

---

## 8. 주의사항

### ❌ 절대 사용 금지
- `GetCountByUidAsync()` - 제거된 메서드
- `DeleteAllByUidAsync()` - 제거된 메서드
- `SAid` - 잘못된 PK 필드명 (Sid 사용)
- `string userId` - 잘못된 파라미터 타입

### ✅ 반드시 사용
- `Sid` - PK 필드명
- `GetByUserIdAllAsync(long uid)` - 25년 메서드 패턴
- `Report_SubItem_Name`, `Report_SubItem_Proportion` - 세부직무 필드
- `Task_Number` - 업무 연계 필드
- `long uid` - 파라미터 타입

### SubAgreement 특성
- Agreement의 하위 직무 상세 평가
- 1개 Agreement → N개 SubAgreement 관계
- TasksDb와 연계 (Task_Number 필드)
- 세부직무 비중 합계 검증 필요 (100%)

### 참조 문서
- [20260203_13_components_agreement_v2.md](20260203_13_components_agreement_v2.md) - Agreement 컴포넌트
- [20260203_13_REWRITE_GUIDE.md](20260203_13_REWRITE_GUIDE.md) - 재작성 가이드
- [20260203_11_fix_entity_db_field_names.md](20260203_11_fix_entity_db_field_names.md) - Entity 변경 내역

---

**작성일**: 2026-02-04
**담당**: Claude Sonnet 4.5
**관련 이슈**: [#009](../issues/009_phase3_webapp_development.md)
**다음 작업지시서**: 20260203_15_components_report_v2.md (17개 컴포넌트)
