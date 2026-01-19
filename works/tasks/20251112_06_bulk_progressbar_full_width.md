# 작업지시서: 대량 문자 전송 프로그레스바 전체 너비 표시

**날짜**: 2025-11-12
**작업자**: Claude
**관련 이슈**: [#002: 대량 문자 전송 프로그레스바 전체 너비 표시](../issues/002_bulk_progressbar_full_width.md)

---

## 1. 작업 개요

**목적**: bulk-message.html의 대량 전송 프로그레스바를 모달 전체 너비만큼 길게 표시

**배경**:
- 현재 프로그레스바가 modal-footer 내부에 위치하여 표시는 되지만 너비가 작음
- 사용자가 전송 진행 상황을 명확히 보기 위해 모달 전체 너비만큼 길게 표시 필요

**수정 대상 파일**:
- `public/bulk-message.html`

---

## 2. 현재 상태 분석

### 2.1. 현재 HTML 구조 (라인 864-876)
```html
<!-- 프로그레스 바 (전송 중일 때만 표시) -->
<div v-if="isBulkSending" class="progress-container">
    <div class="progress-bar-wrapper">
        <div class="progress-bar" :style="{ width: bulkProgress + '%' }">
            {{ bulkProgress }}%
        </div>
    </div>
    <div class="progress-info">
        <span class="progress-status">
            <i class="fas fa-spinner fa-spin"></i> 전송 중... ({{ bulkSentCount }}/{{ totalPatientCount }})
        </span>
        <span>성공: {{ bulkSuccessCount }} | 실패: {{ bulkFailCount }}</span>
    </div>
</div>
```

### 2.2. 현재 CSS 스타일 (라인 502-540)
```css
.progress-container {
    margin: 20px 0;
    padding: 15px;
    background-color: #f8f9fa;
    border-radius: 4px;
}

.progress-bar-wrapper {
    width: 100%;
    height: 30px;
    background-color: #e9ecef;
    border-radius: 15px;
    overflow: hidden;
    margin-bottom: 10px;
}

.progress-bar {
    height: 100%;
    background: linear-gradient(90deg, #28a745 0%, #20c997 100%);
    transition: width 0.3s ease;
    display: flex;
    align-items: center;
    justify-content: center;
    color: white;
    font-weight: bold;
    font-size: 14px;
}

.progress-info {
    display: flex;
    justify-content: space-between;
    align-items: center;
    font-size: 14px;
    color: #495057;
}

.progress-status {
    font-weight: bold;
}
```

### 2.3. 문제점
- `.progress-container`가 modal-footer 내부의 padding과 margin에 영향을 받음
- 모달 전체 너비를 차지하지 못하고 footer의 padding 안쪽에만 표시됨

---

## 3. 수정 방안

### 3.1. CSS 수정 전략

**방법**: `.progress-container`에 음수 margin을 적용하여 부모(modal-footer)의 padding을 무시하고 모달 전체 너비 차지

**수정할 CSS**:
```css
.progress-container {
    margin: 20px -30px -20px -30px;  /* 상 우 하 좌 - footer의 padding 상쇄 */
    padding: 15px 30px;  /* 내부 여백은 유지 */
    background-color: #f8f9fa;
    border-radius: 0;  /* 모서리를 직각으로 변경 (모달 하단에 맞춤) */
    border-top: 1px solid #dee2e6;  /* 상단 구분선 추가 */
}
```

**설명**:
- `margin: 20px -30px -20px -30px`:
  - 상단: `20px` (footer 위쪽 요소와 간격)
  - 우/좌: `-30px` (modal-footer의 padding 30px를 상쇄하여 모달 양쪽 끝까지 확장)
  - 하단: `-20px` (modal-footer의 하단 padding을 상쇄하여 모달 하단에 딱 붙음)

- `padding: 15px 30px`: 내부 콘텐츠의 여백은 유지
- `border-radius: 0`: 모달 하단에 딱 맞게 직각으로 변경
- `border-top: 1px solid #dee2e6`: 상단에 구분선 추가하여 footer 버튼과 시각적 분리

### 3.2. modal-footer padding 확인 필요

작업 전에 `.modal-footer`의 실제 padding 값을 확인해야 합니다.
- 만약 padding이 30px이 아니라면 음수 margin 값을 조정해야 함
- 확인 위치: bulk-message.html의 CSS 영역에서 `.modal-footer` 스타일

---

## 4. 작업 단계

### 4.1. 사전 확인
1. `.modal-footer`의 padding 값 확인
2. 모달 컨테이너의 border-radius 값 확인 (하단 모서리 처리)

### 4.2. CSS 수정
1. `.progress-container` 스타일 수정
   - `margin` 값을 음수로 조정
   - `border-radius` 조정
   - `border-top` 추가

### 4.3. 추가 고려사항
- 프로그레스바가 길어지면서 가독성이 향상되어야 함
- 전송 진행 정보와 성공/실패 카운트가 명확히 보여야 함

---

## 5. 예상 결과

### Before (현재):
```
┌─────────────────────────────┐
│  modal-header               │
│  modal-body                 │
│  modal-footer               │
│    [검진] [취소] [대량전송] │
│    ┌──프로그레스바───┐      │ ← 짧음
│    └─────────────────┘      │
└─────────────────────────────┘
```

### After (수정 후):
```
┌─────────────────────────────┐
│  modal-header               │
│  modal-body                 │
│  modal-footer               │
│    [검진] [취소] [대량전송] │
├─────────────────────────────┤ ← 구분선
│ ████████████░░░░░░░░ 67%    │ ← 모달 전체 너비
│ 전송 중... (2/3) 성공: 2    │
└─────────────────────────────┘
```

---

## 6. 테스트 시나리오

### Test 1: 프로그레스바 너비 확인
1. search-details.html에서 환자 선택
2. 문자 전송 버튼 클릭
3. 템플릿 선택 후 대량 전송
4. **확인**: 프로그레스바가 모달 전체 너비를 차지하는지 확인
5. **확인**: 모달 양쪽 끝까지 프로그레스바가 표시되는지 확인

### Test 2: 시각적 구분
1. 전송 중 프로그레스바 관찰
2. **확인**: 상단 구분선이 footer 버튼과 프로그레스바를 명확히 구분하는지 확인
3. **확인**: 프로그레스바가 모달 하단에 딱 붙어있는지 확인

### Test 3: 반응형 테스트
1. 브라우저 창 크기 조절
2. **확인**: 모달 크기가 변해도 프로그레스바가 항상 전체 너비를 차지하는지 확인

### Test 4: 전송 완료 후
1. 전송 완료 대기
2. **확인**: 프로그레스바가 사라지고 결과 모달이 표시되는지 확인

---

## 7. 참고사항

- modal-footer의 padding 값이 30px이 아닐 경우 음수 margin 값을 조정해야 함
- 모달의 border-radius가 하단에 적용되어 있다면 프로그레스바도 같은 border-radius를 적용할 수 있음
- 다크모드가 있다면 프로그레스바 색상도 고려해야 함

---

## 8. 롤백 계획

문제 발생 시 원래 CSS 값으로 복구:
```css
.progress-container {
    margin: 20px 0;
    padding: 15px;
    background-color: #f8f9fa;
    border-radius: 4px;
}
```
