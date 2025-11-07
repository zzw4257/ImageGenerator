<template>
  <v-snackbar
    v-model="notificationStore.isShowing"
    class="notification-snackbar"
    :color="currentMessage?.type"
    elevation="8"
    location="bottom center"
    rounded="xl"
    :timeout="-1"
  >
    <div v-if="currentMessage" class="d-flex align-center">
      <v-icon v-if="currentMessage.icon" class="mr-3" size="20">
        {{ currentMessage.icon }}
      </v-icon>

      <div class="flex-grow-1">
        <div class="text-body-1 font-weight-medium">
          {{ currentMessage.message }}
        </div>

        <!-- Queue indicator -->
        <div v-if="notificationStore.messages.length > 1" class="text-caption mt-1 opacity-75">
          {{ notificationStore.messages.length - 1 }} more message{{ notificationStore.messages.length > 2 ? 's' : '' }}
        </div>
      </div>

      <!-- Action button -->
      <v-btn
        v-if="currentMessage.action"
        class="ml-3"
        size="small"
        variant="text"
        @click="handleAction"
      >
        {{ currentMessage.action.text }}
      </v-btn>

      <!-- Close button -->
      <v-btn
        class="ml-2"
        icon="mdi-close"
        size="small"
        variant="text"
        @click="notificationStore.dismiss()"
      />
    </div>

    <!-- Progress bar for timed messages -->
    <div
      v-if="currentMessage && !currentMessage.persistent && currentMessage.timeout"
      class="notification-progress"
    >
      <div
        class="notification-progress-bar"
        :style="{
          animation: `progress ${currentMessage.timeout}ms linear`,
          backgroundColor: 'rgba(255, 255, 255, 0.3)'
        }"
      />
    </div>
  </v-snackbar>
</template>

<script lang="ts" setup>
  import { computed } from 'vue'
  import { useNotificationStore } from '@/stores/notification'

  /**
   * A component that displays a queue of snackbar notifications.
   * This component is controlled by the `notification` store.
   */

  const notificationStore = useNotificationStore()

  const currentMessage = computed(() => notificationStore.currentMessage)

  function handleAction () {
    if (currentMessage.value?.action) {
      currentMessage.value.action.handler()
      notificationStore.dismiss()
    }
  }
</script>

<style scoped>
.notification-snackbar {
  margin-top: 20px;
}

.notification-progress {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  height: 3px;
  overflow: hidden;
  border-radius: 0 0 12px 12px;
}

.notification-progress-bar {
  height: 100%;
  width: 100%;
  transform-origin: left;
  transform: scaleX(0);
}

@keyframes progress {
  from {
    transform: scaleX(1);
  }
  to {
    transform: scaleX(0);
  }
}

/* Color variants */
:deep(.v-snackbar--variant-flat.v-snackbar) {
  --v-snackbar-color: rgb(var(--v-theme-surface));
}

:deep(.v-snackbar--variant-flat.v-snackbar.success) {
  background-color: rgb(var(--v-theme-success));
  color: rgb(var(--v-theme-on-success));
}

:deep(.v-snackbar--variant-flat.v-snackbar.error) {
  background-color: rgb(var(--v-theme-error));
  color: rgb(var(--v-theme-on-error));
}

:deep(.v-snackbar--variant-flat.v-snackbar.warning) {
  background-color: rgb(var(--v-theme-warning));
  color: rgb(var(--v-theme-on-warning));
}

:deep(.v-snackbar--variant-flat.v-snackbar.info) {
  background-color: rgb(var(--v-theme-info));
  color: rgb(var(--v-theme-on-info));
}
</style>
