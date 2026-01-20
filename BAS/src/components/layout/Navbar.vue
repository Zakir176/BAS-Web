<template>
  <nav class="navbar">
    <div class="container">
      <div class="navbar-content">
        <div class="navbar-brand">
          <router-link to="/" class="brand-link">
            <div class="logo">
              <svg width="32" height="32" viewBox="0 0 32 32" fill="none">
                <rect width="32" height="32" rx="8" fill="var(--accent-primary)"/>
                <path d="M8 12h16v8H8z" fill="white"/>
                <path d="M12 8v16h8V8h-8z" fill="var(--accent-primary)"/>
              </svg>
            </div>
            <span class="brand-text">BAS</span>
          </router-link>
        </div>
        
        <div class="navbar-menu">
          <router-link to="/" class="nav-link">Home</router-link>
          <template v-if="!isAuthenticated">
            <router-link to="/student-login" class="nav-link">Student Login</router-link>
            <router-link to="/lecturer-login" class="nav-link">Lecturer Login</router-link>
          </template>
          <template v-else>
            <router-link 
              :to="user?.role === 'lecturer' ? '/lecturer-dashboard' : '/student-homepage'" 
              class="nav-link"
            >
              Dashboard
            </router-link>
            <router-link v-if="user?.role === 'student'" to="/student-upload-page" class="nav-link">Upload</router-link>
            <router-link v-if="user?.role === 'lecturer'" to="/report-page" class="nav-link">Reports</router-link>
            <button @click="handleSignOut" class="nav-link sign-out-btn">Sign Out</button>
          </template>
        </div>
        
        <div class="navbar-actions">
          <button @click="toggleTheme" class="theme-toggle">
            <svg v-if="isDark" width="20" height="20" viewBox="0 0 20 20" fill="currentColor">
              <path fill-rule="evenodd" d="M10 2a1 1 0 011 1v1a1 1 0 11-2 0V3a1 1 0 011-1zm4 8a4 4 0 11-8 0 4 4 0 018 0zm-.464 4.95l.707.707a1 1 0 001.414-1.414l-.707-.707a1 1 0 00-1.414 1.414zm2.12-10.607a1 1 0 010 1.414l-.706.707a1 1 0 11-1.414-1.414l.707-.707a1 1 0 011.414 0zM17 11a1 1 0 100-2h-1a1 1 0 100 2h1zm-7 4a1 1 0 011 1v1a1 1 0 11-2 0v-1a1 1 0 011-1zM5.05 6.464A1 1 0 106.465 5.05l-.708-.707a1 1 0 00-1.414 1.414l.707.707zm1.414 8.486l-.707.707a1 1 0 01-1.414-1.414l.707-.707a1 1 0 011.414 1.414zM4 11a1 1 0 100-2H3a1 1 0 000 2h1z" clip-rule="evenodd"/>
            </svg>
            <svg v-else width="20" height="20" viewBox="0 0 20 20" fill="currentColor">
              <path d="M17.293 13.293A8 8 0 016.707 2.707a8.001 8.001 0 1010.586 10.586z"/>
            </svg>
          </button>
        </div>
      </div>
    </div>
  </nav>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useTheme } from '@/composables/useTheme'
import { useAuth } from '@/composables/useAuth'

const router = useRouter()
const { isDark, toggleTheme } = useTheme()
const { isAuthenticated, user, signOut } = useAuth()

const isMobileMenuOpen = ref(false)

const handleSignOut = async () => {
  try {
    await signOut()
    router.push('/')
  } catch (error) {
    console.error('Sign out error:', error)
  }
}

onMounted(() => {
  toggleTheme()
})
</script>

<style scoped>
.navbar {
  background-color: var(--card-bg);
  border-bottom: 1px solid var(--border-primary);
  position: sticky;
  top: 0;
  z-index: 50;
  backdrop-filter: blur(8px);
  background-color: rgba(var(--card-bg), 0.8);
}

.navbar-content {
  display: flex;
  align-items: center;
  justify-content: space-between;
  height: 4rem;
}

.navbar-brand {
  flex-shrink: 0;
}

.brand-link {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  text-decoration: none;
  color: var(--text-primary);
  font-weight: 600;
  font-size: 1.25rem;
}

.brand-text {
  color: var(--accent-primary);
}

.navbar-menu {
  display: flex;
  align-items: center;
  gap: 2rem;
}

.nav-link {
  color: var(--text-secondary);
  text-decoration: none;
  font-weight: 500;
  transition: color 0.2s ease;
  position: relative;
}

.nav-link:hover,
.nav-link.router-link-active {
  color: var(--accent-primary);
}

.nav-link.router-link-active::after {
  content: '';
  position: absolute;
  bottom: -1rem;
  left: 0;
  right: 0;
  height: 2px;
  background-color: var(--accent-primary);
}

.sign-out-btn {
  background: none;
  border: none;
  padding: 0;
  font-family: inherit;
  font-size: inherit;
  cursor: pointer;
}

.navbar-actions {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.theme-toggle {
  background: none;
  border: none;
  color: var(--text-secondary);
  cursor: pointer;
  padding: 0.5rem;
  border-radius: 0.5rem;
  transition: all 0.2s ease;
}

.theme-toggle:hover {
  background-color: var(--bg-tertiary);
  color: var(--text-primary);
}

@media (max-width: 768px) {
  .navbar-menu {
    display: none;
  }
}
</style>
