<template>
  <div class="content-page privacy-policy">
    <Navbar />
    <main class="container">
      <div class="content-wrapper">
        <header class="page-header">
          <h1 class="page-title">Privacy Policy</h1>
          <p class="last-updated">Last Updated: January 29, 2026</p>
        </header>

        <section class="content-section">
          <h2>1. Introduction</h2>
          <p>
            Welcome to the Class Attendance Tracker (CAT). We are committed to protecting your personal information and your right to privacy. If you have any questions or concerns about our policy, or our practices with regards to your personal information, please contact us.
          </p>
        </section>

        <section class="content-section">
          <h2>2. Information We Collect</h2>
          <p>
            We collect personal information that you voluntarily provide to us when you register on the portal, express an interest in obtaining information about us or our products and services, or otherwise when you contact us.
          </p>
          <ul>
            <li><strong>Personal Information:</strong> Name, email address, student/staff ID.</li>
            <li><strong>Attendance Data:</strong> Logs of class attendance, timestamps, and course associations.</li>
          </ul>
        </section>

        <section class="content-section">
          <h2>3. How We Use Your Information</h2>
          <p>
            We use personal information collected via our portal for a variety of business purposes described below. We process your personal information for these purposes in reliance on our legitimate business interests, in order to enter into or perform a contract with you, with your consent, and/or for compliance with our legal obligations.
          </p>
        </section>

        <section class="content-section">
          <h2>4. Data Security</h2>
          <p>
            We use administrative, technical, and physical security measures to help protect your personal information. While we have taken reasonable steps to secure the personal information you provide to us, please be aware that despite our efforts, no security measures are perfect or impenetrable.
          </p>
        </section>
      </div>
    </main>
    <!-- Simple Footer for legal pages -->
    <footer class="simple-footer">
      <div class="container">
        <p>&copy; 2026 CAT System. University Edition.</p>
        <router-link to="/">Back to Home</router-link>
      </div>
    </footer>
  </div>
</template>

<script setup>
import Navbar from '@/components/common/Navbar.vue'
</script>

<style scoped>
.content-page {
  min-height: 100vh;
  background-color: var(--bg-main);
  color: var(--text-main);
}

.container {
  max-width: 800px;
  margin: 0 auto;
  padding: 4rem 1.5rem;
}

.content-wrapper {
  background: var(--bg-card);
  padding: 3rem;
  border-radius: 24px;
  border: 1px solid var(--border-light);
  box-shadow: var(--shadow-soft);
}

.page-header {
  margin-bottom: 3rem;
  border-bottom: 1px solid var(--border-light);
  padding-bottom: 1.5rem;
}

.page-title {
  font-size: 2.5rem;
  font-weight: 800;
  margin-bottom: 0.5rem;
}

.last-updated {
  color: var(--text-muted);
  font-size: 0.9rem;
}

.content-section {
  margin-bottom: 2.5rem;
}

.content-section h2 {
  font-size: 1.5rem;
  font-weight: 700;
  margin-bottom: 1rem;
  color: var(--primary);
}

.content-section p {
  line-height: 1.6;
  margin-bottom: 1rem;
  color: var(--text-muted);
}

.content-section ul {
  padding-left: 1.5rem;
  margin-bottom: 1rem;
}

.content-section li {
  margin-bottom: 0.5rem;
  color: var(--text-muted);
}

.simple-footer {
  text-align: center;
  padding: 4rem 1.5rem;
  color: var(--text-muted);
  font-size: 0.9rem;
}

.simple-footer a {
  color: var(--primary);
  text-decoration: none;
  font-weight: 600;
  margin-top: 1rem;
  display: inline-block;
}

@media (max-width: 640px) {
  .content-wrapper {
    padding: 2rem 1.5rem;
  }
}
</style>
