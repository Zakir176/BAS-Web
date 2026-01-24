import { test, expect } from '@playwright/test';

test.describe('Student Login', () => {
  test('should allow student to login successfully', async ({ page }) => {
    await page.goto('/student-login');

    // Fill in credentials
    await page.fill('input[type="email"]', 'test_student@example.com');
    await page.fill('input[type="password"]', 'password123');

    // Click the login button
    await page.click('button[type="submit"]');

    // Expect to be redirected to the student homepage
    await expect(page).toHaveURL('/student-homepage');

    // Optionally, check for an element on the student homepage to confirm login
    await expect(page.locator('h1')).toHaveText(/Welcome, Student!/); // Assuming a welcome message
  });

  // You can add more tests here, e.g., for invalid credentials, empty fields, etc.
});
