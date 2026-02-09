import { test, expect } from '@playwright/test';
import { createStudentAuthPage, createLecturerAuthPage } from './utils/test-locators';
import { createTestHelpers, TestHelpers } from './utils/test-helpers';

test.describe('Authentication Redirect Integration Tests', () => {
  let studentAuth: ReturnType<typeof createStudentAuthPage>;
  let lecturerAuth: ReturnType<typeof createLecturerAuthPage>;
  let helpers: ReturnType<typeof createTestHelpers>;

  test.beforeEach(async ({ page }) => {
    studentAuth = createStudentAuthPage(page);
    lecturerAuth = createLecturerAuthPage(page);
    helpers = createTestHelpers(page);
    
    // Clear any existing session storage
    await page.evaluate(() => {
      sessionStorage.clear();
      localStorage.clear();
    });
  });

  test.describe('Student Authentication Redirects', () => {
    test('should save intended destination and redirect after login', async ({ page }) => {
      const testData = TestHelpers.generateTestData();
      
      await test.step('Navigate to protected student route', async () => {
        await page.goto('/student-homepage');
        
        // Should redirect to student login with return URL
        await helpers.waitForUrl('/student-login');
        
        // Check return URL parameter
        const currentUrl = page.url();
        expect(currentUrl).toContain('returnUrl=%2Fstudent-homepage');
      });

      await test.step('Login and verify redirect', async () => {
        // Login with test credentials (assuming test user exists)
        await studentAuth.safeFill(studentAuth.studentLoginEmail, testData.student.email);
        await studentAuth.safeFill(studentAuth.studentLoginPassword, testData.student.password);
        await studentAuth.safeClick(studentAuth.studentLoginSubmitButton);
        
        // Should redirect to intended destination
        await helpers.waitForUrl('/student-homepage');
        
        // Verify we're on the intended page
        await expect(studentAuth.studentHomepageHeader).toBeVisible();
      });
    });

    test('should show return URL display on login page', async ({ page }) => {
      await test.step('Navigate to protected route with return URL', async () => {
        await page.goto('/student-homepage/courses/math-101');
        await helpers.waitForUrl('/student-login');
      });

      await test.step('Verify return URL display', async () => {
        // Check if return URL info is displayed
        const returnUrlInfo = page.locator('.return-url-info');
        await expect(returnUrlInfo).toBeVisible();
        
        // Check return URL text
        const returnUrlPath = page.locator('.return-url-path');
        await expect(returnUrlPath).toContainText('/student-homepage/courses/math-101');
        
        // Check return URL label
        const returnUrlLabel = page.locator('.return-url-label');
        await expect(returnUrlLabel).toContainText('After login, you\'ll be redirected to:');
      });
    });

    test('should handle multiple redirect attempts correctly', async ({ page }) => {
      await test.step('First redirect attempt', async () => {
        await page.goto('/student-homepage');
        await helpers.waitForUrl('/student-login');
      });

      await test.step('Second redirect attempt without login', async () => {
        await page.goto('/student-homepage/reports');
        await helpers.waitForUrl('/student-login');
        
        // Should update to latest intended destination
        const returnUrlPath = page.locator('.return-url-path');
        await expect(returnUrlPath).toContainText('/student-homepage/reports');
      });
    });
  });

  test.describe('Lecturer Authentication Redirects', () => {
    test('should save intended destination and redirect after login', async ({ page }) => {
      const testData = TestHelpers.generateTestData();
      
      await test.step('Navigate to protected lecturer route', async () => {
        await page.goto('/lecturer-dashboard');
        
        // Should redirect to lecturer login with return URL
        await helpers.waitForUrl('/lecturer-login');
        
        // Check return URL parameter
        const currentUrl = page.url();
        expect(currentUrl).toContain('returnUrl=%2Flecturer-dashboard');
      });

      await test.step('Login and verify redirect', async () => {
        // Login with test credentials
        await lecturerAuth.safeFill(lecturerAuth.lecturerLoginEmail, testData.lecturer.email);
        await lecturerAuth.safeFill(lecturerAuth.lecturerLoginPassword, testData.lecturer.password);
        await lecturerAuth.safeClick(lecturerAuth.lecturerLoginSubmitButton);
        
        // Should redirect to intended destination
        await helpers.waitForUrl('/lecturer-dashboard');
        
        // Verify we're on the intended page
        await expect(lecturerAuth.lecturerDashboardHeader).toBeVisible();
      });
    });

    test('should show return URL display on lecturer login page', async ({ page }) => {
      await test.step('Navigate to protected route with return URL', async () => {
        await page.goto('/lecturer-dashboard/courses');
        await helpers.waitForUrl('/lecturer-login');
      });

      await test.step('Verify return URL display', async () => {
        // Check if return URL info is displayed
        const returnUrlInfo = page.locator('.return-url-info');
        await expect(returnUrlInfo).toBeVisible();
        
        // Check return URL text
        const returnUrlPath = page.locator('.return-url-path');
        await expect(returnUrlPath).toContainText('/lecturer-dashboard/courses');
      });
    });
  });

  test.describe('Cross-Role Redirects', () => {
    test('should redirect to correct login based on route', async ({ page }) => {
      await test.step('Student route redirects to student login', async () => {
        await page.goto('/student-homepage');
        await helpers.waitForUrl('/student-login');
        
        const currentUrl = page.url();
        expect(currentUrl).toContain('/student-login');
        expect(currentUrl).not.toContain('/lecturer-login');
      });

      await test.step('Lecturer route redirects to lecturer login', async () => {
        // Clear session storage first
        await page.evaluate(() => sessionStorage.clear());
        
        await page.goto('/lecturer-dashboard');
        await helpers.waitForUrl('/lecturer-login');
        
        const currentUrl = page.url();
        expect(currentUrl).toContain('/lecturer-login');
        expect(currentUrl).not.toContain('/student-login');
      });
    });
  });

  test.describe('Session Storage Persistence', () => {
    test('should persist redirect data across page reloads', async ({ page }) => {
      await test.step('Set up redirect scenario', async () => {
        await page.goto('/student-homepage/courses/advanced-math');
        await helpers.waitForUrl('/student-login');
        
        // Verify return URL is stored
        const returnUrlPath = page.locator('.return-url-path');
        await expect(returnUrlPath).toContainText('/student-homepage/courses/advanced-math');
      });

      await test.step('Reload page and verify persistence', async () => {
        // Reload the login page
        await page.reload();
        await helpers.waitForAppReady();
        
        // Return URL should still be displayed
        const returnUrlInfo = page.locator('.return-url-info');
        await expect(returnUrlInfo).toBeVisible();
        
        const returnUrlPath = page.locator('.return-url-path');
        await expect(returnUrlPath).toContainText('/student-homepage/courses/advanced-math');
      });
    });

    test('should expire redirect data after 30 minutes', async ({ page }) => {
      await test.step('Set up redirect scenario', async () => {
        await page.goto('/student-homepage');
        await helpers.waitForUrl('/student-login');
      });

      await test.step('Manually expire session storage', async () => {
        // Simulate expired session by manipulating timestamp
        await page.evaluate(() => {
          const stored = sessionStorage.getItem('authRedirect');
          if (stored) {
            const data = JSON.parse(stored);
            data.timestamp = Date.now() - (31 * 60 * 1000); // 31 minutes ago
            sessionStorage.setItem('authRedirect', JSON.stringify(data));
          }
        });
        
        // Reload page
        await page.reload();
        await helpers.waitForAppReady();
      });

      await test.step('Verify expired data is cleared', async () => {
        // Return URL display should not be visible
        const returnUrlInfo = page.locator('.return-url-info');
        await expect(returnUrlInfo).not.toBeVisible();
      });
    });
  });

  test.describe('Edge Cases', () => {
    test('should not save login pages as intended destinations', async ({ page }) => {
      await test.step('Navigate to login pages directly', async () => {
        await page.goto('/student-login');
        await helpers.waitForAppReady();
        
        // Should not show return URL display
        const returnUrlInfo = page.locator('.return-url-info');
        await expect(returnUrlInfo).not.toBeVisible();
      });

      await test.step('Navigate to lecturer login directly', async () => {
        await page.goto('/lecturer-login');
        await helpers.waitForAppReady();
        
        // Should not show return URL display
        const returnUrlInfo = page.locator('.return-url-info');
        await expect(returnUrlInfo).not.toBeVisible();
      });
    });

    test('should handle malformed redirect data gracefully', async ({ page }) => {
      await test.step('Set up malformed session storage', async () => {
        await page.evaluate(() => {
          sessionStorage.setItem('authRedirect', 'invalid json data');
        });
        
        await page.goto('/student-homepage');
        await helpers.waitForAppReady();
      });

      await test.step('Verify graceful handling', async () => {
        // Should not crash and should not show return URL display
        const returnUrlInfo = page.locator('.return-url-info');
        await expect(returnUrlInfo).not.toBeVisible();
        
        // Should still be able to use login form
        await expect(studentAuth.studentLoginEmail).toBeVisible();
        await expect(studentAuth.studentLoginPassword).toBeVisible();
      });
    });
  });

  test.describe('Default Redirects', () => {
    test('should redirect to role-based default when no intended destination', async ({ page }) => {
      await test.step('Login directly without intended destination', async () => {
        await page.goto('/student-login');
        await helpers.waitForAppReady();
        
        // Login without return URL
        await studentAuth.safeFill(studentAuth.studentLoginEmail, 'test@example.com');
        await studentAuth.safeFill(studentAuth.studentLoginPassword, 'password');
        await studentAuth.safeClick(studentAuth.studentLoginSubmitButton);
        
        // Should redirect to student homepage (default)
        await helpers.waitForUrl('/student-homepage');
      });
    });
  });
});
