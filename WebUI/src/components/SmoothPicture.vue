<script setup lang="ts">
import { animate, createSpring } from "animejs";
import { computed, nextTick, ref } from "vue";
import type { VImg } from "vuetify/components";

const props = defineProps({
  /**
   * The URL of the image to display.
   */
  url: {
    type: String,
    required: true,
  },
  /**
   * The maximum height of the image.
   */
  maxHeight: {
    type: Number,
    default: 570,
  },
  /**
   * The maximum width of the image.
   */
  maxWidth: {
    type: Number,
    default: 800,
  },
  /**
   * Whether to maintain the aspect ratio of the image.
   */
  keepAspectRatio: {
    type: Boolean,
    default: false,
  },
});
const imgRef = ref<VImg | null>(null);
const imgContainerRef = ref<InstanceType<typeof VImg> | null>(null);
const imageLoaded = ref(false);
const imageAspectRatio = ref(1);

function handleFullSize() {
  if (imgContainerRef.value && imageLoaded.value) {
    const container = document.createElement("div");
    const mask = document.createElement("div");
    mask.classList.add("smooth-picutre-mask");
    container.style.backgroundImage = `url(${props.url})`;
    if (!props.keepAspectRatio) container.style.backgroundSize = "cover";
    container.classList.add("zoomed-picture", "rounded-lg");
    const el = imgContainerRef.value.$el as HTMLDivElement;
    const rect: DOMRect = el.getClientRects()[0]!;
    const initialWidth = el.clientWidth || 0;
    const initialHeight = el.clientHeight || 0;
    const initialTop = rect.top || 0;
    const initialLeft = rect.left || 0;
    container.style.top = `${initialTop}px`;
    container.style.left = `${initialLeft}px`;
    container.style.width = `${initialWidth}px`;
    container.style.height = `${initialHeight}px`;
    document.body.appendChild(mask);
    mask.appendChild(container);
    el.style.opacity = "0";
    nextTick(() => {
      let aspect = window.innerWidth / window.innerHeight;
      let height:number, width:number;
      if (aspect < imageAspectRatio.value){
        width = window.innerWidth - 50;
        height = width / imageAspectRatio.value;
      }
      else {
        height = window.innerHeight - 50;
        width = height * imageAspectRatio.value;
      }
      const property = {
        top: `${(window.innerHeight - height)/2}px`,
        left: `${(window.innerWidth - width)/2}px`,
        duration: 500,
        width: `${width}px`,
        height: `${height}px`,
        ease: createSpring({
          damping: 50,
          stiffness: 600,
          mass: 2
        }),
      };
      animate(container, property).then(() => {
        container.style.transition = "box-shadow 0.3s";
        container.style.boxShadow = "0 0 20px rgba(0,0,0,0.5)";
      });
    });

    let isZoomed = true;
    const handleZoomOut = () => {
      if (isZoomed) {
        isZoomed = false;
        const rect: DOMRect = el.getClientRects()[0]!;
        const targetTop = rect.top || 0;
        const targetLeft = rect.left || 0;
        const targetWidth = el.clientWidth || 0;
        const targetHeight = el.clientHeight || 0;
        const property = {
          top: `${targetTop}px`,
          left: `${targetLeft}px`,
          width: `${targetWidth}px`,
          height: `${targetHeight}px`,
          duration: 500,
          ease: createSpring({
            damping: 40,
            stiffness: 400,
            mass: 2
          }),
        };
        animate(container, property).then(() => {
          el.style.opacity = "1";
          setTimeout(() => {
            document.body.removeChild(mask);
          }, 100);
        });
      }
    }

    mask.addEventListener("click", handleZoomOut);
  }
}

const onImageLoad = () => {
  if (imgRef.value) {
    const { naturalWidth, naturalHeight } = imgRef.value;
    if (naturalWidth && naturalHeight) {
      imageAspectRatio.value = naturalWidth / naturalHeight;
      imageLoaded.value = true;
    }
  }
};

const containerStyle = computed(() => {
  if (!imageLoaded.value) return "height: 90%; width: 90%;";

  const maxWidth = props.maxWidth;
  const maxHeight = props.maxHeight;

  let width = maxWidth;
  let height = maxWidth / imageAspectRatio.value;

  if (height > maxHeight) {
    height = maxHeight;
    width = maxHeight * imageAspectRatio.value;
  }

  return `height: ${height}px; width: ${width}px;`;
});
</script>

<template>
  <v-sheet
    class="rounded-lg smooth-picture-container"
    elevation="5"
    :style="keepAspectRatio?containerStyle:'' + ' overflow: hidden;'"
    ref="imgContainerRef"
  >
    <v-img
      :src="url"
      class="smooth-picture"
      cover
      @click="handleFullSize"
      :aspect-ratio="imageAspectRatio"
      ref="imgRef"
      style="border-radius: inherit"
      @load="onImageLoad"
    >
    <slot></slot>
    <template #placeholder>
      <slot name="placeholder"></slot>
    </template>
  </v-img>
</v-sheet>
</template>

<style lang="css">
.smooth-picture-container {
  height: 100%;
  width: 100%;
  transition: 0.1s;
}

.smooth-picutre-mask {
  position: fixed;
  cursor: zoom-out;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  z-index: 9998;
}

.smooth-picture {
  cursor: zoom-in;
  height: 100%;
  width: 100%;
}

.zoomed-picture {
  position: absolute;
  background-size: contain;
  background-repeat: no-repeat;
  background-position: center;
}
</style>
