<template>
  <div class="student-signup-page">
    <Navbar />

    <main class="main-content">
      <div class="container">
        <div class="signup-container">
          <div class="signup-visual">
            <div class="signup-illustration">
              <svg width="300" height="300" viewBox="0 0 300 300" fill="none">
                <rect width="300" height="300" rx="20" fill="var(--success)" opacity="0.05" />
                <circle cx="150" cy="80" r="30" fill="var(--success)" opacity="0.2" />
                <rect
                  x="110"
                  y="130"
                  width="80"
                  height="100"
                  rx="10"
                  fill="var(--success)"
                  opacity="0.3"
                />
                <path d="M130 180h40v30h-40z" fill="var(--success)" />
                <circle cx="150" cy="80" r="20" fill="var(--success)" />
                <path d="M140 75h20v10h-20z" fill="white" />
                <path
                  d="M135 85l10 10 20-20"
                  stroke="white"
                  stroke-width="3"
                  fill="none"
                  stroke-linecap="round"
                  stroke-linejoin="round"
                />
                <rect x="120" y="200" width="60" height="8" rx="4" fill="var(--accent-secondary)" />
                <rect x="120" y="215" width="60" height="8" rx="4" fill="var(--accent-secondary)" />
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
                  <rect width="40" height="40" rx="10" fill="var(--success)" />
                  <path d="M10 15h20v10H10z" fill="white" />
                  <path d="M15 10v20h10V10H15z" fill="var(--success)" />
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

              <div class="form-options">
                <label class="checkbox-label">
                  <input type="checkbox" v-model="formData.agreeToTerms" required />
                  <span class="checkmark"></span>
                  I agree to the terms and conditions
                </label>
              </div>

              <Button type="submit" variant="primary" size="lg" full-width :disabled="isLoading">
                <span v-if="!isLoading">Create Account</span>
                <span v-else>Creating account...</span>
              </Button>
            </form>

            <div class="signup-footer">
              <p>
                Already have an account?
                <router-link to="/student-login" class="link"> Sign In </router-link>
              </p>
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
import { ref, reactive } from "vue";
import { useRouter } from "vue-router";
import { supabase } from "@/supabase";
import Navbar from "@/components/layout/Navbar.vue";
import Button from "@/components/ui/Button.vue";
import Card from "@/components/ui/Card.vue";
import Input from "@/components/ui/Input.vue";

const router = useRouter();

const formData = reactive({
  firstName: "",
  lastName: "",
  studentId: "",
  email: "",
  department: "",
  password: "",
  confirmPassword: "",
  agreeToTerms: false,
});

const errors = reactive({
  firstName: "",
  lastName: "",
  studentId: "",
  email: "",
  department: "",
  password: "",
  confirmPassword: "",
});

const isLoading = ref(false);

const validateForm = () => {
  // Clear previous errors
  Object.keys(errors).forEach((key) => {
    errors[key] = "";
  });

  let isValid = true;

  if (!formData.firstName) {
    errors.firstName = "First name is required";
    isValid = false;
  }

  if (!formData.lastName) {
    errors.lastName = "Last name is required";
    isValid = false;
  }

  if (!formData.studentId) {
    errors.studentId = "Student ID is required";
    isValid = false;
  } else if (formData.studentId.length < 3) {
    errors.studentId = "Please enter a valid student ID";
    isValid = false;
  }

  if (!formData.email) {
    errors.email = "Email is required";
    isValid = false;
  } else if (!formData.email.includes("@")) {
    errors.email = "Please enter a valid email";
    isValid = false;
  }

  if (!formData.department) {
    errors.department = "Department is required";
    isValid = false;
  }

  if (!formData.password) {
    errors.password = "Password is required";
    isValid = false;
  } else if (formData.password.length < 6) {
    errors.password = "Password must be at least 6 characters";
    isValid = false;
  }

  if (!formData.confirmPassword) {
    errors.confirmPassword = "Please confirm your password";
    isValid = false;
  } else if (formData.password !== formData.confirmPassword) {
    errors.confirmPassword = "Passwords do not match";
    isValid = false;
  }

  if (!formData.agreeToTerms) {
    alert("Please agree to the terms and conditions");
    isValid = false;
  }

  return isValid;
};

const handleSignup = async () => {
  if (!validateForm()) return;

  isLoading.value = true;

  try {
    const { data, error } = await supabase.auth.signUp({
      email: formData.email,
      password: formData.password,
      options: {
        data: {
          first_name: formData.firstName,
          last_name: formData.lastName,
          student_id: formData.studentId,
          role: "student",
        },
      },
    });

    if (error) throw error;

    if (data.user) {
      // Insert into students table
      const { error: profileError } = await supabase.from("students").insert({
        id: data.user.id,
        student_id: formData.studentId,
        first_name: formData.firstName,
        last_name: formData.lastName,
        email: formData.email,
        department: formData.department,
        is_present: false,
      });

      if (profileError) throw profileError;

      alert("Registration successful! Please login.");
      router.push("/student-login");
    }
  } catch (error) {
    console.error("Registration failed:", error);
    alert(error.message || "Registration failed. Please try again.");
  } finally {
    isLoading.value = false;
  }
};
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
  padding-left: 1.5rem;
}

.signup-features li::before {
  content: "✓";
  position: absolute;
  left: 0;
  color: var(--success);
  font-weight: bold;
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

.checkbox-label input[type="checkbox"]:checked + .checkmark::after {
  content: "✓";
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  color: white;
  font-size: 0.75rem;
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

.auth-footer a { color: #2563eb; font-weight: 800; }
</style>
