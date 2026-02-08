<template>
  <section class="university-hero">
    <div class="hero-background" :style="{ transform: `translateY(${scrollY * 0.1}px)` }">
      <div class="hero-pattern" :style="{ transform: `translateY(${scrollY * 0.2}px)` }"></div>
    </div>
    <div class="container">
      <header class="hero-content" :style="{ transform: `translateY(${scrollY * 0.05}px)` }">
        <div class="university-badge">
          <img src="@/assets/logo.jpeg" alt="CAT Logo" class="badge-logo" />
          <span>CAT - Class Attendance Tracker</span>
        </div>
        <h1 class="hero-title">
          Revolutionize <span class="highlight">Class Attendance</span>
          <br>with Smart Technology
        </h1>
        <p class="hero-description">
          Transform how your university tracks class attendance. Our CAT system makes 
          attendance tracking effortless, accurate, and secure for both students and lecturers.
        </p>
        
        <div class="hero-stats">
          <article class="stat-item">
            <Skeleton v-if="isLoading" width="120px" height="3rem" />
            <span v-else class="stat-number">{{ stats.students || '0' }}</span>
            <span class="stat-label">Active Students</span>
          </article>
          <article class="stat-item">
            <Skeleton v-if="isLoading" width="120px" height="3rem" />
            <span v-else class="stat-number">{{ stats.courses || '0' }}</span>
            <span class="stat-label">University Courses</span>
          </article>
          <article class="stat-item">
            <Skeleton v-if="isLoading" width="120px" height="3rem" />
            <span v-else class="stat-number">{{ stats.accuracy }}</span>
            <span class="stat-label">Accuracy Rate</span>
          </article>
        </div>

        <div class="hero-actions">
          <template v-if="isAuthenticated">
            <div class="authenticated-action">
              <Button variant="primary" size="lg" @click="$emit('dashboard')" class="dashboard-btn">
                <span class="icon">🚀</span>
                Enter Your Dashboard
              </Button>
              <p class="welcome-back">Welcome back! Access your classes and records instantly.</p>
            </div>
          </template>
          <template v-else>
            <div class="primary-actions">
              <Button variant="primary" size="lg" @click="$emit('student-login')" class="student-btn">
                <span class="icon">👤</span>
                Student Portal
              </Button>
              <Button variant="secondary" size="lg" @click="$emit('lecturer-login')" class="lecturer-btn">
                <span class="icon">👨‍🏫</span>
                Lecturer Portal
              </Button>
            </div>
            <nav class="secondary-actions">
              <router-link to="/student-signup" class="action-link">
                <span class="icon">➕</span>
                New Student? Register Here
              </router-link>
              <router-link to="/lecturer-signup" class="action-link">
                <span class="icon">📚</span>
                Lecturer Registration
              </router-link>
            </nav>
          </template>
        </div>
      </header>
    </div>
  </section>
</template>

<script setup>
import Button from '@/components/ui/Button.vue'
import Skeleton from '@/components/ui/Skeleton.vue'

defineProps({
  scrollY: { type: Number, default: 0 },
  isLoading: { type: Boolean, default: true },
  stats: { type: Object, required: true },
  isAuthenticated: { type: Boolean, default: false }
})

defineEmits(['dashboard', 'student-login', 'lecturer-login'])
</script>

<style scoped>
.university-hero {
  position: relative;
  min-height: 90vh;
  display: flex;
  align-items: center;
  overflow: hidden;
  padding: 4rem 0;
}

.hero-background {
  position: absolute;
  inset: 0;
  background: radial-gradient(circle at 10% 20%, var(--primary-soft) 0%, transparent 40%),
              radial-gradient(circle at 90% 80%, var(--primary-soft) 0%, transparent 40%);
  z-index: 1;
}

.hero-pattern {
  position: absolute;
  inset: 0;
  background-image: radial-gradient(var(--border-medium) 1px, transparent 1px);
  background-size: 40px 40px;
  opacity: 0.2;
}

.hero-content {
  position: relative;
  z-index: 2;
  text-align: center;
  max-width: 900px;
  margin: 0 auto;
}

.university-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.75rem;
  background: var(--bg-card);
  padding: 0.6rem 1.25rem;
  border-radius: 100px;
  border: 1px solid var(--border-light);
  font-size: 0.9rem;
  font-weight: 700;
  color: var(--primary);
  box-shadow: var(--shadow-soft);
  margin-bottom: 2.5rem;
  animation: float 4s ease-in-out infinite;
  backdrop-filter: blur(10px);
}

@keyframes float {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-10px); }
}

.badge-logo {
  width: 1.25rem;
  height: 1.25rem;
  border-radius: 4px;
}

.hero-title {
  font-size: clamp(2.5rem, 6vw, 4.5rem);
  font-weight: 900;
  line-height: 1.05;
  margin-bottom: 2rem;
  letter-spacing: -0.03em;
}

.highlight {
  color: var(--primary);
  position: relative;
}

.hero-description {
  font-size: 1.25rem;
  color: var(--text-muted);
  line-height: 1.6;
  max-width: 650px;
  margin: 0 auto 3.5rem;
}

.hero-stats {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 2rem;
  margin-bottom: 4rem;
  max-width: 700px;
  margin-left: auto;
  margin-right: auto;
}

.stat-item {
  display: flex;
  flex-direction: column;
}

.stat-number {
  font-size: 2.5rem;
  font-weight: 900;
  color: var(--text-main);
  display: block;
}

.stat-label {
  font-size: 0.85rem;
  font-weight: 600;
  color: var(--text-muted);
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.hero-actions {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1.5rem;
}

.authenticated-action {
  background: var(--bg-card);
  backdrop-filter: blur(16px);
  padding: 1.5rem 2.5rem;
  border-radius: 20px;
  border: 1px solid rgba(255, 255, 255, 0.1);
  box-shadow: var(--shadow-card);
}

.welcome-back {
  margin-top: 1rem;
  font-size: 0.9rem;
  color: var(--text-muted);
  font-weight: 500;
}

.primary-actions {
  display: flex;
  gap: 1.25rem;
  flex-wrap: wrap;
  justify-content: center;
}

.secondary-actions {
  display: flex;
  gap: 2.5rem;
  margin-top: 1rem;
}

.action-link {
  color: var(--text-muted);
  text-decoration: none;
  font-weight: 600;
  font-size: 0.95rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  transition: all 0.2s;
}

.action-link:hover {
  color: var(--primary);
}

@media (max-width: 768px) {
  .hero-stats {
    grid-template-columns: 1fr;
    gap: 1.5rem;
  }
}
</style>
