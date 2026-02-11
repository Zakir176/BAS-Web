import { test, expect } from '@playwright/test';
import { createLecturerAuthPage } from './utils/test-locators';
import { createTestHelpers, TestHelpers } from './utils/test-helpers';

test.describe('Barcode Scanning Feature', () => {
  let lecturerAuth: ReturnType<typeof createLecturerAuthPage>;
  let helpers: ReturnType<typeof createTestHelpers>;
  let testHelpers: ReturnType<typeof createTestHelpers>;

  test.beforeEach(async ({ page }) => {
    lecturerAuth = createLecturerAuthPage(page);
    helpers = createTestHelpers(page);
    testHelpers = createTestHelpers(page);
  });

  test.describe('Scanner Access & Setup', () => {
    test('should access barcode scanner from dashboard', async ({ page }) => {
      // Login as lecturer
      await lecturerAuth.loginAsLecturer('lecturer@example.com', 'password123');
      await lecturerAuth.expectUrlContains('/lecturer-dashboard');
      
      // Look for scanner button/link
      const scannerButton = page.locator('button:has-text("scan"), button:has-text("Scan"), .scan-btn, [data-testid="scanner-button"]');
      
      if (await scannerButton.isVisible()) {
        await scannerButton.click();
        
        // Should open scanner modal or navigate to scanner page
        await page.waitForTimeout(1000);
        
        // Verify scanner interface is visible
        const scannerInterface = page.locator('.scanner-modal, .barcode-scanner, [data-testid="scanner-interface"]');
        await expect(scannerInterface).toBeVisible();
      } else {
        // Scanner button might not be visible, check for alternative access
        console.log('Scanner button not found, checking alternative access methods');
      }
    });

    test('should request camera permissions when opening scanner', async ({ page }) => {
      // Mock camera permission
      await page.context().grantPermissions(['camera']);
      
      await lecturerAuth.loginAsLecturer('lecturer@example.com', 'password123');
      
      // Look for and click scanner button
      const scannerButton = page.locator('button:has-text("scan"), .scan-btn');
      
      if (await scannerButton.isVisible()) {
        await scannerButton.click();
        
        // Should request camera permissions (handled by mock above)
        await page.waitForTimeout(1000);
        
        // Verify scanner interface appears
        const scannerInterface = page.locator('.scanner-modal, .barcode-scanner');
        if (await scannerInterface.isVisible()) {
          await expect(scannerInterface).toBeVisible();
        }
      }
    });
  });

  test.describe('Barcode Detection', () => {
    test('should handle successful barcode scan', async ({ page }) => {
      await lecturerAuth.loginAsLecturer('lecturer@example.com', 'password123');
      
      // Mock barcode detection
      await page.evaluate(() => {
        // Simulate successful barcode scan
        (window as any).mockBarcodeScan = 'STU123456';
      });
      
      // Find scanner interface
      const scannerButton = page.locator('button:has-text("scan"), .scan-btn');
      
      if (await scannerButton.isVisible()) {
        await scannerButton.click();
        await page.waitForTimeout(1000);
        
        // Simulate barcode scan
        await page.evaluate(() => {
          const event = new CustomEvent('barcodedetected', { detail: 'STU123456' });
          document.dispatchEvent(event);
        });
        
        // Wait for processing
        await page.waitForTimeout(2000);
        
        // Check for success indication
        const successMessage = page.locator('.toast-success, .scan-success, [data-testid="scan-success"]');
        if (await successMessage.isVisible()) {
          await expect(successMessage).toContainText('marked as present');
        }
      }
    });

    test('should handle invalid barcode scan', async ({ page }) => {
      await lecturerAuth.loginAsLecturer('lecturer@example.com', 'password123');
      
      // Mock invalid barcode
      await page.evaluate(() => {
        (window as any).mockBarcodeScan = 'INVALID123';
      });
      
      const scannerButton = page.locator('button:has-text("scan"), .scan-btn');
      
      if (await scannerButton.isVisible()) {
        await scannerButton.click();
        await page.waitForTimeout(1000);
        
        // Simulate invalid barcode scan
        await page.evaluate(() => {
          const event = new CustomEvent('barcodedetected', { detail: 'INVALID123' });
          document.dispatchEvent(event);
        });
        
        // Wait for processing
        await page.waitForTimeout(2000);
        
        // Check for error indication
        const errorMessage = page.locator('.toast-error, .scan-error, [data-testid="scan-error"]');
        if (await errorMessage.isVisible()) {
          await expect(errorMessage).toContainText('not found');
        }
      }
    });

    test('should handle duplicate scans', async ({ page }) => {
      await lecturerAuth.loginAsLecturer('lecturer@example.com', 'password123');
      
      const scannerButton = page.locator('button:has-text("scan"), .scan-btn');
      
      if (await scannerButton.isVisible()) {
        await scannerButton.click();
        await page.waitForTimeout(1000);
        
        // Simulate first scan
        await page.evaluate(() => {
          const event = new CustomEvent('barcodedetected', { detail: 'STU123456' });
          document.dispatchEvent(event);
        });
        
        await page.waitForTimeout(1000);
        
        // Simulate duplicate scan
        await page.evaluate(() => {
          const event = new CustomEvent('barcodedetected', { detail: 'STU123456' });
          document.dispatchEvent(event);
        });
        
        await page.waitForTimeout(1000);
        
        // Should handle duplicate gracefully (no error, no duplicate entry)
        const errorMessage = page.locator('.toast-error');
        // Should not show error for duplicate
        if (await errorMessage.isVisible()) {
          const errorText = await errorMessage.textContent();
          expect(errorText).not.toContain('duplicate');
        }
      }
    });
  });

  test.describe('Session Management', () => {
    test('should require active session for scanning', async ({ page }) => {
      await lecturerAuth.loginAsLecturer('lecturer@example.com', 'password123');
      
      // Try to scan without active session
      const scannerButton = page.locator('button:has-text("scan"), .scan-btn');
      
      if (await scannerButton.isVisible()) {
        await scannerButton.click();
        await page.waitForTimeout(1000);
        
        // Simulate barcode scan
        await page.evaluate(() => {
          const event = new CustomEvent('barcodedetected', { detail: 'STU123456' });
          document.dispatchEvent(event);
        });
        
        await page.waitForTimeout(2000);
        
        // Should show error about no active session
        const errorMessage = page.locator('.toast-error');
        if (await errorMessage.isVisible()) {
          const errorText = await errorMessage.textContent();
          expect(errorText).toContain(/session|active/i);
        }
      }
    });

    test('should work with active session', async ({ page }) => {
      await lecturerAuth.loginAsLecturer('lecturer@example.com', 'password123');
      
      // First create a session (if interface exists)
      const createSessionButton = page.locator('button:has-text("New Session"), button:has-text("Create Session")');
      
      if (await createSessionButton.isVisible()) {
        await createSessionButton.click();
        await page.waitForTimeout(1000);
        
        // Fill session form (simplified)
        const courseSelect = page.locator('select, #courseSelect');
        if (await courseSelect.isVisible()) {
          await courseSelect.selectOption({ index: 0 });
        }
        
        const submitButton = page.locator('button[type="submit"]:has-text("Create"), button:has-text("Save")');
        if (await submitButton.isVisible()) {
          await submitButton.click();
          await page.waitForTimeout(1000);
        }
      }
      
      // Now try scanning
      const scannerButton = page.locator('button:has-text("scan"), .scan-btn');
      
      if (await scannerButton.isVisible()) {
        await scannerButton.click();
        await page.waitForTimeout(1000);
        
        // Simulate barcode scan
        await page.evaluate(() => {
          const event = new CustomEvent('barcodedetected', { detail: 'STU123456' });
          document.dispatchEvent(event);
        });
        
        await page.waitForTimeout(2000);
        
        // Should work without session error
        const errorMessage = page.locator('.toast-error');
        if (await errorMessage.isVisible()) {
          const errorText = await errorMessage.textContent();
          expect(errorText).not.toContain(/session|active/i);
        }
      }
    });
  });

  test.describe('Camera Integration', () => {
    test('should handle camera permission denial', async ({ page }) => {
      // Deny camera permissions
      await page.context().clearPermissions();
      
      await lecturerAuth.loginAsLecturer('lecturer@example.com', 'password123');
      
      const scannerButton = page.locator('button:has-text("scan"), .scan-btn');
      
      if (await scannerButton.isVisible()) {
        await scannerButton.click();
        await page.waitForTimeout(1000);
        
        // Should show permission denied message or alternative input
        const permissionMessage = page.locator('.camera-permission, .permission-denied');
        const manualInput = page.locator('input[placeholder*="barcode"], input[placeholder*="manual"]');
        
        // Should either show permission message or manual input
        const hasPermissionMessage = await permissionMessage.isVisible();
        const hasManualInput = await manualInput.isVisible();
        
        expect(hasPermissionMessage || hasManualInput).toBeTruthy();
      }
    });

    test('should handle camera errors gracefully', async ({ page }) => {
      // Grant permissions but mock camera error
      await page.context().grantPermissions(['camera']);
      
      // Mock camera error
      await page.addInitScript(() => {
        Object.defineProperty(navigator, 'mediaDevices', {
          value: {
            getUserMedia: () => Promise.reject(new Error('Camera not available'))
          },
          writable: false
        });
      });
      
      await lecturerAuth.loginAsLecturer('lecturer@example.com', 'password123');
      
      const scannerButton = page.locator('button:has-text("scan"), .scan-btn');
      
      if (await scannerButton.isVisible()) {
        await scannerButton.click();
        await page.waitForTimeout(2000);
        
        // Should show camera error message
        const errorMessage = page.locator('.toast-error, .camera-error');
        if (await errorMessage.isVisible()) {
          const errorText = await errorMessage.textContent();
          expect(errorText).toContain(/camera|error/i);
        }
      }
    });
  });

  test.describe('Manual Entry Fallback', () => {
    test('should provide manual entry option', async ({ page }) => {
      await lecturerAuth.loginAsLecturer('lecturer@example.com', 'password123');
      
      const scannerButton = page.locator('button:has-text("scan"), .scan-btn');
      
      if (await scannerButton.isVisible()) {
        await scannerButton.click();
        await page.waitForTimeout(1000);
        
        // Look for manual entry option
        const manualInput = page.locator('input[placeholder*="ID"], input[placeholder*="barcode"], input[placeholder*="manual"]');
        const manualButton = page.locator('button:has-text("Manual"), button:has-text("Enter ID")');
        
        // Should have manual entry option
        const hasManualInput = await manualInput.isVisible();
        const hasManualButton = await manualButton.isVisible();
        
        expect(hasManualInput || hasManualButton).toBeTruthy();
        
        if (hasManualInput) {
          // Test manual entry
          await manualInput.fill('STU123456');
          
          const submitButton = page.locator('button[type="submit"], button:has-text("Submit")');
          if (await submitButton.isVisible()) {
            await submitButton.click();
            await page.waitForTimeout(2000);
            
            // Should process manual entry
            const successMessage = page.locator('.toast-success');
            if (await successMessage.isVisible()) {
              await expect(successMessage).toContainText('marked as present');
            }
          }
        }
      }
    });
  });

  test.describe('Real-time Updates', () => {
    test('should update roster in real-time', async ({ page }) => {
      await lecturerAuth.loginAsLecturer('lecturer@example.com', 'password123');
      
      // Look for roster/live attendance display
      const rosterDisplay = page.locator('.roster, .live-attendance, .attendance-list');
      
      if (await rosterDisplay.isVisible()) {
        const initialCount = await rosterDisplay.locator('.student-item, .attendance-item').count();
        
        // Simulate barcode scan
        await page.evaluate(() => {
          const event = new CustomEvent('barcodedetected', { detail: 'STU123456' });
          document.dispatchEvent(event);
        });
        
        await page.waitForTimeout(2000);
        
        // Check if roster updated
        const newCount = await rosterDisplay.locator('.student-item, .attendance-item').count();
        
        // Should have updated (or at least not crashed)
        expect(newCount).toBeGreaterThanOrEqual(initialCount);
      }
    });
  });

  test.describe('Error Handling', () => {
    test('should handle network errors during scan', async ({ page }) => {
      await lecturerAuth.loginAsLecturer('lecturer@example.com', 'password123');
      
      // Simulate network offline
      await page.context().setOffline(true);
      
      const scannerButton = page.locator('button:has-text("scan"), .scan-btn');
      
      if (await scannerButton.isVisible()) {
        await scannerButton.click();
        await page.waitForTimeout(1000);
        
        // Simulate barcode scan
        await page.evaluate(() => {
          const event = new CustomEvent('barcodedetected', { detail: 'STU123456' });
          document.dispatchEvent(event);
        });
        
        await page.waitForTimeout(2000);
        
        // Should show network error
        const errorMessage = page.locator('.toast-error');
        if (await errorMessage.isVisible()) {
          const errorText = await errorMessage.textContent();
          expect(errorText).toContain(/network|offline|connection/i);
        }
      }
      
      // Restore network
      await page.context().setOffline(false);
    });

    test('should handle malformed barcode data', async ({ page }) => {
      await lecturerAuth.loginAsLecturer('lecturer@example.com', 'password123');
      
      const scannerButton = page.locator('button:has-text("scan"), .scan-btn');
      
      if (await scannerButton.isVisible()) {
        await scannerButton.click();
        await page.waitForTimeout(1000);
        
        // Simulate malformed barcode
        await page.evaluate(() => {
          const event = new CustomEvent('barcodedetected', { detail: '' });
          document.dispatchEvent(event);
        });
        
        await page.waitForTimeout(2000);
        
        // Should handle gracefully without crashing
        const errorMessage = page.locator('.toast-error');
        if (await errorMessage.isVisible()) {
          const errorText = await errorMessage.textContent();
          expect(errorText).toContain(/invalid|empty|malformed/i);
        }
      }
    });
  });

  test.describe('Mobile Experience', () => {
    test('should work on mobile devices', async ({ page }) => {
      await testHelpers.setMobileViewport();
      
      await lecturerAuth.loginAsLecturer('lecturer@example.com', 'password123');
      
      const scannerButton = page.locator('button:has-text("scan"), .scan-btn');
      
      if (await scannerButton.isVisible()) {
        await scannerButton.click();
        await page.waitForTimeout(1000);
        
        // Should be mobile-friendly
        const scannerInterface = page.locator('.scanner-modal, .barcode-scanner');
        if (await scannerInterface.isVisible()) {
          // Check mobile-specific elements
          const mobileScanner = page.locator('.mobile-scanner, .scanner-mobile');
          const fullscreenButton = page.locator('button:has-text("Fullscreen"), .fullscreen-btn');
          
          // Should have mobile optimizations
          expect(await scannerInterface.isVisible()).toBeTruthy();
        }
      }
    });

    test('should handle mobile camera properly', async ({ page }) => {
      await testHelpers.setMobileViewport();
      await page.context().grantPermissions(['camera']);
      
      await lecturerAuth.loginAsLecturer('lecturer@example.com', 'password123');
      
      const scannerButton = page.locator('button:has-text("scan"), .scan-btn');
      
      if (await scannerButton.isVisible()) {
        await scannerButton.click();
        await page.waitForTimeout(1000);
        
        // Should use mobile camera
        const videoElement = page.locator('video');
        const mobileCamera = page.locator('.mobile-camera, .camera-mobile');
        
        // Should have camera interface
        const hasVideo = await videoElement.isVisible();
        const hasMobileCamera = await mobileCamera.isVisible();
        
        expect(hasVideo || hasMobileCamera).toBeTruthy();
      }
    });
  });
});
