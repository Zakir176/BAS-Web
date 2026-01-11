<template>
  <div class="student-upload-page">
    <Navbar />
    
    <main class="main-content">
      <div class="upload-wrapper">
        <Card class="upload-card">
          <div class="upload-header">
            <h2>Upload Document</h2>
            <p>Submit your assignments and documents</p>
          </div>
          
          <div class="upload-section">
            <div 
              class="upload-area"
              :class="{ 'drag-over': isDragOver, 'has-file': selectedFile }"
              @drop="handleDrop"
              @dragover="handleDragOver"
              @dragleave="handleDragLeave"
              @click="triggerFileInput"
            >
              <input
                ref="fileInput"
                type="file"
                accept=".pdf,.doc,.docx"
                @change="handleFileSelect"
                class="file-input"
              />
              
              <div v-if="!selectedFile" class="upload-placeholder">
                <div class="upload-icon">
                  <svg width="48" height="48" viewBox="0 0 48 48" fill="none">
                    <rect width="48" height="48" rx="12" fill="var(--accent-primary)" opacity="0.1"/>
                    <path d="M24 16v12m0 0l-4-4m4 4l4-4" stroke="var(--accent-primary)" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                    <path d="M8 32h32a2 2 0 002-2V18a2 2 0 00-2-2h-8m-24 0h8" stroke="var(--accent-primary)" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                  </svg>
                </div>
                <h3>Drop your file here</h3>
                <p>or click to browse</p>
                <div class="file-types">
                  <span>PDF, DOC, DOCX</span>
                  <span>Max 5MB</span>
                </div>
              </div>
              
              <div v-else class="selected-file">
                <div class="file-info">
                  <div class="file-icon">
                    <svg width="32" height="32" viewBox="0 0 32 32" fill="none">
                      <rect width="32" height="32" rx="8" fill="var(--accent-primary)" opacity="0.1"/>
                      <path d="M10 8h12v16H10z" fill="var(--accent-primary)"/>
                      <path d="M14 4v8h8" stroke="var(--accent-primary)" stroke-width="2" fill="none"/>
                    </svg>
                  </div>
                  <div class="file-details">
                    <h4>{{ selectedFile.name }}</h4>
                    <p>{{ formatFileSize(selectedFile.size) }}</p>
                  </div>
                </div>
                <button @click.stop="removeFile" class="remove-file">
                  <svg width="20" height="20" viewBox="0 0 20 20" fill="currentColor">
                    <path d="M6 4l1 12h6l1-12H6zm2 0V2h4v2H8z"/>
                  </svg>
                </button>
              </div>
            </div>
            
            <div class="upload-actions">
              <Button
                variant="primary"
                size="lg"
                full-width
                :disabled="!selectedFile || isUploading"
                @click="uploadFile"
              >
                <span v-if="!isUploading">Upload Document</span>
                <span v-else>Uploading...</span>
              </Button>
            </div>
          </div>
          
          <div class="recent-uploads">
            <h3>Recent Uploads</h3>
            <div class="uploads-list">
              <div v-for="upload in recentUploads" :key="upload.id" class="upload-item">
                <div class="upload-info">
                  <div class="file-icon small">
                    <svg width="20" height="20" viewBox="0 0 20 20" fill="none">
                      <rect width="20" height="20" rx="4" fill="var(--accent-primary)" opacity="0.1"/>
                      <path d="M6 6h8v8H6z" fill="var(--accent-primary)"/>
                      <path d="M10 2v4h4" stroke="var(--accent-primary)" stroke-width="1" fill="none"/>
                    </svg>
                  </div>
                  <div class="upload-details">
                    <h4>{{ upload.name }}</h4>
                    <p>{{ upload.date }} • {{ upload.size }}</p>
                  </div>
                </div>
                <div class="upload-status" :class="upload.status">
                  {{ upload.status }}
                </div>
              </div>
            </div>
          </div>
        </Card>
      </div>
    </main>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import Navbar from '@/components/layout/Navbar.vue'
import Button from '@/components/ui/Button.vue'
import Card from '@/components/ui/Card.vue'

const router = useRouter()
const fileInput = ref(null)
const selectedFile = ref(null)
const isDragOver = ref(false)
const isUploading = ref(false)

const recentUploads = ref([
  {
    id: 1,
    name: 'Assignment_1.pdf',
    date: 'Dec 10, 2024',
    size: '2.3 MB',
    status: 'completed'
  },
  {
    id: 2,
    name: 'Lab_Report.docx',
    date: 'Dec 8, 2024',
    size: '1.8 MB',
    status: 'completed'
  },
  {
    id: 3,
    name: 'Project_Proposal.pdf',
    date: 'Dec 5, 2024',
    size: '3.2 MB',
    status: 'processing'
  }
])

const triggerFileInput = () => {
  fileInput.value.click()
}

const handleFileSelect = (event) => {
  const file = event.target.files[0]
  if (file) {
    validateAndSetFile(file)
  }
}

const handleDrop = (event) => {
  event.preventDefault()
  isDragOver.value = false
  
  const file = event.dataTransfer.files[0]
  if (file) {
    validateAndSetFile(file)
  }
}

const handleDragOver = (event) => {
  event.preventDefault()
  isDragOver.value = true
}

const handleDragLeave = (event) => {
  event.preventDefault()
  isDragOver.value = false
}

const validateAndSetFile = (file) => {
  const allowedTypes = ['application/pdf', 'application/msword', 'application/vnd.openxmlformats-officedocument.wordprocessingml.document']
  const maxSize = 5 * 1024 * 1024 // 5MB
  
  if (!allowedTypes.includes(file.type)) {
    alert('Please upload a PDF, DOC, or DOCX file')
    return
  }
  
  if (file.size > maxSize) {
    alert('File size must be less than 5MB')
    return
  }
  
  selectedFile.value = file
}

const removeFile = () => {
  selectedFile.value = null
  if (fileInput.value) {
    fileInput.value.value = ''
  }
}

const formatFileSize = (bytes) => {
  if (bytes === 0) return '0 Bytes'
  const k = 1024
  const sizes = ['Bytes', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
}

const uploadFile = async () => {
  if (!selectedFile.value) return
  
  isUploading.value = true
  
  try {
    // Mock upload - replace with actual Supabase storage call
    await new Promise(resolve => setTimeout(resolve, 2000))
    
    // Add to recent uploads
    const newUpload = {
      id: Date.now(),
      name: selectedFile.value.name,
      date: new Date().toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' }),
      size: formatFileSize(selectedFile.value.size),
      status: 'completed'
    }
    
    recentUploads.value.unshift(newUpload)
    
    // Clear selected file
    selectedFile.value = null
    if (fileInput.value) {
      fileInput.value.value = ''
    }
    
    alert('File uploaded successfully!')
  } catch (error) {
    console.error('Upload failed:', error)
    alert('Upload failed. Please try again.')
  } finally {
    isUploading.value = false
  }
}

onMounted(() => {
  console.log('Student upload page loaded')
})
</script>

<style scoped>
.student-upload-page {
  min-height: 100vh;
  background-color: var(--bg-primary);
}

.main-content {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 2rem 0;
}

.upload-wrapper {
  width: 100%;
  max-width: 600px;
  padding: 0 1rem;
}

.upload-card {
  width: 100%;
}

.upload-header {
  text-align: center;
  margin-bottom: 2rem;
}

.upload-header h2 {
  font-size: 1.875rem;
  font-weight: 600;
  color: var(--text-primary);
  margin-bottom: 0.5rem;
}

.upload-header p {
  color: var(--text-secondary);
  margin: 0;
}

.upload-section {
  margin-bottom: 2rem;
}

.upload-area {
  border: 2px dashed var(--border-primary);
  border-radius: 0.75rem;
  padding: 3rem 2rem;
  text-align: center;
  cursor: pointer;
  transition: all 0.3s ease;
  background-color: var(--bg-secondary);
  margin-bottom: 1.5rem;
}

.upload-area:hover {
  border-color: var(--accent-primary);
  background-color: var(--bg-tertiary);
}

.upload-area.drag-over {
  border-color: var(--accent-primary);
  background-color: var(--accent-primary);
  opacity: 0.1;
}

.upload-area.has-file {
  border-style: solid;
  border-color: var(--accent-primary);
  background-color: var(--bg-tertiary);
}

.file-input {
  display: none;
}

.upload-placeholder h3 {
  font-size: 1.25rem;
  font-weight: 600;
  color: var(--text-primary);
  margin-bottom: 0.5rem;
}

.upload-placeholder p {
  color: var(--text-secondary);
  margin-bottom: 1rem;
}

.file-types {
  display: flex;
  justify-content: center;
  gap: 1rem;
  font-size: 0.875rem;
  color: var(--text-tertiary);
}

.selected-file {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.file-info {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.file-details h4 {
  font-size: 1rem;
  font-weight: 600;
  color: var(--text-primary);
  margin-bottom: 0.25rem;
}

.file-details p {
  color: var(--text-secondary);
  font-size: 0.875rem;
  margin: 0;
}

.remove-file {
  background: none;
  border: none;
  color: var(--error);
  cursor: pointer;
  padding: 0.5rem;
  border-radius: 0.5rem;
  transition: background-color 0.2s ease;
}

.remove-file:hover {
  background-color: var(--error);
  color: white;
}

.upload-actions {
  margin-bottom: 2rem;
}

.recent-uploads h3 {
  font-size: 1.25rem;
  font-weight: 600;
  color: var(--text-primary);
  margin-bottom: 1rem;
}

.uploads-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.upload-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  border: 1px solid var(--border-primary);
  border-radius: 0.5rem;
  background-color: var(--bg-secondary);
}

.upload-info {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.file-icon.small {
  width: 2.5rem;
  height: 2.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
}

.upload-details h4 {
  font-size: 0.875rem;
  font-weight: 500;
  color: var(--text-primary);
  margin-bottom: 0.25rem;
}

.upload-details p {
  color: var(--text-secondary);
  font-size: 0.75rem;
  margin: 0;
}

.upload-status {
  padding: 0.25rem 0.75rem;
  border-radius: 1rem;
  font-size: 0.75rem;
  font-weight: 500;
}

.upload-status.completed {
  background-color: var(--success);
  color: white;
}

.upload-status.processing {
  background-color: var(--warning);
  color: white;
}

@media (max-width: 768px) {
  .upload-area {
    padding: 2rem 1rem;
  }
  
  .selected-file {
    flex-direction: column;
    gap: 1rem;
    text-align: center;
  }
  
  .upload-item {
    flex-direction: column;
    gap: 0.75rem;
    text-align: center;
  }
  
  .file-types {
    flex-direction: column;
    gap: 0.5rem;
  }
}
</style>
