# 작업지시서: SubAgreement 페이지 구현 (25년도 코드 복사 기반)

**작업 번호**: 20260204_03
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

**목표**: 25년도 SubAgreement 페이지를 26년도로 이전

**대상 페이지**:
- SubAgreement/User: 5개 페이지 (Index, Create, Edit, Delete, Details)
- SubAgreement/TeamLeader: 5개 페이지 (Index, Details, SubDetails, CompleteSubAgreement, ResetSubAgreement)

**총 10개 페이지 (20개 파일: .razor + .razor.cs)**

---

## 2. 25년도 코드 경로

### 25년도 프로젝트
- **경로**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages\SubAgreement\`

### SubAgreement/User (5개 페이지)
```
C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages\SubAgreement\User\
├── Index.razor
├── Index.razor.cs
├── Create.razor
├── Create.razor.cs
├── Edit.razor
├── Edit.razor.cs
├── Delete.razor
├── Delete.razor.cs
├── Details.razor
└── Details.razor.cs
```

### SubAgreement/TeamLeader (5개 페이지)
```
C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages\SubAgreement\TeamLeader\
├── Index.razor
├── Index.razor.cs
├── Details.razor
├── Details.razor.cs
├── SubDetails.razor
├── SubDetails.razor.cs
├── CompleteSubAgreement.razor
├── CompleteSubAgreement.razor.cs
├── ResetSubAgreement.razor
└── ResetSubAgreement.razor.cs
```

---

## 3. 26년도 코드 경로

### 26년도 프로젝트
- **경로**: `c:\Codes\00_Develop_Cursor\10_MdcHR26Apps\MdcHR26Apps.BlazorServer\Components\Pages\SubAgreement\`

**동일한 구조로 생성**

---

## 4. 26년도 DB 변경사항

### 4.1. Entity PK 필드명 변경
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
Task<bool> UpdateAsync(SubAgreementDb model);  // ❌
Task<bool> DeleteAsync(long id);  // ❌

// 26년도
Task<int> UpdateAsync(SubAgreementDb model);  // ✅ bool → int
Task<int> DeleteAsync(long id);  // ✅ bool → int
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
using MdcHR25Apps.Models.EvaluationSubAgreement;  // ❌
namespace MdcHR25Apps.BlazorApp.Pages.SubAgreement.User;  // ❌

// 26년도
using MdcHR26Apps.Models.EvaluationSubAgreement;  // ✅
namespace MdcHR26Apps.BlazorServer.Components.Pages.SubAgreement.User;  // ✅
```

### 4.4. LoginStatus 속성명 동일
```csharp
// 25년도와 26년도 동일 (변경 없음)
loginStatusService.LoginStatus.LoginUid
loginStatusService.LoginStatus.LoginIsTeamLeader
```

---

## 5. 작업 단계

### Step 1: 25년도 코드 분석
1. 25년도 SubAgreement/User/Index.razor.cs 전체 읽기
2. Repository 메서드 사용 패턴 확인
3. Entity 필드 사용 확인
4. 비즈니스 로직 이해

### Step 2: 파일 복사 (User 페이지 5개)
```bash
# 각 파일을 25년도에서 26년도로 복사
Index.razor + Index.razor.cs
Create.razor + Create.razor.cs
Edit.razor + Edit.razor.cs
Delete.razor + Delete.razor.cs
Details.razor + Details.razor.cs
```

### Step 3: 26년도 DB 변경사항 반영 (User 페이지)

#### 3.1. 네임스페이스 일괄 변경
```csharp
// 모든 파일에서
MdcHR25Apps → MdcHR26Apps
BlazorApp → BlazorServer
Pages.SubAgreement → Components.Pages.SubAgreement
```

#### 3.2. Entity PK 필드명 변경
```csharp
// 모든 .razor.cs 파일에서
model.SAid → model.Sid
[Parameter] public Int64 SAid → [Parameter] public long Sid
```

#### 3.3. UserId → Uid 변경
```csharp
// 모든 .razor.cs 파일에서
model.UserId → model.Uid
loginUser.LoginUserId → loginUser.LoginUid
GetByUserIdAllAsync(userId) → GetByUserIdAllAsync(uid)
```

#### 3.4. Repository 반환 타입 처리 변경
```csharp
// UpdateAsync 사용 부분
// 변경 전
bool success = await repository.UpdateAsync(model);
if (success)

// 변경 후
int affectedRows = await repository.UpdateAsync(model);
if (affectedRows > 0)
```

```csharp
// DeleteAsync 사용 부분
// 변경 전
bool success = await repository.DeleteAsync(id);
if (success)

// 변경 후
int affectedRows = await repository.DeleteAsync(id);
if (affectedRows > 0)
```

### Step 4: 파일 복사 (TeamLeader 페이지 5개)
```bash
# 각 파일을 25년도에서 26년도로 복사
Index.razor + Index.razor.cs
Details.razor + Details.razor.cs
SubDetails.razor + SubDetails.razor.cs
CompleteSubAgreement.razor + CompleteSubAgreement.razor.cs
ResetSubAgreement.razor + ResetSubAgreement.razor.cs
```

### Step 5: 26년도 DB 변경사항 반영 (TeamLeader 페이지)
- User 페이지와 동일한 변경사항 적용
- 네임스페이스, Entity PK, UserId → Uid, 반환 타입 처리

### Step 6: 빌드 테스트
```bash
cd "c:\Codes\00_Develop_Cursor\10_MdcHR26Apps\MdcHR26Apps.BlazorServer"
dotnet build
```

**예상 결과**: 0 errors, 0 warnings

### Step 7: 25년도와 기능 비교
- 25년도 페이지와 26년도 페이지의 기능이 동일한지 확인
- 비즈니스 로직이 그대로 유지되었는지 확인

---

## 6. 변경사항 체크리스트

### 각 파일마다 확인할 사항
- [ ] 네임스페이스: `MdcHR25Apps` → `MdcHR26Apps`
- [ ] 네임스페이스: `BlazorApp` → `BlazorServer`
- [ ] 네임스페이스: `Pages.SubAgreement` → `Components.Pages.SubAgreement`
- [ ] Entity PK: `SAid` → `Sid`
- [ ] 사용자 ID: `UserId` (string) → `Uid` (long)
- [ ] Repository 메서드: `GetByUserIdAllAsync(userId)` → `GetByUserIdAllAsync(uid)`
- [ ] 반환 타입: `bool success` → `int affectedRows`
- [ ] 조건문: `if (success)` → `if (affectedRows > 0)`
- [ ] 25년도 기능과 동일한지 확인

---

## 7. 예시: Index.razor.cs 변경사항

### 25년도 코드 (일부)
```csharp
using MdcHR25Apps.Models.EvaluationSubAgreement;
namespace MdcHR25Apps.BlazorApp.Pages.SubAgreement.User;

public partial class Index
{
    [Inject] public ISubAgreementRepository subAgreementRepository { get; set; } = null!;

    public List<SubAgreementDb> agreements { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        var loginUser = loginStatusService.LoginStatus;
        agreements = await subAgreementRepository.GetByUserIdAllAsync(loginUser.LoginUserId);
    }

    protected void HandleEdit(Int64 said)
    {
        urlActions.MoveSubAgreementUserEditPage(said);
    }
}
```

### 26년도 코드 (변경 후)
```csharp
using MdcHR26Apps.Models.EvaluationSubAgreement;  // ✅ 변경
namespace MdcHR26Apps.BlazorServer.Components.Pages.SubAgreement.User;  // ✅ 변경

public partial class Index
{
    [Inject] public ISubAgreementRepository subAgreementRepository { get; set; } = null!;

    public List<SubAgreementDb> agreements { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        var loginUser = loginStatusService.LoginStatus;
        agreements = await subAgreementRepository.GetByUserIdAllAsync(loginUser.LoginUid);  // ✅ LoginUserId → LoginUid
    }

    protected void HandleEdit(long sid)  // ✅ Int64 SAid → long Sid
    {
        urlActions.MoveSubAgreementUserEditPage(sid);  // ✅ said → sid
    }
}
```

---

## 8. 주의사항

### 8.1. 컴포넌트 경로 확인
25년도에서 사용한 컴포넌트가 26년도에도 존재하는지 확인:
```razor
@* 컴포넌트 사용 예시 *@
<SubAgreementListTable agreements="@agreements" OnEdit="@HandleEdit" />
```

**확인 경로**: `MdcHR26Apps.BlazorServer\Components\Pages\Components\SubAgreement\`

### 8.2. 라우트 경로 확인
```csharp
// 25년도와 26년도 동일하게 유지
@page "/SubAgreement/User/Index"
@page "/SubAgreement/User/Edit/{Sid:long}"
```

### 8.3. 메서드명 확인
```csharp
// Repository 메서드명은 26년도에서도 동일
GetByUserIdAllAsync()  // ✅ (파라미터 타입만 변경)
GetByIdAsync()
AddAsync()
UpdateAsync()  // 반환 타입만 변경
DeleteAsync()  // 반환 타입만 변경
```

---

## 9. 테스트 시나리오

### 빌드 테스트
```bash
dotnet build
```
**예상 결과**: 0 errors

### 기능 테스트 (수동)
1. **User/Index**: 본인의 SubAgreement 리스트 표시
2. **User/Create**: SubAgreement 생성
3. **User/Edit**: SubAgreement 수정
4. **User/Delete**: SubAgreement 삭제
5. **User/Details**: SubAgreement 상세 조회
6. **TeamLeader/Index**: 팀원 SubAgreement 리스트 표시
7. **TeamLeader/Details**: 팀원 SubAgreement 상세 조회
8. **TeamLeader/SubDetails**: 특정 사용자의 SubAgreement 리스트
9. **TeamLeader/CompleteSubAgreement**: 일괄 승인
10. **TeamLeader/ResetSubAgreement**: 일괄 삭제

---

## 10. 완료 조건

### 코드 작성
- [ ] 25년도 SubAgreement/User 페이지 5개 복사 완료
- [ ] 25년도 SubAgreement/TeamLeader 페이지 5개 복사 완료
- [ ] 26년도 DB 변경사항 반영 (네임스페이스)
- [ ] 26년도 DB 변경사항 반영 (Entity PK: SAid → Sid)
- [ ] 26년도 DB 변경사항 반영 (UserId → Uid)
- [ ] 26년도 DB 변경사항 반영 (Repository 반환 타입)

### 검증
- [ ] 빌드 성공 (0 errors)
- [ ] 25년도 기능과 동일한지 확인
- [ ] 비즈니스 로직 변경 없음 확인
- [ ] 컴포넌트 정상 동작 확인

---

## 11. 참조 문서

**이슈**:
- [#015: Agreement TeamLeader 페이지 - 임의 코드 작성으로 인한 디버깅 어려움](../issues/015_agreement_teamleader_arbitrary_code_generation.md)

**25년도 코드**:
- `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages\SubAgreement\`

**26년도 Entity/Repository**:
- `MdcHR26Apps.Models\EvaluationSubAgreement\SubAgreementDb.cs`
- `MdcHR26Apps.Models\EvaluationSubAgreement\ISubAgreementRepository.cs`
- `MdcHR26Apps.Models\EvaluationSubAgreement\SubAgreementRepository.cs`

---

**작성자**: Claude Sonnet 4.5
**작업 방식**: 25년도 코드 복사 → 26년도 DB 변경사항 반영
**참조 이슈**: #015 - 임의 코드 작성 금지 원칙
