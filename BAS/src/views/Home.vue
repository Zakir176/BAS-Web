<template>
  <div class="university-homepage">
    <Navbar />
    
    <main>
      <HeroSection 
        :scroll-y="scrollY"
        :is-loading="isLoading"
        :stats="stats"
        :is-authenticated="isAuthenticated"
        @dashboard="goToDashboard"
        @student-login="goToStudentLogin"
        @lecturer-login="goToLecturerLogin"
      />

      <FeatureGrid :is-loading="isLoading" />

      <WorkflowSteps />

      <CTASection 
        :is-authenticated="isAuthenticated"
        @student-signup="goToStudentSignup"
        @lecturer-signup="goToLecturerSignup"
        @dashboard="goToDashboard"
      />
    </main>
    
    <HomeFooter 
      :is-loading="isLoading"
      :stats="stats"
    />
    
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { supabase } from '@/supabase'
import { useAuth } from '@/composables/useAuth'
import Navbar from '@/components/common/Navbar.vue'
import Button from '@/components/ui/Button.vue'
import Skeleton from '@/components/ui/Skeleton.vue'
import HeroSection from '@/components/home/HeroSection.vue'
import FeatureGrid from '@/components/home/FeatureGrid.vue'
import WorkflowSteps from '@/components/home/WorkflowSteps.vue'
import CTASection from '@/components/home/CTASection.vue'
import HomeFooter from '@/components/home/HomeFooter.vue'

const router = useRouter()
const { isAuthenticated, user, role } = useAuth()

const isLoading = ref(true)
const scrollY = ref(0)
const handleScroll = () => {
  scrollY.value = window.scrollY
}

const stats = ref({
  students: 0,
  courses: 0,
  sessions: 0,
  accuracy: '99.9%'
})

const fetchStats = async () => {
  try {
    const [
      { count: studentCount },
      { count: courseCount },
      { count: sessionCount }
    ] = await Promise.all([
      supabase.from('students').select('*', { count: 'exact', head: true }),
      supabase.from('courses').select('*', { count: 'exact', head: true }),
      supabase.from('sessions').select('*', { count: 'exact', head: true })
    ])

    stats.value.students = studentCount || 0
    stats.value.courses = courseCount || 0
    stats.value.sessions = sessionCount || 0
  } catch (err) {
    console.error('Error fetching global stats:', err)
  } finally {
    isLoading.value = false
  }
}

const goToDashboard = () => {
  const userRole = role.value || user.value?.user_metadata?.role || 'student'
  if (userRole === 'lecturer' || userRole === 'teacher') {
    router.push('/lecturer-dashboard')
  } else {
    router.push('/student-homepage')
  }
}

const goToStudentLogin = () => router.push('/student-login')
const goToLecturerLogin = () => router.push('/lecturer-login')
const goToStudentSignup = () => router.push('/student-signup')
const goToLecturerSignup = () => router.push('/lecturer-signup')

onMounted(() => {
  fetchStats()
  window.addEventListener('scroll', handleScroll, { passive: true })
  
  // Intersection Observer for scroll-reveal animations
  const observer = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
      if (entry.isIntersecting) {
        entry.target.classList.add('is-visible')
      }
    })
  }, { 
    threshold: 0.15,
    rootMargin: '0px 0px -50px 0px'
  })

  document.querySelectorAll('.reveal-on-scroll').forEach(el => observer.observe(el))
})

onUnmounted(() => {
  window.removeEventListener('scroll', handleScroll)
})
</script>

<style scoped>
.university-homepage {
  background-color: var(--bg-main);
  min-height: 100vh;
  color: var(--text-main);
}
</style>
