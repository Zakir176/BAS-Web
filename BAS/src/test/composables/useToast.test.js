import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import { ref } from 'vue'
import { useToast } from '@/composables/useToast'

describe('useToast Composable', () => {
  beforeEach(() => {
    vi.useFakeTimers()
    // Reset global state by clearing toasts array directly
    const { toasts } = useToast()
    toasts.value = []
  })

  afterEach(() => {
    vi.useRealTimers()
  })

  it('initializes with empty toasts array', () => {
    const { toasts } = useToast()
    expect(toasts.value).toEqual([])
  })

  it('adds success toast', () => {
    const { toast, toasts } = useToast()
    
    const toastId = toast.success('Success message')
    
    expect(toasts.value).toHaveLength(1)
    expect(toasts.value[0]).toMatchObject({
      type: 'success',
      message: 'Success message',
      duration: 5000
    })
    expect(toasts.value[0].id).toBe(toastId)
    expect(toasts.value[0].show).toBe(true)
  })

  it('adds error toast', () => {
    const { toast, toasts } = useToast()
    
    toast.error('Error message')
    
    expect(toasts.value).toHaveLength(1)
    expect(toasts.value[0]).toMatchObject({
      type: 'error',
      message: 'Error message',
      duration: 8000
    })
  })

  it('adds warning toast', () => {
    const { toast, toasts } = useToast()
    
    toast.warning('Warning message')
    
    expect(toasts.value).toHaveLength(1)
    expect(toasts.value[0]).toMatchObject({
      type: 'warning',
      message: 'Warning message',
      duration: 5000
    })
  })

  it('adds info toast', () => {
    const { toast, toasts } = useToast()
    
    toast.info('Info message')
    
    expect(toasts.value).toHaveLength(1)
    expect(toasts.value[0]).toMatchObject({
      type: 'info',
      message: 'Info message',
      duration: 5000
    })
  })

  it('adds toast with custom options', () => {
    const { toasts, addToast } = useToast()
    
    const toastId = addToast({
      type: 'success',
      message: 'Custom message',
      duration: 1000,
      showProgress: true,
      icon: '🎉'
    })
    
    expect(toasts.value).toHaveLength(1)
    expect(toasts.value[0]).toMatchObject({
      type: 'success',
      message: 'Custom message',
      duration: 1000,
      showProgress: true,
      icon: '🎉'
    })
    expect(toasts.value[0].id).toBe(toastId)
  })

  it('removes toast manually', () => {
    const { toast, toasts, removeToast } = useToast()
    
    const toastId = toast.success('Test message')
    expect(toasts.value).toHaveLength(1)
    
    removeToast(toastId)
    expect(toasts.value).toHaveLength(0)
  })

  it('auto-removes toast after duration', () => {
    const { toast, toasts } = useToast()
    
    toast.success('Auto remove message')
    expect(toasts.value).toHaveLength(1)
    
    vi.advanceTimersByTime(5000)
    
    expect(toasts.value).toHaveLength(0)
  })

  it('does not auto-remove when duration is 0', () => {
    const { toast, toasts } = useToast()
    
    toast.success('No auto remove', { duration: 0 })
    expect(toasts.value).toHaveLength(1)
    
    vi.advanceTimersByTime(10000)
    
    expect(toasts.value).toHaveLength(1)
  })

  it('clears all toasts', () => {
    const { toast, toasts, clearAllToasts } = useToast()
    
    toast.success('Message 1')
    toast.error('Message 2')
    toast.warning('Message 3')
    
    expect(toasts.value).toHaveLength(3)
    
    clearAllToasts()
    
    expect(toasts.value).toHaveLength(0)
  })

  it('generates unique IDs for each toast', () => {
    const { toast } = useToast()
    
    const id1 = toast.success('Message 1')
    const id2 = toast.success('Message 2')
    const id3 = toast.success('Message 3')
    
    expect(id1).not.toBe(id2)
    expect(id2).not.toBe(id3)
    expect(id1).not.toBe(id3)
  })

  it('handles default toast type', () => {
    const { toasts, addToast } = useToast()
    
    const toastId = addToast({
      message: 'Default message'
    })
    
    expect(toasts.value).toHaveLength(1)
    expect(toasts.value[0]).toMatchObject({
      type: 'info',
      message: 'Default message',
      duration: 5000
    })
  })

  it('does not have maximum limit by default', () => {
    const { toast, toasts } = useToast()
    
    // Add more toasts than a typical limit
    for (let i = 0; i < 15; i++) {
      toast.success(`Message ${i}`)
    }
    
    // Should have all toasts since there's no limit in the implementation
    expect(toasts.value).toHaveLength(15)
  })
})
