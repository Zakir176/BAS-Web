<template>
  <section class="final-cta reveal-on-scroll">
    <div class="container">
      <div class="cta-inner">
        <h2 class="cta-title">Ready to modernize your classroom?</h2>
        <div class="cta-btns" v-if="!isAuthenticated">
          <Button variant="primary" size="lg" @click="$emit('student-signup')">Get Started as Student</Button>
          <Button variant="secondary" size="lg" @click="$emit('lecturer-signup')">Get Started as Lecturer</Button>
        </div>
        <div class="cta-btns" v-else>
          <Button variant="primary" size="lg" @click="$emit('dashboard')">Return to Dashboard</Button>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import Button from '@/components/ui/Button.vue'

defineProps({
  isAuthenticated: { type: Boolean, default: false }
})

defineEmits(['student-signup', 'lecturer-signup', 'dashboard'])
</script>

<style scoped>
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
  .cta-btns {
    flex-direction: column;
  }
}
</style>
