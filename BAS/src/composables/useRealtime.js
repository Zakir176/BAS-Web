import { ref, onUnmounted, onMounted } from 'vue'
import { supabase } from '@/supabase'
import { useToast } from './useToast'
import { useRealtimeNotifications } from './useRealtimeNotifications'

/**
 * Real-time subscription composable for live attendance updates
 * Provides real-time updates for attendance records and session status
 */
export function useRealtime() {
  const { toast } = useToast()
  const notifications = useRealtimeNotifications()
  
  // Subscription state
  const subscriptions = ref(new Map())
  const isConnected = ref(false)
  const isConnecting = ref(false)
  const connectionError = ref(null)
  
  // Real-time data
  const liveAttendance = ref([])
  const liveSessionStatus = ref(null)
  const liveRoster = ref([])
  const lastUpdate = ref(null)

  /**
   * Subscribe to attendance updates for a specific session
   */
  const subscribeToAttendance = (sessionId) => {
    if (!sessionId) return null
    
    const channelName = `attendance:${sessionId}`
    
    // Clean up existing subscription if any
    if (subscriptions.value.has(channelName)) {
      unsubscribe(channelName)
    }
    
    try {
      isConnecting.value = true
      
      // Create subscription
      const subscription = supabase
        .channel(channelName)
        .on('postgres_changes', 
          { 
            event: '*', 
            schema: 'public', 
            table: 'attendance',
            filter: `session_id=eq.${sessionId}`
          }, 
          (payload) => {
            console.log('Attendance update received:', payload)
            handleAttendanceUpdate(payload)
          }
        )
        .subscribe((status) => {
          console.log('Subscription status:', status)
          
          if (status === 'SUBSCRIBED') {
            isConnected.value = true
            isConnecting.value = false
            connectionError.value = null
            toast.success('Live updates enabled')
          } else if (status === 'CHANNEL_ERROR') {
            isConnected.value = false
            isConnecting.value = false
            connectionError.value = 'Connection error'
            toast.error('Failed to connect to live updates')
          }
        })
      
      subscriptions.value.set(channelName, subscription)
      
      return subscription
    } catch (error) {
      console.error('Failed to create attendance subscription:', error)
      isConnecting.value = false
      connectionError.value = error.message
      toast.error('Failed to enable live updates')
      return null
    }
  }

  /**
   * Subscribe to session status updates
   */
  const subscribeToSessionStatus = (sessionId) => {
    if (!sessionId) return null
    
    const channelName = `session:${sessionId}`
    
    // Clean up existing subscription if any
    if (subscriptions.value.has(channelName)) {
      unsubscribe(channelName)
    }
    
    try {
      const subscription = supabase
        .channel(channelName)
        .on('postgres_changes', 
          { 
            event: '*', 
            schema: 'public', 
            table: 'sessions',
            filter: `session_id=eq.${sessionId}`
          }, 
          (payload) => {
            console.log('Session status update received:', payload)
            handleSessionUpdate(payload)
          }
        )
        .subscribe((status) => {
          console.log('Session subscription status:', status)
        })
      
      subscriptions.value.set(channelName, subscription)
      
      return subscription
    } catch (error) {
      console.error('Failed to create session subscription:', error)
      return null
    }
  }

  /**
   * Subscribe to course-level updates (all sessions in a course)
   */
  const subscribeToCourseUpdates = (courseId) => {
    if (!courseId) return null
    
    const channelName = `course:${courseId}`
    
    // Clean up existing subscription if any
    if (subscriptions.value.has(channelName)) {
      unsubscribe(channelName)
    }
    
    try {
      const subscription = supabase
        .channel(channelName)
        .on('postgres_changes', 
          { 
            event: '*', 
            schema: 'public', 
            table: 'sessions',
            filter: `course_id=eq.${courseId}`
          }, 
          (payload) => {
            console.log('Course session update received:', payload)
            handleCourseUpdate(payload)
          }
        )
        .subscribe((status) => {
          console.log('Course subscription status:', status)
        })
      
      subscriptions.value.set(channelName, subscription)
      
      return subscription
    } catch (error) {
      console.error('Failed to create course subscription:', error)
      return null
    }
  }

  /**
   * Handle attendance update events
   */
  const handleAttendanceUpdate = (payload) => {
    const { eventType, new: newRecord, old: oldRecord } = payload
    
    lastUpdate.value = new Date()
    
    switch (eventType) {
      case 'INSERT':
        // New attendance record created
        liveAttendance.value.push(newRecord)
        updateRosterStudent(newRecord.student_id, true)
        break
        
      case 'UPDATE':
        // Attendance record updated
        const index = liveAttendance.value.findIndex(a => a.id === newRecord.id)
        if (index !== -1) {
          liveAttendance.value[index] = newRecord
          updateRosterStudent(newRecord.student_id, newRecord.status === 'Present')
        }
        break
        
      case 'DELETE':
        // Attendance record deleted
        liveAttendance.value = liveAttendance.value.filter(a => a.id !== oldRecord.id)
        updateRosterStudent(oldRecord.student_id, false)
        break
    }
  }

  /**
   * Handle session update events
   */
  const handleSessionUpdate = (payload) => {
    const { eventType, new: newRecord } = payload
    
    switch (eventType) {
      case 'UPDATE':
        liveSessionStatus.value = newRecord
        break
    }
  }

  /**
   * Handle course update events
   */
  const handleCourseUpdate = (payload) => {
    const { eventType, new: newRecord } = payload
    
    switch (eventType) {
      case 'INSERT':
        // New session created in course
        toast.info(`New session created: ${newRecord.session_name || 'Session'}`)
        break
    }
  }

  /**
   * Update roster student status
   */
  const updateRosterStudent = (studentId, isPresent) => {
    const studentIndex = liveRoster.value.findIndex(s => s.student_id === studentId)
    if (studentIndex !== -1) {
      liveRoster.value[studentIndex].present = isPresent
    }
  }

  /**
   * Set initial roster data
   */
  const setRosterData = (roster) => {
    liveRoster.value = roster
  }

  /**
   * Set initial attendance data
   */
  const setAttendanceData = (attendance) => {
    liveAttendance.value = attendance
  }

  /**
   * Unsubscribe from a specific channel
   */
  const unsubscribe = (channelName) => {
    const subscription = subscriptions.value.get(channelName)
    if (subscription) {
      supabase.removeChannel(subscription)
      subscriptions.value.delete(channelName)
      console.log(`Unsubscribed from ${channelName}`)
    }
  }

  /**
   * Unsubscribe from all channels
   */
  const unsubscribeAll = () => {
    subscriptions.value.forEach((subscription, channelName) => {
      supabase.removeChannel(subscription)
      console.log(`Unsubscribed from ${channelName}`)
    })
    subscriptions.value.clear()
    isConnected.value = false
  }

  /**
   * Reconnect all subscriptions
   */
  const reconnect = () => {
    // This would need to track what we were subscribed to
    // For now, just clear and let components re-subscribe
    unsubscribeAll()
  }

  /**
   * Get connection status
   */
  const getConnectionStatus = () => {
    return {
      isConnected: isConnected.value,
      isConnecting: isConnecting.value,
      error: connectionError.value,
      subscriptionCount: subscriptions.value.size
    }
  }

  /**
   * Auto-cleanup on component unmount
   */
  onUnmounted(() => {
    unsubscribeAll()
  })

  /**
   * Initialize connection status
   */
  onMounted(() => {
    // Check if Supabase realtime is available
    if (supabase.realtime) {
      console.log('Supabase realtime is available')
    } else {
      console.warn('Supabase realtime not available')
      connectionError.value = 'Realtime not available'
    }
  })

  return {
    // State
    isConnected,
    isConnecting,
    connectionError,
    liveAttendance,
    liveSessionStatus,
    liveRoster,
    lastUpdate,
    
    // Methods
    subscribeToAttendance,
    subscribeToSessionStatus,
    subscribeToCourseUpdates,
    setRosterData,
    setAttendanceData,
    unsubscribe,
    unsubscribeAll,
    reconnect,
    getConnectionStatus
  }
}
