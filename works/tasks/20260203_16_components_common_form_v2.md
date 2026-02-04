# 작업지시서: Phase 3-4 Common/Form 컴포넌트 구현 (9개)

**날짜**: 2026-02-04
**작업 유형**: 신규 기능 추가
**관련 이슈**: [#009: Phase 3 Blazor Server WebApp 개발](../issues/009_phase3_webapp_development.md)
**참조 가이드**: [20260203_13_REWRITE_GUIDE.md](20260203_13_REWRITE_GUIDE.md)
**선행 작업**: [20260203_15_components_report_v2.md](20260203_15_components_report_v2.md)

---

## 1. 작업 개요

**목적**:
- 공통 Form 컴포넌트 9개 구현
- SelectListModel, EvaluationLists, TasksDb, DeptObjectiveDb 연동
- 재사용 가능한 폼 입력 컴포넌트 제공

**배경**:
- Report 컴포넌트 작업지시서 완료 (15번)
- 평가 페이지에서 사용할 공통 Form 컴포넌트 필요
- 드롭다운, 체크박스, 테이블 등 재사용 UI 구현

---

## 2. 관련 Entity 및 Model 구조

### 2.1. SelectListModel (공통 모델)

**경로**: `MdcHR26Apps.Models\Common\SelectListModel.cs`

**필드 구조**:
```csharp
public class SelectListModel
{
    public string Value { get; set; } = string.Empty;      // 값 (ID)
    public string Text { get; set; } = string.Empty;        // 표시 텍스트
    public bool Selected { get; set; } = false;             // 선택 여부
    public bool Disabled { get; set; } = false;             // 비활성화 여부
    public string? Group { get; set; }                      // 그룹명 (선택사항)
}
```

**⚠️ 중요**: 2025년 프로젝트에서는 `SelectListNumber`/`SelectListName` 사용, 2026년은 `Value`/`Text` 사용

---

### 2.2. EvaluationLists Entity

**경로**: `MdcHR26Apps.Models\EvaluationLists\EvaluationLists.cs`

**필드 구조**:
```csharp
public class EvaluationLists
{
    public Int64 Eid { get; set; }                          // PK
    public int Evaluation_Department_Number { get; set; }
    public string Evaluation_Department_Name { get; set; }
    public int Evaluation_Index_Number { get; set; }
    public string Evaluation_Index_Name { get; set; }
    public int Evaluation_Task_Number { get; set; }
    public string Evaluation_Task_Name { get; set; }
    public string? Evaluation_Lists_Remark { get; set; }
}
```

---

### 2.3. DeptObjectiveDb Entity

**경로**: `MdcHR26Apps.Models\DeptObjective\DeptObjectiveDb.cs`

**필드 구조**:
```csharp
public class DeptObjectiveDb
{
    public Int64 DeptObjectiveDbId { get; set; }           // PK
    public Int64 EDepartId { get; set; }                   // FK (EDepartmentDb)
    public string ObjectiveTitle { get; set; }
    public string ObjectiveContents { get; set; }
    public Int64 CreatedBy { get; set; }                   // 감사 필드
    public DateTime CreatedAt { get; set; }
    public Int64? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? Remarks { get; set; }
}
```

---

## 3. 구현할 컴포넌트 (9개)

### 카테고리별 분류

#### A. 공통 컴포넌트 (1개)
1. CheckboxComponent - 체크박스 (읽기 전용/활성화)

#### B. Form 입력 컴포넌트 (6개)
2. FormAgreeTask - 직무 평가 폼
3. FormAgreeTaskCreate - 직무 평가 생성 폼
4. FormGroup - 폼 그룹 (라벨 + 입력)
5. FormSelectList - 드롭다운 선택 (SelectListModel)
6. FormSelectNumber - 숫자 드롭다운
7. FormTaskItem - 업무 항목 폼

#### C. 테이블 컴포넌트 (2개)
8. ObjectiveListTable - 부서 목표 목록 테이블
9. EDeptListTable - 평가 부서 목록 테이블

---

## 4. 컴포넌트 상세 구현

### 4.1. CheckboxComponent.razor

**경로**: `MdcHR26Apps.BlazorServer\Components\Common\CheckboxComponent.razor`

**목적**: 체크박스 (읽기 전용 또는 활성화)

**주요 기능**:
- IsChecked 파라미터로 체크 상태 제어
- IsDisabled 파라미터로 비활성화 제어

**2025년 참조**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Components\CommonComponents\CheckboxComponent.razor`

**구현 내용**:
```razor
<input class="form-check-input"
       type="checkbox"
       checked="@IsChecked"
       disabled="@IsDisabled"
       @onchange="HandleChange" />

@code {
    [Parameter] public bool IsChecked { get; set; }
    [Parameter] public bool IsDisabled { get; set; } = false;
    [Parameter] public EventCallback<bool> OnChange { get; set; }

    private async Task HandleChange(ChangeEventArgs e)
    {
        if (e.Value is bool value)
        {
            IsChecked = value;
            await OnChange.InvokeAsync(value);
        }
    }
}
```

---

### 4.2. FormAgreeTask.razor

**경로**: `MdcHR26Apps.BlazorServer\Components\Form\FormAgreeTask.razor`

**목적**: 직무 평가 폼 (평가지표, 평가직무, 비중)

**주요 기능**:
- AgreementDb 데이터 바인딩
- 평가 항목 입력 (Report_Item_Name_1, Report_Item_Name_2, Report_Item_Proportion)

**2025년 참조**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Components\FormComponents\FormAgreeTask.razor`

**구현 내용**:
```razor
<div class="mb-3">
    <label class="form-label">평가지표</label>
    <InputText class="form-control" @bind-Value="agreement.Report_Item_Name_1" />
</div>

<div class="mb-3">
    <label class="form-label">평가직무</label>
    <InputText class="form-control" @bind-Value="agreement.Report_Item_Name_2" />
</div>

<div class="mb-3">
    <label class="form-label">직무비중 (%)</label>
    <InputNumber class="form-control" @bind-Value="agreement.Report_Item_Proportion" />
</div>

@code {
    [Parameter] public AgreementDb agreement { get; set; } = new();
    [Parameter] public EventCallback<AgreementDb> OnChange { get; set; }
}
```

---

### 4.3. FormAgreeTaskCreate.razor

**경로**: `MdcHR26Apps.BlazorServer\Components\Form\FormAgreeTaskCreate.razor`

**목적**: 직무 평가 생성 폼 (초기 입력)

**주요 기능**:
- 새로운 AgreementDb 생성용 폼
- 유효성 검사 포함

**구현 내용**:
```razor
<EditForm Model="@newAgreement" OnValidSubmit="HandleValidSubmit">
    <DataAnnotationsValidator />
    <ValidationSummary />

    <div class="mb-3">
        <label class="form-label">평가지표</label>
        <InputText class="form-control" @bind-Value="newAgreement.Report_Item_Name_1" />
        <ValidationMessage For="@(() => newAgreement.Report_Item_Name_1)" />
    </div>

    <div class="mb-3">
        <label class="form-label">평가직무</label>
        <InputText class="form-control" @bind-Value="newAgreement.Report_Item_Name_2" />
        <ValidationMessage For="@(() => newAgreement.Report_Item_Name_2)" />
    </div>

    <div class="mb-3">
        <label class="form-label">직무비중 (%)</label>
        <InputNumber class="form-control" @bind-Value="newAgreement.Report_Item_Proportion" />
        <ValidationMessage For="@(() => newAgreement.Report_Item_Proportion)" />
    </div>

    <button type="submit" class="btn btn-primary">생성</button>
</EditForm>

@code {
    [Parameter] public EventCallback<AgreementDb> OnSubmit { get; set; }

    private AgreementDb newAgreement = new();

    private async Task HandleValidSubmit()
    {
        await OnSubmit.InvokeAsync(newAgreement);
        newAgreement = new(); // 초기화
    }
}
```

---

### 4.4. FormGroup.razor

**경로**: `MdcHR26Apps.BlazorServer\Components\Form\FormGroup.razor`

**목적**: 폼 그룹 (라벨 + 입력 필드)

**주요 기능**:
- 재사용 가능한 폼 그룹 컴포넌트
- 라벨, 입력, 유효성 메시지 통합

**구현 내용**:
```razor
<div class="mb-3">
    @if (!string.IsNullOrEmpty(Label))
    {
        <label for="@Id" class="form-label">@Label</label>
    }

    <div class="input-group">
        @if (!string.IsNullOrEmpty(PrependText))
        {
            <span class="input-group-text">@PrependText</span>
        }

        @ChildContent

        @if (!string.IsNullOrEmpty(AppendText))
        {
            <span class="input-group-text">@AppendText</span>
        }
    </div>

    @if (!string.IsNullOrEmpty(HelpText))
    {
        <div class="form-text">@HelpText</div>
    }
</div>

@code {
    [Parameter] public string? Id { get; set; }
    [Parameter] public string? Label { get; set; }
    [Parameter] public string? PrependText { get; set; }
    [Parameter] public string? AppendText { get; set; }
    [Parameter] public string? HelpText { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
}
```

---

### 4.5. FormSelectList.razor ⭐

**경로**: `MdcHR26Apps.BlazorServer\Components\Form\FormSelectList.razor`

**목적**: 드롭다운 선택 (SelectListModel 사용)

**주요 기능**:
- SelectListModel 리스트 바인딩
- Value/Text 속성 사용 (⚠️ SelectListNumber/SelectListName 아님)
- 선택 이벤트 처리

**⚠️ 중요**: 2025년 프로젝트는 `item.SelectListName` 사용, 2026년은 `item.Text` 사용

**2025년 참조**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Components\FormComponents\FormSelectList.razor`

**구현 내용**:
```razor
@if (IsDisabled)
{
    <div class="input-group mb-3">
        <span class="input-group-text w-20">@Label</span>
        <InputText class="form-control" @bind-Value="BindValue" disabled />
    </div>
}
else
{
    @if (!string.IsNullOrWhiteSpace(Label))
    {
        <label for="@Id" class="form-label">@Label 선택</label>
        <div class="input-group mb-3">
            <span class="input-group-text w-20">@Label</span>
            <InputSelect id="@Id" class="form-select" @bind-Value="BindValue" @oninput="OnInput">
                @if (string.IsNullOrEmpty(BindValue))
                {
                    <option value="" selected disabled>@Label 를 선택하세요</option>
                }
                @foreach (var item in list)
                {
                    <option value="@item.Text">@item.Text</option>
                }
            </InputSelect>
        </div>
    }
}

@code {
    [Parameter] public string? Id { get; set; }
    [Parameter] public string? Label { get; set; }
    [Parameter] public string? BindValue { get; set; }
    [Parameter] public List<SelectListModel> list { get; set; } = new();
    [Parameter] public bool IsDisabled { get; set; } = false;
    [Parameter] public EventCallback<string> OnInput { get; set; }
}
```

---

### 4.6. FormSelectNumber.razor

**경로**: `MdcHR26Apps.BlazorServer\Components\Form\FormSelectNumber.razor`

**목적**: 숫자 드롭다운 (1-10, 비율 등)

**주요 기능**:
- 숫자 범위 선택
- Min, Max, Step 파라미터

**구현 내용**:
```razor
<div class="input-group mb-3">
    <span class="input-group-text">@Label</span>
    <InputSelect class="form-select" @bind-Value="SelectedValue">
        <option value="">선택하세요</option>
        @for (int i = Min; i <= Max; i += Step)
        {
            <option value="@i">@i @Unit</option>
        }
    </InputSelect>
</div>

@code {
    [Parameter] public string Label { get; set; } = "숫자";
    [Parameter] public int Min { get; set; } = 1;
    [Parameter] public int Max { get; set; } = 10;
    [Parameter] public int Step { get; set; } = 1;
    [Parameter] public string Unit { get; set; } = "";
    [Parameter] public int SelectedValue { get; set; }
    [Parameter] public EventCallback<int> OnChange { get; set; }
}
```

---

### 4.7. FormTaskItem.razor

**경로**: `MdcHR26Apps.BlazorServer\Components\Form\FormTaskItem.razor`

**목적**: 업무 항목 폼 (TasksDb 입력)

**주요 기능**:
- TasksDb 데이터 바인딩
- 업무명, 목표, 비중 입력

**구현 내용**:
```razor
<div class="card mb-3">
    <div class="card-body">
        <div class="mb-3">
            <label class="form-label">업무명</label>
            <InputText class="form-control" @bind-Value="task.TaskName" />
        </div>

        <div class="mb-3">
            <label class="form-label">업무 목표</label>
            <InputTextArea class="form-control" @bind-Value="task.TaskObjective" rows="3" />
        </div>

        <div class="mb-3">
            <label class="form-label">목표 비중 (%)</label>
            <InputNumber class="form-control" @bind-Value="task.TargetProportion" />
        </div>

        <div class="mb-3">
            <label class="form-label">목표 날짜</label>
            <InputDate class="form-control" @bind-Value="task.TargetDate" />
        </div>
    </div>
</div>

@code {
    [Parameter] public TasksDb task { get; set; } = new();
    [Parameter] public EventCallback<TasksDb> OnChange { get; set; }
}
```

---

### 4.8. ObjectiveListTable.razor

**경로**: `MdcHR26Apps.BlazorServer\Components\Common\ObjectiveListTable.razor`

**목적**: 부서 목표 목록 테이블

**주요 기능**:
- DeptObjectiveDb 목록 표시
- 생성일, 수정일 표시
- 편집/삭제 버튼

**2025년 참조**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Components\ObjectiveListTable.razor`

**구현 내용**:
```razor
<table class="table table-hover">
    <thead>
        <tr>
            <th>번호</th>
            <th>부서</th>
            <th>목표 제목</th>
            <th>목표 내용</th>
            <th>작성일</th>
            <th>수정일</th>
            <th>관리</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var item in objectives)
        {
            <tr>
                <td>@item.DeptObjectiveDbId</td>
                <td>@item.EDepartment?.DeptName</td>
                <td>@item.ObjectiveTitle</td>
                <td>@item.ObjectiveContents</td>
                <td>@item.CreatedAt.ToString("yyyy-MM-dd")</td>
                <td>@(item.UpdatedAt?.ToString("yyyy-MM-dd") ?? "-")</td>
                <td>
                    <button class="btn btn-sm btn-primary" @onclick="@(() => OnEdit(item.DeptObjectiveDbId))">편집</button>
                    <button class="btn btn-sm btn-danger" @onclick="@(() => OnDelete(item.DeptObjectiveDbId))">삭제</button>
                </td>
            </tr>
        }
    </tbody>
</table>

@code {
    [Parameter] public List<DeptObjectiveDb> objectives { get; set; } = new();
    [Parameter] public EventCallback<long> OnEdit { get; set; }
    [Parameter] public EventCallback<long> OnDelete { get; set; }
}
```

---

### 4.9. EDeptListTable.razor

**경로**: `MdcHR26Apps.BlazorServer\Components\Common\EDeptListTable.razor`

**목적**: 평가 부서 목록 테이블

**주요 기능**:
- EvaluationLists 목록 표시
- 부서/지표/업무 분류 표시

**2025년 참조**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Components\EDeptListTable.razor`

**구현 내용**:
```razor
<table class="table table-hover table-sm">
    <thead>
        <tr>
            <th>번호</th>
            <th>부서명</th>
            <th>평가지표</th>
            <th>평가업무</th>
            <th>비고</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var item in evaluationLists)
        {
            <tr>
                <td>@item.Eid</td>
                <td>
                    (@item.Evaluation_Department_Number)
                    @item.Evaluation_Department_Name
                </td>
                <td>
                    (@item.Evaluation_Index_Number)
                    @item.Evaluation_Index_Name
                </td>
                <td>
                    (@item.Evaluation_Task_Number)
                    @item.Evaluation_Task_Name
                </td>
                <td>@item.Evaluation_Lists_Remark</td>
            </tr>
        }
    </tbody>
</table>

@code {
    [Parameter] public List<EvaluationLists> evaluationLists { get; set; } = new();
}
```

---

## 5. 폴더 구조

**⚠️ 중요**: Agreement/SubAgreement/Report와 동일한 통합 경로 사용

**생성할 폴더**:
```
MdcHR26Apps.BlazorServer/
└── Components/
    └── Pages/
        └── Components/
            ├── Common/                      ← 통합 경로
            │   ├── CheckboxComponent.razor
            │   ├── CheckboxComponent.razor.cs
            │   ├── ObjectiveListTable.razor
            │   ├── ObjectiveListTable.razor.cs
            │   ├── EDeptListTable.razor
            │   └── EDeptListTable.razor.cs
            └── Form/                        ← 통합 경로
                ├── FormAgreeTask.razor
                ├── FormAgreeTask.razor.cs
                ├── FormAgreeTaskCreate.razor
                ├── FormAgreeTaskCreate.razor.cs
                ├── FormGroup.razor
                ├── FormGroup.razor.cs
                ├── FormSelectList.razor
                ├── FormSelectList.razor.cs
                ├── FormSelectNumber.razor
                ├── FormSelectNumber.razor.cs
                ├── FormTaskItem.razor
                └── FormTaskItem.razor.cs
```

---

## 6. 공통 구현 사항

### 6.1. 네임스페이스
```csharp
// Common 컴포넌트
namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Common;

// Form 컴포넌트
namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Form;
```

**⚠️ 중요**: 컴포넌트는 통합 경로에 생성합니다:
- Common: `Components/Pages/Components/Common/`
- Form: `Components/Pages/Components/Form/`

### 6.2. 필수 using 구문
```csharp
@using MdcHR26Apps.Models.Common
@using MdcHR26Apps.Models.EvaluationLists
@using MdcHR26Apps.Models.DeptObjective
@using MdcHR26Apps.Models.EvaluationTasks
@using MdcHR26Apps.Models.EvaluationAgreement
```

### 6.3. SelectListModel 사용 패턴
```csharp
// ✅ 올바른 사용 (2026년)
var selectList = new List<SelectListModel>
{
    new SelectListModel { Value = "1", Text = "옵션1" },
    new SelectListModel { Value = "2", Text = "옵션2" }
};

// ❌ 잘못된 사용 (2025년 방식)
new SelectListModel { SelectListNumber = "1", SelectListName = "옵션1" }
```

---

## 7. 테스트 항목

개발자가 테스트할 항목:

### Test 1: 컴포넌트 빌드 확인
1. 프로젝트 빌드 실행
2. **확인**: 경고 0개, 오류 0개

### Test 2: FormSelectList 드롭다운
1. SelectListModel 리스트 전달
2. 드롭다운 선택
3. **확인**: Value/Text 속성 정상 사용, 선택 이벤트 발생

### Test 3: CheckboxComponent 상태
1. IsChecked true/false 전달
2. IsDisabled true 설정
3. **확인**: 체크 상태 정상, 비활성화 동작

### Test 4: FormAgreeTask 바인딩
1. AgreementDb 전달
2. 입력 필드 수정
3. **확인**: 양방향 바인딩 정상, Report_Item_* 필드 반영

### Test 5: ObjectiveListTable 렌더링
1. DeptObjectiveDb 리스트 전달
2. **확인**: 감사 필드 (CreatedAt, UpdatedAt) 정상 표시

### Test 6: EDeptListTable 렌더링
1. EvaluationLists 리스트 전달
2. **확인**: Evaluation_Department/Index/Task 필드 정상 표시

---

## 8. 완료 조건

- [ ] 9개 컴포넌트 파일 생성 (razor + razor.cs)
- [ ] 폴더 구조 생성 (Components/Pages/Components/Common/, Components/Pages/Components/Form/)
- [ ] 빌드 성공 (경고 0개, 오류 0개)
- [ ] SelectListModel Value/Text 속성 사용
- [ ] EvaluationLists Entity 필드명 정확히 사용
- [ ] DeptObjectiveDb 감사 필드 사용
- [ ] Test 1-6 모두 통과
- [ ] 2025년 프로젝트와 UI/UX 일치 (속성명만 변경)

---

## 9. 주의사항

### ❌ 절대 사용 금지
- `SelectListNumber` / `SelectListName` - 2025년 속성명 (2026년은 Value/Text 사용)
- `ELid` - 잘못된 PK 필드명 (Eid 사용)
- `DOid` - 잘못된 PK 필드명 (DeptObjectiveDbId 사용)

### ✅ 반드시 사용
- `SelectListModel.Value` / `SelectListModel.Text` - 2026년 속성명
- `Eid` - EvaluationLists PK
- `DeptObjectiveDbId` - DeptObjectiveDb PK
- `Evaluation_Department_Number/Name` - EvaluationLists 필드
- `CreatedBy`, `CreatedAt`, `UpdatedBy`, `UpdatedAt` - 감사 필드

### SelectListModel 변환 예시
```csharp
// Repository에서 SelectListModel 생성 시
var selectList = await evaluationListsRepository.GetDepartmentSelectListAsync();

// Repository 메서드 내부
return departments.Select(d => new SelectListModel
{
    Value = d.Evaluation_Department_Number.ToString(),  // ✅ Value
    Text = d.Evaluation_Department_Name                 // ✅ Text
}).ToList();
```

### 참조 문서
- [20260203_15_components_report_v2.md](20260203_15_components_report_v2.md) - Report 컴포넌트
- [20260203_13_REWRITE_GUIDE.md](20260203_13_REWRITE_GUIDE.md) - 재작성 가이드
- [20260203_12_fix_repository_based_on_2025.md](20260203_12_fix_repository_based_on_2025.md) - SelectListModel 변경 내역

---

**작성일**: 2026-02-04
**담당**: Claude Sonnet 4.5
**관련 이슈**: [#009](../issues/009_phase3_webapp_development.md)
**완료**: 4개 작업지시서 재작성 완료 (13-16번)
