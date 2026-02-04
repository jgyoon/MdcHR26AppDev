# 작업지시서: Phase 3-4-1 직무평가 협의 (Agreement, SubAgreement)

**날짜**: 2026-02-03
**작업 타입**: 기능 추가
**예상 소요**: 3-4시간
**관련 이슈**: [#009](../issues/009_phase3_webapp_development.md)
**Phase**: 3-4-1

---

## 1. 작업 개요

### 배경
- Phase 3-3 완료: 관리자 페이지 전체 구현 완료
- Phase 3-4 시작: 평가 프로세스 구현
- 평가 프로세스의 첫 단계: 직무평가 협의

### 목표
직무평가 협의 프로세스를 구현하여 사용자가 본인의 직무 및 세부 업무를 등록하고 부서장이 승인할 수 있도록 함

### 구현 범위
1. **Agreement (직무평가 협의)**
   - User: 사용자용 직무 등록 (Index, Create, Edit, Delete, Details)
   - TeamLeader: 부서장용 직무 승인 (Index, Details)

2. **SubAgreement (세부직무평가)**
   - User: 사용자용 세부 업무 등록 (Index, Create, Edit, Delete, Details)
   - TeamLeader: 부서장용 세부 업무 승인/조정 (Index, Details, SubDetails, CompleteSubAgreement, ResetSubAgreement)

---

## 2. 참조 프로젝트

### 2025년 프로젝트 구조
**경로**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages`

#### Agreement 구조
```
Agreement/
├── User/
│   ├── Index.razor       # 직무 목록
│   ├── Create.razor      # 직무 등록
│   ├── Edit.razor        # 직무 수정
│   ├── Delete.razor      # 직무 삭제
│   └── Details.razor     # 직무 상세
└── TeamLeader/
    ├── Index.razor       # 부서원 직무 목록
    └── Details.razor     # 부서원 직무 상세
```

#### SubAgreement 구조
```
SubAgreement/
├── User/
│   ├── Index.razor       # 세부 업무 목록
│   ├── Create.razor      # 세부 업무 등록
│   ├── Edit.razor        # 세부 업무 수정
│   ├── Delete.razor      # 세부 업무 삭제
│   └── Details.razor     # 세부 업무 상세
└── TeamLeader/
    ├── Index.razor                 # 부서원 세부 업무 목록
    ├── Details.razor               # 부서원 세부 업무 상세
    ├── SubDetails.razor            # 세부 업무 상세 (조정용)
    ├── CompleteSubAgreement.razor  # 세부 업무 승인 완료
    └── ResetSubAgreement.razor     # 세부 업무 초기화
```

---

## 3. 데이터 모델

### AgreementDb (Phase 2 완료)
**경로**: `MdcHR26Apps.Models/AgreementDb/AgreementDb.cs`

**주요 필드**:
- Aid (Int64) - PK
- Uid (Int64) - FK to UserDb
- Item_Number (int) - 직무 번호
- Item_Title (string) - 직무 제목
- Item_Contents (string) - 직무 내용
- Item_Proportion (int) - 비중 (%)

**Repository**: `IAgreementRepository`, `AgreementRepository`
- GetAllAsync()
- GetByIdAsync(Aid)
- GetByUserIdAsync(Uid)
- AddAsync(agreement)
- UpdateAsync(agreement)
- DeleteAsync(Aid)

### SubAgreementDb (Phase 2 완료)
**경로**: `MdcHR26Apps.Models/SubAgreementDb/SubAgreementDb.cs`

**주요 필드**:
- Said (Int64) - PK
- Aid (Int64) - FK to AgreementDb
- Uid (Int64) - FK to UserDb
- Pid (Int64) - FK to ProcessDb
- Sub_Agreement_Number (int) - 세부 업무 번호
- TaskName (string) - 업무명
- TaskObjective (string) - 업무 목표
- Sub_Agreement_Proportion (int) - 비중 (%)
- Completed_Number (int) - 완료 상태

**Repository**: `ISubAgreementRepository`, `SubAgreementRepository`
- GetAllAsync()
- GetByIdAsync(Said)
- GetByAgreementIdAsync(Aid)
- GetByUserIdAsync(Uid)
- GetByProcessIdAsync(Pid)
- AddAsync(subAgreement)
- UpdateAsync(subAgreement)
- DeleteAsync(Said)

---

## 4. 구현 페이지 목록

### Agreement/User (사용자용) - 5개 페이지

#### 1. Agreement/User/Index.razor
**기능**: 로그인한 사용자의 직무 목록 표시
**주요 컴포넌트**:
- SearchbarComponent (검색)
- Table (직무 목록)
- Create/Edit/Delete/Details 버튼

**데이터**:
- `GetByUserIdAsync(currentUser.Uid)` 사용
- 직무 번호, 제목, 내용, 비중 표시

---

#### 2. Agreement/User/Create.razor
**기능**: 새 직무 등록
**주요 컴포넌트**:
- Form (Item_Number, Item_Title, Item_Contents, Item_Proportion)
- 저장/취소 버튼

**유효성 검사**:
- Item_Number: 필수, 중복 불가
- Item_Title: 필수, 200자 이내
- Item_Contents: 필수, 2000자 이내
- Item_Proportion: 필수, 0-100

**저장 로직**:
```csharp
var agreement = new AgreementDb
{
    Uid = currentUser.Uid,
    Item_Number = model.Item_Number,
    Item_Title = model.Item_Title,
    Item_Contents = model.Item_Contents,
    Item_Proportion = model.Item_Proportion
};
await _repository.AddAsync(agreement);
```

---

#### 3. Agreement/User/Edit.razor
**기능**: 기존 직무 수정
**라우트**: `/Agreement/User/Edit/{aid:long}`

**주요 컴포넌트**:
- Form (기존 데이터 로드)
- 저장/취소 버튼

**권한 체크**:
- 본인의 직무만 수정 가능 (agreement.Uid == currentUser.Uid)

---

#### 4. Agreement/User/Delete.razor
**기능**: 직무 삭제
**라우트**: `/Agreement/User/Delete/{aid:long}`

**주요 컴포넌트**:
- 삭제 확인 모달
- 직무 정보 표시 (읽기 전용)

**삭제 조건**:
- 본인의 직무만 삭제 가능
- 연관된 SubAgreement 확인 후 경고

---

#### 5. Agreement/User/Details.razor
**기능**: 직무 상세 정보 표시
**라우트**: `/Agreement/User/Details/{aid:long}`

**주요 컴포넌트**:
- 직무 정보 표시 (읽기 전용)
- Edit/Delete 버튼

---

### Agreement/TeamLeader (부서장용) - 2개 페이지

#### 1. Agreement/TeamLeader/Index.razor
**기능**: 부서원들의 직무 목록 표시
**권한**: IsTeamLeader = true

**주요 컴포넌트**:
- SearchbarComponent (사용자명, 직무명 검색)
- Table (부서원별 직무 목록)

**데이터**:
- 같은 부서(EDepartId) 사용자들의 직무 조회
- UserName, Item_Title, Item_Proportion 표시

---

#### 2. Agreement/TeamLeader/Details.razor
**기능**: 부서원의 직무 상세 정보
**라우트**: `/Agreement/TeamLeader/Details/{aid:long}`

**주요 컴포넌트**:
- 직무 정보 표시 (읽기 전용)
- 사용자 정보 표시
- 승인/반려 버튼 (필요시)

---

### SubAgreement/User (사용자용) - 5개 페이지

#### 1. SubAgreement/User/Index.razor
**기능**: 로그인한 사용자의 세부 업무 목록
**주요 컴포넌트**:
- 직무별 그룹핑 (AgreementDb 조인)
- SearchbarComponent
- Table (세부 업무 목록)

**데이터**:
- `GetByUserIdAsync(currentUser.Uid)` 사용
- 직무명, 업무명, 목표, 비중, 완료 상태 표시

---

#### 2. SubAgreement/User/Create.razor
**기능**: 새 세부 업무 등록
**주요 컴포넌트**:
- Form (Aid 선택, Sub_Agreement_Number, TaskName, TaskObjective, Sub_Agreement_Proportion)
- 직무 선택 드롭다운 (사용자의 Agreement 목록)

**유효성 검사**:
- Aid: 필수 (사용자의 직무 중 선택)
- Sub_Agreement_Number: 필수, 직무 내 중복 불가
- TaskName: 필수, 200자 이내
- TaskObjective: 필수, 2000자 이내
- Sub_Agreement_Proportion: 필수, 0-100

**저장 로직**:
```csharp
var subAgreement = new SubAgreementDb
{
    Aid = model.Aid,
    Uid = currentUser.Uid,
    Pid = currentProcess.Pid, // 현재 프로세스
    Sub_Agreement_Number = model.Sub_Agreement_Number,
    TaskName = model.TaskName,
    TaskObjective = model.TaskObjective,
    Sub_Agreement_Proportion = model.Sub_Agreement_Proportion,
    Completed_Number = 0 // 초기값
};
await _repository.AddAsync(subAgreement);
```

---

#### 3. SubAgreement/User/Edit.razor
**기능**: 기존 세부 업무 수정
**라우트**: `/SubAgreement/User/Edit/{said:long}`

**권한 체크**:
- 본인의 세부 업무만 수정 가능
- Completed_Number가 2 이상이면 수정 불가 (부서장 승인 완료)

---

#### 4. SubAgreement/User/Delete.razor
**기능**: 세부 업무 삭제
**라우트**: `/SubAgreement/User/Delete/{said:long}`

**삭제 조건**:
- 본인의 세부 업무만 삭제 가능
- Completed_Number가 2 이상이면 삭제 불가

---

#### 5. SubAgreement/User/Details.razor
**기능**: 세부 업무 상세 정보
**라우트**: `/SubAgreement/User/Details/{said:long}`

**주요 컴포넌트**:
- 세부 업무 정보 표시
- 관련 직무 정보 표시 (AgreementDb 조인)
- Edit/Delete 버튼

---

### SubAgreement/TeamLeader (부서장용) - 5개 페이지

#### 1. SubAgreement/TeamLeader/Index.razor
**기능**: 부서원들의 세부 업무 목록
**권한**: IsTeamLeader = true

**주요 컴포넌트**:
- SearchbarComponent (사용자명, 업무명 검색)
- Table (부서원별 세부 업무 목록)
- 완료 상태 필터 (전체/미완료/승인완료)

**데이터**:
- 같은 부서 사용자들의 세부 업무 조회
- UserName, TaskName, TaskObjective, Completed_Number 표시

---

#### 2. SubAgreement/TeamLeader/Details.razor
**기능**: 부서원의 세부 업무 상세
**라우트**: `/SubAgreement/TeamLeader/Details/{said:long}`

**주요 컴포넌트**:
- 세부 업무 정보 표시
- 사용자 정보 표시
- 조정/승인 버튼

---

#### 3. SubAgreement/TeamLeader/SubDetails.razor
**기능**: 세부 업무 조정 (부서장이 내용 수정 가능)
**라우트**: `/SubAgreement/TeamLeader/SubDetails/{said:long}`

**주요 컴포넌트**:
- Form (TaskName, TaskObjective, Sub_Agreement_Proportion 수정 가능)
- 저장 버튼

**로직**:
- 부서장이 부서원의 세부 업무 내용을 조정
- 저장 후 Completed_Number = 1 (조정 완료)

---

#### 4. SubAgreement/TeamLeader/CompleteSubAgreement.razor
**기능**: 세부 업무 승인 완료
**라우트**: `/SubAgreement/TeamLeader/CompleteSubAgreement/{said:long}`

**주요 컴포넌트**:
- 승인 확인 모달
- 세부 업무 정보 표시

**로직**:
```csharp
var subAgreement = await _repository.GetByIdAsync(said);
subAgreement.Completed_Number = 2; // 승인 완료
await _repository.UpdateAsync(subAgreement);
```

---

#### 5. SubAgreement/TeamLeader/ResetSubAgreement.razor
**기능**: 세부 업무 초기화 (승인 취소)
**라우트**: `/SubAgreement/TeamLeader/ResetSubAgreement/{said:long}`

**주요 컴포넌트**:
- 초기화 확인 모달
- 세부 업무 정보 표시

**로직**:
```csharp
var subAgreement = await _repository.GetByIdAsync(said);
subAgreement.Completed_Number = 0; // 초기화
await _repository.UpdateAsync(subAgreement);
```

---

## 5. 네비게이션 메뉴 업데이트

### NavMenu.razor 수정
**경로**: `MdcHR26Apps.BlazorServer/Components/Layout/NavMenu.razor`

#### 추가할 메뉴 항목
```razor
@* 평가 프로세스 *@
<div class="nav-item px-3">
    <NavLink class="nav-link" href="agreement/user">
        <span class="bi bi-clipboard-check-fill" aria-hidden="true"></span> 직무평가 협의
    </NavLink>
</div>

<div class="nav-item px-3">
    <NavLink class="nav-link" href="subagreement/user">
        <span class="bi bi-list-task" aria-hidden="true"></span> 세부직무평가
    </NavLink>
</div>

@* 부서장 전용 메뉴 *@
@if (loginStatus?.IsTeamLeader == true)
{
    <div class="nav-item px-3">
        <NavLink class="nav-link" href="agreement/teamleader">
            <span class="bi bi-people-fill" aria-hidden="true"></span> 부서원 직무 확인
        </NavLink>
    </div>

    <div class="nav-item px-3">
        <NavLink class="nav-link" href="subagreement/teamleader">
            <span class="bi bi-check2-square" aria-hidden="true"></span> 부서원 세부 업무 승인
        </NavLink>
    </div>
}
```

---

## 6. 공통 컴포넌트 활용

### 재사용 가능한 컴포넌트
1. **SearchbarComponent** (이미 구현됨)
   - 검색 기능에 사용

2. **DisplayResultText** (이미 구현됨)
   - 저장/삭제 후 결과 메시지 표시

3. **Modal 컴포넌트** (필요시 신규 작성)
   - AgreementDeleteModal
   - SubAgreementDeleteModal
   - CompleteConfirmModal
   - ResetConfirmModal

---

## 7. 라우팅 설정

### Program.cs 확인
기존 라우팅 설정 확인 (이미 구현되어 있을 것으로 예상)

```csharp
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
```

---

## 8. 구현 순서

### Step 1: Agreement 구현 (1시간)
1. Agreement/User/Index.razor
2. Agreement/User/Create.razor
3. Agreement/User/Edit.razor
4. Agreement/User/Delete.razor
5. Agreement/User/Details.razor
6. Agreement/TeamLeader/Index.razor
7. Agreement/TeamLeader/Details.razor

### Step 2: SubAgreement 구현 (2시간)
1. SubAgreement/User/Index.razor
2. SubAgreement/User/Create.razor
3. SubAgreement/User/Edit.razor
4. SubAgreement/User/Delete.razor
5. SubAgreement/User/Details.razor
6. SubAgreement/TeamLeader/Index.razor
7. SubAgreement/TeamLeader/Details.razor
8. SubAgreement/TeamLeader/SubDetails.razor
9. SubAgreement/TeamLeader/CompleteSubAgreement.razor
10. SubAgreement/TeamLeader/ResetSubAgreement.razor

### Step 3: 네비게이션 메뉴 업데이트 (10분)
1. NavMenu.razor 수정

### Step 4: 테스트 (30분)
1. 사용자 시나리오 테스트
2. 부서장 시나리오 테스트
3. 권한 체크 테스트

---

## 9. 테스트 계획

### 테스트 시나리오 1: 사용자 직무 등록
1. 로그인 (일반 사용자)
2. `/Agreement/User` 접속
3. "직무 등록" 버튼 클릭
4. 직무 정보 입력 (Item_Number: 1, Item_Title: "고객 응대", Item_Proportion: 30)
5. 저장 버튼 클릭
6. **확인**: 목록에 새 직무 표시 ✅

### 테스트 시나리오 2: 사용자 세부 업무 등록
1. `/SubAgreement/User` 접속
2. "세부 업무 등록" 버튼 클릭
3. 직무 선택 (드롭다운에서 "고객 응대" 선택)
4. 세부 업무 정보 입력
5. 저장 버튼 클릭
6. **확인**: 목록에 새 세부 업무 표시 ✅

### 테스트 시나리오 3: 부서장 승인
1. 로그아웃 후 부서장 계정으로 로그인
2. `/SubAgreement/TeamLeader` 접속
3. 부서원의 세부 업무 선택
4. "승인" 버튼 클릭
5. **확인**: Completed_Number = 2로 업데이트 ✅

### 테스트 시나리오 4: 권한 체크
1. 일반 사용자로 로그인
2. `/Agreement/TeamLeader` 직접 접속 시도
3. **확인**: 접근 거부 또는 메인 페이지로 리다이렉트 ✅

### 테스트 시나리오 5: 비중 합계 검증
1. 사용자가 직무 3개 등록 (각 30%, 40%, 20%)
2. **확인**: 비중 합계 90% 경고 표시 (100%가 아님) ⚠️
3. 추가 직무 등록 (10%)
4. **확인**: 비중 합계 100% 정상 표시 ✅

---

## 10. 주의사항

1. **권한 체크 필수**:
   - Agreement/TeamLeader, SubAgreement/TeamLeader는 IsTeamLeader = true만 접근
   - Edit/Delete는 본인의 데이터만 가능

2. **비중 검증**:
   - Agreement: 전체 직무 비중 합계 100% 권장
   - SubAgreement: 각 직무별 세부 업무 비중 합계 100% 권장

3. **Completed_Number 상태 관리**:
   - 0: 미작성
   - 1: 부서장 조정 완료
   - 2: 부서장 승인 완료

4. **ProcessDb 연동**:
   - SubAgreement 생성 시 현재 ProcessDb.Pid 필요
   - ProcessDb가 없으면 에러 처리

5. **2025년 코드 참조**:
   - 로직 구조는 2025년 프로젝트 참조
   - UI는 .NET 10 / Bootstrap 5 최신 스타일 적용

---

## 11. 완료 조건

- [ ] Agreement/User 5개 페이지 완료
- [ ] Agreement/TeamLeader 2개 페이지 완료
- [ ] SubAgreement/User 5개 페이지 완료
- [ ] SubAgreement/TeamLeader 5개 페이지 완료
- [ ] NavMenu.razor 메뉴 추가 완료
- [ ] 테스트 시나리오 1 성공
- [ ] 테스트 시나리오 2 성공
- [ ] 테스트 시나리오 3 성공
- [ ] 테스트 시나리오 4 성공
- [ ] 테스트 시나리오 5 성공
- [ ] 빌드 오류 0개
- [ ] 런타임 오류 0개

---

## 12. 다음 단계

**Phase 3-4-2**: 본인평가 (1st_HR_Report) 구현
- 작업지시서: `20260203_02_phase3_4_2_1st_hr_report.md`

---

## 13. 관련 문서

**이슈**: [#009: Phase 3 WebApp 개발](../issues/009_phase3_webapp_development.md)
**Phase 2 Model**:
- [AgreementDb](../../MdcHR26Apps.Models/AgreementDb/)
- [SubAgreementDb](../../MdcHR26Apps.Models/SubAgreementDb/)
**참조 프로젝트**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Pages`
