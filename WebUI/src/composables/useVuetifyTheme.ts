import { watchEffect } from 'vue';
import { useTheme } from 'vuetify';
import { useAppStore } from '@/stores/app';
import type { ThemePreference } from '@/types/index';

export function useVuetifyTheme() {
  const theme = useTheme();
  const store = useAppStore();

  const setVuetifyTheme = (themePreference: ThemePreference, colorPreference: string) => {
    let preferTheme =  themePreference;
    if (preferTheme === 'system') {
      preferTheme = store.isDarkMode ? 'dark' : 'light';
    }
    let themeName = colorPreference !== 'default' ? `${preferTheme}-${colorPreference}` : preferTheme;
    theme.change(themeName)
  };

  setVuetifyTheme(store.ThemePreference, store.ColorPreference);

  watchEffect(()=>{
    setVuetifyTheme(store.ThemePreference, store.ColorPreference);
  })
}