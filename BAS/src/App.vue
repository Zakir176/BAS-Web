<script setup>
import { onMounted } from 'vue'
import { useTheme } from '@/shared/composables/useTheme'
import { useAuth } from '@/shared/composables/useAuth'
import ToastContainer from '@/core/ui/ToastContainer.vue'
import AppNavbar from '@/shared/components/AppNavbar.vue'
import { Analytics } from '@vercel/analytics/vue'

// Initialize theme and auth systems
const { updateTheme } = useTheme()
const { initialize } = useAuth()

onMounted(async () => {
  console.log('App: Component mounted, initializing...')
  updateTheme()
  console.log('App: Theme updated')
  if (initialize) {
    await initialize()
  }
  console.log('App: Auth initialization finished')
})
</script>

<template>
  <div id="app">
    <Analytics />
    <AppNavbar />
    <div class="app-content-wrapper">
      <router-view />
    </div>
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

.app-content-wrapper {
  flex: 1;
  padding-top: 96px; /* Optimized gap for the 1rem-offset floating pill */
  display: flex;
  flex-direction: column;
  transition: padding 0.4s ease;
}
</style>
