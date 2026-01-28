<template>
  <div class="auth-page reset-password">
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
            <h2>Reset Your Password</h2>
            <p>Please enter your new password below.</p>
          </div>

          <form @submit.prevent="handleUpdate" class="auth-form">
            <div class="form-group">
              <label>New Password</label>
              <div class="input-wrapper">
                <span class="input-icon">🔒</span>
                <input 
                  v-model="newPassword" 
                  type="password" 
                  placeholder="••••••••" 
                  required
                  class="input"
                >
              </div>
            </div>

            <div class="form-group">
              <label>Confirm New Password</label>
              <div class="input-wrapper">
                <span class="input-icon">🔒</span>
                <input 
                  v-model="confirmPassword" 
                  type="password" 
                  placeholder="••••••••" 
                  required
                  class="input"
                >
              </div>
            </div>

            <Button
              type="submit"
              variant="primary"
              full-width
              :disabled="isLoading"
              class="submit-btn"
            >
              <span v-if="!isLoading">Update Password</span>
              <span v-else>Updating...</span>
            </Button>
          </form>
        </Card>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '@/composables/useAuth'
import { useToast } from '@/composables/useToast'
import Button from '@/components/ui/Button.vue'
import Card from '@/components/ui/Card.vue'

const router = useRouter()
const newPassword = ref('')
const confirmPassword = ref('')
const { updatePassword, isLoading } = useAuth()
const { toast } = useToast()

const handleUpdate = async () => {
  if (newPassword.value !== confirmPassword.value) {
    toast.error('Passwords do not match')
    return
  }

  if (newPassword.value.length < 6) {
    toast.error('Password must be at least 6 characters')
    return
  }

  try {
    await updatePassword(newPassword.value)
    toast.success('Password updated successfully! You can now log in.')
    setTimeout(() => {
      router.push('/lecturer-login')
    }, 2000)
  } catch (err) {
    console.error('Password update failed:', err)
    toast.error(err.message || 'Failed to update password')
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
  border-radius: 32px;
  padding: 3rem 2.5rem;
  background: var(--bg-card);
  border: none;
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.2);
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

.submit-btn {
  height: 4rem;
  font-size: 1.125rem;
  border-radius: 20px;
  margin-top: 1rem;
}

@keyframes slideUp {
  from { transform: translateY(20px); opacity: 0; }
  to { transform: translateY(0); opacity: 1; }
}
</style>
