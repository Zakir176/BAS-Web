# Deployment Guide

This guide explains how to deploy the Barcode Attendance System (BAS) to a production environment.

## Prerequisites

Before deploying, ensure you have:

*   A [Supabase](https://supabase.io/) project with the database schema applied.
*   Your `VITE_SUPABASE_URL` and `VITE_SUPABASE_ANON_KEY` values ready.

## Build

To create a production-ready build, run the following commands from the **repository root**:

```bash
cd BAS
npm install
npm run build
```

This generates a `BAS/dist/` directory containing the static assets to deploy.

## Deployment

BAS is a Vue Router **Single-Page Application (SPA)**. When deploying to a static host, you must configure a **redirect / rewrite rule** so that all URL paths serve `index.html`. Without this, direct navigation to any route (e.g., `/lecturer/dashboard`) will return a 404.

---

### Netlify

1.  **Sign up** at [netlify.com](https://www.netlify.com/) if you don't have an account.
2.  From your Netlify dashboard, click **"New site from Git"**.
3.  Connect your GitHub account and select the `BAS-Web` repository.
4.  **Configure build settings:**
    *   **Branch to deploy:** `main`
    *   **Build command:** `cd BAS && npm install && npm run build`
    *   **Publish directory:** `BAS/dist`
5.  **Set environment variables** under *Site settings → Environment variables*:
    *   `VITE_SUPABASE_URL` — Your Supabase project URL.
    *   `VITE_SUPABASE_ANON_KEY` — Your Supabase anonymous key.
6.  **Add SPA redirect rule.** Create a file at `BAS/public/_redirects` with the following content:

    ```
    /* /index.html 200
    ```

7.  Click **"Deploy site"**.

---

### Vercel

1.  **Sign up** at [vercel.com](https://vercel.com/) if you don't have an account.
2.  From your Vercel dashboard, click **"New Project"** and import the `BAS-Web` repository.
3.  **Configure project settings:**
    *   **Framework Preset:** `Vite`
    *   **Root Directory:** `BAS`
4.  **Set environment variables** under *Environment Variables*:
    *   `VITE_SUPABASE_URL` — Your Supabase project URL.
    *   `VITE_SUPABASE_ANON_KEY` — Your Supabase anonymous key.
5.  Vercel handles SPA routing automatically for Vite projects — no extra config needed.
6.  Click **"Deploy"**.

---

## Supabase Configuration for Production

*   **Row Level Security (RLS):** Ensure RLS is enabled on all tables. The app expects RLS policies to be in place — see `BAS/migrations/` for reference SQL.
*   **Realtime:** Enable Realtime replication on the `attendance_logs` table for live roster updates to work.
*   **Auth Redirect URLs:** In your Supabase project under *Authentication → URL Configuration*, add your production domain to the **Redirect URLs** list (e.g., `https://your-app.netlify.app/**`). This is required for password-reset and magic-link flows.
*   **Camera / HTTPS:** The barcode scanner requires HTTPS. Both Netlify and Vercel provision SSL automatically.
