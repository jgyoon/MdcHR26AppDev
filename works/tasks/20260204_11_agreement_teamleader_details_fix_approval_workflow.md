# 작업지시서 (상세): TeamLeader/Details 합의 승인/반려 기능 구현

**날짜**: 2026-02-04
**파일**:
- `MdcHR26Apps.BlazorServer/Components/Pages/Agreement/TeamLeader/Details.razor`
- `MdcHR26Apps.BlazorServer/Components/Pages/Agreement/TeamLeader/Details.razor.cs`

**작업 유형**: 기능 추가 / 버그 수정
**관련 이슈**: View Table 기반 Navigation Properties 수정 작업에서 발견된 근본적 기능 누락

---

## 1. 문제 상황

### 1.1. 증상
- 26년도 TeamLeader/Details 페이지가 25년도와 완전히 다른 구조로 구현됨
- 25년도: 합의 승인/반려 기능이 있는 워크플로우 페이지
- 26년도: 단순 조회만 가능한 디스플레이 페이지
- **핵심 기능 누락**: 합의 코멘트 입력, 승인/반려 버튼, ProcessDb 업데이트 로직

### 1.2. 현재 26년도 구현 문제점
1. **Parameter 오류**: `Uid` 파라미터 사용 (올바른 것: `Id` - ProcessDb.Pid)
2. **데이터 로드 오류**: Agreement 1개만 로드 (올바른 것: 해당 사용자의 모든 Agreement)
3. **ProcessDb 미사용**: ProcessDb를 로드하지 않음
4. **승인/반려 기능 없음**: 합의 코멘트 입력 및 승인/반려 버튼이 없음
5. **상태 업데이트 없음**: ProcessDb.Is_Agreement 업데이트 로직 없음

### 1.3. 25년도 정상 동작
**파일**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages\Agreement\TeamLeader\Details.razor`

**동작 흐름**:
1. Index 페이지에서 ProcessDb.Pid (Id)를 전달받음
2. ProcessDb를 로드하여 사용자 정보 확인
3. 해당 사용자의 모든 Agreement 목록 조회
4. 팀리더가 합의 코멘트를 입력
5. "승인" 버튼 클릭 → ProcessDb.Is_Agreement = true, 코멘트에 [승인] 태그 추가
6. "반려" 버튼 클릭 → ProcessDb.Is_Request = false, 코멘트에 [반려] 태그 추가
7. ProcessDb 업데이트 후 Index 페이지로 리다이렉트

---

## 2. 원인 분석

### 2.1. 정상 작동하는 25년도 코드

**파일**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages\Agreement\TeamLeader\Details.razor.cs`

**호출 흐름**:
```
OnInitializedAsync()
  → CheckLogined()
  → SetData(Id)  // Id는 ProcessDb.Pid
    → processDbRepository.GetByIdAsync(Id)  // ProcessDb 로드
    → agreementDbRepository.GetByUserIdAllAsync(processDb.UserId)  // 모든 Agreement 로드
```

**핵심 코드** (라인 13-66):
```csharp
[Parameter]
public Int64 Id { get; set; }  // ProcessDb의 Pid

[Inject]
public IProcessDbRepository processDbRepository { get; set; } = null!;

[Inject]
public IAgreementDbRepository agreementDbRepository { get; set; } = null!;

public ProcessDb processDb { get; set; } = new ProcessDb();
public List<AgreementDb> model { get; set; } = new List<AgreementDb>();
public string Old_Agreement_Comment { get; set; } = String.Empty;

private async Task SetData(Int64 Id)
{
    processDb = await processDbRepository.GetByIdAsync(Id);  // ProcessDb 로드
    model = await agreementDbRepository.GetByUserIdAllAsync(processDb.UserId);  // 모든 Agreement 로드
    userName = processDb.UserName;

    Old_Agreement_Comment =
        !String.IsNullOrEmpty(processDb.Agreement_Comment) ? processDb.Agreement_Comment : String.Empty;
    if (!String.IsNullOrEmpty(processDb.Agreement_Comment))
    {
        processDb.Agreement_Comment = String.Empty;  // 입력 필드 초기화
    }
}
```

**승인 로직** (라인 84-123):
```csharp
private async Task AgreementConfirm()
{
    if (String.IsNullOrEmpty(processDb.Agreement_Comment))
    {
        resultText = "합의 코멘트를 입력하여 주세요.";
        return;
    }

    if (!String.IsNullOrEmpty(Old_Agreement_Comment))
    {
        processDb.Agreement_Comment =
            Old_Agreement_Comment + "\r\n​"
            + "[승인]" + processDb.Agreement_Comment
            + "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", cultureInfo) + "]";
    }
    else
    {
        processDb.Agreement_Comment =
            "[승인]" + processDb.Agreement_Comment
            + "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", cultureInfo) + "]";
    }
    processDb.Is_Agreement = true;

    bool updateStatus = await processDbRepository.UpdateAsync(processDb);

    if (updateStatus)
    {
        resultText = "합의 승인 성공";
        StateHasChanged();
        urlActions.MoveTeamLeaderAgreementMainPage();
    }
    else
    {
        resultText = "합의 승인 실패";
    }
}
```

### 2.2. 문제가 발생하는 26년도 코드

**파일**: `c:\Codes\00_Develop_Cursor\10_MdcHR26Apps\MdcHR26Apps.BlazorServer\Components\Pages\Agreement\TeamLeader\Details.razor.cs`

**호출 흐름**:
```
OnInitializedAsync()
  → CheckLogined()
  → LoadData()
    → agreementRepository.GetByIdAsync(Id)  // ❌ 잘못된 파라미터, 잘못된 메서드
```

**문제 코드** (라인 10-45):
```csharp
[Parameter]
public Int64 Id { get; set; }  // ❌ 원래는 Uid로 되어 있었음 (수정됨)

[Inject]
public IAgreementRepository agreementRepository { get; set; } = null!;  // ❌ 필요한 것: IProcessRepository

public AgreementDb? model { get; set; }  // ❌ 단일 객체 (필요한 것: List<AgreementDb>)

private async Task LoadData()
{
    model = await agreementRepository.GetByIdAsync(Id);  // ❌ Agreement 1개만 로드

    if (model == null)
    {
        errorMessage = "데이터를 찾을 수 없습니다.";
        return;
    }

    // ❌ ProcessDb를 로드하지 않음
    // ❌ 승인/반려 로직 없음
}
```

**문제점**:
1. ProcessDb를 로드하지 않아 승인/반려 기능 구현 불가
2. Agreement 1개만 조회 (실제로는 사용자의 모든 Agreement 필요)
3. 합의 코멘트 입력 및 승인/반려 버튼 UI 없음
4. ProcessDb 업데이트 로직 없음

---

## 3. 수정 내용

### 3.1. Details.razor.cs 전체 재작성

**위치**: `MdcHR26Apps.BlazorServer/Components/Pages/Agreement/TeamLeader/Details.razor.cs`

**현재 코드** (전체 - 76라인):
```csharp
using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.EvaluationAgreement;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Agreement.TeamLeader
{
    public partial class Details
    {
        #region Parameters
        [Parameter]
        public Int64 Id { get; set; }
        #endregion

        // 페이지이동
        [Inject]
        public UrlActions urlActions { get; set; } = null!;

        // 로그인관리(상태관리)
        [Inject]
        public LoginStatusService loginStatusService { get; set; } = null!;

        // Agreement Repository
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

            // 권한 체크 - 같은 부서 확인
            var loginUser = loginStatusService.LoginStatus;
            if (model.User != null && model.User.EDepartment?.EDepartmentName != loginUser.LoginUserEDepartment)
            {
                errorMessage = "다른 부서의 협의는 조회할 수 없습니다.";
                StateHasChanged();
                await Task.Delay(2000);
                urlActions.MoveAgreementTeamLeaderIndexPage();
            }
        }

        private async Task CheckLogined()
        {
            await Task.Delay(0);
            if (!loginStatusService.IsloginAndIsTeamLeaderCheck())
            {
                errorMessage = "팀장 권한이 필요합니다.";
                StateHasChanged();
                await Task.Delay(2000);
                urlActions.MoveMainPage();
            }
        }

        protected void BackToList()
        {
            urlActions.MoveAgreementTeamLeaderIndexPage();
        }
    }
}
```

**수정 후** (전체):
```csharp
using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.EvaluationAgreement;
using MdcHR26Apps.Models.EvaluationProcess;
using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Agreement.TeamLeader
{
    public partial class Details
    {
        #region Parameters
        [Parameter]
        public Int64 Id { get; set; }  // ProcessDb의 Pid
        #endregion

        // 페이지이동
        [Inject]
        public UrlActions urlActions { get; set; } = null!;

        // 로그인관리(상태관리)
        [Inject]
        public LoginStatusService loginStatusService { get; set; } = null!;

        // ProcessDb Repository
        [Inject]
        public IProcessRepository processRepository { get; set; } = null!;

        // Agreement Repository
        [Inject]
        public IAgreementRepository agreementRepository { get; set; } = null!;

        // ProcessDb
        public ProcessDb processDb { get; set; } = new ProcessDb();

        // Agreement 리스트
        public List<AgreementDb> model { get; set; } = new List<AgreementDb>();

        // 기타
        public string userName { get; set; } = String.Empty;
        public string resultText { get; set; } = String.Empty;

        // 이전 합의내역
        public string Old_Agreement_Comment { get; set; } = String.Empty;
        public CultureInfo cultureInfo { get; set; } = new CultureInfo("ko-KR");

        // 펼쳐보기
        public bool Collapsed { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            await CheckLogined();
            await SetData(Id);
            await base.OnInitializedAsync();
        }

        private async Task SetData(Int64 Id)
        {
            // ProcessDb 로드
            processDb = await processRepository.GetByIdAsync(Id);

            if (processDb == null)
            {
                resultText = "데이터를 찾을 수 없습니다.";
                return;
            }

            // 권한 체크 - 같은 부서 확인
            var loginUser = loginStatusService.LoginStatus;
            if (processDb.User != null &&
                processDb.User.EDepartment?.EDepartmentName != loginUser.LoginUserEDepartment)
            {
                resultText = "다른 부서의 협의는 조회할 수 없습니다.";
                StateHasChanged();
                await Task.Delay(2000);
                urlActions.MoveAgreementTeamLeaderIndexPage();
                return;
            }

            // 해당 사용자의 모든 Agreement 로드 (26년도는 Uid 사용)
            model = await agreementRepository.GetByUserIdAllAsync(processDb.Uid);
            userName = processDb.User?.UserName ?? string.Empty;

            // 이전 합의 코멘트 저장 및 입력 필드 초기화
            Old_Agreement_Comment =
                !String.IsNullOrEmpty(processDb.Agreement_Comment)
                    ? processDb.Agreement_Comment
                    : String.Empty;

            if (!String.IsNullOrEmpty(processDb.Agreement_Comment))
            {
                processDb.Agreement_Comment = String.Empty;
            }
        }

        #region CheckLogined
        /// <summary>
        /// 로그인 체크 && 팀리더 체크
        /// </summary>
        private async Task CheckLogined()
        {
            await Task.Delay(0);
            if (!loginStatusService.IsloginAndIsTeamLeaderCheck())
            {
                StateHasChanged();
                urlActions.MoveMainPage();
            }
        }
        #endregion

        #region 합의 승인
        private async Task AgreementConfirm()
        {
            if (String.IsNullOrEmpty(processDb.Agreement_Comment))
            {
                resultText = "합의 코멘트를 입력하여 주세요.";
                return;
            }

            // 코멘트에 [승인] 태그와 타임스탬프 추가
            if (!String.IsNullOrEmpty(Old_Agreement_Comment))
            {
                processDb.Agreement_Comment =
                    Old_Agreement_Comment + "\r\n​"
                    + "[승인]" + processDb.Agreement_Comment
                    + "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", cultureInfo) + "]";
            }
            else
            {
                processDb.Agreement_Comment =
                    "[승인]" + processDb.Agreement_Comment
                    + "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", cultureInfo) + "]";
            }

            processDb.Is_Agreement = true;

            bool updateStatus = await processRepository.UpdateAsync(processDb);

            if (updateStatus)
            {
                resultText = "합의 승인 성공";
                StateHasChanged();
                urlActions.MoveAgreementTeamLeaderIndexPage();
            }
            else
            {
                resultText = "합의 승인 실패";
            }
        }
        #endregion

        #region 합의 반려
        private async Task AgreementRefer()
        {
            if (String.IsNullOrEmpty(processDb.Agreement_Comment))
            {
                resultText = "합의 코멘트를 입력하여 주세요.";
                return;
            }

            // 코멘트에 [반려] 태그와 타임스탬프 추가
            if (!String.IsNullOrEmpty(Old_Agreement_Comment))
            {
                processDb.Agreement_Comment =
                    Old_Agreement_Comment + "\r\n​"
                    + "[반려]" + processDb.Agreement_Comment
                    + "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", cultureInfo) + "]";
            }
            else
            {
                processDb.Agreement_Comment =
                    "[반려]" + processDb.Agreement_Comment
                    + "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", cultureInfo) + "]";
            }

            processDb.Is_Request = false;

            bool updateStatus = await processRepository.UpdateAsync(processDb);

            if (updateStatus)
            {
                resultText = "합의 반려 성공";
                StateHasChanged();
                urlActions.MoveAgreementTeamLeaderIndexPage();
            }
            else
            {
                resultText = "합의 반려 실패";
            }
        }
        #endregion

        #region Toggle 이벤트
        /// <summary>
        /// 이전 합의 코멘트 펼쳐보기/접기
        /// </summary>
        private void Toggle()
        {
            Collapsed = !Collapsed;
        }
        #endregion
    }
}
```

### 3.2. Details.razor UI 전체 재작성

**위치**: `MdcHR26Apps.BlazorServer/Components/Pages/Agreement/TeamLeader/Details.razor`

**현재 코드** (전체 - 68라인):
```razor
@page "/Agreement/TeamLeader/Details/{Uid:long}"
@using MdcHR26Apps.BlazorServer.Components.Pages.Components.Agreement

<PageTitle>부서원 직무평가 협의 상세</PageTitle>

<h3>부서원 직무평가 협의 상세</h3>
<hr />

@if (!string.IsNullOrEmpty(errorMessage))
{
    <div class="alert alert-danger">
        @errorMessage
    </div>
}

@if (model == null)
{
    <LoadingIndicator />
}
else
{
    @if (!string.IsNullOrEmpty(successMessage))
    {
        <div class="alert alert-success">
            @successMessage
        </div>
    }

    <div class="card mb-3">
        <div class="card-header">
            <strong>작성자 정보</strong>
        </div>
        <div class="card-body">
            <p><strong>이름:</strong> @model.UserName</p>
            <p><strong>부서:</strong> @model.EDepartmentName</p>
            <p><strong>직급:</strong> @model.ERankName</p>
        </div>
    </div>

    <table class="table">
        <tbody>
            <tr>
                <th>평가지표</th>
                <td>@model.Report_Item_Name_1</td>
            </tr>
            <tr>
                <th>평가직무</th>
                <td>@model.Report_Item_Name_2</td>
            </tr>
            <tr>
                <th>직무비중</th>
                <td>@model.Report_Item_Proportion %</td>
            </tr>
            <tr>
                <th>항목번호</th>
                <td>@model.Report_Item_Number</td>
            </tr>
        </tbody>
    </table>

    <hr />
    <div class="row">
        <div class="col-md-12">
            <button type="button" class="btn btn-outline-secondary" @onclick="BackToList">목록</button>
        </div>
    </div>
}
```

**수정 후** (전체):
```razor
@page "/Agreement/TeamLeader/Details/{Id:long}"
@using System.Text.RegularExpressions
@using System.Web
@using MdcHR26Apps.BlazorServer.Components.Pages.Components.Agreement

<PageTitle>직무 합의(상세)</PageTitle>

<h3>직무 합의</h3>
<hr />

@if (!String.IsNullOrEmpty(resultText))
{
    <p><em>@resultText</em></p>
}

@if (processDb == null || processDb.Pid == 0)
{
    <LoadingIndicator />
}
else
{
    <h5>@userName 님 직무평가 합의 </h5>
    <div class="row">
        <div class="col-md-12">
            <EditForm Model="processDb">
                @* 이전 합의 코멘트 *@
                @if (!String.IsNullOrEmpty(Old_Agreement_Comment))
                {
                    <div class="row">
                        @if (!Collapsed)
                        {
                            <h5 for="Old_Agreement_Comment" class="form-label mb-1">
                                이전 합의 내역(Old)
                                <span @onclick="@Toggle" class="oi oi-plus mr-1" />
                            </h5>
                        }
                        else
                        {
                            <h5 for="Old_Agreement_Comment" class="form-label mb-1">
                                이전 합의 내역(Old)
                                <span @onclick="@Toggle" class="oi oi-minus mr-1" />
                            </h5>
                        }
                    </div>
                    @if (Collapsed)
                    {
                        <hr />
                        <div>@((MarkupString)ReplaceNewLine(Old_Agreement_Comment))</div>
                    }
                    <hr />
                }

                <div class="input-group mb-1">
                    <span class="input-group-text mb-1">합의코멘트</span>
                    <InputTextArea id="Agreement_Comment" class="form-control mb-1" rows="5"
                                   @bind-Value="@processDb.Agreement_Comment">
                    </InputTextArea>
                </div>
                <div class="input-group mb-1">
                    <button id="confirmbutton" type="button" class="btn btn-primary" @onclick="AgreementConfirm">승인</button>
                    <button id="referbutton" type="button" class="btn btn-light" @onclick="AgreementRefer">반려</button>
                </div>
            </EditForm>
        </div>
    </div>
}

@if (model == null)
{
    <LoadingIndicator />
}
else if (model.Count == 0)
{
    <p><em>No DATA</em></p>
}
else
{
    <hr />
    <p><em>@userName 님 업무비중</em></p>
    <AgreementDetailsTable agreements="model"></AgreementDetailsTable>
}

@code {
    // https://stackoverflow.com/questions/64157834/how-can-i-have-new-line-in-blazor
    /// <summary>
    /// 웹페이지 줄바꿈 처리하는 메서드
    /// </summary>
    /// <param name="content">string content</param>
    /// <returns>string replaceString</returns>
    private string ReplaceNewLine(string content)
    {
        return Regex.Replace(HttpUtility.HtmlEncode(content), "\\r?\\n|\\r", "<br />");
    }
}
```

### 3.3. 수정 요약

**Details.razor.cs 변경 대상**: 9군데

1. **using 추가** (라인 1-5):
   ```csharp
   // 추가
   using MdcHR26Apps.Models.EvaluationProcess;
   using System.Globalization;
   ```

2. **IProcessRepository 추가** (라인 23-25):
   ```csharp
   // 추가
   [Inject]
   public IProcessRepository processRepository { get; set; } = null!;
   ```

3. **ProcessDb 필드 추가** (라인 30-31):
   ```csharp
   // 기존
   public AgreementDb? model { get; set; }

   // 수정 후
   public ProcessDb processDb { get; set; } = new ProcessDb();
   public List<AgreementDb> model { get; set; } = new List<AgreementDb>();
   ```

4. **추가 필드** (라인 35-45):
   ```csharp
   // 추가
   public string userName { get; set; } = String.Empty;
   public string resultText { get; set; } = String.Empty;
   public string Old_Agreement_Comment { get; set; } = String.Empty;
   public CultureInfo cultureInfo { get; set; } = new CultureInfo("ko-KR");
   public bool Collapsed { get; set; } = false;
   ```

5. **LoadData → SetData 변경** (라인 52-90):
   ```csharp
   // 기존
   private async Task LoadData()
   {
       model = await agreementRepository.GetByIdAsync(Id);
       // ...
   }

   // 수정 후
   private async Task SetData(Int64 Id)
   {
       processDb = await processRepository.GetByIdAsync(Id);
       model = await agreementRepository.GetByUserIdAllAsync(processDb.Uid);
       userName = processDb.User?.UserName ?? string.Empty;
       Old_Agreement_Comment = processDb.Agreement_Comment ?? String.Empty;
       if (!String.IsNullOrEmpty(processDb.Agreement_Comment))
       {
           processDb.Agreement_Comment = String.Empty;
       }
   }
   ```

6. **AgreementConfirm 메서드 추가** (라인 109-139):
   - 승인 로직 구현
   - 코멘트에 [승인] 태그 추가
   - ProcessDb.Is_Agreement = true
   - ProcessDb 업데이트

7. **AgreementRefer 메서드 추가** (라인 142-172):
   - 반려 로직 구현
   - 코멘트에 [반려] 태그 추가
   - ProcessDb.Is_Request = false
   - ProcessDb 업데이트

8. **Toggle 메서드 추가** (라인 175-182):
   - 이전 합의 코멘트 펼쳐보기/접기

9. **BackToList 메서드 삭제** (불필요):
   - 승인/반려 후 자동으로 리다이렉트됨

**Details.razor 변경 대상**: 5군데

1. **@page 경로 수정** (라인 1):
   ```razor
   <!-- 기존 -->
   @page "/Agreement/TeamLeader/Details/{Uid:long}"

   <!-- 수정 후 -->
   @page "/Agreement/TeamLeader/Details/{Id:long}"
   ```

2. **using 추가** (라인 2-4):
   ```razor
   @using System.Text.RegularExpressions
   @using System.Web
   ```

3. **EditForm 추가** (라인 24-63):
   - 이전 합의 코멘트 펼쳐보기
   - InputTextArea (합의 코멘트 입력)
   - 승인/반려 버튼

4. **Agreement 리스트 표시** (라인 65-79):
   ```razor
   <!-- 기존: 단일 Agreement 표시 -->
   <table class="table">...</table>

   <!-- 수정 후: 리스트 표시 -->
   <AgreementDetailsTable agreements="model"></AgreementDetailsTable>
   ```

5. **ReplaceNewLine 메서드 추가** (@code 블록):
   - 줄바꿈 처리를 위한 메서드

---

## 4. 수정 로직 설명

### 4.1. ProcessDb 기반 워크플로우

25년도와 동일한 워크플로우:
1. **Index 페이지**: ProcessDb.Pid를 Details 페이지로 전달
2. **Details 페이지**: ProcessDb.Pid로 ProcessDb 로드
3. **데이터 로드**: ProcessDb.Uid로 모든 Agreement 조회
4. **승인/반려**: ProcessDb 상태 업데이트 후 Index로 리다이렉트

### 4.2. 26년도 DB 변경 반영

**25년도 → 26년도 변경사항**:
- `UserId` (string) → `Uid` (long)
- `GetByUserIdAllAsync(processDb.UserId)` → `GetByUserIdAllAsync(processDb.Uid)`

### 4.3. 코멘트 관리

**승인 시**:
```
[기존 코멘트가 있으면]
기존코멘트
[승인]새로운코멘트[2026-02-04 15:30:00]

[기존 코멘트가 없으면]
[승인]새로운코멘트[2026-02-04 15:30:00]
```

**반려 시**:
```
[기존 코멘트가 있으면]
기존코멘트
[반려]새로운코멘트[2026-02-04 15:30:00]

[기존 코멘트가 없으면]
[반려]새로운코멘트[2026-02-04 15:30:00]
```

### 4.4. 상태 업데이트

**승인**:
- `processDb.Is_Agreement = true`
- User가 다시 수정하려면 반려 후 재요청 필요

**반려**:
- `processDb.Is_Request = false`
- User가 다시 수정 가능

---

## 5. 테스트 항목

개발자가 테스트할 항목:

### 5.1. 합의 승인 테스트

**시나리오 1: 정상 승인**
1. TeamLeader로 로그인
2. Agreement/TeamLeader/Index 페이지 이동
3. Is_Request = true인 항목의 "상세" 버튼 클릭
4. Details 페이지에서 사용자 이름, 부서 확인
5. 모든 Agreement 리스트 표시 확인
6. 합의 코멘트 입력 (예: "업무비중 적절함")
7. "승인" 버튼 클릭
8. **확인**:
   - "합의 승인 성공" 메시지 표시 ✅
   - Index 페이지로 리다이렉트 ✅
   - ProcessDb.Is_Agreement = true 확인 ✅
   - Agreement_Comment에 [승인] 태그 추가 확인 ✅

**시나리오 2: 코멘트 없이 승인**
1. 합의 코멘트 입력 없이 "승인" 버튼 클릭
2. **확인**: "합의 코멘트를 입력하여 주세요." 메시지 표시 ✅

### 5.2. 합의 반려 테스트

**시나리오 3: 정상 반려**
1. TeamLeader로 로그인
2. Agreement/TeamLeader/Index 페이지 이동
3. Is_Request = true인 항목의 "상세" 버튼 클릭
4. 합의 코멘트 입력 (예: "1번 항목 비중 수정 필요")
5. "반려" 버튼 클릭
6. **확인**:
   - "합의 반려 성공" 메시지 표시 ✅
   - Index 페이지로 리다이렉트 ✅
   - ProcessDb.Is_Request = false 확인 ✅
   - Agreement_Comment에 [반려] 태그 추가 확인 ✅

### 5.3. 이전 코멘트 히스토리 테스트

**시나리오 4: 재승인 (이전 코멘트 있음)**
1. 이전에 반려된 항목 (Agreement_Comment에 [반려] 태그 있음)
2. User가 재요청 (Is_Request = true)
3. TeamLeader가 Details 페이지 접속
4. "이전 합의 내역(Old)" 헤더 확인
5. "+" 아이콘 클릭 → 이전 코멘트 펼쳐보기
6. 새로운 코멘트 입력 후 "승인" 클릭
7. **확인**:
   - Agreement_Comment에 이전 코멘트 + 새 코멘트 모두 포함 ✅
   - 줄바꿈 처리 확인 ✅

### 5.4. 권한 체크 테스트

**시나리오 5: 다른 부서 접근**
1. TeamLeader로 로그인 (예: 기획부)
2. 다른 부서 User의 ProcessDb.Pid로 URL 직접 접근
3. **확인**:
   - "다른 부서의 협의는 조회할 수 없습니다." 메시지 표시 ✅
   - 2초 후 Index 페이지로 리다이렉트 ✅

**시나리오 6: 일반 User 접근**
1. 일반 User로 로그인 (IsTeamLeader = false)
2. TeamLeader/Details URL 직접 접근
3. **확인**: Main 페이지로 리다이렉트 ✅

### 5.5. UI 테스트

**시나리오 7: 로딩 상태**
1. Details 페이지 접속
2. **확인**: ProcessDb 로드 전 LoadingIndicator 표시 ✅

**시나리오 8: Agreement 리스트 표시**
1. Details 페이지에서 사용자의 모든 Agreement 확인
2. **확인**:
   - AgreementDetailsTable 컴포넌트로 표시 ✅
   - 모든 항목 (5개 이하) 표시 확인 ✅
   - 비중 합계 100% 확인 ✅

---

## 6. 예상 결과

### 수정 전
- 26년도 Details 페이지: 단순 조회만 가능 ❌
- Agreement 1개만 표시 ❌
- 승인/반려 기능 없음 ❌
- ProcessDb 사용 안 함 ❌

### 수정 후
- 25년도와 동일한 워크플로우 ✅
- 사용자의 모든 Agreement 표시 ✅
- 합의 코멘트 입력 및 승인/반려 버튼 ✅
- ProcessDb 상태 업데이트 ✅
- 이전 코멘트 히스토리 관리 ✅
- 26년도 DB 구조 (Uid) 반영 ✅

---

## 7. 주의사항

1. **Parameter 이름**: `Id`는 ProcessDb.Pid를 의미 (Uid가 아님)
2. **26년도 DB 변경**: `UserId` (string) → `Uid` (long)
3. **코멘트 줄바꿈**: `\r\n​` 사용 (특수 공백 문자 포함)
4. **타임스탬프 형식**: `yyyy-MM-dd HH:mm:ss` (CultureInfo: ko-KR)
5. **권한 체크**: 같은 부서만 접근 가능
6. **리다이렉트**: 승인/반려 후 Index 페이지로 이동

---

## 8. 백업 안내

수정 전 현재 파일 백업 권장:
```
Details.razor → Details.razor.backup_20260204
Details.razor.cs → Details.razor.cs.backup_20260204
```

---

## 9. 관련 코드 참조

### 9.1. ProcessRepository.GetByIdAsync

**파일**: `MdcHR26Apps.Models/EvaluationProcess/ProcessRepository.cs`

```csharp
public async Task<ProcessDb> GetByIdAsync(Int64 id)
{
    const string sql = """
        SELECT * FROM ProcessDb
        WHERE Pid = @id
        """;
    using var connection = new SqlConnection(dbContext);
    var result = await connection.QueryFirstOrDefaultAsync<ProcessDb>(sql, new { id });
    return result ?? new ProcessDb();
}
```

### 9.2. AgreementRepository.GetByUserIdAllAsync

**파일**: `MdcHR26Apps.Models/EvaluationAgreement/AgreementRepository.cs`

```csharp
public async Task<List<AgreementDb>> GetByUserIdAllAsync(Int64 uid)
{
    const string sql = """
        SELECT * FROM AgreementDb
        WHERE Uid = @uid
        ORDER BY Aid DESC
        """;
    using var connection = new SqlConnection(dbContext);
    var result = await connection.QueryAsync<AgreementDb>(sql, new { uid });
    return result.AsList();
}
```

### 9.3. AgreementDetailsTable 컴포넌트

**파일**: `Components/Pages/Components/Agreement/AgreementDetailsTable.razor`

25년도와 동일한 컴포넌트 사용 (이미 구현되어 있음)

---

## 10. 완료 조건

- [ ] Details.razor.cs 전체 재작성 완료
- [ ] Details.razor UI 전체 재작성 완료
- [ ] 빌드 성공 (0 errors)
- [ ] 시나리오 1: 정상 승인 성공
- [ ] 시나리오 2: 코멘트 없이 승인 (유효성 검사) 성공
- [ ] 시나리오 3: 정상 반려 성공
- [ ] 시나리오 4: 재승인 (이전 코멘트 히스토리) 성공
- [ ] 시나리오 5: 다른 부서 접근 차단 성공
- [ ] 시나리오 6: 일반 User 접근 차단 성공
- [ ] 시나리오 7: 로딩 상태 표시 성공
- [ ] 시나리오 8: Agreement 리스트 표시 성공
- [ ] 25년도 기능과 동일성 확인 완료

---

## 11. 추가 고려사항

### 11.1. View Table 필요 여부

**사용자 질문**: "그럼 애초에 View Table은 필요없었던 것 아닌가요?"

**답변**:
- Details 페이지에서는 **View Table을 사용하지 않습니다**
- 25년도와 동일하게 **ProcessDb + AgreementDb 직접 조회** 방식 사용
- View Table (v_AgreementDB, v_SubAgreementDB)은 다른 페이지에서 사용할 수 있음

### 11.2. 일관성

- 25년도 코드와 최대한 동일하게 구현
- 26년도 DB 변경사항 (UserId → Uid)만 반영
- 로직, UI, 워크플로우 모두 25년도와 동일

### 11.3. Navigation Properties

- ProcessDb.User (UserDb)
- UserDb.EDepartment (EDepartmentDb)
- 이미 ProcessRepository에 Include 구현되어 있음

---

## 12. 롤백 계획

문제 발생 시 백업 파일로 복구:
```bash
Details.razor.backup_20260204 → Details.razor
Details.razor.cs.backup_20260204 → Details.razor.cs
```

---

## 13. 25년도 참조 파일

**참조 경로**:
- `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages\Agreement\TeamLeader\Details.razor`
- `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages\Agreement\TeamLeader\Details.razor.cs`

**주요 차이점**:
- 25년도: `UserId` (string)
- 26년도: `Uid` (long)
- 나머지는 동일

---
