<template>
  <header class="profile-header">
    <div class="profile-main">
      <div class="avatar-container">
        <div class="avatar-ring">
          <Skeleton v-if="isLoading" width="100px" height="100px" shape="circle" />
          <img v-else :src="`https://ui-avatars.com/api/?name=${studentName || 'Student'}&background=random&size=128`" alt="Avatar" class="avatar-img">
          <div class="status-indicator online"></div>
        </div>
      </div>
      <div class="profile-text">
        <Skeleton v-if="isLoading" width="200px" height="2rem" style="margin: 0 auto 0.5rem" />
        <h1 v-else class="student-name">{{ studentName }}</h1>
        <Skeleton v-if="isLoading" width="150px" height="1rem" style="margin: 0 auto" />
        <p v-else class="student-meta">ID: {{ studentProfile?.student_id || 'STU-001' }} | Class: {{ classSection }}</p>
      </div>
    </div>
    <div class="header-actions">
      <button @click="$emit('viewProfile')" class="icon-btn" title="View Profile">👤</button>
      <button @click="$emit('viewSettings')" class="icon-btn" title="Settings">⚙️</button>
    </div>
  </header>
</template>

<script setup>
import Skeleton from '@/components/ui/Skeleton.vue'

defineProps({
  isLoading: {
    type: Boolean,
    default: false
  },
  studentName: {
    type: String,
    default: ''
  },
  studentProfile: {
    type: Object,
    default: () => null
  },
  classSection: {
    type: String,
    default: ''
  }
})

defineEmits(['viewProfile', 'viewSettings'])
</script>

<style scoped>
/* Header */
.profile-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 2rem;
}

.profile-main {
  display: flex;
  flex-direction: column;
  align-items: center;
  flex: 1;
}

.avatar-container {
  position: relative;
  margin-bottom: 1rem;
}

.avatar-ring {
  width: 100px;
  height: 100px;
  padding: 4px;
  background: var(--bg-card);
  border-radius: 50%;
  box-shadow: var(--shadow-card);
}

.avatar-img {
  width: 100%;
  height: 100%;
  border-radius: 50%;
  object-fit: cover;
}

.status-indicator {
  position: absolute;
  bottom: 4px;
  right: 4px;
  width: 16px;
  height: 16px;
  background: var(--success);
  border: 3px solid var(--bg-card);
  border-radius: 50%;
}

.student-name {
  font-size: 1.5rem;
  font-weight: 800;
  color: #111827;
  text-align: center;
  margin-bottom: 0.25rem;
}

.student-meta {
  font-size: 0.875rem;
  color: #64748b;
  font-weight: 600;
}

.icon-btn {
  background: none;
  border: none;
  font-size: 1.25rem;
  cursor: pointer;
  padding: 0.5rem;
}

/* Responsive Styles */
@media (max-width: 1024px) {
  .profile-header {
    flex-direction: column;
    text-align: center;
    gap: 1rem;
  }
  .header-actions {
    margin: 0 auto;
    gap: 0.5rem;
  }
  .student-name {
    font-size: 1.75rem;
  }
}

@media (max-width: 768px) {
  .profile-header {
    padding: 1.5rem 1rem;
  }
  .student-name {
    font-size: 1.5rem;
  }
  .student-meta {
    font-size: 0.8rem;
  }
  .avatar-ring {
    width: 80px;
    height: 80px;
  }
  .icon-btn {
    padding: 0.5rem;
    font-size: 1rem;
  }
}

@media (max-width: 640px) {
  .profile-header {
    padding: 1rem 0.75rem;
    gap: 0.75rem;
  }
  .student-name {
    font-size: 1.25rem;
  }
  .student-meta {
    font-size: 0.75rem;
  }
  .avatar-ring {
    width: 70px;
    height: 70px;
  }
  .status-indicator {
    width: 14px;
    height: 14px;
    border-width: 2px;
  }
  .icon-btn {
    padding: 0.375rem;
    font-size: 0.875rem;
  }
}

@media (max-width: 480px) {
  .profile-header {
    padding: 0.75rem 0.5rem;
  }
  .student-name {
    font-size: 1.125rem;
  }
  .avatar-ring {
    width: 60px;
    height: 60px;
  }
}
</style>