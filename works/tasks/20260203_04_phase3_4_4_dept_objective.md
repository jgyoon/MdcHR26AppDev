# 작업지시서: Phase 3-4-4 부서 목표 관리 (DeptObjective)

**날짜**: 2026-02-03
**작업 타입**: 기능 추가
**예상 소요**: 2-3시간
**관련 이슈**: [#009](../issues/009_phase3_webapp_development.md)
**Phase**: 3-4-4
**선행 작업**: [20260203_03_phase3_4_3_2nd_3rd_hr_report.md](20260203_03_phase3_4_3_2nd_3rd_hr_report.md)

---

## 1. 작업 개요

### 배경
- Phase 3-4-3 완료: 부서장/임원평가 (2nd, 3rd HR Report) 구현
- 평가 프로세스의 마지막 단계: 부서 목표 관리

### 목표
부서 목표 작성 권한이 있는 사용자가 부서의 주요 목표와 세부 목표를 등록하고 관리할 수 있도록 함

### 구현 범위
1. **DeptObjective (부서 목표 관리)**
   - Main: 주요 목표 목록 및 통계
   - Sub: 세부 목표 목록 및 통계
   - MainObjective/: 주요 목표 CRUD (Create, Edit, Delete, Details)
   - SubObjective/: 세부 목표 CRUD (Create, Edit, Delete, Details)

---

## 2. 참조 프로젝트

### 2025년 프로젝트 구조
**경로**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages\DeptObjective`

```
DeptObjective/
├── Main.razor                      # 주요 목표 목록 및 통계
├── Sub.razor                       # 세부 목표 목록 및 통계
├── MainObjective/
│   ├── Create.razor                # 주요 목표 등록
│   ├── Edit.razor                  # 주요 목표 수정
│   ├── Delete.razor                # 주요 목표 삭제
│   └── Details.razor               # 주요 목표 상세
└── SubObjective/
    ├── Create.razor                # 세부 목표 등록
    ├── Edit.razor                  # 세부 목표 수정
    ├── Delete.razor                # 세부 목표 삭제
    └── Details.razor               # 세부 목표 상세
```

---

## 3. 데이터 모델

### DeptObjectiveDb (Phase 2 완료)
**경로**: `MdcHR26Apps.Models/DeptObjectiveDb/DeptObjectiveDb.cs`

**주요 필드**:
- DeptObjectiveDbId (Int64) - PK
- EDepartId (Int64) - FK to EDepartmentDb
- ObjectiveTitle (string) - 목표 제목
- ObjectiveContents (string) - 목표 내용
- Remarks (string) - 비고

**Repository**: `IDeptObjectiveRepository`, `DeptObjectiveRepository`
- GetAllAsync()
- GetByIdAsync(DeptObjectiveDbId)
- GetByDepartmentAsync(EDepartId)
- AddAsync(deptObjective)
- UpdateAsync(deptObjective)
- DeleteAsync(DeptObjectiveDbId)

### DepartmentObjectiveDb (Phase 2 완료)
**경로**: `MdcHR26Apps.Models/DepartmentObjectiveDb/DepartmentObjectiveDb.cs`

**주요 필드**:
- DepartmentObjectiveDbId (Int64) - PK
- DeptObjectiveDbId (Int64) - FK to DeptObjectiveDb (주요 목표)
- SubObjectiveTitle (string) - 세부 목표 제목
- SubObjectiveContents (string) - 세부 목표 내용
- TargetDate (DateTime) - 목표 달성 일자
- CompletionStatus (int) - 달성 상태 (0: 미달성, 1: 진행중, 2: 완료)
- Remarks (string) - 비고

**Repository**: `IDepartmentObjectiveRepository`, `DepartmentObjectiveRepository`
- GetAllAsync()
- GetByIdAsync(DepartmentObjectiveDbId)
- GetByDeptObjectiveAsync(DeptObjectiveDbId)
- AddAsync(departmentObjective)
- UpdateAsync(departmentObjective)
- DeleteAsync(DepartmentObjectiveDbId)

### v_DeptObjectiveListDb (Phase 2 완료)
**경로**: `MdcHR26Apps.Models/Views/v_DeptObjectiveListDb/v_DeptObjectiveListDb.cs`

**View 필드**:
- DeptObjectiveDbId (Int64)
- EDepartId (Int64)
- EDepartmentName (string)
- ObjectiveTitle (string)
- ObjectiveContents (string)
- Remarks (string)

---

## 4. 부서 목표 관리 흐름

### 주요 목표 작성
1. **권한 확인**: IsDeptObjectiveWriter = true
2. **부서 선택**: 사용자의 EDepartId 또는 관리자는 전체 부서 선택 가능
3. **주요 목표 등록**: ObjectiveTitle, ObjectiveContents 입력

### 세부 목표 작성
1. **주요 목표 선택**: DeptObjectiveDbId 선택
2. **세부 목표 등록**: SubObjectiveTitle, SubObjectiveContents, TargetDate 입력
3. **진행 상태 관리**: CompletionStatus (미달성 → 진행중 → 완료)

---

## 5. 구현 페이지 목록

### DeptObjective/Main.razor
**기능**: 주요 목표 목록 및 통계 대시보드
**라우트**: `/DeptObjective/Main` 또는 `/DeptObjective`
**권한**: IsDeptObjectiveWriter = true

**주요 컴포넌트**:
- 통계 카드 (총 목표 수, 부서별 목표 수)
- SearchbarComponent (목표명, 부서명 검색)
- Table (주요 목표 목록)
- Create/Edit/Delete/Details 버튼

**데이터**:
```csharp
// 관리자: 전체 목표 조회
var objectives = await _deptObjectiveRepository.GetAllAsync();

// 일반 사용자: 본인 부서 목표만
var objectives = await _deptObjectiveRepository.GetByDepartmentAsync(currentUser.EDepartId);
```

**UI 예시**:
```razor
@* 통계 카드 *@
<div class="row mb-4">
    <div class="col-md-4">
        <div class="card text-white bg-primary">
            <div class="card-body">
                <h5 class="card-title">총 목표 수</h5>
                <h2>@totalObjectives</h2>
            </div>
        </div>
    </div>
    <div class="col-md-4">
        <div class="card text-white bg-success">
            <div class="card-body">
                <h5 class="card-title">우리 부서 목표</h5>
                <h2>@deptObjectives</h2>
            </div>
        </div>
    </div>
    <div class="col-md-4">
        <div class="card text-white bg-info">
            <div class="card-body">
                <h5 class="card-title">세부 목표 수</h5>
                <h2>@subObjectives</h2>
            </div>
        </div>
    </div>
</div>

@* 목표 목록 *@
<div class="mb-3">
    <a href="/DeptObjective/MainObjective/Create" class="btn btn-primary">새 목표 등록</a>
</div>

<table class="table table-striped">
    <thead>
        <tr>
            <th>부서명</th>
            <th>목표 제목</th>
            <th>목표 내용</th>
            <th>비고</th>
            <th>작업</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var obj in objectives)
        {
            <tr>
                <td>@obj.EDepartmentName</td>
                <td>@obj.ObjectiveTitle</td>
                <td>@obj.ObjectiveContents</td>
                <td>@obj.Remarks</td>
                <td>
                    <a href="/DeptObjective/MainObjective/Edit/@obj.DeptObjectiveDbId" class="btn btn-sm btn-primary">수정</a>
                    <a href="/DeptObjective/MainObjective/Details/@obj.DeptObjectiveDbId" class="btn btn-sm btn-info">상세</a>
                    <a href="/DeptObjective/MainObjective/Delete/@obj.DeptObjectiveDbId" class="btn btn-sm btn-danger">삭제</a>
                </td>
            </tr>
        }
    </tbody>
</table>
```

---

### DeptObjective/Sub.razor
**기능**: 세부 목표 목록 및 통계
**라우트**: `/DeptObjective/Sub`
**권한**: IsDeptObjectiveWriter = true

**주요 컴포넌트**:
- 주요 목표별 필터 드롭다운
- 진행 상태별 필터 (전체/미달성/진행중/완료)
- Table (세부 목표 목록)
- Create/Edit/Delete/Details 버튼

**데이터**:
```csharp
var subObjectives = await _departmentObjectiveRepository.GetByDeptObjectiveAsync(selectedDeptObjectiveDbId);
```

**UI 예시**:
```razor
@* 필터 *@
<div class="row mb-3">
    <div class="col-md-6">
        <label>주요 목표 선택</label>
        <select @bind="selectedDeptObjectiveDbId" class="form-select">
            <option value="0">전체</option>
            @foreach (var obj in mainObjectives)
            {
                <option value="@obj.DeptObjectiveDbId">@obj.ObjectiveTitle</option>
            }
        </select>
    </div>
    <div class="col-md-6">
        <label>진행 상태</label>
        <select @bind="selectedStatus" class="form-select">
            <option value="-1">전체</option>
            <option value="0">미달성</option>
            <option value="1">진행중</option>
            <option value="2">완료</option>
        </select>
    </div>
</div>

@* 세부 목표 목록 *@
<div class="mb-3">
    <a href="/DeptObjective/SubObjective/Create" class="btn btn-primary">새 세부 목표 등록</a>
</div>

<table class="table table-striped">
    <thead>
        <tr>
            <th>주요 목표</th>
            <th>세부 목표 제목</th>
            <th>세부 목표 내용</th>
            <th>목표 일자</th>
            <th>진행 상태</th>
            <th>작업</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var subObj in filteredSubObjectives)
        {
            <tr>
                <td>@subObj.MainObjectiveTitle</td>
                <td>@subObj.SubObjectiveTitle</td>
                <td>@subObj.SubObjectiveContents</td>
                <td>@subObj.TargetDate.ToString("yyyy-MM-dd")</td>
                <td>
                    @if (subObj.CompletionStatus == 0)
                    {
                        <span class="badge bg-secondary">미달성</span>
                    }
                    else if (subObj.CompletionStatus == 1)
                    {
                        <span class="badge bg-warning">진행중</span>
                    }
                    else
                    {
                        <span class="badge bg-success">완료</span>
                    }
                </td>
                <td>
                    <a href="/DeptObjective/SubObjective/Edit/@subObj.DepartmentObjectiveDbId" class="btn btn-sm btn-primary">수정</a>
                    <a href="/DeptObjective/SubObjective/Details/@subObj.DepartmentObjectiveDbId" class="btn btn-sm btn-info">상세</a>
                    <a href="/DeptObjective/SubObjective/Delete/@subObj.DepartmentObjectiveDbId" class="btn btn-sm btn-danger">삭제</a>
                </td>
            </tr>
        }
    </tbody>
</table>
```

---

### DeptObjective/MainObjective/Create.razor
**기능**: 주요 목표 등록
**라우트**: `/DeptObjective/MainObjective/Create`
**권한**: IsDeptObjectiveWriter = true

**Form 구조**:
```razor
<EditForm Model="@model" OnValidSubmit="HandleValidSubmit">
    <DataAnnotationsValidator />

    <div class="mb-3">
        <label class="form-label">부서 선택</label>
        @if (currentUser.IsAdministrator)
        {
            <select @bind="model.EDepartId" class="form-select" required>
                <option value="0">선택하세요</option>
                @foreach (var dept in departments)
                {
                    <option value="@dept.EDepartId">@dept.EDepartmentName</option>
                }
            </select>
        }
        else
        {
            <input type="text" class="form-control" value="@currentDepartmentName" readonly />
        }
    </div>

    <div class="mb-3">
        <label class="form-label">목표 제목 (필수)</label>
        <InputText @bind-Value="model.ObjectiveTitle" class="form-control" maxlength="200" />
        <ValidationMessage For="@(() => model.ObjectiveTitle)" />
    </div>

    <div class="mb-3">
        <label class="form-label">목표 내용 (필수)</label>
        <InputTextArea @bind-Value="model.ObjectiveContents" class="form-control" rows="5" maxlength="2000" />
        <ValidationMessage For="@(() => model.ObjectiveContents)" />
    </div>

    <div class="mb-3">
        <label class="form-label">비고 (선택)</label>
        <InputTextArea @bind-Value="model.Remarks" class="form-control" rows="3" maxlength="1000" />
    </div>

    <button type="submit" class="btn btn-primary">저장</button>
    <a href="/DeptObjective/Main" class="btn btn-secondary">취소</a>
</EditForm>
```

**저장 로직**:
```csharp
private async Task HandleValidSubmit()
{
    if (!currentUser.IsDeptObjectiveWriter)
    {
        resultMessage = "부서 목표 작성 권한이 없습니다.";
        return;
    }

    var objective = new DeptObjectiveDb
    {
        EDepartId = currentUser.IsAdministrator ? model.EDepartId : currentUser.EDepartId,
        ObjectiveTitle = model.ObjectiveTitle,
        ObjectiveContents = model.ObjectiveContents,
        Remarks = model.Remarks ?? ""
    };

    await _deptObjectiveRepository.AddAsync(objective);

    Navigation.NavigateTo("/DeptObjective/Main");
}
```

---

### DeptObjective/MainObjective/Edit.razor
**기능**: 주요 목표 수정
**라우트**: `/DeptObjective/MainObjective/Edit/{id:long}`
**권한**: IsDeptObjectiveWriter = true, 본인 부서만

---

### DeptObjective/MainObjective/Delete.razor
**기능**: 주요 목표 삭제
**라우트**: `/DeptObjective/MainObjective/Delete/{id:long}`
**권한**: IsDeptObjectiveWriter = true, 본인 부서만

**주의사항**:
- 연관된 세부 목표(DepartmentObjectiveDb) 확인
- 세부 목표가 있으면 삭제 경고 표시

---

### DeptObjective/MainObjective/Details.razor
**기능**: 주요 목표 상세 정보
**라우트**: `/DeptObjective/MainObjective/Details/{id:long}`

**주요 컴포넌트**:
- 주요 목표 정보 표시
- 연관된 세부 목표 목록 표시
- Edit/Delete 버튼

---

### DeptObjective/SubObjective/Create.razor
**기능**: 세부 목표 등록
**라우트**: `/DeptObjective/SubObjective/Create`
**권한**: IsDeptObjectiveWriter = true

**Form 구조**:
```razor
<EditForm Model="@model" OnValidSubmit="HandleValidSubmit">
    <DataAnnotationsValidator />

    <div class="mb-3">
        <label class="form-label">주요 목표 선택 (필수)</label>
        <select @bind="model.DeptObjectiveDbId" class="form-select" required>
            <option value="0">선택하세요</option>
            @foreach (var obj in mainObjectives)
            {
                <option value="@obj.DeptObjectiveDbId">@obj.ObjectiveTitle (@obj.EDepartmentName)</option>
            }
        </select>
        <ValidationMessage For="@(() => model.DeptObjectiveDbId)" />
    </div>

    <div class="mb-3">
        <label class="form-label">세부 목표 제목 (필수)</label>
        <InputText @bind-Value="model.SubObjectiveTitle" class="form-control" maxlength="200" />
        <ValidationMessage For="@(() => model.SubObjectiveTitle)" />
    </div>

    <div class="mb-3">
        <label class="form-label">세부 목표 내용 (필수)</label>
        <InputTextArea @bind-Value="model.SubObjectiveContents" class="form-control" rows="5" maxlength="2000" />
        <ValidationMessage For="@(() => model.SubObjectiveContents)" />
    </div>

    <div class="mb-3">
        <label class="form-label">목표 달성 일자 (필수)</label>
        <InputDate @bind-Value="model.TargetDate" class="form-control" />
        <ValidationMessage For="@(() => model.TargetDate)" />
    </div>

    <div class="mb-3">
        <label class="form-label">진행 상태</label>
        <select @bind="model.CompletionStatus" class="form-select">
            <option value="0">미달성</option>
            <option value="1">진행중</option>
            <option value="2">완료</option>
        </select>
    </div>

    <div class="mb-3">
        <label class="form-label">비고 (선택)</label>
        <InputTextArea @bind-Value="model.Remarks" class="form-control" rows="3" maxlength="1000" />
    </div>

    <button type="submit" class="btn btn-primary">저장</button>
    <a href="/DeptObjective/Sub" class="btn btn-secondary">취소</a>
</EditForm>
```

**저장 로직**:
```csharp
private async Task HandleValidSubmit()
{
    if (!currentUser.IsDeptObjectiveWriter)
    {
        resultMessage = "부서 목표 작성 권한이 없습니다.";
        return;
    }

    var subObjective = new DepartmentObjectiveDb
    {
        DeptObjectiveDbId = model.DeptObjectiveDbId,
        SubObjectiveTitle = model.SubObjectiveTitle,
        SubObjectiveContents = model.SubObjectiveContents,
        TargetDate = model.TargetDate,
        CompletionStatus = model.CompletionStatus,
        Remarks = model.Remarks ?? ""
    };

    await _departmentObjectiveRepository.AddAsync(subObjective);

    Navigation.NavigateTo("/DeptObjective/Sub");
}
```

---

### DeptObjective/SubObjective/Edit.razor, Delete.razor, Details.razor
**기능**: 세부 목표 수정/삭제/상세
**권한**: IsDeptObjectiveWriter = true, 본인 부서만

---

## 6. 네비게이션 메뉴 업데이트

### NavMenu.razor 수정
**경로**: `MdcHR26Apps.BlazorServer/Components/Layout/NavMenu.razor`

#### 추가할 메뉴 항목
```razor
@* 부서 목표 관리 (권한자만) *@
@if (loginStatus?.IsDeptObjectiveWriter == true)
{
    <div class="nav-item px-3">
        <NavLink class="nav-link" href="deptobjective/main">
            <span class="bi bi-bullseye" aria-hidden="true"></span> 부서 목표 관리
        </NavLink>
    </div>

    <div class="nav-item px-3">
        <NavLink class="nav-link" href="deptobjective/sub">
            <span class="bi bi-list-check" aria-hidden="true"></span> 세부 목표 관리
        </NavLink>
    </div>
}
```

---

## 7. 구현 순서

### Step 1: Main.razor 및 MainObjective CRUD (1.5시간)
1. DeptObjective/Main.razor (통계 대시보드)
2. DeptObjective/MainObjective/Create.razor
3. DeptObjective/MainObjective/Edit.razor
4. DeptObjective/MainObjective/Delete.razor
5. DeptObjective/MainObjective/Details.razor

### Step 2: Sub.razor 및 SubObjective CRUD (1.5시간)
1. DeptObjective/Sub.razor (세부 목표 목록)
2. DeptObjective/SubObjective/Create.razor
3. DeptObjective/SubObjective/Edit.razor
4. DeptObjective/SubObjective/Delete.razor
5. DeptObjective/SubObjective/Details.razor

### Step 3: 네비게이션 메뉴 업데이트 (10분)
1. NavMenu.razor 수정

### Step 4: 테스트 (30분)
1. 주요 목표 등록 시나리오
2. 세부 목표 등록 시나리오
3. 권한 체크 테스트

---

## 8. 테스트 계획

### 테스트 시나리오 1: 주요 목표 등록
1. 로그인 (IsDeptObjectiveWriter = true)
2. `/DeptObjective/Main` 접속
3. "새 목표 등록" 버튼 클릭
4. 목표 정보 입력
   - ObjectiveTitle: "고객 만족도 향상"
   - ObjectiveContents: "고객 불만 사항을 50% 감소시킨다"
5. 저장
6. **확인**: 목록에 새 목표 표시 ✅

### 테스트 시나리오 2: 세부 목표 등록
1. `/DeptObjective/Sub` 접속
2. "새 세부 목표 등록" 버튼 클릭
3. 주요 목표 선택 (드롭다운에서 "고객 만족도 향상" 선택)
4. 세부 목표 정보 입력
   - SubObjectiveTitle: "고객 응대 개선"
   - SubObjectiveContents: "고객 응대 매뉴얼 작성"
   - TargetDate: 2026-06-30
   - CompletionStatus: 진행중
5. 저장
6. **확인**: 목록에 새 세부 목표 표시 ✅

### 테스트 시나리오 3: 진행 상태 필터
1. `/DeptObjective/Sub` 접속
2. 진행 상태 드롭다운: "진행중" 선택
3. **확인**: 진행 중인 세부 목표만 표시 ✅
4. "완료" 선택
5. **확인**: 완료된 세부 목표만 표시 ✅

### 테스트 시나리오 4: 권한 체크
1. IsDeptObjectiveWriter = false 사용자로 로그인
2. `/DeptObjective/Main` 직접 접속 시도
3. **확인**: 접근 거부 또는 메인 페이지로 리다이렉트 ✅

### 테스트 시나리오 5: 주요 목표 삭제 경고
1. 세부 목표가 있는 주요 목표의 "삭제" 버튼 클릭
2. **확인**: "연관된 세부 목표가 있습니다" 경고 표시 ✅
3. 세부 목표가 없는 주요 목표 삭제 시도
4. **확인**: 정상 삭제 ✅

---

## 9. 주의사항

1. **권한 체크 필수**:
   - IsDeptObjectiveWriter = true만 접근
   - 관리자는 전체 부서 관리 가능
   - 일반 사용자는 본인 부서만

2. **데이터 무결성**:
   - 주요 목표 삭제 시 연관 세부 목표 확인
   - 세부 목표 삭제 시 주요 목표 존재 확인

3. **진행 상태 관리**:
   - CompletionStatus: 0(미달성), 1(진행중), 2(완료)
   - 세부 목표 상태 변경 시 알림 필요 (선택)

4. **날짜 유효성**:
   - TargetDate는 현재일 이후여야 함 (선택 사항)

5. **2025년 코드 참조**:
   - UI/UX는 2025년 프로젝트 참조
   - .NET 10 최신 기능 적용

---

## 10. 완료 조건

- [ ] DeptObjective/Main.razor 완료
- [ ] DeptObjective/Sub.razor 완료
- [ ] MainObjective CRUD 4개 페이지 완료
- [ ] SubObjective CRUD 4개 페이지 완료
- [ ] NavMenu.razor 메뉴 추가 완료
- [ ] 테스트 시나리오 1 성공
- [ ] 테스트 시나리오 2 성공
- [ ] 테스트 시나리오 3 성공
- [ ] 테스트 시나리오 4 성공
- [ ] 테스트 시나리오 5 성공
- [ ] 빌드 오류 0개
- [ ] 런타임 오류 0개

---

## 11. Phase 3-4 전체 완료

**Phase 3-4 완료 조건**:
- [ ] Phase 3-4-1: 직무평가 협의 완료
- [ ] Phase 3-4-2: 본인평가 완료
- [ ] Phase 3-4-3: 부서장/임원평가 완료
- [ ] Phase 3-4-4: 부서 목표 관리 완료
- [ ] 전체 통합 테스트 성공
- [ ] 이슈 #009 업데이트 및 완료 처리

---

## 12. 다음 단계

**Phase 3-5 이후**:
- 평가 프로세스 전체 통합 테스트
- UI/UX 개선
- 성능 최적화
- 프로젝트 동기화 (checklist-generator, sync-validator)

---

## 13. 관련 문서

**이슈**: [#009: Phase 3 WebApp 개발](../issues/009_phase3_webapp_development.md)
**Phase 2 Model**:
- [DeptObjectiveDb](../../MdcHR26Apps.Models/DeptObjectiveDb/)
- [DepartmentObjectiveDb](../../MdcHR26Apps.Models/DepartmentObjectiveDb/)
**선행 작업**:
- [20260203_01_phase3_4_1_agreement_subagreement.md](20260203_01_phase3_4_1_agreement_subagreement.md)
- [20260203_02_phase3_4_2_1st_hr_report.md](20260203_02_phase3_4_2_1st_hr_report.md)
- [20260203_03_phase3_4_3_2nd_3rd_hr_report.md](20260203_03_phase3_4_3_2nd_3rd_hr_report.md)
**참조 프로젝트**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages\DeptObjective`
