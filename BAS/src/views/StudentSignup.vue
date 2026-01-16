<template>
  <div class="student-signup-page">
    <Navbar />
    
    <main class="main-content">
      <div class="container">
        <div class="signup-container">
          <div class="signup-visual">
            <div class="signup-illustration">
              <svg width="300" height="300" viewBox="0 0 300 300" fill="none">
                <rect width="300" height="300" rx="20" fill="var(--success)" opacity="0.05"/>
                <circle cx="150" cy="80" r="30" fill="var(--success)" opacity="0.2"/>
                <rect x="110" y="130" width="80" height="100" rx="10" fill="var(--success)" opacity="0.3"/>
                <path d="M130 180h40v30h-40z" fill="var(--success)"/>
                <circle cx="150" cy="80" r="20" fill="var(--success)"/>
                <path d="M140 75h20v10h-20z" fill="white"/>
                <path d="M135 85l10 10 20-20" stroke="white" stroke-width="3" fill="none" stroke-linecap="round" stroke-linejoin="round"/>
                <rect x="120" y="200" width="60" height="8" rx="4" fill="var(--accent-secondary)"/>
                <rect x="120" y="215" width="60" height="8" rx="4" fill="var(--accent-secondary)"/>
              </svg>
            </div>
            <div class="signup-features">
              <h3>Join BAS Today</h3>
              <ul>
                <li>Quick and easy registration</li>
                <li>Instant barcode generation</li>
                <li>Mobile-friendly interface</li>
                <li>Secure attendance tracking</li>
              </ul>
            </div>
          </div>
          
          <Card class="signup-card">
            <div class="signup-header">
              <div class="signup-logo">
                <svg width="40" height="40" viewBox="0 0 40 40" fill="none">
                  <rect width="40" height="40" rx="10" fill="var(--success)"/>
                  <path d="M10 15h20v10H10z" fill="white"/>
                  <path d="M15 10v20h10V10H15z" fill="var(--success)"/>
                </svg>
              </div>
              <h2>Create Account</h2>
              <p>Join the Barcode Attendance System</p>
            </div>
            
            <form @submit.prevent="handleSignup" class="signup-form">
              <div class="form-row">
                <Input
                  v-model="formData.firstName"
                  label="First Name"
                  type="text"
                  placeholder="John"
                  required
                  :error="errors.firstName"
                />
                <Input
                  v-model="formData.lastName"
                  label="Last Name"
                  type="text"
                  placeholder="Doe"
                  required
                  :error="errors.lastName"
                />
              </div>
              
              <Input
                v-model="formData.studentId"
                label="Student ID"
                type="text"
                placeholder="2024001"
                required
                :error="errors.studentId"
              />
              
              <Input
                v-model="formData.email"
                label="Email Address"
                type="email"
                placeholder="student@university.edu"
                required
                :error="errors.email"
              />
              
              <Input
                v-model="formData.classSection"
                label="Class Section"
                type="text"
                placeholder="CS101-A"
                :error="errors.classSection"
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
              
              <div class="form-options">
                <label class="checkbox-label">
                  <input type="checkbox" v-model="formData.agreeToTerms" required>
                  <span class="checkmark"></span>
                  I agree to the terms and conditions
                </label>
              </div>
              
              <Button
                type="submit"
                variant="primary"
                size="lg"
                full-width
                :disabled="isLoading"
              >
                <span v-if="!isLoading">Create Account</span>
                <span v-else>Creating account...</span>
              </Button>
            </form>
            
            <div class="signup-footer">
              <p>
                Already have an account? 
                <router-link to="/student-login" class="link">
                  Sign In
                </router-link>
              </p>
            </div>
          </Card>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup>
import { reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '@/composables/useAuth'
import Navbar from '@/components/layout/Navbar.vue'
import Button from '@/components/ui/Button.vue'
import Card from '@/components/ui/Card.vue'
import Input from '@/components/ui/Input.vue'

const router = useRouter()
const { signUp, isLoading } = useAuth()

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
  classSection: '',
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
  
  if (!formData.studentId.trim()) {
    errors.studentId = 'Student ID is required'
    isValid = false
  }
  
  if (!formData.email.trim() || !/^\S+@\S+\.\S+$/.test(formData.email)) {
    errors.email = 'Please enter a valid email'
    isValid = false
  }
  
  if (!formData.classSection.trim()) {
    errors.classSection = 'Class section is required'
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
  
  if (!formData.agreeToTerms) {
    // This could be a more user-friendly error/alert
    alert('You must agree to the terms and conditions.')
    isValid = false
  }
  
  return isValid
}

const handleSignup = async () => {
  if (!validateForm()) return
  
  try {
    await signUp(formData.email, formData.password, {
      role: 'student', // Explicitly set the role
      student_id: formData.studentId,
      first_name: formData.firstName,
      last_name: formData.lastName,
      full_name: `${formData.firstName} ${formData.lastName}`,
      class_section: formData.classSection
    })
    
    // Redirect to login page with a success message (optional)
    alert('Signup successful! Please check your email to confirm your account and then sign in.')
    router.push('/student-login')

  } catch (err) {
    console.error('Signup failed:', err)
    // Display a generic error to the user for security
    errors.password = 'Signup failed. The email or student ID may already be in use.'
  }
}

onMounted(() => {
  console.log('Student signup page loaded')
})
</script>

<style scoped>
.student-signup-page {
  min-height: 100vh;
  background-color: var(--bg-primary);
}

.main-content {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 2rem 0;
}

.signup-container {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 4rem;
  max-width: 1200px;
  width: 100%;
  align-items: center;
}

.signup-visual {
  text-align: center;
}

.signup-illustration {
  margin-bottom: 2rem;
}

.signup-features h3 {
  font-size: 1.5rem;
  font-weight: 600;
  color: var(--text-primary);
  margin-bottom: 1.5rem;
}

.signup-features ul {
  list-style: none;
  padding: 0;
  text-align: left;
  max-width: 300px;
  margin: 0 auto;
}

.signup-features li {
  padding: 0.75rem 0;
  color: var(--text-secondary);
  position: relative;
  padding-left: 1.5rem;
}

.signup-features li::before {
  content: '✓';
  position: absolute;
  left: 0;
  color: var(--success);
  font-weight: bold;
}

.signup-card {
  max-width: 500px;
  width: 100%;
  margin: 0 auto;
}

.signup-header {
  text-align: center;
  margin-bottom: 2rem;
}

.signup-logo {
  display: flex;
  justify-content: center;
  margin-bottom: 1rem;
}

.signup-header h2 {
  font-size: 1.875rem;
  font-weight: 700;
  color: var(--text-primary);
  margin-bottom: 0.5rem;
}

.signup-header p {
  color: var(--text-secondary);
}

.signup-form {
  margin-bottom: 1.5rem;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.form-options {
  margin-bottom: 1.5rem;
}

.checkbox-label {
  display: flex;
  align-items: center;
  color: var(--text-secondary);
  font-size: 0.875rem;
  cursor: pointer;
}

.checkbox-label input[type="checkbox"] {
  display: none;
}

.checkmark {
  width: 1rem;
  height: 1rem;
  border: 1px solid var(--border-primary);
  border-radius: 0.25rem;
  margin-right: 0.5rem;
  position: relative;
  transition: all 0.2s ease;
}

.checkbox-label input[type="checkbox"]:checked + .checkmark {
  background-color: var(--accent-primary);
  border-color: var(--accent-primary);
}

.checkbox-label input[type="checkbox"]:checked + .checkmark::after {
  content: '✓';
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  color: white;
  font-size: 0.75rem;
}

.signup-footer {
  text-align: center;
  padding-top: 1.5rem;
  border-top: 1px solid var(--border-primary);
}

.signup-footer p {
  color: var(--text-secondary);
  font-size: 0.875rem;
}

.link {
  color: var(--accent-primary);
  text-decoration: none;
  font-weight: 500;
  transition: color 0.2s ease;
}

.link:hover {
  color: var(--accent-hover);
}

@media (max-width: 768px) {
  .signup-container {
    grid-template-columns: 1fr;
    gap: 2rem;
    text-align: center;
  }
  
  .signup-visual {
    order: 2;
  }
  
  .signup-card {
    order: 1;
  }
  
  .form-row {
    grid-template-columns: 1fr;
    gap: 0;
  }
}
</style>
