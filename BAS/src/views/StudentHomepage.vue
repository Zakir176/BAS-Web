<template>
  <div class="student-homepage">
    <Navbar />
    
    <main class="main-content">
      <div class="container">
        <!-- Welcome Section -->
        <section class="welcome-section">
          <div class="welcome-content">
            <div class="welcome-text">
              <h1>Welcome back, {{ studentName }}!</h1>
              <p class="welcome-subtitle">Track your attendance and view your academic progress</p>
            </div>
            <div class="welcome-actions">
              <Button variant="primary" size="lg" @click="showBarcodeScanner">
                <svg width="20" height="20" viewBox="0 0 20 20" fill="currentColor">
                  <path d="M3 4a1 1 0 011-1h3a1 1 0 011 1v3a1 1 0 01-1 1H4a1 1 0 01-1-1V4zM12 4a1 1 0 011-1h3a1 1 0 011 1v3a1 1 0 01-1 1h-3a1 1 0 01-1-1V4zM3 12a1 1 0 011-1h3a1 1 0 011 1v3a1 1 0 01-1 1H4a1 1 0 01-1-1v-3zM12 12a1 1 0 011-1h3a1 1 0 011 1v3a1 1 0 01-1 1h-3a1 1 0 01-1-1v-3z"/>
                </svg>
                Mark Attendance
              </Button>
            </div>
          </div>
        </section>

        <!-- Stats Overview -->
        <section class="stats-section">
          <div class="section-header">
            <h2>Attendance Overview</h2>
          </div>
          
          <div v-if="isLoading" class="loading-state">
            <div class="loading-spinner"></div>
            <p>Loading your attendance data...</p>
          </div>
          
          <div v-else class="stats-grid">
            <Card class="stat-card">
              <div class="stat-icon attendance">
                <svg width="32" height="32" viewBox="0 0 32 32" fill="currentColor">
                  <path d="M16 2C8.268 2 2 8.268 2 16s6.268 14 14 14 14-6.268 14-14S23.732 2 16 2zm0 2c6.627 0 12 5.373 12 12s-5.373 12-12 12S4 22.627 4 16 9.373 4 16 4zm-1 5v6h6v2h-8V9h-2z"/>
                </svg>
              </div>
              <div class="stat-content">
                <div class="stat-number">{{ attendanceStats.overall }}%</div>
                <div class="stat-label">Overall Attendance</div>
              </div>
            </Card>

            <Card class="stat-card">
              <div class="stat-icon present">
                <svg width="32" height="32" viewBox="0 0 32 32" fill="currentColor">
                  <path d="M16 2l4.586 4.586L26 6.172l2.828 2.828L28.586 16 26 18.586 23.172 26 16 28.586 13.414 26 6.172 23.172 3.344 16 5.414 13.414 8.828 6.172 16 2z"/>
                </svg>
              </div>
              <div class="stat-content">
                <div class="stat-number">{{ attendanceStats.present }}</div>
                <div class="stat-label">Days Present</div>
              </div>
            </Card>

            <Card class="stat-card">
              <div class="stat-icon absent">
                <svg width="32" height="32" viewBox="0 0 32 32" fill="currentColor">
                  <path d="M16 2C8.268 2 2 8.268 2 16s6.268 14 14 14 14-6.268 14-14S23.732 2 16 2zm0 2c6.627 0 12 5.373 12 12s-5.373 12-12 12S4 22.627 4 16 9.373 4 16 4zm-1 5v8h8v-2h-6V9h-2z"/>
                </svg>
              </div>
              <div class="stat-content">
                <div class="stat-number">{{ attendanceStats.absent }}</div>
                <div class="stat-label">Days Absent</div>
              </div>
            </Card>

            <Card class="stat-card">
              <div class="stat-icon streak">
                <svg width="32" height="32" viewBox="0 0 32 32" fill="currentColor">
                  <path d="M16 2l2.122 6.364L24 10.878l-5.878 2.514L16 20l-2.122-6.608L8 10.878l5.878-2.514L16 2z"/>
                </svg>
              </div>
              <div class="stat-content">
                <div class="stat-number">{{ attendanceStats.streak }}</div>
                <div class="stat-label">Day Streak</div>
              </div>
            </Card>
          </div>
        </section>

        <!-- Recent Activity -->
        <section class="activity-section">
          <div class="section-header">
            <h2>Recent Activity</h2>
            <Button variant="secondary" size="sm" @click="viewAllActivity">
              View All
            </Button>
          </div>
          
          <div v-if="isLoading" class="loading-state">
            <div class="loading-spinner"></div>
            <p>Loading activity data...</p>
          </div>
          
          <div v-else-if="recentActivity.length === 0" class="empty-state">
            <p>No recent activity found</p>
          </div>
          
          <div v-else class="activity-list">
            <Card v-for="activity in recentActivity" :key="activity.id" class="activity-item">
              <div class="activity-content">
                <div class="activity-icon" :class="activity.type">
                  <svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor">
                    <path v-if="activity.type === 'present'" d="M9 16.17L4.83 12l-1.42 1.41L9 19 21 7l-1.41-1.41z"/>
                    <path v-else d="M19 6.41L17.59 5 12 10.59 6.41 5 5 6.41 10.59 12 5 17.59 6.41 19 12 13.41 17.59 19 19 17.59 13.41 12z"/>
                  </svg>
                </div>
                <div class="activity-details">
                  <h4>{{ activity.course }}</h4>
                  <p>{{ activity.date }} • {{ activity.time }}</p>
                </div>
              </div>
              <div class="activity-status" :class="activity.type">
                {{ activity.type === 'present' ? 'Present' : 'Absent' }}
              </div>
            </Card>
          </div>
        </section>

        <!-- Today's Schedule -->
        <section class="schedule-section">
          <div class="section-header">
            <h2>Today's Schedule</h2>
            <span class="current-date">{{ currentDate }}</span>
          </div>
          
          <div v-if="isLoading" class="loading-state">
            <div class="loading-spinner"></div>
            <p>Loading schedule...</p>
          </div>
          
          <div v-else-if="todaySchedule.length === 0" class="empty-state">
            <p>No classes scheduled for today</p>
          </div>
          
          <div v-else class="schedule-grid">
            <Card v-for="classItem in todaySchedule" :key="classItem.id" class="schedule-item">
              <div class="schedule-time">
                <div class="time-start">{{ classItem.startTime }}</div>
                <div class="time-end">{{ classItem.endTime }}</div>
              </div>
              <div class="schedule-details">
                <h4>{{ classItem.course }}</h4>
                <p>{{ classItem.lecturer }} • {{ classItem.room }}</p>
                <div class="schedule-status" :class="classItem.status">
                  {{ classItem.status }}
                </div>
              </div>
              <div class="schedule-action">
                <Button 
                  v-if="classItem.status === 'Upcoming'" 
                  variant="primary" 
                  size="sm"
                  @click="markAttendance(classItem.id)"
                >
                  Mark
                </Button>
                <span v-else-if="classItem.status === 'Completed'" class="completed-mark">
                  ✓
                </span>
              </div>
            </Card>
          </div>
        </section>
      </div>
    </main>
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

const router = useRouter()
const { user } = useAuth()

// Reactive state
const studentName = ref('')
const currentDate = ref(new Date().toLocaleDateString('en-US', { 
  weekday: 'long', 
  year: 'numeric', 
  month: 'long', 
  day: 'numeric' 
}))

const attendanceStats = ref({
  overall: 0,
  present: 0,
  absent: 0,
  late: 0
})

const recentActivity = ref([])
const todaySchedule = ref([])
const isLoading = ref(true)

// Fetch student data
const fetchStudentData = async () => {
  try {
    if (!user.value) return

    // Get student profile
    const { data: student } = await supabase
      .from('students')
      .select('full_name, class_section')
      .eq('email', user.value.email)
      .single()

    if (student) {
      studentName.value = student.full_name
    }

    // Get attendance statistics
    const { data: attendanceData } = await supabase
      .from('attendance')
      .select('status')
      .eq('student_id', student.student_id || '')

    if (attendanceData) {
      const total = attendanceData.length
      const present = attendanceData.filter(a => a.status === 'Present').length
      const absent = attendanceData.filter(a => a.status === 'Absent').length
      const late = attendanceData.filter(a => a.status === 'Late').length
      
      attendanceStats.value = {
        overall: total > 0 ? Math.round((present / total) * 100) : 0,
        present,
        absent,
        late
      }
    }

    // Get recent activity
    const { data: activityData } = await supabase
      .from('attendance')
      .select(`
        status,
        timestamp,
        sessions!inner(
          course_id,
          session_date,
          courses!inner(
            course_name
          )
        )
      `)
      .eq('student_id', student.student_id || '')
      .order('timestamp', { ascending: false })
      .limit(5)

    if (activityData) {
      recentActivity.value = activityData.map(item => ({
        id: item.attendance_id,
        type: item.status === 'Present' ? 'attendance' : 'absence',
        description: `${item.sessions?.courses?.course_name || 'Class'} - ${item.status}`,
        time: new Date(item.timestamp).toLocaleDateString('en-US', {
          month: 'short',
          day: 'numeric',
          hour: '2-digit',
          minute: '2-digit'
        }),
        status: item.status.toLowerCase()
      }))
    }

    // Get today's schedule
    const today = new Date().toISOString().split('T')[0]
    const { data: scheduleData } = await supabase
      .from('sessions')
      .select(`
        session_id,
        session_date,
        session_time,
        courses!inner(
          course_name
        )
      `)
      .eq('session_date', today)
      .order('session_time', { ascending: true })

    if (scheduleData) {
      todaySchedule.value = scheduleData.map(item => ({
        id: item.session_id,
        courseName: item.courses?.course_name || 'Unknown Course',
        time: item.session_time || '09:00 AM',
        location: 'Classroom',
        status: 'upcoming'
      }))
    }

  } catch (error) {
    console.error('Error fetching student data:', error)
  } finally {
    isLoading.value = false
  }
}

const showBarcodeScanner = () => {
  // TODO: Implement barcode scanner
  alert('Barcode scanner feature coming soon!')
}

const markAttendance = async (classId) => {
  try {
    // Mark attendance for this session
    const { error } = await supabase
      .from('attendance')
      .insert({
        session_id: classId,
        student_id: user.value?.user_metadata?.student_id,
        status: 'Present',
        method: 'Web'
      })

    if (error) throw error

    // Update local state
    const classItem = todaySchedule.value.find(c => c.id === classId)
    if (classItem) {
      classItem.status = 'Completed'
    }

    // Refresh data
    await fetchStudentData()

  } catch (error) {
    console.error('Error marking attendance:', error)
    alert('Failed to mark attendance. Please try again.')
  }
}

onMounted(async () => {
  await fetchStudentData()
  console.log('Student homepage loaded')
})
</script>

<style scoped>
.student-homepage {
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

.stats-section {
  margin-bottom: 3rem;
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

.stat-icon.attendance {
  background-color: var(--accent-primary);
}

.stat-icon.present {
  background-color: var(--success);
}

.stat-icon.absent {
  background-color: var(--error);
}

.stat-icon.streak {
  background-color: var(--warning);
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

.activity-section,
.schedule-section {
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

.current-date {
  color: var(--text-secondary);
  font-size: 0.875rem;
}

.activity-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.activity-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 1.5rem;
}

.activity-content {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.activity-icon {
  width: 2.5rem;
  height: 2.5rem;
  border-radius: 0.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
}

.activity-icon.present {
  background-color: var(--success);
}

.activity-icon.absent {
  background-color: var(--error);
}

.activity-details h4 {
  font-size: 1rem;
  font-weight: 600;
  color: var(--text-primary);
  margin-bottom: 0.25rem;
}

.activity-details p {
  color: var(--text-secondary);
  font-size: 0.875rem;
  margin: 0;
}

.activity-status {
  padding: 0.25rem 0.75rem;
  border-radius: 1rem;
  font-size: 0.75rem;
  font-weight: 500;
}

.activity-status.present {
  background-color: var(--success);
  color: white;
}

.activity-status.absent {
  background-color: var(--error);
  color: white;
}

.schedule-grid {
  display: flex;
  flex-direction: column;
  gap: 1rem;
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

.schedule-item {
  display: flex;
  align-items: center;
  gap: 1.5rem;
  padding: 1.5rem;
}

.schedule-time {
  min-width: 80px;
  text-align: center;
}

.time-start {
  font-size: 1rem;
  font-weight: 600;
  color: var(--text-primary);
}

.time-end {
  font-size: 0.875rem;
  color: var(--text-secondary);
}

.schedule-details {
  flex: 1;
}

.schedule-details h4 {
  font-size: 1rem;
  font-weight: 600;
  color: var(--text-primary);
  margin-bottom: 0.25rem;
}

.schedule-details p {
  color: var(--text-secondary);
  font-size: 0.875rem;
  margin: 0 0 0.5rem 0;
}

.schedule-status {
  display: inline-block;
  padding: 0.25rem 0.75rem;
  border-radius: 1rem;
  font-size: 0.75rem;
  font-weight: 500;
}

.schedule-status.Upcoming {
  background-color: var(--warning);
  color: white;
}

.schedule-status.Completed {
  background-color: var(--success);
  color: white;
}

.schedule-action {
  min-width: 80px;
  text-align: center;
}

.completed-mark {
  display: inline-block;
  width: 2rem;
  height: 2rem;
  background-color: var(--success);
  color: white;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: bold;
}

@media (max-width: 768px) {
  .welcome-content {
    flex-direction: column;
    text-align: center;
  }
  
  .welcome-actions {
    flex-direction: column;
    width: 100%;
  }
  
  .stats-grid {
    grid-template-columns: 1fr;
  }
  
  .activity-item,
  .schedule-item {
    flex-direction: column;
    text-align: center;
    gap: 1rem;
  }
  
  .schedule-time {
    min-width: auto;
  }
  
  .section-header {
    flex-direction: column;
    gap: 1rem;
    text-align: center;
  }
}
</style>
