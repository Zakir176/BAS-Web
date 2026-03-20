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
          <label for="courseCode" class="form-label">Course Code</label>
          <Input 
            id="courseCode" 
            v-model="courseCode" 
            type="text" 
            placeholder="e.g., CS101" 
            required 
          />
        </div>
        <div class="form-group">
          <label for="departmentSelect" class="form-label">Department</label>
          <select 
            id="departmentSelect" 
            v-model="selectedDepartmentId" 
            class="form-select" 
            required
          >
            <option value="" disabled>-- Select Department --</option>
            <option v-for="dept in departments" :key="dept.id" :value="dept.id">
              {{ dept.name }}
            </option>
          </select>
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
import { ref, onMounted } from 'vue'
import Modal from '@/components/ui/Modal.vue'
import Button from '@/components/ui/Button.vue'
import Input from '@/components/ui/Input.vue'
import { supabase } from '@/supabase'
import { useAuth } from '@/composables/useAuth'
import { useToast } from '@/composables/useToast'

const props = defineProps({
  isOpen: {
    type: Boolean,
    default: false
  }
})

const emit = defineEmits(['close', 'courseCreated'])

const courseName = ref('')
const courseCode = ref('')
const selectedDepartmentId = ref('')
const departments = ref([])
const isCreating = ref(false)
const { user } = useAuth()
const { toast } = useToast()

const fetchDepartments = async () => {
  console.log('Fetching departments (Modal)...')
  try {
    const { data, error } = await supabase.from('departments').select('*')
    if (error) {
      console.error('Error fetching departments:', error)
      departments.value = [
        { id: '1', name: 'Computer Science' },
        { id: '2', name: 'Information Technology' }
      ]
    } else {
      departments.value = data || []
    }
  } catch (error) {
    console.error('Error fetching departments:', error)
  }
}

const createCourse = async () => {
  if (!courseName.value.trim()) {
    toast.error('Course name cannot be empty.')
    return
  }
  if (!courseCode.value.trim()) {
    toast.error('Course code cannot be empty.')
    return
  }

  isCreating.value = true
  try {
    const { data, error } = await supabase
      .from('courses')
      .insert({
        name: courseName.value,
        code: courseCode.value,
        department_id: selectedDepartmentId.value,
        lecturer_id: user.value.id
      })
      .select()

    if (error) {
      throw error
    }

    if (data && data.length > 0) {
      toast.success('Course created successfully!')
      emit('courseCreated', data[0])
      emit('close')
      // Reset form fields
      courseName.value = ''
      courseCode.value = ''
      selectedDepartmentId.value = ''
    }

  } catch (error) {
    console.error('Error creating course:', error.message)
    toast.error(`Error creating course: ${error.message}`)
  } finally {
    isCreating.value = false
  }
}

onMounted(fetchDepartments)
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