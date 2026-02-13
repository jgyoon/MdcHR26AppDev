# 작업지시서: SubAgreement 관련 컴포넌트 구현

**날짜**: 2026-02-03
**작업 타입**: 컴포넌트 구현
**예상 소요**: 3-4시간
**관련 이슈**: [#009](../issues/009_phase3_webapp_development.md)
**선행 작업**: [20260203_05_components_agreement.md](20260203_05_components_agreement.md)

---

## 1. 작업 개요

### 배경
- Agreement 컴포넌트 구현 완료
- SubAgreement (세부직무평가) 관련 컴포넌트 구현 필요

### 목표
SubAgreement 페이지에서 사용할 컴포넌트를 2026년 프로젝트에 구현

### 구현 범위
1. **Table 컴포넌트** (4개)
   - SubAgreementDbListTable: 세부 직무 목록 테이블 (편집 모드)
   - SubAgreementDetailsTable: 세부 직무 상세 테이블
   - SubAgreementListTable: 세부 직무 목록 (TeamLeader용)
   - SubAgreementResetList: 세부 직무 초기화 목록

2. **ViewPage 컴포넌트** (1개)
   - SubAgreementDbListView: 세부 직무 목록 뷰 (읽기 전용)

3. **Modal 컴포넌트** (1개)
   - SubAgreementDeleteModal: 세부 직무 삭제 확인 모달

4. **CommonView 컴포넌트** (2개)
   - AgreeItemLists: 합의된 직무별 작성내역
   - ReportTaskListCommonView: 세부업무 내역 뷰 (펼쳐보기)

---

## 2. 데이터 모델 (2026년 DB 구조)

### SubAgreementDb
**경로**: `MdcHR26Apps.Models/SubAgreementDb/SubAgreementDb.cs`

**주요 필드**:
- Said (Int64) - PK
- Aid (Int64) - FK to AgreementDb
- Uid (Int64) - FK to UserDb
- Pid (Int64) - FK to ProcessDb
- Sub_Agreement_Number (int) - 세부 업무 번호
- TaskName (string) - 업무명
- TaskObjective (string) - 업무 목표
- Sub_Agreement_Proportion (int) - 비중 (%)
- Completed_Number (int) - 완료 상태 (0: 미작성, 1: 조정완료, 2: 승인완료)

---

## 3. 컴포넌트 구현

### 3.1. SubAgreementDbListTable.razor

**경로**: `MdcHR26Apps.BlazorServer/Components/Pages/Components/Table/SubAgreementDbListTable.razor`

**기능**: 사용자의 세부 직무 목록을 테이블 형태로 표시 (편집 가능)

**Parameters**:
```csharp
[Parameter] public List<SubAgreementDb> subAgreementDbs { get; set; } = new();
[Parameter] public EventCallback<long> OnDetails { get; set; }
```

**코드**:
```razor
@using MdcHR26Apps.Models.SubAgreementDb

<table class="table table-hover table-striped">
    <thead>
        <tr>
            <th>#</th>
            <th>세부업무번호</th>
            <th>업무명</th>
            <th>업무목표</th>
            <th>비중</th>
            <th>상태</th>
            <th>작업</th>
        </tr>
    </thead>
    <tbody>
        @{ int sortNo = 1; }
        @foreach (var item in subAgreementDbs)
        {
            <tr>
                <td>@sortNo</td>
                <td>@item.Sub_Agreement_Number</td>
                <td>@item.TaskName</td>
                <td class="text-truncate" style="max-width: 300px;">@item.TaskObjective</td>
                <td>@item.Sub_Agreement_Proportion %</td>
                <td>
                    @if (item.Completed_Number == 0)
                    {
                        <span class="badge bg-secondary">미작성</span>
                    }
                    else if (item.Completed_Number == 1)
                    {
                        <span class="badge bg-warning">조정완료</span>
                    }
                    else
                    {
                        <span class="badge bg-success">승인완료</span>
                    }
                </td>
                <td>
                    <button class="btn btn-sm btn-info" @onclick="@(() => OnDetails.InvokeAsync(item.Said))">상세</button>
                </td>
            </tr>
            sortNo++;
        }
    </tbody>
</table>
```

---

### 3.2. SubAgreementDetailsTable.razor

**경로**: `MdcHR26Apps.BlazorServer/Components/Pages/Components/Table/SubAgreementDetailsTable.razor`

**기능**: 세부 직무 상세 정보를 테이블 형태로 표시

**Parameters**:
```csharp
[Parameter] public SubAgreementDb? subAgreement { get; set; }
[Parameter] public string? AgreementTitle { get; set; }
```

**코드**:
```razor
@using MdcHR26Apps.Models.SubAgreementDb

@if (subAgreement != null)
{
    <table class="table table-bordered">
        <tbody>
            <tr>
                <th class="bg-light" style="width: 25%;">관련 직무</th>
                <td>@AgreementTitle</td>
            </tr>
            <tr>
                <th class="bg-light">세부 업무 번호</th>
                <td>@subAgreement.Sub_Agreement_Number</td>
            </tr>
            <tr>
                <th class="bg-light">업무명</th>
                <td>@subAgreement.TaskName</td>
            </tr>
            <tr>
                <th class="bg-light">업무 목표</th>
                <td style="white-space: pre-wrap;">@subAgreement.TaskObjective</td>
            </tr>
            <tr>
                <th class="bg-light">비중</th>
                <td>@subAgreement.Sub_Agreement_Proportion %</td>
            </tr>
            <tr>
                <th class="bg-light">완료 상태</th>
                <td>
                    @if (subAgreement.Completed_Number == 0)
                    {
                        <span class="badge bg-secondary">미작성</span>
                    }
                    else if (subAgreement.Completed_Number == 1)
                    {
                        <span class="badge bg-warning">조정완료</span>
                    }
                    else
                    {
                        <span class="badge bg-success">승인완료</span>
                    }
                </td>
            </tr>
        </tbody>
    </table>
}
else
{
    <p><em>세부 직무 정보를 불러올 수 없습니다.</em></p>
}
```

---

### 3.3. SubAgreementListTable.razor

**경로**: `MdcHR26Apps.BlazorServer/Components/Pages/Components/Table/SubAgreementListTable.razor`

**기능**: 부서장이 부서원들의 세부 직무 목록을 보는 테이블

**Parameters**:
```csharp
[Parameter] public List<SubAgreementWithUserInfo> subAgreementList { get; set; } = new();
[Parameter] public EventCallback<long> OnDetails { get; set; }
```

**Model**:
```csharp
// SubAgreementWithUserInfo.cs (로컬 모델)
public class SubAgreementWithUserInfo
{
    public long Said { get; set; }
    public long Uid { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string EDepartmentName { get; set; } = string.Empty;
    public int Sub_Agreement_Number { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public string TaskObjective { get; set; } = string.Empty;
    public int Sub_Agreement_Proportion { get; set; }
    public int Completed_Number { get; set; }
}
```

**코드**:
```razor
<table class="table table-hover table-striped">
    <thead>
        <tr>
            <th>#</th>
            <th>사용자</th>
            <th>부서</th>
            <th>업무번호</th>
            <th>업무명</th>
            <th>비중</th>
            <th>상태</th>
            <th>작업</th>
        </tr>
    </thead>
    <tbody>
        @{ int sortNo = 1; }
        @foreach (var item in subAgreementList)
        {
            <tr>
                <td>@sortNo</td>
                <td>@item.UserName</td>
                <td>@item.EDepartmentName</td>
                <td>@item.Sub_Agreement_Number</td>
                <td>@item.TaskName</td>
                <td>@item.Sub_Agreement_Proportion %</td>
                <td>
                    @if (item.Completed_Number == 0)
                    {
                        <span class="badge bg-secondary">미작성</span>
                    }
                    else if (item.Completed_Number == 1)
                    {
                        <span class="badge bg-warning">조정완료</span>
                    }
                    else
                    {
                        <span class="badge bg-success">승인완료</span>
                    }
                </td>
                <td>
                    <button class="btn btn-sm btn-info" @onclick="@(() => OnDetails.InvokeAsync(item.Said))">상세</button>
                </td>
            </tr>
            sortNo++;
        }
    </tbody>
</table>
```

---

### 3.4. SubAgreementResetList.razor

**경로**: `MdcHR26Apps.BlazorServer/Components/Pages/Components/Table/SubAgreementResetList.razor`

**기능**: 부서장이 세부 직무 승인을 초기화할 목록 (체크박스 포함)

**Parameters**:
```csharp
[Parameter] public List<SubAgreementDb> subAgreements { get; set; } = new();
[Parameter] public HashSet<long> SelectedIds { get; set; } = new();
[Parameter] public EventCallback<(long Said, bool IsChecked)> OnCheckChanged { get; set; }
```

**코드**:
```razor
@using MdcHR26Apps.Models.SubAgreementDb

<table class="table table-bordered">
    <thead>
        <tr>
            <th style="width: 5%;">
                <input type="checkbox" @onchange="ToggleAll" />
            </th>
            <th>업무번호</th>
            <th>업무명</th>
            <th>비중</th>
            <th>상태</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var item in subAgreements.Where(s => s.Completed_Number > 0))
        {
            <tr>
                <td>
                    <input type="checkbox"
                           checked="@SelectedIds.Contains(item.Said)"
                           @onchange="@(e => OnCheckChanged.InvokeAsync((item.Said, (bool)e.Value!)))" />
                </td>
                <td>@item.Sub_Agreement_Number</td>
                <td>@item.TaskName</td>
                <td>@item.Sub_Agreement_Proportion %</td>
                <td>
                    @if (item.Completed_Number == 1)
                    {
                        <span class="badge bg-warning">조정완료</span>
                    }
                    else
                    {
                        <span class="badge bg-success">승인완료</span>
                    }
                </td>
            </tr>
        }
    </tbody>
</table>

@code {
    private void ToggleAll(ChangeEventArgs e)
    {
        bool isChecked = (bool)e.Value!;

        if (isChecked)
        {
            foreach (var item in subAgreements.Where(s => s.Completed_Number > 0))
            {
                if (!SelectedIds.Contains(item.Said))
                {
                    SelectedIds.Add(item.Said);
                }
            }
        }
        else
        {
            SelectedIds.Clear();
        }
    }
}
```

---

### 3.5. SubAgreementDbListView.razor

**경로**: `MdcHR26Apps.BlazorServer/Components/Pages/Components/ViewPage/SubAgreementDbListView.razor`

**기능**: 세부 직무 목록 뷰 (읽기 전용, 합의 완료 후)

**Parameters**:
```csharp
[Parameter] public List<SubAgreementDb> subAgreementDbs { get; set; } = new();
```

**코드**:
```razor
@using MdcHR26Apps.Models.SubAgreementDb

@if (subAgreementDbs == null || subAgreementDbs.Count == 0)
{
    <p><em>세부 직무 내역이 없습니다.</em></p>
}
else
{
    <div class="table-responsive">
        <table class="table table-bordered">
            <thead class="table-light">
                <tr>
                    <th>#</th>
                    <th>업무번호</th>
                    <th>업무명</th>
                    <th>업무목표</th>
                    <th>비중</th>
                    <th>상태</th>
                </tr>
            </thead>
            <tbody>
                @{ int sortNo = 1; }
                @foreach (var item in subAgreementDbs)
                {
                    <tr>
                        <td>@sortNo</td>
                        <td>@item.Sub_Agreement_Number</td>
                        <td>@item.TaskName</td>
                        <td style="white-space: pre-wrap;">@item.TaskObjective</td>
                        <td>@item.Sub_Agreement_Proportion %</td>
                        <td>
                            @if (item.Completed_Number == 0)
                            {
                                <span class="badge bg-secondary">미작성</span>
                            }
                            else if (item.Completed_Number == 1)
                            {
                                <span class="badge bg-warning">조정완료</span>
                            }
                            else
                            {
                                <span class="badge bg-success">승인완료</span>
                            }
                        </td>
                    </tr>
                    sortNo++;
                }
            </tbody>
            <tfoot>
                <tr class="table-info">
                    <th colspan="4" class="text-end">총 비중 합계:</th>
                    <th>@subAgreementDbs.Sum(s => s.Sub_Agreement_Proportion) %</th>
                    <th></th>
                </tr>
            </tfoot>
        </table>
    </div>
}
```

---

### 3.6. SubAgreementDeleteModal.razor

**경로**: `MdcHR26Apps.BlazorServer/Components/Pages/Components/Modal/SubAgreementDeleteModal.razor`

**기능**: 세부 직무 삭제 확인 모달

**Parameters**:
```csharp
[Parameter] public bool Show { get; set; }
[Parameter] public SubAgreementDb? SubAgreement { get; set; }
[Parameter] public EventCallback OnConfirm { get; set; }
[Parameter] public EventCallback OnCancel { get; set; }
```

**코드**:
```razor
@using MdcHR26Apps.Models.SubAgreementDb

@if (Show)
{
    <div class="modal fade show d-block" tabindex="-1" role="dialog" style="background-color: rgba(0,0,0,0.5);">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">세부 직무 삭제 확인</h5>
                    <button type="button" class="btn-close" @onclick="OnCancel"></button>
                </div>
                <div class="modal-body">
                    @if (SubAgreement != null)
                    {
                        <p>다음 세부 직무를 정말 삭제하시겠습니까?</p>
                        <div class="alert alert-warning">
                            <strong>업무 번호:</strong> @SubAgreement.Sub_Agreement_Number<br />
                            <strong>업무명:</strong> @SubAgreement.TaskName<br />
                            <strong>비중:</strong> @SubAgreement.Sub_Agreement_Proportion %
                        </div>

                        @if (SubAgreement.Completed_Number >= 2)
                        {
                            <div class="alert alert-danger">
                                ⚠️ <strong>승인 완료된 세부 직무입니다.</strong> 삭제 시 부서장의 승인을 다시 받아야 합니다.
                            </div>
                        }

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
```

---

### 3.7. AgreeItemLists.razor

**경로**: `MdcHR26Apps.BlazorServer/Components/Pages/Components/CommonView/AgreeItemLists.razor`

**기능**: 합의된 직무별 작성내역 표시 (펼쳐보기)

**Parameters**:
```csharp
[Parameter] public bool Collapsed { get; set; }
[Parameter] public List<AgreeItem> items { get; set; } = new();
[Parameter] public EventCallback Toggle { get; set; }
[Parameter] public int itemCount { get; set; }
```

**Model**:
```csharp
// AgreeItem.cs (로컬 모델)
public class AgreeItem
{
    public string ItemName { get; set; } = string.Empty;
    public int ItemProportion { get; set; }
    public int ItemSubProportion { get; set; }
    public bool ItemCompleteStatus { get; set; }
}
```

**코드**:
```razor
@if (items == null || items.Count == 0)
{
    <p><em>합의된 직무 내용이 없습니다.</em></p>
}
else
{
    <div class="row mb-3">
        @if (!Collapsed)
        {
            <h5>
                합의된 직무 수: @itemCount 개
                <span @onclick="@Toggle" class="oi oi-plus ms-2" style="cursor: pointer;"></span>
            </h5>
        }
        else
        {
            <h5>
                합의된 직무 수: @itemCount 개
                <span @onclick="@Toggle" class="oi oi-minus ms-2" style="cursor: pointer;"></span>
            </h5>
        }
    </div>

    @if (Collapsed)
    {
        <hr />
        <div class="row">
            <div class="col-md-12">
                <ul class="list-group">
                    @foreach (var item in items)
                    {
                        <li class="list-group-item">
                            @if (item.ItemCompleteStatus)
                            {
                                <span class="badge bg-success me-2">✓</span>
                            }
                            <strong>@item.ItemName</strong> (@item.ItemProportion %) : @item.ItemSubProportion %
                        </li>
                    }
                </ul>
            </div>
        </div>
    }
    <hr />
}
```

---

### 3.8. ReportTaskListCommonView.razor

**경로**: `MdcHR26Apps.BlazorServer/Components/Pages/Components/CommonView/ReportTaskListCommonView.razor`

**기능**: 세부업무 내역 뷰 (펼쳐보기) - v_ReportTaskListDB 사용

**Parameters**:
```csharp
[Parameter] public bool Collapsed { get; set; }
[Parameter] public EventCallback Toggle { get; set; }
[Parameter] public List<v_ReportTaskListDB> ReportTaskListDB { get; set; } = new();
```

**코드**:
```razor
@using MdcHR26Apps.Models.Views.v_ReportTaskListDB

<div class="row mb-3">
    @if (!Collapsed)
    {
        <h5>
            세부업무내역 (펼쳐보기 추가)
            <span @onclick="@Toggle" class="oi oi-plus ms-2" style="cursor: pointer;"></span>
        </h5>
    }
    else
    {
        <h5>
            세부업무내역 (펼쳐보기 추가)
            <span @onclick="@Toggle" class="oi oi-minus ms-2" style="cursor: pointer;"></span>
        </h5>
    }
</div>

@if (Collapsed)
{
    @if (ReportTaskListDB == null || ReportTaskListDB.Count == 0)
    {
        <p><em>세부업무 내역이 없습니다.</em></p>
    }
    else
    {
        <hr />
        <div class="table-responsive">
            <table class="table table-sm table-bordered">
                <thead class="table-light">
                    <tr>
                        <th>#</th>
                        <th>업무명</th>
                        <th>목표</th>
                        <th>목표비중</th>
                        <th>결과비중</th>
                        <th>상태</th>
                    </tr>
                </thead>
                <tbody>
                    @{ int sortNo = 1; }
                    @foreach (var task in ReportTaskListDB)
                    {
                        <tr>
                            <td>@sortNo</td>
                            <td>@task.TaskName</td>
                            <td style="max-width: 200px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis;">
                                @task.TaskObjective
                            </td>
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
                        </tr>
                        sortNo++;
                    }
                </tbody>
            </table>
        </div>
    }
}
<hr />
```

---

## 4. 로컬 모델

**경로**: `MdcHR26Apps.BlazorServer/Models/`

### SubAgreementWithUserInfo.cs
```csharp
namespace MdcHR26Apps.BlazorServer.Models;

public class SubAgreementWithUserInfo
{
    public long Said { get; set; }
    public long Uid { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string EDepartmentName { get; set; } = string.Empty;
    public int Sub_Agreement_Number { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public string TaskObjective { get; set; } = string.Empty;
    public int Sub_Agreement_Proportion { get; set; }
    public int Completed_Number { get; set; }
}
```

### AgreeItem.cs
```csharp
namespace MdcHR26Apps.BlazorServer.Models;

public class AgreeItem
{
    public string ItemName { get; set; } = string.Empty;
    public int ItemProportion { get; set; }
    public int ItemSubProportion { get; set; }
    public bool ItemCompleteStatus { get; set; }
}
```

---

## 5. 테스트 계획

### 테스트 시나리오 1: SubAgreementDbListTable
1. SubAgreement 데이터 5개 준비 (Completed_Number: 0, 1, 2 섞어서)
2. SubAgreementDbListTable 렌더링
3. **확인**: 상태 배지 정확히 표시 (미작성/조정완료/승인완료) ✅

### 테스트 시나리오 2: AgreeItemLists (Collapsed)
1. AgreeItem 3개 준비
2. Collapsed = false로 렌더링
3. **확인**: 직무 개수만 표시, 목록 숨김 ✅
4. Toggle 클릭
5. **확인**: 목록 표시, 완료 체크 마크 ✅

### 테스트 시나리오 3: SubAgreementResetList
1. 승인 완료된 SubAgreement 3개 준비
2. 전체 선택 체크박스 클릭
3. **확인**: 모든 항목 선택됨 ✅
4. 개별 항목 체크 해제
5. **확인**: OnCheckChanged 이벤트 발생 ✅

---

## 6. 완료 조건

- [ ] SubAgreementDbListTable.razor 완료
- [ ] SubAgreementDetailsTable.razor 완료
- [ ] SubAgreementListTable.razor 완료
- [ ] SubAgreementResetList.razor 완료
- [ ] SubAgreementDbListView.razor 완료
- [ ] SubAgreementDeleteModal.razor 완료
- [ ] AgreeItemLists.razor 완료
- [ ] ReportTaskListCommonView.razor 완료
- [ ] 로컬 모델 2개 작성 완료
- [ ] 테스트 시나리오 1-3 성공
- [ ] 빌드 오류 0개

---

## 7. 다음 단계

**다음 작업지시서**: [20260203_07_components_report.md](20260203_07_components_report.md)
- Report 관련 컴포넌트 구현

---

## 8. 관련 문서

**이슈**: [#009: Phase 3 WebApp 개발](../issues/009_phase3_webapp_development.md)
**Phase 2 Model**:
- [SubAgreementDb](../../MdcHR26Apps.Models/SubAgreementDb/)
- [v_ReportTaskListDB](../../MdcHR26Apps.Models/Views/v_ReportTaskListDB/)
**참조 프로젝트**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Components`
