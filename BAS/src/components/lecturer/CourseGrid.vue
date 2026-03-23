<template>
  <section class="content-section">
    <div class="section-header">
      <h2>My Courses</h2>
      <button class="text-link" @click="$emit('view-all')">See all</button>
    </div>
    <div class="courses-grid grid-cols-3" v-if="!isLoading">
      <template v-if="courses.length > 0">
        <div v-for="course in courses" :key="course.id" class="course-card-premium glass-panel">
          <div class="card-header">
            <div class="icon-wrapper">
              <span class="icon">📚</span>
            </div>
            <span class="badge active">Active</span>
          </div>
          <div class="card-body">
            <h3>{{ course.course_name }}</h3>
            <div class="metrics-row">
              <div class="metric">
                <span class="lbl">Students</span>
                <span class="val">{{ course.student_count }}</span>
              </div>
              <div class="metric">
                <span class="lbl">Attendance</span>
                <span class="val">{{ course.attendance_rate }}%</span>
              </div>
            </div>
            
            <div class="mini-progress">
              <div class="fill" :style="{ width: course.attendance_rate + '%' }"></div>
            </div>
          </div>
          <div class="card-footer">
            <button class="manage-btn" @click="$emit('manage', course.id)">Manage Course</button>
          </div>
        </div>
      </template>
      <EmptyState 
        v-else
        icon="📚"
        title="No Courses Created"
        description="You haven't added any courses yet. Create your first course to start tracking attendance."
        class="full-width-empty"
      >
        <template #action>
          <Button variant="primary" @click="$emit('create')">Create Course</Button>
        </template>
      </EmptyState>
    </div>
    <div v-else class="courses-grid">
      <Skeleton v-for="i in 3" :key="i" width="100%" height="250px" radius="24px" />
    </div>
  </section>
</template>

<script setup>
import Button from '@/components/ui/Button.vue'
import Skeleton from '@/components/ui/Skeleton.vue'
import EmptyState from '@/components/ui/EmptyState.vue'

defineProps({
  courses: { type: Array, required: true },
  isLoading: { type: Boolean, default: true }
})

defineEmits(['view-all', 'manage', 'create'])
</script>

<style scoped>
.content-section {
  width: 100%;
}



.courses-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 1.5rem;
}

.course-card-premium {
  padding: 1.5rem;
  display: flex;
  flex-direction: column;
}

.course-card-premium:hover {
  box-shadow: 0 15px 35px rgba(59, 130, 246, 0.1);
  border-color: rgba(59, 130, 246, 0.2);
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 1rem;
}

.icon-wrapper {
  width: 44px; height: 44px;
  background: rgba(59, 130, 246, 0.1);
  border-radius: 12px;
  display: flex; align-items: center; justify-content: center;
  font-size: 1.25rem;
}

.badge {
  padding: 4px 10px;
  border-radius: 8px;
  font-size: 0.7rem;
  font-weight: 800;
  letter-spacing: 0.5px;
  text-transform: uppercase;
}

.badge.active { background: rgba(16, 185, 129, 0.1); color: #10b981; }

.card-body h3 {
  font-size: 1.15rem;
  font-weight: 800;
  color: var(--text-main);
  margin-bottom: 1rem;
  line-height: 1.4;
}

.metrics-row {
  display: flex;
  gap: 1.5rem;
  margin-bottom: 0.75rem;
}

.metric {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.metric .lbl {
  font-size: 0.75rem;
  color: var(--text-muted);
  font-weight: 600;
}
.metric .val {
  font-size: 1rem;
  font-weight: 800;
  color: var(--text-main);
}

.mini-progress {
  width: 100%;
  height: 6px;
  background: var(--bg-main);
  border-radius: 8px;
  overflow: hidden;
  margin-top: 0.5rem;
}

.mini-progress .fill {
  height: 100%;
  background: linear-gradient(90deg, #3b82f6, #8b5cf6);
  border-radius: 8px;
}

.card-footer {
  margin-top: 1.5rem;
  padding-top: 1rem;
  border-top: 1px dashed var(--border-light);
}

.manage-btn {
  width: 100%;
  padding: 0.8rem;
  border-radius: 12px;
  border: 1px solid var(--border-medium);
  background: transparent;
  font-weight: 700;
  font-size: 0.9rem;
  cursor: pointer;
  transition: all 0.2s;
  color: var(--text-main);
}

.manage-btn:hover { 
  background: rgba(59, 130, 246, 0.05); 
  border-color: #3b82f6; 
  color: #3b82f6;
}

.full-width-empty {
  grid-column: 1 / -1;
}
</style>
