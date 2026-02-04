# 작업지시서: Agreement/User/Index 합의요청 기능 추가

**작성일**: 2026-02-04
**작업 타입**: 기능 추가 (복잡)
**참조**: 25년도 `Agreement/User/Index` 페이지

---

## 1. 작업 개요

### 배경
- 현재 26년도 Agreement/User/Index는 단순히 리스트만 표시
- 25년도에는 비중 합계가 100%이면 "합의요청" 기능 제공
- ProcessDb를 통한 워크플로우 상태 관리 필요

### 목표
Agreement/User/Index 페이지에 25년도와 동일한 합의요청 기능 추가

---

## 2. 수정 대상 파일

### 수정할 파일
- `MdcHR26Apps.BlazorServer/Components/Pages/Agreement/User/Index.razor`
- `MdcHR26Apps.BlazorServer/Components/Pages/Agreement/User/Index.razor.cs`

### 참조 파일 (25년도)
- `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages\Agreement\User\Index.razor`
- `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages\Agreement\User\Index.razor.cs`

### 의존성 확인
- ✅ `ProcessDb` 엔티티 존재
- ✅ `IProcessRepository` 인터페이스 존재
- ✅ Repository 메서드: `GetByUidAsync()`, `UpdateAsync()`

---

## 3. 구현 상세

### 3.1. Index.razor.cs 수정

#### 추가할 의존성 주입
```csharp
// ProcessDb Repository
[Inject]
public IProcessRepository processRepository { get; set; } = null!;
```

#### 추가할 필드
```csharp
// ProcessDb 상태
public ProcessDb processDb { get; set; } = new();

// 상태 플래그
public bool IsRequest { get; set; } = false;
public bool IsAgreement { get; set; } = false;
public string Agreement_Comment { get; set; } = string.Empty;

// 비중 관련
public int sumperoportion { get; set; } = 0;

// 직무 개수 제한
public int agreementItemMaxCount { get; set; } = 5;

// 평가자 설정 여부
public bool IsTeamLeaderAndDirectorSettings { get; set; } = false;

// 결과 메시지
public string resultText { get; set; } = string.Empty;

// 펼쳐보기
public bool Collapsed { get; set; } = true;
public bool AgreementCollapsed { get; set; } = false;
```

#### 수정할 메서드: LoadData()
```csharp
private async Task LoadData()
{
    var loginUser = loginStatusService.LoginStatus;

    // ProcessDb 로드
    processDb = await processRepository.GetByUidAsync(loginUser.LoginUid) ?? new ProcessDb();
    IsRequest = processDb.Is_Request;
    IsAgreement = processDb.Is_Agreement;
    Agreement_Comment = processDb.Agreement_Comment ?? string.Empty;
    IsTeamLeaderAndDirectorSettings = GetTeamLeaderAndDirectorSettings(processDb);

    // Agreement 리스트 로드
    agreements = await agreementRepository.GetByUserIdAllAsync(loginUser.LoginUid);

    // 비중 합계 계산
    if (agreements != null && agreements.Count > 0)
    {
        setSumPeroportion(agreements);
    }
}
```

#### 추가할 메서드

**1. 비중 합계 계산**
```csharp
/// <summary>
/// 직무 비중 총합을 구하는 메서드
/// </summary>
private void setSumPeroportion(List<AgreementDb> lists)
{
    sumperoportion = 0;
    if (lists != null && lists.Count != 0)
    {
        foreach (var item in lists)
        {
            sumperoportion += item.Report_Item_Proportion;
        }
    }
}
```

**2. 평가자 설정 확인**
```csharp
/// <summary>
/// 부서장 & 임원 설정여부를 확인하는 메서드
/// </summary>
private bool GetTeamLeaderAndDirectorSettings(ProcessDb processDb)
{
    return processDb.TeamLeaderId.HasValue && processDb.TeamLeaderId.Value > 0 &&
           processDb.DirectorId.HasValue && processDb.DirectorId.Value > 0;
}
```

**3. 중복 체크**
```csharp
/// <summary>
/// 중복된 값 찾기
/// </summary>
private bool IsDuplicated(List<AgreementDb> list)
{
    var isDuplicated = list.GroupBy(x => x.Report_Item_Name_2)
                           .Where(g => g.Count() > 1)
                           .Select(g => g.Key)
                           .ToList();
    return isDuplicated.Count > 0;
}
```

**4. 합의요청**
```csharp
/// <summary>
/// 합의요청
/// </summary>
protected async Task SetRequest()
{
    processDb.Is_Request = true;

    int status = await processRepository.UpdateAsync(processDb);

    if (status > 0)
    {
        IsRequest = processDb.Is_Request;
        resultText = "합의요청이 완료되었습니다.";
        StateHasChanged();
    }
    else
    {
        resultText = "합의요청 실패!";
    }
}
```

**5. 요청취소**
```csharp
/// <summary>
/// 요청취소
/// </summary>
protected async Task SetRequestCancel()
{
    processDb.Is_Request = false;

    int status = await processRepository.UpdateAsync(processDb);

    if (status > 0)
    {
        IsRequest = processDb.Is_Request;
        resultText = "요청이 취소되었습니다.";
        StateHasChanged();
    }
    else
    {
        resultText = "요청취소 실패!";
    }
}
```

**6. 토글 메서드**
```csharp
/// <summary>
/// Toggle 이벤트
/// </summary>
private void Toggle()
{
    Collapsed = !Collapsed;
}

/// <summary>
/// AgreementToggle 이벤트
/// </summary>
private void AgreementToggle()
{
    AgreementCollapsed = !AgreementCollapsed;
}
```

**7. Navigation 메서드 수정**

기존 `HandleEdit` 삭제하고 `HandleDetails` 추가:

```csharp
/// <summary>
/// 상세 페이지로 이동
/// </summary>
protected void HandleDetails(long id)
{
    urlActions.MoveAgreementUserDetailsPage(id);
}
```

---

### 3.2. Index.razor 수정

#### UI 구조 (25년도 참조)

```razor
@page "/Agreement/User"
@page "/Agreement/User/Index"
@using MdcHR26Apps.BlazorServer.Components.Pages.Components.Agreement
@using MdcHR26Apps.BlazorServer.Components.Pages.Components.Agreement.CommonView
@using MdcHR26Apps.BlazorServer.Components.Pages.Components.Agreement.ViewPage

<PageTitle>직무평가 협의</PageTitle>

<h3>직무평가 협의</h3>

@* 결과 메시지 표시 *@
@if (!string.IsNullOrEmpty(resultText))
{
    <div class="alert alert-info">@resultText</div>
}

@if (processDb == null)
{
    <p><em>Loading...</em></p>
}
else
{
    @* 평가자 설정여부 확인 *@
    @if (!IsTeamLeaderAndDirectorSettings)
    {
        <div class="alert alert-warning">
            <p><em>2차 평가자와 3차 평가자가 설정되지 않았습니다.</em></p>
        </div>
    }
    @* 평가요청 X && 합의 X *@
    else if (!IsRequest && !IsAgreement)
    {
        @* 비중 합이 100%가 아니면 *@
        @if (sumperoportion != 100)
        {
            @if (agreements != null && agreements.Count >= agreementItemMaxCount)
            {
                <hr />
                <p><em>총 평가비중 : @sumperoportion %</em></p>
                <p><em>총 평가비중이 100 % 되어야 합니다.</em></p>
                <p><em>직무 수 : @agreements.Count 개(최대 : @agreementItemMaxCount)</em></p>
                <p><em>최대 직무 수(@agreementItemMaxCount 개) 이내로 작성해 주세요.</em></p>
            }
            else
            {
                <div class="mb-3">
                    <button class="btn btn-primary" @onclick="CreateAgreement">새 협의 추가</button>
                </div>
                <hr />
                <p><em>직무추가 버튼을 클릭하여 직무를 작성하여 주세요.</em></p>
                <p><em>총 평가비중 : @sumperoportion %</em></p>
                <p><em>총 평가비중이 100 % 되어야 합니다.</em></p>
                <p><em>직무 수 : @agreements.Count 개(최대 : @agreementItemMaxCount)</em></p>
            }
        }
        @* 비중 합이 100%이면 *@
        else
        {
            @if (agreements != null && agreements.Count > agreementItemMaxCount)
            {
                <p><em>직무 작성은 최대 @agreementItemMaxCount 까지 가능합니다.</em></p>
                <p><em>직무 수를 조정해 주세요.</em></p>
            }
            else if (IsDuplicated(agreements))
            {
                <p><em>직무 작성은 중복으로 제출할 수 없습니다.</em></p>
                <p><em>중복된 직무를 삭제하여 주세요.</em></p>
            }
            else
            {
                <div class="mb-3">
                    <button class="btn btn-primary" @onclick="SetRequest">합의요청</button>
                </div>
                <hr />
                <p><em>총 평가비중 : @sumperoportion % - 상단의 합의요청 버튼을 클릭하여 주세요.</em></p>
            }
        }
    }
    @* 평가요청 O && 합의 X *@
    else if (IsRequest && !IsAgreement)
    {
        <div class="mb-3">
            <button class="btn btn-danger" @onclick="SetRequestCancel">요청취소</button>
        </div>
        <hr />
        <p><em>합의여부를 대기중입니다...</em></p>
    }
    @* 평가요청 O && 합의 O *@
    else if (IsRequest && IsAgreement)
    {
        <div class="alert alert-success">
            <p><em>직무합의가 완료되었습니다.</em></p>
            <p><em>세부 직무를 작성하여 주십시오.</em></p>
        </div>
    }
}

<hr />

@* 합의 코멘트 (펼쳐보기) - AgreementComment 컴포넌트 사용 *@
<AgreementComment Collapsed="@Collapsed" Comment="@Agreement_Comment" Toggle="@Toggle" />

<hr />

@* 직무 리스트 *@
@if (agreements == null)
{
    <p><em>Loading...</em></p>
}
else if (agreements.Count == 0)
{
    <h5>직무 수 ( 현재 : 0 개 )</h5>
    <p><em>최대 직무 수 : @agreementItemMaxCount 개</em></p>
    <hr />
}
else
{
    @* 합의요청(X) && 합의승인(X) - 편집 가능 *@
    @if (!IsRequest && !IsAgreement)
    {
        <AgreementDbListTable agreementDbs="@agreements" OnDetailsClick="@HandleDetails" />
    }
    @* 합의 진행 중 또는 완료 - 읽기 전용 *@
    else
    {
        @if (IsRequest && !IsAgreement)
        {
            <p><em>합의여부를 대기중에는 직무 수정이 불가합니다.</em></p>
        }
        else if (IsRequest && IsAgreement)
        {
            <p><em>합의가 완료되면 직무 수정이 불가합니다.</em></p>
        }
        <hr />

        @* 합의내역 펼쳐보기 *@
        <div class="row">
            <div class="col">
                <h5 @onclick="AgreementToggle" style="cursor: pointer;">
                    합의 내역 리스트
                    @if (AgreementCollapsed)
                    {
                        <span class="oi oi-minus float-end" />
                    }
                    else
                    {
                        <span class="oi oi-plus float-end" />
                    }
                </h5>
            </div>
        </div>
        @if (AgreementCollapsed)
        {
            <hr />
            <AgreementDbListView agreements="@agreements" />
        }
    }
}

<hr />
<div class="row">
    <div class="col-md-12">
        <button type="button" class="btn btn-outline-secondary" @onclick="MoveMainPage">메인</button>
    </div>
</div>
```

---

## 4. 비즈니스 로직

### 4.1. 합의요청 조건
다음 **모든 조건**을 만족해야 "합의요청" 버튼 활성화:

1. ✅ **평가자 설정 완료**: `TeamLeaderId` && `DirectorId` 존재
2. ✅ **비중 합 = 100%**: `sumperoportion == 100`
3. ✅ **직무 개수 ≤ 5개**: `agreements.Count <= agreementItemMaxCount`
4. ✅ **중복 없음**: `!IsDuplicated(agreements)`
5. ✅ **요청 전 상태**: `!IsRequest && !IsAgreement`

### 4.2. 상태 전이 다이어그램

```
작성 중 (편집 가능)
  ↓ [합의요청 버튼]
요청 대기 (편집 불가)
  ↓ [팀리더 승인]
합의 완료 (읽기 전용)

※ 요청 대기 상태에서 [요청취소] 가능 → 작성 중으로 복귀
```

### 4.3. ProcessDb 업데이트

**합의요청 시**:
```csharp
processDb.Is_Request = true;  // 요청 플래그 ON
await processRepository.UpdateAsync(processDb);
```

**요청취소 시**:
```csharp
processDb.Is_Request = false;  // 요청 플래그 OFF
await processRepository.UpdateAsync(processDb);
```

---

## 5. 컴포넌트 사용

### 기존 컴포넌트 활용 (25년도 방식)
- ✅ `<AgreementDbListTable>` - 상세 버튼 제공 (작성 중)
  - 경로: `Components/Agreement/AgreementDbListTable.razor`
  - 파라미터: `agreementDbs`, `OnDetailsClick`
- ✅ `<AgreementDbListView>` - 읽기 전용 리스트 (대기 중/완료)
  - 경로: `Components/Agreement/ViewPage/AgreementDbListView.razor`
  - 파라미터: `agreements`
- ✅ `<AgreementComment>` - 합의 코멘트 펼쳐보기
  - 경로: `Components/Agreement/CommonView/AgreementComment.razor`
  - 파라미터: `Comment`, `Collapsed`, `Toggle`

### 컴포넌트 변경 사항
- ❌ **AgreementListTable** (현재 사용 중) → 사용 중단
- ✅ **AgreementDbListTable** (25년도 방식) → 사용
- 이유: 반응형 디자인, 상세 확인 워크플로우, 25년도 일관성

### DisplayResultText 컴포넌트
- 존재 여부 확인 필요
- 없으면 간단한 div로 대체:
  ```razor
  @if (!string.IsNullOrEmpty(resultText))
  {
      <div class="alert alert-info">@resultText</div>
  }
  ```

---

## 6. 테스트 시나리오

### 6.1. 작성 중 상태
1. 로그인 후 `/Agreement/User/Index` 접속
2. "새 협의 추가" 버튼으로 직무 추가
3. 비중 합이 100% 미만일 때:
   - "새 협의 추가" 버튼 표시
   - 현재 비중 합 표시
4. 비중 합이 100% 초과일 때:
   - 경고 메시지 표시
   - 직무 수 조정 안내

### 6.2. 합의요청 조건 확인
1. 비중 합 = 100%
2. 직무 개수 ≤ 5개
3. 중복 직무 없음
4. 평가자 설정 완료
5. → "합의요청" 버튼 활성화

### 6.3. 합의요청 후
1. "합의요청" 버튼 클릭
2. ProcessDb.Is_Request = true로 업데이트
3. "요청취소" 버튼 표시
4. 직무 수정 불가 (읽기 전용)
5. "합의여부를 대기중입니다..." 메시지

### 6.4. 요청취소
1. "요청취소" 버튼 클릭
2. ProcessDb.Is_Request = false로 업데이트
3. 다시 "새 협의 추가" 버튼 표시
4. 편집 가능 상태로 복귀

### 6.5. 합의 완료 (팀리더 승인 후)
1. ProcessDb.Is_Agreement = true (팀리더가 설정)
2. "직무합의가 완료되었습니다." 메시지
3. 읽기 전용 리스트 표시
4. 편집 불가

---

## 7. 주의사항

### 7.1. 25년도와 차이점
- **네임스페이스**: `MdcHR26Apps` (25년도는 `MdcHR25Apps`)
- **Repository 메서드명**:
  - 26년도: `GetByUidAsync()`, `UpdateAsync()`
  - 25년도: `GetByUserIdAsync()`, `UpdateAsync()` (동일)
- **컴포넌트 경로**: `Components.Pages.Components.Agreement`

### 7.2. DisplayResultText 컴포넌트
- 이미 존재하면 사용
- 없으면 간단한 alert div로 대체

### 7.3. OnAfterRenderAsync vs OnInitializedAsync
- 25년도는 `OnAfterRenderAsync` 사용
- 26년도는 `OnInitializedAsync` 사용 (이미 구현됨)
- **권장**: `OnInitializedAsync` 유지 (더 간단)

---

## 8. 빌드 및 검증

### 8.1. 빌드 명령
```bash
cd "c:\Codes\00_Develop_Cursor\10_MdcHR26Apps\MdcHR26Apps.BlazorServer"
dotnet build
```

### 8.2. 예상 오류
- IProcessRepository DI 누락 → `Program.cs`에서 서비스 등록 확인
- DisplayResultText 컴포넌트 없음 → alert div로 대체

### 8.3. 성공 기준
- ✅ 빌드 성공 (0 에러)
- ✅ 비중 합 100% 시 "합의요청" 버튼 표시
- ✅ 합의요청 후 "요청취소" 버튼 표시
- ✅ 상태별 UI 정상 동작

---

## 9. 완료 체크리스트

- [ ] Index.razor.cs 수정 (IProcessRepository 추가, 메서드 구현)
- [ ] Index.razor 수정 (상태별 UI 구현)
- [ ] DisplayResultText 컴포넌트 확인/대체
- [ ] 빌드 성공 확인
- [ ] 비중 합계 계산 테스트
- [ ] 합의요청 조건 확인 테스트
- [ ] 합의요청/취소 기능 테스트
- [ ] 상태별 UI 표시 테스트

---

## 10. 참고 자료

### 25년도 코드 경로
- `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages\Agreement\User\Index.razor`
- `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages\Agreement\User\Index.razor.cs`

### 26년도 엔티티/Repository
- `MdcHR26Apps.Models\EvaluationProcess\ProcessDb.cs`
- `MdcHR26Apps.Models\EvaluationProcess\IProcessRepository.cs`
- `MdcHR26Apps.Models\EvaluationAgreement\AgreementDb.cs`
- `MdcHR26Apps.Models\EvaluationAgreement\IAgreementRepository.cs`
