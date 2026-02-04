# 작업지시서: 25년 기준 Entity 및 Repository 수정

**날짜**: 2026-02-03
**작업 타입**: Entity/Repository 재작성 (25년 기준)
**예상 소요**: 3시간
**관련 이슈**: [#009](../issues/009_phase3_webapp_development.md)

---

## 1. 작업 개요

### 배경
- DB 테이블과 Entity 간 불일치 발견 (5개)
- 26년 프로젝트가 임의로 작성되어 25년 구조와 다름
- **25년 프로젝트를 기준**으로 Entity 및 Repository 수정 필요

### 원칙
1. **DB 테이블**: 25년과 26년 동일 (변경 없음)
2. **Entity**: DB 테이블 구조에 맞춤 = 25년 Entity 사용
3. **Repository**: 25년 Repository 기준으로 수정

---

## 2. Entity 수정 (25년 기준)

### 2.1. AgreementDb.cs

**25년 프로젝트**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.Models\Agreement\AgreementDb.cs`

**25년 Entity (정답)**:
```csharp
public class AgreementDb
{
    public long Aid { get; set; }
    public string UserId { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public int Report_Item_Number { get; set; }
    public string Report_Item_Name_1 { get; set; } = null!;
    public string Report_Item_Name_2 { get; set; } = null!;
    public int Report_Item_Proportion { get; set; }
}
```

**26년 수정**: 25년 Entity 그대로 가져오되, 네임스페이스만 변경
- `namespace MdcHR25Apps.Models.Agreement` → `namespace MdcHR26Apps.Models.EvaluationAgreement`
- Attribute 추가: `[Table("AgreementDb")]`, `[Key]`, `[Required]` 등

---

### 2.2. SubAgreementDb.cs

**25년 프로젝트**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.Models\SubAgreement\SubAgreementDb.cs`

**25년 Entity (정답)**:
```csharp
public class SubAgreementDb
{
    public long Sid { get; set; }
    public string UserId { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public int Report_Item_Number { get; set; }
    public string Report_Item_Name_1 { get; set; } = null!;
    public string Report_Item_Name_2 { get; set; } = null!;
    public int Report_Item_Proportion { get; set; }
    public string Report_SubItem_Name { get; set; } = null!;
    public int Report_SubItem_Proportion { get; set; }
    public Int64 Task_Number { get; set; }
}
```

**26년 수정**: 25년 Entity 그대로 가져오되, 네임스페이스만 변경

---

### 2.3. DeptObjectiveDb.cs, EvaluationLists.cs, TasksDb.cs

25년 프로젝트에서 확인 후 동일하게 수정

---

## 3. Repository 수정 (25년 기준)

### 3.1. AgreementRepository.cs

**25년 프로젝트**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.Models\Agreement\AgreementDbRepositoryDapperAsync.cs`

**25년 Repository 메서드 (7개)**:
1. `AddAsync(AgreementDb model)` - 추가
2. `GetByAllAsync()` - 전체 조회
3. `GetByIdAsync(long id)` - ID 조회
4. `UpdateAsync(AgreementDb model)` - 수정
5. `DeleteAsync(long id)` - 삭제
6. **`GetByUserIdAllAsync(string UserId)`** - 사용자별 조회 (26년에 없음)
7. **`GetByTasksPeroportionAsync(string UserId, string DeptName, string IndexName)`** - 부서명/지표명 조회 (26년에 없음)

**26년에서 제거할 메서드** (25년에 없음):
- `GetCountByUidAsync` → 제거
- `DeleteAllByUidAsync` → 제거
- `IsAgreementCompleteAsync` → 제거
- `GetPendingAgreementAsync` → 제거
- `GetByDeptObjectiveAsync` → 제거

**작업 방법**:
1. 25년 `AgreementDbRepositoryDapperAsync.cs` 전체 복사
2. 네임스페이스 변경: `MdcHR25Apps` → `MdcHR26Apps`
3. 클래스명 변경: `AgreementDbRepositoryDapperAsync` → `AgreementRepository`
4. Primary Constructor 스타일 적용 (C# 13)
5. Interface 변경: `IAgreementDbRepository` → `IAgreementRepository`

---

### 3.2. SubAgreementRepository.cs

**25년 프로젝트**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.Models\SubAgreement\SubAgreementDbRepositoryDapperAsync.cs`

동일한 방식으로 25년 Repository를 기준으로 수정

---

### 3.3. 나머지 Repository

25년 프로젝트에서 확인 후 동일하게 수정

---

## 4. Interface 수정

각 Repository의 Interface도 25년 기준으로 수정:
- `IAgreementRepository.cs`
- `ISubAgreementRepository.cs`
- `IDeptObjectiveRepository.cs`
- `IEvaluationListsRepository.cs`
- `ITasksRepository.cs`

메서드 시그니처를 25년 Interface와 일치시킴

---

## 5. 영향 확인

### 5.1. ReportInit.razor.cs 확인

**현재 사용 중인 메서드**:
- `agreementRepository.GetCountByUidAsync` (Line 62) → **제거됨**
- `agreementRepository.DeleteAllByUidAsync` (Line 105) → **제거됨**
- `subAgreementRepository.GetCountByUidAsync` (Line 61) → **제거됨**
- `subAgreementRepository.DeleteAllByUidAsync` (Line 102) → **제거됨**

**해결 방법**:
- `GetCountByUidAsync` → `GetByUserIdAllAsync().Count` 사용
- `DeleteAllByUidAsync` → 반복문으로 각각 `DeleteAsync` 호출

---

## 6. 작업 순서

1. 25년 Entity 5개를 26년으로 복사 (네임스페이스만 변경)
2. 25년 Repository 5개를 26년으로 복사 (네임스페이스, 클래스명, 스타일 변경)
3. Interface 5개 수정
4. ReportInit.razor.cs 수정 (제거된 메서드 대응)
5. 빌드 테스트
6. 기능 테스트

---

## 7. 완료 조건

- [ ] 5개 Entity 파일 수정 완료 (25년 기준)
  - [ ] AgreementDb.cs
  - [ ] SubAgreementDb.cs
  - [ ] DeptObjectiveDb.cs
  - [ ] EvaluationLists.cs
  - [ ] TasksDb.cs
- [ ] 5개 Repository 파일 수정 완료 (25년 기준)
  - [ ] AgreementRepository.cs
  - [ ] SubAgreementRepository.cs
  - [ ] DeptObjectiveRepository.cs
  - [ ] EvaluationListsRepository.cs
  - [ ] TasksRepository.cs
- [ ] 5개 Interface 파일 수정 완료
- [ ] ReportInit.razor.cs 수정 완료
- [ ] 빌드 성공 (MdcHR26Apps.Models, MdcHR26Apps.BlazorServer)
- [ ] 기능 테스트 완료
- [ ] 관련 이슈 업데이트

---

## 8. 관련 문서

**25년 프로젝트 (기준)**:
- `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.Models\Agreement\`
- `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.Models\SubAgreement\`
- `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.Models\DeptObjective\`
- `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.Models\Tasks\`
- `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.Models\Evaluation\`

**26년 프로젝트 (수정 대상)**:
- `MdcHR26Apps.Models/EvaluationAgreement/`
- `MdcHR26Apps.Models/EvaluationSubAgreement/`
- `MdcHR26Apps.Models/DeptObjective/`
- `MdcHR26Apps.Models/EvaluationTasks/`
- `MdcHR26Apps.Models/EvaluationLists/`

**영향 받는 페이지**:
- `MdcHR26Apps.BlazorServer/Components/Pages/Admin/TotalReport/ReportInit.razor.cs`
