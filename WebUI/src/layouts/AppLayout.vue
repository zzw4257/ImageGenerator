<template>
  <!-- Navigation Drawer -->
  <v-navigation-drawer v-model="drawer" :rail="rail" :temporary="!isExpanded" :permanent="isExpanded" class="app-drawer"
    elevation="2">
    <template v-slot:prepend>
      <div class="pa-3 pt-6 d-flex align-center">
        <!-- Logo/Brand -->
        <v-avatar :size="rail ? 32 : 40" rounded="lg" color="primary" class="mr-3" :class="{ 'mr-0': rail }">
          <v-icon color="white" :size="rail ? 20 : 24">
            mdi-image-multiple
          </v-icon>
        </v-avatar>

        <div v-if="!rail" class="flex-grow-1">
          <div class="text-h6 font-weight-bold">ImageGen</div>
          <div class="text-caption text-grey-darken-1">AI Studio</div>
        </div>
      </div>
    </template>

    <!-- Navigation Items -->
    <v-list nav class="pa-2">
      <v-list-item v-for="item in navigationItems" :key="item.to" :to="item.to" :prepend-icon="item.icon"
        :title="item.title" :subtitle="item.subtitle" rounded="xl" class="mb-1">
        <template v-if="item.badge" v-slot:append>
          <v-chip size="x-small" :color="item.badge.color" :text="item.badge.text" />
        </template>
      </v-list-item>
      <template v-if="isExpanded">
        <v-divider class="mb-2"></v-divider>

        <!-- Rail Toggle -->
        <v-list-item class="pa-2" :prepend-icon="rail ? 'mdi-menu-close' : 'mdi-menu-open'"
          :title="rail ? '' : 'Collapse'" rounded="xl" @click="rail = !rail" />
      </template>
    </v-list>
  </v-navigation-drawer>

  <!-- App Bar -->
  <v-app-bar elevation="0" class="app-bar">
    <template v-slot:prepend>
      <v-btn icon @click="drawer = !drawer" v-if="!isExpanded">
        <v-icon>mdi-menu</v-icon>
      </v-btn>
    </template>

    <v-app-bar-title class="app-title">
      {{ pageTitle }}
    </v-app-bar-title>

    <template v-slot:append>
      <!-- Search -->
      <v-btn icon variant="text" @click="showSearch = !showSearch">
        <v-icon>mdi-magnify</v-icon>
      </v-btn>

      <!-- Notifications -->
      <v-btn icon variant="text" class="mr-2" @click="router.push('/settings')">
        <v-icon>mdi-cog</v-icon>
      </v-btn>

      <!-- User Menu -->
      <v-menu>
        <template v-slot:activator="{ props }">
          <v-btn icon v-bind="props">
            <v-icon> mdi-account </v-icon>
          </v-btn>
        </template>

        <v-list>
          <v-list-item v-for="action in userMenuItems" :key="action.title" :prepend-icon="action.icon"
            :title="action.title" @click="action.action" />
        </v-list>
      </v-menu>
    </template>
  </v-app-bar>

  <!-- Search Overlay -->
  <v-overlay v-model="showSearch" class="align-center justify-center">
    <v-card width="600" rounded="xl" class="pa-4">
      <v-text-field v-model="searchQuery" placeholder="Search conversations, images, or prompts..."
        prepend-inner-icon="mdi-magnify" variant="outlined" rounded="xl" autofocus hide-details
        @keyup.esc="showSearch = false" />

      <div class="mt-4 text-center">
        <v-btn variant="text" @click="showSearch = false">
          Press ESC to close
        </v-btn>
      </div>
    </v-card>
  </v-overlay>

  <!-- Main Content -->
  <v-main class="app-main">
    <div class="main-content" :class="{ 'content-expanded': !rail }">
      <router-view v-slot="{ Component, route }">
        <transition :name="(route.meta?.transition as string) || 'fade'" mode="out-in">
          <component :is="Component" :key="route.path" />
        </transition>
      </router-view>
    </div>
  </v-main>
</template>

<script lang="ts" setup>
import { ref, computed, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useDisplay } from 'vuetify'
import { useAppStore } from '@/stores/app'
import { useNotificationStore } from '@/stores/notification'

interface NavigationItem {
  title: string
  subtitle?: string
  icon: string
  to: string
  badge?: {
    text: string
    color: string
  }
}

interface UserMenuItem {
  title: string
  icon: string
  action: () => void
}

const { mdAndUp } = useDisplay()

const isExpanded = computed(() => {
  return mdAndUp.value
})

const route = useRoute()
const router = useRouter()
const appStore = useAppStore()
const notificationStore = useNotificationStore()

const rail = ref(true)
const drawer = ref(isExpanded.value)
const showSearch = ref(false)
const searchQuery = ref('')

// Add this watch after the ref declarations
watch(isExpanded, (newVal) => {
  drawer.value = newVal
  rail.value = newVal
}, { immediate: true })

const navigationItems: NavigationItem[] = [
  {
    title: 'Home',
    subtitle: 'All conversations',
    icon: 'mdi-home',
    to: '/',
    badge: {
      text: '5',
      color: 'primary'
    }
  },
  {
    title: 'Favorites',
    subtitle: 'Starred images',
    icon: 'mdi-heart',
    to: '/favorites'
  },
  {
    title: 'Invitations',
    subtitle: 'Manage codes',
    icon: 'mdi-ticket',
    to: '/invitation'
  }
]

const userMenuItems: UserMenuItem[] = [
  {
    title: 'Profile',
    icon: 'mdi-account',
    action: () => router.push('/profile')
  },
  {
    title: 'Help',
    icon: 'mdi-help-circle',
    action: () => router.push('/help')
  },
  {
    title: 'Sign Out',
    icon: 'mdi-logout',
    action: () => {
      // Handle logout
      appStore.clearAuthInfo()
      router.push('/login')
      notificationStore.info('Logged out')
    }
  }
]

const pageTitle = computed(() => {
  const routeTitle = route.meta?.title as string
  if (routeTitle) return routeTitle

  // Fallback to route-based titles
  switch (route.path) {
    case '/':
      return 'Image Library'
    case '/gallery':
      return 'Gallery'
    case '/favorites':
      return 'Favorites'
    case '/collections':
      return 'Collections'
    case '/recent':
      return 'Recent Activity'
    case '/invitation':
      return 'Invitation Codes'
    default:
      if (route.path.startsWith('/conversation/')) {
        return 'Conversation'
      }
      return 'ImageGen Studio'
  }
})
</script>

<style scoped>
.app-bar {
  transition: margin-left 0.2s ease;
}

.app-main {
  background: rgb(var(--v-theme-surface-surface));
}

.main-content {
  min-height: calc(100vh - 64px);
  transition: margin-left 0.2s ease;
}

.app-title {
  font-weight: 600;
}

/* Navigation styling */
.v-list-item {
  margin-bottom: 4px;
}

.v-list-item--active {
  background: rgba(var(--v-theme-primary), 0.1);
  color: rgb(var(--v-theme-primary));
}

/* Responsive adjustments */
@media (max-width: 960px) {
  .app-bar-expanded {
    margin-left: 0;
  }

  .content-expanded {
    margin-left: 0;
  }
}

/* Custom scrollbar for drawer */
.app-drawer :deep(.v-navigation-drawer__content) {
  scrollbar-width: thin;
  scrollbar-color: rgba(var(--v-theme-on-surface), 0.2) transparent;
}

.app-drawer :deep(.v-navigation-drawer__content)::-webkit-scrollbar {
  width: 4px;
}

.app-drawer :deep(.v-navigation-drawer__content)::-webkit-scrollbar-thumb {
  background: rgba(var(--v-theme-on-surface), 0.2);
  border-radius: 2px;
}
</style>