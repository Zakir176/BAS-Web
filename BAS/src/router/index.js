import { supabase } from '@/supabase'
import { useUserStore } from '@/stores/userStore'

import Home from '../views/Home.vue'
import LecturerLogin from '../views/LecturerLogin.vue'
import LecturerSignup from '../views/LecturerSignup.vue'
import StudentLogin from '../views/StudentLogin.vue'
import StudentSignup from '../views/StudentSignup.vue'
import StudentHomepage from '../views/StudentHomepage.vue'
import StudentUploadPage from '../views/StudentUploadPage.vue'
import ReportPage from '../views/ReportPage.vue'
import LecturerDashboard from '../views/LecturerDashboard.vue'

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
    path: '/student-upload-page',
    name: 'StudentUploadPage',
    component: StudentUploadPage,
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
  const userStore = useUserStore()

  // Wait for store initialization if needed
  if (userStore.isLoading) {
    // Basic polling or wait for a promise would be better, 
    // but for simplicity we rely on App.vue init
  }

  const user = userStore.user
  const userRole = userStore.role

  const requiresAuth = to.matched.some(record => record.meta.requiresAuth)
  const requiresGuest = to.matched.some(record => record.meta.requiresGuest)
  const requiredRole = to.meta.role

  if (requiresAuth && !user) {
    // Needs auth, but user is not logged in
    console.warn('Unauthorized access attempt to:', to.path)
    return next({ name: 'Home' })
  }

  if (requiresGuest && user) {
    // Guest page (like login), but user is already logged in
    if (userRole === 'lecturer') {
      return next({ name: 'LecturerDashboard' })
    }
    if (userRole === 'student') {
      return next({ name: 'StudentHomepage' })
    }
    return next({ name: 'Home' })
  }

  if (requiresAuth && requiredRole && userRole !== requiredRole) {
    // Has role requirement, but user's role does not match
    console.warn(`Role mismatch: User with role '${userRole}' tried to access '${to.path}' which requires '${requiredRole}'`)
    // Redirect to their respective dashboards
    if (userRole === 'lecturer') {
      return next({ name: 'LecturerDashboard' })
    }
    if (userRole === 'student') {
      return next({ name: 'StudentHomepage' })
    }
    return next({ name: 'Home' })
  }

  // All checks passed
  next()
})

export default router
