<template>
  <nav class="navbar shadow-sm">
    <div class="container container-wide">
      <div class="navbar-inner">
        <!-- Logo -->
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

        <!-- Mobile Menu Toggle & Actions -->
        <div class="navbar-actions">
          <!-- Links -->
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
          <!-- Mobile Menu Toggle -->
          <button @click="toggleMobileMenu" class="mobile-menu-toggle" :class="{ 'active': isMobileMenuOpen }">
            <span></span>
            <span></span>
            <span></span>
          </button>
        </div>

        <!-- Mobile Nav Menu -->
        <div class="nav-links-mobile" :class="{ 'mobile-open': isMobileMenuOpen }">
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

// Mobile menu state
const isMobileMenuOpen = ref(false)

const toggleMobileMenu = () => {
  isMobileMenuOpen.value = !isMobileMenuOpen.value
}

const closeMobileMenu = () => {
  isMobileMenuOpen.value = false
}

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
@keyframes slideDown {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

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
  transition: transform 0.3s ease;
}

.brand-link:hover .brand-icon-box {
  transform: scale(1.1);
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
  line-height: 1.2;
}

.brand-tagline {
  font-size: 0.7rem;
  font-weight: 700;
  color: var(--primary);
  text-transform: uppercase;
  letter-spacing: 0.05em;
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
  padding: 0.5rem 0.25rem;
  position: relative;
  transition: all 0.2s;
}

.nav-item:hover {
  color: var(--text-main);
}

.nav-item.router-link-active {
  color: var(--primary);
}

.nav-item::after {
  content: '';
  position: absolute;
  bottom: -4px;
  left: 0;
  width: 100%;
  height: 2px;
  background: var(--primary);
  border-radius: 2px;
  transform: scaleX(0);
  transition: transform 0.3s cubic-bezier(0.19, 1, 0.22, 1);
}

.nav-item:hover::after,
.nav-item.router-link-active::after {
  transform: scaleX(1);
}


.navbar-actions {
  display: flex;
  align-items: center;
  gap: 3rem;
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
  color: var(--text-muted);
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.1rem;
  transition: all 0.2s ease;
}

.action-circle-btn:hover {
  background: var(--bg-main);
  border-color: var(--border-medium);
  transform: translateY(-1px);
  color: var(--text-main);
}

.divider {
  width: 1px;
  height: 24px;
  background: var(--border-light);
}

.logout-btn {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  background: var(--danger-soft);
  border: 1px solid var(--danger-border);
  padding: 0.5rem 1rem;
  border-radius: 10px;
  color: var(--danger);
  font-weight: 700;
  font-size: 0.9rem;
  cursor: pointer;
  transition: all 0.2s;
}

.logout-btn:hover {
  background: var(--danger-soft-hover);
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
  transition: all 0.2s ease;
}

.nav-auth-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 16px rgba(37, 99, 235, 0.2);
}

.mobile-menu-toggle {
  display: none;
  flex-direction: column;
  justify-content: space-around;
  width: 24px;
  height: 24px;
  background: transparent;
  border: none;
  cursor: pointer;
  padding: 0;
  z-index: 1001;
}

.mobile-menu-toggle span {
  width: 20px;
  height: 2px;
  background: var(--text-main);
  border-radius: 2px;
  transition: all 0.3s ease;
  transform-origin: 1px;
}

.mobile-menu-toggle.active span:nth-child(1) {
  transform: rotate(45deg);
}

.mobile-menu-toggle.active span:nth-child(2) {
  opacity: 0;
  transform: translateX(-20px);
}

.mobile-menu-toggle.active span:nth-child(3) {
  transform: rotate(-45deg);
}

.nav-links-mobile {
  display: none;
  position: absolute;
  top: 72px;
  left: 0;
  right: 0;
  background: var(--bg-card);
  border-top: 1px solid var(--border-light);
  box-shadow: var(--shadow-card);
  flex-direction: column;
  padding: 1rem 0;
  z-index: 999;
  animation: slideDown 0.3s ease-out;
}

.nav-links-mobile.mobile-open {
  display: flex;
}

.nav-links-mobile .nav-item {
  padding: 0.75rem 1.5rem;
  text-align: center;
  border-bottom: 1px solid var(--border-light);
  opacity: 0;
  animation: slideDown 0.5s ease-out forwards;
}

.nav-links-mobile.mobile-open .nav-item:nth-child(1) { animation-delay: 0.1s; }
.nav-links-mobile.mobile-open .nav-item:nth-child(2) { animation-delay: 0.15s; }
.nav-links-mobile.mobile-open .nav-item:nth-child(3) { animation-delay: 0.2s; }
.nav-links-mobile.mobile-open .nav-item:nth-child(4) { animation-delay: 0.25s; }


.nav-links-mobile .nav-item:last-child {
  border-bottom: none;
}

.nav-links-mobile .nav-item:hover {
  background: var(--bg-main);
}

@media (max-width: 920px) {
  .nav-links-desktop {
    display: none;
  }
  
  .mobile-menu-toggle {
    display: flex;
  }
}

@media (max-width: 768px) {
  .brand-tagline {
    display: none;
  }
  
  .user-actions {
    gap: 0.5rem;
  }

  .user-actions .divider,
  .user-actions .logout-btn span {
    display: none;
  }

  .logout-btn {
    background: transparent;
    border: none;
    padding: 0.5rem;
    color: var(--danger);
    font-size: 1.25rem;
  }
  
  .action-circle-btn {
    width: 36px;
    height: 36px;
  }
  
  .nav-auth-btn {
    padding: 0.5rem 1rem;
    font-size: 0.875rem;
  }
}
</style>
