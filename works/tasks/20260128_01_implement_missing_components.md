# 작업지시서: 미구현 컴포넌트 3개 구현

**날짜**: 2026-01-28
**작업 유형**: 컴포넌트 구현
**관련 이슈**:
- [#009: Phase 3 WebApp 개발](../issues/009_phase3_webapp_development.md)
- [#011: Phase 3-3 관리자 페이지 빌드 오류 및 재작업](../issues/011_phase3_3_admin_pages_build_errors.md)
**선행 작업지시서**:
- [20260126_02_restructure_blazor_project.md](20260126_02_restructure_blazor_project.md) (프로젝트 구조 재정리 완료)
- [20260126_03_missing_components_checklist.md](20260126_03_missing_components_checklist.md) (미구현 목록)

---

## 1. 작업 개요

### 1.1. 배경

Phase 3-3 관리자 페이지 개발 중 빌드 경고 발생:
- **RZ10012**: 3개 컴포넌트 미구현 (총 5건)

**프로젝트 구조 재정리 완료** (20260126_02):
- ✅ `Pages/` → `Components/Pages/` 통합
- ✅ 공용 컴포넌트 폴더 정리 (Common/Modal/Table)
- ✅ 네임스페이스 업데이트 완료

### 1.2. 작업 목표

Phase 3-3 완성을 위한 **미구현 컴포넌트 3개 구현**:

1. **DisplayResultText** (우선순위 1) - 결과 메시지 표시
2. **EUserListTable** (우선순위 2) - 평가대상자 목록 테이블
3. **MemberListTable** (우선순위 3) - 부서/직급별 사용자 목록 테이블

### 1.3. 참고 프로젝트

#### 2025년 인사평가 (비즈니스 로직)
- **경로**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp`
- **파일**:
  - `Components/DisplayResultText.razor`
  - `Components/EUserListTable.razor`
  - `Components/MemberListTable.razor`

#### 도서관리 프로젝트 (최신 기술)
- **경로**: `C:\Codes\36_MdcLibrary\MdcLibrary\MdcLibrary.Server`
- **파일**:
  - `Components/Pages/Components/Common/DisplayResultText.razor`
  - 테이블 컴포넌트 예시들

---

## 2. 미구현 컴포넌트 상세

### 2.1. DisplayResultText

**사용 위치**:
- `Components/Pages/Admin/Settings/Depts/Create.razor:9`
- `Components/Pages/Admin/Settings/Ranks/Create.razor:9`

**목적**: 생성/수정/삭제 후 성공/실패 메시지 표시

**구현 위치**:
- `Components/Pages/Components/Common/DisplayResultText.razor`
- `Components/Pages/Components/Common/DisplayResultText.razor.cs`

**참고**: 도서관리 프로젝트 동일 이름 컴포넌트

---

### 2.2. EUserListTable

**사용 위치**:
- `Components/Pages/Admin/EUsersManage.razor:21`

**목적**: 평가대상자(EvaluationUsers) 목록 테이블 표시

**구현 위치**:
- `Components/Pages/Components/Table/EUserListTable.razor`
- `Components/Pages/Components/Table/EUserListTable.razor.cs`

**참고**: 2025년 프로젝트 + UserListTable.razor 구조

---

### 2.3. MemberListTable

**사용 위치**:
- `Components/Pages/Admin/Settings/Depts/Details.razor:52`
- `Components/Pages/Admin/Settings/Ranks/Details.razor:52`

**목적**: 특정 부서/직급에 속한 사용자(v_MemberListDB) 목록 표시

**구현 위치**:
- `Components/Pages/Components/Table/MemberListTable.razor`
- `Components/Pages/Components/Table/MemberListTable.razor.cs`

**참고**: 2025년 프로젝트 + UserListTable.razor 구조

---

## 3. 작업 단계

### Step 1: DisplayResultText 구현

#### 3.1.1. 2025년 코드 읽기

**파일**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Components\DisplayResultText.razor`

**확인 사항**:
- Parameter 속성 (Comment 등)
- Bootstrap Alert 스타일
- 조건부 렌더링 (메시지 있을 때만 표시)

#### 3.1.2. 도서관리 프로젝트 참고

**파일**: `C:\Codes\36_MdcLibrary\MdcLibrary\MdcLibrary.Server\Components\Pages\Components\Common\DisplayResultText.razor`

**확인 사항**:
- .NET 10 스타일 (Primary Constructor)
- InteractiveServer 렌더 모드
- 네임스페이스 규칙

#### 3.1.3. 파일 생성

**DisplayResultText.razor**:
```razor
@namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Common
@rendermode InteractiveServer

@if (!string.IsNullOrEmpty(Comment))
{
    <div class="alert alert-info" role="alert">
        @Comment
    </div>
}

@code {
    [Parameter]
    public string? Comment { get; set; }
}
```

**주요 사항**:
- ✅ 네임스페이스: `MdcHR26Apps.BlazorServer.Components.Pages.Components.Common`
- ✅ InteractiveServer 렌더 모드
- ✅ Parameter: `Comment` (string nullable)
- ✅ 조건부 렌더링 (`!string.IsNullOrEmpty`)
- ✅ Bootstrap Alert 스타일 (alert-info)

**DisplayResultText.razor.cs** (선택사항):
- 필요 시 추가 로직 구현
- 간단한 경우 @code 블록만으로 충분

#### 3.1.4. 빌드 테스트

```bash
dotnet build
```

**예상 결과**:
- ✅ RZ10012 경고 2건 해결 (Depts/Create, Ranks/Create)
- ✅ 오류 0개

---

### Step 2: EUserListTable 구현

#### 3.2.1. 2025년 코드 읽기

**파일**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Components\EUserListTable.razor`

**확인 사항**:
- Parameter 속성 (Users 리스트)
- 테이블 구조 (컬럼 구성)
- 페이지 이동 로직 (Details, Edit)
- 사용되는 필드명

#### 3.2.2. UserListTable.razor 참고

**파일**: `Components/Pages/Components/Table/UserListTable.razor`

**확인 사항**:
- 현재 프로젝트의 테이블 구조 스타일
- UrlActions 사용 패턴
- 네임스페이스 및 렌더 모드

#### 3.2.3. DB 구조 변경 사항 반영

**2025년 → 2026년 주요 변경**:

| 필드 | 2025년 | 2026년 |
|------|--------|--------|
| 사용자 식별 | `UserId` (VARCHAR) + `UserName` | `Uid` (BIGINT FK) |
| 부서장 | `TeamLeader_Id` (VARCHAR) | `TeamLeaderId` (BIGINT FK) |
| 임원 | `Director_Id` (VARCHAR) | `DirectorId` (BIGINT FK) |

**코드 수정 예시**:
```razor
<!-- 2025년 -->
<td>@user.UserId</td>
<td>@user.UserName</td>

<!-- 2026년 -->
<td>@user.Uid</td>
<td>@user.UserDb?.UserName</td> <!-- UserName은 UserDb 참조 -->
```

#### 3.2.4. 파일 생성

**EUserListTable.razor**:
```razor
@namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Table
@rendermode InteractiveServer

<table class="table table-striped">
    <thead>
        <tr>
            <th>사번</th>
            <th>이름</th>
            <th>부서</th>
            <th>직급</th>
            <th>평가대상</th>
            <th>관리</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var user in Users)
        {
            <tr>
                <td>@user.Uid</td>
                <td><!-- UserDb 참조로 이름 표시 --></td>
                <td><!-- EDepartment 표시 --></td>
                <td><!-- ERank 표시 --></td>
                <td>@(user.Is_Evaluation ? "O" : "X")</td>
                <td>
                    <button class="btn btn-sm btn-primary"
                            @onclick="() => urlActions.MoveEUserDetailsPage(user.Uid)">
                        상세
                    </button>
                    <button class="btn btn-sm btn-secondary"
                            @onclick="() => urlActions.MoveEUsersEditPage(user.Uid)">
                        수정
                    </button>
                </td>
            </tr>
        }
    </tbody>
</table>

@code {
    [Parameter]
    public List<EvaluationUsers> Users { get; set; } = new();
}
```

**EUserListTable.razor.cs**:
```csharp
using Microsoft.AspNetCore.Components;
using MdcHR26Apps.Data;
using MdcHR26Apps.Models.HR26;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Table;

public partial class EUserListTable(UrlActions urlActions)
{
    private readonly UrlActions _urlActions = urlActions;

    [Parameter]
    public List<EvaluationUsers> Users { get; set; } = new();
}
```

**주요 사항**:
- ✅ Primary Constructor 사용 (UrlActions 주입)
- ✅ Uid 필드 사용 (UserId 아님!)
- ✅ TeamLeaderId, DirectorId (BIGINT nullable)
- ✅ UrlActions 메서드로 페이지 이동

**작업 시 주의**:
1. 2025년 코드를 **먼저 복사**
2. DB 구조 변경 사항만 **최소한으로 수정**
3. UserDb 참조 필요 시 Navigation Property 확인
4. 빌드 테스트

#### 3.2.5. 빌드 테스트

```bash
dotnet build
```

**예상 결과**:
- ✅ RZ10012 경고 1건 해결 (EUsersManage)
- ✅ 오류 0개

---

### Step 3: MemberListTable 구현

#### 3.3.1. 2025년 코드 읽기

**파일**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Components\MemberListTable.razor`

**확인 사항**:
- Parameter 속성 (Members 리스트)
- 테이블 구조
- 사용되는 필드명

#### 3.3.2. v_MemberListDB 뷰 주의사항

**사용 가능한 필드**:
- ✅ `Uid`, `UserId`, `UserName`, `ENumber`
- ✅ `ERank` (ERankName의 별칭)
- ✅ `EDepartId`, `EDepartmentName`
- ✅ `ActivateStatus`, `IsTeamLeader`, `IsDirector`, `IsAdministrator`

**사용 불가능한 필드**:
- ❌ `ERankId` (뷰에 없음!)

**코드 예시**:
```razor
<!-- 올바른 사용 -->
<td>@member.ERank</td>  <!-- ERankName의 별칭 ✅ -->

<!-- 잘못된 사용 -->
<td>@member.ERankId</td>  <!-- 뷰에 없음! ❌ -->
```

#### 3.3.3. 파일 생성

**MemberListTable.razor**:
```razor
@namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Table
@rendermode InteractiveServer

<table class="table table-striped">
    <thead>
        <tr>
            <th>사번</th>
            <th>아이디</th>
            <th>이름</th>
            <th>부서</th>
            <th>직급</th>
            <th>상태</th>
        </tr>
    </thead>
    <tbody>
        @if (Members == null || !Members.Any())
        {
            <tr>
                <td colspan="6" class="text-center">조회된 사용자가 없습니다.</td>
            </tr>
        }
        else
        {
            @foreach (var member in Members)
            {
                <tr>
                    <td>@member.ENumber</td>
                    <td>@member.UserId</td>
                    <td>@member.UserName</td>
                    <td>@member.EDepartmentName</td>
                    <td>@member.ERank</td> <!-- ✅ ERankId 아님! -->
                    <td>@(member.ActivateStatus ? "활성" : "비활성")</td>
                </tr>
            }
        }
    </tbody>
</table>

@code {
    [Parameter]
    public List<v_MemberListDB>? Members { get; set; }
}
```

**MemberListTable.razor.cs**:
```csharp
using Microsoft.AspNetCore.Components;
using MdcHR26Apps.Models.HR26;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Table;

public partial class MemberListTable
{
    [Parameter]
    public List<v_MemberListDB>? Members { get; set; }
}
```

**주요 사항**:
- ✅ Parameter: `List<v_MemberListDB>?` (nullable)
- ✅ ERank 필드 사용 (ERankId 없음!)
- ✅ 빈 목록 처리 (조회된 사용자가 없습니다)
- ✅ 네임스페이스: `Components.Pages.Components.Table`

#### 3.3.4. 빌드 테스트

```bash
dotnet build
```

**예상 결과**:
- ✅ RZ10012 경고 2건 해결 (Depts/Details, Ranks/Details)
- ✅ 오류 0개
- ✅ **모든 RZ10012 경고 해결** (총 5건)

---

## 4. _Imports.razor 확인

### 4.1. 네임스페이스 확인

**파일**: `Components/_Imports.razor`

**필요한 using 추가 확인**:
```razor
@using MdcHR26Apps.BlazorServer.Components.Pages.Components.Common
@using MdcHR26Apps.BlazorServer.Components.Pages.Components.Modal
@using MdcHR26Apps.BlazorServer.Components.Pages.Components.Table
```

**확인 방법**:
- `Components/_Imports.razor` 파일 열기
- 위 3줄이 있는지 확인
- 없으면 추가

---

## 5. 테스트 계획

개발자가 테스트할 항목:

### 5.1. 빌드 테스트

**각 Step 완료 후**:
```bash
dotnet build
```

**예상 결과**:
- Step 1 완료: RZ10012 경고 2건 감소 (10개 → 8개)
- Step 2 완료: RZ10012 경고 1건 감소 (8개 → 7개)
- Step 3 완료: RZ10012 경고 2건 감소 (7개 → 5개)
- **최종**: 경고 5개 (CS8601, CS9113만 남음)

### 5.2. 컴포넌트 렌더링 테스트

#### Test 1: DisplayResultText
1. `/Admin/Settings/Depts/Create` 접속
2. 부서 생성 시도
3. **확인**: 결과 메시지 표시 ✅

#### Test 2: EUserListTable
1. `/Admin/EUsersManage` 접속
2. 평가대상자 목록 표시
3. **확인**: 테이블 렌더링, Uid 필드 사용 ✅
4. "상세" 버튼 클릭
5. **확인**: Details 페이지 이동 ✅

#### Test 3: MemberListTable
1. `/Admin/Settings/Depts/Details/{DeptId}` 접속
2. 해당 부서 사용자 목록 표시
3. **확인**: 테이블 렌더링, ERank 필드 사용 ✅
4. 직급 Details 페이지도 동일 테스트

### 5.3. 회귀 테스트

**기존 페이지 정상 동작 확인**:
- `/Admin/UserManage` - 사용자 목록
- `/Admin/SettingManage` - 기초정보 관리 (탭 전환)
- `/auth/login` - 로그인

---

## 6. 주의사항

### 6.1. DB 구조 변경 반영 필수

**EvaluationUsers / ProcessDb**:
- ❌ `UserId` (VARCHAR) → ✅ `Uid` (BIGINT)
- ❌ `UserName` → 제거 (UserDb 참조)
- ❌ `TeamLeader_Id` → ✅ `TeamLeaderId`
- ❌ `Director_Id` → ✅ `DirectorId`

**v_MemberListDB**:
- ✅ `ERank` 사용 (ERankName 별칭)
- ❌ `ERankId` 사용 금지 (뷰에 없음!)

### 6.2. 네임스페이스 규칙

```csharp
// Common 컴포넌트
namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Common;

// Table 컴포넌트
namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Table;
```

### 6.3. Primary Constructor 사용

**.NET 10 스타일 (권장)**:
```csharp
public partial class EUserListTable(UrlActions urlActions)
{
    private readonly UrlActions _urlActions = urlActions;
}
```

**기존 Inject 방식** (허용):
```csharp
public partial class EUserListTable
{
    [Inject]
    public UrlActions urlActions { get; set; } = null!;
}
```

### 6.4. 단계별 빌드 테스트

- Step 1 완료 → 빌드 테스트
- Step 2 완료 → 빌드 테스트
- Step 3 완료 → 빌드 테스트
- **오류 발생 시 즉시 수정**, 다음 Step 진행 금지

---

## 7. 참고 문서

### 7.1. 선행 작업지시서

- [20260126_01_phase3_3_admin_pages_rebuild.md](20260126_01_phase3_3_admin_pages_rebuild.md) - DB 구조 변경 사항 정리
- [20260126_02_restructure_blazor_project.md](20260126_02_restructure_blazor_project.md) - 프로젝트 구조 참고
- [20260126_03_missing_components_checklist.md](20260126_03_missing_components_checklist.md) - 미구현 목록

### 7.2. 참고 프로젝트

**2025년 인사평가**:
- `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Components\`
  - `DisplayResultText.razor`
  - `EUserListTable.razor`
  - `MemberListTable.razor`

**도서관리 프로젝트**:
- `C:\Codes\36_MdcLibrary\MdcLibrary\MdcLibrary.Server\Components\Pages\Components\`
  - `Common/DisplayResultText.razor`
  - 테이블 컴포넌트 예시들

---

## 8. 완료 조건

- [ ] Step 1: DisplayResultText 구현 완료
  - [ ] .razor 파일 생성
  - [ ] 네임스페이스 확인
  - [ ] 빌드 테스트 통과
- [ ] Step 2: EUserListTable 구현 완료
  - [ ] 2025년 코드 복사 및 수정
  - [ ] Uid 필드 사용 확인
  - [ ] UrlActions 페이지 이동 확인
  - [ ] 빌드 테스트 통과
- [ ] Step 3: MemberListTable 구현 완료
  - [ ] 2025년 코드 복사 및 수정
  - [ ] ERank 필드 사용 확인 (ERankId 아님!)
  - [ ] 빈 목록 처리 확인
  - [ ] 빌드 테스트 통과
- [ ] _Imports.razor 네임스페이스 확인
- [ ] 전체 빌드 성공 (경고 5개 이하)
- [ ] Test 1 성공 (DisplayResultText 렌더링)
- [ ] Test 2 성공 (EUserListTable 렌더링 및 페이지 이동)
- [ ] Test 3 성공 (MemberListTable 렌더링)
- [ ] 회귀 테스트 성공 (기존 페이지 정상 동작)

---

## 9. 예상 빌드 경고 변화

### 현재 (10개 경고)

```
RZ10012: EUserListTable (1개)
RZ10012: DisplayResultText (2개)
RZ10012: MemberListTable (2개)
CS8601: null 참조 할당 (3개)
CS9113: 사용하지 않는 매개변수 (2개)
```

### 목표 (5개 경고)

```
CS8601: null 참조 할당 (3개) - 기존 페이지
CS9113: 사용하지 않는 매개변수 (2개) - Create.razor.cs (향후 해결)
```

---

**작성자**: Claude AI
**검토자**: 개발자
**승인 상태**: 검토 대기
