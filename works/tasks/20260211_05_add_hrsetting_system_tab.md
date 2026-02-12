# 작업지시서: HRSetting 시스템 설정 탭 추가

**작성일**: 2026-02-11
**작업번호**: 20260211_05
**작업명**: HRSetting 시스템 설정 탭 추가
**난이도**: ⭐⭐⭐ (복잡한 작업 - 3개 이상 파일, 기능 추가)

---

## 1. 작업 배경

### 현재 상황
- **appsettings.json**으로 관리 중: `IsProduction`, `IsOpen`
- 설정 변경 시 서버 재시작 필요
- 관리자 UI 없음

### 문제점
- 설정 변경이 불편 (파일 수정 → 서버 재시작)
- 평가 오픈/수정 설정을 동적으로 관리 필요
- SubAgree 페이지 팀장 초기화 기능 제어 필요

### 해결 방안
- **HRSetting 테이블** 생성 (완료)
- **시스템 설정 탭** 추가 (Admin/SettingManage)
- **토글 스위치**로 평가 오픈/수정 제어
- DB 기반 동적 설정 관리

---

## 2. Database 구조

### HRSetting 테이블 (생성 완료)
```sql
CREATE TABLE [dbo].[HRSetting]
(
    [HRSid] BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    [Evaluation_Open] BIT NOT NULL DEFAULT 0,  -- 평가 오픈
    [Edit_Open] BIT NOT NULL DEFAULT 0          -- 평가 수정
)
```

**필드 설명**:
- `HRSid`: Primary Key (자동 증가)
- `Evaluation_Open`: 평가 입력 가능 여부 (0=종료, 1=오픈)
- `Edit_Open`: SubAgree 팀장 초기화 등 수정 기능 (0=비활성, 1=활성)

**데이터 특성**:
- **단일 레코드 관리** (Settings 패턴)
- CRUD 중 Create/Delete 불필요
- Read(현재 설정) + Update(토글) 만 필요

---

## 3. 작업 단계

### Step 1: HRSettingDb Model 생성

**파일**: `MdcHR26Apps.Models/HRSetting/HRSettingDb.cs`

**작업**: Write (새 파일 생성)

**내용**:
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MdcHR26Apps.Models.HRSetting;

/// <summary>
/// 시스템 설정 Entity (단일 레코드 관리)
/// </summary>
[Table("HRSetting")]
public class HRSettingDb
{
    /// <summary>
    /// 설정 ID (PK, IDENTITY)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Int64 HRSid { get; set; }

    /// <summary>
    /// 평가 오픈 여부 (BIT)
    /// 0: 평가 종료, 1: 평가 오픈
    /// </summary>
    [Required]
    public bool Evaluation_Open { get; set; } = false;

    /// <summary>
    /// 평가 수정 가능 여부 (BIT)
    /// 0: 수정 불가, 1: 수정 가능 (SubAgree 팀장 초기화 등)
    /// </summary>
    [Required]
    public bool Edit_Open { get; set; } = false;
}
```

**패턴**: EDepartmentDb.cs 참고

---

### Step 2: IHRSettingRepository Interface 생성

**파일**: `MdcHR26Apps.Models/HRSetting/IHRSettingRepository.cs`

**작업**: Write (새 파일 생성)

**내용**:
```csharp
namespace MdcHR26Apps.Models.HRSetting;

/// <summary>
/// HRSetting Repository Interface
/// Settings 패턴 (단일 레코드 관리)
/// </summary>
public interface IHRSettingRepository : IDisposable
{
    /// <summary>
    /// 현재 시스템 설정 조회
    /// </summary>
    Task<HRSettingDb?> GetCurrentAsync();

    /// <summary>
    /// 시스템 설정 업데이트
    /// </summary>
    Task<int> UpdateAsync(HRSettingDb setting);

    /// <summary>
    /// 초기 레코드 생성 (없을 경우)
    /// </summary>
    Task<int> InitializeAsync();
}
```

**패턴**: IEDepartmentRepository.cs 참고 (단, CRUD → Settings 패턴)

**주의사항**:
- GetByAllAsync, DeleteAsync 불필요 (단일 레코드)
- GetCurrentAsync: `SELECT TOP 1` 사용
- InitializeAsync: 테이블이 비어있을 때만 INSERT

---

### Step 3: HRSettingRepository Implementation 생성

**파일**: `MdcHR26Apps.Models/HRSetting/HRSettingRepository.cs`

**작업**: Write (새 파일 생성)

**내용**:
```csharp
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MdcHR26Apps.Models.HRSetting;

/// <summary>
/// HRSetting Repository 구현 (Dapper)
/// Primary Constructor 사용 (C# 13)
/// </summary>
public class HRSettingRepository(string connectionString, ILoggerFactory loggerFactory) : IHRSettingRepository
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<HRSettingRepository>();
    private readonly string dbContext = connectionString;

    #region + [1] 현재 설정 조회: GetCurrentAsync
    /// <summary>
    /// 현재 시스템 설정 조회 (단일 레코드)
    /// </summary>
    public async Task<HRSettingDb?> GetCurrentAsync()
    {
        const string sql = "SELECT TOP 1 * FROM HRSetting";

        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<HRSettingDb>(sql);
    }
    #endregion

    #region + [2] 설정 업데이트: UpdateAsync
    /// <summary>
    /// 시스템 설정 업데이트
    /// </summary>
    public async Task<int> UpdateAsync(HRSettingDb setting)
    {
        const string sql = """
            UPDATE HRSetting
            SET Evaluation_Open = @Evaluation_Open,
                Edit_Open = @Edit_Open
            WHERE HRSid = @HRSid
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql, setting);
    }
    #endregion

    #region + [3] 초기화: InitializeAsync
    /// <summary>
    /// 초기 레코드 생성 (없을 경우)
    /// </summary>
    public async Task<int> InitializeAsync()
    {
        const string sql = """
            IF NOT EXISTS (SELECT 1 FROM HRSetting)
            BEGIN
                INSERT INTO HRSetting (Evaluation_Open, Edit_Open)
                VALUES (0, 0)
            END
            """;

        using var connection = new SqlConnection(dbContext);
        return await connection.ExecuteAsync(sql);
    }
    #endregion

    #region + [#] Dispose
    /// <summary>
    /// 리소스 해제
    /// </summary>
    public void Dispose()
    {
        db?.Dispose();
        GC.SuppressFinalize(this);
    }
    #endregion
}
```

**패턴**: EDepartmentRepository.cs 참고

**주의사항**:
- `QueryFirstOrDefaultAsync`: 단일 레코드 조회
- `ExecuteAsync`: INSERT/UPDATE 실행
- InitializeAsync: `IF NOT EXISTS` 사용

---

### Step 4: DI 등록

**파일**: `MdcHR26Apps.Models/MdcHR26AppsAddExtensions.cs`

**작업**: Edit (기존 파일 수정)

**수정 위치**: Line 45 뒤 (ERankRepository 등록 뒤)

**추가 코드**:
```csharp
// HRSetting Repository 등록
builder.Services.AddScoped<IHRSettingRepository>(sp =>
    new HRSettingRepository(connectionString, loggerFactory));
```

**패턴**: 기존 Repository 등록 방식과 동일

---

### Step 5: SettingManage.razor 탭 추가

**파일**: `MdcHR26Apps.BlazorServer/Components/Pages/Admin/SettingManage.razor`

**작업**: Edit (기존 파일 수정)

#### 수정 5-1: 탭 헤더 추가 (Line 24 뒤)

**위치**: 직급 관리 탭 `</li>` 뒤

**추가 코드**:
```razor
    <li class="nav-item" role="presentation">
        <button class="nav-link @(activeTab == "system" ? "active" : "")"
                type="button"
                @onclick='() => SetActiveTab("system")'>
            시스템 설정
        </button>
    </li>
```

#### 수정 5-2: 탭 컨텐츠 추가 (Line 130 뒤)

**위치**: 직급 관리 컨텐츠 `</div>` 뒤, 전체 `</div>` 태그 전

**추가 코드**:
```razor
    else if (activeTab == "system")
    {
        <!-- 시스템 설정 영역 -->
        <div class="tab-pane active">
            <h5>시스템 설정</h5>
            <hr />

            @if (currentSetting == null)
            {
                <p><em>Loading...</em></p>
            }
            else
            {
                <!-- 평가 오픈 설정 -->
                <div class="mb-4">
                    <h6>평가 오픈</h6>
                    <div class="form-check form-switch">
                        <input class="form-check-input"
                               type="checkbox"
                               id="evaluationOpenSwitch"
                               @bind="currentSetting.Evaluation_Open"
                               @onchange="UpdateEvaluationOpen" />
                        <label class="form-check-label" for="evaluationOpenSwitch">
                            @(currentSetting.Evaluation_Open ? "평가 오픈 중" : "평가 종료")
                        </label>
                    </div>
                    <small class="text-muted">
                        평가 오픈 여부를 관리합니다. 오픈 시 일반 사용자가 평가를 입력할 수 있습니다.
                    </small>
                </div>

                <hr />

                <!-- 평가 수정 설정 -->
                <div class="mb-4">
                    <h6>평가 수정</h6>
                    <div class="form-check form-switch">
                        <input class="form-check-input"
                               type="checkbox"
                               id="editOpenSwitch"
                               @bind="currentSetting.Edit_Open"
                               @onchange="UpdateEditOpen" />
                        <label class="form-check-label" for="editOpenSwitch">
                            @(currentSetting.Edit_Open ? "수정 가능" : "수정 불가")
                        </label>
                    </div>
                    <small class="text-muted">
                        SubAgree 페이지의 팀장 초기화 등 수정 기능 활성화 여부를 관리합니다.
                    </small>
                </div>
            }
        </div>
    }
```

**UI 구성**:
- Bootstrap `form-check form-switch` 토글 스위치
- `@bind`: 양방향 바인딩
- `@onchange`: 변경 시 즉시 DB 업데이트
- 설명 텍스트 포함 (사용자 이해 돕기)

---

### Step 6: SettingManage.razor.cs 로직 추가

**파일**: `MdcHR26Apps.BlazorServer/Components/Pages/Admin/SettingManage.razor.cs`

**작업**: Edit (기존 파일 수정)

#### 수정 6-1: Primary Constructor 수정 (Line 8-12)

**변경 전**:
```csharp
public partial class SettingManage(
    IEDepartmentRepository eDepartmentRepository,
    IERankRepository eRankRepository,
    LoginStatusService loginStatusService,
    UrlActions urlActions)
```

**변경 후**:
```csharp
public partial class SettingManage(
    IEDepartmentRepository eDepartmentRepository,
    IERankRepository eRankRepository,
    IHRSettingRepository hRSettingRepository,  // 추가!
    LoginStatusService loginStatusService,
    UrlActions urlActions)
```

#### 수정 6-2: 필드 추가 (Line 21 뒤)

**위치**: `private List<ERankDb> rankLists ...` 뒤

**추가 코드**:
```csharp
    // 시스템 설정
    private HRSettingDb? currentSetting { get; set; } = null;
```

#### 수정 6-3: LoadData 메서드 수정 (Line 30-37)

**변경 전**:
```csharp
private async Task LoadData()
{
    await Task.Delay(1);
    var depts = await eDepartmentRepository.GetByAllAsync();
    var ranks = await eRankRepository.GetByAllAsync();
    deptLists = depts.ToList();
    rankLists = ranks.ToList();
}
```

**변경 후**:
```csharp
private async Task LoadData()
{
    await Task.Delay(1);
    var depts = await eDepartmentRepository.GetByAllAsync();
    var ranks = await eRankRepository.GetByAllAsync();
    deptLists = depts.ToList();
    rankLists = ranks.ToList();

    // 시스템 설정 조회
    var setting = await hRSettingRepository.GetCurrentAsync();
    if (setting == null)
    {
        // 초기 레코드 생성
        await hRSettingRepository.InitializeAsync();
        setting = await hRSettingRepository.GetCurrentAsync();
    }
    currentSetting = setting;
}
```

#### 수정 6-4: 시스템 설정 메서드 추가 (Line 97 뒤)

**위치**: `GetStatusText()` 메서드 region 뒤

**추가 코드**:
```csharp
    #region System Setting Methods
    /// <summary>
    /// 평가 오픈 토글
    /// </summary>
    private async Task UpdateEvaluationOpen()
    {
        if (currentSetting == null) return;

        await hRSettingRepository.UpdateAsync(currentSetting);
        StateHasChanged();
    }

    /// <summary>
    /// 평가 수정 토글
    /// </summary>
    private async Task UpdateEditOpen()
    {
        if (currentSetting == null) return;

        await hRSettingRepository.UpdateAsync(currentSetting);
        StateHasChanged();
    }
    #endregion
```

**로직 설명**:
1. `@bind`로 변경된 값이 `currentSetting`에 반영됨
2. `@onchange` 이벤트로 즉시 DB 업데이트
3. `StateHasChanged()`로 UI 갱신

---

## 4. 빌드 테스트

### 테스트 1: 빌드 확인
```bash
dotnet build
```

**예상 결과**: 0 오류, 기존 경고와 동일

---

### 테스트 2: 시스템 설정 탭 접속

1. 애플리케이션 실행: `dotnet run`
2. Admin 로그인
3. **Admin/SettingManage** 접속
4. **시스템 설정** 탭 클릭

**예상 화면**:
```
┌─────────────────────────────────────┐
│ 시스템 설정                           │
├─────────────────────────────────────┤
│ 평가 오픈                             │
│ ☐ 평가 종료                          │
│ 평가 오픈 여부를 관리합니다. ...      │
├─────────────────────────────────────┤
│ 평가 수정                             │
│ ☐ 수정 불가                          │
│ SubAgree 페이지의 팀장 초기화 등...   │
└─────────────────────────────────────┘
```

---

### 테스트 3: 토글 스위치 작동 확인

1. **평가 오픈** 스위치 ON
   - 라벨: "평가 종료" → "평가 오픈 중" 변경
   - DB 확인: `SELECT * FROM HRSetting` → Evaluation_Open = 1

2. **평가 수정** 스위치 ON
   - 라벨: "수정 불가" → "수정 가능" 변경
   - DB 확인: `SELECT * FROM HRSetting` → Edit_Open = 1

3. 새로고침 후 상태 유지 확인
   - 페이지 새로고침 (F5)
   - 스위치 상태가 DB와 동일한지 확인

---

## 5. 완료 조건

### Backend
- [x] HRSettingDb 모델 생성
- [x] IHRSettingRepository 인터페이스 생성
- [x] HRSettingRepository 구현체 생성
- [x] DI 등록 (MdcHR26AppsAddExtensions.cs)

### Frontend
- [x] SettingManage.razor 탭 추가 (시스템 설정)
- [x] 평가 오픈 토글 UI 구현
- [x] 평가 수정 토글 UI 구현
- [x] SettingManage.razor.cs 로직 추가

### 테스트
- [x] 빌드 성공
- [x] 시스템 설정 탭 접속
- [x] 토글 스위치 작동 확인
- [x] DB 저장 확인
- [x] 새로고침 후 상태 유지 확인

---

## 6. 파일 목록

### 생성할 파일 (3개)
1. `MdcHR26Apps.Models/HRSetting/HRSettingDb.cs`
2. `MdcHR26Apps.Models/HRSetting/IHRSettingRepository.cs`
3. `MdcHR26Apps.Models/HRSetting/HRSettingRepository.cs`

### 수정할 파일 (3개)
4. `MdcHR26Apps.Models/MdcHR26AppsAddExtensions.cs` (DI 등록)
5. `MdcHR26Apps.BlazorServer/Components/Pages/Admin/SettingManage.razor` (탭 추가)
6. `MdcHR26Apps.BlazorServer/Components/Pages/Admin/SettingManage.razor.cs` (로직 추가)

**총 6개 파일**

---

## 7. 참고 패턴

### Settings 패턴 vs CRUD 패턴

| 항목 | CRUD (부서/직급) | Settings (HRSetting) |
|------|-----------------|---------------------|
| 데이터 | 여러 레코드 | 단일 레코드 |
| Create | 필요 | 초기화만 (InitializeAsync) |
| Read | GetByAllAsync | GetCurrentAsync |
| Update | 필요 | 필요 |
| Delete | 필요 | 불필요 |
| UI | 테이블 + CRUD 페이지 | 토글 스위치 |

### Bootstrap Toggle Switch
```razor
<div class="form-check form-switch">
    <input class="form-check-input"
           type="checkbox"
           id="switchId"
           @bind="model.Property"
           @onchange="MethodName" />
    <label class="form-check-label" for="switchId">
        Label Text
    </label>
</div>
```

---

## 8. 향후 확장

### 추가 가능한 설정
- `Feedback_Open`: 피드백 오픈 여부
- `Report_Deadline`: 리포트 제출 마감일
- `System_Maintenance`: 시스템 점검 모드

### appsettings.json 이전
현재 appsettings.json의 `IsProduction`, `IsOpen`도 향후 DB로 이전 가능

---

**작성자**: Claude AI
**검토자**: 개발자
**승인일**: (승인 후 기입)
