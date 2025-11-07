import type { ThemePreference } from "@/types/index";
import { watchEffect } from "vue";
import { useTheme } from "vuetify";
import { useAppStore } from "@/stores/app";

/**
 * A composable for managing the Vuetify theme.
 */
export function useVuetifyTheme() {
  const theme = useTheme();
  const store = useAppStore();

  const setVuetifyTheme = (
    themePreference: ThemePreference,
    colorPreference: string
  ) => {
    let preferTheme = themePreference;
    if (preferTheme === "system") {
      preferTheme = store.isDarkMode ? "dark" : "light";
    }
    const themeName =
      colorPreference === "default"
        ? preferTheme
        : `${preferTheme}-${colorPreference}`;
    theme.change(themeName);
  };

  setVuetifyTheme(store.ThemePreference, store.ColorPreference);

  watchEffect(() => {
    setVuetifyTheme(store.ThemePreference, store.ColorPreference);
  });
}
