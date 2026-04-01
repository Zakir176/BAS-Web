<template>
  <div class="empty-state">
    <div class="icon-box">
      <span class="icon">{{ icon }}</span>
    </div>
    <h3 class="title">{{ title }}</h3>
    <p class="description">{{ description }}</p>
    <div v-if="$slots.action" class="action">
      <slot name="action"></slot>
    </div>
  </div>
</template>

<script setup>
defineProps({
  icon: {
    type: String,
    default: '🔍'
  },
  title: {
    type: String,
    default: 'No Results Found'
  },
  description: {
    type: String,
    default: 'Try adjusting your filters or search terms.'
  }
})
</script>

<style scoped>
.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem 2rem;
  text-align: center;
  background: var(--bg-card);
  border-radius: 24px;
  border: 2px dashed var(--border-light);
}

.icon-box {
  width: 64px;
  height: 64px;
  background: var(--bg-main);
  border-radius: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 2rem;
  margin-bottom: 1.5rem;
  box-shadow: var(--shadow-soft);
}

.title {
  font-size: 1.25rem;
  font-weight: 800;
  color: var(--text-main);
  margin-bottom: 0.5rem;
}

.description {
  color: var(--text-muted);
  max-width: 300px;
  line-height: 1.5;
  margin-bottom: 2rem;
}

.action {
  margin-top: 1rem;
}
</style>
