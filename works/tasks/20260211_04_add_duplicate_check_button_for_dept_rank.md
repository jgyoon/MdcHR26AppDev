# 작업지시서: 부서/직급 생성 페이지에 중복체크 버튼 추가

**날짜**: 2026-02-11
**작업 순번**: 20260211_04
**담당**: Claude AI
**상태**: 작성 완료

---

## 1. 작업 개요

### 배경
- 부서/직급 생성 시 저장 버튼 클릭 시에만 중복 체크 수행 (20260211_03 완료)
- 사용자 등록 페이지에는 "중복체크" 버튼이 있어 저장 전 미리 확인 가능
- 부서/직급 생성 페이지에도 동일한 UX 기능 추가 필요

### 목표
부서/직급 생성 페이지에 사용자 등록과 동일한 "중복체크" 버튼 및 실시간 검증 기능 추가

### 범위
- **대상 페이지**: Depts/Create, Ranks/Create
- **추가 기능**: "중복체크" 버튼, 실시간 메시지 표시
- **참고 페이지**: Users/Create (Line 25-27, Line 143-159)

---

## 2. 참고 구현 (Users/Create)

### UI 구조 (Create.razor Line 21-27)

```razor
<label for="UserId" class="form-label mb-1">ID</label>
<div class="input-group mb-1">
    <InputText id="UserId" class="form-control" placeholder="Enter UserId"
               @bind-Value="@model.UserId"></InputText>
    <button id="checkbutton" class="btn btn-primary" @onclick="@(() => CheckUserId(model.UserId))">중복체크</button>
</div>
<div class="mb-1" style="@resultUserColor">@resultUser</div>
```

### 코드 비하인드 (Create.razor.cs Line 36-38, Line 143-159)

```csharp
// 필드
public string resultUser { get; set; } = String.Empty;
public string resultUserColor { get; set; } = String.Empty;

// 메서드
private async Task CheckUserId(string id)
{
    if (string.IsNullOrEmpty(id))
        return;

    if (!await userRepository.UserIdCheckAsync(id))
    {
        resultUser = "사용가능한 ID입니다.";
        resultUserColor = "color:blue";
    }
    else
    {
        resultUser = "같은 ID가 존재합니다.";
        resultUserColor = "color:red";
    }
}
```

---

## 3. 작업 단계

### Step 1: Depts/Create.razor UI 수정

**파일**: `MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Depts/Create.razor`

**수정 위치**: Line 24-28 (부서번호 입력 필드)

**현재 코드**:
```razor
<label for="EDepartmentNo" class="form-label mb-1">부서번호</label>
<div class="input-group mb-1">
    <InputNumber id="EDepartmentNo" class="form-control"
                 @bind-Value="@model.EDepartmentNo" />
</div>
```

**변경 후**:
```razor
<label for="EDepartmentNo" class="form-label mb-1">부서번호</label>
<div class="input-group mb-1">
    <InputNumber id="EDepartmentNo" class="form-control"
                 @bind-Value="@model.EDepartmentNo" />
    <button type="button" class="btn btn-primary"
            @onclick="@(() => CheckDepartmentNo(model.EDepartmentNo))">중복체크</button>
</div>
<div class="mb-1" style="@resultDeptNoColor">@resultDeptNo</div>
```

**변경 사항**:
- 중복체크 버튼 추가 (`type="button"` 필수 - submit 방지)
- 결과 메시지 div 추가 (색상 동적 변경)

---

### Step 2: Depts/Create.razor.cs 메서드 추가

**파일**: `MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Depts/Create.razor.cs`

**위치 1**: Line 12-13 뒤 (필드 선언)

**추가 코드**:
```csharp
private string resultDeptNo { get; set; } = string.Empty;
private string resultDeptNoColor { get; set; } = string.Empty;
```

**위치 2**: Line 60 뒤 (MoveSettingManagePage 메서드 region 뒤)

**추가 코드**:
```csharp
#region Department Number Check
private async Task CheckDepartmentNo(int deptNo)
{
    // 부서번호가 1 미만이면 return
    if (deptNo < 1)
        return;

    var existing = await eDepartmentRepository.GetByDepartmentNoAsync(deptNo);
    if (existing == null)
    {
        resultDeptNo = "사용가능한 부서번호입니다.";
        resultDeptNoColor = "color:blue";
    }
    else
    {
        resultDeptNo = "이미 사용 중인 부서번호입니다.";
        resultDeptNoColor = "color:red";
    }
}
#endregion
```

---

### Step 3: Ranks/Create.razor UI 수정

**파일**: `MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Ranks/Create.razor`

**수정 위치**: Line 24-28 (직급번호 입력 필드)

**현재 코드**:
```razor
<label for="ERankNo" class="form-label mb-1">직급번호</label>
<div class="input-group mb-1">
    <InputNumber id="ERankNo" class="form-control"
                 @bind-Value="@model.ERankNo" />
</div>
```

**변경 후**:
```razor
<label for="ERankNo" class="form-label mb-1">직급번호</label>
<div class="input-group mb-1">
    <InputNumber id="ERankNo" class="form-control"
                 @bind-Value="@model.ERankNo" />
    <button type="button" class="btn btn-primary"
            @onclick="@(() => CheckRankNo(model.ERankNo))">중복체크</button>
</div>
<div class="mb-1" style="@resultRankNoColor">@resultRankNo</div>
```

**변경 사항**:
- 중복체크 버튼 추가 (`type="button"` 필수)
- 결과 메시지 div 추가 (색상 동적 변경)

---

### Step 4: Ranks/Create.razor.cs 메서드 추가

**파일**: `MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Ranks/Create.razor.cs`

**위치 1**: Line 12-13 뒤 (필드 선언)

**추가 코드**:
```csharp
private string resultRankNo { get; set; } = string.Empty;
private string resultRankNoColor { get; set; } = string.Empty;
```

**위치 2**: Line 63 뒤 (MoveSettingManagePage 메서드 region 뒤)

**추가 코드**:
```csharp
#region Rank Number Check
private async Task CheckRankNo(int rankNo)
{
    // 직급번호가 1 미만이면 return
    if (rankNo < 1)
        return;

    var existing = await eRankRepository.GetByRankNoAsync(rankNo);
    if (existing == null)
    {
        resultRankNo = "사용가능한 직급번호입니다.";
        resultRankNoColor = "color:blue";
    }
    else
    {
        resultRankNo = "이미 사용 중인 직급번호입니다.";
        resultRankNoColor = "color:red";
    }
}
#endregion
```

---

## 4. UI/UX 플로우

### 부서 생성 시나리오

1. Admin 로그인 → Admin/SettingManage → "부서 관리" 탭
2. "부서생성" 클릭
3. 부서번호: 100 입력
4. **"중복체크" 버튼 클릭**
5. **결과 표시**:
   - 사용 가능: "사용가능한 부서번호입니다." (파란색) ✅
   - 중복: "이미 사용 중인 부서번호입니다." (빨간색) ❌
6. 부서명, 사용여부, 비고 입력
7. "저장" 버튼 클릭 → 기존 로직 동작 (중복 재확인)

### 직급 생성 시나리오

1. Admin 로그인 → Admin/SettingManage → "직급 관리" 탭
2. "직급생성" 클릭
3. 직급번호: 10 입력
4. **"중복체크" 버튼 클릭**
5. **결과 표시**:
   - 사용 가능: "사용가능한 직급번호입니다." (파란색) ✅
   - 중복: "이미 사용 중인 직급번호입니다." (빨간색) ❌
6. 직급명, 사용여부, 비고 입력
7. "저장" 버튼 클릭 → 기존 로직 동작 (중복 재확인)

---

## 5. 기술 세부사항

### type="button" 필수

```razor
<button type="button" class="btn btn-primary" ...>중복체크</button>
```

- `type="button"` 명시 필수
- 미지정 시 기본값 `type="submit"`으로 인식되어 폼 제출 발생
- EditForm 내부에서는 반드시 명시해야 함

### 파라미터 타입

```csharp
private async Task CheckDepartmentNo(int deptNo)  // int 타입
```

- `model.EDepartmentNo`는 `int` 타입
- `model.ERankNo`도 `int` 타입
- 1 미만 값은 유효하지 않으므로 early return

### 기존 SaveDepartment/SaveRank 로직 유지

- 중복체크 버튼은 **사전 확인용**
- 저장 시에도 중복 체크 재수행 (20260211_03 로직 유지)
- 이중 검증으로 데이터 무결성 보장

---

## 6. 테스트 시나리오

### 시나리오 1: 부서번호 중복체크 - 사용가능

1. 부서 생성 페이지 이동
2. 부서번호: 999 입력 (존재하지 않는 번호)
3. "중복체크" 버튼 클릭
4. **확인**: "사용가능한 부서번호입니다." (파란색) ✅

### 시나리오 2: 부서번호 중복체크 - 중복

1. 부서 생성 페이지 이동
2. 부서번호: 1 입력 (이미 존재하는 번호)
3. "중복체크" 버튼 클릭
4. **확인**: "이미 사용 중인 부서번호입니다." (빨간색) ✅

### 시나리오 3: 직급번호 중복체크 - 사용가능

1. 직급 생성 페이지 이동
2. 직급번호: 99 입력 (존재하지 않는 번호)
3. "중복체크" 버튼 클릭
4. **확인**: "사용가능한 직급번호입니다." (파란색) ✅

### 시나리오 4: 직급번호 중복체크 - 중복

1. 직급 생성 페이지 이동
2. 직급번호: 10 입력 (이미 존재하는 번호)
3. "중복체크" 버튼 클릭
4. **확인**: "이미 사용 중인 직급번호입니다." (빨간색) ✅

### 시나리오 5: 중복체크 후 저장 (이중 검증)

1. 부서 생성 페이지 이동
2. 부서번호: 999, 중복체크 → "사용가능" ✅
3. 부서명: "테스트부서" 입력
4. "저장" 클릭
5. **확인**: 부서 생성 성공 후 목록 페이지 이동 ✅

### 시나리오 6: 0 또는 음수 입력 시

1. 부서 생성 페이지 이동
2. 부서번호: 0 또는 음수 입력
3. "중복체크" 버튼 클릭
4. **확인**: 아무 메시지도 표시되지 않음 (early return) ✅

---

## 7. 완료 조건

- [x] Depts/Create.razor에 중복체크 버튼 추가
- [x] Depts/Create.razor.cs에 CheckDepartmentNo() 메서드 추가
- [x] Ranks/Create.razor에 중복체크 버튼 추가
- [x] Ranks/Create.razor.cs에 CheckRankNo() 메서드 추가
- [x] 빌드 성공 (0 오류)
- [x] 중복체크 기능 정상 동작 (파란색/빨간색 메시지)
- [x] 저장 시 기존 로직 정상 동작 (이중 검증)

---

## 8. 주의사항

### EditForm 내부 버튼 타입

```razor
<!-- 잘못된 예 -->
<button class="btn btn-primary" @onclick="...">중복체크</button>
<!-- EditForm이 submit으로 인식 ❌ -->

<!-- 올바른 예 -->
<button type="button" class="btn btn-primary" @onclick="...">중복체크</button>
<!-- submit 방지 ✅ -->
```

### 기존 저장 로직 유지

- SaveDepartment() / SaveRank() 메서드의 중복 체크 로직 **유지**
- 중복체크 버튼은 UX 개선용 (사전 확인)
- 저장 시 최종 검증은 여전히 필요

### resultText vs resultDeptNo/resultRankNo

- `resultText`: 저장 결과 메시지 (기존)
- `resultDeptNo` / `resultRankNo`: 중복체크 결과 메시지 (신규)
- 서로 다른 목적이므로 별도 필드 사용

---

## 9. 참고 문서

**관련 작업지시서**:
- [20260211_03: 부서/직급 중복 체크 로직 구현](20260211_03_add_duplicate_check_for_dept_rank_no.md)

**참고 파일**:
- `Admin/Users/Create.razor` (Line 21-27) - UI 구조
- `Admin/Users/Create.razor.cs` (Line 143-159) - 중복체크 메서드

**수정 파일**:
- `Depts/Create.razor` - UI 추가
- `Depts/Create.razor.cs` - 메서드 추가
- `Ranks/Create.razor` - UI 추가
- `Ranks/Create.razor.cs` - 메서드 추가

---

**작성 완료**: 2026-02-11
**예상 작업 시간**: 약 15분
**수정 파일 수**: 4개
