import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest'
import { mount } from '@vue/test-utils'
import BarcodeScanner from '@/components/lecturer/BarcodeScanner.vue'
import Quagga from '@ericblade/quagga2'

describe('BarcodeScanner.vue', () => {
  beforeEach(() => {
    // Reset mocks before each test
    vi.clearAllMocks()
    // Restore the default mock implementation
    Quagga.init.mockImplementation((config, callback) => {
      if (callback) callback()
    })
  })

  it('renders the component', () => {
    const wrapper = mount(BarcodeScanner)
    expect(wrapper.find('.barcode-scanner').exists()).toBe(true)
    expect(wrapper.find('.scanner-viewport').exists()).toBe(true)
  })

  it('initializes Quagga on mount', () => {
    mount(BarcodeScanner)
    expect(Quagga.init).toHaveBeenCalledTimes(1)
    expect(Quagga.start).toHaveBeenCalledTimes(1)
  })

  it('displays an error message if Quagga init fails', async () => {
    const error = { name: 'NotAllowedError' }
    Quagga.init.mockImplementation((config, callback) => {
      if (callback) callback(error)
    })
    const wrapper = mount(BarcodeScanner)
    await wrapper.vm.$nextTick()
    expect(wrapper.find('.bg-red-800').exists()).toBe(true)
    expect(wrapper.text()).toContain('Camera access failed. Please ensure you have a camera and grant permissions.')
  })

  it('emits "detected" event when a barcode is detected', async () => {
    const wrapper = mount(BarcodeScanner)
    const barcode = '123456789'

    // Simulate a barcode detection
    Quagga.triggerDetected({ codeResult: { code: barcode } })

    await wrapper.vm.$nextTick()
    expect(wrapper.emitted().detected).toBeTruthy()
    expect(wrapper.emitted().detected[0]).toEqual([barcode])
  })

  it('shows visual feedback on barcode detection', async () => {
    const wrapper = mount(BarcodeScanner)
    const barcode = '987654321'

    Quagga.triggerDetected({ codeResult: { code: barcode } })
    await wrapper.vm.$nextTick()

    expect(wrapper.find('.border-green-500').exists()).toBe(true)
    expect(wrapper.find('.bg-green-500').text()).toContain(`Detected: ${barcode}`)
  })

  it('pauses and resumes the scanner', async () => {
    const wrapper = mount(BarcodeScanner)
    
    // Pause the scanner
    await wrapper.find('button.bg-yellow-500').trigger('click')
    expect(Quagga.stop).toHaveBeenCalledTimes(1)
    expect(wrapper.find('button.bg-blue-500').exists()).toBe(true)
    
    // Resume the scanner
    await wrapper.find('button.bg-blue-500').trigger('click')
    expect(Quagga.start).toHaveBeenCalledTimes(2) // 1 on mount, 1 on resume
    expect(wrapper.find('button.bg-yellow-500').exists()).toBe(true)
  })

  it('stops Quagga when the component is unmounted', () => {
    const wrapper = mount(BarcodeScanner)
    wrapper.unmount()
    expect(Quagga.stop).toHaveBeenCalledTimes(1)
  })

  it('does not stop Quagga on unmount if already paused', () => {
    const wrapper = mount(BarcodeScanner)
    
    // Pause the scanner
    wrapper.find('button.bg-yellow-500').trigger('click')
    expect(Quagga.stop).toHaveBeenCalledTimes(1) // Called on pause

    // Unmount
    wrapper.unmount()
    expect(Quagga.stop).toHaveBeenCalledTimes(1) // Should not be called again
  })

  it('respects the cooldown period for emitting events', async () => {
    vi.useFakeTimers()
    const wrapper = mount(BarcodeScanner)
    const barcode = 'abc-123'
    
    // First detection
    Quagga.triggerDetected({ codeResult: { code: barcode } })
    await wrapper.vm.$nextTick()
    
    expect(wrapper.emitted().detected).toHaveLength(1)

    // Second detection within cooldown period
    Quagga.triggerDetected({ codeResult: { code: barcode } })
    await wrapper.vm.$nextTick()

    expect(wrapper.emitted().detected).toHaveLength(1) // Should still be 1

    // Advance time past the cooldown
    vi.advanceTimersByTime(2100)
    
    // Third detection after cooldown
    Quagga.triggerDetected({ codeResult: { code: barcode } })
    await wrapper.vm.$nextTick()
    
    expect(wrapper.emitted().detected).toHaveLength(2)

    vi.useRealTimers()
  })
})
