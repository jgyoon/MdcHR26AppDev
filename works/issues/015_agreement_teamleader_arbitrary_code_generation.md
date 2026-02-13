# 이슈 #015: Agreement TeamLeader 페이지 - 임의 코드 작성으로 인한 디버깅 어려움

**날짜**: 2026-02-04
**상태**: 완료
**우선순위**: 높음
**관련 이슈**: [#009](009_phase3_webapp_development.md)

---

## 개발자 요청

**배경**:
- Phase 3-4 Agreement TeamLeader 페이지 개발 중
- 25년도 코드를 26년도로 이전하는 작업
- 26년도 DB는 `UserId` (string) → `Uid` (long) 변경

**문제 발생**:
- Claude가 25년도 코드를 단순 복사하지 않고 **임의로 새로운 코드를 작성**함
- 25년도 코드와 완전히 다른 구조로 구현되어 디버깅에 많은 시간 소요
- DB 변경사항만 반영하면 되는데, 불필요한 수정으로 인해 작업 복잡도 증가

**요청 사항**:
1. 이 이슈를 기록하여 추후 동일한 문제가 발생하지 않도록 함
2. **25년도 코드 복사 → 26년도 DB 변경사항 반영** 방식으로 작업할 것
3. 임의로 코드를 재작성하거나 구조를 변경하지 말 것

---

## 문제 상황

### 1. 증상

**원래 기대**:
- 25년도 `Agreement/TeamLeader/Index.razor` 복사
- 25년도 `Agreement/TeamLeader/Details.razor` 복사
- DB 변경사항만 수정: `UserId` → `Uid`

**실제 발생**:
- 26년도 Details 페이지가 **완전히 다른 구조**로 구현됨
- 25년도: 합의 승인/반려 기능이 있는 워크플로우 페이지
- 26년도 (Claude 작성): 단순 조회만 가능한 디스플레이 페이지
- **핵심 기능 누락**: 합의 코멘트 입력, 승인/반려 버튼, ProcessDb 업데이트 로직

### 2. 발견 과정

**단계별 문제 발생**:

1. **첫 번째 문제**: TeamLeader/Index 페이지에서 사용자 이름/부서 표시 안 됨
   - 원인: ProcessRepository에서 Navigation Properties 로드 안 함
   - 해결: View Table (v_ProcessTRListDB) 사용으로 변경

2. **두 번째 문제**: Details 페이지에서 "데이터를 찾을 수 없습니다" 에러
   - 원인: Parameter 불일치 (Index는 Uid 전달, Details는 Aid 기대)
   - 임시 수정: Parameter를 Uid로 변경

3. **세 번째 문제 (핵심)**: 사용자가 25년도와 26년도 Details 페이지 비교 후 발견
   - **사용자 피드백**: "레이아웃이 문제가 아니라 25년도는 합의 코멘트를 하고 승인 또는 반려하는 기능이 있는데 누락된 것 같네요"
   - **사용자 피드백**: "애초에 25년 Agreement TeamLeader 페이지의 Index와 Details와 전혀 다르네요. 임의로 코드를 작성했나요?"

### 3. 임의 코드 작성 내용

**26년도 Details.razor.cs (Claude가 임의로 작성)**:
```csharp
// ❌ 잘못된 구조
[Parameter]
public Int64 Uid { get; set; }  // 원래는 Id (ProcessDb.Pid)여야 함

public AgreementDb? model { get; set; }  // 원래는 List<AgreementDb>여야 함

private async Task LoadData()
{
    model = await agreementRepository.GetByIdAsync(Uid);  // ❌ Agreement 1개만 로드
    // ❌ ProcessDb 로드하지 않음
    // ❌ 승인/반려 로직 없음
}
```

**25년도 Details.razor.cs (올바른 구조)**:
```csharp
// ✅ 올바른 구조
[Parameter]
public Int64 Id { get; set; }  // ProcessDb.Pid

public ProcessDb processDb { get; set; } = new ProcessDb();
public List<AgreementDb> model { get; set; } = new List<AgreementDb>();

private async Task SetData(Int64 Id)
{
    processDb = await processDbRepository.GetByIdAsync(Id);  // ✅ ProcessDb 로드
    model = await agreementDbRepository.GetByUserIdAllAsync(processDb.UserId);  // ✅ 모든 Agreement 로드
    userName = processDb.UserName;
}

private async Task AgreementConfirm() { /* 승인 로직 */ }
private async Task AgreementRefer() { /* 반려 로직 */ }
```

---

## 해결 방안

### 1. 올바른 작업 방식

**원칙**:
1. ✅ **25년도 코드를 그대로 복사**
2. ✅ **26년도 DB 변경사항만 수정**
   - `UserId` (string) → `Uid` (long)
   - `GetByUserIdAllAsync(processDb.UserId)` → `GetByUserIdAllAsync(processDb.Uid)`
3. ❌ **임의로 코드 재작성 금지**
4. ❌ **구조 변경 금지**

### 2. 실제 적용한 해결책

**작업지시서 작성**:
- `20260204_11_agreement_teamleader_details_fix_approval_workflow.md`
- 25년도 코드를 기준으로 상세한 작업지시서 작성
- 26년도 DB 변경사항 명시

**코드 수정**:
1. Details.razor.cs 전체 재작성 (25년도 기준)
2. Details.razor UI 전체 재작성 (25년도 기준)
3. 26년도 DB 변경사항 반영:
   - `processDb.UserId` → `processDb.Uid`
   - `urlActions.MoveTeamLeaderAgreementMainPage()` → `urlActions.MoveAgreementTeamLeaderIndexPage()`
   - `UpdateAsync` 반환 타입: `bool` → `int`

**추가 수정**:
4. v_AgreementDB View 및 Repository 사용
5. AgreementDetailsTableList 컴포넌트 생성 (리스트 표시용)
6. Index 페이지 라우트 수정
7. AgreementDbListView 라우트 수정
8. 합의 완료 시 "상세" 버튼 숨김 처리

---

## 진행 사항

- [x] 문제 발견 (사용자 피드백)
- [x] 작업지시서 작성 (`20260204_11_agreement_teamleader_details_fix_approval_workflow.md`)
- [x] Details.razor.cs 재작성 (25년도 기준)
- [x] Details.razor UI 재작성 (25년도 기준)
- [x] 26년도 DB 변경사항 반영 (`UserId` → `Uid`)
- [x] v_AgreementDB View 사용
- [x] AgreementDetailsTableList 컴포넌트 생성
- [x] Index 페이지 AgreementListTable 수정 (ProcessDb 리스트용)
- [x] AgreementDbListView 라우트 수정
- [x] 합의 완료 시 "상세" 버튼 숨김 처리
- [x] 빌드 테스트 (0 errors)
- [x] 기능 테스트 (TeamLeader 승인/반려)
- [x] 기능 테스트 (User 상세 페이지)

---

## 테스트 결과

### Test 1: TeamLeader Index 페이지
**결과**: ✅ 성공
- 팀원 리스트 표시 (이름, 요청여부, 승인여부)
- Is_Request = true, Is_Agreement = false인 팀원만 "합의" 버튼 표시

### Test 2: TeamLeader Details 페이지 (합의 승인)
**결과**: ✅ 성공
- ProcessDb.Pid로 Details 페이지 이동
- 사용자 이름, 모든 Agreement 리스트 표시
- 합의 코멘트 입력 후 "승인" 버튼 클릭
- ProcessDb.Is_Agreement = true 업데이트
- Index 페이지로 리다이렉트

### Test 3: User Agreement 상세 페이지
**결과**: ✅ 성공
- Agreement 리스트에서 "상세" 버튼 클릭
- `/Agreement/User/Details/{aid}` 페이지로 이동
- Agreement 상세 정보 표시

### Test 4: 합의 완료 시 "상세" 버튼 숨김
**결과**: ✅ 성공
- Is_Agreement = false: "상세" 버튼 표시
- Is_Agreement = true: "완료" 텍스트 표시 (버튼 숨김)

---

## 교훈 및 개선 사항

### 1. 작업 원칙

**DO (해야 할 것)**:
- ✅ 25년도 코드를 **그대로 복사**
- ✅ 26년도 DB 변경사항**만** 수정
- ✅ 작업 전 25년도 코드 **철저히 분석**
- ✅ 작업지시서에 25년도 코드 **명시**

**DON'T (하지 말아야 할 것)**:
- ❌ 임의로 코드 재작성
- ❌ 구조 변경
- ❌ 기능 단순화
- ❌ "더 나은 방법"으로 개선 시도

### 2. 체크리스트

**코드 이전 작업 시**:
1. [ ] 25년도 코드 전체 읽기
2. [ ] 25년도 코드 구조 이해
3. [ ] 26년도 DB 변경사항 확인
4. [ ] 작업지시서 작성 (25년도 코드 포함)
5. [ ] **복사 → 수정** 방식으로 작업
6. [ ] 25년도 기능과 동일한지 확인

### 3. View Table 사용 여부

**판단 기준**:
- Details 페이지: View Table **불필요** (ProcessDb + AgreementDb 직접 조회)
- Index 페이지: View Table **필요** (v_ProcessTRListDB - UserName 포함)
- 25년도 코드가 View를 사용하지 않으면, 26년도도 사용하지 않음

---

## 관련 문서

**작업지시서**:
- [20260204_11_agreement_teamleader_details_fix_approval_workflow.md](../tasks/20260204_11_agreement_teamleader_details_fix_approval_workflow.md)

**관련 이슈**:
- [#009: Phase 3 Blazor Server WebApp 개발](009_phase3_webapp_development.md) - Phase 3-4 진행 중

**수정된 파일**:
- `Components/Pages/Agreement/TeamLeader/Details.razor`
- `Components/Pages/Agreement/TeamLeader/Details.razor.cs`
- `Components/Pages/Agreement/TeamLeader/Index.razor`
- `Components/Pages/Components/Agreement/AgreementListTable.razor`
- `Components/Pages/Components/Agreement/AgreementListTable.razor.cs`
- `Components/Pages/Components/Agreement/AgreementDetailsTableList.razor` (신규)
- `Components/Pages/Components/Agreement/AgreementDetailsTableList.razor.cs` (신규)
- `Components/Pages/Components/Agreement/ViewPage/AgreementDbListView.razor`
- `Components/Pages/Components/Agreement/ViewPage/AgreementDbListView.razor.cs`
- `Components/Pages/Components/Agreement/AgreementDbListTable.razor`
- `Components/Pages/Components/Agreement/AgreementDbListTable.razor.cs`

---

## 개발자 피드백

**작업 완료 확인**: 2026-02-04
**최종 상태**: 완료
**비고**:
- Agreement TeamLeader 페이지 정상 작동 확인
- 25년도 코드와 동일한 기능 구현 완료
- 향후 작업 시 이 이슈를 참조하여 동일한 문제 방지

---

## 요약

**문제**: Claude가 25년도 코드를 임의로 재작성하여 디버깅 어려움
**원인**: 25년도 코드 구조를 무시하고 독자적으로 구현
**해결**: 작업지시서 작성 후 25년도 코드 기준으로 재작성
**교훈**: **복사 → 수정** 방식 준수, 임의 코드 작성 금지
