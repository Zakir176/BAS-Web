<script setup>
import { onMounted } from 'vue'
import { useTheme } from '@/composables/useTheme'
import { useAuth } from '@/composables/useAuth'
import ToastContainer from '@/components/ui/ToastContainer.vue'

// Initialize theme and auth systems
const { theme, toggleTheme, updateTheme } = useTheme()
const { init } = useAuth()

onMounted(async () => {
  updateTheme()
  await init()
})
</script>

<template>
  <div id="app">
    <router-view />
    <ToastContainer position="top-right" :max-toasts="5" />
  </div>
</template>

<style>
#app {
  font-family: 'Inter', system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  min-height: 100vh;
}

* {
  box-sizing: border-box;
}

body {
  margin: 0;
  padding: 0;
}
</style>
