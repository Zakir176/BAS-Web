<template>
  <div class="lecturer-dashboard">
    <Navbar />
    
    <main class="main-content">
      <div class="container">
        <!-- Welcome Header -->
        <header class="dashboard-header">
          <div class="header-left">
            <h1>Welcome back, <span class="highlight">{{ lecturerName }}</span></h1>
            <p>You have {{ stats.todaySessions }} sessions scheduled for today</p>
          </div>
          <div class="header-right">
            <Button variant="secondary" @click="showCreateCourse" class="action-btn">
              <span class="icon">📘</span> New Course
            </Button>
            <Button variant="primary" @click="showCreateSession" class="action-btn">
              <span class="icon">➕</span> New Session
            </Button>
            <Button variant="success" @click="showBarcodeScanner" class="scan-btn">
              <span class="icon">📷</span> Start Scanning
            </Button>
          </div>
        </header>

        <!-- Stats Grid -->
        <DashboardStats :stats="stats" />

        <!-- Main Content Area -->
        <div class="dashboard-layout">
          <CourseGrid 
            :courses="courses"
            :is-loading="isLoading"
            @view-all="viewAllCourses"
            @manage="manageCourse"
            @create="showCreateCourse"
          />
        </div>

        <!-- Class Roster Section -->
        <LiveRoster 
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
    </main>

    <!-- Scan Barcode Modal (Full Screen Overlay Style) -->
    <Modal :is-open="isScannerOpen" @close="closeBarcodeScanner" class="scanner-modal-fullscreen">
      <template #default>
        <ScannerInterface 
          :session-name="activeSessionName"
          :scanned-count="scannedCount"
          :total-students="stats.totalStudents"
          :last-scanned="lastScanned"
          :scan-status="scanStatus"
          @close="closeBarcodeScanner"
          @detected="handleBarcodeDetected"
        />
      </template>
    </Modal>

    <!-- Create Course Modal -->
    <CreateCourseModal 
      :is-open="isCreateCourseModalOpen" 
      @close="isCreateCourseModalOpen = false"
      @course-created="handleCourseCreated"
    />

    <!-- Create Session Modal -->
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
import { useAuth } from '@/composables/useAuth'
import { useToast } from '@/composables/useToast'
import { supabase } from '@/supabase'
import Navbar from '@/components/common/Navbar.vue'
import Button from '@/components/ui/Button.vue'
import Modal from '@/components/ui/Modal.vue'
import CreateCourseModal from '@/components/lecturer/CreateCourseModal.vue'
import CreateSessionModal from '@/components/lecturer/CreateSessionModal.vue'
import DashboardStats from '@/components/lecturer/DashboardStats.vue'
import CourseGrid from '@/components/lecturer/CourseGrid.vue'
import LiveRoster from '@/components/lecturer/LiveRoster.vue'
import ScannerInterface from '@/components/lecturer/ScannerInterface.vue'

const router = useRouter()
const { user } = useAuth()
const { toast } = useToast()

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
const handleSessionCreated = async () => { await fetchLecturerData() }
const showBarcodeScanner = () => { isScannerOpen.value = true }

const closeBarcodeScanner = () => {
  isScannerOpen.value = false
  lastScanned.value = ''
  scannedCount.value = 0
}

const completeSession = async () => {
  if (!activeSessionId.value) return
  
  if (!confirm('Are you sure you want to end this session? This will clear the active roster.')) {
    return
  }

  isEndingSession.value = true
  try {
    const { error } = await supabase
      .from('sessions')
      .update({ is_completed: true })
      .eq('session_id', activeSessionId.value)

    if (error) throw error

    toast.success('Session completed successfully')
    // Reset active session state
    activeSessionId.value = null
    activeSessionName.value = ''
    activeRoster.value = []
    
    // Refresh dashboard stats
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
    const studentId = barcode.trim()
    if (lastScanned.value.includes(studentId)) return

    const { data: activeSession } = await supabase
      .from('sessions')
      .select('session_id, courses!inner(teacher_id)')
      .eq('courses.teacher_id', user.value.id)
      .eq('is_completed', false)
      .order('created_at', { ascending: false })
      .limit(1)
      .maybeSingle()

    if (!activeSession) {
      lastScanned.value = 'No active session'
      scanStatus.value = 'error'
      toast.error('No active session found. Please start a session first.')
      return
    }

    const { data: student } = await supabase
      .from('students')
      .select('full_name')
      .eq('student_id', studentId)
      .maybeSingle()

    if (!student) {
      lastScanned.value = `ID ${studentId} not found`
      scanStatus.value = 'error'
      toast.error(`Student ID ${studentId} not found in system`)
      return
    }

    await supabase.from('attendance').upsert({
      session_id: activeSession.session_id,
      student_id: studentId,
      status: 'Present',
      method: 'Barcode',
      timestamp: new Date().toISOString()
    })

    lastScanned.value = student.full_name
    scanStatus.value = 'success'
    scannedCount.value++
    
    toast.success(`${student.full_name} marked as present!`)
    
    // Update local roster if student is in it
    const index = activeRoster.value.findIndex(s => s.student_id === studentId)
    if (index !== -1) activeRoster.value[index].present = true

    setTimeout(() => { lastScanned.value = '' }, 3000)
  } catch (error) {
    console.error('Barcode scan error:', error)
    toast.error('Failed to record attendance. Please try again.')
  }
}

const markAsPresent = async (studentId) => {
  try {
    const { data: activeSession } = await supabase
      .from('sessions')
      .select('session_id, courses!inner(teacher_id)')
      .eq('courses.teacher_id', user.value.id)
      .eq('is_completed', false)
      .order('created_at', { ascending: false })
      .limit(1)
      .maybeSingle()

    if (!activeSession) {
      toast.error('No active session found')
      return
    }

    await supabase.from('attendance').upsert({
      session_id: activeSession.session_id,
      student_id: studentId,
      status: 'Present',
      method: 'Manual',
      timestamp: new Date().toISOString()
    })

    const index = activeRoster.value.findIndex(s => s.student_id === studentId)
    if (index !== -1) activeRoster.value[index].present = true
  } catch (err) {
    console.error(err)
  }
}

const fetchLecturerData = async () => {
  try {
    if (!user.value) return
    isLoading.value = true

    // Fetch lecturer profile - Use UID for more reliable matching
    const { data: lecturer, error: profileError } = await supabase
      .from('teachers')
      .select('full_name')
      .eq('teacher_id', user.value.id)
      .maybeSingle()

    if (profileError) {
      console.warn('Dashboard: Could not fetch profile:', profileError)
    }

    if (lecturer) {
      lecturerName.value = lecturer.full_name
    } else {
      // Use fallback from metadata
      lecturerName.value = user.value.user_metadata?.full_name || user.value.email.split('@')[0]
    }

    // Fetch courses with counts
    const { data: coursesData } = await supabase
      .from('courses')
      .select('*, enrollments(count), sessions(session_id)')
      .eq('teacher_id', user.value.id)

    if (coursesData) {
      courses.value = await Promise.all(coursesData.map(async (c) => {
        const studentCount = c.enrollments?.[0]?.count || 0
        const sessionIds = c.sessions?.map(s => s.session_id) || []
        
        let attendanceRate = 0
        if (studentCount > 0 && sessionIds.length > 0) {
          const { count: attendanceCount } = await supabase
            .from('attendance')
            .select('*', { count: 'exact', head: true })
            .in('session_id', sessionIds)
          
          attendanceRate = Math.round((attendanceCount / (studentCount * sessionIds.length)) * 100)
        }

        return {
          ...c,
          student_count: studentCount,
          attendance_rate: attendanceRate
        }
      }))
      
      stats.value.totalCourses = coursesData.length
      stats.value.totalStudents = courses.value.reduce((s, c) => s + c.student_count, 0)
      
      const totalPossible = courses.value.reduce((s, c) => s + (c.student_count * (c.sessions?.length || 0)), 0)
      if (totalPossible > 0) {
        const { count: totalAttendance } = await supabase
          .from('attendance')
          .select('*', { count: 'exact', head: true })
        stats.value.avgAttendance = Math.round((totalAttendance / totalPossible) * 100)
      } else {
        stats.value.avgAttendance = 0
      }
    }

    // Fetch recent sessions - Need to filter through courses to get correct teacher's sessions
    const { data: sessionsData } = await supabase
      .from('sessions')
      .select('*, courses!inner(course_name, teacher_id), attendance(count)')
      .eq('courses.teacher_id', user.value.id)
      .order('session_date', { ascending: false })
      .limit(8)

    if (sessionsData) {
      recentSessions.value = sessionsData.map(s => ({
        ...s,
        course_name: s.courses?.course_name,
        student_count: s.attendance?.[0]?.count || 0,
        status: s.is_completed ? 'completed' : (new Date(s.session_date) > new Date() ? 'upcoming' : 'active')
      }))
      stats.value.todaySessions = sessionsData.filter(s => s.session_date === new Date().toISOString().split('T')[0]).length
    }

    // Fetch roster for most recent session - Join courses to filter by teacher_id
    const { data: latestSession } = await supabase
      .from('sessions')
      .select('*, courses!inner(course_name, teacher_id)')
      .eq('courses.teacher_id', user.value.id)
      .eq('is_completed', false) // Only show non-completed sessions in live roster
      .order('created_at', { ascending: false })
      .limit(1)
      .maybeSingle()

    if (latestSession) {
      activeSessionId.value = latestSession.session_id
      activeSessionName.value = latestSession.courses?.course_name
      const { data: enrolledStudents } = await supabase
        .from('enrollments')
        .select('student_id, students(full_name)')
        .eq('course_id', latestSession.course_id)
      
      const { data: attendanceData } = await supabase
        .from('attendance')
        .select('student_id')
        .eq('session_id', latestSession.session_id)

      const attendedIds = new Set(attendanceData?.map(a => a.student_id) || [])

      activeRoster.value = enrolledStudents.map(e => ({
        student_id: e.student_id,
        full_name: e.students?.full_name,
        present: attendedIds.has(e.student_id)
      }))
    }
  } catch (error) {
    console.error('Error fetching dashboard data:', error)
  } finally {
    isLoading.value = false
  }
}

const formatDate = (d) => new Date(d).toLocaleDateString()
const manageCourse = (id) => router.push(`/course/${id}`)
const viewAllCourses = () => router.push('/courses')

onMounted(fetchLecturerData)
</script>

<style scoped>
.lecturer-dashboard {
  min-height: 100vh;
  background-color: var(--bg-main);
}

.main-content {
  padding: 2.5rem 0;
}

.dashboard-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2.5rem;
}

.header-left h1 {
  font-size: 2rem;
  font-weight: 800;
  color: var(--text-main);
  margin-bottom: 0.25rem;
}

.highlight { color: var(--primary); }

.header-left p {
  color: var(--text-muted);
  font-weight: 500;
}

.header-right {
  display: flex;
  gap: 1rem;
}

/* Mobile responsive - adapt grid layout */
@media (max-width: 768px) {
  .dashboard-header {
    flex-direction: column;
    gap: 1.5rem;
    text-align: center;
  }
  
  .header-right {
    flex-direction: column;
    gap: 0.75rem;
  }
  
  .action-btn {
    width: 100%;
    justify-content: center;
  }
  
  .scan-btn {
    width: 100%;
    justify-content: center;
  }
}

/* Layout */
.dashboard-layout {
  display: grid;
  grid-template-columns: 1fr 340px;
  gap: 2.5rem;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.section-header h2 {
  font-size: 1.25rem;
  font-weight: 800;
}

.text-link {
  background: none;
  border: none;
  color: var(--primary);
  font-weight: 700;
  cursor: pointer;
}

/* Timeline */
.sessions-list-compact {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.session-item-compact {
  padding: 1rem;
  background: var(--bg-card);
  border-radius: 16px;
  display: flex;
  align-items: center;
  gap: 1rem;
}

.session-meta {
  width: 60px;
  font-size: 0.75rem;
  font-weight: 700;
  color: var(--text-muted);
  text-align: center;
}

.session-info h4 { font-size: 0.9rem; font-weight: 800; }
.session-info p { font-size: 0.8rem; color: var(--text-muted); }

.session-status {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  margin-left: auto;
}

.session-status.completed { background: #cbd5e1; }
.session-status.upcoming { background: #3b82f6; }
</style>
