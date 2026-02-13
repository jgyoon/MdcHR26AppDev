# 작업지시서: Agreement 관련 컴포넌트 구현

**날짜**: 2026-02-03
**작업 타입**: 컴포넌트 구현
**예상 소요**: 2-3시간
**관련 이슈**: [#009](../issues/009_phase3_webapp_development.md)
**선행 작업**: View 검증 완료

---

## 1. 작업 개요

### 배경
- Phase 3-4 시작 전 준비 작업
- 2025년 프로젝트의 컴포넌트를 2026년 DB 구조에 맞게 구현
- Agreement (직무평가 협의) 관련 컴포넌트 우선 구현

### 목표
Agreement 페이지에서 사용할 컴포넌트를 2026년 프로젝트에 구현

### 구현 범위
1. **Table 컴포넌트** (3개)
   - AgreementDbListTable: 직무 목록 테이블 (편집 모드)
   - AgreementDetailsTable: 직무 상세 테이블
   - AgreementListTable: 직무 목록 (TeamLeader용)

2. **ViewPage 컴포넌트** (1개)
   - AgreementDbListView: 직무 목록 뷰 (읽기 전용)

3. **Modal 컴포넌트** (1개)
   - AgreementDeleteModal: 직무 삭제 확인 모달

4. **CommonView 컴포넌트** (1개)
   - AgreementComment: 합의 코멘트 (펼쳐보기)

---

## 2. 참조 프로젝트

### 2025년 프로젝트 경로
- 컴포넌트: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Components`
- Agreement 페이지: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages\Agreement`

### 2026년 프로젝트 경로
- 컴포넌트: `MdcHR26Apps.BlazorServer/Components/Pages/Components`

---

## 3. 데이터 모델 (2026년 DB 구조)

### AgreementDb
**경로**: `MdcHR26Apps.Models/AgreementDb/AgreementDb.cs`

**주요 필드**:
- Aid (Int64) - PK
- Uid (Int64) - FK to UserDb
- Item_Number (int) - 직무 번호
- Item_Title (string) - 직무 제목
- Item_Contents (string) - 직무 내용
- Item_Proportion (int) - 비중 (%)

**2025년 대비 차이점**:
- 2025년: `Report_Item_Name_1`, `Report_Item_Name_2`, `Report_Item_Proportion`
- 2026년: `Item_Title`, `Item_Contents`, `Item_Proportion`

---

## 4. 컴포넌트 구현

### 4.1. AgreementDbListTable.razor

**경로**: `MdcHR26Apps.BlazorServer/Components/Pages/Components/Table/AgreementDbListTable.razor`

**기능**: 사용자의 직무 목록을 테이블 형태로 표시 (편집 가능)

**Parameters**:
```csharp
[Parameter] public List<AgreementDb> agreementDbs { get; set; } = new();
[Parameter] public EventCallback<long> OnDetails { get; set; }
```

**코드**:
```razor
@using MdcHR26Apps.Models.AgreementDb

<table class="table table-hover table-responsive-md">
    <thead>
        <tr>
            <th class="table-header">#</th>
            <th class="table-header">
                <div class="row">
                    <div class="col-12 col-sm-12 col-md-3">직무번호</div>
                    <div class="col-12 col-sm-12 col-md-4">직무제목</div>
                    <div class="col-12 col-sm-12 col-md-3">직무내용</div>
                    <div class="col-12 col-sm-12 col-md-2">직무비중</div>
                </div>
            </th>
            <th class="table-header">작업</th>
        </tr>
    </thead>
    <tbody>
        @{ int sortNo = 1; }
        @foreach (var item in agreementDbs)
        {
            <tr>
                <td class="table-cell">@sortNo</td>
                <td class="table-cell">
                    <div class="row">
                        <div class="col-12 col-sm-12 col-md-3">@item.Item_Number</div>
                        <div class="col-12 col-sm-12 col-md-4">@item.Item_Title</div>
                        <div class="col-12 col-sm-12 col-md-3 table-text">@item.Item_Contents</div>
                        <div class="col-12 col-sm-12 col-md-2">@item.Item_Proportion %</div>
                    </div>
                </td>
                <td class="table-cell">
                    <button class="btn btn-sm btn-info" @onclick="@(() => OnDetails.InvokeAsync(item.Aid))">상세</button>
                </td>
            </tr>
            sortNo++;
        }
    </tbody>
</table>

<style>
    .table-header {
        background-color: #f8f9fa;
        font-weight: bold;
    }

    .table-cell {
        vertical-align: middle;
    }

    .table-text {
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
        max-width: 200px;
    }
</style>
```

---

### 4.2. AgreementDetailsTable.razor

**경로**: `MdcHR26Apps.BlazorServer/Components/Pages/Components/Table/AgreementDetailsTable.razor`

**기능**: 직무 상세 정보를 테이블 형태로 표시

**Parameters**:
```csharp
[Parameter] public AgreementDb? agreement { get; set; }
```

**코드**:
```razor
@using MdcHR26Apps.Models.AgreementDb

@if (agreement != null)
{
    <table class="table table-bordered">
        <tbody>
            <tr>
                <th class="bg-light" style="width: 20%;">직무 번호</th>
                <td>@agreement.Item_Number</td>
            </tr>
            <tr>
                <th class="bg-light">직무 제목</th>
                <td>@agreement.Item_Title</td>
            </tr>
            <tr>
                <th class="bg-light">직무 내용</th>
                <td style="white-space: pre-wrap;">@agreement.Item_Contents</td>
            </tr>
            <tr>
                <th class="bg-light">직무 비중</th>
                <td>@agreement.Item_Proportion %</td>
            </tr>
        </tbody>
    </table>
}
else
{
    <p><em>직무 정보를 불러올 수 없습니다.</em></p>
}
```

---

### 4.3. AgreementListTable.razor

**경로**: `MdcHR26Apps.BlazorServer/Components/Pages/Components/Table/AgreementListTable.razor`

**기능**: 부서장이 부서원들의 직무 목록을 보는 테이블 (v_MemberListDB 조인)

**Parameters**:
```csharp
[Parameter] public List<AgreementWithUserInfo> agreementList { get; set; } = new();
[Parameter] public EventCallback<long> OnDetails { get; set; }
```

**Model**:
```csharp
// AgreementWithUserInfo.cs (로컬 모델)
public class AgreementWithUserInfo
{
    public long Aid { get; set; }
    public long Uid { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string EDepartmentName { get; set; } = string.Empty;
    public int Item_Number { get; set; }
    public string Item_Title { get; set; } = string.Empty;
    public string Item_Contents { get; set; } = string.Empty;
    public int Item_Proportion { get; set; }
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
            <th>직무번호</th>
            <th>직무제목</th>
            <th>직무비중</th>
            <th>작업</th>
        </tr>
    </thead>
    <tbody>
        @{ int sortNo = 1; }
        @foreach (var item in agreementList)
        {
            <tr>
                <td>@sortNo</td>
                <td>@item.UserName</td>
                <td>@item.EDepartmentName</td>
                <td>@item.Item_Number</td>
                <td>@item.Item_Title</td>
                <td>@item.Item_Proportion %</td>
                <td>
                    <button class="btn btn-sm btn-info" @onclick="@(() => OnDetails.InvokeAsync(item.Aid))">상세</button>
                </td>
            </tr>
            sortNo++;
        }
    </tbody>
</table>
```

---

### 4.4. AgreementDbListView.razor

**경로**: `MdcHR26Apps.BlazorServer/Components/Pages/Components/ViewPage/AgreementDbListView.razor`

**기능**: 직무 목록 뷰 (읽기 전용, 합의 완료 후)

**Parameters**:
```csharp
[Parameter] public List<AgreementDb> agreementDbs { get; set; } = new();
```

**코드**:
```razor
@using MdcHR26Apps.Models.AgreementDb

@if (agreementDbs == null || agreementDbs.Count == 0)
{
    <p><em>직무 내역이 없습니다.</em></p>
}
else
{
    <div class="table-responsive">
        <table class="table table-bordered">
            <thead class="table-light">
                <tr>
                    <th>#</th>
                    <th>직무번호</th>
                    <th>직무제목</th>
                    <th>직무내용</th>
                    <th>비중</th>
                </tr>
            </thead>
            <tbody>
                @{ int sortNo = 1; }
                @foreach (var item in agreementDbs)
                {
                    <tr>
                        <td>@sortNo</td>
                        <td>@item.Item_Number</td>
                        <td>@item.Item_Title</td>
                        <td style="white-space: pre-wrap;">@item.Item_Contents</td>
                        <td>@item.Item_Proportion %</td>
                    </tr>
                    sortNo++;
                }
            </tbody>
            <tfoot>
                <tr class="table-info">
                    <th colspan="4" class="text-end">총 비중 합계:</th>
                    <th>@agreementDbs.Sum(a => a.Item_Proportion) %</th>
                </tr>
            </tfoot>
        </table>
    </div>
}
```

---

### 4.5. AgreementDeleteModal.razor

**경로**: `MdcHR26Apps.BlazorServer/Components/Pages/Components/Modal/AgreementDeleteModal.razor`

**기능**: 직무 삭제 확인 모달

**Parameters**:
```csharp
[Parameter] public bool Show { get; set; }
[Parameter] public AgreementDb? Agreement { get; set; }
[Parameter] public EventCallback OnConfirm { get; set; }
[Parameter] public EventCallback OnCancel { get; set; }
```

**코드**:
```razor
@using MdcHR26Apps.Models.AgreementDb

@if (Show)
{
    <div class="modal fade show d-block" tabindex="-1" role="dialog" style="background-color: rgba(0,0,0,0.5);">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">직무 삭제 확인</h5>
                    <button type="button" class="btn-close" @onclick="OnCancel"></button>
                </div>
                <div class="modal-body">
                    @if (Agreement != null)
                    {
                        <p>다음 직무를 정말 삭제하시겠습니까?</p>
                        <div class="alert alert-warning">
                            <strong>직무 번호:</strong> @Agreement.Item_Number<br />
                            <strong>직무 제목:</strong> @Agreement.Item_Title<br />
                            <strong>직무 비중:</strong> @Agreement.Item_Proportion %
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
```

---

### 4.6. AgreementComment.razor

**경로**: `MdcHR26Apps.BlazorServer/Components/Pages/Components/CommonView/AgreementComment.razor`

**기능**: 합의 코멘트 표시 (펼쳐보기 토글)

**Parameters**:
```csharp
[Parameter] public bool Collapsed { get; set; }
[Parameter] public string Comment { get; set; } = string.Empty;
[Parameter] public EventCallback Toggle { get; set; }
```

**코드**:
```razor
@if (!string.IsNullOrEmpty(Comment))
{
    <div class="row mb-3">
        @if (!Collapsed)
        {
            <h5 class="form-label mb-1">
                합의 내역
                <span @onclick="@Toggle" class="oi oi-plus ms-2" style="cursor: pointer;"></span>
            </h5>
        }
        else
        {
            <h5 class="form-label mb-1">
                합의 내역
                <span @onclick="@Toggle" class="oi oi-minus ms-2" style="cursor: pointer;"></span>
            </h5>
        }
    </div>

    @if (Collapsed)
    {
        <hr />
        <div class="alert alert-info" style="white-space: pre-wrap;">
            @((MarkupString)ReplaceNewLine(Comment))
        </div>
    }
    <hr />
}

@code {
    private string ReplaceNewLine(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        return text.Replace("\n", "<br />");
    }
}
```

---

## 5. 폴더 구조

```
MdcHR26Apps.BlazorServer/
└── Components/
    └── Pages/
        └── Components/
            ├── Table/
            │   ├── AgreementDbListTable.razor
            │   ├── AgreementDetailsTable.razor
            │   └── AgreementListTable.razor
            ├── ViewPage/
            │   └── AgreementDbListView.razor
            ├── Modal/
            │   └── AgreementDeleteModal.razor
            └── CommonView/
                └── AgreementComment.razor
```

---

## 6. 로컬 모델 (필요시)

**경로**: `MdcHR26Apps.BlazorServer/Models/AgreementWithUserInfo.cs`

```csharp
namespace MdcHR26Apps.BlazorServer.Models;

public class AgreementWithUserInfo
{
    public long Aid { get; set; }
    public long Uid { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string EDepartmentName { get; set; } = string.Empty;
    public int Item_Number { get; set; }
    public string Item_Title { get; set; } = string.Empty;
    public string Item_Contents { get; set; } = string.Empty;
    public int Item_Proportion { get; set; }
}
```

---

## 7. 테스트 계획

### 테스트 시나리오 1: AgreementDbListTable
1. Agreement 데이터 3개 준비
2. AgreementDbListTable 컴포넌트 렌더링
3. **확인**: 3개 행 표시 ✅
4. "상세" 버튼 클릭
5. **확인**: OnDetails 이벤트 발생 ✅

### 테스트 시나리오 2: AgreementComment (Collapsed)
1. Comment 데이터 준비
2. Collapsed = false로 렌더링
3. **확인**: "+" 아이콘 표시, 내용 숨김 ✅
4. "+" 클릭
5. **확인**: "-" 아이콘으로 변경, 내용 표시 ✅

### 테스트 시나리오 3: AgreementDeleteModal
1. Show = true, Agreement 데이터 설정
2. 모달 렌더링
3. **확인**: 직무 정보 표시 ✅
4. "삭제" 버튼 클릭
5. **확인**: OnConfirm 이벤트 발생 ✅

---

## 8. 주의사항

1. **2025년 대비 필드명 변경**:
   - `Report_Item_Name_1` → `Item_Title`
   - `Report_Item_Name_2` → `Item_Contents`
   - `Report_Item_Proportion` → `Item_Proportion`

2. **네임스페이스**:
   - `@using MdcHR26Apps.Models.AgreementDb` 추가 필요

3. **반응형 디자인**:
   - Bootstrap 5.x 그리드 시스템 사용
   - 모바일/데스크톱 대응

4. **접근성**:
   - 버튼에 aria-label 추가 (필요시)
   - 테이블 헤더 명확히

5. **스타일**:
   - 기존 2026년 프로젝트 스타일과 일관성 유지

---

## 9. 완료 조건

- [ ] AgreementDbListTable.razor 작성 완료
- [ ] AgreementDetailsTable.razor 작성 완료
- [ ] AgreementListTable.razor 작성 완료
- [ ] AgreementDbListView.razor 작성 완료
- [ ] AgreementDeleteModal.razor 작성 완료
- [ ] AgreementComment.razor 작성 완료
- [ ] 로컬 모델 (AgreementWithUserInfo.cs) 작성 완료
- [ ] 테스트 시나리오 1 성공
- [ ] 테스트 시나리오 2 성공
- [ ] 테스트 시나리오 3 성공
- [ ] 빌드 오류 0개

---

## 10. 다음 단계

**다음 작업지시서**: [20260203_06_components_subagreement.md](20260203_06_components_subagreement.md)
- SubAgreement 관련 컴포넌트 구현

---

## 11. 관련 문서

**이슈**: [#009: Phase 3 WebApp 개발](../issues/009_phase3_webapp_development.md)
**Phase 2 Model**: [AgreementDb](../../MdcHR26Apps.Models/AgreementDb/)
**참조 프로젝트**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Components`
