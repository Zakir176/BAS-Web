<script setup>
import { onMounted } from 'vue'
import { useTheme } from '@/composables/useTheme'
import { useUserStore } from '@/stores/userStore'

// Initialize theme and auth systems
const { updateTheme } = useTheme()
const userStore = useUserStore()

onMounted(async () => {
  updateTheme()
  await userStore.initialize()
})
</script>

<template>
  <div id="app">
    <router-view />
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
