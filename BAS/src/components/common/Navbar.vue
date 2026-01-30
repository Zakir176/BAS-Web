<template>
  <nav class="navbar shadow-sm">
    <div class="container container-wide">
      <div class="navbar-inner">
        <div class="brand-zone">
          <router-link to="/" class="brand-link">
            <div class="brand-icon-box">
              <img src="@/assets/logo.jpeg" alt="CAT Logo" class="brand-logo-img" />
            </div>
            <div class="brand-meta">
              <span class="brand-title">CAT</span>
              <span class="brand-tagline">Class Attendance Tracker</span>
            </div>
          </router-link>
        </div>

        <div class="navbar-actions">
          <div class="nav-links-desktop">
            <router-link to="/" class="nav-item" @click="closeMobileMenu">Home</router-link>
            <template v-if="isAuthenticated">
              <router-link 
                :to="user?.role === 'lecturer' ? '/lecturer-dashboard' : '/student-homepage'" 
                class="nav-item"
                @click="closeMobileMenu"
              >
                Dashboard
              </router-link>
              <router-link v-if="user?.role === 'lecturer'" to="/report-page" class="nav-item" @click="closeMobileMenu">Reports</router-link>
            </template>
          </div>

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

          <button @click="toggleMobileMenu" class="mobile-menu-toggle" :class="{ 'active': isMobileMenuOpen }">
            <span></span>
            <span></span>
            <span></span>
          </button>
        </div>

        <div class="mobile-nav-overlay" :class="{ 'open': isMobileMenuOpen }" @click="closeMobileMenu"></div>
        
        <div class="mobile-nav-drawer" :class="{ 'open': isMobileMenuOpen }">
          <div class="drawer-header">
            <div v-if="isAuthenticated && user" class="user-profile-mobile">
              <div class="mobile-avatar">
                <img :src="`https://ui-avatars.com/api/?name=${user.user_metadata?.full_name || 'User'}&background=3b82f6&color=fff`" alt="Avatar">
              </div>
              <div class="mobile-user-info">
                <span class="mobile-user-name">{{ user.user_metadata?.full_name || 'User' }}</span>
                <span class="mobile-user-role">{{ user.role }}</span>
              </div>
            </div>
            <div v-else class="brand-zone-mobile">
              <span class="brand-title">CAT</span>
              <span class="brand-tagline">Class Attendance</span>
            </div>
            <button class="close-btn" @click="closeMobileMenu">✕</button>
          </div>

          <div class="drawer-content">
            <router-link to="/" class="drawer-item" @click="closeMobileMenu">
              <span class="icon">🏠</span> Home
            </router-link>
            
            <template v-if="isAuthenticated">
              <router-link 
                :to="user?.role === 'lecturer' ? '/lecturer-dashboard' : '/student-homepage'" 
                class="drawer-item"
                @click="closeMobileMenu"
              >
                <span class="icon">📊</span> Dashboard
              </router-link>
              <router-link v-if="user?.role === 'lecturer'" to="/report-page" class="drawer-item" @click="closeMobileMenu">
                 <span class="icon">📑</span> Reports
              </router-link>
            </template>
            <template v-else>
               <router-link to="/student-login" class="drawer-item highlight" @click="closeMobileMenu">
                <span class="icon">🔐</span> Sign In
              </router-link>
            </template>
          </div>

          <div class="drawer-footer">
            <button @click="toggleTheme" class="drawer-action-btn">
              <span class="icon">{{ isDark ? '☀️' : '🌙' }}</span>
              {{ isDark ? 'Light Mode' : 'Dark Mode' }}
            </button>
            <button v-if="isAuthenticated" @click="() => { handleSignOut(); closeMobileMenu() }" class="drawer-action-btn logout">
              <span class="icon">⏻</span> Logout
            </button>
          </div>
        </div>
      </div>
    </div>
  </nav>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useTheme } from '@/composables/useTheme'
import { useAuth } from '@/composables/useAuth'

const router = useRouter()
const { isDark, toggleTheme } = useTheme()
const { isAuthenticated, user, signOut } = useAuth()

const isMobileMenuOpen = ref(false)
const toggleMobileMenu = () => { isMobileMenuOpen.value = !isMobileMenuOpen.value }
const closeMobileMenu = () => { isMobileMenuOpen.value = false }

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
  background: var(--bg-navbar);
  backdrop-filter: blur(20px);
  -webkit-backdrop-filter: blur(20px);
  border-bottom: 1px solid var(--border-light);
  position: sticky;
  top: 0;
  z-index: 1000;
  height: 72px;
  display: flex;
  align-items: center;
  transition: background 0.3s ease;
}

.container-wide {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 1rem;
  width: 100%;
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
  width: 2.5rem;
  height: 2.5rem;
  border-radius: 0.5rem;
  background: var(--primary);
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.brand-logo-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  border-radius: 0.5rem;
}

.brand-meta {
  display: flex;
  flex-direction: column;
}

.brand-title {
  font-weight: 800;
  color: var(--text-main);
  font-size: 1.125rem;
}

.brand-tagline {
  font-size: 0.7rem;
  font-weight: 700;
  color: var(--primary);
  text-transform: uppercase;
}

.nav-links-desktop {
  display: flex;
  gap: 2rem;
}

.nav-item {
  text-decoration: none;
  font-weight: 700;
  color: var(--text-muted);
  font-size: 0.9375rem;
  transition: all 0.2s;
}

.navbar-actions {
  display: flex;
  align-items: center;
  gap: 1.5rem; /* Reduced gap for mobile flexibility */
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
  border: 1px solid var(--border-light);
  background: var(--bg-card);
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
}

.nav-auth-btn {
  background: var(--primary);
  color: white;
  padding: 0.6rem 1.25rem;
  border-radius: 10px;
  text-decoration: none;
  font-weight: 700;
  font-size: 0.9rem;
}

/* Mobile Menu Toggle */
.mobile-menu-toggle {
  display: none;
  flex-direction: column;
  justify-content: space-around;
  width: 40px;
  height: 40px;
  background: var(--bg-card);
  border: 1px solid var(--border-light);
  border-radius: 8px;
  cursor: pointer;
  padding: 8px;
  z-index: 1002;
}

.mobile-menu-toggle span {
  width: 100%;
  height: 2px;
  background: var(--text-main);
  transition: all 0.3s ease;
}

/* Mobile Drawer Logic */
.mobile-nav-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.5);
  backdrop-filter: blur(4px);
  z-index: 2000; /* Higher than navbar */
  opacity: 0;
  pointer-events: none;
  transition: opacity 0.3s ease;
}

.mobile-nav-overlay.open {
  opacity: 1;
  pointer-events: auto;
}

.mobile-nav-drawer {
  position: fixed;
  top: 0;
  right: 0;
  bottom: 0;
  width: 80%;
  max-width: 300px;
  background: var(--bg-navbar);
  z-index: 2001; /* Higher than overlay */
  transform: translateX(100%);
  transition: transform 0.3s cubic-bezier(0.16, 1, 0.3, 1);
  display: flex;
  flex-direction: column;
  box-shadow: -10px 0 30px rgba(0,0,0,0.2);
}

.mobile-nav-drawer.open {
  transform: translateX(0);
}

.drawer-header { padding: 1.5rem; border-bottom: 1px solid var(--border-light); display: flex; justify-content: space-between; align-items: center; }
.drawer-content { flex: 1; padding: 1rem; display: flex; flex-direction: column; gap: 0.5rem; overflow-y: auto; }
.drawer-item { display: flex; align-items: center; gap: 1rem; padding: 0.85rem; border-radius: 10px; text-decoration: none; color: var(--text-main); font-weight: 600; }
.drawer-footer { padding: 1.5rem; border-top: 1px solid var(--border-light); display: flex; flex-direction: column; gap: 1rem; }
.drawer-action-btn { display: flex; align-items: center; gap: 0.75rem; width: 100%; padding: 0.75rem; border-radius: 10px; border: 1px solid var(--border-medium); background: none; color: var(--text-main); font-weight: 600; cursor: pointer; }

/* Desktop Hide Logic */
@media (max-width: 920px) {
  .nav-links-desktop,
  .user-actions {
    display: none; /* Hide the overlapping desktop buttons */
  }
  
  .mobile-menu-toggle {
    display: flex;
  }
}

@media (max-width: 768px) {
  .brand-tagline { display: none; }
}
</style>
</style>
