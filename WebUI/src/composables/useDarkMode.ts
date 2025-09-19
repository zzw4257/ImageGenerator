import { useAppStore } from "@/stores/app";

export function useDarkMode() {
  const store = useAppStore();
  const HTMLnode = document.documentElement;

  const setDarkMode = () => {
    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
    if (prefersDark) {
      HTMLnode.classList.add('dark');
      store.setDarkMode(true);
    } else {
      HTMLnode.classList.remove('dark');
      store.setDarkMode(false);
    }
  };

  setDarkMode();
  const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
  mediaQuery.addEventListener('change', setDarkMode);
}