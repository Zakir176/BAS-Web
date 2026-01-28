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
        <section class="stats-overview">
          <div class="stats-grid">
            <div class="stat-card">
              <div class="stat-main">
                <span class="stat-label">TOTAL COURSES</span>
                <div class="stat-value">{{ stats.totalCourses }}</div>
              </div>
              <div class="stat-icon courses">🎓</div>
            </div>
            <div class="stat-card">
              <div class="stat-main">
                <span class="stat-label">ENROLLED STUDENTS</span>
                <div class="stat-value">{{ stats.totalStudents }}</div>
              </div>
              <div class="stat-icon students">👥</div>
            </div>
            <div class="stat-card">
              <div class="stat-main">
                <span class="stat-label">AVG. ATTENDANCE</span>
                <div class="stat-value">{{ stats.avgAttendance }}%</div>
              </div>
              <div class="stat-icon attendance">📈</div>
            </div>
          </div>
        </section>

        <!-- Main Content Area -->
        <div class="dashboard-layout">
          <!-- Courses Grid -->
          <section class="content-section">
            <div class="section-header">
              <h2>My Courses</h2>
              <button class="text-link" @click="viewAllCourses">See all</button>
            </div>
            <div class="courses-grid" v-if="!isLoading">
              <div v-for="course in courses" :key="course.course_id" class="course-card-premium">
                <div class="card-header">
                  <h3>{{ course.course_name }}</h3>
                  <span class="badge active">Active</span>
                </div>
                <div class="card-body">
                  <div class="detail-row">
                    <span>Students</span>
                    <strong>{{ course.student_count }}</strong>
                  </div>
                  <div class="detail-row">
                    <span>Performance</span>
                    <div class="mini-progress">
                      <div class="fill" :style="{ width: course.attendance_rate + '%' }"></div>
                    </div>
                  </div>
                </div>
                <div class="card-footer">
                  <button class="manage-btn" @click="manageCourse(course.course_id)">Manage Course</button>
                </div>
              </div>
            </div>
          </section>

        </div>

        <!-- Class Roster Section -->
        <section class="content-section py-8">
          <div class="section-header">
            <div class="header-titles">
              <h2>Active Session Roster</h2>
              <p v-if="activeSessionName">Monitoring: <span class="highlight-alt">{{ activeSessionName }}</span></p>
            </div>
            <div class="roster-actions">
              <span class="roster-stats">
                <strong>{{ presentCount }}</strong> Present / {{ activeRoster.length }} Total
              </span>
            </div>
          </div>

          <div class="roster-grid-v2" v-if="activeRoster.length > 0">
            <div v-for="student in activeRoster" :key="student.student_id" class="student-card-v2" :class="{ 'is-present': student.present }">
              <div class="student-avatar-mini">
                {{ student.full_name?.charAt(0) }}
              </div>
              <div class="student-info-mini">
                <div class="name">{{ student.full_name }}</div>
                <div class="sid">{{ student.student_id }}</div>
              </div>
              <div class="student-status-action">
                <span v-if="student.present" class="status-icon-check">✓</span>
                <button v-else class="mark-btn-v2" @click="markAsPresent(student.student_id)">
                  <span class="icon">📍</span> Mark
                </button>
              </div>
            </div>
          </div>
          <div v-else-if="!isLoading" class="empty-roster shadow-soft">
            <div class="empty-icon">📂</div>
            <p>No active session found. Start scanning to see your roster.</p>
          </div>
        </section>
      </div>
    </main>

    <!-- Scan Barcode Modal (Full Screen Overlay Style) -->
    <Modal :is-open="isScannerOpen" @close="closeBarcodeScanner" class="scanner-modal-fullscreen">
      <template #default>
        <div class="scanner-interface">
          <!-- Scanner Header -->
          <div class="scanner-header-overlay">
            <div class="session-badge">
              <span class="pulse"></span> LIVE RECORDING
            </div>
            <div class="session-title-large">
              <h2>Morning Biology - Section A</h2>
              <p>Scan student ID to mark attendance</p>
            </div>
            <button class="dismiss-btn" @click="closeBarcodeScanner">✕</button>
          </div>

          <!-- Camera Feed Placeholder -->
          <div class="camera-viewport">
            <BarcodeScanner v-if="isScannerOpen" @detected="handleBarcodeDetected" />
            <div class="scan-overlay-guides">
              <div class="guide-corner top-left"></div>
              <div class="guide-corner top-right"></div>
              <div class="guide-corner bottom-left"></div>
              <div class="guide-corner bottom-right"></div>
            </div>
          </div>

          <!-- Scan Status & Counter -->
          <div class="scanner-footer-overlay">
            <div class="stats-row">
              <div class="counter-box">
                <span class="label">ATTENDANCE LOGGED</span>
                <div class="count">{{ scannedCount }}</div>
              </div>
              <div class="progress-box">
                <div class="progress-labels">
                  <span>Present ({{ (scannedCount/stats.totalStudents*100).toFixed(0) }}%)</span>
                  <span>Absent</span>
                </div>
                <div class="master-progress">
                  <div class="present-bar" :style="{ width: (scannedCount/stats.totalStudents*100) + '%' }"></div>
                  <div class="absent-bar" :style="{ width: (100 - (scannedCount/stats.totalStudents*100)) + '%' }"></div>
                </div>
              </div>
            </div>
            
            <transition name="slide-up">
              <div v-if="lastScanned" class="scan-feedback-toast" :class="scanStatus">
                <div class="feedback-icon">{{ scanStatus === 'success' ? '✓' : '!' }}</div>
                <div class="feedback-text">
                  <span class="name">{{ lastScanned }}</span>
                  <span class="msg">{{ scanStatus === 'success' ? 'Attendance Recorded' : 'Scan Failed' }}</span>
                </div>
              </div>
            </transition>
          </div>
        </div>
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
import Navbar from '@/components/layout/Navbar.vue'
import Button from '@/components/ui/Button.vue'
import Modal from '@/components/ui/Modal.vue'
import BarcodeScanner from '@/components/BarcodeScanner.vue'
import CreateCourseModal from '@/components/CreateCourseModal.vue'
import CreateSessionModal from '@/components/CreateSessionModal.vue'

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
const activeSessionName = ref('')
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

const handleBarcodeDetected = async (barcode) => {
  try {
    const studentId = barcode
    if (lastScanned.value.includes(studentId)) return

    const { data: activeSession } = await supabase
      .from('sessions')
      .select('session_id')
      .eq('teacher_id', user.value.id)
      .order('created_at', { ascending: false })
      .limit(1)
      .single()

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
      .single()

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
      .select('session_id')
      .eq('teacher_id', user.value.id)
      .order('created_at', { ascending: false })
      .limit(1)
      .single()

    if (!activeSession) return

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
        status: new Date(s.session_date) > new Date() ? 'upcoming' : 'completed'
      }))
      stats.value.todaySessions = sessionsData.filter(s => s.session_date === new Date().toISOString().split('T')[0]).length
    }

    // Fetch roster for most recent session - Join courses to filter by teacher_id
    const { data: latestSession } = await supabase
      .from('sessions')
      .select('*, courses!inner(course_name, teacher_id)')
      .eq('courses.teacher_id', user.value.id)
      .order('created_at', { ascending: false })
      .limit(1)
      .maybeSingle()

    if (latestSession) {
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

/* Stats */
.stats-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 1.5rem;
}

/* Mobile responsive - adapt grid layout */
@media (max-width: 768px) {
  .stats-grid {
    grid-template-columns: 1fr;
  }
  
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

@media (min-width: 769px) and (max-width: 1024px) {
  .stats-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

.stat-card {
  background: var(--bg-card);
  padding: 1.5rem;
  border-radius: 24px;
  box-shadow: var(--shadow-soft);
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.stat-label {
  font-size: 0.75rem;
  font-weight: 800;
  color: var(--text-muted);
  letter-spacing: 0.05em;
}

.stat-value {
  font-size: 2rem;
  font-weight: 800;
  color: var(--text-main);
}

.stat-icon {
  width: 56px;
  height: 56px;
  border-radius: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
}

.stat-icon.courses { background: #eff6ff; color: #3b82f6; }
.stat-icon.students { background: #f0fdf4; color: #22c55e; }
.stat-icon.attendance { background: #faf5ff; color: #a855f7; }

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

.courses-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 1.5rem;
}

.course-card-premium {
  background: var(--bg-card);
  padding: 1.5rem;
  border-radius: 24px;
  box-shadow: var(--shadow-card);
}

.card-header {
  display: flex;
  justify-content: space-between;
  margin-bottom: 1.5rem;
}

.badge {
  padding: 4px 12px;
  border-radius: 8px;
  font-size: 0.7rem;
  font-weight: 800;
}

.badge.active { background: #dcfce7; color: #166534; }

.detail-row {
  display: flex;
  justify-content: space-between;
  margin-bottom: 1rem;
  font-size: 0.9rem;
}

.mini-progress {
  width: 100px;
  height: 8px;
  background: var(--bg-main);
  border-radius: 4px;
}

.mini-progress .fill {
  height: 100%;
  background: var(--primary);
  border-radius: 4px;
}

.manage-btn {
  width: 100%;
  padding: 0.75rem;
  border-radius: 12px;
  border: 2px solid var(--border-light);
  background: var(--bg-card);
  font-weight: 700;
  cursor: pointer;
  transition: all 0.2s;
  color: var(--text-main);
}

.manage-btn:hover { background: var(--bg-main); border-color: var(--border-medium); }

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

/* Scanner Overlays */
.scanner-interface {
  position: fixed;
  inset: 0;
  background: #000;
  z-index: 1000;
  display: flex;
  flex-direction: column;
}

.scanner-header-overlay {
  padding: 1.5rem;
  background: linear-gradient(to bottom, rgba(0,0,0,0.8), transparent);
  color: white;
  display: flex;
  justify-content: space-between;
  align-items: center;
  z-index: 10;
}

.camera-viewport {
  flex: 1;
  width: 100%;
  position: relative;
  background: #111;
  display: flex;
  align-items: center;
  justify-content: center;
  overflow: hidden;
}

@media (min-width: 1024px) {
  .camera-viewport {
    max-width: 800px;
    max-height: 600px;
    margin: auto;
    border-radius: 24px;
    border: 1px solid rgba(255,255,255,0.1);
  }
}

.session-badge {
  background: #7c3aed;
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 0.75rem;
  font-weight: 800;
  display: flex;
  align-items: center;
  gap: 8px;
}

.pulse {
  width: 8px;
  height: 8px;
  background: white;
  border-radius: 50%;
  animation: pulse 1.5s infinite;
}

@keyframes pulse {
  0% { transform: scale(1); opacity: 1; }
  100% { transform: scale(2.5); opacity: 0; }
}

.dismiss-btn {
  background: rgba(255,255,255,0.1);
  border: none;
  color: white;
  width: 40px;
  height: 40px;
  border-radius: 50%;
  cursor: pointer;
}

.camera-viewport {
  flex: 1;
  position: relative;
  overflow: hidden;
  display: flex;
  align-items: center;
  justify-content: center;
}

.scan-overlay-guides {
  position: absolute;
  inset: 0;
  margin: auto;
  width: 280px;
  height: 280px;
  pointer-events: none;
  z-index: 5;
}

.guide-corner {
  position: absolute;
  width: 40px;
  height: 40px;
  border: 4px solid white;
}

.top-left { top: 0; left: 0; border-right: none; border-bottom: none; }
.top-right { top: 0; right: 0; border-left: none; border-bottom: none; }
.bottom-left { bottom: 0; left: 0; border-right: none; border-top: none; }
.bottom-right { bottom: 0; right: 0; border-left: none; border-top: none; }

.scanner-footer-overlay {
  padding: 2rem;
  background: linear-gradient(to top, rgba(0,0,0,0.95), transparent);
  color: white;
}

.counter-box .count {
  font-size: 3rem;
  font-weight: 800;
}

.master-progress {
  height: 12px;
  background: rgba(255,255,255,0.1);
  border-radius: 6px;
  display: flex;
  overflow: hidden;
  margin-top: 0.5rem;
}

.present-bar { background: #22c55e; transition: width 0.3s; }
.absent-bar { background: rgba(255,255,255,0.1); }

/* Feedback Toast */
.scan-feedback-toast {
  position: fixed;
  bottom: 12rem;
  left: 50%;
  transform: translateX(-50%);
  background: var(--bg-card);
  padding: 1rem 2rem;
  border-radius: 100px;
  display: flex;
  align-items: center;
  gap: 1rem;
  box-shadow: 0 20px 40px rgba(0,0,0,0.4);
}

.scan-feedback-toast.success { border-left: 8px solid #22c55e; }
.scan-feedback-toast .name { color: var(--text-main); font-weight: 800; font-size: 1.1rem; }
.scan-feedback-toast .msg { color: var(--text-muted); font-size: 0.8rem; display: block; }

.slide-up-enter-active, .slide-up-leave-active { transition: all 0.4s cubic-bezier(0.16, 1, 0.3, 1); }
.slide-up-enter-from { opacity: 0; transform: translate(-50%, 40px); }
.slide-up-leave-to { opacity: 0; transform: translate(-50%, -40px); }

/* Roster Styles */
.roster-grid-v2 {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 1.25rem;
}

.student-card-v2 {
  background: var(--bg-card);
  border: 2px solid var(--border-light);
  border-radius: 16px;
  padding: 1rem;
  display: flex;
  align-items: center;
  gap: 0.75rem;
  transition: all 0.2s;
}

.student-card-v2.is-present {
  background: var(--success-bg);
  border-color: var(--success);
}

.student-avatar-mini {
  width: 40px;
  height: 40px;
  border-radius: 12px;
  background: var(--bg-main);
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 800;
  color: var(--text-muted);
}

.student-card-v2.is-present .student-avatar-mini {
  background: var(--success);
  color: var(--text-inverse);
}

.student-info-mini .name {
  font-size: 0.95rem;
  font-weight: 800;
  color: var(--text-main);
}

.student-info-mini .sid {
  font-size: 0.75rem;
  font-weight: 600;
  color: var(--text-muted);
}

.student-status-action {
  margin-left: auto;
}

.status-icon-check {
  display: flex;
  width: 28px;
  height: 28px;
  background: #22c55e;
  color: white;
  border-radius: 50%;
  align-items: center;
  justify-content: center;
  font-weight: 900;
  font-size: 0.9rem;
}

.mark-btn-v2 {
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  padding: 6px 12px;
  border-radius: 10px;
  font-size: 0.75rem;
  font-weight: 700;
  color: #475569;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 4px;
}

.mark-btn-v2:hover {
  background: var(--bg-main);
  color: var(--text-main);
}

.highlight-alt {
  color: #7c3aed;
  font-weight: 800;
}

.empty-roster {
  background: var(--bg-card);
  padding: 4rem;
  text-align: center;
  border-radius: 24px;
}

.empty-icon { font-size: 3rem; margin-bottom: 1rem; }
.empty-roster p { color: var(--text-muted); font-weight: 600; }

.header-titles h2 { margin-bottom: 0.25rem; }
.header-titles p { font-size: 0.85rem; color: var(--text-muted); font-weight: 600; }

.roster-stats {
  font-size: 0.9rem;
  color: var(--text-muted);
  font-weight: 600;
}

.roster-stats strong { color: var(--text-main); font-weight: 800; }
</style>
