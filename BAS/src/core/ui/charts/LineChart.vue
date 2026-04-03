<template>
  <Line :data="mergedData" :options="mergedOptions" />
</template>

<script setup>
import { computed } from 'vue'
import { Line } from 'vue-chartjs'
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  LineElement,
  PointElement,
  CategoryScale,
  LinearScale,
  Filler
} from 'chart.js'

ChartJS.register(
  Title,
  Tooltip,
  Legend,
  LineElement,
  PointElement,
  CategoryScale,
  LinearScale,
  Filler
)

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
  interaction: {
    intersect: false,
    mode: 'index'
  },
  plugins: {
    legend: { display: false },
    tooltip: {
      backgroundColor: 'rgba(15, 23, 42, 0.95)',
      titleColor: getComputedStyle(document.documentElement).getPropertyValue('--text-inverse').trim(),
      bodyColor: getComputedStyle(document.documentElement).getPropertyValue('--text-inverse').trim(),
      titleFont: { family: "'Inter', sans-serif", size: 13, weight: 'bold' },
      bodyFont: { family: "'Inter', sans-serif", size: 12 },
      padding: 14,
      cornerRadius: 12,
      displayColors: true,
      usePointStyle: true,
      borderColor: 'rgba(255, 255, 255, 0.1)',
      borderWidth: 1,
      boxPadding: 6,
      // Subtle glow shadow via transparency and border
      callbacks: {
        labelColor: function(context) {
          return {
            borderColor: context.dataset.borderColor,
            backgroundColor: context.dataset.borderColor,
            borderWidth: 2,
            borderRadius: 2,
          }
        }
      }
    }
  },
  elements: {
    line: {
      tension: 0.45,
      borderWidth: 3,
      capBezierPoints: true
    },
    point: {
      radius: 0, // hide by default
      hitRadius: 20,
      hoverRadius: 6,
      hoverBorderWidth: 4,
      hoverBackgroundColor: getComputedStyle(document.documentElement).getPropertyValue('--text-inverse').trim()
    }
  },
  scales: {
    y: {
      beginAtZero: true,
      grid: {
        display: false, // minimalist look
        drawBorder: false
      },
      ticks: {
        color: 'rgba(100, 116, 139, 0.4)',
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

// Function to create gradients for datasets
const mergedData = computed(() => {
  const data = JSON.parse(JSON.stringify(props.chartData))
  if (data.datasets) {
    data.datasets.forEach(ds => {
      ds.fill = true
      // We pass a function that Chart.js will call with the context
      ds.backgroundColor = (context) => {
        const chart = context.chart
        const {ctx, chartArea} = chart
        if (!chartArea) return null
        const color = ds.borderColor || getComputedStyle(document.documentElement).getPropertyValue('--primary').trim()
        const gradient = ctx.createLinearGradient(0, chartArea.bottom, 0, chartArea.top)
        gradient.addColorStop(0, 'rgba(255, 255, 255, 0)')
        gradient.addColorStop(1, color.replace('1)', '0.2)').replace('rgb', 'rgba')) // soft variant
        return gradient
      }
    })
  }
  return data
})

const mergedOptions = computed(() => {
  return { ...defaultOptions, ...props.chartOptions }
})
</script>
