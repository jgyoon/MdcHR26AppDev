# 작업지시서: Blazor 프로젝트 구조 재정리

**날짜**: 2026-01-26
**작업 유형**: 프로젝트 구조 리팩토링
**관련 이슈**: [#009: Phase 3 WebApp 개발](../issues/009_phase3_webapp_development.md)
**선행 작업지시서**: Phase 3-3 관리자 페이지 개발 (진행 중)

---

## 1. 작업 개요

### 1.1. 재구조화 배경

**문제점**:
1. **Pages 폴더가 최상위에 별도 존재**: `Pages/Admin/` vs `Components/Pages/`
2. **컴포넌트 위치 불일치**:
   - `Components/UserListTable.razor` (최상위에 있음)
   - `Components/CommonComponents/` (이름 불일치)
   - `Components/Modal/` (위치 변경 필요)
3. **최신 Blazor 트렌드 미반영**: 도서관리 프로젝트(.NET 10)의 구조와 다름

**목표**:
- ✅ 도서관리 프로젝트의 최신 구조 적용
- ✅ 모든 페이지를 `Components/Pages/` 하위로 통합
- ✅ 공용 컴포넌트를 `Components/Pages/Components/` 하위로 정리
  - `Common/` (공용)
  - `Modal/` (모달)
  - `Table/` (테이블)
- ✅ 폴더명 복수형 적용 (Dept → Depts, Rank → Ranks)

### 1.2. 참고 프로젝트

**도서관리 프로젝트**:
- 경로: `C:\Codes\36_MdcLibrary\MdcLibrary\MdcLibrary.Server`
- 구조: `Components/Pages/`, `Components/Pages/Components/`, `Components/Pages/Admin/`
- .NET 10, InteractiveServer 렌더 모드

---

## 2. 현재 구조 vs 목표 구조

### 2.1. 현재 구조

```
MdcHR26Apps.BlazorServer/
├── Components/
│   ├── App.razor
│   ├── Routes.razor
│   ├── _Imports.razor
│   ├── UserListTable.razor               ❌ 최상위에 있음
│   ├── CommonComponents/                 ❌ 이름 불일치
│   │   └── SearchbarComponent.razor
│   ├── Layout/                           ✅
│   │   ├── MainLayout.razor
│   │   ├── NavMenu.razor
│   │   └── ReconnectModal.razor
│   ├── Modal/                            ❌ 위치 변경 필요
│   │   └── UserDeleteModal.razor
│   └── Pages/                            ✅
│       ├── Counter.razor
│       ├── Error.razor
│       ├── Home.razor
│       ├── NotFound.razor
│       └── Auth/                         ✅
│           └── Login.razor
└── Pages/                                ❌ 별도 폴더
    ├── _Imports.razor
    └── Admin/
        ├── Index.razor
        ├── UserManage.razor
        ├── SettingManage.razor
        ├── EUsersManage.razor
        ├── EvaluationUsers/
        │   ├── Edit.razor
        │   └── Details.razor
        └── Settings/
            ├── Dept/                     ❌ 단수형
            │   ├── Create.razor
            │   ├── Edit.razor
            │   ├── Delete.razor
            │   └── Details.razor
            └── Rank/                     ❌ 단수형
                ├── Create.razor
                ├── Edit.razor
                ├── Delete.razor
                └── Details.razor
```

### 2.2. 목표 구조 (도서관리 프로젝트 참고)

```
MdcHR26Apps.BlazorServer/
└── Components/
    ├── App.razor
    ├── Routes.razor
    ├── _Imports.razor
    ├── Layout/                           ✅ 유지
    │   ├── MainLayout.razor
    │   ├── NavMenu.razor
    │   └── ReconnectModal.razor
    └── Pages/                            ✅ 모든 페이지 통합
        ├── Counter.razor
        ├── Error.razor
        ├── Home.razor
        ├── NotFound.razor
        ├── Auth/                         ✅ 유지
        │   └── Login.razor
        ├── Admin/                        ✅ 이동
        │   ├── Index.razor
        │   ├── UserManage.razor
        │   ├── SettingManage.razor
        │   ├── EUsersManage.razor
        │   ├── Users/                    ✅ 복수형
        │   │   ├── Create.razor
        │   │   ├── Edit.razor
        │   │   ├── Delete.razor
        │   │   └── Details.razor
        │   ├── EvaluationUsers/          ✅ 유지
        │   │   ├── Edit.razor
        │   │   └── Details.razor
        │   └── Settings/
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

## 3. 작업 단계

### Step 1: 백업 및 준비

**Git 커밋 확인**:
```bash
git status
git add .
git commit -m "feat: Phase 3-3 빌드 경고 수정 완료 (14개)"
```

**현재 파일 목록 저장**:
```bash
# 이동할 파일 목록 출력
find MdcHR26Apps.BlazorServer/Pages/Admin -type f -name "*.razor*"
find MdcHR26Apps.BlazorServer/Components -type f -name "*.razor" | grep -E "(UserListTable|Searchbar|UserDelete)"
```

---

### Step 2: 공용 컴포넌트 이동

#### 2.1. Table 컴포넌트

**이동**:
```bash
# 폴더 생성
mkdir -p "MdcHR26Apps.BlazorServer/Components/Pages/Components/Table"

# 이동
mv "MdcHR26Apps.BlazorServer/Components/UserListTable.razor" \
   "MdcHR26Apps.BlazorServer/Components/Pages/Components/Table/"
mv "MdcHR26Apps.BlazorServer/Components/UserListTable.razor.cs" \
   "MdcHR26Apps.BlazorServer/Components/Pages/Components/Table/"
```

**네임스페이스 변경** (`UserListTable.razor.cs`):
```csharp
// 변경 전
namespace MdcHR26Apps.BlazorServer.Components;

// 변경 후
namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Table;
```

#### 2.2. Common 컴포넌트

**이동**:
```bash
# 폴더 생성
mkdir -p "MdcHR26Apps.BlazorServer/Components/Pages/Components/Common"

# 이동
mv "MdcHR26Apps.BlazorServer/Components/CommonComponents/SearchbarComponent.razor" \
   "MdcHR26Apps.BlazorServer/Components/Pages/Components/Common/"
mv "MdcHR26Apps.BlazorServer/Components/CommonComponents/SearchbarComponent.razor.cs" \
   "MdcHR26Apps.BlazorServer/Components/Pages/Components/Common/"

# 빈 폴더 삭제
rmdir "MdcHR26Apps.BlazorServer/Components/CommonComponents"
```

**네임스페이스 변경** (`SearchbarComponent.razor.cs`):
```csharp
// 변경 전
namespace MdcHR26Apps.BlazorServer.Components.CommonComponents;

// 변경 후
namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Common;
```

#### 2.3. Modal 컴포넌트

**이동**:
```bash
# 폴더 생성
mkdir -p "MdcHR26Apps.BlazorServer/Components/Pages/Components/Modal"

# 이동
mv "MdcHR26Apps.BlazorServer/Components/Modal/UserDeleteModal.razor" \
   "MdcHR26Apps.BlazorServer/Components/Pages/Components/Modal/"
mv "MdcHR26Apps.BlazorServer/Components/Modal/UserDeleteModal.razor.cs" \
   "MdcHR26Apps.BlazorServer/Components/Pages/Components/Modal/"

# 빈 폴더 삭제
rmdir "MdcHR26Apps.BlazorServer/Components/Modal"
```

**네임스페이스 변경** (`UserDeleteModal.razor.cs`):
```csharp
// 변경 전
namespace MdcHR26Apps.BlazorServer.Components.Modal;

// 변경 후
namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Modal;
```

**빌드 테스트**: `dotnet build`

---

### Step 3: Admin 페이지 이동

#### 3.1. Admin 최상위 페이지 이동

**이동**:
```bash
# 폴더 생성
mkdir -p "MdcHR26Apps.BlazorServer/Components/Pages/Admin"

# 최상위 페이지 이동
mv "MdcHR26Apps.BlazorServer/Pages/Admin/Index.razor" \
   "MdcHR26Apps.BlazorServer/Components/Pages/Admin/"
mv "MdcHR26Apps.BlazorServer/Pages/Admin/Index.razor.cs" \
   "MdcHR26Apps.BlazorServer/Components/Pages/Admin/"

mv "MdcHR26Apps.BlazorServer/Pages/Admin/UserManage.razor" \
   "MdcHR26Apps.BlazorServer/Components/Pages/Admin/"
mv "MdcHR26Apps.BlazorServer/Pages/Admin/UserManage.razor.cs" \
   "MdcHR26Apps.BlazorServer/Components/Pages/Admin/"

mv "MdcHR26Apps.BlazorServer/Pages/Admin/SettingManage.razor" \
   "MdcHR26Apps.BlazorServer/Components/Pages/Admin/"
mv "MdcHR26Apps.BlazorServer/Pages/Admin/SettingManage.razor.cs" \
   "MdcHR26Apps.BlazorServer/Components/Pages/Admin/"

mv "MdcHR26Apps.BlazorServer/Pages/Admin/EUsersManage.razor" \
   "MdcHR26Apps.BlazorServer/Components/Pages/Admin/"
mv "MdcHR26Apps.BlazorServer/Pages/Admin/EUsersManage.razor.cs" \
   "MdcHR26Apps.BlazorServer/Components/Pages/Admin/"
```

**네임스페이스 변경** (4개 .cs 파일):
```csharp
// 변경 전
namespace MdcHR26Apps.BlazorServer.Pages.Admin;

// 변경 후
namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin;
```

#### 3.2. EvaluationUsers 이동

**이동**:
```bash
# 폴더 생성
mkdir -p "MdcHR26Apps.BlazorServer/Components/Pages/Admin/EvaluationUsers"

# 이동
mv "MdcHR26Apps.BlazorServer/Pages/Admin/EvaluationUsers/Edit.razor" \
   "MdcHR26Apps.BlazorServer/Components/Pages/Admin/EvaluationUsers/"
mv "MdcHR26Apps.BlazorServer/Pages/Admin/EvaluationUsers/Edit.razor.cs" \
   "MdcHR26Apps.BlazorServer/Components/Pages/Admin/EvaluationUsers/"

mv "MdcHR26Apps.BlazorServer/Pages/Admin/EvaluationUsers/Details.razor" \
   "MdcHR26Apps.BlazorServer/Components/Pages/Admin/EvaluationUsers/"
mv "MdcHR26Apps.BlazorServer/Pages/Admin/EvaluationUsers/Details.razor.cs" \
   "MdcHR26Apps.BlazorServer/Components/Pages/Admin/EvaluationUsers/"
```

**네임스페이스 변경** (2개 .cs 파일):
```csharp
// 변경 전
namespace MdcHR26Apps.BlazorServer.Pages.Admin.EvaluationUsers;

// 변경 후
namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin.EvaluationUsers;
```

#### 3.3. Settings/Dept → Settings/Depts 이동 및 복수형 변경

**이동 및 이름 변경**:
```bash
# 폴더 생성
mkdir -p "MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Depts"

# 이동
mv "MdcHR26Apps.BlazorServer/Pages/Admin/Settings/Dept/Create.razor" \
   "MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Depts/"
mv "MdcHR26Apps.BlazorServer/Pages/Admin/Settings/Dept/Create.razor.cs" \
   "MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Depts/"

mv "MdcHR26Apps.BlazorServer/Pages/Admin/Settings/Dept/Edit.razor" \
   "MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Depts/"
mv "MdcHR26Apps.BlazorServer/Pages/Admin/Settings/Dept/Edit.razor.cs" \
   "MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Depts/"

mv "MdcHR26Apps.BlazorServer/Pages/Admin/Settings/Dept/Delete.razor" \
   "MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Depts/"
mv "MdcHR26Apps.BlazorServer/Pages/Admin/Settings/Dept/Delete.razor.cs" \
   "MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Depts/"

mv "MdcHR26Apps.BlazorServer/Pages/Admin/Settings/Dept/Details.razor" \
   "MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Depts/"
mv "MdcHR26Apps.BlazorServer/Pages/Admin/Settings/Dept/Details.razor.cs" \
   "MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Depts/"
```

**네임스페이스 변경** (4개 .cs 파일):
```csharp
// 변경 전
namespace MdcHR26Apps.BlazorServer.Pages.Admin.Settings.Dept;

// 변경 후
namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin.Settings.Depts;
```

**@page 지시문 변경** (4개 .razor 파일):
```razor
// Create.razor
// 변경 전: @page "/Admin/Settings/Dept/Create"
// 변경 후:
@page "/Admin/Settings/Depts/Create"

// Edit.razor
// 변경 전: @page "/Admin/Settings/Dept/Edit/{Id:long}"
// 변경 후:
@page "/Admin/Settings/Depts/Edit/{Id:long}"

// Delete.razor
// 변경 전: @page "/Admin/Settings/Dept/Delete/{Id:long}"
// 변경 후:
@page "/Admin/Settings/Depts/Delete/{Id:long}"

// Details.razor
// 변경 전: @page "/Admin/Settings/Dept/Details/{Id:long}"
// 변경 후:
@page "/Admin/Settings/Depts/Details/{Id:long}"
```

#### 3.4. Settings/Rank → Settings/Ranks 이동 및 복수형 변경

**이동 및 이름 변경**:
```bash
# 폴더 생성
mkdir -p "MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Ranks"

# 이동
mv "MdcHR26Apps.BlazorServer/Pages/Admin/Settings/Rank/Create.razor" \
   "MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Ranks/"
mv "MdcHR26Apps.BlazorServer/Pages/Admin/Settings/Rank/Create.razor.cs" \
   "MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Ranks/"

mv "MdcHR26Apps.BlazorServer/Pages/Admin/Settings/Rank/Edit.razor" \
   "MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Ranks/"
mv "MdcHR26Apps.BlazorServer/Pages/Admin/Settings/Rank/Edit.razor.cs" \
   "MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Ranks/"

mv "MdcHR26Apps.BlazorServer/Pages/Admin/Settings/Rank/Delete.razor" \
   "MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Ranks/"
mv "MdcHR26Apps.BlazorServer/Pages/Admin/Settings/Rank/Delete.razor.cs" \
   "MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Ranks/"

mv "MdcHR26Apps.BlazorServer/Pages/Admin/Settings/Rank/Details.razor" \
   "MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Ranks/"
mv "MdcHR26Apps.BlazorServer/Pages/Admin/Settings/Rank/Details.razor.cs" \
   "MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Ranks/"
```

**네임스페이스 변경** (4개 .cs 파일):
```csharp
// 변경 전
namespace MdcHR26Apps.BlazorServer.Pages.Admin.Settings.Rank;

// 변경 후
namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin.Settings.Ranks;
```

**@page 지시문 변경** (4개 .razor 파일):
```razor
// Create.razor
// 변경 전: @page "/Admin/Settings/Rank/Create"
// 변경 후:
@page "/Admin/Settings/Ranks/Create"

// Edit.razor
// 변경 전: @page "/Admin/Settings/Rank/Edit/{Id:long}"
// 변경 후:
@page "/Admin/Settings/Ranks/Edit/{Id:long}"

// Delete.razor
// 변경 전: @page "/Admin/Settings/Rank/Delete/{Id:long}"
// 변경 후:
@page "/Admin/Settings/Ranks/Delete/{Id:long}"

// Details.razor
// 변경 전: @page "/Admin/Settings/Rank/Details/{Id:long}"
// 변경 후:
@page "/Admin/Settings/Depts/Details/{Id:long}"
```

**빌드 테스트**: `dotnet build`

---

### Step 4: 빈 폴더 삭제 및 _Imports.razor 정리

#### 4.1. 빈 Pages 폴더 삭제

```bash
# 빈 폴더 삭제
rmdir "MdcHR26Apps.BlazorServer/Pages/Admin/Settings/Dept"
rmdir "MdcHR26Apps.BlazorServer/Pages/Admin/Settings/Rank"
rmdir "MdcHR26Apps.BlazorServer/Pages/Admin/Settings"
rmdir "MdcHR26Apps.BlazorServer/Pages/Admin/EvaluationUsers"
rmdir "MdcHR26Apps.BlazorServer/Pages/Admin"
rm "MdcHR26Apps.BlazorServer/Pages/_Imports.razor"
rmdir "MdcHR26Apps.BlazorServer/Pages"
```

#### 4.2. _Imports.razor 확인

**Components/_Imports.razor** 확인:
```razor
@using System.Net.Http
@using System.Net.Http.Json
@using Microsoft.AspNetCore.Components.Forms
@using Microsoft.AspNetCore.Components.Routing
@using Microsoft.AspNetCore.Components.Web
@using static Microsoft.AspNetCore.Components.Web.RenderMode
@using Microsoft.AspNetCore.Components.Web.Virtualization
@using Microsoft.JSInterop
@using MdcHR26Apps.BlazorServer
@using MdcHR26Apps.BlazorServer.Components
@using MdcHR26Apps.BlazorServer.Components.Layout
@using MdcHR26Apps.BlazorServer.Components.Pages.Components.Common
@using MdcHR26Apps.BlazorServer.Components.Pages.Components.Modal
@using MdcHR26Apps.BlazorServer.Components.Pages.Components.Table
```

**빌드 테스트**: `dotnet build`

---

### Step 5: UrlActions 메서드 경로 업데이트

**파일**: `MdcHR26Apps.BlazorServer/Data/UrlActions.cs`

**Depts 경로 변경**:
```csharp
// 변경 전
public void MoveDeptCreatePage() => _navigationManager.NavigateTo("/Admin/Settings/Dept/Create");
public void MoveDeptDetailsPage(long deptId) => _navigationManager.NavigateTo($"/Admin/Settings/Dept/Details/{deptId}");
public void MoveDeptEditPage(long deptId) => _navigationManager.NavigateTo($"/Admin/Settings/Dept/Edit/{deptId}");
public void MoveDeptDeletePage(long deptId) => _navigationManager.NavigateTo($"/Admin/Settings/Dept/Delete/{deptId}");

// 변경 후
public void MoveDeptCreatePage() => _navigationManager.NavigateTo("/Admin/Settings/Depts/Create");
public void MoveDeptDetailsPage(long deptId) => _navigationManager.NavigateTo($"/Admin/Settings/Depts/Details/{deptId}");
public void MoveDeptEditPage(long deptId) => _navigationManager.NavigateTo($"/Admin/Settings/Depts/Edit/{deptId}");
public void MoveDeptDeletePage(long deptId) => _navigationManager.NavigateTo($"/Admin/Settings/Depts/Delete/{deptId}");
```

**Ranks 경로 변경**:
```csharp
// 변경 전
public void MoveRankCreatePage() => _navigationManager.NavigateTo("/Admin/Settings/Rank/Create");
public void MoveRankDetailsPage(long rankId) => _navigationManager.NavigateTo($"/Admin/Settings/Rank/Details/{rankId}");
public void MoveRankEditPage(long rankId) => _navigationManager.NavigateTo($"/Admin/Settings/Rank/Edit/{rankId}");
public void MoveRankDeletePage(long rankId) => _navigationManager.NavigateTo($"/Admin/Settings/Rank/Delete/{rankId}");

// 변경 후
public void MoveRankCreatePage() => _navigationManager.NavigateTo("/Admin/Settings/Ranks/Create");
public void MoveRankDetailsPage(long rankId) => _navigationManager.NavigateTo($"/Admin/Settings/Ranks/Details/{rankId}");
public void MoveRankEditPage(long rankId) => _navigationManager.NavigateTo($"/Admin/Settings/Ranks/Edit/{rankId}");
public void MoveRankDeletePage(long rankId) => _navigationManager.NavigateTo($"/Admin/Settings/Ranks/Delete/{rankId}");
```

**빌드 테스트**: `dotnet build`

---

### Step 6: 최종 빌드 및 검증

#### 6.1. 전체 빌드

```bash
cd "c:\Codes\00_Develop_Cursor\10_MdcHR26Apps"
dotnet clean
dotnet build
```

#### 6.2. 구조 확인

```bash
# 새 구조 확인
find MdcHR26Apps.BlazorServer/Components/Pages -type d

# Pages 폴더가 삭제되었는지 확인
ls MdcHR26Apps.BlazorServer/Pages  # 오류가 나야 정상
```

#### 6.3. 파일 개수 확인

```bash
# Admin 페이지 개수
find MdcHR26Apps.BlazorServer/Components/Pages/Admin -name "*.razor" | wc -l
# 예상: 12개 (4개 최상위 + 2개 EvaluationUsers + 4개 Depts + 4개 Ranks) - 일부 미완성

# 공용 컴포넌트 개수
find MdcHR26Apps.BlazorServer/Components/Pages/Components -name "*.razor" | wc -l
# 예상: 3개 (SearchbarComponent, UserDeleteModal, UserListTable)
```

---

## 4. Git 커밋

```bash
git add .
git commit -m "refactor: Blazor 프로젝트 구조 재정리

- Pages/ 폴더를 Components/Pages/로 통합
- 공용 컴포넌트를 Components/Pages/Components/로 재정리
  - Common/ (SearchbarComponent)
  - Modal/ (UserDeleteModal)
  - Table/ (UserListTable)
- 폴더명 복수형 적용 (Dept → Depts, Rank → Ranks)
- 네임스페이스 및 @page 경로 업데이트
- UrlActions 메서드 경로 업데이트
- 도서관리 프로젝트(.NET 10) 구조 참고"
```

---

## 5. 테스트 항목

### 5.1. 빌드 테스트
- [ ] `dotnet build` 성공
- [ ] 경고 7개 (RZ10012 - 미구현 컴포넌트) 유지
- [ ] 오류 0개

### 5.2. 구조 검증
- [ ] `Pages/` 폴더 삭제됨
- [ ] `Components/Pages/Admin/` 존재
- [ ] `Components/Pages/Components/Common/` 존재
- [ ] `Components/Pages/Components/Modal/` 존재
- [ ] `Components/Pages/Components/Table/` 존재
- [ ] `Settings/Depts/` (복수형) 확인
- [ ] `Settings/Ranks/` (복수형) 확인

### 5.3. 경로 검증
- [ ] UrlActions의 Depts/Ranks 경로 확인
- [ ] @page 지시문의 /Admin/Settings/Depts/ 경로 확인
- [ ] @page 지시문의 /Admin/Settings/Ranks/ 경로 확인

---

## 6. 주의사항

1. **네임스페이스 일괄 변경**:
   - `Pages.Admin` → `Components.Pages.Admin`
   - `Pages.Admin.Settings.Dept` → `Components.Pages.Admin.Settings.Depts`
   - `Pages.Admin.Settings.Rank` → `Components.Pages.Admin.Settings.Ranks`

2. **@page 경로 일괄 변경**:
   - `/Admin/Settings/Dept/` → `/Admin/Settings/Depts/`
   - `/Admin/Settings/Rank/` → `/Admin/Settings/Ranks/`

3. **UrlActions 메서드**: Dept → Depts, Rank → Ranks 경로 업데이트

4. **빈 폴더 삭제**: Pages/ 폴더 완전히 제거

5. **컴포넌트 참조**: _Imports.razor에 새 네임스페이스 추가

---

## 7. 완료 확인

- [ ] Step 1: 백업 및 준비 완료
- [ ] Step 2: 공용 컴포넌트 이동 완료 (빌드 테스트 통과)
- [ ] Step 3: Admin 페이지 이동 완료 (빌드 테스트 통과)
- [ ] Step 4: 빈 폴더 삭제 완료
- [ ] Step 5: UrlActions 업데이트 완료 (빌드 테스트 통과)
- [ ] Step 6: 최종 빌드 및 검증 완료
- [ ] Git 커밋 완료

---

**작성자**: Claude AI
**검토자**: 개발자
**승인 상태**: 검토 대기
