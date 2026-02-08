import { defineConfig, devices } from '@playwright/test';

/**
 * Read environment variables from file.
 * https://github.com/motdotla/dotenv
 */
// require('dotenv').config();

/**
 * Enhanced Playwright configuration for CAT application
 * Features robust locators, comprehensive testing, and better reporting
 */
export default defineConfig({
  testDir: './tests',
  /* Run tests in files in parallel */
  fullyParallel: true,
  /* Fail the build on CI if you accidentally left test.only in the source code. */
  forbidOnly: !!process.env.CI,
  /* Retry on CI only */
  retries: process.env.CI ? 2 : 0,
  /* Opt out of parallel tests on CI. */
  workers: process.env.CI ? 1 : undefined,
  /* Enhanced reporter configuration */
  reporter: process.env.CI 
    ? [['html'], ['json', { outputFile: 'test-results.json' }], ['junit', { outputFile: 'test-results.xml' }], ['github']]
    : [['html'], ['json', { outputFile: 'test-results.json' }], ['junit', { outputFile: 'test-results.xml' }]],
  /* Shared settings for all the projects below. See https://playwright.dev/docs/api/class-testoptions. */
  use: {
    /* Base URL to use in actions like `await page.goto('/')`. */
    baseURL: 'http://localhost:5173', // Vite's default dev server port

    /* Collect trace when retrying the failed test. See https://playwright.dev/docs/trace-viewer */
    trace: 'on-first-retry',
    
    /* Take screenshot on failure */
    screenshot: 'only-on-failure',
    
    /* Record video on failure */
    video: 'retain-on-failure',
    
    /* Global timeout for actions */
    actionTimeout: 10000,
    
    /* Global timeout for navigation */
    navigationTimeout: 30000,
  },

  /* Global test configuration */
  globalSetup: require.resolve('./tests/global-setup.ts'),
  globalTeardown: require.resolve('./tests/global-teardown.ts'),

  /* Configure projects for major browsers */
  projects: [
    {
      name: 'chromium',
      use: { 
        ...devices['Desktop Chrome'],
        // Enhanced Chrome settings for better debugging
        launchOptions: {
          args: [
            '--disable-web-security',
            '--disable-features=VizDisplayCompositor',
            '--no-sandbox',
            '--disable-setuid-sandbox'
          ]
        }
      },
    },

    {
      name: 'firefox',
      use: { ...devices['Desktop Firefox'] },
    },

    {
      name: 'webkit',
      use: { ...devices['Desktop Safari'] },
    },

    /* Mobile testing projects */
    {
      name: 'Mobile Chrome',
      use: { ...devices['Pixel 5'] },
      testMatch: '**/*.mobile.spec.ts',
    },
    
    {
      name: 'Mobile Safari',
      use: { ...devices['iPhone 12'] },
      testMatch: '**/*.mobile.spec.ts',
    },

    /* Tablet testing */
    {
      name: 'iPad',
      use: { ...devices['iPad Pro'] },
      testMatch: '**/*.tablet.spec.ts',
    },

    /* Accessibility testing */
    {
      name: 'accessibility',
      use: { 
        ...devices['Desktop Chrome'],
        // Accessibility-specific options
        launchOptions: {
          args: ['--force-prefers-reduced-motion', '--high-contrast']
        }
      },
      testMatch: '**/*.a11y.spec.ts',
    },

    /* Performance testing */
    {
      name: 'performance',
      use: { ...devices['Desktop Chrome'] },
      testMatch: '**/*.perf.spec.ts',
    },
  ],

  /* Test organization and filtering */
  testIgnore: [
    '**/node_modules/**',
    '**/dist/**',
    '**/.git/**'
  ],
  
  /* Test matching patterns for different test types */
  testMatch: [
    '**/*.spec.ts',
    '**/*.e2e.spec.ts',
    '**/*.integration.spec.ts'
  ],

  /* Run your local dev server before starting the tests */
  webServer: {
    command: 'npm run dev',
    url: 'http://localhost:5173',
    reuseExistingServer: !process.env.CI,
    timeout: 120000, // Increase timeout to 2 minutes
    stdout: 'pipe',
    stderr: 'pipe',
  },

  /* Output directory configuration */
  outputDir: 'test-results',
  
  /* Global timeout for each test */
  timeout: 60000,
  
  /* Expect configuration */
  expect: {
    /* Timeout for expect assertions */
    timeout: 5000
  }
});
