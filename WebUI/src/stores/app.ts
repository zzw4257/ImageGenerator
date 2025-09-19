// Utilities
import { defineStore } from 'pinia'
import type { AuthInfo, ThemePreference } from '@/types';

export const useAppStore = defineStore('app', () => {
  const isDarkMode = ref<boolean>(false); // Add isDarkMode state
  const ThemePreference = ref<ThemePreference>(localStorage.getItem('themePreference') as ThemePreference || 'system');
  const ColorPreference = ref<string>(localStorage.getItem('colorPreference') || 'default');
  const authInfo = ref<AuthInfo>({
    userId: localStorage.getItem('userId') || '',
    token: localStorage.getItem('token') || '',
    expirationTime: localStorage.getItem('expirationTime') || ''
  });

  const setAuthInfo = (data: AuthInfo) => {
    authInfo.value = data;
    localStorage.setItem('userId', data.userId);
    localStorage.setItem('token', data.token);
    localStorage.setItem('expirationTime', data.expirationTime);
  }

  const setDarkMode = (value: boolean) => {
    isDarkMode.value = value;
  };

  const setPreferences = (theme: ThemePreference, color: string) => {
    ThemePreference.value = theme;
    ColorPreference.value = color;
    localStorage.setItem('themePreference', theme);
    localStorage.setItem('colorPreference', color);
  }

  return {
    ThemePreference,
    ColorPreference,
    setDarkMode,
    isDarkMode,
    setPreferences,
    setAuthInfo
  };
})
