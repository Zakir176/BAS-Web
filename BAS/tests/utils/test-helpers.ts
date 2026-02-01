import { Page, expect } from '@playwright/test';

/**
 * Test helpers and utilities for CAT application E2E tests
 * Provides common functionality for test setup, data generation, and assertions
 */

export class TestHelpers {
  constructor(private page: Page) {}

  /**
   * Generate unique test data with timestamp
   */
  static generateTestData() {
    const timestamp = Date.now();
    const randomSuffix = Math.floor(Math.random() * 1000);
    
    return {
      timestamp,
      student: {
        firstName: `TestFN${timestamp}`,
        lastName: `TestLN${timestamp}`,
        studentId: `STU${timestamp}`,
        email: `student${timestamp}@example.com`,
        classSection: `CS${timestamp % 100}`,
        password: 'Password123!'
      },
      lecturer: {
        firstName: `ProfFN${timestamp}`,
        lastName: `ProfLN${timestamp}`,
        email: `lecturer${timestamp}@university.edu`,
        password: 'Password123!'
      }
    };
  }

  /**
   * Wait for and handle any modal/dialog
   */
  async handleDialog(expectedMessage?: string): Promise<string> {
    return new Promise((resolve) => {
      this.page.once('dialog', async (dialog) => {
        const message = dialog.message();
        if (expectedMessage) {
          expect(message).toContain(expectedMessage);
        }
        await dialog.accept();
        resolve(message);
      });
    });
  }

  /**
   * Take screenshot with automatic naming
   */
  async takeScreenshot(name: string): Promise<void> {
    const timestamp = new Date().toISOString().replace(/[:.]/g, '-');
    await this.page.screenshot({ 
      path: `test-results/screenshots/${name}-${timestamp}.png`,
      fullPage: true 
    });
  }

  /**
   * Wait for network idle and Vue reactivity
   */
  async waitForAppReady(): Promise<void> {
    await this.page.waitForLoadState('networkidle');
    await this.page.waitForTimeout(200); // Allow Vue to re-render
  }

  /**
   * Check if element has specific CSS class
   */
  async hasClass(selector: string, className: string): Promise<boolean> {
    const element = this.page.locator(selector).first();
    const classes = await element.getAttribute('class');
    return classes ? classes.includes(className) : false;
  }

  /**
   * Get text content of element
   */
  async getText(selector: string): Promise<string> {
    return await this.page.locator(selector).first().textContent() || '';
  }

  /**
   * Check if element exists and is visible
   */
  async isVisible(selector: string): Promise<boolean> {
    try {
      await this.page.locator(selector).first().waitFor({ state: 'visible', timeout: 2000 });
      return true;
    } catch {
      return false;
    }
  }

  /**
   * Wait for URL to contain specific path
   */
  async waitForUrl(path: string, timeout: number = 10000): Promise<void> {
    await this.page.waitForURL(`**${path}**`, { timeout });
  }

  /**
   * Fill form with data object
   */
  async fillForm(data: Record<string, string>): Promise<void> {
    for (const [field, value] of Object.entries(data)) {
      const selector = this.getFormFieldSelector(field);
      if (selector) {
        await this.page.locator(selector).fill(value);
      }
    }
  }

  /**
   * Get form field selector based on field name
   */
  private getFormFieldSelector(fieldName: string): string | null {
    const fieldMap: Record<string, string> = {
      'firstName': 'input[name="firstName"], #firstName',
      'lastName': 'input[name="lastName"], #lastName',
      'studentId': 'input[name="studentId"], #studentId',
      'email': 'input[type="email"], input[name="email"]',
      'classSection': 'input[name="classSection"], #classSection',
      'password': 'input[type="password"]:not([name="confirmPassword"])',
      'confirmPassword': 'input[name="confirmPassword"]'
    };

    return fieldMap[fieldName] || null;
  }

  /**
   * Clear all cookies and storage
   */
  async clearSession(): Promise<void> {
    await this.page.context().clearCookies();
    await this.page.evaluate(() => {
      sessionStorage.clear();
      localStorage.clear();
    });
  }

  /**
   * Set viewport to mobile size
   */
  async setMobileViewport(): Promise<void> {
    await this.page.setViewportSize({ width: 375, height: 667 });
  }

  /**
   * Set viewport to desktop size
   */
  async setDesktopViewport(): Promise<void> {
    await this.page.setViewportSize({ width: 1920, height: 1080 });
  }

  /**
   * Simulate slow network
   */
  async setSlowNetwork(): Promise<void> {
    await this.page.route('**/*', async (route) => {
      // Add delay to simulate slow network
      await new Promise(resolve => setTimeout(resolve, 1000));
      await route.continue();
    });
  }

  /**
   * Simulate offline mode
   */
  async setOffline(): Promise<void> {
    await this.page.context().setOffline(true);
  }

  /**
   * Restore online mode
   */
  async setOnline(): Promise<void> {
    await this.page.context().setOffline(false);
  }

  /**
   * Mock API responses
   */
  async mockApi(endpoint: string, response: any, status: number = 200): Promise<void> {
    await this.page.route(endpoint, async (route) => {
      await route.fulfill({
        status,
        contentType: 'application/json',
        body: JSON.stringify(response)
      });
    });
  }

  /**
   * Check console for errors
   */
  async checkConsoleErrors(): Promise<string[]> {
    const errors: string[] = [];
    
    this.page.on('console', (msg) => {
      if (msg.type() === 'error') {
        errors.push(msg.text());
      }
    });
    
    return errors;
  }

  /**
   * Wait for element to be enabled
   */
  async waitForEnabled(selector: string, timeout: number = 5000): Promise<void> {
    await this.page.locator(selector).waitFor({ state: 'visible', timeout });
    // Wait for element to be enabled by checking the disabled attribute
    await this.page.waitForFunction(
      ({ selector }) => {
        const element = document.querySelector(selector);
        return element && !element.hasAttribute('disabled');
      },
      { selector },
      { timeout }
    );
  }

  /**
   * Check accessibility of current page
   */
  async checkAccessibility(): Promise<void> {
    // This would integrate with an accessibility testing library
    // For now, just check for basic accessibility issues
    const images = await this.page.locator('img:not([alt])').count();
    if (images > 0) {
      console.warn(`Found ${images} images without alt text`);
    }
    
    const buttons = await this.page.locator('button:not([aria-label]):not([aria-labelledby])').count();
    if (buttons > 0) {
      console.warn(`Found ${buttons} buttons without accessible labels`);
    }
  }

  /**
   * Generate random string
   */
  static randomString(length: number = 10): string {
    const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    let result = '';
    for (let i = 0; i < length; i++) {
      result += chars.charAt(Math.floor(Math.random() * chars.length));
    }
    return result;
  }

  /**
   * Generate valid email
   */
  static generateEmail(prefix?: string): string {
    const timestamp = Date.now();
    const randomSuffix = Math.floor(Math.random() * 1000);
    const emailPrefix = prefix || `test${timestamp}${randomSuffix}`;
    return `${emailPrefix}@example.com`;
  }

  /**
   * Generate valid student ID
   */
  static generateStudentId(): string {
    const timestamp = Date.now();
    return `STU${timestamp}`;
  }

  /**
   * Wait for toast notification to disappear
   */
  async waitForToastToDisappear(timeout: number = 10000): Promise<void> {
    try {
      await this.page.locator('.toast').waitFor({ state: 'hidden', timeout });
    } catch {
      // Toast might not exist, which is fine
    }
  }

  /**
   * Get current URL path
   */
  async getCurrentPath(): Promise<string> {
    const url = this.page.url();
    return new URL(url).pathname;
  }

  /**
   * Check if element has specific attribute
   */
  async hasAttribute(selector: string, attribute: string, value?: string): Promise<boolean> {
    const element = this.page.locator(selector).first();
    const attrValue = await element.getAttribute(attribute);
    
    if (value === undefined) {
      return attrValue !== null;
    }
    
    return attrValue === value;
  }

  /**
   * Hover over element
   */
  async hover(selector: string): Promise<void> {
    await this.page.locator(selector).first().hover();
  }

  /**
   * Focus on element
   */
  async focus(selector: string): Promise<void> {
    await this.page.locator(selector).first().focus();
  }

  /**
   * Press key combination
   */
  async pressKeys(keys: string[]): Promise<void> {
    for (const key of keys) {
      await this.page.keyboard.press(key);
    }
  }

  /**
   * Scroll to element
   */
  async scrollToElement(selector: string): Promise<void> {
    await this.page.locator(selector).first().scrollIntoViewIfNeeded();
  }

  /**
   * Get element count
   */
  async getElementCount(selector: string): Promise<number> {
    return await this.page.locator(selector).count();
  }

  /**
   * Wait for element count to match expected
   */
  async waitForElementCount(selector: string, expectedCount: number, timeout: number = 5000): Promise<void> {
    await this.page.waitForFunction(
      ({ selector, expectedCount }) => {
        return document.querySelectorAll(selector).length === expectedCount;
      },
      { selector, expectedCount },
      { timeout }
    );
  }
}

/**
 * Factory function to create test helpers
 */
export function createTestHelpers(page: Page): TestHelpers {
  return new TestHelpers(page);
}

/**
 * Common test data fixtures
 */
export const TestData = {
  validStudent: {
    firstName: 'John',
    lastName: 'Doe',
    studentId: 'STU123456',
    email: 'john.doe@university.edu',
    classSection: 'CS101',
    password: 'Password123!'
  },
  
  validLecturer: {
    firstName: 'Jane',
    lastName: 'Smith',
    email: 'jane.smith@university.edu',
    password: 'Password123!'
  },
  
  invalidCredentials: {
    email: 'invalid@example.com',
    password: 'wrongpassword'
  }
};

/**
 * Common assertions
 */
export class CustomAssertions {
  static async expectToast(page: Page, type: 'success' | 'error' | 'warning' | 'info', message?: string): Promise<void> {
    const toastSelector = `.toast-${type}`;
    await expect(page.locator(toastSelector)).toBeVisible();
    
    if (message) {
      const toastElement = page.locator(toastSelector);
      await expect(toastElement).toContainText(message);
    }
  }

  static async expectFormValidation(page: Page, fieldName: string): Promise<void> {
    const fieldSelector = `#${fieldName}.is-invalid`;
    await expect(page.locator(fieldSelector)).toBeVisible();
  }

  static async expectUrlToContain(page: Page, path: string): Promise<void> {
    await expect(page).toHaveURL(new RegExp(path));
  }

  static async expectElementToBeEnabled(page: Page, selector: string): Promise<void> {
    await expect(page.locator(selector)).toBeEnabled();
  }

  static async expectElementToBeDisabled(page: Page, selector: string): Promise<void> {
    await expect(page.locator(selector)).toBeDisabled();
  }
}
