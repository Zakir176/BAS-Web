# API Reference

This document covers the data layer of the Barcode Attendance System (BAS). The backend is powered by [Supabase](https://supabase.io/). All database access goes through the Supabase JavaScript client, which is initialized in [`src/core/api/supabase.js`](../BAS/src/core/api/supabase.js).

## Supabase Client

The client is created once and exported for use across the app:

```js
// src/core/api/supabase.js
import { createClient } from '@supabase/supabase-js'

export const supabase = createClient(
  import.meta.env.VITE_SUPABASE_URL,
  import.meta.env.VITE_SUPABASE_ANON_KEY
)
```

Import it in any service or store:

```js
import { supabase } from '@/core/api/supabase'
```

---

## Authentication

Authentication is handled by Supabase Auth via `supabase.auth`. The role (`student` or `lecturer`) is stored in the user's `user_metadata` and normalized by the `useUserStore`.

| Method | Description |
|---|---|
| `supabase.auth.signUp({ email, password, options: { data: { role } } })` | Registers a new user with a role in their metadata. |
| `supabase.auth.signInWithPassword({ email, password })` | Logs in a user and returns a session. |
| `supabase.auth.signOut()` | Logs out the current user and clears the session. |
| `supabase.auth.resetPasswordForEmail(email)` | Sends a password-reset email. |
| `supabase.auth.getSession()` | Returns the current active session. |
| `supabase.auth.onAuthStateChange(callback)` | Subscribes to auth state changes (login, logout, token refresh). |

**Role normalization** — the `user_metadata.role` value `'teacher'` or `'admin'` is normalized to `'lecturer'` by `useUserStore.setUser()`. Only `'student'` and `'lecturer'` are used inside the app.

---

## Database Tables

### `students`

Stores student profile information. Looked up using `email` as the key when authenticating.

| Column | Type | Description |
|---|---|---|
| `id` | UUID | Primary key (matches Supabase Auth UID for students). |
| `student_number` | VARCHAR | The barcode value printed on the student ID card. |
| `full_name` | VARCHAR | The student's display name. |
| `email` | VARCHAR | Used to link the row to the Supabase Auth user. |
| `class_section` | VARCHAR | The student's class section. |
| `phone` | VARCHAR | Optional contact number. |

**Common queries:**

```js
// Fetch student profile by email
const { data } = await supabase
  .from('students')
  .select('*')
  .eq('email', userEmail)
  .single()

// Identify a student by their barcode
const { data: student } = await supabase
  .from('students')
  .select('id, full_name')
  .eq('student_number', barcodeValue)
  .maybeSingle()
```

---

### `lecturers`

Stores lecturer profile information. Looked up using `id` (UUID) as the key.

| Column | Type | Description |
|---|---|---|
| `id` | UUID | Primary key (matches Supabase Auth UID). |
| `full_name` | VARCHAR | The lecturer's display name. |
| `email` | VARCHAR | The lecturer's email address. |
| `role` | VARCHAR | Role designation (e.g., `'teacher'`, `'admin'`). |

**Common queries:**

```js
// Fetch lecturer profile by their auth UUID
const { data: lecturer } = await supabase
  .from('lecturers')
  .select('full_name')
  .eq('id', lecturerId)
  .maybeSingle()
```

---

### `courses`

Stores course definitions. A course can have multiple sections taught by different lecturers.

| Column | Type | Description |
|---|---|---|
| `id` | SERIAL | Primary key. |
| `name` | VARCHAR | The name of the course. |

---

### `sections`

A section is a specific delivery of a course (e.g., a particular class group). Sections are owned by a lecturer.

| Column | Type | Description |
|---|---|---|
| `id` | SERIAL | Primary key. |
| `name` | VARCHAR | Section identifier (e.g., `'CS101-A'`). |
| `course_id` | INTEGER | Foreign key → `courses.id`. |
| `lecturer_id` | UUID | Foreign key → `lecturers.id`. |

**Common queries:**

```js
// Fetch all sections for a lecturer, with enrollment counts
const { data: sections } = await supabase
  .from('sections')
  .select('id, name, course_id, courses(name), enrollments(count)')
  .eq('lecturer_id', lecturerId)
```

---

### `enrollments`

Maps students to the sections they are enrolled in.

| Column | Type | Description |
|---|---|---|
| `id` | SERIAL | Primary key. |
| `student_id` | UUID | Foreign key → `students.id`. |
| `section_id` | INTEGER | Foreign key → `sections.id`. |

**Common queries:**

```js
// Fetch a full roster for a section
const { data: enrolledStudents } = await supabase
  .from('enrollments')
  .select('student_id, students(id, full_name, student_number)')
  .eq('section_id', sectionId)
```

---

### `attendance_logs`

The core table. One row is inserted per student per session when attendance is marked. A unique constraint on `(student_id, section_id, session_date::date)` prevents duplicate entries.

| Column | Type | Description |
|---|---|---|
| `id` | SERIAL | Primary key. |
| `student_id` | UUID | Foreign key → `students.id`. |
| `section_id` | INTEGER | Foreign key → `sections.id`. |
| `status` | VARCHAR | `'Present'` or `'Absent'`. |
| `session_date` | TIMESTAMPTZ | Timestamp of when attendance was recorded. |

**Common queries:**

```js
// Mark a student present (duplicate ignored via error code 23505)
const { data, error } = await supabase
  .from('attendance_logs')
  .insert({ student_id, section_id, status: 'Present', session_date: new Date().toISOString() })
  .select()
  .single()

// Get today's attendance for a section
const { data: attendanceData } = await supabase
  .from('attendance_logs')
  .select('student_id')
  .eq('section_id', sectionId)
  .gte('session_date', new Date().toISOString().split('T')[0])

// Get recent activity feed for a lecturer's sections
const { data } = await supabase
  .from('attendance_logs')
  .select('*, students(full_name), sections(name, courses(name))')
  .in('section_id', sectionIds)
  .order('session_date', { ascending: false })
  .limit(8)
```

---

## Service Layer

Business logic is encapsulated in service modules under `src/services/`. These are plain JS objects (not composables) and should be called from Pinia stores or Vue components.

| File | Responsibility |
|---|---|
| [`attendanceService.js`](../BAS/src/services/attendanceService.js) | Mark attendance by UUID or barcode, fetch recent activity feed. |
| [`courseService.js`](../BAS/src/services/courseService.js) | Load lecturer dashboard data, fetch a section's live roster. |

---

## 🛠 Need Help?

If you encounter errors or connectivity issues, please refer to the [Troubleshooting Guide](./TROUBLESHOOTING.md).
