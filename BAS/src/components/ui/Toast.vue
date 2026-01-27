<template>
  <transition name="toast" appear>
    <div 
      v-if="show" 
      class="toast-notification"
      :class="[`toast-${type}`, { 'toast-closable': closable }]"
      @click="handleClick"
    >
      <div class="toast-icon">
        <component :is="iconComponent" />
      </div>
      
      <div class="toast-content">
        <h4 v-if="title" class="toast-title">{{ title }}</h4>
        <p v-if="message" class="toast-message">{{ message }}</p>
      </div>
      
      <button 
        v-if="closable" 
        @click="close" 
        class="toast-close"
        aria-label="Close notification"
      >
        <XMarkIcon />
      </button>
      
      <div 
        v-if="showProgress && duration > 0" 
        class="toast-progress"
        :style="{ animationDuration: `${duration}ms` }"
      ></div>
    </div>
  </transition>
</template>

<script setup>
import { computed, onMounted, onUnmounted, ref } from 'vue'
import { 
  CheckCircleIcon, 
  ExclamationTriangleIcon, 
  XCircleIcon, 
  InformationCircleIcon,
  XMarkIcon
} from '@heroicons/vue/24/outline'

const props = defineProps({
  show: {
    type: Boolean,
    default: false
  },
  type: {
    type: String,
    default: 'info',
    validator: (value) => ['success', 'error', 'warning', 'info'].includes(value)
  },
  title: {
    type: String,
    default: ''
  },
  message: {
    type: String,
    default: ''
  },
  duration: {
    type: Number,
    default: 5000
  },
  closable: {
    type: Boolean,
    default: true
  },
  showProgress: {
    type: Boolean,
    default: true
  },
  pauseOnHover: {
    type: Boolean,
    default: true
  },
  onClick: {
    type: Function,
    default: null
  }
})

const emit = defineEmits(['close'])

const timer = ref(null)
const remainingTime = ref(props.duration)
const isPaused = ref(false)

const iconComponent = computed(() => {
  const icons = {
    success: CheckCircleIcon,
    error: XCircleIcon,
    warning: ExclamationTriangleIcon,
    info: InformationCircleIcon
  }
  return icons[props.type] || InformationCircleIcon
})

const startTimer = () => {
  if (props.duration > 0) {
    timer.value = setTimeout(() => {
      close()
    }, props.duration)
  }
}

const clearTimer = () => {
  if (timer.value) {
    clearTimeout(timer.value)
    timer.value = null
  }
}

const pauseTimer = () => {
  if (props.pauseOnHover && timer.value) {
    clearTimer()
    isPaused.value = true
  }
}

const resumeTimer = () => {
  if (props.pauseOnHover && isPaused.value && remainingTime.value > 0) {
    startTimer()
    isPaused.value = false
  }
}

const close = () => {
  clearTimer()
  emit('close')
}

const handleClick = () => {
  if (props.onClick) {
    props.onClick()
  }
}

onMounted(() => {
  if (props.show) {
    startTimer()
  }
})

onUnmounted(() => {
  clearTimer()
})
</script>

<style scoped>
.toast-notification {
  position: relative;
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
  padding: 1rem;
  border-radius: 0.75rem;
  box-shadow: 0 10px 25px -5px rgba(0, 0, 0, 0.1), 0 8px 10px -6px rgba(0, 0, 0, 0.1);
  background: var(--bg-card);
  border: 1px solid var(--border-light);
  max-width: 400px;
  min-width: 300px;
  overflow: hidden;
}

/* Toast Types */
.toast-success {
  border-left: 4px solid var(--success);
}

.toast-error {
  border-left: 4px solid var(--error);
}

.toast-warning {
  border-left: 4px solid var(--warning);
}

.toast-info {
  border-left: 4px solid var(--primary);
}

/* Icon */
.toast-icon {
  flex-shrink: 0;
  width: 1.25rem;
  height: 1.25rem;
  margin-top: 0.125rem;
}

.toast-success .toast-icon {
  color: var(--success);
}

.toast-error .toast-icon {
  color: var(--error);
}

.toast-warning .toast-icon {
  color: var(--warning);
}

.toast-info .toast-icon {
  color: var(--primary);
}

/* Content */
.toast-content {
  flex: 1;
  min-width: 0;
}

.toast-title {
  font-size: 0.875rem;
  font-weight: 600;
  color: var(--text-main);
  margin: 0 0 0.25rem 0;
  line-height: 1.25;
}

.toast-message {
  font-size: 0.875rem;
  color: var(--text-muted);
  margin: 0;
  line-height: 1.25;
}

/* Close Button */
.toast-close {
  position: absolute;
  top: 0.5rem;
  right: 0.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 1.5rem;
  height: 1.5rem;
  border: none;
  background: transparent;
  color: var(--text-muted);
  border-radius: 0.25rem;
  cursor: pointer;
  transition: all 0.15s ease;
}

.toast-close:hover {
  background: var(--bg-main);
  color: var(--text-main);
}

/* Progress Bar */
.toast-progress {
  position: absolute;
  bottom: 0;
  left: 0;
  height: 3px;
  background: currentColor;
  animation: toast-progress linear forwards;
  opacity: 0.3;
}

.toast-success .toast-progress {
  color: var(--success);
}

.toast-error .toast-progress {
  color: var(--error);
}

.toast-warning .toast-progress {
  color: var(--warning);
}

.toast-info .toast-progress {
  color: var(--primary);
}

@keyframes toast-progress {
  from {
    width: 100%;
  }
  to {
    width: 0%;
  }
}

/* Animations */
.toast-enter-active {
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.toast-leave-active {
  transition: all 0.25s cubic-bezier(0.4, 0, 1, 1);
}

.toast-enter-from {
  opacity: 0;
  transform: translateX(100%) scale(0.9);
}

.toast-leave-to {
  opacity: 0;
  transform: translateX(100%) scale(0.9);
}

/* Hover Effects */
.toast-notification:hover {
  transform: translateY(-2px);
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 8px 10px -6px rgba(0, 0, 0, 0.1);
}

.toast-notification:hover .toast-progress {
  animation-play-state: paused;
}

/* Responsive */
@media (max-width: 640px) {
  .toast-notification {
    max-width: calc(100vw - 2rem);
    min-width: 0;
  }
}
</style>
