# 동기화 체크리스트

**생성일시**: 2026-01-26 17:34
**커밋 범위**: 5ba9b4d...4be7886
**변경 파일**: 58개 (신규: 50개, 수정: 8개)
**작업 내용**: Phase 3-3 Admin 페이지 구조 재정리 및 빌드 경고 수정

---

## 1. 동기화 개요

### 소스 프로젝트 (개발)
- **경로**: `C:\Codes\00_Develop_Cursor\10_MdcHR26Apps`
- **IDE**: VSCode
- **용도**: Claude 협업 개발

### 대상 프로젝트 (실제)
- **경로**: `C:\Codes\41_MdcHR26\MdcHR26App`
- **IDE**: Visual Studio 2022
- **용도**: 실제 빌드 및 배포

### 주요 변경 사항
1. ✅ Admin 페이지 구조 재정리 (Pages/ → Components/Pages/)
2. ✅ 컴포넌트 재구성 (Common/Modal/Table 분리)
3. ✅ 복수형 폴더명 적용 (Dept→Depts, Rank→Ranks)
4. ✅ 네임스페이스 통일 (Components.Pages.Admin)
5. ✅ 빌드 경고 수정 (CS9113, CS8601)
6. ✅ UrlActions 경로 업데이트

---

## 2. 동기화 절차

### 2.1. Database 폴더 (1개 파일)

#### ProcessDb 시드 데이터
- [ ] 소스: `Database/03_06_SeedData_Process.sql`
- [ ] 대상: `Database/03_06_SeedData_Process.sql`
- [ ] 설명: Process 테이블 초기 데이터 (Uid 1~10 샘플)
- [ ] 비고: DB 스크립트는 수동 실행 필요

---

### 2.2. Models 프로젝트 (3개 파일)

#### UserDb 모델 수정
- [ ] 소스: `MdcHR26Apps.Models/User/UserDb.cs`
- [ ] 대상: `MdcHR26Apps.Models/User/UserDb.cs`
- [ ] 설명: 컬럼 순서 조정 및 주석 추가
- [ ] 변경: 필드 순서 재배치 (DeptId, RankId, Name, Email 순)

#### UserRepository 수정
- [ ] 소스: `MdcHR26Apps.Models/User/UserRepository.cs`
- [ ] 대상: `MdcHR26Apps.Models/User/UserRepository.cs`
- [ ] 설명: CS8601 경고 수정 (null 체크 추가)
- [ ] 변경: GetByIdAsync, GetByEmailAsync에 null 체크 패턴 적용

#### v_MemberListRepository 수정
- [ ] 소스: `MdcHR26Apps.Models/Views/v_MemberListDB/v_MemberListRepository.cs`
- [ ] 대상: `MdcHR26Apps.Models/Views/v_MemberListDB/v_MemberListRepository.cs`
- [ ] 설명: CS8601 경고 수정 (null 체크 추가)
- [ ] 변경: GetByDeptIdAsync, GetByRankIdAsync에 null 체크 패턴 적용

---

### 2.3. BlazorServer - 핵심 파일 (4개 파일)

#### _Imports.razor 업데이트
- [ ] 소스: `MdcHR26Apps.BlazorServer/Components/_Imports.razor`
- [ ] 대상: `MdcHR26Apps.BlazorServer/Components/_Imports.razor`
- [ ] 설명: 컴포넌트 네임스페이스 추가
- [ ] 추가 항목:
  - `@using MdcHR26Apps.BlazorServer.Components.Pages.Components.Common`
  - `@using MdcHR26Apps.BlazorServer.Components.Pages.Components.Modal`
  - `@using MdcHR26Apps.BlazorServer.Components.Pages.Components.Table`

#### Program.cs 수정
- [ ] 소스: `MdcHR26Apps.BlazorServer/Program.cs`
- [ ] 대상: `MdcHR26Apps.BlazorServer/Program.cs`
- [ ] 설명: UserUtils 서비스 등록
- [ ] 추가: `builder.Services.AddScoped<UserUtils>();`

#### UrlActions.cs 업데이트
- [ ] 소스: `MdcHR26Apps.BlazorServer/Data/UrlActions.cs`
- [ ] 대상: `MdcHR26Apps.BlazorServer/Data/UrlActions.cs`
- [ ] 설명: 경로 변경 (Dept→Depts, Rank→Ranks)
- [ ] 변경 메서드:
  - `MoveDeptCreatePage()` → `/Admin/Settings/Depts/Create`
  - `MoveDeptDetailsPage()` → `/Admin/Settings/Depts/Details/{deptId}`
  - `MoveDeptEditPage()` → `/Admin/Settings/Depts/Edit/{deptId}`
  - `MoveDeptDeletePage()` → `/Admin/Settings/Depts/Delete/{deptId}`
  - `MoveRankCreatePage()` → `/Admin/Settings/Ranks/Create`
  - `MoveRankDetailsPage()` → `/Admin/Settings/Ranks/Details/{rankId}`
  - `MoveRankEditPage()` → `/Admin/Settings/Ranks/Edit/{rankId}`
  - `MoveRankDeletePage()` → `/Admin/Settings/Ranks/Delete/{rankId}`

#### AppStateService.cs 수정
- [ ] 소스: `MdcHR26Apps.BlazorServer/Data/AppStateService.cs`
- [ ] 대상: `MdcHR26Apps.BlazorServer/Data/AppStateService.cs`
- [ ] 설명: 주석 추가 및 코드 정리

---

### 2.4. BlazorServer - Utils 폴더 (1개 파일)

#### UserUtils 신규 생성
- [ ] 소스: `MdcHR26Apps.BlazorServer/Utils/UserUtils.cs`
- [ ] 대상: `MdcHR26Apps.BlazorServer/Utils/UserUtils.cs`
- [ ] 설명: 사용자 생성 유틸리티 (Primary Constructor 사용)
- [ ] 기능: EvaluationUsers, ProcessDb 순차 생성 로직

---

### 2.5. BlazorServer - Admin 페이지 (10개 파일)

#### Admin 메인 페이지
- [ ] 소스: `Components/Pages/Admin/Index.razor`
- [ ] 대상: `Components/Pages/Admin/Index.razor`
- [ ] 설명: 관리자 대시보드

- [ ] 소스: `Components/Pages/Admin/Index.razor.cs`
- [ ] 대상: `Components/Pages/Admin/Index.razor.cs`
- [ ] 네임스페이스: `MdcHR26Apps.BlazorServer.Components.Pages.Admin`

#### 기초정보 관리 페이지
- [ ] 소스: `Components/Pages/Admin/SettingManage.razor`
- [ ] 대상: `Components/Pages/Admin/SettingManage.razor`
- [ ] 설명: 부서/직급 관리 목록 페이지

- [ ] 소스: `Components/Pages/Admin/SettingManage.razor.cs`
- [ ] 대상: `Components/Pages/Admin/SettingManage.razor.cs`
- [ ] 네임스페이스: `MdcHR26Apps.BlazorServer.Components.Pages.Admin`

#### 사용자 관리 페이지
- [ ] 소스: `Components/Pages/Admin/UserManage.razor`
- [ ] 대상: `Components/Pages/Admin/UserManage.razor`
- [ ] 설명: 사용자 목록 및 검색 페이지

- [ ] 소스: `Components/Pages/Admin/UserManage.razor.cs`
- [ ] 대상: `Components/Pages/Admin/UserManage.razor.cs`
- [ ] 네임스페이스: `MdcHR26Apps.BlazorServer.Components.Pages.Admin`

#### 평가대상자 관리 페이지
- [ ] 소스: `Components/Pages/Admin/EUsersManage.razor`
- [ ] 대상: `Components/Pages/Admin/EUsersManage.razor`
- [ ] 설명: 평가대상자 목록 페이지

- [ ] 소스: `Components/Pages/Admin/EUsersManage.razor.cs`
- [ ] 대상: `Components/Pages/Admin/EUsersManage.razor.cs`
- [ ] 네임스페이스: `MdcHR26Apps.BlazorServer.Components.Pages.Admin`
- [ ] 주의: EUserListTable 컴포넌트 미구현 (RZ10012 경고 발생)

---

### 2.6. BlazorServer - Users 하위 페이지 (8개 파일)

#### Create 페이지
- [ ] 소스: `Components/Pages/Admin/Users/Create.razor`
- [ ] 대상: `Components/Pages/Admin/Users/Create.razor`
- [ ] @page: `/Admin/Users/Create`

- [ ] 소스: `Components/Pages/Admin/Users/Create.razor.cs`
- [ ] 대상: `Components/Pages/Admin/Users/Create.razor.cs`
- [ ] 네임스페이스: `MdcHR26Apps.BlazorServer.Components.Pages.Admin.Users`

#### Details 페이지
- [ ] 소스: `Components/Pages/Admin/Users/Details.razor`
- [ ] 대상: `Components/Pages/Admin/Users/Details.razor`
- [ ] @page: `/Admin/Users/Details/{Uid:long}`

- [ ] 소스: `Components/Pages/Admin/Users/Details.razor.cs`
- [ ] 대상: `Components/Pages/Admin/Users/Details.razor.cs`
- [ ] 네임스페이스: `MdcHR26Apps.BlazorServer.Components.Pages.Admin.Users`

#### Edit 페이지
- [ ] 소스: `Components/Pages/Admin/Users/Edit.razor`
- [ ] 대상: `Components/Pages/Admin/Users/Edit.razor`
- [ ] @page: `/Admin/Users/Edit/{Uid:long}`

- [ ] 소스: `Components/Pages/Admin/Users/Edit.razor.cs`
- [ ] 대상: `Components/Pages/Admin/Users/Edit.razor.cs`
- [ ] 네임스페이스: `MdcHR26Apps.BlazorServer.Components.Pages.Admin.Users`
- [ ] 경고 수정: CS8601 null 체크 패턴 적용

#### Delete 페이지
- [ ] 소스: `Components/Pages/Admin/Users/Delete.razor`
- [ ] 대상: `Components/Pages/Admin/Users/Delete.razor`
- [ ] @page: `/Admin/Users/Delete/{Uid:long}`

- [ ] 소스: `Components/Pages/Admin/Users/Delete.razor.cs`
- [ ] 대상: `Components/Pages/Admin/Users/Delete.razor.cs`
- [ ] 네임스페이스: `MdcHR26Apps.BlazorServer.Components.Pages.Admin.Users`

---

### 2.7. BlazorServer - Settings/Depts 하위 페이지 (8개 파일)

#### Create 페이지
- [ ] 소스: `Components/Pages/Admin/Settings/Depts/Create.razor`
- [ ] 대상: `Components/Pages/Admin/Settings/Depts/Create.razor`
- [ ] @page: `/Admin/Settings/Depts/Create`
- [ ] 주의: DisplayResultText 컴포넌트 미구현 (RZ10012 경고 발생)

- [ ] 소스: `Components/Pages/Admin/Settings/Depts/Create.razor.cs`
- [ ] 대상: `Components/Pages/Admin/Settings/Depts/Create.razor.cs`
- [ ] 네임스페이스: `MdcHR26Apps.BlazorServer.Components.Pages.Admin.Settings.Depts`
- [ ] 경고 수정: CS9113 사용하지 않는 매개변수 (`_eDepartmentRepository`)

#### Details 페이지
- [ ] 소스: `Components/Pages/Admin/Settings/Depts/Details.razor`
- [ ] 대상: `Components/Pages/Admin/Settings/Depts/Details.razor`
- [ ] @page: `/Admin/Settings/Depts/Details/{DeptId:long}`
- [ ] 주의: MemberListTable 컴포넌트 미구현 (RZ10012 경고 발생)

- [ ] 소스: `Components/Pages/Admin/Settings/Depts/Details.razor.cs`
- [ ] 대상: `Components/Pages/Admin/Settings/Depts/Details.razor.cs`
- [ ] 네임스페이스: `MdcHR26Apps.BlazorServer.Components.Pages.Admin.Settings.Depts`

#### Edit 페이지
- [ ] 소스: `Components/Pages/Admin/Settings/Depts/Edit.razor`
- [ ] 대상: `Components/Pages/Admin/Settings/Depts/Edit.razor`
- [ ] @page: `/Admin/Settings/Depts/Edit/{DeptId:long}`

- [ ] 소스: `Components/Pages/Admin/Settings/Depts/Edit.razor.cs`
- [ ] 대상: `Components/Pages/Admin/Settings/Depts/Edit.razor.cs`
- [ ] 네임스페이스: `MdcHR26Apps.BlazorServer.Components.Pages.Admin.Settings.Depts`
- [ ] 경고 수정: CS8601 null 체크 패턴 적용

#### Delete 페이지
- [ ] 소스: `Components/Pages/Admin/Settings/Depts/Delete.razor`
- [ ] 대상: `Components/Pages/Admin/Settings/Depts/Delete.razor`
- [ ] @page: `/Admin/Settings/Depts/Delete/{DeptId:long}`

- [ ] 소스: `Components/Pages/Admin/Settings/Depts/Delete.razor.cs`
- [ ] 대상: `Components/Pages/Admin/Settings/Depts/Delete.razor.cs`
- [ ] 네임스페이스: `MdcHR26Apps.BlazorServer.Components.Pages.Admin.Settings.Depts`

---

### 2.8. BlazorServer - Settings/Ranks 하위 페이지 (8개 파일)

#### Create 페이지
- [ ] 소스: `Components/Pages/Admin/Settings/Ranks/Create.razor`
- [ ] 대상: `Components/Pages/Admin/Settings/Ranks/Create.razor`
- [ ] @page: `/Admin/Settings/Ranks/Create`
- [ ] 주의: DisplayResultText 컴포넌트 미구현 (RZ10012 경고 발생)

- [ ] 소스: `Components/Pages/Admin/Settings/Ranks/Create.razor.cs`
- [ ] 대상: `Components/Pages/Admin/Settings/Ranks/Create.razor.cs`
- [ ] 네임스페이스: `MdcHR26Apps.BlazorServer.Components.Pages.Admin.Settings.Ranks`
- [ ] 경고 수정: CS9113 사용하지 않는 매개변수 (`_eRankRepository`)

#### Details 페이지
- [ ] 소스: `Components/Pages/Admin/Settings/Ranks/Details.razor`
- [ ] 대상: `Components/Pages/Admin/Settings/Ranks/Details.razor`
- [ ] @page: `/Admin/Settings/Ranks/Details/{RankId:long}`
- [ ] 주의: MemberListTable 컴포넌트 미구현 (RZ10012 경고 발생)

- [ ] 소스: `Components/Pages/Admin/Settings/Ranks/Details.razor.cs`
- [ ] 대상: `Components/Pages/Admin/Settings/Ranks/Details.razor.cs`
- [ ] 네임스페이스: `MdcHR26Apps.BlazorServer.Components.Pages.Admin.Settings.Ranks`

#### Edit 페이지
- [ ] 소스: `Components/Pages/Admin/Settings/Ranks/Edit.razor`
- [ ] 대상: `Components/Pages/Admin/Settings/Ranks/Edit.razor`
- [ ] @page: `/Admin/Settings/Ranks/Edit/{RankId:long}`

- [ ] 소스: `Components/Pages/Admin/Settings/Ranks/Edit.razor.cs`
- [ ] 대상: `Components/Pages/Admin/Settings/Ranks/Edit.razor.cs`
- [ ] 네임스페이스: `MdcHR26Apps.BlazorServer.Components.Pages.Admin.Settings.Ranks`

#### Delete 페이지
- [ ] 소스: `Components/Pages/Admin/Settings/Ranks/Delete.razor`
- [ ] 대상: `Components/Pages/Admin/Settings/Ranks/Delete.razor`
- [ ] @page: `/Admin/Settings/Ranks/Delete/{RankId:long}`

- [ ] 소스: `Components/Pages/Admin/Settings/Ranks/Delete.razor.cs`
- [ ] 대상: `Components/Pages/Admin/Settings/Ranks/Delete.razor.cs`
- [ ] 네임스페이스: `MdcHR26Apps.BlazorServer.Components.Pages.Admin.Settings.Ranks`

---

### 2.9. BlazorServer - EvaluationUsers 하위 페이지 (4개 파일)

#### Details 페이지
- [ ] 소스: `Components/Pages/Admin/EvaluationUsers/Details.razor`
- [ ] 대상: `Components/Pages/Admin/EvaluationUsers/Details.razor`
- [ ] @page: `/Admin/EvaluationUsers/Details/{Uid:long}`

- [ ] 소스: `Components/Pages/Admin/EvaluationUsers/Details.razor.cs`
- [ ] 대상: `Components/Pages/Admin/EvaluationUsers/Details.razor.cs`
- [ ] 네임스페이스: `MdcHR26Apps.BlazorServer.Components.Pages.Admin.EvaluationUsers`

#### Edit 페이지
- [ ] 소스: `Components/Pages/Admin/EvaluationUsers/Edit.razor`
- [ ] 대상: `Components/Pages/Admin/EvaluationUsers/Edit.razor`
- [ ] @page: `/Admin/EvaluationUsers/Edit/{Uid:long}`

- [ ] 소스: `Components/Pages/Admin/EvaluationUsers/Edit.razor.cs`
- [ ] 대상: `Components/Pages/Admin/EvaluationUsers/Edit.razor.cs`
- [ ] 네임스페이스: `MdcHR26Apps.BlazorServer.Components.Pages.Admin.EvaluationUsers`
- [ ] 경고 수정: CS8601 null 체크 패턴 적용 (3곳)
- [ ] 버그 수정: 중복 중괄호 제거, 변수명 충돌 해결

---

### 2.10. BlazorServer - 공통 컴포넌트 (6개 파일)

#### SearchbarComponent
- [ ] 소스: `Components/Pages/Components/Common/SearchbarComponent.razor`
- [ ] 대상: `Components/Pages/Components/Common/SearchbarComponent.razor`
- [ ] 설명: 검색바 컴포넌트

- [ ] 소스: `Components/Pages/Components/Common/SearchbarComponent.razor.cs`
- [ ] 대상: `Components/Pages/Components/Common/SearchbarComponent.razor.cs`
- [ ] 네임스페이스: `MdcHR26Apps.BlazorServer.Components.Pages.Components.Common`

#### UserDeleteModal
- [ ] 소스: `Components/Pages/Components/Modal/UserDeleteModal.razor`
- [ ] 대상: `Components/Pages/Components/Modal/UserDeleteModal.razor`
- [ ] 설명: 사용자 삭제 확인 모달

- [ ] 소스: `Components/Pages/Components/Modal/UserDeleteModal.razor.cs`
- [ ] 대상: `Components/Pages/Components/Modal/UserDeleteModal.razor.cs`
- [ ] 네임스페이스: `MdcHR26Apps.BlazorServer.Components.Pages.Components.Modal`

#### UserListTable
- [ ] 소스: `Components/Pages/Components/Table/UserListTable.razor`
- [ ] 대상: `Components/Pages/Components/Table/UserListTable.razor`
- [ ] 설명: 사용자 목록 테이블

- [ ] 소스: `Components/Pages/Components/Table/UserListTable.razor.cs`
- [ ] 대상: `Components/Pages/Components/Table/UserListTable.razor.cs`
- [ ] 네임스페이스: `MdcHR26Apps.BlazorServer.Components.Pages.Components.Table`

---

## 3. 검증 절차

### 3.1. 파일 복사 완료 확인
- [ ] 총 58개 파일 복사 완료 확인
- [ ] 폴더 구조 일치 확인
  - `Components/Pages/Admin/`
  - `Components/Pages/Admin/Users/`
  - `Components/Pages/Admin/Settings/Depts/`
  - `Components/Pages/Admin/Settings/Ranks/`
  - `Components/Pages/Admin/EvaluationUsers/`
  - `Components/Pages/Components/Common/`
  - `Components/Pages/Components/Modal/`
  - `Components/Pages/Components/Table/`
  - `Utils/`

### 3.2. Visual Studio 2022 빌드 테스트
- [ ] 솔루션 빌드 (Ctrl+Shift+B)
- [ ] 빌드 경고 확인
  - 예상 경고: 10개
    - RZ10012: 5개 (미구현 컴포넌트)
    - CS8601: 3개 (기존 페이지)
    - CS9113: 2개 (사용하지 않는 매개변수)
- [ ] 빌드 오류: 0개 (오류 없어야 함)

### 3.3. 네임스페이스 검증
- [ ] `MdcHR26Apps.BlazorServer.Components.Pages.Admin` 사용 확인
- [ ] `MdcHR26Apps.BlazorServer.Components.Pages.Components.*` 사용 확인
- [ ] 이전 네임스페이스 잔존 여부 확인 (Pages.Admin 등)

### 3.4. @page 디렉티브 검증
- [ ] `/Admin/Settings/Depts/*` 경로 확인
- [ ] `/Admin/Settings/Ranks/*` 경로 확인
- [ ] 이전 경로 잔존 여부 확인 (/Admin/Settings/Dept/)

### 3.5. 기능 테스트 (수동)
- [ ] 로그인 후 Admin 페이지 접근
- [ ] 기초정보 관리 (부서/직급) 목록 표시
- [ ] 사용자 관리 CRUD 작동
- [ ] 평가대상자 관리 목록 표시 (테이블 미구현 경고 무시)

---

## 4. 주의사항

### 4.1. 수동 작업 필수 항목
1. **Database 스크립트 실행**:
   - `Database/03_06_SeedData_Process.sql` 수동 실행
   - Process 테이블에 Uid 1~10 샘플 데이터 삽입

2. **Git 커밋**:
   - 실제 프로젝트에서 별도 커밋 생성
   - 커밋 메시지 예시: "sync: Phase 3-3 Admin 페이지 구조 재정리"

### 4.2. 미구현 컴포넌트 (동기화 불필요)
다음 컴포넌트는 아직 구현되지 않았으므로 **동기화하지 않음**:
- `EUserListTable.razor` (평가대상자 목록 테이블)
- `DisplayResultText.razor` (결과 메시지 표시)
- `MemberListTable.razor` (부서/직급별 사용자 목록)

**참고 문서**: [works/tasks/20260126_03_missing_components_checklist.md](../tasks/20260126_03_missing_components_checklist.md)

### 4.3. 기존 파일 백업 권장
복사 전 실제 프로젝트의 다음 파일들을 백업하는 것을 권장:
- `Data/UrlActions.cs` (경로 변경)
- `Components/_Imports.razor` (네임스페이스 추가)
- `Program.cs` (서비스 추가)

### 4.4. 네임스페이스 통일 확인
두 프로젝트 모두 `MdcHR26Apps.BlazorServer` 네임스페이스를 사용하므로 파일 그대로 복사 가능.
**단, 경로는 반드시 일치해야 함**:
- ❌ `Pages/Admin/Users/Create.razor`
- ✅ `Components/Pages/Admin/Users/Create.razor`

### 4.5. 트러블슈팅

#### 빌드 오류 발생 시
1. 솔루션 정리 (Clean Solution)
2. NuGet 패키지 복원
3. 네임스페이스 확인
4. 파일 경로 일치 여부 확인

#### 경고가 예상보다 많이 발생할 경우
- RZ10012: 미구현 컴포넌트 관련 (정상)
- CS8601: 기존 페이지의 null 참조 (정상)
- CS9113: Create 페이지의 사용하지 않는 매개변수 (정상)

#### 페이지 라우팅 오류 시
- `@page` 디렉티브 확인
- UrlActions.cs 메서드 경로 일치 여부 확인

---

## 5. 동기화 후 작업

### 5.1. 즉시 작업
- [ ] Visual Studio에서 빌드 테스트
- [ ] 로그인 후 Admin 페이지 접근 테스트
- [ ] 기본 CRUD 동작 확인

### 5.2. 다음 단계 작업
미구현 컴포넌트 구현 (우선순위 순):
1. **DisplayResultText** (가장 단순, ~10분)
2. **EUserListTable** (평가대상자 목록, ~30분)
3. **MemberListTable** (부서/직급별 사용자, ~30분)

**참고 문서**:
- [미구현 컴포넌트 체크리스트](../tasks/20260126_03_missing_components_checklist.md)
- [2026-01-26 작업 종합 정리](../20260126_work_summary.md)

---

## 6. 참고 문서

**작업지시서**:
- [20260126_01_phase3_3_admin_pages_rebuild.md](../tasks/20260126_01_phase3_3_admin_pages_rebuild.md)
- [20260126_02_restructure_blazor_project.md](../tasks/20260126_02_restructure_blazor_project.md)
- [20260126_03_missing_components_checklist.md](../tasks/20260126_03_missing_components_checklist.md)

**종합 정리**:
- [20260126_work_summary.md](../20260126_work_summary.md)

**관련 이슈**:
- [#011: Phase 3-3 Admin 페이지 빌드 오류 및 재작업](../issues/011_phase3_3_admin_pages_build_errors.md)

**참고 프로젝트**:
- 2025년 인사평가: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp`
- 도서관리 (.NET 10): `C:\Codes\36_MdcLibrary\MdcLibrary\MdcLibrary.Server`

---

**작성자**: Claude AI
**검토자**: 개발자
**승인 상태**: 동기화 대기중
