<template>
  <div class="home-page">
    <Navbar />
    
    <main class="main-content">
      <div class="container">
        <section class="hero-section">
          <div class="hero-content">
            <div class="hero-text">
              <h1 class="hero-title">
                Barcode Attendance System
              </h1>
              <p class="hero-subtitle">
                Seamless attendance tracking for modern educational institutions
              </p>
              <div class="hero-actions">
                <Button variant="primary" size="lg" @click="goToStudentLogin">
                  Student Portal
                </Button>
                <Button variant="secondary" size="lg" @click="goToLecturerLogin">
                  Lecturer Portal
                </Button>
              </div>
              <div class="hero-links">
                <router-link to="/student-signup" class="signup-link">
                  New Student? Sign up
                </router-link>
                <router-link to="/lecturer-signup" class="signup-link">
                  New Lecturer? Sign up
                </router-link>
              </div>
            </div>
            <div class="hero-visual">
              <div class="barcode-animation">
                <div class="barcode-lines">
                  <div class="line"></div>
                  <div class="line"></div>
                  <div class="line"></div>
                  <div class="line"></div>
                  <div class="line"></div>
                </div>
                <div class="scanner"></div>
              </div>
            </div>
          </div>
        </section>

        <section class="features-section">
          <div class="features-grid">
            <Card class="feature-card">
              <div class="feature-icon">
                <svg width="48" height="48" viewBox="0 0 48 48" fill="none">
                  <rect width="48" height="48" rx="12" fill="var(--accent-primary)" opacity="0.1"/>
                  <path d="M16 8h16v4H16V8zm0 8h16v4H16v-4zm0 8h16v4H16v-4zm0 8h16v4H16v-4z" fill="var(--accent-primary)"/>
                </svg>
              </div>
              <h3 class="feature-title">Quick Scanning</h3>
              <p class="feature-description">
                Fast barcode scanning for instant attendance recording
              </p>
            </Card>

            <Card class="feature-card">
              <div class="feature-icon">
                <svg width="48" height="48" viewBox="0 0 48 48" fill="none">
                  <rect width="48" height="48" rx="12" fill="var(--success)" opacity="0.1"/>
                  <path d="M24 8l8 8-8 8-8-8 8-8zm0 16l8 8-8 8-8-8 8-8z" fill="var(--success)"/>
                </svg>
              </div>
              <h3 class="feature-title">Real-time Tracking</h3>
              <p class="feature-description">
                Live attendance monitoring and instant updates
              </p>
            </Card>

            <Card class="feature-card">
              <div class="feature-icon">
                <svg width="48" height="48" viewBox="0 0 48 48" fill="none">
                  <rect width="48" height="48" rx="12" fill="var(--warning)" opacity="0.1"/>
                  <path d="M12 12h24v4H12v-4zm0 8h24v4H12v-4zm0 8h24v4H12v-4z" fill="var(--warning)"/>
                </svg>
              </div>
              <h3 class="feature-title">Detailed Reports</h3>
              <p class="feature-description">
                Comprehensive attendance analytics and reporting
              </p>
            </Card>
          </div>
        </section>

        <HowItWorks />

        <section class="stats-section">
          <div class="stats-grid">
            <div class="stat-item">
              <div class="stat-number">10K+</div>
              <div class="stat-label">Students Enrolled</div>
            </div>
            <div class="stat-item">
              <div class="stat-number">500+</div>
              <div class="stat-label">Lecturers</div>
            </div>
            <div class="stat-item">
              <div class="stat-number">99.9%</div>
              <div class="stat-label">Uptime</div>
            </div>
            <div class="stat-item">
              <div class="stat-number">24/7</div>
              <div class="stat-label">Support</div>
            </div>
          </div>
        </section>

        <Cta />

        <Testimonials />
      </div>
    </main>

    <footer class="footer">
      <div class="container">
        <div class="footer-content">
          <div class="footer-brand">
            <div class="logo">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="none">
                <rect width="24" height="24" rx="6" fill="var(--accent-primary)"/>
                <path d="M6 9h12v6H6z" fill="white"/>
                <path d="M9 6v12h6V6H9z" fill="var(--accent-primary)"/>
              </svg>
            </div>
            <span>BAS</span>
          </div>
          <div class="footer-text">
            © 2024 Barcode Attendance System. All rights reserved.
          </div>
          <div class="footer-links">
            <a href="#" class="footer-link">About Us</a>
            <a href="#" class="footer-link">Contact Us</a>
            <a href="#" class="footer-link">Terms of Service</a>
          </div>
          <div class="footer-socials">
            <a href="#" class="social-link">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor">
                <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8zm-1-5h2v-2h-2v2zm0-4h2V7h-2v4z"/>
              </svg>
            </a>
            <a href="#" class="social-link">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor">
                <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8zm-1-5h2v-2h-2v2zm0-4h2V7h-2v4z"/>
              </svg>
            </a>
            <a href="#" class="social-link">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor">
                <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8zm-1-5h2v-2h-2v2zm0-4h2V7h-2v4z"/>
              </svg>
            </a>
          </div>
        </div>
      </div>
    </footer>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'
import Navbar from '@/components/layout/Navbar.vue'
import Button from '@/components/ui/Button.vue'
import HowItWorks from '@/components/layout/HowItWorks.vue'
import Testimonials from '@/components/layout/Testimonials.vue'
import Cta from '@/components/layout/Cta.vue'
import Card from '@/components/ui/Card.vue'

const router = useRouter()

const goToStudentLogin = () => {
  router.push('/student-login')
}

const goToLecturerLogin = () => {
  router.push('/lecturer-login')
}
</script>

<style scoped>
.home-page {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.main-content {
  flex: 1;
}

.hero-section {
  padding: 4rem 0;
  text-align: center;
}

.hero-content {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 4rem;
  align-items: center;
  max-width: 1200px;
  margin: 0 auto;
}

.hero-text {
  text-align: left;
}

.hero-title {
  font-size: 3.5rem;
  font-weight: 700;
  color: var(--text-primary);
  margin-bottom: 1.5rem;
  line-height: 1.1;
}

.hero-subtitle {
  font-size: 1.25rem;
  color: var(--text-secondary);
  margin-bottom: 2.5rem;
  line-height: 1.6;
}

.hero-actions {
  display: flex;
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.hero-links {
  display: flex;
  gap: 1rem;
  justify-content: center;
}

.signup-link {
  color: var(--accent-primary);
  text-decoration: none;
  font-size: 0.875rem;
  font-weight: 500;
  transition: color 0.2s ease;
}

.signup-link:hover {
  color: var(--accent-secondary);
  text-decoration: underline;
}

.hero-visual {
  display: flex;
  justify-content: center;
  align-items: center;
}

.barcode-animation {
  position: relative;
  width: 200px;
  height: 200px;
}

.barcode-lines {
  display: flex;
  gap: 8px;
  align-items: center;
  justify-content: center;
  height: 100%;
}

.line {
  width: 8px;
  height: 120px;
  background: linear-gradient(to bottom, var(--accent-primary), var(--accent-secondary));
  border-radius: 4px;
  animation: pulse 2s ease-in-out infinite;
}

.line:nth-child(2) { animation-delay: 0.2s; }
.line:nth-child(3) { animation-delay: 0.4s; }
.line:nth-child(4) { animation-delay: 0.6s; }
.line:nth-child(5) { animation-delay: 0.8s; }

.scanner {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 4px;
  background: linear-gradient(90deg, transparent, var(--accent-primary), transparent);
  animation: scan 3s ease-in-out infinite;
  border-radius: 2px;
}

@keyframes pulse {
  0%, 100% { opacity: 0.6; transform: scaleY(1); }
  50% { opacity: 1; transform: scaleY(1.1); }
}

@keyframes scan {
  0% { top: 0; }
  50% { top: calc(100% - 4px); }
  100% { top: 0; }
}

.features-section {
  padding: 4rem 0;
}

.features-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 2rem;
}

.feature-card {
  text-align: center;
  padding: 2rem;
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.feature-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.1);
}

.feature-icon {
  margin-bottom: 1.5rem;
  display: flex;
  justify-content: center;
}

.feature-title {
  font-size: 1.5rem;
  font-weight: 600;
  color: var(--text-primary);
  margin-bottom: 1rem;
}

.feature-description {
  color: var(--text-secondary);
  line-height: 1.6;
}

.stats-section {
  padding: 4rem 0;
  background-color: var(--bg-secondary);
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 2rem;
  text-align: center;
}

.stat-item {
  padding: 1rem;
}

.stat-number {
  font-size: 2.5rem;
  font-weight: 700;
  color: var(--accent-primary);
  margin-bottom: 0.5rem;
}

.stat-label {
  color: var(--text-secondary);
  font-size: 1rem;
}

.footer {
  background-color: var(--bg-tertiary);
  padding: 2rem 0;
  border-top: 1px solid var(--border-primary);
}

.footer-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.footer-brand {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  font-weight: 600;
  color: var(--text-primary);
}

.footer-text {
  color: var(--text-secondary);
  font-size: 0.875rem;
}

.footer-links {
  display: flex;
  gap: 1.5rem;
}

.footer-link {
  color: var(--text-secondary);
  text-decoration: none;
  font-size: 0.875rem;
  font-weight: 500;
  transition: color 0.2s ease;
}

.footer-link:hover {
  color: var(--accent-primary);
  text-decoration: underline;
}

.footer-socials {
  display: flex;
  gap: 1rem;
}

.social-link {
  color: var(--text-secondary);
  transition: color 0.2s ease;
}

.social-link:hover {
  color: var(--accent-primary);
}

@media (max-width: 768px) {
  .hero-content {
    grid-template-columns: 1fr;
    gap: 2rem;
    text-align: center;
  }
  
  .hero-text {
    text-align: center;
  }
  
  .hero-title {
    font-size: 2.5rem;
  }
  
  .hero-actions {
    flex-direction: column;
    align-items: center;
  }
  
  .features-grid {
    grid-template-columns: 1fr;
  }
  
  .stats-grid {
    grid-template-columns: repeat(2, 1fr);
  }
  
  .footer-content {
    flex-direction: column;
    gap: 1rem;
    text-align: center;
  }
}
</style>
