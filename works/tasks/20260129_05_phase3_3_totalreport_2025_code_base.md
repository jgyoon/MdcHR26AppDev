# 작업지시서: Phase 3-3 TotalReport/Admin 페이지 완성 (2025년 코드 복사 기반)

**날짜**: 2026-01-29
**작업 유형**: 기능 추가 (2025년 코드 복사 및 최소 변경)
**관련 이슈**:
- [#009: Phase 3 WebApp 개발](../issues/009_phase3_webapp_development.md)
- [#011: Phase 3-3 관리자 페이지 재작업](../issues/011_phase3_3_admin_pages_build_errors.md)
**선행 작업지시서**:
- `20260129_03_phase3_3_totalreport_admin_complete.md` (Step 1-3 완료)
- `20260129_04_fix_view_model_sync.md` (View/Model 동기화 완료 필요)

---

## ⚠️ 절대 원칙

### 1. 코드 복사 우선

**반드시 준수**:
1. ✅ **2025년 코드를 먼저 전체 복사**
2. ✅ 복사 후 **최소한의 변경만** 적용
3. ❌ **임의로 코드 축약 절대 금지**
4. ❌ **"더 효율적"이라고 판단하여 임의 수정 금지**

**작업 순서**:
```
1단계: 2025년 파일 전체 복사
2단계: 네임스페이스만 변경
3단계: 컴파일 확인
4단계: 2026년 구조 변경 사항만 최소 반영
5단계: 빌드 테스트
```

### 2. 2026년 적용 변경 사항 (최소한만)

**허용되는 변경**:
- 네임스페이스 변경: `MdcHR25Apps` → `MdcHR26Apps`
- 폴더 경로 변경: `Pages.TotalReport.Admin` → `Components.Pages.Admin.TotalReport`
- DB 타입 변경: `VARCHAR UserId` → `BIGINT Uid`
- View 이름 변경: 2025년과 다른 경우만

**금지되는 변경**:
- ❌ 로직 단순화
- ❌ 코드 리팩토링
- ❌ "더 나은" 구조로 변경
- ❌ 메서드 통합
- ❌ 불필요해 보이는 코드 삭제

### 3. 의심스러운 경우

**판단 기준**:
- "이 코드 필요 없을 것 같은데?" → ✅ **그대로 복사**
- "이렇게 하면 더 효율적인데?" → ✅ **2025년 코드 그대로 사용**
- "이 부분 축약 가능한데?" → ✅ **축약하지 말고 그대로**

---

## 1. 작업 배경

### 1.1. 이전 작업 실패 원인

**Phase 2 (View Model 생성) 실패 원인**:
- 2025년 코드를 참고하지 않고 임의로 축약
- DB View의 필드를 "필요할 것 같은 것만" 선택
- 결과: 평가 점수 필드 누락, Phase 3 작업 차단

**20260129_03 작업 중단 원인**:
- View/Model 불일치 발견
- 임의 축약으로 인한 필드 누락
- TotalReport/Admin 페이지 구현 불가

### 1.2. 이번 작업의 목표

**주 목표**:
- 2025년 TotalReport/Admin 코드를 **정확히 복사**
- 2026년 DB 구조 변경 사항만 **최소한으로** 반영
- 임의 수정 없이 동작하는 코드 완성

---

## 2. 참고 프로젝트

### 2.1. 2025년 인사평가 프로젝트

**경로**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp`

**복사 대상 폴더**:
```
Pages/
└── ViewPage/
    └── TotalReportViewPage/
        └── AdminReportListView.razor
└── TotalReport/
    └── Admin/
        ├── Index.razor
        ├── Details.razor
        ├── Edit.razor
        └── ReportInit.razor
└── Admin/
    ├── AdminViewExcel.razor
    └── AdminTaskViewExcel.razor
└── Shared/
    └── TotalReportModal/
        └── ReportInitModal.razor
```

**복사 대상 유틸리티**:
```
Utils/
├── ExcelManage.cs
└── ScoreUtils.cs (일부만, 나머지는 이미 작성됨)
```

---

## 3. 작업 단계

### Step 4: AdminReportListView 컴포넌트 작성 (2025년 복사)

**2025년 원본**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Components\ViewPage\TotalReportViewPage\AdminReportListView.razor`

**2026년 대상**: `MdcHR26Apps.BlazorServer/Components/Pages/Components/Table/AdminReportListView.razor`

**작업 순서**:

#### 4-1. 2025년 파일 전체 복사
1. 2025년 `AdminReportListView.razor` 파일 전체 내용 복사
2. 2026년 대상 위치에 붙여넣기
3. **코드 한 줄도 수정하지 않음**

#### 4-2. 네임스페이스 변경
```csharp
// 변경 전 (2025년)
@using MdcHR25Apps.Models.ProcessView

// 변경 후 (2026년)
@using MdcHR26Apps.Models.Views.v_ProcessTRListDB
```

#### 4-3. View 타입 변경
```csharp
// 변경 전 (2025년)
@code {
    [Parameter] public List<v_ProcessTRListDB>? processTRLists { get; set; }
}

// 변경 후 (2026년) - 동일 (View 이름 같음)
@code {
    [Parameter] public List<v_ProcessTRListDB>? processTRLists { get; set; }
}
```

#### 4-4. 빌드 테스트
```bash
dotnet build
```

**예상 결과**: 오류 0개

**주의**:
- 2025년 코드의 모든 HTML 구조 그대로 유지
- CSS 클래스 그대로 유지
- 로직 수정 금지
- "불필요해 보이는" 코드도 모두 유지

---

### Step 5: ExcelManage 클래스 작성 (2025년 복사)

**2025년 원본**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Utils\ExcelManage.cs`

**2026년 대상**: `MdcHR26Apps.BlazorServer/Utils/ExcelManage.cs`

**작업 순서**:

#### 5-1. 2025년 파일 전체 복사
1. 2025년 `ExcelManage.cs` 파일 전체 내용 복사
2. 2026년 위치에 새 파일 생성 후 붙여넣기

#### 5-2. 네임스페이스만 변경
```csharp
// 변경 전
namespace MdcHR25Apps.BlazorApp.Utils;

// 변경 후
namespace MdcHR26Apps.BlazorServer.Utils;
```

#### 5-3. using 문 변경
```csharp
// 변경 전
using MdcHR25Apps.Models.ProcessView;
using MdcHR25Apps.Data.Models;

// 변경 후
using MdcHR26Apps.Models.Views.v_ProcessTRListDB;
using MdcHR26Apps.Models.Views.v_TotalReportListDB;
using MdcHR26Apps.Models.Report;
```

#### 5-4. 빌드 테스트

**중요**:
- ExcelManage의 모든 메서드 그대로 유지
- 주석도 그대로 유지
- "간단하게 만들 수 있다"고 판단해도 2025년 코드 그대로 사용

---

### Step 6-8: Excel 컴포넌트 작성 (2025년 복사)

**동일한 방식으로 진행**:
1. 2025년 파일 전체 복사
2. 네임스페이스만 변경
3. 폴더 경로만 변경
4. 빌드 테스트

**파일 목록**:
- Step 6: AdminViewExcel.razor + .cs
- Step 7: AdminTaskViewExcel.razor + .cs
- Step 8: site.js에 downloadURI 함수 추가

---

### Step 9-12: TotalReport/Admin 페이지 4개 작성 (2025년 복사)

**페이지 목록**:
- Step 9: Index.razor + .cs
- Step 10: Details.razor + .cs
- Step 11: Edit.razor + .cs
- Step 12: ReportInit.razor + .cs

**작업 방식 (모든 페이지 동일)**:

#### 9-1. 2025년 파일 전체 복사
```
2025년: Pages/TotalReport/Admin/Index.razor
→ 2026년: Components/Pages/Admin/TotalReport/Index.razor
```

#### 9-2. 네임스페이스 변경
```csharp
// 변경 전
namespace MdcHR25Apps.BlazorApp.Pages.TotalReport.Admin;

// 변경 후
namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin.TotalReport;
```

#### 9-3. @page 경로 확인
```csharp
// 2025년과 2026년 동일
@page "/Admin/TotalReport"
@page "/Admin/TotalReport/Details/{pid:long}"
```

#### 9-4. DB 타입 변경 (필요 시만)
```csharp
// 변경 전 (2025년에서 UserId를 사용한 경우)
var user = await userRepository.GetByUserIdAsync(userId);

// 변경 후 (2026년은 Uid 사용)
var user = await userRepository.GetByIdAsync(uid);
```

**주의**:
- Repository 메서드 시그니처가 2025년과 다를 수 있음
- 하지만 로직은 그대로 유지
- 메서드 호출만 2026년 Repository에 맞게 변경

---

### Step 13: ReportInitModal 컴포넌트 작성 (2025년 복사)

**2025년 원본**: `Shared/TotalReportModal/ReportInitModal.razor`

**2026년 대상**: `Components/Pages/Components/Modal/ReportInitModal.razor`

**동일한 방식으로 복사 및 최소 변경**

---

### Step 14: Repository 메서드 추가 (2025년 참고)

**작업**:
1. 2025년 Repository 메서드 확인
2. 2026년 Repository에 동일한 시그니처로 추가
3. Dapper 쿼리 복사

**파일**:
- `v_ProcessTRListRepository.cs`
- `v_TotalReportListRepository.cs`
- `TotalReportDbRepository.cs`

**예시**:
```csharp
// 2025년 메서드 복사
public async Task<List<v_ProcessTRListDB>> GetAllWithScoresAsync()
{
    // 2025년 쿼리 그대로 복사
    const string sql = @"
        SELECT * FROM v_ProcessTRListDB
        ORDER BY Pid DESC
    ";

    return (await _connection.QueryAsync<v_ProcessTRListDB>(sql)).AsList();
}
```

---

## 4. 변경 사항 체크리스트

### 허용되는 변경만 체크

각 파일 작업 시 이 체크리스트를 확인:

- [ ] 2025년 파일 전체 복사 완료
- [ ] 네임스페이스 변경 (MdcHR25Apps → MdcHR26Apps)
- [ ] 폴더 경로 변경 (Pages → Components/Pages)
- [ ] using 문 업데이트
- [ ] Repository 메서드 시그니처 확인
- [ ] 빌드 테스트 성공
- [ ] **임의 수정 없음 확인** ✅

### 금지된 변경 체크

다음 중 하나라도 했다면 **잘못된 작업**:

- [ ] 로직 간소화
- [ ] 메서드 통합
- [ ] 코드 리팩토링
- [ ] "불필요한" 코드 삭제
- [ ] HTML 구조 변경
- [ ] CSS 클래스 변경

---

## 5. 테스트 계획

### Test 1: 각 Step 완료 후 빌드
1. Step 완료 시마다 `dotnet build` 실행
2. **확인**: 오류 0개

### Test 2: 전체 빌드
1. 모든 Step 완료 후 `dotnet build`
2. **확인**: 오류 0개

### Test 3: 기능 테스트 (개발자)
1. Admin/TotalReport 페이지 접근
2. 전체 평가 목록 조회
3. 상세 보기, 수정, 초기화 기능 테스트

---

## 6. 완료 조건

**Step 완료**:
- [ ] Step 4: AdminReportListView (2025년 복사)
- [ ] Step 5: ExcelManage (2025년 복사)
- [ ] Step 6: AdminViewExcel (2025년 복사)
- [ ] Step 7: AdminTaskViewExcel (2025년 복사)
- [ ] Step 8: site.js downloadURI (2025년 복사)
- [ ] Step 9: Index.razor (2025년 복사)
- [ ] Step 10: Details.razor (2025년 복사)
- [ ] Step 11: Edit.razor (2025년 복사)
- [ ] Step 12: ReportInit.razor (2025년 복사)
- [ ] Step 13: ReportInitModal (2025년 복사)
- [ ] Step 14: Repository 메서드 추가 (2025년 참고)

**검증**:
- [ ] 모든 파일이 2025년 원본 기반
- [ ] 임의 수정 없음
- [ ] 빌드 성공 (오류 0개)
- [ ] Phase 3-3 완료

---

## 7. 재발 방지

### 원칙 재확인

**금번 작업 원칙**:
1. 2025년 코드 = 진리의 원천
2. 복사 우선, 수정 최소
3. 임의 판단 금지

**향후 모든 작업 적용**:
- View Model 생성 시: DB View SQL = 진리의 원천
- 페이지 작성 시: 2025년 코드 = 진리의 원천
- 컴포넌트 작성 시: 2025년 코드 = 진리의 원천

---

**작업 시작일**: 2026-01-29
**예상 소요 시간**: 4-5시간
**우선순위**: 높음 (Phase 3-3 완료 필수)
**전제 조건**: 20260129_04 (View/Model 동기화) 완료
