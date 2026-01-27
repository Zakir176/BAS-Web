<template>
  <div class="university-homepage">
    <Navbar />
    
    <main>
      <!-- University Hero Section -->
      <section class="university-hero">
        <div class="hero-background">
          <div class="hero-pattern"></div>
        </div>
        <div class="container">
          <div class="hero-content">
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
              <div class="stat-item">
                <span class="stat-number">2,000+</span>
                <span class="stat-label">Active Students</span>
              </div>
              <div class="stat-item">
                <span class="stat-number">150+</span>
                <span class="stat-label">University Courses</span>
              </div>
              <div class="stat-item">
                <span class="stat-number">99.9%</span>
                <span class="stat-label">Accuracy Rate</span>
              </div>
            </div>
            <div class="hero-actions">
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
              <div class="secondary-actions">
                <router-link to="/student-signup" class="action-link">
                  <span class="icon">➕</span>
                  New Student? Register Here
                </router-link>
                <router-link to="/lecturer-signup" class="action-link">
                  <span class="icon">📚</span>
                  Lecturer Registration
                </router-link>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- Features Section -->
      <section class="features-section">
        <div class="container">
          <div class="section-header">
            <h2 class="section-title">Why Choose CAT?</h2>
            <p class="section-subtitle">Built specifically for modern university class management</p>
          </div>
          
          <div class="features-grid">
            <div class="feature-card">
              <div class="feature-icon barcode-icon">
                <span class="icon">📱</span>
              </div>
              <h3>Barcode Scanning</h3>
              <p>Quick and efficient attendance tracking using student ID barcodes. No more manual roll calls.</p>
            </div>
            
            <div class="feature-card">
              <div class="feature-icon real-time-icon">
                <span class="icon">⚡</span>
              </div>
              <h3>Real-time Updates</h3>
              <p>Instant attendance recording and reporting. Students and lecturers see updates immediately.</p>
            </div>
            
            <div class="feature-card">
              <div class="feature-icon analytics-icon">
                <span class="icon">📊</span>
              </div>
              <h3>Advanced Analytics</h3>
              <p>Comprehensive attendance reports and insights for better academic performance tracking.</p>
            </div>
            
            <div class="feature-card">
              <div class="feature-icon security-icon">
                <span class="icon">🔒</span>
              </div>
              <h3>Secure & Private</h3>
              <p>Enterprise-grade security with role-based access control and data privacy protection.</p>
            </div>
            
            <div class="feature-card">
              <div class="feature-icon mobile-icon">
                <span class="icon">📲</span>
              </div>
              <h3>Mobile Friendly</h3>
              <p>Works seamlessly on all devices. Students can check attendance from anywhere.</p>
            </div>
            
            <div class="feature-card">
              <div class="feature-icon integration-icon">
                <span class="icon">🔗</span>
              </div>
              <h3>Easy Integration</h3>
              <p>Integrates with existing university systems and student information databases.</p>
            </div>
          </div>
        </div>
      </section>

      <!-- How It Works Section -->
      <section class="how-it-works">
        <div class="container">
          <div class="section-header">
            <h2 class="section-title">How It Works</h2>
            <p class="section-subtitle">Simple 3-step process for seamless attendance tracking</p>
          </div>
          
          <div class="steps-container">
            <div class="step-card">
              <div class="step-number">1</div>
              <div class="step-content">
                <h3>Register & Login</h3>
                <p>Students and lecturers create accounts with their university credentials</p>
              </div>
            </div>
            
            <div class="step-connector">
              <div class="connector-line"></div>
            </div>
            
            <div class="step-card">
              <div class="step-number">2</div>
              <div class="step-content">
                <h3>Scan Barcode</h3>
                <p>Lecturers scan student ID barcodes or students check in digitally</p>
              </div>
            </div>
            
            <div class="step-connector">
              <div class="connector-line"></div>
            </div>
            
            <div class="step-card">
              <div class="step-number">3</div>
              <div class="step-content">
                <h3>Track & Report</h3>
                <p>View real-time attendance data and generate comprehensive reports</p>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- Benefits Section -->
      <section class="benefits-section">
        <div class="container">
          <div class="benefits-content">
            <div class="benefits-text">
              <h2 class="benefits-title">Benefits for Your University</h2>
              <div class="benefits-list">
                <div class="benefit-item">
                  <span class="check-icon">✓</span>
                  <span>Save up to 80% of class time on attendance tracking</span>
                </div>
                <div class="benefit-item">
                  <span class="check-icon">✓</span>
                  <span>Reduce attendance errors by 99.9%</span>
                </div>
                <div class="benefit-item">
                  <span class="check-icon">✓</span>
                  <span>Improve student accountability and engagement</span>
                </div>
                <div class="benefit-item">
                  <span class="check-icon">✓</span>
                  <span>Generate instant reports for administration</span>
                </div>
                <div class="benefit-item">
                  <span class="check-icon">✓</span>
                  <span>Comply with accreditation requirements</span>
                </div>
              </div>
              <div class="benefits-cta">
                <Button variant="primary" size="lg" @click="goToStudentLogin">
                  Get Started Today
                </Button>
              </div>
            </div>
            <div class="benefits-visual">
              <div class="dashboard-preview">
                <div class="preview-header">
                  <div class="preview-dots">
                    <span class="dot"></span>
                    <span class="dot"></span>
                    <span class="dot"></span>
                  </div>
                  <span class="preview-title">Attendance Dashboard</span>
                </div>
                <div class="preview-content">
                  <div class="preview-stats">
                    <div class="preview-stat">
                      <div class="stat-bar success"></div>
                      <span class="stat-text">Present: 85%</span>
                    </div>
                    <div class="preview-stat">
                      <div class="stat-bar warning"></div>
                      <span class="stat-text">Absent: 15%</span>
                    </div>
                  </div>
                  <div class="preview-chart">
                    <div class="chart-bar" style="height: 60%"></div>
                    <div class="chart-bar" style="height: 80%"></div>
                    <div class="chart-bar" style="height: 75%"></div>
                    <div class="chart-bar" style="height: 90%"></div>
                    <div class="chart-bar" style="height: 85%"></div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- CTA Section -->
      <section class="cta-section">
        <div class="container">
          <div class="cta-content">
            <h2 class="cta-title">Ready to Transform Your University's Class Attendance?</h2>
            <p class="cta-description">
              Join thousands of students and lecturers already using CAT for better class management and attendance tracking.
            </p>
            <div class="cta-actions">
              <Button variant="primary" size="lg" @click="goToStudentLogin" class="cta-primary">
                Start as Student
              </Button>
              <Button variant="secondary" size="lg" @click="goToLecturerLogin" class="cta-secondary">
                Start as Lecturer
              </Button>
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
                  <span class="number">2,000+</span>
                  <span class="label">Students</span>
                </div>
                <div class="mini-stat">
                  <span class="number">150+</span>
                  <span class="label">Courses</span>
                </div>
                <div class="mini-stat">
                  <span class="number">99.9%</span>
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
              <li><a href="#help">Help Center</a></li>
              <li><a href="#contact">Contact Support</a></li>
              <li><a href="#documentation">Documentation</a></li>
              <li><a href="#privacy">Privacy Policy</a></li>
              <li><a href="#terms">Terms of Service</a></li>
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
import { useRouter } from 'vue-router'
import Navbar from '@/components/layout/Navbar.vue'
import Button from '@/components/ui/Button.vue'

const router = useRouter()

const goToStudentLogin = () => router.push('/student-login')
const goToLecturerLogin = () => router.push('/lecturer-login')
</script>

<style scoped>
.university-homepage {
  background-color: var(--bg-main);
  min-height: 100vh;
}

/* University Hero Section */
.university-hero {
  position: relative;
  min-height: 100vh;
  display: flex;
  align-items: center;
  overflow: hidden;
}

.hero-background {
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, var(--primary) 0%, var(--primary-hover) 100%);
  opacity: 0.1;
}

.hero-pattern {
  position: absolute;
  inset: 0;
  background-image: 
    radial-gradient(circle at 20% 80%, var(--primary) 0%, transparent 50%),
    radial-gradient(circle at 80% 20%, var(--primary-hover) 0%, transparent 50%);
  opacity: 0.3;
}

.hero-content {
  position: relative;
  z-index: 2;
  text-align: center;
  max-width: 800px;
  margin: 0 auto;
  padding: 2rem;
}

.university-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  background: var(--bg-card);
  padding: 0.5rem 1rem;
  border-radius: 2rem;
  font-size: 0.875rem;
  font-weight: 600;
  color: var(--text-main);
  box-shadow: var(--shadow-soft);
  margin-bottom: 2rem;
}

.badge-logo {
  width: 1.5rem;
  height: 1.5rem;
  object-fit: cover;
  border-radius: 0.25rem;
}

.hero-title {
  font-size: clamp(2.5rem, 5vw, 4rem);
  font-weight: 900;
  line-height: 1.1;
  color: var(--text-main);
  margin-bottom: 1.5rem;
}

.highlight {
  background: linear-gradient(135deg, var(--primary), var(--primary-hover));
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.hero-description {
  font-size: 1.25rem;
  color: var(--text-muted);
  line-height: 1.6;
  margin-bottom: 3rem;
  max-width: 600px;
  margin-left: auto;
  margin-right: auto;
}

.hero-stats {
  display: flex;
  justify-content: center;
  gap: 3rem;
  margin-bottom: 3rem;
  flex-wrap: wrap;
}

.stat-item {
  text-align: center;
}

.stat-number {
  display: block;
  font-size: 2.5rem;
  font-weight: 900;
  color: var(--primary);
  margin-bottom: 0.5rem;
}

.stat-label {
  font-size: 0.875rem;
  color: var(--text-muted);
  font-weight: 600;
}

.hero-actions {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
  align-items: center;
}

.primary-actions {
  display: flex;
  gap: 1rem;
  flex-wrap: wrap;
  justify-content: center;
}

.student-btn, .lecturer-btn {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  min-width: 180px;
}

.secondary-actions {
  display: flex;
  gap: 2rem;
  flex-wrap: wrap;
  justify-content: center;
}

.action-link {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: var(--text-muted);
  text-decoration: none;
  font-weight: 600;
  transition: color 0.2s ease;
}

.action-link:hover {
  color: var(--primary);
}

/* Features Section */
.features-section {
  padding: 6rem 0;
  background: var(--bg-card);
}

.section-header {
  text-align: center;
  margin-bottom: 4rem;
}

.section-title {
  font-size: 2.5rem;
  font-weight: 800;
  color: var(--text-main);
  margin-bottom: 1rem;
}

.section-subtitle {
  font-size: 1.125rem;
  color: var(--text-muted);
  max-width: 600px;
  margin: 0 auto;
}

.features-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 2rem;
}

.feature-card {
  background: var(--bg-main);
  padding: 2rem;
  border-radius: 1rem;
  text-align: center;
  transition: transform 0.2s ease, box-shadow 0.2s ease;
  border: 1px solid var(--border-light);
}

.feature-card:hover {
  transform: translateY(-4px);
  box-shadow: var(--shadow-card);
}

.feature-icon {
  width: 4rem;
  height: 4rem;
  border-radius: 1rem;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 1.5rem;
  font-size: 1.5rem;
}

.barcode-icon {
  background: linear-gradient(135deg, #3b82f6, #2563eb);
}

.real-time-icon {
  background: linear-gradient(135deg, #10b981, #059669);
}

.analytics-icon {
  background: linear-gradient(135deg, #8b5cf6, #7c3aed);
}

.security-icon {
  background: linear-gradient(135deg, #ef4444, #dc2626);
}

.mobile-icon {
  background: linear-gradient(135deg, #f59e0b, #d97706);
}

.integration-icon {
  background: linear-gradient(135deg, #06b6d4, #0891b2);
}

.feature-card h3 {
  font-size: 1.25rem;
  font-weight: 700;
  color: var(--text-main);
  margin-bottom: 1rem;
}

.feature-card p {
  color: var(--text-muted);
  line-height: 1.6;
}

/* How It Works Section */
.how-it-works {
  padding: 6rem 0;
  background: var(--bg-main);
}

.steps-container {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 2rem;
  max-width: 1000px;
  margin: 0 auto;
  flex-wrap: wrap;
}

.step-card {
  background: var(--bg-card);
  padding: 2rem;
  border-radius: 1rem;
  text-align: center;
  flex: 1;
  min-width: 250px;
  border: 1px solid var(--border-light);
  position: relative;
}

.step-number {
  width: 3rem;
  height: 3rem;
  background: var(--primary);
  color: white;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  font-size: 1.25rem;
  margin: 0 auto 1.5rem;
}

.step-card h3 {
  font-size: 1.25rem;
  font-weight: 700;
  color: var(--text-main);
  margin-bottom: 1rem;
}

.step-card p {
  color: var(--text-muted);
  line-height: 1.6;
}

.step-connector {
  display: flex;
  align-items: center;
  justify-content: center;
  flex: 0 0 auto;
}

.connector-line {
  width: 3rem;
  height: 2px;
  background: var(--border-medium);
}

/* Benefits Section */
.benefits-section {
  padding: 6rem 0;
  background: var(--bg-card);
}

.benefits-content {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 4rem;
  align-items: center;
}

.benefits-title {
  font-size: 2.5rem;
  font-weight: 800;
  color: var(--text-main);
  margin-bottom: 2rem;
}

.benefits-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  margin-bottom: 2rem;
}

.benefit-item {
  display: flex;
  align-items: center;
  gap: 1rem;
  color: var(--text-main);
  font-weight: 600;
}

.check-icon {
  color: var(--success);
  font-weight: 700;
  font-size: 1.125rem;
}

.benefits-visual {
  display: flex;
  justify-content: center;
}

.dashboard-preview {
  background: var(--bg-main);
  border-radius: 1rem;
  overflow: hidden;
  box-shadow: var(--shadow-card);
  max-width: 400px;
  width: 100%;
}

.preview-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1rem;
  border-bottom: 1px solid var(--border-light);
}

.preview-dots {
  display: flex;
  gap: 0.5rem;
}

.dot {
  width: 0.75rem;
  height: 0.75rem;
  border-radius: 50%;
  background: var(--border-medium);
}

.preview-title {
  font-size: 0.875rem;
  color: var(--text-muted);
  font-weight: 600;
}

.preview-content {
  padding: 1.5rem;
}

.preview-stats {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  margin-bottom: 2rem;
}

.preview-stat {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.stat-bar {
  width: 3rem;
  height: 0.5rem;
  border-radius: 0.25rem;
}

.stat-bar.success {
  background: var(--success);
}

.stat-bar.warning {
  background: var(--warning);
}

.stat-text {
  font-size: 0.875rem;
  color: var(--text-main);
  font-weight: 600;
}

.preview-chart {
  display: flex;
  align-items: end;
  gap: 0.5rem;
  height: 4rem;
}

.chart-bar {
  flex: 1;
  background: var(--primary);
  border-radius: 0.25rem 0.25rem 0 0;
  min-height: 0.5rem;
}

/* CTA Section */
.cta-section {
  padding: 6rem 0;
  background: linear-gradient(135deg, var(--primary), var(--primary-hover));
  color: white;
  text-align: center;
}

.cta-content {
  max-width: 600px;
  margin: 0 auto;
  padding: 0 2rem;
}

.cta-title {
  font-size: 2.5rem;
  font-weight: 800;
  margin-bottom: 1.5rem;
}

.cta-description {
  font-size: 1.25rem;
  margin-bottom: 3rem;
  opacity: 0.9;
}

.cta-actions {
  display: flex;
  gap: 1rem;
  justify-content: center;
  flex-wrap: wrap;
}

.cta-primary {
  background: white;
  color: var(--primary);
}

.cta-primary:hover {
  background: var(--bg-main);
}

.cta-secondary {
  background: rgba(255, 255, 255, 0.1);
  color: white;
  border: 1px solid rgba(255, 255, 255, 0.3);
}

.cta-secondary:hover {
  background: rgba(255, 255, 255, 0.2);
}

/* Responsive Design */
@media (max-width: 768px) {
  .hero-stats {
    gap: 2rem;
  }
  
  .primary-actions {
    flex-direction: column;
    align-items: center;
  }
  
  .student-btn, .lecturer-btn {
    width: 100%;
    max-width: 300px;
  }
  
  .secondary-actions {
    flex-direction: column;
    align-items: center;
    gap: 1rem;
  }
  
  .steps-container {
    flex-direction: column;
  }
  
  .step-connector {
    transform: rotate(90deg);
    margin: 1rem 0;
  }
  
  .benefits-content {
    grid-template-columns: 1fr;
    gap: 2rem;
  }
  
  .features-grid {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 640px) {
  .hero-title {
    font-size: 2rem;
  }
  
  .hero-description {
    font-size: 1rem;
  }
  
  .stat-number {
    font-size: 2rem;
  }
  
  .section-title {
    font-size: 2rem;
  }
  
  .cta-title {
    font-size: 1.75rem;
  }
  
  .cta-description {
    font-size: 1rem;
  }
}

/* University Footer */
.university-footer {
  background: var(--bg-card);
  border-top: 1px solid var(--border-light);
  padding: 3rem 0 1rem;
  margin-top: auto;
}

.footer-content {
  display: grid;
  grid-template-columns: 2fr 1fr 1fr 1fr 1.5fr;
  gap: 2rem;
  margin-bottom: 2rem;
}

.footer-section {
  display: flex;
  flex-direction: column;
}

.footer-brand {
  margin-bottom: 1.5rem;
}

.brand-logo {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 1rem;
}

.footer-logo-img {
  width: 2rem;
  height: 2rem;
  object-fit: cover;
  border-radius: 0.25rem;
}

.brand-text {
  font-size: 1.25rem;
  font-weight: 700;
  color: var(--text-main);
}

.brand-description {
  color: var(--text-muted);
  line-height: 1.6;
  margin-bottom: 1.5rem;
  max-width: 300px;
}

.brand-stats {
  display: flex;
  gap: 1.5rem;
}

.mini-stat {
  text-align: center;
}

.mini-stat .number {
  display: block;
  font-size: 1.25rem;
  font-weight: 700;
  color: var(--primary);
  margin-bottom: 0.25rem;
}

.mini-stat .label {
  font-size: 0.75rem;
  color: var(--text-muted);
  font-weight: 600;
}

.footer-title {
  font-size: 1rem;
  font-weight: 700;
  color: var(--text-main);
  margin-bottom: 1rem;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.footer-links {
  list-style: none;
  padding: 0;
  margin: 0;
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.footer-links a {
  color: var(--text-muted);
  text-decoration: none;
  font-size: 0.875rem;
  transition: color 0.2s ease;
  font-weight: 500;
}

.footer-links a:hover {
  color: var(--primary);
}

.university-info {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  margin-bottom: 1.5rem;
}

.info-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  color: var(--text-muted);
  font-size: 0.875rem;
}

.info-item .icon {
  font-size: 1rem;
  color: var(--primary);
}

.social-links {
  margin-top: auto;
}

.social-title {
  font-size: 0.875rem;
  font-weight: 600;
  color: var(--text-main);
  margin-bottom: 0.75rem;
}

.social-icons {
  display: flex;
  gap: 0.75rem;
}

.social-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 2.5rem;
  height: 2.5rem;
  background: var(--bg-main);
  border: 1px solid var(--border-light);
  border-radius: 0.5rem;
  color: var(--text-muted);
  text-decoration: none;
  transition: all 0.2s ease;
}

.social-icon:hover {
  background: var(--primary);
  color: white;
  border-color: var(--primary);
}

.social-icon .icon {
  font-size: 1.125rem;
}

.footer-bottom {
  border-top: 1px solid var(--border-light);
  padding-top: 1.5rem;
}

.footer-bottom-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 1rem;
}

.copyright p {
  color: var(--text-muted);
  font-size: 0.875rem;
  margin: 0;
}

.version {
  font-size: 0.75rem;
  opacity: 0.7;
}

.footer-bottom-links {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.875rem;
}

.footer-bottom-links a {
  color: var(--text-muted);
  text-decoration: none;
  transition: color 0.2s ease;
}

.footer-bottom-links a:hover {
  color: var(--primary);
}

.separator {
  color: var(--border-medium);
}

.accreditation {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: var(--success);
  font-size: 0.875rem;
  font-weight: 600;
}

.accreditation-badge {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 1.25rem;
  height: 1.25rem;
  background: var(--success);
  color: white;
  border-radius: 50%;
  font-size: 0.75rem;
  font-weight: 700;
}

/* Responsive Footer */
@media (max-width: 1024px) {
  .footer-content {
    grid-template-columns: 1fr 1fr;
    gap: 2rem;
  }
  
  .footer-section:nth-child(1) {
    grid-column: 1 / -1;
  }
}

@media (max-width: 768px) {
  .university-footer {
    padding: 2rem 0 1rem;
  }
  
  .footer-content {
    grid-template-columns: 1fr;
    gap: 2rem;
  }
  
  .brand-stats {
    justify-content: center;
  }
  
  .footer-bottom-content {
    flex-direction: column;
    text-align: center;
    gap: 1rem;
  }
  
  .footer-bottom-links {
    justify-content: center;
  }
}

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
