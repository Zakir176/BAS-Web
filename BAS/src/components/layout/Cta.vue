<template>
  <section class="cta-section">
    <div class="container">
      <div class="cta-content">
        <h2 class="cta-title">Ready to Get Started?</h2>
        <p class="cta-subtitle">
          Join the Barcode Attendance System today and streamline your attendance tracking.
        </p>
        <div class="cta-actions">
          <Button variant="primary" size="lg" @click="goToStudentSignup">
            Sign up as a Student
          </Button>
          <Button variant="secondary" size="lg" @click="goToLecturerSignup">
            Sign up as a Lecturer
          </Button>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import { useRouter } from 'vue-router'
import Button from '@/components/ui/Button.vue'

const router = useRouter()

const goToStudentSignup = () => {
  router.push('/student-signup')
}

const goToLecturerSignup = () => {
  router.push('/lecturer-signup')
}
</script>

<style scoped>
.cta-section {
  padding: 4rem 0;
  background-color: var(--bg-secondary);
  text-align: center;
}

.cta-content {
  max-width: 800px;
  margin: 0 auto;
}

.cta-title {
  font-size: 2.5rem;
  font-weight: 700;
  color: var(--text-primary);
  margin-bottom: 1.5rem;
}

.cta-subtitle {
  font-size: 1.25rem;
  color: var(--text-secondary);
  margin-bottom: 2.5rem;
  line-height: 1.6;
}

.cta-actions {
  display: flex;
  gap: 1rem;
  justify-content: center;
}

@media (max-width: 768px) {
  .cta-actions {
    flex-direction: column;
    align-items: center;
  }
}
</style>
