<template>
  <div class="student-upload-page">
    <Navbar />
    
    <main class="main-content">
      <div class="container py-12">
        <div class="upload-v2-container">
          <!-- Left: Upload Info -->
          <div class="upload-info-panel">
            <span class="badge-v2">DOCUMENT PORTAL</span>
            <h1>Submit your <br/><span class="text-gradient">Academic Files.</span></h1>
            <p class="description">Upload your assignments, reports, and certifications directly to the secure campus storage.</p>
            
            <div class="guidelines">
              <div class="guideline-item">
                <span class="bullet"></span>
                <span>Supported: PDF, DOCX (Max 5MB)</span>
              </div>
              <div class="guideline-item">
                <span class="bullet"></span>
                <span>Clear naming convention required</span>
              </div>
            </div>
          </div>

          <!-- Right: Upload Area -->
          <div class="upload-action-panel">
            <div 
              class="drop-zone-v2"
              :class="{ 'is-active': isDragOver, 'has-file': selectedFile }"
              @drop="handleDrop"
              @dragover="handleDragOver"
              @dragleave="handleDragLeave"
              @click="triggerFileInput"
            >
              <input ref="fileInput" type="file" accept=".pdf,.doc,.docx" @change="handleFileSelect" hidden />
              
              <div v-if="!selectedFile" class="placeholder-content">
                <div class="icon-pulse">📁</div>
                <h3>Drop file to upload</h3>
                <p>or click to browse</p>
              </div>
              
              <div v-else class="file-preview-v2">
                <div class="file-meta">
                  <span class="file-icon-large">📄</span>
                  <div class="meta-text">
                    <h4 class="truncate">{{ selectedFile.name }}</h4>
                    <p>{{ formatFileSize(selectedFile.size) }}</p>
                  </div>
                </div>
                <button @click.stop="removeFile" class="close-btn">✕</button>
              </div>
            </div>

            <Button
              variant="primary"
              size="lg"
              full-width
              class="upload-submit-btn"
              :disabled="!selectedFile || isUploading"
              @click="uploadFile"
            >
              <span v-if="!isUploading">Confirm and Upload</span>
              <span v-else class="loading-inline">Uploading...</span>
            </Button>

            <!-- History Section -->
            <div class="upload-history-v2">
              <div class="history-header">
                <h4>Recent Submissions</h4>
                <div class="count-badge">{{ recentUploads.length }}</div>
              </div>
              
              <div class="history-list">
                <div v-for="upload in recentUploads" :key="upload.id" class="history-card">
                  <div class="card-left">
                    <span class="type-icon">📄</span>
                    <div class="info">
                      <h5 class="truncate">{{ upload.file_name }}</h5>
                      <p>{{ formatDate(upload.created_at) }}</p>
                    </div>
                  </div>
                  <div class="card-right">
                    <span class="status-pill" :class="upload.status">{{ upload.status }}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { supabase } from '@/supabase'
import { useAuth } from '@/composables/useAuth'
import Navbar from '@/components/layout/Navbar.vue'
import Button from '@/components/ui/Button.vue'

const { user } = useAuth()
const fileInput = ref(null)
const selectedFile = ref(null)
const isDragOver = ref(false)
const isUploading = ref(false)
const recentUploads = ref([])

const triggerFileInput = () => fileInput.value.click()

const validateAndSetFile = (file) => {
  const allowedTypes = ['application/pdf', 'application/msword', 'application/vnd.openxmlformats-officedocument.wordprocessingml.document']
  const maxSize = 5 * 1024 * 1024 // 5MB

  if (!allowedTypes.includes(file.type)) {
    alert('Please upload PDF or Word documents only.')
    return false
  }
  if (file.size > maxSize) {
    alert('File size exceeds 5MB limit.')
    return false
  }
  selectedFile.value = file
  return true
}

const handleFileSelect = (event) => {
  const file = event.target.files[0]
  if (file) validateAndSetFile(file)
}

const handleDrop = (event) => {
  event.preventDefault()
  isDragOver.value = false
  const file = event.dataTransfer.files[0]
  if (file) validateAndSetFile(file)
}

const handleDragOver = (e) => { e.preventDefault(); isDragOver.value = true }
const handleDragLeave = (e) => { e.preventDefault(); isDragOver.value = false }
const removeFile = () => { selectedFile.value = null; if (fileInput.value) fileInput.value.value = '' }

const formatFileSize = (bytes) => {
  if (bytes === 0) return '0 B'
  const k = 1024
  const sizes = ['B', 'KB', 'MB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return parseFloat((bytes / Math.pow(k, i)).toFixed(1)) + ' ' + sizes[i]
}

const formatDate = (dateStr) => new Date(dateStr).toLocaleDateString()

const fetchRecentUploads = async () => {
  if (!user.value) return
  const { data } = await supabase
    .from('student_uploads')
    .select('*')
    .eq('student_id', user.value.id)
    .order('created_at', { ascending: false })
    .limit(5)
  if (data) recentUploads.value = data
}

const uploadFile = async () => {
  if (!selectedFile.value || !user.value) return
  isUploading.value = true
  
  try {
    const file = selectedFile.value
    const fileName = `${user.value.id}/${Date.now()}_${file.name}`
    
    const { error: storageError } = await supabase.storage
      .from('student-documents')
      .upload(fileName, file)

    if (storageError) throw storageError

    await supabase.from('student_uploads').insert({
      student_id: user.value.id,
      file_name: file.name,
      file_path: fileName,
      file_size: file.size,
      status: 'completed'
    })

    selectedFile.value = null
    await fetchRecentUploads()
    alert('Upload successful!')
  } catch (error) {
    console.error(error)
    alert('Upload failed')
  } finally {
    isUploading.value = false
  }
}

onMounted(fetchRecentUploads)
</script>

<style scoped>
.student-upload-page {
  min-height: 100vh;
  background-color: #f1f5f9;
}

.upload-v2-container {
  display: grid;
  grid-template-columns: 0.8fr 1.2fr;
  gap: 4rem;
  align-items: flex-start;
}

.badge-v2 {
  display: inline-block;
  padding: 4px 12px;
  background: #dbeafe;
  color: #2563eb;
  font-size: 0.75rem;
  font-weight: 800;
  border-radius: 100px;
  margin-bottom: 1.5rem;
}

.upload-info-panel h1 {
  font-size: 3rem;
  font-weight: 900;
  line-height: 1.1;
  color: #1e293b;
  margin-bottom: 1.5rem;
}

.text-gradient {
  background: linear-gradient(to right, #2563eb, #7c3aed);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}

.description {
  color: #64748b;
  font-size: 1.1rem;
  margin-bottom: 2.5rem;
}

.guideline-item {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 1rem;
  font-weight: 700;
  color: #1e293b;
  font-size: 0.9rem;
}

.bullet {
  width: 6px;
  height: 6px;
  background: #3b82f6;
  border-radius: 50%;
}

/* Action Panel */
.upload-action-panel {
  background: white;
  padding: 2rem;
  border-radius: 24px;
  box-shadow: var(--shadow-card);
}

.drop-zone-v2 {
  border: 2px dashed #e2e8f0;
  border-radius: 20px;
  padding: 4rem 2rem;
  text-align: center;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  margin-bottom: 1.5rem;
  position: relative;
}

.drop-zone-v2:hover, .drop-zone-v2.is-active {
  border-color: #3b82f6;
  background: #eff6ff;
  transform: scale(0.995);
}

.icon-pulse {
  font-size: 3rem;
  margin-bottom: 1rem;
}

.drop-zone-v2 h3 {
  font-size: 1.25rem;
  font-weight: 800;
  color: #1e293b;
  margin-bottom: 0.25rem;
}

.drop-zone-v2 p {
  color: #64748b;
  font-size: 0.9rem;
}

/* File Preview */
.file-preview-v2 {
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: white;
  padding: 1rem;
  border-radius: 12px;
  box-shadow: 0 4px 12px rgba(0,0,0,0.05);
}

.file-meta {
  display: flex;
  gap: 1rem;
  align-items: center;
  text-align: left;
}

.file-icon-large { font-size: 2rem; }
.meta-text h4 { font-weight: 800; color: #1e293b; font-size: 0.95rem; max-width: 200px; }
.meta-text p { font-size: 0.8rem; color: #64748b; font-weight: 700; }

.close-btn {
  background: #f1f5f9;
  border: none;
  width: 32px;
  height: 32px;
  border-radius: 50%;
  cursor: pointer;
  font-weight: 800;
  color: #64748b;
}

/* History */
.upload-history-v2 {
  margin-top: 3rem;
  padding-top: 2rem;
  border-top: 1px solid #f1f5f9;
}

.history-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.history-header h4 { font-weight: 800; color: #1e293b; }
.count-badge { background: #f1f5f9; padding: 2px 10px; border-radius: 100px; font-size: 0.75rem; font-weight: 800; color: #64748b; }

.history-list { display: flex; flex-direction: column; gap: 0.75rem; }

.history-card {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  background: #f8fafc;
  border-radius: 12px;
  transition: transform 0.2s;
}

.card-left { display: flex; gap: 1rem; align-items: center; }
.type-icon { font-size: 1.2rem; }
.info h5 { font-size: 0.9rem; font-weight: 800; color: #1e293b; max-width: 180px; }
.info p { font-size: 0.75rem; font-weight: 700; color: #94a3b8; }

.status-pill {
  font-size: 0.7rem;
  font-weight: 800;
  text-transform: uppercase;
  padding: 4px 8px;
  border-radius: 6px;
}

.status-pill.completed { background: #dcfce7; color: #166534; }

.truncate {
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

@media (max-width: 900px) {
  .upload-v2-container { grid-template-columns: 1fr; gap: 2rem; }
  .upload-info-panel { text-align: center; }
  .upload-info-panel h1 { font-size: 2.5rem; }
  .guideline-item { justify-content: center; }
}
</style>
