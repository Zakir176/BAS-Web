<template>
  <Doughnut :data="chartData" :options="mergedOptions" />
</template>

<script setup>
import { computed } from 'vue'
import { Doughnut } from 'vue-chartjs'
import { Chart as ChartJS, Title, Tooltip, Legend, ArcElement } from 'chart.js'

ChartJS.register(Title, Tooltip, Legend, ArcElement)

const props = defineProps({
  chartData: {
    type: Object,
    required: true
  },
  chartOptions: {
    type: Object,
    default: () => ({})
  }
})

const defaultOptions = {
  responsive: true,
  maintainAspectRatio: false,
  cutout: '75%', // makes it a doughnut instead of a pie
  plugins: {
    legend: {
      position: 'bottom',
      labels: {
        usePointStyle: true,
        padding: 20,
        color: getComputedStyle(document.documentElement).getPropertyValue('--text-muted').trim(),
        font: {
          family: "'Inter', sans-serif",
          size: 12,
          weight: '600'
        }
      }
    },
    tooltip: {
      backgroundColor: 'rgba(15, 23, 42, 0.9)',
      titleColor: getComputedStyle(document.documentElement).getPropertyValue('--text-inverse').trim(),
      bodyColor: getComputedStyle(document.documentElement).getPropertyValue('--text-inverse').trim(),
      padding: 12,
      cornerRadius: 8,
      displayColors: true,
      boxPadding: 4
    }
  }
}

const mergedOptions = computed(() => {
  return { ...defaultOptions, ...props.chartOptions }
})
</script>
