<template>
  <div class="live-attendance-status">
    <!-- Connection Status -->
    <div class="connection-indicator" :class="{ 'connected': isConnected, 'connecting': isConnecting, 'error': connectionError }">
      <span class="status-dot"></span>
      <span class="status-text">
        {{ isConnected ? 'Live Updates' : isConnecting ? 'Connecting...' : connectionError || 'Offline' }}
      </span>
    </div>

    <!-- Today's Sessions -->
    <div class="sessions-container">
      <h3>Today's Sessions</h3>
      
      <div v-if="todaySessions.length === 0" class="no-sessions">
        <div class="empty-state">
          <span class="empty-icon">📅</span>
          <p>No sessions scheduled for today</p>
        </div>
      </div>

      <div v-else class="sessions-grid">
        <div 
          v-for="session in todaySessions" 
          :key="session.session_id"
          class="session-card"
          :class="{ 
            'active': session.is_active,
            'attended': session.attendance_status === 'Present',
            'recently-updated': recentlyUpdatedSessions.has(session.session_id)
          }"
        >
          <div class="session-header">
            <div class="session-info">
              <h4>{{ session.course_name }}</h4>
              <p class="session-time">{{ formatSessionTime(session.session_time) }}</p>
            </div>
            <div class="session-status">
              <span v-if="session.is_active" class="active-badge">
                <span class="pulse-dot"></span>
                ACTIVE
              </span>
              <span v-else-if="session.attendance_status" class="status-badge" :class="session.attendance_status.toLowerCase()">
                {{ session.attendance_status }}
              </span>
              <span v-else class="status-badge pending">Pending</span>
            </div>
          </div>

          <div class="session-details">
            <div class="attendance-info">
              <div v-if="session.attendance_time" class="attendance-time">
                <span class="icon">🕐</span>
                Marked at {{ formatTime(session.attendance_time) }}
              </div>
              <div v-if="session.attendance_method" class="attendance-method">
                <span class="icon">{{ getMethodIcon(session.attendance_method) }}</span>
                {{ session.attendance_method }}
              </div>
            </div>

            <div v-if="session.is_active" class="live-indicator-small">
              <span class="live-text">LIVE</span>
              <span class="live-pulse"></span>
            </div>
          </div>

          <!-- Real-time Update Notification -->
          <div v-if="session.showNotification" class="update-notification">
            <span class="notification-icon">🔄</span>
            <span class="notification-text">{{ session.notificationText }}</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Attendance Statistics -->
    <div class="stats-container">
      <h3>Attendance Overview</h3>
      <div class="stats-grid">
        <div class="stat-card">
          <div class="stat-value">{{ attendanceStats.present }}</div>
          <div class="stat-label">Present</div>
          <div class="stat-percentage">{{ attendanceStats.presentPercentage }}%</div>
        </div>
        <div class="stat-card">
          <div class="stat-value">{{ attendanceStats.absent }}</div>
          <div class="stat-label">Absent</div>
          <div class="stat-percentage">{{ attendanceStats.absentPercentage }}%</div>
        </div>
        <div class="stat-card">
          <div class="stat-value">{{ attendanceStats.total }}</div>
          <div class="stat-label">Total Sessions</div>
          <div class="stat-percentage">100%</div>
        </div>
      </div>
    </div>

    <!-- Connection Error -->
    <div v-if="connectionError && !isConnecting" class="connection-error">
      <div class="error-content">
        <span class="error-icon">⚠️</span>
        <div class="error-details">
          <h4>Connection Lost</h4>
          <p>{{ connectionError }}</p>
          <Button variant="secondary" size="sm" @click="reconnect">
            Reconnect
          </Button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch, onMounted, onUnmounted } from 'vue'
import { useAuth } from '@/composables/useAuth'
import { useRealtime } from '@/composables/useRealtime'
import { supabase } from '@/supabase'
import Button from '@/components/ui/Button.vue'

const { user } = useAuth()
const {
  isConnected,
  isConnecting,
  connectionError,
  subscribeToAttendance,
  subscribeToSessionStatus,
  unsubscribe,
  reconnect: reconnectRealtime
} = useRealtime()

// Local state
const todaySessions = ref([])
const attendanceStats = ref({
  present: 0,
  absent: 0,
  total: 0,
  presentPercentage: 0,
  absentPercentage: 0
})
const recentlyUpdatedSessions = ref(new Set())

// Computed properties
const activeSessions = computed(() => {
  return todaySessions.value.filter(session => session.is_active)
})

// Fetch today's sessions
const fetchTodaySessions = async () => {
  if (!user.value) return

  try {
    const today = new Date().toISOString().split('T')[0]
    
    const { data, error } = await supabase
      .from('sessions')
      .select(`
        *,
        courses(course_name),
        attendance!inner(
          student_id,
          status,
          timestamp,
          method
        )
      `)
      .eq('attendance.student_id', user.value.user_metadata?.student_id || user.value.id)
      .eq('session_date', today)
      .order('session_time', { ascending: true })

    if (error) throw error

    // Process sessions data
    const processedSessions = (data || []).map(session => ({
      ...session,
      course_name: session.courses?.course_name || 'Unknown Course',
      attendance_status: session.attendance?.[0]?.status || null,
      attendance_time: session.attendance?.[0]?.timestamp || null,
      attendance_method: session.attendance?.[0]?.method || null,
      is_active: isActiveSession(session.session_time),
      showNotification: false,
      notificationText: ''
    }))

    todaySessions.value = processedSessions
    calculateStats()

    // Subscribe to real-time updates for active sessions
    processedSessions.forEach(session => {
      if (session.is_active) {
        subscribeToAttendance(session.session_id)
      }
    })

  } catch (error) {
    console.error('Failed to fetch sessions:', error)
  }
}

// Check if session is currently active
const isActiveSession = (sessionTime) => {
  if (!sessionTime) return false
  
  const now = new Date()
  const [hours, minutes] = sessionTime.split(':')
  const sessionStart = new Date()
  sessionStart.setHours(parseInt(hours), parseInt(minutes), 0, 0)
  
  // Consider session active for 2 hours after start time
  const sessionEnd = new Date(sessionStart.getTime() + 2 * 60 * 60 * 1000)
  
  return now >= sessionStart && now <= sessionEnd
}

// Calculate attendance statistics
const calculateStats = () => {
  const present = todaySessions.value.filter(s => s.attendance_status === 'Present').length
  const absent = todaySessions.value.filter(s => s.attendance_status === 'Absent').length
  const total = todaySessions.value.length

  attendanceStats.value = {
    present,
    absent,
    total,
    presentPercentage: total > 0 ? Math.round((present / total) * 100) : 0,
    absentPercentage: total > 0 ? Math.round((absent / total) * 100) : 0
  }
}

// Handle real-time attendance updates
const handleAttendanceUpdate = (sessionId, status, timestamp, method) => {
  const sessionIndex = todaySessions.value.findIndex(s => s.session_id === sessionId)
  
  if (sessionIndex !== -1) {
    const session = todaySessions.value[sessionIndex]
    
    // Update session data
    session.attendance_status = status
    session.attendance_time = timestamp
    session.attendance_method = method
    
    // Show notification
    session.showNotification = true
    session.notificationText = `Marked ${status.toLowerCase()} at ${formatTime(timestamp)}`
    
    // Add to recently updated for animation
    recentlyUpdatedSessions.value.add(sessionId)
    
    // Hide notification after 3 seconds
    setTimeout(() => {
      session.showNotification = false
    }, 3000)
    
    // Remove from recently updated after animation
    setTimeout(() => {
      recentlyUpdatedSessions.value.delete(sessionId)
    }, 2000)
    
    // Recalculate stats
    calculateStats()
  }
}

// Format time helper
const formatTime = (timestamp) => {
  if (!timestamp) return ''
  const date = new Date(timestamp)
  return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
}

// Format session time
const formatSessionTime = (sessionTime) => {
  if (!sessionTime) return ''
  const [hours, minutes] = sessionTime.split(':')
  const date = new Date()
  date.setHours(parseInt(hours), parseInt(minutes))
  return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
}

// Get method icon
const getMethodIcon = (method) => {
  switch (method?.toLowerCase()) {
    case 'barcode': return '📷'
    case 'manual': return '✋'
    case 'auto': return '🤖'
    default: return '📝'
  }
}

// Reconnect handler
const reconnect = () => {
  reconnectRealtime()
  fetchTodaySessions()
}

// Watch for user changes
watch(() => user.value, (newUser) => {
  if (newUser) {
    fetchTodaySessions()
  }
}, { immediate: true })

// Cleanup on unmount
onUnmounted(() => {
  todaySessions.value.forEach(session => {
    if (session.is_active) {
      unsubscribe(`attendance:${session.session_id}`)
    }
  })
})

// Initialize on mount
onMounted(() => {
  fetchTodaySessions()
})
</script>

<style scoped>
.live-attendance-status {
  max-width: 800px;
  margin: 0 auto;
  padding: 1rem;
}

.connection-indicator {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  border-radius: 20px;
  font-size: 0.875rem;
  font-weight: 600;
  margin-bottom: 1.5rem;
}

.connection-indicator.connected {
  background: rgba(34, 197, 94, 0.1);
  color: var(--success);
  border: 1px solid rgba(34, 197, 94, 0.2);
}

.connection-indicator.connecting {
  background: rgba(250, 204, 21, 0.1);
  color: var(--warning);
  border: 1px solid rgba(250, 204, 21, 0.2);
}

.connection-indicator.error {
  background: rgba(239, 68, 68, 0.1);
  color: var(--error);
  border: 1px solid rgba(239, 68, 68, 0.2);
}

.status-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background: currentColor;
}

.connection-indicator.connected .status-dot {
  animation: pulse 2s infinite;
}

@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.5; }
}

.sessions-container h3,
.stats-container h3 {
  font-size: 1.25rem;
  font-weight: 700;
  margin-bottom: 1rem;
  color: var(--text-main);
}

.no-sessions {
  text-align: center;
  padding: 2rem;
}

.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
}

.empty-icon {
  font-size: 3rem;
  opacity: 0.5;
}

.sessions-grid {
  display: grid;
  gap: 1rem;
}

.session-card {
  background: var(--bg-card);
  border: 1px solid var(--border-light);
  border-radius: 12px;
  padding: 1rem;
  transition: all 0.3s ease;
}

.session-card.active {
  border-color: var(--primary);
  background: rgba(59, 130, 246, 0.05);
}

.session-card.attended {
  border-color: var(--success);
  background: rgba(34, 197, 94, 0.05);
}

.session-card.recently-updated {
  animation: highlightUpdate 2s ease-out;
}

@keyframes highlightUpdate {
  0% {
    transform: scale(1.02);
    box-shadow: 0 0 20px rgba(59, 130, 246, 0.3);
  }
  100% {
    transform: scale(1);
    box-shadow: none;
  }
}

.session-header {
  display: flex;
  justify-content: space-between;
  align-items: start;
  margin-bottom: 0.75rem;
}

.session-info h4 {
  font-size: 1rem;
  font-weight: 700;
  color: var(--text-main);
  margin-bottom: 0.25rem;
}

.session-time {
  font-size: 0.875rem;
  color: var(--text-muted);
}

.session-status {
  display: flex;
  align-items: center;
}

.active-badge {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  background: var(--primary);
  color: white;
  padding: 0.25rem 0.5rem;
  border-radius: 12px;
  font-size: 0.75rem;
  font-weight: 700;
}

.pulse-dot {
  width: 6px;
  height: 6px;
  background: white;
  border-radius: 50%;
  animation: pulse 2s infinite;
}

.status-badge {
  padding: 0.25rem 0.5rem;
  border-radius: 12px;
  font-size: 0.75rem;
  font-weight: 700;
}

.status-badge.present {
  background: var(--success);
  color: white;
}

.status-badge.absent {
  background: var(--error);
  color: white;
}

.status-badge.pending {
  background: var(--border-medium);
  color: var(--text-muted);
}

.session-details {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.attendance-info {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.attendance-time,
.attendance-method {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.8rem;
  color: var(--text-muted);
}

.icon {
  font-size: 0.875rem;
}

.live-indicator-small {
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.live-text {
  font-size: 0.75rem;
  font-weight: 700;
  color: var(--primary);
}

.live-pulse {
  width: 6px;
  height: 6px;
  background: var(--primary);
  border-radius: 50%;
  animation: pulse 2s infinite;
}

.update-notification {
  margin-top: 0.75rem;
  padding: 0.5rem;
  background: linear-gradient(135deg, #f0f9ff 0%, #e0f2fe 100%);
  border: 1px solid #bae6fd;
  border-radius: 8px;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  animation: slideDown 0.3s ease-out;
}

.notification-icon {
  font-size: 1rem;
  flex-shrink: 0;
}

.notification-text {
  font-size: 0.8rem;
  color: #0369a1;
  font-weight: 500;
}

@keyframes slideDown {
  from {
    opacity: 0;
    transform: translateY(-5px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.stats-container {
  margin-top: 2rem;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 1rem;
}

.stat-card {
  background: var(--bg-card);
  border: 1px solid var(--border-light);
  border-radius: 12px;
  padding: 1rem;
  text-align: center;
}

.stat-value {
  font-size: 2rem;
  font-weight: 800;
  color: var(--text-main);
  margin-bottom: 0.25rem;
}

.stat-label {
  font-size: 0.875rem;
  color: var(--text-muted);
  font-weight: 600;
  margin-bottom: 0.25rem;
}

.stat-percentage {
  font-size: 0.75rem;
  color: var(--primary);
  font-weight: 700;
}

.connection-error {
  margin-top: 1.5rem;
  background: rgba(239, 68, 68, 0.1);
  border: 1px solid rgba(239, 68, 68, 0.2);
  border-radius: 12px;
  padding: 1rem;
}

.error-content {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.error-icon {
  font-size: 1.5rem;
  flex-shrink: 0;
}

.error-details h4 {
  color: var(--error);
  margin-bottom: 0.25rem;
}

.error-details p {
  color: var(--text-muted);
  font-size: 0.875rem;
  margin-bottom: 0.75rem;
}

@media (max-width: 640px) {
  .live-attendance-status {
    padding: 0.5rem;
  }
  
  .session-header {
    flex-direction: column;
    gap: 0.5rem;
  }
  
  .session-details {
    flex-direction: column;
    align-items: start;
    gap: 0.5rem;
  }
  
  .stats-grid {
    grid-template-columns: 1fr;
  }
}
</style>
