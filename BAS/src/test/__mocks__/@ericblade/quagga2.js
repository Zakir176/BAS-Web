// BAS/src/test/__mocks__/@ericblade/quagga2.js
import { vi } from 'vitest'

const Quagga = {
  init: vi.fn((config, callback) => {
    if (callback) {
      callback() // Simulate successful initialization by default
    }
  }),
  start: vi.fn(),
  stop: vi.fn(),
  onDetected: vi.fn((callback) => {
    // Store the callback to simulate detection later
    Quagga.triggerDetected = (data) => callback(data)
  }),
  triggerDetected: null, // This will be assigned in onDetected
  offDetected: vi.fn(),
}

export default Quagga
