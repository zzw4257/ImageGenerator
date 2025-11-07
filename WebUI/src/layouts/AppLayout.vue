<template>
  <!-- Navigation Drawer -->
  <v-navigation-drawer
    v-model="drawer"
    class="app-drawer"
    elevation="2"
    :permanent="isExpanded"
    :rail="rail"
    :temporary="!isExpanded"
  >
    <template #prepend>
      <div class="pa-3 pt-6 d-flex align-center">
        <!-- Logo/Brand -->
        <v-avatar
          class="mr-3"
          :class="{ 'mr-0': rail }"
          color="primary"
          rounded="lg"
          :size="rail ? 32 : 40"
        >
          <v-icon color="white" :size="rail ? 20 : 24">
            mdi-image-multiple
          </v-icon>
        </v-avatar>

        <div v-if="!rail" class="flex-grow-1">
          <div class="text-h6 font-weight-bold">Aetherflow</div>
          <div class="text-caption text-grey-darken-1">凝滞之处，价值新生</div>
        </div>
      </div>
    </template>

    <!-- Navigation Items -->
    <v-list class="pa-2" nav>
      <v-list-item
        v-for="item in navigationItems"
        :key="item.to"
        class="mb-1"
        :prepend-icon="item.icon"
        rounded="xl"
        :subtitle="item.subtitle"
        :title="item.title"
        :to="item.to"
      >
        <template v-if="item.badge" #append>
          <v-chip
            :color="item.badge.color"
            size="x-small"
            :text="item.badge.text"
          />
        </template>
      </v-list-item>
      <template v-if="isExpanded">
        <v-divider class="mb-2" />

        <!-- Rail Toggle -->
        <v-list-item
          class="pa-2"
          :prepend-icon="rail ? 'mdi-menu-close' : 'mdi-menu-open'"
          rounded="xl"
          :title="rail ? '' : 'Collapse'"
          @click="rail = !rail"
        />
      </template>
    </v-list>
  </v-navigation-drawer>

  <!-- App Bar -->
  <v-app-bar class="app-bar" elevation="0">
    <template #prepend>
      <v-btn v-if="!isExpanded" icon @click="drawer = !drawer">
        <v-icon>mdi-menu</v-icon>
      </v-btn>
    </template>

    <v-app-bar-title class="app-title">
      {{ pageTitle }}
    </v-app-bar-title>

    <template #append>
      <!-- Search -->
      <v-btn icon variant="text" @click="showSearch = !showSearch">
        <v-icon>mdi-magnify</v-icon>
      </v-btn>

      <!-- Notifications -->
      <v-btn class="mr-2" icon variant="text" @click="router.push('/settings')">
        <v-icon>mdi-cog</v-icon>
      </v-btn>

      <!-- User Menu -->
      <v-menu>
        <template #activator="{ props }">
          <v-btn icon v-bind="props">
            <v-icon> mdi-account </v-icon>
          </v-btn>
        </template>

        <v-list>
          <v-list-item
            v-for="action in userMenuItems"
            :key="action.title"
            :prepend-icon="action.icon"
            :title="action.title"
            @click="action.action"
          />
        </v-list>
      </v-menu>
    </template>
  </v-app-bar>

  <!-- Search Overlay -->
  <v-overlay v-model="showSearch" class="align-center justify-center">
    <v-card class="pa-4" rounded="xl" width="600">
      <v-text-field
        v-model="searchQuery"
        autofocus
        hide-details
        placeholder="Search conversations, images, or prompts..."
        prepend-inner-icon="mdi-magnify"
        rounded="xl"
        variant="outlined"
        @keyup.esc="showSearch = false"
      />

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
        <transition
          mode="out-in"
          :name="(route.meta?.transition as string) || 'fade'"
        >
          <component :is="Component" :key="route.path" />
        </transition>
      </router-view>
    </div>
  </v-main>
</template>

<script lang="ts" setup>
  import { computed, ref, watch } from 'vue'
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
  watch(
    isExpanded,
    newVal => {
      drawer.value = newVal
      rail.value = newVal
    },
    { immediate: true },
  )

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
      title: 'Muse',
      subtitle: 'Use ready-made prompts',
      icon: 'mdi-format-paint',
      to: '/presets'
    },
    {
      title: 'Market',
      subtitle: 'AIGC assets marketplace',
      icon: 'mdi-store',
      to: '/market',
      badge: {
        text: 'New',
        color: 'success'
      }
    },
    {
      title: 'Analytics',
      subtitle: 'Data insights',
      icon: 'mdi-chart-line',
      to: '/analytics'
    },
    {
      title: 'Community',
      subtitle: 'Creator community',
      icon: 'mdi-account-group',
      to: '/community'
    },
    {
      title: 'Workflow Editor',
      subtitle: 'Visual workflow builder',
      icon: 'mdi-graph',
      to: '/workflow-editor',
      badge: {
        text: 'Beta',
        color: 'info'
      }
    },
    {
      title: 'Enterprise',
      subtitle: 'Business solutions',
      icon: 'mdi-office-building',
      to: '/enterprise'
    },
    {
      title: 'Favorites',
      subtitle: 'Starred images',
      icon: 'mdi-heart',
      to: '/favorites'
    },
    {
      title: 'Account',
      subtitle: 'Balance & History',
      icon: 'mdi-account-circle',
      to: '/account'
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
      action: () => router.push('/profile'),
    },
    {
      title: 'Help',
      icon: 'mdi-help-circle',
      action: () => router.push('/help'),
    },
    {
      title: 'Sign Out',
      icon: 'mdi-logout',
      action: () => {
        // Handle logout
        appStore.clearAuthInfo()
        router.push('/login')
        notificationStore.info('Logged out')
      },
    },
  ]

  const pageTitle = computed(() => {
    const routeTitle = route.meta?.title as string
    if (routeTitle) return routeTitle

    // Fallback to route-based titles
    switch (route.path) {
      case '/': {
        return 'Image Library'
      }
      case '/gallery': {
        return 'Gallery'
      }
      case '/favorites': {
        return 'Favorites'
      }
      case '/account': {
        return 'Account & History'
      }
      case '/collections': {
        return 'Collections'
      }
      case '/recent': {
        return 'Recent Activity'
      }
      case '/invitation': {
        return 'Invitation Codes'
      }
      default: {
        if (route.path.startsWith('/conversation/')) {
          return 'Conversation'
        }
        return 'Aetherflow'
      }
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
