<template>
  <div class="lecturer-dashboard">
    
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

        <!-- Stats -->
        <DashboardStats :stats="stats" />

        <!-- Course Attendance Chart (New) -->
        <section class="chart-section" v-if="courses.length > 0">
          <Card class="chart-card">
            <div class="chart-header">
              <h3>Course Attendance Overview</h3>
            </div>
            <div class="chart-container">
              <BarChart :chart-data="courseChartData" />
            </div>
          </Card>
        </section>

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
import Button from '@/components/ui/Button.vue'
import Modal from '@/components/ui/Modal.vue'
import CreateCourseModal from '@/components/lecturer/CreateCourseModal.vue'
import CreateSessionModal from '@/components/lecturer/CreateSessionModal.vue'
import DashboardStats from '@/components/lecturer/DashboardStats.vue'
import CourseGrid from '@/components/lecturer/CourseGrid.vue'
import LiveRosterRealtime from '@/components/lecturer/LiveRosterRealtime.vue'
import ScannerInterface from '@/components/lecturer/ScannerInterface.vue'
import BarChart from '@/components/ui/charts/BarChart.vue'
import Card from '@/components/ui/Card.vue'

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

const courseChartData = computed(() => {
  return {
    labels: courses.value.map(c => c.course_name || 'Unnamed Course'),
    datasets: [
      {
        label: 'Avg. Attendance %',
        data: courses.value.map(c => c.attendance_rate || 0),
        backgroundColor: ['#3b82f6', '#10b981', '#f59e0b', '#8b5cf6', '#ec4899', '#f43f5e', '#14b8a6', '#6366f1'],
        borderRadius: 6
      }
    ]
  }
})

const presentCount = computed(() => activeRoster.value.filter(s => s.present).length)

const showCreateCourse = () => { isCreateCourseModalOpen.value = true }
const showCreateSession = () => { isCreateSessionModalOpen.value = true }
const handleCourseCreated = async () => { await fetchLecturerData() }
const handleSessionCreated = async (session) => {
  activeSessionId.value = session.id
  activeSessionName.value = session.name
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
    // 1. Get Lecturer's Sections
    const { data: mySections } = await supabase
      .from('sections')
      .select('id, course_id')
      .eq('lecturer_id', user.value.id)
      
    if (!mySections?.length) return toast.error('No sections found')

    let foundActiveSession = null

    // 2. Find any recent attendance log entry to identify "active" session
    for (const s of mySections) {
      const { data: log } = await supabase
        .from('attendance_logs')
        .select('id, session_date')
        .eq('section_id', s.id)
        .order('session_date', { ascending: false })
        .limit(1)
        .maybeSingle()
      
      if (log) {
        foundActiveSession = { ...log, section_id: s.id }
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
      .select('id, full_name') // Fetch UUID and Name
      .eq('student_number', studentId) // Query by the barcode string
      .maybeSingle()

    if (!student) {
      lastScanned.value = `ID ${studentId} not found`
      scanStatus.value = 'error'
      toast.error(`Student ID ${studentId} not found in system`)
      return
    }

    await supabase.from('attendance_logs').insert({
      section_id: activeSession.section_id || activeSession.id, 
      student_id: student.id, // Insert the UUID primary key into the logs
      status: 'Present',
      session_date: new Date().toISOString()
    })

    lastScanned.value = student.full_name
    scanStatus.value = 'success'
    scannedCount.value++
    
    toast.success(`${student.full_name} marked as present!`)
    
    // Update local roster if student is in it
    const index = activeRoster.value.findIndex(s => s.student_id === student.id)
    if (index !== -1) activeRoster.value[index].present = true

    setTimeout(() => { lastScanned.value = '' }, 3000)
  } catch (error) {
    console.error('Barcode scan error:', error)
    toast.error('Failed to record attendance. Please try again.')
  }
}

const markAsPresent = async (studentUUID) => {
  if (!activeSessionId.value) {
    toast.error('No active session selected.')
    return
  }

  try {
    const { error: insertError } = await supabase
      .from('attendance_logs')
      .insert({
        student_id: studentUUID,
        section_id: activeSessionId.value,
        status: 'Present',
        session_date: new Date().toISOString()
      })

    if (insertError) {
       if (insertError.code === '23505') {
         toast.info('Student already marked present.')
       } else {
         throw insertError
       }
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

    // ---------------------------------------------------------
    // Refactored Data Fetching (Defensive N+1 to fix 400 Errors)
    // ---------------------------------------------------------

    // 1. Fetch Lecturer Profile
    const { data: lecturer, error: profileError } = await supabase
      .from('lecturers')
      .select('full_name')
      .eq('id', user.value.id)
      .maybeSingle()

    if (lecturer) {
      lecturerName.value = lecturer.full_name
    } else {
      lecturerName.value = user.value.user_metadata?.full_name || user.value.email.split('@')[0]
    }

    // 2. Fetch ALL Sections for this lecturer
    const { data: mySections } = await supabase
      .from('sections')
      .select('id, name, course_id, courses(name), enrollments(count)')
      .eq('lecturer_id', user.value.id)

    if (!mySections || mySections.length === 0) {
      isLoading.value = false
      return
    }

    // 3. Parallel fetch for deep stats
    const sectionsWithStats = await Promise.all(mySections.map(async (s) => {
      // Get attendance logs for this section
      const { count: attendanceCount } = await supabase
        .from('attendance_logs')
        .select('*', { count: 'exact', head: true })
        .eq('section_id', s.id)
      
      const studentCount = s.enrollments?.[0]?.count || 0
      
      // Calculate attendance rate (rough estimate since we don't have explicit sessions anymore)
      // In a real app, you'd divide by (studentCount * number_of_class_days)
      const attendanceRate = studentCount > 0 ? Math.round((attendanceCount / (studentCount * 10)) * 100) : 0 // Assuming 10 days for now
      
      return {
        ...s,
        course_name: s.courses?.name,
        student_count: studentCount,
        attendance_rate: Math.min(attendanceRate, 100)
      }
    }))
    
    courses.value = sectionsWithStats // UI still uses 'courses' variable name
    stats.value.totalCourses = sectionsWithStats.length
    stats.value.totalStudents = sectionsWithStats.reduce((sum, s) => sum + s.student_count, 0)
    
    // Calculate Avg Attendance
    if (sectionsWithStats.length > 0) {
      stats.value.avgAttendance = Math.round(sectionsWithStats.reduce((sum, s) => sum + s.attendance_rate, 0) / sectionsWithStats.length)
    }

    // 4. Fetch Recent Attendance (Last 8 logs)
    const { data: recentLogs } = await supabase
      .from('attendance_logs')
      .select('*, students(full_name), sections(name, courses(name))')
      .in('section_id', mySections.map(s => s.id))
      .order('session_date', { ascending: false })
      .limit(8)
    
    recentSessions.value = recentLogs?.map(l => ({
      ...l,
      course_name: l.sections?.courses?.name,
      student_name: l.students?.full_name,
      session_date: l.session_date,
      status: 'completed'
    })) || []
    
    stats.value.todaySessions = recentLogs?.filter(l => 
      new Date(l.session_date).toDateString() === new Date().toDateString()
    ).length || 0

    // 5. Select Active Section
    // If we already have one selected via handleSessionCreated, keep it.
    // Otherwise, pick the first one as default.
    let activeSection = null
    if (activeSessionId.value) {
       activeSection = mySections.find(s => s.id === activeSessionId.value) || mySections[0]
    } else {
       activeSection = mySections[0]
    }
    
    if (activeSection) {
      activeSessionId.value = activeSection.id
      activeSessionName.value = activeSection.courses?.name || activeSection.name
      
      const { data: enrolledStudents } = await supabase
        .from('enrollments')
        .select('student_id, students(id, full_name, student_number)')
        .eq('section_id', activeSection.id)
      
      const { data: attendanceData } = await supabase
        .from('attendance_logs')
        .select('student_id')
        .eq('section_id', activeSection.id)
        .gte('session_date', new Date().toISOString().split('T')[0]) // Today

      const attendedIds = new Set(attendanceData?.map(a => a.student_id) || [])

      activeRoster.value = enrolledStudents.map(e => ({
        id: e.student_id,
        student_id: e.students?.student_number || e.student_id,
        student_number: e.students?.student_number,
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

/* Chart Section */
.chart-section {
  margin-bottom: 2.5rem;
}

.chart-card {
  padding: 1.5rem;
  border-radius: 20px;
}

.chart-header h3 {
  font-size: 1.125rem;
  font-weight: 800;
  margin-bottom: 1.5rem;
}

.chart-container {
  height: 250px;
  position: relative;
  width: 100%;
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
