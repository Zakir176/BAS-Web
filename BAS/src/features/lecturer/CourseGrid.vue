<template>
  <section class="courses-section">
    <div class="section-header">
      <div class="title-group">
        <h2>Active Courses</h2>
        <span class="count-badge" v-if="courses.length > 0">{{ courses.length }}</span>
      </div>
      <button class="text-link" @click="$emit('view-all')">View Management</button>
    </div>

    <div class="courses-grid" v-if="!isLoading">
      <template v-if="courses.length > 0">
        <div v-for="course in courses" :key="course.id" class="course-tile glass-panel">
          <div class="tile-top">
            <div class="course-icon">📚</div>
            <div class="course-main">
              <h3>{{ course.course_name }}</h3>
              <span class="section-name">{{ course.name }}</span>
            </div>
          </div>
          
          <div class="tile-metrics">
            <div class="mini-kpi">
              <span class="kpi-label">Students</span>
              <span class="kpi-value">{{ course.student_count }}</span>
            </div>
            <div class="mini-kpi">
              <span class="kpi-label">Attendance</span>
              <span class="kpi-value" :class="getAttendanceClass(course.attendance_rate)">
                {{ course.attendance_rate }}%
              </span>
            </div>
          </div>

          <div class="tile-actions">
            <button class="btn-quick-start" @click="$emit('manage', course.id)">
              <span class="icon">⚙️</span> Manage
            </button>
          </div>
        </div>
      </template>

      <EmptyState 
        v-else
        icon="📚"
        title="No Courses Found"
        description="You haven't assigned any courses to your profile yet."
        class="full-width-empty"
      >
        <template #action>
          <BaseButton variant="primary" @click="$emit('create')">Create Course</BaseButton>
        </template>
      </EmptyState>
    </div>

    <div v-else class="courses-grid">
      <BaseSkeleton v-for="i in 3" :key="i" width="100%" height="160px" radius="20px" />
    </div>
  </section>
</template>

<script setup>
import BaseButton from '@/core/ui/BaseButton.vue'
import BaseSkeleton from '@/core/ui/BaseSkeleton.vue'
import EmptyState from '@/core/ui/EmptyState.vue'

defineProps({
  courses: { type: Array, required: true },
  isLoading: { type: Boolean, default: true }
})

defineEmits(['view-all', 'manage', 'create'])

const getAttendanceClass = (rate) => {
  if (rate >= 80) return 'text-success'
  if (rate >= 60) return 'text-warning'
  return 'text-danger'
}
</script>

<style scoped>
.courses-section {
  width: 100%;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
  margin-bottom: 1.25rem;
}

.title-group {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.title-group h2 {
  font-size: 1.15rem;
  font-weight: 850;
  color: var(--text-main);
  margin: 0;
}

.count-badge {
  background: var(--bg-main);
  color: var(--text-muted);
  font-size: 0.7rem;
  font-weight: 800;
  padding: 0.2rem 0.6rem;
  border-radius: 100px;
  border: 1px solid var(--border-light);
}

.courses-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 1.25rem;
}

.course-tile {
  padding: 1.25rem;
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
  transition: var(--transition-smooth);
}

.course-tile:hover {
  transform: translateY(-4px);
  border-color: var(--primary);
  box-shadow: var(--shadow-md);
}

.tile-top {
  display: flex;
  gap: 1rem;
}

.course-icon {
  width: 48px;
  height: 48px;
  background: var(--bg-main);
  border-radius: 14px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.25rem;
  flex-shrink: 0;
}

.course-main h3 {
  font-size: 1rem;
  font-weight: 800;
  color: var(--text-main);
  margin: 0 0 0.15rem 0;
  line-height: 1.2;
}

.section-name {
  font-size: 0.75rem;
  font-weight: 600;
  color: var(--text-muted);
}

.tile-metrics {
  display: grid;
  grid-template-columns: 1fr 1fr;
  background: var(--bg-main);
  padding: 0.75rem;
  border-radius: 12px;
}

.mini-kpi {
  display: flex;
  flex-direction: column;
}

.mini-kpi .kpi-label {
  font-size: 0.65rem;
  font-weight: 700;
  color: var(--text-muted);
  text-transform: uppercase;
  margin-bottom: 0.1rem;
}

.mini-kpi .kpi-value {
  font-size: 1.1rem;
  font-weight: 850;
  color: var(--text-main);
}

.tile-actions {
  display: flex;
  gap: 0.75rem;
}

.btn-quick-start {
  flex: 1;
  padding: 0.6rem;
  border-radius: 10px;
  border: 1px solid var(--border-medium);
  background: transparent;
  color: var(--text-main);
  font-weight: 700;
  font-size: 0.85rem;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.4rem;
  transition: all 0.2s;
}

.btn-quick-start:hover {
  background: var(--bg-main);
  border-color: var(--primary);
  color: var(--primary);
}

.full-width-empty {
  grid-column: 1 / -1;
}

.text-success { color: var(--success); }
.text-warning { color: var(--warning); }
.text-danger { color: var(--error); }
</style>
