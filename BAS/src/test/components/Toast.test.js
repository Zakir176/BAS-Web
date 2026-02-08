import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import { mount } from '@vue/test-utils'
import { nextTick } from 'vue'
import Toast from '@/components/ui/Toast.vue'

// Mock Heroicons
vi.mock('@heroicons/vue/24/outline', () => ({
  CheckCircleIcon: { template: '<div class="icon-success"></div>' },
  ExclamationTriangleIcon: { template: '<div class="icon-warning"></div>' },
  XCircleIcon: { template: '<div class="icon-error"></div>' },
  InformationCircleIcon: { template: '<div class="icon-info"></div>' },
  XMarkIcon: { template: '<div class="icon-close"></div>' }
}))

describe('Toast Component', () => {
  beforeEach(() => {
    vi.useFakeTimers()
  })

  afterEach(() => {
    vi.useRealTimers()
  })

  it('renders success toast properly', () => {
    const toast = {
      id: '1',
      type: 'success',
      title: 'Success',
      message: 'Success message',
      duration: 3000
    }

    const wrapper = mount(Toast, {
      props: { 
        show: true,
        ...toast
      }
    })

    expect(wrapper.exists()).toBe(true)
    expect(wrapper.text()).toContain('Success message')
    expect(wrapper.text()).toContain('Success')
    expect(wrapper.classes()).toContain('toast-notification')
    expect(wrapper.classes()).toContain('toast-success')
    expect(wrapper.find('.toast-icon').exists()).toBe(true)
    expect(wrapper.find('.icon-success').exists()).toBe(true)
  })

  it('renders error toast properly', () => {
    const toast = {
      id: '1',
      type: 'error',
      title: 'Error',
      message: 'Error message',
      duration: 3000
    }

    const wrapper = mount(Toast, {
      props: { 
        show: true,
        ...toast
      }
    })

    expect(wrapper.classes()).toContain('toast-error')
    expect(wrapper.text()).toContain('Error message')
    expect(wrapper.find('.icon-error').exists()).toBe(true)
  })

  it('renders warning toast properly', () => {
    const toast = {
      id: '1',
      type: 'warning',
      title: 'Warning',
      message: 'Warning message',
      duration: 3000
    }

    const wrapper = mount(Toast, {
      props: { 
        show: true,
        ...toast
      }
    })

    expect(wrapper.classes()).toContain('toast-warning')
    expect(wrapper.text()).toContain('Warning message')
    expect(wrapper.find('.icon-warning').exists()).toBe(true)
  })

  it('renders info toast properly', () => {
    const toast = {
      id: '1',
      type: 'info',
      title: 'Info',
      message: 'Info message',
      duration: 3000
    }

    const wrapper = mount(Toast, {
      props: { 
        show: true,
        ...toast
      }
    })

    expect(wrapper.classes()).toContain('toast-info')
    expect(wrapper.text()).toContain('Info message')
    expect(wrapper.find('.icon-info').exists()).toBe(true)
  })

  it('shows progress bar when progress is enabled', () => {
    const toast = {
      id: '1',
      type: 'success',
      title: 'Success',
      message: 'Progress message',
      duration: 3000,
      showProgress: true
    }

    const wrapper = mount(Toast, {
      props: { 
        show: true,
        ...toast
      }
    })

    expect(wrapper.find('.toast-progress').exists()).toBe(true)
  })

  it('hides progress bar when progress is disabled', () => {
    const toast = {
      id: '1',
      type: 'success',
      title: 'Success',
      message: 'No progress message',
      duration: 3000,
      showProgress: false
    }

    const wrapper = mount(Toast, {
      props: { 
        show: true,
        ...toast
      }
    })

    expect(wrapper.find('.toast-progress').exists()).toBe(false)
  })

  it('dismisses when close button is clicked', async () => {
    const toast = {
      id: '1',
      type: 'success',
      title: 'Success',
      message: 'Manual dismiss',
      duration: 3000,
      closable: true
    }

    const wrapper = mount(Toast, {
      props: { 
        show: true,
        ...toast
      }
    })

    const closeButton = wrapper.find('.toast-close')
    expect(closeButton.exists()).toBe(true)

    await closeButton.trigger('click')

    expect(wrapper.emitted('close')).toBeTruthy()
    expect(wrapper.emitted('close')).toHaveLength(1)
  })

  it('does not show close button when not closable', () => {
    const toast = {
      id: '1',
      type: 'success',
      title: 'Success',
      message: 'No close button',
      duration: 3000,
      closable: false
    }

    const wrapper = mount(Toast, {
      props: { 
        show: true,
        ...toast
      }
    })

    expect(wrapper.find('.toast-close').exists()).toBe(false)
    expect(wrapper.classes()).not.toContain('toast-closable')
  })

  it('shows closable class when closable', () => {
    const toast = {
      id: '1',
      type: 'success',
      title: 'Success',
      message: 'Closable toast',
      duration: 3000,
      closable: true
    }

    const wrapper = mount(Toast, {
      props: { 
        show: true,
        ...toast
      }
    })

    expect(wrapper.classes()).toContain('toast-closable')
  })

  it('renders with title and message', () => {
    const toast = {
      id: '1',
      type: 'info',
      title: 'Test Title',
      message: 'Test Message',
      duration: 3000
    }

    const wrapper = mount(Toast, {
      props: { 
        show: true,
        ...toast
      }
    })

    expect(wrapper.find('.toast-title').exists()).toBe(true)
    expect(wrapper.find('.toast-title').text()).toBe('Test Title')
    expect(wrapper.find('.toast-message').exists()).toBe(true)
    expect(wrapper.find('.toast-message').text()).toBe('Test Message')
  })

  it('renders without title when not provided', () => {
    const toast = {
      id: '1',
      type: 'info',
      message: 'Message only',
      duration: 3000
    }

    const wrapper = mount(Toast, {
      props: { 
        show: true,
        ...toast
      }
    })

    expect(wrapper.find('.toast-title').exists()).toBe(false)
    expect(wrapper.find('.toast-message').exists()).toBe(true)
    expect(wrapper.text()).toContain('Message only')
  })

  it('renders without message when not provided', () => {
    const toast = {
      id: '1',
      type: 'info',
      title: 'Title only',
      duration: 3000
    }

    const wrapper = mount(Toast, {
      props: { 
        show: true,
        ...toast
      }
    })

    expect(wrapper.find('.toast-title').exists()).toBe(true)
    expect(wrapper.find('.toast-message').exists()).toBe(false)
    expect(wrapper.text()).toContain('Title only')
  })

  it('does not render when show is false', () => {
    const toast = {
      id: '1',
      type: 'success',
      title: 'Success',
      message: 'Hidden toast',
      duration: 3000
    }

    const wrapper = mount(Toast, {
      props: { 
        show: false,
        ...toast
      }
    })

    expect(wrapper.find('.toast-notification').exists()).toBe(false)
  })

  it('renders with proper structure', () => {
    const toast = {
      id: '1',
      type: 'success',
      title: 'Success',
      message: 'Structured toast',
      duration: 3000
    }

    const wrapper = mount(Toast, {
      props: { 
        show: true,
        ...toast
      }
    })

    // Check that the main structure exists
    expect(wrapper.find('.toast-notification').exists()).toBe(true)
    expect(wrapper.find('.toast-icon').exists()).toBe(true)
    expect(wrapper.find('.toast-content').exists()).toBe(true)
  })

  it('handles click events when onClick is provided', async () => {
    const mockOnClick = vi.fn()
    const toast = {
      id: '1',
      type: 'success',
      title: 'Success',
      message: 'Clickable toast',
      duration: 3000,
      onClick: mockOnClick
    }

    const wrapper = mount(Toast, {
      props: { 
        show: true,
        ...toast
      }
    })

    await wrapper.find('.toast-notification').trigger('click')
    expect(mockOnClick).toHaveBeenCalled()
  })
})
