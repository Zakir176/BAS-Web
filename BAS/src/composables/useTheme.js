import { ref, watchEffect } from 'vue'

const theme = ref(localStorage.getItem('theme') || 'light')

export function useTheme() {
  const toggleTheme = () => {
    theme.value = theme.value === 'light' ? 'dark' : 'light'
    localStorage.setItem('theme', theme.value)
    updateTheme()
  }

  const updateTheme = () => {
    document.documentElement.setAttribute('data-theme', theme.value)
  }

  watchEffect(() => {
    updateTheme()
  })

  return {
    theme,
    toggleTheme,
    isDark: () => theme.value === 'dark'
  }
}
