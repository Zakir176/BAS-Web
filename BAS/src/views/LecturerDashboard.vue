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
          @complete="completeSession"
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
    // Since sessions table doesn't have is_completed column, we'll just clear the local state
    // In a real implementation, you might want to add an is_completed column to the sessions table
    
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
    
    // ---------------------------------------------------------
    // Find Active Session - Fixed: Remove is_completed filter
    // ---------------------------------------------------------
    // 1. Get Teacher's Courses
    const { data: myCourses } = await supabase
      .from('courses')
      .select('course_id')
      .eq('teacher_id', user.value.id)
      
    if (!myCourses?.length) return toast.error('No courses found')

    let foundActiveSession = null

    // 2. Find the most recent session (since we don't have is_completed)
    for (const c of myCourses) {
      const { data: session } = await supabase
        .from('sessions')
        .select('session_id, session_date, session_time')
        .eq('course_id', c.course_id)
        .order('created_at', { ascending: false })
        .limit(1)
        .maybeSingle()
      
      if (session) {
        foundActiveSession = session
        break
      }
    }

    if (!foundActiveSession) {
      lastScanned.value = 'No session found'
      scanStatus.value = 'error'
      toast.error('No session found. Please create a session first.')
      return
    }

    const activeSession = foundActiveSession

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
    // 1. Get Teacher's Courses
    const { data: myCourses } = await supabase
      .from('courses')
      .select('course_id')
      .eq('teacher_id', user.value.id)
      
    if (!myCourses?.length) return
    
    let activeSession = null
    for (const c of myCourses) {
       const { data: s } = await supabase
         .from('sessions')
         .select('session_id')
         .eq('course_id', c.course_id)
         .order('created_at', { ascending: false })
         .limit(1)
         .maybeSingle()
       
       if (s) {
         activeSession = s
         break
       }
    }

    if (!activeSession) {
      toast.error('No session found')
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
    toast.error('Failed to mark as present.')
  }
}

const fetchLecturerData = async () => {
  try {
    if (!user.value) return
    isLoading.value = true

    // ---------------------------------------------------------
    // Refactored Data Fetching (Defensive N+1 to fix 400 Errors)
    // ---------------------------------------------------------

    // 1. Fetch Lecturer Profile
    const { data: lecturer, error: profileError } = await supabase
      .from('teachers')
      .select('full_name')
      .eq('teacher_id', user.value.id)
      .maybeSingle()

    if (lecturer) {
      lecturerName.value = lecturer.full_name
    } else {
      lecturerName.value = user.value.user_metadata?.full_name || user.value.email.split('@')[0]
    }

    // 2. Fetch ALL Courses for this teacher
    const { data: myCourses } = await supabase
      .from('courses')
      .select('course_id, course_name, enrollments(count)')
      .eq('teacher_id', user.value.id)

    if (!myCourses || myCourses.length === 0) {
      isLoading.value = false
      return
    }

    // 3. Parallel fetch for deep stats to avoid complex joins
    const coursesWithStats = await Promise.all(myCourses.map(async (c) => {
      // Get all sessions for this course
      const { data: cSessions } = await supabase
        .from('sessions')
        .select('session_id')
        .eq('course_id', c.course_id)
      
      const sessionIds = cSessions?.map(s => s.session_id) || []
      const studentCount = c.enrollments?.[0]?.count || 0
      
      let attendanceRate = 0
      if (studentCount > 0 && sessionIds.length > 0) {
        // We have to batch this if too many, but for now strict IN query
        // If this fails with 400, we know the session_id type matches
        const { count: attendanceCount } = await supabase
          .from('attendance')
          .select('*', { count: 'exact', head: true })
          .in('session_id', sessionIds)
        
        attendanceRate = Math.round((attendanceCount / (studentCount * sessionIds.length)) * 100)
      }

      return {
        ...c,
        sessions: cSessions || [], // Store for later usage
        student_count: studentCount,
        attendance_rate: attendanceRate
      }
    }))
    
    courses.value = coursesWithStats
    stats.value.totalCourses = coursesWithStats.length
    stats.value.totalStudents = coursesWithStats.reduce((s, c) => s + c.student_count, 0)
    
    // Calculate Avg Attendance
    const totalPossible = coursesWithStats.reduce((s, c) => s + (c.student_count * c.sessions.length), 0)
    if (totalPossible > 0) {
       // Re-sum based on individual rates? Or do a big query?
       // Let's do a weighted average of the rates we already calculated to save a query
       const weightedSum = coursesWithStats.reduce((s, c) => s + (c.attendance_rate * (c.student_count * c.sessions.length)), 0)
       stats.value.avgAttendance = Math.round(weightedSum / totalPossible)
    } else {
      stats.value.avgAttendance = 0
    }

    // 4. Fetch Recent Sessions (Manually Aggregated from Courses)
    // We already have sessions in 'coursesWithStats', just need details for them
    // But we need 'is_completed' etc.
    // Let's re-query recent sessions per course and merge sort
    const allRecentSessions = await Promise.all(myCourses.map(async (c) => {
       const { data: sData } = await supabase
         .from('sessions')
         .select('*, attendance(count)')
         .eq('course_id', c.course_id)
         .order('session_date', { ascending: false })
         .limit(3) // Get top 3 per course to be safe
       
       return sData?.map(s => ({
         ...s,
         course_name: c.course_name,
         student_count: s.attendance?.[0]?.count || 0,
         status: s.is_completed ? 'completed' : (new Date(s.session_date) > new Date() ? 'upcoming' : 'active')
       })) || []
    }))

    // Flatten and sort by date desc
    const flattenedSessions = allRecentSessions.flat().sort((a, b) => new Date(b.session_date) - new Date(a.session_date))
    recentSessions.value = flattenedSessions.slice(0, 8)
    stats.value.todaySessions = flattenedSessions.filter(s => s.session_date === new Date().toISOString().split('T')[0]).length

    // 5. Find Active Session (Latest Non-Completed)
    // We intentionally iterate because .in() was 400-ing
    let foundActiveSession = null
    
    // Check each course for an active session
    // Optimization: flattenedSessions might ALREADY have it if it's recent
    const potentialActive = flattenedSessions.find(s => !s.is_completed)
    
    if (potentialActive) {
      foundActiveSession = potentialActive
    } else {
      // Deep search if not in recent
      for (const course of myCourses) {
        const { data: deepSession } = await supabase
          .from('sessions')
          .select('*')
          .eq('course_id', course.course_id)
          .eq('is_completed', false)
          .order('created_at', { ascending: false })
          .limit(1)
          .maybeSingle()
          
        if (deepSession) {
          // Found one!
          foundActiveSession = { ...deepSession, course_name: course.course_name }
          break // Assume only 1 active session at a time is managed
        }
      }
    }

    if (foundActiveSession) {
      activeSessionId.value = foundActiveSession.session_id
      activeSessionName.value = foundActiveSession.course_name || 'Active Session'
      
      const { data: enrolledStudents } = await supabase
        .from('enrollments')
        .select('student_id, students(full_name)')
        .eq('course_id', foundActiveSession.course_id)
      
      const { data: attendanceData } = await supabase
        .from('attendance')
        .select('student_id')
        .eq('session_id', foundActiveSession.session_id)

      const attendedIds = new Set(attendanceData?.map(a => a.student_id) || [])

      activeRoster.value = enrolledStudents.map(e => ({
        student_id: e.student_id,
        full_name: e.students?.full_name,
        present: attendedIds.has(e.student_id)
      }))
    } else {
       activeSessionId.value = null
       activeSessionName.value = ''
       activeRoster.value = []
    }
  } catch (error) {
    console.error('Error fetching dashboard data:', error)
    toast.error('Could not load dashboard data. Please try again.')
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
@media (max-width: 1024px) {
  .dashboard-header {
    flex-direction: column;
    gap: 1.5rem;
    text-align: center;
  }
  
  .header-right {
    flex-wrap: wrap;
    justify-content: center;
    gap: 0.75rem;
  }
  
  .action-btn, .scan-btn {
    flex: 1;
    min-width: 140px;
  }
  
  .dashboard-layout {
    grid-template-columns: 1fr;
    gap: 2rem;
  }
}

@media (max-width: 768px) {
  .dashboard-header {
    padding: 1.5rem 1rem;
    gap: 1rem;
  }
  
  .header-left h1 {
    font-size: 1.5rem;
  }
  
  .header-left p {
    font-size: 0.875rem;
  }
  
  .header-right {
    flex-direction: column;
    gap: 0.75rem;
  }
  
  .action-btn, .scan-btn {
    width: 100%;
    justify-content: center;
    padding: 0.875rem 1rem;
  }
  
  .container {
    padding: 0 1rem;
  }
}

@media (max-width: 640px) {
  .dashboard-header {
    padding: 1rem 0.75rem;
  }
  
  .header-left h1 {
    font-size: 1.25rem;
    line-height: 1.3;
  }
  
  .header-left p {
    font-size: 0.8rem;
  }
  
  .header-right {
    gap: 0.5rem;
  }
  
  .action-btn, .scan-btn {
    padding: 0.75rem 0.875rem;
    font-size: 0.875rem;
  }
  
  .container {
    padding: 0 0.75rem;
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
