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
                <img :src="`https://ui-avatars.com/api/?name=${studentName}&background=random&size=128`" alt="Avatar" class="avatar-img">
                <div class="status-indicator online"></div>
              </div>
            </div>
            <div class="profile-text">
              <h1 class="student-name">{{ studentName }}</h1>
              <p class="student-meta">ID: {{ profile?.student_id || 'STU-001' }} | Class: {{ classSection }}</p>
            </div>
          </div>
          <div class="header-actions">
            <button class="icon-btn">📄</button>
            <button class="icon-btn">⋮</button>
          </div>
        </header>

        <!-- Stats Overview -->
        <section class="stats-overview">
          <div class="stats-row">
            <div class="stat-card blue-card">
              <span class="label">ATTENDANCE</span>
              <div class="value">{{ attendanceStats.overall }}%</div>
            </div>
            <div class="stat-card white-card">
              <span class="label">ABSENCES</span>
              <div class="value danger">{{ attendanceStats.absent }}</div>
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
          <Button variant="primary" full-width size="xl" class="contact-button">
            <span class="btn-icon">✉️</span> Contact Parent
          </Button>
          <button class="fab-edit">✎</button>
        </section>

        <!-- History Timeline -->
        <section class="history-section">
          <div class="history-header">
            <h3>Recent Activity</h3>
          </div>
          <div class="timeline">
            <div v-for="item in recentActivity" :key="item.id" class="timeline-item shadow-sm">
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
          </div>
          <button class="view-all-link">View All History</button>
        </section>
      </div>
    </main>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '@/composables/useAuth'
import { useUserStore } from '@/stores/userStore'
import { supabase } from '@/supabase'
import Navbar from '@/components/layout/Navbar.vue'
import Button from '@/components/ui/Button.vue'
import AttendanceCalendar from '@/components/AttendanceCalendar.vue'

const router = useRouter()
const { user } = useAuth()
const { profile } = useUserStore()

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

const fetchStudentData = async () => {
  try {
    if (!user.value) return
    isLoading.value = true

    // 1. Get student profile details
    const { data: student } = await supabase
      .from('students')
      .select('*')
      .eq('email', user.value.email)
      .single()

    if (student) {
      studentName.value = student.full_name
      classSection.value = student.class_section || '10A'
    }

    // 2. Fetch specific attendance records
    const studentId = student?.student_id || profile.value?.student_id
    if (!studentId) return

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
  background: white;
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
  background: #10b981;
  border: 3px solid white;
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
  background: white;
  color: #111827;
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
  color: #111827;
}

.selector {
  font-size: 0.875rem;
  color: #64748b;
  font-weight: 600;
  background: #f1f5f9;
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
  background: #f1f5f9;
  color: #111827;
  font-size: 1.25rem;
  cursor: pointer;
}

/* History */
.history-section {
  margin-bottom: 2.5rem;
}

.history-header h3 {
  font-size: 1.125rem;
  font-weight: 800;
  margin-bottom: 1rem;
}

.timeline {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.timeline-item {
  background: white;
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

.item-icon-box.present { background: #ecfdf5; color: #10b981; }
.item-icon-box.absent { background: #fef2f2; color: #ef4444; }

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
  color: #111827;
}

.badge {
  font-size: 0.65rem;
  font-weight: 800;
  padding: 0.25rem 0.5rem;
  border-radius: 6px;
}

.badge.present { background: #d1fae5; color: #065f46; }
.badge.absent { background: #fee2e2; color: #991b1b; }

.item-meta {
  font-size: 0.75rem;
  color: #64748b;
  font-weight: 500;
}

.view-all-link {
  display: block;
  width: 100%;
  margin-top: 1.5rem;
  padding: 1rem;
  background: #f8fafc;
  border: 1.5px dashed #cbd5e1;
  border-radius: 16px;
  color: #64748b;
  font-weight: 700;
  cursor: pointer;
  transition: all 0.2s;
}

.view-all-link:hover {
  background: #f1f5f9;
  border-color: var(--primary);
  color: var(--primary);
}
</style>
