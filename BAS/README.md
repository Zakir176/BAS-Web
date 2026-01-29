# 🎓 Barcode Attendance System (BAS)

A modern, mobile-first web application for university attendance tracking. BAS allows lecturers to manage courses and sessions, while enabling students to track their attendance history and performance.

![Vue.js](https://img.shields.io/badge/Vue.js-35495E?style=for-the-badge&logo=vuedotjs&logoColor=4FC08D)
![Supabase](https://img.shields.io/badge/Supabase-3ECF8E?style=for-the-badge&logo=supabase&logoColor=white)
![Tailwind CSS](https://img.shields.io/badge/Tailwind_CSS-38B2AC?style=for-the-badge&logo=tailwind-css&logoColor=white)

## ✨ Key Features

### 👨‍🏫 Lecturer Portal
- **Dashboard**: Real-time overview of active courses, student stats, and upcoming sessions.
- **Barcode Scanner**: Integrated camera scanner (using `Quagga2`) for rapid student check-in.
- **Session Management**: Create, manage, and "Complete" attendance sessions.
- **Live Roster**: Watch attendance populate in real-time as students are scanned.
- **Reports**: Export course data to CSV.

### 👨‍🎓 Student Portal
- **Personal Dashboard**: View attendance rates per course.
- **History**: Full log of present/absent dates.
- **Mobile Optimized**: Fully responsive interface with a native-like App Drawer navigation.
- **Contact**: Quick actions to contact parents or faculty.

### 🎨 UI/UX
- **Glassmorphism**: Premium frosted-glass aesthetics on navigation and overlays.
- **Dark Mode**: Fully supported system-wide dark theme.
- **Responsive**: Mobile-first design ensures perfect usability on phones and tablets.

## 🛠️ Technology Stack

- **Frontend**: Vue 3 (Composition API) + Vite
- **Styling**: Tailwind CSS + Custom CSS Variables
- **Backend/Auth**: Supabase (PostgreSQL, Auth, Realtime)
- **State**: Vue Composables (`useAuth.js`)
- **Scanning**: `@ericblade/quagga2`

## 🚀 Getting Started

### Prerequisites
- Node.js (v16+)
- A Supabase project with `students`, `teachers`, `courses`, `sessions`, and `attendance` tables.

### Installation

1.  **Clone the repository**
    ```sh
    git clone https://github.com/yourusername/bas-web.git
    cd bas-web/BAS
    ```

2.  **Install dependencies**
    ```sh
    npm install
    ```

3.  **Environment Setup**
    Create a `.env` file in the root directory:
    ```env
    VITE_SUPABASE_URL=your_supabase_project_url
    VITE_SUPABASE_ANON_KEY=your_supabase_anon_key
    ```

4.  **Run Locally**
    ```sh
    npm run dev
    ```

## 📂 Project Structure

- `src/components/common`: Shared UI (Navbar, Inputs, Modals).
- `src/components/lecturer`: Dashboard widgets, Scanner, Create Modals.
- `src/components/student`: Student-specific views and cards.
- `src/views`: Top-level page views (Home, Dashboard, Login).
- `src/composables`: Reusable logic (`useAuth`).

## 🔍 Recent Updates
- **Robust Scanning**: Refactored session detection to prevent database join errors.
- **Session Completion**: Lecturers can now finalize sessions directly from the scanner.
- **Mobile Experience**: Complete redesign of the mobile navigation drawer.

---
*Developed by Zakir*
