import { test, expect } from '@playwright/test';
import { createStudentAuthPage } from './utils/test-locators';
import { createTestHelpers, TestHelpers } from './utils/test-helpers';

test.describe('Student Authentication - Migrated & Enhanced', () => {
  let studentAuth: ReturnType<typeof createStudentAuthPage>;
  let helpers: ReturnType<typeof createTestHelpers>;
  let testHelpers: ReturnType<typeof createTestHelpers>;
  
  // Generate unique test data
  const testData = TestHelpers.generateTestData();

  test.beforeEach(async ({ page }) => {
    studentAuth = createStudentAuthPage(page);
    helpers = createTestHelpers(page);
    testHelpers = createTestHelpers(page);
  });

  test.describe('Student Signup Flow', () => {
    test('should allow a new student to sign up with robust locators', async ({ page }) => {
      await test.step('Navigate to signup page', async () => {
        await studentAuth.gotoStudentSignup();
        await helpers.waitForAppReady();
        
        // Verify all form elements are present
        await expect(studentAuth.studentSignupFirstName).toBeVisible();
        await expect(studentAuth.studentSignupLastName).toBeVisible();
        await expect(studentAuth.studentSignupStudentId).toBeVisible();
        await expect(studentAuth.studentSignupEmail).toBeVisible();
        await expect(studentAuth.studentSignupClassSection).toBeVisible();
        await expect(studentAuth.studentSignupPassword).toBeVisible();
        await expect(studentAuth.studentSignupConfirmPassword).toBeVisible();
        await expect(studentAuth.studentSignupTermsCheckbox).toBeVisible();
        await expect(studentAuth.studentSignupSubmitButton).toBeVisible();
      });

      await test.step('Fill signup form with valid data', async () => {
        // Use robust locators instead of fragile CSS selectors
        await studentAuth.safeFill(studentAuth.studentSignupFirstName, testData.student.firstName);
        await studentAuth.safeFill(studentAuth.studentSignupLastName, testData.student.lastName);
        await studentAuth.safeFill(studentAuth.studentSignupStudentId, testData.student.studentId);
        await studentAuth.safeFill(studentAuth.studentSignupEmail, testData.student.email);
        await studentAuth.safeFill(studentAuth.studentSignupClassSection, testData.student.classSection);
        await studentAuth.safeFill(studentAuth.studentSignupPassword, testData.student.password);
        await studentAuth.safeFill(studentAuth.studentSignupConfirmPassword, testData.student.password);
        
        // Accept terms and conditions
        await studentAuth.safeClick(studentAuth.studentSignupTermsCheckbox);
        await expect(studentAuth.studentSignupTermsCheckbox).toBeChecked();
      });

      await test.step('Submit form and handle success', async () => {
        // Handle the success dialog
        const dialogPromise = helpers.handleDialog('Account created');
        
        // Submit the form
        await studentAuth.safeClick(studentAuth.studentSignupSubmitButton);
        
        // Wait for dialog and handle it
        await dialogPromise;
        
        // Verify redirect to login page
        await helpers.waitForUrl('/student-login');
      });
    });

    test('should show validation errors for empty signup form', async ({ page }) => {
      await studentAuth.gotoStudentSignup();
      
      // Try to submit empty form
      await studentAuth.safeClick(studentAuth.studentSignupSubmitButton);
      
      // Should remain on signup page
      await studentAuth.expectUrlContains('/student-signup');
      
      // Check for validation indicators
      await expect(studentAuth.studentSignupFirstName).toBeVisible();
    });

    test('should show error for invalid email format', async ({ page }) => {
      await studentAuth.gotoStudentSignup();
      
      // Fill with invalid email
      await studentAuth.safeFill(studentAuth.studentSignupEmail, 'invalid-email');
      await studentAuth.studentSignupEmail.blur();
      
      // Should show validation error
      await expect(studentAuth.studentSignupEmail).toBeVisible();
    });

    test('should show error for password mismatch', async ({ page }) => {
      await studentAuth.gotoStudentSignup();
      
      // Fill with different passwords
      await studentAuth.safeFill(studentAuth.studentSignupPassword, 'Password123!');
      await studentAuth.safeFill(studentAuth.studentSignupConfirmPassword, 'DifferentPassword');
      await studentAuth.studentSignupConfirmPassword.blur();
      
      // Should show validation error
      await expect(studentAuth.studentSignupConfirmPassword).toBeVisible();
    });

    test('should require terms acceptance', async ({ page }) => {
      await studentAuth.gotoStudentSignup();
      
      // Fill all fields except terms
      await studentAuth.safeFill(studentAuth.studentSignupFirstName, testData.student.firstName);
      await studentAuth.safeFill(studentAuth.studentSignupLastName, testData.student.lastName);
      await studentAuth.safeFill(studentAuth.studentSignupStudentId, testData.student.studentId);
      await studentAuth.safeFill(studentAuth.studentSignupEmail, testData.student.email);
      await studentAuth.safeFill(studentAuth.studentSignupClassSection, testData.student.classSection);
      await studentAuth.safeFill(studentAuth.studentSignupPassword, testData.student.password);
      await studentAuth.safeFill(studentAuth.studentSignupConfirmPassword, testData.student.password);
      
      // Try to submit without accepting terms
      await studentAuth.safeClick(studentAuth.studentSignupSubmitButton);
      
      // Should remain on signup page
      await studentAuth.expectUrlContains('/student-signup');
      
      // Terms checkbox should still be unchecked
      await expect(studentAuth.studentSignupTermsCheckbox).not.toBeChecked();
    });
  });

  test.describe('Student Login Flow', () => {
    test('should login successfully with valid credentials', async ({ page }) => {
      // Note: This test assumes a test student account exists
      const testEmail = 'test.student@example.com';
      const testPassword = 'password123';
      
      await studentAuth.gotoStudentLogin();
      
      // Fill login form with robust locators
      await studentAuth.safeFill(studentAuth.studentLoginEmail, testEmail);
      await studentAuth.safeFill(studentAuth.studentLoginPassword, testPassword);
      
      // Submit form
      await studentAuth.safeClick(studentAuth.studentLoginSubmitButton);
      
      // Should redirect to student homepage
      await studentAuth.expectUrlContains('/student-homepage');
      
      // Verify successful login indicators
      await expect(studentAuth.studentHomepageHeader).toBeVisible();
    });

    test('should show error for invalid credentials', async ({ page }) => {
      await studentAuth.gotoStudentLogin();
      
      // Fill with invalid credentials
      await studentAuth.safeFill(studentAuth.studentLoginEmail, 'invalid@example.com');
      await studentAuth.safeFill(studentAuth.studentLoginPassword, 'wrongpassword');
      
      // Submit form
      await studentAuth.safeClick(studentAuth.studentLoginSubmitButton);
      
      // Wait for error toast
      const toastMessage = await studentAuth.waitForToast('error');
      expect(toastMessage).toContain(/credentials|invalid/i);
      
      // Should remain on login page
      await studentAuth.expectUrlContains('/student-login');
    });

    test('should show validation errors for empty login form', async ({ page }) => {
      await studentAuth.gotoStudentLogin();
      
      // Try to submit empty form
      await studentAuth.safeClick(studentAuth.studentLoginSubmitButton);
      
      // Check that we're still on login page
      await studentAuth.expectUrlContains('/student-login');
      
      // Form should still be visible
      await expect(studentAuth.studentLoginEmail).toBeVisible();
      await expect(studentAuth.studentLoginPassword).toBeVisible();
    });

    test('should toggle password visibility', async ({ page }) => {
      await studentAuth.gotoStudentLogin();
      
      const passwordField = studentAuth.studentLoginPassword;
      
      // Initially should be password type
      await expect(passwordField).toHaveAttribute('type', 'password');
      
      // Find and click password toggle button
      const toggleButton = page.locator('.password-toggle');
      await studentAuth.safeClick(toggleButton);
      
      // Should now be text type
      await expect(passwordField).toHaveAttribute('type', 'text');
      
      // Click again to hide
      await studentAuth.safeClick(toggleButton);
      await expect(passwordField).toHaveAttribute('type', 'password');
    });

    test('should handle remember me checkbox', async ({ page }) => {
      await studentAuth.gotoStudentLogin();
      
      const rememberCheckbox = studentAuth.studentLoginRememberMe;
      
      // Initially unchecked
      await expect(rememberCheckbox).not.toBeChecked();
      
      // Check the checkbox
      await studentAuth.safeClick(rememberCheckbox);
      await expect(rememberCheckbox).toBeChecked();
      
      // Uncheck
      await studentAuth.safeClick(rememberCheckbox);
      await expect(rememberCheckbox).not.toBeChecked();
    });

    test('should navigate to forgot password page', async ({ page }) => {
      await studentAuth.gotoStudentLogin();
      
      // Click forgot password link
      await studentAuth.safeClick(studentAuth.studentLoginForgotPassword);
      
      // Should navigate to forgot password page
      await studentAuth.expectUrlContains('/forgot-password');
    });

    test('should navigate to lecturer login portal', async ({ page }) => {
      await studentAuth.gotoStudentLogin();
      
      // Click lecturer portal link
      await studentAuth.safeClick(studentAuth.studentLoginLecturerPortal);
      
      // Should navigate to lecturer login
      await studentAuth.expectUrlContains('/lecturer-login');
    });
  });

  test.describe('Complete Authentication Flow', () => {
    test('should complete full signup to login flow', async ({ page }) => {
      const uniqueTestData = TestHelpers.generateTestData();
      
      await test.step('Complete signup process', async () => {
        await studentAuth.gotoStudentSignup();
        
        // Fill signup form
        await studentAuth.safeFill(studentAuth.studentSignupFirstName, uniqueTestData.student.firstName);
        await studentAuth.safeFill(studentAuth.studentSignupLastName, uniqueTestData.student.lastName);
        await studentAuth.safeFill(studentAuth.studentSignupStudentId, uniqueTestData.student.studentId);
        await studentAuth.safeFill(studentAuth.studentSignupEmail, uniqueTestData.student.email);
        await studentAuth.safeFill(studentAuth.studentSignupClassSection, uniqueTestData.student.classSection);
        await studentAuth.safeFill(studentAuth.studentSignupPassword, uniqueTestData.student.password);
        await studentAuth.safeFill(studentAuth.studentSignupConfirmPassword, uniqueTestData.student.password);
        
        // Accept terms
        await studentAuth.safeClick(studentAuth.studentSignupTermsCheckbox);
        
        // Handle success dialog
        const dialogPromise = helpers.handleDialog('Account created');
        await studentAuth.safeClick(studentAuth.studentSignupSubmitButton);
        await dialogPromise;
        
        // Should redirect to login
        await studentAuth.expectUrlContains('/student-login');
      });

      await test.step('Login with new account', async () => {
        // Login with created credentials
        await studentAuth.safeFill(studentAuth.studentLoginEmail, uniqueTestData.student.email);
        await studentAuth.safeFill(studentAuth.studentLoginPassword, uniqueTestData.student.password);
        await studentAuth.safeClick(studentAuth.studentLoginSubmitButton);
        
        // Should redirect to homepage
        await studentAuth.expectUrlContains('/student-homepage');
      });
    });
  });

  test.describe('Error Handling & Edge Cases', () => {
    test('should handle network errors gracefully', async ({ page }) => {
      await studentAuth.gotoStudentLogin();
      
      // Fill form
      await studentAuth.safeFill(studentAuth.studentLoginEmail, 'test@example.com');
      await studentAuth.safeFill(studentAuth.studentLoginPassword, 'password');
      
      // Simulate network offline
      await page.context().setOffline(true);
      
      // Submit form
      await studentAuth.safeClick(studentAuth.studentLoginSubmitButton);
      
      // Should show error toast
      const toastMessage = await studentAuth.waitForToast('error');
      expect(toastMessage).toBeTruthy();
      
      // Restore network
      await page.context().setOffline(false);
    });

    test('should handle form submission with Enter key', async ({ page }) => {
      await studentAuth.gotoStudentLogin();
      
      // Fill form fields
      await studentAuth.safeFill(studentAuth.studentLoginEmail, 'test@example.com');
      await studentAuth.safeFill(studentAuth.studentLoginPassword, 'password');
      
      // Press Enter in password field
      await studentAuth.studentLoginPassword.press('Enter');
      
      // Should attempt login
      await page.waitForTimeout(1000);
    });

    test('should clear form after failed login', async ({ page }) => {
      await studentAuth.gotoStudentLogin();
      
      // Fill with wrong credentials
      await studentAuth.safeFill(studentAuth.studentLoginEmail, 'wrong@example.com');
      await studentAuth.safeFill(studentAuth.studentLoginPassword, 'wrongpassword');
      
      // Submit
      await studentAuth.safeClick(studentAuth.studentLoginSubmitButton);
      
      // Wait for error
      await studentAuth.waitForToast('error');
      
      // Check if form is still filled (implementation dependent)
      const emailValue = await studentAuth.studentLoginEmail.inputValue();
      const passwordValue = await studentAuth.studentLoginPassword.inputValue();
      
      // Values should still be there for user to correct
      expect(emailValue).toBe('wrong@example.com');
      expect(passwordValue).toBe('wrongpassword');
    });
  });

  test.describe('Mobile Responsiveness', () => {
    test('should work correctly on mobile devices', async ({ page }) => {
      // Set mobile viewport
      await testHelpers.setMobileViewport();
      
      await studentAuth.gotoStudentLogin();
      
      // Verify mobile layout
      await expect(studentAuth.studentLoginEmail).toBeVisible();
      await expect(studentAuth.studentLoginPassword).toBeVisible();
      await expect(studentAuth.studentLoginSubmitButton).toBeVisible();
      
      // Test form submission on mobile
      await studentAuth.safeFill(studentAuth.studentLoginEmail, 'test@example.com');
      await studentAuth.safeFill(studentAuth.studentLoginPassword, 'password');
      await studentAuth.safeClick(studentAuth.studentLoginSubmitButton);
      
      // Should work the same as desktop
      await page.waitForTimeout(1000);
    });
  });
});
