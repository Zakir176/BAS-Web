<template>
  <section class="how-it-works-section">
    <div class="container">
      <h2 class="section-title">How It Works</h2>
      <div class="steps-grid">
        <div class="step-item">
          <div class="step-number">1</div>
          <div class="step-icon">
            <svg width="48" height="48" viewBox="0 0 48 48" fill="none">
              <rect width="48" height="48" rx="12" fill="var(--accent-primary)" opacity="0.1"/>
              <path d="M16 12h16v4H16v-4zm0 8h16v4H16v-4zm0 8h10v4H16v-4z" fill="var(--accent-primary)"/>
            </svg>
          </div>
          <h3 class="step-title">Generate Barcode</h3>
          <p class="step-description">
            Students generate a unique barcode from their portal.
          </p>
        </div>
        <div class="step-item">
          <div class="step-number">2</div>
          <div class="step-icon">
            <svg width="48" height="48" viewBox="0 0 48 48" fill="none">
              <rect width="48" height="48" rx="12" fill="var(--accent-primary)" opacity="0.1"/>
              <path d="M21 14h6v20h-6V14zM14 21h20v6H14v-6z" fill="var(--accent-primary)"/>
            </svg>
          </div>
          <h3 class="step-title">Scan Barcode</h3>
          <p class="step-description">
            Lecturers scan the barcode using the app to record attendance.
          </p>
        </div>
        <div class="step-item">
          <div class="step-number">3</div>
          <div class="step-icon">
            <svg width="48" height="48" viewBox="0 0 48 48" fill="none">
              <rect width="48" height="48" rx="12" fill="var(--accent-primary)" opacity="0.1"/>
              <path d="M12 12h24v4H12v-4zm0 8h24v4H12v-4zm0 8h16v4H12v-4z" fill="var(--accent-primary)"/>
            </svg>
          </div>
          <h3 class="step-title">Track Attendance</h3>
          <p class="step-description">
            Attendance is instantly updated and can be tracked in real-time.
          </p>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
</script>

<style scoped>
.how-it-works-section {
  padding: 4rem 0;
  background-color: var(--bg-secondary);
  text-align: center;
}

.section-title {
  font-size: 2.5rem;
  font-weight: 700;
  color: var(--text-primary);
  margin-bottom: 3rem;
}

.steps-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 2rem;
  max-width: 1000px;
  margin: 0 auto;
}

.step-item {
  position: relative;
  padding: 2rem;
  border-radius: 12px;
  background-color: var(--bg-primary);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.step-item:hover {
  transform: translateY(-5px);
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.1);
}

.step-icon {
  margin-bottom: 1.5rem;
  display: flex;
  justify-content: center;
}

.step-number {
  position: absolute;
  top: -1.5rem;
  left: 50%;
  transform: translateX(-50%);
  width: 3rem;
  height: 3rem;
  border-radius: 50%;
  background-color: var(--accent-primary);
  color: white;
  font-size: 1.5rem;
  font-weight: 700;
  display: flex;
  align-items: center;
  justify-content: center;
}

.step-title {
  font-size: 1.5rem;
  font-weight: 600;
  color: var(--text-primary);
  margin-top: 1rem;
  margin-bottom: 1rem;
}

.step-description {
  color: var(--text-secondary);
  line-height: 1.6;
}
</style>
