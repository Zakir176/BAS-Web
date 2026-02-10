<template>
  <section class="content-section py-8">
    <div class="section-header">
      <div class="header-titles">
        <h2>
          Active Session Roster
          <span v-if="isConnected" class="live-indicator">
            <span class="live-dot"></span>
            LIVE
          </span>
        </h2>
        <p v-if="activeSessionName">Monitoring: <span class="highlight-alt">{{ activeSessionName }}</span></p>
        <p v-if="lastUpdate" class="last-update">
          Last update: {{ formatTime(lastUpdate) }}
        </p>
      </div>
      <div class="roster-actions">
        <div class="connection-status" :class="{ 'connected': isConnected, 'connecting': isConnecting, 'error': connectionError }">
          <span v-if="isConnected" class="status-text">🟢 Connected</span>
          <span v-else-if="isConnecting" class="status-text">🟡 Connecting...</span>
          <span v-else-if="connectionError" class="status-text">🔴 {{ connectionError }}</span>
          <span v-else class="status-text">⚪ Disconnected</span>
        </div>
        <span class="roster-stats">
          <strong>{{ presentCount }}</strong> Present / {{ liveRoster.length }} Total
        </span>
        <Button 
          v-if="activeSessionId" 
          variant="danger" 
          size="sm" 
          @click="$emit('complete')"
          :disabled="isEndingSession"
          class="ml-4"
        >
          {{ isEndingSession ? 'Ending...' : 'Complete Session' }}
        </Button>
      </div>
    </div>

    <!-- Real-time Updates Notification -->
    <div v-if="showUpdateNotification" class="update-notification">
      <div class="notification-content">
        <span class="notification-icon">🔄</span>
        <span class="notification-text">{{ updateNotificationText }}</span>
      </div>
    </div>

    <div class="roster-grid-v2" v-if="liveRoster.length > 0">
      <div 
        v-for="student in liveRoster" 
        :key="student.student_id" 
        class="student-card-v2" 
        :class="{ 
          'is-present': student.present,
          'recently-updated': recentlyUpdatedStudents.has(student.student_id)
        }"
      >
        <div class="student-avatar-mini">
          {{ student.full_name?.charAt(0) }}
        </div>
        <div class="student-info-mini">
          <div class="name">{{ student.full_name }}</div>
          <div class="sid">{{ student.student_id }}</div>
          <div v-if="student.present && student.attendance_time" class="attendance-time">
            {{ formatTime(student.attendance_time) }}
          </div>
        </div>
        <div class="student-status-action">
          <span v-if="student.present" class="status-icon-check">
            ✓
            <span class="check-time">{{ formatTime(student.attendance_time) }}</span>
          </span>
          <button 
            v-else 
            class="mark-btn-v2" 
            @click="$emit('mark', student.student_id)"
            :disabled="!isConnected"
          >
            <span class="icon">📍</span> Mark
          </button>
        </div>
      </div>
    </div>
    
    <div v-else-if="!isLoading">
      <EmptyState 
        icon="📷"
        title="No Active Session"
        description="Roster visibility is only available during an active session. Start scanning students to populate this area."
      >
        <template #action>
          <Button variant="primary" @click="$emit('scan')">Start Scanning</Button>
        </template>
      </EmptyState>
    </div>

    <!-- Connection Error State -->
    <div v-if="connectionError && !isConnecting" class="connection-error-card">
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
  </section>
</template>

<script setup>
import { ref, computed, watch, onMounted, onUnmounted } from 'vue'
import Button from '@/components/ui/Button.vue'
import EmptyState from '@/components/ui/EmptyState.vue'
import { useRealtime } from '@/composables/useRealtime'

const props = defineProps({
  activeRoster: { type: Array, required: true },
  activeSessionName: { type: String, default: '' },
  activeSessionId: { type: [String, Number], default: null },
  presentCount: { type: Number, default: 0 },
  isEndingSession: { type: Boolean, default: false },
  isLoading: { type: Boolean, default: true }
})

const emit = defineEmits(['mark', 'complete', 'scan'])

// Real-time functionality
const {
  isConnected,
  isConnecting,
  connectionError,
  liveRoster,
  lastUpdate,
  subscribeToAttendance,
  setRosterData,
  unsubscribe,
  reconnect: reconnectRealtime
} = useRealtime()

// Local state
const recentlyUpdatedStudents = ref(new Set())
const showUpdateNotification = ref(false)
const updateNotificationText = ref('')

// Computed properties
const presentCount = computed(() => {
  return liveRoster.value.filter(student => student.present).length
})

// Watch for session changes to subscribe/unsubscribe
watch(() => props.activeSessionId, (newSessionId, oldSessionId) => {
  if (oldSessionId) {
    // Unsubscribe from old session
    unsubscribe(`attendance:${oldSessionId}`)
  }
  
  if (newSessionId) {
    // Subscribe to new session
    subscribeToAttendance(newSessionId)
  }
}, { immediate: true })

// Watch for roster changes
watch(() => props.activeRoster, (newRoster) => {
  setRosterData(newRoster)
}, { immediate: true, deep: true })

// Handle real-time updates
const handleRealtimeUpdate = (studentId, isPresent, timestamp) => {
  // Add to recently updated set for animation
  recentlyUpdatedStudents.value.add(studentId)
  
  // Show notification
  const student = liveRoster.value.find(s => s.student_id === studentId)
  if (student) {
    updateNotificationText.value = `${student.full_name} marked ${isPresent ? 'present' : 'absent'}`
    showUpdateNotification.value = true
    
    // Hide notification after 3 seconds
    setTimeout(() => {
      showUpdateNotification.value = false
    }, 3000)
  }
  
  // Remove from recently updated after animation
  setTimeout(() => {
    recentlyUpdatedStudents.value.delete(studentId)
  }, 2000)
}

// Format time helper
const formatTime = (timestamp) => {
  if (!timestamp) return ''
  const date = new Date(timestamp)
  return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
}

// Reconnect handler
const reconnect = () => {
  reconnectRealtime()
  if (props.activeSessionId) {
    subscribeToAttendance(props.activeSessionId)
  }
}

// Cleanup on unmount
onUnmounted(() => {
  if (props.activeSessionId) {
    unsubscribe(`attendance:${props.activeSessionId}`)
  }
})
</script>

<style scoped>
.content-section {
  margin-top: 2.5rem;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.header-titles h2 {
  font-size: 1.5rem;
  font-weight: 850;
  margin-bottom: 0.25rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.live-indicator {
  display: inline-flex;
  align-items: center;
  gap: 0.25rem;
  font-size: 0.75rem;
  font-weight: 700;
  color: var(--success);
  background: rgba(34, 197, 94, 0.1);
  padding: 0.25rem 0.5rem;
  border-radius: 12px;
  border: 1px solid rgba(34, 197, 94, 0.2);
}

.live-dot {
  width: 8px;
  height: 8px;
  background: var(--success);
  border-radius: 50%;
  animation: pulse 2s infinite;
}

@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.5; }
}

.highlight-alt {
  color: var(--primary);
  font-weight: 700;
}

.last-update {
  font-size: 0.8rem;
  color: var(--text-muted);
  margin-top: 0.25rem;
}

.roster-actions {
  display: flex;
  align-items: center;
  gap: 1.5rem;
}

.connection-status {
  display: flex;
  align-items: center;
  padding: 0.25rem 0.75rem;
  border-radius: 20px;
  font-size: 0.8rem;
  font-weight: 600;
}

.connection-status.connected {
  background: rgba(34, 197, 94, 0.1);
  color: var(--success);
}

.connection-status.connecting {
  background: rgba(250, 204, 21, 0.1);
  color: var(--warning);
}

.connection-status.error {
  background: rgba(239, 68, 68, 0.1);
  color: var(--error);
}

.roster-stats {
  font-size: 0.9rem;
  font-weight: 600;
  color: var(--text-muted);
}

.roster-stats strong {
  color: var(--text-main);
  font-size: 1.1rem;
}

.ml-4 {
  margin-left: 1rem;
}

.update-notification {
  background: linear-gradient(135deg, #f0f9ff 0%, #e0f2fe 100%);
  border: 1px solid #bae6fd;
  border-radius: 12px;
  padding: 1rem;
  margin-bottom: 1.5rem;
  animation: slideDown 0.3s ease-out;
}

.notification-content {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.notification-icon {
  font-size: 1.5rem;
  flex-shrink: 0;
}

.notification-text {
  font-size: 0.9rem;
  color: #0369a1;
  font-weight: 500;
}

.roster-grid-v2 {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 1.25rem;
}

.student-card-v2 {
  background: var(--bg-card);
  padding: 1rem;
  border-radius: 20px;
  display: flex;
  align-items: center;
  gap: 1rem;
  border: 1px solid var(--border-light);
  transition: all 0.3s cubic-bezier(0.23, 1, 0.32, 1);
}

.student-card-v2.is-present {
  border-color: var(--success-soft);
  background: var(--success-soft);
  opacity: 0.8;
}

.student-card-v2.recently-updated {
  animation: highlightUpdate 2s ease-out;
}

@keyframes highlightUpdate {
  0% {
    border-color: var(--primary);
    background: rgba(59, 130, 246, 0.1);
    transform: scale(1.02);
  }
  100% {
    border-color: var(--border-light);
    background: var(--bg-card);
    transform: scale(1);
  }
}

.student-avatar-mini {
  width: 40px;
  height: 40px;
  background: var(--bg-main);
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 800;
  color: var(--primary);
  flex-shrink: 0;
}

.student-info-mini {
  flex: 1;
}

.student-info-mini .name {
  font-size: 0.95rem;
  font-weight: 700;
  color: var(--text-main);
}

.student-info-mini .sid {
  font-size: 0.75rem;
  color: var(--text-muted);
  font-weight: 600;
}

.attendance-time {
  font-size: 0.7rem;
  color: var(--success);
  font-weight: 600;
  margin-top: 0.25rem;
}

.status-icon-check {
  width: 28px;
  height: 28px;
  background: var(--success);
  color: white;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.8rem;
  font-weight: 900;
  position: relative;
}

.check-time {
  position: absolute;
  top: -20px;
  right: 0;
  font-size: 0.6rem;
  color: var(--success);
  white-space: nowrap;
  background: white;
  padding: 2px 4px;
  border-radius: 4px;
  box-shadow: 0 1px 3px rgba(0,0,0,0.1);
}

.mark-btn-v2 {
  padding: 0.5rem 1rem;
  border-radius: 10px;
  border: 1px solid var(--border-medium);
  background: var(--bg-card);
  font-size: 0.85rem;
  font-weight: 700;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 0.4rem;
  transition: all 0.2s;
}

.mark-btn-v2:hover:not(:disabled) {
  background: var(--primary);
  border-color: var(--primary);
  color: white;
}

.mark-btn-v2:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.connection-error-card {
  background: rgba(239, 68, 68, 0.1);
  border: 1px solid rgba(239, 68, 68, 0.2);
  border-radius: 12px;
  padding: 1rem;
  margin-top: 1rem;
}

.error-content {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.error-icon {
  font-size: 2rem;
  flex-shrink: 0;
}

.error-details h4 {
  color: var(--error);
  margin-bottom: 0.25rem;
}

.error-details p {
  color: var(--text-muted);
  font-size: 0.9rem;
  margin-bottom: 0.75rem;
}

@keyframes slideDown {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@media (max-width: 640px) {
  .section-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 1rem;
  }
  
  .roster-actions {
    width: 100%;
    justify-content: space-between;
  }
  
  .live-indicator {
    font-size: 0.65rem;
  }
}
</style>
