import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import Button from '@/components/ui/Button.vue'

describe('Button Component', () => {
  it('renders properly with default props', () => {
    const wrapper = mount(Button, {
      props: {
        variant: 'primary',
        size: 'md'
      },
      slots: {
        default: 'Click me'
      }
    })

    expect(wrapper.exists()).toBe(true)
    expect(wrapper.text()).toContain('Click me')
    expect(wrapper.classes()).toContain('btn')
    expect(wrapper.classes()).toContain('btn-primary')
    expect(wrapper.classes()).toContain('px-4')
    expect(wrapper.classes()).toContain('py-2')
    expect(wrapper.classes()).toContain('text-sm')
  })

  it('applies correct variant classes', () => {
    const variants = ['primary', 'secondary', 'white', 'success', 'danger', 'warning']
    
    variants.forEach(variant => {
      const wrapper = mount(Button, {
        props: { variant },
        slots: { default: variant }
      })
      expect(wrapper.classes()).toContain(`btn-${variant}`)
    })
  })

  it('applies correct size classes', () => {
    const sizes = ['sm', 'md', 'lg']
    
    sizes.forEach(size => {
      const wrapper = mount(Button, {
        props: { size },
        slots: { default: size }
      })
      expect(wrapper.classes()).toContain(`px-${size === 'sm' ? '3' : size === 'md' ? '4' : '6'}`)
      expect(wrapper.classes()).toContain(`py-${size === 'sm' ? '1.5' : size === 'md' ? '2' : '3'}`)
      
      // Check text size based on actual implementation
      if (size === 'lg') {
        expect(wrapper.classes()).toContain('text-base')
      } else {
        expect(wrapper.classes()).toContain('text-sm')
      }
    })
  })

  it('handles click events', async () => {
    const wrapper = mount(Button, {
      props: {
        variant: 'primary',
        size: 'md'
      },
      slots: {
        default: 'Click me'
      }
    })

    await wrapper.trigger('click')
    expect(wrapper.emitted()).toHaveProperty('click')
    expect(wrapper.emitted().click).toHaveLength(1)
  })

  it('can be disabled', async () => {
    const wrapper = mount(Button, {
      props: {
        variant: 'primary',
        size: 'md',
        disabled: true
      },
      slots: {
        default: 'Disabled Button'
      }
    })

    expect(wrapper.attributes('disabled')).toBeDefined()
    expect(wrapper.classes()).toContain('btn')
    expect(wrapper.classes()).toContain('btn-primary')
    expect(wrapper.classes()).toContain('px-4')
    expect(wrapper.classes()).toContain('py-2')
    expect(wrapper.classes()).toContain('text-sm')
    
    await wrapper.trigger('click')
    expect(wrapper.emitted('click')).toBeUndefined()
  })

  it('renders as full width when specified', () => {
    const wrapper = mount(Button, {
      props: {
        variant: 'primary',
        size: 'md',
        fullWidth: true
      },
      slots: {
        default: 'Full Width Button'
      }
    })

    expect(wrapper.classes()).toContain('btn')
    expect(wrapper.classes()).toContain('btn-primary')
    expect(wrapper.classes()).toContain('px-4')
    expect(wrapper.classes()).toContain('py-2')
    expect(wrapper.classes()).toContain('text-sm')
    expect(wrapper.classes()).toContain('w-full')
  })

  it('renders loading state correctly', () => {
    const wrapper = mount(Button, {
      props: {
        variant: 'primary',
        size: 'md',
        loading: true
      },
      slots: {
        default: 'Loading...'
      }
    })

    expect(wrapper.classes()).toContain('btn')
    expect(wrapper.classes()).toContain('btn-primary')
    expect(wrapper.classes()).toContain('px-4')
    expect(wrapper.classes()).toContain('py-2')
    expect(wrapper.classes()).toContain('text-sm')
    expect(wrapper.text()).toContain('Loading...')
  })
})
