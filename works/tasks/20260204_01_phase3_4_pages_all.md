# 작업지시서: Phase 3-4 전체 페이지 구현

**작업 번호**: 20260204_01
**작성일**: 2026-02-04
**난이도**: 복잡 (40개 페이지)
**예상 소요**: 8-12시간

---

## 1. 작업 개요

Phase 3-4 평가 프로세스 페이지 전체를 구현합니다.
- Agreement (7개)
- SubAgreement (10개)
- 1st/2nd/3rd HR_Report (13개)
- DeptObjective (10개)

**총 40개 페이지 (80개 파일)**

---

## 2. 실제 Repository 메서드 (확인 완료)

### 공통 CRUD (모든 Repository)
```csharp
Task<T> AddAsync(T model)                    // 생성
Task<List<T>> GetByAllAsync()                // 전체 조회
Task<T> GetByIdAsync(long id)                // ID별 조회
Task<bool> UpdateAsync(T model)              // 수정
Task<bool> DeleteAsync(long id)              // 삭제
```

### 추가 메서드
```csharp
// Agreement, SubAgreement
Task<List<T>> GetByUserIdAllAsync(long userId)
Task<List<T>> GetByTasksPeroportionAsync(long userId, string deptName, string indexName)

// Tasks
Task<List<TasksDb>> GetByListNoAllAsync(long taksListNumber)
Task<bool> DeleteByListNoAllAsync(long taksListNumber)
Task<int> GetCountByUserAsync(long uid)
Task<bool> DeleteAllByUserAsync(long uid)
```

---

## 3. 페이지 구현 원칙

### 기본 구조 (Admin/Users/Details.razor 참조)
```csharp
// .razor.cs
[Parameter] public long Id { get; set; }
[Inject] public UrlActions urlActions { get; set; } = null!;
[Inject] public LoginStatusService loginStatusService { get; set; } = null!;
[Inject] public IRepository repository { get; set; } = null!;

protected override async Task OnInitializedAsync()
{
    await CheckLogined();
    await LoadData();
}
```

### Repository 호출 패턴
```csharp
// 조회
var data = await repository.GetByIdAsync(id);
var list = await repository.GetByUserIdAllAsync(loginUser.LoginUid);

// 생성
var result = await repository.AddAsync(model);

// 수정
var success = await repository.UpdateAsync(model);

// 삭제
var success = await repository.DeleteAsync(id);
```

### 권한 체크 (LoginStatus 사용)
```csharp
var loginUser = await loginStatusService.GetLoginStatusAsync();

// 본인 확인
if (model.Uid != loginUser.LoginUid) { /* 권한 없음 */ }

// 팀장 확인
if (!loginUser.LoginIsTeamLeader) { /* 권한 없음 */ }

// 임원 확인
if (!loginUser.LoginIsDirector) { /* 권한 없음 */ }

// 관리자 확인
if (!loginUser.LoginIsAdministrator) { /* 권한 없음 */ }
```

---

## 4. 구현할 페이지 목록

### Agreement (7개 페이지, 14 files)
```
Components/Pages/Agreement/
├── User/
│   ├── Index.razor/.cs        - GetByUserIdAllAsync()
│   ├── Create.razor/.cs       - AddAsync()
│   ├── Edit.razor/.cs         - GetByIdAsync(), UpdateAsync()
│   ├── Delete.razor/.cs       - GetByIdAsync(), DeleteAsync()
│   └── Details.razor/.cs      - GetByIdAsync()
└── TeamLeader/
    ├── Index.razor/.cs        - GetByAllAsync() + LINQ 필터
    └── Details.razor/.cs      - GetByIdAsync(), UpdateAsync()
```

### SubAgreement (10개 페이지, 20 files)
```
Components/Pages/SubAgreement/
├── User/
│   ├── Index.razor/.cs        - GetByUserIdAllAsync()
│   ├── Create.razor/.cs       - AddAsync()
│   ├── Edit.razor/.cs         - GetByIdAsync(), UpdateAsync()
│   ├── Delete.razor/.cs       - GetByIdAsync(), DeleteAsync()
│   └── Details.razor/.cs      - GetByIdAsync()
└── TeamLeader/
    ├── Index.razor/.cs        - GetByAllAsync() + LINQ 필터
    ├── Details.razor/.cs      - GetByIdAsync()
    ├── SubDetails.razor/.cs   - GetByAllAsync() + LINQ 필터
    ├── CompleteSubAgreement.razor/.cs   - GetByUserIdAllAsync(), UpdateAsync()
    └── ResetSubAgreement.razor/.cs      - GetByUserIdAllAsync(), DeleteAsync()
```

### 1st_HR_Report (3개 페이지, 6 files)
```
Components/Pages/1st_HR_Report/
├── Index.razor/.cs            - TasksRepository.GetByListNoAllAsync()
├── Edit.razor/.cs             - GetByIdAsync(), UpdateAsync()
└── Details.razor/.cs          - GetByIdAsync()
```

### 2nd_HR_Report (5개 페이지, 10 files)
```
Components/Pages/2nd_HR_Report/
├── Index.razor/.cs            - GetByAllAsync() + LINQ 필터
├── Edit.razor/.cs             - GetByIdAsync(), UpdateAsync()
├── Details.razor/.cs          - GetByIdAsync()
├── Complete_2nd_Edit.razor/.cs      - GetByIdAsync(), UpdateAsync()
└── Complete_2nd_Details.razor/.cs   - GetByIdAsync()
```

### 3rd_HR_Report (5개 페이지, 10 files)
```
Components/Pages/3rd_HR_Report/
├── Index.razor/.cs            - GetByAllAsync()
├── Edit.razor/.cs             - GetByIdAsync(), UpdateAsync()
├── Details.razor/.cs          - GetByIdAsync()
├── Complete_3rd_Edit.razor/.cs      - GetByIdAsync(), UpdateAsync()
└── Complete_3rd_Details.razor/.cs   - GetByIdAsync()
```

### DeptObjective (10개 페이지, 20 files)
```
Components/Pages/DeptObjective/
├── Main.razor/.cs             - GetByAllAsync() + LINQ 필터
├── Sub.razor/.cs              - GetByAllAsync() + LINQ 필터
├── MainObjective/
│   ├── Create.razor/.cs       - AddAsync()
│   ├── Edit.razor/.cs         - GetByIdAsync(), UpdateAsync()
│   ├── Delete.razor/.cs       - GetByIdAsync(), DeleteAsync()
│   └── Details.razor/.cs      - GetByIdAsync()
└── SubObjective/
    ├── Create.razor/.cs       - AddAsync()
    ├── Edit.razor/.cs         - GetByIdAsync(), UpdateAsync()
    ├── Delete.razor/.cs       - GetByIdAsync(), DeleteAsync()
    └── Details.razor/.cs      - GetByIdAsync()
```

---

## 5. 작업 단계

### Step 1: Agreement 페이지 (7개)
- Admin/Users/Details.razor 구조 참조
- GetByUserIdAllAsync(), GetByIdAsync() 사용
- 컴포넌트: AgreementListTable, AgreementDetailsTable, FormAgreeTask

### Step 2: SubAgreement 페이지 (10개)
- Agreement와 동일한 구조
- Tasks 연동 추가

### Step 3: 1st/2nd/3rd HR_Report 페이지 (13개)
- TasksRepository 사용
- GetByListNoAllAsync(), GetByAllAsync() 사용
- 평가 점수 필드 업데이트

### Step 4: DeptObjective 페이지 (10개)
- Main/Sub 구분 필터링
- 감사 필드 자동 설정 (CreatedBy/At, UpdatedBy/At)

### Step 5: 빌드 및 검증
```bash
dotnet build
```

---

## 6. 주의사항

### Repository 메서드 이름 (정확히)
- ✅ `AddAsync` (InsertAsync 아님!)
- ✅ `GetByAllAsync` (GetAllAsync 아님!)
- ✅ `GetByIdAsync` (SelectAsync 아님!)
- ✅ `UpdateAsync`
- ✅ `DeleteAsync`

### Entity PK 필드명
- Aid (AgreementDb)
- Sid (SubAgreementDb, 2025: SAid)
- Tid (TasksDb)
- DeptObjectiveDbId (DeptObjectiveDb, 2025: DOid)

### LoginStatus 속성
- `LoginUid` (long)
- `LoginIsTeamLeader` (bool)
- `LoginIsDirector` (bool)
- `LoginIsAdministrator` (bool)

### Navigation Property 확인 필요
- AgreementDb.User (UserDb)
- 다른 Entity는 실제 구현 확인 후 사용

---

## 7. 참조 파일

### 기존 구현 (구조 참조용)
- `Components/Pages/Admin/Users/Details.razor/.cs`

### Repository (실제 메서드)
- `MdcHR26Apps.Models/EvaluationAgreement/AgreementRepository.cs`
- `MdcHR26Apps.Models/EvaluationSubAgreement/SubAgreementRepository.cs`
- `MdcHR26Apps.Models/EvaluationTasks/TasksRepository.cs`
- `MdcHR26Apps.Models/DeptObjective/DeptObjectiveRepository.cs`

### 컴포넌트 (이미 구현됨)
- `Components/Pages/Components/Agreement/` (6개)
- `Components/Pages/Components/SubAgreement/` (8개)
- `Components/Pages/Components/Report/` (15개)
- `Components/Pages/Components/Form/` (6개)
- `Components/Pages/Components/Common/` (3개)

---

## 8. 완료 조건

- [ ] Agreement 7개 페이지 (14 files)
- [ ] SubAgreement 10개 페이지 (20 files)
- [ ] 1st/2nd/3rd HR_Report 13개 페이지 (26 files)
- [ ] DeptObjective 10개 페이지 (20 files)
- [ ] 빌드 성공 (경고 0개, 오류 0개)
- [ ] Repository 메서드 정확 사용
- [ ] LoginStatus 속성 정확 사용
- [ ] 권한 체크 구현

---

**총 파일**: 80개 (40 .razor + 40 .cs)
**빌드**: `dotnet build` 성공

---

**작성자**: Claude Sonnet 4.5
**참조**: 2026년 프로젝트 실제 구현
**선행 작업**: 컴포넌트 38개 구현 완료
