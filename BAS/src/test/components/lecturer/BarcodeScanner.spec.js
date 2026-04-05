import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount } from '@vue/test-utils'
import BarcodeScanner from '@/features/scanner/BarcodeScanner.vue'
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
    expect(wrapper.find('.scanner-error-overlay').exists()).toBe(true)
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

  it('stops Quagga when the component is unmounted', () => {
    const wrapper = mount(BarcodeScanner)
    wrapper.unmount()
    expect(Quagga.stop).toHaveBeenCalledTimes(1)
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

    // No extra emissions due to cooldown
    expect(wrapper.emitted().detected).toHaveLength(1) 

    // Advance time past the cooldown (2000ms delay)
    vi.advanceTimersByTime(2100)
    
    // Third detection after cooldown
    Quagga.triggerDetected({ codeResult: { code: barcode } })
    await wrapper.vm.$nextTick()
    
    expect(wrapper.emitted().detected).toHaveLength(2)

    vi.useRealTimers()
  })
})
