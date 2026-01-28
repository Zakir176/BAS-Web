<template>
  <Modal :is-open="isOpen" @close="$emit('close')">
    <template #header>
      <h3 class="modal-title">Create New Session</h3>
    </template>
    <template #default>
      <form @submit.prevent="createSession">
        <div class="form-group">
          <label for="courseSelect" class="form-label">Select Course</label>
          <select 
            id="courseSelect" 
            v-model="selectedCourseId" 
            class="form-select" 
            required
          >
            <option value="" disabled>-- Please select a course --</option>
            <option v-for="course in courses" :key="course.course_id" :value="course.course_id">
              {{ course.course_name }}
            </option>
          </select>
        </div>
        <div class="form-group">
          <label for="sessionDate" class="form-label">Session Date</label>
          <Input 
            id="sessionDate" 
            v-model="sessionDate" 
            type="date" 
            required 
          />
        </div>
        <div class="form-group">
          <label for="sessionTime" class="form-label">Session Time</label>
          <Input 
            id="sessionTime" 
            v-model="sessionTime" 
            type="time" 
            required 
          />
        </div>
      </form>
    </template>
    <template #footer>
      <Button variant="secondary" @click="$emit('close')">Cancel</Button>
      <Button variant="primary" @click="createSession" :disabled="isCreating">
        {{ isCreating ? 'Creating...' : 'Create Session' }}
      </Button>
    </template>
  </Modal>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import Modal from './ui/Modal.vue'
import Button from './ui/Button.vue'
import Input from './ui/Input.vue'
import { supabase } from '@/supabase'
import { useAuth } from '@/composables/useAuth'

const props = defineProps({
  isOpen: {
    type: Boolean,
    default: false
  }
})

const emit = defineEmits(['close', 'sessionCreated'])

const selectedCourseId = ref('')
const sessionDate = ref('')
const sessionTime = ref('09:00') // Default time
const courses = ref([])
const isCreating = ref(false)
const { user } = useAuth()

const fetchCourses = async () => {
  if (!user.value) return
  const { data, error } = await supabase
    .from('courses')
    .select('course_id, course_name')
    .eq('teacher_id', user.value.id)

  if (error) {
    console.error('Error fetching courses:', error.message)
    return
  }
  courses.value = data
}

const createSession = async () => {
  if (!selectedCourseId.value || !sessionDate.value || !sessionTime.value) {
    alert('Please fill in all session details.')
    return
  }

  isCreating.value = true
  try {
    const { data, error } = await supabase
      .from('sessions')
      .insert({
        course_id: selectedCourseId.value,
        session_date: sessionDate.value,
        session_time: sessionTime.value
      })
      .select()

    if (error) {
      throw error
    }

    if (data && data.length > 0) {
      alert('Session created successfully!')
      emit('sessionCreated', data[0])
      emit('close')
      // Reset form fields
      selectedCourseId.value = ''
      sessionDate.value = ''
      sessionTime.value = '09:00'
    }

  } catch (error) {
    console.error('Error creating session:', error.message)
    alert(`Error creating session: ${error.message}`)
  } finally {
    isCreating.value = false
  }
}

onMounted(() => {
  fetchCourses()
})
</script>

<style scoped>
.form-group {
  margin-bottom: 1.5rem;
}

.form-label {
  display: block;
  font-size: 0.875rem;
  font-weight: 500;
  color: var(--text-primary);
  margin-bottom: 0.5rem;
}

.form-select {
  width: 100%;
  padding: 0.75rem 1rem;
  border: 1px solid var(--border-color);
  border-radius: 0.5rem;
  background-color: var(--bg-secondary);
  color: var(--text-primary);
  font-size: 1rem;
  transition: border-color 0.2s;
  -webkit-appearance: none; /* Remove default arrow on WebKit */
  -moz-appearance: none;    /* Remove default arrow on Firefox */
  appearance: none;         /* Remove default arrow */
  background-image: url('data:image/svg+xml;utf8,<svg fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg"><path fill-rule="evenodd" d="M5.293 7.293a1 1 0 011.414 0L10 10.586l3.293-3.293a1 1 0 111.414 1.414l-4 4a1 1 0 01-1.414 0l-4-4a1 1 0 010-1.414z" clip-rule="evenodd"></path></svg>');
  background-repeat: no-repeat;
  background-position: right 0.75rem center;
  background-size: 1.5rem;
}

.form-select:focus {
  border-color: var(--accent-primary);
  outline: none;
}

.modal-title {
  color: var(--text-primary);
}
</style>