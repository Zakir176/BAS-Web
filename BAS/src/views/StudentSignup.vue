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

          <Form @submit="handleSignup" :validation-schema="schema" class="auth-form" v-slot="{ errors }">
            <div class="form-row-v2">
              <div class="input-group-v2">
                <Field name="firstName" type="text" id="firstName" placeholder=" " class="input-field" :class="{'is-invalid': errors.firstName}" />
                <label for="firstName">First Name</label>
                <ErrorMessage name="firstName" class="error-message" />
              </div>
              <div class="input-group-v2">
                <Field name="lastName" type="text" id="lastName" placeholder=" " class="input-field" :class="{'is-invalid': errors.lastName}" />
                <label for="lastName">Last Name</label>
                <ErrorMessage name="lastName" class="error-message" />
              </div>
            </div>

            <div class="input-group-v2">
              <Field name="studentId" type="text" id="studentId" placeholder=" " class="input-field" :class="{'is-invalid': errors.studentId}" />
              <label for="studentId">Student ID Number</label>
              <ErrorMessage name="studentId" class="error-message" />
            </div>

            <div class="input-group-v2">
              <Field name="email" type="email" id="email" placeholder=" " class="input-field" :class="{'is-invalid': errors.email}" />
              <label for="email">University Email</label>
              <ErrorMessage name="email" class="error-message" />
            </div>

            <div class="input-group-v2">
              <Field name="classSection" type="text" id="classSection" placeholder=" " class="input-field" :class="{'is-invalid': errors.classSection}" />
              <label for="classSection">Class Section (e.g. CS101)</label>
              <ErrorMessage name="classSection" class="error-message" />
            </div>

            <div class="form-row-v2">
              <div class="input-group-v2">
                <Field name="password" type="password" id="password" placeholder=" " class="input-field" :class="{'is-invalid': errors.password}" />
                <label for="password">Password</label>
                <ErrorMessage name="password" class="error-message" />
              </div>
              <div class="input-group-v2">
                <Field name="confirmPassword" type="password" id="confirmPassword" placeholder=" " class="input-field" :class="{'is-invalid': errors.confirmPassword}" />
                <label for="confirmPassword">Confirm</label>
                <ErrorMessage name="confirmPassword" class="error-message" />
              </div>
            </div>

            <div class="auth-options">
              <label class="premium-checkbox">
                <Field name="agreeToTerms" type="checkbox" :value="true" />
                <span class="box"></span>
                <span class="label-txt">I accept the terms of service</span>
              </label>
              <ErrorMessage name="agreeToTerms" class="error-message" />
            </div>

            <Button type="submit" variant="primary" size="lg" full-width class="auth-btn" :disabled="isLoading">
              <span v-if="!isLoading">Create Student Account</span>
              <span v-else>Registering...</span>
            </Button>
          </Form>

          <footer class="auth-footer">
            <p>Already a member? <router-link to="/student-login">Sign In</router-link></p>
          </footer>
        </div>
      </div>
    </main>
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
  studentId: yup.string().required('Student ID is required'),
  email: yup.string().required('Email is required').email('Invalid email format'),
  classSection: yup.string().required('Class section is required'),
  password: yup.string().required('Password is required').min(8, 'Password must be at least 8 characters'),
  confirmPassword: yup.string()
    .required('Please confirm your password')
    .oneOf([yup.ref('password')], 'Passwords must match'),
  agreeToTerms: yup.boolean().oneOf([true], 'You must accept the terms of service'),
});

const handleSignup = async (values) => {
  try {
    await signUp(values.email, values.password, {
      role: 'student',
      student_id: values.studentId,
      first_name: values.firstName,
      last_name: values.lastName,
      full_name: `${values.firstName} ${values.lastName}`,
      class_section: values.classSection
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
.error-message {
  color: #ef4444; /* red-500 */
  font-size: 0.875rem; /* text-sm */
  margin-top: 0.25rem;
}

.input-field.is-invalid {
  border-color: #ef4444;
}

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

.input-group-v2 .input-field {
  width: 100%;
  padding: 1rem;
  background: #f8fafc;
  border: 2px solid #e2e8f0;
  border-radius: 14px;
  font-size: 1rem;
  font-weight: 600;
  transition: all 0.2s;
}

.input-group-v2 .input-field:focus {
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

.input-group-v2 .input-field:focus ~ label,
.input-group-v2 .input-field:not(:placeholder-shown) ~ label {
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
