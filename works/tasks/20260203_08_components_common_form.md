# 작업지시서: 공통 및 Form 컴포넌트 구현

**날짜**: 2026-02-03
**작업 타입**: 컴포넌트 구현
**예상 소요**: 2-3시간
**관련 이슈**: [#009](../issues/009_phase3_webapp_development.md)
**선행 작업**: [20260203_07_components_report.md](20260203_07_components_report.md)

---

## 1. 작업 개요

### 배경
- Agreement, SubAgreement, Report 컴포넌트 구현 완료
- 공통 컴포넌트 및 Form 컴포넌트 구현 필요

### 목표
여러 페이지에서 공통으로 사용하는 컴포넌트와 Form 컴포넌트를 2026년 프로젝트에 구현

### 구현 범위
1. **CommonComponents** (1개)
   - CheckboxComponent: 체크박스 컴포넌트

2. **FormComponents** (6개)
   - FormAgreeTask: 직무 협의 폼
   - FormAgreeTaskCreate: 직무 생성 폼
   - FormGroup: 폼 그룹
   - FormSelectList: 선택 리스트
   - FormSelectNumber: 숫자 선택
   - FormTaskItem: 업무 항목 폼

3. **Table 컴포넌트** (2개)
   - ObjectiveListTable: 부서 목표 목록 테이블
   - EDeptListTable: 부서 목록 테이블

---

## 2. 컴포넌트 구현

### 2.1. CheckboxComponent.razor

**경로**: `MdcHR26Apps.BlazorServer/Components/Pages/Components/CommonComponents/CheckboxComponent.razor`

**기능**: 재사용 가능한 체크박스 컴포넌트

**Parameters**:
```csharp
[Parameter] public bool IsChecked { get; set; }
[Parameter] public string Label { get; set; } = string.Empty;
[Parameter] public EventCallback<bool> OnChanged { get; set; }
[Parameter] public bool Disabled { get; set; } = false;
```

**코드**:
```razor
<div class="form-check">
    <input class="form-check-input"
           type="checkbox"
           checked="@IsChecked"
           disabled="@Disabled"
           @onchange="@(e => OnChanged.InvokeAsync((bool)e.Value!))" />
    @if (!string.IsNullOrEmpty(Label))
    {
        <label class="form-check-label">
            @Label
        </label>
    }
</div>
```

---

### 2.2. FormAgreeTask.razor

**경로**: `MdcHR26Apps.BlazorServer/Components/Pages/Components/FormComponents/FormAgreeTask.razor`

**기능**: 직무 협의 항목 표시 (읽기 전용)

**Parameters**:
```csharp
[Parameter] public int ItemNumber { get; set; }
[Parameter] public string ItemTitle { get; set; } = string.Empty;
[Parameter] public string ItemContents { get; set; } = string.Empty;
[Parameter] public int ItemProportion { get; set; }
```

**코드**:
```razor
<div class="card mb-3">
    <div class="card-header bg-light">
        <strong>직무 @ItemNumber</strong>
    </div>
    <div class="card-body">
        <div class="row mb-2">
            <div class="col-md-4">
                <label class="form-label"><strong>직무 제목:</strong></label>
                <p>@ItemTitle</p>
            </div>
            <div class="col-md-4">
                <label class="form-label"><strong>직무 비중:</strong></label>
                <p>@ItemProportion %</p>
            </div>
        </div>
        <div class="row">
            <div class="col-md-12">
                <label class="form-label"><strong>직무 내용:</strong></label>
                <p style="white-space: pre-wrap;">@ItemContents</p>
            </div>
        </div>
    </div>
</div>
```

---

### 2.3. FormAgreeTaskCreate.razor

**경로**: `MdcHR26Apps.BlazorServer/Components/Pages/Components/FormComponents/FormAgreeTaskCreate.razor`

**기능**: 직무 생성/수정 폼

**Parameters**:
```csharp
[Parameter] public int ItemNumber { get; set; }
[Parameter] public string ItemTitle { get; set; } = string.Empty;
[Parameter] public string ItemContents { get; set; } = string.Empty;
[Parameter] public int ItemProportion { get; set; }
[Parameter] public EventCallback<string> OnItemTitleChanged { get; set; }
[Parameter] public EventCallback<string> OnItemContentsChanged { get; set; }
[Parameter] public EventCallback<int> OnItemProportionChanged { get; set; }
```

**코드**:
```razor
<div class="card mb-3">
    <div class="card-header bg-primary text-white">
        <strong>직무 @ItemNumber</strong>
    </div>
    <div class="card-body">
        <div class="mb-3">
            <label class="form-label">직무 제목 (필수)</label>
            <input type="text"
                   class="form-control"
                   value="@ItemTitle"
                   @oninput="@(e => OnItemTitleChanged.InvokeAsync(e.Value?.ToString() ?? string.Empty))"
                   maxlength="200"
                   required />
        </div>

        <div class="mb-3">
            <label class="form-label">직무 내용 (필수)</label>
            <textarea class="form-control"
                      rows="5"
                      value="@ItemContents"
                      @oninput="@(e => OnItemContentsChanged.InvokeAsync(e.Value?.ToString() ?? string.Empty))"
                      maxlength="2000"
                      required></textarea>
        </div>

        <div class="mb-3">
            <label class="form-label">직무 비중 (%) (필수)</label>
            <input type="number"
                   class="form-control"
                   value="@ItemProportion"
                   @oninput="@(e => OnItemProportionChanged.InvokeAsync(int.TryParse(e.Value?.ToString(), out var val) ? val : 0))"
                   min="0"
                   max="100"
                   required />
        </div>
    </div>
</div>
```

---

### 2.4. FormGroup.razor

**경로**: `MdcHR26Apps.BlazorServer/Components/Pages/Components/FormComponents/FormGroup.razor`

**기능**: Bootstrap Form Group 래퍼

**Parameters**:
```csharp
[Parameter] public string Label { get; set; } = string.Empty;
[Parameter] public bool Required { get; set; } = false;
[Parameter] public RenderFragment? ChildContent { get; set; }
```

**코드**:
```razor
<div class="mb-3">
    @if (!string.IsNullOrEmpty(Label))
    {
        <label class="form-label">
            @Label
            @if (Required)
            {
                <span class="text-danger">*</span>
            }
        </label>
    }
    @ChildContent
</div>
```

---

### 2.5. FormSelectList.razor

**경로**: `MdcHR26Apps.BlazorServer/Components/Pages/Components/FormComponents/FormSelectList.razor`

**기능**: 선택 리스트 (드롭다운)

**Parameters**:
```csharp
[Parameter] public string Label { get; set; } = string.Empty;
[Parameter] public List<SelectOption> Options { get; set; } = new();
[Parameter] public long SelectedValue { get; set; }
[Parameter] public EventCallback<long> OnSelectedChanged { get; set; }
[Parameter] public bool Required { get; set; } = false;
```

**Model**:
```csharp
// SelectOption.cs (로컬 모델)
public class SelectOption
{
    public long Value { get; set; }
    public string Text { get; set; } = string.Empty;
}
```

**코드**:
```razor
<div class="mb-3">
    @if (!string.IsNullOrEmpty(Label))
    {
        <label class="form-label">
            @Label
            @if (Required)
            {
                <span class="text-danger">*</span>
            }
        </label>
    }
    <select class="form-select"
            value="@SelectedValue"
            @onchange="@(e => OnSelectedChanged.InvokeAsync(long.Parse(e.Value?.ToString() ?? "0")))"
            required="@Required">
        <option value="0">-- 선택하세요 --</option>
        @foreach (var option in Options)
        {
            <option value="@option.Value">@option.Text</option>
        }
    </select>
</div>
```

---

### 2.6. FormSelectNumber.razor

**경로**: `MdcHR26Apps.BlazorServer/Components/Pages/Components/FormComponents/FormSelectNumber.razor`

**기능**: 숫자 선택 드롭다운 (1~10 등)

**Parameters**:
```csharp
[Parameter] public string Label { get; set; } = string.Empty;
[Parameter] public int MinValue { get; set; } = 1;
[Parameter] public int MaxValue { get; set; } = 10;
[Parameter] public int SelectedValue { get; set; }
[Parameter] public EventCallback<int> OnSelectedChanged { get; set; }
```

**코드**:
```razor
<div class="mb-3">
    @if (!string.IsNullOrEmpty(Label))
    {
        <label class="form-label">@Label</label>
    }
    <select class="form-select"
            value="@SelectedValue"
            @onchange="@(e => OnSelectedChanged.InvokeAsync(int.Parse(e.Value?.ToString() ?? "0")))">
        <option value="0">-- 선택 --</option>
        @for (int i = MinValue; i <= MaxValue; i++)
        {
            <option value="@i">@i</option>
        }
    </select>
</div>
```

---

### 2.7. FormTaskItem.razor

**경로**: `MdcHR26Apps.BlazorServer/Components/Pages/Components/FormComponents/FormTaskItem.razor`

**기능**: 업무 항목 폼 (TasksDb 입력용)

**Parameters**:
```csharp
[Parameter] public string TaskName { get; set; } = string.Empty;
[Parameter] public string TaskObjective { get; set; } = string.Empty;
[Parameter] public int TargetProportion { get; set; }
[Parameter] public int ResultProportion { get; set; }
[Parameter] public EventCallback<string> OnTaskNameChanged { get; set; }
[Parameter] public EventCallback<string> OnTaskObjectiveChanged { get; set; }
[Parameter] public EventCallback<int> OnTargetProportionChanged { get; set; }
[Parameter] public EventCallback<int> OnResultProportionChanged { get; set; }
```

**코드**:
```razor
<div class="card mb-3">
    <div class="card-body">
        <div class="row">
            <div class="col-md-6 mb-3">
                <label class="form-label">업무명 <span class="text-danger">*</span></label>
                <input type="text"
                       class="form-control"
                       value="@TaskName"
                       @oninput="@(e => OnTaskNameChanged.InvokeAsync(e.Value?.ToString() ?? string.Empty))"
                       maxlength="200"
                       required />
            </div>

            <div class="col-md-3 mb-3">
                <label class="form-label">목표 비중 (%)</label>
                <input type="number"
                       class="form-control"
                       value="@TargetProportion"
                       @oninput="@(e => OnTargetProportionChanged.InvokeAsync(int.TryParse(e.Value?.ToString(), out var val) ? val : 0))"
                       min="0"
                       max="100" />
            </div>

            <div class="col-md-3 mb-3">
                <label class="form-label">결과 비중 (%)</label>
                <input type="number"
                       class="form-control"
                       value="@ResultProportion"
                       @oninput="@(e => OnResultProportionChanged.InvokeAsync(int.TryParse(e.Value?.ToString(), out var val) ? val : 0))"
                       min="0"
                       max="100" />
            </div>
        </div>

        <div class="mb-3">
            <label class="form-label">업무 목표 <span class="text-danger">*</span></label>
            <textarea class="form-control"
                      rows="4"
                      value="@TaskObjective"
                      @oninput="@(e => OnTaskObjectiveChanged.InvokeAsync(e.Value?.ToString() ?? string.Empty))"
                      maxlength="2000"
                      required></textarea>
        </div>
    </div>
</div>
```

---

### 2.8. ObjectiveListTable.razor

**경로**: `MdcHR26Apps.BlazorServer/Components/Pages/Components/Table/ObjectiveListTable.razor`

**기능**: 부서 목표 목록 테이블 (DeptObjectiveDb)

**Parameters**:
```csharp
[Parameter] public List<DeptObjectiveWithDept> objectives { get; set; } = new();
[Parameter] public EventCallback<long> OnEdit { get; set; }
[Parameter] public EventCallback<long> OnDelete { get; set; }
[Parameter] public EventCallback<long> OnDetails { get; set; }
```

**Model**:
```csharp
// DeptObjectiveWithDept.cs (로컬 모델)
public class DeptObjectiveWithDept
{
    public long DeptObjectiveDbId { get; set; }
    public long EDepartId { get; set; }
    public string EDepartmentName { get; set; } = string.Empty;
    public string ObjectiveTitle { get; set; } = string.Empty;
    public string ObjectiveContents { get; set; } = string.Empty;
    public string Remarks { get; set; } = string.Empty;
}
```

**코드**:
```razor
<table class="table table-hover table-striped">
    <thead>
        <tr>
            <th>#</th>
            <th>부서</th>
            <th>목표 제목</th>
            <th>목표 내용</th>
            <th>비고</th>
            <th>작업</th>
        </tr>
    </thead>
    <tbody>
        @{ int sortNo = 1; }
        @foreach (var obj in objectives)
        {
            <tr>
                <td>@sortNo</td>
                <td>@obj.EDepartmentName</td>
                <td>@obj.ObjectiveTitle</td>
                <td class="text-truncate" style="max-width: 300px;">@obj.ObjectiveContents</td>
                <td>@obj.Remarks</td>
                <td>
                    <button class="btn btn-sm btn-primary me-1" @onclick="@(() => OnEdit.InvokeAsync(obj.DeptObjectiveDbId))">수정</button>
                    <button class="btn btn-sm btn-info me-1" @onclick="@(() => OnDetails.InvokeAsync(obj.DeptObjectiveDbId))">상세</button>
                    <button class="btn btn-sm btn-danger" @onclick="@(() => OnDelete.InvokeAsync(obj.DeptObjectiveDbId))">삭제</button>
                </td>
            </tr>
            sortNo++;
        }
    </tbody>
</table>
```

---

### 2.9. EDeptListTable.razor

**경로**: `MdcHR26Apps.BlazorServer/Components/Pages/Components/Table/EDeptListTable.razor`

**기능**: 부서 목록 테이블 (EDepartmentDb)

**Parameters**:
```csharp
[Parameter] public List<EDepartmentDb> departments { get; set; } = new();
[Parameter] public EventCallback<long> OnSelect { get; set; }
```

**코드**:
```razor
@using MdcHR26Apps.Models.EDepartmentDb

<table class="table table-hover table-bordered">
    <thead>
        <tr>
            <th>#</th>
            <th>부서명</th>
            <th>작업</th>
        </tr>
    </thead>
    <tbody>
        @{ int sortNo = 1; }
        @foreach (var dept in departments)
        {
            <tr>
                <td>@sortNo</td>
                <td>@dept.EDepartmentName</td>
                <td>
                    <button class="btn btn-sm btn-primary" @onclick="@(() => OnSelect.InvokeAsync(dept.EDepartId))">선택</button>
                </td>
            </tr>
            sortNo++;
        }
    </tbody>
</table>
```

---

## 3. 로컬 모델

**경로**: `MdcHR26Apps.BlazorServer/Models/`

### SelectOption.cs
```csharp
namespace MdcHR26Apps.BlazorServer.Models;

public class SelectOption
{
    public long Value { get; set; }
    public string Text { get; set; } = string.Empty;
}
```

### DeptObjectiveWithDept.cs
```csharp
namespace MdcHR26Apps.BlazorServer.Models;

public class DeptObjectiveWithDept
{
    public long DeptObjectiveDbId { get; set; }
    public long EDepartId { get; set; }
    public string EDepartmentName { get; set; } = string.Empty;
    public string ObjectiveTitle { get; set; } = string.Empty;
    public string ObjectiveContents { get; set; } = string.Empty;
    public string Remarks { get; set; } = string.Empty;
}
```

---

## 4. 폴더 구조

```
MdcHR26Apps.BlazorServer/
├── Components/
│   └── Pages/
│       └── Components/
│           ├── CommonComponents/
│           │   └── CheckboxComponent.razor
│           ├── FormComponents/
│           │   ├── FormAgreeTask.razor
│           │   ├── FormAgreeTaskCreate.razor
│           │   ├── FormGroup.razor
│           │   ├── FormSelectList.razor
│           │   ├── FormSelectNumber.razor
│           │   └── FormTaskItem.razor
│           └── Table/
│               ├── ObjectiveListTable.razor
│               └── EDeptListTable.razor
└── Models/
    ├── SelectOption.cs
    └── DeptObjectiveWithDept.cs
```

---

## 5. 테스트 계획

### 테스트 시나리오 1: FormAgreeTaskCreate
1. FormAgreeTaskCreate 렌더링
2. 직무 제목 입력
3. **확인**: OnItemTitleChanged 이벤트 발생 ✅
4. 비중 입력 (0~100 범위)
5. **확인**: OnItemProportionChanged 이벤트 발생 ✅

### 테스트 시나리오 2: FormSelectList
1. Options 5개 설정
2. FormSelectList 렌더링
3. **확인**: "선택하세요" + 5개 옵션 표시 ✅
4. 3번째 옵션 선택
5. **확인**: OnSelectedChanged 이벤트 발생 ✅

### 테스트 시나리오 3: CheckboxComponent
1. IsChecked = false로 렌더링
2. 체크박스 클릭
3. **확인**: OnChanged 이벤트 발생 (true) ✅
4. Disabled = true 설정
5. **확인**: 체크박스 비활성화 ✅

---

## 6. 완료 조건

- [ ] CheckboxComponent.razor 완료
- [ ] FormAgreeTask.razor 완료
- [ ] FormAgreeTaskCreate.razor 완료
- [ ] FormGroup.razor 완료
- [ ] FormSelectList.razor 완료
- [ ] FormSelectNumber.razor 완료
- [ ] FormTaskItem.razor 완료
- [ ] ObjectiveListTable.razor 완료
- [ ] EDeptListTable.razor 완료
- [ ] 로컬 모델 2개 작성 완료
- [ ] 테스트 시나리오 1-3 성공
- [ ] 빌드 오류 0개

---

## 7. 전체 컴포넌트 구현 완료

### 완료된 작업지시서
1. ✅ [20260203_05_components_agreement.md](20260203_05_components_agreement.md) - 6개 컴포넌트
2. ✅ [20260203_06_components_subagreement.md](20260203_06_components_subagreement.md) - 8개 컴포넌트
3. ✅ [20260203_07_components_report.md](20260203_07_components_report.md) - 17개 컴포넌트
4. ✅ [20260203_08_components_common_form.md](20260203_08_components_common_form.md) - 9개 컴포넌트

**총 구현 컴포넌트**: 40개

---

## 8. 다음 단계

컴포넌트 구현이 완료되면 Phase 3-4 페이지 구현을 시작합니다:
1. [20260203_01_phase3_4_1_agreement_subagreement.md](20260203_01_phase3_4_1_agreement_subagreement.md) - 재작성 필요
2. [20260203_02_phase3_4_2_1st_hr_report.md](20260203_02_phase3_4_2_1st_hr_report.md) - 재작성 필요
3. [20260203_03_phase3_4_3_2nd_3rd_hr_report.md](20260203_03_phase3_4_3_2nd_3rd_hr_report.md) - 재작성 필요
4. [20260203_04_phase3_4_4_dept_objective.md](20260203_04_phase3_4_4_dept_objective.md) - 재작성 필요

---

## 9. 관련 문서

**이슈**: [#009: Phase 3 WebApp 개발](../issues/009_phase3_webapp_development.md)
**Phase 2 Model**:
- [DeptObjectiveDb](../../MdcHR26Apps.Models/DeptObjectiveDb/)
- [EDepartmentDb](../../MdcHR26Apps.Models/EDepartmentDb/)
**참조 프로젝트**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Components`
