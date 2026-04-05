<template>
  <div 
    class="skeleton" 
    :class="[shape]" 
    :style="{ width, height, borderRadius: radius }"
  >
    <div class="shimmer"></div>
  </div>
</template>

<script setup>
defineProps({
  width: {
    type: String,
    default: '100%'
  },
  height: {
    type: String,
    default: '1rem'
  },
  shape: {
    type: String,
    default: 'rect', // 'rect', 'circle'
    validator: (v) => ['rect', 'circle'].includes(v)
  },
  radius: {
    type: String,
    default: null
  }
})
</script>

<style scoped>
.skeleton {
  position: relative;
  overflow: hidden;
  background-color: var(--border-light);
  display: inline-block;
}

.skeleton.circle {
  border-radius: 50%;
}

.skeleton.rect {
  border-radius: 8px;
}

.shimmer {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: linear-gradient(
    90deg,
    transparent 0%,
    rgba(255, 255, 255, 0.05) 50%,
    transparent 100%
  );
  animation: shimmer 1.5s infinite;
}

@keyframes shimmer {
  0% {
    transform: translateX(-100%);
  }
  100% {
    transform: translateX(100%);
  }
}
</style>
