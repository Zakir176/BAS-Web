import { ref, onMounted } from 'vue'
import { supabase } from '@/supabase'

// Reactive state for auth
const user = ref(null)
const isAuthenticated = ref(false)
const isLoading = ref(false)
const error = ref(null)
const role = ref(null)
const isInitialized = ref(false)
let initPromise = null

export function useAuth() {
  // Centralized error handling
  const handleError = (err, message) => {
    error.value = err ? err.message : message
    console.error('Auth Error Details:', {
      message: err?.message,
      details: err?.details,
      hint: err?.hint,
      code: err?.code
    })
    console.error(message, err)
    isLoading.value = false
    // Propagate the actual error message so the UI can display it in the toast
    // If it's a Supabase error (err.message), use it. Otherwise fall back to the generic message.
    throw new Error(err?.message || message)
  }

  // Set user state
  const setUser = (sessionUser) => {
    if (sessionUser) {
      let rawRole = sessionUser.user_metadata?.role || 'student'
      // Normalize 'teacher' to 'lecturer' for frontend consistency
      const normalizedRole = rawRole === 'teacher' || rawRole === 'admin' ? 'lecturer' : rawRole

      user.value = {
        id: sessionUser.id,
        email: sessionUser.email,
        role: normalizedRole,
        ...sessionUser.user_metadata
      }
      isAuthenticated.value = true
      role.value = normalizedRole
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
      const sanitizedEmail = email.trim()

      const { data, error: signUpError } = await supabase.auth.signUp({
        email: sanitizedEmail,
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
      if (!data.user) throw new Error('Signup succeeded but no user data returned')

      // 2. Create a corresponding profile in the database using the Auth ID
      const userId = data.user.id

      if (metadata.role === 'lecturer') {
        // Create lecturer profile using the Auth UUID
        const { error: profileError } = await supabase.from('teachers').insert({
          teacher_id: userId,
          full_name: metadata.full_name,
          email: sanitizedEmail,
          role: 'teacher' // Database constraint only allows 'teacher' or 'admin'
        });

        if (profileError) throw profileError;
      } else {
        // Create student profile using the Auth UUID
        // Note: We use the Auth UUID as the primary key to ensure RLS works
        const { error: profileError } = await supabase.from('students').insert({
          student_id: metadata.student_id,
          full_name: metadata.full_name,
          email: sanitizedEmail,
          class_section: metadata.class_section,
          qr_code_value: metadata.student_id
        });

        if (profileError) throw profileError;
      }

      return data

    } catch (err) {
      handleError(err, 'Signup failed.')
    } finally {
      isLoading.value = false
    }
  }

  // Track ongoing repairs to avoid race conditions
  const ongoingRepairs = new Set()

  // Ensure teacher profile exists (Account Repair)
  // ONLY runs if the user is NOT a student (or explicit intent to be teacher)
  const ensureTeacherProfile = async (authUser) => {
    if (!authUser || ongoingRepairs.has(authUser.id)) return null

    // Safety check: Never run teacher repair for student accounts
    const role = authUser.user_metadata?.role
    if (role === 'student') return null

    ongoingRepairs.add(authUser.id)

    try {
      // 1. Check if profile exists and matches
      const { data: currentProfile } = await supabase
        .from('teachers')
        .select('teacher_id, role')
        .eq('teacher_id', authUser.id)
        .maybeSingle()

      if (currentProfile) {
        return currentProfile.role === 'admin' ? 'admin' : 'lecturer'
      }

      // 2. Profile missing or ID mismatch - Perform robust upsert by email
      const { error: repairError } = await supabase
        .from('teachers')
        .upsert({
          teacher_id: authUser.id,
          email: authUser.email,
          full_name: authUser.user_metadata?.full_name || authUser.email.split('@')[0],
          role: 'teacher' // Default role for new profiles
        }, {
          onConflict: 'email',
          ignoreDuplicates: false
        })

      if (!repairError) {
        // Sync role to Auth metadata for UI consistency
        await supabase.auth.updateUser({ data: { role: 'lecturer' } })
        return 'lecturer'
      } else {
        console.error('Auth: Account repair failed:', repairError)
      }
    } catch (err) {
      console.error('Auth: Repair logic exception:', err)
    } finally {
      ongoingRepairs.delete(authUser.id)
    }
    return null
  }

  // Secure Sign In with Supabase Auth
  const signIn = async (email, password) => {
    try {
      isLoading.value = true
      error.value = null
      const sanitizedEmail = email.trim()

      const { data, error: signInError } = await supabase.auth.signInWithPassword({
        email: sanitizedEmail,
        password
      })

      if (signInError) {
        if (signInError.message === 'Invalid login credentials') {
          throw new Error('Invalid email or password. Please check your credentials and try again.')
        }
        throw signInError
      }

      // Account Repair & Role Discovery
      const detectedRole = await ensureTeacherProfile(data.user)

      if (detectedRole) {
        // Re-fetch user to get updated metadata if we just synced it
        const { data: { user: updatedUser } } = await supabase.auth.getUser()
        setUser(updatedUser)
        return { ...data, user: updatedUser }
      }

      setUser(data.user)
      return data

    } catch (err) {
      handleError(err, err.message || 'Sign in failed. Check your credentials.')
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

  // Reset password - sends email
  const resetPassword = async (email) => {
    try {
      isLoading.value = true
      error.value = null

      const { error: resetError } = await supabase.auth.resetPasswordForEmail(email.trim(), {
        redirectTo: `${window.location.origin}/reset-password`
      })

      if (resetError) throw resetError

      return true
    } catch (err) {
      handleError(err, 'Password reset request failed.')
    } finally {
      isLoading.value = false
    }
  }

  // Update password - for use after following reset link
  const updatePassword = async (newPassword) => {
    try {
      isLoading.value = true
      error.value = null

      const { error: updateError } = await supabase.auth.updateUser({
        password: newPassword
      })

      if (updateError) throw updateError

      return true
    } catch (err) {
      handleError(err, 'Password update failed.')
    } finally {
      isLoading.value = false
    }
  }


  const init = async () => {
    if (initPromise) return initPromise


    // Set up auth state change listener once
    supabase.auth.onAuthStateChange(async (event, session) => {
      if (session?.user) {
        // Run profile repair in background
        ensureTeacherProfile(session.user)
      }
      setUser(session?.user || null)
      isInitialized.value = true
    })

    initPromise = (async () => {
      try {
        // Use a timeout for getSession to prevent hangs
        const sessionPromise = supabase.auth.getSession()
        const timeoutPromise = new Promise((resolve) =>
          setTimeout(() => resolve({ data: { session: null }, error: new Error('Timeout') }), 3000)
        )

        const result = await Promise.race([sessionPromise, timeoutPromise])
        const session = result?.data?.session

        if (session?.user) {
          await ensureTeacherProfile(session.user)
          setUser(session.user)
        } else {
          setUser(null)
        }
      } catch (err) {
        console.warn('Auth: Initial check failed:', err)
        setUser(null)
      } finally {
        isInitialized.value = true
      }
    })()

    return initPromise
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
    updatePassword,
    getStudentProfile,
    updateStudentProfile,
    updateLecturerProfile,
    init,
    isInitialized
  }
}
