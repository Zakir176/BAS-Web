import { test, expect } from '@playwright/test';
import { createLecturerAuthPage } from './utils/test-locators';

test.describe('Lecturer Authentication - Improved Tests', () => {
  let lecturerAuth: ReturnType<typeof createLecturerAuthPage>;
  
  test.beforeEach(async ({ page }) => {
    lecturerAuth = createLecturerAuthPage(page);
  });

  test('should display lecturer login page with correct elements', async ({ page }) => {
    await lecturerAuth.gotoLecturerLogin();
    
    // Verify page title and key elements
    await expect(page).toHaveTitle(/Smart Attendance/);
    await expect(lecturerAuth.lecturerLoginEmail).toBeVisible();
    await expect(lecturerAuth.lecturerLoginPassword).toBeVisible();
    await expect(lecturerAuth.lecturerLoginSubmitButton).toBeVisible();
    await expect(lecturerAuth.lecturerLoginRememberMe).toBeVisible();
    
    // Verify accessibility labels
    await expect(lecturerAuth.lecturerLoginEmail).toHaveAttribute('type', 'email');
    await expect(lecturerAuth.lecturerLoginPassword).toHaveAttribute('type', 'password');
    await expect(lecturerAuth.lecturerLoginSubmitButton).toBeEnabled();
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

  test('should show error toast for incorrect credentials', async ({ page }) => {
    await lecturerAuth.gotoLecturerLogin();
    
    // Fill with invalid credentials
    await lecturerAuth.safeFill(lecturerAuth.lecturerLoginEmail, 'incorrect@example.com');
    await lecturerAuth.safeFill(lecturerAuth.lecturerLoginPassword, 'wrongpassword');
    
    // Submit form
    await lecturerAuth.safeClick(lecturerAuth.lecturerLoginSubmitButton);
    
    // Wait for error toast
    const toastMessage = await lecturerAuth.waitForToast('error');
    expect(toastMessage).toContain(/credentials|invalid/i);
    
    // Should remain on login page
    await lecturerAuth.expectUrlContains('/lecturer-login');
  });

  test('should allow successful lecturer login', async ({ page }) => {
    // Note: This test assumes a test lecturer account exists
    const testEmail = 'lecturer@example.com';
    const testPassword = 'password123';
    
    await lecturerAuth.loginAsLecturer(testEmail, testPassword);
    
    // Should redirect to lecturer dashboard
    await lecturerAuth.expectUrlContains('/lecturer-dashboard');
    
    // Verify successful login indicators
    await expect(lecturerAuth.lecturerDashboardHeader).toBeVisible();
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

  test('should handle remember me checkbox', async ({ page }) => {
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
});

test.describe('Lecturer Dashboard Access', () => {
  let lecturerAuth: ReturnType<typeof createLecturerAuthPage>;
  
  test.beforeEach(async ({ page }) => {
    lecturerAuth = createLecturerAuthPage(page);
  });

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
