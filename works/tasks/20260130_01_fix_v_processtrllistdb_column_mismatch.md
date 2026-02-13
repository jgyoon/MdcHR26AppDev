# 작업지시서: v_ProcessTRListDB Repository 컬럼 불일치 수정

**날짜**: 2026-01-30
**작업 타입**: 버그 수정
**예상 소요**: 30분
**관련 이슈**: [#012](../issues/012_v_processtrllistdb_view_column_mismatch.md)

---

## 1. 작업 개요

### 배경
- Phase 3-3 TotalReport 관리자 페이지 런타임 오류 발생
- 2025년 코드 기반으로 작성된 Repository가 2026년 DB 구조와 불일치
- `Process_Year`, `Start_Date` 등 존재하지 않는 컬럼 사용

### 목표
2026년 v_ProcessTRListDB View 구조에 맞게 Repository SQL 수정

---

## 2. 작업 내용

### 파일: `MdcHR26Apps.Models/Views/v_ProcessTRListDB/v_ProcessTRListRepository.cs`

#### 수정 1: GetByAllAsync() - Line 21-30
**변경 전**:
```csharp
public async Task<IEnumerable<v_ProcessTRListDB>> GetByAllAsync()
{
    const string sql = """
        SELECT * FROM v_ProcessTRListDB
        ORDER BY Process_Year DESC, Start_Date DESC
        """;

    using var connection = new SqlConnection(dbContext);
    return await connection.QueryAsync<v_ProcessTRListDB>(sql);
}
```

**변경 후**:
```csharp
public async Task<IEnumerable<v_ProcessTRListDB>> GetByAllAsync()
{
    const string sql = """
        SELECT * FROM v_ProcessTRListDB
        ORDER BY Pid DESC
        """;

    using var connection = new SqlConnection(dbContext);
    return await connection.QueryAsync<v_ProcessTRListDB>(sql);
}
```

---

#### 수정 2: GetAllAsync() - Line 138-148
**변경 전**:
```csharp
public async Task<List<v_ProcessTRListDB>> GetAllAsync()
{
    const string sql = """
        SELECT * FROM v_ProcessTRListDB
        ORDER BY Process_Year DESC, Start_Date DESC
        """;

    using var connection = new SqlConnection(dbContext);
    var result = await connection.QueryAsync<v_ProcessTRListDB>(sql);
    return result.AsList();
}
```

**변경 후**:
```csharp
public async Task<List<v_ProcessTRListDB>> GetAllAsync()
{
    const string sql = """
        SELECT * FROM v_ProcessTRListDB
        ORDER BY Pid DESC
        """;

    using var connection = new SqlConnection(dbContext);
    var result = await connection.QueryAsync<v_ProcessTRListDB>(sql);
    return result.AsList();
}
```

---

#### 수정 3: GetByUserIdAsync() - Line 54-64
**변경 전**:
```csharp
public async Task<IEnumerable<v_ProcessTRListDB>> GetByUserIdAsync(Int64 uid)
{
    const string sql = """
        SELECT * FROM v_ProcessTRListDB
        WHERE Uid = @uid
        ORDER BY Process_Year DESC, Start_Date DESC
        """;

    using var connection = new SqlConnection(dbContext);
    return await connection.QueryAsync<v_ProcessTRListDB>(sql, new { uid });
}
```

**변경 후**:
```csharp
public async Task<IEnumerable<v_ProcessTRListDB>> GetByUserIdAsync(Int64 uid)
{
    const string sql = """
        SELECT * FROM v_ProcessTRListDB
        WHERE Uid = @uid
        ORDER BY Pid DESC
        """;

    using var connection = new SqlConnection(dbContext);
    return await connection.QueryAsync<v_ProcessTRListDB>(sql, new { uid });
}
```

---

#### 수정 4: GetByYearAsync(), GetByGradeAsync(), GetByProcessStatusAsync() 제거

**사유**: 2026년 View에 해당 컬럼이 없으며, 현재 사용하지 않음

**제거할 메서드** (Line 67-115):
- `GetByYearAsync()` - Process_Year 컬럼 없음
- `GetByGradeAsync()` - Final_Grade 컬럼 없음
- `GetByProcessStatusAsync()` - Process_Status 컬럼 없음

**인터페이스에서도 제거**: `Iv_ProcessTRListRepository.cs`

---

## 3. 테스트 계획

### 빌드 테스트
```bash
cd MdcHR26Apps.BlazorServer
dotnet build
```
**예상 결과**: 오류 0개

### 런타임 테스트
1. 애플리케이션 실행
2. Admin 로그인
3. `/Admin/TotalReport` 접속
4. **기대 결과**: SqlException 없이 정상 로드

---

## 4. 주의사항

### 데이터 정렬
- 기존: `Process_Year DESC, Start_Date DESC` (연도, 시작일 기준)
- 변경: `Pid DESC` (프로세스 ID 기준)
- **영향**: Pid가 자동 증가하므로 최신순 정렬은 동일하게 동작

### 제거된 메서드
- 현재 사용하지 않는 메서드만 제거
- 향후 필요 시 Pid 기반으로 재구현 가능

---

## 5. 관련 문서

**이슈**: [#012](../issues/012_v_processtrllistdb_view_column_mismatch.md)
**DB 구조**: [v_ProcessTRListDB.cs](../../MdcHR26Apps.Models/Views/v_ProcessTRListDB/v_ProcessTRListDB.cs)
**선행 작업**: [20260129_06](20260129_06_phase3_3_totalreport_step4_14.md)
