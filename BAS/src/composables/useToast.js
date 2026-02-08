import { ref } from 'vue'

const toasts = ref([])
let toastId = 0

export function useToast() {
  const addToast = (options) => {
    const id = ++toastId
    const toast = {
      id,
      show: true,
      type: 'info',
      title: '',
      message: '',
      duration: 5000,
      closable: true,
      showProgress: true,
      pauseOnHover: true,
      onClick: null,
      ...options
    }

    toasts.value.push(toast)

    // Auto remove after duration
    if (toast.duration > 0) {
      setTimeout(() => {
        removeToast(id)
      }, toast.duration)
    }

    return id
  }

  const removeToast = (id) => {
    const index = toasts.value.findIndex(toast => toast.id === id)
    if (index > -1) {
      toasts.value.splice(index, 1)
    }
  }

  const clearAllToasts = () => {
    toasts.value = []
  }

  // Convenience methods
  const success = (message, options = {}) => {
    return addToast({
      type: 'success',
      message,
      ...options
    })
  }

  const error = (message, options = {}) => {
    return addToast({
      type: 'error',
      message,
      duration: 8000, // Errors stay longer
      ...options
    })
  }

  const warning = (message, options = {}) => {
    return addToast({
      type: 'warning',
      message,
      ...options
    })
  }

  const info = (message, options = {}) => {
    return addToast({
      type: 'info',
      message,
      ...options
    })
  }

  // Advanced toast methods
  const promise = (promise, options = {}) => {
    const {
      loading = 'Processing...',
      success = 'Operation completed successfully!',
      error = 'Something went wrong',
      ...toastOptions
    } = options

    const loadingToast = addToast({
      type: 'info',
      message: loading,
      duration: 0, // No auto-dismiss
      closable: false,
      showProgress: false,
      ...toastOptions
    })

    return promise
      .then((result) => {
        removeToast(loadingToast)
        success(typeof success === 'function' ? success(result) : success, toastOptions)
        return result
      })
      .catch((err) => {
        removeToast(loadingToast)
        error(typeof error === 'function' ? error(err) : error, toastOptions)
        throw err
      })
  }

  const confirm = (message, options = {}) => {
    const {
      title = 'Confirm Action',
      confirmText = 'Confirm',
      cancelText = 'Cancel',
      onConfirm,
      onCancel,
      ...toastOptions
    } = options

    return new Promise((resolve) => {
      const confirmToast = addToast({
        type: 'warning',
        title,
        message,
        duration: 0,
        closable: true,
        showProgress: false,
        onClick: () => {
          if (onConfirm) onConfirm()
          removeToast(confirmToast)
          resolve(true)
        },
        ...toastOptions
      })

      // Handle cancel
      const originalClose = toastOptions.onClose
      toastOptions.onClose = () => {
        if (originalClose) originalClose()
        if (onCancel) onCancel()
        resolve(false)
      }
    })
  }

  const toastMethods = {
    success,
    error,
    warning,
    info,
    promise,
    confirm,
    add: addToast,
    remove: removeToast,
    clear: clearAllToasts
  }

  return {
    toasts,
    addToast,
    removeToast,
    clearAllToasts,
    success,
    error,
    warning,
    info,
    promise,
    confirm,
    toast: toastMethods
  }
}
