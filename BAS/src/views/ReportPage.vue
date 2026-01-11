<template>
  <div class="report-page">
    <Navbar />
    
    <main class="main-content">
      <div class="container">
        <!-- Header Section -->
        <section class="header-section">
          <div class="header-content">
            <h1>Attendance Reports</h1>
            <p class="header-subtitle">View and analyze your attendance data</p>
          </div>
          <div class="header-actions">
            <Button variant="primary" @click="exportReport">
              <svg width="20" height="20" viewBox="0 0 20 20" fill="currentColor">
                <path d="M3 17a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zm3.293-7.707a1 1 0 011.414 0L9 10.586V3a1 1 0 112 0v7.586l1.293-1.293a1 1 0 111.414 1.414l-3 3a1 1 0 01-1.414 0l-3-3a1 1 0 010-1.414z"/>
              </svg>
              Export Report
            </Button>
            <Button variant="secondary" @click="refreshData">
              <svg width="20" height="20" viewBox="0 0 20 20" fill="currentColor">
                <path fill-rule="evenodd" d="M4 2a1 1 0 011 1v2.101a7.002 7.002 0 0111.601 2.566 1 1 0 11-1.885.666A5.002 5.002 0 005.999 7H9a1 1 0 010 2H4a1 1 0 01-1-1V3a1 1 0 011-1zm.008 9.057a1 1 0 011.276.61A5.002 5.002 0 0014.001 13H11a1 1 0 110-2h5a1 1 0 011 1v5a1 1 0 11-2 0v-2.101a7.002 7.002 0 01-11.601-2.566 1 1 0 01.61-1.276z" clip-rule="evenodd"/>
              </svg>
              Refresh
            </Button>
          </div>
        </section>

        <!-- Filter Section -->
        <section class="filter-section">
          <Card class="filter-card">
            <div class="filter-grid">
              <div class="filter-item">
                <label class="filter-label">Date Range</label>
                <select v-model="filters.dateRange" class="filter-select">
                  <option value="week">Last 7 Days</option>
                  <option value="month">Last 30 Days</option>
                  <option value="semester">This Semester</option>
                  <option value="year">This Year</option>
                  <option value="custom">Custom Range</option>
                </select>
              </div>
              
              <div class="filter-item">
                <label class="filter-label">Course</label>
                <select v-model="filters.course" class="filter-select">
                  <option value="all">All Courses</option>
                  <option value="cs101">Computer Science 101</option>
                  <option value="math201">Mathematics 201</option>
                  <option value="phys101">Physics 101</option>
                  <option value="chem101">Chemistry 101</option>
                </select>
              </div>
              
              <div class="filter-item">
                <label class="filter-label">Status</label>
                <select v-model="filters.status" class="filter-select">
                  <option value="all">All Status</option>
                  <option value="present">Present Only</option>
                  <option value="absent">Absent Only</option>
                  <option value="late">Late Only</option>
                </select>
              </div>
              
              <div class="filter-item">
                <Button variant="primary" @click="applyFilters">Apply Filters</Button>
              </div>
            </div>
          </Card>
        </section>

        <!-- Summary Cards -->
        <section class="summary-section">
          <div class="summary-grid">
            <Card class="summary-card">
              <div class="summary-icon total">
                <svg width="32" height="32" viewBox="0 0 32 32" fill="currentColor">
                  <path d="M16 2C8.268 2 2 8.268 2 16s6.268 14 14 14 14-6.268 14-14S23.732 2 16 2zm0 2c6.627 0 12 5.373 12 12s-5.373 12-12 12S4 22.627 4 16 9.373 4 16 4z"/>
                  <path d="M16 8v8l4 4"/>
                </svg>
              </div>
              <div class="summary-content">
                <div class="summary-number">{{ summaryStats.totalClasses }}</div>
                <div class="summary-label">Total Classes</div>
              </div>
            </Card>

            <Card class="summary-card">
              <div class="summary-icon present">
                <svg width="32" height="32" viewBox="0 0 32 32" fill="currentColor">
                  <path d="M16 2l4.586 4.586L26 6.172l2.828 2.828L28.586 16 26 18.586 23.172 26 16 28.586 13.414 26 6.172 23.172 3.344 16 5.414 13.414 8.828 6.172 16 2z"/>
                </svg>
              </div>
              <div class="summary-content">
                <div class="summary-number">{{ summaryStats.present }}</div>
                <div class="summary-label">Classes Attended</div>
              </div>
            </Card>

            <Card class="summary-card">
              <div class="summary-icon absent">
                <svg width="32" height="32" viewBox="0 0 32 32" fill="currentColor">
                  <path d="M16 2C8.268 2 2 8.268 2 16s6.268 14 14 14 14-6.268 14-14S23.732 2 16 2zm0 2c6.627 0 12 5.373 12 12s-5.373 12-12 12S4 22.627 4 16 9.373 4 16 4zm-1 5v8h8v-2h-6V9h-2z"/>
                </svg>
              </div>
              <div class="summary-content">
                <div class="summary-number">{{ summaryStats.absent }}</div>
                <div class="summary-label">Classes Missed</div>
              </div>
            </Card>

            <Card class="summary-card">
              <div class="summary-icon percentage">
                <svg width="32" height="32" viewBox="0 0 32 32" fill="currentColor">
                  <path d="M16 2C8.268 2 2 8.268 2 16s6.268 14 14 14 14-6.268 14-14S23.732 2 16 2zm0 2c6.627 0 12 5.373 12 12s-5.373 12-12 12S4 22.627 4 16 9.373 4 16 4z"/>
                  <path d="M16 8v8l6 3"/>
                </svg>
              </div>
              <div class="summary-content">
                <div class="summary-number">{{ summaryStats.percentage }}%</div>
                <div class="summary-label">Attendance Rate</div>
              </div>
            </Card>
          </div>
        </section>

        <!-- Charts Section -->
        <section class="charts-section">
          <div class="charts-grid">
            <Card class="chart-card">
              <h3>Attendance Trend</h3>
              <div class="chart-placeholder">
                <svg width="100%" height="200" viewBox="0 0 400 200" fill="none">
                  <rect width="400" height="200" fill="var(--bg-tertiary)" rx="8"/>
                  <polyline
                    points="20,180 60,160 100,140 140,150 180,120 220,130 260,110 300,100 340,90 380,85"
                    stroke="var(--accent-primary)"
                    stroke-width="3"
                    fill="none"
                  />
                  <circle cx="380" cy="85" r="5" fill="var(--accent-primary)"/>
                </svg>
              </div>
            </Card>

            <Card class="chart-card">
              <h3>Course Distribution</h3>
              <div class="chart-placeholder">
                <svg width="100%" height="200" viewBox="0 0 400 200" fill="none">
                  <rect width="400" height="200" fill="var(--bg-tertiary)" rx="8"/>
                  <rect x="50" y="150" width="60" height="30" fill="var(--accent-primary)"/>
                  <rect x="130" y="120" width="60" height="60" fill="var(--success)"/>
                  <rect x="210" y="100" width="60" height="80" fill="var(--warning)"/>
                  <rect x="290" y="140" width="60" height="40" fill="var(--error)"/>
                </svg>
              </div>
            </Card>
          </div>
        </section>

        <!-- Detailed Records Table -->
        <section class="records-section">
          <Card class="records-card">
            <div class="records-header">
              <h3>Detailed Attendance Records</h3>
              <div class="records-actions">
                <Input
                  v-model="searchQuery"
                  placeholder="Search records..."
                  type="text"
                />
              </div>
            </div>
            
            <div class="table-container">
              <table class="records-table">
                <thead>
                  <tr>
                    <th>Date</th>
                    <th>Course</th>
                    <th>Time</th>
                    <th>Lecturer</th>
                    <th>Status</th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="record in filteredRecords" :key="record.id">
                    <td>{{ record.date }}</td>
                    <td>{{ record.course }}</td>
                    <td>{{ record.time }}</td>
                    <td>{{ record.lecturer }}</td>
                    <td>
                      <span class="status-badge" :class="record.status.toLowerCase()">
                        {{ record.status }}
                      </span>
                    </td>
                    <td>
                      <Button variant="secondary" size="sm" @click="viewDetails(record.id)">
                        View
                      </Button>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
            
            <div class="pagination">
              <Button variant="secondary" size="sm" :disabled="currentPage === 1" @click="previousPage">
                Previous
              </Button>
              <span class="page-info">Page {{ currentPage }} of {{ totalPages }}</span>
              <Button variant="secondary" size="sm" :disabled="currentPage === totalPages" @click="nextPage">
                Next
              </Button>
            </div>
          </Card>
        </section>
      </div>
    </main>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import Navbar from '@/components/layout/Navbar.vue'
import Button from '@/components/ui/Button.vue'
import Card from '@/components/ui/Card.vue'
import Input from '@/components/ui/Input.vue'

// Mock data - replace with actual Supabase calls
const filters = ref({
  dateRange: 'month',
  course: 'all',
  status: 'all'
})

const summaryStats = ref({
  totalClasses: 150,
  present: 138,
  absent: 12,
  percentage: 92
})

const searchQuery = ref('')
const currentPage = ref(1)
const totalPages = ref(5)

const attendanceRecords = ref([
  {
    id: 1,
    date: '2024-12-11',
    course: 'Computer Science 101',
    time: '10:00 AM',
    lecturer: 'Dr. Smith',
    status: 'Present'
  },
  {
    id: 2,
    date: '2024-12-10',
    course: 'Mathematics 201',
    time: '2:00 PM',
    lecturer: 'Prof. Johnson',
    status: 'Present'
  },
  {
    id: 3,
    date: '2024-12-09',
    course: 'Physics 101',
    time: '11:00 AM',
    lecturer: 'Dr. Brown',
    status: 'Absent'
  },
  {
    id: 4,
    date: '2024-12-08',
    course: 'Chemistry Lab',
    time: '3:00 PM',
    lecturer: 'Dr. Davis',
    status: 'Present'
  },
  {
    id: 5,
    date: '2024-12-07',
    course: 'Computer Science 101',
    time: '10:00 AM',
    lecturer: 'Dr. Smith',
    status: 'Late'
  }
])

const filteredRecords = computed(() => {
  let records = [...attendanceRecords.value]
  
  if (searchQuery.value) {
    records = records.filter(record => 
      record.course.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      record.lecturer.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      record.date.includes(searchQuery.value)
    )
  }
  
  if (filters.value.course !== 'all') {
    records = records.filter(record => 
      record.course.toLowerCase().includes(filters.value.course)
    )
  }
  
  if (filters.value.status !== 'all') {
    records = records.filter(record => 
      record.status.toLowerCase() === filters.value.status
    )
  }
  
  return records
})

const applyFilters = () => {
  console.log('Applying filters:', filters.value)
  // Apply filters logic here
}

const exportReport = () => {
  console.log('Exporting report...')
  // Export logic here
  alert('Report would be exported as PDF/Excel')
}

const refreshData = () => {
  console.log('Refreshing data...')
  // Refresh data from Supabase
}

const viewDetails = (recordId) => {
  console.log('Viewing details for record:', recordId)
  // View record details
}

const previousPage = () => {
  if (currentPage.value > 1) {
    currentPage.value--
  }
}

const nextPage = () => {
  if (currentPage.value < totalPages.value) {
    currentPage.value++
  }
}

onMounted(() => {
  console.log('Report page loaded')
})
</script>

<style scoped>
.report-page {
  min-height: 100vh;
  background-color: var(--bg-primary);
}

.main-content {
  padding: 2rem 0;
}

.header-section {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 2rem;
  flex-wrap: wrap;
  gap: 1rem;
}

.header-content h1 {
  font-size: 2.5rem;
  font-weight: 700;
  color: var(--text-primary);
  margin-bottom: 0.5rem;
}

.header-subtitle {
  color: var(--text-secondary);
  font-size: 1.125rem;
  margin: 0;
}

.header-actions {
  display: flex;
  gap: 1rem;
}

.filter-section {
  margin-bottom: 2rem;
}

.filter-card {
  padding: 1.5rem;
}

.filter-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1rem;
  align-items: end;
}

.filter-item {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.filter-label {
  font-size: 0.875rem;
  font-weight: 500;
  color: var(--text-primary);
}

.filter-select {
  padding: 0.75rem;
  border: 1px solid var(--border-primary);
  border-radius: 0.5rem;
  background-color: var(--input-bg);
  color: var(--text-primary);
  font-size: 0.875rem;
}

.filter-select:focus {
  outline: none;
  border-color: var(--accent-primary);
  box-shadow: 0 0 0 3px rgb(59 130 246 / 0.1);
}

.summary-section {
  margin-bottom: 2rem;
}

.summary-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1.5rem;
}

.summary-card {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1.5rem;
}

.summary-icon {
  width: 3rem;
  height: 3rem;
  border-radius: 0.75rem;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
}

.summary-icon.total {
  background-color: var(--accent-primary);
}

.summary-icon.present {
  background-color: var(--success);
}

.summary-icon.absent {
  background-color: var(--error);
}

.summary-icon.percentage {
  background-color: var(--warning);
}

.summary-content {
  flex: 1;
}

.summary-number {
  font-size: 2rem;
  font-weight: 700;
  color: var(--text-primary);
  line-height: 1;
}

.summary-label {
  color: var(--text-secondary);
  font-size: 0.875rem;
  margin-top: 0.25rem;
}

.charts-section {
  margin-bottom: 2rem;
}

.charts-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
  gap: 1.5rem;
}

.chart-card {
  padding: 1.5rem;
}

.chart-card h3 {
  font-size: 1.125rem;
  font-weight: 600;
  color: var(--text-primary);
  margin-bottom: 1rem;
}

.chart-placeholder {
  border-radius: 0.5rem;
  overflow: hidden;
}

.records-section {
  margin-bottom: 2rem;
}

.records-card {
  padding: 1.5rem;
}

.records-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
  flex-wrap: wrap;
  gap: 1rem;
}

.records-header h3 {
  font-size: 1.25rem;
  font-weight: 600;
  color: var(--text-primary);
}

.records-actions {
  min-width: 300px;
}

.table-container {
  overflow-x: auto;
  margin-bottom: 1.5rem;
}

.records-table {
  width: 100%;
  border-collapse: collapse;
}

.records-table th,
.records-table td {
  padding: 0.75rem;
  text-align: left;
  border-bottom: 1px solid var(--border-primary);
}

.records-table th {
  font-weight: 600;
  color: var(--text-primary);
  background-color: var(--bg-secondary);
}

.records-table td {
  color: var(--text-secondary);
}

.status-badge {
  display: inline-block;
  padding: 0.25rem 0.75rem;
  border-radius: 1rem;
  font-size: 0.75rem;
  font-weight: 500;
}

.status-badge.present {
  background-color: var(--success);
  color: white;
}

.status-badge.absent {
  background-color: var(--error);
  color: white;
}

.status-badge.late {
  background-color: var(--warning);
  color: white;
}

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 1rem;
}

.page-info {
  color: var(--text-secondary);
  font-size: 0.875rem;
}

@media (max-width: 768px) {
  .header-section {
    flex-direction: column;
    text-align: center;
  }
  
  .header-actions {
    flex-direction: column;
    width: 100%;
  }
  
  .filter-grid {
    grid-template-columns: 1fr;
  }
  
  .summary-grid {
    grid-template-columns: 1fr;
  }
  
  .charts-grid {
    grid-template-columns: 1fr;
  }
  
  .records-header {
    flex-direction: column;
    text-align: center;
  }
  
  .records-actions {
    min-width: auto;
    width: 100%;
  }
  
  .table-container {
    font-size: 0.875rem;
  }
  
  .pagination {
    flex-direction: column;
    gap: 0.5rem;
  }
}
</style>
