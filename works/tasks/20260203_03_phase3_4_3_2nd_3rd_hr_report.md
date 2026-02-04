# 작업지시서: Phase 3-4-3 부서장/임원평가 (2nd, 3rd HR Report)

**날짜**: 2026-02-03
**작업 타입**: 기능 추가
**예상 소요**: 3-4시간
**관련 이슈**: [#009](../issues/009_phase3_webapp_development.md)
**Phase**: 3-4-3
**선행 작업**: [20260203_02_phase3_4_2_1st_hr_report.md](20260203_02_phase3_4_2_1st_hr_report.md)

---

## 1. 작업 개요

### 배경
- Phase 3-4-2 완료: 본인평가 (1st_HR_Report) 구현
- 평가 프로세스의 세 번째 단계: 부서장평가 (2차) 및 임원평가 (3차)

### 목표
부서장과 임원이 부서원의 평가를 검토하고 2차/3차 평가를 수행할 수 있도록 함

### 구현 범위
1. **2nd_HR_Report (부서장평가 - 2차)**
   - Index: 부서원 평가 목록
   - Edit: 부서장 평가 입력
   - Details: 평가 상세 (본인+부서장)
   - Complete_2nd_Edit: 평가 완료 후 수정
   - Complete_2nd_Details: 완료된 평가 상세

2. **3rd_HR_Report (임원평가 - 3차)**
   - Index: 전체 평가 목록 (부서별)
   - Edit: 임원 평가 입력
   - Details: 평가 상세 (본인+부서장+임원)
   - Complete_3rd_Edit: 평가 완료 후 수정
   - Complete_3rd_Details: 완료된 평가 상세

---

## 2. 참조 프로젝트

### 2025년 프로젝트 구조
**경로**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages`

#### 2nd_HR_Report 구조
```
2nd_HR_Report/
├── Index.razor                 # 부서원 평가 목록
├── Edit.razor                  # 부서장 평가 입력
├── Details.razor               # 평가 상세
├── Complete_2nd_Edit.razor     # 완료 후 수정
└── Complete_2nd_Details.razor  # 완료된 평가 상세
```

#### 3rd_HR_Report 구조
```
3rd_HR_Report/
├── Index.razor                 # 전체 평가 목록
├── Edit.razor                  # 임원 평가 입력
├── Details.razor               # 평가 상세
├── Complete_3rd_Edit.razor     # 완료 후 수정
└── Complete_3rd_Details.razor  # 완료된 평가 상세
```

---

## 3. 데이터 모델

### ReportDb (Phase 2 완료)
**경로**: `MdcHR26Apps.Models/ReportDb/ReportDb.cs`

**평가 필드**:
- User_Evaluation_1~4: 본인 평가 (Phase 3-4-2에서 입력)
- **TeamLeader_Evaluation_1** (double) - 부서장 평가 1 (난이도)
- **TeamLeader_Evaluation_2** (double) - 부서장 평가 2 (성과)
- **TeamLeader_Evaluation_3** (double) - 부서장 평가 3 (총점)
- **TeamLeader_Evaluation_4** (string) - 부서장 코멘트
- **Director_Evaluation_1** (double) - 임원 평가 1 (난이도)
- **Director_Evaluation_2** (double) - 임원 평가 2 (성과)
- **Director_Evaluation_3** (double) - 임원 평가 3 (총점)
- **Director_Evaluation_4** (string) - 임원 코멘트
- **Total_Score** (double) - 최종 총점

### v_MemberListDB (Phase 2 완료)
**경로**: `MdcHR26Apps.Models/Views/v_MemberListDB/v_MemberListDB.cs`

**부서원 조회용**:
- Uid, UserId, UserName
- EDepartId, EDepartmentName
- IsTeamLeader, IsDirector

---

## 4. 평가 프로세스 흐름

### 2차 평가 (부서장)
1. **본인평가 완료**: User_Evaluation_1~4 입력 완료
2. **부서장 평가**: 부서장이 부서원의 본인평가 검토 후 TeamLeader_Evaluation_1~4 입력
3. **2차 평가 완료**: TeamLeader_Evaluation_3 > 0

### 3차 평가 (임원)
1. **2차 평가 완료**: TeamLeader_Evaluation_1~4 입력 완료
2. **임원 평가**: 임원이 부서장 평가 검토 후 Director_Evaluation_1~4 입력
3. **최종 평가 완료**: Director_Evaluation_3 > 0

### 최종 점수 계산
```csharp
// 방식 1: 단순 평균
Total_Score = (User_Evaluation_3 + TeamLeader_Evaluation_3 + Director_Evaluation_3) / 3;

// 방식 2: 가중 평균 (예시: 본인 20%, 부서장 40%, 임원 40%)
Total_Score = (User_Evaluation_3 * 0.2) + (TeamLeader_Evaluation_3 * 0.4) + (Director_Evaluation_3 * 0.4);
```

---

## 5. 구현 페이지 목록 - 2nd_HR_Report

### 2nd_HR_Report/Index.razor
**기능**: 부서원 평가 목록 표시
**라우트**: `/2nd_HR_Report` 또는 `/2nd_HR_Report/Index`
**권한**: IsTeamLeader = true

**주요 컴포넌트**:
- SearchbarComponent (사용자명, 직무명 검색)
- Table (부서원 평가 목록)
- Edit/Details 버튼

**데이터**:
```csharp
// 같은 부서 사용자들의 평가 조회
var members = await _memberRepository.GetByDepartmentAsync(currentUser.EDepartId);
var reports = new List<ReportDb>();
foreach (var member in members)
{
    var userReports = await _reportRepository.GetByUserIdAsync(member.Uid);
    reports.AddRange(userReports);
}
```

**표시 컬럼**:
- UserName (평가 대상자)
- Report_Item_Name_1 (직무명)
- Report_SubItem_Name (세부 업무)
- User_Evaluation_3 (본인 평가)
- TeamLeader_Evaluation_3 (부서장 평가)
- 평가 상태 (미평가/평가 완료)

**UI 예시**:
```razor
<table class="table table-striped">
    <thead>
        <tr>
            <th>평가 대상자</th>
            <th>직무명</th>
            <th>세부 업무</th>
            <th>본인 평가</th>
            <th>부서장 평가</th>
            <th>상태</th>
            <th>작업</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var report in reports)
        {
            <tr>
                <td>@report.UserName</td>
                <td>@report.Report_Item_Name_1</td>
                <td>@report.Report_SubItem_Name</td>
                <td>@report.User_Evaluation_3</td>
                <td>@report.TeamLeader_Evaluation_3</td>
                <td>
                    @if (report.TeamLeader_Evaluation_3 > 0)
                    {
                        <span class="badge bg-success">평가 완료</span>
                    }
                    else
                    {
                        <span class="badge bg-warning">미평가</span>
                    }
                </td>
                <td>
                    <a href="/2nd_HR_Report/Edit/@report.Rid" class="btn btn-sm btn-primary">평가</a>
                    <a href="/2nd_HR_Report/Details/@report.Rid" class="btn btn-sm btn-info">상세</a>
                </td>
            </tr>
        }
    </tbody>
</table>
```

---

### 2nd_HR_Report/Edit.razor
**기능**: 부서장 평가 입력
**라우트**: `/2nd_HR_Report/Edit/{rid:long}`
**권한**: IsTeamLeader = true

**주요 컴포넌트**:
- Form (TeamLeader_Evaluation_1~4 입력)
- 직무/세부 업무 정보 표시
- 본인 평가 결과 표시 (참고용)
- 저장/취소 버튼

**Form 구조**:
```razor
<EditForm Model="@model" OnValidSubmit="HandleValidSubmit">
    <DataAnnotationsValidator />

    @* 평가 대상자 정보 *@
    <div class="card mb-3">
        <div class="card-header">평가 대상자 정보</div>
        <div class="card-body">
            <dl class="row">
                <dt class="col-sm-3">성명</dt>
                <dd class="col-sm-9">@report.UserName</dd>

                <dt class="col-sm-3">직무명</dt>
                <dd class="col-sm-9">@report.Report_Item_Name_1</dd>

                <dt class="col-sm-3">세부 업무</dt>
                <dd class="col-sm-9">@report.Report_SubItem_Name</dd>
            </dl>
        </div>
    </div>

    @* 본인 평가 결과 (참고용) *@
    <div class="card mb-3">
        <div class="card-header">본인 평가 결과 (참고)</div>
        <div class="card-body">
            <dl class="row">
                <dt class="col-sm-3">난이도</dt>
                <dd class="col-sm-9">@report.User_Evaluation_1 / 10</dd>

                <dt class="col-sm-3">성과</dt>
                <dd class="col-sm-9">@report.User_Evaluation_2 / 10</dd>

                <dt class="col-sm-3">총점</dt>
                <dd class="col-sm-9"><strong>@report.User_Evaluation_3</strong> / 10</dd>

                <dt class="col-sm-3">코멘트</dt>
                <dd class="col-sm-9">@report.User_Evaluation_4</dd>
            </dl>
        </div>
    </div>

    @* 부서장 평가 입력 *@
    <div class="card mb-3">
        <div class="card-header">부서장 평가</div>
        <div class="card-body">
            <div class="mb-3">
                <label class="form-label">난이도 평가 (1~10)</label>
                <InputNumber @bind-Value="model.TeamLeader_Evaluation_1" class="form-control" min="1" max="10" />
                <ValidationMessage For="@(() => model.TeamLeader_Evaluation_1)" />
            </div>

            <div class="mb-3">
                <label class="form-label">성과 평가 (1~10)</label>
                <InputNumber @bind-Value="model.TeamLeader_Evaluation_2" class="form-control" min="1" max="10" />
                <ValidationMessage For="@(() => model.TeamLeader_Evaluation_2)" />
            </div>

            <div class="mb-3">
                <label class="form-label">총점 (자동 계산)</label>
                <input type="number" class="form-control" value="@((model.TeamLeader_Evaluation_1 + model.TeamLeader_Evaluation_2) / 2)" readonly />
            </div>

            <div class="mb-3">
                <label class="form-label">코멘트 (선택)</label>
                <InputTextArea @bind-Value="model.TeamLeader_Evaluation_4" class="form-control" rows="5" />
                <ValidationMessage For="@(() => model.TeamLeader_Evaluation_4)" />
            </div>
        </div>
    </div>

    <button type="submit" class="btn btn-primary">저장</button>
    <a href="/2nd_HR_Report" class="btn btn-secondary">취소</a>
</EditForm>
```

**저장 로직**:
```csharp
private async Task HandleValidSubmit()
{
    var report = await _reportRepository.GetByIdAsync(Rid);

    // 권한 체크: 같은 부서 확인
    var targetUser = await _userRepository.GetByIdAsync(report.Uid);
    if (targetUser.EDepartId != currentUser.EDepartId || !currentUser.IsTeamLeader)
    {
        resultMessage = "부서원의 평가만 수정할 수 있습니다.";
        return;
    }

    report.TeamLeader_Evaluation_1 = model.TeamLeader_Evaluation_1;
    report.TeamLeader_Evaluation_2 = model.TeamLeader_Evaluation_2;
    report.TeamLeader_Evaluation_3 = (model.TeamLeader_Evaluation_1 + model.TeamLeader_Evaluation_2) / 2;
    report.TeamLeader_Evaluation_4 = model.TeamLeader_Evaluation_4 ?? "";

    await _reportRepository.UpdateAsync(report);

    Navigation.NavigateTo("/2nd_HR_Report");
}
```

---

### 2nd_HR_Report/Details.razor
**기능**: 평가 상세 (본인+부서장)
**라우트**: `/2nd_HR_Report/Details/{rid:long}`
**권한**: IsTeamLeader = true

**주요 컴포넌트**:
- 평가 대상자 정보 표시
- 본인 평가 결과 표시
- 부서장 평가 결과 표시
- Edit 버튼

---

### 2nd_HR_Report/Complete_2nd_Edit.razor
**기능**: 완료된 평가 수정 (부서장)
**라우트**: `/2nd_HR_Report/Complete_2nd_Edit/{rid:long}`
**권한**: IsTeamLeader = true

**차이점**:
- Edit.razor와 동일하지만, 이미 평가 완료된 항목 수정용
- UI에서 "수정" 경고 메시지 표시

---

### 2nd_HR_Report/Complete_2nd_Details.razor
**기능**: 완료된 평가 상세
**라우트**: `/2nd_HR_Report/Complete_2nd_Details/{rid:long}`

**차이점**:
- Details.razor와 동일하지만, 완료된 평가 조회 전용

---

## 6. 구현 페이지 목록 - 3rd_HR_Report

### 3rd_HR_Report/Index.razor
**기능**: 전체 평가 목록 표시 (부서별)
**라우트**: `/3rd_HR_Report` 또는 `/3rd_HR_Report/Index`
**권한**: IsDirector = true

**주요 컴포넌트**:
- SearchbarComponent (사용자명, 부서명, 직무명 검색)
- 부서별 필터 드롭다운
- Table (전체 평가 목록)
- Edit/Details 버튼

**데이터**:
```csharp
// 전체 평가 조회 (임원은 모든 부서 평가 가능)
var reports = await _reportRepository.GetAllAsync();
```

**표시 컬럼**:
- UserName (평가 대상자)
- EDepartmentName (부서)
- Report_Item_Name_1 (직무명)
- Report_SubItem_Name (세부 업무)
- User_Evaluation_3 (본인)
- TeamLeader_Evaluation_3 (부서장)
- Director_Evaluation_3 (임원)
- 평가 상태

---

### 3rd_HR_Report/Edit.razor
**기능**: 임원 평가 입력
**라우트**: `/3rd_HR_Report/Edit/{rid:long}`
**권한**: IsDirector = true

**Form 구조**:
```razor
@* 평가 대상자 정보 *@
<div class="card mb-3">...</div>

@* 본인 평가 결과 (참고용) *@
<div class="card mb-3">...</div>

@* 부서장 평가 결과 (참고용) *@
<div class="card mb-3">
    <div class="card-header">부서장 평가 결과 (참고)</div>
    <div class="card-body">
        <dl class="row">
            <dt class="col-sm-3">난이도</dt>
            <dd class="col-sm-9">@report.TeamLeader_Evaluation_1 / 10</dd>

            <dt class="col-sm-3">성과</dt>
            <dd class="col-sm-9">@report.TeamLeader_Evaluation_2 / 10</dd>

            <dt class="col-sm-3">총점</dt>
            <dd class="col-sm-9"><strong>@report.TeamLeader_Evaluation_3</strong> / 10</dd>

            <dt class="col-sm-3">코멘트</dt>
            <dd class="col-sm-9">@report.TeamLeader_Evaluation_4</dd>
        </dl>
    </div>
</div>

@* 임원 평가 입력 *@
<div class="card mb-3">
    <div class="card-header">임원 평가</div>
    <div class="card-body">
        <div class="mb-3">
            <label class="form-label">난이도 평가 (1~10)</label>
            <InputNumber @bind-Value="model.Director_Evaluation_1" class="form-control" min="1" max="10" />
        </div>

        <div class="mb-3">
            <label class="form-label">성과 평가 (1~10)</label>
            <InputNumber @bind-Value="model.Director_Evaluation_2" class="form-control" min="1" max="10" />
        </div>

        <div class="mb-3">
            <label class="form-label">총점 (자동 계산)</label>
            <input type="number" class="form-control" value="@((model.Director_Evaluation_1 + model.Director_Evaluation_2) / 2)" readonly />
        </div>

        <div class="mb-3">
            <label class="form-label">코멘트 (선택)</label>
            <InputTextArea @bind-Value="model.Director_Evaluation_4" class="form-control" rows="5" />
        </div>
    </div>
</div>
```

**저장 로직**:
```csharp
private async Task HandleValidSubmit()
{
    var report = await _reportRepository.GetByIdAsync(Rid);

    // 권한 체크
    if (!currentUser.IsDirector)
    {
        resultMessage = "임원만 평가할 수 있습니다.";
        return;
    }

    report.Director_Evaluation_1 = model.Director_Evaluation_1;
    report.Director_Evaluation_2 = model.Director_Evaluation_2;
    report.Director_Evaluation_3 = (model.Director_Evaluation_1 + model.Director_Evaluation_2) / 2;
    report.Director_Evaluation_4 = model.Director_Evaluation_4 ?? "";

    // 최종 점수 계산
    report.Total_Score = (report.User_Evaluation_3 + report.TeamLeader_Evaluation_3 + report.Director_Evaluation_3) / 3;

    await _reportRepository.UpdateAsync(report);

    Navigation.NavigateTo("/3rd_HR_Report");
}
```

---

### 3rd_HR_Report/Details.razor
**기능**: 평가 상세 (본인+부서장+임원)
**라우트**: `/3rd_HR_Report/Details/{rid:long}`
**권한**: IsDirector = true

**주요 컴포넌트**:
- 평가 대상자 정보
- 본인 평가 결과
- 부서장 평가 결과
- 임원 평가 결과
- 최종 총점 표시

---

### 3rd_HR_Report/Complete_3rd_Edit.razor, Complete_3rd_Details.razor
**기능**: 완료된 평가 수정/상세
**차이점**: Edit/Details와 동일, 완료된 평가용

---

## 7. 네비게이션 메뉴 업데이트

### NavMenu.razor 수정
**경로**: `MdcHR26Apps.BlazorServer/Components/Layout/NavMenu.razor`

#### 추가할 메뉴 항목
```razor
@* 부서장 전용 메뉴 *@
@if (loginStatus?.IsTeamLeader == true)
{
    <div class="nav-item px-3">
        <NavLink class="nav-link" href="2nd_hr_report">
            <span class="bi bi-clipboard2-check-fill" aria-hidden="true"></span> 부서장평가 (2차)
        </NavLink>
    </div>
}

@* 임원 전용 메뉴 *@
@if (loginStatus?.IsDirector == true)
{
    <div class="nav-item px-3">
        <NavLink class="nav-link" href="3rd_hr_report">
            <span class="bi bi-award-fill" aria-hidden="true"></span> 임원평가 (3차)
        </NavLink>
    </div>
}
```

---

## 8. 구현 순서

### Step 1: 2nd_HR_Report 구현 (2시간)
1. 2nd_HR_Report/Index.razor
2. 2nd_HR_Report/Edit.razor
3. 2nd_HR_Report/Details.razor
4. 2nd_HR_Report/Complete_2nd_Edit.razor
5. 2nd_HR_Report/Complete_2nd_Details.razor

### Step 2: 3rd_HR_Report 구현 (2시간)
1. 3rd_HR_Report/Index.razor
2. 3rd_HR_Report/Edit.razor
3. 3rd_HR_Report/Details.razor
4. 3rd_HR_Report/Complete_3rd_Edit.razor
5. 3rd_HR_Report/Complete_3rd_Details.razor

### Step 3: 네비게이션 메뉴 업데이트 (10분)
1. NavMenu.razor 수정

### Step 4: 테스트 (30분)
1. 부서장 평가 시나리오
2. 임원 평가 시나리오
3. 권한 체크 테스트

---

## 9. 테스트 계획

### 테스트 시나리오 1: 부서장 평가
1. 로그아웃 후 부서장 계정으로 로그인
2. `/2nd_HR_Report` 접속
3. **확인**: 부서원 평가 목록 표시 ✅
4. "평가" 버튼 클릭 (부서원 A의 첫 번째 항목)
5. **확인**: 본인 평가 결과 표시 ✅
6. 난이도: 8, 성과: 9 입력
7. 코멘트: "우수한 성과를 거두었습니다." 입력
8. 저장
9. **확인**: TeamLeader_Evaluation_3 = 8.5 ✅

### 테스트 시나리오 2: 임원 평가
1. 로그아웃 후 임원 계정으로 로그인
2. `/3rd_HR_Report` 접속
3. **확인**: 전체 평가 목록 표시 (모든 부서) ✅
4. "평가" 버튼 클릭
5. **확인**: 본인+부서장 평가 결과 표시 ✅
6. 난이도: 9, 성과: 9 입력
7. 저장
8. **확인**: Director_Evaluation_3 = 9.0 ✅
9. **확인**: Total_Score 계산됨 ✅

### 테스트 시나리오 3: 권한 체크 (부서장)
1. 부서장 A로 로그인 (A 부서)
2. B 부서 직원의 평가 Edit URL 직접 접속 시도
3. **확인**: 접근 거부 또는 에러 메시지 ✅

### 테스트 시나리오 4: 권한 체크 (임원)
1. 일반 사용자로 로그인
2. `/3rd_HR_Report` 직접 접속 시도
3. **확인**: 접근 거부 또는 메인 페이지로 리다이렉트 ✅

### 테스트 시나리오 5: 최종 점수 계산
1. 본인: 7.0, 부서장: 8.0, 임원: 9.0
2. **확인**: Total_Score = (7.0 + 8.0 + 9.0) / 3 = 8.0 ✅

---

## 10. 주의사항

1. **권한 체크 필수**:
   - 2nd_HR_Report: IsTeamLeader = true, 같은 부서만
   - 3rd_HR_Report: IsDirector = true, 전체 부서 가능

2. **평가 순서**:
   - 2차 평가는 1차 평가 완료 후 가능
   - 3차 평가는 2차 평가 완료 후 가능

3. **점수 계산**:
   - 각 평가 총점은 자동 계산
   - Total_Score는 3차 평가 완료 시 자동 계산

4. **Complete 페이지**:
   - Complete_*_Edit/Details는 완료된 평가 수정/조회용
   - 평가 기간 종료 후 사용

5. **2025년 코드 참조**:
   - UI/UX는 2025년 프로젝트 참조
   - .NET 10 최신 기능 적용

---

## 11. 완료 조건

- [ ] 2nd_HR_Report 5개 페이지 완료
- [ ] 3rd_HR_Report 5개 페이지 완료
- [ ] NavMenu.razor 메뉴 추가 완료
- [ ] 테스트 시나리오 1 성공
- [ ] 테스트 시나리오 2 성공
- [ ] 테스트 시나리오 3 성공
- [ ] 테스트 시나리오 4 성공
- [ ] 테스트 시나리오 5 성공
- [ ] 빌드 오류 0개
- [ ] 런타임 오류 0개

---

## 12. 다음 단계

**Phase 3-4-4**: 부서 목표 관리 (DeptObjective) 구현
- 작업지시서: `20260203_04_phase3_4_4_dept_objective.md`

---

## 13. 관련 문서

**이슈**: [#009: Phase 3 WebApp 개발](../issues/009_phase3_webapp_development.md)
**Phase 2 Model**:
- [ReportDb](../../MdcHR26Apps.Models/ReportDb/)
**선행 작업**:
- [20260203_01_phase3_4_1_agreement_subagreement.md](20260203_01_phase3_4_1_agreement_subagreement.md)
- [20260203_02_phase3_4_2_1st_hr_report.md](20260203_02_phase3_4_2_1st_hr_report.md)
**참조 프로젝트**:
- `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages\2nd_HR_Report`
- `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages\3rd_HR_Report`
