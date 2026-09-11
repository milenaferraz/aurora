import { createRouter, createWebHistory, type Router, type RouterHistory } from 'vue-router'

export function createAuroraRouter(history: RouterHistory = createWebHistory()): Router {
  return createRouter({
    history,
    routes: [
      {
        path: '/',
        name: 'aurora',
        component: () => import('../views/ChatView.vue'),
      },
      {
        path: '/aurora',
        redirect: { name: 'aurora' },
      },
      {
        path: '/chat',
        redirect: { name: 'aurora' },
      },
    ],
  })
}
