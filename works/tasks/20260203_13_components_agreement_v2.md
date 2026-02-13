# 작업지시서: Phase 3-4 Agreement 컴포넌트 구현 (6개)

**날짜**: 2026-02-04
**작업 유형**: 신규 기능 추가
**관련 이슈**: [#009: Phase 3 Blazor Server WebApp 개발](../issues/009_phase3_webapp_development.md)
**참조 가이드**: [20260203_13_REWRITE_GUIDE.md](20260203_13_REWRITE_GUIDE.md)

---

## 1. 작업 개요

**목적**:
- 직무평가 협의(Agreement) 관련 Blazor 컴포넌트 6개 구현
- 2025년 프로젝트 구조 참조, 2026년 Entity/Repository 구조 적용

**배경**:
- Phase 3-4 평가 프로세스 페이지 개발 전 필수 컴포넌트 선행 구현
- Entity/Repository 변경 완료 (작업지시서 11, 12)
- 2026년 DB 구조에 맞는 컴포넌트 필요

---

## 2. Entity 및 Repository 구조

### 2.1. AgreementDb Entity

**경로**: `MdcHR26Apps.Models\EvaluationAgreement\AgreementDb.cs`

**필드 구조**:
```csharp
public class AgreementDb
{
    public Int64 Aid { get; set; }               // PK
    public Int64 Uid { get; set; }               // FK (UserDb)
    public int Report_Item_Number { get; set; }
    public string Report_Item_Name_1 { get; set; }
    public string Report_Item_Name_2 { get; set; }
    public int Report_Item_Proportion { get; set; }
}
```

### 2.2. IAgreementRepository 메서드 (7개)

**경로**: `MdcHR26Apps.Models\EvaluationAgreement\IAgreementRepository.cs`

```csharp
Task<AgreementDb> AddAsync(AgreementDb model);
Task<List<AgreementDb>> GetByAllAsync();
Task<AgreementDb> GetByIdAsync(long id);
Task<bool> UpdateAsync(AgreementDb model);
Task<bool> DeleteAsync(long id);
Task<List<AgreementDb>> GetByUserIdAllAsync(long userId);
Task<List<AgreementDb>> GetByTasksPeroportionAsync(long userId, string deptName, string indexName);
```

**주의**: 제거된 메서드 사용 금지
- ❌ `GetCountByUidAsync()` - 제거됨
- ❌ `DeleteAllByUidAsync()` - 제거됨
- ✅ 대신 `GetByUserIdAllAsync().Count` 사용
- ✅ 대신 `foreach` + `DeleteAsync()` 사용

---

## 3. 구현할 컴포넌트 (6개)

### 3.1. AgreementDbListTable.razor

**경로**: `MdcHR26Apps.BlazorServer\Components\Agreement\AgreementDbListTable.razor`

**목적**: 직무평가 목록을 테이블 형식으로 표시

**주요 기능**:
- AgreementDb 목록 표시 (평가지표, 평가직무, 직무비중)
- 상세 버튼 (UserAgreementDetailsAction 이벤트)
- 반응형 UI (col-12/col-md-5/col-md-2)

**2025년 참조**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Components\AgreementDbListTable.razor`

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
            <th>비고</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var item in agreementDbs)
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
                    <button class="btn btn-info" @onclick="@(() => OnDetailsClick(item.Aid))">상세</button>
                </td>
            </tr>
        }
    </tbody>
</table>

@code {
    [Parameter] public List<AgreementDb> agreementDbs { get; set; } = new();
    [Parameter] public EventCallback<long> OnDetailsClick { get; set; }

    private int sortNo = 1;
}
```

---

### 3.2. AgreementDetailsTable.razor

**경로**: `MdcHR26Apps.BlazorServer\Components\Agreement\AgreementDetailsTable.razor`

**목적**: 직무평가 상세 정보를 테이블 형식으로 표시

**주요 기능**:
- 단일 AgreementDb 상세 정보 표시
- 편집/삭제 버튼

**2025년 참조**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Components\AgreementDetailsTable.razor`

**구현 내용**:
```razor
@if (agreement != null)
{
    <table class="table">
        <tbody>
            <tr>
                <th>평가지표</th>
                <td>@agreement.Report_Item_Name_1</td>
            </tr>
            <tr>
                <th>평가직무</th>
                <td>@agreement.Report_Item_Name_2</td>
            </tr>
            <tr>
                <th>직무비중</th>
                <td>@agreement.Report_Item_Proportion %</td>
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
    [Parameter] public AgreementDb? agreement { get; set; }
    [Parameter] public EventCallback OnEdit { get; set; }
    [Parameter] public EventCallback OnDelete { get; set; }
    [Parameter] public EventCallback OnBack { get; set; }
}
```

---

### 3.3. AgreementListTable.razor

**경로**: `MdcHR26Apps.BlazorServer\Components\Agreement\AgreementListTable.razor`

**목적**: 직무평가 목록 (간소화 버전)

**주요 기능**:
- AgreementDb 목록 간단 표시
- 선택/편집 기능

**2025년 참조**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Components\AgreementListTable.razor`

**구현 내용**:
```razor
<table class="table table-sm">
    <thead>
        <tr>
            <th>번호</th>
            <th>평가지표</th>
            <th>평가직무</th>
            <th>비중</th>
            <th>편집</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var item in agreements)
        {
            <tr>
                <td>@item.Report_Item_Number</td>
                <td>@item.Report_Item_Name_1</td>
                <td>@item.Report_Item_Name_2</td>
                <td>@item.Report_Item_Proportion %</td>
                <td>
                    <button class="btn btn-sm btn-outline-primary" @onclick="@(() => OnEdit(item.Aid))">편집</button>
                </td>
            </tr>
        }
    </tbody>
</table>

@code {
    [Parameter] public List<AgreementDb> agreements { get; set; } = new();
    [Parameter] public EventCallback<long> OnEdit { get; set; }
}
```

---

### 3.4. AgreementDbListView.razor

**경로**: `MdcHR26Apps.BlazorServer\Components\Agreement\ViewPage\AgreementDbListView.razor`

**목적**: 직무평가 목록 전체 페이지

**주요 기능**:
- AgreementDbListTable 컴포넌트 통합
- Repository 연동
- 검색/필터 기능

**2025년 참조**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Components\ViewPage\AgreementDbListView.razor`

**구현 내용**:
```razor
@inject IAgreementRepository agreementRepository

<h3>직무평가 목록</h3>

@if (agreements == null)
{
    <p><em>Loading...</em></p>
}
else
{
    <AgreementDbListTable agreementDbs="agreements" OnDetailsClick="HandleDetailsClick" />
}

@code {
    [Parameter] public long Uid { get; set; }

    private List<AgreementDb> agreements = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        agreements = await agreementRepository.GetByUserIdAllAsync(Uid);
    }

    private void HandleDetailsClick(long aid)
    {
        // 상세 페이지로 이동
        NavigationManager.NavigateTo($"/agreement/details/{aid}");
    }
}
```

---

### 3.5. AgreementDeleteModal.razor

**경로**: `MdcHR26Apps.BlazorServer\Components\Agreement\Modal\AgreementDeleteModal.razor`

**목적**: 직무 삭제 확인 모달

**주요 기능**:
- 삭제 확인 UI
- Repository DeleteAsync 호출
- 결과 메시지 표시

**2025년 참조**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Components\Modal\AgreementDeleteModal.razor`

**구현 내용**:
```razor
@inject IAgreementRepository agreementRepository

@if (model != null)
{
    <div class="modal @ModalClass" tabindex="-1" role="dialog" style="display:@ModalDisplay;">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">직무 삭제</h5>
                    <button type="button" class="btn-close" @onclick="Close"></button>
                </div>
                <div class="modal-body">
                    <p>이 직무를 삭제하시겠습니까?</p>
                    <p>평가지표: @model.Report_Item_Name_1</p>
                    <p>평가직무: @model.Report_Item_Name_2</p>

                    @if (!string.IsNullOrEmpty(resultText))
                    {
                        <div class="alert alert-info">@resultText</div>
                    }
                </div>
                <div class="modal-footer">
                    <button class="btn btn-danger" @onclick="DeleteUserAgreement">예(Yes)</button>
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
    [Parameter] public AgreementDb? model { get; set; }
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

    private async Task DeleteUserAgreement()
    {
        if (model != null)
        {
            var result = await agreementRepository.DeleteAsync(model.Aid);

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

### 3.6. AgreementComment.razor

**경로**: `MdcHR26Apps.BlazorServer\Components\Agreement\CommonView\AgreementComment.razor`

**목적**: 합의 내역 표시 (접기/펼치기)

**주요 기능**:
- 합의 내역 텍스트 표시
- 토글 기능 (접기/펼치기)
- HTML 마크업 렌더링

**2025년 참조**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Components\CommonView\AgreementComment.razor`

**구현 내용**:
```razor
@if (!string.IsNullOrEmpty(Comment))
{
    <div class="row mb-3">
        <div class="col-12">
            <h5 class="form-label">
                합의 내역
                <span @onclick="Toggle" class="@IconClass" style="cursor: pointer;"></span>
            </h5>
        </div>
    </div>

    @if (Collapsed)
    {
        <hr />
        <div class="mb-3">
            @((MarkupString)FormatComment(Comment))
        </div>
        <hr />
    }
}

@code {
    [Parameter] public string Comment { get; set; } = string.Empty;

    private bool Collapsed = false;
    private string IconClass => Collapsed ? "oi oi-minus" : "oi oi-plus";

    private void Toggle()
    {
        Collapsed = !Collapsed;
    }

    private string FormatComment(string comment)
    {
        // 줄바꿈을 <br> 태그로 변환
        return comment.Replace("\n", "<br />");
    }
}
```

---

## 4. 폴더 구조

**생성할 폴더**:
```
MdcHR26Apps.BlazorServer/
└── Components/
    └── Agreement/
        ├── AgreementDbListTable.razor
        ├── AgreementDbListTable.razor.cs
        ├── AgreementDetailsTable.razor
        ├── AgreementDetailsTable.razor.cs
        ├── AgreementListTable.razor
        ├── AgreementListTable.razor.cs
        ├── ViewPage/
        │   ├── AgreementDbListView.razor
        │   └── AgreementDbListView.razor.cs
        ├── Modal/
        │   ├── AgreementDeleteModal.razor
        │   └── AgreementDeleteModal.razor.cs
        └── CommonView/
            ├── AgreementComment.razor
            └── AgreementComment.razor.cs
```

---

## 5. 공통 구현 사항

### 5.1. 네임스페이스
```csharp
namespace MdcHR26Apps.BlazorServer.Components.Agreement;
```

### 5.2. 필수 using 구문
```csharp
@using MdcHR26Apps.Models.EvaluationAgreement
@inject IAgreementRepository agreementRepository
@inject NavigationManager NavigationManager
```

### 5.3. 반응형 CSS 클래스
- `table-responsive-md`: 모바일 대응
- `col-12 col-md-5`: Bootstrap 그리드
- `btn btn-info`, `btn btn-danger`: Bootstrap 버튼

### 5.4. 에러 처리
```csharp
try
{
    // Repository 호출
}
catch (Exception ex)
{
    errorMessage = $"오류: {ex.Message}";
}
```

---

## 6. 테스트 항목

개발자가 테스트할 항목:

### Test 1: 컴포넌트 빌드 확인
1. 프로젝트 빌드 실행
2. **확인**: 경고 0개, 오류 0개

### Test 2: AgreementDbListTable 렌더링
1. 테스트 페이지에 AgreementDbListTable 추가
2. 샘플 데이터 전달
3. **확인**: 테이블 정상 렌더링, 상세 버튼 동작

### Test 3: AgreementDeleteModal 동작
1. 모달 Open() 호출
2. 삭제 확인 클릭
3. **확인**: Repository DeleteAsync 호출, 모달 닫힘

### Test 4: AgreementComment 토글
1. Comment 프로퍼티 설정
2. 접기/펼치기 아이콘 클릭
3. **확인**: 내용 표시/숨김 정상 동작

### Test 5: 반응형 UI
1. 브라우저 창 크기 조정 (모바일 사이즈)
2. **확인**: 컬럼 레이아웃 정상 변경

---

## 7. 완료 조건

- [ ] 6개 컴포넌트 파일 생성 (razor + razor.cs)
- [ ] 폴더 구조 생성 (Agreement/ViewPage/Modal/CommonView)
- [ ] 빌드 성공 (경고 0개, 오류 0개)
- [ ] Entity 필드명 정확히 사용 (Report_Item_*)
- [ ] Repository 메서드 정확히 사용 (제거된 메서드 미사용)
- [ ] Test 1-5 모두 통과
- [ ] 2025년 프로젝트와 UI/UX 일치

---

## 8. 주의사항

### ❌ 절대 사용 금지
- `GetCountByUidAsync()` - 제거된 메서드
- `DeleteAllByUidAsync()` - 제거된 메서드
- `Agreement_Item_*` - 잘못된 필드명 (2026년 이전)
- `string userId` - 잘못된 파라미터 타입

### ✅ 반드시 사용
- `GetByUserIdAllAsync(long uid)` - 25년 메서드 패턴
- `Report_Item_*` - 2026년 Entity 필드명
- `long uid` - 파라미터 타입

### 참조 문서
- [20260203_13_REWRITE_GUIDE.md](20260203_13_REWRITE_GUIDE.md) - 재작성 가이드
- [20260203_11_fix_entity_db_field_names.md](20260203_11_fix_entity_db_field_names.md) - Entity 변경 내역
- [20260203_12_fix_repository_based_on_2025.md](20260203_12_fix_repository_based_on_2025.md) - Repository 변경 내역

---

**작성일**: 2026-02-04
**담당**: Claude Sonnet 4.5
**관련 이슈**: [#009](../issues/009_phase3_webapp_development.md)
**다음 작업지시서**: 20260203_14_components_subagreement_v2.md (8개 컴포넌트)
