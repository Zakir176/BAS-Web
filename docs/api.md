# API Documentation

This document provides a detailed overview of the API endpoints used in the Barcode Attendance System (BAS). The backend is powered by [Supabase](https://supabase.io/), and this documentation covers the three main services used: Authentication, Database, and Storage.

## Authentication

Authentication is handled by Supabase Auth. The following functions are used to manage user authentication:

| Function | Description |
| --- | --- |
| `signUp(credentials)` | Registers a new user (student or lecturer) in the `auth.users` table. |
| `signInWithPassword(credentials)` | Logs in a user with their email and password. |
| `signOut()` | Logs out the current user. |
| `resetPasswordForEmail(email)` | Sends a password reset link to the user's email address. |
| `onAuthStateChange(callback)` | Listens for changes in the user's authentication state. |
| `getSession()` | Retrieves the current user's session information. |

## Database

The application uses the Supabase Database to store and manage data. The following tables are used:

### `students`

*   **Description:** Stores information about students.
*   **Columns:**
    *   `student_id` (VARCHAR): The student's unique ID.
    *   `full_name` (VARCHAR): The student's full name.
    *   `class_section` (VARCHAR): The student's class section.
    *   `qr_code_value` (VARCHAR): The value of the student's QR code.
    *   `email` (VARCHAR): The student's email address.
    *   `phone` (VARCHAR): The student's phone number.
    *   `password` (VARCHAR): The student's hashed password.
*   **API Calls:**
    *   `GET /students`: Retrieves a list of students.
    *   `POST /students`: Creates a new student.

### `teachers`

*   **Description:** Stores information about teachers/lecturers.
*   **Columns:**
    *   `teacher_id` (UUID): The teacher's unique ID.
    *   `full_name` (VARCHAR): The teacher's full name.
    *   `email` (VARCHAR): The teacher's email address.
    *   `password` (VARCHAR): The teacher's hashed password.
    *   `role` (VARCHAR): The teacher's role (e.g., 'teacher', 'admin').
*   **API Calls:**
    *   `GET /teachers`: Retrieves a list of teachers.
    *   `POST /teachers`: Creates a new teacher.

### `courses`

*   **Description:** Stores information about courses.
*   **Columns:**
    *   `course_id` (SERIAL): The course's unique ID.
    *   `course_name` (VARCHAR): The name of the course.
    *   `teacher_id` (UUID): The ID of the teacher who teaches the course.
    *   `max_absences_allowed` (INTEGER): The maximum number of absences allowed for the course.
*   **API Calls:**
    *   `GET /courses`: Retrieves a list of courses.
    *   `POST /courses`: Creates a new course.

### `enrollments`

*   **Description:** Stores information about student enrollments in courses.
*   **Columns:**
    *   `enrollment_id` (SERIAL): The enrollment's unique ID.
    *   `student_id` (VARCHAR): The ID of the student.
    *   `course_id` (INTEGER): The ID of the course.
*   **API Calls:**
    *   `GET /enrollments`: Retrieves a list of enrollments.
    *   `POST /enrollments`: Creates a new enrollment.

### `sessions`

*   **Description:** Stores information about attendance sessions.
*   **Columns:**
    *   `session_id` (SERIAL): The session's unique ID.
    *   `course_id` (INTEGER): The ID of the course.
    *   `session_date` (DATE): The date of the session.
    *   `session_time` (TIME): The time of the session.
*   **API Calls:**
    *   `GET /sessions`: Retrieves a list of sessions.
    *   `POST /sessions`: Creates a new session.

### `attendance`

*   **Description:** Stores attendance records for students in sessions.
*   **Columns:**
    *   `attendance_id` (SERIAL): The attendance record's unique ID.
    *   `session_id` (INTEGER): The ID of the session.
    *   `student_id` (VARCHAR): The ID of the student.
    *   `status` (VARCHAR): The student's attendance status (e.g., 'Present', 'Absent').
    *   `method` (VARCHAR): The method used to take attendance (e.g., 'Barcode', 'Manual').
*   **API Calls:**
    *   `GET /attendance`: Retrieves a list of attendance records.
    *   `POST /attendance`: Creates a new attendance record.
    *   `PUT /attendance`: Updates an existing attendance record.

 |
