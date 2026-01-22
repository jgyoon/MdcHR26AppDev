# Test-Gen - Playwright 테스트 시나리오 자동 생성

작업지시서를 분석하여 Playwright 테스트 시나리오를 자동으로 생성하는 스킬입니다.

## 사용법

```
/test-gen [작업지시서번호]
/test-gen
```

## 예시

```
/test-gen 20260121_01
/test-gen
```

## 동작

### 1. 작업지시서 분석

**입력**:
- 작업지시서 번호 지정 시: `works/tasks/YYYYMMDD_NN_*.md`
- 미지정 시: 가장 최근 작업지시서 자동 선택

**분석 항목**:
1. **변경된 페이지/컴포넌트**
   - 생성된 파일: 새로운 페이지, 컴포넌트
   - 수정된 파일: 기존 기능 변경

2. **주요 기능**
   - 사용자 인터랙션 (버튼 클릭, 입력 등)
   - 데이터 처리 (CRUD 작업)
   - 네비게이션 (페이지 이동)

3. **예상 동작**
   - 정상 케이스
   - 에러 케이스
   - 경계 케이스

4. **테스트 우선순위**
   - 핵심 기능 우선
   - 사용자 경험 영향도 고려

---

### 2. 테스트 시나리오 생성

**출력 위치**: `MdcHR26Apps.BlazorServer/tests/`

**파일명 규칙**:
- `{기능명}_test.spec.js`
- 예: `login_test.spec.js`, `user_management_test.spec.js`

**테스트 유형** (상황에 맞춰 생성):

#### A. UI 테스트
```javascript
test('페이지 로드 확인', async ({ page }) => {
  await page.goto('/Login');
  await expect(page).toHaveTitle(/로그인/);
  await expect(page.locator('h1')).toContainText('로그인');
});
```

#### B. 기능 테스트
```javascript
test('로그인 성공', async ({ page }) => {
  await page.goto('/Login');
  await page.fill('input[name="userid"]', 'test_user');
  await page.fill('input[name="password"]', 'test_password');
  await page.click('button[type="submit"]');
  await expect(page).toHaveURL('/');
});
```

#### C. 에러 케이스 테스트
```javascript
test('잘못된 비밀번호', async ({ page }) => {
  await page.goto('/Login');
  await page.fill('input[name="userid"]', 'test_user');
  await page.fill('input[name="password"]', 'wrong_password');
  await page.click('button[type="submit"]');
  await expect(page.locator('.error-message')).toBeVisible();
});
```

#### D. 전체 플로우 테스트
```javascript
test('사용자 등록 → 로그인 → 데이터 입력 플로우', async ({ page }) => {
  // 여러 단계를 포함한 통합 테스트
});
```

---

### 3. 테스트 파일 구조

```javascript
// tests/{기능명}_test.spec.js
// 작업지시서: YYYYMMDD_NN_작업명.md 기반 생성

import { test, expect } from '@playwright/test';

test.describe('{기능명} 테스트', () => {

  test.beforeEach(async ({ page }) => {
    // 공통 설정 (필요 시)
  });

  test('테스트 케이스 1: {설명}', async ({ page }) => {
    // Given: 초기 상태
    // When: 동작 수행
    // Then: 결과 검증
  });

  test('테스트 케이스 2: {설명}', async ({ page }) => {
    // ...
  });

});
```

---

### 4. 작업 흐름

```
1. 작업지시서 읽기
   - 파일 경로: works/tasks/YYYYMMDD_NN_*.md
   - 내용 분석: 변경 사항, 주요 기능 파악

2. 테스트 시나리오 설계
   - 테스트 대상 페이지/컴포넌트 식별
   - 테스트 케이스 목록 작성:
     * 정상 케이스
     * 에러 케이스
     * 경계 케이스

3. Playwright 테스트 코드 생성
   - test.describe() 블록 생성
   - 각 케이스별 test() 작성
   - 선택자 및 검증 로직 포함

4. 테스트 파일 저장
   - 파일명: tests/{기능명}_test.spec.js
   - 작업지시서 참조 주석 포함

5. 개발자에게 안내
   - 생성된 파일 경로
   - 테스트 케이스 개수
   - 검토 및 수정 권장 사항
   - test-runner 실행 방법
```

---

## 생성 예시

### 입력 (작업지시서)

**파일**: `works/tasks/20260121_01_fix_password_hash_order.md`

**주요 내용**:
- Login.razor 수정
- 비밀번호 검증 로직 개선
- v_MemberListDB 활용

### 출력 (테스트 파일)

**파일**: `tests/login_test.spec.js`

```javascript
// tests/login_test.spec.js
// 작업지시서: 20260121_01_fix_password_hash_order.md 기반 생성
// 생성일시: 2026-01-22 14:30

import { test, expect } from '@playwright/test';

test.describe('로그인 기능 테스트', () => {

  test.beforeEach(async ({ page }) => {
    // 로그인 페이지로 이동
    await page.goto('http://localhost:5000/Login');
  });

  // 정상 케이스
  test('정상 로그인 - 올바른 계정 정보', async ({ page }) => {
    // Given: 로그인 페이지
    await expect(page).toHaveTitle(/로그인/);

    // When: 올바른 계정 정보 입력 및 로그인
    await page.fill('input[name="userid"]', 'test_user');
    await page.fill('input[name="password"]', 'test_password');
    await page.click('button[type="submit"]');

    // Then: 메인 페이지로 이동
    await expect(page).toHaveURL('http://localhost:5000/');
  });

  // 에러 케이스 1
  test('로그인 실패 - 잘못된 비밀번호', async ({ page }) => {
    // Given: 로그인 페이지
    await expect(page.locator('h1')).toContainText('로그인');

    // When: 잘못된 비밀번호 입력
    await page.fill('input[name="userid"]', 'test_user');
    await page.fill('input[name="password"]', 'wrong_password');
    await page.click('button[type="submit"]');

    // Then: 에러 메시지 표시
    await expect(page.locator('.error-message')).toBeVisible();
    await expect(page.locator('.error-message')).toContainText('비밀번호');

    // And: 로그인 페이지 유지
    await expect(page).toHaveURL(/\/Login/);
  });

  // 에러 케이스 2
  test('로그인 실패 - 존재하지 않는 사용자', async ({ page }) => {
    // When: 존재하지 않는 사용자 ID 입력
    await page.fill('input[name="userid"]', 'nonexistent_user');
    await page.fill('input[name="password"]', 'any_password');
    await page.click('button[type="submit"]');

    // Then: 에러 메시지 표시
    await expect(page.locator('.error-message')).toBeVisible();
  });

  // 경계 케이스
  test('로그인 실패 - 빈 입력', async ({ page }) => {
    // When: 빈 입력으로 로그인 시도
    await page.click('button[type="submit"]');

    // Then: 유효성 검증 메시지 표시 또는 로그인 페이지 유지
    await expect(page).toHaveURL(/\/Login/);
  });

  // UI 요소 확인
  test('로그인 페이지 UI 요소 확인', async ({ page }) => {
    // 필수 요소 존재 확인
    await expect(page.locator('input[name="userid"]')).toBeVisible();
    await expect(page.locator('input[name="password"]')).toBeVisible();
    await expect(page.locator('button[type="submit"]')).toBeVisible();
  });

});
```

---

## 테스트 템플릿 선택 가이드

작업지시서 내용에 따라 적절한 템플릿을 선택하여 생성합니다.

### 1. 새 페이지 추가 시

**생성 템플릿**:
- 페이지 로드 테스트
- UI 요소 존재 확인
- 기본 네비게이션 테스트

### 2. CRUD 기능 추가 시

**생성 템플릿**:
- Create (생성) 테스트
- Read (조회) 테스트
- Update (수정) 테스트
- Delete (삭제) 테스트

### 3. 인증/권한 변경 시

**생성 템플릿**:
- 로그인/로그아웃 테스트
- 권한 확인 테스트
- 접근 제한 테스트

### 4. API 연동 시

**생성 템플릿**:
- API 호출 테스트
- 응답 데이터 검증
- 에러 핸들링 테스트

---

## 연계 워크플로우

### test-gen → test-runner 연계

```
1. /test-gen 20260121_01
   ↓
2. Playwright 테스트 파일 생성
   tests/login_test.spec.js
   ↓
3. 개발자 검토 및 수정
   ↓
4. "테스트 실행해줘"
   ↓
5. test-runner Agent 실행
   - 서버 상태 확인
   - Playwright 테스트 실행
   - 결과 리포팅
```

---

## 제약사항

### 생성 제한
- ✅ Playwright 테스트 파일만 생성
- ❌ 실제 서버 코드 수정 안 함
- ❌ 데이터베이스 직접 접근 안 함

### 검토 필수
- 생성된 테스트는 **템플릿 기반 초안**
- 개발자가 **반드시 검토 및 수정**
- 선택자(selector) 정확성 확인 필요

### 실행 환경
- Playwright 설치 필요
- 서버가 실행 중이어야 함
- test-runner Agent와 함께 사용 권장

---

## 주의사항

### 1. 선택자(Selector) 정확성

생성된 테스트의 선택자는 **추정값**입니다.

```javascript
// 생성된 코드 (예상)
await page.fill('input[name="userid"]', 'test');

// 실제 HTML이 다를 수 있음
// → 개발자가 확인하여 수정 필요
```

### 2. 테스트 데이터

테스트에 사용되는 데이터는 **예시**입니다.

```javascript
// 생성된 코드
await page.fill('input[name="userid"]', 'test_user');

// 실제 테스트 환경에 맞게 수정 필요
// → DB에 존재하는 사용자 ID로 변경
```

### 3. URL 및 경로

URL은 기본값(`http://localhost:5000`)을 사용합니다.

```javascript
// 생성된 코드
await page.goto('http://localhost:5000/Login');

// 실제 환경에 맞게 수정 가능
```

---

## 권장 사항

### 1. 작업지시서 품질

테스트 생성 품질은 작업지시서에 달려 있습니다.

**좋은 작업지시서**:
```markdown
## 변경 사항
- Login.razor 수정
- 비밀번호 검증 로직 개선
- v_MemberListDB 활용

## 테스트 시나리오
1. 정상 로그인
2. 잘못된 비밀번호
3. 존재하지 않는 사용자
```

### 2. 단계적 접근

처음부터 완벽한 테스트를 기대하지 마세요.

```
1단계: /test-gen으로 초안 생성
2단계: 개발자가 검토 및 수정
3단계: test-runner로 실행 및 검증
4단계: 실패 케이스 수정 및 보완
```

### 3. 지속적 개선

생성된 테스트를 기반으로 점진적으로 개선하세요.

---

## 트러블슈팅

### 작업지시서를 찾을 수 없음
→ works/tasks/ 폴더 확인
→ 파일명 형식 확인 (YYYYMMDD_NN_*.md)

### 테스트가 너무 간단함
→ 작업지시서에 더 상세한 내용 추가
→ 생성 후 개발자가 직접 보완

### 선택자가 맞지 않음
→ 실제 HTML 구조 확인
→ 개발자 도구로 정확한 선택자 찾기
→ 테스트 파일 수정

---

**작성일**: 2026-01-22
**버전**: 1.0
**담당**: Claude AI
