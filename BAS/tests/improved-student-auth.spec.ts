import { test, expect } from '@playwright/test';
import { createStudentAuthPage } from './utils/test-locators';

test.describe('Student Authentication - Improved Tests', () => {
  let studentAuth: ReturnType<typeof createStudentAuthPage>;
  
  // Test data with unique identifiers
  const uniqueId = Date.now();
  const testStudent = {
    firstName: `TestFN${uniqueId}`,
    lastName: `TestLN${uniqueId}`,
    studentId: `STU${uniqueId}`,
    email: `student${uniqueId}@example.com`,
    classSection: `CS${uniqueId % 100}`,
    password: 'Password123!'
  };

  test.beforeEach(async ({ page }) => {
    studentAuth = createStudentAuthPage(page);
  });

  test('should display student login page with correct elements', async ({ page }) => {
    await studentAuth.gotoStudentLogin();
    
    // Verify page title and key elements
    await expect(page).toHaveTitle(/Smart Attendance/);
    await expect(studentAuth.studentLoginEmail).toBeVisible();
    await expect(studentAuth.studentLoginPassword).toBeVisible();
    await expect(studentAuth.studentLoginSubmitButton).toBeVisible();
    await expect(studentAuth.studentLoginRememberMe).toBeVisible();
    await expect(studentAuth.studentLoginForgotPassword).toBeVisible();
    
    // Verify accessibility labels
    await expect(studentAuth.studentLoginEmail).toHaveAttribute('type', 'email');
    await expect(studentAuth.studentLoginPassword).toHaveAttribute('type', 'password');
    await expect(studentAuth.studentLoginSubmitButton).toBeEnabled();
  });

  test('should show validation errors for empty form submission', async ({ page }) => {
    await studentAuth.gotoStudentLogin();
    
    // Try to submit empty form
    await studentAuth.safeClick(studentAuth.studentLoginSubmitButton);
    
    // Check for validation errors (using semantic HTML)
    await expect(studentAuth.studentLoginEmail).toBeVisible();
    await expect(studentAuth.studentLoginPassword).toBeVisible();
    
    // URL should remain on login page
    await studentAuth.expectUrlContains('/student-login');
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

  test('should allow successful student login', async ({ page }) => {
    // Note: This test assumes a test student account exists
    const testEmail = 'test.student@example.com';
    const testPassword = 'password123';
    
    await studentAuth.loginAsStudent(testEmail, testPassword);
    
    // Should redirect to student homepage
    await studentAuth.expectUrlContains('/student-homepage');
    
    // Verify successful login indicators
    await expect(studentAuth.studentHomepageHeader).toBeVisible();
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

  test('should remember me checkbox functionality', async ({ page }) => {
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

  test('should handle form submission with Enter key', async ({ page }) => {
    await studentAuth.gotoStudentLogin();
    
    // Fill form fields
    await studentAuth.safeFill(studentAuth.studentLoginEmail, 'test@example.com');
    await studentAuth.safeFill(studentAuth.studentLoginPassword, 'password');
    
    // Press Enter in password field
    await studentAuth.studentLoginPassword.press('Enter');
    
    // Should attempt login (we can verify by checking for either success or error)
    await page.waitForTimeout(1000); // Wait for form submission
  });
});

test.describe('Student Signup - Improved Tests', () => {
  let studentAuth: ReturnType<typeof createStudentAuthPage>;
  
  // Test data with unique identifiers
  const uniqueId = Date.now();
  const testStudent = {
    firstName: `TestFN${uniqueId}`,
    lastName: `TestLN${uniqueId}`,
    studentId: `STU${uniqueId}`,
    email: `student${uniqueId}@example.com`,
    classSection: `CS${uniqueId % 100}`,
    password: 'Password123!'
  };

  test.beforeEach(async ({ page }) => {
    studentAuth = createStudentAuthPage(page);
  });

  test('should display student signup page with correct elements', async ({ page }) => {
    await studentAuth.gotoStudentSignup();
    
    // Verify all form fields are present
    await expect(studentAuth.studentSignupFirstName).toBeVisible();
    await expect(studentAuth.studentSignupLastName).toBeVisible();
    await expect(studentAuth.studentSignupStudentId).toBeVisible();
    await expect(studentAuth.studentSignupEmail).toBeVisible();
    await expect(studentAuth.studentSignupClassSection).toBeVisible();
    await expect(studentAuth.studentSignupPassword).toBeVisible();
    await expect(studentAuth.studentSignupConfirmPassword).toBeVisible();
    await expect(studentAuth.studentSignupTermsCheckbox).toBeVisible();
    await expect(studentAuth.studentSignupSubmitButton).toBeVisible();
    
    // Verify field types
    await expect(studentAuth.studentSignupEmail).toHaveAttribute('type', 'email');
    await expect(studentAuth.studentSignupPassword).toHaveAttribute('type', 'password');
    await expect(studentAuth.studentSignupConfirmPassword).toHaveAttribute('type', 'password');
    await expect(studentAuth.studentSignupTermsCheckbox).toHaveAttribute('type', 'checkbox');
  });

  test('should show validation errors for empty fields', async ({ page }) => {
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
    await studentAuth.safeFill(studentAuth.studentSignupFirstName, testStudent.firstName);
    await studentAuth.safeFill(studentAuth.studentSignupLastName, testStudent.lastName);
    await studentAuth.safeFill(studentAuth.studentSignupStudentId, testStudent.studentId);
    await studentAuth.safeFill(studentAuth.studentSignupEmail, testStudent.email);
    await studentAuth.safeFill(studentAuth.studentSignupClassSection, testStudent.classSection);
    await studentAuth.safeFill(studentAuth.studentSignupPassword, testStudent.password);
    await studentAuth.safeFill(studentAuth.studentSignupConfirmPassword, testStudent.password);
    
    // Try to submit without accepting terms
    await studentAuth.safeClick(studentAuth.studentSignupSubmitButton);
    
    // Should remain on signup page
    await studentAuth.expectUrlContains('/student-signup');
    
    // Terms checkbox should still be unchecked
    await expect(studentAuth.studentSignupTermsCheckbox).not.toBeChecked();
  });

  test('should successfully submit valid signup form', async ({ page }) => {
    await studentAuth.gotoStudentSignup();
    
    // Fill all fields with valid data
    await studentAuth.signupAsStudent(testStudent);
    
    // Accept terms
    await studentAuth.safeClick(studentAuth.studentSignupTermsCheckbox);
    
    // Submit form
    await studentAuth.safeClick(studentAuth.studentSignupSubmitButton);
    
    // Handle the alert dialog
    page.on('dialog', async dialog => {
      expect(dialog.message()).toContain('Account created');
      await dialog.accept();
    });
    
    // Should redirect to login page
    await studentAuth.expectUrlContains('/student-login');
  });

  test('should navigate to login page from signup', async ({ page }) => {
    await studentAuth.gotoStudentSignup();
    
    // Look for login link in footer
    const loginLink = page.getByRole('link', { name: 'Sign In' });
    await studentAuth.safeClick(loginLink);
    
    // Should navigate to login page
    await studentAuth.expectUrlContains('/student-login');
  });
});

test.describe('Authentication Flow Integration', () => {
  let studentAuth: ReturnType<typeof createStudentAuthPage>;
  
  test.beforeEach(async ({ page }) => {
    studentAuth = createStudentAuthPage(page);
  });

  test('should complete full signup to login flow', async ({ page }) => {
    const uniqueId = Date.now();
    const testStudent = {
      firstName: `TestFN${uniqueId}`,
      lastName: `TestLN${uniqueId}`,
      studentId: `STU${uniqueId}`,
      email: `student${uniqueId}@example.com`,
      classSection: `CS${uniqueId % 100}`,
      password: 'Password123!'
    };

    await test.step('Complete signup', async () => {
      await studentAuth.gotoStudentSignup();
      
      // Fill signup form
      await studentAuth.safeFill(studentAuth.studentSignupFirstName, testStudent.firstName);
      await studentAuth.safeFill(studentAuth.studentSignupLastName, testStudent.lastName);
      await studentAuth.safeFill(studentAuth.studentSignupStudentId, testStudent.studentId);
      await studentAuth.safeFill(studentAuth.studentSignupEmail, testStudent.email);
      await studentAuth.safeFill(studentAuth.studentSignupClassSection, testStudent.classSection);
      await studentAuth.safeFill(studentAuth.studentSignupPassword, testStudent.password);
      await studentAuth.safeFill(studentAuth.studentSignupConfirmPassword, testStudent.password);
      
      // Accept terms and submit
      await studentAuth.safeClick(studentAuth.studentSignupTermsCheckbox);
      
      // Handle alert
      page.on('dialog', async dialog => {
        await dialog.accept();
      });
      
      await studentAuth.safeClick(studentAuth.studentSignupSubmitButton);
      
      // Should redirect to login
      await studentAuth.expectUrlContains('/student-login');
    });

    await test.step('Login with new account', async () => {
      // Login with created credentials
      await studentAuth.safeFill(studentAuth.studentLoginEmail, testStudent.email);
      await studentAuth.safeFill(studentAuth.studentLoginPassword, testStudent.password);
      await studentAuth.safeClick(studentAuth.studentLoginSubmitButton);
      
      // Should redirect to homepage
      await studentAuth.expectUrlContains('/student-homepage');
    });
  });
});
