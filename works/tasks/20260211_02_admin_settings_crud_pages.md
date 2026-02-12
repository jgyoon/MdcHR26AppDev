# 작업지시서: Admin/Settings CRUD 페이지 구현

**날짜**: 2026-02-11
**작업 유형**: 기능 추가
**관련 이슈**: 없음
**우선순위**: 높음

---

## 1. 작업 개요

### 1.1. 배경
- Admin/SettingManage 페이지는 이미 완성되어 있음 ✅
- 부서(Dept)와 직급(Rank) 탭 UI 구현 완료 ✅
- 하지만 Create/Edit/Delete/Details 페이지들이 빈 껍데기만 존재 ❌

### 1.2. 현재 상태
- **SettingManage.razor**: 완성 ✅
- **SettingManage.razor.cs**: 완성 ✅
- **Create 페이지들**: 빈 @code 블록만 존재 ❌
- **Edit 페이지들**: 빈 @code 블록만 존재 ❌
- **Delete 페이지들**: 빈 @code 블록만 존재 ❌
- **Details 페이지들**: 조회 기능만 있음, 수정/삭제 버튼 없음 ⚠️

### 1.3. 작업 범위
총 8개 페이지 (16개 파일):
1. **Depts/Create** - 부서 생성 페이지
2. **Depts/Edit** - 부서 수정 페이지
3. **Depts/Delete** - 부서 삭제 페이지
4. **Depts/Details** - 부서 상세 페이지 (수정/삭제 버튼 추가)
5. **Ranks/Create** - 직급 생성 페이지
6. **Ranks/Edit** - 직급 수정 페이지
7. **Ranks/Delete** - 직급 삭제 페이지
8. **Ranks/Details** - 직급 상세 페이지 (수정/삭제 버튼 추가)

---

## 2. Entity 구조

### 2.1. EDepartmentDb (부서)
```csharp
public class EDepartmentDb
{
    public Int64 EDepartId { get; set; }          // PK, IDENTITY
    public int EDepartmentNo { get; set; }        // 부서 번호 (정렬용, UNIQUE)
    public string EDepartmentName { get; set; }   // 부서명 (Required)
    public bool ActivateStatus { get; set; }      // 활성화 여부 (기본값: true)
    public string? Remarks { get; set; }          // 비고 (선택)
}
```

### 2.2. ERankDb (직급)
```csharp
public class ERankDb
{
    public Int64 ERankId { get; set; }            // PK, IDENTITY
    public int ERankNo { get; set; }              // 직급 번호 (정렬용, UNIQUE)
    public string ERankName { get; set; }         // 직급명 (Required)
    public bool ActivateStatus { get; set; }      // 활성화 여부 (기본값: true)
    public string? Remarks { get; set; }          // 비고 (선택)
}
```

---

## 3. 작업 단계

### Step 1: Depts/Create 페이지 구현

#### 파일 1: Create.razor
**위치**: `MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Depts/Create.razor`

**전체 코드**:
```razor
@page "/Admin/Settings/Depts/Create"
@rendermode InteractiveServer
@using MdcHR26Apps.BlazorServer.Components.Pages.Components.Form
@using MdcHR26Apps.BlazorServer.Components.Pages.Components.Common

<PageTitle>부서 등록</PageTitle>

<h3>부서 등록</h3>
<hr />

@if (model == null)
{
    <LoadingIndicator />
}
else
{
    <div class="row">
        <div class="col-md-12">
            <EditForm Model="model" OnValidSubmit="CreateDepartment">
                <DataAnnotationsValidator />
                <ValidationSummary />

                <FormInput Label="부서번호" @bind-Value="model.EDepartmentNo" Type="number" />
                <FormInput Label="부서명" @bind-Value="model.EDepartmentName" />
                <FormCheckbox Label="사용여부" @bind-Value="model.ActivateStatus" />
                <FormTextarea Label="비고" @bind-Value="model.Remarks" />

                <div class="input-group mb-1">
                    <button type="submit" class="btn btn-primary">저장</button>
                    <button type="button" class="btn btn-light" @onclick="MoveSettingManagePage">목록</button>
                </div>
            </EditForm>
        </div>
    </div>

    <DisplayResultText Comment="@resultText" />
}
```

#### 파일 2: Create.razor.cs
**위치**: `MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Depts/Create.razor.cs`

**전체 코드**:
```csharp
using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.Department;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin.Settings.Depts;

public partial class Create(
    IEDepartmentRepository eDepartmentRepository,
    LoginStatusService loginStatusService,
    UrlActions urlActions)
{
    private EDepartmentDb model { get; set; } = new EDepartmentDb();
    private string resultText { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await CheckLogined();
        await SetData();
        StateHasChanged();
    }

    private async Task SetData()
    {
        await Task.Delay(1);
        // 기본값 설정
        model.ActivateStatus = true;
    }

    #region Login Check
    private async Task CheckLogined()
    {
        await Task.Delay(0);
        if (!loginStatusService.IsloginAndIsAdminCheck())
        {
            StateHasChanged();
            urlActions.MoveMainPage();
        }
    }
    #endregion

    #region Create Department
    private async Task CreateDepartment()
    {
        try
        {
            // 필수값 검증
            if (string.IsNullOrWhiteSpace(model.EDepartmentName))
            {
                resultText = "부서명은 필수 입력 항목입니다.";
                return;
            }

            if (model.EDepartmentNo <= 0)
            {
                resultText = "부서번호는 1 이상이어야 합니다.";
                return;
            }

            var result = await eDepartmentRepository.AddAsync(model);
            if (result > 0)
            {
                resultText = "부서가 성공적으로 등록되었습니다.";
                StateHasChanged();
                await Task.Delay(1000);
                urlActions.MoveSettingManagePage();
            }
            else
            {
                resultText = "부서 등록에 실패했습니다.";
            }
        }
        catch (Exception ex)
        {
            resultText = $"오류가 발생했습니다: {ex.Message}";
        }
    }
    #endregion

    #region Page Navigation
    private void MoveSettingManagePage()
    {
        urlActions.MoveSettingManagePage();
    }
    #endregion
}
```

---

### Step 2: Depts/Edit 페이지 구현

#### 파일 1: Edit.razor
**위치**: `MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Depts/Edit.razor`

**전체 코드**:
```razor
@page "/Admin/Settings/Depts/Edit/{Id:long}"
@rendermode InteractiveServer
@using MdcHR26Apps.BlazorServer.Components.Pages.Components.Form
@using MdcHR26Apps.BlazorServer.Components.Pages.Components.Common

<PageTitle>부서 수정</PageTitle>

<h3>부서 수정</h3>
<hr />

@if (model == null)
{
    <LoadingIndicator />
}
else
{
    <div class="row">
        <div class="col-md-12">
            <EditForm Model="model" OnValidSubmit="UpdateDepartment">
                <DataAnnotationsValidator />
                <ValidationSummary />

                <FormInput Label="부서번호" @bind-Value="model.EDepartmentNo" Type="number" />
                <FormInput Label="부서명" @bind-Value="model.EDepartmentName" />
                <FormCheckbox Label="사용여부" @bind-Value="model.ActivateStatus" />
                <FormTextarea Label="비고" @bind-Value="model.Remarks" />

                <div class="input-group mb-1">
                    <button type="submit" class="btn btn-primary">수정</button>
                    <button type="button" class="btn btn-light" @onclick="MoveDetailsPage">취소</button>
                </div>
            </EditForm>
        </div>
    </div>

    <DisplayResultText Comment="@resultText" />
}
```

#### 파일 2: Edit.razor.cs
**위치**: `MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Depts/Edit.razor.cs`

**전체 코드**:
```csharp
using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.Department;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin.Settings.Depts;

public partial class Edit(
    IEDepartmentRepository eDepartmentRepository,
    LoginStatusService loginStatusService,
    UrlActions urlActions)
{
    #region Parameters
    [Parameter]
    public Int64 Id { get; set; }
    #endregion

    private EDepartmentDb model { get; set; } = new EDepartmentDb();
    private string resultText { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await CheckLogined();
        await SetData(Id);
        StateHasChanged();
    }

    private async Task SetData(Int64 id)
    {
        var result = await eDepartmentRepository.GetByIdAsync(id);
        if (result != null)
        {
            model = result;
        }
        else
        {
            resultText = "부서 정보를 찾을 수 없습니다.";
        }
    }

    #region Login Check
    private async Task CheckLogined()
    {
        await Task.Delay(0);
        if (!loginStatusService.IsloginAndIsAdminCheck())
        {
            StateHasChanged();
            urlActions.MoveMainPage();
        }
    }
    #endregion

    #region Update Department
    private async Task UpdateDepartment()
    {
        try
        {
            // 필수값 검증
            if (string.IsNullOrWhiteSpace(model.EDepartmentName))
            {
                resultText = "부서명은 필수 입력 항목입니다.";
                return;
            }

            if (model.EDepartmentNo <= 0)
            {
                resultText = "부서번호는 1 이상이어야 합니다.";
                return;
            }

            var result = await eDepartmentRepository.UpdateAsync(model);
            if (result > 0)
            {
                resultText = "부서가 성공적으로 수정되었습니다.";
                StateHasChanged();
                await Task.Delay(1000);
                MoveDetailsPage();
            }
            else
            {
                resultText = "부서 수정에 실패했습니다.";
            }
        }
        catch (Exception ex)
        {
            resultText = $"오류가 발생했습니다: {ex.Message}";
        }
    }
    #endregion

    #region Page Navigation
    private void MoveDetailsPage()
    {
        urlActions.MoveDeptDetailsPage(Id);
    }
    #endregion
}
```

---

### Step 3: Depts/Delete 페이지 구현

#### 파일 1: Delete.razor
**위치**: `MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Depts/Delete.razor`

**전체 코드**:
```razor
@page "/Admin/Settings/Depts/Delete/{Id:long}"
@rendermode InteractiveServer
@using MdcHR26Apps.BlazorServer.Components.Pages.Components.Common

<PageTitle>부서 삭제</PageTitle>

<h3>부서 삭제</h3>
<hr />

@if (model == null)
{
    <LoadingIndicator />
}
else
{
    <div class="alert alert-danger" role="alert">
        <h4>정말로 이 부서를 삭제하시겠습니까?</h4>
        <p>삭제된 데이터는 복구할 수 없습니다.</p>
    </div>

    <div class="row">
        <div class="col-md-12">
            <ul class="list-group">
                <li class="list-group-item">
                    <label>부서번호:</label> @model.EDepartmentNo
                </li>
                <li class="list-group-item">
                    <label>부서명:</label> @model.EDepartmentName
                </li>
                <li class="list-group-item">
                    <label>사용여부:</label> @(model.ActivateStatus ? "사용중" : "사용안함")
                </li>
                <li class="list-group-item">
                    <label>비고:</label> @(string.IsNullOrEmpty(model.Remarks) ? "-" : model.Remarks)
                </li>
            </ul>
        </div>
    </div>

    <hr />

    <div class="input-group mb-1">
        <button class="btn btn-danger" @onclick="DeleteDepartment">삭제</button>
        <button class="btn btn-light" @onclick="MoveDetailsPage">취소</button>
    </div>

    <DisplayResultText Comment="@resultText" />
}
```

#### 파일 2: Delete.razor.cs
**위치**: `MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Depts/Delete.razor.cs`

**전체 코드**:
```csharp
using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.Department;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin.Settings.Depts;

public partial class Delete(
    IEDepartmentRepository eDepartmentRepository,
    LoginStatusService loginStatusService,
    UrlActions urlActions)
{
    #region Parameters
    [Parameter]
    public Int64 Id { get; set; }
    #endregion

    private EDepartmentDb model { get; set; } = new EDepartmentDb();
    private string resultText { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await CheckLogined();
        await SetData(Id);
        StateHasChanged();
    }

    private async Task SetData(Int64 id)
    {
        var result = await eDepartmentRepository.GetByIdAsync(id);
        if (result != null)
        {
            model = result;
        }
        else
        {
            resultText = "부서 정보를 찾을 수 없습니다.";
        }
    }

    #region Login Check
    private async Task CheckLogined()
    {
        await Task.Delay(0);
        if (!loginStatusService.IsloginAndIsAdminCheck())
        {
            StateHasChanged();
            urlActions.MoveMainPage();
        }
    }
    #endregion

    #region Delete Department
    private async Task DeleteDepartment()
    {
        try
        {
            var result = await eDepartmentRepository.DeleteAsync(Id);
            if (result > 0)
            {
                resultText = "부서가 성공적으로 삭제되었습니다.";
                StateHasChanged();
                await Task.Delay(1000);
                urlActions.MoveSettingManagePage();
            }
            else
            {
                resultText = "부서 삭제에 실패했습니다.";
            }
        }
        catch (Exception ex)
        {
            resultText = $"오류가 발생했습니다: {ex.Message}";
        }
    }
    #endregion

    #region Page Navigation
    private void MoveDetailsPage()
    {
        urlActions.MoveDeptDetailsPage(Id);
    }
    #endregion
}
```

---

### Step 4: Depts/Details 페이지 수정 (수정/삭제 버튼 추가)

#### 파일 1: Details.razor (수정)
**위치**: `MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Depts/Details.razor`

**수정 내용**: Line 14-16 수정
```razor
<div class="col-md-12">
    <button class="btn btn-primary" @onclick="MoveEditPage">수정</button>
    <button class="btn btn-danger" @onclick="MoveDeletePage">삭제</button>
    <button class="btn btn-outline-primary" @onclick="MoveSettingManagePage">목록</button>
</div>
```

#### 파일 2: Details.razor.cs (수정)
**위치**: `MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Depts/Details.razor.cs`

**추가 내용**: Line 66 뒤에 추가
```csharp
private void MoveEditPage()
{
    urlActions.MoveDeptEditPage(Id);
}

private void MoveDeletePage()
{
    urlActions.MoveDeptDeletePage(Id);
}
```

---

### Step 5: Ranks/Create 페이지 구현

#### 파일 1: Create.razor
**위치**: `MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Ranks/Create.razor`

**전체 코드**:
```razor
@page "/Admin/Settings/Ranks/Create"
@rendermode InteractiveServer
@using MdcHR26Apps.BlazorServer.Components.Pages.Components.Form
@using MdcHR26Apps.BlazorServer.Components.Pages.Components.Common

<PageTitle>직급 등록</PageTitle>

<h3>직급 등록</h3>
<hr />

@if (model == null)
{
    <LoadingIndicator />
}
else
{
    <div class="row">
        <div class="col-md-12">
            <EditForm Model="model" OnValidSubmit="CreateRank">
                <DataAnnotationsValidator />
                <ValidationSummary />

                <FormInput Label="직급번호" @bind-Value="model.ERankNo" Type="number" />
                <FormInput Label="직급명" @bind-Value="model.ERankName" />
                <FormCheckbox Label="사용여부" @bind-Value="model.ActivateStatus" />
                <FormTextarea Label="비고" @bind-Value="model.Remarks" />

                <div class="input-group mb-1">
                    <button type="submit" class="btn btn-primary">저장</button>
                    <button type="button" class="btn btn-light" @onclick="MoveSettingManagePage">목록</button>
                </div>
            </EditForm>
        </div>
    </div>

    <DisplayResultText Comment="@resultText" />
}
```

#### 파일 2: Create.razor.cs
**위치**: `MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Ranks/Create.razor.cs`

**전체 코드**:
```csharp
using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.Rank;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin.Settings.Ranks;

public partial class Create(
    IERankRepository eRankRepository,
    LoginStatusService loginStatusService,
    UrlActions urlActions)
{
    private ERankDb model { get; set; } = new ERankDb();
    private string resultText { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await CheckLogined();
        await SetData();
        StateHasChanged();
    }

    private async Task SetData()
    {
        await Task.Delay(1);
        // 기본값 설정
        model.ActivateStatus = true;
    }

    #region Login Check
    private async Task CheckLogined()
    {
        await Task.Delay(0);
        if (!loginStatusService.IsloginAndIsAdminCheck())
        {
            StateHasChanged();
            urlActions.MoveMainPage();
        }
    }
    #endregion

    #region Create Rank
    private async Task CreateRank()
    {
        try
        {
            // 필수값 검증
            if (string.IsNullOrWhiteSpace(model.ERankName))
            {
                resultText = "직급명은 필수 입력 항목입니다.";
                return;
            }

            if (model.ERankNo <= 0)
            {
                resultText = "직급번호는 1 이상이어야 합니다.";
                return;
            }

            var result = await eRankRepository.AddAsync(model);
            if (result > 0)
            {
                resultText = "직급이 성공적으로 등록되었습니다.";
                StateHasChanged();
                await Task.Delay(1000);
                urlActions.MoveSettingManagePage();
            }
            else
            {
                resultText = "직급 등록에 실패했습니다.";
            }
        }
        catch (Exception ex)
        {
            resultText = $"오류가 발생했습니다: {ex.Message}";
        }
    }
    #endregion

    #region Page Navigation
    private void MoveSettingManagePage()
    {
        urlActions.MoveSettingManagePage();
    }
    #endregion
}
```

---

### Step 6: Ranks/Edit 페이지 구현

#### 파일 1: Edit.razor
**위치**: `MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Ranks/Edit.razor`

**전체 코드**:
```razor
@page "/Admin/Settings/Ranks/Edit/{Id:long}"
@rendermode InteractiveServer
@using MdcHR26Apps.BlazorServer.Components.Pages.Components.Form
@using MdcHR26Apps.BlazorServer.Components.Pages.Components.Common

<PageTitle>직급 수정</PageTitle>

<h3>직급 수정</h3>
<hr />

@if (model == null)
{
    <LoadingIndicator />
}
else
{
    <div class="row">
        <div class="col-md-12">
            <EditForm Model="model" OnValidSubmit="UpdateRank">
                <DataAnnotationsValidator />
                <ValidationSummary />

                <FormInput Label="직급번호" @bind-Value="model.ERankNo" Type="number" />
                <FormInput Label="직급명" @bind-Value="model.ERankName" />
                <FormCheckbox Label="사용여부" @bind-Value="model.ActivateStatus" />
                <FormTextarea Label="비고" @bind-Value="model.Remarks" />

                <div class="input-group mb-1">
                    <button type="submit" class="btn btn-primary">수정</button>
                    <button type="button" class="btn btn-light" @onclick="MoveDetailsPage">취소</button>
                </div>
            </EditForm>
        </div>
    </div>

    <DisplayResultText Comment="@resultText" />
}
```

#### 파일 2: Edit.razor.cs
**위치**: `MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Ranks/Edit.razor.cs`

**전체 코드**:
```csharp
using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.Rank;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin.Settings.Ranks;

public partial class Edit(
    IERankRepository eRankRepository,
    LoginStatusService loginStatusService,
    UrlActions urlActions)
{
    #region Parameters
    [Parameter]
    public Int64 Id { get; set; }
    #endregion

    private ERankDb model { get; set; } = new ERankDb();
    private string resultText { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await CheckLogined();
        await SetData(Id);
        StateHasChanged();
    }

    private async Task SetData(Int64 id)
    {
        var result = await eRankRepository.GetByIdAsync(id);
        if (result != null)
        {
            model = result;
        }
        else
        {
            resultText = "직급 정보를 찾을 수 없습니다.";
        }
    }

    #region Login Check
    private async Task CheckLogined()
    {
        await Task.Delay(0);
        if (!loginStatusService.IsloginAndIsAdminCheck())
        {
            StateHasChanged();
            urlActions.MoveMainPage();
        }
    }
    #endregion

    #region Update Rank
    private async Task UpdateRank()
    {
        try
        {
            // 필수값 검증
            if (string.IsNullOrWhiteSpace(model.ERankName))
            {
                resultText = "직급명은 필수 입력 항목입니다.";
                return;
            }

            if (model.ERankNo <= 0)
            {
                resultText = "직급번호는 1 이상이어야 합니다.";
                return;
            }

            var result = await eRankRepository.UpdateAsync(model);
            if (result > 0)
            {
                resultText = "직급이 성공적으로 수정되었습니다.";
                StateHasChanged();
                await Task.Delay(1000);
                MoveDetailsPage();
            }
            else
            {
                resultText = "직급 수정에 실패했습니다.";
            }
        }
        catch (Exception ex)
        {
            resultText = $"오류가 발생했습니다: {ex.Message}";
        }
    }
    #endregion

    #region Page Navigation
    private void MoveDetailsPage()
    {
        urlActions.MoveRankDetailsPage(Id);
    }
    #endregion
}
```

---

### Step 7: Ranks/Delete 페이지 구현

#### 파일 1: Delete.razor
**위치**: `MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Ranks/Delete.razor`

**전체 코드**:
```razor
@page "/Admin/Settings/Ranks/Delete/{Id:long}"
@rendermode InteractiveServer
@using MdcHR26Apps.BlazorServer.Components.Pages.Components.Common

<PageTitle>직급 삭제</PageTitle>

<h3>직급 삭제</h3>
<hr />

@if (model == null)
{
    <LoadingIndicator />
}
else
{
    <div class="alert alert-danger" role="alert">
        <h4>정말로 이 직급을 삭제하시겠습니까?</h4>
        <p>삭제된 데이터는 복구할 수 없습니다.</p>
    </div>

    <div class="row">
        <div class="col-md-12">
            <ul class="list-group">
                <li class="list-group-item">
                    <label>직급번호:</label> @model.ERankNo
                </li>
                <li class="list-group-item">
                    <label>직급명:</label> @model.ERankName
                </li>
                <li class="list-group-item">
                    <label>사용여부:</label> @(model.ActivateStatus ? "사용중" : "사용안함")
                </li>
                <li class="list-group-item">
                    <label>비고:</label> @(string.IsNullOrEmpty(model.Remarks) ? "-" : model.Remarks)
                </li>
            </ul>
        </div>
    </div>

    <hr />

    <div class="input-group mb-1">
        <button class="btn btn-danger" @onclick="DeleteRank">삭제</button>
        <button class="btn btn-light" @onclick="MoveDetailsPage">취소</button>
    </div>

    <DisplayResultText Comment="@resultText" />
}
```

#### 파일 2: Delete.razor.cs
**위치**: `MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Ranks/Delete.razor.cs`

**전체 코드**:
```csharp
using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.Rank;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin.Settings.Ranks;

public partial class Delete(
    IERankRepository eRankRepository,
    LoginStatusService loginStatusService,
    UrlActions urlActions)
{
    #region Parameters
    [Parameter]
    public Int64 Id { get; set; }
    #endregion

    private ERankDb model { get; set; } = new ERankDb();
    private string resultText { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await CheckLogined();
        await SetData(Id);
        StateHasChanged();
    }

    private async Task SetData(Int64 id)
    {
        var result = await eRankRepository.GetByIdAsync(id);
        if (result != null)
        {
            model = result;
        }
        else
        {
            resultText = "직급 정보를 찾을 수 없습니다.";
        }
    }

    #region Login Check
    private async Task CheckLogined()
    {
        await Task.Delay(0);
        if (!loginStatusService.IsloginAndIsAdminCheck())
        {
            StateHasChanged();
            urlActions.MoveMainPage();
        }
    }
    #endregion

    #region Delete Rank
    private async Task DeleteRank()
    {
        try
        {
            var result = await eRankRepository.DeleteAsync(Id);
            if (result > 0)
            {
                resultText = "직급이 성공적으로 삭제되었습니다.";
                StateHasChanged();
                await Task.Delay(1000);
                urlActions.MoveSettingManagePage();
            }
            else
            {
                resultText = "직급 삭제에 실패했습니다.";
            }
        }
        catch (Exception ex)
        {
            resultText = $"오류가 발생했습니다: {ex.Message}";
        }
    }
    #endregion

    #region Page Navigation
    private void MoveDetailsPage()
    {
        urlActions.MoveRankDetailsPage(Id);
    }
    #endregion
}
```

---

### Step 8: Ranks/Details 페이지 수정 (수정/삭제 버튼 추가)

#### 파일 1: Details.razor (수정)
**위치**: `MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Ranks/Details.razor`

**수정 내용**: Line 14-16 수정
```razor
<div class="col-md-12">
    <button class="btn btn-primary" @onclick="MoveEditPage">수정</button>
    <button class="btn btn-danger" @onclick="MoveDeletePage">삭제</button>
    <button class="btn btn-outline-primary" @onclick="MoveSettingManagePage">목록</button>
</div>
```

#### 파일 2: Details.razor.cs (수정)
**위치**: `MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Ranks/Details.razor.cs`

**추가 내용**: Line 66 뒤에 추가
```csharp
private void MoveEditPage()
{
    urlActions.MoveRankEditPage(Id);
}

private void MoveDeletePage()
{
    urlActions.MoveRankDeletePage(Id);
}
```

---

### Step 9: UrlActions 메서드 추가 확인

**위치**: `MdcHR26Apps.BlazorServer/Data/UrlActions.cs`

**확인 필요 메서드**:
```csharp
// Dept
public void MoveDeptEditPage(long deptId);
public void MoveDeptDeletePage(long deptId);
public void MoveSettingManagePage();

// Rank
public void MoveRankEditPage(long rankId);
public void MoveRankDeletePage(long rankId);
```

**만약 없다면 추가**:
```csharp
// 부서 관리
public void MoveDeptEditPage(long deptId) => _navigationManager.NavigateTo($"/Admin/Settings/Depts/Edit/{deptId}");
public void MoveDeptDeletePage(long deptId) => _navigationManager.NavigateTo($"/Admin/Settings/Depts/Delete/{deptId}");
public void MoveSettingManagePage() => _navigationManager.NavigateTo("/Admin/SettingManage");

// 직급 관리
public void MoveRankEditPage(long rankId) => _navigationManager.NavigateTo($"/Admin/Settings/Ranks/Edit/{rankId}");
public void MoveRankDeletePage(long rankId) => _navigationManager.NavigateTo($"/Admin/Settings/Ranks/Delete/{rankId}");
```

---

## 4. 필요한 컴포넌트

### 4.1. Form 컴포넌트 (이미 존재하는지 확인 필요)
- `FormInput`: 텍스트/숫자 입력
- `FormCheckbox`: 체크박스
- `FormTextarea`: 여러 줄 텍스트

### 4.2. Common 컴포넌트
- `LoadingIndicator`: 로딩 표시
- `DisplayResultText`: 결과 메시지 표시

---

## 5. 테스트 시나리오

### 5.1. 부서 생성 테스트

**시나리오 1: 정상 생성**
1. Admin/SettingManage 접속
2. "부서 관리" 탭 선택
3. "부서생성" 버튼 클릭
4. 부서번호: 100
5. 부서명: "테스트부서"
6. 사용여부: 체크
7. 비고: "테스트용"
8. "저장" 버튼 클릭
9. **확인**: "부서가 성공적으로 등록되었습니다." 메시지 ✅
10. **확인**: SettingManage 페이지로 이동 ✅
11. **확인**: 목록에 "테스트부서" 표시 ✅

**시나리오 2: 필수값 누락**
1. 부서생성 페이지 접속
2. 부서명 입력하지 않음
3. "저장" 버튼 클릭
4. **확인**: "부서명은 필수 입력 항목입니다." 오류 ✅

**시나리오 3: 부서번호 0 이하**
1. 부서생성 페이지 접속
2. 부서번호: 0
3. 부서명: "테스트"
4. "저장" 버튼 클릭
5. **확인**: "부서번호는 1 이상이어야 합니다." 오류 ✅

### 5.2. 부서 상세 조회 테스트

**시나리오 1: 상세 정보 확인**
1. SettingManage 페이지에서 부서 "상세" 버튼 클릭
2. **확인**: 부서번호, 부서명, 사용여부, 비고 표시 ✅
3. **확인**: 구성원 목록 표시 ✅
4. **확인**: "수정", "삭제", "목록" 버튼 표시 ✅

### 5.3. 부서 수정 테스트

**시나리오 1: 정상 수정**
1. Details 페이지에서 "수정" 버튼 클릭
2. 부서명: "수정된부서"로 변경
3. "수정" 버튼 클릭
4. **확인**: "부서가 성공적으로 수정되었습니다." 메시지 ✅
5. **확인**: Details 페이지로 이동 ✅
6. **확인**: 변경 내용 반영 ✅

**시나리오 2: 취소**
1. Edit 페이지에서 내용 수정
2. "취소" 버튼 클릭
3. **확인**: Details 페이지로 이동 (저장 안 됨) ✅

### 5.4. 부서 삭제 테스트

**시나리오 1: 정상 삭제**
1. Details 페이지에서 "삭제" 버튼 클릭
2. **확인**: 삭제 확인 메시지 표시 ✅
3. **확인**: 부서 정보 표시 ✅
4. "삭제" 버튼 클릭
5. **확인**: "부서가 성공적으로 삭제되었습니다." 메시지 ✅
6. **확인**: SettingManage 페이지로 이동 ✅
7. **확인**: 목록에서 삭제된 부서 사라짐 ✅

**시나리오 2: 취소**
1. Delete 페이지에서 "취소" 버튼 클릭
2. **확인**: Details 페이지로 이동 (삭제 안 됨) ✅

### 5.5. 직급 CRUD 테스트

**직급 관리도 부서와 동일한 테스트 시나리오 적용**:
- 직급 생성 테스트 (정상, 필수값 누락, 번호 0 이하)
- 직급 상세 조회 테스트
- 직급 수정 테스트 (정상, 취소)
- 직급 삭제 테스트 (정상, 취소)

### 5.6. 탭 전환 테스트

**시나리오: 부서/직급 탭 전환**
1. SettingManage 페이지 접속
2. "부서 관리" 탭 선택
3. **확인**: 부서 목록 표시 ✅
4. "직급 관리" 탭 선택
5. **확인**: 직급 목록 표시 ✅
6. 다시 "부서 관리" 탭 선택
7. **확인**: 부서 목록 표시 ✅

---

## 6. 완료 조건

- [ ] Step 1: Depts/Create 페이지 구현 완료
- [ ] Step 2: Depts/Edit 페이지 구현 완료
- [ ] Step 3: Depts/Delete 페이지 구현 완료
- [ ] Step 4: Depts/Details 페이지 수정 완료
- [ ] Step 5: Ranks/Create 페이지 구현 완료
- [ ] Step 6: Ranks/Edit 페이지 구현 완료
- [ ] Step 7: Ranks/Delete 페이지 구현 완료
- [ ] Step 8: Ranks/Details 페이지 수정 완료
- [ ] Step 9: UrlActions 메서드 추가 확인 완료
- [ ] 부서 생성 테스트 성공
- [ ] 부서 수정 테스트 성공
- [ ] 부서 삭제 테스트 성공
- [ ] 부서 상세 조회 테스트 성공
- [ ] 직급 생성 테스트 성공
- [ ] 직급 수정 테스트 성공
- [ ] 직급 삭제 테스트 성공
- [ ] 직급 상세 조회 테스트 성공
- [ ] 탭 전환 테스트 성공
- [ ] 빌드 오류 없음
- [ ] 브라우저 콘솔 오류 없음

---

## 7. 주의사항

1. **Primary Constructor 사용**: .cs 파일들은 Primary Constructor 패턴 사용 중
2. **네임스페이스 일관성**: `MdcHR26Apps.BlazorServer.Components.Pages.Admin.Settings.Depts/Ranks`
3. **Admin 권한 체크**: 모든 페이지에서 `IsloginAndIsAdminCheck()` 사용
4. **비동기 처리**: Repository 호출은 모두 async/await 사용
5. **에러 처리**: try-catch로 예외 처리 및 사용자 친화적 메시지 표시
6. **유효성 검증**: 필수값 체크 (부서명/직급명, 번호 1 이상)
7. **자동 이동**: 저장/수정/삭제 성공 시 1초 후 자동 페이지 이동
8. **FormInput Type**: 부서번호/직급번호는 `Type="number"` 사용

---

## 8. 관련 파일

### 8.1. Models
- `MdcHR26Apps.Models/Department/EDepartmentDb.cs`
- `MdcHR26Apps.Models/Department/IEDepartmentRepository.cs`
- `MdcHR26Apps.Models/Rank/ERankDb.cs`
- `MdcHR26Apps.Models/Rank/IERankRepository.cs`

### 8.2. Components (필요 컴포넌트)
- `Components/Pages/Components/Form/FormInput.razor`
- `Components/Pages/Components/Form/FormCheckbox.razor`
- `Components/Pages/Components/Form/FormTextarea.razor`
- `Components/Pages/Components/Common/LoadingIndicator.razor`
- `Components/Pages/Components/Common/DisplayResultText.razor`
- `Components/Pages/Components/Common/MemberListTable.razor`

### 8.3. Data
- `MdcHR26Apps.BlazorServer/Data/UrlActions.cs`
- `MdcHR26Apps.BlazorServer/Data/LoginStatusService.cs`

---

## 9. 빌드 테스트

각 Step 완료 후 빌드 테스트 필수:
```bash
dotnet build
```

---

**작성일**: 2026-02-11
**작성자**: Claude AI
