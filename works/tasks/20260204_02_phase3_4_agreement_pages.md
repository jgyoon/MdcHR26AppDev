# 작업지시서: Phase 3-4 Agreement 페이지 구현

**작업 번호**: 20260204_02
**작성일**: 2026-02-04
**난이도**: 중간 (7개 페이지)
**예상 소요**: 2-3시간

---

## 1. 작업 개요

Phase 3-4 평가 프로세스 중 Agreement (직무평가 협의) 페이지를 구현합니다.
- User 페이지 (5개): Index, Create, Edit, Delete, Details
- TeamLeader 페이지 (2개): Index, Details

**총 7개 페이지 (14개 파일)**

---

## 2. 실제 Repository 메서드 (확인 완료)

### AgreementRepository
```csharp
// 공통 CRUD
Task<AgreementDb> AddAsync(AgreementDb model)                    // 생성
Task<List<AgreementDb>> GetByAllAsync()                          // 전체 조회
Task<AgreementDb> GetByIdAsync(long id)                          // ID별 조회
Task<bool> UpdateAsync(AgreementDb model)                        // 수정
Task<bool> DeleteAsync(long id)                                  // 삭제

// 추가 메서드
Task<List<AgreementDb>> GetByUserIdAllAsync(long userId)
Task<List<AgreementDb>> GetByTasksPeroportionAsync(long userId, string deptName, string indexName)
```

---

## 3. 페이지 구현 원칙

### 기본 구조 (Admin/Users/Details.razor 참조)
```csharp
// .razor.cs
[Parameter] public long Id { get; set; }
[Inject] public UrlActions urlActions { get; set; } = null!;
[Inject] public LoginStatusService loginStatusService { get; set; } = null!;
[Inject] public IAgreementRepository agreementRepository { get; set; } = null!;

protected override async Task OnInitializedAsync()
{
    await CheckLogined();
    await LoadData();
}
```

### Repository 호출 패턴
```csharp
// 조회
var data = await agreementRepository.GetByIdAsync(id);
var list = await agreementRepository.GetByUserIdAllAsync(loginUser.LoginUid);

// 생성
var result = await agreementRepository.AddAsync(model);

// 수정
var success = await agreementRepository.UpdateAsync(model);

// 삭제
var success = await agreementRepository.DeleteAsync(id);
```

### 권한 체크 (LoginStatus 사용)
```csharp
var loginUser = await loginStatusService.GetLoginStatusAsync();

// 본인 확인
if (model.Uid != loginUser.LoginUid) { /* 권한 없음 */ }

// 팀장 확인
if (!loginUser.LoginIsTeamLeader) { /* 권한 없음 */ }
```

---

## 4. 구현할 페이지 목록

### Agreement/User (5개 페이지, 10 files)
```
Components/Pages/Agreement/User/
├── Index.razor/.cs        - GetByUserIdAllAsync()
├── Create.razor/.cs       - AddAsync()
├── Edit.razor/.cs         - GetByIdAsync(), UpdateAsync()
├── Delete.razor/.cs       - GetByIdAsync(), DeleteAsync()
└── Details.razor/.cs      - GetByIdAsync()
```

### Agreement/TeamLeader (2개 페이지, 4 files)
```
Components/Pages/Agreement/TeamLeader/
├── Index.razor/.cs        - GetByAllAsync() + LINQ 필터
└── Details.razor/.cs      - GetByIdAsync(), UpdateAsync()
```

---

## 5. 작업 단계

### Step 1: Agreement/User/Index.razor + .cs
- **라우트**: `/Agreement/User/Index`
- **Repository**: `GetByUserIdAllAsync(loginUser.LoginUid)`
- **컴포넌트**: `AgreementListTable`
- **권한**: 로그인 사용자
- **기능**: 본인의 Agreement 목록 조회

### Step 2: Agreement/User/Create.razor + .cs
- **라우트**: `/Agreement/User/Create`
- **Repository**: `AddAsync(model)`
- **컴포넌트**: `FormAgreeTask`
- **권한**: 로그인 사용자
- **기능**: 새 Agreement 생성

### Step 3: Agreement/User/Edit.razor + .cs
- **라우트**: `/Agreement/User/Edit/{Id:long}`
- **Repository**: `GetByIdAsync(id)`, `UpdateAsync(model)`
- **컴포넌트**: `FormAgreeTask`
- **권한**: 본인 데이터만 수정 가능
- **기능**: Agreement 수정

### Step 4: Agreement/User/Delete.razor + .cs
- **라우트**: `/Agreement/User/Delete/{Id:long}`
- **Repository**: `GetByIdAsync(id)`, `DeleteAsync(id)`
- **컴포넌트**: `AgreementDetailsTable`, `AgreementDeleteModal`
- **권한**: 본인 데이터만 삭제 가능
- **기능**: Agreement 삭제 확인 및 실행

### Step 5: Agreement/User/Details.razor + .cs
- **라우트**: `/Agreement/User/Details/{Id:long}`
- **Repository**: `GetByIdAsync(id)`
- **컴포넌트**: `AgreementDetailsTable`
- **권한**: 로그인 사용자
- **기능**: Agreement 상세 조회

### Step 6: Agreement/TeamLeader/Index.razor + .cs
- **라우트**: `/Agreement/TeamLeader/Index`
- **Repository**: `GetByAllAsync()` + LINQ 필터 (부서별)
- **컴포넌트**: `TeamLeaderAgreementListTable`
- **권한**: 팀장 (`LoginIsTeamLeader`)
- **기능**: 부서원의 Agreement 목록 조회

### Step 7: Agreement/TeamLeader/Details.razor + .cs
- **라우트**: `/Agreement/TeamLeader/Details/{Id:long}`
- **Repository**: `GetByIdAsync(id)`, `UpdateAsync(model)`
- **컴포넌트**: `TeamLeaderAgreementDetailsTable`
- **권한**: 팀장 (`LoginIsTeamLeader`)
- **기능**: Agreement 상세 조회 및 승인/반려

### Step 8: 빌드 테스트
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
- `Aid` (AgreementDb)

### LoginStatus 속성
- `LoginUid` (long)
- `LoginIsTeamLeader` (bool)
- `LoginIsAdministrator` (bool)

### Navigation Property
- `AgreementDb.User` (UserDb) - 사용자 정보

---

## 7. 참조 파일

### 기존 구현 (구조 참조용)
- `Components/Pages/Admin/Users/Details.razor/.cs`

### Repository (실제 메서드)
- `MdcHR26Apps.Models/EvaluationAgreement/IAgreementRepository.cs`
- `MdcHR26Apps.Models/EvaluationAgreement/AgreementRepository.cs`

### 컴포넌트 (이미 구현됨)
- `Components/Pages/Components/Agreement/AgreementListTable.razor`
- `Components/Pages/Components/Agreement/AgreementDetailsTable.razor`
- `Components/Pages/Components/Agreement/TeamLeaderAgreementListTable.razor`
- `Components/Pages/Components/Agreement/TeamLeaderAgreementDetailsTable.razor`
- `Components/Pages/Components/Agreement/AgreementDeleteModal.razor`
- `Components/Pages/Components/Form/FormAgreeTask.razor`

---

## 8. 완료 조건

- [ ] Agreement/User/Index.razor/.cs (GetByUserIdAllAsync)
- [ ] Agreement/User/Create.razor/.cs (AddAsync)
- [ ] Agreement/User/Edit.razor/.cs (GetByIdAsync, UpdateAsync)
- [ ] Agreement/User/Delete.razor/.cs (GetByIdAsync, DeleteAsync)
- [ ] Agreement/User/Details.razor/.cs (GetByIdAsync)
- [ ] Agreement/TeamLeader/Index.razor/.cs (GetByAllAsync + LINQ)
- [ ] Agreement/TeamLeader/Details.razor/.cs (GetByIdAsync, UpdateAsync)
- [ ] 빌드 성공 (경고 0개, 오류 0개)
- [ ] Repository 메서드 정확 사용
- [ ] LoginStatus 속성 정확 사용
- [ ] 권한 체크 구현

---

**총 파일**: 14개 (7 .razor + 7 .cs)
**빌드**: `dotnet build` 성공

---

**작성자**: Claude Sonnet 4.5
**참조**: 2026년 프로젝트 실제 구현
**선행 작업**: Agreement 컴포넌트 6개 구현 완료
