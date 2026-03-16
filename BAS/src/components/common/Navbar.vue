<template>
  <header class="premium-navbar" :class="{ 'scrolled': isScrolled }">
    <div class="nav-container">
      <!-- Logo -->
      <router-link to="/" class="brand-logo" @click="closeMobileMenu">
        <div class="logo-mark">
          <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
            <rect width="24" height="24" rx="6" fill="currentColor" />
            <path d="M6 9h12v6H6z" fill="var(--bg-card)" />
            <path d="M9 6v12h6V6H9z" fill="currentColor" />
          </svg>
        </div>
        <span class="brand-text">BAS</span>
      </router-link>

      <!-- Desktop Navigation -->
      <nav class="desktop-nav">
        <!-- Not Authenticated Links -->
        <template v-if="!isAuthenticated">
          <router-link to="/" class="nav-link">Home</router-link>
          <div class="nav-actions">
            <Button variant="secondary" size="sm" @click="() => routeTo('/student-login')" class="action-btn">Sign In</Button>
            <Button variant="primary" size="sm" @click="() => routeTo('/student-signup')" class="action-btn highlight">Get Started</Button>
          </div>
        </template>

        <!-- Authenticated Links -->
        <template v-else>
          <router-link :to="dashboardRoute" class="nav-link">Dashboard</router-link>
          <router-link v-if="role === 'student'" to="/report-page" class="nav-link">Reports</router-link>
          <div class="user-actions">
            <!-- Theme Toggle Component placeholder, or simple button -->
            <button @click="handleToggleTheme" class="theme-toggle" aria-label="Toggle Theme">
              <span class="icon">{{ theme === 'dark' ? '☀️' : '🌙' }}</span>
            </button>
            <div class="user-profile-btn" @click="toggleDropdown">
              <div class="avatar">{{ userInitials }}</div>
              <span class="user-name">{{ userFirstName }}</span>
              <svg class="chevron" :class="{ 'open': dropdownOpen }" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <polyline points="6 9 12 15 18 9"></polyline>
              </svg>
            </div>
            
            <!-- User Dropdown Menu -->
            <Transition name="fade-down">
              <div v-if="dropdownOpen" class="user-dropdown">
                <div class="dropdown-header">
                  <p class="dropdown-name">{{ userFullName }}</p>
                  <p class="dropdown-email">{{ userEmail }}</p>
                </div>
                <div class="dropdown-divider"></div>
                <button class="dropdown-item text-danger" @click="handleSignOut">
                  <span class="icon">🚪</span> Sign Out
                </button>
              </div>
            </Transition>
          </div>
        </template>
        
        <!-- Theme toggle for unauthenticated users -->
        <button v-if="!isAuthenticated" @click="handleToggleTheme" class="theme-toggle unauth-theme" aria-label="Toggle Theme">
          <span class="icon">{{ theme === 'dark' ? '☀️' : '🌙' }}</span>
        </button>
      </nav>

      <!-- Mobile Hamburger Toggle -->
      <button class="mobile-toggle" @click="toggleMobileMenu" :class="{ 'active': mobileMenuOpen }" aria-label="Toggle Menu">
        <span class="bar"></span>
        <span class="bar"></span>
        <span class="bar"></span>
      </button>
    </div>

    <!-- Mobile Navigation Drawer -->
    <Transition name="slide-right">
      <div v-if="mobileMenuOpen" class="mobile-nav-overlay" @click.self="closeMobileMenu">
        <div class="mobile-nav-drawer">
          <div class="drawer-header">
            <span class="brand-text">Menu</span>
            <button @click="handleToggleTheme" class="theme-toggle mobile-theme" aria-label="Toggle Theme">
              <span class="icon">{{ theme === 'dark' ? '☀️' : '🌙' }} {{ theme === 'dark' ? 'Light Mode' : 'Dark Mode' }}</span>
            </button>
          </div>
          
          <div class="drawer-content">
            <template v-if="!isAuthenticated">
              <router-link to="/" class="mobile-link" @click="closeMobileMenu">Home</router-link>
              <div class="mobile-divider"></div>
              <p class="mobile-section-title">Students</p>
              <router-link to="/student-login" class="mobile-link" @click="closeMobileMenu">Student Sign In</router-link>
              <router-link to="/student-signup" class="mobile-link" @click="closeMobileMenu">Create Student Account</router-link>
              <div class="mobile-divider"></div>
              <p class="mobile-section-title">Faculty</p>
              <router-link to="/lecturer-login" class="mobile-link" @click="closeMobileMenu">Lecturer Portal</router-link>
            </template>

            <template v-else>
              <div class="mobile-user-card">
                <div class="avatar lg">{{ userInitials }}</div>
                <div class="user-info">
                  <p class="user-name">{{ userFullName }}</p>
                  <p class="user-role">{{ role === 'lecturer' ? 'Lecturer' : 'Student' }}</p>
                </div>
              </div>
              <div class="mobile-divider"></div>
              <router-link :to="dashboardRoute" class="mobile-link" @click="closeMobileMenu">Dashboard</router-link>
              <router-link v-if="role === 'student'" to="/report-page" class="mobile-link" @click="closeMobileMenu">My Reports</router-link>
              <div class="mobile-divider"></div>
              <button class="mobile-link text-danger w-full text-left" @click="handleSignOut">Sign Out</button>
            </template>
          </div>
        </div>
      </div>
    </Transition>
  </header>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue';
import { useRouter } from 'vue-router';
import { useAuth } from '@/composables/useAuth';
import { useTheme } from '@/composables/useTheme';
import Button from '@/components/ui/Button.vue';

const router = useRouter();
const { user, isAuthenticated, role, signOut } = useAuth();
const { theme, toggleTheme } = useTheme();

const isScrolled = ref(false);
const mobileMenuOpen = ref(false);
const dropdownOpen = ref(false);

const handleScroll = () => {
  isScrolled.value = window.scrollY > 20;
};

const toggleMobileMenu = () => {
  mobileMenuOpen.value = !mobileMenuOpen.value;
  if (mobileMenuOpen.value) {
    document.body.style.overflow = 'hidden';
  } else {
    document.body.style.overflow = '';
  }
};

const closeMobileMenu = () => {
  mobileMenuOpen.value = false;
  document.body.style.overflow = '';
};

const toggleDropdown = (e) => {
  e.stopPropagation();
  dropdownOpen.value = !dropdownOpen.value;
};

const closeDropdown = () => {
  dropdownOpen.value = false;
};

const handleToggleTheme = () => {
  toggleTheme();
  // Ensure the theme is saved
  localStorage.setItem('bas-theme', theme.value);
};

const routeTo = (path) => {
  router.push(path);
  closeMobileMenu();
};

const handleSignOut = async () => {
  await signOut();
  closeMobileMenu();
  closeDropdown();
  router.push('/');
};

const dashboardRoute = computed(() => {
  return role.value === 'lecturer' ? '/lecturer-dashboard' : '/student-homepage';
});

const userFullName = computed(() => {
  return user.value?.user_metadata?.full_name || user.value?.email?.split('@')[0] || 'User';
});

const userFirstName = computed(() => {
  return userFullName.value.split(' ')[0];
});

const userInitials = computed(() => {
  const names = userFullName.value.split(' ');
  if (names.length >= 2) {
    return `${names[0][0]}${names[1][0]}`.toUpperCase();
  }
  return userFullName.value.substring(0, 2).toUpperCase();
});

const userEmail = computed(() => {
  return user.value?.email || '';
});

onMounted(() => {
  window.addEventListener('scroll', handleScroll, { passive: true });
  document.addEventListener('click', closeDropdown);
});

onUnmounted(() => {
  window.removeEventListener('scroll', handleScroll);
  document.removeEventListener('click', closeDropdown);
  document.body.style.overflow = ''; // Cleanup just in case
});
</script>

<style scoped>
/* Navbar Base Styles */
.premium-navbar {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  height: 72px;
  z-index: 1000;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  background: transparent;
  border-bottom: 1px solid transparent;
}

.premium-navbar.scrolled {
  background: var(--bg-navbar);
  backdrop-filter: blur(16px);
  -webkit-backdrop-filter: blur(16px);
  border-bottom: 1px solid var(--border-light);
  box-shadow: 0 4px 20px -10px rgba(0, 0, 0, 0.1);
}

.nav-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 1.5rem;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

/* Brand Logo */
.brand-logo {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  text-decoration: none;
  z-index: 1001; /* Keep above mobile menu */
}

.logo-mark {
  color: var(--primary);
  display: flex;
  align-items: center;
  justify-content: center;
  transition: transform 0.3s ease;
}

.brand-logo:hover .logo-mark {
  transform: scale(1.05) rotate(5deg);
}

.brand-text {
  font-size: 1.5rem;
  font-weight: 900;
  letter-spacing: -0.02em;
  color: var(--text-main);
}

/* Desktop Navigation */
.desktop-nav {
  display: none;
  align-items: center;
  gap: 2rem;
}

@media (min-width: 768px) {
  .desktop-nav {
    display: flex;
  }
}

.nav-link {
  color: var(--text-muted);
  text-decoration: none;
  font-weight: 600;
  font-size: 0.95rem;
  position: relative;
  transition: color 0.2s ease;
}

.nav-link:hover,
.nav-link.router-link-active {
  color: var(--primary);
}

.nav-link::after {
  content: '';
  position: absolute;
  bottom: -4px;
  left: 0;
  width: 0%;
  height: 2px;
  background-color: var(--primary);
  transition: width 0.3s ease;
  border-radius: 2px;
}

.nav-link.router-link-active::after {
  width: 100%;
}

.nav-actions {
  display: flex;
  align-items: center;
  gap: 1rem;
}

/* Theme Toggle */
.theme-toggle {
  background: var(--bg-main);
  border: 1px solid var(--border-medium);
  border-radius: 50%;
  width: 36px;
  height: 36px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  color: var(--text-main);
  transition: all 0.2s ease;
}

.theme-toggle.unauth-theme {
  margin-left: -1rem;
}

.theme-toggle:hover {
  background: var(--bg-card);
  transform: translateY(-1px);
  box-shadow: var(--shadow-soft);
  color: var(--primary);
  border-color: var(--primary);
}

.theme-toggle .icon {
  font-size: 1rem;
  line-height: 1;
}

/* User Profile Dropdown */
.user-actions {
  display: flex;
  align-items: center;
  gap: 1.25rem;
  position: relative;
}

.user-profile-btn {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  cursor: pointer;
  padding: 0.35rem 0.75rem 0.35rem 0.35rem;
  background: var(--bg-main);
  border: 1px solid var(--border-medium);
  border-radius: 999px;
  transition: all 0.2s ease;
}

.user-profile-btn:hover {
  background: var(--bg-card);
  border-color: var(--primary);
  box-shadow: var(--shadow-soft);
}

.avatar {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  background: var(--primary);
  color: var(--text-inverse);
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 800;
  font-size: 0.85rem;
  letter-spacing: 0.05em;
}

.avatar.lg {
  width: 48px;
  height: 48px;
  font-size: 1.25rem;
}

.user-name {
  font-weight: 600;
  font-size: 0.9rem;
  color: var(--text-main);
}

.chevron {
  transition: transform 0.3s ease;
}

.chevron.open {
  transform: rotate(180deg);
}

.user-dropdown {
  position: absolute;
  top: calc(100% + 10px);
  right: 0;
  width: 240px;
  background: var(--bg-card);
  border: 1px solid var(--border-light);
  border-radius: 16px;
  padding: 0.5rem;
  box-shadow: var(--shadow-card);
  z-index: 1010;
}

.dropdown-header {
  padding: 1rem;
}

.dropdown-name {
  font-weight: 700;
  color: var(--text-main);
  margin: 0 0 0.25rem 0;
}

.dropdown-email {
  font-size: 0.8rem;
  color: var(--text-muted);
  margin: 0;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.dropdown-divider {
  height: 1px;
  background: var(--border-light);
  margin: 0.5rem 0;
}

.dropdown-item {
  width: 100%;
  text-align: left;
  padding: 0.75rem 1rem;
  background: transparent;
  border: none;
  font-weight: 600;
  font-size: 0.9rem;
  color: var(--text-main);
  border-radius: 10px;
  cursor: pointer;
  transition: background 0.2s ease;
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.dropdown-item:hover {
  background: var(--bg-main);
}

.text-danger {
  color: var(--error) !important;
}

.text-danger:hover {
  background: var(--error-bg) !important;
}

/* Mobile Toggle Hamburger */
.mobile-toggle {
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  width: 24px;
  height: 18px;
  background: transparent;
  border: none;
  cursor: pointer;
  padding: 0;
  z-index: 1015; /* Above drawer and overlay */
}

@media (min-width: 768px) {
  .mobile-toggle {
    display: none;
  }
}

.mobile-toggle .bar {
  width: 100%;
  height: 2px;
  background-color: var(--text-main);
  border-radius: 2px;
  transition: all 0.3s ease;
}

.mobile-toggle.active .bar {
  background-color: var(--text-main); /* Ensure it is dark on the white background */
}

.mobile-toggle.active .bar:nth-child(1) {
  transform: translateY(8px) rotate(45deg);
}

.mobile-toggle.active .bar:nth-child(2) {
  opacity: 0;
}

.mobile-toggle.active .bar:nth-child(3) {
  transform: translateY(-8px) rotate(-45deg);
}

/* Mobile Navigation Drawer */
.mobile-nav-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.4);
  backdrop-filter: blur(4px);
  -webkit-backdrop-filter: blur(4px);
  z-index: 1010; /* Ensure overlay is above navbar */
}

.mobile-nav-drawer {
  position: absolute;
  top: 0;
  right: 0;
  bottom: 0;
  width: 300px; /* Slightly wider for better content fit */
  max-width: 85vw;
  background: var(--bg-card);
  box-shadow: -10px 0 30px rgba(0, 0, 0, 0.1);
  display: flex;
  flex-direction: column;
  padding: 5rem 1.5rem 2rem;
  padding-bottom: calc(2rem + env(safe-area-inset-bottom)); /* Safe area for iOS */
  overflow-y: auto;
  z-index: 1011;
}

.drawer-header {
  position: absolute;
  top: 1.5rem;
  left: 1.5rem;
  right: 1.5rem; /* Allow full width to accommodate the theme button better */
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding-top: env(safe-area-inset-top);
  z-index: 1012;
}

.drawer-header .brand-text {
  font-size: 1.25rem;
}

.mobile-theme {
  border-radius: 999px;
  width: auto;
  padding: 0.5rem 1rem;
  font-size: 0.85rem;
  font-weight: 600;
  gap: 0.5rem;
}

.drawer-content {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.mobile-link {
  padding: 1rem;
  color: var(--text-main);
  text-decoration: none;
  font-size: 1.1rem;
  font-weight: 700;
  border-radius: 12px;
  transition: all 0.2s ease;
  background: transparent;
  border: none;
  cursor: pointer;
}

.mobile-link:hover,
.mobile-link.router-link-active {
  background: var(--bg-main);
  color: var(--primary);
}

.mobile-divider {
  height: 1px;
  background: var(--border-light);
  margin: 1rem 0;
}

.mobile-section-title {
  font-size: 0.75rem;
  text-transform: uppercase;
  letter-spacing: 0.1em;
  font-weight: 800;
  color: var(--text-muted);
  margin: 0.5rem 0 0.5rem 1rem;
}

.mobile-user-card {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1rem;
  background: var(--bg-main);
  border-radius: 16px;
  margin-bottom: 0.5rem;
}

.user-info .user-name {
  font-weight: 800;
  font-size: 1rem;
  margin: 0 0 0.25rem 0;
}

.user-info .user-role {
  font-size: 0.8rem;
  color: var(--primary);
  margin: 0;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.w-full {
  width: 100%;
}

.text-left {
  text-align: left;
}

/* Transitions */
.slide-right-enter-active,
.slide-right-leave-active {
  transition: opacity 0.3s ease;
}

.slide-right-enter-active .mobile-nav-drawer,
.slide-right-leave-active .mobile-nav-drawer {
  transition: transform 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.slide-right-enter-from,
.slide-right-leave-to {
  opacity: 0;
}

.slide-right-enter-from .mobile-nav-drawer,
.slide-right-leave-to .mobile-nav-drawer {
  transform: translateX(100%);
}

.fade-down-enter-active,
.fade-down-leave-active {
  transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
}

.fade-down-enter-from,
.fade-down-leave-to {
  opacity: 0;
  transform: translateY(-10px) scale(0.95);
}
</style>
