<template>
  <div class="split-auth-page">
    <!-- Left Side: Brand Showcase -->
    <div class="brand-panel lecturer-theme">
      <div class="brand-content">
        <div class="logo-mark">👨‍🏫</div>
        <h1 class="brand-title">Join BAS</h1>
        <p class="brand-subtitle">Streamline your classroom management with instant barcode attendance.</p>
        
        <div class="feature-list">
          <div class="feature-item">
            <span class="icon">✨</span>
            <span>Eliminate manual data entry</span>
          </div>
          <div class="feature-item">
            <span class="icon">📈</span>
            <span>Identify at-risk students early</span>
          </div>
          <div class="feature-item">
            <span class="icon">🔍</span>
            <span>Export comprehensive CSV reports</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Right Side: Interaction Form -->
    <div class="form-panel">
      <div class="form-wrapper">
        <div class="form-header">
          <h2>Create Account</h2>
          <p>Sign up as an instructor to get started.</p>
        </div>

        <Form @submit="handleSignup" :validation-schema="schema" class="auth-form" v-slot="{ errors }">
          <div class="form-row-v2">
            <div class="input-group-clean">
              <label for="firstName" class="clean-label">First Name</label>
              <Field name="firstName" type="text" id="firstName" placeholder="Jane" class="clean-input" :class="{'is-invalid': errors.firstName}" />
              <ErrorMessage name="firstName" class="error-message" />
            </div>
            <div class="input-group-clean">
              <label for="lastName" class="clean-label">Last Name</label>
              <Field name="lastName" type="text" id="lastName" placeholder="Doe" class="clean-input" :class="{'is-invalid': errors.lastName}" />
              <ErrorMessage name="lastName" class="error-message" />
            </div>
          </div>

          <div class="input-group-clean">
            <label for="email" class="clean-label">Official Email</label>
            <Field name="email" type="email" id="email" placeholder="instructor@university.edu" class="clean-input" :class="{'is-invalid': errors.email}" />
            <ErrorMessage name="email" class="error-message" />
          </div>

          <div class="input-group-clean">
            <label for="department" class="clean-label">Department</label>
            <Field name="departmentId" as="select" id="department" class="clean-input" :class="{'is-invalid': errors.departmentId}">
              <option value="" disabled>-- Select your department --</option>
              <option v-for="dept in departments" :key="dept.id" :value="dept.id">
                {{ dept.name }}
              </option>
            </Field>
            <ErrorMessage name="departmentId" class="error-message" />
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

          <BaseButton type="submit" variant="primary" size="lg" full-width class="submit-btn" :disabled="isLoading">
            <span v-if="!isLoading">Register Account</span>
            <span v-else>Registering...</span>
          </BaseButton>
        </Form>

        <p class="auth-footer">
          Already registered? 
          <router-link to="/lecturer-login" class="footer-link">Sign in instead</router-link>
        </p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { Form, Field, ErrorMessage } from 'vee-validate'
import * as yup from 'yup'
import { supabase } from '@/core/api/supabase'
import { useAuth } from '@/shared/composables/useAuth'
import { useToast } from '@/shared/composables/useToast'
import BaseButton from '@/core/ui/BaseButton.vue'

const router = useRouter()
const { signUp, isLoading } = useAuth()
const { toast } = useToast()
const departments = ref([])

const fetchDepartments = async () => {
  console.log('Fetching departments...')
  try {
    const { data, error } = await supabase.from('departments').select('*')
    if (error) {
       console.error('Error fetching departments:', error)
       // Fallback for UI testing
       departments.value = [
         { id: '1', name: 'Computer Science' },
         { id: '2', name: 'Information Technology' }
       ]
    } else {
       console.log('Departments loaded:', data)
       departments.value = data || []
    }
  } catch (err) {
    console.error('Catch fetching departments:', err)
  }
}

const schema = yup.object({
  firstName: yup.string().required('First name is required'),
  lastName: yup.string().required('Last name is required'),
  email: yup.string().required('Email is required').email('Please enter a valid email address'),
  departmentId: yup.string().required('Department is required'),
  password: yup.string().required('Password is required').min(8, 'Password must be at least 8 characters'),
  confirmPassword: yup.string()
    .required('Please confirm your password')
    .oneOf([yup.ref('password')], 'Passwords must match'),
});

const handleSignup = async (values) => {
  try {
    await signUp(values.email, values.password, {
      role: 'lecturer',
      first_name: values.firstName,
      last_name: values.lastName,
      full_name: `${values.firstName} ${values.lastName}`,
      department_id: values.departmentId
    })
    toast.success('Account created successfully! Please check your email.', { duration: 5000 })
    setTimeout(() => {
      router.push('/lecturer-login')
    }, 2000)
  } catch (err) {
    console.error('Signup error:', err)
    toast.error(err.message || 'Signup failed. Please try again.')
  }
}

onMounted(fetchDepartments)
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
  display: none;
  flex: 1;
  position: relative;
  overflow: hidden;
  padding: 4rem;
  color: var(--text-inverse);
}

.lecturer-theme {
  background-color: var(--success-hover); /* Deep emerald for lecturers */
  background-image: 
    radial-gradient(circle at 15% 50%, var(--success-bg), transparent 40%),
    radial-gradient(circle at 85% 30%, var(--success-bg), transparent 50%);
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
  overflow-y: auto; /* Important for long signup forms */
}

.form-wrapper {
  width: 100%;
  max-width: 440px; /* slightly wider for signup grids */
  margin: auto 0;
  padding: 2rem 0; /* extra padding for scroll breathing room */
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
  border-color: var(--success);
  box-shadow: 0 0 0 3px var(--success-bg);
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
.submit-btn {
  margin-top: 1rem;
  background-color: var(--success);
  border: none;
}

.submit-btn:hover {
  background-color: var(--success-hover);
}

.auth-footer {
  margin-top: 2rem;
  text-align: center;
  color: var(--text-muted);
  font-size: 0.95rem;
}

.footer-link {
  color: var(--success);
  font-weight: 700;
  text-decoration: none;
}

.footer-link:hover {
  text-decoration: underline;
}
</style>
