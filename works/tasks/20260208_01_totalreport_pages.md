# 작업지시서: TotalReport 페이지 추가 (TeamLeader, User, 공통)

**날짜**: 2026-02-08
**작업 유형**: 기능 추가
**관련 이슈**: [#009: Phase 3 WebApp Development](../issues/009_phase3_webapp_development.md)

---

## 1. 작업 개요

### 1.1. 현재 상황
- 26년도 프로젝트에는 **Admin/TotalReport** 페이지만 완성됨
- 25년도 프로젝트에는 전체 TotalReport 구조가 존재함
  - TotalReport/Index.razor (메인)
  - TotalReport/Result.razor (결과 조회)
  - TotalReport/TeamLeader/Index.razor (팀장용)
  - TotalReport/TeamLeader/CompleteDetails.razor (완료 상세)

### 1.2. 작업 목표
- 25년도 TotalReport 페이지를 26년도로 마이그레이션
- 26년도 DB 구조에 맞게 수정
- Issue #015 원칙 준수: 25년도 코드 복사 후 DB 변경사항만 반영

---

## 2. 파일 구조 비교

### 2.1. 25년도 구조
```
MdcHR25Apps.BlazorApp/Pages/TotalReport/
├── Index.razor                    # 메인 페이지
├── Index.razor.cs
├── Result.razor                   # 결과 조회
├── Result.razor.cs
├── Admin/
│   ├── Details.razor             # ✅ 이미 완성
│   ├── Edit.razor                # ✅ 이미 완성
│   ├── Index.razor               # ✅ 이미 완성
│   └── ReportInit.razor          # ✅ 이미 완성
└── TeamLeader/
    ├── Index.razor               # 팀장 목록
    ├── Index.razor.cs
    ├── CompleteDetails.razor     # 완료 상세
    └── CompleteDetails.razor.cs
```

### 2.2. 26년도 현재 구조
```
MdcHR26Apps.BlazorServer/Components/Pages/
└── Admin/TotalReport/
    ├── Details.razor             # ✅ 완성
    ├── Edit.razor                # ✅ 완성
    ├── Index.razor               # ✅ 완성
    └── ReportInit.razor          # ✅ 완성
```

### 2.3. 26년도 목표 구조
```
MdcHR26Apps.BlazorServer/Components/Pages/
├── TotalReport/                  # ⬅️ 신규 추가
│   ├── Index.razor              # 메인 페이지
│   ├── Index.razor.cs
│   ├── Result.razor             # 결과 조회
│   ├── Result.razor.cs
│   └── TeamLeader/              # ⬅️ 신규 추가
│       ├── Index.razor          # 팀장 목록
│       ├── Index.razor.cs
│       ├── CompleteDetails.razor
│       └── CompleteDetails.razor.cs
└── Admin/TotalReport/           # ✅ 이미 완성
    ├── Details.razor
    ├── Edit.razor
    ├── Index.razor
    └── ReportInit.razor
```

---

## 3. 작업 내용

### 3.1. 파일 생성 목록

#### 📂 TotalReport/ (메인 페이지)
1. **Index.razor** + **Index.razor.cs**
   - 경로: `MdcHR26Apps.BlazorServer/Components/Pages/TotalReport/`
   - 용도: TotalReport 메인 페이지 (사용자 역할별 라우팅)
   - 25년도 소스: `MdcHR25Apps.BlazorApp/Pages/TotalReport/Index.razor`

2. **Result.razor** + **Result.razor.cs**
   - 경로: `MdcHR26Apps.BlazorServer/Components/Pages/TotalReport/`
   - 용도: 평가 결과 조회 페이지
   - 25년도 소스: `MdcHR25Apps.BlazorApp/Pages/TotalReport/Result.razor`

#### 📂 TotalReport/TeamLeader/ (팀장용)
3. **Index.razor** + **Index.razor.cs**
   - 경로: `MdcHR26Apps.BlazorServer/Components/Pages/TotalReport/TeamLeader/`
   - 용도: 팀장용 전체 평가 목록
   - 25년도 소스: `MdcHR25Apps.BlazorApp/Pages/TotalReport/TeamLeader/Index.razor`

4. **CompleteDetails.razor** + **CompleteDetails.razor.cs**
   - 경로: `MdcHR26Apps.BlazorServer/Components/Pages/TotalReport/TeamLeader/`
   - 용도: 팀장이 확인하는 완료된 평가 상세
   - 25년도 소스: `MdcHR25Apps.BlazorApp/Pages/TotalReport/TeamLeader/CompleteDetails.razor`

---

## 4. 26년도 DB 변경사항 반영

### 4.1. 네임스페이스 변경
```csharp
// 25년도
using MdcHR25Apps.BlazorApp.Data;
using MdcHR25Apps.Models.Result.ListDB;

// 26년도
using MdcHR26Apps.BlazorServer.Data;
using MdcHR26Apps.Models.Views.v_TotalReportListDB;
using MdcHR26Apps.Models.Views.v_ProcessTRListDB;
```

### 4.2. 모델 변경
```csharp
// 25년도
v_TotalReportListDB (MdcHR25Apps.Models.Result.ListDB)

// 26년도
v_TotalReportListDB (MdcHR26Apps.Models.Views.v_TotalReportListDB)
v_ProcessTRListDB (MdcHR26Apps.Models.Views.v_ProcessTRListDB)
```

### 4.3. Repository 반환 타입
```csharp
// 25년도
if (await repository.UpdateAsync(model))

// 26년도
int affectedRows = await repository.UpdateAsync(model);
if (affectedRows > 0)
```

### 4.4. Page Directive
```razor
<!-- 25년도 -->
@page "/TotalReport"

<!-- 26년도 -->
@page "/totalreport"
@rendermode InteractiveServer
```

### 4.5. v_ProcessTRListDB 뷰 사용
- **25년도**: ProcessDb 테이블 직접 조회
- **26년도**: v_ProcessTRListDB 뷰 사용 (이미 Issue #015에서 추가됨)

```csharp
// 26년도에서 추가로 확인할 필드
v_ProcessTRListDB.TeamLeader_Score  // ✅ 최근 추가됨
v_ProcessTRListDB.Director_Score
```

---

## 5. 작업 절차

### 5.1. 폴더 생성
```bash
MdcHR26Apps.BlazorServer/Components/Pages/TotalReport/
MdcHR26Apps.BlazorServer/Components/Pages/TotalReport/TeamLeader/
```

### 5.2. 파일별 작업 순서

#### Step 1: TotalReport/Index.razor
1. 25년도 `Index.razor` 복사
2. 네임스페이스 변경
3. @rendermode InteractiveServer 추가
4. 라우팅 경로 확인 (/totalreport)

#### Step 2: TotalReport/Index.razor.cs
1. 25년도 `Index.razor.cs` 복사
2. 네임스페이스 변경
3. LoginStatusService 주입 확인
4. UrlActions 메서드 확인

#### Step 3: TotalReport/Result.razor
1. 25년도 `Result.razor` 복사
2. v_TotalReportListDB 사용 확인
3. 컴포넌트 파라미터 확인

#### Step 4: TotalReport/Result.razor.cs
1. 25년도 `Result.razor.cs` 복사
2. Repository 사용 확인
3. v_TotalReportListDB 조회 로직 확인

#### Step 5: TotalReport/TeamLeader/Index.razor
1. 25년도 `TeamLeader/Index.razor` 복사
2. v_ProcessTRListDB 사용 확인
3. TeamLeader_Score 필드 사용

#### Step 6: TotalReport/TeamLeader/Index.razor.cs
1. 25년도 `TeamLeader/Index.razor.cs` 복사
2. v_ProcessTRListDB Repository 주입
3. TeamLeader 권한 확인 로직

#### Step 7: TotalReport/TeamLeader/CompleteDetails.razor
1. 25년도 `TeamLeader/CompleteDetails.razor` 복사
2. v_TotalReportListDB 파라미터 확인

#### Step 8: TotalReport/TeamLeader/CompleteDetails.razor.cs
1. 25년도 `TeamLeader/CompleteDetails.razor.cs` 복사
2. Repository 주입 확인

---

## 6. UrlActions 메서드 추가 필요

### 6.1. 추가할 메서드
```csharp
// TotalReport 메인
public void MoveTotalReportMainPage() =>
    _navigationManager.NavigateTo("/totalreport");

// TotalReport 결과
public void MoveTotalReportResultPage(long uid) =>
    _navigationManager.NavigateTo($"/totalreport/result/{uid}");

// TeamLeader TotalReport
public void MoveTeamLeaderTotalReportIndexPage() =>
    _navigationManager.NavigateTo("/totalreport/teamleader");

public void MoveTeamLeaderCompleteDetailsPage(long pid) =>
    _navigationManager.NavigateTo($"/totalreport/teamleader/completedetails/{pid}");
```

**파일 위치**: `MdcHR26Apps.BlazorServer/Data/UrlActions.cs`

---

## 7. 테스트 항목

### 7.1. TotalReport/Index.razor 테스트
**시나리오 1: 관리자 로그인**
1. 관리자 계정으로 로그인
2. `/totalreport` 페이지 접근
3. **확인**: Admin TotalReport 페이지로 리다이렉트 ✅

**시나리오 2: 팀장 로그인**
1. 팀장 계정으로 로그인
2. `/totalreport` 페이지 접근
3. **확인**: TeamLeader TotalReport 페이지로 리다이렉트 ✅

**시나리오 3: 일반 사용자 로그인**
1. 일반 사용자 계정으로 로그인
2. `/totalreport` 페이지 접근
3. **확인**: 메인 페이지로 리다이렉트 또는 권한 없음 메시지 ✅

### 7.2. TotalReport/Result.razor 테스트
**시나리오 1: 결과 조회**
1. `/totalreport/result/{uid}` 페이지 접근
2. **확인**: v_TotalReportListDB 데이터 표시 ✅
3. **확인**: 본인평가, 팀장평가, 임원평가 점수 표시 ✅

### 7.3. TotalReport/TeamLeader/Index.razor 테스트
**시나리오 1: 팀장 목록 표시**
1. 팀장 계정으로 로그인
2. `/totalreport/teamleader` 페이지 접근
3. **확인**: v_ProcessTRListDB 목록 표시 ✅
4. **확인**: TeamLeader_Score 필드 정상 표시 ✅

**시나리오 2: 완료 상세 이동**
1. 목록에서 "완료(상세)" 버튼 클릭
2. **확인**: CompleteDetails 페이지로 이동 ✅

### 7.4. TotalReport/TeamLeader/CompleteDetails.razor 테스트
**시나리오 1: 완료 상세 표시**
1. `/totalreport/teamleader/completedetails/{pid}` 접근
2. **확인**: v_TotalReportListDB 데이터 표시 ✅
3. **확인**: 팀장 코멘트, 피드백 코멘트 표시 ✅

---

## 8. 주의사항

1. **Issue #015 원칙 준수**:
   - 25년도 코드를 그대로 복사
   - DB 변경사항만 수정 (네임스페이스, 모델명, Repository 반환 타입)

2. **v_ProcessTRListDB.TeamLeader_Score 필드**:
   - 최근에 추가된 필드 (Issue #016)
   - 25년도 코드에서 사용하던 방식 그대로 유지

3. **권한 확인**:
   - 각 페이지에서 LoginStatusService를 통한 권한 확인 필수
   - Admin, TeamLeader, User 역할 구분

4. **라우팅 경로**:
   - 25년도: `/TotalReport` (대문자)
   - 26년도: `/totalreport` (소문자) - 기존 Admin 경로와 통일

5. **컴포넌트 재사용**:
   - 기존에 작성한 ViewPage 컴포넌트 활용
   - User_TotalReportListView
   - FeedBack_TotalReportListView
   - Complete_TotalReportListView

---

## 9. 완료 조건

- [ ] TotalReport/Index.razor 생성 및 테스트
- [ ] TotalReport/Index.razor.cs 생성 및 테스트
- [ ] TotalReport/Result.razor 생성 및 테스트
- [ ] TotalReport/Result.razor.cs 생성 및 테스트
- [ ] TotalReport/TeamLeader/Index.razor 생성 및 테스트
- [ ] TotalReport/TeamLeader/Index.razor.cs 생성 및 테스트
- [ ] TotalReport/TeamLeader/CompleteDetails.razor 생성 및 테스트
- [ ] TotalReport/TeamLeader/CompleteDetails.razor.cs 생성 및 테스트
- [ ] UrlActions 메서드 4개 추가
- [ ] 관리자 권한 테스트 성공
- [ ] 팀장 권한 테스트 성공
- [ ] 일반 사용자 권한 테스트 성공
- [ ] v_TotalReportListDB 데이터 정상 표시
- [ ] TeamLeader_Score 필드 정상 표시

---

## 10. 예상 소요 시간

- **파일 생성 및 복사**: 30분
- **26년도 DB 변경사항 반영**: 1시간
- **UrlActions 메서드 추가**: 15분
- **테스트 및 디버깅**: 1시간
- **총 예상 시간**: **2시간 45분**

---

## 11. 후속 작업

- User용 TotalReport 페이지 (필요 시)
- Excel 다운로드 기능 추가 (DirectorViewExcel, TeamLeaderViewExcel)
- 피드백 페이지 추가 (필요 시)
