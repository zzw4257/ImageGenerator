<template>
  <div class="image-container">
    <v-img
      :src="thumbnail"
      :height="height"
      cover
      class="conversation-thumbnail"
      :class="{ 'thumbnail-hover': isHovered }"
    >
      <template v-slot:placeholder>
        <div v-if="thumbnail" class="d-flex align-center justify-center fill-height">
          <v-progress-circular
            color="grey-lighten-4"
            indeterminate
            size="small"
          ></v-progress-circular>
        </div>
        <div v-else class="d-flex align-center justify-center fill-height no-thumbnail">
          <v-icon size="48">mdi-image-off</v-icon>
        </div>
      </template>
    </v-img>
    
    <!-- Record count badge -->
    <v-chip
      v-if="showBadge"
      class="record-count-badge"
      :color="badgeColor"
      size="small"
      :prepend-icon="badgeIcon"
    >
      {{ recordCount }}
    </v-chip>

    <!-- Hover overlay gradient -->
    <div 
      v-if="isHovered && showHoverEffect"
      class="hover-gradient"
    ></div>
  </div>
</template>

<script lang="ts" setup>
interface Props {
  thumbnail: string
  recordCount?: number
  height?: number | string
  isHovered?: boolean
  showBadge?: boolean
  showHoverEffect?: boolean
  badgeColor?: string
  badgeIcon?: string
}

withDefaults(defineProps<Props>(), {
  height: 200,
  isHovered: false,
  showBadge: true,
  showHoverEffect: true,
  badgeColor: 'primary',
  badgeIcon: 'mdi-comment'
})
</script>

<style scoped>
.image-container {
  position: relative;
  overflow: hidden;
  border-radius: inherit;
}

.conversation-thumbnail {
  transition: transform 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  border-radius: inherit;
}

.thumbnail-hover {
  transform: scale(1.05);
}

.record-count-badge {
  position: absolute;
  top: 8px;
  right: 8px;
  z-index: 3;
  backdrop-filter: blur(4px);
}

.hover-gradient {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: linear-gradient(
    to bottom,
    rgba(0, 0, 0, 0.1),
    rgba(0, 0, 0, 0.3)
  );
  transition: opacity 0.3s ease;
  z-index: 2;
}

/* Ensure proper rounded corners */
.image-container,
.conversation-thumbnail {
  border-top-left-radius: 24px;
  border-top-right-radius: 24px;
}

.no-thumbnail{
  background-image: linear-gradient(to top, #fbc2eb 0%, #a6c1ee 100%);
}
</style>