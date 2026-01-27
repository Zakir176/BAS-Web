<template>
  <div class="auth-page lecturer-signup-theme">
    <div class="auth-overlay"></div>
    <main class="auth-main">
      <div class="auth-card-wrapper">
        <div class="auth-card">
          <div class="auth-header">
            <div class="auth-brand">👨‍🏫</div>
            <h2>Lecturer <span class="text-gradient">Portal.</span></h2>
            <p>Empowering educators with smart tools.</p>
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
              <input v-model="formData.email" type="email" placeholder=" " required />
              <label>Official Email</label>
            </div>

            <div class="input-group-v2">
              <input v-model="formData.department" type="text" placeholder=" " required />
              <label>Department</label>
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

            <Button type="submit" variant="primary" size="lg" full-width class="auth-btn" :disabled="isLoading">
              <span v-if="!isLoading">Register as Lecturer</span>
              <span v-else>Registering...</span>
            </Button>
          </form>

          <footer class="auth-footer">
            <p>Already registered? <router-link to="/lecturer-login">Sign In</router-link></p>
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
  email: '',
  department: '',
  password: '',
  confirmPassword: ''
})

const validateForm = () => {
  if (formData.password !== formData.confirmPassword) {
    toast.error('Passwords do not match')
    return false
  }
  
  if (!formData.firstName || !formData.lastName || !formData.email || !formData.department || !formData.password) {
    toast.error('Please fill in all required fields')
    return false
  }
  
  return true
}

const handleSignup = async () => {
  if (!validateForm()) return
  
  try {
    await signUp(formData.email, formData.password, {
      role: 'lecturer',
      first_name: formData.firstName,
      last_name: formData.lastName,
      full_name: `${formData.firstName} ${formData.lastName}`,
      department: formData.department
    })
    toast.success('Lecturer account created successfully! Please check your email for verification.')
    setTimeout(() => {
      router.push('/lecturer-login')
    }, 2000)
  } catch (err) {
    console.error('Signup error:', err)
    toast.error('Signup failed. Please check your details and try again.')
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

.lecturer-signup-theme {
  background-image: url('https://images.unsplash.com/photo-1517245386807-bb43f82c33c4?q=80&w=2070&auto=format&fit=crop');
}

.auth-overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, rgba(6,78,59,0.8), rgba(30,58,138,0.8));
  backdrop-filter: blur(10px);
}

.auth-card-wrapper {
  position: relative;
  width: 100%;
  max-width: 520px;
  padding: 1.5rem;
  z-index: 10;
}

.auth-card {
  background: rgba(255, 255, 255, 0.98);
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
  color: #064e3b;
  margin-bottom: 0.5rem;
}

.text-gradient {
  background: linear-gradient(to right, #059669, #2563eb);
  -webkit-background-clip: text;
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
  background: #f0fdf4;
  border: 2px solid #dcfce7;
  border-radius: 14px;
  font-size: 1rem;
  font-weight: 600;
  transition: all 0.2s;
}

.input-group-v2 label {
  position: absolute;
  left: 1rem;
  top: 1rem;
  color: #64748b;
  pointer-events: none;
  transition: all 0.2s;
}

.input-group-v2 input:focus ~ label,
.input-group-v2 input:not(:placeholder-shown) ~ label {
  top: -1.4rem;
  left: 0.5rem;
  font-size: 0.8rem;
  font-weight: 800;
  color: #059669;
}

.auth-btn {
  margin-top: 1rem;
  background: #059669;
  border-radius: 14px;
}

.auth-footer {
  margin-top: 2rem;
  text-align: center;
  font-weight: 600;
  color: #64748b;
}

.auth-footer a { color: #059669; font-weight: 800; }
</style>
