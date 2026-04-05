<template>
  <section class="stat-tiles-grid">
    <div class="stat-tile glass-panel">
      <div class="tile-icon blue">📘</div>
      <div class="tile-info">
        <div class="tile-label">Active Courses</div>
        <div class="tile-value">{{ stats.totalCourses }}</div>
      </div>
      <div class="tile-trend positive">+1</div>
    </div>

    <div class="stat-tile glass-panel">
      <div class="tile-icon emerald">👥</div>
      <div class="tile-info">
        <div class="tile-label">Total Students</div>
        <div class="tile-value">{{ stats.totalStudents }}</div>
      </div>
      <div class="tile-trend">Steady</div>
    </div>

    <div class="stat-tile glass-panel">
      <div class="tile-icon purple">📈</div>
      <div class="tile-info">
        <div class="tile-label">Avg Attendance</div>
        <div class="tile-value">{{ stats.avgAttendance }}%</div>
      </div>
      <div class="tile-trend positive">+2.4%</div>
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
.stat-tiles-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 1.25rem;
  margin-top: 1rem;
}

.stat-tile {
  display: flex;
  align-items: center;
  gap: 1.25rem;
  padding: 1.25rem 1.5rem;
  position: relative;
  overflow: hidden;
  transition: var(--transition-smooth);
}

.stat-tile::before {
  content: '';
  position: absolute;
  top: 0; left: 0; width: 4px; height: 100%;
  background: var(--primary);
  opacity: 0;
  transition: opacity 0.3s ease;
}

.stat-tile:hover {
  transform: translateY(-4px);
  box-shadow: var(--shadow-lg);
}

.stat-tile:hover::before {
  opacity: 1;
}

.tile-icon {
  width: 44px;
  height: 44px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.2rem;
  flex-shrink: 0;
}

.tile-icon.blue { background: rgba(59, 130, 246, 0.1); border: 1px solid rgba(59, 130, 246, 0.2); }
.tile-icon.emerald { background: rgba(16, 185, 129, 0.1); border: 1px solid rgba(16, 185, 129, 0.2); }
.tile-icon.purple { background: rgba(168, 85, 247, 0.1); border: 1px solid rgba(168, 85, 247, 0.2); }

.tile-info {
  flex: 1;
}

.tile-label {
  font-size: 0.75rem;
  font-weight: 700;
  color: var(--text-muted);
  text-transform: uppercase;
  letter-spacing: 0.05em;
  margin-bottom: 0.25rem;
}

.tile-value {
  font-size: 1.75rem;
  font-weight: 850;
  color: var(--text-main);
  line-height: 1;
}

.tile-trend {
  font-size: 0.7rem;
  font-weight: 700;
  color: var(--text-muted);
  background: var(--bg-main);
  padding: 0.25rem 0.6rem;
  border-radius: 8px;
  align-self: flex-start;
}

.tile-trend.positive {
  color: var(--success);
  background: var(--success-bg);
}

@media (max-width: 900px) {
  .stat-tiles-grid { grid-template-columns: 1fr; }
}
</style>
