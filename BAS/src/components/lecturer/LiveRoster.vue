<template>
  <section class="content-section py-8">
    <div class="section-header">
      <div class="header-titles">
        <h2>Active Session Roster</h2>
        <p v-if="activeSessionName">Monitoring: <span class="highlight-alt">{{ activeSessionName }}</span></p>
      </div>
      <div class="roster-actions">
        <span class="roster-stats">
          <strong>{{ presentCount }}</strong> Present / {{ activeRoster.length }} Total
        </span>
        <Button 
          v-if="activeSessionId" 
          variant="danger" 
          size="sm" 
          @click="$emit('complete')"
          :disabled="isEndingSession"
          class="ml-4"
        >
          {{ isEndingSession ? 'Ending...' : 'Complete Session' }}
        </Button>
      </div>
    </div>

    <div class="roster-grid-v2" v-if="activeRoster.length > 0">
      <div v-for="student in activeRoster" :key="student.student_id" class="student-card-v2" :class="{ 'is-present': student.present }">
        <div class="student-avatar-mini">
          {{ student.full_name?.charAt(0) }}
        </div>
        <div class="student-info-mini">
          <div class="name">{{ student.full_name }}</div>
          <div class="sid">{{ student.student_id }}</div>
        </div>
        <div class="student-status-action">
          <span v-if="student.present" class="status-icon-check">✓</span>
          <button v-else class="mark-btn-v2" @click="$emit('mark', student.student_id)">
            <span class="icon">📍</span> Mark
          </button>
        </div>
      </div>
    </div>
    <div v-else-if="!isLoading">
      <EmptyState 
        icon="📷"
        title="No Active Session"
        description="Roster visibility is only available during an active session. Start scanning students to populate this area."
      >
        <template #action>
          <Button variant="primary" @click="$emit('scan')">Start Scanning</Button>
        </template>
      </EmptyState>
    </div>
  </section>
</template>

<script setup>
import Button from '@/components/ui/Button.vue'
import EmptyState from '@/components/ui/EmptyState.vue'

defineProps({
  activeRoster: { type: Array, required: true },
  activeSessionName: { type: String, default: '' },
  activeSessionId: { type: [String, Number], default: null },
  presentCount: { type: Number, default: 0 },
  isEndingSession: { type: Boolean, default: false },
  isLoading: { type: Boolean, default: true }
})

defineEmits(['mark', 'complete', 'scan'])
</script>

<style scoped>
.content-section {
  margin-top: 2.5rem;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.header-titles h2 {
  font-size: 1.5rem;
  font-weight: 850;
  margin-bottom: 0.25rem;
}

.highlight-alt {
  color: var(--primary);
  font-weight: 700;
}

.roster-actions {
  display: flex;
  align-items: center;
  gap: 1.5rem;
}

.roster-stats {
  font-size: 0.9rem;
  font-weight: 600;
  color: var(--text-muted);
}

.roster-stats strong {
  color: var(--text-main);
  font-size: 1.1rem;
}

.ml-4 {
  margin-left: 1rem;
}

.roster-grid-v2 {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 1.25rem;
}

.student-card-v2 {
  background: var(--bg-card);
  padding: 1rem;
  border-radius: 20px;
  display: flex;
  align-items: center;
  gap: 1rem;
  border: 1px solid var(--border-light);
  transition: all 0.3s cubic-bezier(0.23, 1, 0.32, 1);
}

.student-card-v2.is-present {
  border-color: var(--success-soft);
  background: var(--success-soft);
  opacity: 0.8;
}

.student-avatar-mini {
  width: 40px;
  height: 40px;
  background: var(--bg-main);
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 800;
  color: var(--primary);
  flex-shrink: 0;
}

.student-info-mini {
  flex: 1;
}

.student-info-mini .name {
  font-size: 0.95rem;
  font-weight: 700;
  color: var(--text-main);
}

.student-info-mini .sid {
  font-size: 0.75rem;
  color: var(--text-muted);
  font-weight: 600;
}

.status-icon-check {
  width: 28px;
  height: 28px;
  background: var(--success);
  color: white;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.8rem;
  font-weight: 900;
}

.mark-btn-v2 {
  padding: 0.5rem 1rem;
  border-radius: 10px;
  border: 1px solid var(--border-medium);
  background: var(--bg-card);
  font-size: 0.85rem;
  font-weight: 700;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 0.4rem;
  transition: all 0.2s;
}

.mark-btn-v2:hover {
  background: var(--primary);
  border-color: var(--primary);
  color: white;
}

@media (max-width: 640px) {
  .section-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 1rem;
  }
}
</style>
