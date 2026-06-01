# 🛠 Troubleshooting & FAQ

This guide addresses common technical hurdles and frequently asked questions for developers and users of the Barcode Attendance System (BAS).

---

## 📸 Barcode Scanner Issues

### 1. Camera Access Denied
**Issue:** The browser or app cannot access the camera.
*   **Fix (Web):** Ensure the site is served over **HTTPS**. Modern browsers block camera access on non-secure origins (except `localhost`).
*   **Fix (Mobile):** Check if you have granted camera permissions to the browser (Chrome/Safari) or the Native App in your device settings.

### 2. Scanner Won't Detect Barcode
**Issue:** The red laser/line appears but doesn't "beep" or record attendance.
*   **Lighting:** Ensure the student ID is well-lit. Shadows or glare on the barcode can prevent detection.
*   **Focus:** Maintain a distance of about 10-15cm. If the camera is too close, it may not be able to focus.
*   **Barcode Type:** BAS is optimized for standard 1D barcodes. Ensure the student IDs are using supported formats (Code 128, EAN, etc.).

---

## 🔐 Authentication & Permissions

### 1. "User Not Found" or Login Failure
*   **Credentials:** Verify the email and password in the Supabase Dashboard (`Authentication` tab).
*   **Roles:** Ensure the user has the correct role in their profile table (`students` or `teachers`). The login logic checks these tables to redirect to the correct dashboard.

### 2. Permission Denied (403 Forbidden)
**Issue:** API calls fail with a 403 error even when logged in.
*   **RLS Policies:** This usually means the Row Level Security (RLS) policies on Supabase are blocking the request. Check if the user is "enrolled" in the course they are trying to access.
*   **Session Expiry:** Try logging out and back in to refresh the JWT token.

---

## 🌐 Connectivity & Database

### 1. Changes Not Appearing in Real-time
*   **Realtime Enablement:** Ensure that "Realtime" is enabled for the specific table (e.g., `attendance`) in the Supabase Dashboard under `Database -> Replication`.
*   **Network:** WebSocket connections (used for Realtime) may be blocked by some campus firewalls or VPNs.

### 2. Supabase Connection Error
*   **Env Variables:** Double-check your `VITE_SUPABASE_URL` and `VITE_SUPABASE_ANON_KEY` in your `.env` file.
*   **Service Status:** Check the [Supabase Status Page](https://status.supabase.com/) to see if there are any ongoing outages.

---

## ❓ Frequently Asked Questions

**Q: Can I use this without an internet connection?**
A: Currently, BAS requires an active connection to Supabase for authentication and data sync. Offline support is a planned feature for the mobile client.

**Q: How do I reset the entire database for a new semester?**
A: You can use the `BAS/migrations/NUCLEAR_RESET.sql` script, but **be extremely careful** as this will wipe all attendance data.

---

*Found a bug not listed here? Please report it via GitHub Issues!*
