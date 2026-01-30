<template>
  <div class="student-dashboard">
    <Navbar />
    
    <main class="main-content">
      <div class="container mobile-friendly">
        <!-- Profile Header -->
        <header class="profile-header">
          <div class="profile-main">
            <div class="avatar-container">
              <div class="avatar-ring">
                <Skeleton v-if="isLoading" width="100px" height="100px" shape="circle" />
                <img v-else :src="`https://ui-avatars.com/api/?name=${studentName || 'Student'}&background=random&size=128`" alt="Avatar" class="avatar-img">
                <div class="status-indicator online"></div>
              </div>
            </div>
            <div class="profile-text">
              <Skeleton v-if="isLoading" width="200px" height="2rem" style="margin: 0 auto 0.5rem" />
              <h1 v-else class="student-name">{{ studentName }}</h1>
              <Skeleton v-if="isLoading" width="150px" height="1rem" style="margin: 0 auto" />
              <p v-else class="student-meta">ID: {{ studentProfile?.student_id || 'STU-001' }} | Class: {{ classSection }}</p>
            </div>
          </div>
          <div class="header-actions">
            <button class="icon-btn" title="View Profile">👤</button>
            <button class="icon-btn" title="Settings">⚙️</button>
          </div>
        </header>

        <!-- Barcode Section -->
        <section v-if="!isLoading && studentProfile?.student_id" class="barcode-section">
          <StudentBarcode :student-id="studentProfile.student_id" />
        </section>
        <section v-else-if="isLoading" class="barcode-section">
          <div class="barcode-skeleton">
            <Skeleton width="100%" height="200px" radius="20px" />
          </div>
        </section>

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

        <!-- History Timeline -->
        <section class="history-section">
          <div class="history-header">
            <h3>Recent Activity</h3>
          </div>
          <div class="timeline">
            <template v-if="isLoading">
              <div v-for="i in 3" :key="i" class="timeline-item shadow-sm">
                <Skeleton width="40px" height="40px" radius="12px" />
                <div class="item-content">
                  <Skeleton width="100px" height="1rem" style="margin-bottom: 0.5rem" />
                  <Skeleton width="150px" height="0.75rem" />
                </div>
              </div>
            </template>
            <template v-else>
              <div v-for="item in visibleActivity" :key="item.id" class="timeline-item shadow-sm">
                <div class="item-icon-box" :class="item.status">
                  <span class="icon">{{ item.status === 'present' ? '✓' : '✕' }}</span>
                </div>
                <div class="item-content">
                  <div class="item-top">
                    <span class="item-status">{{ item.status.toUpperCase() }}</span>
                    <span class="badge" :class="item.status">{{ item.status === 'present' ? 'VERIFIED' : 'UNEXCUSED' }}</span>
                  </div>
                  <div class="item-meta">{{ item.time }}</div>
                </div>
              </div>
            </template>
          </div>
          <button 
            v-if="!isLoading && recentActivity.length > 5" 
            class="view-all-link"
            @click="showAllHistory = !showAllHistory"
          >
            {{ showAllHistory ? 'Show Less' : 'View All History' }}
          </button>
        </section>
      </div>
    </main>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '@/composables/useAuth'
import { useToast } from '@/composables/useToast'
import { supabase } from '@/supabase'
import Navbar from '@/components/common/Navbar.vue'
import Button from '@/components/ui/Button.vue'
import Skeleton from '@/components/ui/Skeleton.vue'
import AttendanceCalendar from '@/components/student/AttendanceCalendar.vue'
import StudentBarcode from '@/components/student/StudentBarcode.vue'

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

const visibleActivity = computed(() => {
  if (showAllHistory.value) return recentActivity.value
  return recentActivity.value.slice(0, 5)
})

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
      recentActivity.value = attendanceData.slice(0, 5).map(item => ({
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
  background-color: #f8fafc;
}

.main-content {
  padding-bottom: 5rem;
}

.mobile-friendly {
  max-width: 480px;
  margin: 0 auto;
  padding: 1.5rem;
}

/* Header */
.profile-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 2rem;
}

.profile-main {
  display: flex;
  flex-direction: column;
  align-items: center;
  flex: 1;
}

.avatar-container {
  position: relative;
  margin-bottom: 1rem;
}

.avatar-ring {
  width: 100px;
  height: 100px;
  padding: 4px;
  background: var(--bg-card);
  border-radius: 50%;
  box-shadow: var(--shadow-card);
}

.avatar-img {
  width: 100%;
  height: 100%;
  border-radius: 50%;
  object-fit: cover;
}

.status-indicator {
  position: absolute;
  bottom: 4px;
  right: 4px;
  width: 16px;
  height: 16px;
  background: var(--success);
  border: 3px solid var(--bg-card);
  border-radius: 50%;
}

.student-name {
  font-size: 1.5rem;
  font-weight: 800;
  color: #111827;
  text-align: center;
  margin-bottom: 0.25rem;
}

.student-meta {
  font-size: 0.875rem;
  color: #64748b;
  font-weight: 600;
}

.icon-btn {
  background: none;
  border: none;
  font-size: 1.25rem;
  cursor: pointer;
  padding: 0.5rem;
}

/* Barcode Section */
.barcode-section {
  margin-bottom: 2rem;
}

.barcode-skeleton {
  width: 100%;
}


/* Stats */
.stats-row {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
}

/* Mobile responsive - stack stats on small screens */
@media (max-width: 640px) {
  .stats-row {
    flex-direction: column;
  }
  
  .profile-header {
    flex-direction: column;
    text-align: center;
    gap: 1rem;
  }
  
  .header-actions {
    justify-content: center;
  }
  
  .student-name {
    font-size: 1.5rem;
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

/* History */
.history-section {
  margin-bottom: 2.5rem;
}

.history-header h3 {
  font-size: 1.125rem;
  font-weight: 800;
  margin-bottom: 1rem;
  color: var(--text-main);
}

.timeline {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.timeline-item {
  background: var(--bg-card);
  padding: 1rem;
  border-radius: 18px;
  display: flex;
  align-items: center;
  gap: 1rem;
}

.item-icon-box {
  width: 40px;
  height: 40px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 900;
}

.item-icon-box.present { background: var(--success-bg); color: var(--success); }
.item-icon-box.absent { background: var(--error-bg); color: var(--error); }

.item-content { flex: 1; }

.item-top {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.25rem;
}

.item-status {
  font-weight: 800;
  font-size: 0.9rem;
  color: var(--text-main);
}

.badge {
  font-size: 0.65rem;
  font-weight: 800;
  padding: 0.25rem 0.5rem;
  border-radius: 6px;
}

.badge.present { background: var(--success-bg); color: var(--success); }
.badge.absent { background: var(--error-bg); color: var(--error); }

.item-meta {
  font-size: 0.75rem;
  color: var(--text-muted);
  font-weight: 500;
}

.view-all-link {
  display: block;
  width: 100%;
  margin-top: 1.5rem;
  padding: 1rem;
  background: var(--bg-card);
  border: 1.5px dashed var(--border-medium);
  border-radius: 16px;
  color: var(--text-muted);
  font-weight: 700;
  cursor: pointer;
  transition: all 0.2s;
}

.view-all-link:hover {
  background: var(--bg-main);
  border-color: var(--primary);
  color: var(--primary);
}

</style>
