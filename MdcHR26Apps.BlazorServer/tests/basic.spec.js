// @ts-check
const { test, expect } = require('@playwright/test');

test.describe('Blazor Server 기본 테스트', () => {
  test('홈페이지가 정상적으로 로드되는지 확인', async ({ page }) => {
    // 페이지 로드
    await page.goto('/');

    // 페이지 제목 확인
    await expect(page).toHaveTitle(/2026년 인사평가프로그램/);

    // 메인 헤딩 확인
    await expect(page.locator('h1')).toContainText('2026년 인사평가 시스템');
  });

  test('Bootstrap CSS가 정상적으로 로드되는지 확인', async ({ page }) => {
    await page.goto('/');

    // Bootstrap 클래스가 적용된 요소 확인
    const container = page.locator('.container');
    await expect(container).toBeVisible();
  });

  test('로그인 상태 메시지가 표시되는지 확인', async ({ page }) => {
    await page.goto('/');

    // 로그인 필요 또는 환영 메시지 중 하나가 표시되어야 함
    const alertWarning = page.locator('.alert-warning');
    const alertSuccess = page.locator('.alert-success');

    const warningVisible = await alertWarning.isVisible().catch(() => false);
    const successVisible = await alertSuccess.isVisible().catch(() => false);

    expect(warningVisible || successVisible).toBeTruthy();
  });

  test('404 페이지가 정상적으로 작동하는지 확인', async ({ page }) => {
    await page.goto('/non-existent-page');

    // Blazor 로드 대기
    await page.waitForLoadState('networkidle');

    // 404 메시지 확인
    await expect(page.locator('h3')).toContainText('페이지를 찾을 수 없습니다');

    // 홈으로 돌아가기 버튼 확인
    const homeButton = page.locator('a.btn-primary');
    await expect(homeButton).toContainText('홈으로 돌아가기');
  });
});
