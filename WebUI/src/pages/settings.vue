<template>
    <div class="settings-page pa-4">
        <v-card class="pa-4 mt-4" variant="tonal">
          <v-card-title class="font-weight-bold">Appearance</v-card-title>
          <v-card-text>
            <div class="setting-item mt-4">
              <h3 class="text-subtitle-1 mb-2">Theme</h3>
              <v-radio-group v-model="theme" inline>
                <v-radio label="Light" value="light"></v-radio>
                <v-radio label="Dark" value="dark"></v-radio>
                <v-radio label="Auto" value="system"></v-radio>
              </v-radio-group>
            </div>

            <div class="setting-item">
              <h3 class="text-subtitle-1 mb-3">Color Theme</h3>
              <div class="color-palette">
                <v-btn
                  v-for="color in materialColors"
                  :key="color.name"
                  icon
                  :color="color.hex"
                  @click="selectedColor = color"
                  class="ma-1"
                  border
                >
                  <v-icon v-if="selectedColor.hex === color.hex">mdi-check</v-icon>
                </v-btn>
              </div>
            </div>
          </v-card-text>
        </v-card>
      </div>
</template>

<script setup lang="ts">
import { ref, watchEffect } from 'vue';
import { useAppStore } from '@/stores/app';
import type { ThemePreference } from '@/types/index';

const appStore = useAppStore();
const theme = ref<ThemePreference>(appStore.ThemePreference);

const materialColors = [
    { name: 'default', hex: '#00000000' },
    { name: 'red', hex: '#F44336' },
    { name: 'pink', hex: '#E91E63' },
    { name: 'purple', hex: '#9C27B0' },
    { name: 'deepPurple', hex: '#673AB7' },
    { name: 'indigo', hex: '#3F51B5' },
    { name: 'blue', hex: '#2196F3' },
    { name: 'lightBlue', hex: '#03A9F4' },
    { name: 'cyan', hex: '#00BCD4' },
    { name: 'teal', hex: '#009688' },
    { name: 'green', hex: '#4CAF50' },
    { name: 'lightGreen', hex: '#8BC34A' },
    { name: 'lime', hex: '#CDDC39' },
    { name: 'yellow', hex: '#FFEB3B' },
    { name: 'amber', hex: '#FFC107' },
    { name: 'orange', hex: '#FF9800' },
    { name: 'deepOrange', hex: '#FF5722' },
];

const selectedColor = ref(materialColors.find(color => color.name === appStore.ColorPreference) ?? materialColors[0] as typeof materialColors[0]);

watchEffect(() => {
    appStore.setPreferences(theme.value, selectedColor.value.name);
});
</script>

<route lang="yaml">
meta:
  title: Settings
</route>