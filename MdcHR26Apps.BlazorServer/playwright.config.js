// @ts-check
const { defineConfig, devices } = require('@playwright/test');

/**
 * @see https://playwright.dev/docs/test-configuration
 */
module.exports = defineConfig({
  testDir: './tests',

  /* 병렬 실행 비활성화 (개발 서버 안정성) */
  fullyParallel: false,
  workers: 1,

  /* 실패 시 재시도 */
  retries: 2,

  /* 타임아웃 설정 */
  timeout: 30000,

  /* Reporter 설정 */
  reporter: [
    ['list'],
    ['html', { outputFolder: 'playwright-report' }]
  ],

  /* 공통 설정 */
  use: {
    /* Base URL */
    baseURL: 'http://localhost:5132',

    /* 스크린샷 및 비디오 */
    screenshot: 'only-on-failure',
    video: 'retain-on-failure',

    /* 추적 */
    trace: 'on-first-retry',
  },

  /* 프로젝트 설정 */
  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
  ],

  /* 개발 서버 설정 (주석 처리 - 수동으로 서버 실행) */
  // webServer: {
  //   command: 'dotnet run',
  //   url: 'https://localhost:7249',
  //   reuseExistingServer: true,
  //   ignoreHTTPSErrors: true,
  //   timeout: 120000,
  // },
});
