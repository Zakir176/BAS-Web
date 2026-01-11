<script setup>
import { ref, onMounted } from 'vue'
import { supabase } from '../supabase' // Assuming your supabase client is exported from here
import ExcelJS from 'exceljs'

// Reactive state variables
const students = ref([])
const loading = ref(false)
const isDragover = ref(false)
const fileInput = ref(null) // To trigger file input click
const feedback = ref({ type: '', message: '', details: '' })

// Fetches the current list of students from the database
const fetchStudents = async () => {
  try {
    loading.value = true
    const { data, error } = await supabase
      .from('students')
      .select('*')
      .order('full_name', { ascending: true })

    if (error) throw error
    students.value = data
  } catch (error) {
    feedback.value = {
      type: 'error',
      message: 'Failed to fetch students.',
      details: error.message
    }
  } finally {
    loading.value = false
  }
}

// Lifecycle hook to fetch students when the component is first mounted
onMounted(() => {
  fetchStudents()
})

// Triggers the hidden file input when the dropzone is clicked
const triggerFileInput = () => {
  fileInput.value.click()
}

// Handles file selection from the file input dialog
const handleFileChange = (event) => {
  const files = event.target.files
  if (files.length > 0) {
    processFile(files[0])
  }
}

// Handles file drop event
const onDrop = (event) => {
  isDragover.value = false
  const files = event.dataTransfer.files
  if (files.length > 0) {
    processFile(files[0])
  }
}

// Main function to process the uploaded file
const processFile = (file) => {
  feedback.value = { type: '', message: '' } // Reset feedback
  const validTypes = ['application/vnd.ms-excel', 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet']
  if (!validTypes.includes(file.type)) {
    feedback.value = {
      type: 'error',
      message: 'Invalid file type. Please upload an .xlsx or .xls file.'
    }
    return
  }

  const reader = new FileReader()
  reader.onload = async (e) => {
    try {
      loading.value = true
      const workbook = new ExcelJS.Workbook()
      await workbook.xlsx.load(e.target.result)
      const worksheet = workbook.getWorksheet(1) // Get the first worksheet

      const studentData = []
      // Iterate over all rows that have values in a worksheet
      worksheet.eachRow({ includeEmpty: false }, (row, rowNumber) => {
        // Skip header row
        if (rowNumber > 1) {
          const student = {
            student_id: row.values[1],
            full_name: row.values[2],
            class_section: row.values[3] || null,
            qr_code_value: row.values[1] // Use student_id as qr_code_value
          }
          if (student.student_id) { // Filter out rows without a student_id
            studentData.push(student)
          }
        }
      })

      if (studentData.length === 0) {
        feedback.value = {
          type: 'warning',
          message: 'No valid student data found in the file. Make sure student_id is present.'
        }
        return
      }

      // Upsert data into Supabase
      const { error: upsertError } = await supabase
        .from('students')
        .upsert(studentData, { onConflict: 'student_id' })

      if (upsertError) throw upsertError

      feedback.value = {
        type: 'success',
        message: `Successfully imported ${studentData.length} students.`
      }
      
      // Refresh the student list to show the new data
      await fetchStudents()

    } catch (error) {
      feedback.value = {
        type: 'error',
        message: 'An error occurred while processing the file.',
        details: error.message
      }
    } finally {
      loading.value = false
      // Reset file input to allow re-uploading the same file
      if (fileInput.value) fileInput.value.value = ''
    }
  }
  reader.readAsArrayBuffer(file)
}
</script>

<template>
  <div class="max-w-4xl mx-auto p-4 sm:p-6 lg:p-8 font-sans">
    
    <!-- Component Title -->
    <h1 class="text-2xl sm:text-3xl font-bold text-gray-800 mb-6">Manage Students</h1>

    <!-- File Upload Section -->
    <div class="bg-white p-6 rounded-lg shadow-md border border-gray-200 mb-8">
      <h2 class="text-xl font-semibold text-gray-700 mb-4">Upload Student List</h2>
      
      <!-- Drag-and-Drop Area -->
      <div
        @click="triggerFileInput"
        @dragover.prevent="isDragover = true"
        @dragleave.prevent="isDragover = false"
        @drop.prevent="onDrop"
        :class="[
          'flex justify-center items-center flex-col w-full h-48 px-4 transition bg-white border-2 border-gray-300 border-dashed rounded-md appearance-none cursor-pointer hover:border-indigo-600 focus:outline-none',
          { 'border-indigo-600 bg-indigo-50': isDragover }
        ]"
      >
        <div class="flex flex-col items-center space-y-2 text-center">
          <svg class="w-12 h-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 16a4 4 0 01-4-4V7a4 4 0 014-4h4a4 4 0 014 4v5m-4 4h4a4 4 0 004-4V7m-7 9v6m-3-3h6"></path></svg>
          <p class="text-sm text-gray-600">
            Drag and drop your .xlsx or .xls file here, or
            <span class="font-semibold text-indigo-600">click to browse</span>.
          </p>
          <p class="text-xs text-gray-500">
            Columns: student_id, full_name, class_section (optional)
          </p>
        </div>
        
        <!-- Hidden File Input -->
        <input
          ref="fileInput"
          type="file"
          @change="handleFileChange"
          class="hidden"
          accept=".xlsx, .xls"
        />
      </div>

      <!-- Loading and Feedback Messages -->
      <div v-if="loading" class="mt-4 text-center text-sm text-gray-600">
        Processing file...
      </div>
      
      <div v-if="feedback.message" class="mt-4 p-4 rounded-md text-sm" :class="{
        'bg-green-100 text-green-800': feedback.type === 'success',
        'bg-red-100 text-red-800': feedback.type === 'error',
        'bg-yellow-100 text-yellow-800': feedback.type === 'warning'
      }">
        <p class="font-medium">{{ feedback.message }}</p>
        <p v-if="feedback.details" class="mt-1 text-xs">{{ feedback.details }}</p>
      </div>
    </div>
    
    <!-- Current Students Display -->
    <div>
      <h2 class="text-xl font-semibold text-gray-700 mb-4">Current Student Roster</h2>
      <div v-if="loading && students.length === 0" class="text-center text-gray-500">
        Loading students...
      </div>
      <div v-else-if="students.length === 0" class="text-center text-gray-500 bg-white p-6 rounded-lg shadow-sm border">
        No students found. Upload a file to get started.
      </div>
      
      <!-- Student Grid Layout -->
      <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
        <div 
          v-for="student in students" 
          :key="student.student_id" 
          class="bg-white p-4 rounded-lg shadow-sm border border-gray-200 hover:shadow-md transition-shadow"
        >
          <p class="font-bold text-gray-800 truncate">{{ student.full_name }}</p>
          <p class="text-sm text-gray-600">{{ student.student_id }}</p>
          <p v-if="student.class_section" class="text-xs text-gray-500 mt-1 bg-gray-100 px-2 py-1 rounded-full inline-block">
            {{ student.class_section }}
          </p>
        </div>
      </div>
    </div>
  </div>
</template>
