<template>
  <div class="lecturer-dashboard">
    <main class="main-content">
      <div class="dashboard-container">
        
        <!-- Streamlined Pro Header -->
        <header class="pro-dashboard-header">
          <div class="header-left">
            <div class="pro-badge">Lecturer Pro</div>
            <h1>Good morning, <span class="highlight-text">{{ lecturerName }}</span></h1>
            <p class="session-summary">
              <span class="icon">📅</span> 
              <strong>{{ stats.todaySessions }} sessions</strong> active for today
            </p>
          </div>
          <div class="header-right">
            <div class="quick-actions">
              <button class="action-btn-secondary" @click="showCreateCourse">
                <span class="icon">📘</span> Course
              </button>
              <button class="action-btn-secondary" @click="showCreateSession">
                <span class="icon">➕</span> Session
              </button>
              <BaseButton variant="primary" size="md" @click="showBarcodeScanner" class="scan-cta">
                <span class="icon">📷</span> Start Scanning
              </BaseButton>
            </div>
          </div>
        </header>

        <!-- High-Density Stats -->
        <DashboardStats :stats="stats" />

        <!-- High-Density Dashboard Layout -->
        <div class="dashboard-grid mt-6">
          
          <!-- Column 1: Courses & Activity -->
          <div class="grid-col main-col">
            <CourseGrid 
              :courses="courses"
              :is-loading="isLoading"
              @view-all="viewAllCourses"
              @manage="manageCourse"
              @create="showCreateCourse"
            />
            
            <section class="activity-section mt-8">
              <RecentActivity :activities="recentSessions" />
            </section>
          </div>

          <!-- Column 2: Live Monitor (Sticky) -->
          <div class="grid-col side-col">
            <div class="sticky-side">
              <LiveRosterRealtime 
                :active-roster="activeRoster"
                :active-session-name="activeSessionName"
                :active-session-id="activeSessionId"
                :present-count="presentCount"
                :is-ending-session="isEndingSession"
                :is-loading="isLoading"
                @mark="markAsPresent"
                @complete="completeSession"
                @scan="showBarcodeScanner"
              />
            </div>
          </div>
          
        </div>
      </div>
    </main>

    <!-- Modals -->
    <BaseModal :is-open="isScannerOpen" @close="closeBarcodeScanner" class="scanner-modal-fullscreen">
      <template #default>
        <ScannerInterface 
          :session-name="activeSessionName"
          :scanned-count="scannedCount"
          :total-students="stats.totalStudents"
          :last-scanned="lastScanned"
          :scan-status="scanStatus"
          :roster="activeRoster"
          @close="closeBarcodeScanner"
          @detected="handleBarcodeDetected"
          @complete="completeSession"
        />
      </template>
    </BaseModal>

    <CreateCourseModal 
      :is-open="isCreateCourseModalOpen" 
      @close="isCreateCourseModalOpen = false"
      @course-created="handleCourseCreated"
    />

    <CreateSessionModal 
      :is-open="isCreateSessionModalOpen" 
      @close="isCreateSessionModalOpen = false"
      @session-created="handleSessionCreated"
    />
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '@/shared/composables/useAuth'
import { useToast } from '@/shared/composables/useToast'
import { useRealtimeNotifications } from '@/shared/composables/useRealtimeNotifications'
import BaseButton from '@/core/ui/BaseButton.vue'
import BaseModal from '@/core/ui/BaseModal.vue'

// Services
import { courseService } from '@/services/courseService'
import { attendanceService } from '@/services/attendanceService'

// Feature Components
import CreateCourseModal from './CreateCourseModal.vue'
import CreateSessionModal from './CreateSessionModal.vue'
import DashboardStats from './DashboardStats.vue'
import CourseGrid from './CourseGrid.vue'
import LiveRosterRealtime from './LiveRosterRealtime.vue'
import RecentActivity from './RecentActivity.vue'

// Cross-Feature Components
import ScannerInterface from '@/features/scanner/ScannerInterface.vue'

const router = useRouter()
const { user } = useAuth()
const { toast } = useToast()
const { initializeNotifications, showSessionStartNotification, showSessionEndNotification } = useRealtimeNotifications()

const lecturerName = ref('')
const isLoading = ref(true)
const stats = ref({
  totalCourses: 0,
  totalStudents: 0,
  todaySessions: 0,
  avgAttendance: 0
})
const courses = ref([])
const recentSessions = ref([])
const activeRoster = ref([])
const activeSessionId = ref(null)
const activeSessionName = ref('')
const isEndingSession = ref(false)
const isScannerOpen = ref(false)
const lastScanned = ref('')
const scanStatus = ref('success')
const scannedCount = ref(0)
const isCreateCourseModalOpen = ref(false)
const isCreateSessionModalOpen = ref(false)

const presentCount = computed(() => activeRoster.value.filter(s => s.present).length)

const showCreateCourse = () => { isCreateCourseModalOpen.value = true }
const showCreateSession = () => { isCreateSessionModalOpen.value = true }
const handleCourseCreated = async () => { await fetchLecturerData() }
const handleSessionCreated = async (session) => {
  activeSessionId.value = session.id
  activeSessionName.value = session.name
  
  const activeCourse = courses.value.find(c => c.id === session.course_id)
  showSessionStartNotification(session.name, activeCourse ? activeCourse.name : 'Course')
  
  await fetchLecturerData()
}
const showBarcodeScanner = () => { isScannerOpen.value = true }

const closeBarcodeScanner = () => {
  isScannerOpen.value = false
  lastScanned.value = ''
  scannedCount.value = 0
}

const completeSession = async () => {
  if (!activeSessionId.value) return
  
  if (!confirm('Are you sure you want to end this session?')) {
    return
  }

  isEndingSession.value = true
  try {
    toast.success('Session completed successfully')
    
    showSessionEndNotification(activeSessionName.value, 'Course', { 
      present: presentCount.value, 
      absent: activeRoster.value.length - presentCount.value 
    })

    activeSessionId.value = null
    activeSessionName.value = ''
    activeRoster.value = []
    
    await fetchLecturerData()
  } catch (err) {
    console.error('Error completing session:', err)
    toast.error('Failed to complete session')
  } finally {
    isEndingSession.value = false
  }
}

const handleBarcodeDetected = async (barcode) => {
  try {
    if (lastScanned.value.includes(barcode.trim())) return
    
    if (!activeSessionId.value) {
      lastScanned.value = 'No session found'
      scanStatus.value = 'error'
      toast.error('No session found. Please create a session first.')
      return
    }

    const { success, error, studentName, alreadyMarked } = await attendanceService.markByBarcode(barcode, activeSessionId.value)

    if (!success) {
      lastScanned.value = error
      scanStatus.value = 'error'
      toast.error(error)
      return
    }

    if (alreadyMarked) return

    lastScanned.value = studentName
    scanStatus.value = 'success'
    scannedCount.value++
    toast.success(`${studentName} marked as present!`)
    
    const index = activeRoster.value.findIndex(s => s.full_name === studentName)
    if (index !== -1) {
      activeRoster.value[index].present = true
    } else {
      activeRoster.value.push({
        full_name: studentName,
        present: true,
        isGuest: true
      })
    }

    setTimeout(() => { lastScanned.value = '' }, 3000)
  } catch (err) {
    console.error('Barcode scan error:', err)
    toast.error('Failed to record attendance.')
  }
}

const markAsPresent = async (studentUUID) => {
  try {
    const { alreadyMarked } = await attendanceService.markPresent(studentUUID, activeSessionId.value)
    if (alreadyMarked) {
      toast.info('Already marked present.')
    } else {
      toast.success('Attendance recorded.')
      const index = activeRoster.value.findIndex(s => s.id === studentUUID)
      if (index !== -1) activeRoster.value[index].present = true
    }
  } catch (err) {
    console.error(err)
    toast.error('Failed to mark as present.')
  }
}

const fetchLecturerData = async () => {
  try {
    if (!user.value) return
    isLoading.value = true

    const { lecturerName: name, sections } = await courseService.getLecturerDashboardData(user.value.id)
    lecturerName.value = name || user.value.user_metadata?.full_name || user.value.email.split('@')[0]
    
    courses.value = sections
    stats.value.totalCourses = sections.length
    stats.value.totalStudents = sections.reduce((sum, s) => sum + s.student_count, 0)
    
    if (sections.length > 0) {
      stats.value.avgAttendance = Math.round(sections.reduce((sum, s) => sum + s.attendance_rate, 0) / sections.length)
    }

    const sectionIds = sections.map(s => s.id)
    if (sectionIds.length > 0) {
      recentSessions.value = await attendanceService.getRecentActivity(sectionIds)
      
      stats.value.todaySessions = recentSessions.value.filter(l => 
        new Date(l.session_date).toDateString() === new Date().toDateString()
      ).length
    }

    // Active Session Management
    let activeSection = null
    if (activeSessionId.value) {
       activeSection = sections.find(s => s.id === activeSessionId.value) || sections[0]
    } else {
       activeSection = sections[0]
    }
    
    if (activeSection) {
      activeSessionId.value = activeSection.id
      activeSessionName.value = activeSection.course_name || activeSection.name
      activeRoster.value = await courseService.getSectionRoster(activeSection.id)
    }
  } catch (error) {
    console.error('Error fetching dashboard data:', error)
    toast.error('Could not load dashboard data.')
  } finally {
    isLoading.value = false
  }
}

const manageCourse = (id) => router.push(`/course/${id}`)
const viewAllCourses = () => router.push('/courses')

onMounted(() => {
  initializeNotifications()
  fetchLecturerData()
})
</script>

<style scoped>
.lecturer-dashboard {
  min-height: 100vh;
  background-color: transparent;
  font-family: 'Inter', sans-serif;
}

.main-content {
  padding: 6rem 0 5rem 0;
}

.dashboard-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 1.5rem;
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

/* === STREAMLINED HEADER === */
.pro-dashboard-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
  padding: 1.5rem 0;
  margin-bottom: 0.5rem;
}

.header-left .pro-badge {
  display: inline-block;
  background: var(--primary-glow);
  color: var(--primary);
  padding: 0.25rem 0.75rem;
  border-radius: 100px;
  font-size: 0.7rem;
  font-weight: 800;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  margin-bottom: 0.75rem;
  border: 1px solid var(--primary);
}

.header-left h1 {
  font-size: 2rem;
  font-weight: 850;
  color: var(--text-main);
  margin: 0 0 0.5rem 0;
  letter-spacing: -0.02em;
}

.highlight-text {
  color: var(--primary);
}

.session-summary {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.9rem;
  color: var(--text-muted);
}

.session-summary strong {
  color: var(--text-main);
}

.quick-actions {
  display: flex;
  gap: 0.75rem;
  align-items: center;
}

.action-btn-secondary {
  background: var(--bg-card);
  border: 1px solid var(--border-medium);
  color: var(--text-main);
  padding: 0.6rem 1rem;
  border-radius: 12px;
  font-weight: 700;
  font-size: 0.85rem;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  transition: all 0.2s;
}

.action-btn-secondary:hover {
  background: var(--bg-main);
  border-color: var(--primary);
}

.scan-cta {
  font-weight: 800;
  letter-spacing: -0.01em;
}

/* === LAYOUT === */
.dashboard-grid {
  display: grid;
  grid-template-columns: 1fr 340px;
  gap: 2rem;
  align-items: start;
}

.main-col {
  display: flex;
  flex-direction: column;
}

.sticky-side {
  position: sticky;
  top: 6rem;
}

.mt-6 { margin-top: 1.5rem; }
.mt-8 { margin-top: 2rem; }

/* === MEDIA QUERIES === */
@media (max-width: 1024px) {
  .dashboard-grid {
    grid-template-columns: 1fr;
  }
  
  .sticky-side {
    position: static;
  }
}

@media (max-width: 768px) {
  .pro-dashboard-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 1.5rem;
  }
  
  .quick-actions {
    width: 100%;
  }
  
  .action-btn-secondary, .scan-cta {
    flex: 1;
    justify-content: center;
  }
}
</style>
