<template>
  <v-app>
    <!-- Navigation Drawer -->
    <v-navigation-drawer
      v-model="drawer"
      :rail="rail"
      permanent
      class="app-drawer"
      rounded="r-xl"
      elevation="2"
    >
      <template v-slot:prepend>
        <div class="pa-4 d-flex align-center">
          <!-- Logo/Brand -->
          <v-avatar
            :size="rail ? 32 : 40"
            rounded="lg"
            color="primary"
            class="mr-3"
            :class="{ 'mr-0': rail }"
          >
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
        <v-list-item
          v-for="item in navigationItems"
          :key="item.to"
          :to="item.to"
          :prepend-icon="item.icon"
          :title="item.title"
          :subtitle="item.subtitle"
          rounded="xl"
          class="mb-1"
        >
          <template v-if="item.badge" v-slot:append>
            <v-chip
              size="x-small"
              :color="item.badge.color"
              :text="item.badge.text"
            />
          </template>
        </v-list-item>
      </v-list>

      <template v-slot:append>
        <!-- Settings and User -->
        <div class="pa-2">
          <v-divider class="mb-2"></v-divider>
          
          <v-list-item
            :prepend-icon="themeIcon"
            :title="rail ? '' : 'Toggle Theme'"
            rounded="xl"
            @click="toggleTheme"
          />
          
          <v-list-item
            prepend-icon="mdi-cog"
            :title="rail ? '' : 'Settings'"
            rounded="xl"
            to="/settings"
          />
          
          <!-- Rail Toggle -->
          <v-list-item
            :prepend-icon="rail ? 'mdi-menu' : 'mdi-menu-open'"
            :title="rail ? '' : 'Collapse'"
            rounded="xl"
            @click="rail = !rail"
          />
        </div>
      </template>
    </v-navigation-drawer>

    <!-- App Bar -->
    <v-app-bar
      elevation="0"
      class="app-bar"
      :class="{ 'app-bar-expanded': !rail }"
    >
      <template v-slot:prepend>
        <v-btn
          icon
          @click="drawer = !drawer"
          class="d-md-none"
        >
          <v-icon>mdi-menu</v-icon>
        </v-btn>
      </template>

      <v-app-bar-title class="app-title">
        {{ pageTitle }}
      </v-app-bar-title>

      <template v-slot:append>
        <!-- Search -->
        <v-btn
          icon
          variant="text"
          @click="showSearch = !showSearch"
        >
          <v-icon>mdi-magnify</v-icon>
        </v-btn>

        <!-- Notifications -->
        <v-btn
          icon
          variant="text"
          class="mr-2"
        >
          <v-badge
            v-if="notificationCount > 0"
            :content="notificationCount"
            color="error"
          >
            <v-icon>mdi-bell</v-icon>
          </v-badge>
          <v-icon v-else>mdi-bell</v-icon>
        </v-btn>

        <!-- User Menu -->
        <v-menu>
          <template v-slot:activator="{ props }">
            <v-btn
              icon
              v-bind="props"
            >
              <v-avatar size="32">
                <v-img :src="userAvatar" />
              </v-avatar>
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
    <v-overlay
      v-model="showSearch"
      class="align-center justify-center"
    >
      <v-card
        width="600"
        rounded="xl"
        class="pa-4"
      >
        <v-text-field
          v-model="searchQuery"
          placeholder="Search conversations, images, or prompts..."
          prepend-inner-icon="mdi-magnify"
          variant="outlined"
          rounded="xl"
          autofocus
          hide-details
          @keyup.esc="showSearch = false"
        />
        
        <div class="mt-4 text-center">
          <v-btn
            variant="text"
            @click="showSearch = false"
          >
            Press ESC to close
          </v-btn>
        </div>
      </v-card>
    </v-overlay>

    <!-- Main Content -->
    <v-main class="app-main">
      <div class="main-content" :class="{ 'content-expanded': !rail }">
        <router-view />
      </div>
    </v-main>

    <!-- Floating Action Button -->
    <v-fab
      v-if="showFab"
      :icon="fabIcon"
      :color="fabColor"
      location="bottom end"
      size="large"
      app
      @click="handleFabClick"
    />
  </v-app>
</template>

<script lang="ts" setup>
import { ref, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useTheme } from 'vuetify'

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

const route = useRoute()
const router = useRouter()
const theme = useTheme()

const drawer = ref(true)
const rail = ref(false)
const showSearch = ref(false)
const searchQuery = ref('')
const notificationCount = ref(3)

const userAvatar = 'https://cdn.vuetifyjs.com/images/profiles/marcus.jpg'

const navigationItems: NavigationItem[] = [
  {
    title: 'Home',
    subtitle: 'All conversations',
    icon: 'mdi-home',
    to: '/'
  },
  {
    title: 'Recent',
    subtitle: 'Latest activity',
    icon: 'mdi-clock-outline',
    to: '/recent',
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
    title: 'Collections',
    subtitle: 'Organized groups',
    icon: 'mdi-folder-multiple',
    to: '/collections'
  },
  {
    title: 'Gallery',
    subtitle: 'All images',
    icon: 'mdi-view-grid',
    to: '/gallery'
  }
]

const userMenuItems: UserMenuItem[] = [
  {
    title: 'Profile',
    icon: 'mdi-account',
    action: () => router.push('/profile')
  },
  {
    title: 'Settings',
    icon: 'mdi-cog',
    action: () => router.push('/settings')
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
      console.log('Signing out...')
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
    default:
      if (route.path.startsWith('/conversation/')) {
        return 'Conversation'
      }
      return 'ImageGen Studio'
  }
})

const themeIcon = computed(() => {
  return theme.global.current.value.dark ? 'mdi-weather-sunny' : 'mdi-weather-night'
})

const showFab = computed(() => {
  // Show FAB on home page
  return route.path === '/'
})

const fabIcon = computed(() => {
  return 'mdi-plus'
})

const fabColor = computed(() => {
  return 'primary'
})

const handleFabClick = () => {
  router.push('/conversation/new')
}
</script>

<style scoped>
.app-drawer {
  border-right: none !important;
}

.app-bar {
  transition: margin-left 0.2s ease;
}

.app-bar-expanded {
  margin-left: 256px;
}

.app-main {
  background: rgb(var(--v-theme-surface-surface));
}

.main-content {
  min-height: calc(100vh - 64px);
  transition: margin-left 0.2s ease;
}

.content-expanded {
  margin-left: 0;
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