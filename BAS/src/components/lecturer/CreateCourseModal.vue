<template>
  <Modal :is-open="isOpen" @close="$emit('close')">
    <template #header>
      <h3 class="modal-title">Create New Course</h3>
    </template>
    <template #default>
      <form @submit.prevent="createCourse">
        <div class="form-group">
          <label for="courseName" class="form-label">Course Name</label>
          <Input 
            id="courseName" 
            v-model="courseName" 
            type="text" 
            placeholder="e.g., Introduction to Programming" 
            required 
          />
        </div>
        <div class="form-group">
          <label for="maxAbsences" class="form-label">Max Absences Allowed</label>
          <Input 
            id="maxAbsences" 
            v-model.number="maxAbsencesAllowed" 
            type="number" 
            placeholder="e.g., 3" 
            min="0"
          />
        </div>
      </form>
    </template>
    <template #footer>
      <Button variant="secondary" @click="$emit('close')">Cancel</Button>
      <Button variant="primary" @click="createCourse" :disabled="isCreating">
        {{ isCreating ? 'Creating...' : 'Create Course' }}
      </Button>
    </template>
  </Modal>
</template>

<script setup>
import { ref } from 'vue'
import Modal from '@/components/ui/Modal.vue'
import Button from '@/components/ui/Button.vue'
import Input from '@/components/ui/Input.vue'
import { supabase } from '@/supabase'
import { useAuth } from '@/composables/useAuth'

const props = defineProps({
  isOpen: {
    type: Boolean,
    default: false
  }
})

const emit = defineEmits(['close', 'courseCreated'])

const courseName = ref('')
const maxAbsencesAllowed = ref(0)
const isCreating = ref(false)
const { user } = useAuth()

const createCourse = async () => {
  if (!courseName.value.trim()) {
    alert('Course name cannot be empty.')
    return
  }
  if (maxAbsencesAllowed.value < 0) {
    alert('Maximum absences cannot be negative.')
    return
  }

  isCreating.value = true
  try {
    const { data, error } = await supabase
      .from('courses')
      .insert({
        course_name: courseName.value,
        max_absences_allowed: maxAbsencesAllowed.value,
        teacher_id: user.value.id // Assuming teacher_id is tied to the logged-in user
      })
      .select()

    if (error) {
      throw error
    }

    if (data && data.length > 0) {
      alert('Course created successfully!')
      emit('courseCreated', data[0])
      emit('close')
      // Reset form fields
      courseName.value = ''
      maxAbsencesAllowed.value = 0
    }

  } catch (error) {
    console.error('Error creating course:', error.message)
    alert(`Error creating course: ${error.message}`)
  } finally {
    isCreating.value = false
  }
}
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

.modal-title {
  color: var(--text-primary);
}
</style>