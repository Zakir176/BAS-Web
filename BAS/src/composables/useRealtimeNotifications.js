import { ref, onUnmounted } from 'vue'
import { useToast } from './useToast'
import { useRealtime } from './useRealtime'

/**
 * Real-time notification system for attendance events
 * Provides desktop notifications and in-app alerts for real-time updates
 */
export function useRealtimeNotifications() {
  const { toast } = useToast()
  const { isConnected } = useRealtime()
  
  // Notification state
  const notificationPermission = ref('default')
  const isNotificationSupported = ref(false)
  const activeNotifications = ref(new Map())
  const notificationHistory = ref([])
  
  // Initialize notification support
  const initializeNotifications = () => {
    if ('Notification' in window) {
      isNotificationSupported.value = true
      notificationPermission.value = Notification.permission
      
      // Request permission if not granted
      if (notificationPermission.value === 'default') {
        requestNotificationPermission()
      }
    } else {
      isNotificationSupported.value = false
      console.warn('Browser notifications not supported')
    }
  }

  // Request notification permission
  const requestNotificationPermission = async () => {
    if (!isNotificationSupported.value) return false
    
    try {
      const permission = await Notification.requestPermission()
      notificationPermission.value = permission
      
      if (permission === 'granted') {
        toast.success('Notifications enabled! You\'ll receive real-time attendance updates.')
        return true
      } else if (permission === 'denied') {
        toast.info('Notifications blocked. Enable in browser settings to receive real-time updates.')
        return false
      }
    } catch (error) {
      console.error('Failed to request notification permission:', error)
      return false
    }
    
    return false
  }

  // Show desktop notification
  const showDesktopNotification = (title, options = {}) => {
    if (!isNotificationSupported.value || notificationPermission.value !== 'granted') {
      return null
    }

    const notificationOptions = {
      icon: '/favicon.ico',
      badge: '/favicon.ico',
      tag: options.tag || 'attendance-update',
      requireInteraction: options.important || false,
      ...options
    }

    try {
      const notification = new Notification(title, notificationOptions)
      
      // Store active notification
      const notificationId = Date.now().toString()
      activeNotifications.value.set(notificationId, notification)
      
      // Auto-close after 5 seconds unless important
      if (!options.important) {
        setTimeout(() => {
          notification.close()
          activeNotifications.value.delete(notificationId)
        }, 5000)
      }
      
      // Handle click
      notification.onclick = () => {
        notification.close()
        activeNotifications.value.delete(notificationId)
        
        // Focus window if possible
        if (window.focus) {
          window.focus()
        }
        
        // Handle custom click action
        if (options.onClick) {
          options.onClick()
        }
      }
      
      // Add to history
      notificationHistory.value.unshift({
        id: notificationId,
        title,
        options,
        timestamp: new Date(),
        type: options.type || 'info'
      })
      
      // Keep only last 50 notifications in history
      if (notificationHistory.value.length > 50) {
        notificationHistory.value = notificationHistory.value.slice(0, 50)
      }
      
      return notificationId
    } catch (error) {
      console.error('Failed to show notification:', error)
      return null
    }
  }

  // Show attendance notification
  const showAttendanceNotification = (studentName, action, sessionName, timestamp) => {
    const title = `Attendance Update: ${action}`
    const body = `${studentName} was marked ${action.toLowerCase()} for ${sessionName}`
    
    // In-app toast
    toast.info(`${studentName} marked ${action.toLowerCase()}`)
    
    // Desktop notification
    showDesktopNotification(title, {
      body,
      tag: `attendance-${studentName}-${sessionName}`,
      type: 'attendance',
      onClick: () => {
        // Navigate to lecturer dashboard if user is a lecturer
        if (window.location.pathname.includes('lecturer')) {
          window.location.href = '/lecturer-dashboard'
        }
      }
    })
  }

  // Show session start notification
  const showSessionStartNotification = (sessionName, courseName) => {
    const title = 'Session Started'
    const body = `${sessionName} has started for ${courseName}`
    
    // In-app toast
    toast.info(`Session started: ${sessionName}`)
    
    // Desktop notification
    showDesktopNotification(title, {
      body,
      tag: `session-start-${sessionName}`,
      type: 'session',
      important: true,
      onClick: () => {
        // Navigate to relevant page
        if (window.location.pathname.includes('lecturer')) {
          window.location.href = '/lecturer-dashboard'
        } else if (window.location.pathname.includes('student')) {
          window.location.href = '/student-homepage'
        }
      }
    })
  }

  // Show session end notification
  const showSessionEndNotification = (sessionName, courseName, stats) => {
    const title = 'Session Ended'
    const body = `${sessionName} has ended. ${stats.present} present, ${stats.absent} absent.`
    
    // In-app toast
    toast.info(`Session ended: ${sessionName}`)
    
    // Desktop notification
    showDesktopNotification(title, {
      body,
      tag: `session-end-${sessionName}`,
      type: 'session',
      onClick: () => {
        if (window.location.pathname.includes('lecturer')) {
          window.location.href = '/lecturer-dashboard'
        }
      }
    })
  }

  // Show barcode scan notification
  const showBarcodeScanNotification = (studentName, sessionName) => {
    const title = 'Barcode Scanned'
    const body = `${studentName} scanned barcode for ${sessionName}`
    
    // In-app toast
    toast.success(`${studentName} scanned successfully`)
    
    // Desktop notification
    showDesktopNotification(title, {
      body,
      tag: `barcode-${studentName}`,
      type: 'barcode',
      onClick: () => {
        if (window.location.pathname.includes('lecturer')) {
          window.location.href = '/lecturer-dashboard'
        }
      }
    })
  }

  // Show connection status notification
  const showConnectionNotification = (status, error = null) => {
    if (status === 'connected') {
      toast.success('Real-time connection established')
      
      showDesktopNotification('Connected', {
        body: 'Real-time updates are now active',
        tag: 'connection-status',
        type: 'connection'
      })
    } else if (status === 'disconnected') {
      toast.warning('Real-time connection lost')
      
      showDesktopNotification('Disconnected', {
        body: error || 'Real-time updates are temporarily unavailable',
        tag: 'connection-status',
        type: 'connection',
        important: true
      })
    } else if (status === 'reconnecting') {
      toast.info('Reconnecting to real-time updates...')
    }
  }

  // Clear all notifications
  const clearAllNotifications = () => {
    activeNotifications.value.forEach((notification) => {
      notification.close()
    })
    activeNotifications.value.clear()
  }

  // Clear notification by ID
  const clearNotification = (notificationId) => {
    const notification = activeNotifications.value.get(notificationId)
    if (notification) {
      notification.close()
      activeNotifications.value.delete(notificationId)
    }
  }

  // Get notification stats
  const getNotificationStats = () => {
    return {
      supported: isNotificationSupported.value,
      permission: notificationPermission.value,
      activeCount: activeNotifications.value.size,
      historyCount: notificationHistory.value.length,
      isConnected: isConnected.value
    }
  }

  // Monitor connection status and show notifications
  const monitorConnection = () => {
    // This would be called from the realtime composable when connection status changes
    // For now, we'll just expose the method
  }

  // Cleanup on unmount
  onUnmounted(() => {
    clearAllNotifications()
  })

  // Initialize on creation
  initializeNotifications()

  return {
    // State
    notificationPermission,
    isNotificationSupported,
    activeNotifications,
    notificationHistory,
    
    // Methods
    initializeNotifications,
    requestNotificationPermission,
    showDesktopNotification,
    showAttendanceNotification,
    showSessionStartNotification,
    showSessionEndNotification,
    showBarcodeScanNotification,
    showConnectionNotification,
    clearAllNotifications,
    clearNotification,
    getNotificationStats,
    monitorConnection
  }
}
