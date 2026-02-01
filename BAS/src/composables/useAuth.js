import { storeToRefs } from 'pinia'
import { supabase } from '@/supabase'
import { useUserStore } from '@/stores/userStore'

export function useAuth() {
  const store = useUserStore()
  const {
    user,
    isAuthenticated,
    isLoading,
    error,
    role,
    isInitialized
  } = storeToRefs(store)

  // Secure Sign Up with Supabase Auth
  const signUp = async (email, password, metadata = {}) => {
    try {
      store.isLoading = true
      store.error = null
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

      const userId = data.user.id
      if (metadata.role === 'lecturer') {
        const { error: profileError } = await supabase.from('teachers').insert({
          teacher_id: userId,
          full_name: metadata.full_name,
          email: sanitizedEmail,
          role: 'teacher'
        });
        if (profileError) throw profileError;
      } else {
        const { error: profileError } = await supabase.from('students').insert({
          student_id: metadata.student_id,
          full_name: metadata.full_name,
          email: sanitizedEmail,
          class_section: metadata.class_section,
          qr_code_value: metadata.student_id
        });
        if (profileError) {
          if (profileError.code === '23505') {
            throw new Error('This Student ID or Email is already registered. Please Log In.')
          }
          throw profileError
        }
      }
      return data
    } catch (err) {
      store.handleError(err, 'Signup failed.')
    } finally {
      store.isLoading = false
    }
  }

  const ongoingRepairs = new Set()
  const ensureTeacherProfile = async (authUser) => {
    if (!authUser || ongoingRepairs.has(authUser.id)) return null
    const userRole = authUser.user_metadata?.role
    if (userRole === 'student') return null
    ongoingRepairs.add(authUser.id)
    try {
      const { data: currentProfile } = await supabase
        .from('teachers')
        .select('teacher_id, role')
        .eq('teacher_id', authUser.id)
        .maybeSingle()
      if (currentProfile) {
        return currentProfile.role === 'admin' ? 'admin' : 'lecturer'
      }
      const { error: repairError } = await supabase.from('teachers').upsert({
        teacher_id: authUser.id,
        email: authUser.email,
        full_name: authUser.user_metadata?.full_name || authUser.email.split('@')[0],
        role: 'teacher'
      }, { onConflict: 'email', ignoreDuplicates: false })
      if (!repairError) {
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
      store.isLoading = true
      store.error = null
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

      const detectedRole = await ensureTeacherProfile(data.user)
      if (detectedRole) {
        const { data: { user: updatedUser } } = await supabase.auth.getUser()
        store.setUser(updatedUser) // Use store action
        return { ...data, user: updatedUser }
      }

      store.setUser(data.user) // Use store action
      return data
    } catch (err) {
      store.handleError(err, err.message || 'Sign in failed. Check your credentials.')
    } finally {
      store.isLoading = false
    }
  }

  // Secure Sign Out with Supabase Auth
  const signOut = async () => {
    try {
      store.isLoading = true
      store.error = null
      const { error: signOutError } = await supabase.auth.signOut()
      if (signOutError) throw signOutError
      store.setUser(null) // Use store action
    } catch (err) {
      store.handleError(err, 'Sign out failed.')
    } finally {
      store.isLoading = false
    }
  }

  // Reset password - sends email
  const resetPassword = async (email) => {
    try {
      store.isLoading = true
      store.error = null
      const { error: resetError } = await supabase.auth.resetPasswordForEmail(email.trim(), {
        redirectTo: `${window.location.origin}/reset-password`
      })
      if (resetError) throw resetError
      return true
    } catch (err) {
      store.handleError(err, 'Password reset request failed.')
    } finally {
      store.isLoading = false
    }
  }

  // Update password
  const updatePassword = async (newPassword) => {
    try {
      store.isLoading = true
      store.error = null
      const { error: updateError } = await supabase.auth.updateUser({ password: newPassword })
      if (updateError) throw updateError
      return true
    } catch (err) {
      store.handleError(err, 'Password update failed.')
    } finally {
      store.isLoading = false
    }
  }

  // Profile-related functions remain here as they are direct DB interactions
  const getStudentProfile = async (studentId) => {
    // ... implementation ...
  }
  const getLecturerProfile = async (teacherId) => {
    // ... implementation ...
  }
  const updateStudentProfile = async (studentId, updates) => {
    // ... implementation ...
  }
  const updateLecturerProfile = async (teacherId, updates) => {
    // ... implementation ...
  }

  return {
    // State and getters from store
    user,
    isAuthenticated,
    isLoading,
    error,
    role,
    isInitialized,

    // Actions from store
    initialize: store.initialize,

    // Auth functions
    signUp,
    signIn,
    signOut,
    resetPassword,
    updatePassword,

    // Profile functions
    getStudentProfile,
    updateStudentProfile,
    updateLecturerProfile,
  }
}
