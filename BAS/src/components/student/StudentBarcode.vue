<template>
  <div class="barcode-container">
    <div class="barcode-card">
      <div class="barcode-header">
        <h3>Student ID Barcode</h3>
        <button class="download-btn" @click="downloadBarcode" title="Download Barcode">
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" class="icon">
            <path stroke-linecap="round" stroke-linejoin="round" d="M3 16.5v2.25A2.25 2.25 0 005.25 21h13.5A2.25 2.25 0 0021 18.75V16.5M16.5 12L12 16.5m0 0L7.5 12m4.5 4.5V3" />
          </svg>
        </button>
      </div>
      
      <div class="barcode-display">
        <svg ref="barcodeRef" class="barcode-svg"></svg>
        <p class="barcode-text">{{ studentId }}</p>
      </div>
      
      <p class="barcode-hint">Show this barcode to your lecturer for attendance scanning</p>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue'
import JsBarcode from 'jsbarcode'
import { useToast } from '@/composables/useToast'

const props = defineProps({
  studentId: {
    type: String,
    required: true
  }
})

const { toast } = useToast()
const barcodeRef = ref(null)

const generateBarcode = () => {
  if (!barcodeRef.value || !props.studentId) return
  
  try {
    JsBarcode(barcodeRef.value, props.studentId, {
      format: 'CODE128',
      width: 2,
      height: 80,
      displayValue: false,
      margin: 10,
      background: 'transparent',
      lineColor: '#111827'
    })
  } catch (error) {
    console.error('Barcode generation failed:', error)
    toast.error('Failed to generate barcode')
  }
}

const downloadBarcode = () => {
  if (!barcodeRef.value) return
  
  try {
    // Create a temporary canvas to export the barcode
    const canvas = document.createElement('canvas')
    const ctx = canvas.getContext('2d')
    const svgData = new XMLSerializer().serializeToString(barcodeRef.value)
    const img = new Image()
    
    img.onload = () => {
      canvas.width = img.width
      canvas.height = img.height
      ctx.fillStyle = 'white'
      ctx.fillRect(0, 0, canvas.width, canvas.height)
      ctx.drawImage(img, 0, 0)
      
      // Download the image
      const link = document.createElement('a')
      link.download = `student-barcode-${props.studentId}.png`
      link.href = canvas.toDataURL('image/png')
      link.click()
      
      toast.success('Barcode downloaded successfully!')
    }
    
    img.src = 'data:image/svg+xml;base64,' + btoa(unescape(encodeURIComponent(svgData)))
  } catch (error) {
    console.error('Download failed:', error)
    toast.error('Failed to download barcode')
  }
}

onMounted(() => {
  generateBarcode()
})

watch(() => props.studentId, () => {
  generateBarcode()
})
</script>

<style scoped>
.barcode-container {
  margin-bottom: 2rem;
}

.barcode-card {
  background: var(--bg-card);
  border-radius: 20px;
  padding: 1.5rem;
  box-shadow: var(--shadow-soft);
  border: 1px solid rgba(0, 0, 0, 0.05);
}

.barcode-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.25rem;
}

.barcode-header h3 {
  font-size: 1.125rem;
  font-weight: 800;
  color: var(--text-main);
  margin: 0;
}

.download-btn {
  background: var(--primary);
  color: white;
  border: none;
  border-radius: 12px;
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.2s;
  box-shadow: 0 2px 8px rgba(59, 130, 246, 0.3);
}

.download-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(59, 130, 246, 0.4);
}

.download-btn:active {
  transform: translateY(0);
}

.download-btn .icon {
  width: 20px;
  height: 20px;
}

.barcode-display {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 1.5rem 1rem;
  background: #ffffff;
  border-radius: 16px;
  border: 2px dashed #e2e8f0;
  margin-bottom: 1rem;
}

.barcode-svg {
  max-width: 100%;
  height: auto;
  margin-bottom: 0.75rem;
}

.barcode-text {
  font-size: 1rem;
  font-weight: 700;
  color: var(--text-main);
  letter-spacing: 0.1em;
  margin: 0;
  font-family: 'Courier New', monospace;
}

.barcode-hint {
  font-size: 0.875rem;
  color: var(--text-muted);
  text-align: center;
  margin: 0;
  font-weight: 500;
}

/* Dark mode support */
[data-theme="dark"] .barcode-card {
  background: var(--bg-card);
  border-color: var(--border-light);
}

[data-theme="dark"] .barcode-display {
  background: #f8fafc;
  border-color: #cbd5e1;
}

[data-theme="dark"] .barcode-header h3 {
  color: var(--text-main);
}

[data-theme="dark"] .barcode-hint {
  color: var(--text-muted);
}

[data-theme="dark"] .download-btn {
  box-shadow: 0 2px 8px rgba(96, 165, 250, 0.4);
}

[data-theme="dark"] .download-btn:hover {
  box-shadow: 0 4px 12px rgba(96, 165, 250, 0.5);
}

/* Mobile responsive */
@media (max-width: 640px) {
  .barcode-card {
    padding: 1.25rem;
  }
  
  .barcode-display {
    padding: 1rem 0.5rem;
  }
  
  .barcode-header h3 {
    font-size: 1rem;
  }
}
</style>
