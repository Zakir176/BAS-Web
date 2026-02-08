# Component Documentation

This document provides a detailed overview of the reusable Vue components used in the Barcode Attendance System (BAS).

## UI Components

### `Button.vue`

*   **Description:** A versatile button component with different variants and sizes.
*   **Props:**
    *   `variant` (String): The button's style variant. Can be `primary`, `secondary`, `danger`, or `ghost`. Defaults to `primary`.
    *   `size` (String): The button's size. Can be `sm`, `md`, or `lg`. Defaults to `md`.
    *   `full-width` (Boolean): If `true`, the button will take up the full width of its container. Defaults to `false`.
*   **Events:**
    *   `click`: Emitted when the button is clicked.

### `Card.vue`

*   **Description:** A flexible card component for displaying content in a structured way.
*   **Props:**
    *   `padding` (String): The padding size of the card. Can be `none`, `sm`, `normal`, or `lg`. Defaults to `normal`.
    *   `shadow` (String): The shadow depth of the card. Can be `none`, `sm`, `md`, or `lg`. Defaults to `sm`.
    *   `rounded` (String): The border radius of the card. Can be `none`, `sm`, `md`, `lg`, or `xl`. Defaults to `md`.
*   **Slots:**
    *   `header`: For content to be placed in the card's header.
    *   `default`: For the main content of the card.
    *   `footer`: For content to be placed in the card's footer.

### `Input.vue`

*   **Description:** A standard input field component with a label, placeholder, and error message.
*   **Props:**
    *   `modelValue` (String, Number): The value of the input.
    *   `label` (String): The label for the input.
    *   `type` (String): The type of the input (e.g., 'text', 'password', 'email'). Defaults to `text`.
    *   `placeholder` (String): The placeholder text for the input.
    *   `required` (Boolean): If `true`, the input will be required. Defaults to `false`.
    *   `disabled` (Boolean): If `true`, the input will be disabled. Defaults to `false`.
    *   `error` (String): An error message to display below the input.
*   **Events:**
    *   `update:modelValue`: Emitted when the value of the input changes.
    *   `blur`: Emitted when the input loses focus.
    *   `focus`: Emitted when the input gains focus.

### `Modal.vue`

*   **Description:** A modal component for displaying content in a dialog box.
*   **Props:**
    *   `isOpen` (Boolean): If `true`, the modal will be open.
*   **Events:**
    *   `close`: Emitted when the modal is closed.
*   **Slots:**
    *   `header`: For the modal's header content.
    *   `default`: For the modal's main content.
    *   `footer`: For the modal's footer content.

## Functional Components

### `AttendanceCalendar.vue`

*   **Description:** A component that displays a calendar with attendance data.
*   **Props:**
    *   `attendanceRecords` (Array): An array of attendance records to display on the calendar. Each record should have a `date` and `status` property.
*   **Events:**
    *   None
*   **Slots:**
    *   None

### `BarcodeScanner.vue`

*   **Description:** A component that uses the device's camera to scan barcodes.
*   **Props:**
    *   None
*   **Events:**
    *   `detected`: Emitted when a barcode is detected. The event payload is the decoded barcode data.
*   **Slots:**
    *   None

### `CreateCourseModal.vue`

*   **Description:** A modal component for creating a new course.
*   **Props:**
    *   `isOpen` (Boolean): If `true`, the modal will be open.
*   **Events:**
    *   `close`: Emitted when the modal is closed.
    *   `courseCreated`: Emitted when a new course is created. The event payload is the newly created course object.
*   **Slots:**
    *   None

### `CreateSessionModal.vue`

*   **Description:** A modal component for creating a new session.
*   **Props:**
    *   `isOpen` (Boolean): If `true`, the modal will be open.
*   **Events:**
    *   `close`: Emitted when the modal is closed.
    *   `sessionCreated`: Emitted when a new session is created. The event payload is the newly created session object.
*   **Slots:**
    *   None

## Layout Components

### `Cta.vue`

*   **Description:** A call-to-action component that encourages users to sign up.
*   **Props:**
    *   None
*   **Events:**
    *   None
*   **Slots:**
    *   None

### `HowItWorks.vue`

*   **Description:** A component that explains how the barcode attendance system works.
*   **Props:**
    *   None
*   **Events:**
    *   None
*   **Slots:**
    *   None

### `Navbar.vue`

*   **Description:** The main navigation bar for the application.
*   **Props:**
    *   None
*   **Events:**
    *   None
*   **Slots:**
    *   None

### `Testimonials.vue`

*   **Description:** A component that displays testimonials from users.
*   **Props:**
    *   None
*   **Events:**
    *   None
*   **Slots:**
    *   None
---
