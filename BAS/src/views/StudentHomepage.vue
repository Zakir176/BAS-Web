<template>
  <div class="student-dashboard">
    <main class="main-content">
      <div class="dashboard-container">
        
        <!-- Premium Hero Banner -->
        <section class="hero-banner">
          <div class="hero-content">
            <div class="hero-badge">Student Portal</div>
            <h1>Welcome back, {{ studentName }}!</h1>
            <p class="hero-subtitle">Your academic journey is looking great. Keep up the strong momentum!</p>
            <div class="hero-actions">
              <button class="btn-glow" @click="showBarcodeScanner">
                <svg width="20" height="20" viewBox="0 0 20 20" fill="currentColor">
                  <path d="M3 4a1 1 0 011-1h3a1 1 0 011 1v3a1 1 0 01-1 1H4a1 1 0 01-1-1V4zM12 4a1 1 0 011-1h3a1 1 0 011 1v3a1 1 0 01-1 1h-3a1 1 0 01-1-1V4zM3 12a1 1 0 011-1h3a1 1 0 011 1v3a1 1 0 01-1 1H4a1 1 0 01-1-1v-3zM12 12a1 1 0 011-1h3a1 1 0 011 1v3a1 1 0 01-1 1h-3a1 1 0 01-1-1v-3z" />
                </svg>
                Show ID Card
              </button>
              <button class="btn-glass" @click="goToReports">
                View Reports
              </button>
            </div>
          </div>
          <div class="hero-decoration">
            <div class="circle-1"></div>
            <div class="circle-2"></div>
          </div>
        </section>

        <!-- Premium Bento Grid -->
        <section class="bento-grid">
          
          <!-- Main Chart -->
          <div class="bento-card col-span-2 row-span-2 glass-panel p-6">
            <div class="bento-header">
              <h2>Attendance Ratio</h2>
            </div>
            <div class="doughnut-wrapper">
              <DoughnutChart :chart-data="attendanceChartData" />
            </div>
            <div class="overall-overlay">
              <div class="big-percent">{{ attendanceStats.overall }}%</div>
              <div class="lbl">OVERALL</div>
            </div>
          </div>

          <!-- KPI: Present -->
          <div class="bento-card glass-panel p-hover emerald-glow p-5">
            <div class="kpi-icon emerald">
              <svg width="24" height="24" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                <path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" />
              </svg>
            </div>
            <div class="kpi-value">{{ attendanceStats.present }}</div>
            <div class="kpi-label">Days Present</div>
          </div>

          <!-- KPI: Absent -->
          <div class="bento-card glass-panel p-hover rose-glow p-5">
            <div class="kpi-icon rose">
              <svg width="24" height="24" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </div>
            <div class="kpi-value">{{ attendanceStats.absent }}</div>
            <div class="kpi-label">Days Absent</div>
          </div>

          <!-- KPI: Streak -->
          <div class="bento-card col-span-2 glass-panel flex-row items-center justify-between p-5 p-hover blue-glow">
            <div class="kpi-info">
              <div class="kpi-label">Current Streak</div>
              <div class="kpi-value">🔥 {{ attendanceStats.overall >= 80 ? 'Active' : 'Needs Repair' }}</div>
            </div>
            <div class="kpi-visual">
              <div class="mini-bar-bg">
                <div class="mini-bar-fill" :style="{ width: attendanceStats.overall + '%' }"></div>
              </div>
            </div>
          </div>

        </section>

        <!-- Secondary Layout -->
        <div class="secondary-grid mt-8">
          
          <!-- History Timeline -->
          <div class="glass-panel p-6">
            <div class="section-header">
              <h2>Attendance Heatmap</h2>
            </div>
            <AttendanceCalendar :attendance-records="calendarData" />
          </div>

          <!-- Today's Schedule -->
          <div class="glass-panel p-6">
            <div class="section-header">
              <h2>Today's Schedule</h2>
              <span class="current-date">{{ currentDate }}</span>
            </div>
            <div class="schedule-list">
              <div v-for="classItem in todaySchedule" :key="classItem.id" class="schedule-flight shadow-sm">
                <div class="flight-time">
                  <strong>{{ classItem.startTime }}</strong>
                  <span class="text-xs text-gray-400">{{ classItem.endTime }}</span>
                </div>
                <div class="flight-details">
                  <h4>{{ classItem.course }}</h4>
                  <p>{{ classItem.room }}</p>
                </div>
                <div class="flight-action">
                  <span class="completed-mark">✓</span>
                </div>
              </div>
              <div v-if="todaySchedule.length === 0" class="text-center text-gray-500 text-sm mt-4">
                No classes scheduled for today.
              </div>
            </div>
          </div>

        </div>
      </div>
    </main>

    <!-- Barcode Scanner Modal -->
    <Modal :is-open="isBarcodeModalOpen" @close="closeBarcodeModal" transparent hide-header hide-footer>
      <template #default>
        <StudentBarcode v-if="studentProfile" :student-id="studentProfile.student_number" />
        <div v-else class="text-center text-gray-500 py-8 bg-white rounded-3xl p-8">
          <p>Loading barcode...</p>
        </div>
      </template>
    </Modal>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted, computed } from "vue";
import { useRouter } from "vue-router";
import { supabase } from "@/supabase";
import Modal from "@/components/ui/Modal.vue";
import StudentBarcode from "@/components/student/StudentBarcode.vue";
import DoughnutChart from "@/components/ui/charts/DoughnutChart.vue";
import AttendanceCalendar from "@/components/student/AttendanceCalendar.vue";

const router = useRouter();

const studentName = ref("Loading...");
const studentProfile = ref(null);
const isLoading = ref(true);
const showAllHistory = ref(false);

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

const attendanceChartData = computed(() => {
  return {
    labels: ['Present', 'Absent'],
    datasets: [
      {
        data: [attendanceStats.value.present, attendanceStats.value.absent],
        backgroundColor: ['#10b981', '#ef4444'], // emerald-500, red-500
        borderWidth: 0,
        hoverOffset: 4
      }
    ]
  }
});

const recentActivity = ref([]);
const calendarData = ref([]);
const todaySchedule = ref([]); // Current mock as no schedule table exists yet

const isBarcodeModalOpen = ref(false);

const fetchStudentData = async () => {
  try {
    const {
      data: { user },
    } = await supabase.auth.getUser();
    if (!user) {
      router.push("/student-login");
      return;
    }

    // Fetch student profile using email
    const { data: profile, error: profileError } = await supabase
      .from("students")
      .select("*")
      .eq("email", user.email)
      .maybeSingle();

    if (profileError) throw profileError;
    studentProfile.value = profile;
    studentName.value = profile?.full_name || user.user_metadata?.full_name || user.email.split('@')[0];

    // Fetch attendance records from attendance_logs
    const { data: logs, error: logsError } = await supabase
      .from("attendance_logs")
      .select("*, sections(name, courses(name))")
      .eq("student_id", profile.id) // Using UUID id
      .order("session_date", { ascending: false });

    if (logsError) throw logsError;

    recentActivity.value = logs.map((log) => ({
      id: log.id,
      course: log.sections?.courses?.name || log.sections?.name || "General Attendance",
      date: new Date(log.session_date).toLocaleDateString(),
      time: new Date(log.session_date).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" }),
      type: log.status?.toLowerCase() || "present",
    }));

    calendarData.value = logs.map((log) => ({
      date: log.session_date.split('T')[0],
      status: log.status || 'Present'
    }));

    // Calculate Stats
    const total = logs.length;
    const presentCount = logs.filter((l) => l.status === "Present" || l.status === "present").length;
    attendanceStats.value = {
      overall: total > 0 ? Math.round((presentCount / total) * 100) : 0,
      present: presentCount,
      absent: total - presentCount,
      streak: 0, 
    };
  } catch (error) {
    console.error("Error fetching student data:", error);
  } finally {
    isLoading.value = false;
  }
};

const showBarcodeScanner = () => {
  isBarcodeModalOpen.value = true;
};

const closeBarcodeModal = () => {
  isBarcodeModalOpen.value = false;
};

const goToReports = () => {
  router.push("/report-page");
};

let attendanceSubscription = null;

onMounted(async () => {
  await fetchStudentData();
  
  if (studentProfile.value?.id) {
    attendanceSubscription = supabase
      .channel('student-attendance')
      .on('postgres_changes', { 
        event: 'INSERT', 
        schema: 'public', 
        table: 'attendance_logs',
        filter: `student_id=eq.${studentProfile.value.id}`
      }, () => {
        // Automatically refresh dashboard when lecturer marks them present
        fetchStudentData();
      })
      .subscribe();
  }
});

onUnmounted(() => {
  if (attendanceSubscription) {
    supabase.removeChannel(attendanceSubscription);
  }
});
</script>

<style scoped>
.student-dashboard {
  min-height: 100vh;
  background-color: var(--bg-main);
  font-family: 'Inter', sans-serif;
}

.main-content {
  padding: 2rem 0 5rem 0;
}

.dashboard-container {
  max-width: 1100px;
  margin: 0 auto;
  padding: 0 1.5rem;
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

/* === HERO BANNER === */
.hero-banner {
  position: relative;
  background: linear-gradient(135deg, #4f46e5 0%, #7c3aed 100%);
  border-radius: 28px;
  padding: 3rem;
  color: white;
  overflow: hidden;
  box-shadow: 0 20px 40px rgba(79, 70, 229, 0.25);
}

.hero-content {
  position: relative;
  z-index: 2;
  max-width: 600px;
}

.hero-badge {
  display: inline-block;
  background: rgba(255,255,255,0.2);
  backdrop-filter: blur(10px);
  padding: 0.4rem 1rem;
  border-radius: 100px;
  font-size: 0.75rem;
  font-weight: 700;
  letter-spacing: 1px;
  text-transform: uppercase;
  margin-bottom: 1rem;
}

.hero-banner h1 {
  font-size: 2.5rem;
  font-weight: 800;
  line-height: 1.2;
  margin-bottom: 0.75rem;
}

.hero-subtitle {
  font-size: 1.1rem;
  opacity: 0.9;
  margin-bottom: 2rem;
}

.hero-actions {
  display: flex;
  gap: 1rem;
}

.btn-glow {
  background: white;
  color: #4f46e5;
  border: none;
  padding: 0.8rem 1.5rem;
  border-radius: 14px;
  font-weight: 700;
  font-size: 1rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  box-shadow: 0 10px 25px rgba(255,255,255,0.3);
}

.btn-glow:hover {
  transform: translateY(-3px);
  box-shadow: 0 15px 35px rgba(255,255,255,0.4);
}

.btn-glass {
  background: rgba(255,255,255,0.1);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255,255,255,0.2);
  color: white;
  padding: 0.8rem 1.5rem;
  border-radius: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-glass:hover {
  background: rgba(255,255,255,0.2);
}

.hero-decoration {
  position: absolute;
  top: 0; left: 0; right: 0; bottom: 0;
  pointer-events: none;
  z-index: 1;
}

.circle-1 {
  position: absolute;
  width: 400px; height: 400px;
  background: radial-gradient(circle, rgba(255,255,255,0.2) 0%, transparent 70%);
  top: -150px; right: -50px;
}

.circle-2 {
  position: absolute;
  width: 300px; height: 300px;
  background: radial-gradient(circle, rgba(236,72,153,0.3) 0%, transparent 70%);
  bottom: -100px; right: 20%;
}

/* === BENTO GRID === */
.bento-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  grid-auto-rows: minmax(130px, auto);
  gap: 1.5rem;
}

.glass-panel {
  background: var(--bg-card);
  border: 1px solid rgba(0,0,0,0.05);
  border-radius: 24px;
  box-shadow: 0 10px 30px rgba(0,0,0,0.03);
  position: relative;
  overflow: hidden;
}

[data-theme='dark'] .glass-panel {
  background: rgba(30, 41, 59, 0.7);
  backdrop-filter: blur(20px);
  border-color: rgba(255,255,255,0.05);
  box-shadow: 0 10px 30px rgba(0,0,0,0.2);
}

.col-span-2 { grid-column: span 2; }
.row-span-2 { grid-row: span 2; }
.p-6 { padding: 1.5rem; }
.p-5 { padding: 1.25rem; }
.mt-8 { margin-top: 2rem; }
.flex-row { display: flex; align-items: center; }

/* Interactive Hover Lift */
.p-hover {
  transition: transform 0.3s cubic-bezier(0.34, 1.56, 0.64, 1), box-shadow 0.3s ease;
  cursor: default;
}
.p-hover:hover {
  transform: translateY(-6px) scale(1.02);
}

.emerald-glow:hover { box-shadow: 0 15px 35px rgba(16, 185, 129, 0.15); border-color: rgba(16, 185, 129, 0.2); }
.rose-glow:hover { box-shadow: 0 15px 35px rgba(244, 63, 94, 0.15); border-color: rgba(244, 63, 94, 0.2); }
.blue-glow:hover { box-shadow: 0 15px 35px rgba(59, 130, 246, 0.15); border-color: rgba(59, 130, 246, 0.2); }

/* Bento specific contents */
.bento-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.bento-header h2 {
  font-size: 1.15rem;
  font-weight: 800;
  color: var(--text-main);
}

.doughnut-wrapper {
  height: 200px;
  position: relative;
  z-index: 2;
}

.overall-overlay {
  position: absolute;
  top: 55%;
  left: 50%;
  transform: translate(-50%, -50%);
  text-align: center;
  z-index: 1;
}

.big-percent {
  font-size: 2.2rem;
  font-weight: 900;
  color: var(--text-main);
  line-height: 1;
}

.overall-overlay .lbl {
  font-size: 0.65rem;
  font-weight: 700;
  color: var(--text-muted);
  letter-spacing: 1px;
}

.kpi-icon {
  width: 48px;
  height: 48px;
  border-radius: 14px;
  display: flex;
  justify-content: center;
  align-items: center;
  margin-bottom: 1rem;
}

.kpi-icon.emerald { background: rgba(16, 185, 129, 0.1); color: #10b981; }
.kpi-icon.rose { background: rgba(244, 63, 94, 0.1); color: #f43f5e; }

.kpi-value {
  font-size: 2.2rem;
  font-weight: 800;
  color: var(--text-main);
  line-height: 1.1;
  margin-bottom: 0.25rem;
}

.kpi-label {
  font-size: 0.85rem;
  color: var(--text-muted);
  font-weight: 600;
}

/* Mini Bar Fill */
.mini-bar-bg {
  width: 120px;
  height: 8px;
  background: var(--bg-main);
  border-radius: 10px;
  overflow: hidden;
}

.mini-bar-fill {
  height: 100%;
  background: linear-gradient(90deg, #3b82f6, #8b5cf6);
  border-radius: 10px;
  transition: width 1s cubic-bezier(0.16, 1, 0.3, 1);
}

/* === SECONDARY GRID === */
.secondary-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1.5rem;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}
.section-header h2 {
  font-size: 1.25rem;
  font-weight: 800;
}

.current-date {
  font-size: 0.85rem;
  color: var(--text-muted);
  font-weight: 600;
}

.schedule-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.schedule-flight {
  background: var(--bg-main);
  border-radius: 16px;
  padding: 1rem;
  display: flex;
  align-items: center;
  gap: 1.5rem;
}

.flight-time {
  display: flex;
  flex-direction: column;
  align-items: center;
  min-width: 60px;
  color: var(--text-main);
}
.flight-time strong { font-size: 1.1rem; }

.flight-details h4 {
  font-size: 1rem;
  font-weight: 700;
  color: var(--text-main);
  margin-bottom: 0.2rem;
}
.flight-details p {
  font-size: 0.85rem;
  color: var(--text-muted);
}

.flight-action {
  margin-left: auto;
}
.completed-mark {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 32px; height: 32px;
  background: var(--success-bg);
  color: var(--success);
  border-radius: 50%;
  font-weight: bold;
}

/* === MEDIA QUERIES === */
@media (max-width: 1000px) {
  .bento-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 768px) {
  .hero-banner { padding: 2rem; }
  .hero-banner h1 { font-size: 2rem; }
  .bento-grid { grid-template-columns: 1fr; }
  .col-span-2 { grid-column: span 1; }
  .secondary-grid { grid-template-columns: 1fr; }
}
</style>
