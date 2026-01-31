<template>
  <div class="student-dashboard">
    <Navbar />
    
    <main class="main-content">
      <div class="container dashboard-container">
        <!-- Profile Header -->
        <StudentProfileHeader
          :is-loading="isLoading"
          :student-name="studentName"
          :student-profile="studentProfile"
          :class-section="classSection"
          @view-profile="toast.info('Profile page coming soon!')"
          @view-settings="toast.info('Settings page coming soon!')"
        />

        <!-- Desktop Grid Layout -->
        <div class="dashboard-grid">
          <!-- Left Column (Main Content) -->
          <div class="grid-main">
            <!-- Stats Overview -->
            <section class="stats-overview">
              <div class="stats-row">
                <div class="stat-card blue-card">
                  <span class="label">ATTENDANCE</span>
                  <Skeleton v-if="isLoading" width="60px" height="2rem" />
                  <div v-else class="value">{{ attendanceStats.overall }}%</div>
                </div>
                <div class="stat-card white-card">
                  <span class="label">ABSENCES</span>
                  <Skeleton v-if="isLoading" width="60px" height="2rem" />
                  <div v-else class="value danger">{{ attendanceStats.absent }}</div>
                </div>
              </div>
            </section>

            <!-- Heatmap Section -->
            <section class="heatmap-section">
              <div class="card calendar-surface">
                <div class="section-header">
                  <h3>Attendance Heatmap</h3>
                  <div class="selector">Term 1 <span>▾</span></div>
                </div>
                <AttendanceCalendar :attendance-records="attendanceRecords" />
              </div>
            </section>

            <!-- History Timeline -->
            <RecentActivity 
              :is-loading="isLoading"
              :activities="recentActivity"
              :show-all="showAllHistory"
              @toggle-show-all="showAllHistory = !showAllHistory"
            />
          </div>

          <!-- Right Column (Sidebar) -->
          <div class="grid-sidebar">
            <!-- Barcode Section -->
            <section v-if="!isLoading && studentProfile?.student_id" class="barcode-section">
              <StudentBarcode :student-id="studentProfile.student_id" />
            </section>
            <section v-else-if="isLoading" class="barcode-section">
              <div class="barcode-skeleton">
                <Skeleton width="100%" height="200px" radius="20px" />
              </div>
            </section>

            <!-- Main Action Area -->
            <section class="action-area">
              <Button 
                variant="primary" 
                full-width 
                size="lg" 
                class="contact-button"
                @click="handleContactParent"
              >
                <span class="btn-icon">✉️</span> Contact Counselor
              </Button>
              <button class="fab-edit" @click="toast.info('Profile editing coming soon!')">✎</button>
            </section>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '@/composables/useAuth'
import { useToast } from '@/composables/useToast'
import { supabase } from '@/supabase'
import Navbar from '@/components/common/Navbar.vue'
import Button from '@/components/ui/Button.vue'
import Skeleton from '@/components/ui/Skeleton.vue'
import AttendanceCalendar from '@/components/student/AttendanceCalendar.vue'
import StudentBarcode from '@/components/student/StudentBarcode.vue'
import StudentProfileHeader from '@/components/student/StudentProfileHeader.vue'
import RecentActivity from '@/components/student/RecentActivity.vue'

const router = useRouter()
const { user } = useAuth()
const { toast } = useToast()
const studentProfile = ref(null)
const studentName = ref('')
const classSection = ref('10A') // Default/Placeholder

const attendanceStats = ref({
  overall: 0,
  present: 0,
  absent: 0
})

const recentActivity = ref([])
const attendanceRecords = ref([])
const isLoading = ref(true)
const showAllHistory = ref(false)

const handleContactParent = () => {
  const subject = encodeURIComponent(`Inquiry regarding student ${studentName.value}`)
  const body = encodeURIComponent(`Hello,\n\nI am inquiring about the attendance status for student ID: ${studentProfile.value?.student_id}.`)
  window.location.href = `mailto:advisor@university.edu?subject=${subject}&body=${body}`
}

const fetchStudentData = async () => {
  try {
    if (!user.value) {
      toast.error('User not found. Please login again.')
      return
    }
    
    isLoading.value = true

    // 1. Get student profile details using email
    const { data: studentData, error } = await supabase
      .from('students')
      .select('*')
      .eq('email', user.value.email)
      .single()
    
    if (error) {
      toast.error('Failed to fetch student profile')
      console.error('Student profile error:', error)
      return
    }
    
    if (studentData) {
      studentProfile.value = studentData
      studentName.value = studentData.full_name
      classSection.value = studentData.class_section || '10A'
      toast.success(`Welcome back, ${studentData.full_name}!`)
    } else {
      toast.warning('Student profile not found')
    }

    // 2. Fetch specific attendance records
    const studentId = studentProfile.value?.student_id
    if (!studentId) {
      toast.warning('Student ID not found')
      return
    }

    const { data: attendanceData } = await supabase
      .from('attendance')
      .select('status, timestamp, attendance_id')
      .eq('student_id', studentId)
      .order('timestamp', { ascending: false })

    if (attendanceData) {
      const total = attendanceData.length
      const presentCount = attendanceData.filter(a => a.status === 'Present').length
      
      attendanceStats.value = {
        overall: total > 0 ? Math.round((presentCount / total) * 100) : 0,
        present: presentCount,
        absent: total - presentCount
      }

      // Map for Activity Timeline
      recentActivity.value = attendanceData.map(item => ({
        id: item.attendance_id,
        status: item.status.toLowerCase(),
        time: new Date(item.timestamp).toLocaleDateString('en-US', {
          month: 'short', day: 'numeric', year: 'numeric'
        }) + ' • ' + new Date(item.timestamp).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
      }))

      // Map for Calendar
      attendanceRecords.value = attendanceData.map(record => ({
        date: record.timestamp.split('T')[0],
        status: record.status
      }))
    }

  } catch (error) {
    console.error('Data sync failed:', error)
  } finally {
    isLoading.value = false
  }
}

onMounted(() => {
  fetchStudentData()
})
</script>

<style scoped>
.student-dashboard {
  min-height: 100vh;
}

.main-content {
  padding-bottom: 5rem;
}

.dashboard-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 1.5rem;
}

/* Desktop Grid Layout */
.dashboard-grid {
  display: grid;
  grid-template-columns: 1fr 380px;
  gap: 2rem;
  align-items: start;
}

.grid-main {
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

.grid-sidebar {
  display: flex;
  flex-direction: column;
  gap: 2rem;
  position: sticky;
  top: 90px;
}

/* Tablet: Adjusted Layout */
@media (max-width: 1024px) {
  .dashboard-grid {
    grid-template-columns: 1fr;
    gap: 2rem;
  }
  
  .grid-sidebar {
    position: static;
    order: 2;
  }
  
  .grid-main {
    order: 1;
  }
}

/* Mobile: Optimized Layout */
@media (max-width: 768px) {
  .dashboard-container {
    max-width: 100%;
    padding: 1rem;
  }
  
  .dashboard-grid {
    gap: 1.5rem;
  }
  
  .grid-main,
  .grid-sidebar {
    gap: 1.5rem;
  }
}

/* Small Mobile: Compact Layout */
@media (max-width: 640px) {
  .dashboard-container {
    padding: 0.75rem;
  }
  
  .dashboard-grid {
    gap: 1rem;
  }
  
  .grid-main,
  .grid-sidebar {
    gap: 1rem;
  }

  .stats-row {
    flex-direction: column;
  }
  
  .action-area {
    position: relative;
  }
  
  .fab-edit {
    position: absolute;
    bottom: 1rem;
    right: 1rem;
  }
}

/* Extra Small Mobile: Minimal Layout */
@media (max-width: 480px) {
  .dashboard-container {
    padding: 0.5rem;
  }
  
  .dashboard-grid {
    gap: 0.75rem;
  }
}

/* Barcode Section */
.barcode-skeleton {
  width: 100%;
}


/* Stats */
.stats-row {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
}

.stat-card {
  flex: 1;
  padding: 1.25rem;
  border-radius: 20px;
  box-shadow: var(--shadow-soft);
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.blue-card {
  background: var(--primary);
  color: white;
}

.white-card {
  background: var(--bg-card);
  color: var(--text-main);
}

.stat-card .label {
  font-size: 0.7rem;
  font-weight: 800;
  letter-spacing: 0.05em;
  opacity: 0.8;
}

.stat-card .value {
  font-size: 1.75rem;
  font-weight: 800;
}

.stat-card .value.danger {
  color: var(--error);
}

/* Heatmap */
.heatmap-section {
  margin-bottom: 2rem;
}

.calendar-surface {
  padding: 1.5rem;
  border: none;
  background: var(--bg-card);
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.25rem;
}

.section-header h3 {
  font-size: 1.125rem;
  font-weight: 800;
  color: var(--text-main);
}

.selector {
  font-size: 0.875rem;
  color: var(--text-muted);
  font-weight: 600;
  background: var(--bg-main);
  padding: 0.25rem 0.75rem;
  border-radius: 8px;
}

/* Actions */
.action-area {
  display: flex;
  gap: 1rem;
  align-items: center;
  margin-bottom: 2.5rem;
}

.contact-button {
  height: 3.5rem;
  border-radius: 16px;
  font-weight: 700;
}

.fab-edit {
  width: 3.5rem;
  height: 3.5rem;
  border-radius: 16px;
  border: none;
  background: var(--bg-card);
  color: var(--text-main);
  font-size: 1.25rem;
  cursor: pointer;
  box-shadow: var(--shadow-soft);
}
</style>
