<template>
  <section class="content-section">
    <div class="section-header">
      <h2>My Courses</h2>
      <button class="text-link" @click="$emit('view-all')">See all</button>
    </div>
    <div class="courses-grid" v-if="!isLoading">
      <template v-if="courses.length > 0">
        <div v-for="course in courses" :key="course.course_id" class="course-card-premium">
          <div class="card-header">
            <h3>{{ course.course_name }}</h3>
            <span class="badge active">Active</span>
          </div>
          <div class="card-body">
            <div class="detail-row">
              <span>Students</span>
              <strong>{{ course.student_count }}</strong>
            </div>
            <div class="detail-row">
              <span>Performance</span>
              <div class="mini-progress">
                <div class="fill" :style="{ width: course.attendance_rate + '%' }"></div>
              </div>
            </div>
          </div>
          <div class="card-footer">
            <button class="manage-btn" @click="$emit('manage', course.course_id)">Manage Course</button>
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
  margin-top: 2.5rem;
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

.text-link {
  background: none;
  border: none;
  color: var(--primary);
  font-weight: 700;
  cursor: pointer;
}

.courses-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 1.5rem;
}

.course-card-premium {
  background: var(--bg-card);
  padding: 1.5rem;
  border-radius: 24px;
  box-shadow: var(--shadow-card);
}

.card-header {
  display: flex;
  justify-content: space-between;
  margin-bottom: 1.5rem;
}

.card-header h3 {
  font-size: 1.1rem;
  font-weight: 800;
}

.badge {
  padding: 4px 12px;
  border-radius: 8px;
  font-size: 0.7rem;
  font-weight: 800;
}

.badge.active { background: #dcfce7; color: #166534; }

.detail-row {
  display: flex;
  justify-content: space-between;
  margin-bottom: 1rem;
  font-size: 0.9rem;
}

.mini-progress {
  width: 100px;
  height: 8px;
  background: var(--bg-main);
  border-radius: 4px;
}

.mini-progress .fill {
  height: 100%;
  background: var(--primary);
  border-radius: 4px;
}

.card-footer {
  margin-top: 1.5rem;
}

.manage-btn {
  width: 100%;
  padding: 0.75rem;
  border-radius: 12px;
  border: 2px solid var(--border-light);
  background: var(--bg-card);
  font-weight: 700;
  cursor: pointer;
  transition: all 0.2s;
  color: var(--text-main);
}

.manage-btn:hover { background: var(--bg-main); border-color: var(--border-medium); }

.full-width-empty {
  grid-column: 1 / -1;
}
</style>
