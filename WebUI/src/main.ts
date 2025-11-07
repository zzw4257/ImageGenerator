/**
 * main.ts
 *
 * Bootstraps Vuetify and other plugins then mounts the App`
 */

// Plugins
import { registerPlugins } from '@/plugins'

// Components
import App from './App.vue'

// Composables
import { createApp } from 'vue'

import { createPinia } from 'pinia'

import router from './router'

// Styles
// import 'unfonts.css'
import '@mdi/font/css/materialdesignicons.css'
import { useDarkMode } from "./composables/useDarkMode.ts";

const app = createApp(App)

registerPlugins(app)

useDarkMode();

app.mount('#app')
