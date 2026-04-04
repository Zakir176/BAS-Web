<template>
  <div class="split-auth-page">
    <!-- Left Side: Brand Showcase -->
    <div class="brand-panel student-theme">
      <div class="brand-content">
        <div class="logo-mark">🎓</div>
        <h1 class="brand-title">Student Portal</h1>
        <p class="brand-subtitle">Your academic journey, tracked and simplified.</p>
        
        <div class="feature-list">
          <div class="feature-item">
            <span class="icon">📱</span>
            <span>Mobile-friendly access</span>
          </div>
          <div class="feature-item">
            <span class="icon">⚡</span>
            <span>Instant barcode check-ins</span>
          </div>
          <div class="feature-item">
            <span class="icon">📈</span>
            <span>Track your attendance history</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Right Side: Interaction Form -->
    <div class="form-panel">
      <div class="form-wrapper">
        <div class="form-header">
          <h2>Welcome Back</h2>
          <p>Please log in to your student account.</p>
        </div>

        <Form @submit="handleLogin" :validation-schema="schema" class="auth-form" v-slot="{ errors }">
          <div class="input-group-clean">
            <label for="email" class="clean-label">University Email</label>
            <Field name="email" type="email" id="email" placeholder="student@university.edu" class="clean-input" :class="{'is-invalid': errors.email}" />
            <ErrorMessage name="email" class="error-message" />
          </div>

          <div class="input-group-clean">
            <label for="password" class="clean-label">Password</label>
            <Field name="password" type="password" id="password" placeholder="••••••••" class="clean-input" :class="{'is-invalid': errors.password}" />
            <ErrorMessage name="password" class="error-message" />
          </div>

          <div class="form-actions">
            <!-- Space for 'Remember Me' if needed later -->
            <div></div>
            <router-link to="/forgot-password" class="forgot-link">Forgot password?</router-link>
          </div>

          <BaseButton type="submit" variant="primary" size="lg" full-width class="submit-btn" :disabled="isLoading">
            <span v-if="!isLoading">Sign In</span>
            <span v-else>Signing In...</span>
          </BaseButton>
        </Form>

        <p class="auth-footer">
          Don't have an account? 
          <router-link to="/student-signup" class="footer-link">Create one here</router-link>
        </p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { useRouter, useRoute } from 'vue-router'
import { Form, Field, ErrorMessage } from 'vee-validate'
import * as yup from 'yup'
import { useAuth } from '@/shared/composables/useAuth'
import { useToast } from '@/shared/composables/useToast'
import BaseButton from '@/core/ui/BaseButton.vue'

const router = useRouter()
const route = useRoute()
const { signIn, isLoading } = useAuth()
const { toast } = useToast()

const schema = yup.object({
  email: yup.string().required('Email is required').email('Please enter a valid email address'),
  password: yup.string().required('Password is required'),
});

const handleLogin = async (values) => {
  try {
    const data = await signIn(values.email, values.password)
    
    // Verify role and route dynamically
    const userRole = data?.user?.user_metadata?.role || 'student'
    toast.success('Successfully logged in!')
    
    const redirectPath = route.query.redirect 
                         ? route.query.redirect 
                         : (userRole === 'student' ? '/student-homepage' : '/lecturer-dashboard')
                         
    router.push(redirectPath)
  } catch (err) {
    if (err.message && err.message.includes('Invalid login credentials')) {
      toast.error('Invalid email or password.')
    } else {
      toast.error(err.message || 'Login failed. Please check your credentials.')
    }
  }
}
</script>

<style scoped>
/* Split Layout Base */
.split-auth-page {
  display: flex;
  min-height: 100vh;
  width: 100%;
  background-color: transparent;
}

/* Left Panel: Brand Showcase */
.brand-panel {
  display: none; /* Hidden on standard mobile */
  flex: 1;
  position: relative;
  overflow: hidden;
  padding: 4rem;
  color: var(--text-inverse);
}

.student-theme {
  background-color: var(--bg-panel); /* Deep slate blue for students */
  background-image: 
    radial-gradient(circle at 85% 15%, var(--info-bg), transparent 40%),
    radial-gradient(circle at 15% 85%, var(--accent-bg), transparent 50%);
}

.brand-content {
  position: relative;
  z-index: 10;
  height: 100%;
  max-width: 480px;
  display: flex;
  flex-direction: column;
  justify-content: center;
}

.logo-mark {
  font-size: 3.5rem;
  margin-bottom: 2rem;
}

.brand-title {
  font-size: 3rem;
  font-weight: 800;
  line-height: 1.1;
  margin-bottom: 1.5rem;
  letter-spacing: -0.02em;
}

.brand-subtitle {
  font-size: 1.25rem;
  line-height: 1.6;
  opacity: 0.9;
  margin-bottom: 3rem;
}

.feature-list {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.feature-item {
  display: flex;
  align-items: center;
  gap: 1rem;
  font-size: 1.1rem;
  font-weight: 500;
  opacity: 0.95;
}

.feature-item .icon {
  background: rgba(255, 255, 255, 0.1);
  padding: 0.5rem;
  border-radius: 8px;
  font-size: 1.25rem;
}

@media (min-width: 900px) {
  .brand-panel {
    display: flex;
  }
}

/* Right Panel: Form Area */
.form-panel {
  flex: 1;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  padding: 2rem;
  background-color: var(--bg-card);
}

.form-wrapper {
  width: 100%;
  max-width: 400px;
}

.form-header {
  margin-bottom: 2.5rem;
}

.form-header h2 {
  font-size: 2rem;
  font-weight: 800;
  color: var(--text-main);
  margin-bottom: 0.5rem;
  letter-spacing: -0.01em;
}

.form-header p {
  color: var(--text-muted);
  font-size: 1rem;
}

/* Clean Form Inputs */
.auth-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.input-group-clean {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.clean-label {
  font-size: 0.875rem;
  font-weight: 600;
  color: var(--text-main);
}

.clean-input {
  width: 100%;
  padding: 0.875rem 1rem;
  border: 1px solid var(--border-medium);
  border-radius: 8px;
  background-color: var(--bg-main);
  color: var(--text-main);
  font-size: 1rem;
  transition: all 0.2s ease;
}

.clean-input::placeholder {
  color: var(--text-muted);
  opacity: 0.6;
}

.clean-input:focus {
  outline: none;
  border-color: var(--primary);
  box-shadow: 0 0 0 3px var(--primary-glow);
  background-color: var(--bg-card);
}

.clean-input.is-invalid {
  border-color: var(--error);
}

.clean-input.is-invalid:focus {
  box-shadow: 0 0 0 3px var(--error-bg);
}

.error-message {
  color: var(--error);
  font-size: 0.8rem;
  font-weight: 500;
  margin-top: 0.25rem;
}

/* Actions & Footer */
.form-actions {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: -0.5rem;
}

.forgot-link {
  font-size: 0.875rem;
  font-weight: 600;
  color: var(--primary);
  text-decoration: none;
}

.forgot-link:hover {
  text-decoration: underline;
}

.submit-btn {
  margin-top: 1rem;
  background-color: var(--primary);
  border: none;
}

.submit-btn:hover {
  background-color: var(--primary-hover);
}

.auth-footer {
  margin-top: 2rem;
  text-align: center;
  color: var(--text-muted);
  font-size: 0.95rem;
}

.footer-link {
  color: var(--primary);
  font-weight: 700;
  text-decoration: none;
}

.footer-link:hover {
  text-decoration: underline;
}
</style>
