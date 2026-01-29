<template>
  <section class="stats-overview">
    <div class="stats-grid">
      <div class="stat-card">
        <div class="stat-main">
          <span class="stat-label">TOTAL COURSES</span>
          <div class="stat-value">{{ stats.totalCourses }}</div>
        </div>
        <div class="stat-icon courses">🎓</div>
      </div>
      <div class="stat-card">
        <div class="stat-main">
          <span class="stat-label">ENROLLED STUDENTS</span>
          <div class="stat-value">{{ stats.totalStudents }}</div>
        </div>
        <div class="stat-icon students">👥</div>
      </div>
      <div class="stat-card">
        <div class="stat-main">
          <span class="stat-label">AVG. ATTENDANCE</span>
          <div class="stat-value">{{ stats.avgAttendance }}%</div>
        </div>
        <div class="stat-icon attendance">📈</div>
      </div>
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
.stats-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 1.5rem;
}

.stat-card {
  background: var(--bg-card);
  padding: 1.5rem;
  border-radius: 24px;
  box-shadow: var(--shadow-soft);
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.stat-label {
  font-size: 0.75rem;
  font-weight: 800;
  color: var(--text-muted);
  letter-spacing: 0.05em;
}

.stat-value {
  font-size: 2rem;
  font-weight: 800;
  color: var(--text-main);
}

.stat-icon {
  width: 56px;
  height: 56px;
  border-radius: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
}

.stat-icon.courses { background: #eff6ff; color: #3b82f6; }
.stat-icon.students { background: #f0fdf4; color: #22c55e; }
.stat-icon.attendance { background: #faf5ff; color: #a855f7; }

@media (max-width: 768px) {
  .stats-grid {
    grid-template-columns: 1fr;
  }
}

@media (min-width: 769px) and (max-width: 1024px) {
  .stats-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}
</style>
