import { ref, onMounted } from 'vue'
import { supabase } from '@/supabase'

// Reactive state for auth
const user = ref(null)
const isAuthenticated = ref(false)
const isLoading = ref(false)
const error = ref(null)
const role = ref(null)

export function useAuth() {
  // Centralized error handling
  const handleError = (err, message) => {
    error.value = err ? err.message : message
    console.error(message, err)
    isLoading.value = false
    throw new Error(message)
  }

  // Set user state
  const setUser = (sessionUser) => {
    if (sessionUser) {
      user.value = {
        id: sessionUser.id,
        email: sessionUser.email,
        role: sessionUser.user_metadata?.role || 'student',
        ...sessionUser.user_metadata
      }
      isAuthenticated.value = true
      role.value = sessionUser.user_metadata?.role || 'student'
    } else {
      user.value = null
      isAuthenticated.value = false
      role.value = null
    }
  }

  // Secure Sign Up with Supabase Auth
  const signUp = async (email, password, metadata = {}) => {
    try {
      isLoading.value = true
      error.value = null

      const { data, error: signUpError } = await supabase.auth.signUp({
        email,
        password,
        options: {
          data: {
            full_name: `${metadata.first_name} ${metadata.last_name}`,
            role: metadata.role || 'student',
            ...metadata
          }
        }
      })

      if (signUpError) throw signUpError

      // 2. Create a corresponding student profile in the public.students table
      // This is a critical step that was missing.
      const { error: profileError } = await supabase.from('students').insert({
        student_id: metadata.student_id,
        full_name: metadata.full_name,
        email: email, // Use the email from the form
        class_section: metadata.class_section,
        // The password should NOT be stored here. Supabase auth handles it.
        // We are intentionally leaving the 'password' column empty.
        // It's recommended to remove that column from the 'students' table.
        qr_code_value: metadata.student_id // Using student_id as QR value
      });

      if (profileError) {
        // If profile creation fails, we should ideally delete the auth user
        // to avoid orphaned auth entries. This is a more advanced transactional
        // approach, but for now, we'll just throw the error.
        throw profileError;
      }
      
      // The user is signed up but might need to confirm their email.
      // The onAuthStateChange listener will handle the session once logged in.
      return data

    } catch (err) {
      handleError(err, 'Signup failed.')
    } finally {
      isLoading.value = false
    }
  }

  // Secure Sign In with Supabase Auth
  const signIn = async (email, password) => {
    try {
      isLoading.value = true
      error.value = null

      const { data, error: signInError } = await supabase.auth.signInWithPassword({
        email,
        password
      })

      if (signInError) throw signInError

      // The onAuthStateChange listener will automatically set the user state
      setUser(data.user)
      return data

    } catch (err) {
      handleError(err, 'Sign in failed. Check your credentials.')
    } finally {
      isLoading.value = false
    }
  }

  // Secure Sign Out with Supabase Auth
  const signOut = async () => {
    try {
      isLoading.value = true
      error.value = null
      
      const { error: signOutError } = await supabase.auth.signOut()

      if (signOutError) throw signOutError
      
      // The onAuthStateChange listener will clear the user state
      setUser(null)
      
    } catch (err) {
      handleError(err, 'Sign out failed.')
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
      handleError(err, 'Password reset failed.')
    } finally {
      isLoading.value = false
    }
  }
  
  const init = async () => {
    supabase.auth.onAuthStateChange((_event, session) => {
      setUser(session?.user || null)
      isLoading.value = false
    })

    // Also check for the initial session on component mount
    const { data: { session } } = await supabase.auth.getSession()
    setUser(session?.user || null)
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

  // Get current lecturer profile
  const getLecturerProfile = async (teacherId) => {
    try {
      const { data, error } = await supabase
        .from('teachers')
        .select('*')
        .eq('teacher_id', teacherId)
        .single()
      
      if (error) throw error
      return data
    } catch (err) {
      console.error('Get lecturer profile error:', err)
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

  // Update lecturer profile
  const updateLecturerProfile = async (teacherId, updates) => {
    try {
      const { data, error } = await supabase
        .from('teachers')
        .update(updates)
        .eq('teacher_id', teacherId)
        .select()
        .single()
      
      if (error) throw error
      return data
    } catch (err) {
      console.error('Update lecturer profile error:', err)
      throw err
    }
  }


  return {
    user,
    isAuthenticated,
    isLoading,
    error,
    role,
    signUp,
    signIn,
    signOut,
    resetPassword,
    getStudentProfile,
    updateStudentProfile,
    getLecturerProfile,
    updateLecturerProfile,
    init
  }
}
