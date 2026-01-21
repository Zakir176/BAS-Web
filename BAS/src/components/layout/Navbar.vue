<template>
  <nav class="navbar shadow-sm">
    <div class="container container-wide">
      <div class="navbar-inner">
        <!-- Logo -->
        <div class="brand-zone">
          <router-link to="/" class="brand-link">
            <div class="brand-icon-box">
              <span class="icon">🎓</span>
            </div>
            <div class="brand-meta">
              <span class="brand-title">Smart Attendance</span>
              <span class="brand-tagline">Academic System</span>
            </div>
          </router-link>
        </div>

        <!-- Links -->
        <div class="nav-links">
          <router-link to="/" class="nav-item">Home</router-link>
          <template v-if="isAuthenticated">
            <router-link 
              :to="user?.role === 'lecturer' ? '/lecturer-dashboard' : '/student-homepage'" 
              class="nav-item"
            >
              Dashboard
            </router-link>
            <router-link v-if="user?.role === 'student'" to="/student-upload-page" class="nav-item">Upload</router-link>
            <router-link v-if="user?.role === 'lecturer'" to="/report-page" class="nav-item">Reports</router-link>
          </template>
        </div>

        <!-- Actions -->
        <div class="user-actions">
          <button @click="toggleTheme" class="action-circle-btn theme-btn" :title="isDark ? 'Light Mode' : 'Dark Mode'">
            <span v-if="isDark">☀️</span>
            <span v-else>🌙</span>
          </button>
          
          <template v-if="isAuthenticated">
            <div class="divider"></div>
            <button @click="handleSignOut" class="logout-btn">
              <span>Logout</span>
              <span class="icon">⏻</span>
            </button>
          </template>
          <template v-else>
            <router-link to="/student-login" class="nav-auth-btn">Sign In</router-link>
          </template>
        </div>
      </div>
    </div>
  </nav>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { useTheme } from '@/composables/useTheme'
import { useAuth } from '@/composables/useAuth'

const router = useRouter()
const { isDark, toggleTheme } = useTheme()
const { isAuthenticated, user, signOut } = useAuth()

const handleSignOut = async () => {
  try {
    await signOut()
    router.push('/')
  } catch (error) {
    console.error('Sign out error:', error)
  }
}
</script>

<style scoped>
.navbar {
  background: rgba(255, 255, 255, 0.85);
  backdrop-filter: blur(20px);
  -webkit-backdrop-filter: blur(20px);
  border-bottom: 1px solid rgba(226, 232, 240, 0.8);
  position: sticky;
  top: 0;
  z-index: 1000;
  height: 72px;
  display: flex;
  align-items: center;
}

.container-wide {
  max-width: 1400px;
}

.navbar-inner {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.brand-link {
  display: flex;
  align-items: center;
  gap: 1rem;
  text-decoration: none;
}

.brand-icon-box {
  width: 44px;
  height: 44px;
  background: var(--primary);
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 4px 12px rgba(37, 99, 235, 0.2);
}

.brand-icon-box .icon {
  font-size: 1.5rem;
}

.brand-meta {
  display: flex;
  flex-direction: column;
}

.brand-title {
  font-weight: 800;
  color: #1e293b;
  font-size: 1.125rem;
  line-height: 1.2;
}

.brand-tagline {
  font-size: 0.7rem;
  font-weight: 700;
  color: #3b82f6;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.nav-links {
  display: flex;
  gap: 2rem;
}

.nav-item {
  text-decoration: none;
  font-weight: 700;
  color: #64748b;
  font-size: 0.9375rem;
  padding: 0.5rem 0.25rem;
  position: relative;
  transition: all 0.2s;
}

.nav-item:hover {
  color: #1e293b;
}

.nav-item.router-link-active {
  color: var(--primary);
}

.nav-item.router-link-active::after {
  content: '';
  position: absolute;
  bottom: -4px;
  left: 0;
  width: 100%;
  height: 2px;
  background: var(--primary);
  border-radius: 2px;
}

.user-actions {
  display: flex;
  align-items: center;
  gap: 1.25rem;
}

.action-circle-btn {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  border: 1px solid #e2e8f0;
  background: white;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.1rem;
  transition: all 0.2s;
}

.action-circle-btn:hover {
  background: #f8fafc;
  transform: translateY(-1px);
}

.divider {
  width: 1px;
  height: 24px;
  background: #e2e8f0;
}

.logout-btn {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  background: #fef2f2;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 10px;
  color: #dc2626;
  font-weight: 700;
  font-size: 0.9rem;
  cursor: pointer;
  transition: all 0.2s;
}

.logout-btn:hover {
  background: #fee2e2;
  transform: translateY(-1px);
}

.nav-auth-btn {
  background: var(--primary);
  color: white;
  padding: 0.6rem 1.25rem;
  border-radius: 10px;
  text-decoration: none;
  font-weight: 700;
  font-size: 0.9rem;
  box-shadow: 0 4px 12px rgba(37, 99, 235, 0.15);
}

@media (max-width: 768px) {
  .nav-links, .brand-tagline {
    display: none;
  }
}
</style>
