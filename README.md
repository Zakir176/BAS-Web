# Barcode Attendance System (BAS)

A modern, reliable, and easy-to-use barcode attendance system designed for universities. This project is the frontend implementation, built with Vue.js and designed to provide a clean, professional, and user-friendly interface for both students and lecturers.

## 🎨 Design & Interface

The UI is designed to be modern, clean, and academic-friendly. It features a dual-theme system, allowing users to switch between a light and a dark mode for optimal viewing comfort. The interface is fully responsive and built with a focus on clear visual hierarchy and high readability.

### Light Mode
| Home | Student Login | Lecturer Login |
| --- | --- | --- |
| <img src="docs/ui/light_mode/Home.png" alt="Home Page" width="300"/> | <img src="docs/ui/light_mode/student_login_page.png" alt="Student Login" width="300"/> | <img src="docs/ui/light_mode/lecturer_login_page.png" alt="Lecturer Login" width="300"/> |

| Student Homepage | Report Page |
| --- | --- |
| <img src="docs/ui/light_mode/student_homepage.png" alt="Student Homepage" width="300"/> | <img src="docs/ui/light_mode/report_page.png" alt="Report Page" width="300"/> |

### Dark Mode
| Home | Student Login | Lecturer Login |
| --- | --- | --- |
| <img src="docs/ui/dark_mode/Home.png" alt="Home Page" width="300"/> | <img src="docs/ui/dark_mode/student_login_page.png" alt="Student Login" width="300"/> | <img src="docs/ui/dark_mode/lecturer_login_page.png" alt="Lecturer Login" width="300"/> |

| Student Homepage | Report Page |
| --- | --- |
| <img src="docs/ui/dark_mode/student_homepage.png" alt="Student Homepage" width="300"/> | <img src="docs/ui/dark_mode/report_page.png" alt="Report Page" width="300"/> |


## ✨ Features

*   **Dual-Theme UI:** Seamlessly switch between Light and Dark modes.
*   **Distinct User Portals:** Separate login and dashboard experiences for Students and Lecturers.
*   **Student Dashboard:** A comprehensive view for students to track their attendance, view recent activity, and generate reports.
*   **Attendance Reporting:** Detailed attendance reports with filtering capabilities.
*   **Modern Tech Stack:** Built with Vue.js 3, Vite, and modern CSS for a fast and reliable experience.

## 🛠 Tech Stack

*   **Framework:** [Vue.js 3](https://vuejs.org/)
*   **Build Tool:** [Vite](https://vitejs.dev/)
*   **Routing:** [Vue Router](https://router.vuejs.org/)
*   **Styling:** CSS with Variables for Theming
*   **Linting:** [ESLint](https://eslint.org/)

## 🚀 Getting Started

To get the frontend running locally, follow these steps:

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/your-username/BAS-Web.git
    cd BAS-Web
    ```

2.  **Navigate to the project directory:**
    The Vue.js project is located in the `BAS` sub-directory.
    ```bash
    cd BAS
    ```

3.  **Install dependencies:**
    ```bash
    npm install
    ```

4.  **Run the development server:**
    ```bash
    npm run dev
    ```

The application will be available at the local address provided by Vite (e.g., `http://localhost:5173`).

## 📄 Project Structure

*   `BAS/`: Contains the Vue.js application source code.
    *   `src/`: The main source folder.
        *   `assets/`: Global styles and static assets.
        *   `components/`: Reusable Vue components (UI elements, layout).
        *   `composables/`: Vue composables (e.g., `useTheme.js`).
        *   `router/`: Vue Router configuration.
        *   `views/`: Page-level components.
*   `docs/`: Contains UI design images and project documentation.

This project was developed by [Gemini](https://gemini.google.com/).