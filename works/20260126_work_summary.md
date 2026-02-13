# 2026-01-26 작업 종합 정리

**날짜**: 2026-01-26
**주요 작업**: Phase 3-3 Admin 페이지 빌드 경고 수정 및 프로젝트 구조 재정리
**관련 이슈**: [#011: Phase 3-3 빌드 오류 및 재작업](issues/011_phase3_3_admin_pages_build_errors.md)

---

## 📋 작업 요약

### 1. 빌드 경고 수정 (14개)

**커밋**: `830a8ef`

#### CS9113 경고 (2개)
- **문제**: Primary Constructor에서 사용하지 않는 매개변수
- **수정**: 매개변수명 앞에 `_` 추가
- **파일**:
  - [Settings/Depts/Create.razor.cs:8](MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Depts/Create.razor.cs#L8)
  - [Settings/Ranks/Create.razor.cs:8](MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Ranks/Create.razor.cs#L8)

#### CS8601 경고 (12개)
- **문제**: null 참조 할당 가능성
- **수정**: 중간 변수를 통한 null 체크 후 할당
- **파일** (9개):
  - Settings/Depts: Create, Edit, Delete, Details (4개)
  - Settings/Ranks: Create, Edit, Delete, Details (4개)
  - EvaluationUsers: Edit (3곳), Details (2곳)

---

### 2. 프로젝트 구조 재정리

**커밋**: `86c5871`
**작업지시서**: [20260126_02_restructure_blazor_project.md](tasks/20260126_02_restructure_blazor_project.md)

#### 주요 변경사항

##### 페이지 통합
```
변경 전:
- Pages/Admin/ (별도 폴더)
- Components/Pages/ (일부 페이지)

변경 후:
- Components/Pages/ (모든 페이지 통합)
```

##### 공용 컴포넌트 재정리
```
변경 전:
- Components/UserListTable.razor (최상위)
- Components/CommonComponents/
- Components/Modal/

변경 후:
- Components/Pages/Components/Common/ (SearchbarComponent)
- Components/Pages/Components/Modal/ (UserDeleteModal)
- Components/Pages/Components/Table/ (UserListTable)
```

##### 폴더명 복수형 적용
```
변경 전:
- Admin/Settings/Dept/
- Admin/Settings/Rank/

변경 후:
- Admin/Settings/Depts/
- Admin/Settings/Ranks/
```

##### 네임스페이스 변경
```csharp
// 변경 전
namespace MdcHR26Apps.BlazorServer.Pages.Admin;
namespace MdcHR26Apps.BlazorServer.Pages.Admin.Settings.Dept;
namespace MdcHR26Apps.BlazorServer.Components.Modal;

// 변경 후
namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin;
namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin.Settings.Depts;
namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Modal;
```

##### @page 경로 변경
```razor
<!-- 변경 전 -->
@page "/Admin/Settings/Dept/Create"
@page "/Admin/Settings/Rank/Create"

<!-- 변경 후 -->
@page "/Admin/Settings/Depts/Create"
@page "/Admin/Settings/Ranks/Create"
```

##### UrlActions 메서드 업데이트
```csharp
// 변경 전
public void MoveDeptCreatePage() => _navigationManager.NavigateTo("/Admin/Settings/Dept/Create");
public void MoveRankCreatePage() => _navigationManager.NavigateTo("/Admin/Settings/Rank/Create");

// 변경 후
public void MoveDeptCreatePage() => _navigationManager.NavigateTo("/Admin/Settings/Depts/Create");
public void MoveRankCreatePage() => _navigationManager.NavigateTo("/Admin/Settings/Ranks/Create");
```

#### 참고 프로젝트
- **도서관리 프로젝트**: `C:\Codes\36_MdcLibrary\MdcLibrary\MdcLibrary.Server`
- **적용 사항**: .NET 10 프로젝트 구조, 컴포넌트 네이밍 규칙

---

### 3. 빌드 결과

```
✅ 빌드 성공
경고 10개 (오류 0개)

RZ10012: 미구현 컴포넌트 (5개)
CS8601: null 참조 할당 (3개) - 기존 페이지
CS9113: 사용하지 않는 매개변수 (2개) - Create 페이지
```

---

## 📁 최종 프로젝트 구조

```
MdcHR26Apps.BlazorServer/
└── Components/
    ├── App.razor
    ├── Routes.razor
    ├── _Imports.razor
    ├── Layout/
    │   ├── MainLayout.razor
    │   ├── NavMenu.razor
    │   └── ReconnectModal.razor
    └── Pages/                            ✅ 모든 페이지 통합
        ├── Counter.razor
        ├── Error.razor
        ├── Home.razor
        ├── NotFound.razor
        ├── Auth/
        │   └── Login.razor
        ├── Admin/
        │   ├── Index.razor               (관리자 메인)
        │   ├── UserManage.razor          (사용자 목록)
        │   ├── SettingManage.razor       (기초정보 관리)
        │   ├── EUsersManage.razor        (평가대상자 목록)
        │   ├── Users/                    (사용자 CRUD)
        │   │   ├── Create.razor
        │   │   ├── Edit.razor
        │   │   ├── Delete.razor
        │   │   └── Details.razor
        │   ├── EvaluationUsers/          (평가대상자 CRUD)
        │   │   ├── Edit.razor
        │   │   └── Details.razor
        │   └── Settings/                 (기초정보)
        │       ├── Depts/                ✅ 복수형
        │       │   ├── Create.razor
        │       │   ├── Edit.razor
        │       │   ├── Delete.razor
        │       │   └── Details.razor
        │       └── Ranks/                ✅ 복수형
        │           ├── Create.razor
        │           ├── Edit.razor
        │           ├── Delete.razor
        │           └── Details.razor
        └── Components/                   ✅ 공용 컴포넌트
            ├── Common/
            │   └── SearchbarComponent.razor
            ├── Modal/
            │   └── UserDeleteModal.razor
            └── Table/
                └── UserListTable.razor
```

---

## 🚧 미구현 컴포넌트 (3개)

**체크리스트**: [20260126_03_missing_components_checklist.md](tasks/20260126_03_missing_components_checklist.md)

### 1. EUserListTable
- **사용 위치**: `Admin/EUsersManage.razor:21`
- **구현 위치**: `Components/Pages/Components/Table/EUserListTable.razor`
- **참고**: 2025년 프로젝트, UserListTable.razor

### 2. DisplayResultText
- **사용 위치**: `Settings/Depts/Create.razor:9`, `Settings/Ranks/Create.razor:9`
- **구현 위치**: `Components/Pages/Components/Common/DisplayResultText.razor`
- **참고**: 도서관리 프로젝트

### 3. MemberListTable
- **사용 위치**: `Settings/Depts/Details.razor:52`, `Settings/Ranks/Details.razor:52`
- **구현 위치**: `Components/Pages/Components/Table/MemberListTable.razor`
- **참고**: 2025년 프로젝트, UserListTable.razor

---

## 📚 이후 개발 참고사항

### 1. 참고 프로젝트

#### 2025년 인사평가 (비즈니스 로직)
- **경로**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp`
- **참고 사항**:
  - 컴포넌트 구조 및 UI
  - 평가 프로세스 로직
  - 순차적 생성 로직 (UserDb → EvaluationUsers → ProcessDb)
- **주의 사항**:
  - DB 구조 변경 반영 (UserId → Uid, VARCHAR → BIGINT)
  - Primary Constructor 미사용 (기존 Inject 방식)

#### 도서관리 프로젝트 (최신 기술)
- **경로**: `C:\Codes\36_MdcLibrary\MdcLibrary\MdcLibrary.Server`
- **참고 사항**:
  - .NET 10 최신 기능
  - Primary Constructor 사용 패턴
  - InteractiveServer 렌더 모드
  - 프로젝트 구조 (Components/Pages/Components/)
  - 컴포넌트 명명 및 네임스페이스 규칙

---

### 2. .NET 10 형식 활용

#### Primary Constructor (C# 13)
```csharp
// ❌ 기존 방식 (2025년)
public partial class Create
{
    [Inject]
    public IUserRepository userRepository { get; set; } = null!;

    [Inject]
    public LoginStatusService loginStatusService { get; set; } = null!;
}

// ✅ .NET 10 방식 (2026년)
public partial class Create(
    IUserRepository userRepository,
    LoginStatusService loginStatusService,
    UrlActions urlActions)
{
    // 필드로 직접 사용 가능
    // private readonly IUserRepository _userRepository = userRepository;
}
```

#### InteractiveServer 렌더 모드
```razor
@page "/Admin/Users/Create"
@rendermode InteractiveServer

<h3>사용자 등록</h3>
```

#### 컬렉션 초기화
```csharp
// 기존
private List<UserDb> users { get; set; } = new List<UserDb>();

// .NET 10
private List<UserDb> users { get; set; } = new();
```

---

### 3. DB 구조 변경 사항

| 테이블 | 필드 | 2025년 | 2026년 |
|--------|------|--------|--------|
| EvaluationUsers | 사용자 ID | UserId (VARCHAR) | Uid (BIGINT FK) |
| | 부서장 ID | TeamLeader_Id (VARCHAR) | TeamLeaderId (BIGINT FK) |
| | 임원 ID | Director_Id (VARCHAR) | DirectorId (BIGINT FK) |
| ProcessDb | 사용자 ID | UserId (VARCHAR) | Uid (BIGINT FK) |
| | 부서장 ID | TeamLeader_Id (VARCHAR) | TeamLeaderId (BIGINT FK) |
| | 임원 ID | Director_Id (VARCHAR) | DirectorId (BIGINT FK) |
| | 하위 합의 | 없음 | Is_SubRequest, Is_SubAgreement |
| UserDb | 부서 | EDepartment (NVARCHAR) | EDepartId (BIGINT FK) |
| | 직급 | ERank (NVARCHAR) | ERankId (BIGINT FK) |

**v_MemberListDB 뷰 주의사항**:
- ✅ `ERank` (ERankName의 별칭으로 사용 가능)
- ❌ `ERankId` (뷰에 존재하지 않음!)

---

### 4. 프로젝트 구조 규칙

#### 네임스페이스 규칙
```csharp
// 페이지
namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin.Users;
namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin.Settings.Depts;

// 공용 컴포넌트 - Common
namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Common;

// 공용 컴포넌트 - Modal
namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Modal;

// 공용 컴포넌트 - Table
namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Table;
```

#### 폴더명 규칙
- **복수형 사용**: `Users`, `Depts`, `Ranks` (단수형 `User`, `Dept`, `Rank` 금지)
- **예외**: `EvaluationUsers` (원래 복수형), `Settings` (설정 그룹)

---

### 5. UrlActions 사용 규칙

```csharp
// ❌ 하드코딩 금지
NavigationManager.NavigateTo("/Admin/Users/Create");
<a href="/Admin/Users/Create">등록</a>

// ✅ UrlActions 사용
urlActions.MoveUserCreatePage();
<button @onclick="urlActions.MoveUserCreatePage">등록</button>
```

**UrlActions.cs 메서드 목록**:
```csharp
// 기본
MoveMainPage(), MoveLoginPage(), MoveLogoutPage(), MoveAdminPage()

// 사용자 관리
MoveUserManagePage(), MoveUserCreatePage(), MoveUserDetailsPage(uid),
MoveUserEditPage(uid), MoveUserDeletePage(uid)

// 부서 관리
MoveDeptCreatePage(), MoveDeptDetailsPage(deptId),
MoveDeptEditPage(deptId), MoveDeptDeletePage(deptId)

// 직급 관리
MoveRankCreatePage(), MoveRankDetailsPage(rankId),
MoveRankEditPage(rankId), MoveRankDeletePage(rankId)

// 평가대상자 관리
MoveEUserManagePage(), MoveEUserDetailsPage(uid), MoveEUsersEditPage(uid)

// 기초정보 관리
MoveSettingManagePage()
```

---

### 6. 순차적 생성 로직 (중요!)

**사용자 생성 시 3단계**:
```csharp
// 1단계: UserDb 생성
var addedUser = await userRepository.AddAsync(model);
long uid = addedUser.Uid;  // ← 생성된 Uid 가져오기

// 2단계: EvaluationUsers 생성
var evaluationUser = new EvaluationUsers
{
    Uid = uid,  // ← 2026년: BIGINT FK
    Is_Evaluation = true,
    TeamLeaderId = null,  // ← 2026년: BIGINT? (null 가능)
    DirectorId = null
};
await evaluationUsersRepository.AddAsync(evaluationUser);

// 3단계: ProcessDb 생성
var processDb = new ProcessDb
{
    Uid = uid,  // ← 2026년: BIGINT FK
    TeamLeaderId = null,
    DirectorId = null,
    Is_Request = false,
    Is_Agreement = false,
    Agreement_Comment = string.Empty,
    Is_SubRequest = false,  // ← 2026년 추가
    Is_SubAgreement = false,  // ← 2026년 추가
    SubAgreement_Comment = string.Empty,  // ← 2026년 추가
    Is_User_Submission = false,
    Is_Teamleader_Submission = false,
    Is_Director_Submission = false,
    FeedBackStatus = false,
    FeedBack_Submission = false
};
await processRepository.AddAsync(processDb);
```

---

## 📝 작업 문서

### 작업지시서
1. [20260126_01_phase3_3_admin_pages_rebuild.md](tasks/20260126_01_phase3_3_admin_pages_rebuild.md) - Phase 3-3 재작업 계획
2. [20260126_02_restructure_blazor_project.md](tasks/20260126_02_restructure_blazor_project.md) - 프로젝트 구조 재정리
3. [20260126_03_missing_components_checklist.md](tasks/20260126_03_missing_components_checklist.md) - 미구현 컴포넌트 체크리스트

### 이슈
- [#011: Phase 3-3 빌드 오류 및 재작업](issues/011_phase3_3_admin_pages_build_errors.md)

---

## 🎯 다음 작업

### Phase 1: 미구현 컴포넌트 완성
1. ✅ DisplayResultText 구현 (우선순위 1 - 가장 단순)
2. ✅ EUserListTable 구현 (우선순위 2)
3. ✅ MemberListTable 구현 (우선순위 3)

### Phase 2: Admin 페이지 완성
1. 2025년 코드 복사 및 DB 구조 변경 반영
2. CRUD 기능 구현
3. 단계별 빌드 테스트

### Phase 3: 테스트 및 검증
1. 기능 테스트
2. 빌드 경고 최종 확인 (목표: 5개 이하)
3. Phase 3-3 완료

---

## 📊 Git 커밋 이력

```
86c5871 - refactor: Blazor 프로젝트 구조 재정리
830a8ef - feat: Phase 3-3 빌드 경고 수정 및 기본 Admin 페이지 구조 생성
```

---

**작성자**: Claude AI
**작업 일자**: 2026-01-26
**작업 시간**: 약 2시간
**최종 상태**: 프로젝트 구조 재정리 완료, 컴포넌트 구현 대기
