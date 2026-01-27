<template>
  <div class="landing-page">
    <Navbar />
    
    <main>
      <!-- Hero Section -->
      <section class="hero-v2">
        <div class="hero-bg-overlay"></div>
        <div class="container relative">
          <div class="hero-flex">
            <div class="hero-left">
              <span class="badge-new">NEW VERSION 2.0</span>
              <h1 class="display-title">Simplify Campus <br/><span class="text-gradient">Attendance.</span></h1>
              <p class="lead-text">
                The most advanced barcode-based tracking system for modern universities. Secure, fast, and remarkably easy to use.
              </p>
              <div class="hero-cta-group">
                <Button variant="primary" size="lg" @click="goToStudentLogin" class="premium-btn">
                  Start Scanning
                </Button>
                <div class="hero-links">
                  <router-link to="/student-signup" class="signup-link">Student Signup</router-link>
                  <router-link to="/lecturer-signup" class="signup-link">Lecturer Signup</router-link>
                </div>
                <div class="avatars-group">
                  <div class="avatar-stack">
                    <img src="https://i.pravatar.cc/100?u=1" alt="U">
                    <img src="https://i.pravatar.cc/100?u=2" alt="U">
                    <img src="https://i.pravatar.cc/100?u=3" alt="U">
                  </div>
                  <span class="avatar-text">Join 2,000+ students</span>
                </div>
              </div>
            </div>
            <div class="hero-right">
              <div class="hero-mockup-floating">
                <div class="mockup-card">
                  <div class="mockup-header">
                    <div class="dot red"></div>
                    <div class="dot yellow"></div>
                    <div class="dot green"></div>
                  </div>
                  <div class="mockup-body">
                    <div class="barcode-skeleton"></div>
                    <div class="text-skeleton-group">
                      <div class="text-skeleton long"></div>
                      <div class="text-skeleton short"></div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- Trusted By Section -->
      <section class="trust-section">
        <div class="container">
          <p class="trust-label">TRUSTED BY LEADING DEPARTMENTS</p>
          <div class="logo-cloud">
            <span class="dept-logo">BIOLOGY</span>
            <span class="dept-logo">ENGINEERING</span>
            <span class="dept-logo">MATHEMATICS</span>
            <span class="dept-logo">COMPUTING</span>
          </div>
        </div>
      </section>

      <!-- Features Grid -->
      <section class="features-v2">
        <div class="container">
          <div class="section-intro">
            <h2>Everything you need for <br/><span>perfect records.</span></h2>
          </div>
          <div class="features-grid-v2">
            <div class="feature-card-v2">
              <div class="feat-icon-box">⚡</div>
              <h3>Instant Scan</h3>
              <p>Detect barcodes in milliseconds with sub-pixel precision under any lighting.</p>
            </div>
            <div class="feature-card-v2">
              <div class="feat-icon-box">📊</div>
              <h3>Live Dashboards</h3>
              <p>Visualize attendance trends with real-time analytics for every course.</p>
            </div>
            <div class="feature-card-v2">
              <div class="feat-icon-box">🛡️</div>
              <h3>Secure Access</h3>
              <p>Enterprise-grade security ensuring student data remains private and protected.</p>
            </div>
          </div>
        </div>
      </section>

      <!-- CTA Section -->
      <section class="final-cta">
        <div class="container">
          <div class="cta-inner">
            <h2>Ready to transform your classroom?</h2>
            <p>Get started with Smart Attendance today and save hours every week.</p>
            <Button variant="white" size="lg" @click="goToLecturerLogin">Get Started Now</Button>
          </div>
        </div>
      </section>
    </main>

    <footer class="footer-v2">
      <div class="container">
        <div class="footer-top">
          <div class="footer-brand">
            <span class="brand-text">Smart Attendance</span>
            <p>Modernizing academic administration.</p>
          </div>
          <div class="footer-nav">
            <div class="nav-col">
              <h4>System</h4>
              <router-link to="/student-login">Student Portal</router-link>
              <router-link to="/lecturer-login">Lecturer Portal</router-link>
            </div>
            <div class="nav-col">
              <h4>Company</h4>
              <a href="#">About</a>
              <a href="#">Privacy</a>
            </div>
          </div>
        </div>
        <div class="footer-bottom">
          <p>© 2024 Smart Attendance System. Built for higher education.</p>
        </div>
      </div>
    </footer>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'
import Navbar from '@/components/layout/Navbar.vue'
import Button from '@/components/ui/Button.vue'

const router = useRouter()
const goToStudentLogin = () => router.push('/student-login')
const goToLecturerLogin = () => router.push('/lecturer-login')
</script>

<style scoped>
.landing-page {
  background-color: var(--bg-main);
}

/* Hero V2 */
.hero-v2 {
  position: relative;
  padding: 8rem 0 10rem;
  background-color: var(--bg-main);
  overflow: hidden;
}

.hero-v2::before {
  content: '';
  position: absolute;
  top: -10%;
  right: -5%;
  width: 600px;
  height: 600px;
  background: radial-gradient(circle, rgba(59, 130, 246, 0.1) 0%, transparent 70%);
  z-index: 1;
}

.hero-flex {
  display: grid;
  grid-template-columns: 1.1fr 0.9fr;
  align-items: center;
  gap: 4rem;
  z-index: 2;
  position: relative;
}

/* Mobile responsive - single column on small screens */
@media (max-width: 768px) {
  .hero-flex {
    grid-template-columns: 1fr;
    gap: 2rem;
    text-align: center;
  }
  
  .display-title {
    font-size: 2.5rem;
  }
  
  .hero-cta-group {
    flex-direction: column;
    gap: 1rem;
  }
  
  .hero-links {
    flex-direction: column;
    gap: 0.5rem;
  }
}

.badge-new {
  display: inline-block;
  padding: 6px 12px;
  background: #dbeafe;
  color: #2563eb;
  border-radius: 100px;
  font-size: 0.75rem;
  font-weight: 800;
  letter-spacing: 0.05em;
  margin-bottom: 2rem;
}

.display-title {
  font-size: 4.5rem;
  font-weight: 900;
  line-height: 1.05;
  color: var(--text-main);
  margin-bottom: 1.5rem;
}

.text-gradient {
  background: linear-gradient(to right, #2563eb, #7c3aed);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}

.lead-text {
  font-size: 1.25rem;
  color: var(--text-muted);
  margin-bottom: 3rem;
  max-width: 580px;
  line-height: 1.6;
}

.hero-cta-group {
  display: flex;
  align-items: center;
  gap: 2.5rem;
}

.premium-btn {
  box-shadow: 0 10px 25px rgba(37, 99, 235, 0.25);
}

.avatars-group {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.avatar-stack {
  display: flex;
  margin-left: 0.5rem;
}

.avatar-stack img {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  border: 2px solid white;
  margin-left: -10px;
}

.avatar-text {
  font-size: 0.875rem;
  font-weight: 700;
  color: var(--text-main);
}

/* Floating Mockup */
.hero-mockup-floating {
  animation: float 6s ease-in-out infinite;
}

@keyframes float {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-20px); }
}

.mockup-card {
  background: white;
  border-radius: 24px;
  padding: 1.5rem;
  box-shadow: 0 40px 80px rgba(0,0,0,0.1);
  border: 1px solid #e2e8f0;
  width: 400px;
}

.mockup-header {
  display: flex;
  gap: 6px;
  margin-bottom: 2rem;
}

.dot { width: 10px; height: 10px; border-radius: 50%; }
.red { background: #ff5f56; }
.yellow { background: #ffbd2e; }
.green { background: #27c93f; }

.barcode-skeleton {
  height: 120px;
  background: repeating-linear-gradient(90deg, #f1f5f9, #f1f5f9 2px, transparent 2px, transparent 8px);
  margin-bottom: 1.5rem;
  border-radius: 4px;
}

.text-skeleton {
  height: 12px;
  background: #f1f5f9;
  border-radius: 6px;
  margin-bottom: 10px;
}

.text-skeleton.long { width: 80%; }
.text-skeleton.short { width: 50%; }

/* Trust */
.trust-section {
  padding: 4rem 0;
  border-bottom: 1px solid #f1f5f9;
  text-align: center;
}

.trust-label {
  font-size: 0.75rem;
  font-weight: 800;
  color: #94a3b8;
  letter-spacing: 0.1em;
  margin-bottom: 2rem;
}

.logo-cloud {
  display: flex;
  justify-content: center;
  gap: 4rem;
  opacity: 0.6;
}

.dept-logo {
  font-weight: 900;
  font-size: 1.25rem;
  color: #cbd5e1;
}

/* Features V2 */
.features-v2 {
  padding: 10rem 0;
}

.section-intro {
  text-align: center;
  margin-bottom: 6rem;
}

.section-intro h2 {
  font-size: 3rem;
  font-weight: 900;
  color: #0f172a;
}

.section-intro span { color: #2563eb; }

.features-grid-v2 {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 3rem;
}

.feature-card-v2 {
  padding: 3rem;
  background: #f8fafc;
  border-radius: 24px;
  transition: all 0.3s;
}

.feature-card-v2:hover {
  background: white;
  box-shadow: 0 20px 40px rgba(0,0,0,0.05);
  transform: translateY(-5px);
}

.feat-icon-box {
  width: 64px;
  height: 64px;
  background: white;
  border-radius: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.75rem;
  margin-bottom: 2rem;
  box-shadow: 0 10px 20px rgba(0,0,0,0.04);
}

.feature-card-v2 h3 {
  font-size: 1.5rem;
  font-weight: 800;
  margin-bottom: 1rem;
}

.feature-card-v2 p {
  color: #64748b;
  line-height: 1.6;
}

/* Final CTA */
.final-cta {
  padding-bottom: 10rem;
}

.cta-inner {
  background: linear-gradient(135deg, #2563eb, #7c3aed);
  padding: 6rem;
  border-radius: 40px;
  text-align: center;
  color: white;
}

.cta-inner h2 {
  font-size: 3rem;
  font-weight: 900;
  margin-bottom: 1.5rem;
}

.hero-links {
  display: flex;
  gap: 1rem;
  margin: 1rem 0;
  justify-content: center;
}

.signup-link {
  color: var(--primary);
  text-decoration: none;
  font-size: 0.9rem;
  padding: 0.5rem 1rem;
  border: 1px solid var(--primary);
  border-radius: 0.5rem;
  transition: all 0.3s ease;
}

.signup-link:hover {
  background: var(--primary);
  color: white;
  transform: translateY(-2px);
}

.cta-inner p {
  font-size: 1.25rem;
  margin-bottom: 3rem;
  opacity: 0.9;
}

/* Footer V2 */
.footer-v2 {
  padding: 8rem 0 4rem;
  background: #0f172a;
  color: white;
}

.footer-top {
  display: flex;
  justify-content: space-between;
  margin-bottom: 6rem;
}

.footer-brand .brand-text {
  font-size: 1.5rem;
  font-weight: 900;
  margin-bottom: 1rem;
  display: block;
}

.footer-brand p { color: #94a3b8; }

.footer-nav {
  display: flex;
  gap: 6rem;
}

.nav-col h4 {
  font-size: 0.75rem;
  font-weight: 900;
  color: #475569;
  text-transform: uppercase;
  margin-bottom: 1.5rem;
}

.nav-col a {
  display: block;
  color: #94a3b8;
  text-decoration: none;
  margin-bottom: 1rem;
  font-weight: 600;
  transition: color 0.2s;
}

.nav-col a:hover { color: white; }

.footer-bottom {
  padding-top: 4rem;
  border-top: 1px solid #1e293b;
  color: #475569;
  font-size: 0.875rem;
  font-weight: 600;
}

@media (max-width: 1024px) {
  .hero-flex { grid-template-columns: 1fr; text-align: center; }
  .hero-left { display: flex; flex-direction: column; align-items: center; }
  .display-title { font-size: 3rem; }
  .hero-right { display: none; }
  .features-grid-v2 { grid-template-columns: 1fr; }
  .cta-inner { padding: 4rem 2rem; }
}
</style>
