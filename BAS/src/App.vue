<script setup>
import { onMounted } from 'vue'
import { useTheme } from '@/composables/useTheme'
import { useAuth } from '@/composables/useAuth'
import ToastContainer from '@/components/ui/ToastContainer.vue'
import { Analytics } from '@vercel/analytics/vue'

// Initialize theme and auth systems
const { theme, toggleTheme, updateTheme } = useTheme()
const { init } = useAuth()

onMounted(async () => {
  console.log('App: Component mounted, initializing...')
  updateTheme()
  console.log('App: Theme updated')
  await init()
  console.log('App: Auth initialization finished')
})
</script>

<template>
  <div id="app">
    <Analytics />
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
