<template>
  <div class="auth-page student-signup-theme">
    <div class="auth-overlay"></div>
    <main class="auth-main">
      <div class="auth-card-wrapper">
        <div class="auth-card">
          <div class="auth-header">
            <div class="auth-brand">🎓</div>
            <h2>Join the <span class="text-gradient">Future.</span></h2>
            <p>Create your student account to get started.</p>
          </div>

          <form @submit.prevent="handleSignup" class="auth-form">
            <div class="form-row-v2">
              <div class="input-group-v2">
                <input v-model="formData.firstName" type="text" placeholder=" " required />
                <label>First Name</label>
              </div>
              <div class="input-group-v2">
                <input v-model="formData.lastName" type="text" placeholder=" " required />
                <label>Last Name</label>
              </div>
            </div>

            <div class="input-group-v2">
              <input v-model="formData.studentId" type="text" placeholder=" " required />
              <label>Student ID Number</label>
              <span class="error-msg" v-if="errors.studentId">{{ errors.studentId }}</span>
            </div>

            <div class="input-group-v2">
              <input v-model="formData.email" type="email" placeholder=" " required />
              <label>University Email</label>
              <span class="error-msg" v-if="errors.email">{{ errors.email }}</span>
            </div>

            <div class="input-group-v2">
              <input v-model="formData.classSection" type="text" placeholder=" " required />
              <label>Class Section (e.g. CS101)</label>
            </div>

            <div class="form-row-v2">
              <div class="input-group-v2">
                <input v-model="formData.password" type="password" placeholder=" " required />
                <label>Password</label>
              </div>
              <div class="input-group-v2">
                <input v-model="formData.confirmPassword" type="password" placeholder=" " required />
                <label>Confirm</label>
              </div>
            </div>

            <div class="auth-options">
              <label class="premium-checkbox">
                <input type="checkbox" v-model="formData.agreeToTerms" required>
                <span class="box"></span>
                <span class="label-txt">I accept the terms of service</span>
              </label>
            </div>

            <Button type="submit" variant="primary" size="lg" full-width class="auth-btn" :disabled="isLoading">
              <span v-if="!isLoading">Create Student Account</span>
              <span v-else>Registering...</span>
            </Button>
          </form>

          <footer class="auth-footer">
            <p>Already a member? <router-link to="/student-login">Sign In</router-link></p>
          </footer>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup>
import { reactive } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '@/composables/useAuth'
import { useToast } from '@/composables/useToast'
import Button from '@/components/ui/Button.vue'

const router = useRouter()
const { signUp, isLoading } = useAuth()
const { toast } = useToast()

const formData = reactive({
  firstName: '',
  lastName: '',
  studentId: '',
  email: '',
  classSection: '',
  password: '',
  confirmPassword: '',
  agreeToTerms: false
})

const errors = reactive({
  firstName: '',
  lastName: '',
  studentId: '',
  email: '',
  password: ''
})

const validateForm = () => {
  Object.keys(errors).forEach(k => errors[k] = '')
  if (formData.password !== formData.confirmPassword) {
    toast.error('Passwords do not match')
    return false
  }
  return true
}

const handleSignup = async () => {
  if (!formData.agreeToTerms) {
    toast.error('Please accept the terms of service')
    return
  }

  if (formData.password !== formData.confirmPassword) {
    toast.error('Passwords do not match')
    return
  }

  if (!formData.firstName || !formData.lastName || !formData.email || !formData.studentId || !formData.password) {
    toast.error('Please fill in all required fields')
    return
  }

  try {
    await signUp(formData.email, formData.password, {
      role: 'student',
      student_id: formData.studentId,
      first_name: formData.firstName,
      last_name: formData.lastName,
      full_name: `${formData.firstName} ${formData.lastName}`,
      class_section: formData.classSection
    })
    toast.success('Account created successfully! Please check your email for verification.')
    setTimeout(() => {
      router.push('/student-login')
    }, 2000)
  } catch (err) {
    console.error('Signup error:', err)
    toast.error(err.message)
  }
}
</script>

<style scoped>
.auth-page {
  min-height: 100vh;
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  background-size: cover;
  background-position: center;
}

.student-signup-theme {
  background-image: url('https://images.unsplash.com/photo-1523050335392-99238e814041?q=80&w=2070&auto=format&fit=crop');
}

.auth-overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, rgba(30,58,138,0.8), rgba(88,28,135,0.8));
  backdrop-filter: blur(8px);
}

.auth-card-wrapper {
  position: relative;
  width: 100%;
  max-width: 540px;
  padding: 1.5rem;
  z-index: 10;
}

.auth-card {
  background: rgba(255, 255, 255, 0.95);
  padding: 3rem;
  border-radius: 32px;
  box-shadow: 0 25px 50px -12px rgba(0,0,0,0.5);
}

.auth-header {
  text-align: center;
  margin-bottom: 2.5rem;
}

.auth-brand {
  font-size: 3rem;
  margin-bottom: 1rem;
}

.auth-header h2 {
  font-size: 2rem;
  font-weight: 900;
  color: #1e293b;
  margin-bottom: 0.5rem;
}

.text-gradient {
  background: linear-gradient(to right, #2563eb, #7c3aed);
  -webkit-background-clip: text;
  background-clip: text;
  -webkit-text-fill-color: transparent;
}

.auth-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.form-row-v2 {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

/* Mobile responsive - single column on small screens */
@media (max-width: 640px) {
  .form-row-v2 {
    grid-template-columns: 1fr;
  }
  
  .auth-card {
    padding: 2rem;
  }
  
  .auth-header h2 {
    font-size: 1.5rem;
  }
  
  .auth-brand {
    font-size: 2.5rem;
  }
}

.input-group-v2 {
  position: relative;
}

.input-group-v2 input {
  width: 100%;
  padding: 1rem;
  background: #f8fafc;
  border: 2px solid #e2e8f0;
  border-radius: 14px;
  font-size: 1rem;
  font-weight: 600;
  transition: all 0.2s;
}

.input-group-v2 input:focus {
  border-color: #3b82f6;
  background: white;
  outline: none;
}

.input-group-v2 label {
  position: absolute;
  left: 1rem;
  top: 1rem;
  color: #94a3b8;
  pointer-events: none;
  transition: all 0.2s;
}

.input-group-v2 input:focus ~ label,
.input-group-v2 input:not(:placeholder-shown) ~ label {
  top: -1.4rem;
  left: 0.5rem;
  font-size: 0.8rem;
  font-weight: 800;
  color: #2563eb;
}

.premium-checkbox {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  cursor: pointer;
}

.premium-checkbox input {
  position: absolute;
  opacity: 0;
  width: 0;
  height: 0;
}
.premium-checkbox .box {
  width: 20px;
  height: 20px;
  border: 2px solid #cbd5e1;
  border-radius: 6px;
  transition: all 0.2s;
}

.premium-checkbox input:checked + .box {
  background: #2563eb;
  border-color: #2563eb;
}

.label-txt { font-size: 0.9rem; font-weight: 600; color: #64748b; }

.auth-btn {
  margin-top: 1rem;
  border-radius: 14px;
}

.auth-footer {
  margin-top: 2rem;
  text-align: center;
  font-weight: 600;
  color: #64748b;
}

.auth-footer a { color: #2563eb; font-weight: 800; }
</style>
