import { test, expect } from '@playwright/test';

test.describe('Lecturer Login', () => {
  const testLecturerEmail = 'lecturer@example.com';
  const testPassword = 'password123';

  test('should show error toast for incorrect login', async ({ page }) => {
    await page.goto('/lecturer-login');

    // Fill in incorrect credentials
    await page.fill('input[type="email"]', 'incorrect@example.com');
    await page.fill('input[type="password"]', 'wrongpassword');

    // Click the login button
    await page.click('button[type="submit"]');

    // Check for toast notification
    const toast = page.locator('.toast-error');
    await expect(toast).toBeVisible();
    await expect(toast).toContainText('credentials');

    // Assert that the URL has not changed
    await expect(page).toHaveURL('/lecturer-login');
  });
});
