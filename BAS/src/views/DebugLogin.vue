<template>
  <div class="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 flex items-center justify-center p-4">
    <div class="w-full max-w-md">
      <div class="bg-white rounded-2xl shadow-xl p-8">
        <h1 class="text-2xl font-bold text-gray-900 mb-6">Debug Login</h1>
        
        <!-- Debug Info -->
        <div class="mb-6 p-4 bg-gray-100 rounded-lg">
          <h3 class="font-semibold mb-2">Debug Info:</h3>
          <p class="text-sm">Is Authenticated: {{ isAuthenticated }}</p>
          <p class="text-sm">User: {{ user ? JSON.stringify(user) : 'null' }}</p>
          <p class="text-sm">Role: {{ role }}</p>
          <p class="text-sm">Loading: {{ isLoading }}</p>
          <p class="text-sm text-red-600">Error: {{ error }}</p>
        </div>

        <!-- Test Login Form -->
        <form @submit.prevent="testLogin" class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Email</label>
            <input 
              v-model="testEmail" 
              type="email" 
              class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              placeholder="Enter email"
            />
          </div>
          
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Password</label>
            <input 
              v-model="testPassword" 
              type="password" 
              class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              placeholder="Enter password"
            />
          </div>

          <button 
            type="submit" 
            :disabled="isLoading"
            class="w-full bg-blue-600 text-white py-2 px-4 rounded-lg hover:bg-blue-700 disabled:opacity-50"
          >
            {{ isLoading ? 'Testing...' : 'Test Login' }}
          </button>
        </form>

        <!-- Quick Actions -->
        <div class="mt-6 space-y-2">
          <button 
            @click="checkSession" 
            class="w-full bg-green-600 text-white py-2 px-4 rounded-lg hover:bg-green-700"
          >
            Check Current Session
          </button>
          
          <button 
            @click="signOut" 
            class="w-full bg-red-600 text-white py-2 px-4 rounded-lg hover:bg-red-700"
          >
            Sign Out
          </button>
        </div>

        <!-- Test Account Info -->
        <div class="mt-6 p-4 bg-blue-50 rounded-lg">
          <h3 class="font-semibold mb-2">Test Account:</h3>
          <p class="text-sm">Email: test@lecturer.com</p>
          <p class="text-sm">Password: password123</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useAuth } from '@/composables/useAuth'

const { user, isAuthenticated, role, isLoading, error, signIn, signOut, init } = useAuth()

const testEmail = ref('test@lecturer.com')
const testPassword = ref('password123')

const testLogin = async () => {
  try {
    console.log('Testing login with:', testEmail.value)
    await signIn(testEmail.value, testPassword.value)
    console.log('Login successful!')
  } catch (err) {
    console.error('Login failed:', err)
  }
}

const checkSession = async () => {
  try {
    await init()
    console.log('Session checked')
  } catch (err) {
    console.error('Session check failed:', err)
  }
}
</script>
