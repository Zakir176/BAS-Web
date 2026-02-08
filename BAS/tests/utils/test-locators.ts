import { Page, Locator } from '@playwright/test';

/**
 * Robust locators for CAT application E2E tests
 * Uses semantic HTML, accessibility attributes, and test IDs for reliable element selection
 */

export class AppLocators {
  constructor(public page: Page) {}

  // ===== AUTHENTICATION LOCATORS =====
  
  // Student Login Page
  get studentLoginEmail() {
    return this.page.getByLabel('Student Email', { exact: true });
  }
  
  get studentLoginPassword() {
    return this.page.getByLabel('Password', { exact: true });
  }
  
  get studentLoginSubmitButton() {
    return this.page.getByRole('button', { name: /Log In to Portal/i });
  }
  
  get studentLoginRememberMe() {
    return this.page.getByLabel('Remember me');
  }
  
  get studentLoginForgotPassword() {
    return this.page.getByRole('link', { name: 'Forgot Password?' });
  }
  
  get studentLoginLecturerPortal() {
    return this.page.getByRole('link', { name: 'Lecturer Portal' });
  }

  // Lecturer Login Page
  get lecturerLoginEmail() {
    return this.page.getByLabel('Staff Email', { exact: true });
  }
  
  get lecturerLoginPassword() {
    return this.page.getByLabel('Password', { exact: true });
  }
  
  get lecturerLoginSubmitButton() {
    return this.page.getByRole('button', { name: /Sign In to Manage/i });
  }
  
  get lecturerLoginRememberMe() {
    return this.page.getByLabel('Remember me');
  }

  // Student Signup Page
  get studentSignupFirstName() {
    return this.page.getByLabel('First Name', { exact: true });
  }
  
  get studentSignupLastName() {
    return this.page.getByLabel('Last Name', { exact: true });
  }
  
  get studentSignupStudentId() {
    return this.page.getByLabel('Student ID Number', { exact: true });
  }
  
  get studentSignupEmail() {
    return this.page.getByLabel('University Email', { exact: true });
  }
  
  get studentSignupClassSection() {
    return this.page.getByLabel('Class Section (e.g. CS101)', { exact: true });
  }
  
  get studentSignupPassword() {
    return this.page.getByLabel('Password', { exact: true });
  }
  
  get studentSignupConfirmPassword() {
    return this.page.getByLabel('Confirm', { exact: true });
  }
  
  get studentSignupTermsCheckbox() {
    return this.page.getByLabel('I accept the terms of service');
  }
  
  get studentSignupSubmitButton() {
    return this.page.getByRole('button', { name: 'Create Student Account' });
  }

  // ===== NAVIGATION LOCATORS =====
  
  get navbarBrand() {
    return this.page.getByRole('link', { name: /Smart Attendance/i });
  }
  
  get navbarStudentLogin() {
    return this.page.getByRole('link', { name: 'Student Login' });
  }
  
  get navbarLecturerLogin() {
    return this.page.getByRole('link', { name: 'Lecturer Login' });
  }
  
  get navbarStudentSignup() {
    return this.page.getByRole('link', { name: 'Student Signup' });
  }
  
  get navbarLecturerSignup() {
    return this.page.getByRole('link', { name: 'Lecturer Signup' });
  }

  // ===== TOAST NOTIFICATION LOCATORS =====
  
  get toastContainer() {
    return this.page.locator('[data-testid="toast-container"]');
  }
  
  get toastSuccess() {
    return this.page.locator('.toast-success');
  }
  
  get toastError() {
    return this.page.locator('.toast-error');
  }
  
  get toastWarning() {
    return this.page.locator('.toast-warning');
  }
  
  get toastInfo() {
    return this.page.locator('.toast-info');
  }

  // ===== FORM VALIDATION LOCATORS =====
  
  errorMessage(fieldName: string) {
    return this.page.locator(`[data-testid="error-${fieldName}"]`);
  }
  
  inputError(fieldName: string) {
    return this.page.locator(`#${fieldName}.is-invalid`);
  }

  // ===== STUDENT HOMEPAGE LOCATORS =====
  
  get studentHomepageHeader() {
    return this.page.getByRole('heading', { name: /Welcome back/i });
  }
  
  get studentName() {
    return this.page.locator('.student-name');
  }
  
  get studentLogoutButton() {
    return this.page.getByRole('button', { name: /Logout/i });
  }

  // ===== LECTURHER DASHBOARD LOCATORS =====
  
  get lecturerDashboardHeader() {
    return this.page.getByRole('heading', { name: /Dashboard/i });
  }
  
  get lecturerLogoutButton() {
    return this.page.getByRole('button', { name: /Logout/i });
  }

  // ===== UTILITY METHODS =====
  
  /**
   * Wait for page to be fully loaded and interactive
   */
  async waitForPageLoad() {
    await this.page.waitForLoadState('networkidle');
    await this.page.waitForTimeout(100); // Small delay for Vue reactivity
  }

  /**
   * Fill form fields with provided data
   */
  async fillForm(fields: Record<string, string>) {
    for (const [fieldName, value] of Object.entries(fields)) {
      const locator = this.getFieldLocator(fieldName);
      if (locator) {
        await locator.fill(value);
      } else {
        throw new Error(`No locator found for field: ${fieldName}`);
      }
    }
  }

  /**
   * Get locator for a form field by name
   */
  getFieldLocator(fieldName: string): Locator | null {
    const fieldMap: Record<string, () => Locator> = {
      'firstName': () => this.studentSignupFirstName,
      'lastName': () => this.studentSignupLastName,
      'studentId': () => this.studentSignupStudentId,
      'email': () => this.studentSignupEmail,
      'classSection': () => this.studentSignupClassSection,
      'password': () => this.studentSignupPassword,
      'confirmPassword': () => this.studentSignupConfirmPassword,
      'agreeToTerms': () => this.studentSignupTermsCheckbox,
    };

    const locatorFn = fieldMap[fieldName];
    return locatorFn ? locatorFn() : null;
  }

  /**
   * Check if element is visible and enabled
   */
  async isInteractable(locator: Locator): Promise<boolean> {
    try {
      await locator.waitFor({ state: 'visible', timeout: 5000 });
      const isVisible = await locator.isVisible();
      const isEnabled = await locator.isEnabled();
      return isVisible && isEnabled;
    } catch {
      return false;
    }
  }

  /**
   * Safe click with retry logic
   */
  async safeClick(locator: Locator, timeout: number = 10000): Promise<void> {
    await locator.waitFor({ state: 'visible', timeout });
    await locator.scrollIntoViewIfNeeded();
    await locator.click({ timeout });
  }

  /**
   * Safe fill with clear and type
   */
  async safeFill(locator: Locator, value: string, timeout: number = 10000): Promise<void> {
    await locator.waitFor({ state: 'visible', timeout });
    await locator.scrollIntoViewIfNeeded();
    await locator.clear();
    await locator.fill(value);
  }

  /**
   * Wait for toast notification to appear and return its text
   */
  async waitForToast(type: 'success' | 'error' | 'warning' | 'info' = 'success'): Promise<string> {
    const toastLocator = this[`toast${type.charAt(0).toUpperCase() + type.slice(1)}`] as Locator;
    await toastLocator.waitFor({ state: 'visible', timeout: 10000 });
    const text = await toastLocator.textContent();
    return text || '';
  }

  /**
   * Check if URL contains expected path
   */
  async expectUrlContains(path: string): Promise<void> {
    await this.page.waitForURL(`**${path}**`);
    const currentUrl = this.page.url();
    if (!currentUrl.includes(path)) {
      throw new Error(`Expected URL to contain ${path}, but got ${currentUrl}`);
    }
  }

  /**
   * Take screenshot with timestamp
   */
  async takeScreenshot(name: string): Promise<void> {
    const timestamp = new Date().toISOString().replace(/[:.]/g, '-');
    await this.page.screenshot({ 
      path: `test-results/screenshots/${name}-${timestamp}.png`,
      fullPage: true 
    });
  }
}

/**
 * Page Object Model for Student Authentication
 */
export class StudentAuthPage extends AppLocators {
  async gotoStudentLogin(): Promise<void> {
    await this.page.goto('/student-login');
    await this.waitForPageLoad();
  }

  async gotoStudentSignup(): Promise<void> {
    await this.page.goto('/student-signup');
    await this.waitForPageLoad();
  }

  async loginAsStudent(email: string, password: string): Promise<void> {
    await this.gotoStudentLogin();
    await this.safeFill(this.studentLoginEmail, email);
    await this.safeFill(this.studentLoginPassword, password);
    await this.safeClick(this.studentLoginSubmitButton);
  }

  async signupAsStudent(data: {
    firstName: string;
    lastName: string;
    studentId: string;
    email: string;
    classSection: string;
    password: string;
  }): Promise<void> {
    await this.gotoStudentSignup();
    
    await this.safeFill(this.studentSignupFirstName, data.firstName);
    await this.safeFill(this.studentSignupLastName, data.lastName);
    await this.safeFill(this.studentSignupStudentId, data.studentId);
    await this.safeFill(this.studentSignupEmail, data.email);
    await this.safeFill(this.studentSignupClassSection, data.classSection);
    await this.safeFill(this.studentSignupPassword, data.password);
    await this.safeFill(this.studentSignupConfirmPassword, data.password);
    
    await this.safeClick(this.studentSignupTermsCheckbox);
    await this.safeClick(this.studentSignupSubmitButton);
  }
}

/**
 * Page Object Model for Lecturer Authentication
 */
export class LecturerAuthPage extends AppLocators {
  async gotoLecturerLogin(): Promise<void> {
    await this.page.goto('/lecturer-login');
    await this.waitForPageLoad();
  }

  async loginAsLecturer(email: string, password: string): Promise<void> {
    await this.gotoLecturerLogin();
    await this.safeFill(this.lecturerLoginEmail, email);
    await this.safeFill(this.lecturerLoginPassword, password);
    await this.safeClick(this.lecturerLoginSubmitButton);
  }
}

/**
 * Factory function to create page objects
 */
export function createStudentAuthPage(page: Page): StudentAuthPage {
  return new StudentAuthPage(page);
}

export function createLecturerAuthPage(page: Page): LecturerAuthPage {
  return new LecturerAuthPage(page);
}

export function createAppLocators(page: Page): AppLocators {
  return new AppLocators(page);
}
