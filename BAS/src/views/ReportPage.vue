<template>
  <div class="report-page">
    <main class="main-content">
      <div class="container py-8">
        <!-- Premium Header -->
        <header class="page-header-v2">
          <div class="header-main">
            <span class="badge-v2">DATA & ANALYTICS</span>
            <h1>Attendance <span class="highlight">Intelligence</span></h1>
          </div>
          <div class="header-actions">
            <Button variant="secondary" size="sm" @click="exportReport" class="glass-btn">
              <svg width="16" height="16" viewBox="0 0 20 20" fill="currentColor">
                <path d="M3 17a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zm3.293-7.707a1 1 0 011.414 0L9 10.586V3a1 1 0 112 0v7.586l1.293-1.293a1 1 0 111.414 1.414l-3 3a1 1 0 01-1.414 0l-3-3a1 1 0 010-1.414z" />
              </svg>
              <span>Export</span>
            </Button>
            <Button variant="secondary" size="sm" @click="refreshData" class="glass-btn">
              <svg width="16" height="16" viewBox="0 0 20 20" fill="currentColor">
                <path fill-rule="evenodd" d="M4 2a1 1 0 011 1v2.101a7.002 7.002 0 0111.601 2.566 1 1 0 11-1.885.666A5.002 5.002 0 005.999 7H9a1 1 0 010 2H4a1 1 0 01-1-1V3a1 1 0 011-1zm.008 9.057a1 1 0 011.276.61A5.002 5.002 0 0014.001 13H11a1 1 0 110-2h5a1 1 0 011 1v5a1 1 0 11-2 0v-2.101a7.002 7.002 0 01-11.601-2.566 1 1 0 01.61-1.276z" clip-rule="evenodd" />
              </svg>
            </Button>
          </div>
        </header>

        <!-- Horizontal High-Density Filter Bar -->
        <section class="filter-toolbar glass-panel">
          <div class="toolbar-items">
            <div class="toolbar-item">
              <label>Range</label>
              <select v-model="filters.dateRange">
                <option value="week">Week</option>
                <option value="month">Month</option>
                <option value="semester">Semester</option>
              </select>
            </div>
            <div class="toolbar-divider"></div>
            <div class="toolbar-item">
              <label>Course</label>
              <select v-model="filters.course">
                <option value="all">All Courses</option>
                <option v-for="c in availableCourses" :key="c.id" :value="c.id">{{ c.name }}</option>
              </select>
            </div>
            <div class="toolbar-divider"></div>
            <div class="toolbar-item">
              <label>Status</label>
              <select v-model="filters.status">
                <option value="all">All Status</option>
                <option value="present">Present</option>
                <option value="absent">Absent</option>
              </select>
            </div>
          </div>
          <button class="apply-toolbar-btn" @click="applyFilters">Update Analytics</button>
        </section>

        <!-- Summary Tiles (High Density) -->
        <section class="metrics-grid">
          <div class="metric-tile glass-panel">
            <div class="tile-icon blue"><span>📊</span></div>
            <div class="tile-info">
              <span class="tile-label">Total Classes</span>
              <span class="tile-value">{{ summaryStats.totalClasses }}</span>
            </div>
          </div>
          <div class="metric-tile glass-panel">
            <div class="tile-icon emerald"><span>✅</span></div>
            <div class="tile-info">
              <span class="tile-label">Present</span>
              <span class="tile-value">{{ summaryStats.present }}</span>
            </div>
          </div>
          <div class="metric-tile glass-panel">
            <div class="tile-icon amber"><span>❌</span></div>
            <div class="tile-info">
              <span class="tile-label">Absent</span>
              <span class="tile-value">{{ summaryStats.absent }}</span>
            </div>
          </div>
          <div class="metric-tile glass-panel highlight">
            <div class="tile-icon indigo"><span>🎯</span></div>
            <div class="tile-info">
              <span class="tile-label">Rate</span>
              <span class="tile-value">{{ summaryStats.percentage }}%</span>
            </div>
          </div>
        </section>

        <!-- Analytics Visuals -->
        <section class="analytics-grid">
          <div class="visual-card glass-panel">
            <div class="visual-header">
              <h3>Attendance Trends</h3>
              <div class="visual-actions">
                <span class="trend-tag positive">↑ 12%</span>
              </div>
            </div>
            <div class="chart-box">
              <LineChart v-if="hasTrendData" :chart-data="trendChartData" />
              <div v-else class="chart-skeleton">
                <div class="skeleton-pulse"></div>
                <p>Establishing baseline...</p>
              </div>
            </div>
          </div>

          <div class="visual-card glass-panel">
            <div class="visual-header">
              <h3>Distribution</h3>
            </div>
            <div class="chart-box">
              <BarChart v-if="hasCourseData" :chart-data="courseChartData" />
              <div v-else class="chart-skeleton">
                <div class="skeleton-pulse"></div>
                <p>Waiting for course allocation...</p>
              </div>
            </div>
          </div>
        </section>

        <!-- Detailed Records Table -->
        <section class="records-section">
          <Card class="records-card">
            <div class="records-header">
              <h3>Detailed Attendance Records</h3>
              <div class="records-actions">
                <Input v-model="searchQuery" placeholder="Search records..." type="text" />
              </div>
            </div>

            <div class="table-container">
              <table class="records-table">
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

            <div class="pagination">
              <Button
                variant="secondary"
                size="sm"
                :disabled="currentPage === 1"
                @click="previousPage"
              >
                Previous
              </Button>
              <span class="page-info">Page {{ currentPage }} of {{ totalPages }}</span>
              <Button
                variant="secondary"
                size="sm"
                :disabled="currentPage === totalPages"
                @click="nextPage"
              >
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
import { ref, computed, onMounted } from "vue";
import { useRouter } from "vue-router";
import { supabase } from "@/supabase";
import Button from "@/components/ui/Button.vue";
import Card from "@/components/ui/Card.vue";
import Input from "@/components/ui/Input.vue";
import LineChart from "@/components/ui/charts/LineChart.vue";
import BarChart from "@/components/ui/charts/BarChart.vue";

const router = useRouter();

// Filters
const filters = ref({
  dateRange: "month",
  course: "all",
  status: "all",
});

const summaryStats = ref({
  totalClasses: 0,
  present: 0,
  absent: 0,
  percentage: 0,
});

const searchQuery = ref("");
const currentPage = ref(1);
const totalPages = ref(1);
const attendanceRecords = ref([]);

const fetchAttendanceData = async () => {
  try {
    const {
      data: { user },
    } = await supabase.auth.getUser();
    if (!user) {
      router.push("/student-login");
      return;
    }

    // Fetch activity logs
    const { data: logs, error } = await supabase
      .from("activity_logs")
      .select("*")
      .eq("student_id", user.id)
      .order("date_time", { ascending: false });

    if (error) throw error;

    attendanceRecords.value = logs.map((log) => ({
      id: log.id,
      date: new Date(log.date_time).toLocaleDateString(),
      course: "General Attendance",
      time: new Date(log.date_time).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" }),
      lecturer: "N/A",
      status: log.status || "Present",
    }));

    // Calculate Summary
    const total = logs.length;
    const presentCount = logs.filter((l) => l.status === "Present").length;
    summaryStats.value = {
      totalClasses: total,
      present: presentCount,
      absent: total - presentCount,
      percentage: total > 0 ? Math.round((presentCount / total) * 100) : 0,
    };

    totalPages.value = Math.ceil(attendanceRecords.value.length / 10) || 1;
  } catch (error) {
    console.error("Error fetching attendance data:", error);
  }
};

const filteredRecords = computed(() => {
  let records = [...attendanceRecords.value];

  if (searchQuery.value) {
    records = records.filter(
      (record) =>
        record.course.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
        record.date.includes(searchQuery.value),
    );
  }

  if (filters.value.status !== "all") {
    records = records.filter((record) => record.status.toLowerCase() === filters.value.status);
  }

  // Pagination
  const start = (currentPage.value - 1) * 10;
  return records.slice(start, start + 10);
});

// Chart.js Data Computations
const hasTrendData = computed(() => {
  return trendChartData.value.datasets[0].data.some(d => d > 0);
});

const hasCourseData = computed(() => {
  return courseChartData.value.datasets[0].data.some(d => d > 0);
});

const availableCourses = ref([
  { id: 'cs101', name: 'CS 101' },
  { id: 'math201', name: 'MATH 201' },
  { id: 'phys101', name: 'PHYS 101' }
]);

const trendChartData = computed(() => {
  // Aggregate recent attendance by date (past 7 distinct dates)
  const map = new Map()
  
  // Sort oldest to newest for the line chart X-axis
  const sortedRecords = [...attendanceRecords.value].sort((a, b) => new Date(a.date) - new Date(b.date));
  
  sortedRecords.forEach(r => {
    const d = r.date;
    if (!map.has(d)) map.set(d, { present: 0, total: 0 });
    const stats = map.get(d);
    stats.total++;
    if (r.status === 'Present') stats.present++;
  });

  const recentDates = Array.from(map.keys()).slice(-7);
  const dataPoints = recentDates.map(d => {
    const s = map.get(d);
    return Math.round((s.present / s.total) * 100);
  });

  return {
    labels: recentDates.length ? recentDates : ['No Data'],
    datasets: [
      {
        label: 'Attendance %',
        data: dataPoints.length ? dataPoints : [0],
        borderColor: '#6366f1', // Indigo-500
        borderWidth: 3,
        pointBackgroundColor: '#6366f1',
        pointBorderColor: '#fff',
        pointHoverRadius: 6,
        fill: true,
      }
    ]
  };
});

const courseChartData = computed(() => {
  const map = new Map()
  
  attendanceRecords.value.forEach(r => {
    const c = r.course;
    if (!map.has(c)) map.set(c, { present: 0, total: 0 });
    const stats = map.get(c);
    stats.total++;
    if (r.status === 'Present') stats.present++;
  });

  const courses = Array.from(map.keys());
  const dataPoints = courses.map(c => {
    const s = map.get(c);
    return Math.round((s.present / s.total) * 100);
  });

  return {
    labels: courses.length ? courses : ['No Data'],
    datasets: [
      {
        label: 'Course Attendance %',
        data: dataPoints.length ? dataPoints : [0],
        backgroundColor: [
          '#6366f1', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6'
        ],
        borderRadius: 8,
      }
    ]
  };
});

const applyFilters = () => {
  fetchAttendanceData();
};

const exportReport = () => {
  alert("Report export feature will be available soon.");
};

const refreshData = () => {
  fetchAttendanceData();
};

const viewDetails = (recordId) => {
  console.log("Viewing details for record:", recordId);
};

const previousPage = () => {
  if (currentPage.value > 1) {
    currentPage.value--;
  }
};

const nextPage = () => {
  if (currentPage.value < totalPages.value) {
    currentPage.value++;
  }
};

onMounted(() => {
  fetchAttendanceData();
});
</script>

<style scoped>
.report-page {
  min-height: 100vh;
  background-color: var(--bg-main);
}

.page-header-v2 {
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

.chart-container {
  height: 250px;
  width: 100%;
  position: relative;
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

/* === MEDIA .report-page {
  min-height: 100vh;
  padding-top: 80px;
}

.page-header-v2 {
  margin-bottom: 1.5rem;
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
}

.header-main h1 {
  font-size: 1.75rem;
  font-weight: 800;
  letter-spacing: -0.02em;
  margin: 0.25rem 0 0;
}

.header-actions {
  display: flex;
  gap: 0.75rem;
}

/* Horizontal Filter Toolbar */
.filter-toolbar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0.75rem 1.25rem;
  margin-bottom: 1.5rem;
  border-radius: 16px;
  background: rgba(255, 255, 255, 0.4);
}

[data-theme='dark'] .filter-toolbar {
  background: rgba(15, 23, 42, 0.4);
}

.toolbar-items {
  display: flex;
  align-items: center;
  gap: 1.5rem;
}

.toolbar-item {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.toolbar-item label {
  font-size: 0.65rem;
  font-weight: 800;
  text-transform: uppercase;
  color: var(--text-muted);
  letter-spacing: 0.05em;
}

.toolbar-item select {
  background: transparent;
  border: none;
  font-weight: 700;
  font-size: 0.9rem;
  color: var(--text-main);
  padding: 0;
  cursor: pointer;
  outline: none;
}

.toolbar-divider {
  width: 1px;
  height: 24px;
  background: var(--border-light);
}

.apply-toolbar-btn {
  padding: 0.5rem 1rem;
  background: var(--primary);
  color: white;
  border: none;
  border-radius: 8px;
  font-weight: 700;
  font-size: 0.85rem;
  cursor: pointer;
  transition: all 0.2s;
}

.apply-toolbar-btn:hover {
  transform: translateY(-1px);
  filter: brightness(1.1);
}

/* Metrics Grid (High Density) */
.metrics-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.metric-tile {
  padding: 1rem;
  display: flex;
  align-items: center;
  gap: 1rem;
  border-radius: 16px;
}

.metric-tile.highlight {
  border-left: 3px solid var(--primary);
}

.tile-icon {
  width: 40px;
  height: 40px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.25rem;
}

.tile-icon.blue { background: rgba(59, 130, 246, 0.1); }
.tile-icon.emerald { background: rgba(16, 185, 129, 0.1); }
.tile-icon.amber { background: rgba(245, 158, 11, 0.1); }
.tile-icon.indigo { background: rgba(99, 102, 241, 0.1); }

.tile-info {
  display: flex;
  flex-direction: column;
}

.tile-label {
  font-size: 0.75rem;
  font-weight: 600;
  color: var(--text-muted);
}

.tile-value {
  font-size: 1.25rem;
  font-weight: 800;
  color: var(--text-main);
}

/* Analytics Grid */
.analytics-grid {
  display: grid;
  grid-template-columns: 2fr 1fr;
  gap: 1.5rem;
}

.visual-card {
  padding: 1.5rem;
  border-radius: 20px;
  display: flex;
  flex-direction: column;
}

.visual-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.visual-header h3 {
  font-size: 1rem;
  font-weight: 800;
  margin: 0;
}

.trend-tag {
  padding: 4px 8px;
  border-radius: 6px;
  font-size: 0.75rem;
  font-weight: 700;
}

.trend-tag.positive {
  background: rgba(16, 185, 129, 0.1);
  color: #10b981;
}

.chart-box {
  flex: 1;
  min-height: 300px;
}

/* Skeleton Pulse */
.chart-skeleton {
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 1rem;
}

.skeleton-pulse {
  width: 80%;
  height: 200px;
  background: linear-gradient(90deg, transparent, rgba(148, 163, 184, 0.05), transparent);
  background-size: 200% 100%;
  animation: pulse 1.5s infinite;
  border-radius: 12px;
}

@keyframes pulse {
  0% { background-position: 200% 0; }
  100% { background-position: -200% 0; }
}

@media (max-width: 1024px) {
  .analytics-grid { grid-template-columns: 1fr; }
}

@media (max-width: 768px) {
  .metrics-grid { grid-template-columns: repeat(2, 1fr); }
  .filter-toolbar { flex-direction: column; align-items: stretch; gap: 1rem; }
  .toolbar-items { flex-wrap: wrap; justify-content: space-between; gap: 1rem; }
  .toolbar-divider { display: none; }
}
</style>
