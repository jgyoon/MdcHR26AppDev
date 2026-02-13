# 작업지시서: 부서번호/직급번호 중복 체크 기능 추가

**날짜**: 2026-02-11
**작업 유형**: 기능 추가
**관련 작업지시서**: `20260211_02_admin_settings_crud_pages.md`

---

## 1. 문제 상황

### 1.1. 현재 상태
- 부서 등록/수정 시 부서번호(`EDepartmentNo`) 중복 체크 없음
- 직급 등록/수정 시 직급번호(`ERankNo`) 중복 체크 없음
- 유효성 검증: 필수 값 체크만 수행 (이름, 번호 >= 1)

### 1.2. 요구사항
- 부서번호와 직급번호는 유니크(Unique)해야 함
- 등록 시: 동일한 번호가 이미 존재하면 오류 메시지
- 수정 시: 자기 자신을 제외한 다른 레코드와 중복되면 오류 메시지

---

## 2. 해결 방안

### 2.1. Repository에 메서드 추가
- `GetByDepartmentNoAsync(int departmentNo)` - 부서번호로 조회
- `GetByRankNoAsync(int rankNo)` - 직급번호로 조회

### 2.2. Create/Edit 페이지에 중복 체크 로직 추가
- Create: 해당 번호가 존재하면 오류
- Edit: 자기 자신(`Id`) 제외하고 해당 번호가 존재하면 오류

---

## 3. 수정 파일 목록

### 3.1. Repository 인터페이스 (2개 파일)
1. `MdcHR26Apps.Models/Department/IEDepartmentRepository.cs`
2. `MdcHR26Apps.Models/Rank/IERankRepository.cs`

### 3.2. Repository 구현체 (2개 파일)
3. `MdcHR26Apps.Models/Department/EDepartmentRepository.cs`
4. `MdcHR26Apps.Models/Rank/ERankRepository.cs`

### 3.3. Blazor 페이지 (4개 파일)
5. `MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Depts/Create.razor.cs`
6. `MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Depts/Edit.razor.cs`
7. `MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Ranks/Create.razor.cs`
8. `MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Ranks/Edit.razor.cs`

**총 8개 파일 수정**

---

## 4. 상세 수정 내용

### Step 1: IEDepartmentRepository에 메서드 추가

**파일**: `MdcHR26Apps.Models/Department/IEDepartmentRepository.cs`

**위치**: Line 32 뒤에 추가

```csharp
/// <summary>
/// 부서번호로 부서 조회 (중복 체크용)
/// </summary>
Task<EDepartmentDb?> GetByDepartmentNoAsync(int departmentNo);
```

---

### Step 2: EDepartmentRepository에 메서드 구현

**파일**: `MdcHR26Apps.Models/Department/EDepartmentRepository.cs`

**위치**: GetIdByNameAsync 메서드 뒤에 추가 (region 안에)

```csharp
#region + [비즈니스] 부서번호로 조회
public async Task<EDepartmentDb?> GetByDepartmentNoAsync(int departmentNo)
{
    const string sql = "SELECT * FROM EDepartmentDb WHERE EDepartmentNo = @DepartmentNo";

    using var connection = new SqlConnection(dbContext);
    return await connection.QueryFirstOrDefaultAsync<EDepartmentDb>(sql, new { DepartmentNo = departmentNo });
}
#endregion
```

---

### Step 3: IERankRepository에 메서드 추가

**파일**: `MdcHR26Apps.Models/Rank/IERankRepository.cs`

**위치**: Line 27 뒤에 추가

```csharp
/// <summary>
/// 직급번호로 직급 조회 (중복 체크용)
/// </summary>
Task<ERankDb?> GetByRankNoAsync(int rankNo);
```

---

### Step 4: ERankRepository에 메서드 구현

**파일**: `MdcHR26Apps.Models/Rank/ERankRepository.cs`

**위치**: GetSelectListAsync 메서드 뒤에 추가 (region 안에)

```csharp
#region + [비즈니스] 직급번호로 조회
public async Task<ERankDb?> GetByRankNoAsync(int rankNo)
{
    const string sql = "SELECT * FROM ERankDb WHERE ERankNo = @RankNo";

    using var connection = new SqlConnection(dbContext);
    return await connection.QueryFirstOrDefaultAsync<ERankDb>(sql, new { RankNo = rankNo });
}
#endregion
```

---

### Step 5: Depts/Create.razor.cs 중복 체크 추가

**파일**: `MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Depts/Create.razor.cs`

**메서드**: `SaveDepartment()` 수정

**기존 코드** (Line 34-40):
```csharp
private async Task SaveDepartment()
{
    if (string.IsNullOrEmpty(model.EDepartmentName) || model.EDepartmentNo < 1)
    {
        resultText = "부서명과 부서번호(1 이상)는 필수입니다.";
        return;
    }

    var result = await eDepartmentRepository.AddAsync(model);
```

**수정 후**:
```csharp
private async Task SaveDepartment()
{
    if (string.IsNullOrEmpty(model.EDepartmentName) || model.EDepartmentNo < 1)
    {
        resultText = "부서명과 부서번호(1 이상)는 필수입니다.";
        return;
    }

    // 중복 체크
    var existing = await eDepartmentRepository.GetByDepartmentNoAsync(model.EDepartmentNo);
    if (existing != null)
    {
        resultText = $"부서번호 {model.EDepartmentNo}는 이미 사용 중입니다.";
        return;
    }

    var result = await eDepartmentRepository.AddAsync(model);
```

---

### Step 6: Depts/Edit.razor.cs 중복 체크 추가

**파일**: `MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Depts/Edit.razor.cs`

**메서드**: `UpdateDepartment()` 수정

**기존 코드**:
```csharp
private async Task UpdateDepartment()
{
    if (string.IsNullOrEmpty(model.EDepartmentName) || model.EDepartmentNo < 1)
    {
        resultText = "부서명과 부서번호(1 이상)는 필수입니다.";
        return;
    }

    var result = await eDepartmentRepository.UpdateAsync(model);
```

**수정 후**:
```csharp
private async Task UpdateDepartment()
{
    if (string.IsNullOrEmpty(model.EDepartmentName) || model.EDepartmentNo < 1)
    {
        resultText = "부서명과 부서번호(1 이상)는 필수입니다.";
        return;
    }

    // 중복 체크 (자기 자신 제외)
    var existing = await eDepartmentRepository.GetByDepartmentNoAsync(model.EDepartmentNo);
    if (existing != null && existing.EDepartId != model.EDepartId)
    {
        resultText = $"부서번호 {model.EDepartmentNo}는 이미 사용 중입니다.";
        return;
    }

    var result = await eDepartmentRepository.UpdateAsync(model);
```

---

### Step 7: Ranks/Create.razor.cs 중복 체크 추가

**파일**: `MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Ranks/Create.razor.cs`

**메서드**: `SaveRank()` 수정

**수정 내용**: Step 5와 동일한 패턴, `EDepartment` → `ERank` 치환

```csharp
// 중복 체크
var existing = await eRankRepository.GetByRankNoAsync(model.ERankNo);
if (existing != null)
{
    resultText = $"직급번호 {model.ERankNo}는 이미 사용 중입니다.";
    return;
}
```

---

### Step 8: Ranks/Edit.razor.cs 중복 체크 추가

**파일**: `MdcHR26Apps.BlazorServer/Components/Pages/Admin/Settings/Ranks/Edit.razor.cs`

**메서드**: `UpdateRank()` 수정

**수정 내용**: Step 6과 동일한 패턴, `EDepartment` → `ERank` 치환

```csharp
// 중복 체크 (자기 자신 제외)
var existing = await eRankRepository.GetByRankNoAsync(model.ERankNo);
if (existing != null && existing.ERankId != model.ERankId)
{
    resultText = $"직급번호 {model.ERankNo}는 이미 사용 중입니다.";
    return;
}
```

---

## 5. 테스트 시나리오

### 시나리오 1: 부서 등록 - 중복 체크
1. 부서번호 100, 부서명 "테스트부서" 등록 → **성공**
2. 다시 부서번호 100, 부서명 "다른부서" 등록 시도
3. **확인**: "부서번호 100는 이미 사용 중입니다." 오류 메시지 ✅

### 시나리오 2: 부서 수정 - 자기 자신은 허용
1. 부서번호 100 상세 페이지 → 수정
2. 부서명만 "수정된부서"로 변경 (부서번호 100 유지)
3. **확인**: "부서 수정 성공" (자기 자신이므로 중복 아님) ✅

### 시나리오 3: 부서 수정 - 다른 부서 번호와 중복
1. 부서번호 100, 200 두 개가 존재하는 상태
2. 부서번호 200 수정 페이지에서 부서번호를 100으로 변경 시도
3. **확인**: "부서번호 100는 이미 사용 중입니다." 오류 메시지 ✅

### 시나리오 4: 직급 등록/수정
- 부서와 동일한 테스트 (번호만 직급번호로 변경)

---

## 6. 예상 결과

### 수정 전
- 부서번호/직급번호 중복 가능 ❌
- 데이터 무결성 문제 발생 가능

### 수정 후
- 부서번호/직급번호 유니크 보장 ✅
- 사용자에게 명확한 오류 메시지 제공
- 수정 시 자기 자신은 허용 (정상 동작)

---

## 7. 주의사항

1. **Edit 시 자기 자신 제외**: `existing.EDepartId != model.EDepartId` 조건 필수
2. **int vs Int64**: 부서번호/직급번호는 `int` 타입
3. **Null 체크**: `existing != null` 확인 필수
4. **메서드 추가 위치**: Repository 구현체는 기존 region 구조 유지

---

## 8. 완료 조건

- [ ] IEDepartmentRepository에 GetByDepartmentNoAsync 추가
- [ ] EDepartmentRepository에 GetByDepartmentNoAsync 구현
- [ ] IERankRepository에 GetByRankNoAsync 추가
- [ ] ERankRepository에 GetByRankNoAsync 구현
- [ ] Depts/Create.razor.cs 중복 체크 추가
- [ ] Depts/Edit.razor.cs 중복 체크 추가 (자기 제외)
- [ ] Ranks/Create.razor.cs 중복 체크 추가
- [ ] Ranks/Edit.razor.cs 중복 체크 추가 (자기 제외)
- [ ] 빌드 성공 (dotnet build)
- [ ] 테스트 시나리오 1-4 성공

---

**작성일**: 2026-02-11
**수정 파일 수**: 8개
**예상 작업 시간**: 약 20분
