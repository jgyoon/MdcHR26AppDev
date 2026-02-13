# 미구현 컴포넌트 체크리스트

**날짜**: 2026-01-26
**작업 유형**: 컴포넌트 구현 필요 목록
**관련 이슈**: [#009: Phase 3 WebApp 개발](../issues/009_phase3_webapp_development.md)
**선행 작업**: 프로젝트 구조 재정리 완료

---

## 1. 미구현 컴포넌트 목록

### 1.1. EUserListTable (평가대상자 목록 테이블)

**사용 위치**:
- `Components/Pages/Admin/EUsersManage.razor:21`

**현재 상태**: ❌ 미구현

**예상 구조**:
```razor
<!-- EUsersManage.razor에서 사용 -->
<EUserListTable Users="@userlist"></EUserListTable>
```

**구현 위치**: `Components/Pages/Components/Table/EUserListTable.razor`

**참고**:
- 2025년 프로젝트: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Components\EUserListTable.razor`
- 유사 컴포넌트: [UserListTable.razor](../../../MdcHR26Apps.BlazorServer/Components/Pages/Components/Table/UserListTable.razor)

**필요 기능**:
- 평가대상자 목록 테이블 표시
- UserId → Uid 변경 적용
- 상세/수정 페이지 이동 버튼

---

### 1.2. DisplayResultText (결과 메시지 표시)

**사용 위치**:
- `Components/Pages/Admin/Settings/Depts/Create.razor:9`
- `Components/Pages/Admin/Settings/Ranks/Create.razor:9`

**현재 상태**: ❌ 미구현

**예상 구조**:
```razor
<DisplayResultText Comment="@resultText" />
```

**구현 위치**: `Components/Pages/Components/Common/DisplayResultText.razor`

**참고**:
- 2025년 프로젝트: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Components\DisplayResultText.razor`
- 도서관리 프로젝트: `C:\Codes\36_MdcLibrary\MdcLibrary\MdcLibrary.Server\Components\Pages\Components\Common\DisplayResultText.razor`

**필요 기능**:
- 성공/실패 메시지 표시
- Bootstrap Alert 스타일 적용
- 자동 사라짐 효과 (선택사항)

---

### 1.3. MemberListTable (부서/직급별 사용자 목록)

**사용 위치**:
- `Components/Pages/Admin/Settings/Depts/Details.razor:52`
- `Components/Pages/Admin/Settings/Ranks/Details.razor:52`

**현재 상태**: ❌ 미구현

**예상 구조**:
```razor
<MemberListTable Members="@memberList"></MemberListTable>
```

**구현 위치**: `Components/Pages/Components/Table/MemberListTable.razor`

**참고**:
- 2025년 프로젝트: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Components\MemberListTable.razor`
- 유사 컴포넌트: [UserListTable.razor](../../../MdcHR26Apps.BlazorServer/Components/Pages/Components/Table/UserListTable.razor)

**필요 기능**:
- 부서/직급에 속한 사용자 목록 표시
- v_MemberListDB 뷰 데이터 사용
- ERank 필드 사용 (ERankId 없음!)

---

## 2. 컴포넌트 구현 우선순위

### 우선순위 1: DisplayResultText
- **이유**: 가장 단순하고, 여러 페이지에서 사용
- **난이도**: 낮음
- **예상 시간**: 10분

### 우선순위 2: EUserListTable
- **이유**: 평가대상자 관리 페이지에서 필수
- **난이도**: 중간
- **예상 시간**: 30분

### 우선순위 3: MemberListTable
- **이유**: Details 페이지에서만 사용
- **난이도**: 중간
- **예상 시간**: 30분

---

## 3. 구현 시 주의사항

### 3.1. 네임스페이스 규칙

**Common 컴포넌트**:
```csharp
namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Common;
```

**Table 컴포넌트**:
```csharp
namespace MdcHR26Apps.BlazorServer.Components.Pages.Components.Table;
```

### 3.2. DB 구조 변경 사항 반영

**2025년 → 2026년 주요 변경**:
- `UserId` (VARCHAR) → `Uid` (BIGINT)
- `TeamLeader_Id` → `TeamLeaderId`
- `Director_Id` → `DirectorId`
- `ERank` (뷰에서 ERankName의 별칭)
- ❌ `ERankId` (뷰에 없음!)

### 3.3. Primary Constructor 사용

```csharp
// .NET 10 스타일 (C# 13)
public partial class EUserListTable(UrlActions urlActions)
{
    private readonly UrlActions _urlActions = urlActions;

    [Parameter]
    public List<EvaluationUsers> Users { get; set; } = new();
}
```

### 3.4. InteractiveServer 렌더 모드

```razor
@rendermode InteractiveServer
```

---

## 4. 참고 프로젝트

### 4.1. 2025년 인사평가 (비즈니스 로직)

**경로**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp`

**참고 파일**:
- `Components/EUserListTable.razor`
- `Components/DisplayResultText.razor`
- `Components/MemberListTable.razor`

**주의사항**:
- 네임스페이스 변경 필요
- DB 구조 변경 사항 반영
- Primary Constructor 미사용 (기존 방식)

### 4.2. 도서관리 프로젝트 (최신 기술)

**경로**: `C:\Codes\36_MdcLibrary\MdcLibrary\MdcLibrary.Server`

**참고 파일**:
- `Components/Pages/Components/Common/DisplayResultText.razor`
- `Components/Pages/Components/Table/` (테이블 컴포넌트 예시)

**장점**:
- .NET 10 적용
- Primary Constructor 사용
- InteractiveServer 렌더 모드
- 최신 Blazor 패턴

---

## 5. 구현 체크리스트

### DisplayResultText

- [ ] 파일 생성: `Components/Pages/Components/Common/DisplayResultText.razor`
- [ ] 파일 생성: `Components/Pages/Components/Common/DisplayResultText.razor.cs`
- [ ] Parameter 설정: `Comment` (string)
- [ ] Bootstrap Alert 스타일 적용
- [ ] 빌드 테스트
- [ ] Settings/Depts/Create.razor에서 테스트
- [ ] Settings/Ranks/Create.razor에서 테스트

### EUserListTable

- [ ] 파일 생성: `Components/Pages/Components/Table/EUserListTable.razor`
- [ ] 파일 생성: `Components/Pages/Components/Table/EUserListTable.razor.cs`
- [ ] Parameter 설정: `Users` (List<EvaluationUsers>)
- [ ] 테이블 구조 작성
- [ ] Uid 필드 사용 (UserId 아님!)
- [ ] 페이지 이동 버튼 추가
- [ ] 빌드 테스트
- [ ] Admin/EUsersManage.razor에서 테스트

### MemberListTable

- [ ] 파일 생성: `Components/Pages/Components/Table/MemberListTable.razor`
- [ ] 파일 생성: `Components/Pages/Components/Table/MemberListTable.razor.cs`
- [ ] Parameter 설정: `Members` (List<v_MemberListDB>)
- [ ] 테이블 구조 작성
- [ ] ERank 필드 사용 (ERankId 없음!)
- [ ] 빌드 테스트
- [ ] Settings/Depts/Details.razor에서 테스트
- [ ] Settings/Ranks/Details.razor에서 테스트

---

## 6. 예상 빌드 경고 변화

### 현재 (10개 경고)

```
RZ10012: EUserListTable (1개)
RZ10012: DisplayResultText (2개)
RZ10012: MemberListTable (2개)
CS8601: null 참조 할당 (3개)
CS9113: 사용하지 않는 매개변수 (2개)
```

### 목표 (5개 경고)

```
CS8601: null 참조 할당 (3개) - 기존 페이지
CS9113: 사용하지 않는 매개변수 (2개) - Create.razor.cs (아직 구현 안 됨)
```

---

## 7. 다음 작업

1. **DisplayResultText 구현** (가장 단순)
2. **EUserListTable 구현** (평가대상자 목록)
3. **MemberListTable 구현** (부서/직급별 사용자 목록)
4. **빌드 테스트 및 검증**
5. **Phase 3-3 Admin 페이지 완성**

---

**작성자**: Claude AI
**검토자**: 개발자
**승인 상태**: 참고용
