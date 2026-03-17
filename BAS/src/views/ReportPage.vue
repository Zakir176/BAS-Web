<template>
  <div class="report-page">
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
            <Button variant="primary" @click="exportReport">
              <svg width="20" height="20" viewBox="0 0 20 20" fill="currentColor">
                <path
                  d="M3 17a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zm3.293-7.707a1 1 0 011.414 0L9 10.586V3a1 1 0 112 0v7.586l1.293-1.293a1 1 0 111.414 1.414l-3 3a1 1 0 01-1.414 0l-3-3a1 1 0 010-1.414z"
                />
              </svg>
              Export Report
            </Button>
            <Button variant="secondary" @click="refreshData">
              <svg width="20" height="20" viewBox="0 0 20 20" fill="currentColor">
                <path
                  fill-rule="evenodd"
                  d="M4 2a1 1 0 011 1v2.101a7.002 7.002 0 0111.601 2.566 1 1 0 11-1.885.666A5.002 5.002 0 005.999 7H9a1 1 0 010 2H4a1 1 0 01-1-1V3a1 1 0 011-1zm.008 9.057a1 1 0 011.276.61A5.002 5.002 0 0014.001 13H11a1 1 0 110-2h5a1 1 0 011 1v5a1 1 0 11-2 0v-2.101a7.002 7.002 0 01-11.601-2.566 1 1 0 01.61-1.276z"
                  clip-rule="evenodd"
                />
              </svg>
              Refresh
            </Button>
          </div>
        </header>

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
                  <path
                    d="M16 2C8.268 2 2 8.268 2 16s6.268 14 14 14 14-6.268 14-14S23.732 2 16 2zm0 2c6.627 0 12 5.373 12 12s-5.373 12-12 12S4 22.627 4 16 9.373 4 16 4z"
                  />
                  <path d="M16 8v8l4 4" />
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
                  <path
                    d="M16 2l4.586 4.586L26 6.172l2.828 2.828L28.586 16 26 18.586 23.172 26 16 28.586 13.414 26 6.172 23.172 3.344 16 5.414 13.414 8.828 6.172 16 2z"
                  />
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
                  <path
                    d="M16 2C8.268 2 2 8.268 2 16s6.268 14 14 14 14-6.268 14-14S23.732 2 16 2zm0 2c6.627 0 12 5.373 12 12s-5.373 12-12 12S4 22.627 4 16 9.373 4 16 4zm-1 5v8h8v-2h-6V9h-2z"
                  />
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
                  <path
                    d="M16 2C8.268 2 2 8.268 2 16s6.268 14 14 14 14-6.268 14-14S23.732 2 16 2zm0 2c6.627 0 12 5.373 12 12s-5.373 12-12 12S4 22.627 4 16 9.373 4 16 4z"
                  />
                  <path d="M16 8v8l6 3" />
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
              <div class="chart-container">
                <LineChart :chart-data="trendChartData" />
              </div>
            </Card>

            <Card class="chart-card">
              <h3>Course Distribution</h3>
              <div class="chart-container">
                <BarChart :chart-data="courseChartData" />
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
        borderColor: '#3b82f6',
        backgroundColor: 'rgba(59, 130, 246, 0.1)',
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
          '#3b82f6', '#10b981', '#f59e0b', '#8b5cf6', '#ec4899'
        ],
        borderRadius: 4
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
