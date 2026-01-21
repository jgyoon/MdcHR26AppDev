// @ts-check
const { test, expect } = require('@playwright/test');

test.describe('인증 시스템 테스트', () => {
  test('로그인 페이지가 정상적으로 로드되는지 확인', async ({ page }) => {
    await page.goto('/auth/login');

    // 페이지 제목 확인
    await expect(page).toHaveTitle(/로그인/);

    // 로그인 폼 요소 확인
    await expect(page.locator('h3')).toContainText('로그인');
    await expect(page.locator('label')).toContainText('아이디');
    await expect(page.locator('label')).toContainText('비밀번호');

    // 입력 필드 확인
    const userIdInput = page.locator('input[type="text"]');
    const passwordInput = page.locator('input[type="password"]');
    await expect(userIdInput).toBeVisible();
    await expect(passwordInput).toBeVisible();

    // 로그인 버튼 확인
    const loginButton = page.locator('button[type="submit"]');
    await expect(loginButton).toContainText('로그인');
  });

  test('빈 입력으로 로그인 시도 시 validation 확인', async ({ page }) => {
    await page.goto('/auth/login');

    // 빈 상태로 로그인 버튼 클릭
    const loginButton = page.locator('button[type="submit"]');
    await loginButton.click();

    // 페이지가 여전히 로그인 페이지인지 확인 (리다이렉트 안됨)
    await expect(page).toHaveURL(/\/auth\/login/);
  });

  test('잘못된 자격증명으로 로그인 시도', async ({ page }) => {
    await page.goto('/auth/login');

    // 잘못된 자격증명 입력
    await page.locator('input[type="text"]').fill('wronguser');
    await page.locator('input[type="password"]').fill('wrongpassword');

    // 로그인 버튼 클릭
    const loginButton = page.locator('button[type="submit"]');
    await loginButton.click();

    // 에러 메시지 또는 로그인 페이지 유지 확인
    // (구현에 따라 에러 메시지가 표시되거나 페이지가 유지되어야 함)
    await page.waitForTimeout(1000); // Blazor 처리 대기

    // 여전히 로그인 페이지에 있는지 확인
    await expect(page).toHaveURL(/\/auth\/login/);
  });

  test('올바른 자격증명으로 로그인 성공', async ({ page }) => {
    await page.goto('/auth/login');

    // 테스트용 유효한 자격증명 입력
    // 주의: 실제 테스트 DB에 존재하는 사용자 정보 필요
    await page.locator('input[type="text"]').fill('msadmin');
    await page.locator('input[type="password"]').fill('xnd0580!!');

    // 로그인 버튼 클릭
    const loginButton = page.locator('button[type="submit"]');
    await loginButton.click();

    // Blazor 서버 처리 대기
    await page.waitForTimeout(2000);

    // 로그인 성공 후 홈페이지로 리다이렉트 확인
    await expect(page).toHaveURL('/');

    // 로그인 상태 확인 (홈페이지에 로그인 상태 메시지 표시)
    const successAlert = page.locator('.alert-success');
    await expect(successAlert).toBeVisible();
    await expect(successAlert).toContainText('환영합니다');
  });

  test('로그아웃 기능 확인', async ({ page }) => {
    // 먼저 로그인
    await page.goto('/auth/login');
    await page.locator('input[type="text"]').fill('admin');
    await page.locator('input[type="password"]').fill('1234');
    await page.locator('button[type="submit"]').click();
    await page.waitForTimeout(2000);

    // 홈페이지에 있는지 확인
    await expect(page).toHaveURL('/');

    // 로그아웃 페이지로 이동
    await page.goto('/auth/logout');

    // Blazor 처리 대기
    await page.waitForTimeout(2000);

    // 로그인 페이지로 리다이렉트 확인
    await expect(page).toHaveURL(/\/auth\/login/);

    // 로그인 폼이 다시 표시되는지 확인
    const loginButton = page.locator('button[type="submit"]');
    await expect(loginButton).toBeVisible();
  });

  test('로그인 없이 보호된 페이지 접근 시도', async ({ page }) => {
    // 로그아웃 상태 확인을 위해 먼저 로그아웃
    await page.goto('/auth/logout');
    await page.waitForTimeout(1000);

    // 홈페이지 접근
    await page.goto('/');

    // 로그인 필요 메시지 확인
    const warningAlert = page.locator('.alert-warning');
    await expect(warningAlert).toBeVisible();
    await expect(warningAlert).toContainText('로그인이 필요합니다');
  });

  test('로그인 후 새로고침해도 세션 유지', async ({ page }) => {
    // 로그인
    await page.goto('/auth/login');
    await page.locator('input[type="text"]').fill('admin');
    await page.locator('input[type="password"]').fill('1234');
    await page.locator('button[type="submit"]').click();
    await page.waitForTimeout(2000);

    // 로그인 성공 확인
    await expect(page).toHaveURL('/');

    // 페이지 새로고침
    await page.reload();
    await page.waitForTimeout(1000);

    // 여전히 로그인 상태인지 확인
    const successAlert = page.locator('.alert-success');
    await expect(successAlert).toBeVisible();
  });
});
