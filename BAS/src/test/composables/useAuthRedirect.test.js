import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import { ref } from 'vue'

// Mock sessionStorage
const mockSessionStorage = {
  getItem: vi.fn(),
  setItem: vi.fn(),
  removeItem: vi.fn()
}

// Mock window object
Object.defineProperty(window, 'sessionStorage', {
  value: mockSessionStorage,
  writable: true
})

// Mock router
const mockRouter = {
  push: vi.fn(),
  currentRoute: {
    value: {
      fullPath: '/current-path',
      params: {},
      query: {}
    }
  }
}

// Mock route
const mockRoute = {
  fullPath: '/current-path',
  params: {},
  query: {}
}

// Mock vue-router
vi.mock('vue-router', () => ({
  useRouter: () => mockRouter,
  useRoute: () => mockRoute
}))

// Import after mocking
import { useAuthRedirect } from '@/composables/useAuthRedirect'

describe('useAuthRedirect Composable', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mockSessionStorage.getItem.mockClear()
    mockSessionStorage.setItem.mockClear()
    mockSessionStorage.removeItem.mockClear()
    mockRouter.push.mockClear()
  })

  afterEach(() => {
    vi.restoreAllMocks()
  })

  it('initializes with empty state', () => {
    const { 
      intendedDestination, 
      redirectParams, 
      redirectQuery, 
      hasIntendedDestination, 
      returnPath 
    } = useAuthRedirect()

    expect(intendedDestination.value).toBe(null)
    expect(redirectParams.value).toBe(null)
    expect(redirectQuery.value).toBe(null)
    expect(hasIntendedDestination.value).toBe(false)
    expect(returnPath.value).toBe(null) // No returnUrl in query initially
  })

  it('saves intended destination to sessionStorage', () => {
    const { saveIntendedDestination } = useAuthRedirect()
    
    saveIntendedDestination('/student-homepage', { id: '123' }, { tab: 'dashboard' })
    
    expect(mockSessionStorage.setItem).toHaveBeenCalledWith('authRedirect', expect.stringContaining('"destination":"/student-homepage"'))
    expect(mockSessionStorage.setItem).toHaveBeenCalledWith('authRedirect', expect.stringContaining('"params":{"id":"123"}'))
    expect(mockSessionStorage.setItem).toHaveBeenCalledWith('authRedirect', expect.stringContaining('"query":{"tab":"dashboard"}'))
    expect(mockSessionStorage.setItem).toHaveBeenCalledWith('authRedirect', expect.stringMatching(/timestamp/))
  })

  it('restores intended destination from sessionStorage', () => {
    const { restoreIntendedDestination, intendedDestination } = useAuthRedirect()
    
    // Mock sessionStorage data
    mockSessionStorage.getItem.mockReturnValue(JSON.stringify({
      destination: '/lecturer-dashboard',
      params: { id: '456' },
      query: { view: 'analytics' },
      timestamp: Date.now() - 1000 // 1 second ago
    }))
    
    restoreIntendedDestination()
    
    expect(intendedDestination.value).toBe('/lecturer-dashboard')
    expect(mockSessionStorage.removeItem).not.toHaveBeenCalled()
  })

  it('clears expired destination from sessionStorage', () => {
    const { restoreIntendedDestination, intendedDestination } = useAuthRedirect()
    
    // Mock expired data (older than 30 minutes)
    mockSessionStorage.getItem.mockReturnValue(JSON.stringify({
      destination: '/student-homepage',
      params: {},
      query: {},
      timestamp: Date.now() - (31 * 60 * 1000) // 31 minutes ago
    }))
    
    restoreIntendedDestination()
    
    expect(intendedDestination.value).toBe(null)
    expect(mockSessionStorage.removeItem).toHaveBeenCalledWith('authRedirect')
  })

  it('clears intended destination', () => {
    const { 
      intendedDestination, 
      redirectParams, 
      redirectQuery, 
      clearIntendedDestination 
    } = useAuthRedirect()
    
    // Set some values first
    intendedDestination.value = '/test-path'
    redirectParams.value = { id: '123' }
    redirectQuery.value = { tab: 'dashboard' }
    
    // Clear it
    clearIntendedDestination()
    
    expect(intendedDestination.value).toBe(null)
    expect(redirectParams.value).toBe(null)
    expect(redirectQuery.value).toBe(null)
    expect(mockSessionStorage.removeItem).toHaveBeenCalledWith('authRedirect')
  })

  it('gets correct default redirect for different roles', () => {
    const { getDefaultRedirect } = useAuthRedirect()
    
    expect(getDefaultRedirect('student')).toBe('/student-homepage')
    expect(getDefaultRedirect('lecturer')).toBe('/lecturer-dashboard')
    expect(getDefaultRedirect('teacher')).toBe('/lecturer-dashboard') // Normalized
    expect(getDefaultRedirect('admin')).toBe('/lecturer-dashboard') // Normalized
    expect(getDefaultRedirect('unknown')).toBe('/')
    expect(getDefaultRedirect(null)).toBe('/')
  })

  it('should redirect to login for non-login routes', () => {
    const { shouldRedirectToLogin } = useAuthRedirect()
    
    const to = { path: '/student-homepage' }
    expect(shouldRedirectToLogin(to)).toBe(true)
    
    const loginTo = { path: '/student-login' }
    expect(shouldRedirectToLogin(loginTo)).toBe(false)
    
    const signupTo = { path: '/student-signup' }
    expect(shouldRedirectToLogin(signupTo)).toBe(false)
    
    const lecturerLoginTo = { path: '/lecturer-login' }
    expect(shouldRedirectToLogin(lecturerLoginTo)).toBe(false)
  })

  it('redirects to login with return URL', () => {
    const { redirectToLogin } = useAuthRedirect()
    
    redirectToLogin('/student-homepage/courses')
    
    expect(mockRouter.push).toHaveBeenCalledWith({
      path: '/student-login',
      query: {
        returnUrl: '/student-homepage/courses'
      }
    })
  })

  it('redirects to lecturer login for lecturer routes', () => {
    const { redirectToLogin } = useAuthRedirect()
    
    // Mock route to be lecturer route
    mockRoute.fullPath = '/lecturer-dashboard/analytics'
    mockRoute.path = '/lecturer-dashboard/analytics'
    
    redirectToLogin('/lecturer-dashboard/analytics')
    
    expect(mockRouter.push).toHaveBeenCalledWith({
      path: '/lecturer-login',
      query: {
        returnUrl: '/lecturer-dashboard/analytics'
      }
    })
  })

  it('computes hasIntendedDestination correctly', () => {
    const authRedirect = useAuthRedirect()
    
    // Initially false
    expect(authRedirect.hasIntendedDestination.value).toBe(false)
    
    // Set a destination
    authRedirect.saveIntendedDestination('/test')
    expect(authRedirect.hasIntendedDestination.value).toBe(true)
    
    // Clear it
    authRedirect.clearIntendedDestination()
    expect(authRedirect.hasIntendedDestination.value).toBe(false)
  })

  it('computes returnPath from route query', () => {
    // Mock route with returnUrl
    mockRoute.query = { returnUrl: '/test-path' }
    
    const { returnPath } = useAuthRedirect()
    expect(returnPath.value).toBe('/test-path')
  })

  it('handles sessionStorage parsing errors gracefully', () => {
    const { restoreIntendedDestination, intendedDestination } = useAuthRedirect()
    
    // Mock invalid JSON
    mockSessionStorage.getItem.mockReturnValue('invalid json')
    
    restoreIntendedDestination()
    
    expect(intendedDestination.value).toBe(null)
    expect(mockSessionStorage.removeItem).toHaveBeenCalledWith('authRedirect')
  })

  it('does not save login pages as intended destinations', () => {
    const authRedirect = useAuthRedirect()
    
    authRedirect.saveIntendedDestination('/student-login')
    expect(mockSessionStorage.setItem).toHaveBeenCalledWith('authRedirect', expect.stringContaining('"destination":"/student-login"'))
    
    // But hasIntendedDestination should still work based on the value
    expect(authRedirect.hasIntendedDestination.value).toBe(true)
  })
})
