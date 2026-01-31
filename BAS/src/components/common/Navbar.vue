<template>
  <nav class="navbar shadow-sm">
    <div class="container container-wide">
      <div class="navbar-inner">
        <!-- Brand -->
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

        <!-- Desktop nav -->
        <div class="navbar-actions">
          <div class="nav-tabs">
            <router-link to="/" class="nav-tab">Home</router-link>
            <template v-if="isAuthenticated">
              <router-link
                :to="user?.role === 'lecturer' ? '/lecturer-dashboard' : '/student-homepage'"
                class="nav-tab"
              >
                Dashboard
              </router-link>
              <router-link v-if="user?.role === 'lecturer'" to="/report-page" class="nav-tab">Reports</router-link>
            </template>
          </div>

          <!-- User actions -->
          <div class="user-actions">
            <button @click="toggleTheme" class="action-circle-btn">
              {{ isDark ? '☀️' : '🌙' }}
            </button>

            <template v-if="isAuthenticated">
              <button @click="handleSignOut" class="logout-btn">
                Logout <span class="icon">⏻</span>
              </button>
            </template>
            <template v-else>
              <router-link to="/student-login" class="nav-auth-btn">Sign In</router-link>
            </template>
          </div>

          <!-- Mobile menu toggle -->
          <button @click="toggleMobileMenu" class="mobile-menu-toggle" :class="{ 'active': isMobileMenuOpen }">
            <span></span><span></span><span></span>
          </button>
        </div>

        <!-- Mobile drawer overlay -->
        <div class="mobile-nav-overlay" :class="{ 'open': isMobileMenuOpen }" @click="closeMobileMenu"></div>

        <!-- Mobile drawer -->
        <div class="mobile-nav-drawer" :class="{ 'open': isMobileMenuOpen }">
          <div class="drawer-header">
            <div v-if="isAuthenticated && user" class="user-profile-mobile">
              <div class="mobile-avatar">
                <img
                  :src="`https://ui-avatars.com/api/?name=${user.user_metadata?.full_name || 'User'}&background=3b82f6&color=fff`"
                  alt="Avatar"
                >
              </div>
              <div class="mobile-user-info">
                <span class="mobile-user-name">{{ user.user_metadata?.full_name || 'User' }}</span>
                <span class="mobile-user-role">{{ user.role }}</span>
              </div>
            </div>
            <div v-else class="brand-zone-mobile">
              <span class="brand-title">CAT</span>
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
/* ================== BASE NAVBAR ================== */
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
  padding: 0 1rem;
}

.container-wide { max-width: 1400px; margin: 0 auto; width: 100%; }
.navbar-inner { display: flex; justify-content: space-between; align-items: center; width: 100%; }

.brand-link { display: flex; align-items: center; gap: 1rem; text-decoration: none; }
.brand-icon-box { width: 2.5rem; height: 2.5rem; border-radius: 0.5rem; background: var(--primary); display: flex; align-items: center; justify-content: center; overflow: hidden; }
.brand-logo-img { width: 100%; height: 100%; object-fit: cover; }
.brand-title { font-weight: 800; color: var(--text-main); font-size: 1.125rem; }
.brand-tagline { font-size: 0.7rem; font-weight: 700; color: var(--primary); text-transform: uppercase; }

/* ================== DESKTOP TABS ================== */
.nav-tabs { display: flex; gap: 1.5rem; align-items: center; }

.nav-tab {
  position: relative;
  padding: 0.25rem 0;
  font-weight: 600;
  font-size: 0.95rem;
  text-decoration: none;
  color: var(--text-muted);
  transition: color 0.2s ease;
}

.nav-tab:hover { color: var(--text-main); }

.nav-tab.router-link-exact-active { color: var(--primary); }
.nav-tab.router-link-exact-active::after {
  content: '';
  position: absolute;
  left: 0;
  bottom: -6px;
  width: 100%;
  height: 2px;
  background: var(--primary);
  border-radius: 2px;
}

/* ================== USER ACTIONS ================== */
.user-actions {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-left: 2rem;
  padding-left: 2rem;
  border-left: 1px solid var(--border-light);
}

.action-circle-btn {
  width: 36px;
  height: 36px;
  border-radius: 8px;
  border: 1px solid var(--border-light);
  background: transparent;
  color: var(--text-muted);
  font-size: 1rem;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background 0.2s ease, color 0.2s ease;
}
.action-circle-btn:hover {
  background: var(--bg-card);
  color: var(--text-main);
}

.logout-btn {
  background: none;
  border: none;
  color: var(--text-muted);
  font-weight: 600;
  font-size: 0.9rem;
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  cursor: pointer;
  transition: color 0.2s ease;
}
.logout-btn:hover { color: var(--danger); }

.nav-auth-btn {
  font-weight: 600;
  font-size: 0.9rem;
  color: var(--primary);
  text-decoration: none;
  padding: 0.25rem 0;
}
.nav-auth-btn:hover { text-decoration: underline; }

/* ================== MOBILE ================== */
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

.mobile-nav-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.4);
  backdrop-filter: blur(4px);
  z-index: 2000;
  opacity: 0;
  pointer-events: none;
  transition: opacity 0.3s ease;
}
.mobile-nav-overlay.open { opacity: 1; pointer-events: auto; }

.mobile-nav-drawer {
  position: fixed;
  top: 0; right: 0; bottom: 0;
  width: 85%;
  max-width: 320px;
  background: var(--bg-navbar);
  z-index: 2001;
  transform: translateX(100%);
  transition: transform 0.4s cubic-bezier(0.16, 1, 0.3, 1);
  display: flex;
  flex-direction: column;
  box-shadow: -10px 0 30px rgba(0,0,0,0.1);
}
.mobile-nav-drawer.open { transform: translateX(0); }

.drawer-header { padding: 1.5rem; display: flex; justify-content: space-between; align-items: center; border-bottom: 1px solid var(--border-light); }
.drawer-content { flex: 1; padding: 1.5rem 1rem; display: flex; flex-direction: column; gap: 0.75rem; overflow-y: auto; }
.drawer-footer { padding: 1.5rem; display: flex; flex-direction: column; gap: 1rem; }
.drawer-item { display: flex; align-items: center; gap: 1rem; padding: 0.85rem; border-radius: 10px; text-decoration: none; color: var(--text-main); font-weight: 600; transition: all 0.3s ease; }
.drawer-item.router-link-active { background: rgba(37, 99, 235, 0.1); color: var(--primary); }

.drawer-action-btn { display: flex; align-items: center; gap: 0.75rem; width: 100%; padding: 0.75rem; border-radius: 10px; border: 1px solid var(--border-medium); background: none; color: var(--text-main); font-weight: 600; cursor: pointer; }

@media (max-width: 1024px) {
  .nav-tabs, .user-actions { display: none; }
  .mobile-menu-toggle { display: flex; }
}

@media (max-width: 768px) {
  .brand-tagline { display: none; }
}
</style>
