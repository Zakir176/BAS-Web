<template>
  <div class="split-auth-page">
    <!-- Left Side: Brand Showcase -->
    <div class="brand-panel lecturer-theme">
      <div class="brand-content">
        <div class="logo-mark">👨‍🏫</div>
        <h1 class="brand-title">Lecturer Portal</h1>
        <p class="brand-subtitle">Manage courses, track sessions, and empower your students with BAS.</p>
        
        <div class="feature-list">
          <div class="feature-item">
            <span class="icon">⚡</span>
            <span>Real-time scanner capabilities</span>
          </div>
          <div class="feature-item">
            <span class="icon">📊</span>
            <span>Comprehensive course analytics</span>
          </div>
          <div class="feature-item">
            <span class="icon">🔒</span>
            <span>Secure attendance sessions</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Right Side: Interaction Form -->
    <div class="form-panel">
      <div class="form-wrapper">
        <div class="form-header">
          <h2>Welcome Back</h2>
          <p>Please log in to your instructor account.</p>
        </div>

        <Form @submit="handleLogin" :validation-schema="schema" class="auth-form" v-slot="{ errors }">
          <div class="input-group-clean">
            <label for="email" class="clean-label">Official Email</label>
            <Field name="email" type="email" id="email" placeholder="instructor@university.edu" class="clean-input" :class="{'is-invalid': errors.email}" />
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

          <Button type="submit" variant="primary" size="lg" full-width class="submit-btn" :disabled="isLoading">
            <span v-if="!isLoading">Sign In</span>
            <span v-else>Signing In...</span>
          </Button>
        </Form>

        <p class="auth-footer">
          New to the portal? 
          <router-link to="/lecturer-signup" class="footer-link">Register here</router-link>
        </p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { useRouter, useRoute } from 'vue-router'
import { Form, Field, ErrorMessage } from 'vee-validate'
import * as yup from 'yup'
import { useAuth } from '@/composables/useAuth'
import { useToast } from '@/composables/useToast'
import Button from '@/components/ui/Button.vue'

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
    
    const userRole = data?.user?.user_metadata?.role || 'student'
    if (userRole !== 'teacher' && userRole !== 'admin' && userRole !== 'lecturer') {
      throw new Error('Unauthorized Access. This portal is for Instructors only.')
    }
    
    toast.success('Successfully logged in!')
    
    const redirectPath = route.query.redirect || '/lecturer-dashboard'
    router.push(redirectPath)
  } catch (err) {
    if (err.message && err.message.includes('Unauthorized Access')) {
      toast.error(err.message)
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
  background-color: var(--bg-main);
}

/* Left Panel: Brand Showcase */
.brand-panel {
  display: none;
  flex: 1;
  position: relative;
  overflow: hidden;
  padding: 4rem;
  color: #ffffff;
}

.lecturer-theme {
  background-color: #064e3b; /* Deep emerald for lecturers */
  background-image: 
    radial-gradient(circle at 15% 50%, rgba(16, 185, 129, 0.15), transparent 40%),
    radial-gradient(circle at 85% 30%, rgba(52, 211, 153, 0.1), transparent 50%);
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
  border-color: #059669; /* Match lecturer emerald */
  box-shadow: 0 0 0 3px rgba(5, 150, 105, 0.15);
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
  color: #059669;
  text-decoration: none;
}

.forgot-link:hover {
  text-decoration: underline;
}

.submit-btn {
  margin-top: 1rem;
  background-color: #059669;
  border: none;
}

.submit-btn:hover {
  background-color: #047857;
}

.auth-footer {
  margin-top: 2rem;
  text-align: center;
  color: var(--text-muted);
  font-size: 0.95rem;
}

.footer-link {
  color: #059669;
  font-weight: 700;
  text-decoration: none;
}

.footer-link:hover {
  text-decoration: underline;
}
</style>
