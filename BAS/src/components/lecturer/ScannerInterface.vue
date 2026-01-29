<template>
  <div class="scanner-interface">
    <!-- Scanner Header -->
    <div class="scanner-header-overlay">
      <div class="session-badge">
        <span class="pulse"></span> LIVE RECORDING
      </div>
      <div class="session-title-large">
        <h2>{{ sessionName || 'Active Session' }}</h2>
        <p>Scan student ID to mark attendance</p>
      </div>
      <button class="dismiss-btn" @click="$emit('close')">✕</button>
    </div>

    <!-- Camera Viewport -->
    <div class="camera-viewport">
      <BarcodeScanner @detected="$emit('detected', $event)" />
      <div class="scan-overlay-guides">
        <div class="guide-corner top-left"></div>
        <div class="guide-corner top-right"></div>
        <div class="guide-corner bottom-left"></div>
        <div class="guide-corner bottom-right"></div>
      </div>
    </div>

    <!-- Scan Status & Counter -->
    <div class="scanner-footer-overlay">
      <div class="stats-row">
        <div class="counter-box">
          <span class="label">ATTENDANCE LOGGED</span>
          <div class="count">{{ scannedCount }}</div>
        </div>
        <div class="actions-box">
           <button class="complete-btn" @click="$emit('complete')">
             <span class="icon">✓</span> Complete Session
           </button>
        </div>
      </div>
      <div class="progress-container">
        <div class="progress-labels">
          <span>Present ({{ progress }}%)</span>
          <span>Enrolled Students</span>
        </div>
        <div class="master-progress">
          <div class="present-bar" :style="{ width: progress + '%' }"></div>
          <div class="absent-bar" :style="{ width: (100 - progress) + '%' }"></div>
        </div>
      </div>
      
      <transition name="slide-up">
        <div v-if="lastScanned" class="scan-feedback-toast" :class="scanStatus">
          <div class="feedback-icon">{{ scanStatus === 'success' ? '✓' : '!' }}</div>
          <div class="feedback-text">
            <span class="name">{{ lastScanned }}</span>
            <span class="msg">{{ scanStatus === 'success' ? 'Attendance Recorded' : 'Scan Failed' }}</span>
          </div>
        </div>
      </transition>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import BarcodeScanner from '@/components/lecturer/BarcodeScanner.vue'

const props = defineProps({
  sessionName: { type: String, default: '' },
  scannedCount: { type: Number, default: 0 },
  totalStudents: { type: Number, default: 0 },
  lastScanned: { type: String, default: '' },
  scanStatus: { type: String, default: 'success' }
})

const progress = computed(() => {
  if (props.totalStudents === 0) return 0
  return Math.min(100, Math.round((props.scannedCount / props.totalStudents) * 100))
})

defineEmits(['close', 'detected', 'complete'])
</script>

<style scoped>
.scanner-interface {
  position: fixed;
  inset: 0;
  background: #000;
  z-index: 1000;
  display: flex;
  flex-direction: column;
}

.scanner-header-overlay {
  padding: 1.5rem;
  background: linear-gradient(to bottom, rgba(0,0,0,0.8), transparent);
  color: white;
  display: flex;
  justify-content: space-between;
  align-items: center;
  z-index: 10;
}

.session-badge {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  background: rgba(220, 38, 38, 0.2);
  color: #ef4444;
  padding: 0.4rem 0.8rem;
  border-radius: 100px;
  font-size: 0.75rem;
  font-weight: 800;
  border: 1px solid rgba(239, 68, 68, 0.3);
}

.pulse {
  width: 8px;
  height: 8px;
  background: #ef4444;
  border-radius: 50%;
  animation: pulse 2s infinite;
}

@keyframes pulse {
  0% { transform: scale(0.95); box-shadow: 0 0 0 0 rgba(239, 68, 68, 0.7); }
  70% { transform: scale(1); box-shadow: 0 0 0 10px rgba(239, 68, 68, 0); }
  100% { transform: scale(0.95); box-shadow: 0 0 0 0 rgba(239, 68, 68, 0); }
}

.session-title-large h2 { font-size: 1.25rem; font-weight: 800; }
.session-title-large p { font-size: 0.85rem; opacity: 0.7; }

.dismiss-btn {
  background: rgba(255,255,255,0.1);
  border: none;
  color: white;
  width: 40px;
  height: 40px;
  border-radius: 50%;
  cursor: pointer;
}

.camera-viewport {
  flex: 1;
  position: relative;
  overflow: hidden;
}

.scan-overlay-guides {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 250px;
  height: 250px;
  pointer-events: none;
}

.guide-corner {
  position: absolute;
  width: 40px;
  height: 40px;
  border-color: var(--primary);
  border-style: solid;
}

.top-left { top: 0; left: 0; border-width: 4px 0 0 4px; border-top-left-radius: 20px; }
.top-right { top: 0; right: 0; border-width: 4px 4px 0 0; border-top-right-radius: 20px; }
.bottom-left { bottom: 0; left: 0; border-width: 0 0 4px 4px; border-bottom-left-radius: 20px; }
.bottom-right { bottom: 0; right: 0; border-width: 0 4px 4px 0; border-bottom-right-radius: 20px; }

.scanner-footer-overlay {
  padding: 2rem;
  background: linear-gradient(to top, rgba(0,0,0,0.9), transparent);
  color: white;
  z-index: 10;
}

.stats-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.counter-box .label { font-size: 0.7rem; font-weight: 800; opacity: 0.6; }
.counter-box .count { font-size: 2.5rem; font-weight: 900; line-height: 1; }

.actions-box {
  display: flex;
  align-items: center;
}

.complete-btn {
  background: var(--primary);
  color: white;
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: 12px;
  font-weight: 700;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.9rem;
  cursor: pointer;
  box-shadow: 0 4px 12px rgba(59, 130, 246, 0.3);
  transition: transform 0.2s;
}

.complete-btn:active { transform: scale(0.95); }

.progress-container { width: 100%; }
.progress-labels { display: flex; justify-content: space-between; font-size: 0.8rem; margin-bottom: 0.5rem; font-weight: 700; }

.master-progress {
  height: 8px;
  background: rgba(255,255,255,0.1);
  border-radius: 100px;
  overflow: hidden;
  display: flex;
}

.present-bar { background: var(--primary); transition: width 0.5s ease; }
.absent-bar { background: rgba(255,255,255,0.05); }

.scan-feedback-toast {
  position: absolute;
  bottom: 8rem;
  left: 50%;
  transform: translateX(-50%);
  background: rgba(255,255,255,0.95);
  padding: 1rem 2rem;
  border-radius: 20px;
  display: flex;
  align-items: center;
  gap: 1.5rem;
  min-width: 300px;
  box-shadow: 0 20px 40px rgba(0,0,0,0.4);
  color: #000;
}

.scan-feedback-toast.success { border-left: 6px solid #22c55e; }
.scan-feedback-toast.error { border-left: 6px solid #ef4444; }

.feedback-icon {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 900;
}

.success .feedback-icon { background: #dcfce7; color: #166534; }
.error .feedback-icon { background: #fee2e2; color: #991b1b; }

.feedback-text { display: flex; flex-direction: column; }
.feedback-text .name { font-weight: 800; font-size: 1.1rem; }
.feedback-text .msg { font-size: 0.85rem; opacity: 0.7; }

.slide-up-enter-active, .slide-up-leave-active { transition: all 0.4s cubic-bezier(0.23, 1, 0.32, 1); }
.slide-up-enter-from { opacity: 0; transform: translate(-50%, 20px); }
.slide-up-leave-to { opacity: 0; transform: translate(-50%, -20px); }

@media (max-width: 640px) {
  .stats-row { flex-direction: column; align-items: stretch; gap: 1.5rem; }
}
</style>
