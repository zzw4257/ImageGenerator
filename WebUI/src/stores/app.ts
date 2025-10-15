// Utilities
import { defineStore } from 'pinia'
import type { AuthInfo, ThemePreference } from '@/types';

/**
 * Represents the information for the delete confirmation dialog.
 */
export interface DeleteDialogInfo {
  /** Whether the dialog is visible. */
  show: boolean;
  /** The title of the dialog. */
  title: string;
  /** The message to display in the dialog. */
  message: string;
  /** The name of the item to be deleted. */
  itemName?: string;
  /** The text for the confirm button. */
  confirmText?: string;
  /** Whether the deletion is in progress. */
  isDeleting: boolean;
  /** The function to call when the deletion is confirmed. */
  onConfirm?: () => Promise<void> | void;
}

/**
 * Represents the information for the prompt replacement confirmation dialog.
 */
export interface PromptReplaceDialogInfo {
  /** Whether the dialog is visible. */
  show: boolean;
  /** The title of the dialog. */
  title: string;
  /** The message to display in the dialog. */
  message: string;
  /** The current prompt text. */
  currentPrompt: string;
  /** The new prompt text. */
  newPrompt: string;
  /** Whether the replacement is in progress. */
  isProcessing: boolean;
  /** The function to call when the replacement is confirmed. */
  onConfirm?: () => Promise<void> | void;
  /** The function to call when the replacement is canceled. */
  onCancel?: () => void;
}

/**
 * The main store for the application, managing global state such as theme, authentication, and dialogs.
 */
export const useAppStore = defineStore('app', () => {
  /** Whether dark mode is enabled. */
  const isDarkMode = ref<boolean>(false);
  /** The user's theme preference. */
  const ThemePreference = ref<ThemePreference>(localStorage.getItem('themePreference') as ThemePreference || 'system');
  /** The user's color preference. */
  const ColorPreference = ref<string>(localStorage.getItem('colorPreference') || 'default');
  /** The user's authentication information. */
  const authInfo = ref<AuthInfo>({
    userId: localStorage.getItem('userId') || '',
    token: localStorage.getItem('token') || '',
    expirationTime: localStorage.getItem('expirationTime') || ''
  });

  const deleteDialogInfo = ref<DeleteDialogInfo>({
    show: false,
    title: '',
    message: '',
    itemName: '',
    confirmText: 'Delete',
    isDeleting: false,
    onConfirm: undefined
  });

  const promptReplaceDialogInfo = ref<PromptReplaceDialogInfo>({
    show: false,
    title: '',
    message: '',
    currentPrompt: '',
    newPrompt: '',
    isProcessing: false,
    onConfirm: undefined,
    onCancel: undefined
  });

  /**
   * Sets the user's authentication information.
   * @param data - The authentication information.
   */
  const setAuthInfo = (data: AuthInfo) => {
    authInfo.value = data;
    localStorage.setItem('userId', data.userId);
    localStorage.setItem('token', data.token);
    localStorage.setItem('expirationTime', data.expirationTime);
  }

  /**
   * Clears the user's authentication information.
   */
  const clearAuthInfo = () => {
    authInfo.value = { userId: '', token: '', expirationTime: '' };
    localStorage.removeItem('userId');
    localStorage.removeItem('token');
    localStorage.removeItem('expirationTime');
  }

  /**
   * Sets the dark mode preference.
   * @param value - Whether dark mode is enabled.
   */
  const setDarkMode = (value: boolean) => {
    isDarkMode.value = value;
  };

  /**
   * Sets the user's theme and color preferences.
   * @param theme - The theme preference.
   * @param color - The color preference.
   */
  const setPreferences = (theme: ThemePreference, color: string) => {
    ThemePreference.value = theme;
    ColorPreference.value = color;
    localStorage.setItem('themePreference', theme);
    localStorage.setItem('colorPreference', color);
  }

  /**
   * Shows the delete confirmation dialog.
   * @param options - The options for the dialog.
   * @returns A promise that resolves to `true` if the deletion is confirmed, and `false` otherwise.
   */
  const showDeleteDialog = (options: {
    title: string;
    message: string;
    itemName?: string;
    confirmText?: string;
    onConfirm: () => Promise<void> | void;
  }): Promise<boolean> => {
    return new Promise((resolve) => {
      deleteDialogInfo.value = {
        show: true,
        title: options.title,
        message: options.message,
        itemName: options.itemName,
        confirmText: options.confirmText || 'Delete',
        isDeleting: false,
        onConfirm: async () => {
          try {
            deleteDialogInfo.value.isDeleting = true;
            await options.onConfirm();
            hideDeleteDialog();
            resolve(true);
          } catch (error) {
            deleteDialogInfo.value.isDeleting = false;
            resolve(false);
            throw error;
          }
        }
      };
    });
  };

  /**
   * Hides the delete confirmation dialog.
   */
  const hideDeleteDialog = () => {
    deleteDialogInfo.value = {
      show: false,
      title: '',
      message: '',
      itemName: '',
      confirmText: 'Delete',
      isDeleting: false,
      onConfirm: undefined
    };
  };

  /**
   * Shows the prompt replacement confirmation dialog.
   * @param options - The options for the dialog.
   * @returns A promise that resolves to `true` if the replacement is confirmed, and `false` otherwise.
   */
  const showPromptReplaceDialog = (options: {
    currentPrompt: string;
    newPrompt: string;
    onConfirm: () => Promise<void> | void;
    onCancel?: () => void;
  }): Promise<boolean> => {
    return new Promise((resolve) => {
      promptReplaceDialogInfo.value = {
        show: true,
        title: 'Replace Prompt',
        message: 'You have text in the prompt input. Do you want to replace it with the selected timeline prompt?',
        currentPrompt: options.currentPrompt,
        newPrompt: options.newPrompt,
        isProcessing: false,
        onConfirm: async () => {
          try {
            promptReplaceDialogInfo.value.isProcessing = true;
            await options.onConfirm();
            hidePromptReplaceDialog();
            resolve(true);
          } catch (error) {
            promptReplaceDialogInfo.value.isProcessing = false;
            resolve(false);
            throw error;
          }
        },
        onCancel: () => {
          hidePromptReplaceDialog();
          if (options.onCancel) {
            options.onCancel();
          }
          resolve(false);
        }
      };
    });
  };

  /**
   * Hides the prompt replacement confirmation dialog.
   */
  const hidePromptReplaceDialog = () => {
    promptReplaceDialogInfo.value = {
      show: false,
      title: '',
      message: '',
      currentPrompt: '',
      newPrompt: '',
      isProcessing: false,
      onConfirm: undefined,
      onCancel: undefined
    };
  };

  return {
    ThemePreference,
    ColorPreference,
    authInfo,
    deleteDialogInfo,
    promptReplaceDialogInfo,
    setDarkMode,
    isDarkMode,
    setPreferences,
    setAuthInfo,
    clearAuthInfo,
    showDeleteDialog,
    hideDeleteDialog,
    showPromptReplaceDialog,
    hidePromptReplaceDialog
  };
})
