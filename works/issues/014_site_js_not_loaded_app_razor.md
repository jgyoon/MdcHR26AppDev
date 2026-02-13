# 이슈 #014: site.js 파일이 App.razor에 로드되지 않음

**날짜**: 2026-01-30
**상태**: 완료
**우선순위**: 중간
**관련 이슈**: [#012](012_v_processtrllistdb_view_column_mismatch.md), [#013](013_v_reporttasklistdb_entity_db_mismatch.md)

---

## 개발자 요청

**배경**:
- Issue #012, #013 런타임 테스트 중 발견
- `/Admin/TotalReport` 페이지는 정상 로드
- "평가리스트 다운로드" 버튼 클릭 시 JavaScript 오류 발생

**오류 현상**:
```
JSException: The value 'downloadURI' is not a function.
Error: The value 'downloadURI' is not a function.
```

**발생 위치**:
- `AdminViewExcel.ClickExportXLS()` - Line 44
- `/Admin/TotalReport` 페이지의 "평가리스트 다운로드" 버튼

**요청 사항**:
1. JavaScript 함수 `downloadURI`가 브라우저에서 사용 가능하도록 수정
2. App.razor에 site.js 스크립트 태그 추가

---

## 문제 분석

### 1. 원인

**site.js는 존재하지만 로드되지 않음**:
- `wwwroot/js/site.js` 파일에 `downloadURI` 함수 정의됨 (Line 2-10)
- `App.razor`에 해당 스크립트 태그가 없음
- blazor.web.js만 로드되어 있음 (Line 22)

### 2. 현재 상태

**site.js (정상)**:
```javascript
// 엑셀 파일 다운로드
window.downloadURI = function (uri, name) {
    var link = document.createElement("a");
    link.download = name;
    link.href = uri;
    link.target = "_blank";
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}
```

**App.razor (문제)**:
```html
<body>
    <Routes @rendermode="InteractiveServer" />
    <ReconnectModal />
    <script src="@Assets["_framework/blazor.web.js"]"></script>
    <!-- site.js 누락 -->
</body>
```

**AdminViewExcel.razor.cs (Line 44)**:
```csharp
await js.InvokeVoidAsync("downloadURI", fileUrl, fileName);
```

### 3. 영향 범위

**현재 영향**:
- AdminViewExcel 컴포넌트의 엑셀 다운로드 기능 작동 불가
- AdminTaskViewExcel 컴포넌트도 동일 문제 가능성

**잠재적 영향**:
- site.js에 정의된 다른 JavaScript 함수가 있다면 모두 사용 불가

---

## 해결 방안

### Option 1: App.razor에 스크립트 태그 추가 ✅ (권장)

**파일**: `MdcHR26Apps.BlazorServer/Components/App.razor`

**수정 위치**: Line 22 이후

**변경 전**:
```html
<body>
    <Routes @rendermode="InteractiveServer" />
    <ReconnectModal />
    <script src="@Assets["_framework/blazor.web.js"]"></script>
</body>
```

**변경 후**:
```html
<body>
    <Routes @rendermode="InteractiveServer" />
    <ReconnectModal />
    <script src="@Assets["_framework/blazor.web.js"]"></script>
    <script src="js/site.js"></script>
</body>
```

**장점**:
- 간단하고 명확한 수정
- .NET 10 Blazor 표준 방식
- 다른 JavaScript 함수도 함께 로드

---

### Option 2: C# 코드로 파일 다운로드 구현 ❌

**방법**: JavaScript 없이 C#만으로 다운로드 구현

**단점**:
- 코드 변경 범위가 큼
- Blazor Server에서 파일 다운로드는 JavaScript 사용이 표준

---

## 작업 계획

### Phase 1: App.razor 수정

**파일**: `MdcHR26Apps.BlazorServer/Components/App.razor`

1. Line 22 이후에 site.js 스크립트 태그 추가
2. 순서: blazor.web.js → site.js (Blazor 초기화 후 커스텀 스크립트 로드)

### Phase 2: 테스트

1. 애플리케이션 실행
2. `/Admin/TotalReport` 접속
3. "평가리스트 다운로드" 버튼 클릭
4. **기대 결과**: 엑셀 파일 다운로드 성공, JavaScript 오류 없음

### Phase 3: 다른 컴포넌트 확인

**확인 필요**:
- AdminTaskViewExcel.razor.cs - downloadURI 사용 여부 확인
- 다른 JavaScript 함수 사용 여부 확인

---

## 진행 사항

- [x] 문제 확인 및 원인 분석
- [x] App.razor 수정 (개발자)
- [x] 빌드 테스트
- [x] 런타임 테스트 (평가리스트 다운로드 정상 작동)
- [x] 확인 완료

---

## 관련 문서

**관련 이슈**:
- [#012: v_ProcessTRListDB View 컬럼 불일치 오류](012_v_processtrllistdb_view_column_mismatch.md) - 런타임 테스트 중 발견
- [#013: v_ReportTaskListDB Entity와 DB View 구조 불일치](013_v_reporttasklistdb_entity_db_mismatch.md) - 런타임 테스트 중 발견
- [#011: Phase 3-3 관리자 페이지 빌드 오류 및 재작업](011_phase3_3_admin_pages_build_errors.md) - 상위 이슈

**관련 파일**:
- `MdcHR26Apps.BlazorServer/Components/App.razor` - 수정 필요
- `MdcHR26Apps.BlazorServer/wwwroot/js/site.js` - JavaScript 함수 정의
- `MdcHR26Apps.BlazorServer/Components/Pages/Components/Common/AdminViewExcel.razor.cs` - downloadURI 호출
- `MdcHR26Apps.BlazorServer/Components/Pages/Components/Common/AdminTaskViewExcel.razor.cs` - 확인 필요

---

## 개발자 피드백

**작업 시작**: 2026-01-30
**작업 완료**: 2026-01-30
**최종 상태**: 완료 ✅
**비고**:
- Issue #012, #013 런타임 테스트 중 발견
- JavaScript 파일 site.js가 App.razor에 로드되지 않아 downloadURI 함수 사용 불가
- App.razor Line 23에 `<script src="js/site.js"></script>` 추가 (개발자 작업)
- 런타임 테스트 성공: 평가리스트 다운로드 기능 정상 작동, 엑셀 파일 다운로드 성공
