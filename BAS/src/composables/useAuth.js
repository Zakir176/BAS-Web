import { ref } from 'vue'
import { supabase } from '@/supabase'

const user = ref(null)
const isAuthenticated = ref(false)
const isLoading = ref(false)
const error = ref(null)

export function useAuth() {
  // Initialize auth state
  const init = async () => {
    try {
      isLoading.value = true
      const { data: { session } } = await supabase.auth.getSession()
      user.value = session?.user || null
      isAuthenticated.value = !!session?.user
    } catch (err) {
      error.value = err.message
      console.error('Auth initialization error:', err)
    } finally {
      isLoading.value = false
    }
  }

  // Sign up student
  const signUp = async (email, password, metadata = {}) => {
    try {
      isLoading.value = true
      error.value = null
      
      const { data, error: signUpError } = await supabase.auth.signUp({
        email,
        password,
        options: {
          data: {
            role: 'student',
            ...metadata
          }
        }
      })

      if (signUpError) throw signUpError

      // Store student profile in database (using student_id as primary key)
      if (data.user) {
        const { error: profileError } = await supabase
          .from('students')
          .insert({
            student_id: metadata.student_id,
            full_name: `${metadata.first_name} ${metadata.last_name}`,
            email: data.user.email,
            class_section: metadata.class_section || null,
            qr_code_value: metadata.student_id, // Use student_id as QR code
            created_at: new Date().toISOString()
          })

        if (profileError) throw profileError
      }

      return data
    } catch (err) {
      error.value = err.message
      console.error('Signup error:', err)
      throw err
    } finally {
      isLoading.value = false
    }
  }

  // Sign in (student or teacher)
  const signIn = async (email, password) => {
    try {
      isLoading.value = true
      error.value = null
      
      const { data, error: signInError } = await supabase.auth.signInWithPassword({
        email,
        password
      })

      if (signInError) throw signInError

      user.value = data.user
      isAuthenticated.value = true

      return data
    } catch (err) {
      error.value = err.message
      console.error('Sign in error:', err)
      throw err
    } finally {
      isLoading.value = false
    }
  }

  // Sign out
  const signOut = async () => {
    try {
      isLoading.value = true
      error.value = null
      
      const { error: signOutError } = await supabase.auth.signOut()
      
      if (signOutError) throw signOutError
      
      user.value = null
      isAuthenticated.value = false
    } catch (err) {
      error.value = err.message
      console.error('Sign out error:', err)
      throw err
    } finally {
      isLoading.value = false
    }
  }

  // Reset password
  const resetPassword = async (email) => {
    try {
      isLoading.value = true
      error.value = null
      
      const { error: resetError } = await supabase.auth.resetPasswordForEmail(email)
      
      if (resetError) throw resetError
      
      return true
    } catch (err) {
      error.value = err.message
      console.error('Password reset error:', err)
      throw err
    } finally {
      isLoading.value = false
    }
  }

  // Get current student profile
  const getStudentProfile = async (studentId) => {
    try {
      const { data, error } = await supabase
        .from('students')
        .select('*')
        .eq('student_id', studentId)
        .single()
      
      if (error) throw error
      return data
    } catch (err) {
      console.error('Get student profile error:', err)
      throw err
    }
  }

  // Update student profile
  const updateStudentProfile = async (studentId, updates) => {
    try {
      const { data, error } = await supabase
        .from('students')
        .update(updates)
        .eq('student_id', studentId)
        .select()
        .single()
      
      if (error) throw error
      return data
    } catch (err) {
      console.error('Update student profile error:', err)
      throw err
    }
  }

  // Listen to auth changes
  supabase.auth.onAuthStateChange((event, session) => {
    user.value = session?.user || null
    isAuthenticated.value = !!session?.user
    
    if (event === 'SIGNED_IN') {
      console.log('User signed in:', session.user)
    } else if (event === 'SIGNED_OUT') {
      console.log('User signed out')
    }
  })

  return {
    user,
    isAuthenticated,
    isLoading,
    error,
    init,
    signUp,
    signIn,
    signOut,
    resetPassword,
    getStudentProfile,
    updateStudentProfile
  }
}
