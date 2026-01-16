<template>
  <div class="lecturer-signup">
    <div class="signup-container">
      <div class="signup-card">
        <div class="signup-header">
          <h1>Lecturer Registration</h1>
          <p>Create your account to manage courses and track attendance</p>
        </div>

        <form @submit.prevent="handleSignup" class="signup-form">
          <div class="form-row">
            <Input
              v-model="formData.firstName"
              label="First Name"
              type="text"
              placeholder="Enter your first name"
              required
              :error="errors.firstName"
            />
            
            <Input
              v-model="formData.lastName"
              label="Last Name"
              type="text"
              placeholder="Enter your last name"
              required
              :error="errors.lastName"
            />
          </div>
          
          <Input
            v-model="formData.email"
            label="Email Address"
            type="email"
            placeholder="lecturer@university.edu"
            required
            :error="errors.email"
          />
          
          <Input
            v-model="formData.department"
            label="Department"
            type="text"
            placeholder="Computer Science"
            required
            :error="errors.department"
          />
          
          <div class="form-row">
            <Input
              v-model="formData.password"
              label="Password"
              type="password"
              placeholder="Create a strong password"
              required
              :error="errors.password"
            />
            
            <Input
              v-model="formData.confirmPassword"
              label="Confirm Password"
              type="password"
              placeholder="Confirm your password"
              required
              :error="errors.confirmPassword"
            />
          </div>
          
          <div class="form-actions">
            <Button 
              type="submit" 
              variant="primary" 
              size="lg"
              :disabled="isLoading"
              class="submit-button"
            >
              <span v-if="!isLoading">Create Account</span>
              <span v-else>Creating Account...</span>
            </Button>
          </div>
        </form>

        <div class="signup-footer">
          <p>Already have an account?</p>
          <router-link to="/lecturer-login" class="login-link">
            Sign In
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { reactive } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '@/composables/useAuth'
import Input from '@/components/ui/Input.vue'
import Button from '@/components/ui/Button.vue'

const router = useRouter()
const { signUp, isLoading } = useAuth()

const formData = reactive({
  firstName: '',
  lastName: '',
  email: '',
  department: '',
  password: '',
  confirmPassword: ''
})

const errors = reactive({
  firstName: '',
  lastName: '',
  email: '',
  department: '',
  password: '',
  confirmPassword: ''
})

const validateForm = () => {
  // Reset errors
  Object.keys(errors).forEach(key => errors[key] = '')
  
  let isValid = true
  
  if (!formData.firstName.trim()) {
    errors.firstName = 'First name is required'
    isValid = false
  }
  
  if (!formData.lastName.trim()) {
    errors.lastName = 'Last name is required'
    isValid = false
  }
  
  if (!formData.email.trim() || !/^\S+@\S+\.\S+$/.test(formData.email)) {
    errors.email = 'Please enter a valid email'
    isValid = false
  }
  
  if (!formData.department.trim()) {
    errors.department = 'Department is required'
    isValid = false
  }
  
  if (formData.password.length < 6) {
    errors.password = 'Password must be at least 6 characters'
    isValid = false
  }
  
  if (formData.password !== formData.confirmPassword) {
    errors.confirmPassword = 'Passwords do not match'
    isValid = false
  }
  
  return isValid
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
    
    // Show success message and redirect
    alert('Signup successful! Please check your email to confirm your account and then sign in.')
    router.push('/lecturer-login')

  } catch (err) {
    console.error('Signup failed:', err)
    // Display a generic error message
    errors.password = 'Signup failed. This email may already be registered.'
  }
}
</script>

<style scoped>
.lecturer-signup {
  min-height: 100vh;
  background-color: var(--bg-primary);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 2rem;
}

.signup-container {
  width: 100%;
  max-width: 500px;
}

.signup-card {
  background-color: var(--card-bg);
  border-radius: 1rem;
  padding: 2.5rem;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);
}

.signup-header {
  text-align: center;
  margin-bottom: 2rem;
}

.signup-header h1 {
  font-size: 2rem;
  font-weight: 700;
  color: var(--text-primary);
  margin-bottom: 0.5rem;
}

.signup-header p {
  color: var(--text-secondary);
  margin: 0;
}

.signup-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.form-actions {
  margin-top: 1rem;
}

.submit-button {
  width: 100%;
}

.signup-footer {
  text-align: center;
  margin-top: 2rem;
  padding-top: 1.5rem;
  border-top: 1px solid var(--border-primary);
}

.signup-footer p {
  color: var(--text-secondary);
  margin: 0 0 0.5rem 0;
}

.login-link {
  color: var(--accent-primary);
  text-decoration: none;
  font-weight: 500;
}

.login-link:hover {
  text-decoration: underline;
}

@media (max-width: 640px) {
  .form-row {
    grid-template-columns: 1fr;
  }
  
  .signup-card {
    padding: 1.5rem;
  }
}
</style>
