<script setup>
import { onMounted } from 'vue'
import { useTheme } from '@/composables/useTheme'
import { useAuth } from '@/composables/useAuth'
import ToastContainer from '@/components/ui/ToastContainer.vue'
import Navbar from '@/components/common/Navbar.vue'
import { Analytics } from '@vercel/analytics/vue'

// Initialize theme and auth systems
const { theme, toggleTheme, updateTheme } = useTheme()
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
    <Navbar />
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
  padding-top: 72px; /* To prevent content from being hidden behind the fixed navbar */
  display: flex;
  flex-direction: column;
}
</style>
