# Component Documentation

This document provides a reference for the Vue components in the Barcode Attendance System (BAS). Components are organized by layer: core UI primitives, feature-specific components, and layout/page sections.

---

## Core UI Primitives

These stateless, reusable components live in `src/core/ui/` and form the design system foundation. Import them globally (registered in `main.js`) or locally as needed.

### `BaseButton.vue`

A versatile button with semantic variants and three sizes.

**Props:**

| Prop | Type | Default | Options |
|---|---|---|---|
| `variant` | String | `'primary'` | `'primary'`, `'secondary'`, `'white'`, `'success'`, `'danger'`, `'warning'` |
| `size` | String | `'md'` | `'sm'`, `'md'`, `'lg'` |
| `disabled` | Boolean | `false` | — |
| `fullWidth` | Boolean | `false` | — |

**Events:** `click`

**Slots:** `default` (button label/content)

---

### `BaseCard.vue`

A flexible content container with configurable padding, shadow, and border-radius.

**Props:**

| Prop | Type | Default | Options |
|---|---|---|---|
| `padding` | String | `'normal'` | `'none'`, `'sm'`, `'normal'`, `'lg'` |
| `shadow` | String | `'sm'` | `'none'`, `'sm'`, `'md'`, `'lg'` |
| `rounded` | String | `'md'` | `'none'`, `'sm'`, `'md'`, `'lg'`, `'xl'` |

**Slots:** `header`, `default` (body), `footer`

---

### `BaseInput.vue`

A labelled input field with built-in error state display.

**Props:**

| Prop | Type | Default | Description |
|---|---|---|---|
| `modelValue` | String \| Number | `''` | Bound value (use `v-model`) |
| `label` | String | `''` | Label text shown above the field |
| `type` | String | `'text'` | Any valid HTML input type |
| `placeholder` | String | `''` | Placeholder text |
| `required` | Boolean | `false` | Appends `*` to label and sets `required` attribute |
| `disabled` | Boolean | `false` | Greys out and disables the field |
| `error` | String | `''` | Error message displayed below the field in red |

**Events:** `update:modelValue`, `blur`, `focus`

---

### `BaseModal.vue`

A dialog overlay with optional header, footer, and a transparent mode for camera/media content.

**Props:**

| Prop | Type | Default | Description |
|---|---|---|---|
| `isOpen` | Boolean | required | Controls visibility |
| `title` | String | `'Modal Title'` | Heading shown in the header bar |
| `hideHeader` | Boolean | `false` | Hides the header bar; shows a floating close button instead |
| `hideFooter` | Boolean | `false` | Hides the footer bar |
| `transparent` | Boolean | `false` | Removes background, shadow, and padding (used for scanner overlay) |

**Events:** `close`

**Slots:** `header`, `default` (body), `footer`

---

### `BaseToast.vue`

A self-dismissing notification toast with a progress bar and hover-pause support. Managed via `ToastContainer.vue`.

**Props:**

| Prop | Type | Default | Description |
|---|---|---|---|
| `show` | Boolean | `false` | Controls visibility |
| `type` | String | `'info'` | `'success'`, `'error'`, `'warning'`, `'info'` |
| `title` | String | `''` | Bold heading line |
| `message` | String | `''` | Secondary body text |
| `duration` | Number | `5000` | Auto-dismiss delay in milliseconds. `0` = never dismiss |
| `closable` | Boolean | `true` | Shows a close button |
| `showProgress` | Boolean | `true` | Displays a shrinking progress bar |
| `pauseOnHover` | Boolean | `true` | Pauses the auto-dismiss timer on hover |
| `onClick` | Function | `null` | Optional callback when the toast body is clicked |

**Events:** `close`

---

### `BaseSkeleton.vue`

An animated shimmer placeholder for loading states.

**Props:**

| Prop | Type | Default | Description |
|---|---|---|---|
| `width` | String | `'100%'` | CSS width value |
| `height` | String | `'1rem'` | CSS height value |
| `shape` | String | `'rect'` | `'rect'` or `'circle'` |
| `radius` | String | `null` | Overrides the default border-radius |

---

### `EmptyState.vue`

A placeholder shown when a list or data set is empty.

**Usage:** Provides a consistent empty-state UI across the app (icon + message). Check the component source for slot details.

---

## Feature Components

### Auth (`src/features/auth/`)

| Component | Description |
|---|---|
| `ForgotPassword.vue` | Form for requesting a password-reset email via Supabase Auth. |
| `ResetPassword.vue` | Form for setting a new password after clicking the reset link. |

---

### Lecturer (`src/features/lecturer/`)

| Component | Description |
|---|---|
| `LecturerDashboard.vue` | Main lecturer view. Fetches sections via `courseService` and renders `CourseGrid`, `DashboardStats`, and `RecentActivity`. |
| `LecturerLogin.vue` | Login form for lecturers, validated with VeeValidate + Yup. |
| `LecturerSignup.vue` | Registration form for new lecturer accounts. |
| `CourseGrid.vue` | Displays the lecturer's sections as interactive cards with enrollment counts and attendance rates. |
| `DashboardStats.vue` | Summary stat cards (total students, sessions, attendance rate) at the top of the lecturer dashboard. |
| `CreateCourseModal.vue` | Modal form for creating a new course. Emits `courseCreated` with the new course object on success. |
| `CreateSessionModal.vue` | Modal form for creating a new attendance session. Emits `sessionCreated` on success. |
| `LiveRosterRealtime.vue` | Shows the live student roster for an active session. Subscribes to Supabase Realtime for instant presence updates. |
| `RecentActivity.vue` | Timeline of the latest attendance events for the lecturer's sections. |
| `ReportPage.vue` | Full attendance report view with filtering and Excel export via ExcelJS. |

---

### Student (`src/features/student/`)

| Component | Description |
|---|---|
| `StudentHomepage.vue` | Main student dashboard. Shows attendance stats, heatmap, and recent activity. |
| `StudentLogin.vue` | Login form for students, validated with VeeValidate + Yup. |
| `StudentSignup.vue` | Registration form for new student accounts. |
| `StudentBarcode.vue` | Displays the student's personal barcode (generated with JsBarcode) for scanning. |
| `StudentProfileHeader.vue` | Header card showing the student's name, ID, and class section. |
| `AttendanceCalendar.vue` | Heatmap calendar visualizing attendance patterns across the semester. |
| `LiveAttendanceStatus.vue` | Real-time display of the student's current attendance status for an active session. |
| `RecentActivity.vue` | Timeline of the student's own recent attendance activity. |

**`AttendanceCalendar.vue` props:**

| Prop | Type | Description |
|---|---|---|
| `attendanceRecords` | Array | Array of objects with `date` (ISO string) and `status` (`'Present'` \| `'Absent'`) properties. |

---

### Scanner (`src/features/scanner/`)

| Component | Description |
|---|---|
| `BarcodeScanner.vue` | Low-level camera component. Initializes QuaggaJS, reads the live stream, and emits a `detected` event with the decoded barcode string. Has a 2-second cooldown between emissions to prevent duplicate scans. |
| `ScannerInterface.vue` | Full scanner UI. Wraps `BarcodeScanner.vue`, handles section selection, calls `attendanceService.markByBarcode()`, and displays result feedback. |

**`BarcodeScanner.vue` events:**

| Event | Payload | Description |
|---|---|---|
| `detected` | `String` | The decoded barcode value, emitted at most once every 2 seconds. |

**Supported barcode formats:** Code 128, EAN-13, EAN-8, Code 39, Codabar, UPC-A, UPC-E, Interleaved 2of5, Code 93.

---

### Home (`src/features/home/`)

These are landing-page section components used in `AppHome.vue`. They are presentational and have no props.

| Component | Description |
|---|---|
| `HeroSection.vue` | Full-width hero with headline, CTA buttons, and UI preview. |
| `FeatureGrid.vue` | Grid of feature highlight cards. |
| `WorkflowSteps.vue` | Step-by-step "How It Works" section. |
| `CTASection.vue` | Bottom call-to-action banner. |
| `HomeFooter.vue` | Page footer with links and branding. |
