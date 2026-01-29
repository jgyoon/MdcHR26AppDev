# 작업지시서 (Part 2): v_EvaluationUsersList Model 및 페이지 구현

**날짜**: 2026-01-29
**작업 유형**: Model/Repository/페이지 수정
**관련 이슈**:
- [#011: Phase 3-3 관리자 페이지 빌드 오류 및 재작업](../issues/011_phase3_3_admin_pages_build_errors.md)
**선행 작업지시서**:
- [20260129_01_create_v_evaluation_users_list_view.md](20260129_01_create_v_evaluation_users_list_view.md) - **DB 뷰 생성 완료 후 진행**

---

## 1. 작업 개요

**목적**:
- v_EvaluationUsersList 뷰를 C# 애플리케이션에서 사용할 수 있도록 Model/Repository 구현
- EUsersManage 페이지를 뷰 기반으로 수정

**선행 조건**:
- ✅ DB에 v_EvaluationUsersList 뷰 생성 완료 (20260129_01)

---

## 2. 파일 생성 및 수정

### 2.1. 생성할 파일 (3개)

#### File 1: View Entity

**파일**: `MdcHR26Apps.Models/Views/v_EvaluationUsersList/v_EvaluationUsersList.cs`

**내용**:
```csharp
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MdcHR26Apps.Models.Views.v_EvaluationUsersList;

/// <summary>
/// v_EvaluationUsersList View Entity (읽기 전용)
/// EvaluationUsers + UserDb + 부서장/임원 정보 조인 뷰
/// </summary>
[Keyless]
[Table("v_EvaluationUsersList")]
public class v_EvaluationUsersList
{
    public Int64 EUid { get; set; }
    public Int64 Uid { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string ENumber { get; set; } = string.Empty;
    public Int64 EDepartId { get; set; }
    public string EDepartmentName { get; set; } = string.Empty;
    public string ERank { get; set; } = string.Empty;
    public bool Is_Evaluation { get; set; }
    public Int64? TeamLeaderId { get; set; }
    public string? TeamLeaderName { get; set; }
    public Int64? DirectorId { get; set; }
    public string? DirectorName { get; set; }
    public bool Is_TeamLeader { get; set; }
}
```

---

#### File 2: Repository Interface

**파일**: `MdcHR26Apps.Models/Views/v_EvaluationUsersList/Iv_EvaluationUsersListRepository.cs`

**내용**:
```csharp
namespace MdcHR26Apps.Models.Views.v_EvaluationUsersList;

public interface Iv_EvaluationUsersListRepository : IDisposable
{
    Task<IEnumerable<v_EvaluationUsersList>> GetByAllAsync();
    Task<v_EvaluationUsersList?> GetByUidAsync(Int64 uid);
    Task<IEnumerable<v_EvaluationUsersList>> SearchByNameAsync(string userName);
}
```

---

#### File 3: Repository Implementation

**파일**: `MdcHR26Apps.Models/Views/v_EvaluationUsersList/v_EvaluationUsersListRepository.cs`

**내용**:
```csharp
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MdcHR26Apps.Models.Views.v_EvaluationUsersList;

public class v_EvaluationUsersListRepository(string connectionString, ILoggerFactory loggerFactory)
    : Iv_EvaluationUsersListRepository
{
    private readonly SqlConnection db = new(connectionString);
    private readonly ILogger _logger = loggerFactory.CreateLogger<v_EvaluationUsersListRepository>();
    private readonly string dbContext = connectionString;

    #region + [1] 전체 조회: GetByAllAsync
    public async Task<IEnumerable<v_EvaluationUsersList>> GetByAllAsync()
    {
        const string sql = "SELECT * FROM v_EvaluationUsersList ORDER BY EUid";
        using var connection = new SqlConnection(dbContext);
        return await connection.QueryAsync<v_EvaluationUsersList>(sql);
    }
    #endregion

    #region + [2] Uid로 조회: GetByUidAsync
    public async Task<v_EvaluationUsersList?> GetByUidAsync(Int64 uid)
    {
        const string sql = "SELECT * FROM v_EvaluationUsersList WHERE Uid = @uid";
        using var connection = new SqlConnection(dbContext);
        return await connection.QueryFirstOrDefaultAsync<v_EvaluationUsersList>(sql, new { uid });
    }
    #endregion

    #region + [3] 이름 검색: SearchByNameAsync
    public async Task<IEnumerable<v_EvaluationUsersList>> SearchByNameAsync(string userName)
    {
        const string sql = """
            SELECT * FROM v_EvaluationUsersList
            WHERE UserName LIKE @userName
            ORDER BY EUid
            """;
        using var connection = new SqlConnection(dbContext);
        // NVARCHAR 검색: 파라미터에 와일드카드 포함
        return await connection.QueryAsync<v_EvaluationUsersList>(sql, new { userName = userName + "%" });
    }
    #endregion

    #region + [#] Dispose
    public void Dispose()
    {
        db?.Dispose();
        GC.SuppressFinalize(this);
    }
    #endregion
}
```

---

### 2.2. 수정할 파일 (5개)

#### Modify 1: ApplicationDbContext.cs

**파일**: `MdcHR26Apps.Models/ApplicationDbContext.cs`

**위치**: DbSet 정의 부분 (라인 30-40 부근)

**변경 사항**:
```csharp
// Views
public DbSet<Views.v_MemberListDB.v_MemberListDB> v_MemberListDB { get; set; }
public DbSet<Views.v_EvaluationUsersList.v_EvaluationUsersList> v_EvaluationUsersList { get; set; } // 추가
public DbSet<Views.v_DeptObjectiveListDb.v_DeptObjectiveListDb> v_DeptObjectiveListDb { get; set; }
```

---

#### Modify 2: Program.cs

**파일**: `MdcHR26Apps.BlazorServer/Program.cs`

**위치**: Repository DI 등록 부분 (라인 70 부근)

**변경 사항**:
```csharp
// Views
builder.Services.AddScoped<Iv_MemberListRepository>(sp =>
    new v_MemberListRepository(connectionString, loggerFactory));
builder.Services.AddScoped<Iv_EvaluationUsersListRepository>(sp => // 추가
    new v_EvaluationUsersListRepository(connectionString, loggerFactory));
```

---

#### Modify 3: EUsersManage.razor.cs

**파일**: `Components/Pages/Admin/EUsersManage.razor.cs`

**변경 전**:
```csharp
using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.EvaluationUsers;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin;

public partial class EUsersManage(
    IEvaluationUsersRepository evaluationUsersRepository,
    LoginStatusService loginStatusService,
    UrlActions urlActions)
{
    private List<Models.EvaluationUsers.EvaluationUsers> userlist { get; set; } = new();
    private string searchTerm { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await CheckLogined();
        await SetData();
        StateHasChanged();
    }

    private async Task SetData()
    {
        var users = await evaluationUsersRepository.GetByAllAsync();
        userlist = users.ToList();
    }

    private async Task Search()
    {
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var allUsers = await evaluationUsersRepository.GetByAllAsync();
            // TODO: Navigation Property를 통한 UserName 검색 필요
            userlist = allUsers.ToList();
        }
        else
        {
            await SetData();
        }
    }

    // ... 나머지 메서드
}
```

**변경 후**:
```csharp
using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.Views.v_EvaluationUsersList;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Admin;

public partial class EUsersManage(
    Iv_EvaluationUsersListRepository evaluationUsersListRepository,
    LoginStatusService loginStatusService,
    UrlActions urlActions)
{
    private List<v_EvaluationUsersList> userlist { get; set; } = new();
    private string searchTerm { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await CheckLogined();
        await SetData();
        StateHasChanged();
    }

    private async Task SetData()
    {
        var users = await evaluationUsersListRepository.GetByAllAsync();
        userlist = users.ToList();
    }

    private async Task Search()
    {
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var allUsers = await evaluationUsersListRepository.SearchByNameAsync(searchTerm);
            userlist = allUsers.ToList();
        }
        else
        {
            await SetData();
        }
    }

    // ... 나머지 메서드 (변경 없음)
}
```

**주요 변경**:
1. using 변경: `MdcHR26Apps.Models.EvaluationUsers` → `MdcHR26Apps.Models.Views.v_EvaluationUsersList`
2. Repository 변경: `IEvaluationUsersRepository` → `Iv_EvaluationUsersListRepository`
3. Model 변경: `EvaluationUsers` → `v_EvaluationUsersList`
4. 검색 기능 활성화: `SearchByNameAsync` 메서드 사용

---

#### Modify 4: EUserListTable.razor

**파일**: `Components/Pages/Components/Table/EUserListTable.razor`

**위치**: 테이블 body 부분 (라인 17-32)

**변경 전**:
```razor
@foreach (var item in Users)
{
    <tr>
        <td>@sortNoAdd2(sortNo)</td>
        <td>@item.Uid</td>
        <td>@(item.User?.UserName ?? "미지정")</td>
        <td>@(item.TeamLeader?.UserName ?? "미지정")</td>
        <td>@(item.Director?.UserName ?? "미지정")</td>
        <td>@isuse(item.Is_Evaluation)</td>
        <td>
            <button class="btn btn-info" @onclick="@(() => DetailsAction(item.Uid))">상세</button>
            <button class="btn btn-secondary" @onclick="@(() => EditAction(item.Uid))">수정</button>
        </td>
    </tr>
}
```

**변경 후**:
```razor
@foreach (var item in Users)
{
    <tr>
        <td>@sortNoAdd2(sortNo)</td>
        <td>@item.Uid</td>
        <td>@item.UserName</td>
        <td>@(item.TeamLeaderName ?? "미지정")</td>
        <td>@(item.DirectorName ?? "미지정")</td>
        <td>@isuse(item.Is_Evaluation)</td>
        <td>
            <button class="btn btn-info" @onclick="@(() => DetailsAction(item.Uid))">상세</button>
            <button class="btn btn-secondary" @onclick="@(() => EditAction(item.Uid))">수정</button>
        </td>
    </tr>
}
```

**주요 변경**:
1. `item.User?.UserName` → `item.UserName` (뷰에서 직접 제공)
2. `item.TeamLeader?.UserName` → `item.TeamLeaderName` (뷰에서 직접 제공)
3. `item.Director?.UserName` → `item.DirectorName` (뷰에서 직접 제공)

---

#### Modify 5: EUserListTable.razor.cs

**파일**: `Components/Pages/Components/Table/EUserListTable.razor.cs`

**변경 전**:
```csharp
using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.EvaluationUsers;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Table;

public partial class EUserListTable
{
    [Parameter]
    public List<EvaluationUsers> Users { get; set; } = new();

    [Inject]
    public UrlActions urlActions { get; set; } = null!;

    // ... 나머지 코드
}
```

**변경 후**:
```csharp
using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.Views.v_EvaluationUsersList;
using Microsoft.AspNetCore.Components;

namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Table;

public partial class EUserListTable
{
    [Parameter]
    public List<v_EvaluationUsersList> Users { get; set; } = new();

    [Inject]
    public UrlActions urlActions { get; set; } = null!;

    // ... 나머지 코드 (변경 없음)
}
```

**주요 변경**:
1. using 변경: `MdcHR26Apps.Models.EvaluationUsers` → `MdcHR26Apps.Models.Views.v_EvaluationUsersList`
2. Parameter 타입 변경: `List<EvaluationUsers>` → `List<v_EvaluationUsersList>`

---

## 3. 빌드 테스트

```bash
cd MdcHR26Apps.BlazorServer
dotnet build
```

**확인**: 빌드 성공, 경고 5개 이하 ✅

---

## 4. 런타임 테스트

### Test 1: 평가대상자 목록 표시

1. 서버 시작: `dotnet run`
2. `/Admin/EUsersManage` 접속
3. **확인**:
   - 이름: **윤종국** (미지정 아님) ✅
   - 평가부서장: **관리자** ✅
   - 평가임원: **홍길동** 또는 미지정 ✅

### Test 2: 검색 기능

1. 검색창에 "윤" 입력
2. **확인**: 해당 이름 포함 사용자만 표시 ✅
3. 검색 초기화 버튼 클릭
4. **확인**: 전체 목록 표시 ✅

### Test 3: 상세 페이지 이동

1. "상세" 버튼 클릭
2. **확인**: `/Admin/EvaluationUsers/Details/{Uid}` 정상 이동 ✅

### Test 4: 회귀 테스트

- `/Admin/UserManage` - 정상 작동 ✅
- `/Admin/SettingManage` - 정상 작동 ✅

---

## 5. 완료 조건

- [ ] v_EvaluationUsersList.cs 생성
- [ ] Iv_EvaluationUsersListRepository.cs 생성
- [ ] v_EvaluationUsersListRepository.cs 생성
- [ ] ApplicationDbContext.cs DbSet 추가
- [ ] Program.cs DI 등록
- [ ] EUsersManage.razor.cs 수정
- [ ] EUserListTable.razor 수정
- [ ] EUserListTable.razor.cs 수정
- [ ] 빌드 성공
- [ ] Test 1 성공 (이름 표시)
- [ ] Test 2 성공 (검색 기능)
- [ ] Test 3 성공 (상세 이동)
- [ ] Test 4 성공 (회귀 테스트)

---

## 6. 주의사항

1. **네임스페이스 일관성**: `MdcHR26Apps.Models.Views.v_EvaluationUsersList`
2. **Nullable 처리**: `TeamLeaderName?`, `DirectorName?`
3. **v_MemberListDB 패턴**: 기존 뷰와 동일한 구조 및 명명 규칙
4. **Primary Constructor**: Repository에서 사용 (C# 13)
5. **Dapper 사용**: Entity Framework 아님

---

**작성자**: Claude AI
**검토자**: 개발자
**승인 상태**: 검토 대기
**선행 조건**: 20260129_01 완료 (DB 뷰 생성)
