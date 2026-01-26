# 이슈 #011: Phase 3-3 관리자 페이지 빌드 오류 및 재작업 필요

**날짜**: 2026-01-22
**상태**: 진행중
**우선순위**: 높음
**관련 이슈**: [#009](009_phase3_webapp_development.md)

---

## 개발자 요청

**배경**:
- Phase 3-3 관리자 페이지 개발 작업 진행 중 다수의 빌드 오류 발생
- 기존 코드 구조와 API를 정확히 파악하지 않고 작업하여 불일치 문제 발생
- 작업지시서 작성 이전으로 롤백 후 재작업 필요

**요청 사항**:
1. Phase 3-3 관리자 페이지 개발 작업을 내일로 연기
2. 이슈 등록하여 문제점 기록 및 다음 작업 시 참고

---

## 발생한 문제점

### 1. API 및 메서드 시그니처 불일치
- **LoginStatusService**: `IsLogined`, `IsAdministrator` 속성명 잘못 사용
- **PasswordHasher**: 메서드 시그니처 미확인 (`HashPassword` 튜플 반환 vs 단일 반환)
- **IUserRepository**: `UserIdCheck` 메서드 존재 여부 미확인

### 2. 모델 속성 불일치
- **v_MemberListDB**: `ERankId` 속성이 실제로 존재하지 않음
- **EDepartmentDb/ERankDb**: `DeleteAsync`에 객체 대신 ID 전달 필요

### 3. 문자열 처리 오류
- 한글 문자열 잘못된 사용: `"Count개의"`, `"Count명"`, `"memberCount명의"` 등
- 템플릿 보간 구문 오류

### 4. 네임스페이스 오류
- 중복 using 지시문 (sed 일괄 변경 부작용)
- Services vs Data 네임스페이스 혼용

### 5. ProcessDb 구조 불일치
- 초기 작성: ProcessName, SortNo 필드 사용 (1st, 2nd, 3rd 3개 레코드)
- 실제 구조: 사용자당 단일 레코드, 평가 워크플로우 상태 관리 필드들

---

## 롤백 내역

**롤백 시점**: `5ba9b4d feat: test-gen Skill 추가`

**삭제된 커밋**:
1. `42ac528` fix: 빌드 오류 수정
2. `3033949` feat: Phase 3-3 Step 4-6 완료 (부서/직급/평가대상자 관리)
3. `227c146` feat: Phase 3-3 Step 3 완료 (사용자 관리 5개 페이지)
4. `d7e1ce5` feat: Phase 3-3 Step 1-2 완료 (유틸리티, 컴포넌트, 관리자 허브)
5. `4b3218a` docs: Phase 3-3 관리자 페이지 작업지시서 작성

---

## 재작업 시 필요한 사전 작업

### 1. 기존 코드 구조 파악
**LoginStatusService 확인**:
```
- 정확한 속성명 확인 (IsLoggedIn? IsLogined?)
- 메서드 시그니처 확인
```

**PasswordHasher 확인**:
```
- HashPassword 메서드 반환 타입 확인
- 튜플 반환 여부 ((hash, salt) vs byte[])
```

**Repository 메서드 확인**:
```
- IUserRepository.UserIdCheck vs CheckUserIdAsync
- DeleteAsync 파라미터 타입 (ID vs Entity)
```

### 2. 모델 및 뷰 속성 정확히 파악
**v_MemberListDB**:
```
- 사용 가능한 모든 속성 목록 확인
- ERankId vs ERankName 등 정확한 필드명
```

**EDepartmentDb / ERankDb**:
```
- PK 필드명 확인 (EDepartId, ERankId)
- DeleteAsync 메서드 시그니처
```

**ProcessDb**:
```
- 실제 필드 구조 확인
- 사용자당 레코드 개수 (단일 vs 다중)
- 초기 생성 시 설정해야 할 필드
```

### 3. 단계별 빌드 테스트
- Step 1 완료 후 빌드 테스트
- Step 2 완료 후 빌드 테스트
- 각 Step마다 검증하며 진행

---

## 해결 방안

### 1. 코드 분석 우선 진행

**Phase 1**: 기존 코드 읽기
```
1. LoginStatusService.cs 전체 읽기
2. PasswordHasher.cs 전체 읽기
3. IUserRepository, IEDepartmentRepository, IERankRepository 인터페이스 읽기
4. v_MemberListDB, EDepartmentDb, ERankDb, ProcessDb 모델 읽기
```

**Phase 2**: API 목록 정리
```
- 각 클래스/인터페이스의 공개 메서드 및 속성 목록 작성
- 메서드 시그니처 정확히 기록
```

### 2. 작업지시서 재작성

**수정 사항**:
- 실제 API에 맞춘 코드 예시 작성
- 각 모델의 정확한 속성 사용
- 한글 문자열 템플릿 보간 문법 수정

### 3. 점진적 개발

**Step-by-Step 빌드**:
```
Step 1 완료 → 빌드 테스트 → Step 2 진행
각 단계마다 검증하며 오류 즉시 수정
```

---

## 진행 사항

- [x] 문제점 분석
- [x] 작업지시서 이전으로 롤백
- [x] 이슈 등록
- [ ] 기존 코드 구조 분석 (내일)
- [ ] 작업지시서 재작성 (내일)
- [ ] 단계별 개발 진행 (내일)

---

## 테스트 계획

### 빌드 테스트
1. Step 1 완료 후: UserUtils, 컴포넌트 빌드 확인
2. Step 2 완료 후: Admin Hub, UrlActions 빌드 확인
3. Step 3-6 각 완료 후: 해당 페이지 빌드 확인

### 기능 테스트
1. 각 CRUD 페이지 동작 확인
2. 권한 체크 확인
3. N+1 쿼리 방지 확인

---

## 관련 문서

**작업지시서** (삭제됨):
- 20260122_01_phase3_3_admin_pages.md (롤백으로 삭제)

**관련 이슈**:
- [#009: Phase 3 Blazor Server WebApp 개발](009_phase3_webapp_development.md)

**참고 파일**:
- works/tasks/TEMPLATE_detailed.md (복잡한 작업 템플릿)
- works/tasks/20260120_02_phase3_1_project_setup.md (성공 사례)

---

## 개발자 피드백

**작업 연기 사유**:
- 빌드 오류 다수 발생
- 기존 코드 구조 파악 필요
- 내일 재작업 예정

**다음 작업 시 주의사항**:
- 기존 코드 구조 먼저 파악
- API 시그니처 정확히 확인
- 단계별 빌드 테스트

---

## 개발자 문제 진단 (2026-01-26)

### 근본 원인 분석

#### 1. 임의로 코드 개발 ⚠️

**문제점**:
- 본 프로젝트는 **2025년 인사평가를 복사해서 2026년에 적용**하는 것이 목적
- 변경 사항(.NET 10, DB 최적화)만 적용하면 됨
- **하지만 임의의 효율성을 고려해 코드를 임의로 수정**한 것이 문제

**영향**:
- API 시그니처 불일치
- 모델 속성 불일치
- 기존 동작 로직 손상

**해결책**:
- **참고 프로젝트 코드를 먼저 복사**
- 변경 사항만 최소한으로 적용
- 임의 수정 금지

#### 2. 코드 이해 부족 - 순차적 생성 로직 미반영 ⚠️⚠️

**문제점**:
- 사용자 생성 시 **순차적으로 3개 정보를 생성**해야 함:
  1. 사용자 정보 생성 (UserDb)
  2. 인사평가 대상자 정보 생성 (테이블명 확인 필요)
  3. 프로세스 정보 생성 (ProcessDb)

**현재 프로그램의 문제**:
- ProcessDb 구조를 잘못 이해:
  - 잘못된 이해: `ProcessName`, `SortNo` 필드로 3개 레코드 생성 (1st, 2nd, 3rd)
  - 실제 구조: 사용자당 **단일 레코드**, 평가 워크플로우 상태 관리
- 순차적 생성 로직이 전혀 반영되지 않음

**해결책**:
- **2025년 인사평가 프로젝트의 사용자 생성 코드를 분석**
- 순차적 생성 로직 정확히 파악
- 작업지시서에 명확히 기술

---

## 참고 프로젝트 정보

### 2025년 인사평가 (주 참고)
**프로젝트명**: MdcHR25Apps.BlazorApp
**경로**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp`

**복사 대상**:
- Admin 폴더 (사용자, 부서, 직급, 평가대상자 관리)
- 사용자 생성 순차 로직
- 모델 및 Repository 사용 패턴

### 도서관리 (기술 참고)
**프로젝트명**: MdcLibrary.Server
**경로**: `C:\Codes\36_MdcLibrary\MdcLibrary\MdcLibrary.Server`

**참고 사항**:
- .NET 10 적용 방법
- InteractiveServer 렌더 모드
- 최신 Blazor 패턴

---

## 재작업 계획 (수정)

### Phase 1: 참고 프로젝트 분석 (필수 선행)

#### 1-1. 2025년 Admin 폴더 분석
```
C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages\Admin\
├── User/ (사용자 관리)
│   ├── UserList.razor
│   ├── UserInsert.razor  ← 순차적 생성 로직 확인
│   ├── UserUpdate.razor
│   ├── UserDelete.razor
│   └── UserDetail.razor
├── Department/ (부서 관리)
├── Rank/ (직급 관리)
└── Member/ (평가대상자 관리)
```

**확인 사항**:
- UserInsert.razor의 사용자 생성 로직
- 순차적 생성: UserDb → 대상자 정보 → ProcessDb
- ProcessDb 초기값 설정 방법
- 사용되는 Repository 메서드 시그니처

#### 1-2. 모델 및 Repository 확인
```
MdcHR25Apps.Data/
├── Models/
│   ├── UserDb.cs
│   ├── ProcessDb.cs  ← 필드 구조 확인
│   └── (대상자 모델 확인)
└── Repositories/
    ├── IUserRepository.cs
    ├── IProcessRepository.cs
    └── (기타 Repository)
```

**확인 사항**:
- ProcessDb의 실제 필드 구조
- 사용자당 ProcessDb 레코드 개수 (단일 vs 다중)
- Repository 메서드 시그니처

### Phase 2: 세밀한 작업지시서 재작성

**TEMPLATE_detailed.md 기반**:

```markdown
# Step 0: 참고 코드 복사
- 2025년 Admin 폴더 파일 복사
- 네임스페이스 변경 (MdcHR25Apps → MdcHR26Apps)
- .NET 10 적용 사항만 반영

# Step 1: 사용자 관리 - UserList.razor
- 2025년 코드 복사
- API 시그니처 확인 후 최소 변경
- 빌드 테스트

# Step 2: 사용자 관리 - UserInsert.razor
- 2025년 코드 복사
- 순차적 생성 로직 그대로 유지:
  1. UserDb 생성
  2. 대상자 정보 생성
  3. ProcessDb 생성 (초기값 설정)
- 빌드 테스트

(계속)
```

### Phase 3: 점진적 개발

- **복사 우선, 수정 최소화**
- Step 단위로 빌드 테스트
- 오류 발견 시 즉시 수정

---

---

## 분석 완료 (2026-01-26)

### Phase 1: 2025년 Admin 폴더 구조 분석 ✅

**분석 결과**:
- 사용자 생성 순차 로직 확인 (Users/Create.razor.cs)
- 3단계 생성: UserDb → EvaluationUsers → ProcessDb
- 평가대상자 테이블명 확인: **EvaluationUsers**
- ProcessDb는 사용자당 **단일 레코드**

### Phase 2: DB 구조 변경 사항 파악 ✅

**2025년 vs 2026년 핵심 차이**:

| 테이블 | 필드 | 2025년 | 2026년 |
|--------|------|--------|--------|
| EvaluationUsers | 사용자 | UserId (VARCHAR) | Uid (BIGINT FK) |
| | 부서장 | TeamLeader_Id (VARCHAR) | TeamLeaderId (BIGINT FK) |
| ProcessDb | 사용자 | UserId (VARCHAR) | Uid (BIGINT FK) |
| | 하위 합의 | 없음 | Is_SubRequest, Is_SubAgreement |
| UserDb | 부서 | EDepartment (NVARCHAR) | EDepartId (BIGINT FK) |
| | 직급 | ERank (NVARCHAR) | ERankId (BIGINT FK) |

### Phase 3: 작업지시서 작성 ✅

**작업지시서**: [20260126_01_phase3_3_admin_pages_rebuild.md](../tasks/20260126_01_phase3_3_admin_pages_rebuild.md)

**주요 내용**:
- 2025년 코드 복사 → DB 구조 변경만 최소 수정
- 12개 Step (각 Step마다 빌드 테스트)
- 순차적 생성 로직 수정 가이드
- Repository API 확인 필수 사항
- **페이지 이동 통일** (Step 1):
  - UrlActions.cs에 모든 페이지 이동 메서드 추가
  - 하드코딩된 `<a href="/Admin/...">` 사용 금지
  - `urlActions.Move...Page()` 메서드로 통일
  - 2025년: UrlControls.cs 참고
- **폴더 구조 개선**: Settings 폴더로 기초정보 그룹화
  - `Admin/Dept/` → `Admin/Settings/Dept/` (부서 관리)
  - `Admin/Settings/Rank/` 추가 (직급 관리)
  - 외래키 연결된 마스터 데이터로 관리
  - 향후 확장 가능 (평가기간 설정 등)
- **UI 개선**: SettingManage.razor 통합 페이지
  - 탭 방식으로 부서/직급 관리 (DeptManage + RankManage 통합)
  - 탭 전환으로 빠른 이동
  - 향후 설정 항목 추가 용이

---

## 관련 문서 (업데이트)

**작업지시서**:
- [20260126_01_phase3_3_admin_pages_rebuild.md](../tasks/20260126_01_phase3_3_admin_pages_rebuild.md) ✅

**관련 이슈**:
- [#009: Phase 3 Blazor Server WebApp 개발](009_phase3_webapp_development.md)

---

## 작업 진행 상황 (2026-01-26)

### ✅ 완료된 작업

#### 1. 빌드 경고 수정 (14개)
- CS9113 경고 수정: 사용하지 않는 매개변수 제거 (2개)
- CS8601 경고 수정: null 처리 추가 (12개)
- 커밋: `830a8ef`

#### 2. 프로젝트 구조 재정리
- **배경**: 도서관리 프로젝트(.NET 10)의 최신 구조 적용
- **작업지시서**: [20260126_02_restructure_blazor_project.md](../tasks/20260126_02_restructure_blazor_project.md)
- **주요 변경사항**:
  - `Pages/` 폴더 → `Components/Pages/` 통합
  - 공용 컴포넌트 재정리:
    - `Components/Pages/Components/Common/` (SearchbarComponent)
    - `Components/Pages/Components/Modal/` (UserDeleteModal)
    - `Components/Pages/Components/Table/` (UserListTable)
  - 폴더명 복수형 적용: `Dept` → `Depts`, `Rank` → `Ranks`
  - 네임스페이스 업데이트: `Pages.Admin` → `Components.Pages.Admin`
  - @page 경로 업데이트: `/Admin/Settings/Dept/` → `/Admin/Settings/Depts/`
  - UrlActions 메서드 경로 업데이트
- **빌드 결과**: ✅ 성공 (경고 10개, 오류 0개)
- **커밋**: `86c5871`

---

### 📋 미구현 컴포넌트 (3개)

**체크리스트**: [20260126_03_missing_components_checklist.md](../tasks/20260126_03_missing_components_checklist.md)

1. **EUserListTable** (평가대상자 목록)
   - 사용 위치: `Admin/EUsersManage.razor`
   - 참고: 2025년 프로젝트, UserListTable.razor

2. **DisplayResultText** (결과 메시지 표시)
   - 사용 위치: `Settings/Depts/Create.razor`, `Settings/Ranks/Create.razor`
   - 참고: 도서관리 프로젝트

3. **MemberListTable** (부서/직급별 사용자 목록)
   - 사용 위치: `Settings/Depts/Details.razor`, `Settings/Ranks/Details.razor`
   - 참고: 2025년 프로젝트, UserListTable.razor

---

### 🎯 이후 개발 참고사항

#### 1. 참고 프로젝트

##### 2025년 인사평가 (비즈니스 로직)
- **경로**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp`
- **용도**:
  - 컴포넌트 구조 및 UI 참고
  - 평가 프로세스 로직 참고
  - 순차적 생성 로직 (UserDb → EvaluationUsers → ProcessDb)
- **주의사항**:
  - DB 구조 변경 반영 필요 (UserId → Uid, VARCHAR → BIGINT)
  - Primary Constructor 미사용 (기존 Inject 방식)

##### 도서관리 프로젝트 (최신 기술)
- **경로**: `C:\Codes\36_MdcLibrary\MdcLibrary\MdcLibrary.Server`
- **용도**:
  - .NET 10 최신 기능 참고
  - Primary Constructor 사용 패턴
  - InteractiveServer 렌더 모드
  - 프로젝트 구조 (Components/Pages/Components/)
  - 컴포넌트 명명 및 네임스페이스 규칙

#### 2. .NET 10 형식 활용

##### Primary Constructor (C# 13)
```csharp
// 기존 방식 (2025년)
public partial class Create
{
    [Inject]
    public IUserRepository userRepository { get; set; } = null!;
}

// .NET 10 방식 (2026년)
public partial class Create(
    IUserRepository userRepository,
    LoginStatusService loginStatusService,
    UrlActions urlActions)
{
    // 필드로 직접 사용 가능
}
```

##### InteractiveServer 렌더 모드
```razor
@page "/Admin/Users/Create"
@rendermode InteractiveServer
```

##### 컬렉션 초기화
```csharp
// 기존
private List<UserDb> users { get; set; } = new List<UserDb>();

// .NET 10
private List<UserDb> users { get; set; } = new();
```

#### 3. DB 구조 변경 사항

| 항목 | 2025년 | 2026년 |
|------|--------|--------|
| 사용자 ID | UserId (VARCHAR) | Uid (BIGINT FK) |
| 부서장 ID | TeamLeader_Id (VARCHAR) | TeamLeaderId (BIGINT FK) |
| 임원 ID | Director_Id (VARCHAR) | DirectorId (BIGINT FK) |
| 부서 | EDepartment (NVARCHAR) | EDepartId (BIGINT FK) |
| 직급 | ERank (NVARCHAR) | ERankId (BIGINT FK) |
| 하위 합의 | 없음 | Is_SubRequest, Is_SubAgreement |

**v_MemberListDB 뷰 주의**:
- ✅ `ERank` (ERankName의 별칭)
- ❌ `ERankId` (뷰에 없음!)

#### 4. 프로젝트 구조 규칙

```
Components/
├── Layout/                            # 레이아웃
├── Pages/                             # 모든 페이지
│   ├── Auth/                          # 인증 페이지
│   ├── Admin/                         # 관리자 페이지
│   │   ├── Users/                     # 복수형
│   │   ├── Settings/
│   │   │   ├── Depts/                 # 복수형
│   │   │   └── Ranks/                 # 복수형
│   │   └── EvaluationUsers/
│   └── Components/                    # 공용 컴포넌트
│       ├── Common/                    # 공통 UI
│       ├── Modal/                     # 모달
│       └── Table/                     # 테이블
```

#### 5. 네임스페이스 규칙

```csharp
// 페이지
namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin.Users;

// 공용 컴포넌트 - Common
namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Common;

// 공용 컴포넌트 - Modal
namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Modal;

// 공용 컴포넌트 - Table
namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Table;
```

#### 6. UrlActions 사용 규칙

```csharp
// 하드코딩 금지
❌ NavigationManager.NavigateTo("/Admin/Users/Create");
❌ <a href="/Admin/Users/Create">등록</a>

// UrlActions 사용
✅ urlActions.MoveUserCreatePage();
✅ <button @onclick="urlActions.MoveUserCreatePage">등록</button>
```

---

### 📝 다음 작업 계획

#### Phase 1: 미구현 컴포넌트 완성
1. DisplayResultText 구현 (우선순위 1)
2. EUserListTable 구현 (우선순위 2)
3. MemberListTable 구현 (우선순위 3)

#### Phase 2: Admin 페이지 완성
1. 2025년 코드 복사 및 수정
2. CRUD 기능 구현
3. 단계별 빌드 테스트

#### Phase 3: 테스트 및 검증
1. 기능 테스트
2. 빌드 경고 최종 확인
3. Phase 3-3 완료

---

**작업 일자**: 2026-01-26
**최종 상태**: 진행중 (구조 재정리 완료, 컴포넌트 구현 대기)
