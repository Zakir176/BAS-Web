import { createRouter, createWebHistory } from 'vue-router'
import { useAuth } from '@/composables/useAuth'
import { watch } from 'vue'

import Home from '../views/Home.vue'
import LecturerLogin from '../views/LecturerLogin.vue'
import LecturerSignup from '../views/LecturerSignup.vue'
import StudentLogin from '../views/StudentLogin.vue'
import StudentSignup from '../views/StudentSignup.vue'
import StudentHomepage from '../views/StudentHomepage.vue'

import ReportPage from '../views/ReportPage.vue'
import LecturerDashboard from '../views/LecturerDashboard.vue'
import DebugLogin from '../views/DebugLogin.vue'
import ToastDemo from '../views/ToastDemo.vue'

const routes = [
  {
    path: '/',
    name: 'Home',
    component: Home,
    meta: { requiresAuth: false }
  },
  {
    path: '/lecturer-login',
    name: 'LecturerLogin',
    component: LecturerLogin,
    meta: { requiresAuth: false, requiresGuest: true }
  },
  {
    path: '/lecturer-signup',
    name: 'LecturerSignup',
    component: LecturerSignup,
    meta: { requiresAuth: false, requiresGuest: true }
  },
  {
    path: '/lecturer-dashboard',
    name: 'LecturerDashboard',
    component: LecturerDashboard,
    meta: { requiresAuth: true, role: 'lecturer' }
  },
  {
    path: '/debug-login',
    name: 'DebugLogin',
    component: DebugLogin,
    meta: { requiresAuth: false }
  },
  {
    path: '/toast-demo',
    name: 'ToastDemo',
    component: ToastDemo,
    meta: { requiresAuth: false }
  },
  {
    path: '/student-login',
    name: 'StudentLogin',
    component: StudentLogin,
    meta: { requiresAuth: false, requiresGuest: true }
  },
  {
    path: '/student-signup',
    name: 'StudentSignup',
    component: StudentSignup,
    meta: { requiresAuth: false, requiresGuest: true }
  },
  {
    path: '/student-homepage',
    name: 'StudentHomepage',
    component: StudentHomepage,
    meta: { requiresAuth: true, role: 'student' }
  },

  {
    path: '/report-page',
    name: 'ReportPage',
    component: ReportPage,
    meta: { requiresAuth: true, role: 'lecturer' }
  },
  {
    path: '/lecturer-homepage',
    redirect: '/lecturer-dashboard'
  },
  {
    path: '/forgot-password',
    name: 'ForgotPassword',
    component: () => import('../views/ForgotPassword.vue'),
    meta: { requiresAuth: false, requiresGuest: true }
  },
  {
    path: '/reset-password',
    name: 'ResetPassword',
    component: () => import('../views/ResetPassword.vue'),
    meta: { requiresAuth: false } // Supabase handles auth state during reset link callback
  },
  {
    path: '/:pathMatch(.*)*',
    redirect: '/'
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// Navigation guard
router.beforeEach(async (to, from, next) => {
  const auth = useAuth()

  // Wait for auth to initialize (max 3s timeout internally)
  console.log(`Router: Entering ${to.path}, initialized: ${auth.isInitialized.value}`)
  if (!auth.isInitialized.value) {
    console.log('Router: Waiting for auth initialization...')
    await auth.init()
  }

  const { user, isAuthenticated, role } = auth
  console.log('Router: Auth ready. User:', user.value?.email, 'Role:', role.value)

  const requiresAuth = to.matched.some(record => record.meta.requiresAuth)
  const requiresGuest = to.matched.some(record => record.meta.requiresGuest)
  const requiredRole = to.meta.role

  if (requiresAuth && !isAuthenticated.value) {
    // Needs auth, but user is not logged in
    console.warn('Unauthorized access attempt to:', to.path)
    return next({ name: 'Home' })
  }

  if (requiresGuest && isAuthenticated.value) {
    // Guest page (like login), but user is already logged in
    let userRole = role.value || user.value?.user_metadata?.role || 'student'
    // Normalize role
    if (userRole === 'teacher' || userRole === 'admin') userRole = 'lecturer'

    if (userRole === 'lecturer') {
      return next({ name: 'LecturerDashboard' })
    }
    if (userRole === 'student') {
      return next({ name: 'StudentHomepage' })
    }
    return next({ name: 'Home' })
  }

  if (requiresAuth && requiredRole) {
    let userRole = role.value || user.value?.user_metadata?.role || 'student'
    // Normalize role
    if (userRole === 'teacher' || userRole === 'admin') userRole = 'lecturer'

    // Handle both 'teacher' and 'lecturer' for lecturer routes
    const isLecturerRoute = requiredRole === 'lecturer'
    const isUserLecturer = userRole === 'teacher' || userRole === 'lecturer'

    if (isLecturerRoute && !isUserLecturer) {
      console.warn(`Role mismatch: User with role '${userRole}' tried to access '${to.path}' which requires '${requiredRole}'`)
      return next({ name: 'Home' })
    }
    if (!isLecturerRoute && userRole !== requiredRole) {
      console.warn(`Role mismatch: User with role '${userRole}' tried to access '${to.path}' which requires '${requiredRole}'`)
      return next({ name: 'Home' })
    }

    // Redirect to their respective dashboards if role mismatch
    if (isUserLecturer && !isLecturerRoute) {
      return next({ name: 'LecturerDashboard' })
    }
    if (userRole === 'student' && !isLecturerRoute && requiredRole !== 'student') {
      return next({ name: 'StudentHomepage' })
    }
  }

  // All checks passed
  next()
})

export default router
