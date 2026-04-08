<template>
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
</template>

<style scoped>
.workflow-steps {
  padding: 8rem 0;
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

.reveal-on-scroll {
  opacity: 0;
  transform: translateY(30px);
  transition: all 0.8s cubic-bezier(0.2, 1, 0.3, 1);
}

.reveal-on-scroll.is-visible {
  opacity: 1;
  transform: translateY(0);
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 1.5rem;
}

@media (max-width: 768px) {
  .steps-items {
    grid-template-columns: 1fr;
    gap: 3rem;
  }
  .step-line {
    display: none;
  }
}
</style>
