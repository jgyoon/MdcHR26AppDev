# 작업지시서: DeptObjective 페이지 구현 (25년도 코드 복사 기반)

**작업 번호**: 20260204_05
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

**목표**: 25년도 DeptObjective 페이지를 26년도로 이전

**대상 페이지**:
- DeptObjective 목록: 2개 페이지 (Main, Sub)
- MainObjective CRUD: 4개 페이지 (Create, Edit, Delete, Details)
- SubObjective CRUD: 4개 페이지 (Create, Edit, Delete, Details)

**총 10개 페이지 (20개 파일: .razor + .razor.cs)**

---

## 2. 25년도 코드 경로

### 25년도 프로젝트
- **경로**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages\DeptObjective\`

### DeptObjective 목록 (2개 페이지)
```
C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages\DeptObjective\
├── Main.razor
├── Main.razor.cs
├── Sub.razor
└── Sub.razor.cs
```

### MainObjective CRUD (4개 페이지)
```
C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages\DeptObjective\MainObjective\
├── Create.razor
├── Create.razor.cs
├── Edit.razor
├── Edit.razor.cs
├── Delete.razor
├── Delete.razor.cs
├── Details.razor
└── Details.razor.cs
```

### SubObjective CRUD (4개 페이지)
```
C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages\DeptObjective\SubObjective\
├── Create.razor
├── Create.razor.cs
├── Edit.razor
├── Edit.razor.cs
├── Delete.razor
├── Delete.razor.cs
├── Details.razor
└── Details.razor.cs
```

---

## 3. 26년도 코드 경로

### 26년도 프로젝트
- **경로**: `c:\Codes\00_Develop_Cursor\10_MdcHR26Apps\MdcHR26Apps.BlazorServer\Components\Pages\DeptObjective\`

**동일한 구조로 생성**:
- `Components\Pages\DeptObjective\Main.razor` + `.cs`
- `Components\Pages\DeptObjective\Sub.razor` + `.cs`
- `Components\Pages\DeptObjective\MainObjective\` (4개 페이지)
- `Components\Pages\DeptObjective\SubObjective\` (4개 페이지)

---

## 4. 26년도 DB 변경사항

### 4.1. Entity PK 필드명 변경
```csharp
// 25년도
public class DeptObjectiveDb
{
    public Int64 DOid { get; set; }  // ❌
}

// 26년도
public class DeptObjectiveDb
{
    public long DeptObjectiveDbId { get; set; }  // ✅ DOid → DeptObjectiveDbId
}
```

**중요**: PK 필드명이 `DOid`에서 `DeptObjectiveDbId`로 **완전히 변경**되었습니다.

### 4.2. Repository 반환 타입 변경
```csharp
// 25년도
Task<bool> UpdateAsync(DeptObjectiveDb model);  // ❌
Task<bool> DeleteAsync(long id);  // ❌

// 26년도
Task<int> UpdateAsync(DeptObjectiveDb model);  // ✅ bool → int
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

### 4.3. 감사 필드 (Audit Fields)
```csharp
// 25년도와 26년도 동일 (변경 없음)
public long? CreatedBy { get; set; }
public DateTime? CreatedAt { get; set; }
public long? UpdatedBy { get; set; }
public DateTime? UpdatedAt { get; set; }
```

**중요**: Create 시 CreatedBy/CreatedAt 설정, Update 시 UpdatedBy/UpdatedAt 설정

### 4.4. 네임스페이스 변경
```csharp
// 25년도
using MdcHR25Apps.Models.DeptObjective;  // ❌
namespace MdcHR25Apps.BlazorApp.Pages.DeptObjective;  // ❌

// 26년도
using MdcHR26Apps.Models.DeptObjective;  // ✅
namespace MdcHR26Apps.BlazorServer.Components.Pages.DeptObjective;  // ✅
```

### 4.5. LoginStatus 속성명 동일
```csharp
// 25년도와 26년도 동일 (변경 없음)
loginStatusService.LoginStatus.LoginUid
loginStatusService.LoginStatus.LoginIsDeptObjectiveWriter
loginStatusService.LoginStatus.LoginIsAdministrator
```

---

## 5. 작업 단계

### Step 1: 25년도 코드 분석
1. 25년도 DeptObjective/Main.razor.cs 전체 읽기
2. 25년도 MainObjective/Create.razor.cs 전체 읽기
3. Repository 메서드 사용 패턴 확인
4. Entity 필드 사용 확인 (특히 DOid)
5. 감사 필드 설정 로직 확인
6. 비즈니스 로직 이해

### Step 2: 파일 복사 (DeptObjective 목록 2개)
```bash
# 각 파일을 25년도에서 26년도로 복사
Main.razor + Main.razor.cs
Sub.razor + Sub.razor.cs
```

### Step 3: 26년도 DB 변경사항 반영 (목록 페이지)

#### 3.1. 네임스페이스 일괄 변경
```csharp
// 모든 파일에서
MdcHR25Apps → MdcHR26Apps
BlazorApp → BlazorServer
Pages.DeptObjective → Components.Pages.DeptObjective
```

#### 3.2. Entity PK 필드명 변경
```csharp
// 모든 .razor.cs 파일에서
model.DOid → model.DeptObjectiveDbId
[Parameter] public Int64 DOid → [Parameter] public long DeptObjectiveDbId
```

**주의**: PK 필드명이 완전히 바뀌었으므로 모든 참조를 변경해야 합니다.

### Step 4: 파일 복사 (MainObjective CRUD 4개)
```bash
# 각 파일을 25년도에서 26년도로 복사
Create.razor + Create.razor.cs
Edit.razor + Edit.razor.cs
Delete.razor + Delete.razor.cs
Details.razor + Details.razor.cs
```

### Step 5: 26년도 DB 변경사항 반영 (MainObjective)

#### 5.1. 네임스페이스 변경
```csharp
MdcHR25Apps → MdcHR26Apps
BlazorApp → BlazorServer
Pages.DeptObjective.MainObjective → Components.Pages.DeptObjective.MainObjective
```

#### 5.2. Entity PK 필드명 변경
```csharp
// 모든 사용 부분
model.DOid → model.DeptObjectiveDbId
[Parameter] public Int64 DOid → [Parameter] public long DeptObjectiveDbId
urlActions.MoveDeptObjectiveMainDetailsPage(model.DOid)
  → urlActions.MoveDeptObjectiveMainDetailsPage(model.DeptObjectiveDbId)
```

#### 5.3. Repository 반환 타입 처리 변경
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
bool success = await repository.DeleteAsync(DOid);
if (success)

// 변경 후
int affectedRows = await repository.DeleteAsync(DeptObjectiveDbId);
if (affectedRows > 0)
```

#### 5.4. 감사 필드 설정 확인
```csharp
// Create.razor.cs에서
model.CreatedBy = loginUser.LoginUid;
model.CreatedAt = DateTime.Now;

// Edit.razor.cs에서
model.UpdatedBy = loginUser.LoginUid;
model.UpdatedAt = DateTime.Now;
```

**주의**: 25년도 코드에 이미 설정되어 있으면 그대로 복사

### Step 6: 파일 복사 (SubObjective CRUD 4개)
```bash
# 각 파일을 25년도에서 26년도로 복사
Create.razor + Create.razor.cs
Edit.razor + Edit.razor.cs
Delete.razor + Delete.razor.cs
Details.razor + Details.razor.cs
```

### Step 7: 26년도 DB 변경사항 반영 (SubObjective)
- MainObjective와 동일한 변경사항 적용
- 네임스페이스, Entity PK (DOid → DeptObjectiveDbId), 반환 타입 처리, 감사 필드

### Step 8: 빌드 테스트
```bash
cd "c:\Codes\00_Develop_Cursor\10_MdcHR26Apps\MdcHR26Apps.BlazorServer"
dotnet build
```

**예상 결과**: 0 errors, 0 warnings

### Step 9: 25년도와 기능 비교
- 25년도 페이지와 26년도 페이지의 기능이 동일한지 확인
- 비즈니스 로직이 그대로 유지되었는지 확인
- ObjectiveType 필터링이 정상 작동하는지 확인 ("Main" / "Sub")

---

## 6. 변경사항 체크리스트

### 각 파일마다 확인할 사항
- [ ] 네임스페이스: `MdcHR25Apps` → `MdcHR26Apps`
- [ ] 네임스페이스: `BlazorApp` → `BlazorServer`
- [ ] 네임스페이스: `Pages.DeptObjective` → `Components.Pages.DeptObjective`
- [ ] Entity PK: `DOid` → `DeptObjectiveDbId`
- [ ] Parameter: `[Parameter] public Int64 DOid` → `[Parameter] public long DeptObjectiveDbId`
- [ ] Repository 메서드 호출: `GetByIdAsync(DOid)` → `GetByIdAsync(DeptObjectiveDbId)`
- [ ] 반환 타입: `bool success` → `int affectedRows`
- [ ] 조건문: `if (success)` → `if (affectedRows > 0)`
- [ ] 감사 필드 설정: CreatedBy, CreatedAt (Create), UpdatedBy, UpdatedAt (Edit)
- [ ] 25년도 기능과 동일한지 확인

---

## 7. 예시: MainObjective/Edit.razor.cs 변경사항

### 25년도 코드 (일부)
```csharp
using MdcHR25Apps.Models.DeptObjective;
namespace MdcHR25Apps.BlazorApp.Pages.DeptObjective.MainObjective;

public partial class Edit
{
    [Parameter] public Int64 DOid { get; set; }
    [Inject] public IDeptObjectiveRepository deptObjectiveRepository { get; set; } = null!;

    public DeptObjectiveDb model { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        model = await deptObjectiveRepository.GetByIdAsync(DOid);
    }

    protected async Task HandleSubmit()
    {
        model.UpdatedBy = loginUser.LoginUid;
        model.UpdatedAt = DateTime.Now;

        bool success = await deptObjectiveRepository.UpdateAsync(model);

        if (success)
        {
            urlActions.MoveDeptObjectiveMainIndexPage();
        }
    }
}
```

### 26년도 코드 (변경 후)
```csharp
using MdcHR26Apps.Models.DeptObjective;  // ✅ 변경
namespace MdcHR26Apps.BlazorServer.Components.Pages.DeptObjective.MainObjective;  // ✅ 변경

public partial class Edit
{
    [Parameter] public long DeptObjectiveDbId { get; set; }  // ✅ Int64 DOid → long DeptObjectiveDbId
    [Inject] public IDeptObjectiveRepository deptObjectiveRepository { get; set; } = null!;

    public DeptObjectiveDb model { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        model = await deptObjectiveRepository.GetByIdAsync(DeptObjectiveDbId);  // ✅ DOid → DeptObjectiveDbId
    }

    protected async Task HandleSubmit()
    {
        model.UpdatedBy = loginUser.LoginUid;
        model.UpdatedAt = DateTime.Now;

        int affectedRows = await deptObjectiveRepository.UpdateAsync(model);  // ✅ bool → int

        if (affectedRows > 0)  // ✅ success → affectedRows > 0
        {
            urlActions.MoveDeptObjectiveMainIndexPage();
        }
    }
}
```

---

## 8. 주의사항

### 8.1. PK 필드명 완전 변경
```csharp
// 25년도
model.DOid

// 26년도
model.DeptObjectiveDbId  // 완전히 다른 이름
```

**주의**: 단순히 약어가 아니라 **완전히 다른 필드명**이므로, 모든 참조를 빠짐없이 변경해야 합니다.

### 8.2. ObjectiveType 필터링
```csharp
// Main.razor.cs
var mainList = allObjectives.Where(x => x.ObjectiveType == "Main").ToList();

// Sub.razor.cs
var subList = allObjectives.Where(x => x.ObjectiveType == "Sub").ToList();
```

**주의**: 25년도 코드에 이미 포함되어 있으면 그대로 복사

### 8.3. 라우트 경로 확인
```csharp
// 25년도와 26년도 동일하게 유지
@page "/DeptObjective/Main"
@page "/DeptObjective/MainObjective/Edit/{DeptObjectiveDbId:long}"  // ✅ PK 필드명 변경
```

### 8.4. 감사 필드 필수 설정
```csharp
// Create
model.CreatedBy = loginUser.LoginUid;
model.CreatedAt = DateTime.Now;

// Edit
model.UpdatedBy = loginUser.LoginUid;
model.UpdatedAt = DateTime.Now;
```

---

## 9. 테스트 시나리오

### 빌드 테스트
```bash
dotnet build
```
**예상 결과**: 0 errors

### 기능 테스트 (수동)
1. **DeptObjective/Main**: Main Objective 목록 조회 (ObjectiveType == "Main")
2. **DeptObjective/Sub**: Sub Objective 목록 조회 (ObjectiveType == "Sub")
3. **MainObjective/Create**: Main Objective 생성 (ObjectiveType = "Main" 설정)
4. **MainObjective/Edit**: Main Objective 수정 (감사 필드 UpdatedBy, UpdatedAt 설정)
5. **MainObjective/Delete**: Main Objective 삭제
6. **MainObjective/Details**: Main Objective 상세 조회 (감사 필드 표시)
7. **SubObjective/Create**: Sub Objective 생성 (ObjectiveType = "Sub" 설정)
8. **SubObjective/Edit**: Sub Objective 수정 (감사 필드 UpdatedBy, UpdatedAt 설정)
9. **SubObjective/Delete**: Sub Objective 삭제
10. **SubObjective/Details**: Sub Objective 상세 조회 (감사 필드 표시)

---

## 10. 완료 조건

### 코드 작성
- [ ] 25년도 DeptObjective 목록 페이지 2개 복사 완료
- [ ] 25년도 MainObjective CRUD 4개 복사 완료
- [ ] 25년도 SubObjective CRUD 4개 복사 완료
- [ ] 26년도 DB 변경사항 반영 (네임스페이스)
- [ ] 26년도 DB 변경사항 반영 (Entity PK: DOid → DeptObjectiveDbId)
- [ ] 26년도 DB 변경사항 반영 (Repository 반환 타입)
- [ ] 감사 필드 설정 확인 (Create: CreatedBy/CreatedAt, Edit: UpdatedBy/UpdatedAt)

### 검증
- [ ] 빌드 성공 (0 errors)
- [ ] 25년도 기능과 동일한지 확인
- [ ] 비즈니스 로직 변경 없음 확인
- [ ] ObjectiveType 필터링 정상 동작 확인
- [ ] 감사 필드 정상 설정 확인

---

## 11. 참조 문서

**이슈**:
- [#015: Agreement TeamLeader 페이지 - 임의 코드 작성으로 인한 디버깅 어려움](../issues/015_agreement_teamleader_arbitrary_code_generation.md)

**25년도 코드**:
- `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages\DeptObjective\`

**26년도 Entity/Repository**:
- `MdcHR26Apps.Models\DeptObjective\DeptObjectiveDb.cs`
- `MdcHR26Apps.Models\DeptObjective\IDeptObjectiveRepository.cs`
- `MdcHR26Apps.Models\DeptObjective\DeptObjectiveRepository.cs`

---

**작성자**: Claude Sonnet 4.5
**작업 방식**: 25년도 코드 복사 → 26년도 DB 변경사항 반영
**참조 이슈**: #015 - 임의 코드 작성 금지 원칙
