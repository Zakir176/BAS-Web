<template>
  <Bar :data="mergedData" :options="mergedOptions" />
</template>

<script setup>
import { computed } from 'vue'
import { Bar } from 'vue-chartjs'
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  BarElement,
  CategoryScale,
  LinearScale
} from 'chart.js'

ChartJS.register(Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale)

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
  plugins: {
    legend: { display: false },
    tooltip: {
      backgroundColor: 'rgba(15, 23, 42, 0.95)',
      titleColor: '#fff',
      bodyColor: '#fff',
      titleFont: { family: "'Inter', sans-serif", size: 13, weight: 'bold' },
      bodyFont: { family: "'Inter', sans-serif", size: 12 },
      padding: 14,
      cornerRadius: 12,
      displayColors: true,
      usePointStyle: true,
      borderColor: 'rgba(255, 255, 255, 0.1)',
      borderWidth: 1,
      boxPadding: 6,
      callbacks: {
        labelColor: function(context) {
          return {
            borderColor: context.dataset.backgroundColor[context.dataIndex] || context.dataset.backgroundColor,
            backgroundColor: context.dataset.backgroundColor[context.dataIndex] || context.dataset.backgroundColor,
            borderWidth: 2,
            borderRadius: 2,
          }
        }
      }
    }
  },
  scales: {
    y: {
      beginAtZero: true,
      grid: {
        display: false,
        drawBorder: false
      },
      ticks: {
        color: 'rgba(100, 116, 139, 0.5)',
        maxTicksLimit: 5,
        font: { family: "'Inter', sans-serif", size: 10 }
      }
    },
    x: {
      grid: {
        display: false,
        drawBorder: false
      },
      ticks: {
        color: 'rgba(100, 116, 139, 0.5)',
        font: { family: "'Inter', sans-serif", size: 10 }
      }
    }
  }
}

const mergedData = computed(() => {
  const data = JSON.parse(JSON.stringify(props.chartData))
  if (data.datasets) {
    data.datasets.forEach(ds => {
      // For bars, we can use a function or just standard colors if they look good, 
      // but let's try to add a subtle gradient if possible
      ds.borderRadius = 6
      ds.hoverBorderWidth = 2
      ds.hoverBorderColor = 'rgba(255, 255, 255, 0.5)'
    })
  }
  return data
})

const mergedOptions = computed(() => {
  return { ...defaultOptions, ...props.chartOptions }
})
</script>
