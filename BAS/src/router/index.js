import { createRouter, createWebHistory } from 'vue-router'
import Home from '../views/Home.vue'
import LecturerLogin from '../views/LecturerLogin.vue'
import StudentLogin from '../views/StudentLogin.vue'
import StudentSignup from '../views/StudentSignup.vue'
import StudentHomepage from '../views/StudentHomepage.vue'
import ReportPage from '../views/ReportPage.vue'

const routes = [
  {
    path: '/',
    name: 'Home',
    component: Home
  },
  {
    path: '/lecturer-login',
    name: 'LecturerLogin',
    component: LecturerLogin
  },
  {
    path: '/student-login',
    name: 'StudentLogin',
    component: StudentLogin
  },
  {
    path: '/student-signup',
    name: 'StudentSignup',
    component: StudentSignup
  },
  {
    path: '/student-homepage',
    name: 'StudentHomepage',
    component: StudentHomepage
  },
  {
    path: '/report-page',
    name: 'ReportPage',
    component: ReportPage
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
