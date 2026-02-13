# 동기화 체크리스트

**생성일시**: 2026-01-30 13:45
**현재 커밋**: c675790 (feat: Phase 3-3 TotalReport 관리자 페이지 구현 완료)
**마지막 동기화**: db3f4e1
**변경 파일**: 23개 (동기화 대상)

---

## 📋 작업 파일 목록

### 1. 생성 (12개):

**Admin/TotalReport 페이지:**
- MdcHR26Apps.BlazorServer/Components/Pages/Admin/TotalReport/Details.razor
- MdcHR26Apps.BlazorServer/Components/Pages/Admin/TotalReport/Details.razor.cs
- MdcHR26Apps.BlazorServer/Components/Pages/Admin/TotalReport/Edit.razor
- MdcHR26Apps.BlazorServer/Components/Pages/Admin/TotalReport/Edit.razor.cs
- MdcHR26Apps.BlazorServer/Components/Pages/Admin/TotalReport/Index.razor
- MdcHR26Apps.BlazorServer/Components/Pages/Admin/TotalReport/Index.razor.cs
- MdcHR26Apps.BlazorServer/Components/Pages/Admin/TotalReport/ReportInit.razor
- MdcHR26Apps.BlazorServer/Components/Pages/Admin/TotalReport/ReportInit.razor.cs

**공통 컴포넌트:**
- MdcHR26Apps.BlazorServer/Components/Pages/Components/Common/AdminTaskViewExcel.razor
- MdcHR26Apps.BlazorServer/Components/Pages/Components/Common/AdminTaskViewExcel.razor.cs
- MdcHR26Apps.BlazorServer/Components/Pages/Components/Common/AdminViewExcel.razor
- MdcHR26Apps.BlazorServer/Components/Pages/Components/Common/AdminViewExcel.razor.cs
- MdcHR26Apps.BlazorServer/Components/Pages/Components/Modal/ReportInitModal.razor
- MdcHR26Apps.BlazorServer/Components/Pages/Components/Table/AdminReportListView.razor

**유틸리티 및 모델:**
- MdcHR26Apps.BlazorServer/Models/TotalScoreRankModel.cs
- MdcHR26Apps.BlazorServer/Utils/ExcelManage.cs
- MdcHR26Apps.BlazorServer/Utils/ScoreUtils.cs

**wwwroot:**
- MdcHR26Apps.BlazorServer/wwwroot/files/tasks/file_tasks.html
- MdcHR26Apps.BlazorServer/wwwroot/js/site.js

### 2. 수정 (23개):

**Blazor Server:**
- MdcHR26Apps.BlazorServer/Components/App.razor
- MdcHR26Apps.BlazorServer/Components/Pages/Admin/EvaluationUsers/Details.razor.cs
- MdcHR26Apps.BlazorServer/Components/Pages/Admin/EvaluationUsers/Edit.razor.cs
- MdcHR26Apps.BlazorServer/Components/Pages/Admin/Index.razor
- MdcHR26Apps.BlazorServer/Components/Pages/Admin/Index.razor.cs
- MdcHR26Apps.BlazorServer/Components/Pages/Admin/Users/Create.razor.cs
- MdcHR26Apps.BlazorServer/Data/UrlActions.cs
- MdcHR26Apps.BlazorServer/Program.cs

**Models - Repository:**
- MdcHR26Apps.Models/EvaluationAgreement/AgreementRepository.cs
- MdcHR26Apps.Models/EvaluationAgreement/IAgreementRepository.cs
- MdcHR26Apps.Models/EvaluationReport/IReportRepository.cs
- MdcHR26Apps.Models/EvaluationReport/ReportRepository.cs
- MdcHR26Apps.Models/EvaluationSubAgreement/ISubAgreementRepository.cs
- MdcHR26Apps.Models/EvaluationSubAgreement/SubAgreementRepository.cs
- MdcHR26Apps.Models/EvaluationTasks/ITasksRepository.cs
- MdcHR26Apps.Models/EvaluationTasks/TasksRepository.cs
- MdcHR26Apps.Models/Result/ITotalReportRepository.cs
- MdcHR26Apps.Models/Result/TotalReportRepository.cs

**Models - Views:**
- MdcHR26Apps.Models/Views/v_DeptObjectiveListDb/v_DeptObjectiveListDb.cs
- MdcHR26Apps.Models/Views/v_ProcessTRListDB/Iv_ProcessTRListRepository.cs
- MdcHR26Apps.Models/Views/v_ProcessTRListDB/v_ProcessTRListDB.cs
- MdcHR26Apps.Models/Views/v_ProcessTRListDB/v_ProcessTRListRepository.cs
- MdcHR26Apps.Models/Views/v_ReportTaskListDB/Iv_ReportTaskListRepository.cs
- MdcHR26Apps.Models/Views/v_ReportTaskListDB/v_ReportTaskListDB.cs
- MdcHR26Apps.Models/Views/v_ReportTaskListDB/v_ReportTaskListRepository.cs
- MdcHR26Apps.Models/Views/v_TotalReportListDB/v_TotalReportListDB.cs

**Models - DbContext:**
- MdcHR26Apps.Models/MdcHR26AppsAddDbContext.cs

### 3. 삭제:

없음

---

## 📌 메모

**커밋 범위**: db3f4e1..c675790 (8개 커밋)

**주요 변경 사항**:

1. **Phase 3-3 TotalReport 관리자 페이지 구현 완료**
   - Admin/TotalReport 페이지 전체 구현 (Index, Details, Edit, ReportInit)
   - 엑셀 다운로드 컴포넌트 추가 (AdminViewExcel, AdminTaskViewExcel)
   - 리포트 초기화 모달 추가 (ReportInitModal)
   - 관리자용 리포트 리스트 뷰 (AdminReportListView)

2. **DB View 동기화**
   - v_ProcessTRListDB 컬럼 불일치 수정 (Issue #012)
   - v_ReportTaskListDB 엔티티/DB 불일치 수정 (Issue #013)
   - v_EvaluationUsersList 뷰 추가

3. **엑셀 다운로드 및 유틸리티**
   - ExcelManage.cs 추가 (엑셀 생성 및 관리)
   - ScoreUtils.cs 추가 (점수 계산 유틸리티)
   - TotalScoreRankModel.cs 추가

4. **site.js 추가 및 App.razor 수정**
   - site.js 파일 추가 (Issue #014)
   - App.razor에서 site.js 로딩

5. **Repository 인터페이스 및 구현 개선**
   - 2026년 DB 구조에 맞춰 전체 동기화
   - GetByYearsAsync 등 메서드 추가

---

## 🔍 제외된 파일 (동기화 불필요)

**Database 폴더** (개발자가 양쪽 프로젝트에서 직접 작업):
- Database/02_CreateViews.sql
- Database/dbo/v_EvaluationUsersList.sql
- Database/dbo/v_ReportTaskListDB.sql

**Claude 및 문서 폴더**:
- .claude/ (2개 파일)
- .claude/settings.local.json
- works/ (9개 파일)
- claude.md
- temp_validate.ps1

---

## ✅ 동기화 절차

1. **생성 파일 (12개)**: 현재 프로젝트 → 실제 프로젝트로 복사
2. **수정 파일 (23개)**: 현재 프로젝트 → 실제 프로젝트로 덮어쓰기
3. **Visual Studio 2022에서 빌드 확인**
4. **수동 테스트**
5. **Git commit**

---

**완료 일시**: __________
