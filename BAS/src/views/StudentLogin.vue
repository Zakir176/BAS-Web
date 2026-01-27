<template>
  <div class="auth-page student-login">
    <div class="auth-overlay">
      <div class="auth-header">
        <div class="logo-box">
          <svg width="32" height="32" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M12 2L2 7L12 12L22 7L12 2Z" fill="var(--primary)" stroke="var(--primary)" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
            <path d="M2 17L12 22L22 17" stroke="var(--primary)" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
            <path d="M2 12L12 17L22 12" stroke="var(--primary)" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
        </div>
        <h1 class="brand-name">Smart Attendance</h1>
      </div>

      <div class="auth-card-wrapper">
        <Card class="auth-card">
          <div class="auth-card-header">
            <h2>Welcome back, Scholar</h2>
            <p>Sign in to your account to continue</p>
          </div>

          <form @submit.prevent="handleLogin" class="auth-form">
            <div class="form-group">
              <label>Student Email</label>
              <div class="input-wrapper">
                <span class="input-icon">👤</span>
                <input 
                  v-model="formData.email" 
                  type="email" 
                  placeholder="name@university.edu" 
                  required
                  class="input"
                >
              </div>
            </div>

            <div class="form-group">
              <label>Password</label>
              <div class="input-wrapper">
                <span class="input-icon">🔒</span>
                <input 
                  v-model="formData.password" 
                  :type="showPassword ? 'text' : 'password'" 
                  placeholder="••••••••" 
                  required
                  class="input"
                >
                <button type="button" class="password-toggle" @click="showPassword = !showPassword">
                  {{ showPassword ? '👁️' : '👁️‍🗨️' }}
                </button>
              </div>
            </div>

            <div class="auth-options">
              <label class="checkbox-container">
                <input type="checkbox" v-model="formData.rememberMe">
                <span class="checkmark"></span>
                Remember me
              </label>
              <a href="#" class="forgot-link">Forgot Password?</a>
            </div>

            <Button
              type="submit"
              variant="primary"
              full-width
              :disabled="isLoading"
              class="submit-btn"
            >
              <span v-if="!isLoading">Log In to Portal</span>
              <span v-else>Authenticating...</span>
              <span class="btn-arrow">→</span>
            </Button>
          </form>

          <div class="auth-footer">
            <p>Are you a faculty member? <router-link to="/lecturer-login">Lecturer Portal</router-link></p>
            <div class="secure-badge">
              <span>🛡️</span> SECURE STUDENT ACCESS
            </div>
          </div>
        </Card>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '@/composables/useAuth'
import { useToast } from '@/composables/useToast'
import Button from '@/components/ui/Button.vue'
import Card from '@/components/ui/Card.vue'

const router = useRouter()
const { signIn, isLoading } = useAuth()
const { toast } = useToast()
const showPassword = ref(false)

const formData = reactive({
  email: '',
  password: '',
  rememberMe: false
})

const handleLogin = async () => {
  if (!formData.email || !formData.password) {
    toast.error('Please fill in all fields')
    return
  }

  try {
    await signIn(formData.email, formData.password)
    toast.success('Welcome back! Redirecting to dashboard...')
    setTimeout(() => {
      router.push('/student-homepage')
    }, 1500)
  } catch (err) {
    console.error('Login failed:', err)
    toast.error('Invalid credentials. Please check your email and password.')
  }
}
</script>

<style scoped>
.auth-overlay {
  flex: 1;
  display: flex;
  flex-direction: column;
  padding: 2rem;
  background-color: rgba(15, 23, 42, 0.4);
  backdrop-filter: blur(2px);
}

.auth-header {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: auto;
}

.logo-box {
  background: white;
  padding: 0.75rem;
  border-radius: 12px;
  box-shadow: var(--shadow-soft);
  display: flex;
  align-items: center;
  justify-content: center;
}

.brand-name {
  color: white;
  font-size: 1.5rem;
  font-weight: 700;
  letter-spacing: -0.02em;
}

.auth-card-wrapper {
  margin: 0 auto;
  max-width: 480px;
  width: 100%;
  animation: slideUp 0.6s cubic-bezier(0.16, 1, 0.3, 1);
}

.auth-card {
  border-radius: 32px 32px 0 0;
  padding: 3rem 2.5rem;
  background: var(--bg-card);
  border: none;
  box-shadow: 0 -10px 40px rgba(0, 0, 0, 0.2);
}

.auth-card-header {
  margin-bottom: 2.5rem;
}

.auth-card-header h2 {
  font-size: 2rem;
  font-weight: 800;
  color: var(--text-main);
  margin-bottom: 0.5rem;
  letter-spacing: -0.03em;
}

.auth-card-header p {
  color: var(--text-muted);
  font-size: 1.125rem;
}

.auth-form {
  display: flex;
  flex-direction: column;
  gap: 1.75rem;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.625rem;
}

.form-group label {
  font-size: 0.875rem;
  font-weight: 700;
  color: var(--text-main);
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.input-wrapper {
  position: relative;
  display: flex;
  align-items: center;
}

.input-icon {
  position: absolute;
  left: 1.25rem;
  font-size: 1.25rem;
  opacity: 0.7;
}

.input-wrapper .input {
  padding-left: 3.5rem;
  height: 3.75rem;
  border-radius: 18px;
  background-color: #f9fafb;
  border: 1.5px solid #f3f4f6;
  font-size: 1rem;
}

.input-wrapper .input:focus {
  background-color: white;
  border-color: var(--primary);
}

.password-toggle {
  position: absolute;
  right: 1.25rem;
  background: none;
  border: none;
  cursor: pointer;
  font-size: 1.25rem;
  padding: 0.5rem;
  border-radius: 50%;
  transition: background 0.2s;
}

.password-toggle:hover {
  background: #eee;
}

.auth-options {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.checkbox-container {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  font-size: 0.9375rem;
  color: var(--text-muted);
  cursor: pointer;
}

.forgot-link {
  color: var(--primary);
  font-weight: 600;
  text-decoration: none;
  font-size: 0.9375rem;
}

.submit-btn {
  height: 4rem;
  font-size: 1.125rem;
  border-radius: 20px;
  margin-top: 1rem;
}

.btn-arrow {
  margin-left: auto;
  font-size: 1.5rem;
}

.auth-footer {
  margin-top: 2.5rem;
  text-align: center;
}

.auth-footer p {
  color: var(--text-muted);
  margin-bottom: 2rem;
}

.auth-footer a {
  color: var(--primary);
  font-weight: 700;
  text-decoration: none;
}

.secure-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.625rem 1.25rem;
  background: #f8fafc;
  border-radius: 12px;
  font-size: 0.75rem;
  font-weight: 800;
  color: #64748b;
  letter-spacing: 0.1em;
  border: 1px solid #e2e8f0;
}

@keyframes slideUp {
  from { transform: translateY(100%); opacity: 0; }
  to { transform: translateY(0); opacity: 1; }
}

@media (max-width: 640px) {
  .auth-overlay {
    padding: 1.5rem;
  }
  
  .auth-card-wrapper {
    margin: 0 -1.5rem -1.5rem -1.5rem;
    max-width: none;
    width: auto;
  }
  
  .auth-card {
    border-radius: 40px 40px 0 0;
    padding: 3rem 2rem;
  }
}
</style>
