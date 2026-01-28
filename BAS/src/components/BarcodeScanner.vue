<template>
  <div class="barcode-scanner">
    <div ref="scanner" class="scanner-viewport"></div>
  </div>
</template>

<script setup>
import { onMounted, onBeforeUnmount, ref } from 'vue'
import Quagga from '@ericblade/quagga2'

const scanner = ref(null)
const emit = defineEmits(['detected'])

onMounted(() => {
  Quagga.init({
    inputStream: {
      name: "Live",
      type: "LiveStream",
      target: scanner.value,
      constraints: {
        width: { ideal: 1280 },
        height: { ideal: 720 },
        facingMode: "environment"
      },
      area: { // Focus scanning on the center area
        top: "20%",
        right: "20%",
        left: "20%",
        bottom: "20%"
      }
    },
    decoder: {
      readers: [
        "code_128_reader",
        "ean_reader",
        "code_39_reader"
      ]
    },
  }, (err) => {
    if (err) {
      console.error(err)
      return
    }
    Quagga.start()
  })

  let lastEmitTime = 0
  const cooldown = 2000 // 2 seconds

  Quagga.onDetected((data) => {
    const now = Date.now()
    if (now - lastEmitTime > cooldown) {
      lastEmitTime = now
      emit('detected', data.codeResult.code)
    }
  })
})

onBeforeUnmount(() => {
  Quagga.stop()
})
</script>

<style scoped>
.barcode-scanner {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
}

.scanner-viewport {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
}

.scanner-viewport :deep(video) {
  width: 100% !important;
  height: 100% !important;
  object-fit: contain !important; /* contain is safer for maintaining aspect ratio on desktop */
}

.scanner-viewport :deep(canvas) {
  display: none;
}
</style>
