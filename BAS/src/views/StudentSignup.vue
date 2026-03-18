<template>
  <div class="split-auth-page">
    <!-- Left Side: Brand Showcase -->
    <div class="brand-panel student-theme">
      <div class="brand-content">
        <div class="logo-mark">🎓</div>
        <h1 class="brand-title">Welcome to BAS</h1>
        <p class="brand-subtitle">Join thousands of students managing their academic success.</p>
        
        <div class="feature-list">
          <div class="feature-item">
            <span class="icon">📱</span>
            <span>Mobile-first dashboard</span>
          </div>
          <div class="feature-item">
            <span class="icon">🏆</span>
            <span>Visualize your attendance rates</span>
          </div>
          <div class="feature-item">
            <span class="icon">🔔</span>
            <span>Never miss a class check-in</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Right Side: Interaction Form -->
    <div class="form-panel">
      <div class="form-wrapper">
        <div class="form-header">
          <h2>Create Account</h2>
          <p>Register as a student to access the portal.</p>
        </div>

        <Form @submit="handleSignup" :validation-schema="schema" class="auth-form" v-slot="{ errors }">
          <div class="form-row-v2">
            <div class="input-group-clean">
              <label for="firstName" class="clean-label">First Name</label>
              <Field name="firstName" type="text" id="firstName" placeholder="John" class="clean-input" :class="{'is-invalid': errors.firstName}" />
              <ErrorMessage name="firstName" class="error-message" />
            </div>
            <div class="input-group-clean">
              <label for="lastName" class="clean-label">Last Name</label>
              <Field name="lastName" type="text" id="lastName" placeholder="Smith" class="clean-input" :class="{'is-invalid': errors.lastName}" />
              <ErrorMessage name="lastName" class="error-message" />
            </div>
          </div>

          <div class="form-row-v2">
            <div class="input-group-clean">
              <label for="studentId" class="clean-label">Student ID</label>
              <Field name="studentId" type="text" id="studentId" placeholder="e.g. S1234567" class="clean-input" :class="{'is-invalid': errors.studentId}" />
              <ErrorMessage name="studentId" class="error-message" />
            </div>
            <div class="input-group-clean">
              <label for="classSection" class="clean-label">Class Section</label>
              <Field name="classSection" type="text" id="classSection" placeholder="e.g. BCS11" class="clean-input" :class="{'is-invalid': errors.classSection}" />
              <ErrorMessage name="classSection" class="error-message" />
            </div>
          </div>

          <div class="input-group-clean">
            <label for="email" class="clean-label">University Email</label>
            <Field name="email" type="email" id="email" placeholder="student@university.edu" class="clean-input" :class="{'is-invalid': errors.email}" />
            <ErrorMessage name="email" class="error-message" />
          </div>

          <div class="form-row-v2">
            <div class="input-group-clean">
              <label for="password" class="clean-label">Password</label>
              <Field name="password" type="password" id="password" placeholder="••••••••" class="clean-input" :class="{'is-invalid': errors.password}" />
              <ErrorMessage name="password" class="error-message" />
            </div>
            <div class="input-group-clean">
              <label for="confirmPassword" class="clean-label">Confirm</label>
              <Field name="confirmPassword" type="password" id="confirmPassword" placeholder="••••••••" class="clean-input" :class="{'is-invalid': errors.confirmPassword}" />
              <ErrorMessage name="confirmPassword" class="error-message" />
            </div>
          </div>

          <div class="terms-group">
            <label class="clean-checkbox-container">
              <Field name="agreeToTerms" type="checkbox" value="true" class="sr-only" />
              <div class="clean-checkbox">
                <svg viewBox="0 0 24 24" fill="none" class="check-icon">
                  <path d="M5 13l4 4L19 7" stroke="currentColor" stroke-width="3" stroke-linecap="round" stroke-linejoin="round"/>
                </svg>
              </div>
              <span class="terms-text">I agree to the <router-link to="/terms-of-service" class="terms-link" target="_blank">Terms</router-link> and <router-link to="/privacy-policy" class="terms-link" target="_blank">Privacy Policy</router-link></span>
            </label>
            <ErrorMessage name="agreeToTerms" class="error-message block-error" />
          </div>

          <Button type="submit" variant="primary" size="lg" full-width class="submit-btn" :disabled="isLoading">
            <span v-if="!isLoading">Register Account</span>
            <span v-else>Registering...</span>
          </Button>
        </Form>

        <p class="auth-footer">
          Already have an account? 
          <router-link to="/student-login" class="footer-link">Sign in instead</router-link>
        </p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { Form, Field, ErrorMessage } from 'vee-validate'
import * as yup from 'yup'
import { useAuth } from '@/composables/useAuth'
import { useToast } from '@/composables/useToast'
import Button from '@/components/ui/Button.vue'

const router = useRouter()
const { signUp, isLoading } = useAuth()
const { toast } = useToast()

const schema = yup.object({
  firstName: yup.string().required('First name is required'),
  lastName: yup.string().required('Last name is required'),
  studentId: yup.string().required('Student ID is required').matches(/^[a-zA-Z0-9]+$/, 'Alphanumeric only'),
  classSection: yup.string().required('Section is required'),
  email: yup.string().required('Email is required').email('Please enter a valid email address'),
  password: yup.string().required('Password is required').min(8, 'Must be at least 8 characters'),
  confirmPassword: yup.string()
    .required('Please confirm password')
    .oneOf([yup.ref('password')], 'Passwords must match'),
  agreeToTerms: yup.boolean().oneOf([true], 'You must agree to the terms').required('You must agree to the terms'),
});

const handleSignup = async (values) => {
  try {
    const metadata = {
      role: 'student',
      first_name: values.firstName,
      last_name: values.lastName,
      full_name: `${values.firstName} ${values.lastName}`,
      student_id: values.studentId.toUpperCase(),
      class_section: values.classSection.toUpperCase()
    }
    
    await signUp(values.email, values.password, metadata)
    
    toast.success('Account created! Please check your email.', { duration: 5000 })
    setTimeout(() => {
      router.push('/student-login')
    }, 2000)
    
  } catch (err) {
    if (err.message && err.message.includes('registered')) {
      toast.error('This Student ID or Email is already registered.')
    } else {
      toast.error(err.message || 'Signup failed. Please try again.')
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

.student-theme {
  background-color: #0f172a; /* Deep slate blue for students */
  background-image: 
    radial-gradient(circle at 85% 15%, rgba(59, 130, 246, 0.15), transparent 40%),
    radial-gradient(circle at 15% 85%, rgba(139, 92, 246, 0.1), transparent 50%);
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
  overflow-y: auto; /* For long forms */
}

.form-wrapper {
  width: 100%;
  max-width: 460px; /* slightly wider for signup grids */
  margin: auto 0;
  padding: 2rem 0;
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
  gap: 1.25rem;
}

.form-row-v2 {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

@media (max-width: 640px) {
  .form-row-v2 {
    grid-template-columns: 1fr;
  }
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
  border-color: #3b82f6; /* Student primary blue */
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.15);
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

/* Custom Checkbox */
.terms-group {
  margin-top: 0.5rem;
}

.sr-only {
  position: absolute;
  width: 1px;
  height: 1px;
  padding: 0;
  margin: -1px;
  overflow: hidden;
  clip: rect(0, 0, 0, 0);
  border: 0;
}

.clean-checkbox-container {
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
  cursor: pointer;
  padding-top: 0.25rem;
}

.clean-checkbox {
  width: 1.25rem;
  height: 1.25rem;
  min-width: 1.25rem;
  border: 1px solid var(--border-medium);
  border-radius: 4px;
  background-color: var(--bg-main);
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;
  margin-top: 2px;
}

.check-icon {
  width: 12px;
  height: 12px;
  color: white;
  opacity: 0;
  transform: scale(0.5);
  transition: all 0.2s cubic-bezier(0.175, 0.885, 0.32, 1.275);
}

.sr-only:checked + .clean-checkbox {
  background-color: #3b82f6;
  border-color: #3b82f6;
}

.sr-only:checked + .clean-checkbox .check-icon {
  opacity: 1;
  transform: scale(1);
}

.sr-only:focus-visible + .clean-checkbox {
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.3);
}

.terms-text {
  font-size: 0.9rem;
  color: var(--text-muted);
  line-height: 1.5;
}

.terms-link {
  color: #3b82f6;
  font-weight: 600;
  text-decoration: none;
}

.terms-link:hover {
  text-decoration: underline;
}

.block-error {
  display: block;
  margin-top: 0.5rem;
  margin-left: 2rem;
}

/* Actions & Footer */
.submit-btn {
  margin-top: 1rem;
  background-color: #3b82f6;
  border: none;
}

.submit-btn:hover {
  background-color: #2563eb;
}

.auth-footer {
  margin-top: 2rem;
  text-align: center;
  color: var(--text-muted);
  font-size: 0.95rem;
}

.footer-link {
  color: #3b82f6;
  font-weight: 700;
  text-decoration: none;
}

.footer-link:hover {
  text-decoration: underline;
}
</style>
