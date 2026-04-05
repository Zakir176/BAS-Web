<template>
  <div class="recent-activity-panel glass-panel">
    <div class="panel-header">
      <h3>Recent Activity</h3>
      <span class="live-indicator">
        <span class="dot"></span> Live
      </span>
    </div>
    
    <div class="activity-list" v-if="activities.length > 0">
      <div v-for="activity in activities" :key="activity.id" class="activity-item">
        <div class="activity-avatar">{{ activity.student_name?.charAt(0) }}</div>
        <div class="activity-body">
          <p class="activity-text">
            <strong>{{ activity.student_name }}</strong> 
            <span class="action">was marked present</span> in 
            <span class="course">{{ activity.course_name }}</span>
          </p>
          <span class="activity-time">{{ formatTime(activity.session_date) }}</span>
        </div>
      </div>
    </div>
    
    <div v-else class="empty-activity">
      <p>No recent activity recorded.</p>
    </div>
  </div>
</template>

<script setup>
defineProps({
  activities: {
    type: Array,
    default: () => []
  }
})

const formatTime = (dateString) => {
  if (!dateString) return ''
  const date = new Date(dateString)
  return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
}
</script>

<style scoped>
.recent-activity-panel {
  display: flex;
  flex-direction: column;
  height: 100%;
}

.panel-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.25rem;
  border-bottom: 1px solid var(--border-light);
}

.panel-header h3 {
  font-size: 1rem;
  font-weight: 850;
  color: var(--text-main);
  margin: 0;
}

.live-indicator {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  font-size: 0.65rem;
  font-weight: 800;
  color: var(--success);
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.live-indicator .dot {
  width: 6px;
  height: 6px;
  background: var(--success);
  border-radius: 50%;
  box-shadow: 0 0 10px var(--success);
}

.activity-list {
  display: flex;
  flex-direction: column;
  padding: 0.5rem;
  overflow-y: auto;
}

.activity-item {
  display: flex;
  gap: 0.75rem;
  padding: 0.75rem;
  border-radius: 12px;
  transition: background 0.2s ease;
}

.activity-item:hover {
  background: var(--bg-main);
}

.activity-avatar {
  width: 32px;
  height: 32px;
  background: var(--bg-main);
  border: 1px solid var(--border-light);
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.85rem;
  font-weight: 800;
  color: var(--text-muted);
  flex-shrink: 0;
}

.activity-body {
  flex: 1;
}

.activity-text {
  font-size: 0.85rem;
  line-height: 1.4;
  color: var(--text-main);
  margin: 0 0 0.15rem 0;
}

.activity-text strong {
  font-weight: 700;
}

.activity-text .action {
  color: var(--text-muted);
}

.activity-text .course {
  font-weight: 600;
  color: var(--primary);
}

.activity-time {
  font-size: 0.7rem;
  font-weight: 600;
  color: var(--text-muted);
}

.empty-activity {
  padding: 3rem 1.25rem;
  text-align: center;
  color: var(--text-muted);
  font-size: 0.85rem;
}
</style>
