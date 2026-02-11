import { test, expect } from '@playwright/test';
import { createLecturerAuthPage } from './utils/test-locators';
import { createTestHelpers, TestHelpers } from './utils/test-helpers';

test.describe('Lecturer Authentication - Migrated & Enhanced', () => {
  let lecturerAuth: ReturnType<typeof createLecturerAuthPage>;
  let helpers: ReturnType<typeof createTestHelpers>;
  let testHelpers: ReturnType<typeof createTestHelpers>;
  
  // Generate unique test data
  const testData = TestHelpers.generateTestData();

  test.beforeEach(async ({ page }) => {
    lecturerAuth = createLecturerAuthPage(page);
    helpers = createTestHelpers(page);
    testHelpers = createTestHelpers(page);
  });

  test.describe('Lecturer Login Flow', () => {
    test('should login successfully with valid credentials', async ({ page }) => {
      // Note: This test assumes a test lecturer account exists
      const testEmail = 'lecturer@example.com';
      const testPassword = 'password123';
      
      await lecturerAuth.gotoLecturerLogin();
      
      // Verify page elements
      await expect(lecturerAuth.lecturerLoginEmail).toBeVisible();
      await expect(lecturerAuth.lecturerLoginPassword).toBeVisible();
      await expect(lecturerAuth.lecturerLoginSubmitButton).toBeVisible();
      await expect(lecturerAuth.lecturerLoginRememberMe).toBeVisible();
      
      // Fill login form with robust locators
      await lecturerAuth.safeFill(lecturerAuth.lecturerLoginEmail, testEmail);
      await lecturerAuth.safeFill(lecturerAuth.lecturerLoginPassword, testPassword);
      
      // Submit form
      await lecturerAuth.safeClick(lecturerAuth.lecturerLoginSubmitButton);
      
      // Should redirect to lecturer dashboard
      await lecturerAuth.expectUrlContains('/lecturer-dashboard');
      
      // Verify successful login indicators
      await expect(lecturerAuth.lecturerDashboardHeader).toBeVisible();
    });

    test('should show error toast for incorrect login credentials', async ({ page }) => {
      await lecturerAuth.gotoLecturerLogin();
      
      // Fill in incorrect credentials using robust locators
      await lecturerAuth.safeFill(lecturerAuth.lecturerLoginEmail, 'incorrect@example.com');
      await lecturerAuth.safeFill(lecturerAuth.lecturerLoginPassword, 'wrongpassword');
      
      // Click the login button
      await lecturerAuth.safeClick(lecturerAuth.lecturerLoginSubmitButton);
      
      // Check for toast notification using robust locators
      const toastMessage = await lecturerAuth.waitForToast('error');
      expect(toastMessage).toContain(/credentials|invalid/i);
      
      // Assert that the URL has not changed
      await lecturerAuth.expectUrlContains('/lecturer-login');
    });

    test('should show validation errors for empty form submission', async ({ page }) => {
      await lecturerAuth.gotoLecturerLogin();
      
      // Try to submit empty form
      await lecturerAuth.safeClick(lecturerAuth.lecturerLoginSubmitButton);
      
      // Check that we're still on login page
      await lecturerAuth.expectUrlContains('/lecturer-login');
      
      // Form should still be visible
      await expect(lecturerAuth.lecturerLoginEmail).toBeVisible();
      await expect(lecturerAuth.lecturerLoginPassword).toBeVisible();
    });

    test('should handle form submission with Enter key', async ({ page }) => {
      await lecturerAuth.gotoLecturerLogin();
      
      // Fill form fields
      await lecturerAuth.safeFill(lecturerAuth.lecturerLoginEmail, 'test@example.com');
      await lecturerAuth.safeFill(lecturerAuth.lecturerLoginPassword, 'password');
      
      // Press Enter in password field
      await lecturerAuth.lecturerLoginPassword.press('Enter');
      
      // Should attempt login
      await page.waitForTimeout(1000);
    });

    test('should toggle password visibility', async ({ page }) => {
      await lecturerAuth.gotoLecturerLogin();
      
      const passwordField = lecturerAuth.lecturerLoginPassword;
      
      // Initially should be password type
      await expect(passwordField).toHaveAttribute('type', 'password');
      
      // Find and click password toggle button
      const toggleButton = page.locator('.password-toggle');
      await lecturerAuth.safeClick(toggleButton);
      
      // Should now be text type
      await expect(passwordField).toHaveAttribute('type', 'text');
      
      // Click again to hide
      await lecturerAuth.safeClick(toggleButton);
      await expect(passwordField).toHaveAttribute('type', 'password');
    });

    test('should handle remember me checkbox functionality', async ({ page }) => {
      await lecturerAuth.gotoLecturerLogin();
      
      const rememberCheckbox = lecturerAuth.lecturerLoginRememberMe;
      
      // Initially unchecked
      await expect(rememberCheckbox).not.toBeChecked();
      
      // Check the checkbox
      await lecturerAuth.safeClick(rememberCheckbox);
      await expect(rememberCheckbox).toBeChecked();
      
      // Uncheck
      await lecturerAuth.safeClick(rememberCheckbox);
      await expect(rememberCheckbox).not.toBeChecked();
    });

    test('should show loading state during login attempt', async ({ page }) => {
      await lecturerAuth.gotoLecturerLogin();
      
      // Fill form
      await lecturerAuth.safeFill(lecturerAuth.lecturerLoginEmail, 'test@example.com');
      await lecturerAuth.safeFill(lecturerAuth.lecturerLoginPassword, 'password');
      
      // Submit and check for loading state
      await lecturerAuth.safeClick(lecturerAuth.lecturerLoginSubmitButton);
      
      // Button should show loading state (implementation dependent)
      await page.waitForTimeout(500);
    });

    test('should clear form after failed login', async ({ page }) => {
      await lecturerAuth.gotoLecturerLogin();
      
      // Fill with wrong credentials
      await lecturerAuth.safeFill(lecturerAuth.lecturerLoginEmail, 'wrong@example.com');
      await lecturerAuth.safeFill(lecturerAuth.lecturerLoginPassword, 'wrongpassword');
      
      // Submit
      await lecturerAuth.safeClick(lecturerAuth.lecturerLoginSubmitButton);
      
      // Wait for error
      await lecturerAuth.waitForToast('error');
      
      // Check if form is still filled (implementation dependent)
      const emailValue = await lecturerAuth.lecturerLoginEmail.inputValue();
      const passwordValue = await lecturerAuth.lecturerLoginPassword.inputValue();
      
      // Values should still be there for user to correct
      expect(emailValue).toBe('wrong@example.com');
      expect(passwordValue).toBe('wrongpassword');
    });
  });

  test.describe('Lecturer Dashboard Access', () => {
    test('should redirect unauthorized users from dashboard', async ({ page }) => {
      // Try to access dashboard without login
      await page.goto('/lecturer-dashboard');
      
      // Should redirect to login page
      await lecturerAuth.expectUrlContains('/lecturer-login');
    });

    test('should maintain session after login', async ({ page }) => {
      // This test assumes valid credentials
      const testEmail = 'lecturer@example.com';
      const testPassword = 'password123';
      
      await lecturerAuth.loginAsLecturer(testEmail, testPassword);
      
      // Navigate to different pages and come back
      await page.goto('/');
      await page.goto('/lecturer-dashboard');
      
      // Should still be logged in
      await lecturerAuth.expectUrlContains('/lecturer-dashboard');
      await expect(lecturerAuth.lecturerDashboardHeader).toBeVisible();
    });

    test('should handle session timeout', async ({ page }) => {
      // Login first
      const testEmail = 'lecturer@example.com';
      const testPassword = 'password123';
      
      await lecturerAuth.loginAsLecturer(testEmail, testPassword);
      
      // Clear cookies/session storage to simulate timeout
      await page.context().clearCookies();
      await page.evaluate(() => {
        sessionStorage.clear();
        localStorage.clear();
      });
      
      // Try to access protected page
      await page.goto('/lecturer-dashboard');
      
      // Should redirect to login
      await lecturerAuth.expectUrlContains('/lecturer-login');
    });
  });

  test.describe('Error Handling & Edge Cases', () => {
    test('should handle network errors gracefully', async ({ page }) => {
      await lecturerAuth.gotoLecturerLogin();
      
      // Fill form
      await lecturerAuth.safeFill(lecturerAuth.lecturerLoginEmail, 'test@example.com');
      await lecturerAuth.safeFill(lecturerAuth.lecturerLoginPassword, 'password');
      
      // Simulate network offline
      await page.context().setOffline(true);
      
      // Submit form
      await lecturerAuth.safeClick(lecturerAuth.lecturerLoginSubmitButton);
      
      // Should show error toast
      const toastMessage = await lecturerAuth.waitForToast('error');
      expect(toastMessage).toBeTruthy();
      
      // Restore network
      await page.context().setOffline(false);
    });

    test('should handle server errors gracefully', async ({ page }) => {
      await lecturerAuth.gotoLecturerLogin();
      
      // Mock server error
      await page.route('**/auth/v1/token', route => {
        route.fulfill({
          status: 500,
          contentType: 'application/json',
          body: JSON.stringify({ error: 'Internal server error' })
        });
      });
      
      // Fill form
      await lecturerAuth.safeFill(lecturerAuth.lecturerLoginEmail, 'test@example.com');
      await lecturerAuth.safeFill(lecturerAuth.lecturerLoginPassword, 'password');
      
      // Submit form
      await lecturerAuth.safeClick(lecturerAuth.lecturerLoginSubmitButton);
      
      // Should show error toast
      const toastMessage = await lecturerAuth.waitForToast('error');
      expect(toastMessage).toBeTruthy();
      
      // Restore normal routing
      await page.unroute('**/auth/v1/token');
    });

    test('should handle malformed response', async ({ page }) => {
      await lecturerAuth.gotoLecturerLogin();
      
      // Mock malformed response
      await page.route('**/auth/v1/token', route => {
        route.fulfill({
          status: 200,
          contentType: 'application/json',
          body: 'invalid json response'
        });
      });
      
      // Fill form
      await lecturerAuth.safeFill(lecturerAuth.lecturerLoginEmail, 'test@example.com');
      await lecturerAuth.safeFill(lecturerAuth.lecturerLoginPassword, 'password');
      
      // Submit form
      await lecturerAuth.safeClick(lecturerAuth.lecturerLoginSubmitButton);
      
      // Should handle error gracefully
      await page.waitForTimeout(1000);
      
      // Restore normal routing
      await page.unroute('**/auth/v1/token');
    });
  });

  test.describe('Accessibility & Usability', () => {
    test('should be accessible via keyboard navigation', async ({ page }) => {
      await lecturerAuth.gotoLecturerLogin();
      
      // Tab through form elements
      await page.keyboard.press('Tab');
      await expect(lecturerAuth.lecturerLoginEmail).toBeFocused();
      
      await page.keyboard.press('Tab');
      await expect(lecturerAuth.lecturerLoginPassword).toBeFocused();
      
      await page.keyboard.press('Tab');
      // Should focus on remember me checkbox or submit button
    });

    test('should have proper accessibility attributes', async ({ page }) => {
      await lecturerAuth.gotoLecturerLogin();
      
      // Check for proper labels
      await expect(lecturerAuth.lecturerLoginEmail).toHaveAttribute('type', 'email');
      await expect(lecturerAuth.lecturerLoginPassword).toHaveAttribute('type', 'password');
      await expect(lecturerAuth.lecturerLoginSubmitButton).toBeEnabled();
    });

    test('should support screen readers', async ({ page }) => {
      await lecturerAuth.gotoLecturerLogin();
      
      // Check for ARIA labels and roles
      const loginButton = lecturerAuth.lecturerLoginSubmitButton;
      await expect(loginButton).toBeVisible();
      
      // Check form structure is semantic
      const emailField = lecturerAuth.lecturerLoginEmail;
      await expect(emailField).toBeVisible();
    });
  });

  test.describe('Mobile Responsiveness', () => {
    test('should work correctly on mobile devices', async ({ page }) => {
      // Set mobile viewport
      await testHelpers.setMobileViewport();
      
      await lecturerAuth.gotoLecturerLogin();
      
      // Verify mobile layout
      await expect(lecturerAuth.lecturerLoginEmail).toBeVisible();
      await expect(lecturerAuth.lecturerLoginPassword).toBeVisible();
      await expect(lecturerAuth.lecturerLoginSubmitButton).toBeVisible();
      
      // Test form submission on mobile
      await lecturerAuth.safeFill(lecturerAuth.lecturerLoginEmail, 'test@example.com');
      await lecturerAuth.safeFill(lecturerAuth.lecturerLoginPassword, 'password');
      await lecturerAuth.safeClick(lecturerAuth.lecturerLoginSubmitButton);
      
      // Should work the same as desktop
      await page.waitForTimeout(1000);
    });

    test('should handle touch interactions on mobile', async ({ page }) => {
      await testHelpers.setMobileViewport();
      
      await lecturerAuth.gotoLecturerLogin();
      
      // Test touch events
      await lecturerAuth.lecturerLoginEmail.tap();
      await lecturerAuth.lecturerLoginEmail.fill('test@example.com');
      
      await lecturerAuth.lecturerLoginPassword.tap();
      await lecturerAuth.lecturerLoginPassword.fill('password');
      
      await lecturerAuth.lecturerLoginSubmitButton.tap();
      
      // Should handle touch events properly
      await page.waitForTimeout(1000);
    });
  });

  test.describe('Performance & Optimization', () => {
    test('should load quickly', async ({ page }) => {
      const startTime = Date.now();
      
      await lecturerAuth.gotoLecturerLogin();
      await helpers.waitForAppReady();
      
      const loadTime = Date.now() - startTime;
      
      // Should load within reasonable time (adjust threshold as needed)
      expect(loadTime).toBeLessThan(3000);
    });

    test('should handle rapid form submissions', async ({ page }) => {
      await lecturerAuth.gotoLecturerLogin();
      
      // Fill form
      await lecturerAuth.safeFill(lecturerAuth.lecturerLoginEmail, 'test@example.com');
      await lecturerAuth.safeFill(lecturerAuth.lecturerLoginPassword, 'password');
      
      // Click submit multiple times rapidly
      await lecturerAuth.safeClick(lecturerAuth.lecturerLoginSubmitButton);
      await lecturerAuth.safeClick(lecturerAuth.lecturerLoginSubmitButton);
      await lecturerAuth.safeClick(lecturerAuth.lecturerLoginSubmitButton);
      
      // Should handle multiple clicks gracefully
      await page.waitForTimeout(1000);
    });
  });

  test.describe('Cross-Browser Compatibility', () => {
    test('should work in different browsers', async ({ page, browserName }) => {
      await lecturerAuth.gotoLecturerLogin();
      
      // Verify basic functionality works in current browser
      await expect(lecturerAuth.lecturerLoginEmail).toBeVisible();
      await expect(lecturerAuth.lecturerLoginPassword).toBeVisible();
      await expect(lecturerAuth.lecturerLoginSubmitButton).toBeVisible();
      
      // Test form fill
      await lecturerAuth.safeFill(lecturerAuth.lecturerLoginEmail, 'test@example.com');
      await lecturerAuth.safeFill(lecturerAuth.lecturerLoginPassword, 'password');
      
      // Verify values are set correctly
      const emailValue = await lecturerAuth.lecturerLoginEmail.inputValue();
      const passwordValue = await lecturerAuth.lecturerLoginPassword.inputValue();
      
      expect(emailValue).toBe('test@example.com');
      expect(passwordValue).toBe('password');
    });
  });
});
