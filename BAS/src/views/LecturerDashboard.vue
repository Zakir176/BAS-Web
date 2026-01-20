<template>
  <div class="lecturer-dashboard">
    <Navbar />
    
    <main class="main-content">
      <div class="container">
        <!-- Welcome Section -->
        <section class="welcome-section">
          <div class="welcome-content">
            <div class="welcome-text">
              <h1>Welcome back, {{ lecturerName }}!</h1>
              <p class="welcome-subtitle">Manage your courses and track student attendance</p>
            </div>
            <div class="welcome-actions">
              <Button variant="primary" size="lg" @click="showCreateCourse">
                <svg width="20" height="20" viewBox="0 0 20 20" fill="currentColor">
                  <path d="M10 3a1 1 0 011 1v5h5a1 1 0 110 2h-5v5a1 1 0 11-2 0v-5H4a1 1 0 110-2h5V4a1 1 0 011-1z"/>
                </svg>
                Create Course
              </Button>
              <Button variant="secondary" size="lg" @click="showCreateSession">
                <svg width="20" height="20" viewBox="0 0 20 20" fill="currentColor">
                  <path d="M6 2a1 1 0 00-1 1v1H4a2 2 0 00-2 2v10a2 2 0 002 2h12a2 2 0 002-2V6a2 2 0 00-2-2h-1V3a1 1 0 10-2 1v1H7V3a1 1 0 00-1-1zm0 5a1 1 0 000 2h8a1 1 0 100-2H6z"/>
                </svg>
                Start Session
              </Button>
              <Button variant="success" size="lg" @click="showBarcodeScanner">
                <svg width="20" height="20" viewBox="0 0 20 20" fill="currentColor">
                  <path d="M2 6a2 2 0 012-2h12a2 2 0 012 2v8a2 2 0 01-2 2H4a2 2 0 01-2-2V6zm2-2v2h12V4H4zm0 4v6h2V8H4zm4 0v6h2V8H8zm4 0v6h2V8h-2zm4 0v6h2V8h-2z"/>
                </svg>
                Scan Barcode
              </Button>
            </div>
          </div>
        </section>

        <!-- Stats Overview -->
        <section class="stats-section">
          <div class="section-header">
            <h2>Overview</h2>
          </div>
          
          <div v-if="isLoading" class="loading-state">
            <div class="loading-spinner"></div>
            <p>Loading dashboard data...</p>
          </div>
          
          <div v-else class="stats-grid">
            <Card class="stat-card">
              <div class="stat-icon courses">
                <svg width="32" height="32" viewBox="0 0 32 32" fill="currentColor">
                  <path d="M4 4h24v4H4V4zm0 6h24v4H4v-4zm0 6h24v4H4v-4zm0 6h24v4H4v-4z"/>
                </svg>
              </div>
              <div class="stat-content">
                <div class="stat-number">{{ stats.totalCourses }}</div>
                <div class="stat-label">Total Courses</div>
              </div>
            </Card>

            <Card class="stat-card">
              <div class="stat-icon students">
                <svg width="32" height="32" viewBox="0 0 32 32" fill="currentColor">
                  <path d="M16 2C8.268 2 2 8.268 2 16s6.268 14 14 14 14-6.268 14-14S23.732 2 16 2zm0 2c6.627 0 12 5.373 12 12s-5.373 12-12S4 22.627 4 16 9.373 4 16 4zm-1 5v6h6v-2h-6V9h-2z"/>
                </svg>
              </div>
              <div class="stat-content">
                <div class="stat-number">{{ stats.totalStudents }}</div>
                <div class="stat-label">Total Students</div>
              </div>
            </Card>

            <Card class="stat-card">
              <div class="stat-icon sessions">
                <svg width="32" height="32" viewBox="0 0 32 32" fill="currentColor">
                  <path d="M16 2l4.586 4.586L26 6.172l2.828 2.828L28.586 16 26 18.586 23.172 26 16 28.586 13.414 26 6.172 23.172 3.344 16 5.414 13.414 8.828 6.172 16 2z"/>
                </svg>
              </div>
              <div class="stat-content">
                <div class="stat-number">{{ stats.todaySessions }}</div>
                <div class="stat-label">Today's Sessions</div>
              </div>
            </Card>

            <Card class="stat-card">
              <div class="stat-icon attendance">
                <svg width="32" height="32" viewBox="0 0 32 32" fill="currentColor">
                  <path d="M16 2C8.268 2 2 8.268 2 16s6.268 14 14 14 14-6.268 14-14S23.732 2 16 2zm0 2c6.627 0 12 5.373 12 12s-5.373 12-12S4 22.627 4 16 9.373 4 16 4zm-1 5v6h6v2h-8V9h-2z"/>
                </svg>
              </div>
              <div class="stat-content">
                <div class="stat-number">{{ stats.avgAttendance }}%</div>
                <div class="stat-label">Avg Attendance</div>
              </div>
            </Card>
          </div>
        </section>

        <!-- My Courses -->
        <section class="courses-section">
          <div class="section-header">
            <h2>My Courses</h2>
            <Button variant="secondary" size="sm" @click="viewAllCourses">
              View All
            </Button>
          </div>
          
          <div v-if="isLoading" class="loading-state">
            <div class="loading-spinner"></div>
            <p>Loading courses...</p>
          </div>
          
          <div v-else-if="courses.length === 0" class="empty-state">
            <p>No courses found. Create your first course to get started!</p>
          </div>
          
          <div v-else class="courses-grid">
            <Card v-for="course in courses" :key="course.course_id" class="course-card">
              <div class="course-header">
                <h3>{{ course.course_name }}</h3>
                <div class="course-status" :class="course.status">
                  {{ course.status }}
                </div>
              </div>
              <div class="course-stats">
                <div class="course-stat">
                  <span class="stat-label">Students</span>
                  <span class="stat-value">{{ course.student_count || 0 }}</span>
                </div>
                <div class="course-stat">
                  <span class="stat-label">Sessions</span>
                  <span class="stat-value">{{ course.session_count || 0 }}</span>
                </div>
                <div class="course-stat">
                  <span class="stat-label">Attendance</span>
                  <span class="stat-value">{{ course.attendance_rate || 0 }}%</span>
                </div>
              </div>
              <div class="course-actions">
                <Button variant="primary" size="sm" @click="manageCourse(course.course_id)">
                  Manage
                </Button>
                <Button variant="secondary" size="sm" @click="viewAttendance(course.course_id)">
                  Attendance
                </Button>
              </div>
            </Card>
          </div>
        </section>

        <!-- Recent Sessions -->
        <section class="sessions-section">
          <div class="section-header">
            <h2>Recent Sessions</h2>
            <Button variant="secondary" size="sm" @click="viewAllSessions">
              View All
            </Button>
          </div>
          
          <div v-if="isLoading" class="loading-state">
            <div class="loading-spinner"></div>
            <p>Loading sessions...</p>
          </div>
          
          <div v-else-if="recentSessions.length === 0" class="empty-state">
            <p>No recent sessions found</p>
          </div>
          
          <div v-else class="sessions-list">
            <Card v-for="session in recentSessions" :key="session.session_id" class="session-item">
              <div class="session-time">
                <div class="date">{{ formatDate(session.session_date) }}</div>
                <div class="time">{{ session.session_time || '09:00 AM' }}</div>
              </div>
              <div class="session-details">
                <h4>{{ session.course_name }}</h4>
                <p>{{ session.student_count || 0 }} students enrolled</p>
                <div class="session-status" :class="session.status">
                  {{ session.status }}
                </div>
              </div>
              <div class="session-action">
                <Button 
                  v-if="session.status === 'active'" 
                  variant="success" 
                  size="sm"
                  @click="viewLiveSession(session.session_id)"
                >
                  View Live
                </Button>
                <Button 
                  v-else 
                  variant="primary" 
                  size="sm"
                  @click="viewSessionDetails(session.session_id)"
                >
                  Details
                </Button>
              </div>
            </Card>
          </div>
        </section>
      </div>
    </main>

    <Modal :is-open="isScannerOpen" @close="closeBarcodeScanner">
      <template #header>
        <h3 class="modal-title">Scan Student Barcode</h3>
      </template>
      <template #default>
        <BarcodeScanner v-if="isScannerOpen" @detected="handleBarcodeDetected" />
      </template>
      <template #footer>
        <Button @click="closeBarcodeScanner">Cancel</Button>
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
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '@/composables/useAuth'
import { supabase } from '@/supabase'
import Navbar from '@/components/layout/Navbar.vue'
import Button from '@/components/ui/Button.vue'
import Card from '@/components/ui/Card.vue'
import Modal from '@/components/ui/Modal.vue'
import BarcodeScanner from '@/components/BarcodeScanner.vue'
import CreateCourseModal from '@/components/CreateCourseModal.vue'
import CreateSessionModal from '@/components/CreateSessionModal.vue'

const router = useRouter()
const { user } = useAuth()

// Reactive state
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
const isScannerOpen = ref(false)
const isCreateCourseModalOpen = ref(false)
const isCreateSessionModalOpen = ref(false)

const showCreateCourse = () => {
  isCreateCourseModalOpen.value = true
}

const showCreateSession = () => {
  isCreateSessionModalOpen.value = true
}

const handleCourseCreated = async () => {
  await fetchLecturerData()
}

const handleSessionCreated = async () => {
  await fetchLecturerData()
}

const showBarcodeScanner = () => {
  isScannerOpen.value = true
}

const closeBarcodeScanner = () => {
  isScannerOpen.value = false
}

const handleBarcodeDetected = async (barcode) => {
  try {
    // Assuming the barcode is the student's ID
    const studentId = barcode

    // Get the active session
    // This is a simplified example, you might need to select a session
    const { data: activeSession, error: sessionError } = await supabase
      .from('sessions')
      .select('session_id')
      .eq('status', 'active')
      .eq('teacher_id', user.value.id)
      .single()

    if (sessionError || !activeSession) {
      alert('No active session found.')
      return
    }

    // 2. Verify student exists
    const { data: student, error: studentError } = await supabase
      .from('students')
      .select('full_name')
      .eq('student_id', studentId)
      .single()

    if (studentError || !student) {
      alert(`Student ID ${studentId} not found in database.`)
      return
    }

    // 3. Mark the student as present
    const { error: attendanceError } = await supabase
      .from('attendance')
      .insert({
        session_id: activeSession.session_id,
        student_id: studentId,
        status: 'Present',
        method: 'Barcode'
      })

    if (attendanceError) {
      alert('Error marking attendance.')
      console.error(attendanceError)
      return
    }

    alert(`Student ${student.full_name} (${studentId}) marked as present.`)
    closeBarcodeScanner()

  } catch (error) {
    alert('An error occurred.')
    console.error(error)
  }
}


// Fetch lecturer data
const fetchLecturerData = async () => {
  try {
    if (!user.value) return

    // Get lecturer profile
    const { data: lecturer } = await supabase
      .from('teachers')
      .select('full_name')
      .eq('email', user.value.email)
      .single()

    if (lecturer) {
      lecturerName.value = lecturer.full_name
    }

    // Get lecturer's courses
    const { data: coursesData } = await supabase
      .from('courses')
      .select(`
        course_id,
        course_name,
        max_absences_allowed,
        created_at,
        enrollments(count),
        sessions(count)
      `)
      .eq('teacher_id', user.value.id)

    if (coursesData) {
      courses.value = coursesData.map(course => ({
        ...course,
        student_count: course.enrollments?.length || 0,
        session_count: course.sessions?.length || 0,
        status: 'active'
      }))

      stats.value.totalCourses = coursesData.length
      stats.value.totalStudents = coursesData.reduce((sum, course) => sum + (course.enrollments?.length || 0), 0)
    }

    // Get recent sessions
    const { data: sessionsData } = await supabase
      .from('sessions')
      .select(`
        session_id,
        session_date,
        session_time,
        courses!inner(
          course_name
        ),
        enrollments(count)
      `)
      .in('course_id', coursesData.map(c => c.course_id))
      .order('session_date', { ascending: false })
      .limit(5)

    if (sessionsData) {
      recentSessions.value = sessionsData.map(session => ({
        ...session,
        student_count: session.enrollments?.length || 0,
        status: new Date(session.session_date) > new Date() ? 'upcoming' : 'completed'
      }))

      // Calculate today's sessions
      const today = new Date().toISOString().split('T')[0]
      stats.value.todaySessions = sessionsData.filter(s => s.session_date === today).length
    }

    // Calculate average attendance (mock for now)
    stats.value.avgAttendance = 85

  } catch (error) {
    console.error('Error fetching lecturer data:', error)
  } finally {
    isLoading.value = false
  }
}

const manageCourse = (courseId) => {
  router.push(`/course/${courseId}`)
}

const viewAttendance = (courseId) => {
  router.push(`/attendance/${courseId}`)
}

const viewAllCourses = () => {
  router.push('/courses')
}

const viewAllSessions = () => {
  router.push('/sessions')
}

const viewLiveSession = (sessionId) => {
  router.push(`/session/${sessionId}/live`)
}

const viewSessionDetails = (sessionId) => {
  router.push(`/session/${sessionId}`)
}

const formatDate = (dateString) => {
  return new Date(dateString).toLocaleDateString('en-US', {
    month: 'short',
    day: 'numeric',
    year: 'numeric'
  })
}

onMounted(async () => {
  await fetchLecturerData()
})
</script>

<style scoped>
.lecturer-dashboard {
  min-height: 100vh;
  background-color: var(--bg-primary);
}

.main-content {
  padding: 2rem 0;
}

.welcome-section {
  margin-bottom: 3rem;
}

.welcome-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 2rem;
}

.welcome-text h1 {
  font-size: 2.5rem;
  font-weight: 700;
  color: var(--text-primary);
  margin-bottom: 0.5rem;
}

.welcome-subtitle {
  font-size: 1.125rem;
  color: var(--text-secondary);
  margin: 0;
}

.welcome-actions {
  display: flex;
  gap: 1rem;
}

.stats-section,
.courses-section,
.sessions-section {
  margin-bottom: 3rem;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.section-header h2 {
  font-size: 1.5rem;
  font-weight: 600;
  color: var(--text-primary);
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1.5rem;
}

.stat-card {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1.5rem;
}

.stat-icon {
  width: 3rem;
  height: 3rem;
  border-radius: 0.75rem;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
}

.stat-icon.courses {
  background-color: var(--accent-primary);
}

.stat-icon.students {
  background-color: var(--success);
}

.stat-icon.sessions {
  background-color: var(--warning);
}

.stat-icon.attendance {
  background-color: var(--info);
}

.stat-content {
  flex: 1;
}

.stat-number {
  font-size: 2rem;
  font-weight: 700;
  color: var(--text-primary);
  line-height: 1;
}

.stat-label {
  color: var(--text-secondary);
  font-size: 0.875rem;
  margin-top: 0.25rem;
}

.loading-state {
  text-align: center;
  padding: 2rem;
}

.loading-spinner {
  width: 2rem;
  height: 2rem;
  border: 2px solid var(--accent-primary);
  border-top: 2px solid var(--accent-primary);
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 1rem;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.empty-state {
  text-align: center;
  padding: 2rem;
  color: var(--text-secondary);
  font-style: italic;
}

.courses-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 1.5rem;
}

.course-card {
  padding: 1.5rem;
}

.course-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 1rem;
}

.course-header h3 {
  font-size: 1.25rem;
  font-weight: 600;
  color: var(--text-primary);
  margin: 0;
}

.course-status {
  padding: 0.25rem 0.75rem;
  border-radius: 1rem;
  font-size: 0.75rem;
  font-weight: 500;
  background-color: var(--success);
  color: white;
}

.course-stats {
  display: flex;
  gap: 1rem;
  margin-bottom: 1rem;
}

.course-stat {
  display: flex;
  flex-direction: column;
  align-items: center;
}

.course-stat .stat-label {
  font-size: 0.75rem;
  color: var(--text-secondary);
  margin-bottom: 0.25rem;
}

.course-stat .stat-value {
  font-size: 1.125rem;
  font-weight: 600;
  color: var(--text-primary);
}

.course-actions {
  display: flex;
  gap: 0.5rem;
}

.sessions-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.session-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem;
}

.session-time {
  min-width: 120px;
  text-align: center;
}

.session-time .date {
  font-weight: 600;
  color: var(--text-primary);
  margin-bottom: 0.25rem;
}

.session-time .time {
  font-size: 0.875rem;
  color: var(--text-secondary);
}

.session-details {
  flex: 1;
  margin-left: 1.5rem;
}

.session-details h4 {
  font-size: 1rem;
  font-weight: 600;
  color: var(--text-primary);
  margin-bottom: 0.25rem;
}

.session-details p {
  color: var(--text-secondary);
  font-size: 0.875rem;
  margin: 0;
}

.session-status {
  display: inline-block;
  padding: 0.25rem 0.75rem;
  border-radius: 1rem;
  font-size: 0.75rem;
  font-weight: 500;
  margin-top: 0.5rem;
}

.session-status.active {
  background-color: var(--success);
  color: white;
}

.session-status.upcoming {
  background-color: var(--warning);
  color: white;
}

.session-status.completed {
  background-color: var(--text-secondary);
  color: white;
}

.session-action {
  min-width: 100px;
  text-align: right;
}

@media (max-width: 768px) {
  .welcome-content {
    flex-direction: column;
    align-items: flex-start;
  }
  
  .stats-grid {
    grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  }
  
  .courses-grid {
    grid-template-columns: 1fr;
  }
  
  .session-item {
    flex-direction: column;
    align-items: flex-start;
    gap: 1rem;
  }
  
  .session-action {
    width: 100%;
    text-align: left;
  }
}
</style>
