<template>
  <div class="physical-id-card-wrap">
    
    <div class="id-card js-tilt" :class="{ 'is-flipped': isFlipped }" @click="isFlipped = !isFlipped">
      <!-- FRONT OF CARD -->
      <div class="card-face card-front">
        <div class="card-glare"></div>
        <div class="card-header">
          <div class="university-logo">
            <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M12 14l9-5-9-5-9 5 9 5zm0 0l6.16-3.422a12.083 12.083 0 01.665 6.479A11.952 11.952 0 0012 20.055a11.952 11.952 0 00-6.824-2.998 12.078 12.078 0 01.665-6.479L12 14z" />
            </svg>
            <span>BAS WEB</span>
          </div>
          <button class="download-btn" @click.stop="downloadBarcode" title="Download ID">
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" class="icon">
              <path stroke-linecap="round" stroke-linejoin="round" d="M3 16.5v2.25A2.25 2.25 0 005.25 21h13.5A2.25 2.25 0 0021 18.75V16.5M16.5 12L12 16.5m0 0L7.5 12m4.5 4.5V3" />
            </svg>
          </button>
        </div>

        <div class="card-body">
          <div class="student-photo-placeholder">
            <svg width="40" height="40" fill="currentColor" viewBox="0 0 20 20">
               <path fill-rule="evenodd" d="M10 9a3 3 0 100-6 3 3 0 000 6zm-7 9a7 7 0 1114 0H3z" clip-rule="evenodd" />
            </svg>
          </div>
          <div class="student-info">
            <h3 class="student-name">Student Account</h3>
            <p class="student-major">Undergraduate</p>
          </div>
        </div>

        <div class="card-barcode-section">
          <div class="barcode-display">
            <svg ref="barcodeRef" class="barcode-svg"></svg>
          </div>
          <p class="barcode-text">{{ studentId }}</p>
        </div>
      </div>
      
      <!-- BACK OF CARD -->
      <div class="card-face card-back">
        <div class="magnetic-strip"></div>
        <div class="back-content">
          <h4>Property of BAS University</h4>
          <p>This digital identification card is strictly for institutional attendance and privileges. If found, please return to the administration office.</p>
          <div class="qr-mock">
            <svg width="60" height="60" viewBox="0 0 24 24" fill="var(--text-main)">
               <rect x="3" y="3" width="7" height="7"/>
               <rect x="14" y="3" width="7" height="7"/>
               <rect x="3" y="14" width="7" height="7"/>
               <rect x="14" y="14" width="3" height="3"/>
               <rect x="18" y="18" width="3" height="3"/>
               <path d="M14 18h3v3h-3z"/>
               <path d="M18 14h3v3h-3z"/>
            </svg>
          </div>
          <p class="click-hint">Tap to flip</p>
        </div>
      </div>

    </div>
    
    <div class="card-hint">
      <p>Show this digital ID for attendance scanning. <br/><strong>Tap the card to flip.</strong></p>
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
const isFlipped = ref(false)

const generateBarcode = () => {
  if (!barcodeRef.value || !props.studentId) return
  
  try {
    JsBarcode(barcodeRef.value, props.studentId, {
      format: 'CODE128',
      width: 2.2,
      height: 70,
      displayValue: false,
      margin: 0,
      background: 'transparent',
      lineColor: '#000000'
    })
  } catch (error) {
    console.error('Barcode generation failed:', error)
    toast.error('Failed to generate barcode')
  }
}

const downloadBarcode = () => {
  if (!barcodeRef.value) return
  
  try {
    const canvas = document.createElement('canvas')
    const ctx = canvas.getContext('2d')
    const svgData = new XMLSerializer().serializeToString(barcodeRef.value)
    const img = new Image()
    
    img.onload = () => {
      canvas.width = img.width + 40
      canvas.height = img.height + 40
      ctx.fillStyle = 'white'
      ctx.fillRect(0, 0, canvas.width, canvas.height)
      ctx.drawImage(img, 20, 20)
      
      const link = document.createElement('a')
      link.download = `BAS-ID-${props.studentId}.png`
      link.href = canvas.toDataURL('image/png')
      link.click()
      
      toast.success('Digital ID downloaded!')
    }
    
    img.src = 'data:image/svg+xml;base64,' + btoa(unescape(encodeURIComponent(svgData)))
  } catch (error) {
    toast.error('Failed to download ID')
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
.physical-id-card-wrap {
  display: flex;
  flex-direction: column;
  align-items: center;
  perspective: 1000px;
  padding: 1rem 0;
}

.id-card {
  width: 340px;
  height: 540px;
  position: relative;
  transition: transform 0.8s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  transform-style: preserve-3d;
  cursor: pointer;
}

.id-card.is-flipped {
  transform: rotateY(180deg);
}

.card-face {
  position: absolute;
  width: 100%;
  height: 100%;
  backface-visibility: hidden;
  border-radius: 24px;
  box-shadow: 0 25px 50px -12px rgba(0,0,0,0.3);
  overflow: hidden;
  display: flex;
  flex-direction: column;
}

/* === FRONT OF CARD === */
.card-front {
  background: linear-gradient(135deg, rgba(255,255,255,1) 0%, rgba(243,244,246,1) 100%);
  border: 1px solid rgba(255,255,255,0.8);
}

[data-theme='dark'] .card-front {
  background: linear-gradient(135deg, #1e293b 0%, #0f172a 100%);
  border: 1px solid rgba(255,255,255,0.1);
}

.card-glare {
  position: absolute;
  top: 0; left: 0; right: 0; bottom: 0;
  background: linear-gradient(105deg, transparent 20%, rgba(255,255,255,0.4) 25%, transparent 30%);
  z-index: 10;
  pointer-events: none;
  animation: sweep 4s infinite linear;
}

[data-theme='dark'] .card-glare {
  background: linear-gradient(105deg, transparent 20%, rgba(255,255,255,0.1) 25%, transparent 30%);
}

@keyframes sweep {
  0% { background-position: -400px 0; }
  100% { background-position: 400px 0; }
}

.card-header {
  background: linear-gradient(90deg, #4f46e5 0%, #7c3aed 100%);
  color: white;
  padding: 1.25rem 1.5rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.university-logo {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-weight: 900;
  letter-spacing: 1px;
}

.download-btn {
  background: rgba(255,255,255,0.2);
  border: none;
  border-radius: 50%;
  width: 36px; height: 36px;
  display: flex; align-items: center; justify-content: center;
  color: white;
  cursor: pointer;
  transition: all 0.2s;
}

.download-btn:hover { background: rgba(255,255,255,0.3); transform: scale(1.05); }
.download-btn .icon { width: 18px; height: 18px; }

.card-body {
  padding: 2rem 1.5rem;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  flex: 1;
}

.student-photo-placeholder {
  width: 120px;
  height: 140px;
  background: var(--bg-main);
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--text-muted);
  margin-bottom: 1.5rem;
  box-shadow: inset 0 2px 4px rgba(0,0,0,0.05);
  border: 4px solid var(--bg-card);
}

.student-name {
  font-size: 1.5rem;
  font-weight: 800;
  color: var(--text-main);
  margin-bottom: 0.25rem;
}

.student-major {
  font-size: 0.95rem;
  color: var(--primary);
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 1px;
}

.card-barcode-section {
  background: white;
  padding: 1.5rem;
  display: flex;
  flex-direction: column;
  align-items: center;
  border-top: 1px dashed var(--border-light);
}

.barcode-display {
  width: 100%;
  display: flex;
  justify-content: center;
  margin-bottom: 0.5rem;
}

.barcode-svg {
  width: 100%;
  height: 70px;
}

.barcode-text {
  font-family: 'Courier New', monospace;
  font-size: 1.15rem;
  font-weight: 800;
  color: #000;
  letter-spacing: 0.3em;
}

/* === BACK OF CARD === */
.card-back {
  background: linear-gradient(135deg, rgba(255,255,255,1) 0%, rgba(229,231,235,1) 100%);
  transform: rotateY(180deg);
}

[data-theme='dark'] .card-back {
  background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%);
}

.magnetic-strip {
  width: 100%;
  height: 60px;
  background: #111827;
  margin-top: 2rem;
}

.back-content {
  padding: 2rem 1.5rem;
  text-align: left;
  display: flex;
  flex-direction: column;
  flex: 1;
}

.back-content h4 {
  font-size: 0.9rem;
  font-weight: 800;
  color: var(--text-main);
  margin-bottom: 0.75rem;
  text-transform: uppercase;
}

.back-content p {
  font-size: 0.8rem;
  color: var(--text-muted);
  line-height: 1.5;
  margin-bottom: 2rem;
}

.qr-mock {
  opacity: 0.5;
  margin-bottom: auto;
}

.click-hint {
  text-align: center !important;
  font-size: 0.75rem !important;
  font-weight: 700 !important;
  color: var(--primary) !important;
  margin-bottom: 0 !important;
  animation: pulse 2s infinite;
}

@keyframes pulse {
  0%, 100% { opacity: 0.5; }
  50% { opacity: 1; }
}

.card-hint {
  margin-top: 2rem;
  text-align: center;
}

.card-hint p {
  font-size: 0.9rem;
  color: var(--text-muted);
  line-height: 1.4;
}

@media (max-width: 400px) {
  .id-card {
    width: 300px;
    height: 480px;
  }
}
</style>
