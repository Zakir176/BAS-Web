<template>
  <div class="scanner-interface" :class="{ 'is-mobile': isMobile, 'is-web': !isMobile }">
    <!-- IMERSIVE HUD OVERLAY (WEB ONLY) -->
    <div v-if="!isMobile" class="web-pro-hud">
      <div class="hud-frame">
        <div class="hud-corner tl"><span>RADAR_ACTIVE</span></div>
        <div class="hud-corner tr"><span>SENSORS_OK</span></div>
        <div class="hud-corner bl"><span>LOC_LOCKED</span></div>
        <div class="hud-corner br"><span>DATA_SYNC</span></div>
      </div>
      <div class="holographic-focus"></div>
    </div>

    <!-- Scanner Header (Native Style) -->
    <div class="scanner-header">
      <div class="back-btn-wrap" @click="$emit('close')">
        <span class="back-icon">←</span>
      </div>
      <div class="session-info">
        <h3>{{ sessionName || 'New Session' }}</h3>
        <p>{{ dateDisplay }}</p>
      </div>
      <div class="settings-icon">⚙</div>
    </div>

    <div class="main-scrolling-content">
      <!-- Camera Section -->
      <div class="camera-section">
        <div class="camera-border">
          <BarcodeScanner @detected="handleDetection" />
          <div class="scan-overlay">
            <div class="scan-bracket"></div>
            <div class="scan-active-badge">
              <span class="pulse"></span> SCANNING ACTIVE
            </div>
          </div>
          <div class="elegant-beam"></div>
        </div>
      </div>

      <!-- Feedback Toast (Inline Native Style) -->
      <transition name="toast-slide">
        <div v-if="localLastScanned" class="native-toast success">
          <div class="toast-check">✔</div>
          <div class="toast-content">
            <span class="toast-label">ATTENDANCE LOGGED</span>
            <span class="toast-msg"><strong>{{ localLastScanned }}</strong> marked Present</span>
          </div>
          <button class="undo-btn" @click="undoLastScan">Undo</button>
        </div>
      </transition>

      <!-- Stats Grid (Twin Cards) -->
      <div class="stats-grid">
        <div class="stat-card success-glow">
          <span class="card-label">PRESENT</span>
          <div class="card-value-row">
            <span class="big-val">{{ scannedCount }}</span>
            <span class="total-val">/ {{ totalStudents }}</span>
          </div>
          <div class="mini-progress"><div class="fill" :style="{ width: progress + '%' }"></div></div>
        </div>
        <div class="stat-card error-glow">
          <span class="card-label">ABSENT</span>
          <div class="card-value-row">
            <span class="big-val">{{ totalStudents - scannedCount }}</span>
            <span class="total-val">/ {{ totalStudents }}</span>
          </div>
          <div class="mini-progress"><div class="fill error" :style="{ width: (100 - progress) + '%' }"></div></div>
        </div>
      </div>

      <!-- Roster Section (Integrated) -->
      <div class="roster-section">
        <div class="roster-header">
          <span class="title">CLASS ROSTER</span>
          <span class="filter">📶 FILTERS</span>
        </div>
        <div class="roster-list">
          <div v-for="student in rosterData" :key="student.id" class="roster-item">
            <div class="student-avatar">{{ getInitials(student.full_name) }}</div>
            <div class="student-info">
              <span class="name">{{ student.full_name }}</span>
              <span class="id">{{ student.student_number }}</span>
            </div>
            <div class="status-btn" :class="{ 'present': student.present }">
              {{ student.present ? 'PRESENT' : 'ABSENT' }}
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Finish Button (Floating) -->
    <div class="footer-action">
      <button class="finish-btn" @click="$emit('complete')">
        ✓ FINISH SESSION
      </button>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted, watch } from 'vue'
import BarcodeScanner from './BarcodeScanner.vue'

const props = defineProps({
  sessionName: { type: String, default: '' },
  scannedCount: { type: Number, default: 0 },
  totalStudents: { type: Number, default: 0 },
  lastScanned: { type: String, default: '' },
  scanStatus: { type: String, default: 'success' },
  roster: { type: Array, default: () => [] }
})

const emit = defineEmits(['close', 'detected', 'complete'])
const isMobile = ref(false)
const localLastScanned = ref('')

const dateDisplay = computed(() => {
  return new Date().toLocaleDateString('en-US', { weekday: 'short', month: 'short', day: '2-digit', year: 'numeric' })
})

const progress = computed(() => {
  if (props.totalStudents === 0) return 0
  return Math.min(100, Math.round((props.scannedCount / props.totalStudents) * 100))
})

const rosterData = computed(() => {
  // Mock sorting: present students at top
  return [...props.roster].sort((a,b) => (b.present === a.present) ? 0 : b.present ? -1 : 1)
})

const handleDetection = (barcode) => {
  emit('detected', barcode)
}

const undoLastScan = () => {
  localLastScanned.value = ''
  // Logic to undo would need to emit back to parent
}

const getInitials = (name) => {
  if (!name) return '?'
  return name.split(' ').map(n => n[0]).join('').toUpperCase().substring(0, 2)
}

const checkMobile = () => {
  isMobile.value = window.innerWidth <= 768
}

onMounted(() => {
  checkMobile()
  window.addEventListener('resize', checkMobile)
})

onUnmounted(() => {
  window.removeEventListener('resize', checkMobile)
})

// Sync local state for toast
watch(() => props.lastScanned, (newVal) => {
  if (newVal) {
    localLastScanned.value = newVal
    setTimeout(() => {
      localLastScanned.value = ''
    }, 4000)
  }
})
</script>

<style scoped>
.scanner-interface {
  position: fixed;
  inset: 0;
  background: var(--bg-main);
  z-index: 2500;
  display: flex;
  flex-direction: column;
  overflow: hidden;
  color: var(--text-main);
}

/* === WEB HUD (ELEGANT CRAZY) === */
.web-pro-hud {
  position: absolute;
  inset: 0;
  pointer-events: none;
  z-index: 50;
  display: flex;
  align-items: center;
  justify-content: center;
}

.holographic-focus {
  width: 400px; height: 400px;
  background: radial-gradient(circle, rgba(96, 165, 250, 0.05) 0%, transparent 70%);
  border-radius: 50%;
  animation: pulse-focus 4s infinite ease-in-out;
}

@keyframes pulse-focus { 0%, 100% { transform: scale(0.9); opacity: 0.3; } 50% { transform: scale(1.1); opacity: 0.6; } }

.hud-frame { position: absolute; inset: 40px; border: 1px solid rgba(255,255,255,0.02); }
.hud-corner { position: absolute; font-family: 'JetBrains Mono', monospace; font-size: 0.6rem; color: var(--primary); opacity: 0.3; }
.hud-corner.tl { top: 0; left: 0; border-top: 1px solid currentColor; border-left: 1px solid currentColor; padding: 4px; }
.hud-corner.tr { top: 0; right: 0; border-top: 1px solid currentColor; border-right: 1px solid currentColor; padding: 4px; }
.hud-corner.bl { bottom: 0; left: 0; border-bottom: 1px solid currentColor; border-left: 1px solid currentColor; padding: 4px; }
.hud-corner.br { bottom: 0; right: 0; border-bottom: 1px solid currentColor; border-right: 1px solid currentColor; padding: 4px; }

/* === HEADER === */
.scanner-header {
  padding: 1rem 1.5rem;
  background: var(--bg-card);
  border-bottom: 1px solid var(--border-light);
  display: flex;
  align-items: center;
  gap: 1rem;
  z-index: 100;
}

.back-btn-wrap {
  width: 40px; height: 40px;
  background: var(--bg-panel);
  border-radius: 50%;
  display: flex; align-items: center; justify-content: center;
  cursor: pointer;
}
.back-icon { font-size: 1.2rem; }

.session-info { flex: 1; text-align: center; }
.session-info h3 { margin: 0; font-size: 1.1rem; font-weight: 850; }
.session-info p { margin: 0; font-size: 0.75rem; color: var(--text-muted); font-weight: 600; }

.settings-icon { font-size: 1.5rem; color: var(--text-muted); cursor: pointer; }

/* === MAIN SCROLLING AREA === */
.main-scrolling-content {
  flex: 1;
  overflow-y: auto;
  padding: 1.5rem;
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
  max-width: 600px;
  margin: 0 auto;
  width: 100%;
}

/* Camera Section */
.camera-section { width: 100%; }
.camera-border {
  height: 250px;
  border: 1.5px solid var(--primary);
  border-radius: 24px;
  position: relative;
  overflow: hidden;
  background: black;
}

.scan-overlay {
  position: absolute;
  inset: 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  pointer-events: none;
}

.scan-bracket {
  width: 200px; height: 150px;
  border: 4px solid var(--primary);
  border-radius: 12px;
  opacity: 0.8;
}

.scan-active-badge {
  position: absolute;
  bottom: 20px;
  background: rgba(30, 41, 59, 0.8);
  backdrop-filter: blur(4px);
  padding: 0.5rem 1rem;
  border-radius: 20px;
  border: 1px solid var(--border-light);
  display: flex; align-items: center; gap: 0.5rem;
  font-size: 0.75rem; font-weight: 800; color: var(--primary);
}

.elegant-beam {
  position: absolute;
  top: 0; left: 0; width: 100%; height: 2px;
  background: linear-gradient(90deg, transparent, var(--primary), transparent);
  box-shadow: 0 0 10px var(--primary);
  animation: beam-move 3s infinite ease-in-out;
}

@keyframes beam-move { 0%, 100% { top: 10%; opacity: 0.2; } 50% { top: 90%; opacity: 1; } }

/* Feedback Toast */
.native-toast {
  background: var(--bg-card);
  border: 1px solid var(--success);
  border-radius: 16px;
  padding: 1rem;
  display: flex;
  align-items: center;
  gap: 1rem;
  animation: toast-in 0.4s cubic-bezier(0.18, 0.89, 0.32, 1.28);
}

@keyframes toast-in { from { transform: scale(0.9); opacity: 0; } to { transform: scale(1); opacity: 1; } }

.toast-check { width: 32px; height: 32px; background: var(--success-bg); color: var(--success); border-radius: 50%; display: flex; align-items: center; justify-content: center; font-weight: 900; }
.toast-content { flex: 1; display: flex; flex-direction: column; }
.toast-label { font-size: 0.65rem; font-weight: 900; color: var(--success); }
.toast-msg { font-size: 0.85rem; }

.undo-btn {
  background: var(--bg-panel);
  border: 1px solid var(--border-light);
  color: white;
  padding: 0.4rem 0.8rem;
  border-radius: 12px;
  font-size: 0.75rem;
  font-weight: 700;
  cursor: pointer;
}

/* Stats Grid */
.stats-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 1rem; }
.stat-card { background: var(--bg-card); padding: 1rem; border-radius: 20px; border: 1px solid var(--border-light); }
.card-label { font-size: 0.65rem; font-weight: 850; color: var(--text-muted); }
.card-value-row { display: flex; align-items: baseline; gap: 0.5rem; margin: 0.5rem 0; }
.big-val { font-size: 1.75rem; font-weight: 900; line-height: 1; }
.total-val { font-size: 0.8rem; opacity: 0.5; font-weight: 700; }
.mini-progress { height: 4px; background: var(--bg-panel); border-radius: 10px; overflow: hidden; }
.fill { height: 100%; background: var(--success); transition: width 0.5s; }
.fill.error { background: var(--error); }

/* Roster Section */
.roster-section { background: var(--bg-card); border-radius: 20px; border: 1px solid var(--border-light); overflow: hidden; margin-bottom: 5rem; }
.roster-header { padding: 1rem; display: flex; justify-content: space-between; border-bottom: 1px solid var(--border-light); background: rgba(255,255,255,0.02); }
.roster-header .title { font-size: 0.7rem; font-weight: 900; color: var(--text-muted); }
.roster-header .filter { font-size: 0.7rem; font-weight: 900; color: var(--primary); }

.roster-item { padding: 0.75rem 1rem; display: flex; align-items: center; gap: 1rem; border-bottom: 1px solid var(--border-light); }
.student-avatar { width: 44px; height: 44px; background: var(--bg-panel); border-radius: 16px; border: 1px solid var(--border-light); display: flex; align-items: center; justify-content: center; font-weight: 800; color: var(--primary); font-size: 0.8rem; }
.student-info { flex: 1; display: flex; flex-direction: column; }
.student-info .name { font-size: 0.9rem; font-weight: 700; }
.student-info .id { font-size: 0.75rem; color: var(--text-muted); font-weight: 600; }

.status-btn { padding: 0.4rem 0.75rem; border-radius: 12px; font-size: 0.65rem; font-weight: 900; background: var(--error-bg); color: var(--error); }
.status-btn.present { background: var(--success-bg); color: var(--success); }

/* Finish Button */
.footer-action {
  position: absolute;
  bottom: 0; left: 0; right: 0;
  padding: 2rem;
  background: linear-gradient(to top, var(--bg-main) 70%, transparent);
  display: flex; justify-content: center;
  z-index: 100;
}

.finish-btn {
  background: var(--primary);
  color: white;
  border: none;
  padding: 1rem 2rem;
  border-radius: 100px;
  font-weight: 850;
  font-size: 0.95rem;
  box-shadow: 0 10px 25px var(--primary-glow);
  cursor: pointer;
  transition: transform 0.2s;
}
.finish-btn:active { transform: scale(0.95); }

/* === ADAPTIVE === */
@media (min-width: 1000px) {
  .main-scrolling-content { max-width: 800px; display: grid; grid-template-columns: 1fr 1fr; gap: 2rem; padding: 3rem; }
  .camera-section { grid-column: 1; }
  .stats-grid { grid-column: 1; }
  .roster-section { grid-row: 1 / span 3; grid-column: 2; margin-bottom: 0; }
  .native-toast { grid-column: 1; }
  .footer-action { position: relative; padding: 2rem 0; background: none; }
}
</style>
