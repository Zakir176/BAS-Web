<template>
  <div class="physical-id-card-wrap">
    
    <div class="id-card-container js-tilt" :class="{ 'is-flipped': isFlipped }" @click="isFlipped = !isFlipped">
      <!-- FRONT OF CARD -->
      <div class="card-face card-front">
        <!-- Premium Effects Overlays -->
        <div class="card-shimmer"></div>
        <div class="card-edge-glow"></div>
        
        <div class="card-header">
          <div class="uni-brand">
            <div class="uni-logo-icon">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                <path stroke-linecap="round" stroke-linejoin="round" d="M12 14l9-5-9-5-9 5 9 5zm0 0l6.16-3.422a12.083 12.083 0 01.665 6.479A11.952 11.952 0 0012 20.055a11.952 11.952 0 00-6.824-2.998 12.078 12.078 0 01.665-6.479L12 14z" />
              </svg>
            </div>
            <div class="uni-name-stack">
              <span class="uni-name">BAS UNIVERSITY</span>
              <span class="uni-tagline">Excellence in Innovation</span>
            </div>
          </div>
          <div class="card-type-badge">PRO</div>
        </div>

        <div class="card-body">
          <div class="student-profile-sector">
            <div class="photo-frame">
              <div class="status-ring"></div>
              <div class="photo-placeholder">
                <svg width="48" height="48" fill="currentColor" viewBox="0 0 20 20" opacity="0.3">
                   <path fill-rule="evenodd" d="M10 9a3 3 0 100-6 3 3 0 000 6zm-7 9a7 7 0 1114 0H3z" clip-rule="evenodd" />
                </svg>
              </div>
            </div>
            <div class="metadata-stack">
              <div class="status-badge">
                <span class="dot"></span> ACTIVE STUDENT
              </div>
              <h3 class="student-name">{{ studentName || 'Student Account' }}</h3>
              <p class="student-level">UNDERGRADUATE / YEAR 3</p>
            </div>
          </div>

          <div class="card-details-grid">
            <div class="detail-item">
              <span class="label">ISSUED ON</span>
              <span class="value">JAN 2024</span>
            </div>
            <div class="detail-item">
              <span class="label">LEVEL</span>
              <span class="value">PROFESSIONAL</span>
            </div>
            <div class="detail-item">
              <span class="label">VALID THRU</span>
              <span class="value">DEC 2026</span>
            </div>
          </div>
        </div>

        <div class="card-scan-sector">
          <div class="scan-label">
            <span class="scan-icon">📡</span>
            SECURE ACCESS BARCODE
          </div>
          <div class="barcode-chamber">
            <svg ref="barcodeRef" class="barcode-svg"></svg>
          </div>
          <div class="student-id-number">{{ studentId }}</div>
        </div>
        
        <div class="card-footer-branding">
          <span>IDENTITY VERIFIED BY THE REGISTRAR</span>
          <div class="mini-hologram"></div>
        </div>
      </div>
      
      <!-- BACK OF CARD -->
      <div class="card-face card-back">
        <div class="mag-stripe"></div>
        <div class="back-content">
          <div class="policy-block">
            <h4>Institutional Policy</h4>
            <p>This identification card remains the property of BAS University. It is strictly non-transferable and must be presented on demand by authorized personnel.</p>
          </div>
          
          <div class="contact-sector">
            <div class="contact-item">
              <span class="c-label">EMERGENCY CONTACT</span>
              <span class="c-value">+27 11 555 0123</span>
            </div>
            <div class="contact-item">
              <span class="c-label">SUPPORT EMAIL</span>
              <span class="c-value">registrar@bas.edu</span>
            </div>
          </div>

          <div class="qr-secure-zone">
            <div class="qr-wrapper">
              <svg width="80" height="80" viewBox="0 0 24 24" fill="currentColor" opacity="0.8">
                 <rect x="3" y="3" width="7" height="7" rx="1"/>
                 <rect x="14" y="3" width="7" height="7" rx="1"/>
                 <rect x="3" y="14" width="7" height="7" rx="1"/>
                 <rect x="14" y="14" width="3" height="3" rx="0.5"/>
                 <rect x="18" y="18" width="3" height="3" rx="0.5"/>
                 <path d="M14 18h3v3h-3z"/>
                 <path d="M18 14h3v3h-3z"/>
              </svg>
            </div>
            <div class="secure-tag">ENCRYPTED_AUTH_DATA</div>
          </div>
          
          <button class="download-trigger" @click.stop="downloadBarcode">
            <svg width="14" height="14" fill="none" stroke="currentColor" stroke-width="2.5" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" d="M3 16.5v2.25A2.25 2.25 0 005.25 21h13.5A2.25 2.25 0 0021 18.75V16.5M16.5 12L12 16.5m0 0L7.5 12m4.5 4.5V3" />
            </svg>
            SAVE DIGITAL COPY
          </button>
        </div>
      </div>

    </div>
    
    <div class="interaction-hint">
      <div class="tap-icon">👆</div>
      <p>Tap card to view security details</p>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue'
import JsBarcode from 'jsbarcode'
import { useToast } from '@/shared/composables/useToast'

const props = defineProps({
  studentId: { type: String, required: true },
  studentName: { type: String, default: '' }
})

const { toast } = useToast()
const barcodeRef = ref(null)
const isFlipped = ref(false)

const generateBarcode = () => {
  if (!barcodeRef.value || !props.studentId) return
  
  try {
    const isDark = document.documentElement.getAttribute('data-theme') === 'dark'
    JsBarcode(barcodeRef.value, props.studentId, {
      format: 'CODE128',
      width: 2.5,
      height: 60,
      displayValue: false,
      margin: 0,
      background: 'transparent',
      lineColor: isDark ? '#ffffff' : '#0f172a'
    })
  } catch (error) {
    console.error('Barcode generation failed:', error)
  }
}

const downloadBarcode = () => {
  toast.success('Digital ID saved to device!')
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
  perspective: 2000px;
  padding: 2rem 0;
}

.id-card-container {
  width: 360px;
  height: 560px;
  position: relative;
  transition: transform 0.8s cubic-bezier(0.34, 1.56, 0.64, 1);
  transform-style: preserve-3d;
  cursor: pointer;
}

.id-card-container.is-flipped { transform: rotateY(180deg); }

.card-face {
  position: absolute;
  width: 100%;
  height: 100%;
  backface-visibility: hidden;
  border-radius: 28px;
  box-shadow: 0 40px 80px -20px rgba(0,0,0,0.5);
  overflow: hidden;
  display: flex;
  flex-direction: column;
  background: var(--bg-card);
  border: 1px solid rgba(255,255,255,0.1);
}

/* === FRONT OF CARD === */
.card-front {
  background: linear-gradient(135deg, #1e293b 0%, #0f172a 100%);
  color: white;
}

[data-theme='light'] .card-front {
  background: linear-gradient(135deg, #ffffff 0%, #f1f5f9 100%);
  color: #0f172a;
}

/* Effects */
.card-shimmer {
  position: absolute;
  inset: 0;
  background: linear-gradient(105deg, transparent 40%, rgba(255,255,255,0.05) 45%, rgba(255,255,255,0.15) 50%, rgba(255,255,255,0.05) 55%, transparent 60%);
  opacity: 0.2;
  pointer-events: none;
  animation: shimmer-sweep 6s infinite linear;
}

.card-edge-glow {
  position: absolute;
  inset: 0;
  border: 1px solid rgba(96, 165, 250, 0.3);
  border-radius: 28px;
  box-shadow: inset 0 0 20px rgba(96, 165, 250, 0.1);
  pointer-events: none;
}

@keyframes shimmer-sweep { 0% { background-position: -600px 0; } 100% { background-position: 600px 0; } }

/* Header */
.card-header {
  padding: 1.5rem 1.75rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-bottom: 1px solid rgba(255,255,255,0.05);
  background: rgba(255,255,255,0.02);
}

.uni-brand { display: flex; align-items: center; gap: 0.75rem; }
.uni-logo-icon { color: var(--primary); }
.uni-name-stack { display: flex; flex-direction: column; line-height: 1; }
.uni-name { font-weight: 950; font-size: 0.9rem; letter-spacing: 1px; }
.uni-tagline { font-size: 0.55rem; font-weight: 600; opacity: 0.5; text-transform: uppercase; margin-top: 0.1rem; }

.card-type-badge {
  background: var(--primary);
  color: white;
  padding: 0.25rem 0.6rem;
  border-radius: 6px;
  font-size: 0.65rem;
  font-weight: 900;
  letter-spacing: 1px;
}

/* Body */
.card-body { padding: 2rem 1.75rem; flex: 1; display: flex; flex-direction: column; }

.student-profile-sector {
  display: flex;
  align-items: center;
  gap: 1.5rem;
  margin-bottom: 2.5rem;
}

.photo-frame {
  position: relative;
  width: 100px;
  height: 120px;
  background: rgba(255,255,255,0.02);
  border-radius: 16px;
  border: 4px solid rgba(255,255,255,0.05);
  display: flex;
  align-items: center; justify-content: center;
  overflow: hidden;
}

.status-ring {
  position: absolute;
  inset: 0;
  border: 2px solid var(--success);
  border-radius: 50%;
  transform: scale(1.5) translate(-30%, -30%);
  opacity: 0.1;
}

.metadata-stack { flex: 1; display: flex; flex-direction: column; align-items: flex-start; }

.status-badge {
  background: rgba(16, 185, 129, 0.1);
  color: #34d399;
  font-size: 0.6rem;
  font-weight: 900;
  padding: 0.3rem 0.6rem;
  border-radius: 8px;
  display: flex; align-items: center; gap: 0.35rem;
  margin-bottom: 0.5rem;
  border: 1px solid rgba(16, 185, 129, 0.2);
}

.status-badge .dot { width: 4px; height: 4px; border-radius: 50%; background: currentColor; }

.student-name { font-size: 1.6rem; font-weight: 900; margin: 0; line-height: 1.1; }
.student-level { font-size: 0.75rem; font-weight: 700; color: var(--primary); margin-top: 0.35rem; letter-spacing: 0.5px; opacity: 0.8; }

.card-details-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1.5rem;
  margin-top: auto;
}

.detail-item { display: flex; flex-direction: column; gap: 0.15rem; }
.detail-item .label { font-size: 0.55rem; font-weight: 800; color: rgba(255,255,255,0.3); letter-spacing: 0.5px; }
.detail-item .value { font-size: 0.8rem; font-weight: 700; opacity: 0.9; }

/* Scan Sector */
.card-scan-sector {
  background: rgba(255,255,255,0.03);
  padding: 2rem 1.75rem;
  border-top: 2px dashed rgba(255,255,255,0.05);
  display: flex;
  flex-direction: column;
  align-items: center;
  position: relative;
}

.scan-label {
  font-size: 0.65rem;
  font-weight: 850;
  opacity: 0.4;
  margin-bottom: 1rem;
  display: flex; align-items: center; gap: 0.4rem;
  text-transform: uppercase;
  letter-spacing: 1px;
}

.barcode-chamber {
  width: 100%;
  display: flex; justify-content: center;
  padding: 0.75rem;
  background: rgba(255,255,255,0.02);
  border-radius: 12px;
  border: 1px solid rgba(255,255,255,0.05);
}

.barcode-svg { width: 100%; height: 60px; filter: drop-shadow(0 0 5px rgba(255,255,255,0.1)); }

.student-id-number {
  margin-top: 0.75rem;
  font-family: 'JetBrains Mono', monospace;
  font-weight: 850;
  font-size: 1rem;
  opacity: 0.6;
  letter-spacing: 4px;
}

/* Footer branding */
.card-footer-branding {
  padding: 1rem 1.75rem;
  background: rgba(0,0,0,0.2);
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 0.55rem;
  font-weight: 700;
  color: rgba(255,255,255,0.2);
  text-transform: uppercase;
}

.mini-hologram { width: 24px; height: 16px; background: linear-gradient(135deg, #a855f7, #3b82f6, #10b981); border-radius: 4px; opacity: 0.3; }

/* === BACK OF CARD === */
.card-back { transform: rotateY(180deg); background: linear-gradient(135deg, #020617 0%, #1e293b 100%); }

.mag-stripe { width: 100%; height: 50px; background: #000; margin-top: 2.5rem; opacity: 0.8; }

.back-content { padding: 2.5rem 1.75rem; flex: 1; display: flex; flex-direction: column; }

.policy-block h4 { font-size: 0.75rem; font-weight: 900; margin-bottom: 0.5rem; color: var(--primary); }
.policy-block p { font-size: 0.7rem; color: rgba(255,255,255,0.4); line-height: 1.5; margin-bottom: 2rem; }

.contact-sector { display: flex; flex-direction: column; gap: 1rem; margin-bottom: auto; }
.c-label { font-size: 0.55rem; font-weight: 850; opacity: 0.3; text-transform: uppercase; }
.c-value { font-size: 0.85rem; font-weight: 700; color: rgba(255,255,255,0.8); }

.qr-secure-zone {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 2rem;
}

.qr-wrapper { padding: 0.75rem; background: white; border-radius: 12px; box-shadow: 0 10px 20px rgba(0,0,0,0.3); color: #000; }
.secure-tag { font-family: 'JetBrains Mono', monospace; font-size: 0.5rem; color: var(--primary); font-weight: 900; opacity: 0.5; }

.download-trigger {
  background: rgba(255,255,255,0.05);
  border: 1px solid rgba(255,255,255,0.1);
  color: white;
  padding: 0.75rem;
  border-radius: 12px;
  font-size: 0.7rem;
  font-weight: 850;
  display: flex; align-items: center; justify-content: center; gap: 0.5rem;
  cursor: pointer;
}

.interaction-hint { margin-top: 2rem; display: flex; align-items: center; gap: 0.5rem; opacity: 0.4; }
.interaction-hint p { font-size: 0.8rem; font-weight: 700; margin: 0; }
.tap-icon { animation: tap-pulse 2s infinite; }

@keyframes tap-pulse { 0%, 100% { transform: scale(1); } 50% { transform: scale(1.2); } }

@media (max-width: 400px) {
  .id-card-container { width: 320px; height: 500px; }
  .student-name { font-size: 1.3rem; }
  .photo-frame { width: 80px; height: 100px; }
}
</style>
