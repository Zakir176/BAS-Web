# Deployment Guide

This guide explains how to deploy the Barcode Attendance System (BAS) to a production environment.

## Build

Before deploying, you need to build the application. This will generate a `dist` directory with the production-ready files.

To build the application, run the following command in the `BAS` directory:

```bash
npm run build
```

## Deployment

You can deploy the application to any static hosting service. Here are instructions for deploying to Netlify and Vercel.

### Netlify

1.  **Sign up for a Netlify account:** If you don't have one already, sign up for a free account at [netlify.com](https://www.netlify.com/).
2.  **Create a new site:** From your Netlify dashboard, click the "New site from Git" button.
3.  **Connect to your Git provider:** Connect Netlify to your GitHub, GitLab, or Bitbucket account.
4.  **Select your repository:** Choose the repository for the Barcode Attendance System.
5.  **Configure your build settings:**
    *   **Branch to deploy:** `main` (or your preferred branch)
    *   **Build command:** `npm run build`
    *   **Publish directory:** `BAS/dist`
6.  **Set up environment variables:** In the "Advanced build settings" section, add your Supabase environment variables:
    *   `VITE_SUPABASE_URL`: Your Supabase project URL.
    *   `VITE_SUPABASE_ANON_KEY`: Your Supabase anonymous key.
7.  **Deploy your site:** Click the "Deploy site" button.

### Vercel

1.  **Sign up for a Vercel account:** If you don't have one already, sign up for a free account at [vercel.com](https://www.vercel.com/).
2.  **Create a new project:** From your Vercel dashboard, click the "New Project" button.
3.  **Import your Git repository:** Select your Git provider and choose the repository for the Barcode Attendance System.
4.  **Configure your project:**
    *   **Framework Preset:** `Vite`
    *   **Root Directory:** `BAS`
5.  **Set up environment variables:** In the "Environment Variables" section, add your Supabase environment variables:
    *   `VITE_SUPABASE_URL`: Your Supabase project URL.
    *   `VITE_SUPABASE_ANON_KEY`: Your Supabase anonymous key.
6.  **Deploy your project:** Click the "Deploy" button.
