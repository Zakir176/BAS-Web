<template>
  <div :class="cardClasses">
    <div v-if="$slots.header" class="card-header">
      <slot name="header" />
    </div>
    <div class="card-body">
      <slot />
    </div>
    <div v-if="$slots.footer" class="card-footer">
      <slot name="footer" />
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  padding: {
    type: String,
    default: 'normal',
    validator: (value) => ['none', 'sm', 'normal', 'lg'].includes(value)
  },
  shadow: {
    type: String,
    default: 'sm',
    validator: (value) => ['none', 'sm', 'md', 'lg'].includes(value)
  },
  rounded: {
    type: String,
    default: 'md',
    validator: (value) => ['none', 'sm', 'md', 'lg', 'xl'].includes(value)
  }
})

const cardClasses = computed(() => {
  const baseClasses = 'card'
  const paddingClasses = {
    none: '',
    sm: 'p-4',
    normal: 'p-6',
    lg: 'p-8'
  }[props.padding]
  
  const shadowClasses = {
    none: '',
    sm: 'shadow-sm',
    md: 'shadow-md',
    lg: 'shadow-lg'
  }[props.shadow]
  
  const roundedClasses = {
    none: '',
    sm: 'rounded-sm',
    md: 'rounded-md',
    lg: 'rounded-lg',
    xl: 'rounded-xl'
  }[props.rounded]
  
  return [baseClasses, paddingClasses, shadowClasses, roundedClasses].filter(Boolean).join(' ')
})
</script>

<style scoped>
.card-header {
  border-bottom: 1px solid var(--border-primary);
  padding-bottom: 1rem;
  margin-bottom: 1rem;
}

.card-body {
  flex: 1;
}

.card-footer {
  border-top: 1px solid var(--border-primary);
  padding-top: 1rem;
  margin-top: 1rem;
}
</style>
