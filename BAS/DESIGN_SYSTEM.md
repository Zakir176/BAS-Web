# 🎨 `catv1` Design System: Slate Architecture

The **`catv1`** design system is a high-density, semantic token-based styling framework optimized for professional operating environments (Portals, Dashboards, and Scanners).

---

## 🏛️ Theme Core Logic
The system uses **HSL (Hue, Saturation, Lightness)** tokens to ensure perfect harmony across components. Accessibility and contrast are prioritized, with deep support for **Slate-Dark** (Primary Operating Mode) and **Slate-Light** (Administrative Mode).

### 🌑 Slate-Dark (Primary)
Designed for low-fatigue, high-focus operation.
- **Background**: Deep Slate (`#0f172a`)
- **Cards**: Mid-Slate (`#1e293b`) with Glassmorphism blurs.
- **Accents**: Cyber Blue (`#3b82f6`) with neon glow effects.

### 🌕 Slate-Light (Admin)
Optimized for high-clarity document review.
- **Background**: Neutral Gray-White (`#f8fafc`)
- **Cards**: Pure White with subtle soft-shadows.
- **Accents**: Professional Navy (`#1e3a8a`).

---

## 🛠️ Semantic CSS Variables

All components must use semantic variables rather than hardcoded hex values to ensure full theme compatibility.

| Variable | Usage | Current Dark (HSL) |
| :--- | :--- | :--- |
| `--bg-main` | Page background | `222, 47%, 11%` |
| `--bg-card` | Surface level (Cards, Modals) | `222, 47%, 16%` |
| `--primary` | Actions, Badges, Highlights | `221, 83%, 53%` |
| `--primary-glow` | Translucent glow/neon effects | `221, 83%, 53%, 0.15` |
| `--success` | Confirmed actions, Attendance Present | `142, 71%, 45%` |
| `--error` | Destructive actions, Attendance Absent | `0, 84%, 60%` |
| `--text-main` | Primary headings, Readability base | `210, 40%, 98%` |
| `--text-muted` | Captions, Labels, Supporting info | `215, 20%, 65%` |

---

## ✨ Component Standards

### 1. Glassmorphism & Depth
- **Blur**: Maintain `backdrop-filter: blur(12px)`.
- **Borders**: Use `1px solid rgba(255,255,255,0.05)`.
- **Shadows**: Large, soft-diffuse shadows (`0 20px 40px rgba(0,0,0,0.3)`).

### 2. High-Density Layouts
- **Grid Column Gaps**: Standardized at `1.5rem` (24px).
- **Metric Tiles**: Always include a trend indicator or icon badge.
- **Typography**: Utilize **Inter** for UI and **JetBrains Mono** for technical data (IDs, Coordinates).

### 3. Animations ("Pro" Feel)
- **Entrance**: Use `cubic-bezier(0.18, 0.89, 0.32, 1.28)` for bouncy, high-end feel.
- **Pulse**: Used for active states (Scanning, Recording).
- **Shimmer**: Used for "Pro" assets (ID Cards, Achievement Badges).

---

*This design system is a living document. All new features must comply with the `catv1` tokenization.*
