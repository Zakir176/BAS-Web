<template>
  <div class="student-dashboard">
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
                  <path
                    d="M3 4a1 1 0 011-1h3a1 1 0 011 1v3a1 1 0 01-1 1H4a1 1 0 01-1-1V4zM12 4a1 1 0 011-1h3a1 1 0 011 1v3a1 1 0 01-1 1h-3a1 1 0 01-1-1V4zM3 12a1 1 0 011-1h3a1 1 0 011 1v3a1 1 0 01-1 1H4a1 1 0 01-1-1v-3zM12 12a1 1 0 011-1h3a1 1 0 011 1v3a1 1 0 01-1 1h-3a1 1 0 01-1-1v-3z"
                  />
                </svg>
                Mark Attendance
              </Button>
              <Button variant="secondary" size="lg" @click="goToReports"> View Reports </Button>
            </div>
          </div>
        </section>

        <!-- Stats Overview -->
        <section class="stats-section">
          <div class="stats-grid">
            <Card class="stat-card">
              <div class="stat-icon attendance">
                <svg width="32" height="32" viewBox="0 0 32 32" fill="currentColor">
                  <path
                    d="M16 2C8.268 2 2 8.268 2 16s6.268 14 14 14 14-6.268 14-14S23.732 2 16 2zm0 2c6.627 0 12 5.373 12 12s-5.373 12-12 12S4 22.627 4 16 9.373 4 16 4zm-1 5v6h6v2h-8V9h2z"
                  />
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
                  <path
                    d="M16 2l4.586 4.586L26 6.172l2.828 2.828L28.586 16 26 18.586 23.172 26 16 28.586 13.414 26 6.172 23.172 3.344 16 5.414 13.414 8.828 6.172 16 2z"
                  />
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
                  <path
                    d="M16 2C8.268 2 2 8.268 2 16s6.268 14 14 14 14-6.268 14-14S23.732 2 16 2zm0 2c6.627 0 12 5.373 12 12s-5.373 12-12 12S4 22.627 4 16 9.373 4 16 4zm-1 5v8h8v-2h-6V9h-2z"
                  />
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
                  <path
                    d="M16 2l2.122 6.364L24 10.878l-5.878 2.514L16 20l-2.122-6.608L8 10.878l5.878-2.514L16 2z"
                  />
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
            <Button variant="secondary" size="sm" @click="viewAllActivity"> View All </Button>
          </div>

          <div class="activity-list">
            <Card v-for="activity in recentActivities" :key="activity.id" class="activity-item">
              <div class="activity-content">
                <div class="activity-icon" :class="activity.type">
                  <svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor">
                    <path
                      v-if="activity.type === 'present'"
                      d="M9 16.17L4.83 12l-1.42 1.41L9 19 21 7l-1.41-1.41z"
                    />
                    <path
                      v-else
                      d="M19 6.41L17.59 5 12 10.59 6.41 5 5 6.41 10.59 12 5 17.59 6.41 19 12 13.41 17.59 19 19 17.59 13.41 12z"
                    />
                  </svg>
                </div>
                <div class="stat-card white-card">
                  <span class="label">ABSENCES</span>
                  <Skeleton v-if="isLoading" width="60px" height="2rem" />
                  <div v-else class="value danger">{{ attendanceStats.absent }}</div>
                </div>
              </div>
              <div class="activity-status" :class="activity.type">
                {{ activity.type === "present" ? "Present" : "Absent" }}
              </div>
            </Card>
          </div>

          <!-- History Timeline -->
          <RecentActivity 
            :is-loading="isLoading"
            :activities="recentActivity"
            :show-all="showAllHistory"
            @toggle-show-all="showAllHistory = !showAllHistory"
          />
        </section>

        <!-- Today's Schedule -->
        <section class="schedule-section">
          <div class="section-header">
            <h2>Today's Schedule</h2>
            <span class="current-date">{{ currentDate }}</span>
          </div>

          <div class="schedule-grid">
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
                <span v-else-if="classItem.status === 'Completed'" class="completed-mark"> ✓ </span>
              </div>
            </Card>
          </div>
        </section>
      </div>
    </main>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { useRouter } from "vue-router";
import { supabase } from "@/supabase";
import Navbar from "@/components/common/Navbar.vue";
import Button from "@/components/ui/Button.vue";
import Card from "@/components/ui/Card.vue";

const router = useRouter();

const studentName = ref("Loading...");
const currentDate = ref(
  new Date().toLocaleDateString("en-US", {
    weekday: "long",
    year: "numeric",
    month: "long",
    day: "numeric",
  }),
);

const attendanceStats = ref({
  overall: 0,
  present: 0,
  absent: 0,
  streak: 0,
});

const recentActivities = ref([]);
const todaySchedule = ref([]); // Currently mock as no schedule table exists yet

const fetchStudentData = async () => {
  try {
    const {
      data: { user },
    } = await supabase.auth.getUser();
    if (!user) {
      router.push("/student-login");
      return;
    }

    // Fetch student profile
    const { data: profile, error: profileError } = await supabase
      .from("students")
      .select("*")
      .eq("id", user.id)
      .single();

    if (profileError) throw profileError;
    studentName.value = `${profile.first_name} ${profile.last_name}`;

    // Fetch activity logs
    const { data: logs, error: logsError } = await supabase
      .from("activity_logs")
      .select("*")
      .eq("student_id", user.id)
      .order("date_time", { ascending: false });

    if (logsError) throw logsError;

    recentActivities.value = logs.map((log) => ({
      id: log.id,
      course: "General Attendance", // Placeholder until classes are implemented
      date: new Date(log.date_time).toLocaleDateString(),
      time: new Date(log.date_time).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" }),
      type: log.status?.toLowerCase() || "present",
    }));

    // Calculate Stats
    const total = logs.length;
    const presentCount = logs.filter((l) => l.status === "Present").length;
    attendanceStats.value = {
      overall: total > 0 ? Math.round((presentCount / total) * 100) : 0,
      present: presentCount,
      absent: total - presentCount,
      streak: 0, // Logic for streak can be added later
    };
  } catch (error) {
    console.error("Error fetching student data:", error);
  }
};

const showBarcodeScanner = () => {
  alert("In the Web version, please use the Mobile App to scan your ID.");
};

const goToReports = () => {
  router.push("/report-page");
};

const viewAllActivity = () => {
  router.push("/report-page");
};

const markAttendance = (classId) => {
  alert("Please use the Mobile App to mark attendance via QR scanning.");
};

onMounted(() => {
  fetchStudentData();
});
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
