# 작업지시서: Phase 3-4-2 본인평가 (1st_HR_Report)

**날짜**: 2026-02-03
**작업 타입**: 기능 추가
**예상 소요**: 2-3시간
**관련 이슈**: [#009](../issues/009_phase3_webapp_development.md)
**Phase**: 3-4-2
**선행 작업**: [20260203_01_phase3_4_1_agreement_subagreement.md](20260203_01_phase3_4_1_agreement_subagreement.md)

---

## 1. 작업 개요

### 배경
- Phase 3-4-1 완료: 직무평가 협의 (Agreement, SubAgreement) 구현
- 평가 프로세스의 두 번째 단계: 본인평가 (1차 평가)

### 목표
사용자가 협의된 직무 및 세부 업무를 기반으로 본인 평가를 수행할 수 있도록 함

### 구현 범위
1. **1st_HR_Report (본인평가)**
   - Index: 평가 대상 세부 업무 목록
   - Edit: 본인 평가 입력
   - Details: 평가 상세 내용 확인

---

## 2. 참조 프로젝트

### 2025년 프로젝트 구조
**경로**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages\1st_HR_Report`

```
1st_HR_Report/
├── Index.razor       # 평가 대상 목록
├── Edit.razor        # 본인 평가 입력
└── Details.razor     # 평가 상세 내용
```

---

## 3. 데이터 모델

### ReportDb (Phase 2 완료)
**경로**: `MdcHR26Apps.Models/ReportDb/ReportDb.cs`

**주요 필드**:
- Rid (Int64) - PK
- Uid (Int64) - FK to UserDb
- Report_Item_Number (int) - 직무 번호
- Report_Item_Name_1 (string) - 직무명
- Report_Item_Name_2 (string) - 세부 직무명
- Report_Item_Proportion (int) - 직무 비중
- Report_SubItem_Name (string) - 세부 업무명
- Report_SubItem_Proportion (int) - 세부 업무 비중
- Task_Number (Int64) - FK to TasksDb
- **User_Evaluation_1** (double) - 본인 평가 1 (난이도)
- **User_Evaluation_2** (double) - 본인 평가 2 (성과)
- **User_Evaluation_3** (double) - 본인 평가 3 (총점)
- **User_Evaluation_4** (string) - 본인 평가 4 (코멘트)
- TeamLeader_Evaluation_1 (double) - 부서장 평가 1
- TeamLeader_Evaluation_2 (double) - 부서장 평가 2
- TeamLeader_Evaluation_3 (double) - 부서장 평가 3
- TeamLeader_Evaluation_4 (string) - 부서장 코멘트
- Director_Evaluation_1 (double) - 임원 평가 1
- Director_Evaluation_2 (double) - 임원 평가 2
- Director_Evaluation_3 (double) - 임원 평가 3
- Director_Evaluation_4 (string) - 임원 코멘트
- Total_Score (double) - 최종 총점

**Repository**: `IReportRepository`, `ReportRepository`
- GetAllAsync()
- GetByIdAsync(Rid)
- GetByUserIdAsync(Uid)
- AddAsync(report)
- UpdateAsync(report)
- DeleteAsync(Rid)

### TasksDb (Phase 2 완료)
**경로**: `MdcHR26Apps.Models/TasksDb/TasksDb.cs`

**주요 필드**:
- Tid (Int64) - PK
- TaskName (string) - 업무명
- TaksListNumber (Int64) - 업무 번호 (오타 그대로)
- TaskStatus (int) - 진행 상태
- TaskObjective (string) - 업무 목표
- TargetProportion (int) - 목표 비중
- ResultProportion (int) - 결과 비중
- TargetDate (DateTime) - 목표 일자
- ResultDate (DateTime) - 완료 일자
- Task_Evaluation_1 (double) - 업무 평가 1
- Task_Evaluation_2 (double) - 업무 평가 2
- TaskLevel (double) - 난이도
- TaskComments (string) - 코멘트

---

## 4. 본인평가 프로세스 흐름

### 평가 시작 전 준비
1. **Agreement 작성 완료**: 사용자가 직무를 등록
2. **SubAgreement 작성 완료**: 사용자가 세부 업무를 등록
3. **부서장 승인 완료**: SubAgreement.Completed_Number = 2

### ReportDb 데이터 생성
**시점**: 평가 기간 시작 시 (관리자가 일괄 생성)
**로직**: SubAgreement 데이터를 기반으로 ReportDb 생성

```csharp
// 관리자가 평가 시작 시 실행 (별도 작업지시서에서 구현)
var subAgreements = await _subAgreementRepository.GetByProcessIdAsync(currentPid);
foreach (var sa in subAgreements)
{
    var agreement = await _agreementRepository.GetByIdAsync(sa.Aid);
    var report = new ReportDb
    {
        Uid = sa.Uid,
        Report_Item_Number = agreement.Item_Number,
        Report_Item_Name_1 = agreement.Item_Title,
        Report_Item_Name_2 = agreement.Item_Contents,
        Report_Item_Proportion = agreement.Item_Proportion,
        Report_SubItem_Name = sa.TaskName,
        Report_SubItem_Proportion = sa.Sub_Agreement_Proportion,
        Task_Number = sa.Said, // TasksDb 연동 시 변경
        User_Evaluation_1 = 0,
        User_Evaluation_2 = 0,
        User_Evaluation_3 = 0,
        User_Evaluation_4 = "",
        // ... 나머지 필드는 0 또는 빈 값
    };
    await _reportRepository.AddAsync(report);
}
```

### 본인평가 입력
사용자가 ReportDb의 User_Evaluation_1~4 입력

---

## 5. 구현 페이지 목록

### 1st_HR_Report/Index.razor
**기능**: 본인 평가 대상 목록 표시
**라우트**: `/1st_HR_Report` 또는 `/1st_HR_Report/Index`

**주요 컴포넌트**:
- SearchbarComponent (검색)
- Table (평가 대상 목록)
- Edit/Details 버튼

**데이터**:
```csharp
var reports = await _reportRepository.GetByUserIdAsync(currentUser.Uid);
```

**표시 컬럼**:
- Report_Item_Number (직무 번호)
- Report_Item_Name_1 (직무명)
- Report_SubItem_Name (세부 업무명)
- Report_SubItem_Proportion (비중)
- User_Evaluation_3 (본인 평가 총점)
- 평가 상태 (미평가/평가 완료)

**UI 예시**:
```razor
<table class="table table-striped">
    <thead>
        <tr>
            <th>직무 번호</th>
            <th>직무명</th>
            <th>세부 업무</th>
            <th>비중 (%)</th>
            <th>본인 평가</th>
            <th>상태</th>
            <th>작업</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var report in reports)
        {
            <tr>
                <td>@report.Report_Item_Number</td>
                <td>@report.Report_Item_Name_1</td>
                <td>@report.Report_SubItem_Name</td>
                <td>@report.Report_SubItem_Proportion</td>
                <td>@report.User_Evaluation_3</td>
                <td>
                    @if (report.User_Evaluation_3 > 0)
                    {
                        <span class="badge bg-success">평가 완료</span>
                    }
                    else
                    {
                        <span class="badge bg-warning">미평가</span>
                    }
                </td>
                <td>
                    <a href="/1st_HR_Report/Edit/@report.Rid" class="btn btn-sm btn-primary">평가</a>
                    <a href="/1st_HR_Report/Details/@report.Rid" class="btn btn-sm btn-info">상세</a>
                </td>
            </tr>
        }
    </tbody>
</table>
```

---

### 1st_HR_Report/Edit.razor
**기능**: 본인 평가 입력
**라우트**: `/1st_HR_Report/Edit/{rid:long}`

**주요 컴포넌트**:
- Form (User_Evaluation_1~4 입력)
- 직무/세부 업무 정보 표시 (읽기 전용)
- 저장/취소 버튼

**권한 체크**:
- 본인의 평가만 수정 가능 (report.Uid == currentUser.Uid)

**Form 구조**:
```razor
<EditForm Model="@model" OnValidSubmit="HandleValidSubmit">
    <DataAnnotationsValidator />

    @* 직무 정보 (읽기 전용) *@
    <div class="mb-3">
        <label class="form-label">직무명</label>
        <input type="text" class="form-control" value="@report.Report_Item_Name_1" readonly />
    </div>

    <div class="mb-3">
        <label class="form-label">세부 업무</label>
        <input type="text" class="form-control" value="@report.Report_SubItem_Name" readonly />
    </div>

    <div class="mb-3">
        <label class="form-label">업무 목표</label>
        <textarea class="form-control" rows="3" value="@taskObjective" readonly></textarea>
    </div>

    @* 본인 평가 입력 *@
    <div class="mb-3">
        <label class="form-label">난이도 평가 (1~10)</label>
        <InputNumber @bind-Value="model.User_Evaluation_1" class="form-control" min="1" max="10" />
        <ValidationMessage For="@(() => model.User_Evaluation_1)" />
        <small class="form-text text-muted">1: 매우 쉬움, 10: 매우 어려움</small>
    </div>

    <div class="mb-3">
        <label class="form-label">성과 평가 (1~10)</label>
        <InputNumber @bind-Value="model.User_Evaluation_2" class="form-control" min="1" max="10" />
        <ValidationMessage For="@(() => model.User_Evaluation_2)" />
        <small class="form-text text-muted">1: 매우 낮음, 10: 매우 높음</small>
    </div>

    <div class="mb-3">
        <label class="form-label">총점 (자동 계산)</label>
        <input type="number" class="form-control" value="@((model.User_Evaluation_1 + model.User_Evaluation_2) / 2)" readonly />
    </div>

    <div class="mb-3">
        <label class="form-label">코멘트 (선택)</label>
        <InputTextArea @bind-Value="model.User_Evaluation_4" class="form-control" rows="5" />
        <ValidationMessage For="@(() => model.User_Evaluation_4)" />
    </div>

    <button type="submit" class="btn btn-primary">저장</button>
    <a href="/1st_HR_Report" class="btn btn-secondary">취소</a>
</EditForm>
```

**저장 로직**:
```csharp
private async Task HandleValidSubmit()
{
    var report = await _reportRepository.GetByIdAsync(Rid);

    if (report.Uid != currentUser.Uid)
    {
        resultMessage = "본인의 평가만 수정할 수 있습니다.";
        return;
    }

    report.User_Evaluation_1 = model.User_Evaluation_1;
    report.User_Evaluation_2 = model.User_Evaluation_2;
    report.User_Evaluation_3 = (model.User_Evaluation_1 + model.User_Evaluation_2) / 2;
    report.User_Evaluation_4 = model.User_Evaluation_4 ?? "";

    await _reportRepository.UpdateAsync(report);

    Navigation.NavigateTo("/1st_HR_Report");
}
```

**유효성 검사**:
- User_Evaluation_1: 필수, 1~10 범위
- User_Evaluation_2: 필수, 1~10 범위
- User_Evaluation_4: 선택, 최대 2000자

---

### 1st_HR_Report/Details.razor
**기능**: 평가 상세 내용 확인
**라우트**: `/1st_HR_Report/Details/{rid:long}`

**주요 컴포넌트**:
- 직무/세부 업무 정보 표시
- 본인 평가 결과 표시
- Edit 버튼

**UI 구조**:
```razor
<div class="card">
    <div class="card-header">
        <h5>본인평가 상세</h5>
    </div>
    <div class="card-body">
        <h6>직무 정보</h6>
        <dl class="row">
            <dt class="col-sm-3">직무 번호</dt>
            <dd class="col-sm-9">@report.Report_Item_Number</dd>

            <dt class="col-sm-3">직무명</dt>
            <dd class="col-sm-9">@report.Report_Item_Name_1</dd>

            <dt class="col-sm-3">세부 업무</dt>
            <dd class="col-sm-9">@report.Report_SubItem_Name</dd>

            <dt class="col-sm-3">비중</dt>
            <dd class="col-sm-9">@report.Report_SubItem_Proportion %</dd>
        </dl>

        <hr />

        <h6>본인 평가 결과</h6>
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

        <a href="/1st_HR_Report/Edit/@report.Rid" class="btn btn-primary">수정</a>
        <a href="/1st_HR_Report" class="btn btn-secondary">목록</a>
    </div>
</div>
```

---

## 6. 네비게이션 메뉴 업데이트

### NavMenu.razor 수정
**경로**: `MdcHR26Apps.BlazorServer/Components/Layout/NavMenu.razor`

#### 추가할 메뉴 항목
```razor
@* 평가 프로세스 - 본인평가 *@
<div class="nav-item px-3">
    <NavLink class="nav-link" href="1st_hr_report">
        <span class="bi bi-file-earmark-text-fill" aria-hidden="true"></span> 본인평가 (1차)
    </NavLink>
</div>
```

---

## 7. 평가 점수 계산 로직

### 총점 계산
**방식 1: 단순 평균**
```csharp
User_Evaluation_3 = (User_Evaluation_1 + User_Evaluation_2) / 2;
```

**방식 2: 가중 평균** (필요시)
```csharp
// 난이도 30%, 성과 70%
User_Evaluation_3 = (User_Evaluation_1 * 0.3) + (User_Evaluation_2 * 0.7);
```

**방식 3: 비중 반영** (필요시)
```csharp
// 세부 업무 비중 반영
var weightedScore = User_Evaluation_3 * (Report_SubItem_Proportion / 100.0);
```

**권장**: 방식 1 (단순 평균) 사용, 추후 요구사항에 따라 변경

---

## 8. 구현 순서

### Step 1: 1st_HR_Report/Index.razor (30분)
1. ReportRepository DI
2. 현재 사용자의 평가 대상 목록 조회
3. Table 컴포넌트로 목록 표시
4. 평가 상태 표시 (미평가/평가 완료)

### Step 2: 1st_HR_Report/Edit.razor (1시간)
1. Form 구조 작성
2. 직무/세부 업무 정보 표시
3. User_Evaluation_1~4 입력 필드
4. 유효성 검사
5. 저장 로직 구현

### Step 3: 1st_HR_Report/Details.razor (30분)
1. 평가 상세 정보 표시
2. Edit 버튼 연결

### Step 4: 네비게이션 메뉴 업데이트 (10분)
1. NavMenu.razor 수정

### Step 5: 테스트 (30분)
1. 평가 입력 시나리오 테스트
2. 권한 체크 테스트
3. 점수 계산 검증

---

## 9. 테스트 계획

### 테스트 시나리오 1: 본인평가 입력
1. 로그인 (일반 사용자)
2. `/1st_HR_Report` 접속
3. **확인**: 평가 대상 목록 표시 ✅
4. "평가" 버튼 클릭 (첫 번째 항목)
5. 난이도: 7, 성과: 8 입력
6. 코멘트: "프로젝트를 성공적으로 완료했습니다." 입력
7. 저장 버튼 클릭
8. **확인**: 목록으로 돌아가고, 평가 완료 상태 표시 ✅
9. **확인**: User_Evaluation_3 = 7.5 (자동 계산) ✅

### 테스트 시나리오 2: 평가 수정
1. `/1st_HR_Report` 접속
2. 이미 평가한 항목의 "평가" 버튼 클릭
3. **확인**: 기존 입력값 로드됨 ✅
4. 난이도: 8로 수정
5. 저장
6. **확인**: User_Evaluation_3 = 8.0으로 업데이트 ✅

### 테스트 시나리오 3: 평가 상세 확인
1. `/1st_HR_Report` 접속
2. "상세" 버튼 클릭
3. **확인**: 평가 결과 전체 표시 ✅
4. "수정" 버튼 클릭
5. **확인**: Edit 페이지로 이동 ✅

### 테스트 시나리오 4: 유효성 검사
1. Edit 페이지 접속
2. 난이도: 11 입력 (범위 초과)
3. 저장 시도
4. **확인**: 유효성 검사 오류 메시지 표시 ✅
5. 난이도: 9로 수정
6. **확인**: 저장 성공 ✅

### 테스트 시나리오 5: 권한 체크
1. 사용자 A로 로그인
2. 사용자 B의 평가 Edit URL 직접 접속 시도
3. **확인**: 접근 거부 또는 에러 메시지 ✅

---

## 10. 주의사항

1. **권한 체크 필수**:
   - Edit 페이지에서 report.Uid == currentUser.Uid 검증
   - 다른 사용자의 평가 수정 방지

2. **점수 계산**:
   - User_Evaluation_3는 자동 계산 (수동 입력 불가)
   - 소수점 첫째 자리까지 표시 (예: 7.5)

3. **평가 기간 체크** (선택 사항):
   - ProcessDb의 평가 기간 확인
   - 평가 기간 외에는 입력/수정 불가 처리

4. **데이터 무결성**:
   - ReportDb 데이터는 관리자가 일괄 생성
   - 사용자는 User_Evaluation_1~4만 수정 가능

5. **2025년 코드 참조**:
   - UI/UX는 2025년 프로젝트 참조
   - .NET 10 최신 기능 적용

---

## 11. 완료 조건

- [ ] 1st_HR_Report/Index.razor 완료
- [ ] 1st_HR_Report/Edit.razor 완료
- [ ] 1st_HR_Report/Details.razor 완료
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

**Phase 3-4-3**: 부서장/임원평가 (2nd, 3rd HR Report) 구현
- 작업지시서: `20260203_03_phase3_4_3_2nd_3rd_hr_report.md`

---

## 13. 관련 문서

**이슈**: [#009: Phase 3 WebApp 개발](../issues/009_phase3_webapp_development.md)
**Phase 2 Model**:
- [ReportDb](../../MdcHR26Apps.Models/ReportDb/)
- [TasksDb](../../MdcHR26Apps.Models/TasksDb/)
**선행 작업**: [20260203_01_phase3_4_1_agreement_subagreement.md](20260203_01_phase3_4_1_agreement_subagreement.md)
**참조 프로젝트**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages\1st_HR_Report`
