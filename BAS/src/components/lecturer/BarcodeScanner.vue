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
        facingMode: { ideal: "environment" }
      },
    },
    locator: {
      patchSize: "large",
      halfSample: true
    },
    decoder: {
      readers: [
        "code_128_reader",
        "ean_reader",
        "ean_8_reader",
        "code_39_reader",
        "code_39_vin_reader",
        "codabar_reader",
        "upc_reader",
        "upc_e_reader",
        "i2of5_reader",
        "code_93_reader"
      ]
    },
    frequency: 10
  }, (err) => {
    if (err) {
      console.error('Quagga init failed:', err)
      // Fallback for desktop/specific devices
      if (err.name === "OverconstrainedError" || err.name === "NotAllowedError") {
        alert("Camera access failed. Please check permissions.")
      }
      return
    }
    Quagga.start()
  })

  let lastEmitTime = 0
  const cooldown = 2000 // 2 seconds

  Quagga.onDetected((data) => {
    if (!data.codeResult || !data.codeResult.code) return
    
    const now = Date.now()
    if (now - lastEmitTime > cooldown) {
      lastEmitTime = now
      console.log('Barcode detected:', data.codeResult.code)
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
