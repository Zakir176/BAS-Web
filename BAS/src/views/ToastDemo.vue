<template>
  <div class="toast-demo-page">
    <Navbar />
    
    <main class="main-content">
      <div class="container">
        <div class="demo-header">
          <h1>Toast Notifications Demo</h1>
          <p>Test the different types of toast notifications available in the BAS system.</p>
        </div>

        <div class="demo-grid">
          <!-- Basic Toasts -->
          <section class="demo-section">
            <h2>Basic Toasts</h2>
            <div class="button-grid">
              <button @click="showSuccess" class="demo-btn success">
                <span class="icon">✓</span>
                Success Toast
              </button>
              <button @click="showError" class="demo-btn error">
                <span class="icon">✗</span>
                Error Toast
              </button>
              <button @click="showWarning" class="demo-btn warning">
                <span class="icon">⚠</span>
                Warning Toast
              </button>
              <button @click="showInfo" class="demo-btn info">
                <span class="icon">ℹ</span>
                Info Toast
              </button>
            </div>
          </section>

          <!-- Advanced Toasts -->
          <section class="demo-section">
            <h2>Advanced Toasts</h2>
            <div class="button-grid">
              <button @click="showWithTitle" class="demo-btn">
                <span class="icon">📝</span>
                Toast with Title
              </button>
              <button @click="showPersistent" class="demo-btn">
                <span class="icon">📌</span>
                Persistent Toast
              </button>
              <button @click="showWithAction" class="demo-btn">
                <span class="icon">👆</span>
                Toast with Action
              </button>
              <button @click="showCustomDuration" class="demo-btn">
                <span class="icon">⏱</span>
                Custom Duration
              </button>
            </div>
          </section>

          <!-- Promise Toasts -->
          <section class="demo-section">
            <h2>Promise Toasts</h2>
            <div class="button-grid">
              <button @click="simulateSuccess" class="demo-btn">
                <span class="icon">🔄</span>
                Simulate Success
              </button>
              <button @click="simulateError" class="demo-btn">
                <span class="icon">❌</span>
                Simulate Error
              </button>
              <button @click="simulateUpload" class="demo-btn">
                <span class="icon">📤</span>
                Simulate Upload
              </button>
            </div>
          </section>

          <!-- Toast Management -->
          <section class="demo-section">
            <h2>Toast Management</h2>
            <div class="button-grid">
              <button @click="showMultiple" class="demo-btn">
                <span class="icon">📢</span>
                Show Multiple
              </button>
              <button @click="clearAll" class="demo-btn danger">
                <span class="icon">🗑</span>
                Clear All
              </button>
            </div>
          </section>
        </div>

        <!-- Toast Counter -->
        <div class="toast-stats">
          <div class="stat-card">
            <span class="stat-label">Active Toasts</span>
            <span class="stat-value">{{ toasts.length }}</span>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup>
import { useToast } from '@/composables/useToast'
import Navbar from '@/components/common/Navbar.vue'

const { toasts, success, error, warning, info, addToast, clearAllToasts } = useToast()

// Basic Toasts
const showSuccess = () => {
  success('Operation completed successfully!')
}

const showError = () => {
  error('Something went wrong. Please try again.')
}

const showWarning = () => {
  warning('Please review your input before proceeding.')
}

const showInfo = () => {
  info('New features have been added to the system.')
}

// Advanced Toasts
const showWithTitle = () => {
  addToast({
    type: 'success',
    title: 'Profile Updated',
    message: 'Your changes have been saved successfully.'
  })
}

const showPersistent = () => {
  addToast({
    type: 'info',
    title: 'System Maintenance',
    message: 'Scheduled maintenance will occur at 2:00 AM.',
    duration: 0, // No auto-dismiss
    closable: true
  })
}

const showWithAction = () => {
  addToast({
    type: 'warning',
    title: 'Session Expiring',
    message: 'Your session will expire in 5 minutes.',
    onClick: () => {
      success('Session extended!')
    }
  })
}

const showCustomDuration = () => {
  addToast({
    type: 'info',
    message: 'This toast will disappear in 2 seconds.',
    duration: 2000
  })
}

// Promise Toasts
const simulateSuccess = () => {
  const promise = new Promise((resolve) => {
    setTimeout(() => resolve('Data loaded successfully'), 2000)
  })

  useToast().promise(promise, {
    loading: 'Loading data...',
    success: (result) => result,
    error: 'Failed to load data'
  })
}

const simulateError = () => {
  const promise = new Promise((resolve, reject) => {
    setTimeout(() => reject(new Error('Network error')), 2000)
  })

  useToast().promise(promise, {
    loading: 'Connecting to server...',
    success: 'Connected successfully',
    error: 'Connection failed'
  })
}

const simulateUpload = () => {
  const promise = new Promise((resolve) => {
    setTimeout(() => resolve('File uploaded successfully'), 3000)
  })

  useToast().promise(promise, {
    loading: 'Uploading file...',
    success: (result) => result,
    error: 'Upload failed'
  })
}

// Toast Management
const showMultiple = () => {
  success('First toast')
  setTimeout(() => warning('Second toast'), 500)
  setTimeout(() => error('Third toast'), 1000)
  setTimeout(() => info('Fourth toast'), 1500)
}

const clearAll = () => {
  clearAllToasts()
  success('All toasts cleared!')
}
</script>

<style scoped>
.toast-demo-page {
  min-height: 100vh;
  background: var(--bg-main);
}

.main-content {
  padding: 2rem 0;
}

.demo-header {
  text-align: center;
  margin-bottom: 3rem;
}

.demo-header h1 {
  font-size: 2.5rem;
  font-weight: 800;
  color: var(--text-main);
  margin-bottom: 1rem;
}

.demo-header p {
  font-size: 1.125rem;
  color: var(--text-muted);
  max-width: 600px;
  margin: 0 auto;
}

.demo-grid {
  display: grid;
  gap: 2rem;
  margin-bottom: 3rem;
}

.demo-section {
  background: var(--bg-card);
  padding: 2rem;
  border-radius: 1rem;
  box-shadow: var(--shadow-soft);
}

.demo-section h2 {
  font-size: 1.5rem;
  font-weight: 700;
  color: var(--text-main);
  margin-bottom: 1.5rem;
}

.button-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1rem;
}

.demo-btn {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1rem;
  border: 2px solid var(--border-medium);
  border-radius: 0.5rem;
  background: var(--bg-card);
  color: var(--text-main);
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s ease;
}

.demo-btn:hover {
  transform: translateY(-2px);
  box-shadow: var(--shadow-soft);
}

.demo-btn.success {
  border-color: var(--success);
  color: var(--success);
}

.demo-btn.error {
  border-color: var(--error);
  color: var(--error);
}

.demo-btn.warning {
  border-color: var(--warning);
  color: var(--warning);
}

.demo-btn.info {
  border-color: var(--primary);
  color: var(--primary);
}

.demo-btn.danger {
  border-color: var(--error);
  background: var(--error);
  color: white;
}

.demo-btn .icon {
  font-size: 1.125rem;
}

.toast-stats {
  display: flex;
  justify-content: center;
}

.stat-card {
  background: var(--bg-card);
  padding: 1.5rem 2rem;
  border-radius: 0.75rem;
  box-shadow: var(--shadow-soft);
  text-align: center;
  min-width: 200px;
}

.stat-label {
  display: block;
  font-size: 0.875rem;
  color: var(--text-muted);
  margin-bottom: 0.5rem;
}

.stat-value {
  display: block;
  font-size: 2rem;
  font-weight: 800;
  color: var(--primary);
}

@media (max-width: 768px) {
  .demo-header h1 {
    font-size: 2rem;
  }
  
  .button-grid {
    grid-template-columns: 1fr;
  }
  
  .demo-section {
    padding: 1.5rem;
  }
}
</style>
