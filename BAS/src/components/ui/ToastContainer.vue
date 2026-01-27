<template>
  <teleport to="body">
    <div class="toast-container" :class="position">
      <Toast
        v-for="toast in toasts"
        :key="toast.id"
        :show="toast.show"
        :type="toast.type"
        :title="toast.title"
        :message="toast.message"
        :duration="toast.duration"
        :closable="toast.closable"
        :showProgress="toast.showProgress"
        :pauseOnHover="toast.pauseOnHover"
        :onClick="toast.onClick"
        @close="removeToast(toast.id)"
      />
    </div>
  </teleport>
</template>

<script setup>
import { computed } from 'vue'
import Toast from './Toast.vue'
import { useToast } from '@/composables/useToast'

const props = defineProps({
  position: {
    type: String,
    default: 'top-right',
    validator: (value) => [
      'top-right',
      'top-left',
      'top-center',
      'bottom-right',
      'bottom-left',
      'bottom-center'
    ].includes(value)
  },
  maxToasts: {
    type: Number,
    default: 5
  }
})

const { toasts, removeToast } = useToast()

// Limit the number of visible toasts
const visibleToasts = computed(() => {
  return toasts.value.slice(-props.maxToasts)
})
</script>

<style scoped>
.toast-container {
  position: fixed;
  z-index: 9999;
  pointer-events: none;
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  padding: 1rem;
  max-width: 400px;
}

.toast-container > * {
  pointer-events: auto;
}

/* Positions */
.toast-container.top-right {
  top: 0;
  right: 0;
}

.toast-container.top-left {
  top: 0;
  left: 0;
}

.toast-container.top-center {
  top: 0;
  left: 50%;
  transform: translateX(-50%);
}

.toast-container.bottom-right {
  bottom: 0;
  right: 0;
}

.toast-container.bottom-left {
  bottom: 0;
  left: 0;
}

.toast-container.bottom-center {
  bottom: 0;
  left: 50%;
  transform: translateX(-50%);
}

/* Stack animations */
.toast-container.top-right,
.toast-container.top-left {
  align-items: flex-start;
}

.toast-container.bottom-right,
.toast-container.bottom-left {
  align-items: flex-start;
}

.toast-container.top-center,
.toast-container.bottom-center {
  align-items: center;
}

/* Responsive adjustments */
@media (max-width: 640px) {
  .toast-container {
    max-width: calc(100vw - 2rem);
    padding: 0.75rem;
  }
  
  .toast-container.top-center,
  .toast-container.bottom-center {
    left: 1rem;
    right: 1rem;
    transform: none;
    max-width: none;
  }
}
</style>
