# 작업지시서 (상세): Agreement/User/Edit 페이지 재작성

**날짜**: 2026-02-11
**파일**:
- `MdcHR26Apps.BlazorServer/Components/Pages/Agreement/User/Edit.razor`
- `MdcHR26Apps.BlazorServer/Components/Pages/Agreement/User/Edit.razor.cs`
**작업 유형**: 버그 수정 / 기능 추가
**관련 이슈**: [#015: Agreement TeamLeader 임의 코드 작성 문제](../issues/015_agreement_teamleader_arbitrary_code_generation.md)
**관련 작업지시서**:
- `20260204_02_phase3_4_agreement_pages.md` (최초 생성)
- `20260204_11_agreement_teamleader_details_fix_approval_workflow.md` (이슈 #015 해결)

---

## 1. 문제 상황

### 1.1. 증상
- Agreement/User/Edit 페이지 접속 시 "Loading..." 무한 로딩
- 브라우저 콘솔에 `System.ObjectDisposedException` 오류 발생
- 평가비중 수정 기능 누락
- 25년도 코드와 완전히 다른 구조로 작성됨

### 1.2. 테스트 결과
**URL**: `http://localhost:5132/Agreement/User/Edit/20002`

**오류 로그**:
```
fail: Microsoft.AspNetCore.Components.Server.Circuits.CircuitHost[111]
      Unhandled exception in circuit '_D2Uac7i8xZgbeoGIYC5F6SdGh9aAlcA2GYKg9yelB4'.
      System.ObjectDisposedException: Cannot access a disposed object.
         at Microsoft.AspNetCore.Components.RenderTree.RenderTreeDiffBuilder.InsertNewFrame(DiffContext& diffContext, Int32 newFrameIndex)
```

**재현 방법**:
1. 로그인 (test4 / Xnd0580+)
2. 직무평가 > 직무작성 메뉴 클릭
3. 임의의 협의 항목에서 "수정" 버튼 클릭
4. Loading... 무한 대기 → 에러 발생

### 1.3. 25년도 vs 26년도 비교

| 구성 요소 | 25년도 (정상) | 26년도 (현재) | 상태 |
|----------|--------------|--------------|------|
| **UI 구조** | FormSelectList + FormSelectNumber | FormAgreeTask (잘못됨) | ❌ |
| **평가비중 수정** | FormSelectNumber로 직접 수정 | 없음 | ❌ |
| **maxperoportion** | 계산 로직 있음 | 없음 | ❌ |
| **HandlePeroportionChanged** | 있음 | 없음 | ❌ |
| **EditUserAgreement** | 상세 검증 로직 | 간단한 HandleValidSubmit만 | ❌ |
| **agreementDblist** | 전체 목록 로드해서 비중 계산 | 없음 | ❌ |

---

## 2. 원인 분석

### 2.1. 정상 작동하는 경우 (25년도)

**호출 흐름**:
```
OnInitializedAsync()
  → SetData(Id)
    → agreementDbRepository.GetByUserIdAllAsync() (전체 목록)
    → agreementDbRepository.GetByIdAsync(Id) (현재 항목)
    → GetMaxVaule(agreementDblist) (사용 가능한 비중 계산)
      → maxperoportion = current + available
```

**코드 위치**: Edit.razor.cs (25년도)
```csharp
private async Task SetData(Int64 Id)
{
    string? sessionUserId = loginStatusService.LoginStatus.LoginUserId;
    if (!String.IsNullOrEmpty(sessionUserId))
    {
        agreementDblist = await agreementDbRepository.GetByUserIdAllAsync(sessionUserId);
    }

    model = await agreementDbRepository.GetByIdAsync(Id);
    if (model != null)
    {
        maxperoportion = model.Report_Item_Proportion;
        if (agreementDblist != null && agreementDblist.Count > 0)
        {
            maxperoportion += GetMaxVaule(agreementDblist);
        }
    }
}
```

### 2.2. 문제가 발생하는 경우 (26년도)

**호출 흐름**:
```
OnInitializedAsync()
  → LoadData()
    → agreementRepository.GetByIdAsync(Id) (현재 항목만)
    → FormAgreeTask 컴포넌트 렌더링 시도
      → ObjectDisposedException 발생 (FormAgreeTask가 Agreement Edit에 부적합)
```

**문제 코드 위치**: Edit.razor (26년도, 라인 18)
```razor
<FormAgreeTask agreement="@model" />
```

**문제점**:
1. **FormAgreeTask 컴포넌트 오용**:
   - FormAgreeTask는 Agreement 생성/상세보기용 컴포넌트
   - Edit 페이지에서는 FormSelectNumber로 비중을 직접 수정해야 함
2. **필수 로직 누락**:
   - `agreementDblist` 로드 안 함 → 사용 가능한 비중 계산 불가
   - `maxperoportion` 계산 로직 없음
   - `HandlePeroportionChanged` 메서드 없음
   - `EditUserAgreement` 검증 로직 없음
3. **25년도 코드 무시**:
   - 이슈 #015와 동일한 패턴 (임의 코드 작성)

---

## 3. 수정 내용

### 3.1. Edit.razor 전체 재작성

**위치**: 전체 파일

**현재 코드** (26년도):
```razor
@page "/Agreement/User/Edit/{Id:long}"
@using MdcHR26Apps.BlazorServer.Components.Pages.Components.Form

<PageTitle>직무평가 협의 수정</PageTitle>

<h3>직무평가 협의 수정</h3>
<hr />

@if (model == null)
{
    <LoadingIndicator />
}
else
{
    <EditForm Model="@model" OnValidSubmit="@HandleValidSubmit">
        <DataAnnotationsValidator />

        <FormAgreeTask agreement="@model" />

        @if (!string.IsNullOrEmpty(errorMessage))
        {
            <div class="alert alert-danger mt-3">
                @errorMessage
            </div>
        }

        @if (!string.IsNullOrEmpty(successMessage))
        {
            <div class="alert alert-success mt-3">
                @successMessage
            </div>
        }

        <hr />
        <div class="row">
            <div class="col-md-12">
                <button type="submit" class="btn btn-primary">저장</button>
                <button type="button" class="btn btn-outline-secondary" @onclick="Cancel">취소</button>
            </div>
        </div>
    </EditForm>
}
```

**수정 후** (25년도 기반):
```razor
@page "/Agreement/User/Edit/{Id:long}"
@using MdcHR26Apps.BlazorServer.Components.Pages.Components.Form
@using MdcHR26Apps.BlazorServer.Components.Pages.Components.Common

<PageTitle>직무 수정</PageTitle>

<h3>직무 수정</h3>

@if (model == null)
{
    <LoadingIndicator />
}
else
{
    <div class="row">
        <div class="col-md-12">
            <EditForm Model="model">
                <FormSelectList Label="평가지표" BindValue="@model.Report_Item_Name_1" IsDisabled="true" />
                <FormSelectList Label="평가직무" BindValue="@model.Report_Item_Name_2" IsDisabled="true" />
                <FormSelectNumber Label="평가비중"
                                  peroportion="@model.Report_Item_Proportion"
                                  maxperoportion="@maxperoportion"
                                  PeroportionChanged="HandlePeroportionChanged"
                                  IsEdit="true" />
                <div>
                    <small>설정가능한 비중 : 0 % ~ @maxperoportion %</small>
                </div>
                <div class="input-group mb-1">
                    <button id="editbutton" class="btn btn-primary" @onclick="EditUserAgreement">변경</button>
                    <button id="movepageutton" class="btn btn-light" @onclick="MoveUserAgreementMainPage">목록</button>
                </div>
            </EditForm>
        </div>
    </div>
    <DisplayResultText Comment="@resultText" />
}
```

### 3.2. Edit.razor.cs 전체 재작성

**위치**: 전체 파일

**현재 코드** (26년도):
```csharp
using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.EvaluationAgreement;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Agreement.User
{
    public partial class Edit
    {
        #region Parameters
        [Parameter]
        public Int64 Id { get; set; }
        #endregion

        [Inject]
        public UrlActions urlActions { get; set; } = null!;

        [Inject]
        public LoginStatusService loginStatusService { get; set; } = null!;

        [Inject]
        public IAgreementRepository agreementRepository { get; set; } = null!;

        public AgreementDb? model { get; set; }
        public string errorMessage { get; set; } = string.Empty;
        public string successMessage { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await CheckLogined();
            await LoadData();
            await base.OnInitializedAsync();
        }

        private async Task LoadData()
        {
            model = await agreementRepository.GetByIdAsync(Id);

            if (model == null)
            {
                errorMessage = "데이터를 찾을 수 없습니다.";
                return;
            }

            var loginUser = loginStatusService.LoginStatus;
            if (model.Uid != loginUser.LoginUid)
            {
                errorMessage = "수정 권한이 없습니다.";
                StateHasChanged();
                await Task.Delay(2000);
                urlActions.MoveAgreementUserIndexPage();
            }
        }

        private async Task CheckLogined()
        {
            await Task.Delay(0);
            if (!loginStatusService.IsloginCheck())
            {
                StateHasChanged();
                urlActions.MoveLoginPage();
            }
        }

        private async Task HandleValidSubmit()
        {
            if (model == null)
            {
                errorMessage = "데이터가 없습니다.";
                return;
            }

            try
            {
                errorMessage = string.Empty;
                successMessage = string.Empty;

                var result = await agreementRepository.UpdateAsync(model);
                if (result)
                {
                    successMessage = "직무평가 협의가 성공적으로 수정되었습니다.";
                    StateHasChanged();
                    await Task.Delay(1000);
                    urlActions.MoveAgreementUserIndexPage();
                }
                else
                {
                    errorMessage = "수정에 실패했습니다.";
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"오류가 발생했습니다: {ex.Message}";
            }
        }

        protected void Cancel()
        {
            urlActions.MoveAgreementUserIndexPage();
        }
    }
}
```

**수정 후** (25년도 기반 + 26년도 네임스페이스):
```csharp
using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.EvaluationAgreement;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Agreement.User
{
    public partial class Edit
    {
        #region Parameters
        [Parameter]
        public Int64 Id { get; set; }
        #endregion

        // 로그인관리(상태관리)
        [Inject]
        public LoginStatusService loginStatusService { get; set; } = null!;

        // 페이지이동
        [Inject]
        public UrlActions urlActions { get; set; } = null!;

        // 직무협의관리
        [Inject]
        public IAgreementRepository agreementRepository { get; set; } = null!;
        public AgreementDb model { get; set; } = new AgreementDb();
        public List<AgreementDb> agreementDblist { get; set; } = new List<AgreementDb>();

        // 기타
        public string resultText { get; set; } = String.Empty;
        public int maxperoportion { get; set; } = 0;

        protected override async Task OnInitializedAsync()
        {
            await CheckLogined();
            await SetData(Id);
            await base.OnInitializedAsync();
        }

        private async Task SetData(Int64 Id)
        {
            var loginUser = loginStatusService.LoginStatus;
            long sessionUid = loginUser.LoginUid;

            // 전체 Agreement 목록 로드 (비중 계산용)
            agreementDblist = await agreementRepository.GetByUidAllAsync(sessionUid);

            // 현재 수정할 Agreement 로드
            model = await agreementRepository.GetByIdAsync(Id);

            if (model != null)
            {
                // 현재 비중 + 사용 가능한 비중 = 최대 설정 가능 비중
                maxperoportion = model.Report_Item_Proportion;
                if (agreementDblist != null && agreementDblist.Count > 0)
                {
                    maxperoportion += GetMaxVaule(agreementDblist);
                }
            }
        }

        #region + CheckLogined : 로그인 체크
        /// <summary>
        /// 로그인 체크
        /// </summary>
        private async Task CheckLogined()
        {
            await Task.Delay(0);
            if (!loginStatusService.IsloginCheck())
            {
                StateHasChanged();
                urlActions.MoveLoginPage();
            }
        }
        #endregion

        /// <summary>
        /// 사용가능한 비중을 구하는 메서드
        /// </summary>
        /// <param name="lists">전체 Agreement 목록</param>
        /// <returns>현재 설정된 비중 외 메서드</returns>
        private int GetMaxVaule(List<AgreementDb> lists)
        {
            int defalutVaule = 100;
            if (lists != null && lists.Count != 0)
            {
                foreach (var item in lists)
                {
                    defalutVaule = defalutVaule - item.Report_Item_Proportion;
                }
            }

            return defalutVaule;
        }

        /// <summary>
        /// FormSelectNumber에서 비중 변경 시 호출
        /// </summary>
        private void HandlePeroportionChanged(int newValue)
        {
            model.Report_Item_Proportion = newValue;
        }

        #region + MoveUserAgreementMainPage : 직무작성 메인페이지 이동
        public void MoveUserAgreementMainPage()
        {
            urlActions.MoveAgreementUserIndexPage();
        }
        #endregion

        #region + 직무수정 : EditUserAgreement
        private async Task EditUserAgreement()
        {
            // 비중 0% 검증
            if (model.Report_Item_Proportion == 0)
            {
                resultText = "직무 비중은 0 % 로 설정할 수 없습니다. 다시 설정해주세요.";
                return;
            }

            // 최대 직무 비중 이상 설정 금지
            model.Report_Item_Proportion = model.Report_Item_Proportion > maxperoportion
                ? maxperoportion
                : model.Report_Item_Proportion;

            if (await agreementRepository.UpdateAsync(model))
            {
                resultText = "평가 수정에 성공하였습니다.";
                StateHasChanged();
                // 평가메인페이지 이동
                urlActions.MoveAgreementUserIndexPage();
            }
            else
            {
                resultText = "평가 수정에 실패하였습니다.";
            }
        }
        #endregion
    }
}
```

### 3.3. 수정 요약

**변경 대상**: 2개 파일 (Edit.razor, Edit.razor.cs)

**주요 변경 사항**:
1. **UI 구조 변경** (Edit.razor):
   - FormAgreeTask 제거 → FormSelectList + FormSelectNumber 사용
   - OnValidSubmit 제거 → @onclick="EditUserAgreement" 사용
   - DisplayResultText 추가

2. **비중 계산 로직 추가** (Edit.razor.cs):
   - `agreementDblist` 필드 추가
   - `maxperoportion` 필드 추가
   - `GetMaxVaule()` 메서드 추가 (사용 가능한 비중 계산)
   - `SetData()` 메서드에서 전체 목록 로드 및 비중 계산

3. **핸들러 메서드 추가**:
   - `HandlePeroportionChanged()` - FormSelectNumber 비중 변경 시
   - `EditUserAgreement()` - 실제 저장 로직 (검증 포함)
   - `MoveUserAgreementMainPage()` - 목록 페이지 이동

4. **26년도 네임스페이스 유지**:
   - `MdcHR26Apps` (25년도: MdcHR25Apps)
   - `IAgreementRepository` / `AgreementDb`
   - `LoginStatusService.LoginStatus.LoginUid` (25년도: LoginUserId)

---

## 4. 수정 로직 설명

### 4.1. 비중 계산 로직

**동작 원리**:
1. 사용자의 모든 Agreement 목록 로드 (`GetByUidAllAsync`)
2. 각 Agreement의 `Report_Item_Proportion` 합산
3. `100 - 합산값 = 사용 가능한 비중`
4. `현재 항목 비중 + 사용 가능한 비중 = 최대 설정 가능 비중`

**예시**:
- 기존 Agreement 3개: 30%, 40%, 20% (합계 90%)
- 현재 수정 중인 항목: 20%
- 사용 가능한 비중: 100 - 90 = 10%
- 최대 설정 가능 비중: 20 + 10 = 30%
- 사용자는 0% ~ 30% 범위 내에서 수정 가능

### 4.2. FormSelectNumber 양방향 바인딩

**동작 원리**:
1. `peroportion="@model.Report_Item_Proportion"` - 초기값 표시
2. 사용자가 슬라이더 조작 → `PeroportionChanged` 이벤트 발생
3. `HandlePeroportionChanged(newValue)` 호출
4. `model.Report_Item_Proportion = newValue` 업데이트
5. 화면 자동 갱신

### 4.3. 검증 로직

**EditUserAgreement()에서 수행**:
1. **0% 검증**: 비중은 최소 1% 이상
2. **최대값 검증**: maxperoportion 초과 시 자동으로 maxperoportion으로 조정
3. **업데이트 성공/실패** 메시지 표시

### 4.4. 25년도 코드 복사 원칙 준수

**이슈 #015 교훈 적용**:
- 25년도 코드 그대로 복사
- 26년도 변경사항만 수정:
  - `MdcHR25Apps` → `MdcHR26Apps`
  - `LoginUserId` → `LoginUid`
  - `IAgreementDbRepository` → `IAgreementRepository`
  - `GetByUserIdAllAsync` → `GetByUidAllAsync`

---

## 5. 테스트 항목

개발자가 테스트할 항목:

### 5.1. [주요 테스트 시나리오]

**시나리오 1: 정상 비중 수정**
1. 로그인 (test4 / Xnd0580+)
2. 직무평가 > 직무작성 메뉴 클릭
3. 기존 협의 항목 중 하나 선택 → "수정" 버튼 클릭
4. Edit 페이지 로딩 확인
5. **확인**: 평가지표, 평가직무, 평가비중 필드가 정상 표시됨 ✅
6. **확인**: "설정가능한 비중 : 0 % ~ XX %" 메시지 표시됨 ✅
7. 평가비중 슬라이더 조작 (예: 30% → 40%)
8. "변경" 버튼 클릭
9. **확인**: "평가 수정에 성공하였습니다." 메시지 표시 ✅
10. **확인**: 목록 페이지로 자동 이동 ✅
11. **확인**: 수정된 비중이 반영되어 있음 ✅

**시나리오 2: 비중 0% 설정 시도 (검증)**
1. Agreement Edit 페이지 접속
2. 평가비중을 0%로 설정
3. "변경" 버튼 클릭
4. **확인**: "직무 비중은 0 % 로 설정할 수 없습니다." 오류 메시지 표시 ✅
5. **확인**: 저장되지 않고 Edit 페이지 유지 ✅

**시나리오 3: 최대 비중 초과 시도 (자동 조정)**
1. Agreement Edit 페이지 접속
2. 설정가능한 비중이 20%라고 가정
3. 평가비중을 50%로 설정 (최대값 초과)
4. "변경" 버튼 클릭
5. **확인**: 자동으로 20%로 조정되어 저장됨 ✅
6. **확인**: "평가 수정에 성공하였습니다." 메시지 표시 ✅

**시나리오 4: 목록 버튼 클릭**
1. Agreement Edit 페이지 접속
2. "목록" 버튼 클릭
3. **확인**: 변경사항 저장 없이 목록 페이지로 이동 ✅

### 5.2. 다양한 조건 테스트

| 현재 비중 | 기존 합계 | 사용 가능 비중 | 최대 설정 가능 | 테스트 입력 | 예상 결과 |
|---------|---------|--------------|--------------|-----------|----------|
| 20%     | 80%     | 20%          | 40%          | 30%       | 성공 (30% 저장) |
| 20%     | 80%     | 20%          | 40%          | 50%       | 성공 (40%로 자동 조정) |
| 30%     | 70%     | 30%          | 60%          | 0%        | 실패 (0% 불가) |
| 10%     | 90%     | 10%          | 20%          | 15%       | 성공 (15% 저장) |

### 5.3. 회귀 테스트

**영향받을 수 있는 기능**:
- Agreement/User/Index (목록) - 정상 작동 확인
- Agreement/User/Create (생성) - 정상 작동 확인
- Agreement/User/Delete (삭제) - 정상 작동 확인
- Agreement/User/Details (상세) - 정상 작동 확인

### 5.4. 로그 확인

브라우저 콘솔에서 확인:
- ObjectDisposedException 오류 없어야 함 ✅
- 기타 JavaScript 오류 없어야 함 ✅

---

## 6. 예상 결과

### 수정 전
- Loading... 무한 로딩 ❌
- ObjectDisposedException 오류 발생 ❌
- 평가비중 수정 불가 ❌
- FormAgreeTask 잘못된 사용 ❌

### 수정 후
- 정상 로딩 및 화면 표시 ✅
- 평가비중 수정 가능 (슬라이더 + 입력) ✅
- 비중 검증 로직 작동 (0% 방지, 최대값 자동 조정) ✅
- 25년도와 동일한 UX ✅

---

## 7. 주의사항

1. **25년도 코드 복사 원칙**: 임의로 수정하지 말고 25년도 코드를 기반으로 작성
2. **네임스페이스 변경**: MdcHR25Apps → MdcHR26Apps
3. **Entity 이름 변경**:
   - `LoginUserId` → `LoginUid`
   - `GetByUserIdAllAsync` → `GetByUidAllAsync`
4. **FormAgreeTask 사용 금지**: Agreement Edit에서는 사용하지 않음
5. **비중 계산 로직 필수**: agreementDblist를 로드해서 maxperoportion 계산

---

## 8. 관련 코드 참조

### 8.1. IAgreementRepository 메서드

**GetByUidAllAsync**:
```csharp
Task<List<AgreementDb>> GetByUidAllAsync(long uid);
```
- 특정 사용자의 모든 Agreement 목록 반환

**GetByIdAsync**:
```csharp
Task<AgreementDb?> GetByIdAsync(long aid);
```
- 특정 Agreement 조회

**UpdateAsync**:
```csharp
Task<bool> UpdateAsync(AgreementDb agreement);
```
- Agreement 업데이트

### 8.2. FormSelectNumber 컴포넌트 사용법

```razor
<FormSelectNumber Label="평가비중"
                  peroportion="@model.Report_Item_Proportion"
                  maxperoportion="@maxperoportion"
                  PeroportionChanged="HandlePeroportionChanged"
                  IsEdit="true" />
```

**파라미터**:
- `peroportion`: 현재 비중 값
- `maxperoportion`: 최대 설정 가능 비중
- `PeroportionChanged`: 값 변경 시 호출할 이벤트 핸들러
- `IsEdit`: 수정 모드 (슬라이더 + 입력 필드 표시)

---

## 9. 완료 조건

- [ ] Edit.razor 파일 25년도 기반 재작성 완료
- [ ] Edit.razor.cs 파일 25년도 기반 재작성 완료
- [ ] 테스트 시나리오 1 성공 (정상 비중 수정)
- [ ] 테스트 시나리오 2 성공 (0% 검증)
- [ ] 테스트 시나리오 3 성공 (최대값 자동 조정)
- [ ] 테스트 시나리오 4 성공 (목록 버튼)
- [ ] 회귀 테스트 성공 (다른 Agreement 페이지들)
- [ ] 브라우저 콘솔 오류 없음
- [ ] 실제 동작 확인 완료

---

## 10. 추가 고려사항

### 10.1. 이슈 #015 교훈

**교훈**: 25년도 코드를 임의로 수정하지 말고 그대로 복사 후 필요한 부분만 변경

**적용**:
- Agreement/User/Edit 페이지 25년도 코드 100% 복사
- 네임스페이스, Entity 이름만 26년도에 맞게 변경

### 10.2. 향후 유사 작업 시 참조

**다른 페이지 재작성 시**:
1. 25년도 코드 복사
2. 네임스페이스 변경 (MdcHR25Apps → MdcHR26Apps)
3. Repository 이름 변경 (필요 시)
4. Entity 필드명 변경 (DB 변경사항 체크리스트 참조)
5. 작업지시서 작성 및 개발자 검토

---

**작성자**: Claude AI
**검토 필요**: 개발자 승인 후 작업 시작
