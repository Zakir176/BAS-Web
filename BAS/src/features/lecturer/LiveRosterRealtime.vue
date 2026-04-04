<template>
  <section class="live-roster-wrapper">
    <!-- Header Area -->
    <div class="roster-header">
      <div class="header-main">
        <h2 class="title">
          Live Roster
          <div v-if="isConnected" class="pulse-badge">
            <span class="pulsing-dot"></span> LIVE
          </div>
        </h2>
        <p class="subtitle" v-if="activeSessionName">
          Monitoring: <strong>{{ activeSessionName }}</strong>
        </p>
      </div>
      
      <div class="header-actions">
        <div class="roster-search">
          <span class="search-icon">🔍</span>
          <input 
            v-model="searchQuery" 
            type="text" 
            placeholder="Search students..." 
            class="search-input"
          />
        </div>
        
        <div class="roster-filters">
          <button 
            v-for="filter in ['all', 'present', 'absent']" 
            :key="filter"
            class="filter-tab"
            :class="{ 'active': activeFilter === filter }"
            @click="activeFilter = filter"
          >
            {{ filter.toUpperCase() }}
          </button>
        </div>

        <div class="header-divider"></div>

        <div class="roster-stats">
          <span class="stat-number">{{ livePresentCount }}</span>
          <span class="stat-divider">/</span>
          <span class="stat-total">{{ filteredRoster.length }}</span>
        </div>
        
        <button 
          v-if="activeSessionId" 
          class="btn-danger-outline" 
          @click="$emit('complete')"
          :disabled="isEndingSession"
        >
          {{ isEndingSession ? 'Ending...' : 'End' }}
        </button>
      </div>
    </div>

    <!-- Notification Ribbon -->
    <transition name="ribbon-slide">
      <div v-if="showUpdateNotification" class="update-ribbon">
        <span class="ribbon-icon">⚡</span>
        <span class="ribbon-text">{{ updateNotificationText }}</span>
      </div>
    </transition>

    <!-- Roster Grid Table -->
    <div class="roster-grid-container" v-if="filteredRoster.length > 0">
      <div class="roster-table-header">
        <div class="col-ident">STUDENT</div>
        <div class="col-status">STATUS</div>
        <div class="col-time">TIMESTAMP</div>
        <div class="col-action">ACTION</div>
      </div>
      <div class="roster-rows">
        <div 
          v-for="student in filteredRoster" 
          :key="student.id" 
          class="roster-row" 
          :class="{ 
            'is-present': student.present,
            'recently-scanned': recentlyUpdatedStudents.has(student.id || student.student_id)
          }"
        >
          <div class="col-ident">
            <div class="student-avatar-small">{{ student.full_name?.charAt(0) }}</div>
            <div class="student-info-dense">
              <span class="name">{{ student.full_name }}</span>
              <span class="id">{{ student.student_number || student.student_id }}</span>
            </div>
          </div>
          
          <div class="col-status">
            <div v-if="student.present" class="status-badge-present">
              <span class="dot-live"></span> PRESENT
            </div>
            <div v-else class="status-badge-absent">ABSENT</div>
          </div>

          <div class="col-time">
            <span v-if="student.present && (student.attendance_time || lastUpdate)" class="precise-time">
              {{ formatPreciseTime(student.attendance_time || lastUpdate) }}
            </span>
            <span v-else class="text-muted-xs">--:--:--</span>
          </div>

          <div class="col-action">
            <button 
              v-if="!student.present"
              class="btn-action-sm" 
              @click="$emit('mark', student.id || student.student_id)"
              :disabled="!isConnected"
            >
              Mark Manual
            </button>
            <div v-else class="success-check-icon">
              <svg width="18" height="18" viewBox="0 0 20 20" fill="currentColor">
                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd" />
              </svg>
            </div>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Empty State -->
    <div v-else-if="!isLoading" class="empty-roster-state">
      <div class="empty-icon">📍</div>
      <h3>No Active Session</h3>
      <p>Start a new session to populate the live roster with students.</p>
      <button class="btn-glow-small" @click="$emit('scan')">
        Open Scanner
      </button>
    </div>

    <!-- Connection Error -->
    <div v-if="connectionError && !isConnecting" class="error-banner">
      <div class="error-info">
        <span class="icon">⚠️</span>
        <div class="text">
          <h5>Connection Lost</h5>
          <p>{{ connectionError }}</p>
        </div>
      </div>
      <button class="btn-retry" @click="reconnect">Retry</button>
    </div>
  </section>
</template>

<script setup>
import { ref, computed, watch, onUnmounted } from 'vue'
import { useRealtime } from '@/shared/composables/useRealtime'

const props = defineProps({
  activeRoster: { type: Array, required: true },
  activeSessionName: { type: String, default: '' },
  activeSessionId: { type: [String, Number], default: null },
  presentCount: { type: Number, default: 0 },
  isEndingSession: { type: Boolean, default: false },
  isLoading: { type: Boolean, default: true }
})

defineEmits(['mark', 'complete', 'scan'])

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
const searchQuery = ref('')
const activeFilter = ref('all')

// Computed properties
const filteredRoster = computed(() => {
  let roster = [...liveRoster.value]
  
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    roster = roster.filter(s => 
      s.full_name?.toLowerCase().includes(query) || 
      (s.student_number || s.student_id)?.toLowerCase().includes(query)
    )
  }
  
  if (activeFilter.value === 'present') {
    roster = roster.filter(s => s.present)
  } else if (activeFilter.value === 'absent') {
    roster = roster.filter(s => !s.present)
  }
  
  return roster
})

const livePresentCount = computed(() => {
  return liveRoster.value.filter(student => student.present).length
})

// Watch for session changes to subscribe/unsubscribe
watch(() => props.activeSessionId, (newSessionId, oldSessionId) => {
  if (oldSessionId) {
    unsubscribe(`attendance:${oldSessionId}`)
  }
  if (newSessionId) {
    subscribeToAttendance(newSessionId)
  }
}, { immediate: true })

// Watch for roster changes
watch(() => props.activeRoster, (newRoster) => {
  setRosterData(newRoster)
}, { immediate: true, deep: true })

// Watch liveRoster for changes to trigger flash animation
watch(liveRoster, (newRoster, oldRoster) => {
  if (!oldRoster || oldRoster.length === 0) return
  
  newRoster.forEach(newStudent => {
    const oldStudent = oldRoster.find(s => (s.id === newStudent.id || s.student_id === newStudent.student_id))
    if (oldStudent && !oldStudent.present && newStudent.present) {
      handleRealtimeUpdate(newStudent.id || newStudent.student_id, true)
    }
  })
}, { deep: true })

// Handle real-time updates
const handleRealtimeUpdate = (studentId, isPresent) => {
  recentlyUpdatedStudents.value.add(studentId)
  const student = liveRoster.value.find(s => (s.id === studentId || s.student_id === studentId))
  if (student) {
    updateNotificationText.value = `${student.full_name} marked ${isPresent ? 'present' : 'absent'}`
    showUpdateNotification.value = true
    setTimeout(() => { showUpdateNotification.value = false }, 3000)
  }
  setTimeout(() => { recentlyUpdatedStudents.value.delete(studentId) }, 2000)
}

// Format precise time helper
const formatPreciseTime = (timestamp) => {
  if (!timestamp) return ''
  const date = new Date(timestamp)
  const h = date.getHours().toString().padStart(2, '0')
  const m = date.getMinutes().toString().padStart(2, '0')
  const s = date.getSeconds().toString().padStart(2, '0')
  const ms = date.getMilliseconds().toString().padStart(3, '0')
  return `${h}:${m}:${s}.${ms}`
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
.live-roster-wrapper {
  display: flex;
  flex-direction: column;
  height: 100%;
}

.roster-header {
  padding-bottom: 1.5rem;
  border-bottom: 1px solid var(--border-light);
  margin-bottom: 1rem;
}

.header-main {
  margin-bottom: 1rem;
}

.title {
  font-size: 1.35rem;
  font-weight: 850;
  display: flex;
  align-items: center;
  gap: 0.75rem;
  color: var(--text-main);
  margin-bottom: 0.25rem;
}

.pulse-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.4rem;
  background: var(--success-bg);
  color: var(--success);
  padding: 0.2rem 0.6rem;
  border-radius: 20px;
  font-size: 0.7rem;
  font-weight: 800;
  letter-spacing: 0.5px;
  border: 1px solid var(--success);
}

.pulsing-dot {
  width: 6px; height: 6px;
  background: var(--success);
  border-radius: 50%;
  animation: pulse 1.5s infinite;
}

@keyframes pulse {
  0% { transform: scale(0.95); box-shadow: 0 0 0 0 rgba(16, 185, 129, 0.7); }
  70% { transform: scale(1); box-shadow: 0 0 0 6px rgba(16, 185, 129, 0); }
  100% { transform: scale(0.95); box-shadow: 0 0 0 0 rgba(16, 185, 129, 0); }
}

.subtitle {
  font-size: 0.85rem;
  color: var(--text-muted);
}
.subtitle strong {
  color: var(--primary);
}

.header-actions {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.roster-search {
  position: relative;
  display: flex;
  align-items: center;
}

.search-icon {
  position: absolute;
  left: 0.75rem;
  font-size: 0.75rem;
  color: var(--text-muted);
  pointer-events: none;
}

.search-input {
  background: var(--bg-main);
  border: 1px solid var(--border-light);
  border-radius: 8px;
  padding: 0.4rem 0.75rem 0.4rem 2rem;
  font-size: 0.75rem;
  color: var(--text-main);
  width: 180px;
  transition: all 0.2s;
}

.search-input:focus {
  outline: none;
  border-color: var(--primary);
  width: 220px;
  box-shadow: 0 0 0 3px var(--primary-glow);
}

.roster-filters {
  display: flex;
  background: var(--bg-main);
  padding: 2px;
  border-radius: 8px;
  border: 1px solid var(--border-light);
}

.filter-tab {
  background: transparent;
  border: none;
  padding: 0.35rem 0.75rem;
  border-radius: 6px;
  font-size: 0.65rem;
  font-weight: 800;
  color: var(--text-muted);
  cursor: pointer;
  transition: all 0.2s;
}

.filter-tab.active {
  background: var(--bg-card);
  color: var(--primary);
  box-shadow: var(--shadow-sm);
}

.header-divider {
  width: 1px;
  height: 24px;
  background: var(--border-light);
}

.roster-stats {
  font-size: 0.8rem;
  color: var(--text-muted);
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 0.2rem;
}

.stat-number {
  font-size: 1rem;
  font-weight: 850;
  color: var(--text-main);
}
.stat-divider { opacity: 0.5; }

.btn-danger-outline {
  background: transparent;
  color: var(--error);
  border: 1px solid var(--error);
  padding: 0.4rem 0.8rem;
  border-radius: 8px;
  font-size: 0.75rem;
  font-weight: 800;
  cursor: pointer;
  transition: all 0.2s;
  text-transform: uppercase;
}
.btn-danger-outline:hover:not(:disabled) {
  background: var(--error-bg);
  border-color: var(--error);
}
.btn-danger-outline:disabled { opacity: 0.5; cursor: not-allowed; }

/* Ribbon */
.update-ribbon {
  background: linear-gradient(90deg, var(--primary) 0%, var(--accent) 100%);
  color: white;
  padding: 0.6rem 1rem;
  border-radius: 12px;
  margin-bottom: 1rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.85rem;
  font-weight: 600;
}

.ribbon-slide-enter-active, .ribbon-slide-leave-active { transition: all 0.3s ease; }
.ribbon-slide-enter-from, .ribbon-slide-leave-to { opacity: 0; transform: translateY(-10px); }

/* Roster Grid Table */
.roster-grid-container {
  flex: 1;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
}

.roster-table-header {
  display: grid;
  grid-template-columns: 2fr 1fr 1fr 100px;
  padding: 0.75rem 1rem;
  background: var(--bg-main);
  border-radius: 12px;
  margin-bottom: 0.5rem;
  font-size: 0.65rem;
  font-weight: 800;
  color: var(--text-muted);
  letter-spacing: 0.1em;
  text-transform: uppercase;
}

.roster-rows {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.roster-row {
  display: grid;
  grid-template-columns: 2fr 1fr 1fr 100px;
  align-items: center;
  padding: 0.5rem 1rem;
  border-radius: 8px;
  transition: all 0.2s ease;
  border: 1px solid transparent;
}

.roster-row:hover {
  background: var(--bg-main);
}

.roster-row.is-present {
  background: var(--success-bg);
}

/* HIGH INTENSITY FLASH */
.recently-scanned {
  animation: premium-flash 2.5s cubic-bezier(0.19, 1, 0.22, 1) forwards;
  z-index: 10;
  position: relative;
}

@keyframes premium-flash {
  0% {
    background: var(--success);
    box-shadow: 0 0 20px var(--primary-glow);
    border-color: var(--success);
    transform: scale(1.02);
  }
  20% {
    transform: scale(1);
  }
  100% {
    background: var(--success-bg);
    box-shadow: none;
    border-color: transparent;
  }
}

.col-ident {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.student-avatar-small {
  width: 28px;
  height: 28px;
  background: var(--bg-main);
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 800;
  font-size: 0.75rem;
  color: var(--text-muted);
}

.student-info-dense {
  display: flex;
  flex-direction: column;
}

.student-info-dense .name {
  font-size: 0.85rem;
  font-weight: 700;
  color: var(--text-main);
}

.student-info-dense .id {
  font-size: 0.65rem;
  color: var(--text-muted);
  font-weight: 600;
}

.status-badge-present {
  display: inline-flex;
  align-items: center;
  gap: 0.4rem;
  color: var(--success);
  font-size: 0.65rem;
  font-weight: 800;
}

.dot-live {
  width: 6px;
  height: 6px;
  background: var(--success);
  border-radius: 50%;
}

.status-badge-absent {
  color: var(--text-muted);
  font-size: 0.65rem;
  font-weight: 700;
  opacity: 0.6;
}

.precise-time {
  font-family: 'JetBrains Mono', 'Courier New', monospace;
  font-size: 0.75rem;
  font-weight: 600;
  color: var(--success);
}

.text-muted-xs {
  font-size: 0.7rem;
  color: var(--text-muted);
  opacity: 0.4;
}

.btn-action-sm {
  background: var(--bg-main);
  border: 1px solid var(--border-light);
  padding: 0.25rem 0.5rem;
  border-radius: 6px;
  font-size: 0.65rem;
  font-weight: 700;
  color: var(--text-main);
  cursor: pointer;
  transition: all 0.2s;
}

.btn-action-sm:hover {
  background: var(--primary);
  color: white;
  border-color: var(--primary);
}

.success-check-icon {
  color: var(--success);
  display: flex;
  justify-content: center;
}

/* Empty state */
.empty-roster-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem 2rem;
  text-align: center;
  color: var(--text-muted);
  background: var(--bg-main);
  border-radius: 16px;
  border: 1px dashed var(--border-medium);
  margin-top: 1.5rem;
}

.empty-icon {
  font-size: 2.5rem;
  margin-bottom: 1rem;
}

.empty-roster-state h3 {
  font-size: 1.1rem;
  font-weight: 800;
  color: var(--text-main);
  margin-bottom: 0.5rem;
}

.empty-roster-state p {
  font-size: 0.85rem;
  margin-bottom: 1.5rem;
  max-width: 250px;
}

.btn-glow-small {
  background: var(--primary);
  color: white;
  border: none;
  padding: 0.6rem 1.2rem;
  border-radius: 10px;
  font-weight: 700;
  font-size: 0.85rem;
  cursor: pointer;
  box-shadow: var(--primary-glow);
  transition: all 0.2s;
}
.btn-glow-small:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 16px rgba(59, 130, 246, 0.4);
}

/* Error banner */
.error-banner {
  margin-top: 1rem;
  background: var(--error-bg);
  border: 1px solid var(--error);
  border-radius: 12px;
  padding: 1rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.error-info {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}
</style>
