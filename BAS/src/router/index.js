import { createRouter, createWebHistory } from 'vue-router'
import { useAuth } from '@/shared/composables/useAuth'
import { useAuthRedirect } from '@/shared/composables/useAuthRedirect'

import AppHome from '@/shared/views/AppHome.vue'
import LecturerLogin from '@/features/lecturer/LecturerLogin.vue'
import LecturerSignup from '@/features/lecturer/LecturerSignup.vue'
import StudentLogin from '@/features/student/StudentLogin.vue'
import StudentSignup from '@/features/student/StudentSignup.vue'
import StudentHomepage from '@/features/student/StudentHomepage.vue'

import ReportPage from '@/features/lecturer/ReportPage.vue'
import LecturerDashboard from '@/features/lecturer/LecturerDashboard.vue'
import PrivacyPolicy from '@/shared/views/PrivacyPolicy.vue'
import TermsOfService from '@/shared/views/TermsOfService.vue'

const routes = [
  {
    path: '/',
    name: 'AppHome',
    component: AppHome,
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
    path: '/privacy-policy',
    name: 'PrivacyPolicy',
    component: PrivacyPolicy,
    meta: { requiresAuth: false }
  },
  {
    path: '/terms-of-service',
    name: 'TermsOfService',
    component: TermsOfService,
    meta: { requiresAuth: false }
  },
  {
    path: '/lecturer-homepage',
    redirect: '/lecturer-dashboard'
  },
  {
    path: '/forgot-password',
    name: 'ForgotPassword',
    component: () => import('@/shared/views/ForgotPassword.vue'),
    meta: { requiresAuth: false, requiresGuest: true }
  },
  {
    path: '/reset-password',
    name: 'ResetPassword',
    component: () => import('@/shared/views/ResetPassword.vue'),
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
  const authRedirect = useAuthRedirect()

  // Wait for auth to initialize (max 3s timeout internally)
  if (!auth.isInitialized.value) {
    await auth.initialize()
  }

  const { user, isAuthenticated, role } = auth

  const requiresAuth = to.matched.some(record => record.meta.requiresAuth)
  const requiresGuest = to.matched.some(record => record.meta.requiresGuest)
  const requiredRole = to.meta.role

  if (requiresAuth && !isAuthenticated.value) {
    // Needs auth, but user is not logged in
    console.warn('Unauthorized access attempt to:', to.path)
    
    // Save the intended destination before redirecting to login
    if (authRedirect.shouldRedirectToLogin(to)) {
      authRedirect.redirectToLogin()
      return // Don't call next() as we're redirecting
    }
    
    return next({ name: 'AppHome' })
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
    return next({ name: 'AppHome' })
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
      return next({ name: 'AppHome' })
    }
    if (!isLecturerRoute && userRole !== requiredRole) {
      console.warn(`Role mismatch: User with role '${userRole}' tried to access '${to.path}' which requires '${requiredRole}'`)
      return next({ name: 'AppHome' })
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
