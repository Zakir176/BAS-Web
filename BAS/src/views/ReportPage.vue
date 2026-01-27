<template>
  <div class="report-page">
    <Navbar />
    
    <main class="main-content">
      <div class="container py-8">
        <!-- Header Section -->
        <header class="page-header-v2">
          <div class="header-left">
            <span class="badge-v2">DATA & ANALYTICS</span>
            <h1>Attendance <span class="highlight">Intelligence</span></h1>
            <p>Comprehensive records and performance insights.</p>
          </div>
          <div class="header-actions">
            <Button variant="secondary" @click="refreshData">
              <span>Sync Data</span>
            </Button>
            <Button variant="primary" @click="exportReport" class="export-btn">
              <span>Export CSV</span>
            </Button>
          </div>
        </header>

        <!-- Stats Grid -->
        <section class="premium-stats-bar">
          <div class="stat-box-v2">
            <div class="stat-label">TOTAL RECORDS</div>
            <div class="stat-value">{{ summaryStats.totalClasses }}</div>
            <div class="stat-trend">+12% vs last month</div>
          </div>
          <div class="stat-box-v2">
            <div class="stat-label">TOTAL PRESENT</div>
            <div class="stat-value text-success">{{ summaryStats.present }}</div>
            <div class="stat-trend text-success">Positive trend</div>
          </div>
          <div class="stat-box-v2">
            <div class="stat-label">TOTAL ABSENT</div>
            <div class="stat-value text-error">{{ summaryStats.absent }}</div>
            <div class="stat-trend text-error">-2% improvement</div>
          </div>
          <div class="stat-box-v2 highlight-box">
            <div class="stat-label">AVG. COMPLIANCE</div>
            <div class="stat-value">{{ summaryStats.percentage }}%</div>
            <div class="mini-chart">
              <div class="bar-bg">
                <div class="bar-fill" :style="{ width: summaryStats.percentage + '%' }"></div>
              </div>
            </div>
          </div>
        </section>

        <!-- Filter Bar -->
        <div class="filter-bar-v2">
          <div class="filter-group">
            <div class="filter-field">
              <label>Timeframe</label>
              <select v-model="filters.dateRange">
                <option value="week">Last 7 Days</option>
                <option value="month">Last 30 Days</option>
                <option value="semester">This Semester</option>
              </select>
            </div>
            <div class="filter-field">
              <label>Course Subject</label>
              <select v-model="filters.course">
                <option value="all">Global (All Courses)</option>
                <option v-for="course in courses" :key="course.course_id" :value="course.course_id">
                  {{ course.course_name }}
                </option>
              </select>
            </div>
            <div class="filter-field">
              <label>Status Filter</label>
              <select v-model="filters.status">
                <option value="all">All Statuses</option>
                <option value="present">Present Only</option>
                <option value="absent">Absent Only</option>
              </select>
            </div>
          </div>
          <Button variant="primary" size="lg" @click="applyFilters" class="apply-btn">Refine View</Button>
        </div>

        <!-- Data Table -->
        <div class="table-card-v2 shadow-soft">
          <div class="card-header-v2">
            <h3>Detailed Session Log</h3>
            <div class="search-box-v2">
              <input v-model="searchQuery" placeholder="Search students or dates..." />
            </div>
          </div>
          
          <div class="table-responsive">
            <table class="premium-table">
              <thead>
                <tr>
                  <th>DATE</th>
                  <th>COURSE UNIT</th>
                  <th>SESSION TIME</th>
                  <th>STATUS</th>
                  <th class="text-right">ACTION</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="record in paginatedRecords" :key="record.id">
                  <td><span class="date-txt">{{ record.date }}</span></td>
                  <td><span class="course-name-txt">{{ record.course }}</span></td>
                  <td><span class="time-txt">{{ record.time }}</span></td>
                  <td>
                    <span class="status-pill-v2" :class="record.status.toLowerCase()">
                      {{ record.status }}
                    </span>
                  </td>
                  <td class="text-right">
                    <button class="action-btn-circle" @click="viewDetails(record.id)">⋯</button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>

          <div class="pagination-v2">
            <span class="page-count">Page {{ currentPage }} of {{ totalPages }}</span>
            <div class="page-btns">
              <button @click="previousPage" :disabled="currentPage === 1">←</button>
              <button @click="nextPage" :disabled="currentPage === totalPages">→</button>
            </div>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { supabase } from '@/supabase'
import { useAuth } from '@/composables/useAuth'
import Navbar from '@/components/layout/Navbar.vue'
import Button from '@/components/ui/Button.vue'

const { user } = useAuth()
const filters = ref({ dateRange: 'month', course: 'all', status: 'all' })
const courses = ref([])
const summaryStats = ref({ totalClasses: 0, present: 0, absent: 0, percentage: 0 })
const attendanceRecords = ref([])
const isLoading = ref(true)
const searchQuery = ref('')
const currentPage = ref(1)
const itemsPerPage = 10

const fetchCourses = async () => {
  if (!user.value) return
  const { data } = await supabase.from('courses').select('course_id, course_name').eq('teacher_id', user.value.id)
  if (data) courses.value = data
}

const fetchReportData = async () => {
  if (!user.value) return
  isLoading.value = true
  try {
    let query = supabase.from('attendance').select(`
      attendance_id, status, timestamp, sessions!inner(session_date, session_time, courses!inner(course_name, teacher_id))
    `).eq('sessions.courses.teacher_id', user.value.id)

    if (filters.value.course !== 'all') query = query.eq('sessions.course_id', filters.value.course)
    if (filters.value.status !== 'all') query = query.eq('status', filters.value.status.charAt(0).toUpperCase() + filters.value.status.slice(1))

    const { data, error } = await query.order('timestamp', { ascending: false })
    if (error) throw error

    attendanceRecords.value = data.map(record => ({
      id: record.attendance_id,
      date: new Date(record.sessions.session_date).toLocaleDateString(),
      course: record.sessions.courses.course_name,
      time: record.sessions.session_time || 'N/A',
      status: record.status
    }))

    const total = attendanceRecords.value.length
    const present = attendanceRecords.value.filter(r => r.status === 'Present').length
    const absent = attendanceRecords.value.filter(r => r.status === 'Absent').length
    summaryStats.value = {
      totalClasses: total,
      present: present,
      absent: absent,
      percentage: total > 0 ? Math.round((present / total) * 100) : 0
    }
  } catch (err) {
    console.error(err)
  } finally { isLoading.value = false }
}

const totalPages = computed(() => Math.ceil(filteredRecords.value.length / itemsPerPage) || 1)
const filteredRecords = computed(() => {
  let records = [...attendanceRecords.value]
  if (searchQuery.value) {
    const q = searchQuery.value.toLowerCase()
    records = records.filter(r => r.course.toLowerCase().includes(q) || r.date.includes(q))
  }
  return records
})
const paginatedRecords = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage
  return filteredRecords.value.slice(start, start + itemsPerPage)
})

const applyFilters = () => { currentPage.value = 1; fetchReportData() }
const refreshData = () => fetchReportData()
const exportReport = () => { /* CSV logic */ }
const previousPage = () => { if (currentPage.value > 1) currentPage.value-- }
const nextPage = () => { if (currentPage.value < totalPages.value) currentPage.value++ }

onMounted(async () => {
  await fetchCourses()
  await fetchReportData()
})
</script>

<style scoped>
.report-page {
  min-height: 100vh;
  background-color: var(--bg-main);
}

.page-header-v2 {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 3rem;
}

.badge-v2 {
  display: inline-block;
  padding: 4px 12px;
  background: var(--primary);
  color: var(--text-inverse);
  font-size: 0.75rem;
  font-weight: 800;
  border-radius: 100px;
  margin-bottom: 0.5rem;
}

.header-left h1 {
  font-size: 2.25rem;
  font-weight: 900;
  color: var(--text-main);
}

.highlight { color: var(--primary); }

.header-actions {
  display: flex;
  gap: 1rem;
}

/* Premium Stats Bar */
.premium-stats-bar {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 1.5rem;
  margin-bottom: 3rem;
}

.stat-box-v2 {
  background: var(--bg-card);
  padding: 1.5rem;
  border-radius: 20px;
  box-shadow: 0 4px 12px rgba(0,0,0,0.03);
}

.highlight-box {
  background: var(--primary);
  color: var(--text-inverse);
}

.stat-label {
  font-size: 0.7rem;
  font-weight: 800;
  letter-spacing: 0.05em;
  color: var(--text-muted);
  margin-bottom: 0.5rem;
}

.highlight-box .stat-label { color: var(--text-inverse); opacity: 0.8; }

.stat-value {
  font-size: 2rem;
  font-weight: 800;
  color: var(--text-main);
  margin-top: 0.5rem;
}

.stat-trend {
  font-size: 0.75rem;
  font-weight: 700;
  margin-top: 0.5rem;
}

.text-success { color: var(--success); }
.text-error { color: var(--error); }

.mini-chart {
  margin-top: 1rem;
}

.bar-bg { height: 6px; background: rgba(255,255,255,0.1); border-radius: 3px; }
.bar-fill { height: 100%; background: var(--primary); border-radius: 3px; }

/* Filter Bar */
.filter-bar-v2 {
  background: var(--bg-card);
  padding: 1rem;
  border-radius: 20px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 2rem;
  margin-bottom: 3rem;
  box-shadow: 0 2px 8px rgba(0,0,0,0.02);
}

.filter-group {
  display: flex;
  gap: 2.5rem;
  flex: 1;
}

.filter-field label {
  display: block;
  font-size: 0.7rem;
  font-weight: 800;
  color: var(--text-muted);
  margin-bottom: 0.25rem;
  text-transform: uppercase;
}

.filter-field select {
  background: transparent;
  border: none;
  font-size: 0.95rem;
  font-weight: 700;
  color: var(--text-main);
  cursor: pointer;
  padding: 0;
}

/* Table Card */
.table-card-v2 {
  background: var(--bg-card);
  border-radius: 24px;
  overflow: hidden;
}

.card-header-v2 {
  padding: 1.5rem 2rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-bottom: 1px solid var(--border-light);
}

.card-header-v2 h3 { font-weight: 800; color: var(--text-main); }

.search-box-v2 input {
  background: var(--bg-main);
  border: none;
  padding: 0.75rem 1.25rem;
  border-radius: 12px;
  font-size: 0.9rem;
  width: 280px;
  font-weight: 600;
  color: var(--text-main);
}

.premium-table {
  width: 100%;
  border-collapse: collapse;
}

.premium-table th {
  padding: 1.25rem 2rem;
  text-align: left;
  font-size: 0.7rem;
  font-weight: 800;
  color: var(--text-muted);
  letter-spacing: 0.05em;
  background: var(--bg-card);
}

.premium-table td {
  padding: 1.25rem 2rem;
  border-bottom: 1px solid var(--border-light);
}

.date-txt { font-weight: 800; color: var(--text-main); }
.course-name-txt { font-weight: 700; color: var(--text-muted); }
.time-txt { color: var(--text-muted); font-weight: 600; font-size: 0.9rem; }

.status-pill-v2 {
  font-size: 0.7rem;
  font-weight: 800;
  text-transform: uppercase;
  padding: 4px 10px;
  border-radius: 6px;
}

.status-pill-v2.present { background: #dcfce7; color: #059669; }
.status-pill-v2.absent { background: #fee2e2; color: #dc2626; }

.action-btn-circle {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  border: none;
  background: var(--bg-main);
  color: var(--text-muted);
  cursor: pointer;
  font-weight: 800;
}

.pagination-v2 {
  padding: 1.5rem 2rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.page-count { font-size: 0.85rem; font-weight: 700; color: var(--text-muted); }

.page-btns { display: flex; gap: 0.5rem; }
.page-btns button {
  width: 36px;
  height: 36px;
  border: 1px solid var(--border-medium);
  border-radius: 10px;
  background: var(--bg-card);
  cursor: pointer;
  font-weight: 800;
  transition: all 0.2s;
  color: var(--text-main);
}

.page-btns button:hover:not(:disabled) { background: var(--bg-main); border-color: var(--border-medium); }

@media (max-width: 1024px) {
  .premium-stats-bar { grid-template-columns: repeat(2, 1fr); }
  .filter-bar-v2 { flex-direction: column; align-items: stretch; gap: 1.5rem; }
  .filter-group { flex-direction: column; gap: 1rem; }
}
</style>
