/**
 * router/index.ts
 *
 * Automatic routes for `./src/pages/*.vue`
 */

// Composables
import { setupLayouts } from 'virtual:generated-layouts'
import { createRouter, createWebHistory } from 'vue-router'
import { routes } from 'vue-router/auto-routes'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: setupLayouts(routes),
})

// Helper function to check if user is authenticated
function isAuthenticated(): boolean {
  const token = localStorage.getItem('token')
  const userId = localStorage.getItem('userId')
  return !!(token && userId)
}

// Navigation guard to check authentication
router.beforeEach((to, from, next) => {
  // Define public routes that don't require authentication
  const publicRoutes = ['/login', '/register', '/landing']
  const isPublicRoute = publicRoutes.includes(to.path)
  
  // If user is authenticated and trying to access landing page, redirect to home
  if (isAuthenticated() && to.path === '/landing') {
    next('/')
    return
  }
  
  // If user is not authenticated and trying to access protected route
  if (!isAuthenticated() && !isPublicRoute) {
    // Redirect to landing page for unauthenticated users
    next('/landing')
    return
  }
  
  // Allow access to public routes or authenticated users
  next()
})

// Workaround for https://github.com/vitejs/vite/issues/11804
router.onError((err, to) => {
  if (err?.message?.includes?.('Failed to fetch dynamically imported module')) {
    if (localStorage.getItem('vuetify:dynamic-reload')) {
      console.error('Dynamic import error, reloading page did not fix it', err)
    } else {
      console.log('Reloading page to fix dynamic import error')
      localStorage.setItem('vuetify:dynamic-reload', 'true')
      location.assign(to.fullPath)
    }
  } else {
    console.error(err)
  }
})

router.isReady().then(() => {
  localStorage.removeItem('vuetify:dynamic-reload')
})

export default router
