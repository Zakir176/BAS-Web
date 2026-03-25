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
        </template>

        <!-- Global Actions (Theme & Profile) -->
        <div class="nav-global-actions">
          <button @click="handleToggleTheme" class="theme-toggle" aria-label="Toggle Theme">
            <span class="icon">{{ theme === 'dark' ? '☀️' : '🌙' }}</span>
          </button>
          
          <template v-if="isAuthenticated">
            <div class="user-profile-btn" @click.stop="toggleDropdown">
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
          </template>
        </div>
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

const toggleDropdown = () => {
  dropdownOpen.value = !dropdownOpen.value;
};

const closeDropdown = () => {
  dropdownOpen.value = false;
};

const handleToggleTheme = () => {
  toggleTheme();
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
  document.body.style.overflow = ''; 
});
</script>

<style scoped>
/* Floating Glass Pill Navbar */
.premium-navbar {
  position: fixed;
  top: 1rem;
  left: 50%;
  transform: translateX(-50%);
  width: 95%;
  max-width: 1200px;
  height: 64px;
  z-index: 1000;
  transition: all 0.4s cubic-bezier(0.16, 1, 0.3, 1);
  background: rgba(255, 255, 255, 0.7);
  backdrop-filter: blur(12px);
  -webkit-backdrop-filter: blur(12px);
  border: 1px solid rgba(255, 255, 255, 0.3);
  border-radius: 100px;
  box-shadow: 0 10px 30px -10px rgba(0, 0, 0, 0.1);
}

[data-theme='dark'] .premium-navbar {
  background: rgba(15, 23, 42, 0.7);
  border-color: rgba(255, 255, 255, 0.08);
  box-shadow: 0 10px 30px -10px rgba(0, 0, 0, 0.4);
}

.premium-navbar.scrolled {
  top: 0.5rem;
  width: 98%;
  background: rgba(255, 255, 255, 0.85);
  box-shadow: 0 20px 40px -15px rgba(0, 0, 0, 0.15);
}

[data-theme='dark'] .premium-navbar.scrolled {
  background: rgba(15, 23, 42, 0.85);
}

.nav-container {
  max-width: 100%;
  margin: 0 auto;
  padding: 0 1.25rem 0 1.5rem;
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
  z-index: 1001;
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
  font-size: 1.25rem;
  font-weight: 900;
  letter-spacing: -0.03em;
  color: var(--text-main);
}

/* Desktop Navigation */
.desktop-nav {
  display: none;
  align-items: center;
  gap: 2rem;
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

.nav-global-actions {
  display: flex;
  align-items: center;
  gap: 1.25rem;
  position: relative;
}

.nav-actions {
  display: flex;
  align-items: center;
  gap: 1rem;
}

/* Theme Toggle */
.theme-toggle {
  background: rgba(0, 0, 0, 0.05);
  border: 1px solid transparent;
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

[data-theme='dark'] .theme-toggle {
  background: rgba(255, 255, 255, 0.05);
}

.theme-toggle:hover {
  background: var(--primary);
  color: white;
  transform: translateY(-2px);
}

.theme-toggle .icon {
  font-size: 1rem;
  line-height: 1;
}

/* User Profile & Dropdown */
.user-profile-btn {
  display: flex;
  align-items: center;
  gap: 0.6rem;
  cursor: pointer;
  padding: 0.25rem 0.75rem 0.25rem 0.25rem;
  background: rgba(0, 0, 0, 0.04);
  border: 1px solid transparent;
  border-radius: 999px;
  transition: all 0.2s ease;
}

[data-theme='dark'] .user-profile-btn {
  background: rgba(255, 255, 255, 0.05);
}

.user-profile-btn:hover {
  background: rgba(0, 0, 0, 0.08);
}

[data-theme='dark'] .user-profile-btn:hover {
  background: rgba(255, 255, 255, 0.1);
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

.chevron { transition: transform 0.3s ease; }
.chevron.open { transform: rotate(180deg); }

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

.dropdown-header { padding: 1rem; }
.dropdown-name { font-weight: 700; color: var(--text-main); margin: 0 0 0.25rem 0; }
.dropdown-email { font-size: 0.8rem; color: var(--text-muted); margin: 0; }
.dropdown-divider { height: 1px; background: var(--border-light); margin: 0.5rem 0; }

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

.dropdown-item:hover { background: var(--bg-main); }

/* Mobile Utilities */
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
  z-index: 1015;
}

.mobile-toggle .bar {
  width: 100%;
  height: 2px;
  background-color: var(--text-main);
  border-radius: 2px;
  transition: all 0.3s ease;
}

.mobile-toggle.active .bar:nth-child(1) { transform: translateY(8px) rotate(45deg); }
.mobile-toggle.active .bar:nth-child(2) { opacity: 0; }
.mobile-toggle.active .bar:nth-child(3) { transform: translateY(-8px) rotate(-45deg); }

.mobile-nav-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.4);
  backdrop-filter: blur(4px);
  z-index: 1010;
}

.mobile-nav-drawer {
  position: absolute;
  top: 1rem;
  right: 1rem;
  bottom: 1rem;
  width: 280px;
  max-width: 85vw;
  background: rgba(255, 255, 255, 0.8);
  backdrop-filter: blur(20px);
  border: 1px solid rgba(255, 255, 255, 0.3);
  border-radius: 32px;
  box-shadow: -10px 20px 40px rgba(0, 0, 0, 0.1);
  display: flex;
  flex-direction: column;
  padding: 5rem 1rem 2rem;
  overflow-y: auto;
  z-index: 1011;
}

[data-theme='dark'] .mobile-nav-drawer {
  background: rgba(15, 23, 42, 0.8);
  border-color: rgba(255, 255, 255, 0.08);
}

.drawer-header {
  position: absolute;
  top: 1.5rem;
  left: 1.5rem;
  right: 1.5rem;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.mobile-link {
  padding: 1rem;
  color: var(--text-main);
  text-decoration: none;
  font-size: 1.1rem;
  font-weight: 700;
  border-radius: 12px;
  transition: all 0.2s ease;
}

.mobile-link:hover, .mobile-link.router-link-active {
  background: rgba(0,0,0,0.05);
  color: var(--primary);
}

[data-theme='dark'] .mobile-link:hover {
  background: rgba(255,255,255,0.05);
}

.mobile-divider { height: 1px; background: var(--border-light); margin: 1rem 0; }
.mobile-section-title { font-size: 0.75rem; text-transform: uppercase; font-weight: 800; color: var(--text-muted); margin: 0.5rem 0 0.5rem 1rem; }

.mobile-user-card {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1rem;
  background: rgba(0,0,0,0.03);
  border-radius: 16px;
  margin-bottom: 0.5rem;
}

[data-theme='dark'] .mobile-user-card {
  background: rgba(255,255,255,0.03);
}

.user-info .user-name { font-weight: 800; font-size: 1rem; margin: 0 0 0.25rem 0; }
.user-info .user-role { font-size: 0.8rem; color: var(--primary); font-weight: 600; text-transform: uppercase; }

/* Responsive Adjustments */
@media (max-width: 768px) {
  .desktop-nav { display: none; }
  .mobile-toggle { display: flex; }
  .premium-navbar { width: calc(100% - 1.5rem); height: 56px; top: 0.75rem; }
}

@media (min-width: 769px) {
  .desktop-nav { display: flex; }
  .mobile-toggle { display: none; }
}

/* Transitions */
.slide-right-enter-active, .slide-right-leave-active { transition: opacity 0.3s ease; }
.slide-right-enter-active .mobile-nav-drawer, .slide-right-leave-active .mobile-nav-drawer { transition: transform 0.3s cubic-bezier(0.4, 0, 0.2, 1); }
.slide-right-enter-from, .slide-right-leave-to { opacity: 0; }
.slide-right-enter-from .mobile-nav-drawer, .slide-right-leave-to .mobile-nav-drawer { transform: translateX(100%); }

.fade-down-enter-active, .fade-down-leave-active { transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1); }
.fade-down-enter-from, .fade-down-leave-to { opacity: 0; transform: translateY(-10px) scale(0.95); }
</style>
