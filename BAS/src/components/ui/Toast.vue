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
  gap: 1rem;
  padding: 1.125rem;
  border-radius: 1.25rem;
  background: rgba(var(--bg-card-rgb, 255, 255, 255), 0.85);
  backdrop-filter: blur(12px) saturate(180%);
  -webkit-backdrop-filter: blur(12px) saturate(180%);
  border: 1px solid rgba(255, 255, 255, 0.2);
  box-shadow: 
    0 10px 15px -3px rgba(0, 0, 0, 0.05),
    0 4px 6px -2px rgba(0, 0, 0, 0.02),
    inset 0 0 0 1px rgba(255, 255, 255, 0.1);
  max-width: 420px;
  min-width: 320px;
  overflow: hidden;
  transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
}

:border-deep[data-theme='dark'] .toast-notification {
  background: rgba(30, 41, 59, 0.8);
  border: 1px solid rgba(255, 255, 255, 0.05);
  box-shadow: 
    0 20px 25px -5px rgba(0, 0, 0, 0.3),
    0 10px 10px -5px rgba(0, 0, 0, 0.2);
}

/* Toast Types Accents */
.toast-success {
  border-bottom: 3px solid var(--success);
}

.toast-error {
  border-bottom: 3px solid var(--error);
}

.toast-warning {
  border-bottom: 3px solid var(--warning);
}

.toast-info {
  border-bottom: 3px solid var(--primary);
}

/* Icon Container */
.toast-icon {
  flex-shrink: 0;
  width: 2.25rem;
  height: 2.25rem;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 0.75rem;
  background: rgba(var(--primary-rgb), 0.1);
  transition: transform 0.3s ease;
}

.toast-success .toast-icon { background: rgba(34, 197, 94, 0.1); color: #22c55e; }
.toast-error .toast-icon { background: rgba(239, 68, 68, 0.1); color: #ef4444; }
.toast-warning .toast-icon { background: rgba(245, 158, 11, 0.1); color: #f59e0b; }
.toast-info .toast-icon { background: rgba(59, 130, 246, 0.1); color: #3b82f6; }

.toast-icon svg {
  width: 1.25rem;
  height: 1.25rem;
  stroke-width: 2.5px;
}

/* Content */
.toast-content {
  flex: 1;
  min-width: 0;
  padding-top: 0.25rem;
}

.toast-title {
  font-size: 0.9375rem;
  font-weight: 700;
  color: var(--text-main);
  margin: 0 0 0.25rem 0;
  line-height: 1.2;
  letter-spacing: -0.01em;
}

.toast-message {
  font-size: 0.875rem;
  color: var(--text-muted);
  margin: 0;
  line-height: 1.4;
  font-weight: 500;
}

/* Close Button */
.toast-close {
  position: absolute;
  top: 0.75rem;
  right: 0.75rem;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 1.75rem;
  height: 1.75rem;
  border: none;
  background: rgba(0, 0, 0, 0.05);
  color: var(--text-muted);
  border-radius: 50%;
  cursor: pointer;
  transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
  opacity: 0.6;
}

.toast-close:hover {
  background: rgba(239, 68, 68, 0.1);
  color: #ef4444;
  transform: rotate(90deg);
  opacity: 1;
}

.toast-close svg {
  width: 1rem;
  height: 1rem;
  stroke-width: 2.5px;
}

/* Progress Bar */
.toast-progress {
  position: absolute;
  bottom: 0;
  left: 0;
  height: 3px;
  background: currentColor;
  animation: toast-progress linear forwards;
  opacity: 0.6;
}

.toast-success .toast-progress { color: #22c55e; }
.toast-error .toast-progress { color: #ef4444; }
.toast-warning .toast-progress { color: #f59e0b; }
.toast-info .toast-progress { color: #3b82f6; }

@keyframes toast-progress {
  from { width: 100%; }
  to { width: 0%; }
}

/* Animations */
.toast-enter-active {
  transition: all 0.5s cubic-bezier(0.19, 1, 0.22, 1);
}

.toast-leave-active {
  transition: all 0.4s cubic-bezier(1, 0, 0, 1);
}

.toast-enter-from {
  opacity: 0;
  transform: translateX(30px) scale(0.95);
  filter: blur(4px);
}

.toast-leave-to {
  opacity: 0;
  transform: translateX(30px) scale(0.95) translateY(-10px);
  filter: blur(8px);
}

/* Hover Effects */
.toast-notification:hover {
  transform: translateY(-4px) scale(1.02);
  box-shadow: 
    0 20px 25px -5px rgba(0, 0, 0, 0.1),
    0 10px 10px -5px rgba(0, 0, 0, 0.04);
  background: rgba(var(--bg-card-rgb, 255, 255, 255), 0.95);
}

.toast-notification:hover .toast-icon {
  transform: scale(1.1) rotate(5deg);
}

.toast-notification:hover .toast-progress {
  animation-play-state: paused;
}

/* Responsive */
@media (max-width: 640px) {
  .toast-notification {
    max-width: calc(100vw - 2.5rem);
    min-width: 0;
    margin: 0 1.25rem;
    border-radius: 1rem;
  }
}
</style>
