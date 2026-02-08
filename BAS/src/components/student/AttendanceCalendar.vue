<template>
  <div class="attendance-heatmap">
    <div class="heatmap-header">
      <div class="month-info">
        <h3>{{ monthYear }}</h3>
      </div>
      <div class="nav-controls">
        <button class="nav-btn" @click="prevMonth">←</button>
        <button class="nav-btn" @click="nextMonth">→</button>
      </div>
    </div>

    <div class="heatmap-grid">
      <div v-for="day in weekDays" :key="day" class="weekday-cell">
        {{ day[0] }}
      </div>
      
      <div 
        v-for="date in calendarDays" 
        :key="date.iso"
        class="day-cell"
        :class="{ 
          'is-inactive': !date.isCurrentMonth,
          'is-today': date.isToday,
          'status-present': getStatus(date.iso) === 'Present',
          'status-absent': getStatus(date.iso) === 'Absent',
          'status-excused': getStatus(date.iso) === 'Excused'
        }"
      >
        <span class="day-num">{{ date.day }}</span>
      </div>
    </div>

    <div class="heatmap-legend">
      <div class="legend-item"><span class="box present"></span> Present</div>
      <div class="legend-item"><span class="box absent"></span> Absent</div>
      <div class="legend-item"><span class="box excused"></span> Excused</div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const props = defineProps({
  attendanceRecords: {
    type: Array,
    required: true,
    default: () => []
  }
})

const currentDate = ref(new Date())
const weekDays = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat']

const monthYear = computed(() => {
  return currentDate.value.toLocaleString('default', { month: 'long', year: 'numeric' })
})

const calendarDays = computed(() => {
  const year = currentDate.value.getFullYear()
  const month = currentDate.value.getMonth()
  
  const firstDay = new Date(year, month, 1)
  const lastDay = new Date(year, month + 1, 0)
  const days = []
  
  const startPadding = firstDay.getDay()
  for (let i = startPadding - 1; i >= 0; i--) {
    const d = new Date(year, month, -i)
    days.push(formatDate(d, false))
  }
  
  for (let i = 1; i <= lastDay.getDate(); i++) {
    const d = new Date(year, month, i)
    days.push(formatDate(d, true))
  }
  
  const endPadding = 42 - days.length
  for (let i = 1; i <= endPadding; i++) {
    const d = new Date(year, month + 1, i)
    days.push(formatDate(d, false))
  }
  
  return days
})

const formatDate = (date, isCurrentMonth) => {
  const today = new Date()
  return {
    day: date.getDate(),
    iso: date.toISOString().split('T')[0],
    isCurrentMonth,
    isToday: date.toDateString() === today.toDateString()
  }
}

const getStatus = (date) => {
  const record = props.attendanceRecords.find(r => r.date === date)
  return record ? record.status : null
}

const prevMonth = () => {
  currentDate.value = new Date(currentDate.value.getFullYear(), currentDate.value.getMonth() - 1, 1)
}

const nextMonth = () => {
  currentDate.value = new Date(currentDate.value.getFullYear(), currentDate.value.getMonth() + 1, 1)
}
</script>

<style scoped>
.attendance-heatmap {
  background: transparent;
}

.heatmap-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.25rem;
}

.month-info h3 {
  font-size: 1rem;
  font-weight: 800;
  color: #1e293b;
}

.nav-controls {
  display: flex;
  gap: 0.5rem;
}

.nav-btn {
  width: 32px;
  height: 32px;
  border-radius: 8px;
  border: 1px solid #e2e8f0;
  background: white;
  color: #64748b;
  font-weight: 800;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;
}

.nav-btn:hover {
  background: #f8fafc;
  border-color: #cbd5e1;
}

.heatmap-grid {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 8px;
}

.weekday-cell {
  text-align: center;
  font-size: 0.7rem;
  font-weight: 800;
  color: #94a3b8;
  padding-bottom: 0.5rem;
  text-transform: uppercase;
}

.day-cell {
  aspect-ratio: 1;
  border-radius: 8px;
  background: #f1f5f9;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.75rem;
  font-weight: 700;
  color: #64748b;
  transition: all 0.2s;
  cursor: default;
}

.day-cell.is-inactive {
  opacity: 0.3;
}

.day-cell.is-today {
  box-shadow: inset 0 0 0 2px var(--primary);
}

/* Status variants */
.status-present {
  background: #10b981 !important;
  color: white !important;
}

.status-absent {
  background: #ef4444 !important;
  color: white !important;
}

.status-excused {
  background: #f59e0b !important;
  color: white !important;
}

.heatmap-legend {
  display: flex;
  gap: 1.25rem;
  margin-top: 1.5rem;
  justify-content: center;
  font-size: 0.75rem;
  font-weight: 700;
}

.legend-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: #64748b;
}

.box {
  width: 12px;
  height: 12px;
  border-radius: 3px;
}

.box.present { background: #10b981; }
.box.absent { background: #ef4444; }
.box.excused { background: #f59e0b; }
</style>
