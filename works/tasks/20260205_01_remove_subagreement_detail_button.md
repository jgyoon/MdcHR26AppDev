# 작업지시서 (간소화): 세부직무 목록 상세 버튼 제거

**날짜**: 2026-02-05
**파일**: `MdcHR26Apps.BlazorServer/Components/Pages/Components/SubAgreement/SubAgreementDbListTable.razor`
**작업 유형**: UI 개선
**관련 이슈**: 없음

---

## 1. 작업 개요

**목적**:
- 25년 화면과 동일하게 세부직무 목록에서 "상세" 버튼/비고 컬럼을 제거

**배경**:
- 26년 화면에는 "상세" 버튼이 표시되나 25년 화면에는 없음

---

## 2. 수정 내용

### 2.1. 파일: `MdcHR26Apps.BlazorServer/Components/Pages/Components/SubAgreement/SubAgreementDbListTable.razor`

**위치**: 테이블 헤더/바디의 비고 컬럼

**변경 사항**:
```razor
// 기존 코드
<th class="table-header">비고</th>
...
<td class="table-cell">
    <button type="button" id="DetailButton" class="btn btn-info" @onclick="@(() => OnDetailsClick.InvokeAsync(item.Sid))">상세</button>
</td>

// 수정 후 코드
// 비고 컬럼 및 상세 버튼 제거
```

**설명**:
- 25년 UI와 동일하게 상세 버튼 제거
- 세부직무 목록의 컬럼 수를 25년과 동일하게 유지

---

## 3. 테스트 항목

개발자가 테스트할 항목:

### Test 1: 세부직무 목록 버튼 확인
1. `세부직무작성 > 세부직무 목록` 페이지 진입
2. 목록 테이블 확인
3. **확인**: "비고" 컬럼과 "상세" 버튼이 표시되지 않음

### Test 2: 회귀 테스트
- 세부직무 목록 데이터 렌더링 정상 여부 확인

---

## 4. 완료 조건

- [ ] 코드 수정 완료
- [ ] Test 1 성공
- [ ] Test 2 성공
