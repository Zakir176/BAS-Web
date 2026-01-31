<template>
  <section class="history-section">
    <div class="history-header">
      <h3>Recent Activity</h3>
    </div>
    <div class="timeline">
      <template v-if="isLoading">
        <div v-for="i in 3" :key="i" class="timeline-item shadow-sm">
          <Skeleton width="40px" height="40px" radius="12px" />
          <div class="item-content">
            <Skeleton width="100px" height="1rem" style="margin-bottom: 0.5rem" />
            <Skeleton width="150px" height="0.75rem" />
          </div>
        </div>
      </template>
      <template v-else>
        <div v-for="item in visibleActivities" :key="item.id" class="timeline-item shadow-sm">
          <div class="item-icon-box" :class="item.status">
            <span class="icon">{{ item.status === 'present' ? '✓' : '✕' }}</span>
          </div>
          <div class="item-content">
            <div class="item-top">
              <span class="item-status">{{ item.status.toUpperCase() }}</span>
              <span class="badge" :class="item.status">{{ item.status === 'present' ? 'VERIFIED' : 'UNEXCUSED' }}</span>
            </div>
            <div class="item-meta">{{ item.time }}</div>
          </div>
        </div>
      </template>
    </div>
    <button 
      v-if="!isLoading && activities.length > 5" 
      class="view-all-link"
      @click="$emit('toggleShowAll')"
    >
      {{ showAll ? 'Show Less' : 'View All History' }}
    </button>
  </section>
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
.history-section {
  margin-bottom: 2.5rem;
}

.history-header h3 {
  font-size: 1.125rem;
  font-weight: 800;
  margin-bottom: 1rem;
  color: var(--text-main);
}

.timeline {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.timeline-item {
  background: var(--bg-card);
  padding: 1rem;
  border-radius: 18px;
  display: flex;
  align-items: center;
  gap: 1rem;
}

.item-icon-box {
  width: 40px;
  height: 40px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 900;
}

.item-icon-box.present { background: var(--success-bg); color: var(--success); }
.item-icon-box.absent { background: var(--error-bg); color: var(--error); }

.item-content { flex: 1; }

.item-top {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.25rem;
}

.item-status {
  font-weight: 800;
  font-size: 0.9rem;
  color: var(--text-main);
}

.badge {
  font-size: 0.65rem;
  font-weight: 800;
  padding: 0.25rem 0.5rem;
  border-radius: 6px;
}

.badge.present { background: var(--success-bg); color: var(--success); }
.badge.absent { background: var(--error-bg); color: var(--error); }

.item-meta {
  font-size: 0.75rem;
  color: var(--text-muted);
  font-weight: 500;
}

.view-all-link {
  display: block;
  width: 100%;
  margin-top: 1.5rem;
  padding: 1rem;
  background: var(--bg-card);
  border: 1.5px dashed var(--border-medium);
  border-radius: 16px;
  color: var(--text-muted);
  font-weight: 700;
  cursor: pointer;
  transition: all 0.2s;
}

.view-all-link:hover {
  background: var(--bg-main);
  border-color: var(--primary);
  color: var(--primary);
}
</style>