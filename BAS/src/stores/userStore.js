import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { supabase } from '@/supabase'

export const useUserStore = defineStore('user', () => {
    // STATE
    const user = ref(null)
    const profile = ref(null)
    const role = ref(null)
    const isLoading = ref(false)
    const error = ref(null)
    const isInitialized = ref(false)

    // GETTERS
    const isAuthenticated = computed(() => !!user.value)

    // Internal logic (not exposed)
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
        // Re-throw the error to be caught by the UI
        throw new Error(err?.message || message)
    }

    // ACTIONS
    async function fetchProfile() {
        if (!user.value) return

        isLoading.value = true
        try {
            // Role is now set directly on the user object, no need to guess
            const currentRole = user.value.role || 'student'
            const table = currentRole === 'lecturer' ? 'teachers' : 'students'

            const { data, error: fetchError } = await supabase
                .from(table)
                .select('*')
                .eq('id', user.value.id) // Use the user ID from the user object
                .single()

            if (fetchError) throw fetchError

            profile.value = data
        } catch (err) {
            handleError(err, 'Error fetching profile.')
        } finally {
            isLoading.value = false
        }
    }

    function setUser(sessionUser) {
        if (sessionUser) {
            let rawRole = sessionUser.user_metadata?.role || 'student'
            const normalizedRole = rawRole === 'teacher' || rawRole === 'admin' ? 'lecturer' : rawRole

            user.value = {
                id: sessionUser.id,
                email: sessionUser.email,
                role: normalizedRole,
                ...sessionUser.user_metadata
            }
            role.value = normalizedRole
            fetchProfile() // Fetch profile whenever user is set
        } else {
            user.value = null
            profile.value = null
            role.value = null
        }
    }

    let initPromise = null
    async function initialize() {
        if (initPromise) return initPromise

        // Set up listener once
        supabase.auth.onAuthStateChange((_event, session) => {
            setUser(session?.user || null)
            isInitialized.value = true
        })

        initPromise = (async () => {
            try {
                const { data: { session }, error: sessionError } = await supabase.auth.getSession()
                if (sessionError) throw sessionError;
                
                setUser(session?.user || null)
            } catch (err) {
                handleError(err, 'Failed to initialize session.')
            } finally {
                isInitialized.value = true
            }
        })()

        return initPromise
    }


    return {
        user,
        profile,
        role,
        isLoading,
        error,
        isInitialized,
        isAuthenticated,
        setUser,
        initialize,
        fetchProfile,
        handleError // Exposing for use in the auth composable
    }
})
