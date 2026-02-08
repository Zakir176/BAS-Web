<template>
  <div class="student-login-page">
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
                <circle cx="150" cy="80" r="30" fill="var(--accent-primary)" opacity="0.2" />
                <rect
                  x="110"
                  y="130"
                  width="80"
                  height="100"
                  rx="10"
                  fill="var(--accent-primary)"
                  opacity="0.3"
                />
                <path d="M130 180h40v30h-40z" fill="var(--accent-primary)" />
                <circle cx="150" cy="80" r="20" fill="var(--accent-primary)" />
                <path d="M140 75h20v10h-20z" fill="white" />
                <rect x="120" y="200" width="60" height="8" rx="4" fill="var(--accent-secondary)" />
                <rect x="120" y="215" width="60" height="8" rx="4" fill="var(--accent-secondary)" />
              </svg>
            </div>
            <div class="login-features">
              <h3>Student Portal</h3>
              <ul>
                <li>Mark attendance with barcode</li>
                <li>View attendance history</li>
                <li>Check class schedules</li>
                <li>Download attendance reports</li>
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
              <h2>Student Login</h2>
              <p>Sign in to mark your attendance</p>
            </div>

            <form @submit.prevent="handleLogin" class="login-form">
              <Input
                v-model="formData.email"
                label="Email Address"
                type="email"
                placeholder="student@university.edu"
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

            <div class="divider">
              <span>OR</span>
            </div>

            <Button variant="secondary" size="lg" full-width @click="goToSignup">
              Create New Account
            </Button>

            <div class="login-footer">
              <p>
                Are you a lecturer?
                <router-link to="/lecturer-login" class="link"> Lecturer Portal </router-link>
              </p>
            </div>
          </Card>
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
  studentId: "", // Note: Still using studentId as a conceptual label, but we need email for auth
  email: "", // Added email field for auth
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
      // Redirect to student homepage
      router.push("/student-homepage");
    }
  } catch (error) {
    console.error("Login failed:", error);
    errors.password = error.message || "Invalid email or password";
  } finally {
    isLoading.value = false;
  }
};

const goToSignup = () => {
  router.push("/student-signup");
};
</script>

<style scoped>
.student-login-page {
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

.login-container {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 4rem;
  max-width: 1000px;
  width: 100%;
  align-items: center;
}

.login-visual {
  text-align: center;
}

.login-illustration {
  margin-bottom: 2rem;
}

.login-features h3 {
  font-size: 1.5rem;
  font-weight: 600;
  color: var(--text-primary);
  margin-bottom: 1.5rem;
}

.login-features ul {
  list-style: none;
  padding: 0;
  text-align: left;
  max-width: 300px;
  margin: 0 auto;
}

.login-features li {
  padding: 0.75rem 0;
  color: var(--text-secondary);
  position: relative;
  padding-left: 1.5rem;
}

.login-features li::before {
  content: "✓";
  position: absolute;
  left: 0;
  color: var(--success);
  font-weight: bold;
}

.login-card {
  max-width: 400px;
  width: 100%;
  margin: 0 auto;
}

.login-header {
  text-align: center;
  margin-bottom: 2rem;
}

.login-logo {
  display: flex;
  justify-content: center;
  margin-bottom: 1rem;
}

.login-header h2 {
  font-size: 1.875rem;
  font-weight: 700;
  color: var(--text-primary);
  margin-bottom: 0.5rem;
}

.login-header p {
  color: var(--text-secondary);
}

.login-form {
  margin-bottom: 1.5rem;
}

.form-options {
  display: flex;
  justify-content: space-between;
  align-items: center;
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
  content: "✓";
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  color: white;
  font-size: 0.75rem;
}

.forgot-password {
  color: var(--accent-primary);
  text-decoration: none;
  font-size: 0.875rem;
  transition: color 0.2s ease;
}

.forgot-password:hover {
  color: var(--accent-hover);
}

.divider {
  text-align: center;
  margin: 1.5rem 0;
  position: relative;
}

.divider::before {
  content: "";
  position: absolute;
  top: 50%;
  left: 0;
  right: 0;
  height: 1px;
  background-color: var(--border-primary);
}

.divider span {
  background-color: var(--card-bg);
  padding: 0 1rem;
  color: var(--text-secondary);
  font-size: 0.875rem;
  position: relative;
  z-index: 1;
}

.login-footer {
  text-align: center;
  padding-top: 1.5rem;
  border-top: 1px solid var(--border-primary);
  margin-top: 1.5rem;
}

.login-footer p {
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
