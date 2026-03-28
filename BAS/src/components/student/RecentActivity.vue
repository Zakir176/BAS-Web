<template>
  <div class="recent-activity-timeline">
    <div class="timeline-container">
      <template v-if="isLoading">
        <div v-for="i in 3" :key="i" class="timeline-node loading-node">
          <div class="node-dot"></div>
          <div class="node-content placeholder-pulse">
            <Skeleton width="120px" height="1rem" style="margin-bottom: 0.5rem" />
            <Skeleton width="80px" height="0.75rem" />
          </div>
        </div>
      </template>
      
      <template v-else-if="activities.length > 0">
        <transition-group name="timeline-slide" tag="div">
          <div v-for="(item, index) in visibleActivities" :key="item.id" class="timeline-node" :style="{ animationDelay: `${index * 0.1}s` }">
            <div class="node-dot" :class="item.type">
              <span class="inner-glow"></span>
            </div>
            <div class="node-content glass-panel">
              <div class="content-header">
                <h4>{{ item.course }}</h4>
                <span class="badge" :class="item.type">{{ item.type.toUpperCase() }}</span>
              </div>
              <div class="content-meta">
                <span>{{ item.date }}</span> • <span>{{ item.time }}</span>
              </div>
            </div>
          </div>
        </transition-group>
      </template>

      <div v-else class="empty-timeline">
        <div class="empty-icon">📌</div>
        <p>Your attendance log is perfectly clean.</p>
        <span>Records will appear here automatically.</span>
      </div>
    </div>

    <button 
      v-if="!isLoading && activities.length > 5" 
      class="view-all-btn"
      @click="$emit('toggleShowAll')"
    >
      {{ showAll ? 'Collapse' : 'Expand Full History' }}
    </button>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import Skeleton from '@/components/ui/Skeleton.vue'

const props = defineProps({
  isLoading: {
    type: Boolean,
    default: true,
  },
  activities: {
    type: Array,
    default: () => [],
  },
  showAll: {
    type: Boolean,
    default: false,
  },
})

defineEmits(['toggleShowAll'])

const visibleActivities = computed(() => {
  if (props.showAll) {
    return props.activities
  }
  return props.activities.slice(0, 5)
})
</script>

<style scoped>
.recent-activity-timeline {
  padding-top: 1rem;
}

.timeline-container {
  position: relative;
  border-left: 2px dashed rgba(160, 160, 160, 0.2);
  margin-left: 12px;
  padding-left: 24px;
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

[data-theme='dark'] .timeline-container {
  border-left-color: rgba(255, 255, 255, 0.1);
}

.timeline-node {
  position: relative;
  opacity: 0;
  animation: slideIn 0.5s ease forwards;
}

@keyframes slideIn {
  from { opacity: 0; transform: translateX(-10px); }
  to { opacity: 1; transform: translateX(0); }
}

.node-dot {
  position: absolute;
  left: -31px; /* Center over the dashed border */
  top: 12px;
  width: 14px;
  height: 14px;
  border-radius: 50%;
  background: var(--bg-card);
  border: 2px solid var(--text-muted);
  box-shadow: 0 0 0 4px var(--bg-card);
}

.node-dot.present {
  border-color: #10b981;
}

.node-dot.present .inner-glow {
  position: absolute;
  top: 50%; left: 50%;
  transform: translate(-50%, -50%);
  width: 100%; height: 100%;
  background: #10b981;
  border-radius: 50%;
  filter: blur(4px);
  opacity: 0.6;
}

.node-dot.absent {
  border-color: #f43f5e;
}

.node-dot.absent .inner-glow {
  position: absolute;
  top: 50%; left: 50%;
  transform: translate(-50%, -50%);
  width: 100%; height: 100%;
  background: #f43f5e;
  border-radius: 50%;
  filter: blur(4px);
  opacity: 0.6;
}

.node-content.glass-panel {
  background: rgba(255, 255, 255, 0.5);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(0,0,0,0.05);
  border-radius: 16px;
  padding: 1rem 1.25rem;
  transition: transform 0.2s, box-shadow 0.2s;
}

[data-theme='dark'] .node-content.glass-panel {
  background: rgba(30, 41, 59, 0.4);
  border-color: rgba(255,255,255,0.05);
}

.node-content:hover {
  transform: translateY(-2px);
  box-shadow: 0 10px 20px rgba(0,0,0,0.05);
}

.content-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.25rem;
}

.content-header h4 {
  font-size: 0.95rem;
  font-weight: 700;
  color: var(--text-main);
  margin: 0;
}

.badge {
  font-size: 0.65rem;
  font-weight: 800;
  padding: 0.25rem 0.6rem;
  border-radius: 8px;
  letter-spacing: 0.5px;
}

.badge.present { background: rgba(16, 185, 129, 0.15); color: #10b981; }
.badge.absent { background: rgba(244, 63, 94, 0.15); color: #f43f5e; }

.content-meta {
  font-size: 0.8rem;
  color: var(--text-muted);
  font-weight: 500;
}

/* Empty State */
.empty-timeline {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem 1rem;
  text-align: center;
  color: var(--text-muted);
  left: -12px;
  position: relative;
}

.empty-icon {
  font-size: 2.5rem;
  margin-bottom: 1rem;
  opacity: 0.5;
}

.empty-timeline p {
  font-weight: 700;
  color: var(--text-main);
  margin-bottom: 0.25rem;
}

/* Button */
.view-all-btn {
  width: 100%;
  margin-top: 2rem;
  padding: 0.8rem;
  background: transparent;
  border: 1px solid var(--border-medium);
  border-radius: 12px;
  color: var(--text-muted);
  font-weight: 700;
  cursor: pointer;
  transition: all 0.2s;
}

.view-all-btn:hover {
  border-color: var(--primary);
  color: var(--primary);
  background: rgba(79, 70, 229, 0.05);
}

/* Transitions */
.timeline-slide-move,
.timeline-slide-enter-active,
.timeline-slide-leave-active {
  transition: all 0.4s ease;
}

.timeline-slide-enter-from,
.timeline-slide-leave-to {
  opacity: 0;
  transform: translateX(-20px);
}

.timeline-slide-leave-active {
  position: absolute;
}
</style>