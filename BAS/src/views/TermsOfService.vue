<template>
  <div class="content-page terms-of-service">
    <Navbar />
    <main class="container">
      <div class="content-wrapper">
        <header class="page-header">
          <h1 class="page-title">Terms of Service</h1>
          <p class="last-updated">Last Updated: January 29, 2026</p>
        </header>

        <section class="content-section">
          <h2>1. Agreement to Terms</h2>
          <p>
            By accessing or using the Class Attendance Tracker (CAT), you agree to be bound by these Terms of Service. If you do not agree to all of these terms, you are prohibited from using the system.
          </p>
        </section>

        <section class="content-section">
          <h2>2. Academic Integrity</h2>
          <p>
            The CAT system is designed to support honest and accurate attendance records. Any attempt to circumvent the system (e.g., scanning barcodes for students who are not present) constitutes a violation of academic integrity policies and may result in disciplinary action by the university.
          </p>
        </section>

        <section class="content-section">
          <h2>3. User Responsibilities</h2>
          <p>
            You are responsible for maintaining the confidentiality of your login credentials and for all activities that occur under your account. You agree to notify us immediately of any unauthorized use of your account.
          </p>
        </section>

        <section class="content-section">
          <h2>4. System Availability</h2>
          <p>
            While we strive for 24/7 availability, the CAT system may be subject to maintenance windows or unexpected downtime. The university is not responsible for any issues arising from temporary system unavailability.
          </p>
        </section>

        <section class="content-section">
          <h2>5. Modifications</h2>
          <p>
            We reserve the right to modify these terms at any time. Your continued use of the system following any changes constitutes your acceptance of the new terms.
          </p>
        </section>
      </div>
    </main>
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
