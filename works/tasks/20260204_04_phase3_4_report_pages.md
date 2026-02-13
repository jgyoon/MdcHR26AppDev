# 작업지시서: HR Report 페이지 구현 (25년도 코드 복사 기반)

**작업 번호**: 20260204_04
**작성일**: 2026-02-04
**작업 타입**: 25년도 코드 복사 → 26년도 DB 변경사항 반영
**참조 이슈**: [#015](../issues/015_agreement_teamleader_arbitrary_code_generation.md)

---

## ⚠️ 작업 원칙 (이슈 #015)

### DO (반드시 해야 할 것)
- ✅ **25년도 코드를 그대로 복사**
- ✅ **26년도 DB 변경사항만 수정**
- ✅ **작업 전 25년도 코드 철저히 분석**
- ✅ **변경사항을 명확히 문서화**

### DON'T (절대 하지 말아야 할 것)
- ❌ **임의로 코드 재작성 금지**
- ❌ **구조 변경 금지**
- ❌ **기능 단순화 금지**
- ❌ **"더 나은 방법"으로 개선 시도 금지**

---

## 1. 작업 개요

**목표**: 25년도 HR Report 페이지를 26년도로 이전

**대상 페이지**:
- 1st_HR_Report (본인평가): 3개 페이지 (Index, Edit, Details)
- 2nd_HR_Report (부서장평가): 5개 페이지 (Index, Edit, Details, Complete_2nd_Edit, Complete_2nd_Details)
- 3rd_HR_Report (임원평가): 5개 페이지 (Index, Edit, Details, Complete_3rd_Edit, Complete_3rd_Details)

**총 13개 페이지 (26개 파일: .razor + .razor.cs)**

---

## 2. 25년도 코드 경로

### 25년도 프로젝트
- **경로**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages\`

### 1st_HR_Report (3개 페이지)
```
C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages\1st_HR_Report\
├── Index.razor
├── Index.razor.cs
├── Edit.razor
├── Edit.razor.cs
├── Details.razor
└── Details.razor.cs
```

### 2nd_HR_Report (5개 페이지)
```
C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages\2nd_HR_Report\
├── Index.razor
├── Index.razor.cs
├── Edit.razor
├── Edit.razor.cs
├── Details.razor
├── Details.razor.cs
├── Complete_2nd_Edit.razor
├── Complete_2nd_Edit.razor.cs
├── Complete_2nd_Details.razor
└── Complete_2nd_Details.razor.cs
```

### 3rd_HR_Report (5개 페이지)
```
C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages\3rd_HR_Report\
├── Index.razor
├── Index.razor.cs
├── Edit.razor
├── Edit.razor.cs
├── Details.razor
├── Details.razor.cs
├── Complete_3rd_Edit.razor
├── Complete_3rd_Edit.razor.cs
├── Complete_3rd_Details.razor
└── Complete_3rd_Details.razor.cs
```

---

## 3. 26년도 코드 경로

### 26년도 프로젝트
- **경로**: `c:\Codes\00_Develop_Cursor\10_MdcHR26Apps\MdcHR26Apps.BlazorServer\Components\Pages\`

**동일한 구조로 생성**:
- `Components\Pages\1st_HR_Report\`
- `Components\Pages\2nd_HR_Report\`
- `Components\Pages\3rd_HR_Report\`

---

## 4. 26년도 DB 변경사항

### 4.1. Entity PK 필드명 변경

#### TasksDb (1st Report)
```csharp
// 25년도
public class TasksDb
{
    public Int64 Tid { get; set; }  // ✅ 동일
    public string UserId { get; set; }  // ❌
}

// 26년도
public class TasksDb
{
    public long Tid { get; set; }  // ✅ 동일 (Int64 → long만 변경)
    public long Uid { get; set; }  // ✅ UserId (string) → Uid (long)
}
```

#### SubAgreementDb (2nd/3rd Report)
```csharp
// 25년도
public class SubAgreementDb
{
    public Int64 SAid { get; set; }  // ❌
    public string UserId { get; set; }  // ❌
}

// 26년도
public class SubAgreementDb
{
    public long Sid { get; set; }  // ✅ SAid → Sid
    public long Uid { get; set; }  // ✅ UserId (string) → Uid (long)
}
```

### 4.2. Repository 반환 타입 변경
```csharp
// 25년도
Task<bool> UpdateAsync(TasksDb model);  // ❌
Task<bool> UpdateAsync(SubAgreementDb model);  // ❌

// 26년도
Task<int> UpdateAsync(TasksDb model);  // ✅ bool → int
Task<int> UpdateAsync(SubAgreementDb model);  // ✅ bool → int
```

**사용 예시**:
```csharp
// 25년도
bool success = await repository.UpdateAsync(model);
if (success) { /* 성공 */ }

// 26년도
int affectedRows = await repository.UpdateAsync(model);
if (affectedRows > 0) { /* 성공 */ }
```

### 4.3. 네임스페이스 변경
```csharp
// 25년도
using MdcHR25Apps.Models.EvaluationTasks;  // ❌
using MdcHR25Apps.Models.EvaluationSubAgreement;  // ❌
namespace MdcHR25Apps.BlazorApp.Pages._1st_HR_Report;  // ❌

// 26년도
using MdcHR26Apps.Models.EvaluationTasks;  // ✅
using MdcHR26Apps.Models.EvaluationSubAgreement;  // ✅
namespace MdcHR26Apps.BlazorServer.Components.Pages._1st_HR_Report;  // ✅
```

### 4.4. LoginStatus 속성명 동일
```csharp
// 25년도와 26년도 동일 (변경 없음)
loginStatusService.LoginStatus.LoginUid
loginStatusService.LoginStatus.LoginIsTeamLeader
loginStatusService.LoginStatus.LoginIsDirector
```

---

## 5. 작업 단계

### Step 1: 25년도 코드 분석
1. 25년도 1st_HR_Report/Index.razor.cs 전체 읽기
2. TasksRepository 메서드 사용 패턴 확인
3. SubAgreementRepository 메서드 사용 패턴 확인
4. Entity 필드 사용 확인
5. 비즈니스 로직 이해

### Step 2: 파일 복사 (1st_HR_Report 3개)
```bash
# 각 파일을 25년도에서 26년도로 복사
Index.razor + Index.razor.cs
Edit.razor + Edit.razor.cs
Details.razor + Details.razor.cs
```

### Step 3: 26년도 DB 변경사항 반영 (1st_HR_Report)

#### 3.1. 네임스페이스 일괄 변경
```csharp
// 모든 파일에서
MdcHR25Apps → MdcHR26Apps
BlazorApp → BlazorServer
Pages._1st_HR_Report → Components.Pages._1st_HR_Report
```

#### 3.2. Entity 필드명 변경
```csharp
// TasksDb 사용 부분
model.Tid  // ✅ 동일
model.UserId → model.Uid  // ✅ 변경
[Parameter] public Int64 Tid → [Parameter] public long Tid  // ✅ 타입만 변경
```

#### 3.3. UserId → Uid 변경
```csharp
// 모든 .razor.cs 파일에서
model.UserId → model.Uid
loginUser.LoginUserId → loginUser.LoginUid
```

#### 3.4. Repository 반환 타입 처리 변경
```csharp
// UpdateAsync 사용 부분
// 변경 전
bool success = await tasksRepository.UpdateAsync(model);
if (success)

// 변경 후
int affectedRows = await tasksRepository.UpdateAsync(model);
if (affectedRows > 0)
```

### Step 4: 파일 복사 (2nd_HR_Report 5개)
```bash
# 각 파일을 25년도에서 26년도로 복사
Index.razor + Index.razor.cs
Edit.razor + Edit.razor.cs
Details.razor + Details.razor.cs
Complete_2nd_Edit.razor + Complete_2nd_Edit.razor.cs
Complete_2nd_Details.razor + Complete_2nd_Details.razor.cs
```

### Step 5: 26년도 DB 변경사항 반영 (2nd_HR_Report)

#### 5.1. 네임스페이스 변경
```csharp
MdcHR25Apps → MdcHR26Apps
BlazorApp → BlazorServer
Pages._2nd_HR_Report → Components.Pages._2nd_HR_Report
```

#### 5.2. Entity PK 필드명 변경
```csharp
// SubAgreementDb 사용 부분
model.SAid → model.Sid
[Parameter] public Int64 SAid → [Parameter] public long Sid
```

#### 5.3. UserId → Uid 변경
```csharp
model.UserId → model.Uid
loginUser.LoginUserId → loginUser.LoginUid
```

#### 5.4. Repository 반환 타입 처리 변경
```csharp
// 변경 전
bool success = await subAgreementRepository.UpdateAsync(model);
if (success)

// 변경 후
int affectedRows = await subAgreementRepository.UpdateAsync(model);
if (affectedRows > 0)
```

### Step 6: 파일 복사 (3rd_HR_Report 5개)
```bash
# 각 파일을 25년도에서 26년도로 복사
Index.razor + Index.razor.cs
Edit.razor + Edit.razor.cs
Details.razor + Details.razor.cs
Complete_3rd_Edit.razor + Complete_3rd_Edit.razor.cs
Complete_3rd_Details.razor + Complete_3rd_Details.razor.cs
```

### Step 7: 26년도 DB 변경사항 반영 (3rd_HR_Report)
- 2nd_HR_Report와 동일한 변경사항 적용
- 네임스페이스, Entity PK (SAid → Sid), UserId → Uid, 반환 타입 처리

### Step 8: 빌드 테스트
```bash
cd "c:\Codes\00_Develop_Cursor\10_MdcHR26Apps\MdcHR26Apps.BlazorServer"
dotnet build
```

**예상 결과**: 0 errors, 0 warnings

### Step 9: 25년도와 기능 비교
- 25년도 페이지와 26년도 페이지의 기능이 동일한지 확인
- 비즈니스 로직이 그대로 유지되었는지 확인

---

## 6. 변경사항 체크리스트

### 1st_HR_Report (각 파일마다)
- [ ] 네임스페이스: `MdcHR25Apps` → `MdcHR26Apps`
- [ ] 네임스페이스: `BlazorApp` → `BlazorServer`
- [ ] 네임스페이스: `Pages._1st_HR_Report` → `Components.Pages._1st_HR_Report`
- [ ] Entity PK: `Tid` (동일, 타입만 Int64 → long)
- [ ] 사용자 ID: `UserId` (string) → `Uid` (long)
- [ ] 반환 타입: `bool success` → `int affectedRows`
- [ ] 조건문: `if (success)` → `if (affectedRows > 0)`
- [ ] 25년도 기능과 동일한지 확인

### 2nd_HR_Report (각 파일마다)
- [ ] 네임스페이스: `MdcHR25Apps` → `MdcHR26Apps`
- [ ] 네임스페이스: `BlazorApp` → `BlazorServer`
- [ ] 네임스페이스: `Pages._2nd_HR_Report` → `Components.Pages._2nd_HR_Report`
- [ ] Entity PK: `SAid` → `Sid`
- [ ] 사용자 ID: `UserId` (string) → `Uid` (long)
- [ ] 반환 타입: `bool success` → `int affectedRows`
- [ ] 조건문: `if (success)` → `if (affectedRows > 0)`
- [ ] 25년도 기능과 동일한지 확인

### 3rd_HR_Report (각 파일마다)
- [ ] 네임스페이스: `MdcHR25Apps` → `MdcHR26Apps`
- [ ] 네임스페이스: `BlazorApp` → `BlazorServer`
- [ ] 네임스페이스: `Pages._3rd_HR_Report` → `Components.Pages._3rd_HR_Report`
- [ ] Entity PK: `SAid` → `Sid`
- [ ] 사용자 ID: `UserId` (string) → `Uid` (long)
- [ ] 반환 타입: `bool success` → `int affectedRows`
- [ ] 조건문: `if (success)` → `if (affectedRows > 0)`
- [ ] 25년도 기능과 동일한지 확인

---

## 7. 예시: 2nd_HR_Report/Edit.razor.cs 변경사항

### 25년도 코드 (일부)
```csharp
using MdcHR25Apps.Models.EvaluationSubAgreement;
namespace MdcHR25Apps.BlazorApp.Pages._2nd_HR_Report;

public partial class Edit
{
    [Parameter] public Int64 SAid { get; set; }
    [Inject] public ISubAgreementRepository subAgreementRepository { get; set; } = null!;

    public SubAgreementDb model { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        model = await subAgreementRepository.GetByIdAsync(SAid);
    }

    protected async Task HandleSubmit()
    {
        model.Report_Score_2nd = /* 점수 */;
        bool success = await subAgreementRepository.UpdateAsync(model);

        if (success)
        {
            urlActions.Move2ndHRReportIndexPage();
        }
    }
}
```

### 26년도 코드 (변경 후)
```csharp
using MdcHR26Apps.Models.EvaluationSubAgreement;  // ✅ 변경
namespace MdcHR26Apps.BlazorServer.Components.Pages._2nd_HR_Report;  // ✅ 변경

public partial class Edit
{
    [Parameter] public long Sid { get; set; }  // ✅ Int64 SAid → long Sid
    [Inject] public ISubAgreementRepository subAgreementRepository { get; set; } = null!;

    public SubAgreementDb model { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        model = await subAgreementRepository.GetByIdAsync(Sid);  // ✅ SAid → Sid
    }

    protected async Task HandleSubmit()
    {
        model.Report_Score_2nd = /* 점수 */;
        int affectedRows = await subAgreementRepository.UpdateAsync(model);  // ✅ bool → int

        if (affectedRows > 0)  // ✅ success → affectedRows > 0
        {
            urlActions.Move2ndHRReportIndexPage();
        }
    }
}
```

---

## 8. 주의사항

### 8.1. 컴포넌트 경로 확인
25년도에서 사용한 컴포넌트가 26년도에도 존재하는지 확인:
```razor
@* 컴포넌트 사용 예시 *@
<ReportListTable tasks="@tasks" OnEdit="@HandleEdit" />
<TeamLeaderReportListTable subAgreements="@subAgreements" OnEdit="@HandleEdit" />
```

**확인 경로**: `MdcHR26Apps.BlazorServer\Components\Pages\Components\Report\`

### 8.2. 라우트 경로 확인
```csharp
// 25년도와 26년도 동일하게 유지
@page "/1st_HR_Report/Index"
@page "/1st_HR_Report/Edit/{Tid:long}"
@page "/2nd_HR_Report/Edit/{Sid:long}"
```

### 8.3. 평가 점수 필드명 확인
```csharp
// 1st Report (TasksDb)
model.SelfScore  // 본인 평가 점수

// 2nd Report (SubAgreementDb)
model.Report_Score_2nd  // 부서장 평가 점수

// 3rd Report (SubAgreementDb)
model.Report_Score_3rd  // 임원 평가 점수
```

---

## 9. 테스트 시나리오

### 빌드 테스트
```bash
dotnet build
```
**예상 결과**: 0 errors

### 기능 테스트 (수동)
1. **1st_HR_Report/Index**: 본인의 Tasks 리스트 표시
2. **1st_HR_Report/Edit**: Tasks 본인 평가 점수 입력
3. **1st_HR_Report/Details**: Tasks 상세 조회
4. **2nd_HR_Report/Index**: 부서원 SubAgreement 리스트 표시 (팀장 권한)
5. **2nd_HR_Report/Edit**: SubAgreement 부서장 평가 점수 입력
6. **2nd_HR_Report/Details**: SubAgreement 상세 조회
7. **2nd_HR_Report/Complete_2nd_Edit**: 2차 평가 완료 처리
8. **2nd_HR_Report/Complete_2nd_Details**: 완료된 2차 평가 조회
9. **3rd_HR_Report/Index**: 전체 SubAgreement 리스트 표시 (임원 권한)
10. **3rd_HR_Report/Edit**: SubAgreement 임원 평가 점수 입력
11. **3rd_HR_Report/Details**: SubAgreement 상세 조회
12. **3rd_HR_Report/Complete_3rd_Edit**: 3차 평가 완료 처리
13. **3rd_HR_Report/Complete_3rd_Details**: 완료된 3차 평가 조회

---

## 10. 완료 조건

### 코드 작성
- [ ] 25년도 1st_HR_Report 페이지 3개 복사 완료
- [ ] 25년도 2nd_HR_Report 페이지 5개 복사 완료
- [ ] 25년도 3rd_HR_Report 페이지 5개 복사 완료
- [ ] 26년도 DB 변경사항 반영 (네임스페이스)
- [ ] 26년도 DB 변경사항 반영 (Entity PK: SAid → Sid)
- [ ] 26년도 DB 변경사항 반영 (UserId → Uid)
- [ ] 26년도 DB 변경사항 반영 (Repository 반환 타입)

### 검증
- [ ] 빌드 성공 (0 errors)
- [ ] 25년도 기능과 동일한지 확인
- [ ] 비즈니스 로직 변경 없음 확인
- [ ] 컴포넌트 정상 동작 확인
- [ ] 평가 점수 필드 정상 동작 확인

---

## 11. 참조 문서

**이슈**:
- [#015: Agreement TeamLeader 페이지 - 임의 코드 작성으로 인한 디버깅 어려움](../issues/015_agreement_teamleader_arbitrary_code_generation.md)

**25년도 코드**:
- `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages\1st_HR_Report\`
- `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages\2nd_HR_Report\`
- `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages\3rd_HR_Report\`

**26년도 Entity/Repository**:
- `MdcHR26Apps.Models\EvaluationTasks\TasksDb.cs` (1st Report)
- `MdcHR26Apps.Models\EvaluationTasks\ITasksRepository.cs`
- `MdcHR26Apps.Models\EvaluationSubAgreement\SubAgreementDb.cs` (2nd/3rd Report)
- `MdcHR26Apps.Models\EvaluationSubAgreement\ISubAgreementRepository.cs`

---

**작성자**: Claude Sonnet 4.5
**작업 방식**: 25년도 코드 복사 → 26년도 DB 변경사항 반영
**참조 이슈**: #015 - 임의 코드 작성 금지 원칙
