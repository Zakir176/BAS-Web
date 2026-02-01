import { mount } from '@vue/test-utils'
import { createRouter, createWebHistory } from 'vue-router'

// Test utilities for the CAT application

/**
 * Creates a test router with common routes
 */
export function createTestRouter() {
  return createRouter({
    history: createWebHistory(),
    routes: [
      { path: '/', name: 'home', component: { template: '<div>Home</div>' } },
      { path: '/student-login', name: 'student-login', component: { template: '<div>Student Login</div>' } },
      { path: '/student-homepage', name: 'student-homepage', component: { template: '<div>Student Dashboard</div>' } },
      { path: '/lecturer-login', name: 'lecturer-login', component: { template: '<div>Lecturer Login</div>' } },
      { path: '/lecturer-dashboard', name: 'lecturer-dashboard', component: { template: '<div>Lecturer Dashboard</div>' } },
      { path: '/report-page', name: 'report-page', component: { template: '<div>Reports</div>' } },
    ]
  })
}

/**
 * Mounts a component with common test setup
 */
export function mountWithTestSetup(component, options = {}) {
  const router = createTestRouter()
  
  const defaultOptions = {
    global: {
      plugins: [router],
      stubs: {
        'router-link': { template: '<a><slot></slot></a>' },
        'router-view': { template: '<div></div>' },
        'font-awesome-icon': { template: '<i></i>' }
      }
    }
  }

  return mount(component, { ...defaultOptions, ...options })
}

/**
 * Creates mock user data for testing
 */
export function createMockUser(overrides = {}) {
  return {
    id: 'test-user-id',
    email: 'test@example.com',
    role: 'student',
    user_metadata: {
      full_name: 'Test User',
      first_name: 'Test',
      last_name: 'User'
    },
    created_at: '2024-01-01T00:00:00Z',
    ...overrides
  }
}

/**
 * Creates mock student data for testing
 */
export function createMockStudent(overrides = {}) {
  return {
    student_id: 'STU001',
    email: 'student@example.com',
    full_name: 'Student Name',
    first_name: 'Student',
    last_name: 'Name',
    class_section: 'CS101',
    created_at: '2024-01-01T00:00:00Z',
    ...overrides
  }
}

/**
 * Creates mock lecturer data for testing
 */
export function createMockLecturer(overrides = {}) {
  return {
    lecturer_id: 'LEC001',
    email: 'lecturer@example.com',
    full_name: 'Lecturer Name',
    first_name: 'Lecturer',
    last_name: 'Name',
    department: 'Computer Science',
    created_at: '2024-01-01T00:00:00Z',
    ...overrides
  }
}

/**
 * Creates mock course data for testing
 */
export function createMockCourse(overrides = {}) {
  return {
    course_id: 'CS101',
    course_name: 'Introduction to Computer Science',
    description: 'Basic computer science concepts',
    lecturer_id: 'LEC001',
    created_at: '2024-01-01T00:00:00Z',
    ...overrides
  }
}

/**
 * Creates mock attendance data for testing
 */
export function createMockAttendance(overrides = {}) {
  return {
    attendance_id: 'ATT001',
    student_id: 'STU001',
    session_id: 'SES001',
    status: 'Present',
    method: 'Barcode',
    timestamp: '2024-01-01T10:00:00Z',
    ...overrides
  }
}

/**
 * Creates mock session data for testing
 */
export function createMockSession(overrides = {}) {
  return {
    session_id: 'SES001',
    course_id: 'CS101',
    lecturer_id: 'LEC001',
    session_name: 'Morning Lecture',
    created_at: '2024-01-01T09:00:00Z',
    ...overrides
  }
}

/**
 * Waits for Vue's next tick and any pending promises
 */
export async function flushPromises() {
  await new Promise(resolve => setTimeout(resolve, 0))
}

/**
 * Simulates user typing in an input field
 */
export async function type(wrapper, selector, text) {
  const input = wrapper.find(selector)
  await input.setValue(text)
  await input.trigger('input')
  await flushPromises()
}

/**
 * Simulates form submission
 */
export async function submitForm(wrapper, formSelector = 'form') {
  const form = wrapper.find(formSelector)
  await form.trigger('submit')
  await flushPromises()
}

/**
 * Asserts that a toast was called with specific parameters
 */
export function expectToast(mockToast, type, message) {
  expect(mockToast[type]).toHaveBeenCalledWith(message)
}

/**
 * Asserts that a router navigation occurred
 */
export function expectNavigation(mockRouter, path) {
  expect(mockRouter.push).toHaveBeenCalledWith(path)
}

/**
 * Creates a mock Supabase response
 */
export function createMockSupabaseResponse(data, error = null) {
  return {
    data,
    error,
    count: data?.length || 0
  }
}

/**
 * Mocks the Supabase client
 */
export function mockSupabase(overrides = {}) {
  const defaultMock = {
    auth: {
      signInWithPassword: vi.fn(),
      signUp: vi.fn(),
      signOut: vi.fn(),
      getUser: vi.fn(),
      onAuthStateChange: vi.fn(() => ({ data: { subscription: { unsubscribe: vi.fn() } } })),
    },
    from: vi.fn(() => ({
      select: vi.fn(() => ({
        eq: vi.fn(() => ({
          single: vi.fn(),
          data: [],
          error: null
        })),
        order: vi.fn(() => ({
          data: [],
          error: null
        }))
      })),
      insert: vi.fn(() => createMockSupabaseResponse({})),
      update: vi.fn(() => createMockSupabaseResponse({})),
      delete: vi.fn(() => createMockSupabaseResponse({}))
    }))
  }

  return { ...defaultMock, ...overrides }
}

/**
 * Test data factory for creating multiple test records
 */
export class TestDataFactory {
  static createUsers(count = 5, role = 'student') {
    return Array.from({ length: count }, (_, i) => 
      createMockUser({
        id: `user-${i + 1}`,
        email: `${role}${i + 1}@example.com`,
        role,
        user_metadata: {
          full_name: `${role.charAt(0).toUpperCase() + role.slice(1)} ${i + 1}`,
          first_name: `${role.charAt(0).toUpperCase() + role.slice(1)}`,
          last_name: `User ${i + 1}`
        }
      })
    )
  }

  static createStudents(count = 5) {
    return Array.from({ length: count }, (_, i) => 
      createMockStudent({
        student_id: `STU${String(i + 1).padStart(3, '0')}`,
        email: `student${i + 1}@example.com`,
        full_name: `Student ${i + 1}`,
        first_name: 'Student',
        last_name: `User ${i + 1}`
      })
    )
  }

  static createCourses(count = 3) {
    return Array.from({ length: count }, (_, i) => 
      createMockCourse({
        course_id: `CS${String(i + 1).padStart(3, '0')}`,
        course_name: `Course ${i + 1}`,
        description: `Description for course ${i + 1}`
      })
    )
  }

  static createAttendanceRecords(count = 10, studentIds = ['STU001'], sessionIds = ['SES001']) {
    return Array.from({ length: count }, (_, i) => 
      createMockAttendance({
        attendance_id: `ATT${String(i + 1).padStart(3, '0')}`,
        student_id: studentIds[i % studentIds.length],
        session_id: sessionIds[i % sessionIds.length],
        status: Math.random() > 0.2 ? 'Present' : 'Absent',
        timestamp: new Date(Date.now() - i * 24 * 60 * 60 * 1000).toISOString()
      })
    )
  }
}
