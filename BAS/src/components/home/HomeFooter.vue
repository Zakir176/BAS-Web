<template>
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
              <a href="https://facebook.com" class="social-icon" aria-label="Facebook">
                <span class="icon">📘</span>
              </a>
              <a href="https://twitter.com" class="social-icon" aria-label="Twitter">
                <span class="icon">🐦</span>
              </a>
              <a href="https://linkedin.com" class="social-icon" aria-label="LinkedIn">
                <span class="icon">💼</span>
              </a>
              <a href="https://instagram.com" class="social-icon" aria-label="Instagram">
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
            <router-link to="/privacy-policy">Privacy</router-link>
            <span class="separator">•</span>
            <router-link to="/terms-of-service">Terms</router-link>
            <span class="separator">•</span>
            <a href="mailto:support@university.edu">Cookies</a>
            <span class="separator">•</span>
            <a href="mailto:support@university.edu">Accessibility</a>
          </div>
          <div class="accreditation">
            <span class="accreditation-badge">✓</span>
            <span>Accredited University System</span>
          </div>
        </div>
      </div>
    </div>
  </footer>
</template>

<script setup>
import Skeleton from '@/components/ui/Skeleton.vue'

defineProps({
  isLoading: { type: Boolean, default: true },
  stats: { type: Object, required: true }
})
</script>

<style scoped>
.university-footer {
  background: var(--bg-card);
  padding: 5rem 0 2rem;
  border-top: 1px solid var(--border-light);
  color: var(--text-main);
}

.footer-content {
  display: grid;
  grid-template-columns: 1.5fr repeat(4, 1fr);
  gap: 3rem;
  margin-bottom: 4rem;
}

.footer-brand .brand-logo {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 1.5rem;
}

.footer-logo-img {
  width: 2.5rem;
  height: 2.5rem;
  border-radius: 8px;
}

.brand-text {
  font-size: 1.5rem;
  font-weight: 900;
  letter-spacing: -0.02em;
  color: var(--primary);
}

.brand-description {
  color: var(--text-muted);
  font-size: 0.95rem;
  line-height: 1.6;
  margin-bottom: 2rem;
}

.brand-stats {
  display: flex;
  gap: 2rem;
}

.mini-stat {
  display: flex;
  flex-direction: column;
}

.mini-stat .number {
  font-size: 1.25rem;
  font-weight: 800;
}

.mini-stat .label {
  font-size: 0.75rem;
  color: var(--text-muted);
  text-transform: uppercase;
  font-weight: 700;
}

.footer-title {
  font-size: 1.1rem;
  font-weight: 800;
  margin-bottom: 1.5rem;
}

.footer-links {
  list-style: none;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.footer-links a {
  color: var(--text-muted);
  text-decoration: none;
  font-size: 0.95rem;
  transition: color 0.2s;
  font-weight: 500;
}

.footer-links a:hover {
  color: var(--primary);
}

.university-info {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  margin-bottom: 2rem;
}

.info-item {
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
  font-size: 0.9rem;
  color: var(--text-muted);
}

.social-title {
  font-size: 0.9rem;
  font-weight: 800;
  margin-bottom: 1rem;
}

.social-icons {
  display: flex;
  gap: 1rem;
}

.social-icon {
  width: 2.5rem;
  height: 2.5rem;
  background: var(--bg-main);
  border: 1px solid var(--border-light);
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  text-decoration: none;
  transition: all 0.2s;
}

.social-icon:hover {
  border-color: var(--primary);
  transform: translateY(-3px);
  box-shadow: var(--shadow-soft);
}

.footer-bottom {
  padding-top: 2rem;
  border-top: 1px solid var(--border-light);
}

.footer-bottom-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 2rem;
}

.copyright p {
  font-size: 0.9rem;
  color: var(--text-muted);
}

.version {
  font-size: 0.75rem;
  font-weight: 700;
  color: var(--text-muted);
  opacity: 0.6;
}

.footer-bottom-links {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.footer-bottom-links a {
  color: var(--text-muted);
  text-decoration: none;
  font-size: 0.85rem;
  font-weight: 600;
}

.separator {
  color: var(--border-medium);
  font-size: 0.8rem;
}

.accreditation {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.8rem;
  font-weight: 700;
  color: var(--success);
}

.accreditation-badge {
  width: 1.25rem;
  height: 1.25rem;
  background: var(--success-soft);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.7rem;
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 1.5rem;
}

@media (max-width: 1024px) {
  .footer-content {
    grid-template-columns: 1fr 1fr;
    gap: 3rem;
  }
}

@media (max-width: 640px) {
  .footer-content {
    grid-template-columns: 1fr;
  }
  .footer-bottom-content {
    flex-direction: column;
    text-align: center;
  }
}
</style>
