<template>
  <div class="barcode-scanner">
    <div ref="scanner" class="scanner-viewport"></div>
  </div>
</template>

<script setup>
import { onMounted, onBeforeUnmount, ref } from 'vue'
import Quagga from 'quagga'

const scanner = ref(null)
const emit = defineEmits(['detected'])

onMounted(() => {
  Quagga.init({
    inputStream: {
      name: "Live",
      type: "LiveStream",
      target: scanner.value,
      constraints: {
        width: 480,
        height: 320,
        facingMode: "environment"
      },
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
        "i2of5_reader"
      ],
      debug: {
        drawBoundingBox: true,
        showFrequency: true,
        drawScanline: true,
        showPattern: true
      }
    },
  }, (err) => {
    if (err) {
      console.error(err)
      return
    }
    Quagga.start()
  })

  Quagga.onDetected((data) => {
    emit('detected', data.codeResult.code)
  })
})

onBeforeUnmount(() => {
  Quagga.stop()
})
</script>

<style scoped>
.barcode-scanner {
  position: relative;
  width: 100%;
  max-width: 640px;
  margin: 0 auto;
}

.scanner-viewport {
  width: 100%;
  height: auto;
  overflow: hidden;
  border-radius: 8px;
}
</style>
