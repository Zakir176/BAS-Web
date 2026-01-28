<template>
  <div class="university-homepage">
    <Navbar />
    
    <main>
      <!-- University Hero Section -->
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
                  <Button variant="primary" size="lg" @click="goToDashboard" class="dashboard-btn">
                    <span class="icon">🚀</span>
                    Enter Your Dashboard
                  </Button>
                  <p class="welcome-back">Welcome back! Access your classes and records instantly.</p>
                </div>
              </template>
              <template v-else>
                <div class="primary-actions">
                  <Button variant="primary" size="lg" @click="goToStudentLogin" class="student-btn">
                    <span class="icon">👤</span>
                    Student Portal
                  </Button>
                  <Button variant="secondary" size="lg" @click="goToLecturerLogin" class="lecturer-btn">
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

      <!-- Platform Overview (Merged Features & Benefits) -->
      <section id="features" class="platform-overview reveal-on-scroll">
        <div class="container">
          <header class="section-header">
            <h2 class="section-title">One Platform, Infinite Accuracy</h2>
            <p class="section-subtitle">A professional attendance ecosystem built for modern academia.</p>
          </header>
          
          <div class="overview-grid">
            <div class="benefit-showcase">
              <ul class="benefits-list">
                <li class="benefit-item">
                  <span class="check-icon">✓</span>
                  <div>
                    <strong>80% Time Reduction</strong>
                    <p>Automate roll calls and save valuable lecture time.</p>
                  </div>
                </li>
                <li class="benefit-item">
                  <span class="check-icon">✓</span>
                  <div>
                    <strong>Instant Reporting</strong>
                    <p>Generate CSV and PDF reports for administration in one click.</p>
                  </div>
                </li>
                <li class="benefit-item">
                  <span class="check-icon">✓</span>
                  <div>
                    <strong>Failsafe Security</strong>
                    <p>Role-based access ensuring data privacy and integrity.</p>
                  </div>
                </li>
              </ul>
            </div>

            <div class="features-mini-grid">
              <template v-if="isLoading">
                <article v-for="i in 4" :key="i" class="feature-mini-card">
                  <Skeleton width="40px" height="40px" shape="circle" style="margin-bottom: 1rem" />
                  <Skeleton width="60%" height="1.5rem" style="margin-bottom: 0.5rem" />
                  <Skeleton width="100%" height="3rem" />
                </article>
              </template>
              <template v-else>
                <article class="feature-mini-card">
                  <span class="icon">📱</span>
                  <h3>Barcode Entry</h3>
                  <p>Lightning fast scanning using student ID cards.</p>
                </article>
                <article class="feature-mini-card">
                  <span class="icon">⚡</span>
                  <h3>Live Sync</h3>
                  <p>Attendance updates across all devices in real-time.</p>
                </article>
                <article class="feature-mini-card">
                  <span class="icon">📊</span>
                  <h3>Analytics</h3>
                  <p>Track student performance through attendance trends.</p>
                </article>
                <article class="feature-mini-card">
                  <span class="icon">🔗</span>
                  <h3>Integration</h3>
                  <p>Easy data export for university records.</p>
                </article>
              </template>
            </div>
          </div>
        </div>
      </section>

      <!-- 3-Step Workflow -->
      <section class="workflow-steps reveal-on-scroll">
        <div class="container">
          <header class="section-header">
            <h2 class="section-title">Simple Implementation</h2>
          </header>
          <div class="steps-wrapper">
            <div class="step-line"></div>
            <div class="steps-items">
              <article class="step">
                <div class="step-num">01</div>
                <h3>Identify</h3>
                <p>Log in with your secure credentials</p>
              </article>
              <article class="step">
                <div class="step-num">02</div>
                <h3>Verify</h3>
                <p>Scan barcode or mark presence</p>
              </article>
              <article class="step">
                <div class="step-num">03</div>
                <h3>Analyze</h3>
                <p>Review and finalize class reports</p>
              </article>
            </div>
          </div>
        </div>
      </section>

      <section class="final-cta reveal-on-scroll">
        <div class="container">
          <div class="cta-inner">
            <h2 class="cta-title">Ready to modernize your classroom?</h2>
            <div class="cta-btns" v-if="!isAuthenticated">
              <Button variant="primary" size="lg" @click="goToStudentSignup">Get Started as Student</Button>
              <Button variant="secondary" size="lg" @click="goToLecturerSignup">Get Started as Lecturer</Button>
            </div>
            <div class="cta-btns" v-else>
              <Button variant="primary" size="lg" @click="goToDashboard">Return to Dashboard</Button>
            </div>
          </div>
        </div>
      </section>
    </main>
    
    <!-- University Footer -->
    <footer class="university-footer">
      <div class="container">
        <div class="footer-content">
          <div class="footer-section">
            <div class="footer-brand">
              <div class="brand-logo">
                <img src="@/assets/logo.jpeg" alt="CAT Logo" class="footer-logo-img" />
                <span class="brand-text">CAT</span>
              </div>
              <p class="brand-description">
                Class Attendance Tracker - Advanced barcode-based attendance system for modern universities. 
                Secure, efficient, and reliable class attendance management.
              </p>
              <div class="brand-stats">
                <div class="mini-stat">
                  <Skeleton v-if="isLoading" width="40px" height="1.5rem" />
                  <span v-else class="number">{{ stats.students || '0' }}</span>
                  <span class="label">Students</span>
                </div>
                <div class="mini-stat">
                  <Skeleton v-if="isLoading" width="40px" height="1.5rem" />
                  <span v-else class="number">{{ stats.courses || '0' }}</span>
                  <span class="label">Courses</span>
                </div>
                <div class="mini-stat">
                  <Skeleton v-if="isLoading" width="40px" height="1.5rem" />
                  <span v-else class="number">{{ stats.accuracy }}</span>
                  <span class="label">Accuracy</span>
                </div>
              </div>
            </div>
          </div>
          
          <div class="footer-section">
            <h3 class="footer-title">Quick Access</h3>
            <ul class="footer-links">
              <li><router-link to="/student-login">Student Portal</router-link></li>
              <li><router-link to="/lecturer-login">Lecturer Portal</router-link></li>
              <li><router-link to="/student-signup">Student Registration</router-link></li>
              <li><router-link to="/lecturer-signup">Lecturer Registration</router-link></li>
              <li><router-link to="/report-page">Attendance Reports</router-link></li>
            </ul>
          </div>
          
          <div class="footer-section">
            <h3 class="footer-title">Features</h3>
            <ul class="footer-links">
              <li><a href="#features">Barcode Scanning</a></li>
              <li><a href="#features">Real-time Updates</a></li>
              <li><a href="#features">Advanced Analytics</a></li>
              <li><a href="#features">Mobile Access</a></li>
              <li><a href="#features">System Integration</a></li>
            </ul>
          </div>
          
          <div class="footer-section">
            <h3 class="footer-title">Support</h3>
            <ul class="footer-links">
              <li><router-link to="/student-signup">Join as Student</router-link></li>
              <li><router-link to="/lecturer-signup">Join as Lecturer</router-link></li>
              <li><router-link to="/privacy-policy">Privacy Policy</router-link></li>
              <li><router-link to="/terms-of-service">Terms of Service</router-link></li>
            </ul>
          </div>
          
          <div class="footer-section">
            <h3 class="footer-title">University Info</h3>
            <div class="university-info">
              <div class="info-item">
                <span class="icon">📍</span>
                <span>University Campus System</span>
              </div>
              <div class="info-item">
                <span class="icon">📧</span>
                <span>support@university.edu</span>
              </div>
              <div class="info-item">
                <span class="icon">📞</span>
                <span>+1 (555) 123-4567</span>
              </div>
              <div class="info-item">
                <span class="icon">🕐</span>
                <span>24/7 System Support</span>
              </div>
            </div>
            
            <div class="social-links">
              <h4 class="social-title">Connect With Us</h4>
              <div class="social-icons">
                <a href="#" class="social-icon" aria-label="Facebook">
                  <span class="icon">📘</span>
                </a>
                <a href="#" class="social-icon" aria-label="Twitter">
                  <span class="icon">🐦</span>
                </a>
                <a href="#" class="social-icon" aria-label="LinkedIn">
                  <span class="icon">💼</span>
                </a>
                <a href="#" class="social-icon" aria-label="Instagram">
                  <span class="icon">📷</span>
                </a>
              </div>
            </div>
          </div>
        </div>
        
        <div class="footer-bottom">
          <div class="footer-bottom-content">
            <div class="copyright">
              <p>&copy; 2026 CAT - Class Attendance Tracker. All rights reserved.</p>
              <p class="version">Version 1 | University Edition</p>
            </div>
            <div class="footer-bottom-links">
              <a href="#privacy">Privacy</a>
              <span class="separator">•</span>
              <a href="#terms">Terms</a>
              <span class="separator">•</span>
              <a href="#cookies">Cookies</a>
              <span class="separator">•</span>
              <a href="#accessibility">Accessibility</a>
            </div>
            <div class="accreditation">
              <span class="accreditation-badge">✓</span>
              <span>Accredited University System</span>
            </div>
          </div>
        </div>
      </div>
    </footer>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { supabase } from '@/supabase'
import { useAuth } from '@/composables/useAuth'
import Navbar from '@/components/layout/Navbar.vue'
import Button from '@/components/ui/Button.vue'
import Skeleton from '@/components/ui/Skeleton.vue'

const router = useRouter()
const { isAuthenticated, user, role } = useAuth()

const isLoading = ref(true)
const scrollY = ref(0)
const handleScroll = () => {
  scrollY.value = window.scrollY
}

const stats = ref({
  students: 0,
  courses: 0,
  sessions: 0,
  accuracy: '99.9%'
})

const fetchStats = async () => {
  try {
    const [
      { count: studentCount },
      { count: courseCount },
      { count: sessionCount }
    ] = await Promise.all([
      supabase.from('students').select('*', { count: 'exact', head: true }),
      supabase.from('courses').select('*', { count: 'exact', head: true }),
      supabase.from('sessions').select('*', { count: 'exact', head: true })
    ])

    stats.value.students = studentCount || 0
    stats.value.courses = courseCount || 0
    stats.value.sessions = sessionCount || 0
  } catch (err) {
    console.error('Error fetching global stats:', err)
  } finally {
    isLoading.value = false
  }
}

const goToDashboard = () => {
  const userRole = role.value || user.value?.user_metadata?.role || 'student'
  if (userRole === 'lecturer' || userRole === 'teacher') {
    router.push('/lecturer-dashboard')
  } else {
    router.push('/student-homepage')
  }
}

const goToStudentLogin = () => router.push('/student-login')
const goToLecturerLogin = () => router.push('/lecturer-login')
const goToStudentSignup = () => router.push('/student-signup')
const goToLecturerSignup = () => router.push('/lecturer-signup')

onMounted(() => {
  fetchStats()
  window.addEventListener('scroll', handleScroll, { passive: true })
  
  // Intersection Observer for scroll-reveal animations
  const observer = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
      if (entry.isIntersecting) {
        entry.target.classList.add('is-visible')
      }
    })
  }, { 
    threshold: 0.15,
    rootMargin: '0px 0px -50px 0px'
  })

  document.querySelectorAll('.reveal-on-scroll').forEach(el => observer.observe(el))
})

onUnmounted(() => {
  window.removeEventListener('scroll', handleScroll)
})
</script>

<style scoped>
.university-homepage {
  background-color: var(--bg-main);
  min-height: 100vh;
  color: var(--text-main);
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 1.5rem;
}

/* University Hero Section */
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

/* Hero Stats */
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

/* Hero Actions */
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

/* Platform Overview */
.platform-overview {
  padding: 8rem 0;
  background: var(--bg-card);
  border-top: 1px solid var(--border-light);
}

.section-header {
  text-align: center;
  margin-bottom: 5rem;
}

.section-title {
  font-size: 2.75rem;
  font-weight: 850;
  letter-spacing: -0.02em;
  margin-bottom: 1rem;
}

.section-subtitle {
  font-size: 1.2rem;
  color: var(--text-muted);
}

.overview-grid {
  display: grid;
  grid-template-columns: 1fr 1.2fr;
  gap: 5rem;
  align-items: center;
}

.benefits-list {
  list-style: none;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: 2.5rem;
}

.benefit-item {
  display: flex;
  gap: 1.5rem;
}

.benefit-item .check-icon {
  width: 2.5rem;
  height: 2.5rem;
  background: var(--success-soft);
  color: var(--success);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  font-weight: 900;
}

.benefit-item div strong {
  display: block;
  font-size: 1.2rem;
  margin-bottom: 0.25rem;
}

.benefit-item div p {
  color: var(--text-muted);
  line-height: 1.5;
}

.features-mini-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1.5rem;
}

.feature-mini-card {
  background: rgba(255, 255, 255, 0.03);
  backdrop-filter: blur(12px);
  padding: 2rem;
  border-radius: 24px;
  border: 1px solid rgba(255, 255, 255, 0.05);
  transition: all 0.4s cubic-bezier(0.23, 1, 0.32, 1);
  opacity: 0; /* Managed by slideIn animation */
}

.feature-mini-card:hover {
  transform: translateY(-8px) scale(1.02);
  background: rgba(255, 255, 255, 0.06);
  border-color: var(--primary-soft);
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.2);
}

.feature-mini-card .icon {
  font-size: 1.75rem;
  margin-bottom: 1rem;
  display: block;
}

.feature-mini-card h3 {
  font-size: 1.1rem;
  font-weight: 700;
  margin-bottom: 0.5rem;
}

.feature-mini-card p {
  font-size: 0.9rem;
  color: var(--text-muted);
  line-height: 1.5;
}

/* Workflow Steps */
.workflow-steps {
  padding: 8rem 0;
}

.steps-wrapper {
  position: relative;
  max-width: 1000px;
  margin: 0 auto;
}

.step-line {
  position: absolute;
  top: 3.5rem;
  left: 10%;
  right: 10%;
  height: 2px;
  background: var(--border-medium);
  z-index: 1;
}

.steps-items {
  position: relative;
  z-index: 2;
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 2rem;
}

.step {
  text-align: center;
}

.step-num {
  width: 7rem;
  height: 7rem;
  background: var(--bg-card);
  border: 2px solid var(--border-light);
  border-radius: 50%;
  margin: 0 auto 1.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.75rem;
  font-weight: 900;
  color: var(--primary);
  box-shadow: var(--shadow-soft);
}

.step h3 {
  font-size: 1.25rem;
  font-weight: 800;
  margin-bottom: 0.5rem;
}

.step p {
  color: var(--text-muted);
  font-size: 1rem;
}

/* Final CTA */
.final-cta {
  padding: 6rem 0;
  margin-bottom: 4rem;
}

.cta-inner {
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.05) 0%, rgba(255, 255, 255, 0.01) 100%);
  backdrop-filter: blur(20px);
  padding: 5rem 3rem;
  border-radius: 48px;
  border: 1px solid rgba(255, 255, 255, 0.1);
  text-align: center;
  box-shadow: var(--shadow-card);
}

.cta-title {
  font-size: 2.5rem;
  font-weight: 900;
  margin-bottom: 2.5rem;
  letter-spacing: -0.02em;
}

.cta-btns {
  display: flex;
  gap: 1.5rem;
  justify-content: center;
}

/* Responsive */
@media (max-width: 960px) {
  .overview-grid {
    grid-template-columns: 1fr;
    gap: 4rem;
  }
}

@media (max-width: 768px) {
  .hero-stats {
    grid-template-columns: 1fr;
    gap: 1.5rem;
  }
  
  .step-line {
    display: none;
  }
  
  .steps-items {
    grid-template-columns: 1fr;
    gap: 4rem;
  }
  
  .secondary-actions {
    flex-direction: column;
    align-items: center;
    gap: 1.25rem;
  }
  
  .cta-btns {
    flex-direction: column;
    align-items: stretch;
    max-width: 300px;
    margin: 0 auto;
  }
}

/* Footer (Kept and Refined) */
.university-footer {
  background: var(--bg-card);
  border-top: 1px solid var(--border-light);
  padding: 4rem 0 2rem;
}

.footer-content {
  display: grid;
  grid-template-columns: 2fr 1fr 1fr 1fr 1.5fr;
  gap: 3rem;
  margin-bottom: 4rem;
}

.brand-text {
  font-size: 1.5rem;
  font-weight: 900;
  letter-spacing: -0.03em;
}

.brand-description {
  color: var(--text-muted);
  font-size: 0.9rem;
  line-height: 1.6;
  margin: 1rem 0 1.5rem;
}

.footer-title {
  font-size: 0.8rem;
  font-weight: 800;
  color: var(--text-muted);
  text-transform: uppercase;
  margin-bottom: 1.5rem;
  letter-spacing: 0.1em;
}

.footer-links {
  list-style: none;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.footer-links a {
  color: var(--text-main);
  text-decoration: none;
  font-weight: 600;
  font-size: 0.875rem;
  transition: color 0.2s;
}

.footer-links a:hover {
  color: var(--primary);
}

.footer-bottom {
  border-top: 1px solid var(--border-light);
  padding-top: 2rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 0.8rem;
  color: var(--text-muted);
}

/* Scroll Reveal Animations */
.reveal-on-scroll {
  opacity: 0;
  transform: translateY(50px);
  transition: opacity 1.2s cubic-bezier(0.23, 1, 0.32, 1), 
              transform 1.2s cubic-bezier(0.23, 1, 0.32, 1);
  will-change: opacity, transform;
}

.reveal-on-scroll.is-visible {
  opacity: 1;
  transform: translateY(0);
}

/* Staggered entry for children of revealed sections */
.reveal-on-scroll.is-visible .benefit-item {
  animation: slideIn 0.8s cubic-bezier(0.23, 1, 0.32, 1) forwards;
}

.reveal-on-scroll.is-visible .feature-mini-card {
  animation: slideIn 0.8s cubic-bezier(0.23, 1, 0.32, 1) forwards;
}

@keyframes slideIn {
  from { opacity: 0; transform: translateY(20px); }
  to { opacity: 1; transform: translateY(0); }
}

/* Staggered Delays */
.benefit-item:nth-child(1) { animation-delay: 0.1s; }
.benefit-item:nth-child(2) { animation-delay: 0.2s; }
.benefit-item:nth-child(3) { animation-delay: 0.3s; }

.feature-mini-card:nth-child(1) { animation-delay: 0.2s; }
.feature-mini-card:nth-child(2) { animation-delay: 0.3s; }
.feature-mini-card:nth-child(3) { animation-delay: 0.4s; }
.feature-mini-card:nth-child(4) { animation-delay: 0.5s; }

@media (max-width: 640px) {
  .brand-stats {
    flex-direction: column;
    align-items: center;
    gap: 1rem;
  }
  
  .social-icons {
    justify-content: center;
  }
  
  .footer-bottom-links {
    flex-wrap: wrap;
    justify-content: center;
  }
}
</style>
