<template>
  <div class="landing-page">
    <iframe
      ref="landingFrame"
      src="/landing.html"
      frameborder="0"
      style="width: 100%; height: 100vh; border: none;"
      allow="autoplay; encrypted-media; fullscreen"
      @load="onFrameLoad"
    />
    
    <!-- Floating Login/Register Buttons -->
    <div class="floating-buttons">
      <v-btn
        color="primary"
        size="large"
        rounded="xl"
        prepend-icon="mdi-login"
        @click="goToLogin"
      >
        Login
      </v-btn>
      <v-btn
        color="secondary"
        size="large"
        rounded="xl"
        prepend-icon="mdi-account-plus"
        variant="outlined"
        @click="goToRegister"
      >
        Register
      </v-btn>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAppStore } from '@/stores/app'

const router = useRouter()
const appStore = useAppStore()
const landingFrame = ref<HTMLIFrameElement | null>(null)

function onFrameLoad() {
  // 当iframe加载完成后，可以添加一些逻辑
  console.log('Landing page loaded')
}

function goToLogin() {
  router.push('/login')
}

function goToRegister() {
  router.push('/register')
}

onMounted(() => {
  // 监听登录状态变化
  // 如果用户已登录，可以在这里添加重定向逻辑
})
</script>

<route lang="yaml">
meta:
  layout: empty
  title: Aetherflow - Landing Page
</route>

<style scoped>
.landing-page {
  width: 100%;
  height: 100vh;
  overflow: hidden;
  position: relative;
}

.floating-buttons {
  position: fixed;
  top: 20px;
  right: 20px;
  display: flex;
  gap: 12px;
  z-index: 1000;
}

@media (max-width: 768px) {
  .floating-buttons {
    flex-direction: column;
    width: calc(100% - 40px);
    right: 20px;
    left: 20px;
  }
}
</style>

