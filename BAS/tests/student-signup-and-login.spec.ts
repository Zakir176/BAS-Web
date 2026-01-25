import { test, expect } from '@playwright/test';

test.describe('Student Signup and Login Flow', () => {
  const uniqueId = Date.now();
  const testFirstName = `TestFN${uniqueId}`;
  const testLastName = `TestLN${uniqueId}`;
  const testStudentId = `STU${uniqueId}`;
  const testEmail = `student${uniqueId}@example.com`;
  const testClassSection = `CS${uniqueId % 100}`;
  const testPassword = 'Password123!';

  test('should allow a new student to sign up and then log in', async ({ page }) => {
    // --- Signup Flow ---
    await test.step('Signup as a new student', async () => {
      await page.goto('/student-signup');

      await page.locator('.input-group-v2:has(label:has-text("First Name")) input').fill(testFirstName);
      await page.locator('.input-group-v2:has(label:has-text("Last Name")) input').fill(testLastName);
      await page.locator('.input-group-v2:has(label:has-text("Student ID Number")) input').fill(testStudentId);
      await page.locator('.input-group-v2:has(label:has-text("University Email")) input').fill(testEmail);
      await page.locator('.input-group-v2:has(label:has-text("Class Section (e.g. CS101)")) input').fill(testClassSection);
      await page.locator('.input-group-v2:has(label:has-text("Password")) input').first().fill(testPassword); // first() because there are two password fields
      await page.locator('.input-group-v2:has(label:has-text("Confirm")) input').fill(testPassword);

      await page.getByLabel('I accept the terms of service').evaluate(node => node.checked = true);

      await page.click('button[type="submit"]');

      // Expect alert and then redirection to login page
      // Playwright can handle alerts automatically or you can listen for them
      page.on('dialog', async dialog => {
        expect(dialog.message()).toContain('Account created! Please verify email.');
        await dialog.accept();
      });

      await expect(page).toHaveURL('/student-login');
      // Ensure the dialog listener is triggered before moving on
      await page.waitForTimeout(100); 
    });

    // --- Login Flow ---
    await test.step('Login with the newly created student account', async () => {
      // Assuming we are already on the /student-login page after signup
      // If not, navigate to it: await page.goto('/student-login');

      await page.fill('input[type="email"]', testEmail);
      await page.fill('input[type="password"]', testPassword);

      await page.click('button[type="submit"]');

      await expect(page).toHaveURL('/student-homepage');
      await expect(page.locator('h1.student-name')).toContainText(testFirstName);
    });
  });
});
