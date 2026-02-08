<template>
  <div class="lecturer-login-page">
    <Navbar />

    <main class="main-content">
      <div class="container">
        <div class="login-container">
          <div class="login-visual">
            <div class="login-illustration">
              <svg width="300" height="300" viewBox="0 0 300 300" fill="none">
                <rect
                  width="300"
                  height="300"
                  rx="20"
                  fill="var(--accent-primary)"
                  opacity="0.05"
                />
                <circle cx="150" cy="100" r="40" fill="var(--accent-primary)" opacity="0.2" />
                <rect
                  x="100"
                  y="160"
                  width="100"
                  height="80"
                  rx="10"
                  fill="var(--accent-primary)"
                  opacity="0.3"
                />
                <path d="M130 200h40v20h-40z" fill="var(--accent-primary)" />
                <circle cx="150" cy="100" r="25" fill="var(--accent-primary)" />
                <path d="M140 95h20v10h-20z" fill="white" />
                <path d="M145 90h10v15h-10z" fill="white" />
              </svg>
            </div>
            <div class="login-features">
              <h3>Lecturer Portal</h3>
              <ul>
                <li>Manage class attendance</li>
                <li>Generate reports</li>
                <li>View student statistics</li>
                <li>Export attendance data</li>
              </ul>
            </div>
          </div>

          <Card class="login-card">
            <div class="login-header">
              <div class="login-logo">
                <svg width="40" height="40" viewBox="0 0 40 40" fill="none">
                  <rect width="40" height="40" rx="10" fill="var(--accent-primary)" />
                  <path d="M10 15h20v10H10z" fill="white" />
                  <path d="M15 10v20h10V10H15z" fill="var(--accent-primary)" />
                </svg>
              </div>
              <ErrorMessage name="email" class="error-message" />
            </div>

            <form @submit.prevent="handleLogin" class="login-form">
              <Input
                v-model="formData.email"
                label="Email Address"
                type="email"
                placeholder="lecturer@university.edu"
                required
                :error="errors.email"
              />

              <Input
                v-model="formData.password"
                label="Password"
                type="password"
                placeholder="Enter your password"
                required
                :error="errors.password"
              />

              <div class="form-options">
                <label class="checkbox-label">
                  <input type="checkbox" v-model="formData.rememberMe" />
                  <span class="checkmark"></span>
                  Remember me
                </label>
                <a href="#" class="forgot-password">Forgot password?</a>
              </div>

              <Button type="submit" variant="primary" size="lg" full-width :disabled="isLoading">
                <span v-if="!isLoading">Sign In</span>
                <span v-else>Signing in...</span>
              </Button>
            </form>

            <div class="login-footer">
              <p>
                Not a lecturer?
                <router-link to="/student-login" class="link"> Student Portal </router-link>
              </p>
            </div>

            <div class="auth-options">
              <label class="checkbox-container">
                <input type="checkbox" v-model="rememberMe">
                <span class="checkmark"></span>
                Remember me
              </label>
              <router-link to="/forgot-password" class="forgot-link">Forgot Password?</router-link>
            </div>

            <Button
              type="submit"
              variant="primary"
              full-width
              :disabled="isLoading"
              class="submit-btn"
            >
              <span v-if="!isLoading">Log In to Dashboard</span>
              <span v-else>Authenticating...</span>
              <span class="btn-arrow">→</span>
            </Button>
          </Form>

          <div class="auth-footer">
            <p>Not a lecturer? <router-link to="/student-login">Student Portal</router-link></p>
            <div class="secure-badge">
              <span>🛡️</span> SECURE FACULTY PORTAL
            </div>
            <button 
              @click="handleClearSession" 
              class="clear-session-btn"
              title="Fix login issues by clearing stale credentials"
            >
              Stuck? Clear Auth Session
            </button>
          </div>
        </Card>
      </div>
    </div>
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
  email: "",
  password: "",
  rememberMe: false,
});

const errors = reactive({
  email: "",
  password: "",
});

const isLoading = ref(false);

const validateForm = () => {
  errors.email = "";
  errors.password = "";

  if (!formData.email) {
    errors.email = "Email is required";
    return false;
  }

  if (!formData.email.includes("@")) {
    errors.email = "Please enter a valid email";
    return false;
  }

  if (!formData.password) {
    errors.password = "Password is required";
    return false;
  }

  return true;
};

const handleLogin = async () => {
  if (!validateForm()) return;

  isLoading.value = true;

  try {
    const { data, error } = await supabase.auth.signInWithPassword({
      email: formData.email,
      password: formData.password,
    });

    if (error) throw error;

    if (data.session) {
      // Redirect to dashboard (create this route later)
      router.push("/student-homepage"); // Temporary redirect
    }
  } catch (error) {
    console.error("Login failed:", error);
    errors.password = error.message || "Invalid email or password";
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

.return-url-info {
  margin-top: 0.5rem;
  padding: 0.5rem;
  background: rgba(59, 130, 246, 0.1); /* blue-500/10 */
  border: 1px solid rgba(59, 130, 246, 0.2);
  border-radius: 6px;
  color: #3b82f6;
  font-size: 0.75rem;
}

.return-url-info strong {
  color: #1d4ed8;
  word-break: break-all;
}

.input.is-invalid {
  border-color: #ef4444; /* red-500 */
}

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

.login-features li::before {
  content: "✓";
  position: absolute;
  left: 0;
  color: var(--success);
  font-weight: bold;
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

.checkbox-label input[type="checkbox"]:checked + .checkmark::after {
  content: "✓";
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  color: white;
  font-size: 0.75rem;
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
  margin-bottom: 1rem;
}

.clear-session-btn {
  display: block;
  width: 100%;
  background: none;
  border: none;
  font-size: 0.75rem;
  color: var(--text-muted);
  text-decoration: underline;
  cursor: pointer;
  opacity: 0.6;
  margin-top: 1rem;
}

.clear-session-btn:hover {
  color: var(--error);
  opacity: 1;
}

@keyframes slideUp {
  from { transform: translateY(100%); opacity: 0; }
  to { transform: translateY(0); opacity: 1; }
}

@media (max-width: 768px) {
  .login-container {
    grid-template-columns: 1fr;
    gap: 2rem;
    text-align: center;
  }

  .login-visual {
    order: 2;
  }

  .login-card {
    order: 1;
  }

  .form-options {
    flex-direction: column;
    gap: 1rem;
    align-items: flex-start;
  }
}
</style>
