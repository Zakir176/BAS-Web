# 🎓 BAS Pro: Digital Campus Infrastructure

> **Elevating Academic Operations to SaaS Standards.**  
> A high-density, professional attendance management ecosystem built with **Vue 3**, **Supabase**, and the custom **`catv1`** design system.

---

## 💎 The "Pro" Experience

### 🚀 Immersive Scanner HUD
Engineered for both speed and operational depth.
- **Web Operator Mode**: A futuristic, holographic HUD for classroom desktop environments. Features integrated GPS lock, radar-active scanning focus, and real-time log streams.
- **Native Alignment**: 1:1 design parity with the `catv1` native mobile application.
- **Intelligent Feedback**: Success toasts with granular "Undo" support and dual-card parity (Present vs. Absent) for real-time KPIs.

### 💳 Digital ID Card "Pro"
Transforming static student identification into an interactive digital asset.
- **Glassmorphism**: Premium frosted-surface aesthetics with backdrop blur and depth.
- **Holographic Security**: Dynamic shimmer-sweep and edge-glow animations.
- **Meta-Data Badges**: High-density display of student level, status, and validity periods.

### 👨‍🏫 Lecturer Command Center
- **Metric Tiles**: Visual KPIs for attendance trends and course health.
- **Live Roster**: A real-time, searchable operating table for classroom management.
- **Global Activity**: Integrated institution-wide event tracking.

---

## 🎨 Design Language: `catv1`
The portal utilizes the **`catv1`** theme architecture, providing a seamless transition between high-density operational views and modern minimalist aesthetics.

- **Slate-Dark**: A deep, neutral slate base designed for focus and low-light environments.
- **Slate-Light**: A clean, professional high-contrast version for administrative clarity.
- **Semantic Variables**: Fully driven by a robust CSS variable system for instant skinning and brand alignment.

---

## 🛠️ Technology Stack

| Layer | Technology |
| :--- | :--- |
| **Frontend** | [Vue 3](https://vuejs.org/) (Composition API) |
| **Styling** | [Tailwind CSS](https://tailwindcss.com/) + Custom HSL Tokens |
| **Database** | [Supabase](https://supabase.com/) (PostgreSQL + Realtime) |
| **Scanning** | [@ericblade/quagga2](https://github.com/ericblade/quagga2) |
| **Deployment** | Vite + Vercel Optimized |

---

## 🚀 Getting Started

### Prerequisites
- Node.js v18.0 or later
- [Supabase Project](https://supabase.com/) with configured `attendance_logs`, `enrollments`, and `students` tables.

### Quick Start
1.  **Clone & Install**
    ```bash
    git clone https://github.com/yourusername/bas-web.git
    cd BAS
    npm install
    ```

2.  **Environment Configuration**
    Create a `.env` file from the example:
    ```bash
    cp .env.example .env
    # Add your Supabase keys to .env
    ```

3.  **Launch Dashboard**
    ```bash
    npm run dev
    ```

---

## 📂 Architecture Overview

```text
src/
├── components/
│   ├── lecturer/     # Dashboard widgets, Immersive Scanner, Roster
│   ├── student/      # Pro ID Card, Attendance History
│   └── ui/           # Base components (Modals, Icons, Buttons)
├── composables/      # Unified Auth & Database Logic (useAuth, useToast)
└── views/            # High-density route containers
```

---

*Developed and Optimized by Zakir (Professional Edition)*
