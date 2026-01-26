import { test, expect } from '@playwright/test';

test.describe('Lecturer Login', () => {
  const testLecturerEmail = 'lecturer@example.com';
  const testPassword = 'password123';

  test('should show "Invalid credentials" for incorrect login', async ({ page }) => {
    await page.goto('/lecturer-login');

    // Fill in incorrect credentials
    await page.fill('input[type="email"]', testLecturerEmail);
    await page.fill('input[type="password"]', testPassword);

    // Set up a listener for the dialog/alert
    page.on('dialog', async dialog => {
      // Assert the message of the dialog
      expect(dialog.message()).toBe('Invalid credentials');
      // Accept the dialog
      await dialog.accept();
    });

    // Click the login button
    await page.click('button[type="submit"]');

    // Wait for the dialog to be handled
    await page.waitForEvent('dialog');

    // Assert that the URL has not changed
    await expect(page).toHaveURL('/lecturer-login');
  });
});
