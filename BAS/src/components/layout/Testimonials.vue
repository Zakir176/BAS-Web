<template>
  <section class="testimonials-section">
    <div class="container">
      <h2 class="section-title">What Our Users Say</h2>
      <div class="testimonials-grid">
        <Card class="testimonial-card">
          <div class="testimonial-author">
            <img src="https://i.pravatar.cc/150?u=a042581f4e29026704d" alt="Student" class="author-avatar" />
            <div>
              <h4 class="author-name">Sarah Johnson</h4>
              <p class="author-role">Student</p>
            </div>
          </div>
          <p class="testimonial-text">
            "The barcode attendance system is so easy to use! It saves me so much time in the morning. I just generate my barcode and I'm good to go."
          </p>
        </Card>
        <Card class="testimonial-card">
          <div class="testimonial-author">
            <img src="https://i.pravatar.cc/150?u=a042581f4e29026704e" alt="Lecturer" class="author-avatar" />
            <div>
              <h4 class="author-name">David Miller</h4>
              <p class="author-role">Lecturer</p>
            </div>
          </div>
          <p class="testimonial-text">
            "This system has been a game-changer for my lectures. I can quickly scan everyone's barcode and have a digital record of attendance instantly."
          </p>
        </Card>
      </div>
    </div>
  </section>
</template>

<script setup>
import Card from '@/components/ui/Card.vue'
</script>

<style scoped>
.testimonials-section {
  padding: 4rem 0;
  text-align: center;
}

.section-title {
  font-size: 2.5rem;
  font-weight: 700;
  color: var(--text-primary);
  margin-bottom: 3rem;
}

.testimonials-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 2rem;
  max-width: 1000px;
  margin: 0 auto;
}

.testimonial-card {
  padding: 2rem;
  text-align: left;
}

.testimonial-author {
  display: flex;
  align-items: center;
  margin-bottom: 1.5rem;
}

.author-avatar {
  width: 50px;
  height: 50px;
  border-radius: 50%;
  margin-right: 1rem;
}

.author-name {
  font-size: 1.125rem;
  font-weight: 600;
  color: var(--text-primary);
}

.author-role {
  color: var(--text-secondary);
  font-size: 0.875rem;
}

.testimonial-text {
  color: var(--text-secondary);
  line-height: 1.6;
  font-style: italic;
}
</style>
