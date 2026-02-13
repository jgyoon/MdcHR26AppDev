# 작업지시서: DeptObjective 페이지 구현 (25년도 코드 복사 기반) - 수정본

**작업 번호**: 20260208_02
**작성일**: 2026-02-08
**작업 타입**: 25년도 코드 복사 → 26년도 DB 변경사항 반영
**참조 이슈**: [#015](../issues/015_agreement_teamleader_arbitrary_code_generation.md), [#016](../issues/016_phase3_4_db_sync_and_2025_differences.md)
**이전 작업지시서**: 20260204_05 (❌ 폐기 - DB 변경사항 분석 오류)

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

## 2. 25년도 코드 분석 결과

### 2.1. Entity 구조 (변경 없음!)
```csharp
// 25년도와 26년도 동일
public class DeptObjectiveDb
{
    public Int64 DeptObjectiveDbId { get; set; }  // ✅ PK (동일)
    public Int64 EDepartId { get; set; }
    public string ObjectiveTitle { get; set; }
    public string ObjectiveContents { get; set; }
    public string? Remarks { get; set; }
}
```

### 2.2. 파라미터 이름 (변경 없음!)
```csharp
// 25년도
[Parameter] public Int64 Id { get; set; }  // ✅ Id 사용 (DeptObjectiveDbId 아님!)

// 26년도도 동일하게 사용
[Parameter] public long Id { get; set; }  // ✅ Int64 → long (동의어)
```

### 2.3. Repository 반환 타입 (변경 없음!)
```csharp
// 25년도와 26년도 동일
Task<DeptObjectiveDb> AddAsync(DeptObjectiveDb model);  // ✅ 모델 반환
Task<bool> UpdateAsync(DeptObjectiveDb model);  // ✅ bool 반환 (변경 없음!)
Task<bool> DeleteAsync(long id);  // ✅ bool 반환 (변경 없음!)
```

### 2.4. ObjectiveType 필드 (25년도에 없음!)
```csharp
// 25년도: ObjectiveType 필드 없음
// - Main(전사목표): EDepartId = 1 (메디칼파크 고정)
// - Sub(부서목표): 로그인 사용자의 EDepartId

// 26년도: ObjectiveType 필드 필요
public string ObjectiveType { get; set; }  // "Main" or "Sub"
```

### 2.5. 감사 필드 (25년도에 없음!)
```csharp
// 25년도: 감사 필드 없음

// 26년도: 감사 필드 필요
public Int64 CreatedBy { get; set; }
public DateTime CreatedAt { get; set; }
public Int64? UpdatedBy { get; set; }
public DateTime? UpdatedAt { get; set; }
```

---

## 3. 26년도 DB 변경사항 (정확한 분석)

### ✅ 변경되는 항목

| 항목 | 25년도 | 26년도 | 변경 내용 |
|------|--------|--------|----------|
| **네임스페이스** | `MdcHR25Apps.BlazorApp` | `MdcHR26Apps.BlazorServer` | ✅ 변경 필요 |
| **ObjectiveType 필드** | ❌ 없음 | ✅ "Main" / "Sub" | ✅ **추가 필요** |
| **감사 필드** | ❌ 없음 | ✅ Created/UpdatedBy/At | ✅ **추가 필요** |

### ❌ 변경되지 않는 항목

| 항목 | 25년도 | 26년도 | 변경 여부 |
|------|--------|--------|-----------|
| **Entity PK** | `DeptObjectiveDbId` | `DeptObjectiveDbId` | ❌ 동일 |
| **파라미터 이름** | `Id` | `Id` | ❌ 동일 |
| **AddAsync 반환** | `Task<Model>` | `Task<Model>` | ❌ 동일 |
| **Update/Delete 반환** | `Task<bool>` | `Task<bool>` | ❌ 동일 |

---

## 4. 작업 단계

### Step 1: 25년도 코드 복사 (10개 페이지, 20개 파일)

#### DeptObjective 목록 (2개)
```
25년도: C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages\DeptObjective\
26년도: c:\Codes\00_Develop_Cursor\10_MdcHR26Apps\MdcHR26Apps.BlazorServer\Components\Pages\DeptObjective\

복사:
- Main.razor + Main.razor.cs
- Sub.razor + Sub.razor.cs
```

#### MainObjective CRUD (4개)
```
25년도: C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages\DeptObjective\MainObjective\
26년도: c:\Codes\00_Develop_Cursor\10_MdcHR26Apps\MdcHR26Apps.BlazorServer\Components\Pages\DeptObjective\MainObjective\

복사:
- Create.razor + Create.razor.cs
- Edit.razor + Edit.razor.cs
- Delete.razor + Delete.razor.cs
- Details.razor + Details.razor.cs
```

#### SubObjective CRUD (4개)
```
25년도: C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages\DeptObjective\SubObjective\
26년도: c:\Codes\00_Develop_Cursor\10_MdcHR26Apps\MdcHR26Apps.BlazorServer\Components\Pages\DeptObjective\SubObjective\

복사:
- Create.razor + Create.razor.cs
- Edit.razor + Edit.razor.cs
- Delete.razor + Delete.razor.cs
- Details.razor + Details.razor.cs
```

### Step 2: 네임스페이스 변경 (전체 파일)

```csharp
// 모든 .razor.cs 파일에서
using MdcHR25Apps → using MdcHR26Apps
namespace MdcHR25Apps.BlazorApp.Pages.DeptObjective → namespace MdcHR26Apps.BlazorServer.Components.Pages.DeptObjective
```

### Step 3: ObjectiveType 필드 설정

#### MainObjective/Create.razor.cs
```csharp
// 25년도 코드 (AddAsync 호출 전에 추가)
model.EDepartId = 1;
model.Remarks = string.Empty;

// 26년도 추가 (ObjectiveType 설정)
model.ObjectiveType = "Main";  // ✅ 추가

model = await deptObjectiveDbReposiory.AddAsync(model);
```

#### SubObjective/Create.razor.cs
```csharp
// 25년도 코드
model.EDepartId = deptId;
model.Remarks = string.Empty;

// 26년도 추가 (ObjectiveType 설정)
model.ObjectiveType = "Sub";  // ✅ 추가

model = await deptObjectiveDbReposiory.AddAsync(model);
```

### Step 4: 감사 필드 설정

#### MainObjective/Create.razor.cs & SubObjective/Create.razor.cs
```csharp
// ObjectiveType 설정 후 추가
model.CreatedBy = loginStatusService.LoginStatus.LoginUid;  // ✅ 추가
model.CreatedAt = DateTime.Now;  // ✅ 추가

model = await deptObjectiveDbReposiory.AddAsync(model);
```

#### MainObjective/Edit.razor.cs & SubObjective/Edit.razor.cs
```csharp
// UpdateAsync 호출 전에 추가
model.UpdatedBy = loginStatusService.LoginStatus.LoginUid;  // ✅ 추가
model.UpdatedAt = DateTime.Now;  // ✅ 추가

if (await deptObjectiveDbReposiory.UpdateAsync(model))  // ✅ bool 반환 (변경 없음)
```

### Step 5: 라우트 경로 확인 (변경 없음)

```razor
// 25년도와 26년도 동일
@page "/DeptObjective/Main"
@page "/DeptObjective/MainObjective/Create"
@page "/DeptObjective/MainObjective/Edit/{Id:long}"  // ✅ Id 사용
@page "/DeptObjective/MainObjective/Delete/{Id:long}"
@page "/DeptObjective/MainObjective/Details/{Id:long}"
```

---

## 5. 변경사항 체크리스트

각 파일마다 확인할 사항:

### 네임스페이스 (전체 파일)
- [ ] `using MdcHR25Apps` → `using MdcHR26Apps`
- [ ] `namespace MdcHR25Apps.BlazorApp` → `namespace MdcHR26Apps.BlazorServer`
- [ ] `Pages.DeptObjective` → `Components.Pages.DeptObjective`

### ObjectiveType 설정 (Create.razor.cs만)
- [ ] MainObjective/Create: `model.ObjectiveType = "Main";` 추가
- [ ] SubObjective/Create: `model.ObjectiveType = "Sub";` 추가

### 감사 필드 설정
- [ ] Create: `CreatedBy`, `CreatedAt` 설정
- [ ] Edit: `UpdatedBy`, `UpdatedAt` 설정

### 변경하지 않는 항목 (확인만)
- [ ] 파라미터 이름: `Id` 유지 (DeptObjectiveDbId로 변경하지 않음!)
- [ ] Repository 반환: `bool` 유지 (int로 변경하지 않음!)
- [ ] Entity PK: `DeptObjectiveDbId` 유지 (변경하지 않음!)

---

## 6. 예시: MainObjective/Create.razor.cs 변경

### 25년도 코드
```csharp
using MdcHR25Apps.BlazorApp.Data;
using MdcHR25Apps.Models.DeptObjective;

namespace MdcHR25Apps.BlazorApp.Pages.DeptObjective.MainObjective
{
    public partial class Create
    {
        // ... 의존성 주입 ...

        private async Task CreateObjective()
        {
            model.EDepartId = 1;
            model.Remarks = string.Empty;

            // ObjectiveType 설정 없음
            // 감사 필드 설정 없음

            model = await deptObjectiveDbReposiory.AddAsync(model);

            if (model.DeptObjectiveDbId != 0)
            {
                resultText = "목표 생성에 성공하셨습니다.";
                urlActions.MoveMainObjectivePage();
            }
        }
    }
}
```

### 26년도 코드 (변경 후)
```csharp
using MdcHR26Apps.BlazorServer.Data;  // ✅ 네임스페이스 변경
using MdcHR26Apps.Models.DeptObjective;  // ✅ 네임스페이스 변경

namespace MdcHR26Apps.BlazorServer.Components.Pages.DeptObjective.MainObjective  // ✅ 네임스페이스 변경
{
    public partial class Create
    {
        // ... 의존성 주입 (동일) ...

        private async Task CreateObjective()
        {
            model.EDepartId = 1;
            model.Remarks = string.Empty;

            // ✅ ObjectiveType 설정 추가
            model.ObjectiveType = "Main";

            // ✅ 감사 필드 설정 추가
            model.CreatedBy = loginStatusService.LoginStatus.LoginUid;
            model.CreatedAt = DateTime.Now;

            model = await deptObjectiveDbReposiory.AddAsync(model);  // ✅ 반환 타입 동일

            if (model.DeptObjectiveDbId != 0)  // ✅ PK 필드명 동일
            {
                resultText = "목표 생성에 성공하셨습니다.";
                urlActions.MoveMainObjectivePage();
            }
        }
    }
}
```

---

## 7. 예시: MainObjective/Edit.razor.cs 변경

### 25년도 코드
```csharp
private async Task EditObjective()
{
    // 감사 필드 설정 없음

    if (await deptObjectiveDbReposiory.UpdateAsync(model))  // bool 반환
    {
        resultText = "목표 수정에 성공하셨습니다.";
        urlActions.MoveMainObjectivePage();
    }
}
```

### 26년도 코드 (변경 후)
```csharp
private async Task EditObjective()
{
    // ✅ 감사 필드 설정 추가
    model.UpdatedBy = loginStatusService.LoginStatus.LoginUid;
    model.UpdatedAt = DateTime.Now;

    if (await deptObjectiveDbReposiory.UpdateAsync(model))  // ✅ bool 반환 (동일)
    {
        resultText = "목표 수정에 성공하셨습니다.";
        urlActions.MoveMainObjectivePage();
    }
}
```

---

## 8. 주의사항

### 8.1. 25년도 코드 그대로 복사
- **Entity PK**: `DeptObjectiveDbId` (변경하지 않음!)
- **파라미터 이름**: `Id` (DeptObjectiveDbId로 변경하지 않음!)
- **라우트**: `/DeptObjective/MainObjective/Edit/{Id:long}` (동일)
- **Repository 반환**: `bool` (int로 변경하지 않음!)

### 8.2. 추가만 하는 항목
- **ObjectiveType**: Create.razor.cs에서만 설정 ("Main" / "Sub")
- **감사 필드**: Create (CreatedBy/At), Edit (UpdatedBy/At)

### 8.3. Main vs Sub 구분
```csharp
// Main (전사목표)
model.EDepartId = 1;  // 메디칼파크 고정
model.ObjectiveType = "Main";  // ✅ 추가

// Sub (부서목표)
model.EDepartId = deptId;  // 로그인 사용자 부서
model.ObjectiveType = "Sub";  // ✅ 추가
```

---

## 9. 테스트 시나리오

### 빌드 테스트
```bash
dotnet build
```
**예상 결과**: 0 errors

### 기능 테스트 (수동)
1. **Main 목록**: 전사목표 조회 (관리자만)
2. **Sub 목록**: 부서목표 조회 (팀장)
3. **MainObjective/Create**: ObjectiveType = "Main", CreatedBy/At 확인
4. **SubObjective/Create**: ObjectiveType = "Sub", CreatedBy/At 확인
5. **Edit**: UpdatedBy/At 확인
6. **Delete**: 정상 삭제 확인

---

## 10. 완료 조건

### 코드 작성
- [ ] 25년도 DeptObjective 목록 2개 복사
- [ ] 25년도 MainObjective CRUD 4개 복사
- [ ] 25년도 SubObjective CRUD 4개 복사
- [ ] 네임스페이스 변경 (MdcHR25 → MdcHR26, BlazorApp → BlazorServer)
- [ ] ObjectiveType 설정 (Create만: "Main" / "Sub")
- [ ] 감사 필드 설정 (Create: CreatedBy/At, Edit: UpdatedBy/At)

### 검증
- [ ] 빌드 성공 (0 errors)
- [ ] 25년도 기능과 동일한지 확인
- [ ] ObjectiveType 정상 설정 확인
- [ ] 감사 필드 정상 설정 확인

---

## 11. 참조 문서

**이슈**:
- [#015: Agreement TeamLeader 페이지 - 임의 코드 작성으로 인한 디버깅 어려움](../issues/015_agreement_teamleader_arbitrary_code_generation.md)
- [#016: Phase 3-4 DB 변경사항 미반영 및 2025년 차이점 발견](../issues/016_phase3_4_db_sync_and_2025_differences.md)

**25년도 코드**:
- `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages\DeptObjective\`

**26년도 Entity/Repository**:
- `MdcHR26Apps.Models\DeptObjective\DeptObjectiveDb.cs`
- `MdcHR26Apps.Models\DeptObjective\IDeptObjectiveRepository.cs`

---

**작성자**: Claude Sonnet 4.5
**작업 방식**: 25년도 코드 복사 → 26년도 DB 변경사항 반영
**참조 이슈**: #015, #016 - 25년도 코드 분석 후 정확한 변경사항만 반영
**이전 작업지시서**: 20260204_05 (폐기 - DB 변경사항 분석 오류)
