import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { supabase } from '@/supabase'

export const useUserStore = defineStore('user', () => {
    const user = ref(null)
    const profile = ref(null)
    const role = ref(null)
    const isLoading = ref(false)

    const isAuthenticated = computed(() => !!user.value)

    async function fetchProfile() {
        if (!user.value) return

        isLoading.value = true
        try {
            const currentRole = user.value.user_metadata?.role || 'student'
            const table = currentRole === 'lecturer' ? 'teachers' : 'students'
            const idField = currentRole === 'lecturer' ? 'teacher_id' : 'student_id'

            // For lecturers we use auth.uid(), for students we use email in our RLS logic
            // But for the query, we can use the ID from metadata if available
            const queryField = currentRole === 'lecturer' ? 'teacher_id' : 'email'
            const queryValue = currentRole === 'lecturer' ? user.value.id : user.value.email

            const { data, error } = await supabase
                .from(table)
                .select('*')
                .eq(queryField, queryValue)
                .single()

            if (error) throw error

            profile.value = data
            role.value = currentRole
        } catch (error) {
            console.error('Error fetching profile:', error)
            profile.value = null
        } finally {
            isLoading.value = false
        }
    }

    function setUser(newUser) {
        user.value = newUser
        if (newUser) {
            fetchProfile()
        } else {
            profile.value = null
            role.value = null
        }
    }

    async function initialize() {
        const { data: { session } } = await supabase.auth.getSession()
        setUser(session?.user || null)

        supabase.auth.onAuthStateChange((_event, session) => {
            setUser(session?.user || null)
        })
    }

    return {
        user,
        profile,
        role,
        isLoading,
        isAuthenticated,
        setUser,
        initialize,
        fetchProfile
    }
})
