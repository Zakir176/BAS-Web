<template>
  <section class="bento-stats grid-cols-3 mt-8">
    <div class="bento-card glass-panel p-hover blue-glow p-6">
      <div class="kpi-icon blue">🎓</div>
      <div class="kpi-value">{{ stats.totalCourses }}</div>
      <div class="kpi-label">Total Courses</div>
    </div>
    <div class="bento-card glass-panel p-hover emerald-glow p-6">
      <div class="kpi-icon emerald">👥</div>
      <div class="kpi-value">{{ stats.totalStudents }}</div>
      <div class="kpi-label">Enrolled Students</div>
    </div>
    <div class="bento-card glass-panel p-hover purple-glow p-6">
      <div class="kpi-icon purple">📈</div>
      <div class="kpi-value">{{ stats.avgAttendance }}%</div>
      <div class="kpi-label">Avg. Attendance</div>
    </div>
  </section>
</template>

<script setup>
defineProps({
  stats: {
    type: Object,
    required: true
  }
})
</script>

<style scoped>
.bento-stats { margin-top: 1.5rem; }

.bento-card {
  display: flex;
  flex-direction: column;
  justify-content: center;
}

.p-hover {
  transition: transform 0.3s cubic-bezier(0.34, 1.56, 0.64, 1), box-shadow 0.3s ease;
  cursor: default;
}
.p-hover:hover {
  transform: translateY(-6px) scale(1.02);
}

.blue-glow:hover { box-shadow: 0 15px 35px rgba(59, 130, 246, 0.15); border-color: rgba(59, 130, 246, 0.2); }
.emerald-glow:hover { box-shadow: 0 15px 35px rgba(16, 185, 129, 0.15); border-color: rgba(16, 185, 129, 0.2); }
.purple-glow:hover { box-shadow: 0 15px 35px rgba(168, 85, 247, 0.15); border-color: rgba(168, 85, 247, 0.2); }

.kpi-icon {
  width: 48px;
  height: 48px;
  border-radius: 14px;
  display: flex;
  justify-content: center;
  align-items: center;
  margin-bottom: 1rem;
  font-size: 1.5rem;
}

.kpi-icon.blue { background: rgba(59, 130, 246, 0.1); }
.kpi-icon.emerald { background: rgba(16, 185, 129, 0.1); }
.kpi-icon.purple { background: rgba(168, 85, 247, 0.1); }

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
</style>
