import { ref, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'

/**
 * Composable for handling authentication redirects
 * Stores the intended destination and provides methods to handle post-login navigation
 */
export function useAuthRedirect() {
  const router = useRouter()
  const route = useRoute()
  
  // Store the intended destination
  const intendedDestination = ref(null)
  const redirectParams = ref(null)
  const redirectQuery = ref(null)

  /**
   * Save the current route as the intended destination before redirecting to login
   */
  const saveIntendedDestination = (path = null, params = null, query = null) => {
    intendedDestination.value = path || route.fullPath
    redirectParams.value = params || route.params
    redirectQuery.value = query || route.query
    
    // Store in sessionStorage for persistence across page reloads
    if (typeof window !== 'undefined') {
      sessionStorage.setItem('authRedirect', JSON.stringify({
        destination: intendedDestination.value,
        params: redirectParams.value,
        query: redirectQuery.value,
        timestamp: Date.now()
      }))
    }
  }

  /**
   * Restore the intended destination from sessionStorage
   */
  const restoreIntendedDestination = () => {
    if (typeof window !== 'undefined') {
      const stored = sessionStorage.getItem('authRedirect')
      if (stored) {
        try {
          const data = JSON.parse(stored)
          // Only restore if it's less than 30 minutes old
          if (Date.now() - data.timestamp < 30 * 60 * 1000) {
            intendedDestination.value = data.destination
            redirectParams.value = data.params
            redirectQuery.value = data.query
          } else {
            // Clear expired data
            sessionStorage.removeItem('authRedirect')
          }
        } catch (error) {
          console.warn('Failed to parse auth redirect data:', error)
          sessionStorage.removeItem('authRedirect')
        }
      }
    }
  }

  /**
   * Clear the stored intended destination
   */
  const clearIntendedDestination = () => {
    intendedDestination.value = null
    redirectParams.value = null
    redirectQuery.value = null
    
    if (typeof window !== 'undefined') {
      sessionStorage.removeItem('authRedirect')
    }
  }

  /**
   * Get the default redirect path based on user role
   */
  const getDefaultRedirect = (userRole) => {
    switch (userRole) {
      case 'lecturer':
      case 'teacher':
      case 'admin':
        return '/lecturer-dashboard'
      case 'student':
        return '/student-homepage'
      default:
        return '/'
    }
  }

  /**
   * Redirect to the intended destination after successful login
   */
  const redirectToIntendedDestination = (userRole = null) => {
    const destination = intendedDestination.value
    const params = redirectParams.value
    const query = redirectQuery.value

    // Clear the stored destination
    clearIntendedDestination()

    if (destination && destination !== '/' && destination !== '/student-login' && destination !== '/lecturer-login') {
      // Redirect to the intended destination
      router.push({
        path: destination,
        params: params,
        query: query
      })
    } else {
      // No specific destination, redirect to role-based default
      const defaultPath = getDefaultRedirect(userRole)
      router.push(defaultPath)
    }
  }

  /**
   * Check if the current route should trigger an auth redirect
   */
  const shouldRedirectToLogin = (to) => {
    // Don't redirect if already on login pages
    const loginPaths = ['/student-login', '/lecturer-login', '/student-signup', '/lecturer-signup']
    return !loginPaths.includes(to.path)
  }

  /**
   * Redirect to login with return URL
   */
  const redirectToLogin = (returnPath = null) => {
    saveIntendedDestination(returnPath)
    
    // Determine which login page to use based on the attempted route
    const isStudentRoute = returnPath?.includes('/student') || route.path.includes('/student')
    const loginRoute = isStudentRoute ? '/student-login' : '/lecturer-login'
    
    router.push({
      path: loginRoute,
      query: {
        ...route.query,
        returnUrl: returnPath || route.fullPath
      }
    })
  }

  // Computed properties
  const hasIntendedDestination = computed(() => {
    return !!intendedDestination.value && intendedDestination.value !== '/'
  })

  const returnPath = computed(() => {
    return route.query.returnUrl || intendedDestination.value
  })

  return {
    // State
    intendedDestination,
    redirectParams,
    redirectQuery,
    hasIntendedDestination,
    returnPath,
    
    // Methods
    saveIntendedDestination,
    restoreIntendedDestination,
    clearIntendedDestination,
    redirectToIntendedDestination,
    getDefaultRedirect,
    shouldRedirectToLogin,
    redirectToLogin
  }
}
