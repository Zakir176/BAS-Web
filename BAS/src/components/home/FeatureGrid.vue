<template>
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
</template>

<script setup>
import Skeleton from '@/components/ui/Skeleton.vue'

defineProps({
  isLoading: { type: Boolean, default: true }
})
</script>

<style scoped>
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

.reveal-on-scroll {
  opacity: 0;
  transform: translateY(30px);
  transition: all 0.8s cubic-bezier(0.2, 1, 0.3, 1);
}

.reveal-on-scroll.is-visible {
  opacity: 1;
  transform: translateY(0);
}

@media (max-width: 960px) {
  .overview-grid {
    grid-template-columns: 1fr;
    gap: 4rem;
  }
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 1.5rem;
}
</style>
